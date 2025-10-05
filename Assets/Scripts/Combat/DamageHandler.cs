using System.Collections;
using UnityEngine;
using BulkUpHeroes.Utils;
using BulkUpHeroes.Core;

namespace BulkUpHeroes.Combat
{
    /// <summary>
    /// Handles damage reception, visual feedback, and death for entities.
    /// Integrates with Stats components and triggers appropriate events.
    ///
    /// Phase 2: Basic damage handling with visual feedback
    /// Future: Will support armor, resistance, damage types, and buffs
    /// </summary>
    public class DamageHandler : MonoBehaviour
    {
        #region Visual Feedback Settings
        [Header("Damage Feedback")]
        [SerializeField] private bool _enableDamageFeedback = true;
        [SerializeField] private Color _damageFlashColor = Color.red;
        [SerializeField] private float _damageFlashDuration = 0.1f;
        [SerializeField] private float _damageShakeDuration = 0.1f;
        [SerializeField] private float _damageShakeIntensity = 0.1f;

        [Header("Death Settings")]
        [SerializeField] private float _deathFadeDuration = 0.3f;
        [SerializeField] private float _deathScaleDuration = 0.3f;
        [SerializeField] private float _deathDelay = 0.5f;
        #endregion

        #region Components
        private IDamageable _damageable;
        private Renderer[] _renderers;
        private Transform _transform;
        #endregion

        #region State
        private bool _isPlayer = false;
        private bool _isDying = false;

        // Flash effect state
        private bool _isFlashing = false;
        private float _flashTimer = 0f;
        private Color[] _originalColors;

        // Use MaterialPropertyBlock instead of material instances
        private MaterialPropertyBlock _propertyBlock;
        private static readonly int _colorProperty = Shader.PropertyToID("_Color");

        // Death animation state
        private Vector3 _originalScale;
        private float _deathTimer = 0f;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            InitializeComponents();
        }

        private void Update()
        {
            UpdateDamageFlash();
            UpdateDeathAnimation();
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Initialize components and cache references.
        /// Uses MaterialPropertyBlock to avoid creating material instances.
        /// </summary>
        private void InitializeComponents()
        {
            _transform = transform;
            _originalScale = _transform.localScale;
            _damageable = GetComponent<IDamageable>();
            _renderers = GetComponentsInChildren<Renderer>();

            _isPlayer = gameObject.CompareTag(GameConstants.TAG_PLAYER);

            // Initialize MaterialPropertyBlock
            _propertyBlock = new MaterialPropertyBlock();

            // Cache original colors from SHARED materials (no instances created!)
            if (_renderers.Length > 0)
            {
                _originalColors = new Color[_renderers.Length];

                for (int i = 0; i < _renderers.Length; i++)
                {
                    // Use sharedMaterial to avoid creating instances
                    if (_renderers[i].sharedMaterial != null)
                    {
                        // Check if material has _Color property (standard shader)
                        if (_renderers[i].sharedMaterial.HasProperty("_Color"))
                        {
                            _originalColors[i] = _renderers[i].sharedMaterial.color;
                        }
                        // Try _BaseColor for URP/Sidekick shaders
                        else if (_renderers[i].sharedMaterial.HasProperty("_BaseColor"))
                        {
                            _originalColors[i] = _renderers[i].sharedMaterial.GetColor("_BaseColor");
                        }
                        else
                        {
                            // Fallback to white if no color property found
                            _originalColors[i] = Color.white;
                            Debug.LogWarning($"[DamageHandler] Material '{_renderers[i].sharedMaterial.name}' has no _Color or _BaseColor property, using white");
                        }
                    }
                }

                Debug.Log($"[DamageHandler] Initialized with MaterialPropertyBlock (NO material instances created)");
            }

            if (_damageable == null)
            {
                Debug.LogError($"[DamageHandler] No IDamageable component on {gameObject.name}!");
            }

            Debug.Log($"[DamageHandler] Initialized on {gameObject.name} with {_renderers.Length} renderers");
        }
        #endregion

        #region Damage Reception
        /// <summary>
        /// Process incoming damage. Called by external systems (like CombatManager).
        /// </summary>
        /// <param name="amount">Damage amount</param>
        /// <param name="source">Source of damage</param>
        public void ProcessDamage(float amount, GameObject source)
        {
            if (_isDying || _damageable == null || !_damageable.IsAlive)
                return;

            Debug.Log($"[DamageHandler] {gameObject.name} processing {amount} damage");

            // Apply damage to damageable component
            _damageable.TakeDamage(amount, source);

            // Visual feedback
            if (_enableDamageFeedback)
            {
                TriggerDamageFlash();
            }

            // Trigger event
            if (_isPlayer)
            {
                GameEvents.TriggerPlayerDamaged(amount, source);
            }
            else
            {
                GameEvents.TriggerEnemyDamaged(gameObject, amount);
            }

            // Check for death
            if (!_damageable.IsAlive && !_isDying)
            {
                Debug.Log($"[DamageHandler] {gameObject.name} is dead! Triggering death sequence...");
                TriggerDeath();
            }
        }
        #endregion

        #region Visual Feedback
        /// <summary>
        /// Trigger damage flash effect using MaterialPropertyBlock.
        /// </summary>
        private void TriggerDamageFlash()
        {
            _isFlashing = true;
            _flashTimer = _damageFlashDuration;

            // Set flash color using MaterialPropertyBlock (NO shader compilation!)
            for (int i = 0; i < _renderers.Length; i++)
            {
                if (_renderers[i] != null)
                {
                    _propertyBlock.SetColor(_colorProperty, _damageFlashColor);
                    _renderers[i].SetPropertyBlock(_propertyBlock);
                }
            }
        }

        /// <summary>
        /// Update damage flash animation.
        /// </summary>
        private void UpdateDamageFlash()
        {
            if (!_isFlashing) return;

            _flashTimer -= Time.deltaTime;

            if (_flashTimer <= 0f)
            {
                // Flash complete - restore original colors
                _isFlashing = false;
                RestoreOriginalColors();
            }
        }

        /// <summary>
        /// Restore original material colors using MaterialPropertyBlock.
        /// </summary>
        private void RestoreOriginalColors()
        {
            for (int i = 0; i < _renderers.Length; i++)
            {
                if (_renderers[i] != null && i < _originalColors.Length)
                {
                    _propertyBlock.SetColor(_colorProperty, _originalColors[i]);
                    _renderers[i].SetPropertyBlock(_propertyBlock);
                }
            }
        }
        #endregion

        #region Death Handling
        /// <summary>
        /// Trigger death sequence.
        /// </summary>
        private void TriggerDeath()
        {
            _isDying = true;
            _deathTimer = _deathFadeDuration;

            Debug.Log($"[DamageHandler] {gameObject.name} died");

            // Disable other components
            DisableComponentsOnDeath();

            // Trigger death event
            if (_isPlayer)
            {
                GameEvents.TriggerPlayerDeath();
            }
            else
            {
                GameEvents.TriggerEnemyDeath(gameObject);
            }

            // Start death animation
            StartDeathAnimation();
        }

        /// <summary>
        /// Start death animation sequence.
        /// </summary>
        private void StartDeathAnimation()
        {
            // Death animation will run in Update
            // After animation, destroy or return to pool
        }

        /// <summary>
        /// Update death animation using MaterialPropertyBlock.
        /// </summary>
        private void UpdateDeathAnimation()
        {
            if (!_isDying) return;

            _deathTimer -= Time.deltaTime;

            if (_deathTimer > 0f)
            {
                // Animate scale and fade
                float progress = 1f - (_deathTimer / _deathFadeDuration);

                // Scale down
                _transform.localScale = Vector3.Lerp(_originalScale, Vector3.zero, progress);

                // Fade out using MaterialPropertyBlock
                float alpha = Mathf.Lerp(1f, 0f, progress);
                for (int i = 0; i < _renderers.Length; i++)
                {
                    if (_renderers[i] != null && i < _originalColors.Length)
                    {
                        Color color = _originalColors[i];
                        color.a = alpha;
                        _propertyBlock.SetColor(_colorProperty, color);
                        _renderers[i].SetPropertyBlock(_propertyBlock);
                    }
                }
            }
            else
            {
                // Animation complete - cleanup
                HandlePostDeath();
            }
        }

        /// <summary>
        /// Handle post-death cleanup.
        /// </summary>
        private void HandlePostDeath()
        {
            if (_isPlayer)
            {
                // Player death - handled by GameManager
                // Don't destroy, just disable
                gameObject.SetActive(false);
            }
            else
            {
                // Enemy death - return to pool
                if (PoolManager.Instance != null)
                {
                    // Reset visual state before returning to pool
                    ResetVisualState();
                    PoolManager.Instance.ReturnEnemy(gameObject);
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }

        /// <summary>
        /// Disable components that should not be active when dead.
        /// </summary>
        private void DisableComponentsOnDeath()
        {
            // Disable movement
            var controller = GetComponent<MonoBehaviour>();
            if (controller != null)
            {
                // Don't disable this DamageHandler - we need it for death animation
                var playerController = GetComponent<Player.PlayerController>();
                if (playerController != null) playerController.enabled = false;

                var enemyAI = GetComponent<Enemies.EnemyAI>();
                if (enemyAI != null) enemyAI.enabled = false;
            }

            // Disable combat
            var combat = GetComponent<CombatManager>();
            if (combat != null) combat.enabled = false;

            // Disable collisions
            var rigidbody = GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
                rigidbody.linearVelocity = Vector3.zero;
                rigidbody.isKinematic = true;
            }
        }

        /// <summary>
        /// Reset visual state (for pooling) using MaterialPropertyBlock.
        /// </summary>
        private void ResetVisualState()
        {
            _transform.localScale = _originalScale;

            // Reset colors and alpha using MaterialPropertyBlock
            for (int i = 0; i < _renderers.Length; i++)
            {
                if (_renderers[i] != null && i < _originalColors.Length)
                {
                    Color color = _originalColors[i];
                    color.a = 1f;
                    _propertyBlock.SetColor(_colorProperty, color);
                    _renderers[i].SetPropertyBlock(_propertyBlock);
                }
            }

            _isDying = false;
            _isFlashing = false;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Reset damage handler (for pooling or respawn).
        /// </summary>
        public void ResetHandler()
        {
            ResetVisualState();

            // Re-enable components
            var playerController = GetComponent<Player.PlayerController>();
            if (playerController != null) playerController.enabled = true;

            var enemyAI = GetComponent<Enemies.EnemyAI>();
            if (enemyAI != null) enemyAI.enabled = true;

            var combat = GetComponent<CombatManager>();
            if (combat != null) combat.enabled = true;

            var rigidbody = GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
                rigidbody.isKinematic = false;
            }
        }
        #endregion
    }
}
