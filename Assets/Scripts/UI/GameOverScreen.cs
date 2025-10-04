using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BulkUpHeroes.Core;

namespace BulkUpHeroes.UI
{
    /// <summary>
    /// Game Over screen with statistics display and restart functionality.
    /// Shows final wave, kills, survival time, and provides retry option.
    ///
    /// Phase 2: Basic game over screen with stats
    /// Future: Will support leaderboards, achievements, replay system
    /// </summary>
    public class GameOverScreen : MonoBehaviour
    {
        #region UI Components
        [Header("Game Over Panel")]
        [SerializeField] private GameObject _gameOverPanel;
        [SerializeField] private CanvasGroup _canvasGroup;

        [Header("Text Elements")]
        [SerializeField] private TextMeshProUGUI _gameOverTitle;
        [SerializeField] private TextMeshProUGUI _waveReachedText;
        [SerializeField] private TextMeshProUGUI _killsText;
        [SerializeField] private TextMeshProUGUI _survivalTimeText;

        [Header("Buttons")]
        [SerializeField] private Button _restartButton;
        [SerializeField] private Button _quitButton;

        [Header("Display Settings")]
        [SerializeField] private float _fadeInDuration = 0.5f;
        [SerializeField] private float _delayBeforeShow = 1f;

        [Header("Text Formats")]
        [SerializeField] private string _waveFormat = "Wave Reached: {0}";
        [SerializeField] private string _killsFormat = "Total Kills: {0}";
        [SerializeField] private string _survivalTimeFormat = "Survival Time: {0:F1}s";
        #endregion

        #region State
        private bool _isShowing = false;
        private float _fadeTimer = 0f;
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

        private void Update()
        {
            UpdateFadeAnimation();
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Initialize components and setup buttons.
        /// </summary>
        private void InitializeComponents()
        {
            // Get or add CanvasGroup for fading
            if (_gameOverPanel != null && _canvasGroup == null)
            {
                _canvasGroup = _gameOverPanel.GetComponent<CanvasGroup>();
                if (_canvasGroup == null)
                {
                    _canvasGroup = _gameOverPanel.AddComponent<CanvasGroup>();
                }
            }

            // Setup buttons
            if (_restartButton != null)
            {
                _restartButton.onClick.AddListener(OnRestartClicked);
            }

            if (_quitButton != null)
            {
                _quitButton.onClick.AddListener(OnQuitClicked);
            }

            // Start hidden
            HideImmediate();
        }

        /// <summary>
        /// Subscribe to game events.
        /// </summary>
        private void SubscribeToEvents()
        {
            GameEvents.OnGameOver += HandleGameOver;
            GameEvents.OnGameRestart += HandleGameRestart;
        }

        /// <summary>
        /// Unsubscribe from game events.
        /// </summary>
        private void UnsubscribeFromEvents()
        {
            GameEvents.OnGameOver -= HandleGameOver;
            GameEvents.OnGameRestart -= HandleGameRestart;
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Handle game over event.
        /// </summary>
        private void HandleGameOver()
        {
            // Get stats from GameManager
            if (GameManager.Instance != null)
            {
                int wave = GameManager.Instance.GetCurrentWave();
                int kills = GameManager.Instance.GetTotalKills();
                float survivalTime = GameManager.Instance.GetSurvivalTime();

                ShowGameOverScreen(wave, kills, survivalTime);
            }
            else
            {
                ShowGameOverScreen(0, 0, 0f);
            }
        }

        /// <summary>
        /// Handle game restart event.
        /// </summary>
        private void HandleGameRestart()
        {
            HideImmediate();
        }
        #endregion

        #region Game Over Display
        /// <summary>
        /// Show game over screen with statistics.
        /// </summary>
        public void ShowGameOverScreen(int waveReached, int totalKills, float survivalTime)
        {
            // Update statistics
            if (_waveReachedText != null)
            {
                _waveReachedText.text = string.Format(_waveFormat, waveReached);
            }

            if (_killsText != null)
            {
                _killsText.text = string.Format(_killsFormat, totalKills);
            }

            if (_survivalTimeText != null)
            {
                _survivalTimeText.text = string.Format(_survivalTimeFormat, survivalTime);
            }

            // Show with delay
            Invoke(nameof(StartFadeIn), _delayBeforeShow);

            Debug.Log($"[GameOverScreen] Showing - Wave: {waveReached}, Kills: {totalKills}, Time: {survivalTime:F1}s");
        }

        /// <summary>
        /// Start fade in animation.
        /// </summary>
        private void StartFadeIn()
        {
            if (_gameOverPanel != null)
            {
                _gameOverPanel.SetActive(true);
            }

            _isShowing = true;
            _fadeTimer = 0f;

            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = 0f;
            }
        }

        /// <summary>
        /// Update fade animation.
        /// </summary>
        private void UpdateFadeAnimation()
        {
            if (!_isShowing || _canvasGroup == null) return;

            _fadeTimer += Time.deltaTime;

            if (_fadeTimer < _fadeInDuration)
            {
                float alpha = Mathf.Lerp(0f, 1f, _fadeTimer / _fadeInDuration);
                _canvasGroup.alpha = alpha;
            }
            else
            {
                _canvasGroup.alpha = 1f;
                _isShowing = false;
            }
        }

        /// <summary>
        /// Hide game over screen immediately.
        /// </summary>
        private void HideImmediate()
        {
            if (_gameOverPanel != null)
            {
                _gameOverPanel.SetActive(false);
            }

            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = 0f;
            }

            _isShowing = false;
            _fadeTimer = 0f;
        }
        #endregion

        #region Button Handlers
        /// <summary>
        /// Handle restart button click.
        /// </summary>
        private void OnRestartClicked()
        {
            Debug.Log("[GameOverScreen] Restart button clicked");

            HideImmediate();

            // Trigger restart through GameManager
            if (GameManager.Instance != null)
            {
                GameManager.Instance.RestartGame();
            }
        }

        /// <summary>
        /// Handle quit button click.
        /// </summary>
        private void OnQuitClicked()
        {
            Debug.Log("[GameOverScreen] Quit button clicked");

            // For now, just restart (Phase 2 doesn't have main menu)
            // In Phase 3+, this will return to main menu
            OnRestartClicked();

            // In final game, this would be:
            // #if UNITY_EDITOR
            //     UnityEditor.EditorApplication.isPlaying = false;
            // #else
            //     Application.Quit();
            // #endif
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Check if game over screen is currently showing.
        /// </summary>
        public bool IsShowing()
        {
            return _gameOverPanel != null && _gameOverPanel.activeSelf;
        }

        /// <summary>
        /// Force hide game over screen.
        /// </summary>
        public void Hide()
        {
            HideImmediate();
        }
        #endregion

        #region Debug Helpers
#if UNITY_EDITOR
        [ContextMenu("Test Game Over Screen")]
        private void TestGameOverScreen()
        {
            ShowGameOverScreen(5, 42, 123.5f);
        }
#endif
        #endregion
    }
}
