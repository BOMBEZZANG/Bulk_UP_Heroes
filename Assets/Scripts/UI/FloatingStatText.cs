using System.Collections;
using UnityEngine;
using TMPro;

namespace BulkUpHeroes.UI
{
    /// <summary>
    /// Floating text that appears above player when stats change.
    /// Shows stat increase/decrease with color coding.
    ///
    /// Phase 3: Simple floating text
    /// Future: Could add more elaborate animations and pooling
    /// </summary>
    public class FloatingStatText : MonoBehaviour
    {
        #region Serialized Fields
        [Header("Text Settings")]
        [SerializeField] private TextMeshProUGUI _textElement;

        [Header("Animation Settings")]
        [SerializeField] private float _duration = 1f;
        [SerializeField] private float _floatDistance = 1f;
        [SerializeField] private float _fadeDelay = 0.5f;
        #endregion

        #region State
        private RectTransform _rectTransform;
        private CanvasGroup _canvasGroup;
        #endregion

        #region Initialization
        /// <summary>
        /// Initialize the floating text.
        /// </summary>
        /// <param name="text">Text to display</param>
        /// <param name="color">Color of the text</param>
        /// <param name="worldPosition">World position to spawn at</param>
        public void Initialize(string text, Color color, Vector3 worldPosition)
        {
            // Get or add components
            _rectTransform = GetComponent<RectTransform>();
            _canvasGroup = GetComponent<CanvasGroup>();

            if (_canvasGroup == null)
            {
                _canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }

            // Create text element if not assigned
            if (_textElement == null)
            {
                _textElement = GetComponentInChildren<TextMeshProUGUI>();
                if (_textElement == null)
                {
                    Debug.LogError("[FloatingStatText] No TextMeshProUGUI found!");
                    Destroy(gameObject);
                    return;
                }
            }

            // Set text and color
            _textElement.text = text;
            _textElement.color = color;

            // Convert world position to screen position
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                Vector3 screenPosition = mainCamera.WorldToScreenPoint(worldPosition);
                _rectTransform.position = screenPosition;
            }

            // Start animation
            StartCoroutine(AnimateAndDestroy());
        }
        #endregion

        #region Animation
        /// <summary>
        /// Animate the floating text upward and fade out.
        /// </summary>
        private IEnumerator AnimateAndDestroy()
        {
            float elapsed = 0f;
            Vector3 startPosition = _rectTransform.position;
            Vector3 endPosition = startPosition + Vector3.up * _floatDistance * 100f; // UI units

            _canvasGroup.alpha = 1f;

            while (elapsed < _duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / _duration;

                // Move upward
                _rectTransform.position = Vector3.Lerp(startPosition, endPosition, t);

                // Fade out after delay
                if (elapsed >= _fadeDelay)
                {
                    float fadeT = (elapsed - _fadeDelay) / (_duration - _fadeDelay);
                    _canvasGroup.alpha = Mathf.Lerp(1f, 0f, fadeT);
                }

                yield return null;
            }

            // Destroy when done
            Destroy(gameObject);
        }
        #endregion

        #region Static Factory
        /// <summary>
        /// Create a floating stat text at a world position.
        /// </summary>
        /// <param name="text">Text to display</param>
        /// <param name="color">Color of the text</param>
        /// <param name="worldPosition">World position to spawn at</param>
        /// <param name="canvas">Canvas to spawn under</param>
        /// <returns>Created FloatingStatText instance</returns>
        public static FloatingStatText Create(string text, Color color, Vector3 worldPosition, Canvas canvas)
        {
            // Create GameObject
            GameObject textObj = new GameObject("FloatingStatText");
            textObj.transform.SetParent(canvas.transform, false);

            // Add RectTransform
            RectTransform rectTransform = textObj.AddComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(200f, 50f);

            // Add TextMeshProUGUI
            TextMeshProUGUI textMesh = textObj.AddComponent<TextMeshProUGUI>();
            textMesh.alignment = TextAlignmentOptions.Center;
            textMesh.fontSize = 24f;
            textMesh.fontStyle = FontStyles.Bold;
            textMesh.enableWordWrapping = false;

            // Add FloatingStatText component
            FloatingStatText floatingText = textObj.AddComponent<FloatingStatText>();
            floatingText._textElement = textMesh;

            // Initialize
            floatingText.Initialize(text, color, worldPosition);

            return floatingText;
        }
        #endregion
    }
}
