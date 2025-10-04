using System.Collections.Generic;
using UnityEngine;
using BulkUpHeroes.Utils;

namespace BulkUpHeroes.Core
{
    /// <summary>
    /// Central game manager that controls game state, scene management, and global game flow.
    /// Implements Singleton pattern for easy access from any script.
    ///
    /// Phase 2: Enhanced with event system integration, wave tracking, and statistics
    /// Future: Will handle progression system, unlockables, and save data
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        #region Singleton
        private static GameManager _instance;
        public static GameManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<GameManager>();
                    if (_instance == null)
                    {
                        GameObject go = new GameObject("GameManager");
                        _instance = go.AddComponent<GameManager>();
                    }
                }
                return _instance;
            }
        }
        #endregion

        #region Events
        // Game state change event for observers
        public delegate void GameStateChanged(GameState newState);
        public event GameStateChanged OnGameStateChanged;
        #endregion

        #region Properties
        [Header("Game State")]
        [SerializeField] private GameState _currentState = GameState.MainMenu;
        public GameState CurrentState => _currentState;

        [Header("Game Statistics (Phase 1)")]
        [SerializeField] private int _currentWave = 0;
        [SerializeField] private int _enemiesDefeated = 0;
        [SerializeField] private float _survivalTime = 0f;

        public int CurrentWave => _currentWave;
        public int EnemiesDefeated => _enemiesDefeated;
        public float SurvivalTime => _survivalTime;

        [Header("References")]
        [SerializeField] private GameObject _playerPrefab;
        [SerializeField] private Transform _playerSpawnPoint;

        private GameObject _currentPlayer;
        private List<IGameStateObserver> _stateObservers = new List<IGameStateObserver>();
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

            InitializeGame();
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
            // Phase 2: Auto-start game for testing
            // Future: Will show main menu first
            StartGame();
        }

        private void Update()
        {
            // Update survival time while playing
            if (_currentState == GameState.Playing)
            {
                _survivalTime += Time.deltaTime;
            }
        }
        #endregion

        #region Event Handling
        /// <summary>
        /// Subscribe to game events.
        /// </summary>
        private void SubscribeToEvents()
        {
            GameEvents.OnPlayerDeath += HandlePlayerDeath;
        }

        /// <summary>
        /// Unsubscribe from game events.
        /// </summary>
        private void UnsubscribeFromEvents()
        {
            GameEvents.OnPlayerDeath -= HandlePlayerDeath;
        }

        /// <summary>
        /// Handle player death event.
        /// </summary>
        private void HandlePlayerDeath()
        {
            Debug.Log("[GameManager] Player died - triggering game over");
            EndGame();
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Initialize game systems and settings.
        /// </summary>
        private void InitializeGame()
        {
            // Set target frame rate for mobile
            Application.targetFrameRate = GameConstants.TARGET_FRAME_RATE;

            // Ensure screen doesn't sleep during gameplay
            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            Debug.Log("[GameManager] Game initialized successfully");
        }
        #endregion

        #region Game State Management
        /// <summary>
        /// Change the current game state and notify observers.
        /// </summary>
        /// <param name="newState">The new game state to transition to</param>
        public void ChangeGameState(GameState newState)
        {
            if (_currentState == newState) return;

            Debug.Log($"[GameManager] State change: {_currentState} -> {newState}");

            GameState previousState = _currentState;
            _currentState = newState;

            // Notify observers
            OnGameStateChanged?.Invoke(newState);
            NotifyStateObservers(newState);

            // Handle state-specific logic
            HandleStateChange(previousState, newState);
        }

        /// <summary>
        /// Handle logic when transitioning between states.
        /// </summary>
        private void HandleStateChange(GameState oldState, GameState newState)
        {
            switch (newState)
            {
                case GameState.MainMenu:
                    Time.timeScale = 1f;
                    break;

                case GameState.Playing:
                    Time.timeScale = 1f;
                    GameEvents.TriggerGameStart();
                    break;

                case GameState.WaveComplete:
                    // Handled by WaveManager
                    break;

                case GameState.GameOver:
                    // Don't pause time - let death animations play
                    GameEvents.TriggerGameOver();
                    break;

                case GameState.Paused:
                    Time.timeScale = 0f;
                    break;
            }
        }

        /// <summary>
        /// Register an observer to receive game state change notifications.
        /// </summary>
        public void RegisterStateObserver(IGameStateObserver observer)
        {
            if (!_stateObservers.Contains(observer))
            {
                _stateObservers.Add(observer);
            }
        }

        /// <summary>
        /// Unregister an observer from game state notifications.
        /// </summary>
        public void UnregisterStateObserver(IGameStateObserver observer)
        {
            _stateObservers.Remove(observer);
        }

        /// <summary>
        /// Notify all registered observers of state change.
        /// </summary>
        private void NotifyStateObservers(GameState newState)
        {
            foreach (var observer in _stateObservers)
            {
                observer?.OnGameStateChanged(newState);
            }
        }
        #endregion

        #region Game Flow Control
        /// <summary>
        /// Start a new game session.
        /// </summary>
        public void StartGame()
        {
            Debug.Log("[GameManager] Starting new game...");

            // Reset statistics
            _currentWave = 0;
            _enemiesDefeated = 0;
            _survivalTime = 0f;

            // Spawn player if not already in scene
            if (_currentPlayer == null)
            {
                SpawnPlayer();
            }

            // Change to playing state
            ChangeGameState(GameState.Playing);
        }

        /// <summary>
        /// End the current game session.
        /// </summary>
        public void EndGame()
        {
            Debug.Log($"[GameManager] Game Over - Wave: {_currentWave}, Enemies Defeated: {_enemiesDefeated}");
            ChangeGameState(GameState.GameOver);
        }

        /// <summary>
        /// Restart the current game.
        /// </summary>
        public void RestartGame()
        {
            Debug.Log("[GameManager] Restarting game...");

            // Trigger restart event (for cleanup)
            GameEvents.TriggerGameRestart();

            // Reset time scale
            Time.timeScale = 1f;

            // Cleanup current game
            if (_currentPlayer != null)
            {
                Destroy(_currentPlayer);
                _currentPlayer = null;
            }

            // Start fresh game
            StartGame();
        }

        /// <summary>
        /// Pause the game.
        /// </summary>
        public void PauseGame()
        {
            if (_currentState == GameState.Playing)
            {
                ChangeGameState(GameState.Paused);
            }
        }

        /// <summary>
        /// Resume the game from pause.
        /// </summary>
        public void ResumeGame()
        {
            if (_currentState == GameState.Paused)
            {
                ChangeGameState(GameState.Playing);
            }
        }
        #endregion

        #region Player Management
        /// <summary>
        /// Spawn the player character at the designated spawn point.
        /// </summary>
        private void SpawnPlayer()
        {
            if (_playerPrefab == null)
            {
                Debug.LogError("[GameManager] Player prefab not assigned!");
                return;
            }

            Vector3 spawnPosition = _playerSpawnPoint != null
                ? _playerSpawnPoint.position
                : new Vector3(0, GameConstants.PLAYER_SPAWN_HEIGHT, 0);

            _currentPlayer = Instantiate(_playerPrefab, spawnPosition, Quaternion.identity);
            _currentPlayer.name = "Player";

            // Register player with PlayerManager for fast access
            Player.PlayerManager.RegisterPlayer(_currentPlayer);

            Debug.Log($"[GameManager] Player spawned at {spawnPosition}");
        }

        /// <summary>
        /// Get reference to the current player GameObject.
        /// </summary>
        public GameObject GetPlayer()
        {
            return _currentPlayer;
        }
        #endregion

        #region Statistics Tracking
        /// <summary>
        /// Increment enemy defeat counter.
        /// Called by enemy AI when defeated.
        /// </summary>
        public void RegisterEnemyDefeated()
        {
            _enemiesDefeated++;
            Debug.Log($"[GameManager] Enemy defeated. Total: {_enemiesDefeated}");
        }

        /// <summary>
        /// Increment wave counter.
        /// Called by wave manager when new wave starts.
        /// </summary>
        public void IncrementWave()
        {
            _currentWave++;
            Debug.Log($"[GameManager] Wave {_currentWave} started");
        }

        /// <summary>
        /// Get current wave number (for UI display).
        /// </summary>
        public int GetCurrentWave()
        {
            return _currentWave;
        }

        /// <summary>
        /// Get total enemies defeated (for UI display).
        /// </summary>
        public int GetTotalKills()
        {
            return _enemiesDefeated;
        }

        /// <summary>
        /// Get survival time (for UI display).
        /// </summary>
        public float GetSurvivalTime()
        {
            return _survivalTime;
        }
        #endregion

        #region Debug Helpers
#if UNITY_EDITOR
        [ContextMenu("Start Game")]
        private void DebugStartGame()
        {
            StartGame();
        }

        [ContextMenu("End Game")]
        private void DebugEndGame()
        {
            EndGame();
        }

        [ContextMenu("Restart Game")]
        private void DebugRestartGame()
        {
            RestartGame();
        }
#endif
        #endregion
    }
}
