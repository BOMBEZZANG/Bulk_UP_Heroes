using UnityEngine;
using BulkUpHeroes.Utils;
using BulkUpHeroes.UI;

namespace BulkUpHeroes.Player
{
    /// <summary>
    /// Handles player movement, rotation, and input processing.
    /// Uses virtual joystick for mobile touch controls.
    ///
    /// Phase 1: Basic movement with touch controls
    /// Future: Will integrate combat abilities and special moves
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(PlayerStats))]
    public class PlayerController : MonoBehaviour
    {
        #region Components
        private Rigidbody _rigidbody;
        private PlayerStats _playerStats;
        private VirtualJoystick _virtualJoystick;
        #endregion

        #region Movement Properties
        [Header("Movement Settings")]
        [SerializeField] private float _rotationSpeed = 720f; // Degrees per second

        private Vector3 _moveDirection = Vector3.zero;
        private float _currentSpeed;
        #endregion

        #region Arena Boundaries
        [Header("Arena Boundaries")]
        [SerializeField] private bool _clampToArena = true;
        private float _arenaHalfSize;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            InitializeComponents();
            InitializeBoundaries();
        }

        private void Start()
        {
            FindVirtualJoystick();
        }

        private void Update()
        {
            ProcessInput();
            UpdateRotation();
        }

        private void FixedUpdate()
        {
            ApplyMovement();
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Initialize required components.
        /// </summary>
        private void InitializeComponents()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            _rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
            _rigidbody.interpolation = RigidbodyInterpolation.Interpolate;

            _playerStats = GetComponent<PlayerStats>();

            if (_playerStats == null)
            {
                Debug.LogError("[PlayerController] PlayerStats component missing!");
            }

            Debug.Log("[PlayerController] Initialized");
        }

        /// <summary>
        /// Initialize arena boundary settings.
        /// </summary>
        private void InitializeBoundaries()
        {
            _arenaHalfSize = GameConstants.ARENA_HALF_SIZE - 0.5f; // Small margin
        }

        /// <summary>
        /// Find and cache reference to VirtualJoystick.
        /// </summary>
        private void FindVirtualJoystick()
        {
            _virtualJoystick = FindObjectOfType<VirtualJoystick>();

            if (_virtualJoystick == null)
            {
                Debug.LogWarning("[PlayerController] VirtualJoystick not found in scene!");
            }
        }
        #endregion

        #region Input Processing
        /// <summary>
        /// Process input from virtual joystick.
        /// </summary>
        private void ProcessInput()
        {
            // Don't process input if player is dead
            if (_playerStats != null && !_playerStats.IsAlive)
            {
                _moveDirection = Vector3.zero;
                return;
            }

            // Get input from virtual joystick
            if (_virtualJoystick != null)
            {
                Vector2 input = _virtualJoystick.InputDirection;

                // Convert 2D input to 3D movement direction
                _moveDirection = new Vector3(input.x, 0f, input.y);

                // Apply dead zone
                if (_moveDirection.magnitude < GameConstants.JOYSTICK_DEAD_ZONE)
                {
                    _moveDirection = Vector3.zero;
                }
                else
                {
                    _moveDirection.Normalize();
                }
            }
            else
            {
                _moveDirection = Vector3.zero;
            }

            // Get current speed from stats
            _currentSpeed = _playerStats != null ? _playerStats.MoveSpeed : GameConstants.PLAYER_BASE_SPEED;
        }
        #endregion

        #region Movement
        /// <summary>
        /// Apply movement to rigidbody.
        /// </summary>
        private void ApplyMovement()
        {
            if (_moveDirection == Vector3.zero)
            {
                // Stop movement
                _rigidbody.linearVelocity = Vector3.zero;
                return;
            }

            // Calculate target velocity
            Vector3 targetVelocity = _moveDirection * _currentSpeed;
            targetVelocity.y = _rigidbody.linearVelocity.y; // Preserve vertical velocity

            // Apply movement
            _rigidbody.linearVelocity = targetVelocity;

            // Clamp position to arena boundaries
            if (_clampToArena)
            {
                ClampPositionToArena();
            }
        }

        /// <summary>
        /// Clamp player position within arena boundaries.
        /// </summary>
        private void ClampPositionToArena()
        {
            Vector3 position = transform.position;

            position.x = Mathf.Clamp(position.x, -_arenaHalfSize, _arenaHalfSize);
            position.z = Mathf.Clamp(position.z, -_arenaHalfSize, _arenaHalfSize);

            transform.position = position;
        }
        #endregion

        #region Rotation
        /// <summary>
        /// Update player rotation to face movement direction.
        /// </summary>
        private void UpdateRotation()
        {
            if (_moveDirection == Vector3.zero) return;

            // Calculate target rotation
            Quaternion targetRotation = Quaternion.LookRotation(_moveDirection);

            // Smoothly rotate towards target
            float rotationStep = _rotationSpeed * Time.deltaTime;
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                rotationStep
            );
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Get current movement direction.
        /// </summary>
        public Vector3 GetMoveDirection()
        {
            return _moveDirection;
        }

        /// <summary>
        /// Check if player is currently moving.
        /// </summary>
        public bool IsMoving()
        {
            return _moveDirection.magnitude > GameConstants.JOYSTICK_DEAD_ZONE;
        }

        /// <summary>
        /// Stop player movement immediately.
        /// </summary>
        public void StopMovement()
        {
            _moveDirection = Vector3.zero;
            _rigidbody.linearVelocity = Vector3.zero;
        }

        /// <summary>
        /// Teleport player to a specific position.
        /// </summary>
        /// <param name="position">Target position</param>
        public void Teleport(Vector3 position)
        {
            transform.position = position;
            _rigidbody.linearVelocity = Vector3.zero;
        }
        #endregion

        #region Collision Handling
        private void OnCollisionEnter(Collision collision)
        {
            // Phase 1: Basic collision logging
            // Future: Handle specific collision types
            if (collision.gameObject.CompareTag(GameConstants.TAG_WALL))
            {
                // Wall collision - movement will be stopped by physics
                Debug.Log("[PlayerController] Hit wall");
            }
        }

        private void OnCollisionStay(Collision collision)
        {
            // Handle continuous collision with enemies (Phase 2: Combat)
            if (collision.gameObject.CompareTag(GameConstants.TAG_ENEMY))
            {
                // Future: Take damage from enemy contact
            }
        }
        #endregion

        #region Debug Helpers
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            // Visualize arena boundaries
            if (_clampToArena)
            {
                Gizmos.color = Color.cyan;
                float halfSize = GameConstants.ARENA_HALF_SIZE - 0.5f;
                Vector3 center = new Vector3(0, 0.1f, 0);
                Vector3 size = new Vector3(halfSize * 2, 0.1f, halfSize * 2);
                Gizmos.DrawWireCube(center, size);
            }

            // Visualize movement direction
            if (Application.isPlaying && _moveDirection != Vector3.zero)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawRay(transform.position, _moveDirection * 2f);
            }
        }

        [ContextMenu("Teleport to Center")]
        private void DebugTeleportCenter()
        {
            Teleport(new Vector3(0, GameConstants.PLAYER_SPAWN_HEIGHT, 0));
        }

        [ContextMenu("Stop Movement")]
        private void DebugStopMovement()
        {
            StopMovement();
        }
#endif
        #endregion
    }
}
