using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BulkUpHeroes.Core;

namespace BulkUpHeroes.UI
{
    /// <summary>
    /// Health bar UI component with smooth fill animation and color transitions.
    /// Listens to health change events and updates visual representation.
    ///
    /// Phase 2: Basic health bar with smooth fill
    /// Future: Will support damage flash, heal effects, shield overlay
    /// </summary>
    public class HealthBar : MonoBehaviour
    {
        #region Visual Settings
        [Header("Health Bar Components")]
        [SerializeField] private Image _fillImage;
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private TextMeshProUGUI _healthText;

        [Header("Visual Settings")]
        [SerializeField] private bool _smoothFill = true;
        [SerializeField] private float _fillSpeed = 5f;
        [SerializeField] private bool _enableColorTransition = true;
        [SerializeField] private bool _showHealthText = true;

        [Header("Color Thresholds")]
        [SerializeField] private Color _healthyColor = Color.green;
        [SerializeField] private Color _warningColor = Color.yellow;
        [SerializeField] private Color _criticalColor = Color.red;
        [SerializeField] private float _warningThreshold = 0.5f;
        [SerializeField] private float _criticalThreshold = 0.25f;
        #endregion

        #region State
        private float _currentFillAmount = 1f;
        private float _targetFillAmount = 1f;
        private float _currentHealth = 100f;
        private float _maxHealth = 100f;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            ValidateComponents();
        }

        private void Start()
        {
            // Pre-warm after one frame to ensure Canvas is ready
            StartCoroutine(PrewarmUIDelayed());
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
            UpdateFillAnimation();
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Validate required components.
        /// </summary>
        private void ValidateComponents()
        {
            if (_fillImage == null)
            {
                Debug.LogError("[HealthBar] Fill Image not assigned!");
            }

            if (_healthText != null && !_showHealthText)
            {
                _healthText.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Subscribe to health change events.
        /// </summary>
        private void SubscribeToEvents()
        {
            GameEvents.OnPlayerHealthChanged += HandleHealthChanged;
            Debug.Log("[HealthBar] Subscribed to player health events");
        }

        /// <summary>
        /// Unsubscribe from health change events.
        /// </summary>
        private void UnsubscribeFromEvents()
        {
            GameEvents.OnPlayerHealthChanged -= HandleHealthChanged;
        }

        /// <summary>
        /// Pre-warm UI elements with delay to ensure Canvas is ready.
        /// </summary>
        private System.Collections.IEnumerator PrewarmUIDelayed()
        {
            // Wait for end of frame to ensure Canvas and rendering systems are ready
            yield return new WaitForEndOfFrame();

            PrewarmUI();
        }

        /// <summary>
        /// Pre-warm UI elements to avoid first-update lag.
        /// Forces TextMeshPro font atlas generation and Image rendering.
        /// </summary>
        private void PrewarmUI()
        {
            if (_fillImage != null)
            {
                // Trigger multiple fill and color changes to initialize rendering pipeline
                for (int i = 0; i < 5; i++)
                {
                    _fillImage.fillAmount = 1f;
                    _fillImage.color = _healthyColor;
                    _fillImage.fillAmount = 0.5f;
                    _fillImage.color = _warningColor;
                    _fillImage.fillAmount = 0.25f;
                    _fillImage.color = _criticalColor;
                    _fillImage.fillAmount = 1f;
                    _fillImage.color = _healthyColor;
                }
            }

            if (_healthText != null && _showHealthText)
            {
                // Aggressively force TextMeshPro font atlas generation
                // This is the main cause of first-update lag
                string originalText = _healthText.text;

                // Force rendering with various character combinations to build font atlas
                _healthText.text = "0123456789";
                _healthText.ForceMeshUpdate(true, true);

                _healthText.text = "888 / 888";
                _healthText.ForceMeshUpdate(true, true);

                _healthText.text = "999 / 999";
                _healthText.ForceMeshUpdate(true, true);

                _healthText.text = "500 / 500";
                _healthText.ForceMeshUpdate(true, true);

                _healthText.text = "1 / 1";
                _healthText.ForceMeshUpdate(true, true);

                // Restore original or set default
                _healthText.text = originalText;
                _healthText.ForceMeshUpdate(true, true);
            }

            // Force Canvas rebuild
            Canvas.ForceUpdateCanvases();

            Debug.Log("[HealthBar] UI pre-warmed with font atlas generation");
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Handle health change event.
        /// </summary>
        private void HandleHealthChanged(float currentHealth, float maxHealth)
        {
            Debug.Log($"[HealthBar] Health changed: {currentHealth}/{maxHealth}");

            _currentHealth = currentHealth;
            _maxHealth = maxHealth;

            float newFillAmount = maxHealth > 0 ? currentHealth / maxHealth : 0f;
            SetHealth(newFillAmount);

            // Update health text
            if (_showHealthText && _healthText != null)
            {
                _healthText.text = $"{Mathf.CeilToInt(currentHealth)} / {Mathf.CeilToInt(maxHealth)}";
            }
        }
        #endregion

        #region Health Bar Update
        /// <summary>
        /// Set health bar fill amount (0-1).
        /// </summary>
        public void SetHealth(float fillAmount)
        {
            _targetFillAmount = Mathf.Clamp01(fillAmount);

            if (!_smoothFill)
            {
                _currentFillAmount = _targetFillAmount;
                UpdateFillImage();
            }
        }

        /// <summary>
        /// Update fill animation (smooth transition).
        /// </summary>
        private void UpdateFillAnimation()
        {
            if (!_smoothFill) return;

            if (Mathf.Abs(_currentFillAmount - _targetFillAmount) > 0.001f)
            {
                _currentFillAmount = Mathf.Lerp(_currentFillAmount, _targetFillAmount, Time.deltaTime * _fillSpeed);
                UpdateFillImage();
            }
            else
            {
                _currentFillAmount = _targetFillAmount;
            }
        }

        /// <summary>
        /// Update fill image visual.
        /// </summary>
        private void UpdateFillImage()
        {
            if (_fillImage == null) return;

            _fillImage.fillAmount = _currentFillAmount;

            if (_enableColorTransition)
            {
                UpdateFillColor();
            }
        }

        /// <summary>
        /// Update fill color based on health percentage.
        /// </summary>
        private void UpdateFillColor()
        {
            Color targetColor;

            if (_currentFillAmount <= _criticalThreshold)
            {
                targetColor = _criticalColor;
            }
            else if (_currentFillAmount <= _warningThreshold)
            {
                // Lerp between warning and critical
                float t = (_currentFillAmount - _criticalThreshold) / (_warningThreshold - _criticalThreshold);
                targetColor = Color.Lerp(_criticalColor, _warningColor, t);
            }
            else
            {
                // Lerp between healthy and warning
                float t = (_currentFillAmount - _warningThreshold) / (1f - _warningThreshold);
                targetColor = Color.Lerp(_warningColor, _healthyColor, t);
            }

            _fillImage.color = targetColor;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Initialize health bar with specific values.
        /// </summary>
        public void Initialize(float currentHealth, float maxHealth)
        {
            _currentHealth = currentHealth;
            _maxHealth = maxHealth;

            float fillAmount = maxHealth > 0 ? currentHealth / maxHealth : 0f;
            _currentFillAmount = fillAmount;
            _targetFillAmount = fillAmount;

            UpdateFillImage();

            if (_showHealthText && _healthText != null)
            {
                _healthText.text = $"{Mathf.CeilToInt(currentHealth)} / {Mathf.CeilToInt(maxHealth)}";
            }
        }

        /// <summary>
        /// Reset health bar to full.
        /// </summary>
        public void ResetToFull()
        {
            SetHealth(1f);
            if (_showHealthText && _healthText != null)
            {
                _healthText.text = $"{Mathf.CeilToInt(_maxHealth)} / {Mathf.CeilToInt(_maxHealth)}";
            }
        }
        #endregion
    }
}
