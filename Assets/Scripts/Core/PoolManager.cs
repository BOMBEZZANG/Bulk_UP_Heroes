using System.Collections;
using UnityEngine;
using BulkUpHeroes.Utils;

namespace BulkUpHeroes.Core
{
    /// <summary>
    /// Centralized pool manager for easy access to different object pools.
    /// Use this MonoBehaviour to manage multiple pools in your scene.
    ///
    /// Phase 1: Enemy pooling
    /// Future: Part pickups, particles, damage numbers
    /// </summary>
    public class PoolManager : MonoBehaviour
    {
        #region Singleton
        private static PoolManager _instance;
        public static PoolManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<PoolManager>();
                    if (_instance == null)
                    {
                        GameObject go = new GameObject("PoolManager");
                        _instance = go.AddComponent<PoolManager>();
                    }
                }
                return _instance;
            }
        }
        #endregion

        #region Pool Definitions
        [Header("Enemy Pool")]
        [SerializeField] private GameObject _enemyPrefab;
        [SerializeField] private int _enemyPoolSize = 20;
        [SerializeField] private Transform _enemyPoolParent;

        [Header("Pickup Pool (Future)")]
        [SerializeField] private GameObject _pickupPrefab;
        [SerializeField] private int _pickupPoolSize = 10;
        [SerializeField] private Transform _pickupPoolParent;

        // Pool instances
        private ObjectPool<Enemies.EnemyAI> _enemyPool;
        // Future: ObjectPool for pickups
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            // Singleton enforcement
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);

            InitializePools();
        }

        private void Start()
        {
            // Initialization complete
        }

        private void OnDestroy()
        {
            CleanupPools();
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Initialize all object pools.
        /// </summary>
        private void InitializePools()
        {
            // Create enemy pool parent if not assigned
            if (_enemyPoolParent == null)
            {
                GameObject enemyPoolGO = new GameObject("EnemyPool");
                enemyPoolGO.transform.SetParent(transform);
                _enemyPoolParent = enemyPoolGO.transform;
            }

            // Initialize enemy pool
            if (_enemyPrefab != null)
            {
                _enemyPool = new ObjectPool<Enemies.EnemyAI>(
                    _enemyPrefab,
                    _enemyPoolSize,
                    _enemyPoolParent,
                    allowGrowth: true
                );
            }
            else
            {
                Debug.LogWarning("[PoolManager] Enemy prefab not assigned!");
            }

            // Future: Initialize pickup pool

            Debug.Log("[PoolManager] All pools initialized - NO WARMUP");
        }

        /// <summary>
        /// Pre-warm physics system to avoid first-frame lag.
        /// </summary>
        private void PrewarmPhysics()
        {
            // Trigger physics initialization with dummy queries using cached LayerMasks
            Physics.OverlapSphere(Vector3.zero, 1f, GameConstants.EnemyLayerMask);
            Physics.OverlapSphere(Vector3.zero, 1f, GameConstants.PlayerLayerMask);

            Debug.Log("[PoolManager] Physics system pre-warmed");
        }

        /// <summary>
        /// Cleanup all pools on destroy.
        /// </summary>
        private void CleanupPools()
        {
            try
            {
                _enemyPool?.Clear();
                // Future: Clear other pools
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"[PoolManager] Error during cleanup: {e.Message}");
            }
        }
        #endregion

        #region Enemy Pool Access
        /// <summary>
        /// Spawn an enemy from the pool.
        /// </summary>
        /// <param name="position">Spawn position</param>
        /// <param name="rotation">Spawn rotation</param>
        /// <returns>Enemy AI component</returns>
        public Enemies.EnemyAI SpawnEnemy(Vector3 position, Quaternion rotation)
        {
            if (_enemyPool == null)
            {
                Debug.LogError("[PoolManager] Enemy pool not initialized!");
                return null;
            }

            return _enemyPool.Get(position, rotation);
        }

        /// <summary>
        /// Return an enemy to the pool.
        /// </summary>
        public void ReturnEnemy(Enemies.EnemyAI enemy)
        {
            if (_enemyPool == null) return;
            _enemyPool.Return(enemy);
        }

        /// <summary>
        /// Return an enemy to the pool by GameObject.
        /// </summary>
        public void ReturnEnemy(GameObject enemyObject)
        {
            if (_enemyPool == null) return;
            _enemyPool.Return(enemyObject);
        }

        /// <summary>
        /// Get enemy pool statistics.
        /// </summary>
        public (int available, int active, int total) GetEnemyPoolStats()
        {
            if (_enemyPool == null)
                return (0, 0, 0);

            return (_enemyPool.AvailableCount, _enemyPool.ActiveCount, _enemyPool.TotalCount);
        }
        #endregion

        #region Future: Pickup Pool Access
        // Will be implemented in Phase 3
        #endregion

        #region Debug Helpers
#if UNITY_EDITOR
        [ContextMenu("Log Pool Statistics")]
        private void LogPoolStatistics()
        {
            if (_enemyPool != null)
            {
                Debug.Log($"[PoolManager] Enemy Pool - Available: {_enemyPool.AvailableCount}, " +
                         $"Active: {_enemyPool.ActiveCount}, Total: {_enemyPool.TotalCount}");
            }
        }

        [ContextMenu("Spawn Test Enemy")]
        private void SpawnTestEnemy()
        {
            SpawnEnemy(Vector3.zero, Quaternion.identity);
        }
#endif
        #endregion
    }
}
