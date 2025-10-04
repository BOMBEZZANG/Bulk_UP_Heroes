using UnityEngine;
using TMPro;
using BulkUpHeroes.Core;
using BulkUpHeroes.Parts;
using BulkUpHeroes.Utils;

namespace BulkUpHeroes.UI
{
    /// <summary>
    /// Displays currently equipped parts in a HUD element.
    /// Shows part type and rarity with color-coding.
    ///
    /// Phase 3: Simple text list display
    /// Future: Could add part icons and fancy animations
    /// </summary>
    public class PartInventoryUI : MonoBehaviour
    {
        #region Serialized Fields
        [Header("Part Slot Text Elements")]
        [SerializeField] private TextMeshProUGUI _headText;
        [SerializeField] private TextMeshProUGUI _armsText;
        [SerializeField] private TextMeshProUGUI _torsoText;
        [SerializeField] private TextMeshProUGUI _legsText;

        [Header("Display Settings")]
        [SerializeField] private string _emptySlotText = "Empty";
        [SerializeField] private Color _emptySlotColor = Color.gray;
        #endregion

        #region State
        private bool _isInitialized = false;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            ValidateReferences();
        }

        private void Start()
        {
            InitializeDisplay();
            SubscribeToEvents();
        }

        private void OnDestroy()
        {
            UnsubscribeFromEvents();
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Validate all required UI references.
        /// </summary>
        private void ValidateReferences()
        {
            if (_headText == null) Debug.LogError("[PartInventoryUI] Head text not assigned!");
            if (_armsText == null) Debug.LogError("[PartInventoryUI] Arms text not assigned!");
            if (_torsoText == null) Debug.LogError("[PartInventoryUI] Torso text not assigned!");
            if (_legsText == null) Debug.LogError("[PartInventoryUI] Legs text not assigned!");
        }

        /// <summary>
        /// Initialize all part slots to empty state.
        /// </summary>
        private void InitializeDisplay()
        {
            UpdatePartSlot(PartType.Head, null);
            UpdatePartSlot(PartType.Arms, null);
            UpdatePartSlot(PartType.Torso, null);
            UpdatePartSlot(PartType.Legs, null);

            _isInitialized = true;
            Debug.Log("[PartInventoryUI] Display initialized");
        }
        #endregion

        #region Event Subscription
        /// <summary>
        /// Subscribe to part-related events.
        /// </summary>
        private void SubscribeToEvents()
        {
            GameEvents.OnPartEquipped += HandlePartEquipped;
            GameEvents.OnPartUnequipped += HandlePartUnequipped;
            Debug.Log("[PartInventoryUI] Subscribed to GameEvents");
        }

        /// <summary>
        /// Unsubscribe from events to prevent memory leaks.
        /// </summary>
        private void UnsubscribeFromEvents()
        {
            GameEvents.OnPartEquipped -= HandlePartEquipped;
            GameEvents.OnPartUnequipped -= HandlePartUnequipped;
            Debug.Log("[PartInventoryUI] Unsubscribed from GameEvents");
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Handle part equipped event.
        /// </summary>
        private void HandlePartEquipped(PartType partType, PartData partData)
        {
            if (!_isInitialized) return;

            Debug.Log($"[PartInventoryUI] Part equipped: {partType} - {partData.displayName}");
            UpdatePartSlot(partType, partData);
        }

        /// <summary>
        /// Handle part unequipped event.
        /// </summary>
        private void HandlePartUnequipped(PartType partType)
        {
            if (!_isInitialized) return;

            Debug.Log($"[PartInventoryUI] Part unequipped: {partType}");
            UpdatePartSlot(partType, null);
        }
        #endregion

        #region UI Update
        /// <summary>
        /// Update a specific part slot display.
        /// </summary>
        private void UpdatePartSlot(PartType partType, PartData partData)
        {
            TextMeshProUGUI textElement = GetTextElementForPartType(partType);

            if (textElement == null)
            {
                Debug.LogWarning($"[PartInventoryUI] No text element for {partType}!");
                return;
            }

            if (partData == null)
            {
                // Empty slot
                textElement.text = $"{partType}: {_emptySlotText}";
                textElement.color = _emptySlotColor;
            }
            else
            {
                // Equipped part
                textElement.text = $"{partType}: {partData.rarity}";
                textElement.color = partData.GetRarityColor();
            }
        }

        /// <summary>
        /// Get the text element for a specific part type.
        /// </summary>
        private TextMeshProUGUI GetTextElementForPartType(PartType partType)
        {
            return partType switch
            {
                PartType.Head => _headText,
                PartType.Arms => _armsText,
                PartType.Torso => _torsoText,
                PartType.Legs => _legsText,
                _ => null
            };
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Force refresh all part slots from CharacterPartManager.
        /// Useful after scene reload or initialization.
        /// </summary>
        public void RefreshDisplay(Player.CharacterPartManager partManager)
        {
            if (partManager == null)
            {
                Debug.LogWarning("[PartInventoryUI] Cannot refresh - CharacterPartManager is null!");
                return;
            }

            var equippedParts = partManager.GetAllEquippedParts();

            foreach (var kvp in equippedParts)
            {
                UpdatePartSlot(kvp.Key, kvp.Value);
            }

            Debug.Log("[PartInventoryUI] Display refreshed from CharacterPartManager");
        }
        #endregion

        #region Debug Helpers
#if UNITY_EDITOR
        [ContextMenu("Test Display - All Empty")]
        private void DebugTestEmpty()
        {
            InitializeDisplay();
        }

        [ContextMenu("Test Display - Mock Parts")]
        private void DebugTestMockParts()
        {
            // Create mock part data for testing
            UpdatePartSlot(PartType.Head, CreateMockPart(PartType.Head, Rarity.Common));
            UpdatePartSlot(PartType.Arms, CreateMockPart(PartType.Arms, Rarity.Rare));
            UpdatePartSlot(PartType.Torso, CreateMockPart(PartType.Torso, Rarity.Epic));
            UpdatePartSlot(PartType.Legs, null); // Empty
        }

        private PartData CreateMockPart(PartType type, Rarity rarity)
        {
            var mock = ScriptableObject.CreateInstance<PartData>();
            mock.partType = type;
            mock.rarity = rarity;
            mock.displayName = $"Mock {rarity} {type}";
            return mock;
        }
#endif
        #endregion
    }
}
