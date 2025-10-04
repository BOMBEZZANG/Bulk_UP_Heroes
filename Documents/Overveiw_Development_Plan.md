# Bulk Up Heroes - MVP Development Specification

## 1. Project Overview

### 1.1 Game Concept
**Bulk Up Heroes** is a mobile top-down arena combat game where players start with a minimal character and progressively "bulk up" by collecting and equipping modular character parts in real-time during combat.

### 1.2 Core Gameplay Loop
1. Player starts with basic/naked character in small arena
2. Defeat enemies to earn parts
3. Instantly equip parts to become stronger
4. Face increasingly difficult waves
5. Survive as long as possible while building your hero

### 1.3 Target Platform
- **Primary**: Mobile (iOS/Android)
- **Orientation**: Portrait mode
- **Input**: Touch controls only

## 2. Technical Requirements

### 2.1 Unity Setup
```
Unity Version: 2022.3 LTS or newer
Render Pipeline: URP (Universal Render Pipeline)
Platform: Mobile
Orientation: Portrait
Target Frame Rate: 60 FPS
```

### 2.2 Required Assets
- **Synty Sidekick Modular Characters FREE** (already imported)
- Built-in Unity packages:
  - Input System (new)
  - TextMeshPro
  - URP

### 2.3 Scene Setup
```
Scene Name: Arena_Main
Camera Setup:
  - Position: (0, 10, 0)
  - Rotation: (90, 0, 0)
  - Projection: Orthographic
  - Orthographic Size: 5
  - Clear Flags: Solid Color
  - Background: Dark gray (#2C2C2C)
```

## 3. Core Systems Implementation

### 3.1 Arena Setup
```csharp
Arena Specifications:
- Size: 10x10 units
- Floor: Single plane with grid texture
- Walls: 4 cube barriers at edges (invisible colliders)
- Lighting: Single directional light from above
- No decorations or props in MVP
```

**Implementation Requirements:**
- Create prefab: "Arena_Base"
- Add boundary colliders to prevent player/enemy escape
- Set up layers: "Ground", "Wall", "Player", "Enemy", "Pickup"

### 3.2 Character Controller System

**File**: `PlayerController.cs`

```csharp
Required Features:
- Movement: Virtual joystick (bottom-left screen)
- Speed: Base 5 units/second (modifiable by parts)
- Rotation: Face movement direction
- Collision: Rigidbody-based with capsule collider

Touch Input Zones:
- Left half screen: Movement joystick
- Right half screen: Future skill button (not in MVP)
```

**Implementation Details:**
```csharp
public class PlayerController : MonoBehaviour {
    [Header("Movement")]
    public float baseSpeed = 5f;
    public float currentSpeed;
    
    [Header("Combat Stats")]
    public float damage = 10f;
    public float attackSpeed = 1f;
    public float health = 100f;
    public float maxHealth = 100f;
    
    private Rigidbody rb;
    private Vector3 moveDirection;
    
    // Joystick input handling
    // Auto-rotation to face movement
    // Boundary clamping within arena
}
```

### 3.3 Part System

**File**: `PartSystem.cs`

```csharp
Part Types:
1. Head - Affects: Attack Speed
2. Torso - Affects: Max Health
3. Arms - Affects: Damage
4. Legs - Affects: Movement Speed

Part Rarity (MVP - visual only):
- Common: Gray outline
- Rare: Blue outline
- Epic: Purple outline
```

**Part Data Structure:**
```csharp
[System.Serializable]
public class PartData {
    public PartType type;
    public GameObject prefab;
    public Rarity rarity;
    public float statBonus;
    public string partID;
}

public enum PartType {
    Head,
    Torso,
    Arms,
    Legs
}
```

### 3.4 Combat System

**File**: `CombatManager.cs`

**Combat Mechanics:**
- **Auto-attack**: Automatically attack nearest enemy within 1.5 units
- **Attack Rate**: 1 attack per second (modifiable)
- **Damage**: Base 10 (modifiable by parts)
- **No projectiles in MVP** - instant damage on attack

**Implementation:**
```csharp
public class CombatManager : MonoBehaviour {
    private float attackTimer;
    private Transform currentTarget;
    
    void Update() {
        // Find nearest enemy
        // If enemy in range (1.5 units)
        // Attack automatically every attackSpeed seconds
        // Play attack animation (if available)
        // Apply damage instantly
    }
}
```

### 3.5 Enemy System

**File**: `EnemyAI.cs`

**Enemy Behavior (MVP):**
```csharp
Enemy Types: 1 (basic melee)
Movement: Direct path to player
Speed: 3 units/second
Health: 50
Damage: 5
Attack Range: 1 unit
Behavior: Chase → Attack when in range
```

**Wave System:**
```csharp
Wave 1: 1 enemy
Wave 2: 2 enemies
Wave 3: 3 enemies
Wave n: n enemies (cap at 10 per wave)

Spawn Points: 4 corners of arena
Spawn Delay: 0.5 seconds between each enemy
Wave Delay: 3 seconds after last enemy killed
```

### 3.6 Part Collection System

**File**: `PartPickup.cs`

**Pickup Mechanics:**
1. Enemy dies → Drop part at death location
2. Part glows/rotates to attract attention
3. Player walks over → Auto-pickup
4. Instant equip with visual feedback
5. Update stats immediately

**Visual Feedback:**
```csharp
On Pickup:
- Flash effect on character
- +Stats text popup
- Part swap animation (0.3 seconds)
- Sound effect (if time permits)
```

## 4. UI Systems

### 4.1 HUD Layout
```
Top Bar:
- Wave Counter: "Wave: 5"
- Enemy Counter: "Enemies: 3/8"

Top-Left:
- Health Bar (red bar with white text)

Bottom-Left:
- Virtual Joystick (semi-transparent)

Center Screen (when needed):
- Wave Complete text
- Game Over screen
```

### 4.2 Game Over Screen
```
Elements:
- "GAME OVER" title
- "Wave Reached: X"
- "Enemies Defeated: X"
- [Restart] button
- [Main Menu] button (leads back to restart in MVP)
```

## 5. Synty Sidekick Integration

### 5.1 Runtime Part Swapping
```csharp
Required Implementation:
1. Initialize Sidekick Character Creator
2. Create method: SwapPart(PartType type, GameObject partPrefab)
3. Ensure smooth transition (no frame drops)
4. Maintain animations during swap
```

### 5.2 Part Pool Setup
```csharp
MVP Part List (use free assets only):
- 3 Head variations
- 3 Torso variations  
- 3 Arm variations
- 3 Leg variations

Total: 12 parts cycling through drops
```

## 6. Game Flow State Machine

```csharp
public enum GameState {
    MainMenu,    // Simple start screen
    Playing,     // Active gameplay
    WaveComplete,// 2-second pause between waves
    GameOver,    // Death screen
    Paused       // Pause menu (optional for MVP)
}
```

**State Transitions:**
1. MainMenu → Playing: Tap "Start"
2. Playing → WaveComplete: All enemies defeated
3. WaveComplete → Playing: After 2 seconds
4. Playing → GameOver: Player health = 0
5. GameOver → MainMenu: Tap "Restart"

## 7. Performance Optimization

### 7.1 Mobile Optimization Requirements
- **Object Pooling**: Enemies and pickups
- **Max Enemies**: 10 on screen
- **Max Pickups**: 5 on screen
- **Texture Size**: 512x512 max for parts
- **Draw Calls**: Under 50
- **Triangle Count**: Under 50k total

### 7.2 Required Optimizations
```csharp
1. Pool all enemies at start
2. Pool all pickup effects
3. Disable shadows on mobile
4. Use mobile shaders
5. Bake character after 3 seconds of no changes
```

## 8. MVP Feature List (Priority Order)

### Must Have (Week 1)
- [x] Basic arena setup
- [x] Player movement with touch
- [x] Enemy spawn and basic AI
- [x] Player-enemy collision
- [x] Health and death system
- [x] Wave progression

### Must Have (Week 2)
- [x] Part pickup system
- [x] Visual part swapping
- [x] Stat modifications from parts
- [x] Basic HUD
- [x] Game over/restart flow

### Nice to Have (If Time Permits)
- [ ] Sound effects
- [ ] Particle effects
- [ ] Enemy varieties
- [ ] Special abilities
- [ ] Score system
- [ ] Haptic feedback

## 9. Project Structure

```
Assets/
├── Scripts/
│   ├── Player/
│   │   ├── PlayerController.cs
│   │   └── PlayerStats.cs
│   ├── Combat/
│   │   ├── CombatManager.cs
│   │   └── DamageHandler.cs
│   ├── Enemies/
│   │   ├── EnemyAI.cs
│   │   ├── EnemySpawner.cs
│   │   └── WaveManager.cs
│   ├── Parts/
│   │   ├── PartSystem.cs
│   │   ├── PartPickup.cs
│   │   └── PartData.cs
│   ├── UI/
│   │   ├── HUDManager.cs
│   │   ├── VirtualJoystick.cs
│   │   └── GameOverScreen.cs
│   └── Core/
│       ├── GameManager.cs
│       └── ObjectPool.cs
├── Prefabs/
│   ├── Player/
│   ├── Enemies/
│   ├── Parts/
│   ├── UI/
│   └── Arena/
├── Materials/
├── Textures/
└── Scenes/
    └── Arena_Main.unity
```

## 10. Testing Checklist

### Core Functionality
- [ ] Player can move in all directions
- [ ] Enemies spawn correctly
- [ ] Enemies chase player
- [ ] Combat works (auto-attack)
- [ ] Parts drop on enemy death
- [ ] Parts can be picked up
- [ ] Parts visually change character
- [ ] Stats update with parts
- [ ] Waves progress correctly
- [ ] Game over triggers on death
- [ ] Restart works properly

### Mobile Specific
- [ ] Touch controls responsive
- [ ] Runs at 30+ FPS on mid-range devices
- [ ] No memory leaks over 10-minute session
- [ ] UI scales correctly on different screens

## 11. Delivery Milestones

### Day 1-2: Foundation
- Arena setup
- Player controller
- Basic enemy AI
- Touch input

### Day 3-4: Combat
- Auto-attack system
- Enemy spawning
- Wave management
- Health/death

### Day 5-6: Parts System
- Synty integration
- Part pickups
- Stat system
- Visual swapping

### Day 7: Polish
- UI implementation
- Game flow
- Testing
- Bug fixes

## 12. Success Criteria

The MVP is considered complete when:
1. Player can survive multiple waves
2. Parts visibly change character
3. Difficulty increases each wave
4. Game runs smoothly on mobile
5. Full game loop works (Start → Play → Die → Restart)

---

**Note to Developer**: Focus on getting the core loop working first. All visual polish, effects, and additional features can be added after the MVP is functional. The key is having a playable game where picking up parts makes you visibly and mechanically stronger.