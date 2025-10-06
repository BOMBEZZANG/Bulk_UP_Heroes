using UnityEngine;
using BulkUpHeroes.Parts;

namespace BulkUpHeroes.Player
{
    /// <summary>
    /// Controls player character animations based on movement and actions.
    /// Updates the Animator parameters for the Sidekick character.
    /// </summary>
    public class PlayerAnimationController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerController _playerController;
        [SerializeField] private SidekickCharacterController _sidekickController;

        [Header("Settings")]
        [SerializeField] private float _speedThreshold = 0.1f;

        private Animator _animator;
        private Rigidbody _rigidbody;

        // Animator parameter hashes (more efficient than strings)
        private static readonly int SpeedHash = Animator.StringToHash("Speed");
        private static readonly int AttackHash = Animator.StringToHash("Attack");

        private void Awake()
        {
            Debug.Log("[PlayerAnimationController] Awake() called - script is running!");

            // Auto-find references
            if (_playerController == null)
            {
                _playerController = GetComponent<PlayerController>();
            }

            if (_sidekickController == null)
            {
                _sidekickController = GetComponent<SidekickCharacterController>();
            }

            _rigidbody = GetComponent<Rigidbody>();

            Debug.Log($"[PlayerAnimationController] References found - SidekickController: {_sidekickController != null}, Rigidbody: {_rigidbody != null}");
        }

        private void Start()
        {
            Debug.Log("[PlayerAnimationController] Start() called - initializing...");

            // Get animator from Sidekick character (created at runtime)
            FindAnimator();

            if (_animator == null)
            {
                Debug.LogWarning("[PlayerAnimationController] Animator not found in Start - will retry in Update");
            }
        }

        private void Update()
        {
            // Try to find animator if not found yet
            if (_animator == null)
            {
                FindAnimator();
            }

            if (_animator != null)
            {
                UpdateAnimations();
            }
            else
            {
                // Debug once every 60 frames if animator not found
                if (Time.frameCount % 60 == 0)
                {
                    Debug.LogWarning("[PlayerAnimationController] Update running but Animator is null - still searching...");
                }
            }
        }

        /// <summary>
        /// Find the Animator component on the Sidekick character.
        /// </summary>
        private void FindAnimator()
        {
            if (_sidekickController == null) return;

            GameObject sidekickCharacter = _sidekickController.GetCurrentCharacter();
            if (sidekickCharacter != null)
            {
                _animator = sidekickCharacter.GetComponent<Animator>();

                if (_animator != null)
                {
                    Debug.Log($"[PlayerAnimationController] Found Animator on {sidekickCharacter.name}");
                }
                else
                {
                    Debug.LogWarning($"[PlayerAnimationController] No Animator found on {sidekickCharacter.name}");
                }
            }
        }

        /// <summary>
        /// Update animation parameters based on player state.
        /// </summary>
        private void UpdateAnimations()
        {
            // Calculate movement speed
            float speed = CalculateSpeed();

            // Update Speed parameter (for Idle/Running transitions)
            _animator.SetFloat(SpeedHash, speed);

            // Debug: Log speed every 60 frames (about once per second)
            if (Time.frameCount % 60 == 0)
            {
                Debug.Log($"[PlayerAnimation] Speed: {speed:F2}, Current State: {GetCurrentStateName()}");
            }
        }

        /// <summary>
        /// Calculate player's current movement speed.
        /// </summary>
        private float CalculateSpeed()
        {
            if (_rigidbody != null)
            {
                // Use rigidbody velocity (horizontal only, ignore Y)
                Vector3 horizontalVelocity = new Vector3(_rigidbody.linearVelocity.x, 0, _rigidbody.linearVelocity.z);
                return horizontalVelocity.magnitude;
            }
            else
            {
                // Fallback: use transform movement
                return 0f;
            }
        }

        /// <summary>
        /// Trigger attack animation.
        /// Call this from combat system when player attacks.
        /// </summary>
        public void TriggerAttack()
        {
            if (_animator != null)
            {
                _animator.SetTrigger(AttackHash);
                Debug.Log("[PlayerAnimationController] Attack animation triggered");
            }
        }

        /// <summary>
        /// Called when Sidekick character is rebuilt (e.g., part equipped).
        /// Need to re-find the animator.
        /// </summary>
        public void OnCharacterRebuilt()
        {
            _animator = null;
            FindAnimator();
        }

        #region Public Accessors
        /// <summary>
        /// Check if animator is ready.
        /// </summary>
        public bool IsAnimatorReady()
        {
            return _animator != null;
        }

        /// <summary>
        /// Get current animation state name (for debugging).
        /// </summary>
        public string GetCurrentStateName()
        {
            if (_animator == null) return "No Animator";

            AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
            return stateInfo.IsName("Idle") ? "Idle" :
                   stateInfo.IsName("Running") ? "Running" :
                   stateInfo.IsName("Punching") ? "Punching" : "Unknown";
        }
        #endregion
    }
}
