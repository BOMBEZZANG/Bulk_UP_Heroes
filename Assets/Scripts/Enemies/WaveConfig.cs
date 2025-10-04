using UnityEngine;

namespace BulkUpHeroes.Enemies
{
    /// <summary>
    /// Configuration data for a wave of enemies.
    /// Defines enemy count, stats, and spawn parameters.
    ///
    /// Phase 2: Basic linear progression
    /// Future: Can add special wave types, boss waves, mixed enemy types
    /// </summary>
    [System.Serializable]
    public class WaveConfig
    {
        [Header("Wave Identification")]
        public int waveNumber = 1;

        [Header("Enemy Configuration")]
        public int enemyCount = 1;           // Number of enemies to spawn
        public float enemyHealth = 50f;       // Health per enemy
        public float enemySpeed = 2f;         // Movement speed
        public float enemyDamage = 5f;        // Damage per attack

        [Header("Spawn Settings")]
        public float spawnDelay = 0.5f;       // Delay between enemy spawns

        /// <summary>
        /// Generate a wave config for a specific wave number using the standard formula.
        /// Phase 3: Rebalanced for parts progression.
        /// Wave 1: 20 HP, 8 DMG (player needs 7 hits without parts)
        /// Wave 2: 25 HP, 10 DMG (requires first part)
        /// Wave 3: 30 HP, 12 DMG (difficult without 2+ parts)
        /// Wave 5: 40 HP, 15 DMG (impossible without good parts)
        /// Wave 10: 60 HP, 25 DMG (requires near-complete set)
        /// </summary>
        public static WaveConfig GenerateForWave(int waveNumber)
        {
            return new WaveConfig
            {
                waveNumber = waveNumber,
                enemyCount = Mathf.Min(waveNumber, 10),  // Cap at 10 enemies per wave
                enemyHealth = 20f + ((waveNumber - 1) * 4.5f),
                enemySpeed = 2f + (waveNumber * 0.1f),
                enemyDamage = 8f + ((waveNumber - 1) * 1.9f),
                spawnDelay = 0.5f
            };
        }

        /// <summary>
        /// Calculate total threat level of this wave (for balancing).
        /// </summary>
        public float GetThreatLevel()
        {
            return enemyCount * enemyHealth * enemyDamage * enemySpeed;
        }

        public override string ToString()
        {
            return $"Wave {waveNumber}: {enemyCount} enemies (HP:{enemyHealth}, SPD:{enemySpeed:F1}, DMG:{enemyDamage})";
        }
    }
}
