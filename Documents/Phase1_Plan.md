# Bulk Up Heroes - Phase 1 Development Specification
## Foundation & Core Movement System

### Phase Overview

**Phase 1 Goal**: Establish the fundamental game environment with a fully functional arena, player character, and basic enemy system with proper mobile touch controls.

**Duration**: 2-3 Days  
**Priority**: Critical - All subsequent phases depend on Phase 1 completion

---

## Development Phases Breakdown

### Overall Development Roadmap

| Phase | Name | Core Deliverables | Duration |
|-------|------|------------------|----------|
| **Phase 1** | **Foundation & Movement** | Arena, Player Control, Basic Enemy | 2-3 days |
| **Phase 2** | **Combat & Wave System** | Auto-combat, Enemy Waves, Death/Respawn | 2-3 days |
| **Phase 3** | **Parts & Progression** | Synty Integration, Part Pickups, Stats | 3-4 days |
| **Phase 4** | **Polish & Game Loop** | UI, Game States, Optimization | 2-3 days |

---

## Phase 1: Detailed Requirements

### 1.1 Project Setup Requirements

#### Unity Configuration
- **Create New Project**: "BulkUpHeroes"
- **Unity Version**: 2022.3 LTS (mandatory)
- **Template**: 3D URP Mobile
- **Company Name**: Set your organization name
- **Product Name**: "Bulk Up Heroes"
- **Package Name**: com.yourcompany.bulkupheroes

#### Project Settings Configuration
```
Edit > Project Settings > Player:
- Orientation: Portrait (disable all other orientations)
- Minimum API Level: Android 7.0 (API 24)
- Target API Level: Latest
- iOS Minimum Version: 12.0
- Graphics APIs: Remove Vulkan (Android), keep OpenGLES3
- Color Space: Linear
```

#### Required Package Installation
```
Window > Package Manager:
1. Input System (com.unity.inputsystem) - Latest
2. TextMeshPro - Import TMP Essentials when prompted
3. Synty Sidekick Modular Characters FREE - Already imported
```

### 1.2 Scene Setup Requirements

#### Scene Hierarchy Structure
```
Arena_Main (Scene)
├── Managers (Empty GameObject)
│   └── GameManager
├── Environment
│   ├── Arena
│   │   ├── Floor
│   │   ├── Walls (Empty Parent)
│   │   │   ├── Wall_North
│   │   │   ├── Wall_South
│   │   │   ├── Wall_East
│   │   │   └── Wall_West
│   │   └── SpawnPoints (Empty Parent)
│   │       ├── SpawnPoint_NE
│   │       ├── SpawnPoint_NW
│   │       ├── SpawnPoint_SE
│   │       └── SpawnPoint_SW
│   └── Lighting
│       └── Directional Light
├── Camera
│   └── Main Camera
├── Player (Will be created)
├── Enemies (Empty Parent)
└── UI
    └── Canvas
        └── TouchInput
```

#### Camera Configuration
| Property | Value | Validation |
|----------|-------|------------|
| Position | (0, 10, 0) | Must be exact |
| Rotation | (90, 0, 0) | Looking straight down |
| Projection | Orthographic | Required for 2D-style view |
| Size | 6 | Adjust based on arena visibility |
| Near Clip | 0.3 | Default |
| Far Clip | 20 | Sufficient for arena |
| Background | Solid Color #1a1a1a | Dark gray |

#### Arena Specifications
| Component | Specifications | Validation Criteria |
|-----------|---------------|-------------------|
| **Floor** | 10x10 unit plane | Visible grid or texture |
| **Material** | Simple grid or solid color | Must contrast with characters |
| **Walls** | Invisible colliders at edges | Player cannot pass through |
| **Height** | 2 units tall (colliders) | Prevents jumping out |
| **Spawn Points** | 4 corners, 1 unit from walls | Gizmos visible in Scene view |

### 1.3 Player System Requirements

#### Player GameObject Setup
```
Player (GameObject)
├── Components Required:
│   ├── CharacterController OR Rigidbody
│   ├── CapsuleCollider
│   ├── PlayerController.cs
│   ├── PlayerStats.cs
│   └── Animator (if using Synty animations)
└── Child Objects:
    └── CharacterModel (Synty Sidekick prefab)
```

#### PlayerController Implementation Requirements

| Feature | Requirement | Acceptance Criteria |
|---------|------------|-------------------|
| **Movement Input** | Virtual joystick on left side of screen | Responds to touch drag |
| **Movement Speed** | 5 units/second base speed | Smooth, consistent movement |
| **Rotation** | Face movement direction | Instant rotation, no lag |
| **Bounds Checking** | Cannot leave 10x10 arena | Stops at wall collision |
| **Dead Zone** | 0.1 units joystick threshold | Prevents drift |
| **Max Speed** | Capped at defined speed | No acceleration exploits |

#### Touch Input System Requirements

**Virtual Joystick Specifications:**
- **Position**: Bottom-left quadrant of screen
- **Size**: 150x150 pixels base
- **Appearance**: Semi-transparent (50% alpha)
- **Behavior**: 
  - Appears on touch down
  - Follows finger within bounds
  - Disappears on touch up
- **Visual Elements**:
  - Outer ring (boundary)
  - Inner circle (thumb position)
  - Optional: Direction indicator

#### Player Stats Structure
```
Base Stats (Phase 1):
- Health: 100
- Max Health: 100
- Move Speed: 5.0
- Damage: 10 (for Phase 2)
- Attack Speed: 1.0 (for Phase 2)
```

### 1.4 Basic Enemy System Requirements

#### Enemy GameObject Setup
```
Enemy (GameObject)
├── Components Required:
│   ├── Rigidbody (Kinematic)
│   ├── CapsuleCollider
│   ├── EnemyAI.cs
│   ├── EnemyStats.cs
│   └── Animator (optional for Phase 1)
└── Child Objects:
    └── EnemyModel (Capsule or Synty model)
```

#### Enemy AI Requirements (Phase 1 - Simplified)

| Behavior | Specification | Validation |
|----------|--------------|------------|
| **Detection** | Always knows player position | Immediate response |
| **Movement** | Direct path to player | Straight line movement |
| **Speed** | 2 units/second | Slower than player |
| **Collision** | Stops when touching player | No overlap |
| **Spawning** | Manual placement for testing | 1-2 enemies in scene |
| **Rotation** | Face player direction | Smooth rotation |

#### Enemy Stats (Phase 1)
```
Test Enemy Stats:
- Health: 50
- Move Speed: 2.0
- Damage: 5 (for Phase 2)
- Detection Range: Infinite (simplified)
```

### 1.5 Layer and Tag Setup

#### Required Layers
| Layer Name | Layer Number | Usage |
|------------|--------------|-------|
| Default | 0 | General objects |
| Player | 6 | Player character |
| Enemy | 7 | All enemies |
| Wall | 8 | Arena boundaries |
| Floor | 9 | Arena floor |
| Pickup | 10 | Future part pickups |

#### Required Tags
| Tag Name | Usage |
|----------|-------|
| Player | Player GameObject |
| Enemy | Enemy GameObjects |
| Wall | Wall colliders |
| SpawnPoint | Enemy spawn locations |

### 1.6 Input System Configuration

#### Input Actions Map
```
Create: InputActions.inputactions
Action Map: Player
├── Actions:
│   ├── Move
│   │   ├── Type: Value
│   │   ├── Control Type: Vector2
│   │   └── Binding: Touch/Primary Touch/Position
│   └── Touch
│       ├── Type: Button
│       └── Binding: Touch/Primary Touch/Press
```

### 1.7 Testing & Validation Checklist

#### Scene Validation
- [ ] Arena floor is exactly 10x10 units
- [ ] Camera shows entire arena with small margin
- [ ] All 4 walls have colliders
- [ ] Spawn points are visible as gizmos in Scene view
- [ ] Lighting creates clear visibility

#### Player System Validation
- [ ] Player spawns at center (0, 0.5, 0)
- [ ] Touch anywhere on left side creates joystick
- [ ] Player moves in all 8 directions smoothly
- [ ] Player speed is consistent (5 units/second)
- [ ] Player cannot pass through walls
- [ ] Player rotates to face movement direction
- [ ] Joystick disappears on release

#### Enemy System Validation
- [ ] Enemy spawns at designated spawn point
- [ ] Enemy immediately moves toward player
- [ ] Enemy moves at 2 units/second
- [ ] Enemy stops when reaching player
- [ ] Enemy rotates smoothly toward player
- [ ] Multiple enemies don't overlap each other

#### Mobile Testing
- [ ] Build runs on Android/iOS device
- [ ] Touch controls are responsive
- [ ] No frame drops (maintain 30+ FPS)
- [ ] UI scales correctly on different screen sizes

### 1.8 Performance Benchmarks (Phase 1)

| Metric | Target | Maximum |
|--------|--------|---------|
| FPS | 60 | Never below 30 |
| Draw Calls | <20 | 30 |
| Triangles | <10k | 20k |
| Build Size | <50MB | 100MB |
| RAM Usage | <200MB | 300MB |
| Battery Drain | Minimal | Medium |

### 1.9 Deliverables Checklist

#### Code Deliverables
- [ ] `GameManager.cs` - Basic scene management
- [ ] `PlayerController.cs` - Movement and input handling
- [ ] `PlayerStats.cs` - Player statistics container
- [ ] `VirtualJoystick.cs` - Touch input visualization
- [ ] `EnemyAI.cs` - Basic chase behavior
- [ ] `EnemyStats.cs` - Enemy statistics container
- [ ] `TouchInputHandler.cs` - Input system wrapper

#### Prefab Deliverables
- [ ] `Player.prefab` - Fully configured player
- [ ] `Enemy_Basic.prefab` - Test enemy
- [ ] `Arena.prefab` - Complete arena setup
- [ ] `VirtualJoystick.prefab` - UI joystick

#### Scene Deliverables
- [ ] `Arena_Main.unity` - Fully configured test scene
- [ ] All lighting properly baked/configured
- [ ] Post-processing profile (if used)

### 1.10 Common Issues & Solutions

| Potential Issue | Solution |
|----------------|----------|
| Joystick not appearing | Check Canvas render mode (Screen Space - Overlay) |
| Player passing through walls | Ensure Rigidbody collision detection is Continuous |
| Enemy jittery movement | Use Rigidbody.MovePosition instead of transform |
| Touch input lag | Disable "Touch Delay" in Input System settings |
| Poor mobile performance | Disable shadows, reduce texture sizes |

### 1.11 Phase 1 Completion Criteria

**Phase 1 is considered COMPLETE when:**

1. **Arena is fully functional**
   - 10x10 playable space with working boundaries
   - Camera properly positioned for full visibility

2. **Player control is smooth**
   - Virtual joystick works reliably
   - Movement feels responsive on mobile device
   - Cannot escape arena bounds

3. **Basic enemy exists**
   - At least one enemy that chases player
   - Smooth pathfinding toward player
   - Proper collision with player

4. **Mobile build works**
   - Runs on actual device (not just Unity Remote)
   - Maintains acceptable framerate
   - Touch controls function properly

5. **Code is organized**
   - Clear folder structure
   - Scripts are modular and commented
   - Prefabs are created and reusable

### 1.12 Handoff Requirements for Phase 2

Before proceeding to Phase 2, ensure:
- [ ] All Phase 1 validation tests pass
- [ ] Code is committed to version control
- [ ] Known issues are documented
- [ ] Performance metrics are recorded
- [ ] Test build is created and archived

---

## Phase 1 Sign-off

| Role | Name | Date | Signature |
|------|------|------|-----------|
| Developer | | | |
| Tech Lead | | | |
| QA Tester | | | |

**Next Phase**: Phase 2 - Combat & Wave System (Do not begin until Phase 1 is approved)

---

*Note: This document represents the minimum viable implementation for Phase 1. Additional features or improvements should be noted but saved for the polish phase.*