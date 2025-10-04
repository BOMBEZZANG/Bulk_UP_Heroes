using UnityEngine;
using BulkUpHeroes.Core;
using BulkUpHeroes.Parts;
using BulkUpHeroes.Utils;

namespace BulkUpHeroes.UI
{
    /// <summary>
    /// Manages spawning of floating stat popup texts above player when parts are equipped.
    /// Shows the stat bonuses from equipped parts.
    ///
    /// Phase 3: Simple floating text above player
    /// Future: Could add particle effects and sound
    /// </summary>
    public class PartStatPopupManager : MonoBehaviour
    {
        #region Serialized Fields
        [Header("References")]
        [SerializeField] private Canvas _canvas;
        [SerializeField] private Transform _playerTransform;

        [Header("Popup Settings")]
        [SerializeField] private Vector3 _popupOffset = new Vector3(0f, 2f, 0f);
        [SerializeField] private bool _showPopups = true;
        #endregion

        #region Unity Lifecycle
        private void Start()
        {
            ValidateReferences();
            SubscribeToEvents();
            PrewarmFloatingText();
        }

        private void OnDestroy()
        {
            UnsubscribeFromEvents();
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Validate required references.
        /// </summary>
        private void ValidateReferences()
        {
            if (_canvas == null)
            {
                _canvas = FindObjectOfType<Canvas>();
                if (_canvas == null)
                {
                    Debug.LogError("[PartStatPopupManager] No Canvas found!");
                }
            }

            if (_playerTransform == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag(GameConstants.TAG_PLAYER);
                if (player != null)
                {
                    _playerTransform = player.transform;
                }
                else
                {
                    Debug.LogError("[PartStatPopupManager] Player not found!");
                }
            }
        }
        #endregion

        #region Event Subscription
        /// <summary>
        /// Subscribe to part events.
        /// </summary>
        private void SubscribeToEvents()
        {
            GameEvents.OnPartEquipped += HandlePartEquipped;
            Debug.Log("[PartStatPopupManager] Subscribed to GameEvents");
        }

        /// <summary>
        /// Unsubscribe from events.
        /// </summary>
        private void UnsubscribeFromEvents()
        {
            GameEvents.OnPartEquipped -= HandlePartEquipped;
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Handle part equipped event and spawn stat popup.
        /// </summary>
        private void HandlePartEquipped(PartType partType, PartData partData)
        {
            if (!_showPopups || _playerTransform == null || _canvas == null) return;

            // Get primary stat bonus
            string statText = GetPrimaryStatText(partData);
            Color statColor = GetPartTypeColor(partType);

            // Spawn floating text above player
            Vector3 popupPosition = _playerTransform.position + _popupOffset;
            FloatingStatText.Create(statText, statColor, popupPosition, _canvas);

            Debug.Log($"[PartStatPopupManager] Spawned popup: {statText}");
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Get the primary stat bonus text for a part.
        /// </summary>
        private string GetPrimaryStatText(PartData partData)
        {
            float healthBonus = partData.GetFinalHealthBonus();
            float damageBonus = partData.GetFinalDamageMultiplier();
            float attackSpeedBonus = partData.GetFinalAttackSpeedMultiplier();
            float moveSpeedBonus = partData.GetFinalMoveSpeedMultiplier();

            // Return the highest bonus stat
            if (healthBonus > 0 && healthBonus >= damageBonus && healthBonus >= attackSpeedBonus && healthBonus >= moveSpeedBonus)
            {
                return $"+{healthBonus * 100:F0}% Health";
            }
            else if (damageBonus > 0 && damageBonus >= attackSpeedBonus && damageBonus >= moveSpeedBonus)
            {
                return $"+{damageBonus * 100:F0}% Damage";
            }
            else if (attackSpeedBonus > 0 && attackSpeedBonus >= moveSpeedBonus)
            {
                return $"+{attackSpeedBonus * 100:F0}% Attack Speed";
            }
            else if (moveSpeedBonus > 0)
            {
                return $"+{moveSpeedBonus * 100:F0}% Move Speed";
            }

            return "+Stats";
        }

        /// <summary>
        /// Get color for part type (matches Phase3_plan.md spec).
        /// </summary>
        private Color GetPartTypeColor(PartType partType)
        {
            return partType switch
            {
                PartType.Head => new Color(1f, 0.3f, 0.3f),    // Red
                PartType.Arms => new Color(0.3f, 0.5f, 1f),     // Blue
                PartType.Torso => new Color(0.3f, 1f, 0.3f),    // Green
                PartType.Legs => new Color(1f, 1f, 0.3f),       // Yellow
                _ => Color.white
            };
        }
        #endregion

        #region Initialization Helper
        /// <summary>
        /// Pre-warm TextMeshPro to avoid first-time lag spike.
        /// Creates a floating text off-screen and lets it complete its lifecycle.
        /// </summary>
        private void PrewarmFloatingText()
        {
            if (_canvas == null || _playerTransform == null) return;

            // Create a dummy floating text off-screen to initialize TextMeshPro and coroutines
            Vector3 warmupPosition = _playerTransform.position + Vector3.down * 100f; // Far below ground
            FloatingStatText warmupText = FloatingStatText.Create("+0", Color.clear, warmupPosition, _canvas);

            // Let it run and self-destruct naturally (its coroutine will handle cleanup)
            // This ensures the coroutine system is fully initialized

            Debug.Log("[PartStatPopupManager] Pre-warmed TextMeshPro system");
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Toggle stat popups on/off.
        /// </summary>
        public void SetPopupsEnabled(bool enabled)
        {
            _showPopups = enabled;
            Debug.Log($"[PartStatPopupManager] Popups {(enabled ? "enabled" : "disabled")}");
        }
        #endregion
    }
}
