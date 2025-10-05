using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using BulkUpHeroes.Utils;

namespace BulkUpHeroes.UI
{
    /// <summary>
    /// Virtual joystick for mobile touch controls.
    /// Appears on touch down in the left half of the screen.
    ///
    /// Phase 1: Basic joystick functionality using direct touch input
    /// Future: Can add visual effects and haptic feedback
    /// </summary>
    public class VirtualJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        #region UI Components
        [Header("UI References")]
        [SerializeField] private RectTransform _joystickContainer;
        [SerializeField] private RectTransform _joystickBackground;
        [SerializeField] private RectTransform _joystickHandle;

        [Header("Visual Settings")]
        [SerializeField] private float _handleRange = 50f;
        [SerializeField] private float _deadZone = 0.1f;
        [SerializeField] private bool _dynamicJoystick = false; // Joystick appears at touch position (disabled for Phase 1)
        [SerializeField] private bool _useDirectTouchInput = true; // Use Input.touches instead of EventSystem (more reliable on mobile)

        [Header("Colors")]
        [SerializeField] private Color _inactiveColor = new Color(1f, 1f, 1f, 0.5f);
        [SerializeField] private Color _activeColor = new Color(1f, 1f, 1f, 0.9f);

        private Image _backgroundImage;
        private Image _handleImage;
        #endregion

        #region Input Properties
        private Vector2 _inputDirection = Vector2.zero;
        private Vector2 _touchStartPosition;
        private bool _isTouching = false;
        private int _touchId = -1;

        public Vector2 InputDirection => _inputDirection;
        public bool IsTouching => _isTouching;
        #endregion

        #region Canvas Properties
        private Canvas _canvas;
        private RectTransform _canvasRect;
        private Vector2 _joystickStartPosition;
        private float _screenWidthHalf;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            InitializeComponents();
        }

        private void Start()
        {
            ResetJoystick();
            _screenWidthHalf = Screen.width * 0.5f;
        }

        private void Update()
        {
            if (_useDirectTouchInput)
            {
                ProcessDirectTouchInput();
            }
            else
            {
                // Handle multi-touch scenario for EventSystem mode
                if (_isTouching && Input.touchCount > 0)
                {
                    // Verify our touch is still active
                    bool touchStillActive = false;
                    foreach (Touch touch in Input.touches)
                    {
                        if (touch.fingerId == _touchId)
                        {
                            touchStillActive = true;
                            break;
                        }
                    }

                    if (!touchStillActive)
                    {
                        OnPointerUp(null);
                    }
                }
            }
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Initialize UI components and references.
        /// </summary>
        private void InitializeComponents()
        {
            // Get canvas references
            _canvas = GetComponentInParent<Canvas>();
            if (_canvas == null)
            {
                Debug.LogError("[VirtualJoystick] Canvas not found in parent hierarchy!");
                return;
            }
            _canvasRect = _canvas.GetComponent<RectTransform>();

            // Auto-assign if not set
            if (_joystickContainer == null)
                _joystickContainer = GetComponent<RectTransform>();

            if (_joystickBackground == null)
                _joystickBackground = transform.Find("Background")?.GetComponent<RectTransform>();

            if (_joystickHandle == null)
                _joystickHandle = transform.Find("Background/Handle")?.GetComponent<RectTransform>();

            // Get image components
            if (_joystickBackground != null)
                _backgroundImage = _joystickBackground.GetComponent<Image>();

            if (_joystickHandle != null)
                _handleImage = _joystickHandle.GetComponent<Image>();

            // Force set initial colors to ensure visibility
            if (_backgroundImage != null)
            {
                _backgroundImage.color = Color.white; // Start with full white
                Debug.Log($"[VirtualJoystick] Background Image initial color: {_backgroundImage.color}");
            }

            if (_handleImage != null)
            {
                _handleImage.color = Color.white; // Start with full white
                Debug.Log($"[VirtualJoystick] Handle Image initial color: {_handleImage.color}");
            }

            // Store initial position
            _joystickStartPosition = _joystickContainer.anchoredPosition;

            Debug.Log("[VirtualJoystick] Initialized");
        }
        #endregion

        #region Direct Touch Input Processing
        /// <summary>
        /// Process touch input directly using Input.touches API.
        /// More reliable than EventSystem on mobile builds.
        /// </summary>
        private void ProcessDirectTouchInput()
        {
            // Handle touch input
            if (Input.touchCount > 0)
            {
                // Find touch in left half of screen (joystick zone)
                Touch? activeTouch = null;

                if (_isTouching)
                {
                    // Already tracking a touch - find it
                    foreach (Touch touch in Input.touches)
                    {
                        if (touch.fingerId == _touchId)
                        {
                            activeTouch = touch;
                            break;
                        }
                    }
                }
                else
                {
                    // Look for new touch in left half
                    foreach (Touch touch in Input.touches)
                    {
                        if (touch.position.x < _screenWidthHalf && touch.phase == TouchPhase.Began)
                        {
                            activeTouch = touch;
                            break;
                        }
                    }
                }

                if (activeTouch.HasValue)
                {
                    Touch touch = activeTouch.Value;

                    switch (touch.phase)
                    {
                        case TouchPhase.Began:
                            HandleTouchBegan(touch.position, touch.fingerId);
                            break;

                        case TouchPhase.Moved:
                        case TouchPhase.Stationary:
                            HandleTouchDrag(touch.position);
                            break;

                        case TouchPhase.Ended:
                        case TouchPhase.Canceled:
                            HandleTouchEnded();
                            break;
                    }
                }
                else if (_isTouching)
                {
                    // Our touch was lost
                    HandleTouchEnded();
                }
            }
            // Handle mouse input for Unity Editor testing
            else if (Application.isEditor)
            {
                if (Input.GetMouseButtonDown(0) && Input.mousePosition.x < _screenWidthHalf)
                {
                    HandleTouchBegan(Input.mousePosition, 0);
                }
                else if (Input.GetMouseButton(0) && _isTouching)
                {
                    HandleTouchDrag(Input.mousePosition);
                }
                else if (Input.GetMouseButtonUp(0) && _isTouching)
                {
                    HandleTouchEnded();
                }
            }
        }

        /// <summary>
        /// Handle touch/mouse began.
        /// </summary>
        private void HandleTouchBegan(Vector2 screenPosition, int touchId)
        {
            _isTouching = true;
            _touchId = touchId;
            _touchStartPosition = screenPosition;

            // Position joystick at touch location if dynamic
            if (_dynamicJoystick)
            {
                Vector2 localPoint;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    _canvasRect,
                    screenPosition,
                    _canvas.worldCamera,
                    out localPoint
                );

                _joystickContainer.anchoredPosition = localPoint;
                Debug.Log($"[VirtualJoystick] Container repositioned to: {localPoint}");
            }
            else
            {
                // FIXED: Keep joystick at its original position (don't move it!)
                // Container stays where it was placed in the scene
                Debug.Log($"[VirtualJoystick] Container staying at original position: {_joystickContainer.anchoredPosition}");
            }

            // Show joystick
            ShowJoystick(true);

            Debug.Log($"[VirtualJoystick] Touch started at {screenPosition}, Dynamic={_dynamicJoystick}");
        }

        /// <summary>
        /// Handle touch/mouse drag.
        /// </summary>
        private void HandleTouchDrag(Vector2 currentPosition)
        {
            Vector2 offset;

            if (_dynamicJoystick)
            {
                // Dynamic: Calculate offset from touch start position
                offset = currentPosition - _touchStartPosition;
            }
            else
            {
                // FIXED: Calculate offset from joystick container's CENTER position
                // Convert joystick center to screen space
                Vector2 joystickScreenPos;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    _canvasRect,
                    currentPosition,
                    _canvas.worldCamera,
                    out Vector2 localTouchPos
                );

                // Get joystick container's local position
                Vector2 joystickLocalPos = _joystickBackground.anchoredPosition;

                // Calculate offset in local space
                offset = localTouchPos - (_joystickContainer.anchoredPosition + joystickLocalPos);

                // Scale offset to match screen space
                // For Screen Space - Overlay, we can use direct pixel offset
                Vector3[] corners = new Vector3[4];
                _canvasRect.GetWorldCorners(corners);
                float canvasScale = _canvas.scaleFactor;

                // Simpler approach: calculate offset from joystick center in screen space
                Vector2 joystickCenterScreen = RectTransformUtility.WorldToScreenPoint(
                    _canvas.worldCamera,
                    _joystickBackground.position
                );

                offset = currentPosition - joystickCenterScreen;
            }

            // Clamp the offset to handle range
            Vector2 clampedDirection = Vector2.ClampMagnitude(offset, _handleRange);

            // Update handle position
            if (_joystickHandle != null)
            {
                _joystickHandle.anchoredPosition = clampedDirection;
            }

            // Calculate normalized input (-1 to 1)
            _inputDirection = clampedDirection / _handleRange;

            // Apply dead zone
            if (_inputDirection.magnitude < _deadZone)
            {
                _inputDirection = Vector2.zero;
            }

            Debug.Log($"[VirtualJoystick] Input: {_inputDirection:F2}, Offset: {offset:F2}, CurrentPos: {currentPosition}");
        }

        /// <summary>
        /// Handle touch/mouse ended.
        /// </summary>
        private void HandleTouchEnded()
        {
            ResetJoystick();
        }
        #endregion

        #region Input Handlers (EventSystem)
        /// <summary>
        /// Called when pointer/touch begins on joystick area.
        /// </summary>
        public void OnPointerDown(PointerEventData eventData)
        {
            _isTouching = true;
            _touchId = eventData.pointerId;
            _touchStartPosition = eventData.position;

            // Position joystick at touch location if dynamic
            if (_dynamicJoystick)
            {
                Vector2 localPoint;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    _canvasRect,
                    eventData.position,
                    _canvas.worldCamera,
                    out localPoint
                );

                _joystickContainer.anchoredPosition = localPoint;
                Debug.Log($"[VirtualJoystick] Container repositioned to: {localPoint}");
            }

            // Show joystick
            ShowJoystick(true);

            Debug.Log($"[VirtualJoystick] Touch started at {eventData.position}");
        }

        /// <summary>
        /// Called when pointer/touch is dragged.
        /// </summary>
        public void OnDrag(PointerEventData eventData)
        {
            if (!_isTouching) return;

            // Calculate offset from touch start position (not from container position)
            // This works correctly regardless of canvas scaling
            Vector2 offset = eventData.position - _touchStartPosition;

            // Clamp the offset to handle range
            Vector2 clampedDirection = Vector2.ClampMagnitude(offset, _handleRange);

            // Update handle position
            if (_joystickHandle != null)
            {
                _joystickHandle.anchoredPosition = clampedDirection;
            }

            // Calculate normalized input (-1 to 1)
            _inputDirection = clampedDirection / _handleRange;

            // Apply dead zone
            if (_inputDirection.magnitude < _deadZone)
            {
                _inputDirection = Vector2.zero;
            }

            // Debug visualization (commented out to avoid performance issues)
            // Debug.Log($"[VirtualJoystick] Input: {_inputDirection}, Offset: {offset}, Clamped: {clampedDirection}");
        }

        /// <summary>
        /// Called when pointer/touch is released.
        /// </summary>
        public void OnPointerUp(PointerEventData eventData)
        {
            ResetJoystick();
        }
        #endregion

        #region Joystick Control
        /// <summary>
        /// Reset joystick to default state.
        /// </summary>
        private void ResetJoystick()
        {
            _isTouching = false;
            _touchId = -1;
            _inputDirection = Vector2.zero;

            // Reset handle position
            if (_joystickHandle != null)
            {
                _joystickHandle.anchoredPosition = Vector2.zero;
            }

            // Return to start position if dynamic
            if (_dynamicJoystick)
            {
                _joystickContainer.anchoredPosition = _joystickStartPosition;
            }

            // Show joystick as inactive (dimmed)
            // Don't hide it completely - keep it visible so player knows where to touch
            ShowJoystick(false);

            // Debug.Log("[VirtualJoystick] Reset - joystick dimmed");
        }

        /// <summary>
        /// Show or hide joystick visuals.
        /// </summary>
        private void ShowJoystick(bool show)
        {
            Color targetColor = show ? _activeColor : _inactiveColor;

            // Debug.Log($"[VirtualJoystick] ShowJoystick({show}) - Setting color to: {targetColor}");

            if (_backgroundImage != null)
            {
                _backgroundImage.color = targetColor;
                // Debug.Log($"[VirtualJoystick] Background color set to: {_backgroundImage.color}");
            }

            if (_handleImage != null)
            {
                _handleImage.color = targetColor;
                // Debug.Log($"[VirtualJoystick] Handle color set to: {_handleImage.color}");
            }

            // For dynamic joystick, we just change opacity, not activate/deactivate
            // This makes it easier to see and debug
            // Keep the GameObjects active always
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Get the current input magnitude (0 to 1).
        /// </summary>
        public float GetInputMagnitude()
        {
            return _inputDirection.magnitude;
        }

        /// <summary>
        /// Check if joystick is being actively used.
        /// </summary>
        public bool IsActive()
        {
            return _isTouching && _inputDirection.magnitude > _deadZone;
        }

        /// <summary>
        /// Force reset the joystick (for game state changes).
        /// </summary>
        public void ForceReset()
        {
            ResetJoystick();
        }
        #endregion

        #region Debug Helpers
#if UNITY_EDITOR
        private void OnValidate()
        {
            // Ensure values are reasonable
            _handleRange = Mathf.Max(10f, _handleRange);
            _deadZone = Mathf.Clamp01(_deadZone);
        }

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying) return;

            // Visualize input direction in scene view
            if (_inputDirection.magnitude > 0.01f)
            {
                Vector3 worldPos = transform.position;
                Vector3 direction = new Vector3(_inputDirection.x, 0, _inputDirection.y);
                Gizmos.color = Color.yellow;
                Gizmos.DrawRay(worldPos, direction * 2f);
            }
        }
#endif
        #endregion
    }
}
