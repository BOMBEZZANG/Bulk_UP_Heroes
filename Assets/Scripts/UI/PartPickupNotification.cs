using System.Collections;
using UnityEngine;
using TMPro;
using BulkUpHeroes.Core;
using BulkUpHeroes.Parts;
using BulkUpHeroes.Utils;

namespace BulkUpHeroes.UI
{
    /// <summary>
    /// Displays part pickup notifications at center screen.
    /// Shows part name and stat bonuses when parts are picked up.
    ///
    /// Phase 3: Simple text notifications
    /// Future: Could add animated icons and sound effects
    /// </summary>
    public class PartPickupNotification : MonoBehaviour
    {
        #region Serialized Fields
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private TextMeshProUGUI _subtitleText;
        [SerializeField] private CanvasGroup _canvasGroup;

        [Header("Animation Settings")]
        [SerializeField] private float _displayDuration = 2f;
        [SerializeField] private float _fadeInDuration = 0.2f;
        [SerializeField] private float _fadeOutDuration = 0.5f;
        [SerializeField] private float _slideUpDistance = 50f;
        #endregion

        #region State
        private RectTransform _rectTransform;
        private Vector2 _startPosition;
        private Coroutine _currentAnimation;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            InitializeComponents();
            ValidateReferences();
        }

        private void Start()
        {
            SubscribeToEvents();
            HideNotification();
        }

        private void OnDestroy()
        {
            UnsubscribeFromEvents();
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Initialize component references.
        /// </summary>
        private void InitializeComponents()
        {
            _rectTransform = GetComponent<RectTransform>();

            if (_canvasGroup == null)
            {
                _canvasGroup = GetComponent<CanvasGroup>();
                if (_canvasGroup == null)
                {
                    _canvasGroup = gameObject.AddComponent<CanvasGroup>();
                }
            }

            _startPosition = _rectTransform.anchoredPosition;
        }

        /// <summary>
        /// Validate required references.
        /// </summary>
        private void ValidateReferences()
        {
            if (_titleText == null) Debug.LogError("[PartPickupNotification] Title text not assigned!");
            if (_subtitleText == null) Debug.LogError("[PartPickupNotification] Subtitle text not assigned!");
        }

        /// <summary>
        /// Hide notification initially.
        /// </summary>
        private void HideNotification()
        {
            _canvasGroup.alpha = 0f;
            gameObject.SetActive(false);
        }
        #endregion

        #region Event Subscription
        /// <summary>
        /// Subscribe to part pickup events.
        /// </summary>
        private void SubscribeToEvents()
        {
            GameEvents.OnPartPickedUp += HandlePartPickedUp;
            Debug.Log("[PartPickupNotification] Subscribed to GameEvents");
        }

        /// <summary>
        /// Unsubscribe from events.
        /// </summary>
        private void UnsubscribeFromEvents()
        {
            GameEvents.OnPartPickedUp -= HandlePartPickedUp;
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Handle part picked up event.
        /// </summary>
        private void HandlePartPickedUp(PartData partData)
        {
            if (partData == null) return;

            ShowNotification(partData);
        }
        #endregion

        #region Notification Display
        /// <summary>
        /// Show pickup notification for a part.
        /// </summary>
        public void ShowNotification(PartData partData)
        {
            // Ensure gameObject is active before starting coroutine
            if (!gameObject.activeInHierarchy)
            {
                gameObject.SetActive(true);
            }

            // Stop any current animation
            if (_currentAnimation != null)
            {
                StopCoroutine(_currentAnimation);
            }

            // Build notification text
            string title = $"{partData.partType.ToString().ToUpper()} ACQUIRED!";
            string subtitle = GetStatBonusText(partData);

            // Set text
            _titleText.text = title;
            _titleText.color = partData.GetRarityColor();
            _subtitleText.text = subtitle;
            _subtitleText.color = Color.white;

            // Start animation
            _currentAnimation = StartCoroutine(AnimateNotification());

            Debug.Log($"[PartPickupNotification] Showing notification for {partData.displayName}");
        }

        /// <summary>
        /// Get formatted stat bonus text for a part.
        /// </summary>
        private string GetStatBonusText(PartData partData)
        {
            string statText = "";

            // Find the primary stat bonus
            float healthBonus = partData.GetFinalHealthBonus();
            float damageBonus = partData.GetFinalDamageMultiplier();
            float attackSpeedBonus = partData.GetFinalAttackSpeedMultiplier();
            float moveSpeedBonus = partData.GetFinalMoveSpeedMultiplier();

            // Determine which stat has the highest bonus
            if (healthBonus > 0 && healthBonus >= damageBonus && healthBonus >= attackSpeedBonus && healthBonus >= moveSpeedBonus)
            {
                statText = $"+{healthBonus * 100:F0}% Health";
            }
            else if (damageBonus > 0 && damageBonus >= attackSpeedBonus && damageBonus >= moveSpeedBonus)
            {
                statText = $"+{damageBonus * 100:F0}% Damage";
            }
            else if (attackSpeedBonus > 0 && attackSpeedBonus >= moveSpeedBonus)
            {
                statText = $"+{attackSpeedBonus * 100:F0}% Attack Speed";
            }
            else if (moveSpeedBonus > 0)
            {
                statText = $"+{moveSpeedBonus * 100:F0}% Move Speed";
            }

            return statText;
        }

        /// <summary>
        /// Animate notification: fade in, slide up, fade out.
        /// </summary>
        private IEnumerator AnimateNotification()
        {
            // Reset position
            _rectTransform.anchoredPosition = _startPosition;

            // Fade in
            yield return StartCoroutine(FadeIn());

            // Hold
            yield return new WaitForSeconds(_displayDuration - _fadeInDuration - _fadeOutDuration);

            // Slide up and fade out simultaneously
            yield return StartCoroutine(SlideUpAndFadeOut());

            // Hide
            gameObject.SetActive(false);
            _currentAnimation = null;
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
                _canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / _fadeInDuration);
                yield return null;
            }

            _canvasGroup.alpha = 1f;
        }

        /// <summary>
        /// Slide up and fade out animation.
        /// </summary>
        private IEnumerator SlideUpAndFadeOut()
        {
            float elapsed = 0f;
            Vector2 startPos = _rectTransform.anchoredPosition;
            Vector2 endPos = startPos + Vector2.up * _slideUpDistance;

            while (elapsed < _fadeOutDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / _fadeOutDuration;

                _rectTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
                _canvasGroup.alpha = Mathf.Lerp(1f, 0f, t);

                yield return null;
            }

            _rectTransform.anchoredPosition = _startPosition;
            _canvasGroup.alpha = 0f;
        }
        #endregion

        #region Debug Helpers
#if UNITY_EDITOR
        [ContextMenu("Test Notification - Common")]
        private void DebugTestCommon()
        {
            var mockPart = ScriptableObject.CreateInstance<PartData>();
            mockPart.partType = PartType.Arms;
            mockPart.rarity = Rarity.Common;
            mockPart.displayName = "Common Arms";
            mockPart.damageMultiplier = 1.0f;
            ShowNotification(mockPart);
        }

        [ContextMenu("Test Notification - Epic")]
        private void DebugTestEpic()
        {
            var mockPart = ScriptableObject.CreateInstance<PartData>();
            mockPart.partType = PartType.Torso;
            mockPart.rarity = Rarity.Epic;
            mockPart.displayName = "Epic Torso";
            mockPart.healthBonus = 1.0f;
            ShowNotification(mockPart);
        }
#endif
        #endregion
    }
}
