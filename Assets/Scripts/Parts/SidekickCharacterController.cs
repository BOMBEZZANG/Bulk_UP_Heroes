using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using BulkUpHeroes.Utils;
using Synty.SidekickCharacters.API;
using Synty.SidekickCharacters.Database;
using Synty.SidekickCharacters.Database.DTO;
using Synty.SidekickCharacters.Enums;

namespace BulkUpHeroes.Parts
{
    /// <summary>
    /// Controls Sidekick character creation and part swapping at runtime.
    /// Uses Sidekick Runtime API to build characters from individual parts.
    ///
    /// Phase 4: Actual Sidekick integration
    /// </summary>
    public class SidekickCharacterController : MonoBehaviour
    {
        #region Configuration
        [Header("Sidekick Setup")]
        [SerializeField] private bool _enableSidekick = true;
        [SerializeField] private string _characterName = "Player Character";
        [SerializeField] private Transform _characterParent; // Where to spawn the character

        [Header("Base Resources")]
        [SerializeField] private string _baseModelPath = "Meshes/SK_BaseModel";
        [SerializeField] private string _baseMaterialPath = "Materials/M_BaseMaterial";
        #endregion

        #region State
        private DatabaseManager _dbManager;
        private SidekickRuntime _sidekickRuntime;
        private Dictionary<CharacterPartType, Dictionary<string, SidekickPart>> _partLibrary;
        private GameObject _currentCharacter;

        // Track which parts are currently equipped (by part name)
        private Dictionary<PartType, string> _equippedParts = new Dictionary<PartType, string>();

        private bool _isInitialized = false;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            if (_characterParent == null)
            {
                _characterParent = transform;
            }

            // Initialize equipped parts dictionary
            _equippedParts[PartType.Head] = null;
            _equippedParts[PartType.Arms] = null;
            _equippedParts[PartType.Torso] = null;
            _equippedParts[PartType.Legs] = null;

            // Initialize Sidekick in Awake so it's ready for other scripts
            if (_enableSidekick)
            {
                InitializeSidekick();
            }
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Initialize Sidekick Runtime API and load part library.
        /// </summary>
        private void InitializeSidekick()
        {
            try
            {
                Debug.Log("[SidekickCharacterController] Initializing Sidekick Runtime API...");

                // Create database manager
                _dbManager = new DatabaseManager();

                // Load base resources
                GameObject baseModel = Resources.Load<GameObject>(_baseModelPath);
                Material baseMaterial = Resources.Load<Material>(_baseMaterialPath);

                if (baseModel == null)
                {
                    Debug.LogError($"[SidekickCharacterController] Failed to load base model at '{_baseModelPath}'");
                    return;
                }

                if (baseMaterial == null)
                {
                    Debug.LogError($"[SidekickCharacterController] Failed to load base material at '{_baseMaterialPath}'");
                    return;
                }

                // Initialize Sidekick Runtime
                _sidekickRuntime = new SidekickRuntime(baseModel, baseMaterial, null, _dbManager);

                // Populate part library
                SidekickRuntime.PopulateToolData(_sidekickRuntime);
                _partLibrary = _sidekickRuntime.MappedPartDictionary;

                _isInitialized = true;

                Debug.Log($"[SidekickCharacterController] Initialized successfully. Part library loaded with {_partLibrary.Count} categories");

                // Create initial base character
                RebuildCharacter();
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[SidekickCharacterController] Failed to initialize: {ex.Message}");
                Debug.LogError($"Stack trace: {ex.StackTrace}");
                _isInitialized = false;
            }
        }
        #endregion

        #region Part Management
        /// <summary>
        /// Equip a part by name (Sidekick part ID).
        /// </summary>
        public bool EquipPart(PartType partType, string sidekickPartName)
        {
            if (!_isInitialized)
            {
                Debug.LogWarning("[SidekickCharacterController] Not initialized - cannot equip part");
                return false;
            }

            if (string.IsNullOrEmpty(sidekickPartName))
            {
                Debug.LogWarning("[SidekickCharacterController] Part name is empty");
                return false;
            }

            // Store the equipped part
            _equippedParts[partType] = sidekickPartName;

            Debug.Log($"[SidekickCharacterController] Equipping {partType}: {sidekickPartName}");

            // Rebuild entire character with new part
            RebuildCharacter();

            return true;
        }

        /// <summary>
        /// Remove a part.
        /// </summary>
        public void RemovePart(PartType partType)
        {
            if (!_isInitialized) return;

            _equippedParts[partType] = null;

            Debug.Log($"[SidekickCharacterController] Removing {partType}");

            // Rebuild character without this part
            RebuildCharacter();
        }
        #endregion

        #region Character Building
        /// <summary>
        /// Rebuild the entire character from scratch with currently equipped parts.
        /// </summary>
        private void RebuildCharacter()
        {
            if (!_isInitialized) return;

            try
            {
                // Collect all parts to use
                List<SkinnedMeshRenderer> partsToUse = new List<SkinnedMeshRenderer>();

                // Add base body parts (required)
                AddRequiredBodyParts(partsToUse);

                // Add equipped armor parts
                AddEquippedArmorParts(partsToUse);

                // Destroy old character
                if (_currentCharacter != null)
                {
                    Destroy(_currentCharacter);
                }

                // Create new character
                _currentCharacter = _sidekickRuntime.CreateCharacter(_characterName, partsToUse, false, true);

                if (_currentCharacter != null)
                {
                    _currentCharacter.transform.SetParent(_characterParent);
                    _currentCharacter.transform.localPosition = Vector3.zero;
                    _currentCharacter.transform.localRotation = Quaternion.identity;

                    Debug.Log($"[SidekickCharacterController] Character rebuilt with {partsToUse.Count} parts");
                }
                else
                {
                    Debug.LogError("[SidekickCharacterController] Failed to create character!");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[SidekickCharacterController] Error rebuilding character: {ex.Message}");
            }
        }

        /// <summary>
        /// Add required body parts (base naked character).
        /// </summary>
        private void AddRequiredBodyParts(List<SkinnedMeshRenderer> parts)
        {
            // Add basic body parts that should always be present
            // You can customize this based on what your base character needs

            // For now, we'll just add torso/hips as minimum
            TryAddPartByType(CharacterPartType.Hips, parts, "Hips_Underwear"); // Basic underwear
        }

        /// <summary>
        /// Add equipped armor parts based on what player has collected.
        /// </summary>
        private void AddEquippedArmorParts(List<SkinnedMeshRenderer> parts)
        {
            // Head
            if (!string.IsNullOrEmpty(_equippedParts[PartType.Head]))
            {
                TryAddPartByName(CharacterPartType.Head, parts, _equippedParts[PartType.Head]);
                TryAddPartByName(CharacterPartType.AttachmentHead, parts, _equippedParts[PartType.Head]);
            }

            // Arms (map to both left and right, upper and lower)
            if (!string.IsNullOrEmpty(_equippedParts[PartType.Arms]))
            {
                string armPart = _equippedParts[PartType.Arms];
                TryAddPartByName(CharacterPartType.ArmUpperLeft, parts, armPart);
                TryAddPartByName(CharacterPartType.ArmUpperRight, parts, armPart);
                TryAddPartByName(CharacterPartType.ArmLowerLeft, parts, armPart);
                TryAddPartByName(CharacterPartType.ArmLowerRight, parts, armPart);
                TryAddPartByName(CharacterPartType.HandLeft, parts, armPart);
                TryAddPartByName(CharacterPartType.HandRight, parts, armPart);
            }

            // Torso
            if (!string.IsNullOrEmpty(_equippedParts[PartType.Torso]))
            {
                TryAddPartByName(CharacterPartType.Torso, parts, _equippedParts[PartType.Torso]);
                TryAddPartByName(CharacterPartType.AttachmentBack, parts, _equippedParts[PartType.Torso]);
            }

            // Legs (map to both left and right)
            if (!string.IsNullOrEmpty(_equippedParts[PartType.Legs]))
            {
                string legPart = _equippedParts[PartType.Legs];
                TryAddPartByName(CharacterPartType.LegLeft, parts, legPart);
                TryAddPartByName(CharacterPartType.LegRight, parts, legPart);
                TryAddPartByName(CharacterPartType.FootLeft, parts, legPart);
                TryAddPartByName(CharacterPartType.FootRight, parts, legPart);
            }
        }

        /// <summary>
        /// Try to find and add a part by its name.
        /// </summary>
        private void TryAddPartByName(CharacterPartType partType, List<SkinnedMeshRenderer> parts, string partName)
        {
            if (!_partLibrary.ContainsKey(partType)) return;

            var partsOfType = _partLibrary[partType];

            // Try exact match first
            if (partsOfType.ContainsKey(partName))
            {
                AddPart(partsOfType[partName], parts);
                return;
            }

            // Try partial match (part name might be a substring)
            foreach (var kvp in partsOfType)
            {
                if (kvp.Key.Contains(partName) || partName.Contains(kvp.Key))
                {
                    AddPart(kvp.Value, parts);
                    return;
                }
            }

            Debug.LogWarning($"[SidekickCharacterController] Part '{partName}' not found in {partType} library");
        }

        /// <summary>
        /// Try to find and add any part of a specific type (for base body).
        /// </summary>
        private void TryAddPartByType(CharacterPartType partType, List<SkinnedMeshRenderer> parts, string preferredName = null)
        {
            if (!_partLibrary.ContainsKey(partType)) return;

            var partsOfType = _partLibrary[partType];

            if (partsOfType.Count == 0) return;

            // Try to find preferred part
            if (!string.IsNullOrEmpty(preferredName) && partsOfType.ContainsKey(preferredName))
            {
                AddPart(partsOfType[preferredName], parts);
                return;
            }

            // Otherwise just add first part
            AddPart(partsOfType.Values.First(), parts);
        }

        /// <summary>
        /// Add a SidekickPart to the parts list.
        /// </summary>
        private void AddPart(SidekickPart part, List<SkinnedMeshRenderer> parts)
        {
            GameObject partModel = part.GetPartModel();
            if (partModel != null)
            {
                SkinnedMeshRenderer renderer = partModel.GetComponentInChildren<SkinnedMeshRenderer>();
                if (renderer != null)
                {
                    parts.Add(renderer);
                }
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Check if Sidekick is initialized and available.
        /// </summary>
        public bool IsAvailable()
        {
            return _isInitialized;
        }

        /// <summary>
        /// Get the current character GameObject.
        /// </summary>
        public GameObject GetCurrentCharacter()
        {
            return _currentCharacter;
        }
        #endregion
    }
}
