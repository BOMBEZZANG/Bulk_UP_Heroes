using UnityEngine;
using UnityEngine.Events;
using BulkUpHeroes.Utils;

namespace BulkUpHeroes.Enemies
{
    /// <summary>
    /// Manages enemy statistics and health.
    /// Implements IDamageable for combat system integration.
    /// Implements IStatsModifiable for stat access by CombatManager.
    ///
    /// Phase 2: Added IStatsModifiable for combat system
    /// Future: Will include stat variations based on enemy type and wave difficulty
    /// </summary>
    public class EnemyStats : MonoBehaviour, IDamageable, IStatsModifiable
    {
        #region Events
        [System.Serializable]
        public class EnemyDeathEvent : UnityEvent { }

        public EnemyDeathEvent OnEnemyDeath;
        #endregion

        #region Properties
        [Header("Enemy Stats")]
        [SerializeField] private EnemyType _enemyType = EnemyType.BasicMelee;
        [SerializeField] private float _currentHealth;
        [SerializeField] private float _maxHealth;
        [SerializeField] private float _damage;
        [SerializeField] private float _moveSpeed;
        [SerializeField] private float _attackSpeed = 1f;

        // Public read-only access
        public EnemyType EnemyType => _enemyType;
        public float CurrentHealth => _currentHealth;
        public float MaxHealth => _maxHealth;
        public float Damage => _damage;
        public float MoveSpeed => _moveSpeed;
        public float AttackSpeed => _attackSpeed;
        public bool IsAlive => _currentHealth > 0;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            InitializeStats();
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Initialize enemy stats based on type.
        /// </summary>
        private void InitializeStats()
        {
            // Phase 1: All enemies use basic stats
            // Future: Stats will vary based on _enemyType
            _maxHealth = GameConstants.ENEMY_BASE_HEALTH;
            _currentHealth = _maxHealth;
            _damage = GameConstants.ENEMY_BASE_DAMAGE;
            _moveSpeed = GameConstants.ENEMY_BASE_SPEED;

            Debug.Log($"[EnemyStats] Initialized {_enemyType} - Health: {_maxHealth}, Damage: {_damage}");
        }

        /// <summary>
        /// Initialize with custom stats (for pooling system).
        /// </summary>
        /// <param name="enemyType">Type of enemy</param>
        /// <param name="waveMultiplier">Difficulty multiplier based on wave (future)</param>
        public void Initialize(EnemyType enemyType, float waveMultiplier = 1f)
        {
            _enemyType = enemyType;

            // Apply base stats
            switch (_enemyType)
            {
                case EnemyType.BasicMelee:
                    _maxHealth = GameConstants.ENEMY_BASE_HEALTH;
                    _damage = GameConstants.ENEMY_BASE_DAMAGE;
                    _moveSpeed = GameConstants.ENEMY_BASE_SPEED;
                    break;

                // Future enemy types
                case EnemyType.FastMelee:
                    _maxHealth = GameConstants.ENEMY_BASE_HEALTH * 0.7f;
                    _damage = GameConstants.ENEMY_BASE_DAMAGE * 0.8f;
                    _moveSpeed = GameConstants.ENEMY_BASE_SPEED * 1.5f;
                    break;

                case EnemyType.HeavyMelee:
                    _maxHealth = GameConstants.ENEMY_BASE_HEALTH * 2f;
                    _damage = GameConstants.ENEMY_BASE_DAMAGE * 1.5f;
                    _moveSpeed = GameConstants.ENEMY_BASE_SPEED * 0.6f;
                    break;

                case EnemyType.Ranged:
                    _maxHealth = GameConstants.ENEMY_BASE_HEALTH * 0.6f;
                    _damage = GameConstants.ENEMY_BASE_DAMAGE * 1.2f;
                    _moveSpeed = GameConstants.ENEMY_BASE_SPEED * 0.8f;
                    break;
            }

            // Apply wave multiplier (future scaling)
            _maxHealth *= waveMultiplier;
            _damage *= waveMultiplier;

            _currentHealth = _maxHealth;
        }

        /// <summary>
        /// Initialize with exact stat values (for wave-based spawning).
        /// </summary>
        public void InitializeWithStats(EnemyType enemyType, float health, float damage, float speed)
        {
            _enemyType = enemyType;
            _maxHealth = health;
            _damage = damage;
            _moveSpeed = speed;
            _currentHealth = _maxHealth;
        }

        /// <summary>
        /// Reset stats to initial values (for object pooling).
        /// </summary>
        public void ResetStats()
        {
            InitializeStats();
        }
        #endregion

        #region IDamageable Implementation
        /// <summary>
        /// Apply damage to the enemy.
        /// </summary>
        /// <param name="amount">Amount of damage to receive</param>
        /// <param name="source">Source of damage (optional)</param>
        public void TakeDamage(float amount, GameObject source = null)
        {
            if (!IsAlive) return;

            _currentHealth = Mathf.Max(0, _currentHealth - amount);

            Debug.Log($"[EnemyStats] Took {amount} damage. Health: {_currentHealth}/{_maxHealth}");

            // Check for death
            if (_currentHealth <= 0)
            {
                Die(source);
            }
        }
        #endregion

        #region Death Handling
        /// <summary>
        /// Handle enemy death.
        /// </summary>
        /// <param name="killer">GameObject that killed this enemy</param>
        private void Die(GameObject killer = null)
        {
            Debug.Log($"[EnemyStats] {_enemyType} died!");

            // Notify listeners
            OnEnemyDeath?.Invoke();

            // Register defeat with GameManager
            if (Core.GameManager.Instance != null)
            {
                Core.GameManager.Instance.RegisterEnemyDefeated();
            }

            // Phase 1: Destroy enemy
            // Future: Return to pool and spawn part pickup
            HandleDeath();
        }

        /// <summary>
        /// Handle post-death logic.
        /// </summary>
        private void HandleDeath()
        {
            // Phase 2: DamageHandler handles death animation and pooling
            // This method is now just for triggering events
            // DamageHandler will return the enemy to pool after death animation

            // Future: Spawn part pickup at death location

            // Note: Do NOT destroy the GameObject here!
            // DamageHandler manages the death sequence in Phase 2
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Get health as a percentage (0-1).
        /// </summary>
        public float GetHealthPercent()
        {
            return _maxHealth > 0 ? _currentHealth / _maxHealth : 0f;
        }

        /// <summary>
        /// Check if enemy is at full health.
        /// </summary>
        public bool IsAtFullHealth()
        {
            return Mathf.Approximately(_currentHealth, _maxHealth);
        }
        #endregion

        #region IStatsModifiable Implementation
        /// <summary>
        /// Get current value of a stat by name.
        /// </summary>
        public float GetStat(string statType)
        {
            switch (statType)
            {
                case "Health":
                case "CurrentHealth":
                    return _currentHealth;

                case "MaxHealth":
                    return _maxHealth;

                case "Damage":
                    return _damage;

                case "MoveSpeed":
                case "Speed":
                    return _moveSpeed;

                case "AttackSpeed":
                    return _attackSpeed;

                default:
                    Debug.LogWarning($"[EnemyStats] Unknown stat type: {statType}");
                    return 0f;
            }
        }

        /// <summary>
        /// Modify a stat by adding the given value.
        /// </summary>
        public void ModifyStat(string statType, float value)
        {
            switch (statType)
            {
                case "MaxHealth":
                    _maxHealth += value;
                    _maxHealth = Mathf.Max(1f, _maxHealth);
                    break;

                case "Damage":
                    _damage += value;
                    _damage = Mathf.Max(0f, _damage);
                    break;

                case "MoveSpeed":
                case "Speed":
                    _moveSpeed += value;
                    _moveSpeed = Mathf.Max(0.1f, _moveSpeed);
                    break;

                case "AttackSpeed":
                    _attackSpeed += value;
                    _attackSpeed = Mathf.Max(0.1f, _attackSpeed);
                    break;

                default:
                    Debug.LogWarning($"[EnemyStats] Cannot modify stat: {statType}");
                    break;
            }
        }
        #endregion

        #region Debug Helpers
#if UNITY_EDITOR
        [ContextMenu("Take 20 Damage")]
        private void DebugTakeDamage()
        {
            TakeDamage(20f);
        }

        [ContextMenu("Kill Enemy")]
        private void DebugKill()
        {
            TakeDamage(_currentHealth);
        }

        [ContextMenu("Reset Stats")]
        private void DebugResetStats()
        {
            ResetStats();
        }
#endif
        #endregion
    }
}
