using UnityEngine;
using BulkUpHeroes.Utils;

namespace BulkUpHeroes.Parts
{
    /// <summary>
    /// Helper class to abstract Synty Sidekick API for part swapping.
    /// Handles both Sidekick-enabled and fallback (dummy) modes.
    ///
    /// Phase 4: Full Sidekick Runtime API integration
    /// </summary>
    public class SidekickPartSwapper : MonoBehaviour
    {
        #region Configuration
        [Header("Sidekick Integration")]
        [SerializeField] private bool _useSidekickIntegration = true;
        [SerializeField] private SidekickCharacterController _sidekickController;

        [Header("Fallback Mode (Dummy Visuals)")]
        [SerializeField] private Transform _headSlot;
        [SerializeField] private Transform _armsSlot;
        [SerializeField] private Transform _torsoSlot;
        [SerializeField] private Transform _legsSlot;
        #endregion

        #region State
        private bool _sidekickAvailable = false;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            InitializeSidekick();
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Initialize Sidekick character reference.
        /// </summary>
        private void InitializeSidekick()
        {
            if (_useSidekickIntegration)
            {
                // Try to find controller if not assigned
                if (_sidekickController == null)
                {
                    _sidekickController = GetComponent<SidekickCharacterController>();
                }

                if (_sidekickController != null)
                {
                    // Check if it initialized successfully
                    _sidekickAvailable = _sidekickController.IsAvailable();

                    if (_sidekickAvailable)
                    {
                        Debug.Log("[SidekickPartSwapper] Sidekick integration enabled and ready!");
                    }
                    else
                    {
                        Debug.LogWarning("[SidekickPartSwapper] SidekickCharacterController found but not initialized. Using fallback mode.");
                    }
                }
                else
                {
                    Debug.LogWarning("[SidekickPartSwapper] SidekickCharacterController not found. Using fallback dummy visuals.");
                    _sidekickAvailable = false;
                }
            }
            else
            {
                Debug.Log("[SidekickPartSwapper] Sidekick integration disabled. Using fallback dummy visuals mode.");
                _sidekickAvailable = false;
            }
        }
        #endregion

        #region Part Swapping
        /// <summary>
        /// Swap a part using Sidekick API or fallback to dummy system.
        /// Returns the slot transform for fallback rendering.
        /// </summary>
        public Transform SwapPart(PartType partType, string sidekickPartID, out bool usedSidekick)
        {
            usedSidekick = false;

            // Re-check Sidekick availability in case it initialized after Awake
            if (!_sidekickAvailable && _sidekickController != null)
            {
                _sidekickAvailable = _sidekickController.IsAvailable();
                if (_sidekickAvailable)
                {
                    Debug.Log("[SidekickPartSwapper] Sidekick now available!");
                }
            }

            // Try Sidekick first
            if (_sidekickAvailable && !string.IsNullOrEmpty(sidekickPartID))
            {
                Debug.Log($"[SidekickPartSwapper] Attempting to equip {partType} with Sidekick Part ID: '{sidekickPartID}'");
                bool success = _sidekickController.EquipPart(partType, sidekickPartID);

                if (success)
                {
                    usedSidekick = true;
                    Debug.Log($"[SidekickPartSwapper] Swapped {partType} to '{sidekickPartID}' using Sidekick");
                    return null; // No slot needed for Sidekick
                }
                else
                {
                    Debug.LogWarning($"[SidekickPartSwapper] Sidekick swap failed for {partType}, falling back to dummy");
                }
            }

            // Fallback: Return slot transform for dummy visual
            Transform slotTransform = GetSlotTransform(partType);

            // Log why we're using fallback
            if (!_sidekickAvailable)
            {
                Debug.LogWarning($"[SidekickPartSwapper] Using fallback for {partType} - Sidekick not available");
            }
            else if (string.IsNullOrEmpty(sidekickPartID))
            {
                Debug.LogWarning($"[SidekickPartSwapper] Using fallback for {partType} - Part ID is empty");
            }

            return slotTransform;
        }

        /// <summary>
        /// Remove a part using Sidekick API or fallback.
        /// </summary>
        public Transform RemovePart(PartType partType, out bool usedSidekick)
        {
            usedSidekick = false;

            // Try Sidekick first
            if (_sidekickAvailable)
            {
                _sidekickController.RemovePart(partType);
                usedSidekick = true;
                Debug.Log($"[SidekickPartSwapper] Removed {partType} using Sidekick");
                return null;
            }

            // Fallback: Return slot transform for manual cleanup
            return GetSlotTransform(partType);
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Convert PartType to Sidekick category name.
        /// </summary>
        private string GetSidekickCategoryName(PartType partType)
        {
            return partType switch
            {
                PartType.Head => "Head",
                PartType.Arms => "Arms",
                PartType.Torso => "Torso",
                PartType.Legs => "Legs",
                _ => string.Empty
            };
        }

        /// <summary>
        /// Get slot transform for fallback mode.
        /// </summary>
        private Transform GetSlotTransform(PartType partType)
        {
            return partType switch
            {
                PartType.Head => _headSlot,
                PartType.Arms => _armsSlot,
                PartType.Torso => _torsoSlot,
                PartType.Legs => _legsSlot,
                _ => null
            };
        }

        /// <summary>
        /// Check if Sidekick integration is active.
        /// </summary>
        public bool IsSidekickAvailable()
        {
            return _sidekickAvailable;
        }
        #endregion

        #region Public Accessors
        /// <summary>
        /// Get the Sidekick character controller (if available).
        /// </summary>
        public SidekickCharacterController GetSidekickController()
        {
            return _sidekickController;
        }

        /// <summary>
        /// Set slot transforms for fallback mode.
        /// </summary>
        public void SetSlots(Transform head, Transform arms, Transform torso, Transform legs)
        {
            _headSlot = head;
            _armsSlot = arms;
            _torsoSlot = torso;
            _legsSlot = legs;
        }
        #endregion
    }
}
