namespace BulkUpHeroes.Utils
{
    /// <summary>
    /// Defines all game states for the main game flow state machine.
    /// </summary>
    public enum GameState
    {
        MainMenu,       // Initial start screen
        Playing,        // Active gameplay
        WaveComplete,   // Pause between waves
        GameOver,       // Death screen
        Paused          // Pause menu
    }

    /// <summary>
    /// Types of character parts that can be equipped.
    /// Each type affects different player statistics.
    /// </summary>
    public enum PartType
    {
        Head,   // Affects: Attack Speed
        Torso,  // Affects: Max Health
        Arms,   // Affects: Damage
        Legs    // Affects: Movement Speed
    }

    /// <summary>
    /// Rarity levels for parts (visual differentiation in MVP).
    /// Can be expanded for gameplay effects in future phases.
    /// </summary>
    public enum Rarity
    {
        Common,     // Gray outline
        Rare,       // Blue outline
        Epic        // Purple outline
    }

    /// <summary>
    /// Types of enemies in the game.
    /// Currently only basic melee for Phase 1.
    /// </summary>
    public enum EnemyType
    {
        BasicMelee,     // Phase 1: Simple chase and attack
        FastMelee,      // Future: Quick but weak
        HeavyMelee,     // Future: Slow but strong
        Ranged          // Future: Keeps distance and shoots
    }

    /// <summary>
    /// Current state of AI behavior.
    /// Allows for future expansion of enemy behaviors.
    /// </summary>
    public enum AIState
    {
        Idle,       // Not doing anything
        Chasing,    // Moving toward target
        Attacking,  // Executing attack
        Fleeing,    // Running away (future)
        Dead        // Enemy is dead
    }

    /// <summary>
    /// Touch input zones for different controls.
    /// </summary>
    public enum InputZone
    {
        None,
        LeftHalf,   // Movement joystick
        RightHalf   // Future: Attack/skill buttons
    }

    /// <summary>
    /// Types of stat modifiers for applying bonuses.
    /// Phase 3: Used for part stat bonuses
    /// </summary>
    public enum StatModifierType
    {
        Flat,           // Adds flat value (e.g., +10 health)
        PercentAdd,     // Adds percentage of base value (e.g., +50% = base * 1.5)
        PercentMult     // Multiplies total value (e.g., 1.5x total)
    }
}
