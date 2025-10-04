using System.Collections.Generic;
using UnityEngine;
using BulkUpHeroes.Parts;
using BulkUpHeroes.Core;
using BulkUpHeroes.Utils;

namespace BulkUpHeroes.Player
{
    /// <summary>
    /// Manages character part slots, equipping/unequipping, and visual representation.
    /// Applies stat modifiers from equipped parts to PlayerStats.
    ///
    /// Phase 3: Dummy visuals using colored primitives
    /// Future: Will use actual 3D model parts with animations
    /// </summary>
    public class CharacterPartManager : MonoBehaviour
    {
        #region Serialized Fields
        [Header("Part Slots")]
        [SerializeField] private Transform _headSlot;
        [SerializeField] private Transform _armsSlot;
        [SerializeField] private Transform _torsoSlot;
        [SerializeField] private Transform _legsSlot;

        [Header("References")]
        [SerializeField] private PlayerStats _playerStats;
        #endregion

        #region State
        private Dictionary<PartType, PartSlot> _partSlots;
        private Dictionary<PartType, PartData> _equippedParts;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            InitializeSlots();
            InitializeEquippedParts();
        }

        private void Start()
        {
            ValidateReferences();
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Initialize part slot dictionary.
        /// </summary>
        private void InitializeSlots()
        {
            // Position dummies on surface of player for visual clarity during development
            _partSlots = new Dictionary<PartType, PartSlot>
            {
                { PartType.Head, new PartSlot(_headSlot, new Vector3(0, 1.2f, 0.5f), new Vector3(0.3f, 0.3f, 0.3f)) },
                { PartType.Arms, new PartSlot(_armsSlot, new Vector3(0.7f, 0.5f, 0.3f), new Vector3(0.6f, 0.3f, 0.3f)) },
                { PartType.Torso, new PartSlot(_torsoSlot, new Vector3(0, 0.3f, 0.5f), new Vector3(0.5f, 0.7f, 0.4f)) },
                { PartType.Legs, new PartSlot(_legsSlot, new Vector3(0, -0.5f, 0.5f), new Vector3(0.3f, 0.5f, 0.3f)) }
            };

            Debug.Log("[CharacterPartManager] Part slots initialized");
        }

        /// <summary>
        /// Initialize equipped parts dictionary.
        /// </summary>
        private void InitializeEquippedParts()
        {
            _equippedParts = new Dictionary<PartType, PartData>
            {
                { PartType.Head, null },
                { PartType.Arms, null },
                { PartType.Torso, null },
                { PartType.Legs, null }
            };

            Debug.Log("[CharacterPartManager] Equipped parts dictionary initialized");
        }

        /// <summary>
        /// Validate required references.
        /// </summary>
        private void ValidateReferences()
        {
            if (_playerStats == null)
            {
                _playerStats = GetComponent<PlayerStats>();
                if (_playerStats == null)
                {
                    Debug.LogError("[CharacterPartManager] PlayerStats component not found!");
                }
            }

            foreach (var kvp in _partSlots)
            {
                if (kvp.Value.slotTransform == null)
                {
                    Debug.LogWarning($"[CharacterPartManager] {kvp.Key} slot transform not assigned!");
                }
            }
        }
        #endregion

        #region Part Equipping
        /// <summary>
        /// Try to equip a part. Returns true if successful, false if player already has better part.
        /// </summary>
        public bool TryEquipPart(PartData newPart)
        {
            if (newPart == null)
            {
                Debug.LogWarning("[CharacterPartManager] Attempted to equip null part!");
                return false;
            }

            PartType partType = newPart.partType;
            PartData currentPart = _equippedParts[partType];

            // Check if we should equip this part
            if (!ShouldEquipPart(currentPart, newPart))
            {
                Debug.Log($"[CharacterPartManager] Already have better {partType}: {currentPart.rarity}");
                return false;
            }

            // Remove old part if exists
            if (currentPart != null)
            {
                RemovePartStats(currentPart);
                RemovePartVisual(partType);
            }

            // Equip new part
            _equippedParts[partType] = newPart;
            ApplyPartStats(newPart);
            CreatePartVisual(partType, newPart);

            // Trigger event
            GameEvents.TriggerPartEquipped(partType, newPart);

            Debug.Log($"[CharacterPartManager] Equipped {newPart.rarity} {partType}: {newPart.displayName}");
            return true;
        }

        /// <summary>
        /// Determine if new part should replace current part.
        /// </summary>
        private bool ShouldEquipPart(PartData currentPart, PartData newPart)
        {
            // If no part equipped, always equip
            if (currentPart == null)
                return true;

            // Compare rarity (higher is better)
            return newPart.rarity > currentPart.rarity;
        }

        /// <summary>
        /// Unequip a part by type.
        /// </summary>
        public void UnequipPart(PartType partType)
        {
            PartData currentPart = _equippedParts[partType];

            if (currentPart == null)
            {
                Debug.LogWarning($"[CharacterPartManager] No {partType} equipped to remove!");
                return;
            }

            // Remove stats and visual
            RemovePartStats(currentPart);
            RemovePartVisual(partType);

            // Clear slot
            _equippedParts[partType] = null;

            // Trigger event
            GameEvents.TriggerPartUnequipped(partType);

            Debug.Log($"[CharacterPartManager] Unequipped {partType}");
        }
        #endregion

        #region Stat Management
        /// <summary>
        /// Apply part stat modifiers to player.
        /// </summary>
        private void ApplyPartStats(PartData part)
        {
            if (_playerStats == null)
            {
                Debug.LogError("[CharacterPartManager] Cannot apply stats - PlayerStats is null!");
                return;
            }

            // Apply stat modifiers
            if (part.healthBonus > 0)
            {
                float healthBonus = part.GetFinalHealthBonus();
                _playerStats.ModifyStat("MaxHealth", healthBonus, StatModifierType.PercentAdd);
                Debug.Log($"[CharacterPartManager] Applied +{healthBonus * 100:F0}% health");
            }

            if (part.damageMultiplier > 0)
            {
                float damageBonus = part.GetFinalDamageMultiplier();
                _playerStats.ModifyStat("Damage", damageBonus, StatModifierType.PercentAdd);
                Debug.Log($"[CharacterPartManager] Applied +{damageBonus * 100:F0}% damage");
            }

            if (part.attackSpeedMultiplier > 0)
            {
                float attackSpeedBonus = part.GetFinalAttackSpeedMultiplier();
                _playerStats.ModifyStat("AttackSpeed", attackSpeedBonus, StatModifierType.PercentAdd);
                Debug.Log($"[CharacterPartManager] Applied +{attackSpeedBonus * 100:F0}% attack speed");
            }

            if (part.moveSpeedMultiplier > 0)
            {
                float moveSpeedBonus = part.GetFinalMoveSpeedMultiplier();
                _playerStats.ModifyStat("MoveSpeed", moveSpeedBonus, StatModifierType.PercentAdd);
                Debug.Log($"[CharacterPartManager] Applied +{moveSpeedBonus * 100:F0}% move speed");
            }
        }

        /// <summary>
        /// Remove part stat modifiers from player.
        /// </summary>
        private void RemovePartStats(PartData part)
        {
            if (_playerStats == null)
            {
                Debug.LogError("[CharacterPartManager] Cannot remove stats - PlayerStats is null!");
                return;
            }

            // Remove stat modifiers (use negative values)
            if (part.healthBonus > 0)
            {
                float healthBonus = part.GetFinalHealthBonus();
                _playerStats.ModifyStat("MaxHealth", -healthBonus, StatModifierType.PercentAdd);
            }

            if (part.damageMultiplier > 0)
            {
                float damageBonus = part.GetFinalDamageMultiplier();
                _playerStats.ModifyStat("Damage", -damageBonus, StatModifierType.PercentAdd);
            }

            if (part.attackSpeedMultiplier > 0)
            {
                float attackSpeedBonus = part.GetFinalAttackSpeedMultiplier();
                _playerStats.ModifyStat("AttackSpeed", -attackSpeedBonus, StatModifierType.PercentAdd);
            }

            if (part.moveSpeedMultiplier > 0)
            {
                float moveSpeedBonus = part.GetFinalMoveSpeedMultiplier();
                _playerStats.ModifyStat("MoveSpeed", -moveSpeedBonus, StatModifierType.PercentAdd);
            }

            Debug.Log($"[CharacterPartManager] Removed stats for {part.rarity} {part.partType}");
        }
        #endregion

        #region Visual Management
        /// <summary>
        /// Create visual representation of equipped part.
        /// </summary>
        private void CreatePartVisual(PartType partType, PartData part)
        {
            PartSlot slot = _partSlots[partType];

            if (slot.slotTransform == null)
            {
                Debug.LogWarning($"[CharacterPartManager] Cannot create visual - {partType} slot transform is null!");
                return;
            }

            GameObject visual;
            GameObject visualPrefab = part.equippedPrefab;

            // If no prefab assigned, create primitive directly
            if (visualPrefab == null)
            {
                Debug.Log($"[CharacterPartManager] No prefab for {partType}, creating dummy primitive");
                visual = CreateDummyPrimitive(part);
                visual.transform.SetParent(slot.slotTransform, false);
                Debug.Log($"[CharacterPartManager] Dummy created and parented to {slot.slotTransform.name}");
            }
            else
            {
                // Instantiate visual from prefab
                Debug.Log($"[CharacterPartManager] Using prefab for {partType}");
                visual = Instantiate(visualPrefab, slot.slotTransform);
            }

            visual.name = $"{part.rarity}_{partType}_Visual";

            // Set local transform
            visual.transform.localPosition = slot.localPosition;
            visual.transform.localRotation = Quaternion.identity;
            visual.transform.localScale = slot.baseScale * part.GetRarityScale();

            Debug.Log($"[CharacterPartManager] Visual positioned at local: {visual.transform.localPosition}, scale: {visual.transform.localScale}");

            // Apply color if has renderer
            Renderer renderer = visual.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = part.partColor;
            }

            // Store reference
            slot.currentVisual = visual;

            Debug.Log($"[CharacterPartManager] Created visual for {part.rarity} {partType}");
        }

        /// <summary>
        /// Remove visual representation of part.
        /// </summary>
        private void RemovePartVisual(PartType partType)
        {
            PartSlot slot = _partSlots[partType];

            if (slot.currentVisual != null)
            {
                Destroy(slot.currentVisual);
                slot.currentVisual = null;
                Debug.Log($"[CharacterPartManager] Removed visual for {partType}");
            }
        }

        /// <summary>
        /// Create dummy primitive GameObject for part.
        /// </summary>
        private GameObject CreateDummyPrimitive(PartData part)
        {
            PrimitiveType primitiveType = part.shapeType;
            GameObject primitive = GameObject.CreatePrimitive(primitiveType);
            primitive.name = $"Dummy_{part.partType}";

            // Remove collider from visual (not needed)
            Collider collider = primitive.GetComponent<Collider>();
            if (collider != null)
            {
                Destroy(collider);
            }

            return primitive;
        }
        #endregion

        #region Query Methods
        /// <summary>
        /// Get currently equipped part of specified type.
        /// </summary>
        public PartData GetEquippedPart(PartType partType)
        {
            return _equippedParts[partType];
        }

        /// <summary>
        /// Check if part slot is occupied.
        /// </summary>
        public bool HasPart(PartType partType)
        {
            return _equippedParts[partType] != null;
        }

        /// <summary>
        /// Get all equipped parts.
        /// </summary>
        public Dictionary<PartType, PartData> GetAllEquippedParts()
        {
            return new Dictionary<PartType, PartData>(_equippedParts);
        }

        /// <summary>
        /// Get total stat bonus from all equipped parts.
        /// </summary>
        public float GetTotalStatBonus(string statType)
        {
            float total = 0f;

            foreach (var part in _equippedParts.Values)
            {
                if (part == null) continue;

                switch (statType)
                {
                    case "Health":
                        total += part.GetFinalHealthBonus();
                        break;
                    case "Damage":
                        total += part.GetFinalDamageMultiplier();
                        break;
                    case "AttackSpeed":
                        total += part.GetFinalAttackSpeedMultiplier();
                        break;
                    case "MoveSpeed":
                        total += part.GetFinalMoveSpeedMultiplier();
                        break;
                }
            }

            return total;
        }
        #endregion

        #region Debug Helpers
#if UNITY_EDITOR
        [ContextMenu("Clear All Parts")]
        private void DebugClearAllParts()
        {
            foreach (PartType partType in System.Enum.GetValues(typeof(PartType)))
            {
                if (HasPart(partType))
                {
                    UnequipPart(partType);
                }
            }
            Debug.Log("[CharacterPartManager] All parts cleared");
        }

        [ContextMenu("Print Equipped Parts")]
        private void DebugPrintEquippedParts()
        {
            Debug.Log("=== EQUIPPED PARTS ===");
            foreach (var kvp in _equippedParts)
            {
                string status = kvp.Value != null ? $"{kvp.Value.rarity} - {kvp.Value.displayName}" : "Empty";
                Debug.Log($"{kvp.Key}: {status}");
            }
            Debug.Log("======================");
        }

        [ContextMenu("Print Total Stats")]
        private void DebugPrintTotalStats()
        {
            Debug.Log("=== TOTAL PART BONUSES ===");
            Debug.Log($"Health: +{GetTotalStatBonus("Health") * 100:F0}%");
            Debug.Log($"Damage: +{GetTotalStatBonus("Damage") * 100:F0}%");
            Debug.Log($"Attack Speed: +{GetTotalStatBonus("AttackSpeed") * 100:F0}%");
            Debug.Log($"Move Speed: +{GetTotalStatBonus("MoveSpeed") * 100:F0}%");
            Debug.Log("==========================");
        }
#endif
        #endregion

        #region Helper Classes
        /// <summary>
        /// Internal class to track part slot data.
        /// </summary>
        private class PartSlot
        {
            public Transform slotTransform;
            public Vector3 localPosition;
            public Vector3 baseScale;
            public GameObject currentVisual;

            public PartSlot(Transform transform, Vector3 position, Vector3 scale)
            {
                slotTransform = transform;
                localPosition = position;
                baseScale = scale;
                currentVisual = null;
            }
        }
        #endregion
    }
}
