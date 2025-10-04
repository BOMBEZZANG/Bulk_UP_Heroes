using UnityEngine;
using BulkUpHeroes.Utils;

namespace BulkUpHeroes.UI
{
    /// <summary>
    /// Handles touch input and routing to appropriate game systems.
    /// Manages input zones (left for movement, right for future actions).
    ///
    /// Phase 1: Basic touch zone detection for joystick using legacy Input system
    /// Future: Can be upgraded to new Input System and will handle attack buttons and special abilities
    /// </summary>
    public class TouchInputHandler : MonoBehaviour
    {
        #region Input Zones
        [Header("Touch Zone Settings")]
        [SerializeField] private float _screenDivision = 0.5f; // 0-1, where joystick ends
        [SerializeField] private bool _showDebugZones = false;

        private InputZone _currentTouchZone = InputZone.None;
        #endregion

        #region Touch Tracking
        [Header("Touch State")]
        [SerializeField] private bool _isTouchActive = false;
        [SerializeField] private Vector2 _currentTouchPosition;
        [SerializeField] private Vector2 _touchStartPosition;
        private int _activeTouchId = -1;
        #endregion

        #region Properties
        public bool IsTouchActive => _isTouchActive;
        public Vector2 CurrentTouchPosition => _currentTouchPosition;
        public InputZone CurrentTouchZone => _currentTouchZone;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            InitializeInput();
        }

        private void OnEnable()
        {
            EnableInput();
        }

        private void OnDisable()
        {
            DisableInput();
        }

        private void Update()
        {
            ProcessTouchInput();
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Initialize input system.
        /// </summary>
        private void InitializeInput()
        {
            // For Phase 1, we'll use Unity's Input class for simplicity
            // Can be upgraded to Input System in future phases
            Debug.Log("[TouchInputHandler] Initialized");
        }
        #endregion

        #region Input Processing
        /// <summary>
        /// Process touch input each frame.
        /// </summary>
        private void ProcessTouchInput()
        {
            // Handle touch input
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0); // Primary touch

                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        OnTouchBegan(touch);
                        break;

                    case TouchPhase.Moved:
                    case TouchPhase.Stationary:
                        OnTouchMoved(touch);
                        break;

                    case TouchPhase.Ended:
                    case TouchPhase.Canceled:
                        OnTouchEnded(touch);
                        break;
                }
            }
            // Handle mouse input for Unity Editor testing
            else if (Input.GetMouseButtonDown(0))
            {
                OnMouseDown();
            }
            else if (Input.GetMouseButton(0))
            {
                OnMouseDrag();
            }
            else if (Input.GetMouseButtonUp(0))
            {
                OnMouseUp();
            }
        }

        /// <summary>
        /// Handle touch begin.
        /// </summary>
        private void OnTouchBegan(Touch touch)
        {
            _isTouchActive = true;
            _activeTouchId = touch.fingerId;
            _touchStartPosition = touch.position;
            _currentTouchPosition = touch.position;
            _currentTouchZone = DetermineInputZone(touch.position);

            Debug.Log($"[TouchInputHandler] Touch began in zone: {_currentTouchZone}");
        }

        /// <summary>
        /// Handle touch move.
        /// </summary>
        private void OnTouchMoved(Touch touch)
        {
            if (touch.fingerId != _activeTouchId) return;

            _currentTouchPosition = touch.position;
            // Zone is determined at touch start and doesn't change
        }

        /// <summary>
        /// Handle touch end.
        /// </summary>
        private void OnTouchEnded(Touch touch)
        {
            if (touch.fingerId != _activeTouchId) return;

            _isTouchActive = false;
            _activeTouchId = -1;
            _currentTouchZone = InputZone.None;

            Debug.Log("[TouchInputHandler] Touch ended");
        }

        #endregion

        #region Mouse Input (Editor Testing)
        /// <summary>
        /// Handle mouse down (for editor testing).
        /// </summary>
        private void OnMouseDown()
        {
            _isTouchActive = true;
            _touchStartPosition = Input.mousePosition;
            _currentTouchPosition = Input.mousePosition;
            _currentTouchZone = DetermineInputZone(Input.mousePosition);
        }

        /// <summary>
        /// Handle mouse drag (for editor testing).
        /// </summary>
        private void OnMouseDrag()
        {
            _currentTouchPosition = Input.mousePosition;
        }

        /// <summary>
        /// Handle mouse up (for editor testing).
        /// </summary>
        private void OnMouseUp()
        {
            _isTouchActive = false;
            _currentTouchZone = InputZone.None;
        }
        #endregion

        #region Input Zone Detection
        /// <summary>
        /// Determine which input zone a screen position belongs to.
        /// </summary>
        /// <param name="screenPosition">Screen position to check</param>
        /// <returns>Input zone (Left or Right half)</returns>
        private InputZone DetermineInputZone(Vector2 screenPosition)
        {
            float screenWidthDivision = Screen.width * _screenDivision;

            if (screenPosition.x < screenWidthDivision)
            {
                return InputZone.LeftHalf;
            }
            else
            {
                return InputZone.RightHalf;
            }
        }

        /// <summary>
        /// Check if a screen position is in the left zone (movement).
        /// </summary>
        public bool IsInMovementZone(Vector2 screenPosition)
        {
            return DetermineInputZone(screenPosition) == InputZone.LeftHalf;
        }

        /// <summary>
        /// Check if a screen position is in the right zone (future: actions).
        /// </summary>
        public bool IsInActionZone(Vector2 screenPosition)
        {
            return DetermineInputZone(screenPosition) == InputZone.RightHalf;
        }
        #endregion

        #region Enable/Disable
        /// <summary>
        /// Enable input processing.
        /// </summary>
        private void EnableInput()
        {
            // Input is always active in Phase 1
        }

        /// <summary>
        /// Disable input processing.
        /// </summary>
        private void DisableInput()
        {
            _isTouchActive = false;
            _currentTouchZone = InputZone.None;
        }

        /// <summary>
        /// Public method to disable input (for game state changes).
        /// </summary>
        public void SetInputEnabled(bool enabled)
        {
            if (!enabled)
            {
                DisableInput();
            }
        }
        #endregion

        #region Debug Visualization
#if UNITY_EDITOR
        private void OnGUI()
        {
            if (!_showDebugZones) return;

            // Draw input zone divider
            float divisionX = Screen.width * _screenDivision;

            // Left zone (Movement)
            GUI.color = new Color(0, 1, 0, 0.2f);
            GUI.Box(new Rect(0, 0, divisionX, Screen.height), "MOVEMENT ZONE");

            // Right zone (Actions)
            GUI.color = new Color(1, 0, 0, 0.2f);
            GUI.Box(new Rect(divisionX, 0, Screen.width - divisionX, Screen.height), "ACTION ZONE");

            // Show current touch
            if (_isTouchActive)
            {
                GUI.color = Color.yellow;
                GUI.Box(new Rect(_currentTouchPosition.x - 25, Screen.height - _currentTouchPosition.y - 25, 50, 50), "");
            }

            GUI.color = Color.white;
        }
#endif
        #endregion
    }
}
