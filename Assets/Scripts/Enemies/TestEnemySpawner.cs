using UnityEngine;
using BulkUpHeroes.Core;
using BulkUpHeroes.Utils;

namespace BulkUpHeroes.Enemies
{
    /// <summary>
    /// Simple test spawner for Phase 1 development.
    /// Spawns a few enemies at the corners for testing player movement and combat.
    ///
    /// Phase 2 will replace this with proper WaveManager.
    /// </summary>
    public class TestEnemySpawner : MonoBehaviour
    {
        [Header("Test Spawn Settings")]
        [SerializeField] private int _numberOfEnemies = 2;
        [SerializeField] private float _spawnDelay = 1f;
        [SerializeField] private bool _spawnOnStart = true;

        [Header("Spawn Positions (Auto-detected)")]
        [SerializeField] private Transform[] _spawnPoints;

        private float _spawnTimer = 0f;
        private int _enemiesSpawned = 0;
        private bool _hasSpawned = false;

        #region Unity Lifecycle
        private void Start()
        {
            // Auto-find spawn points if not assigned
            if (_spawnPoints == null || _spawnPoints.Length == 0)
            {
                FindSpawnPoints();
            }

            if (_spawnOnStart)
            {
                _spawnTimer = _spawnDelay;
            }
        }

        private void Update()
        {
            // Spawn enemies with delay
            if (!_hasSpawned && _spawnOnStart)
            {
                _spawnTimer -= Time.deltaTime;

                if (_spawnTimer <= 0)
                {
                    SpawnTestEnemies();
                    _hasSpawned = true;
                }
            }
        }
        #endregion

        #region Spawn Points
        /// <summary>
        /// Auto-find spawn points tagged as SpawnPoint.
        /// </summary>
        private void FindSpawnPoints()
        {
            GameObject[] spawnPointObjects = GameObject.FindGameObjectsWithTag(GameConstants.TAG_SPAWN_POINT);

            if (spawnPointObjects.Length == 0)
            {
                Debug.LogWarning("[TestEnemySpawner] No spawn points found with tag 'SpawnPoint'");
                CreateDefaultSpawnPoints();
            }
            else
            {
                _spawnPoints = new Transform[spawnPointObjects.Length];
                for (int i = 0; i < spawnPointObjects.Length; i++)
                {
                    _spawnPoints[i] = spawnPointObjects[i].transform;
                }
                Debug.Log($"[TestEnemySpawner] Found {_spawnPoints.Length} spawn points");
            }
        }

        /// <summary>
        /// Create default spawn points if none exist.
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
                _spawnPoints[i] = spawnPoint.transform;
            }

            Debug.Log("[TestEnemySpawner] Created 4 default spawn points");
        }
        #endregion

        #region Spawning
        /// <summary>
        /// Spawn test enemies for Phase 1 testing.
        /// </summary>
        public void SpawnTestEnemies()
        {
            if (PoolManager.Instance == null)
            {
                Debug.LogError("[TestEnemySpawner] PoolManager not found!");
                return;
            }

            if (_spawnPoints == null || _spawnPoints.Length == 0)
            {
                Debug.LogError("[TestEnemySpawner] No spawn points available!");
                return;
            }

            Debug.Log($"[TestEnemySpawner] Spawning {_numberOfEnemies} test enemies...");

            for (int i = 0; i < _numberOfEnemies; i++)
            {
                // Pick a random spawn point
                Transform spawnPoint = _spawnPoints[i % _spawnPoints.Length];

                // Spawn enemy from pool
                EnemyAI enemy = PoolManager.Instance.SpawnEnemy(
                    spawnPoint.position,
                    Quaternion.identity
                );

                if (enemy != null)
                {
                    _enemiesSpawned++;
                    Debug.Log($"[TestEnemySpawner] Spawned enemy {i + 1} at {spawnPoint.position}");
                }
                else
                {
                    Debug.LogError($"[TestEnemySpawner] Failed to spawn enemy {i + 1}");
                }
            }

            Debug.Log($"[TestEnemySpawner] Total enemies spawned: {_enemiesSpawned}");
        }

        /// <summary>
        /// Spawn a single enemy at a specific position.
        /// </summary>
        /// <param name="position">World position to spawn at</param>
        public void SpawnEnemyAt(Vector3 position)
        {
            if (PoolManager.Instance == null)
            {
                Debug.LogError("[TestEnemySpawner] PoolManager not found!");
                return;
            }

            EnemyAI enemy = PoolManager.Instance.SpawnEnemy(position, Quaternion.identity);

            if (enemy != null)
            {
                _enemiesSpawned++;
                Debug.Log($"[TestEnemySpawner] Spawned enemy at {position}");
            }
        }

        /// <summary>
        /// Clear all spawned enemies.
        /// </summary>
        public void ClearAllEnemies()
        {
            EnemyAI[] enemies = FindObjectsOfType<EnemyAI>();

            foreach (EnemyAI enemy in enemies)
            {
                if (PoolManager.Instance != null)
                {
                    PoolManager.Instance.ReturnEnemy(enemy);
                }
                else
                {
                    Destroy(enemy.gameObject);
                }
            }

            _enemiesSpawned = 0;
            Debug.Log("[TestEnemySpawner] Cleared all enemies");
        }
        #endregion

        #region Debug Helpers
#if UNITY_EDITOR
        [ContextMenu("Spawn Test Enemies Now")]
        private void DebugSpawnNow()
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning("[TestEnemySpawner] Enter Play Mode first!");
                return;
            }

            SpawnTestEnemies();
        }

        [ContextMenu("Spawn 1 Enemy at (3, 0.5, 3)")]
        private void DebugSpawnOne()
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning("[TestEnemySpawner] Enter Play Mode first!");
                return;
            }

            SpawnEnemyAt(new Vector3(3, 0.5f, 3));
        }

        [ContextMenu("Clear All Enemies")]
        private void DebugClearAll()
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning("[TestEnemySpawner] Enter Play Mode first!");
                return;
            }

            ClearAllEnemies();
        }

        private void OnDrawGizmos()
        {
            // Visualize spawn points
            if (_spawnPoints != null)
            {
                Gizmos.color = Color.red;
                foreach (Transform spawnPoint in _spawnPoints)
                {
                    if (spawnPoint != null)
                    {
                        Gizmos.DrawWireSphere(spawnPoint.position, 0.5f);
                        Gizmos.DrawLine(spawnPoint.position, spawnPoint.position + Vector3.up * 2f);
                    }
                }
            }
        }
#endif
        #endregion
    }
}
