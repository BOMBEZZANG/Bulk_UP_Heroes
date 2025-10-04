using UnityEngine;
using BulkUpHeroes.Utils;

namespace BulkUpHeroes.Enemies
{
    /// <summary>
    /// Basic enemy AI that chases the player.
    /// Implements simple state machine for enemy behavior.
    /// Works with CombatManager for actual combat behavior.
    ///
    /// Phase 2: Chase behavior with CombatManager integration
    /// Future: Will include varied behaviors, formations, and smarter pathfinding
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(EnemyStats))]
    public class EnemyAI : MonoBehaviour, IPoolable
    {
        #region Components
        private Rigidbody _rigidbody;
        private EnemyStats _enemyStats;
        #endregion

        #region AI Properties
        [Header("AI State")]
        [SerializeField] private AIState _currentState = AIState.Idle;

        [Header("Target Tracking")]
        [SerializeField] private Transform _targetPlayer;
        [SerializeField] private float _targetUpdateInterval = 0.2f;
        private float _targetUpdateTimer = 0f;

        [Header("Movement")]
        [SerializeField] private float _rotationSpeed = 360f;
        private Vector3 _moveDirection = Vector3.zero;

        [Header("Attack Settings (Phase 2)")]
        [SerializeField] private float _attackRange = 1f;
        [SerializeField] private float _attackCooldown = 1f;
        private float _lastAttackTime = -999f;
        #endregion

        #region Properties
        public AIState CurrentState => _currentState;
        public GameObject GameObject => gameObject;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            InitializeComponents();
        }

        // Note: Start() removed - initialization handled by OnSpawnFromPool() for pooled objects

        private void Update()
        {
            if (!_enemyStats.IsAlive)
            {
                if (_currentState != AIState.Dead)
                {
                    ChangeState(AIState.Dead);
                }
                return;
            }

            UpdateTargetTracking();
            UpdateAIBehavior();
        }

        private void FixedUpdate()
        {
            if (_enemyStats.IsAlive)
            {
                ApplyMovement();
            }
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

            _enemyStats = GetComponent<EnemyStats>();

            // Set attack range from constants
            _attackRange = GameConstants.ENEMY_ATTACK_RANGE;

            Debug.Log("[EnemyAI] Initialized");
        }

        /// <summary>
        /// Find and cache reference to player.
        /// </summary>
        private void FindPlayer()
        {
            // Use PlayerManager for cached reference (avoids expensive FindGameObjectWithTag)
            if (Player.PlayerManager.HasPlayer())
            {
                _targetPlayer = Player.PlayerManager.PlayerTransform;
                Debug.Log("[EnemyAI] Player target acquired");
            }
            else
            {
                Debug.LogWarning("[EnemyAI] Player not found!");
            }
        }
        #endregion

        #region AI State Machine
        /// <summary>
        /// Change AI state.
        /// </summary>
        private void ChangeState(AIState newState)
        {
            if (_currentState == newState) return;

            Debug.Log($"[EnemyAI] State change: {_currentState} -> {newState}");

            _currentState = newState;

            // Handle state entry logic
            OnStateEnter(newState);
        }

        /// <summary>
        /// Called when entering a new state.
        /// </summary>
        private void OnStateEnter(AIState state)
        {
            switch (state)
            {
                case AIState.Idle:
                    _moveDirection = Vector3.zero;
                    break;

                case AIState.Chasing:
                    // No special setup needed
                    break;

                case AIState.Attacking:
                    _moveDirection = Vector3.zero;
                    break;

                case AIState.Dead:
                    _moveDirection = Vector3.zero;
                    _rigidbody.linearVelocity = Vector3.zero;
                    break;
            }
        }

        /// <summary>
        /// Update AI behavior based on current state.
        /// </summary>
        private void UpdateAIBehavior()
        {
            switch (_currentState)
            {
                case AIState.Idle:
                    UpdateIdleState();
                    break;

                case AIState.Chasing:
                    UpdateChasingState();
                    break;

                case AIState.Attacking:
                    UpdateAttackingState();
                    break;

                case AIState.Dead:
                    // No behavior when dead
                    break;
            }
        }
        #endregion

        #region AI States
        /// <summary>
        /// Update idle state behavior.
        /// </summary>
        private void UpdateIdleState()
        {
            // Phase 1: Immediately start chasing if player exists
            if (_targetPlayer != null)
            {
                ChangeState(AIState.Chasing);
            }
        }

        /// <summary>
        /// Update chasing state behavior.
        /// </summary>
        private void UpdateChasingState()
        {
            if (_targetPlayer == null)
            {
                ChangeState(AIState.Idle);
                return;
            }

            // Calculate direction to player
            Vector3 directionToPlayer = (_targetPlayer.position - transform.position);
            directionToPlayer.y = 0; // Keep movement on horizontal plane
            float distanceToPlayer = directionToPlayer.magnitude;

            // Check if in attack range
            if (distanceToPlayer <= _attackRange)
            {
                // Stop moving when in range - CombatManager handles the attacks
                _moveDirection = Vector3.zero;
            }
            else
            {
                // Move toward player
                _moveDirection = directionToPlayer.normalized;
            }

            // Rotate to face player
            RotateTowardsTarget(_targetPlayer.position);
        }

        /// <summary>
        /// Update attacking state behavior.
        /// Note: Actual combat is handled by CombatManager component.
        /// This state is reserved for future attack animations.
        /// </summary>
        private void UpdateAttackingState()
        {
            // CombatManager handles actual attacks
            // This state reserved for future attack animations
            // For now, return to chasing
            ChangeState(AIState.Chasing);
        }
        #endregion

        #region Target Tracking
        /// <summary>
        /// Periodically update target tracking to improve performance.
        /// </summary>
        private void UpdateTargetTracking()
        {
            _targetUpdateTimer -= Time.deltaTime;

            if (_targetUpdateTimer <= 0)
            {
                _targetUpdateTimer = _targetUpdateInterval;

                // Re-find player if lost
                if (_targetPlayer == null)
                {
                    FindPlayer();
                }
            }
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
                _rigidbody.linearVelocity = Vector3.zero;
                return;
            }

            // Calculate target velocity
            float currentSpeed = _enemyStats != null ? _enemyStats.MoveSpeed : GameConstants.ENEMY_BASE_SPEED;
            Vector3 targetVelocity = _moveDirection * currentSpeed;
            targetVelocity.y = _rigidbody.linearVelocity.y; // Preserve vertical velocity

            // Apply movement
            _rigidbody.linearVelocity = targetVelocity;
        }

        /// <summary>
        /// Rotate smoothly towards target position.
        /// </summary>
        private void RotateTowardsTarget(Vector3 targetPosition)
        {
            Vector3 direction = (targetPosition - transform.position);
            direction.y = 0;

            if (direction.magnitude < 0.1f) return;

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            float rotationStep = _rotationSpeed * Time.deltaTime;

            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                rotationStep
            );
        }
        #endregion

        #region IPoolable Implementation
        /// <summary>
        /// Called when spawned from object pool.
        /// </summary>
        public void OnSpawnFromPool()
        {
            // Reset state
            ChangeState(AIState.Idle);
            _moveDirection = Vector3.zero;
            _rigidbody.linearVelocity = Vector3.zero;

            // Find player
            FindPlayer();

            // Start chasing
            ChangeState(AIState.Chasing);

            Debug.Log("[EnemyAI] Spawned from pool");
        }

        /// <summary>
        /// Called when returned to object pool.
        /// </summary>
        public void OnReturnToPool()
        {
            // Reset state
            ChangeState(AIState.Idle);
            _moveDirection = Vector3.zero;
            _rigidbody.linearVelocity = Vector3.zero;
            _targetPlayer = null;

            Debug.Log("[EnemyAI] Returned to pool");
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Set the enemy's target.
        /// </summary>
        public void SetTarget(Transform target)
        {
            _targetPlayer = target;
        }

        /// <summary>
        /// Get distance to current target.
        /// </summary>
        public float GetDistanceToTarget()
        {
            if (_targetPlayer == null) return float.MaxValue;

            return Vector3.Distance(transform.position, _targetPlayer.position);
        }

        /// <summary>
        /// Force stop all movement.
        /// </summary>
        public void StopMovement()
        {
            _moveDirection = Vector3.zero;
            _rigidbody.linearVelocity = Vector3.zero;
        }
        #endregion

        #region Debug Helpers
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            // Visualize state
            Gizmos.color = GetStateColor();
            Gizmos.DrawWireSphere(transform.position, 0.5f);

            // Visualize attack range
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _attackRange);

            // Visualize direction to player
            if (_targetPlayer != null && Application.isPlaying)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.position, _targetPlayer.position);
            }

            // Visualize movement direction
            if (_moveDirection != Vector3.zero)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawRay(transform.position, _moveDirection * 2f);
            }
        }

        private Color GetStateColor()
        {
            switch (_currentState)
            {
                case AIState.Idle: return Color.gray;
                case AIState.Chasing: return Color.yellow;
                case AIState.Attacking: return Color.red;
                case AIState.Dead: return Color.black;
                default: return Color.white;
            }
        }

        [ContextMenu("Force Chase Player")]
        private void DebugChasePlayer()
        {
            FindPlayer();
            ChangeState(AIState.Chasing);
        }
#endif
        #endregion
    }
}
