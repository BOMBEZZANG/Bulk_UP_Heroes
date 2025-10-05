using UnityEngine;
using System.Collections.Generic;
using Synty.SidekickCharacters.API;
using Synty.SidekickCharacters.Database;
using Synty.SidekickCharacters.Database.DTO;
using Synty.SidekickCharacters.Enums;

namespace BulkUpHeroes.Parts
{
    /// <summary>
    /// Debug tool to list all available Sidekick parts in your package.
    /// Attach this to any GameObject and check the console to see what parts you have.
    /// </summary>
    public class SidekickPartLister : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private bool _listOnStart = true;
        [SerializeField] private bool _filterByUsefulParts = true;

        private DatabaseManager _dbManager;
        private SidekickRuntime _sidekickRuntime;

        private void Start()
        {
            if (_listOnStart)
            {
                ListAllParts();
            }
        }

        [ContextMenu("List All Available Parts")]
        public void ListAllParts()
        {
            Debug.Log("=== LISTING ALL SIDEKICK PARTS ===");

            try
            {
                // Initialize
                _dbManager = new DatabaseManager();
                GameObject baseModel = Resources.Load<GameObject>("Meshes/SK_BaseModel");
                Material baseMaterial = Resources.Load<Material>("Materials/M_BaseMaterial");

                if (baseModel == null || baseMaterial == null)
                {
                    Debug.LogError("Failed to load base model or material!");
                    return;
                }

                _sidekickRuntime = new SidekickRuntime(baseModel, baseMaterial, null, _dbManager);
                SidekickRuntime.PopulateToolData(_sidekickRuntime);

                var partLibrary = _sidekickRuntime.MappedPartDictionary;

                Debug.Log($"Found {partLibrary.Count} part categories");
                Debug.Log("");

                // List parts we care about for the game
                if (_filterByUsefulParts)
                {
                    ListUsefulParts(partLibrary);
                }
                else
                {
                    ListAllPartsFull(partLibrary);
                }

                Debug.Log("=== END PART LIST ===");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Error listing parts: {ex.Message}");
                Debug.LogError($"Stack: {ex.StackTrace}");
            }
        }

        private void ListUsefulParts(Dictionary<CharacterPartType, Dictionary<string, SidekickPart>> partLibrary)
        {
            Debug.Log("=== PARTS USEFUL FOR YOUR GAME ===\n");

            // HEAD PARTS
            if (partLibrary.ContainsKey(CharacterPartType.Head))
            {
                Debug.Log("--- HEAD PARTS (use for game 'Head' slot) ---");
                foreach (var partName in partLibrary[CharacterPartType.Head].Keys)
                {
                    Debug.Log($"  {partName}");
                }
                Debug.Log("");
            }

            // TORSO PARTS
            if (partLibrary.ContainsKey(CharacterPartType.Torso))
            {
                Debug.Log("--- TORSO PARTS (use for game 'Torso' slot) ---");
                foreach (var partName in partLibrary[CharacterPartType.Torso].Keys)
                {
                    Debug.Log($"  {partName}");
                }
                Debug.Log("");
            }

            // ARM PARTS (Upper Left as example)
            if (partLibrary.ContainsKey(CharacterPartType.ArmUpperLeft))
            {
                Debug.Log("--- ARM PARTS (use for game 'Arms' slot) ---");
                Debug.Log("NOTE: You'll use same name for Upper/Lower Left/Right arms");
                foreach (var partName in partLibrary[CharacterPartType.ArmUpperLeft].Keys)
                {
                    Debug.Log($"  {partName}");
                }
                Debug.Log("");
            }

            // LEG PARTS (Left as example)
            if (partLibrary.ContainsKey(CharacterPartType.LegLeft))
            {
                Debug.Log("--- LEG PARTS (use for game 'Legs' slot) ---");
                Debug.Log("NOTE: You'll use same name for both left and right legs");
                foreach (var partName in partLibrary[CharacterPartType.LegLeft].Keys)
                {
                    Debug.Log($"  {partName}");
                }
                Debug.Log("");
            }

            // HIPS PARTS
            if (partLibrary.ContainsKey(CharacterPartType.Hips))
            {
                Debug.Log("--- HIPS PARTS (also useful for 'Legs' slot) ---");
                foreach (var partName in partLibrary[CharacterPartType.Hips].Keys)
                {
                    Debug.Log($"  {partName}");
                }
                Debug.Log("");
            }
        }

        private void ListAllPartsFull(Dictionary<CharacterPartType, Dictionary<string, SidekickPart>> partLibrary)
        {
            foreach (var category in partLibrary)
            {
                Debug.Log($"\n--- {category.Key} ({category.Value.Count} parts) ---");

                foreach (var partName in category.Value.Keys)
                {
                    Debug.Log($"  {partName}");
                }
            }
        }

        [ContextMenu("List Part Categories Only")]
        public void ListCategories()
        {
            try
            {
                _dbManager = new DatabaseManager();
                GameObject baseModel = Resources.Load<GameObject>("Meshes/SK_BaseModel");
                Material baseMaterial = Resources.Load<Material>("Materials/M_BaseMaterial");

                _sidekickRuntime = new SidekickRuntime(baseModel, baseMaterial, null, _dbManager);
                SidekickRuntime.PopulateToolData(_sidekickRuntime);

                var partLibrary = _sidekickRuntime.MappedPartDictionary;

                Debug.Log("=== PART CATEGORIES ===");
                foreach (var category in partLibrary.Keys)
                {
                    Debug.Log($"{category} - {partLibrary[category].Count} parts");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Error: {ex.Message}");
            }
        }
    }
}
