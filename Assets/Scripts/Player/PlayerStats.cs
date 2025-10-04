using UnityEngine;
using UnityEngine.Events;
using BulkUpHeroes.Utils;
using BulkUpHeroes.Core;

namespace BulkUpHeroes.Player
{
    /// <summary>
    /// Manages all player statistics and handles stat modifications.
    /// Implements IDamageable for damage system and IStatsModifiable for part bonuses.
    ///
    /// Phase 1: Basic health and movement stats
    /// Future: Will track all stat bonuses from equipped parts
    /// </summary>
    public class PlayerStats : MonoBehaviour, IDamageable, IStatsModifiable
    {
        #region Events
        [System.Serializable]
        public class HealthChangedEvent : UnityEvent<float, float> { } // current, max
        [System.Serializable]
        public class PlayerDeathEvent : UnityEvent { }

        public HealthChangedEvent OnHealthChanged;
        public PlayerDeathEvent OnPlayerDeath;
        #endregion

        #region Properties
        [Header("Health Stats")]
        [SerializeField] private float _currentHealth;
        [SerializeField] private float _maxHealth;

        [Header("Combat Stats")]
        [SerializeField] private float _damage;
        [SerializeField] private float _attackSpeed;

        [Header("Movement Stats")]
        [SerializeField] private float _moveSpeed;

        // Public read-only access
        public float CurrentHealth => _currentHealth;
        public float MaxHealth => _maxHealth;
        public float Damage => _damage;
        public float AttackSpeed => _attackSpeed;
        public float MoveSpeed => _moveSpeed;
        public bool IsAlive => _currentHealth > 0;

        // Stat modifiers from equipped parts (Future: Phase 3)
        private float _healthModifier = 0f;
        private float _damageModifier = 0f;
        private float _attackSpeedModifier = 0f;
        private float _moveSpeedModifier = 0f;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            InitializeStats();
        }

        private void Start()
        {
            // Pre-warm health event system by triggering multiple times
            // This forces all subscribers to initialize
            StartCoroutine(PrewarmHealthEvents());
        }

        /// <summary>
        /// Pre-warm health event system to avoid first-damage lag.
        /// </summary>
        private System.Collections.IEnumerator PrewarmHealthEvents()
        {
            // Wait for end of frame to ensure all UI systems are ready
            yield return new WaitForEndOfFrame();

            // Trigger health events multiple times with different values
            // This forces TextMeshPro, UI, and other systems to initialize
            for (int i = 0; i < 3; i++)
            {
                OnHealthChanged?.Invoke(_maxHealth, _maxHealth);
                GameEvents.TriggerPlayerHealthChanged(_maxHealth, _maxHealth);

                OnHealthChanged?.Invoke(_maxHealth * 0.5f, _maxHealth);
                GameEvents.TriggerPlayerHealthChanged(_maxHealth * 0.5f, _maxHealth);

                OnHealthChanged?.Invoke(_maxHealth * 0.25f, _maxHealth);
                GameEvents.TriggerPlayerHealthChanged(_maxHealth * 0.25f, _maxHealth);
            }

            // Final event with actual current health
            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
            GameEvents.TriggerPlayerHealthChanged(_currentHealth, _maxHealth);

            Debug.Log($"[PlayerStats] Health event system pre-warmed");
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Initialize player stats to base values from GameConstants.
        /// </summary>
        private void InitializeStats()
        {
            _maxHealth = GameConstants.PLAYER_BASE_HEALTH;
            _currentHealth = _maxHealth;
            _damage = GameConstants.PLAYER_BASE_DAMAGE;
            _attackSpeed = GameConstants.PLAYER_BASE_ATTACK_SPEED;
            _moveSpeed = GameConstants.PLAYER_BASE_SPEED;

            Debug.Log($"[PlayerStats] Initialized - Health: {_maxHealth}, Damage: {_damage}, Speed: {_moveSpeed}");
        }

        /// <summary>
        /// Reset all stats to base values.
        /// Used when restarting game.
        /// </summary>
        public void ResetStats()
        {
            _healthModifier = 0f;
            _damageModifier = 0f;
            _attackSpeedModifier = 0f;
            _moveSpeedModifier = 0f;

            InitializeStats();
            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
            GameEvents.TriggerPlayerHealthChanged(_currentHealth, _maxHealth);
        }
        #endregion

        #region IDamageable Implementation
        /// <summary>
        /// Apply damage to the player.
        /// </summary>
        /// <param name="amount">Amount of damage to receive</param>
        /// <param name="source">Source of damage (optional)</param>
        public void TakeDamage(float amount, GameObject source = null)
        {
            if (!IsAlive) return;

            _currentHealth = Mathf.Max(0, _currentHealth - amount);

            Debug.Log($"[PlayerStats] Took {amount} damage. Health: {_currentHealth}/{_maxHealth}");

            // Notify UI
            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
            GameEvents.TriggerPlayerHealthChanged(_currentHealth, _maxHealth);

            // Check for death
            if (_currentHealth <= 0)
            {
                Die();
            }
        }

        /// <summary>
        /// Heal the player.
        /// </summary>
        /// <param name="amount">Amount of health to restore</param>
        public void Heal(float amount)
        {
            if (!IsAlive) return;

            _currentHealth = Mathf.Min(_maxHealth, _currentHealth + amount);

            Debug.Log($"[PlayerStats] Healed {amount}. Health: {_currentHealth}/{_maxHealth}");

            // Notify UI
            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
            GameEvents.TriggerPlayerHealthChanged(_currentHealth, _maxHealth);
        }
        #endregion

        #region IStatsModifiable Implementation
        /// <summary>
        /// Modify a specific stat by a given value.
        /// Used for applying part bonuses.
        /// </summary>
        /// <param name="statType">Type of stat to modify (Health, Damage, AttackSpeed, MoveSpeed)</param>
        /// <param name="value">Value to add (can be negative)</param>
        /// <param name="modifierType">Type of modifier (Flat, PercentAdd, PercentMult)</param>
        public void ModifyStat(string statType, float value, StatModifierType modifierType = StatModifierType.PercentAdd)
        {
            switch (statType.ToLower())
            {
                case "health":
                case "maxhealth":
                    _healthModifier += value;
                    RecalculateMaxHealth();
                    break;

                case "damage":
                    _damageModifier += value;
                    RecalculateDamage();
                    break;

                case "attackspeed":
                    _attackSpeedModifier += value;
                    RecalculateAttackSpeed();
                    break;

                case "movespeed":
                case "speed":
                    _moveSpeedModifier += value;
                    RecalculateMoveSpeed();
                    break;

                default:
                    Debug.LogWarning($"[PlayerStats] Unknown stat type: {statType}");
                    break;
            }
        }

        /// <summary>
        /// Legacy overload for backward compatibility.
        /// </summary>
        public void ModifyStat(string statType, float value)
        {
            ModifyStat(statType, value, StatModifierType.PercentAdd);
        }

        /// <summary>
        /// Get current value of a specific stat.
        /// </summary>
        /// <param name="statType">Type of stat to retrieve</param>
        /// <returns>Current stat value</returns>
        public float GetStat(string statType)
        {
            switch (statType.ToLower())
            {
                case "health":
                    return _currentHealth;
                case "maxhealth":
                    return _maxHealth;
                case "damage":
                    return _damage;
                case "attackspeed":
                    return _attackSpeed;
                case "movespeed":
                case "speed":
                    return _moveSpeed;
                default:
                    Debug.LogWarning($"[PlayerStats] Unknown stat type: {statType}");
                    return 0f;
            }
        }
        #endregion

        #region Stat Recalculation
        /// <summary>
        /// Recalculate max health with modifiers.
        /// Phase 3: Uses multiplicative formula: Base * (1 + modifier)
        /// Maintains health percentage when max health increases.
        /// </summary>
        private void RecalculateMaxHealth()
        {
            float healthPercentage = _maxHealth > 0 ? _currentHealth / _maxHealth : 1f;
            _maxHealth = GameConstants.PLAYER_BASE_HEALTH * (1f + _healthModifier);
            _currentHealth = _maxHealth * healthPercentage;

            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
            GameEvents.TriggerPlayerHealthChanged(_currentHealth, _maxHealth);

            Debug.Log($"[PlayerStats] Max Health updated: {_maxHealth:F1} (Base: {GameConstants.PLAYER_BASE_HEALTH}, Modifier: +{_healthModifier * 100:F0}%)");
        }

        /// <summary>
        /// Recalculate damage with modifiers.
        /// Phase 3: Uses multiplicative formula: Base * (1 + modifier)
        /// </summary>
        private void RecalculateDamage()
        {
            _damage = GameConstants.PLAYER_BASE_DAMAGE * (1f + _damageModifier);
            Debug.Log($"[PlayerStats] Damage updated: {_damage:F1} (Base: {GameConstants.PLAYER_BASE_DAMAGE}, Modifier: +{_damageModifier * 100:F0}%)");
        }

        /// <summary>
        /// Recalculate attack speed with modifiers.
        /// Phase 3: Uses multiplicative formula: Base * (1 + modifier)
        /// </summary>
        private void RecalculateAttackSpeed()
        {
            _attackSpeed = GameConstants.PLAYER_BASE_ATTACK_SPEED * (1f + _attackSpeedModifier);
            Debug.Log($"[PlayerStats] Attack Speed updated: {_attackSpeed:F2} (Base: {GameConstants.PLAYER_BASE_ATTACK_SPEED}, Modifier: +{_attackSpeedModifier * 100:F0}%)");
        }

        /// <summary>
        /// Recalculate move speed with modifiers.
        /// Phase 3: Uses multiplicative formula: Base * (1 + modifier)
        /// </summary>
        private void RecalculateMoveSpeed()
        {
            _moveSpeed = GameConstants.PLAYER_BASE_SPEED * (1f + _moveSpeedModifier);
            Debug.Log($"[PlayerStats] Move Speed updated: {_moveSpeed:F1} (Base: {GameConstants.PLAYER_BASE_SPEED}, Modifier: +{_moveSpeedModifier * 100:F0}%)");
        }
        #endregion

        #region Death Handling
        /// <summary>
        /// Handle player death.
        /// </summary>
        private void Die()
        {
            Debug.Log("[PlayerStats] Player died!");

            OnPlayerDeath?.Invoke();

            // Notify GameManager
            if (Core.GameManager.Instance != null)
            {
                Core.GameManager.Instance.EndGame();
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

        [ContextMenu("Heal to Full")]
        private void DebugHeal()
        {
            Heal(_maxHealth);
        }

        [ContextMenu("Add 10 Speed")]
        private void DebugAddSpeed()
        {
            ModifyStat("MoveSpeed", 10f);
        }
#endif
        #endregion
    }
}
