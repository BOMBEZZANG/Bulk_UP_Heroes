using UnityEngine;

namespace BulkUpHeroes.Utils
{
    /// <summary>
    /// Interface for any entity that can take damage.
    /// Allows for consistent damage handling across players, enemies, and future destructibles.
    /// </summary>
    public interface IDamageable
    {
        /// <summary>
        /// Apply damage to this entity.
        /// </summary>
        /// <param name="amount">Amount of damage to apply</param>
        /// <param name="source">The GameObject that caused the damage (optional)</param>
        void TakeDamage(float amount, GameObject source = null);

        /// <summary>
        /// Check if this entity is currently alive.
        /// </summary>
        bool IsAlive { get; }

        /// <summary>
        /// Current health value.
        /// </summary>
        float CurrentHealth { get; }

        /// <summary>
        /// Maximum health value.
        /// </summary>
        float MaxHealth { get; }
    }

    /// <summary>
    /// Interface for entities that can be pooled (reused instead of destroyed).
    /// Essential for mobile optimization.
    /// </summary>
    public interface IPoolable
    {
        /// <summary>
        /// Called when object is retrieved from pool.
        /// Reset object to default state here.
        /// </summary>
        void OnSpawnFromPool();

        /// <summary>
        /// Called when object is returned to pool.
        /// Clean up references and disable components here.
        /// </summary>
        void OnReturnToPool();

        /// <summary>
        /// The GameObject associated with this poolable object.
        /// </summary>
        GameObject GameObject { get; }
    }

    /// <summary>
    /// Interface for entities that can attack.
    /// Allows for different attack implementations (melee, ranged, area, etc.).
    /// </summary>
    public interface IAttacker
    {
        /// <summary>
        /// Execute an attack action.
        /// </summary>
        /// <param name="target">The target to attack</param>
        void Attack(IDamageable target);

        /// <summary>
        /// Check if this attacker can currently attack.
        /// </summary>
        bool CanAttack { get; }

        /// <summary>
        /// Damage dealt by this attacker.
        /// </summary>
        float Damage { get; }

        /// <summary>
        /// Time between attacks in seconds.
        /// </summary>
        float AttackSpeed { get; }
    }

    /// <summary>
    /// Interface for entities with stats that can be modified.
    /// Used for applying part bonuses and temporary buffs.
    /// </summary>
    public interface IStatsModifiable
    {
        /// <summary>
        /// Apply a stat modifier to this entity.
        /// </summary>
        /// <param name="statType">Which stat to modify</param>
        /// <param name="value">Amount to add (can be negative)</param>
        void ModifyStat(string statType, float value);

        /// <summary>
        /// Get current value of a specific stat.
        /// </summary>
        /// <param name="statType">Which stat to retrieve</param>
        /// <returns>Current stat value</returns>
        float GetStat(string statType);
    }

    /// <summary>
    /// Interface for game state observers.
    /// Used for systems that need to respond to game state changes.
    /// </summary>
    public interface IGameStateObserver
    {
        /// <summary>
        /// Called when game state changes.
        /// </summary>
        /// <param name="newState">The new game state</param>
        void OnGameStateChanged(GameState newState);
    }
}
