using System.Collections.Generic;
using UnityEngine;
using BulkUpHeroes.Core;
using BulkUpHeroes.Utils;

namespace BulkUpHeroes.Parts
{
    /// <summary>
    /// Manages part drops from enemies, including drop chances, rarity distribution,
    /// and object pooling for pickup objects.
    ///
    /// Phase 3: Simple drop system with wave-based chances
    /// Future: More complex loot tables, guaranteed drops on bosses
    /// </summary>
    public class PartDropManager : MonoBehaviour
    {
        #region Singleton
        private static PartDropManager _instance;
        public static PartDropManager Instance => _instance;
        #endregion

        #region Configuration
        [Header("Drop Chances by Wave")]
        [SerializeField] private DropRateConfig[] _dropRatesByWave = new DropRateConfig[]
        {
            new DropRateConfig { minWave = 1, maxWave = 2, dropChance = 1.0f, commonWeight = 90, rareWeight = 10, epicWeight = 0 },
            new DropRateConfig { minWave = 3, maxWave = 4, dropChance = 0.8f, commonWeight = 70, rareWeight = 25, epicWeight = 5 },
            new DropRateConfig { minWave = 5, maxWave = 6, dropChance = 0.7f, commonWeight = 60, rareWeight = 30, epicWeight = 10 },
            new DropRateConfig { minWave = 7, maxWave = 999, dropChance = 0.6f, commonWeight = 50, rareWeight = 35, epicWeight = 15 }
        };

        [Header("Part Data")]
        [SerializeField] private PartData[] _availableParts;

        [Header("Pickup Prefab")]
        [SerializeField] private GameObject _pickupPrefab;

        [Header("Pool Settings")]
        [SerializeField] private int _initialPoolSize = 20;
        [SerializeField] private int _maxPartsOnGround = 10;
        [SerializeField] private Transform _poolParent;
        #endregion

        #region State
        private ObjectPool<PartPickup> _pickupPool;
        private List<PartPickup> _activePickups = new List<PartPickup>();
        private int _currentWave = 1;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            SetupSingleton();
            InitializePool();
        }

        private void OnEnable()
        {
            GameEvents.OnEnemyDeath += HandleEnemyDeath;
            GameEvents.OnWaveStart += HandleWaveStart;
        }

        private void OnDisable()
        {
            GameEvents.OnEnemyDeath -= HandleEnemyDeath;
            GameEvents.OnWaveStart -= HandleWaveStart;
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Setup singleton pattern.
        /// </summary>
        private void SetupSingleton()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
        }

        /// <summary>
        /// Initialize pickup object pool.
        /// </summary>
        private void InitializePool()
        {
            if (_pickupPrefab == null)
            {
                Debug.LogError("[PartDropManager] Pickup prefab not assigned!");
                return;
            }

            if (_poolParent == null)
            {
                GameObject poolParentObj = new GameObject("PartPickupPool");
                _poolParent = poolParentObj.transform;
                _poolParent.SetParent(transform);
            }

            _pickupPool = new ObjectPool<PartPickup>(
                _pickupPrefab,
                _initialPoolSize,
                _poolParent,
                allowGrowth: true
            );

            Debug.Log($"[PartDropManager] Initialized pickup pool with {_initialPoolSize} objects");
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Handle enemy death event.
        /// </summary>
        private void HandleEnemyDeath(GameObject enemy)
        {
            Vector3 dropPosition = enemy.transform.position;
            AttemptDrop(dropPosition);
        }

        /// <summary>
        /// Handle wave start event to update current wave.
        /// </summary>
        private void HandleWaveStart(int waveNumber)
        {
            _currentWave = waveNumber;
        }
        #endregion

        #region Drop Logic
        /// <summary>
        /// Attempt to drop a part at the given position.
        /// </summary>
        public void AttemptDrop(Vector3 position)
        {
            // Get drop configuration for current wave
            DropRateConfig config = GetDropConfigForWave(_currentWave);

            // Roll for drop
            if (Random.value > config.dropChance)
            {
                Debug.Log($"[PartDropManager] No drop (chance: {config.dropChance * 100}%)");
                return;
            }

            // Determine rarity
            Rarity rarity = RollRarity(config);

            // Select random part type
            PartType partType = (PartType)Random.Range(0, System.Enum.GetValues(typeof(PartType)).Length);

            // Find matching part data
            PartData partData = FindPartData(partType, rarity);

            if (partData == null)
            {
                Debug.LogWarning($"[PartDropManager] No part data found for {rarity} {partType}");
                return;
            }

            // Spawn the part
            SpawnPart(partData, position);
        }

        /// <summary>
        /// Spawn a part pickup at the specified position.
        /// </summary>
        private void SpawnPart(PartData partData, Vector3 position)
        {
            // Check max parts limit
            if (_activePickups.Count >= _maxPartsOnGround)
            {
                // Remove oldest pickup
                PartPickup oldest = _activePickups[0];
                _activePickups.RemoveAt(0);
                oldest.OnReturnToPool();
            }

            // Get pickup from pool
            PartPickup pickup = _pickupPool.Get(position, Quaternion.identity);

            if (pickup == null)
            {
                Debug.LogError("[PartDropManager] Failed to get pickup from pool!");
                return;
            }

            // Initialize pickup (will set correct position)
            pickup.Initialize(partData, position);
            _activePickups.Add(pickup);

            Debug.Log($"[PartDropManager] Dropped {partData.rarity} {partData.partType} at {position}");
        }
        #endregion

        #region Rarity System
        /// <summary>
        /// Roll for rarity based on wave configuration.
        /// </summary>
        private Rarity RollRarity(DropRateConfig config)
        {
            int totalWeight = config.commonWeight + config.rareWeight + config.epicWeight;
            int roll = Random.Range(0, totalWeight);

            if (roll < config.commonWeight)
                return Rarity.Common;
            else if (roll < config.commonWeight + config.rareWeight)
                return Rarity.Rare;
            else
                return Rarity.Epic;
        }

        /// <summary>
        /// Get drop configuration for current wave.
        /// </summary>
        private DropRateConfig GetDropConfigForWave(int wave)
        {
            foreach (var config in _dropRatesByWave)
            {
                if (wave >= config.minWave && wave <= config.maxWave)
                {
                    return config;
                }
            }

            // Fallback to last config
            return _dropRatesByWave[_dropRatesByWave.Length - 1];
        }

        /// <summary>
        /// Find part data matching type and rarity.
        /// </summary>
        private PartData FindPartData(PartType partType, Rarity rarity)
        {
            foreach (var part in _availableParts)
            {
                if (part.partType == partType && part.rarity == rarity)
                {
                    return part;
                }
            }

            // Fallback: Try to find any part of this type
            foreach (var part in _availableParts)
            {
                if (part.partType == partType)
                {
                    Debug.LogWarning($"[PartDropManager] Exact rarity not found, using fallback");
                    return part;
                }
            }

            return null;
        }
        #endregion

        #region Pool Management
        /// <summary>
        /// Return a pickup to the pool.
        /// </summary>
        public void ReturnPickupToPool(PartPickup pickup)
        {
            _activePickups.Remove(pickup);
            _pickupPool.Return(pickup);
        }
        #endregion

        #region Debug Helpers
#if UNITY_EDITOR
        [ContextMenu("Force Drop At Player")]
        private void DebugDropAtPlayer()
        {
            GameObject player = GameObject.FindGameObjectWithTag(GameConstants.TAG_PLAYER);
            if (player != null)
            {
                AttemptDrop(player.transform.position + Vector3.forward * 2f);
            }
        }

        [ContextMenu("Drop Epic Part")]
        private void DebugDropEpic()
        {
            GameObject player = GameObject.FindGameObjectWithTag(GameConstants.TAG_PLAYER);
            if (player != null && _availableParts.Length > 0)
            {
                // Find first epic part
                PartData epicPart = System.Array.Find(_availableParts, p => p.rarity == Rarity.Epic);
                if (epicPart != null)
                {
                    SpawnPart(epicPart, player.transform.position + Vector3.forward * 2f);
                }
            }
        }
#endif
        #endregion

        #region Helper Classes
        /// <summary>
        /// Configuration for drop rates per wave range.
        /// </summary>
        [System.Serializable]
        public class DropRateConfig
        {
            public int minWave;
            public int maxWave;
            [Range(0f, 1f)] public float dropChance;
            [Range(0, 100)] public int commonWeight;
            [Range(0, 100)] public int rareWeight;
            [Range(0, 100)] public int epicWeight;
        }
        #endregion
    }
}
