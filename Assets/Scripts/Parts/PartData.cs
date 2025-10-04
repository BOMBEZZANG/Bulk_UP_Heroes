using UnityEngine;
using BulkUpHeroes.Utils;

namespace BulkUpHeroes.Parts
{
    /// <summary>
    /// ScriptableObject that defines a character part's properties and stat modifiers.
    ///
    /// Phase 3: Dummy visuals using colored primitives
    /// Future: Will reference actual 3D model assets
    /// </summary>
    [CreateAssetMenu(fileName = "NewPart", menuName = "Bulk Up Heroes/Part Data")]
    public class PartData : ScriptableObject
    {
        #region Identification
        [Header("Part Identification")]
        [Tooltip("Unique identifier for this part")]
        public string partID;

        [Tooltip("Type of part (Head, Arms, Torso, Legs)")]
        public PartType partType;

        [Tooltip("Rarity level affecting stat multipliers")]
        public Rarity rarity;

        [Tooltip("Display name for UI")]
        public string displayName;
        #endregion

        #region Stat Modifiers
        [Header("Stat Modifiers (Additive %)")]
        [Tooltip("Health bonus percentage (e.g., 1.0 = +100%)")]
        [Range(0f, 3f)]
        public float healthBonus = 0f;

        [Tooltip("Damage multiplier (e.g., 1.0 = +100%)")]
        [Range(0f, 3f)]
        public float damageMultiplier = 0f;

        [Tooltip("Attack speed multiplier (e.g., 0.5 = +50%)")]
        [Range(0f, 2f)]
        public float attackSpeedMultiplier = 0f;

        [Tooltip("Move speed multiplier (e.g., 0.3 = +30%)")]
        [Range(0f, 2f)]
        public float moveSpeedMultiplier = 0f;
        #endregion

        #region Visual Data (Dummy Phase)
        [Header("Dummy Visual Data")]
        [Tooltip("Colored primitive for pickup")]
        public GameObject dropPrefab;

        [Tooltip("Colored primitive when equipped")]
        public GameObject equippedPrefab;

        [Tooltip("Color for particle effects and UI")]
        public Color partColor = Color.white;

        [Tooltip("Primitive shape type (for reference)")]
        public PrimitiveType shapeType;
        #endregion

        #region Calculated Properties
        /// <summary>
        /// Get the rarity multiplier for stat bonuses.
        /// </summary>
        public float GetRarityMultiplier()
        {
            return rarity switch
            {
                Rarity.Common => 1.0f,
                Rarity.Rare => 1.5f,
                Rarity.Epic => 2.0f,
                _ => 1.0f
            };
        }

        /// <summary>
        /// Get final health bonus with rarity multiplier applied.
        /// </summary>
        public float GetFinalHealthBonus()
        {
            return healthBonus * GetRarityMultiplier();
        }

        /// <summary>
        /// Get final damage multiplier with rarity applied.
        /// </summary>
        public float GetFinalDamageMultiplier()
        {
            return damageMultiplier * GetRarityMultiplier();
        }

        /// <summary>
        /// Get final attack speed multiplier with rarity applied.
        /// </summary>
        public float GetFinalAttackSpeedMultiplier()
        {
            return attackSpeedMultiplier * GetRarityMultiplier();
        }

        /// <summary>
        /// Get final move speed multiplier with rarity applied.
        /// </summary>
        public float GetFinalMoveSpeedMultiplier()
        {
            return moveSpeedMultiplier * GetRarityMultiplier();
        }

        /// <summary>
        /// Get visual scale based on rarity.
        /// </summary>
        public float GetRarityScale()
        {
            return rarity switch
            {
                Rarity.Common => 1.0f,
                Rarity.Rare => 1.2f,
                Rarity.Epic => 1.4f,
                _ => 1.0f
            };
        }

        /// <summary>
        /// Get color for UI display based on rarity.
        /// </summary>
        public Color GetRarityColor()
        {
            return rarity switch
            {
                Rarity.Common => new Color(0.7f, 0.7f, 0.7f), // Gray
                Rarity.Rare => new Color(0.2f, 0.5f, 1.0f),   // Blue
                Rarity.Epic => new Color(0.6f, 0.2f, 0.8f),   // Purple
                _ => Color.white
            };
        }
        #endregion

        #region Debug Helpers
        /// <summary>
        /// Get formatted string describing this part's stats.
        /// </summary>
        public override string ToString()
        {
            return $"{rarity} {partType} - {displayName}\n" +
                   $"HP: +{GetFinalHealthBonus() * 100:F0}%, " +
                   $"DMG: +{GetFinalDamageMultiplier() * 100:F0}%, " +
                   $"AS: +{GetFinalAttackSpeedMultiplier() * 100:F0}%, " +
                   $"SPD: +{GetFinalMoveSpeedMultiplier() * 100:F0}%";
        }

        /// <summary>
        /// Validate part data on creation/modification.
        /// </summary>
        private void OnValidate()
        {
            // Auto-generate partID if empty
            if (string.IsNullOrEmpty(partID))
            {
                partID = $"{partType}_{rarity}_{System.Guid.NewGuid().ToString().Substring(0, 8)}";
            }

            // Auto-generate display name if empty
            if (string.IsNullOrEmpty(displayName))
            {
                displayName = $"{rarity} {partType}";
            }
        }
        #endregion
    }
}
