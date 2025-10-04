using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BulkUpHeroes.Core;

namespace BulkUpHeroes.UI
{
    /// <summary>
    /// Handles center-screen announcements for wave start, wave complete, and game events.
    /// Provides animated text with fade-in/fade-out effects.
    ///
    /// Phase 2: Basic wave announcements with fade animation
    /// Future: Will support multiple announcement types, animations, sound effects
    /// </summary>
    public class AnnouncementSystem : MonoBehaviour
    {
        #region Announcement Settings
        [Header("Announcement Components")]
        [SerializeField] private GameObject _announcementPanel;
        [SerializeField] private TextMeshProUGUI _announcementText;
        [SerializeField] private TextMeshProUGUI _subText;

        [Header("Animation Settings")]
        [SerializeField] private float _fadeInDuration = 0.5f;
        [SerializeField] private float _displayDuration = 2f;
        [SerializeField] private float _fadeOutDuration = 0.5f;
        [SerializeField] private bool _scaleAnimation = true;
        [SerializeField] private float _scaleFrom = 0.5f;
        [SerializeField] private float _scaleTo = 1f;

        [Header("Wave Announcement Settings")]
        [SerializeField] private string _waveStartPrefix = "WAVE";
        [SerializeField] private string _waveCompleteText = "WAVE COMPLETE!";
        [SerializeField] private Color _waveStartColor = Color.white;
        [SerializeField] private Color _waveCompleteColor = Color.green;
        #endregion

        #region State
        private CanvasGroup _canvasGroup;
        private RectTransform _panelTransform;
        private bool _isShowingAnnouncement = false;
        private Coroutine _currentAnnouncementCoroutine;
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
        #endregion

        #region Initialization
        /// <summary>
        /// Initialize components and references.
        /// </summary>
        private void InitializeComponents()
        {
            // Get or add CanvasGroup for fading
            if (_announcementPanel != null)
            {
                _canvasGroup = _announcementPanel.GetComponent<CanvasGroup>();
                if (_canvasGroup == null)
                {
                    _canvasGroup = _announcementPanel.AddComponent<CanvasGroup>();
                }

                _panelTransform = _announcementPanel.GetComponent<RectTransform>();

                // Start hidden
                _canvasGroup.alpha = 0f;
                _announcementPanel.SetActive(false);
            }

            if (_announcementText == null)
            {
                Debug.LogError("[AnnouncementSystem] Announcement Text not assigned!");
            }
        }

        /// <summary>
        /// Subscribe to game events.
        /// </summary>
        private void SubscribeToEvents()
        {
            GameEvents.OnWaveStart += HandleWaveStart;
            GameEvents.OnWaveComplete += HandleWaveComplete;
            GameEvents.OnGameStart += HandleGameStart;
            GameEvents.OnGameOver += HandleGameOver;
        }

        /// <summary>
        /// Unsubscribe from game events.
        /// </summary>
        private void UnsubscribeFromEvents()
        {
            GameEvents.OnWaveStart -= HandleWaveStart;
            GameEvents.OnWaveComplete -= HandleWaveComplete;
            GameEvents.OnGameStart -= HandleGameStart;
            GameEvents.OnGameOver -= HandleGameOver;
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Handle wave start event.
        /// </summary>
        private void HandleWaveStart(int waveNumber)
        {
            string mainText = $"{_waveStartPrefix} {waveNumber}";
            string subtext = "Get Ready!";
            ShowAnnouncement(mainText, subtext, _waveStartColor);
        }

        /// <summary>
        /// Handle wave complete event.
        /// </summary>
        private void HandleWaveComplete(int waveNumber)
        {
            string mainText = _waveCompleteText;
            string subtext = "Prepare for next wave...";
            ShowAnnouncement(mainText, subtext, _waveCompleteColor, 1.5f);
        }

        /// <summary>
        /// Handle game start event.
        /// </summary>
        private void HandleGameStart()
        {
            ShowAnnouncement("BULK UP HEROES", "Survive the waves!", Color.cyan, 2f);
        }

        /// <summary>
        /// Handle game over event.
        /// </summary>
        private void HandleGameOver()
        {
            // Game Over screen will handle this
            // Just hide any active announcements
            HideAnnouncement();
        }
        #endregion

        #region Announcement Display
        /// <summary>
        /// Show announcement with text and color.
        /// </summary>
        public void ShowAnnouncement(string mainText, string subText = "", Color? color = null, float? displayDuration = null)
        {
            // Stop current announcement if any
            if (_currentAnnouncementCoroutine != null)
            {
                StopCoroutine(_currentAnnouncementCoroutine);
            }

            // Start new announcement
            _currentAnnouncementCoroutine = StartCoroutine(AnnouncementSequence(
                mainText,
                subText,
                color ?? Color.white,
                displayDuration ?? _displayDuration
            ));
        }

        /// <summary>
        /// Announcement animation sequence.
        /// </summary>
        private IEnumerator AnnouncementSequence(string mainText, string subText, Color color, float displayDuration)
        {
            _isShowingAnnouncement = true;

            // Setup
            _announcementPanel.SetActive(true);
            _announcementText.text = mainText;
            _announcementText.color = color;

            if (_subText != null)
            {
                _subText.text = subText;
                _subText.gameObject.SetActive(!string.IsNullOrEmpty(subText));
            }

            // Fade in
            yield return StartCoroutine(FadeIn());

            // Display
            yield return new WaitForSeconds(displayDuration);

            // Fade out
            yield return StartCoroutine(FadeOut());

            // Cleanup
            _announcementPanel.SetActive(false);
            _isShowingAnnouncement = false;
        }

        /// <summary>
        /// Fade in animation.
        /// </summary>
        private IEnumerator FadeIn()
        {
            float elapsed = 0f;

            while (elapsed < _fadeInDuration)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / _fadeInDuration;

                // Alpha
                _canvasGroup.alpha = Mathf.Lerp(0f, 1f, progress);

                // Scale
                if (_scaleAnimation && _panelTransform != null)
                {
                    float scale = Mathf.Lerp(_scaleFrom, _scaleTo, progress);
                    _panelTransform.localScale = Vector3.one * scale;
                }

                yield return null;
            }

            _canvasGroup.alpha = 1f;
            if (_scaleAnimation && _panelTransform != null)
            {
                _panelTransform.localScale = Vector3.one * _scaleTo;
            }
        }

        /// <summary>
        /// Fade out animation.
        /// </summary>
        private IEnumerator FadeOut()
        {
            float elapsed = 0f;

            while (elapsed < _fadeOutDuration)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / _fadeOutDuration;

                // Alpha
                _canvasGroup.alpha = Mathf.Lerp(1f, 0f, progress);

                // Scale
                if (_scaleAnimation && _panelTransform != null)
                {
                    float scale = Mathf.Lerp(_scaleTo, _scaleFrom, progress);
                    _panelTransform.localScale = Vector3.one * scale;
                }

                yield return null;
            }

            _canvasGroup.alpha = 0f;
        }

        /// <summary>
        /// Immediately hide announcement.
        /// </summary>
        public void HideAnnouncement()
        {
            if (_currentAnnouncementCoroutine != null)
            {
                StopCoroutine(_currentAnnouncementCoroutine);
                _currentAnnouncementCoroutine = null;
            }

            if (_announcementPanel != null)
            {
                _announcementPanel.SetActive(false);
            }

            _isShowingAnnouncement = false;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Check if currently showing an announcement.
        /// </summary>
        public bool IsShowingAnnouncement()
        {
            return _isShowingAnnouncement;
        }

        /// <summary>
        /// Show custom announcement (for external calls).
        /// </summary>
        public void ShowCustomAnnouncement(string text, float duration = 2f)
        {
            ShowAnnouncement(text, "", Color.white, duration);
        }
        #endregion
    }
}
