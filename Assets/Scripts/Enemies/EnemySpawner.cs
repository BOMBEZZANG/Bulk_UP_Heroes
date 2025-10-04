using System.Collections;
using UnityEngine;
using BulkUpHeroes.Core;
using BulkUpHeroes.Utils;

namespace BulkUpHeroes.Enemies
{
    /// <summary>
    /// Handles enemy spawning from object pool using spawn points.
    /// Works with WaveManager to spawn waves of enemies progressively.
    ///
    /// Phase 2: Sequential spawning with delays
    /// Future: Will support spawn effects, special formations, spawn patterns
    /// </summary>
    public class EnemySpawner : MonoBehaviour
    {
        #region Spawn Configuration
        [Header("Spawn Points")]
        [SerializeField] private Transform[] _spawnPoints;
        [SerializeField] private bool _autoFindSpawnPoints = true;

        [Header("Spawn Settings")]
        [SerializeField] private float _spawnEffectDuration = 0.3f;
        [SerializeField] private bool _rotateSpawnPoints = true;

        private int _currentSpawnPointIndex = 0;
        #endregion

        #region State
        private bool _isSpawning = false;
        private WaveConfig _currentWaveConfig;
        private int _enemiesSpawnedThisWave = 0;
        #endregion

        #region Properties
        public bool IsSpawning => _isSpawning;
        public int SpawnPointCount => _spawnPoints != null ? _spawnPoints.Length : 0;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            if (_autoFindSpawnPoints)
            {
                FindSpawnPoints();
            }
        }

        private void Start()
        {
            ValidateSetup();
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Auto-find spawn points tagged as SpawnPoint.
        /// </summary>
        private void FindSpawnPoints()
        {
            GameObject[] spawnPointObjects = GameObject.FindGameObjectsWithTag(GameConstants.TAG_SPAWN_POINT);

            if (spawnPointObjects.Length == 0)
            {
                Debug.LogWarning("[EnemySpawner] No spawn points found with tag 'SpawnPoint'. Creating defaults.");
                CreateDefaultSpawnPoints();
            }
            else
            {
                _spawnPoints = new Transform[spawnPointObjects.Length];
                for (int i = 0; i < spawnPointObjects.Length; i++)
                {
                    _spawnPoints[i] = spawnPointObjects[i].transform;
                }
                Debug.Log($"[EnemySpawner] Found {_spawnPoints.Length} spawn points");
            }
        }

        /// <summary>
        /// Create default spawn points at arena corners.
        /// </summary>
        private void CreateDefaultSpawnPoints()
        {
            _spawnPoints = new Transform[4];
            Vector3[] positions = new Vector3[]
            {
                new Vector3(4, GameConstants.ENEMY_SPAWN_HEIGHT, 4),   // NE
                new Vector3(-4, GameConstants.ENEMY_SPAWN_HEIGHT, 4),  // NW
                new Vector3(4, GameConstants.ENEMY_SPAWN_HEIGHT, -4),  // SE
                new Vector3(-4, GameConstants.ENEMY_SPAWN_HEIGHT, -4)  // SW
            };

            for (int i = 0; i < 4; i++)
            {
                GameObject spawnPoint = new GameObject($"SpawnPoint_{i}");
                spawnPoint.transform.position = positions[i];
                spawnPoint.transform.SetParent(transform);
                spawnPoint.tag = GameConstants.TAG_SPAWN_POINT;
                _spawnPoints[i] = spawnPoint.transform;
            }

            Debug.Log("[EnemySpawner] Created 4 default spawn points");
        }

        /// <summary>
        /// Validate spawner setup.
        /// </summary>
        private void ValidateSetup()
        {
            if (_spawnPoints == null || _spawnPoints.Length == 0)
            {
                Debug.LogError("[EnemySpawner] No spawn points configured!");
                return;
            }

            if (PoolManager.Instance == null)
            {
                Debug.LogError("[EnemySpawner] PoolManager not found!");
            }
        }
        #endregion

        #region Wave Spawning
        /// <summary>
        /// Spawn a wave of enemies based on configuration.
        /// </summary>
        /// <param name="waveConfig">Configuration for this wave</param>
        /// <param name="onComplete">Callback when all enemies spawned</param>
        public void SpawnWave(WaveConfig waveConfig, System.Action onComplete = null)
        {
            if (_isSpawning)
            {
                Debug.LogWarning("[EnemySpawner] Already spawning a wave!");
                return;
            }

            if (waveConfig == null)
            {
                Debug.LogError("[EnemySpawner] Wave config is null!");
                return;
            }

            _currentWaveConfig = waveConfig;
            _enemiesSpawnedThisWave = 0;

            Debug.Log($"[EnemySpawner] Starting wave spawn: {waveConfig}");

            StartCoroutine(SpawnWaveCoroutine(onComplete));
        }

        /// <summary>
        /// Coroutine to spawn enemies with delays.
        /// </summary>
        private IEnumerator SpawnWaveCoroutine(System.Action onComplete)
        {
            _isSpawning = true;

            int enemiesToSpawn = _currentWaveConfig.enemyCount;
            float spawnDelay = _currentWaveConfig.spawnDelay;

            for (int i = 0; i < enemiesToSpawn; i++)
            {
                // Spawn enemy
                SpawnSingleEnemy(_currentWaveConfig);

                _enemiesSpawnedThisWave++;

                // Wait before spawning next
                if (i < enemiesToSpawn - 1)
                {
                    yield return new WaitForSeconds(spawnDelay);
                }
            }

            _isSpawning = false;

            Debug.Log($"[EnemySpawner] Wave spawn complete: {_enemiesSpawnedThisWave} enemies spawned");

            // Notify completion
            onComplete?.Invoke();
        }

        /// <summary>
        /// Spawn a single enemy at the next spawn point.
        /// </summary>
        private void SpawnSingleEnemy(WaveConfig config)
        {
            if (PoolManager.Instance == null)
            {
                Debug.LogError("[EnemySpawner] PoolManager not available!");
                return;
            }

            // Get spawn position
            Vector3 spawnPosition = GetNextSpawnPoint();

            // Spawn enemy from pool
            EnemyAI enemy = PoolManager.Instance.SpawnEnemy(spawnPosition, Quaternion.identity);

            if (enemy != null)
            {
                // Apply wave-specific stats
                ApplyWaveStats(enemy, config);

                // Trigger spawn event
                GameEvents.TriggerEnemySpawned(enemy.gameObject);

                Debug.Log($"[EnemySpawner] Spawned enemy at {spawnPosition} with stats from wave {config.waveNumber}");
            }
            else
            {
                Debug.LogError("[EnemySpawner] Failed to spawn enemy from pool!");
            }
        }

        /// <summary>
        /// Apply wave-specific stats to spawned enemy.
        /// </summary>
        private void ApplyWaveStats(EnemyAI enemy, WaveConfig config)
        {
            EnemyStats stats = enemy.GetComponent<EnemyStats>();
            if (stats != null)
            {
                // Initialize with exact wave stats
                stats.InitializeWithStats(
                    EnemyType.BasicMelee,
                    config.enemyHealth,
                    config.enemyDamage,
                    config.enemySpeed
                );

                Debug.Log($"[EnemySpawner] Applied wave stats - HP: {config.enemyHealth}, DMG: {config.enemyDamage}, SPD: {config.enemySpeed}");
            }
        }
        #endregion

        #region Spawn Point Management
        /// <summary>
        /// Get the next spawn point position.
        /// </summary>
        private Vector3 GetNextSpawnPoint()
        {
            if (_spawnPoints == null || _spawnPoints.Length == 0)
            {
                Debug.LogWarning("[EnemySpawner] No spawn points available, using default position");
                return new Vector3(0, GameConstants.ENEMY_SPAWN_HEIGHT, 5);
            }

            Vector3 position = _spawnPoints[_currentSpawnPointIndex].position;

            // Rotate to next spawn point
            if (_rotateSpawnPoints)
            {
                _currentSpawnPointIndex = (_currentSpawnPointIndex + 1) % _spawnPoints.Length;
            }

            return position;
        }

        /// <summary>
        /// Reset spawn point rotation.
        /// </summary>
        public void ResetSpawnPointRotation()
        {
            _currentSpawnPointIndex = 0;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Stop current wave spawn.
        /// </summary>
        public void StopSpawning()
        {
            if (_isSpawning)
            {
                StopAllCoroutines();
                _isSpawning = false;
                Debug.Log("[EnemySpawner] Spawning stopped");
            }
        }

        /// <summary>
        /// Get number of enemies spawned in current wave.
        /// </summary>
        public int GetEnemiesSpawnedThisWave()
        {
            return _enemiesSpawnedThisWave;
        }
        #endregion

        #region Debug Helpers
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            // Visualize spawn points
            if (_spawnPoints != null)
            {
                Gizmos.color = Color.cyan;
                for (int i = 0; i < _spawnPoints.Length; i++)
                {
                    if (_spawnPoints[i] != null)
                    {
                        Vector3 pos = _spawnPoints[i].position;
                        Gizmos.DrawWireSphere(pos, 0.5f);
                        Gizmos.DrawLine(pos, pos + Vector3.up * 2f);

                        // Draw spawn point number
                        UnityEditor.Handles.Label(pos + Vector3.up * 2.5f, $"SP {i}");
                    }
                }

                // Highlight next spawn point
                if (Application.isPlaying && _spawnPoints.Length > 0)
                {
                    Gizmos.color = Color.red;
                    Vector3 nextPos = _spawnPoints[_currentSpawnPointIndex].position;
                    Gizmos.DrawWireSphere(nextPos, 0.7f);
                }
            }
        }

        [ContextMenu("Test Spawn Single Enemy")]
        private void DebugSpawnSingle()
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning("[EnemySpawner] Enter Play Mode first!");
                return;
            }

            WaveConfig testConfig = WaveConfig.GenerateForWave(1);
            SpawnSingleEnemy(testConfig);
        }
#endif
        #endregion
    }
}
