# Bulk Up Heroes - Phase 2 Development Specification
## Combat & Wave System Implementation

### Phase Overview

**Phase 2 Goal**: Implement automatic combat mechanics, enemy wave spawning system, and complete game loop with death/respawn functionality, building upon the Phase 1 foundation.

**Duration**: 2-3 Days  
**Prerequisites**: Phase 1 must be fully complete and tested  
**Priority**: Critical - This phase creates the core gameplay loop

---

## Phase 2 System Architecture

### Core Systems to Implement

| System | Purpose | Dependencies |
|--------|---------|--------------|
| **Combat System** | Auto-attack mechanics between player and enemies | Player, Enemy, Stats |
| **Damage System** | Handle health reduction and death | Stats, Events |
| **Wave Manager** | Control enemy spawning and progression | PoolManager, SpawnPoints |
| **Game State Manager** | Handle game flow (playing, wave complete, game over) | GameManager, UI |
| **HUD System** | Display game information | Canvas, Events |

---

## 1. Combat System Implementation

### 1.1 Auto-Attack System Requirements

#### Combat Detection Architecture
```
Detection Method: Proximity-based automatic targeting
Attack Range: 1.5 units
Target Selection: Nearest enemy first
Attack Rate: Based on AttackSpeed stat
Damage Application: Instant (no projectiles in Phase 2)
```

#### CombatManager.cs Requirements

**Location**: `Assets/Scripts/Combat/CombatManager.cs`

**Core Functionality**:
| Feature | Specification | Validation Criteria |
|---------|--------------|-------------------|
| **Target Detection** | Find enemies within 1.5 units | Updates every 0.2 seconds |
| **Auto Attack** | Attack nearest target automatically | No player input required |
| **Attack Timing** | Respect attack speed stat | 1 attack per second default |
| **Damage Calculation** | Apply attacker's damage stat | Instant damage, no miss chance |
| **Visual Feedback** | Attack animation or effect | Clear visual when attacking |
| **Target Switching** | Auto-select new target when current dies | Seamless transition |

**Component Integration**:
```
Player GameObject additions:
├── Existing Components (from Phase 1)
└── NEW: CombatManager
    ├── Attack Range: 1.5
    ├── Attack Timer: 0
    ├── Current Target: null
    ├── Is Attacking: false
    └── Attack Effect Prefab: [optional]

Enemy GameObject additions:
├── Existing Components (from Phase 1)
└── NEW: CombatManager (same script, different stats)
```

### 1.2 Damage System Implementation

#### DamageHandler.cs Requirements

**Location**: `Assets/Scripts/Combat/DamageHandler.cs`

**Damage Flow**:
```
1. Attacker calls TakeDamage(float damage) on target
2. Target's DamageHandler processes damage
3. Update health in Stats component
4. Trigger damage feedback (color flash, number popup)
5. Check for death condition
6. Broadcast appropriate events
```

**Implementation Requirements**:
| Component | Requirement | Details |
|-----------|------------|---------|
| **Damage Reception** | TakeDamage(float amount, GameObject source) | Track damage source for stats |
| **Health Update** | Modify currentHealth in Stats | Clamp between 0 and maxHealth |
| **Damage Feedback** | Visual indication of hit | Red flash for 0.1 seconds |
| **Death Check** | If health <= 0, trigger death | Call Die() method |
| **Invulnerability** | None for Phase 2 | All damage applies instantly |
| **Events** | OnDamageReceived, OnDeath | For UI and system updates |

### 1.3 Attack Animation System (Simplified)

**Basic Attack Visualization**:
```
Player Attack Visual:
- Scale punch: 1.0 → 1.2 → 1.0 over 0.2 seconds
- Optional: Rotation wobble
- Color: Brief white flash

Enemy Attack Visual:
- Scale punch: 1.0 → 1.15 → 1.0 over 0.2 seconds
- Red tint during attack
```

---

## 2. Wave Management System

### 2.1 WaveManager.cs Implementation

**Location**: `Assets/Scripts/Enemies/WaveManager.cs`

#### Wave Configuration Structure
```csharp
[System.Serializable]
public class WaveConfig {
    public int waveNumber;
    public int enemyCount;      // Equals wave number (1, 2, 3...)
    public float enemyHealth;    // 50 + (waveNumber * 10)
    public float enemySpeed;     // 2.0 + (waveNumber * 0.1)
    public float enemyDamage;    // 5 + (waveNumber * 2)
    public float spawnDelay;     // 0.5 seconds between spawns
}
```

#### Wave Progression Logic
| Wave | Enemies | Health | Speed | Damage | Total Threat |
|------|---------|--------|-------|--------|--------------|
| 1 | 1 | 50 | 2.0 | 5 | Baseline |
| 2 | 2 | 60 | 2.1 | 7 | 2.4x |
| 3 | 3 | 70 | 2.2 | 9 | 4.2x |
| 4 | 4 | 80 | 2.3 | 11 | 6.4x |
| 5 | 5 | 90 | 2.4 | 13 | 9.0x |
| 10 | 10 | 140 | 2.9 | 23 | 32.2x |

#### WaveManager State Machine
```
States:
├── WaitingToStart (Initial state)
├── SpawningEnemies (Spawning current wave)
├── WaveInProgress (All spawned, fighting)
├── WaveComplete (All enemies dead)
├── PreparingNextWave (3-second delay)
└── GameOver (Player died)
```

### 2.2 EnemySpawner.cs Implementation

**Location**: `Assets/Scripts/Enemies/EnemySpawner.cs`

**Spawning Requirements**:
| Feature | Specification | Implementation |
|---------|--------------|----------------|
| **Spawn Points** | Use 4 corner positions from Phase 1 | SpawnPoint tags |
| **Spawn Order** | Rotate through points sequentially | NE→NW→SE→SW→repeat |
| **Spawn Timing** | 0.5 second intervals | Prevent clustering |
| **Spawn Effect** | Optional: Scale from 0→1 | 0.3 second animation |
| **Pool Usage** | Get enemy from PoolManager | Never instantiate directly |
| **Enemy Setup** | Apply wave-specific stats | Before activation |

**Spawn Sequence**:
```
1. WaveManager requests X enemies
2. EnemySpawner begins spawn sequence
3. Every 0.5 seconds:
   - Get inactive enemy from pool
   - Position at next spawn point
   - Apply current wave stats
   - Activate enemy
   - Rotate to next spawn point
4. Signal WaveManager when all spawned
```

### 2.3 Wave UI Updates

**Wave Display Elements**:
```
Top Center HUD:
├── Wave Counter: "Wave: 3"
├── Enemy Counter: "Enemies: 2/5"
└── Wave Progress Bar (optional)

Center Screen Announcements:
├── "Wave 3 Complete!" (2 seconds)
├── "Wave 4 Starting..." (1 second)
├── "Get Ready!" (0.5 seconds)
└── Fade in/out animations
```

---

## 3. Death & Respawn System

### 3.1 Player Death Implementation

#### Player Death Sequence
```
1. Health reaches 0
2. Disable PlayerController (stop movement)
3. Play death animation (fall/fade)
4. Wait 1 second
5. Show Game Over screen
6. Display statistics
7. Enable restart options
```

#### Game Over Screen Requirements
**Location**: `Assets/Scripts/UI/GameOverScreen.cs`

**UI Elements**:
```
Panel_GameOver (Full screen overlay)
├── Background (Black, 80% opacity)
├── Container (Center panel)
│   ├── Title: "GAME OVER"
│   ├── Wave Reached: "Wave: 7"
│   ├── Enemies Defeated: "Enemies Killed: 23"
│   ├── Survival Time: "Time: 3:45"
│   ├── Button_Restart
│   └── Button_MainMenu (leads to restart in MVP)
```

### 3.2 Enemy Death Implementation

#### Enemy Death Sequence
```
1. Health reaches 0
2. Disable EnemyAI (stop movement)
3. Trigger death effect (scale down + fade)
4. Update WaveManager enemy count
5. Return to pool after 0.5 seconds
6. Check for wave completion
```

#### Death Effects
```
Visual Feedback:
- Scale: 1.0 → 0.0 over 0.3 seconds
- Alpha: 1.0 → 0.0 over 0.3 seconds
- Optional: Particle burst
- Color: Flash white then fade

Audio Feedback (if time permits):
- Enemy death sound
- Player death sound
- Wave complete fanfare
```

---

## 4. Game State Management

### 4.1 GameState Enum Expansion

**Location**: Update `GameManager.cs`

```csharp
public enum GameState {
    MainMenu,      // Initial state (simplified for MVP)
    Countdown,     // 3-2-1 before wave starts
    Playing,       // Active gameplay
    WaveComplete,  // Between waves
    GameOver,      // Player died
    Paused        // Optional pause state
}
```

### 4.2 State Transitions

| From State | To State | Trigger | Actions |
|------------|----------|---------|---------|
| MainMenu | Countdown | Start button | Spawn player, reset stats |
| Countdown | Playing | Timer ends | Start Wave 1 |
| Playing | WaveComplete | All enemies dead | Show wave complete UI |
| WaveComplete | Playing | 3-second timer | Start next wave |
| Playing | GameOver | Player health = 0 | Show game over screen |
| GameOver | MainMenu | Restart button | Reset everything |

### 4.3 GameManager Updates

**Required Methods**:
```csharp
- StartGame()           // Initialize first wave
- RestartGame()         // Reset all systems
- NextWave()           // Progress to next wave
- OnPlayerDeath()      // Handle game over
- OnWaveComplete()     // Handle wave completion
- UpdateGameState()    // State machine logic
```

---

## 5. HUD System Implementation

### 5.1 HUDManager.cs

**Location**: `Assets/Scripts/UI/HUDManager.cs`

#### HUD Components Structure
```
Canvas_Main/
├── Panel_HUD/
│   ├── HealthBar/
│   │   ├── Background (dark red)
│   │   ├── Fill (bright red)
│   │   └── Text_Health ("100/100")
│   ├── WaveInfo/
│   │   ├── Text_Wave ("Wave: 3")
│   │   └── Text_Enemies ("Enemies: 2/5")
│   └── Text_Time ("Time: 2:35")
├── Panel_Announcements/
│   └── Text_Announcement (center screen)
└── Panel_GameOver/ (Initially inactive)
```

### 5.2 Health Bar Implementation

**HealthBar.cs Requirements**:
```csharp
Features:
- Smooth fill animation (0.2 seconds)
- Color change based on percentage:
  - > 60%: Green
  - 30-60%: Yellow
  - < 30%: Red
- Damage flash effect
- Text overlay showing exact values
```

### 5.3 Wave Announcements

**AnnouncementSystem.cs**:
```csharp
Announcement Types:
- Wave Start: "Wave [X]" (large, fade in)
- Wave Complete: "Wave Complete!" (with particle effect)
- New Record: "New Best!" (if past previous best)
- Game Over: "Game Over" (slow fade)

Animation:
- Fade in: 0.3 seconds
- Display: 1.5 seconds  
- Fade out: 0.3 seconds
```

---

## 6. Integration Requirements

### 6.1 Script Dependencies

```
PlayerController.cs:
├── Requires: PlayerStats, CombatManager
└── Provides: Movement input to combat system

PlayerStats.cs:
├── Requires: DamageHandler
└── Provides: Health/damage values

CombatManager.cs:
├── Requires: Stats components, DamageHandler
└── Provides: Auto-attack functionality

EnemyAI.cs:
├── Requires: EnemyStats, CombatManager
└── Provides: Movement toward player

WaveManager.cs:
├── Requires: EnemySpawner, PoolManager, GameManager
└── Provides: Wave progression

GameManager.cs:
├── Requires: All systems
└── Provides: Central game flow control
```

### 6.2 Event System

**Required Events**:
```csharp
// Player Events
public static event Action<float> OnPlayerHealthChanged;
public static event Action OnPlayerDeath;
public static event Action<GameObject> OnPlayerAttack;

// Enemy Events  
public static event Action<GameObject> OnEnemySpawn;
public static event Action<GameObject> OnEnemyDeath;
public static event Action<GameObject> OnEnemyAttack;

// Wave Events
public static event Action<int> OnWaveStart;
public static event Action<int> OnWaveComplete;
public static event Action<int, int> OnEnemyCountChanged;

// Game Events
public static event Action<GameState> OnGameStateChanged;
public static event Action OnGameOver;
public static event Action OnGameRestart;
```

---

## 7. Testing Requirements

### 7.1 Combat System Tests

- [ ] Player automatically attacks nearest enemy within 1.5 units
- [ ] Attack occurs every 1 second (based on attack speed)
- [ ] Damage is applied instantly to target
- [ ] Enemy health decreases by correct damage amount
- [ ] Visual feedback occurs on both attack and hit
- [ ] Dead enemies stop being targeted
- [ ] New target is selected automatically

### 7.2 Wave System Tests

- [ ] Wave 1 spawns exactly 1 enemy
- [ ] Each wave spawns correct number of enemies
- [ ] Enemies spawn with 0.5 second delays
- [ ] Spawn points rotate correctly (NE→NW→SE→SW)
- [ ] Wave complete triggers when all enemies dead
- [ ] 3-second delay between waves
- [ ] Enemy stats increase per wave

### 7.3 Death & Respawn Tests

- [ ] Player dies when health reaches 0
- [ ] Game Over screen appears on player death
- [ ] Statistics display correctly
- [ ] Restart button resets entire game
- [ ] Enemies return to pool on death
- [ ] Death animations play correctly

### 7.4 HUD Tests

- [ ] Health bar updates in real-time
- [ ] Wave counter shows current wave
- [ ] Enemy counter shows alive/total
- [ ] Announcements appear and fade correctly
- [ ] All text is readable on mobile screens

### 7.5 Integration Tests

- [ ] Complete 5 waves without errors
- [ ] Game properly resets after game over
- [ ] No memory leaks after 10 minutes
- [ ] Consistent 30+ FPS with 10 enemies
- [ ] All systems communicate via events

---

## 8. Performance Benchmarks

| Metric | Target | Maximum | Notes |
|--------|--------|---------|-------|
| FPS (10 enemies) | 60 | 30 minimum | Mobile priority |
| Draw Calls | <50 | 75 | With UI |
| Triangles | <30k | 50k | All enemies + player |
| Memory Usage | <250MB | 400MB | Include pools |
| Attack Calculations/sec | 100 | 200 | All entities |

---

## 9. Phase 2 Deliverables

### 9.1 Script Deliverables

**Combat System**:
- [ ] `CombatManager.cs` - Auto-attack logic
- [ ] `DamageHandler.cs` - Damage reception and death
- [ ] `CombatEffects.cs` - Visual feedback for combat

**Wave System**:
- [ ] `WaveManager.cs` - Wave progression controller
- [ ] `EnemySpawner.cs` - Spawn point management
- [ ] `WaveConfig.cs` - Wave data structure

**UI System**:
- [ ] `HUDManager.cs` - Main HUD controller
- [ ] `HealthBar.cs` - Health display component
- [ ] `GameOverScreen.cs` - Death screen logic
- [ ] `AnnouncementSystem.cs` - Wave announcements

**Updated Scripts**:
- [ ] `GameManager.cs` - Added state machine
- [ ] `PlayerStats.cs` - Integrated with combat
- [ ] `EnemyStats.cs` - Integrated with combat
- [ ] `EnemyAI.cs` - Added attack behavior

### 9.2 Prefab Updates

- [ ] `Player.prefab` - Added CombatManager
- [ ] `Enemy_Basic.prefab` - Added CombatManager
- [ ] `UI/HUD.prefab` - Complete HUD
- [ ] `UI/GameOverScreen.prefab` - Game over panel

### 9.3 Scene Updates

- [ ] Wave spawn points verified
- [ ] HUD properly anchored for portrait
- [ ] Game Over screen positioned
- [ ] All systems connected via GameManager

---

## 10. Common Phase 2 Issues & Solutions

### Issue: Enemies don't attack player
**Solution**:
- Verify CombatManager on both entities
- Check attack range (1.5 units for player, 1.0 for enemy)
- Ensure DamageHandler exists on targets
- Verify layer collision matrix

### Issue: Wave doesn't progress
**Solution**:
- Check WaveManager state machine
- Verify OnEnemyDeath event is fired
- Ensure enemy count updates correctly
- Check wave complete condition

### Issue: Game Over doesn't trigger
**Solution**:
- Verify PlayerStats broadcasts OnPlayerDeath
- Check GameManager subscribes to event
- Ensure health actually reaches 0 (not negative)

### Issue: Performance drops with many enemies
**Solution**:
- Verify object pooling is working
- Reduce attack check frequency (0.3 seconds instead of 0.2)
- Disable shadows on all objects
- Use simpler attack effects

---

## 11. Phase 2 Completion Criteria

**Phase 2 is COMPLETE when:**

1. **Combat works automatically**
   - Player and enemies attack without input
   - Damage is applied correctly
   - Death removes entities properly

2. **Waves progress correctly**
   - Each wave spawns correct number
   - Difficulty increases per wave
   - Wave complete/start announcements work

3. **Game loop is complete**
   - Start → Play → Die → Game Over → Restart
   - All transitions work smoothly
   - Stats track correctly

4. **HUD displays information**
   - Health bar updates in real-time
   - Wave/enemy counters accurate
   - Announcements appear correctly

5. **Performance is acceptable**
   - 30+ FPS with 10 enemies on mobile
   - No memory leaks
   - Smooth gameplay

---

## 12. Handoff to Phase 3

### Required for Phase 3 Success
- [ ] All Phase 2 systems tested and stable
- [ ] Combat system easily extendable for part bonuses
- [ ] Enemy death location tracked for part drops
- [ ] Stats system ready for modifiers
- [ ] Clean event system for part pickups

### Documentation Needed
- [ ] List of all combat events
- [ ] Current stat balance values
- [ ] Wave progression formula
- [ ] Known issues or limitations

---

## Phase 2 Sign-off

| Role | Name | Date | Notes |
|------|------|------|-------|
| Developer | | | |
| QA Tester | | | |
| Tech Lead | | | |

**Next Phase**: Phase 3 - Parts & Progression System

---

*Note: This document assumes Phase 1 is complete with all movement and basic enemy systems working. Phase 2 focuses solely on making these entities fight each other with progressive difficulty.*