using UnityEngine;
using BulkUpHeroes.Parts;
using BulkUpHeroes.Player;

namespace BulkUpHeroes.Debugging
{
    /// <summary>
    /// Debug helper to diagnose Sidekick integration issues.
    /// Add this to Player GameObject temporarily to debug.
    /// </summary>
    public class SidekickDebugHelper : MonoBehaviour
    {
        [Header("References to Check")]
        [SerializeField] private SidekickPartSwapper _partSwapper;
        [SerializeField] private CharacterPartManager _partManager;

        [Header("Debug Actions")]
        [SerializeField] private bool _logSetupOnStart = true;
        [SerializeField] private bool _logOnPartEquip = true;

        private void Start()
        {
            if (_logSetupOnStart)
            {
                LogSetupStatus();
            }
        }

        [ContextMenu("Log Setup Status")]
        public void LogSetupStatus()
        {
            Debug.Log("=== SIDEKICK SETUP DIAGNOSTIC ===");

            // Check SidekickPartSwapper
            if (_partSwapper == null)
            {
                _partSwapper = GetComponent<SidekickPartSwapper>();
            }

            if (_partSwapper == null)
            {
                Debug.LogError("❌ SidekickPartSwapper NOT FOUND on Player!");
            }
            else
            {
                Debug.Log("✓ SidekickPartSwapper found");

                // Check if Sidekick is available
                bool sidekickAvailable = _partSwapper.IsSidekickAvailable();
                Debug.Log($"Sidekick Available: {sidekickAvailable}");

                if (!sidekickAvailable)
                {
                    Debug.LogWarning("⚠ Sidekick not available - will use fallback dummy visuals");
                }

                // Check Sidekick controller reference
                SidekickCharacterController controller = _partSwapper.GetSidekickController();
                if (controller == null)
                {
                    Debug.LogWarning("⚠ SidekickCharacterController not found!");
                    Debug.Log("SOLUTION: Add SidekickCharacterController component to Player GameObject");
                }
                else
                {
                    Debug.Log($"✓ SidekickCharacterController found");

                    if (controller.IsAvailable())
                    {
                        Debug.Log("✓ Sidekick initialized successfully!");
                        GameObject character = controller.GetCurrentCharacter();
                        if (character != null)
                        {
                            Debug.Log($"✓ Character created: {character.name}");
                        }
                    }
                    else
                    {
                        Debug.LogWarning("⚠ SidekickCharacterController not initialized");
                        Debug.Log("Check console for initialization errors");
                    }
                }
            }

            // Check CharacterPartManager
            if (_partManager == null)
            {
                _partManager = GetComponent<CharacterPartManager>();
            }

            if (_partManager == null)
            {
                Debug.LogError("❌ CharacterPartManager NOT FOUND!");
            }
            else
            {
                Debug.Log("✓ CharacterPartManager found");
            }

            Debug.Log("ℹ Sidekick builds character at runtime (no BaseCharacter prefab needed)");

            Debug.Log("=== END DIAGNOSTIC ===");
        }

        [ContextMenu("Test Part Swap")]
        public void TestPartSwap()
        {
            Debug.Log("=== TESTING PART SWAP ===");

            if (_partSwapper == null)
            {
                Debug.LogError("Cannot test - no SidekickPartSwapper!");
                return;
            }

            // Try to swap a test part
            bool usedSidekick;
            Transform slot = _partSwapper.SwapPart(Utils.PartType.Head, "Test_Head_ID", out usedSidekick);

            Debug.Log($"Swap result - Used Sidekick: {usedSidekick}, Returned slot: {slot}");

            Debug.Log("=== END TEST ===");
        }

        [ContextMenu("Check All PartData Assets")]
        public void CheckPartDataAssets()
        {
            Debug.Log("=== CHECKING PARTDATA ASSETS ===");

            // Find all PartData assets
            string[] guids = UnityEditor.AssetDatabase.FindAssets("t:PartData");

            int missingIDs = 0;
            foreach (string guid in guids)
            {
                string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                PartData part = UnityEditor.AssetDatabase.LoadAssetAtPath<PartData>(path);

                if (part != null)
                {
                    bool hasID = !string.IsNullOrEmpty(part.sidekickPartID);
                    string status = hasID ? "✓" : "❌";
                    Debug.Log($"{status} {part.displayName} ({part.partType} - {part.rarity}) - ID: '{part.sidekickPartID}'");

                    if (!hasID)
                    {
                        missingIDs++;
                        Debug.LogWarning($"Missing Sidekick Part ID for: {part.displayName} at {path}");
                    }
                }
            }

            if (missingIDs > 0)
            {
                Debug.LogWarning($"⚠ {missingIDs} PartData assets are missing Sidekick Part IDs!");
                Debug.Log("SOLUTION: Fill in 'Sidekick Part ID' field for each PartData asset in Inspector");
            }
            else
            {
                Debug.Log("✓ All PartData assets have Sidekick Part IDs set");
            }

            Debug.Log("=== END CHECK ===");
        }
    }
}
