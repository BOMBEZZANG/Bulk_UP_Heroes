using UnityEngine;
using BulkUpHeroes.Utils;
using BulkUpHeroes.Core;

namespace BulkUpHeroes.Combat
{
    /// <summary>
    /// Manages automatic combat for both player and enemies.
    /// Handles target detection, attack timing, and damage application.
    ///
    /// Phase 2: Auto-attack with instant damage
    /// Future: Will support different attack types, abilities, and effects
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class CombatManager : MonoBehaviour, IAttacker
    {
        #region Combat Settings
        [Header("Combat Configuration")]
        [SerializeField] private float _attackRange = 1.5f;
        [SerializeField] private float _targetUpdateInterval = 0.2f;
        [SerializeField] private LayerMask _targetLayer;

        [Header("Visual Feedback")]
        [SerializeField] private bool _enableVisualFeedback = true;
        [SerializeField] private float _attackScalePunch = 1.2f;
        [SerializeField] private float _attackAnimDuration = 0.2f;
        #endregion

        #region Components
        private IStatsModifiable _stats;
        private IDamageable _selfDamageable;
        private Transform _transform;
        #endregion

        #region Combat State
        private GameObject _currentTarget;
        private IDamageable _currentTargetDamageable;
        private float _attackTimer = 0f;
        private float _targetUpdateTimer = 0f;
        private bool _isAttacking = false;
        private bool _isPlayer = false;

        // Animation state
        private Vector3 _originalScale;
        private float _attackAnimTimer = 0f;
        #endregion

        #region Properties (IAttacker)
        public bool CanAttack => _currentTarget != null && _attackTimer <= 0f;
        public float Damage => _stats != null ? _stats.GetStat("Damage") : 10f;
        public float AttackSpeed => _stats != null ? _stats.GetStat("AttackSpeed") : 1f;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            InitializeComponents();
            SetupCombat();
        }

        private void Update()
        {
            if (!IsAlive()) return;

            UpdateTargetSearch();
            UpdateAttackTiming();
            UpdateAttackAnimation();
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Initialize required components.
        /// </summary>
        private void InitializeComponents()
        {
            _transform = transform;
            _originalScale = _transform.localScale;

            // Get stats component (works for both Player and Enemy)
            _stats = GetComponent<IStatsModifiable>();
            _selfDamageable = GetComponent<IDamageable>();

            if (_stats == null)
            {
                Debug.LogError($"[CombatManager] No stats component found on {gameObject.name}!");
            }
        }

        /// <summary>
        /// Setup combat based on GameObject type.
        /// </summary>
        private void SetupCombat()
        {
            // Determine if this is player or enemy
            _isPlayer = gameObject.CompareTag(GameConstants.TAG_PLAYER);

            // Set appropriate target layer using cached LayerMasks
            if (_isPlayer)
            {
                _targetLayer = GameConstants.EnemyLayerMask;
                _attackRange = GameConstants.ATTACK_RANGE;
            }
            else
            {
                _targetLayer = GameConstants.PlayerLayerMask;
                _attackRange = GameConstants.ENEMY_ATTACK_RANGE;
            }

            Debug.Log($"[CombatManager] {gameObject.name} initialized - Range: {_attackRange}, IsPlayer: {_isPlayer}");
        }
        #endregion

        #region Target Detection
        /// <summary>
        /// Update target search on interval.
        /// </summary>
        private void UpdateTargetSearch()
        {
            _targetUpdateTimer -= Time.deltaTime;

            if (_targetUpdateTimer <= 0f)
            {
                _targetUpdateTimer = _targetUpdateInterval;
                FindNearestTarget();
            }

            // Validate current target
            if (_currentTarget != null)
            {
                if (!_currentTarget.activeInHierarchy ||
                    (_currentTargetDamageable != null && !_currentTargetDamageable.IsAlive))
                {
                    ClearTarget();
                }
            }
        }

        /// <summary>
        /// Find the nearest valid target within range.
        /// </summary>
        private void FindNearestTarget()
        {
            Collider[] hits = Physics.OverlapSphere(_transform.position, _attackRange, _targetLayer);

            if (hits.Length == 0)
            {
                ClearTarget();
                return;
            }

            // Find nearest target
            GameObject nearestTarget = null;
            float nearestDistance = float.MaxValue;

            foreach (Collider hit in hits)
            {
                IDamageable damageable = hit.GetComponent<IDamageable>();
                if (damageable != null && damageable.IsAlive)
                {
                    float distance = Vector3.Distance(_transform.position, hit.transform.position);
                    if (distance < nearestDistance)
                    {
                        nearestDistance = distance;
                        nearestTarget = hit.gameObject;
                    }
                }
            }

            if (nearestTarget != _currentTarget)
            {
                SetTarget(nearestTarget);
            }
        }

        /// <summary>
        /// Set new combat target.
        /// </summary>
        private void SetTarget(GameObject target)
        {
            _currentTarget = target;

            if (target != null)
            {
                _currentTargetDamageable = target.GetComponent<IDamageable>();
            }
            else
            {
                _currentTargetDamageable = null;
            }
        }

        /// <summary>
        /// Clear current target.
        /// </summary>
        private void ClearTarget()
        {
            _currentTarget = null;
            _currentTargetDamageable = null;
        }
        #endregion

        #region Attack System
        /// <summary>
        /// Update attack timing and execute attacks.
        /// </summary>
        private void UpdateAttackTiming()
        {
            _attackTimer -= Time.deltaTime;

            if (CanAttack)
            {
                Attack(_currentTargetDamageable);
            }
        }

        /// <summary>
        /// Execute attack on target.
        /// </summary>
        public void Attack(IDamageable target)
        {
            if (target == null || !target.IsAlive) return;

            // Apply damage through DamageHandler for visual feedback
            float damageAmount = Damage;

            // Try to find DamageHandler on target
            DamageHandler damageHandler = _currentTarget.GetComponent<DamageHandler>();
            if (damageHandler != null)
            {
                // Use DamageHandler for proper death handling and visual effects
                damageHandler.ProcessDamage(damageAmount, gameObject);
            }
            else
            {
                // Fallback to direct damage (shouldn't happen in Phase 2)
                target.TakeDamage(damageAmount, gameObject);
                Debug.LogWarning($"[CombatManager] No DamageHandler on {_currentTarget.name}!");
            }

            // Reset attack timer
            float attackSpeed = AttackSpeed;
            _attackTimer = attackSpeed > 0 ? 1f / attackSpeed : 1f;

            // Visual feedback
            if (_enableVisualFeedback)
            {
                PlayAttackAnimation();
            }

            // Trigger events
            if (_isPlayer)
            {
                GameEvents.TriggerPlayerAttack(_currentTarget);
            }
            else
            {
                GameEvents.TriggerEnemyAttack(_currentTarget);
            }

            Debug.Log($"[CombatManager] {gameObject.name} attacked {_currentTarget.name} for {damageAmount} damage");
        }
        #endregion

        #region Visual Feedback
        /// <summary>
        /// Play attack animation (scale punch).
        /// </summary>
        private void PlayAttackAnimation()
        {
            _isAttacking = true;
            _attackAnimTimer = _attackAnimDuration;
        }

        /// <summary>
        /// Update attack animation.
        /// </summary>
        private void UpdateAttackAnimation()
        {
            if (!_isAttacking) return;

            _attackAnimTimer -= Time.deltaTime;

            if (_attackAnimTimer <= 0f)
            {
                // Animation complete
                _transform.localScale = _originalScale;
                _isAttacking = false;
            }
            else
            {
                // Animate scale punch
                float progress = 1f - (_attackAnimTimer / _attackAnimDuration);
                float scale = Mathf.Lerp(1f, _attackScalePunch, Mathf.Sin(progress * Mathf.PI));
                _transform.localScale = _originalScale * scale;
            }
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Check if this entity is alive.
        /// </summary>
        private bool IsAlive()
        {
            return _selfDamageable != null && _selfDamageable.IsAlive;
        }

        /// <summary>
        /// Get current target (for debugging).
        /// </summary>
        public GameObject GetCurrentTarget()
        {
            return _currentTarget;
        }

        /// <summary>
        /// Check if currently has a target.
        /// </summary>
        public bool HasTarget()
        {
            return _currentTarget != null;
        }
        #endregion

        #region Debug Helpers
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            // Visualize attack range
            Gizmos.color = _currentTarget != null ? Color.red : Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _attackRange);

            // Draw line to current target
            if (_currentTarget != null && Application.isPlaying)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, _currentTarget.transform.position);
            }
        }
#endif
        #endregion
    }
}
