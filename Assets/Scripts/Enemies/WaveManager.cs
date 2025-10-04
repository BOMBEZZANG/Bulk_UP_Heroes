using System.Collections;
using UnityEngine;
using BulkUpHeroes.Core;
using BulkUpHeroes.Utils;

namespace BulkUpHeroes.Enemies
{
    /// <summary>
    /// Manages wave progression, enemy tracking, and wave state machine.
    /// Controls the core gameplay loop of spawning increasingly difficult waves.
    ///
    /// Phase 2: Linear wave progression with basic state machine
    /// Future: Will support special waves, boss waves, endless mode
    /// </summary>
    public class WaveManager : MonoBehaviour
    {
        #region Wave States
        public enum WaveState
        {
            WaitingToStart,
            PreparingWave,
            SpawningEnemies,
            WaveInProgress,
            WaveComplete,
            GameOver
        }
        #endregion

        #region Configuration
        [Header("Wave Settings")]
        [SerializeField] private int _startingWave = 1;
        [SerializeField] private float _wavePrepareDelay = 3f;
        [SerializeField] private float _waveCompleteDelay = 2f;
        [SerializeField] private bool _autoStartFirstWave = true;

        [Header("References")]
        [SerializeField] private EnemySpawner _enemySpawner;
        #endregion

        #region State
        private WaveState _currentState = WaveState.WaitingToStart;
        private int _currentWaveNumber = 0;
        private WaveConfig _currentWaveConfig;

        private int _enemiesAlive = 0;
        private int _enemiesSpawnedThisWave = 0;
        private int _totalEnemiesDefeated = 0;
        #endregion

        #region Properties
        public WaveState CurrentState => _currentState;
        public int CurrentWaveNumber => _currentWaveNumber;
        public int EnemiesAlive => _enemiesAlive;
        public int TotalEnemiesDefeated => _totalEnemiesDefeated;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            InitializeComponents();
        }

        private void OnEnable()
        {
            SubscribeToEvents();
        }

        private void OnDisable()
        {
            UnsubscribeFromEvents();
        }

        private void Start()
        {
            if (_autoStartFirstWave)
            {
                StartCoroutine(StartFirstWaveDelayed());
            }
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Initialize components and references.
        /// </summary>
        private void InitializeComponents()
        {
            // Auto-find spawner if not assigned
            if (_enemySpawner == null)
            {
                _enemySpawner = FindObjectOfType<EnemySpawner>();
                if (_enemySpawner == null)
                {
                    Debug.LogError("[WaveManager] No EnemySpawner found!");
                }
            }

            _currentWaveNumber = _startingWave - 1; // Will be incremented before first wave
        }

        /// <summary>
        /// Start first wave after a delay.
        /// Increased delay ensures all systems (pooling, physics, shaders) are fully initialized.
        /// </summary>
        private IEnumerator StartFirstWaveDelayed()
        {
            // Wait for systems to initialize
            yield return new WaitForSeconds(0.5f);

            // Ensure player is registered
            if (!Player.PlayerManager.HasPlayer())
            {
                Debug.LogWarning("[WaveManager] Player not ready, waiting...");
                yield return new WaitForSeconds(0.5f);
            }

            StartNextWave();
        }
        #endregion

        #region Event Handling
        /// <summary>
        /// Subscribe to game events.
        /// </summary>
        private void SubscribeToEvents()
        {
            GameEvents.OnEnemyDeath += HandleEnemyDeath;
            GameEvents.OnEnemySpawned += HandleEnemySpawned;
            GameEvents.OnPlayerDeath += HandlePlayerDeath;
            GameEvents.OnGameRestart += HandleGameRestart;
        }

        /// <summary>
        /// Unsubscribe from game events.
        /// </summary>
        private void UnsubscribeFromEvents()
        {
            GameEvents.OnEnemyDeath -= HandleEnemyDeath;
            GameEvents.OnEnemySpawned -= HandleEnemySpawned;
            GameEvents.OnPlayerDeath -= HandlePlayerDeath;
            GameEvents.OnGameRestart -= HandleGameRestart;
        }

        /// <summary>
        /// Handle enemy death event.
        /// </summary>
        private void HandleEnemyDeath(GameObject enemy)
        {
            _enemiesAlive--;
            _totalEnemiesDefeated++;

            Debug.Log($"[WaveManager] Enemy died. Remaining: {_enemiesAlive}, State: {_currentState}");

            // Update enemy count UI
            if (_currentWaveConfig != null)
            {
                GameEvents.TriggerEnemyCountChanged(_enemiesAlive, _currentWaveConfig.enemyCount);
            }

            // Check for wave completion
            if (_currentState == WaveState.WaveInProgress && _enemiesAlive <= 0)
            {
                Debug.Log($"[WaveManager] All enemies defeated! Completing wave...");
                CompleteWave();
            }
            else
            {
                Debug.Log($"[WaveManager] Not completing yet - State: {_currentState}, Enemies: {_enemiesAlive}");
            }
        }

        /// <summary>
        /// Handle enemy spawned event.
        /// </summary>
        private void HandleEnemySpawned(GameObject enemy)
        {
            _enemiesAlive++;
            _enemiesSpawnedThisWave++;

            // Update UI
            if (_currentWaveConfig != null)
            {
                GameEvents.TriggerEnemyCountChanged(_enemiesAlive, _currentWaveConfig.enemyCount);
            }
        }

        /// <summary>
        /// Handle player death event.
        /// </summary>
        private void HandlePlayerDeath()
        {
            ChangeState(WaveState.GameOver);
            StopAllCoroutines();
        }

        /// <summary>
        /// Handle game restart event.
        /// </summary>
        private void HandleGameRestart()
        {
            ResetWaveManager();
        }
        #endregion

        #region State Machine
        /// <summary>
        /// Change wave state.
        /// </summary>
        private void ChangeState(WaveState newState)
        {
            if (_currentState == newState) return;

            Debug.Log($"[WaveManager] State change: {_currentState} -> {newState}");

            _currentState = newState;

            OnStateEnter(newState);
        }

        /// <summary>
        /// Called when entering a new state.
        /// </summary>
        private void OnStateEnter(WaveState state)
        {
            switch (state)
            {
                case WaveState.WaitingToStart:
                    // Do nothing, waiting for manual start
                    break;

                case WaveState.PreparingWave:
                    PrepareNextWave();
                    break;

                case WaveState.SpawningEnemies:
                    SpawnCurrentWave();
                    break;

                case WaveState.WaveInProgress:
                    // Waiting for all enemies to die
                    break;

                case WaveState.WaveComplete:
                    OnWaveCompleted();
                    break;

                case WaveState.GameOver:
                    OnGameOver();
                    break;
            }
        }
        #endregion

        #region Wave Control
        /// <summary>
        /// Start the next wave.
        /// </summary>
        public void StartNextWave()
        {
            if (_currentState == WaveState.GameOver)
            {
                Debug.LogWarning("[WaveManager] Cannot start wave - game is over");
                return;
            }

            _currentWaveNumber++;
            ChangeState(WaveState.PreparingWave);
        }

        /// <summary>
        /// Prepare the next wave.
        /// </summary>
        private void PrepareNextWave()
        {
            // Generate wave configuration
            _currentWaveConfig = WaveConfig.GenerateForWave(_currentWaveNumber);
            _enemiesSpawnedThisWave = 0;

            Debug.Log($"[WaveManager] Preparing {_currentWaveConfig}");

            // Trigger wave start event
            GameEvents.TriggerWaveStart(_currentWaveNumber);

            // Notify GameManager
            if (GameManager.Instance != null)
            {
                GameManager.Instance.IncrementWave();
            }

            // Wait before spawning
            StartCoroutine(WaitThenSpawn());
        }

        /// <summary>
        /// Wait for delay then start spawning.
        /// </summary>
        private IEnumerator WaitThenSpawn()
        {
            // For wave 1, minimal delay. For others, standard delay
            float delay = _currentWaveNumber == 1 ? 0.5f : _wavePrepareDelay;
            yield return new WaitForSeconds(delay);

            ChangeState(WaveState.SpawningEnemies);
        }

        /// <summary>
        /// Spawn enemies for current wave.
        /// </summary>
        private void SpawnCurrentWave()
        {
            if (_enemySpawner == null)
            {
                Debug.LogError("[WaveManager] Cannot spawn - no spawner assigned!");
                return;
            }

            Debug.Log($"[WaveManager] Spawning wave {_currentWaveNumber}");

            _enemySpawner.SpawnWave(_currentWaveConfig, OnWaveSpawnComplete);
        }

        /// <summary>
        /// Called when all enemies for wave are spawned.
        /// </summary>
        private void OnWaveSpawnComplete()
        {
            Debug.Log($"[WaveManager] Wave {_currentWaveNumber} spawn complete. Enemies: {_enemiesAlive}");

            ChangeState(WaveState.WaveInProgress);
        }

        /// <summary>
        /// Complete the current wave.
        /// </summary>
        private void CompleteWave()
        {
            Debug.Log($"[WaveManager] Wave {_currentWaveNumber} complete!");

            ChangeState(WaveState.WaveComplete);
        }

        /// <summary>
        /// Called when wave is completed.
        /// </summary>
        private void OnWaveCompleted()
        {
            // Trigger wave complete event
            GameEvents.TriggerWaveComplete(_currentWaveNumber);

            // Start next wave after delay
            StartCoroutine(PrepareNextWaveDelayed());
        }

        /// <summary>
        /// Wait then prepare next wave.
        /// </summary>
        private IEnumerator PrepareNextWaveDelayed()
        {
            yield return new WaitForSeconds(_waveCompleteDelay);

            StartNextWave();
        }

        /// <summary>
        /// Called when game is over.
        /// </summary>
        private void OnGameOver()
        {
            Debug.Log($"[WaveManager] Game Over at wave {_currentWaveNumber}");

            // Stop spawning
            if (_enemySpawner != null)
            {
                _enemySpawner.StopSpawning();
            }
        }
        #endregion

        #region Reset
        /// <summary>
        /// Reset wave manager to initial state.
        /// </summary>
        public void ResetWaveManager()
        {
            StopAllCoroutines();

            _currentWaveNumber = _startingWave - 1;
            _enemiesAlive = 0;
            _enemiesSpawnedThisWave = 0;
            _totalEnemiesDefeated = 0;
            _currentWaveConfig = null;

            ChangeState(WaveState.WaitingToStart);

            Debug.Log("[WaveManager] Reset complete");

            if (_autoStartFirstWave)
            {
                StartCoroutine(StartFirstWaveDelayed());
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Get current wave configuration.
        /// </summary>
        public WaveConfig GetCurrentWaveConfig()
        {
            return _currentWaveConfig;
        }

        /// <summary>
        /// Manually trigger next wave (for testing).
        /// </summary>
        public void ForceNextWave()
        {
            if (_currentState == WaveState.WaveInProgress)
            {
                // Kill all remaining enemies
                EnemyAI[] enemies = FindObjectsOfType<EnemyAI>();
                foreach (EnemyAI enemy in enemies)
                {
                    if (enemy.gameObject.activeInHierarchy)
                    {
                        GameEvents.TriggerEnemyDeath(enemy.gameObject);
                        PoolManager.Instance?.ReturnEnemy(enemy);
                    }
                }
            }

            StartNextWave();
        }
        #endregion

        #region Debug Helpers
#if UNITY_EDITOR
        [ContextMenu("Start Next Wave")]
        private void DebugStartNextWave()
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning("[WaveManager] Enter Play Mode first!");
                return;
            }

            ForceNextWave();
        }

        [ContextMenu("Skip to Wave 5")]
        private void DebugSkipToWave5()
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning("[WaveManager] Enter Play Mode first!");
                return;
            }

            _currentWaveNumber = 4;
            StartNextWave();
        }

        [ContextMenu("Log Wave Stats")]
        private void DebugLogWaveStats()
        {
            Debug.Log($"[WaveManager] Current Wave: {_currentWaveNumber}, State: {_currentState}");
            Debug.Log($"[WaveManager] Enemies Alive: {_enemiesAlive}, Total Defeated: {_totalEnemiesDefeated}");
            if (_currentWaveConfig != null)
            {
                Debug.Log($"[WaveManager] Config: {_currentWaveConfig}");
            }
        }
#endif
        #endregion
    }
}
