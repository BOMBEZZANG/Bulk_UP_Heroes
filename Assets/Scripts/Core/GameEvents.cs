using System;
using UnityEngine;
using BulkUpHeroes.Utils;

namespace BulkUpHeroes.Core
{
    /// <summary>
    /// Central event system for game-wide communication.
    /// Allows decoupled systems to communicate without direct references.
    ///
    /// Phase 2: Combat and wave events
    /// Future: Will expand for parts, UI, and achievements
    /// </summary>
    public static class GameEvents
    {
        #region Player Events
        /// <summary>
        /// Fired when player health changes. Parameters: (currentHealth, maxHealth)
        /// </summary>
        public static event Action<float, float> OnPlayerHealthChanged;

        /// <summary>
        /// Fired when player dies.
        /// </summary>
        public static event Action OnPlayerDeath;

        /// <summary>
        /// Fired when player performs an attack. Parameter: target GameObject
        /// </summary>
        public static event Action<GameObject> OnPlayerAttack;

        /// <summary>
        /// Fired when player takes damage. Parameters: (damage amount, source GameObject)
        /// </summary>
        public static event Action<float, GameObject> OnPlayerDamaged;
        #endregion

        #region Enemy Events
        /// <summary>
        /// Fired when an enemy spawns. Parameter: enemy GameObject
        /// </summary>
        public static event Action<GameObject> OnEnemySpawned;

        /// <summary>
        /// Fired when an enemy dies. Parameter: enemy GameObject
        /// </summary>
        public static event Action<GameObject> OnEnemyDeath;

        /// <summary>
        /// Fired when an enemy attacks. Parameter: target GameObject
        /// </summary>
        public static event Action<GameObject> OnEnemyAttack;

        /// <summary>
        /// Fired when an enemy takes damage. Parameters: (enemy, damage amount)
        /// </summary>
        public static event Action<GameObject, float> OnEnemyDamaged;
        #endregion

        #region Wave Events
        /// <summary>
        /// Fired when a new wave starts. Parameter: wave number
        /// </summary>
        public static event Action<int> OnWaveStart;

        /// <summary>
        /// Fired when a wave is completed. Parameter: wave number
        /// </summary>
        public static event Action<int> OnWaveComplete;

        /// <summary>
        /// Fired when enemy count changes. Parameters: (remaining, total)
        /// </summary>
        public static event Action<int, int> OnEnemyCountChanged;
        #endregion

        #region Game Events
        /// <summary>
        /// Fired when game state changes. Parameter: new state
        /// </summary>
        public static event Action<GameState> OnGameStateChanged;

        /// <summary>
        /// Fired when game starts.
        /// </summary>
        public static event Action OnGameStart;

        /// <summary>
        /// Fired when game over occurs.
        /// </summary>
        public static event Action OnGameOver;

        /// <summary>
        /// Fired when game restarts.
        /// </summary>
        public static event Action OnGameRestart;
        #endregion

        #region Part Events
        /// <summary>
        /// Fired when a part is picked up. Parameter: PartData
        /// </summary>
        public static event Action<Parts.PartData> OnPartPickedUp;

        /// <summary>
        /// Fired when a part is equipped. Parameters: (partType, partData)
        /// </summary>
        public static event Action<PartType, Parts.PartData> OnPartEquipped;

        /// <summary>
        /// Fired when a part is unequipped. Parameter: partType
        /// </summary>
        public static event Action<PartType> OnPartUnequipped;
        #endregion

        #region Event Triggers
        // Player
        public static void TriggerPlayerHealthChanged(float current, float max)
            => OnPlayerHealthChanged?.Invoke(current, max);

        public static void TriggerPlayerDeath()
            => OnPlayerDeath?.Invoke();

        public static void TriggerPlayerAttack(GameObject target)
            => OnPlayerAttack?.Invoke(target);

        public static void TriggerPlayerDamaged(float damage, GameObject source)
            => OnPlayerDamaged?.Invoke(damage, source);

        // Enemy
        public static void TriggerEnemySpawned(GameObject enemy)
            => OnEnemySpawned?.Invoke(enemy);

        public static void TriggerEnemyDeath(GameObject enemy)
            => OnEnemyDeath?.Invoke(enemy);

        public static void TriggerEnemyAttack(GameObject target)
            => OnEnemyAttack?.Invoke(target);

        public static void TriggerEnemyDamaged(GameObject enemy, float damage)
            => OnEnemyDamaged?.Invoke(enemy, damage);

        // Wave
        public static void TriggerWaveStart(int waveNumber)
            => OnWaveStart?.Invoke(waveNumber);

        public static void TriggerWaveComplete(int waveNumber)
            => OnWaveComplete?.Invoke(waveNumber);

        public static void TriggerEnemyCountChanged(int remaining, int total)
            => OnEnemyCountChanged?.Invoke(remaining, total);

        // Game
        public static void TriggerGameStateChanged(GameState newState)
            => OnGameStateChanged?.Invoke(newState);

        public static void TriggerGameStart()
            => OnGameStart?.Invoke();

        public static void TriggerGameOver()
            => OnGameOver?.Invoke();

        public static void TriggerGameRestart()
            => OnGameRestart?.Invoke();

        // Part
        public static void TriggerPartPickedUp(Parts.PartData partData)
            => OnPartPickedUp?.Invoke(partData);

        public static void TriggerPartEquipped(PartType partType, Parts.PartData partData)
            => OnPartEquipped?.Invoke(partType, partData);

        public static void TriggerPartUnequipped(PartType partType)
            => OnPartUnequipped?.Invoke(partType);
        #endregion

        #region Cleanup
        /// <summary>
        /// Clear all event subscriptions. Call this when changing scenes.
        /// </summary>
        public static void ClearAllEvents()
        {
            OnPlayerHealthChanged = null;
            OnPlayerDeath = null;
            OnPlayerAttack = null;
            OnPlayerDamaged = null;

            OnEnemySpawned = null;
            OnEnemyDeath = null;
            OnEnemyAttack = null;
            OnEnemyDamaged = null;

            OnWaveStart = null;
            OnWaveComplete = null;
            OnEnemyCountChanged = null;

            OnGameStateChanged = null;
            OnGameStart = null;
            OnGameOver = null;
            OnGameRestart = null;

            OnPartPickedUp = null;
            OnPartEquipped = null;
            OnPartUnequipped = null;

            Debug.Log("[GameEvents] All events cleared");
        }
        #endregion
    }
}
