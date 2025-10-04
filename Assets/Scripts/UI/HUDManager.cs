using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BulkUpHeroes.Core;

namespace BulkUpHeroes.UI
{
    /// <summary>
    /// Main HUD controller that manages all UI elements during gameplay.
    /// Coordinates health bar, wave counter, enemy counter, and announcements.
    ///
    /// Phase 2: Basic HUD with health, wave, and enemy count
    /// Future: Will support score, combo system, power-up indicators
    /// </summary>
    public class HUDManager : MonoBehaviour
    {
        #region HUD Components
        [Header("HUD Panels")]
        [SerializeField] private GameObject _hudPanel;
        [SerializeField] private HealthBar _healthBar;
        [SerializeField] private AnnouncementSystem _announcementSystem;

        [Header("Wave Display")]
        [SerializeField] private TextMeshProUGUI _waveText;
        [SerializeField] private string _waveTextFormat = "Wave: {0}";

        [Header("Enemy Counter")]
        [SerializeField] private TextMeshProUGUI _enemyCountText;
        [SerializeField] private string _enemyCountFormat = "Enemies: {0}";
        [SerializeField] private bool _showRemainingTotal = true;
        [SerializeField] private string _enemyCountDetailFormat = "Enemies: {0}/{1}";

        [Header("Stats Display")]
        [SerializeField] private TextMeshProUGUI _killCountText;
        [SerializeField] private string _killCountFormat = "Kills: {0}";
        #endregion

        #region State
        private int _currentWave = 0;
        private int _enemiesRemaining = 0;
        private int _totalEnemiesThisWave = 0;
        private int _totalKills = 0;
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
            InitializeHUD();
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Initialize HUD components.
        /// </summary>
        private void InitializeComponents()
        {
            // Auto-find components if not assigned
            if (_healthBar == null)
            {
                _healthBar = GetComponentInChildren<HealthBar>();
            }

            if (_announcementSystem == null)
            {
                _announcementSystem = GetComponentInChildren<AnnouncementSystem>();
            }

            ValidateComponents();
        }

        /// <summary>
        /// Validate required components.
        /// </summary>
        private void ValidateComponents()
        {
            if (_hudPanel == null)
            {
                Debug.LogWarning("[HUDManager] HUD Panel not assigned!");
            }

            if (_healthBar == null)
            {
                Debug.LogWarning("[HUDManager] HealthBar not found!");
            }

            if (_waveText == null)
            {
                Debug.LogWarning("[HUDManager] Wave Text not assigned!");
            }
        }

        /// <summary>
        /// Subscribe to game events.
        /// </summary>
        private void SubscribeToEvents()
        {
            // Wave events
            GameEvents.OnWaveStart += HandleWaveStart;
            GameEvents.OnWaveComplete += HandleWaveComplete;

            // Enemy events
            GameEvents.OnEnemyCountChanged += HandleEnemyCountChanged;
            GameEvents.OnEnemyDeath += HandleEnemyDeath;

            // Game state events
            GameEvents.OnGameStart += HandleGameStart;
            GameEvents.OnGameOver += HandleGameOver;
            GameEvents.OnGameRestart += HandleGameRestart;
        }

        /// <summary>
        /// Unsubscribe from game events.
        /// </summary>
        private void UnsubscribeFromEvents()
        {
            GameEvents.OnWaveStart -= HandleWaveStart;
            GameEvents.OnWaveComplete -= HandleWaveComplete;
            GameEvents.OnEnemyCountChanged -= HandleEnemyCountChanged;
            GameEvents.OnEnemyDeath -= HandleEnemyDeath;
            GameEvents.OnGameStart -= HandleGameStart;
            GameEvents.OnGameOver -= HandleGameOver;
            GameEvents.OnGameRestart -= HandleGameRestart;
        }

        /// <summary>
        /// Initialize HUD with starting values.
        /// </summary>
        private void InitializeHUD()
        {
            UpdateWaveDisplay(1);
            UpdateEnemyCountDisplay(0, 0);
            UpdateKillCount(0);

            if (_hudPanel != null)
            {
                _hudPanel.SetActive(true);
            }
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Handle wave start event.
        /// </summary>
        private void HandleWaveStart(int waveNumber)
        {
            _currentWave = waveNumber;
            UpdateWaveDisplay(waveNumber);

            Debug.Log($"[HUDManager] Wave {waveNumber} started");
        }

        /// <summary>
        /// Handle wave complete event.
        /// </summary>
        private void HandleWaveComplete(int waveNumber)
        {
            Debug.Log($"[HUDManager] Wave {waveNumber} completed");
        }

        /// <summary>
        /// Handle enemy count changed event.
        /// </summary>
        private void HandleEnemyCountChanged(int remaining, int total)
        {
            _enemiesRemaining = remaining;
            _totalEnemiesThisWave = total;
            UpdateEnemyCountDisplay(remaining, total);
        }

        /// <summary>
        /// Handle enemy death event.
        /// </summary>
        private void HandleEnemyDeath(GameObject enemy)
        {
            _totalKills++;
            UpdateKillCount(_totalKills);
        }

        /// <summary>
        /// Handle game start event.
        /// </summary>
        private void HandleGameStart()
        {
            ShowHUD();
            Debug.Log("[HUDManager] Game started - HUD shown");
        }

        /// <summary>
        /// Handle game over event.
        /// </summary>
        private void HandleGameOver()
        {
            // Keep HUD visible but game over screen will overlay
            Debug.Log("[HUDManager] Game over");
        }

        /// <summary>
        /// Handle game restart event.
        /// </summary>
        private void HandleGameRestart()
        {
            _totalKills = 0;
            InitializeHUD();
            ShowHUD();
            Debug.Log("[HUDManager] Game restarted - HUD reset");
        }
        #endregion

        #region HUD Updates
        /// <summary>
        /// Update wave number display.
        /// </summary>
        private void UpdateWaveDisplay(int waveNumber)
        {
            if (_waveText != null)
            {
                _waveText.text = string.Format(_waveTextFormat, waveNumber);
            }
        }

        /// <summary>
        /// Update enemy count display.
        /// </summary>
        private void UpdateEnemyCountDisplay(int remaining, int total)
        {
            if (_enemyCountText != null)
            {
                if (_showRemainingTotal)
                {
                    _enemyCountText.text = string.Format(_enemyCountDetailFormat, remaining, total);
                }
                else
                {
                    _enemyCountText.text = string.Format(_enemyCountFormat, remaining);
                }
            }
        }

        /// <summary>
        /// Update kill count display.
        /// </summary>
        private void UpdateKillCount(int kills)
        {
            if (_killCountText != null)
            {
                _killCountText.text = string.Format(_killCountFormat, kills);
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Show HUD.
        /// </summary>
        public void ShowHUD()
        {
            if (_hudPanel != null)
            {
                _hudPanel.SetActive(true);
            }
        }

        /// <summary>
        /// Hide HUD.
        /// </summary>
        public void HideHUD()
        {
            if (_hudPanel != null)
            {
                _hudPanel.SetActive(false);
            }
        }

        /// <summary>
        /// Get current wave number.
        /// </summary>
        public int GetCurrentWave()
        {
            return _currentWave;
        }

        /// <summary>
        /// Get total kills.
        /// </summary>
        public int GetTotalKills()
        {
            return _totalKills;
        }

        /// <summary>
        /// Get health bar component.
        /// </summary>
        public HealthBar GetHealthBar()
        {
            return _healthBar;
        }

        /// <summary>
        /// Get announcement system.
        /// </summary>
        public AnnouncementSystem GetAnnouncementSystem()
        {
            return _announcementSystem;
        }
        #endregion

        #region Debug Helpers
#if UNITY_EDITOR
        [ContextMenu("Test Wave Update")]
        private void TestWaveUpdate()
        {
            UpdateWaveDisplay(_currentWave + 1);
        }

        [ContextMenu("Test Enemy Count Update")]
        private void TestEnemyCountUpdate()
        {
            UpdateEnemyCountDisplay(5, 10);
        }
#endif
        #endregion
    }
}
