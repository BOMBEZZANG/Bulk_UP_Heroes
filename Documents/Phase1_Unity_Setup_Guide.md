# Bulk Up Heroes - Phase 1 Unity Setup Guide
## Complete Scene & Hierarchy Configuration

---

## Table of Contents
1. [Project Settings](#1-project-settings)
2. [Scene Setup](#2-scene-setup)
3. [Managers & Core Systems](#3-managers--core-systems)
4. [Environment & Arena](#4-environment--arena)
5. [Camera Setup](#5-camera-setup)
6. [Player Setup](#6-player-setup)
7. [Enemy Setup](#7-enemy-setup)
8. [UI Setup](#8-ui-setup)
9. [Prefab Creation](#9-prefab-creation)
10. [Testing Checklist](#10-testing-checklist)

---

## 1. Project Settings

### 1.1 Player Settings
1. **File > Build Settings**
   - Add current scene to build
   - Platform: Android or iOS

2. **Edit > Project Settings > Player**
   ```
   Company Name: [Your Company]
   Product Name: Bulk Up Heroes

   Resolution and Presentation:
   ├── Default Orientation: Portrait
   ├── Allowed Orientations:
   │   ├── Portrait: ✓
   │   ├── Portrait Upside Down: ✗
   │   ├── Landscape Right: ✗
   │   └── Landscape Left: ✗

   Other Settings:
   ├── Color Space: Linear
   ├── Auto Graphics API: ✗
   ├── Graphics APIs: OpenGLES3 (Remove Vulkan for Android)
   ├── Minimum API Level: Android 7.0 (API 24)
   └── Target API Level: Automatic (Latest)
   ```

### 1.2 Quality Settings
```
Edit > Project Settings > Quality
├── Current Quality Level: Medium (for mobile)
├── VSync Count: Don't Sync
├── Anti Aliasing: Disabled (mobile optimization)
└── Shadows: Disable Shadows (for Phase 1)
```

### 1.3 Physics Settings
```
Edit > Project Settings > Physics
├── Default Solver Iterations: 6
├── Default Solver Velocity Iterations: 1
├── Gravity: Y = -9.81
└── Layer Collision Matrix:
    Player vs Enemy: ✓
    Player vs Wall: ✓
    Enemy vs Wall: ✓
    Enemy vs Enemy: ✓
```

### 1.4 Tags and Layers

#### Create Tags:
```
Edit > Project Settings > Tags and Layers

Tags:
├── Player
├── Enemy
├── Wall
└── SpawnPoint
```

#### Create Layers:
```
Layers:
├── Layer 6: Player
├── Layer 7: Enemy
├── Layer 8: Wall
├── Layer 9: Floor
└── Layer 10: Pickup (for future)
```

---

## 2. Scene Setup

### 2.1 Create New Scene
1. **File > New Scene**
   - Template: Basic (Built-in) or URP
   - Save As: `Assets/Scenes/Arena_Main.unity`

### 2.2 Scene Lighting Settings
```
Window > Rendering > Lighting

Environment:
├── Skybox Material: None (Solid Color)
├── Sun Source: Directional Light
├── Environment Lighting:
│   ├── Source: Color
│   └── Ambient Color: #5A5A5A (Gray)
└── Environment Reflections: None

Lightmapping Settings (for future):
├── Lightmapper: Progressive GPU
└── For now: No baking needed
```

---

## 3. Managers & Core Systems

### 3.1 Create Managers Hierarchy
```
Hierarchy > Right Click > Create Empty
Name: "Managers"
Position: (0, 0, 0)

Managers/
├── GameManager (GameObject)
├── PoolManager (GameObject)
└── EventSystem (for UI)
```

### 3.2 GameManager Setup

#### Create GameObject:
1. Select "Managers" parent
2. Right Click > Create Empty
3. Name: `GameManager`
4. Tag: Untagged
5. Layer: Default

#### Add Component:
1. **Add Component > Scripts > BulkUpHeroes.Core > GameManager**

#### Inspector Settings:
```
GameManager (Script)
├── Current State: MainMenu
├── Current Wave: 0
├── Enemies Defeated: 0
├── Survival Time: 0
└── Player Prefab: [Assign after creating Player prefab]
└── Player Spawn Point: [Assign after creating spawn point]
```

### 3.3 PoolManager Setup

#### Create GameObject:
1. Select "Managers" parent
2. Right Click > Create Empty
3. Name: `PoolManager`

#### Add Component:
1. **Add Component > Scripts > BulkUpHeroes.Core > PoolManager**

#### Inspector Settings:
```
PoolManager (Script)
├── Enemy Prefab: [Assign after creating Enemy prefab]
├── Enemy Pool Size: 20
├── Enemy Pool Parent: [Auto-created]
├── Pickup Prefab: (None - Phase 3)
└── Pickup Pool Size: 10
```

### 3.4 EventSystem Setup

#### Check if exists:
- If **EventSystem** already exists in scene: Keep it
- If not: Create manually

#### Create EventSystem:
1. Hierarchy > Right Click > UI > Event System
2. Move under "Managers" parent

#### Inspector Settings:
```
Event System (Script)
└── First Selected: None

Standalone Input Module (Script)
├── Horizontal Axis: Horizontal
├── Vertical Axis: Vertical
└── Submit Button: Submit
```

---

## 4. Environment & Arena

### 4.1 Create Environment Parent
```
Hierarchy > Right Click > Create Empty
Name: "Environment"
Position: (0, 0, 0)

Environment/
├── Arena/
│   ├── Floor
│   ├── Walls/
│   └── SpawnPoints/
└── Lighting/
```

### 4.2 Create Arena Floor

#### Create Floor:
1. Select "Environment" > Right Click > 3D Object > Plane
2. Name: `Floor`
3. Transform:
   ```
   Position: (0, 0, 0)
   Rotation: (0, 0, 0)
   Scale: (1, 1, 1)  // Plane is 10x10 by default
   ```
4. **Layer: Floor**
5. **Tag: Untagged**

#### Floor Material:
1. Create Material: `Assets/Materials/Mat_ArenaFloor`
   ```
   Shader: URP/Lit (or Standard)
   Base Color: #3C3C3C (Dark Gray)
   Metallic: 0
   Smoothness: 0.2
   ```
2. Assign to Floor's Mesh Renderer

#### Optional Grid Texture:
```
For visual clarity, add a grid texture:
1. Create texture or use solid color
2. Tiling: (10, 10) for 1-unit grid
```

### 4.3 Create Arena Walls

#### Create Walls Parent:
1. Select "Environment/Arena"
2. Right Click > Create Empty
3. Name: `Walls`

#### Create 4 Wall Colliders:

**North Wall:**
```
Hierarchy > Right Click > 3D Object > Cube
Name: Wall_North
Parent: Walls

Transform:
├── Position: (0, 1, 5)
├── Rotation: (0, 0, 0)
└── Scale: (10, 2, 0.2)

Cube (Mesh Filter):
└── (Keep default)

Mesh Renderer:
└── Enabled: ✗ (Disable - invisible walls)

Box Collider:
├── Is Trigger: ✗
├── Center: (0, 0, 0)
└── Size: (1, 1, 1)

Layer: Wall
Tag: Wall
```

**South Wall:**
```
Name: Wall_South
Position: (0, 1, -5)
Rotation: (0, 0, 0)
Scale: (10, 2, 0.2)
Layer: Wall
Tag: Wall
Mesh Renderer: Disabled
```

**East Wall:**
```
Name: Wall_East
Position: (5, 1, 0)
Rotation: (0, 0, 0)
Scale: (0.2, 2, 10)
Layer: Wall
Tag: Wall
Mesh Renderer: Disabled
```

**West Wall:**
```
Name: Wall_West
Position: (-5, 1, 0)
Rotation: (0, 0, 0)
Scale: (0.2, 2, 10)
Layer: Wall
Tag: Wall
Mesh Renderer: Disabled
```

### 4.4 Create Spawn Points

#### Create SpawnPoints Parent:
1. Select "Environment/Arena"
2. Right Click > Create Empty
3. Name: `SpawnPoints`

#### Create 4 Corner Spawn Points:

**Northeast Spawn:**
```
Hierarchy > Right Click > Create Empty
Name: SpawnPoint_NE
Parent: SpawnPoints

Transform:
├── Position: (4, 0.5, 4)
├── Rotation: (0, 0, 0)
└── Scale: (1, 1, 1)

Tag: SpawnPoint
Layer: Default

Add Component > Gizmos Helper (Optional):
// For visibility in Scene view
```

**Northwest Spawn:**
```
Name: SpawnPoint_NW
Position: (-4, 0.5, 4)
Tag: SpawnPoint
```

**Southeast Spawn:**
```
Name: SpawnPoint_SE
Position: (4, 0.5, -4)
Tag: SpawnPoint
```

**Southwest Spawn:**
```
Name: SpawnPoint_SW
Position: (-4, 0.5, -4)
Tag: SpawnPoint
```

### 4.5 Create Lighting

#### Directional Light:
```
Hierarchy > Light > Directional Light
Name: Directional Light
Parent: Environment/Lighting

Transform:
├── Position: (0, 10, 0)
├── Rotation: (50, -30, 0)
└── Scale: (1, 1, 1)

Light Component:
├── Type: Directional
├── Mode: Realtime
├── Color: #FFFFFF
├── Intensity: 1
├── Indirect Multiplier: 1
├── Shadow Type: No Shadows (Phase 1 optimization)
└── Cookie: None
```

---

## 5. Camera Setup

### 5.1 Main Camera Configuration
```
Select: Main Camera (should exist in scene)

Transform:
├── Position: (0, 10, 0)
├── Rotation: (90, 0, 0)  // Looking straight down
└── Scale: (1, 1, 1)

Camera Component:
├── Projection: Orthographic ⚠️ IMPORTANT
├── Size: 6  // Adjust to see full arena
├── Clipping Planes:
│   ├── Near: 0.3
│   └── Far: 20
├── Viewport Rect: (0, 0, 1, 1)
├── Depth: 0
├── Rendering Path: Use Graphics Settings
├── Target Display: Display 1
├── Culling Mask: Everything
└── Clear Flags: Solid Color
    └── Background: #1A1A1A (Dark gray)

Tag: MainCamera
Layer: Default
```

### 5.2 Camera Additional Components (URP)

If using URP:
```
Add Component > Rendering > Universal Additional Camera Data

Render Type: Base
Post Processing: ✗ (Disabled for Phase 1)
Anti-aliasing: None
Stop NaN: ✗
Dithering: ✗
```

---

## 6. Player Setup

### 6.1 Create Player GameObject
```
Hierarchy > Right Click > Create Empty
Name: Player
Parent: (Root - not parented)

Transform:
├── Position: (0, 0.5, 0)  // Center of arena
├── Rotation: (0, 0, 0)
└── Scale: (1, 1, 1)

Tag: Player
Layer: Player
```

### 6.2 Add Components to Player

#### Add Rigidbody:
```
Add Component > Physics > Rigidbody

Rigidbody:
├── Mass: 1
├── Drag: 0
├── Angular Drag: 0.05
├── Use Gravity: ✓
├── Is Kinematic: ✗
├── Interpolate: Interpolate
├── Collision Detection: Continuous
└── Constraints:
    └── Freeze Rotation: X, Y, Z ✓ (All rotation axes)
```

#### Add Capsule Collider:
```
Add Component > Physics > Capsule Collider

Capsule Collider:
├── Is Trigger: ✗
├── Material: None
├── Center: (0, 0, 0)
├── Radius: 0.3
├── Height: 1.8
└── Direction: Y-Axis
```

#### Add PlayerController Script:
```
Add Component > Scripts > BulkUpHeroes.Player > PlayerController

PlayerController (Script):
├── Rotation Speed: 720
└── Clamp To Arena: ✓

// VirtualJoystick reference will auto-find on Start
```

#### Add PlayerStats Script:
```
Add Component > Scripts > BulkUpHeroes.Player > PlayerStats

PlayerStats (Script):
├── Current Health: 100 (auto-set)
├── Max Health: 100 (auto-set)
├── Damage: 10 (auto-set)
├── Attack Speed: 1 (auto-set)
└── Move Speed: 5 (auto-set)

Events:
├── On Health Changed: (empty for now)
└── On Player Death: (empty for now)
```

### 6.3 Add Player Visual (Temporary)

For Phase 1 testing, use a simple capsule:

```
Select Player > Right Click > 3D Object > Capsule
Name: PlayerModel

Transform:
├── Position: (0, 0, 0) // Local
├── Rotation: (0, 0, 0)
└── Scale: (0.6, 0.9, 0.6)

Capsule Collider: Remove this component (parent has collider)

Mesh Renderer:
└── Materials: Create Mat_Player (Blue color)
```

**Mat_Player Material:**
```
Assets/Materials/Mat_Player

Shader: URP/Lit
Base Color: #4A90E2 (Blue)
Metallic: 0
Smoothness: 0.3
```

### 6.4 Link to GameManager

```
Select GameManager
Inspector:
└── Player Prefab: [Drag Player from Hierarchy AFTER making prefab]
└── Player Spawn Point: (0, 0.5, 0) or assign SpawnPoint_Center if created
```

---

## 7. Enemy Setup

### 7.1 Create Enemies Parent
```
Hierarchy > Right Click > Create Empty
Name: Enemies
Position: (0, 0, 0)

// This will hold active enemies during gameplay
```

### 7.2 Create Enemy Prefab

#### Create Enemy GameObject:
```
Hierarchy > Right Click > Create Empty
Name: Enemy_Basic
Parent: (Root temporarily)

Transform:
├── Position: (0, 0.5, 0)
├── Rotation: (0, 0, 0)
└── Scale: (1, 1, 1)

Tag: Enemy
Layer: Enemy
```

#### Add Rigidbody:
```
Add Component > Physics > Rigidbody

Rigidbody:
├── Mass: 1
├── Drag: 0
├── Angular Drag: 0.05
├── Use Gravity: ✓
├── Is Kinematic: ✗
├── Interpolate: Interpolate
├── Collision Detection: Continuous
└── Constraints:
    └── Freeze Rotation: X, Y, Z ✓
```

#### Add Capsule Collider:
```
Add Component > Physics > Capsule Collider

Capsule Collider:
├── Is Trigger: ✗
├── Material: None
├── Center: (0, 0, 0)
├── Radius: 0.3
├── Height: 1.8
└── Direction: Y-Axis
```

#### Add EnemyAI Script:
```
Add Component > Scripts > BulkUpHeroes.Enemies > EnemyAI

EnemyAI (Script):
├── Current State: Idle (will auto-change)
├── Target Player: (auto-finds on Start)
├── Target Update Interval: 0.2
├── Rotation Speed: 360
├── Attack Range: 1
└── Attack Cooldown: 1
```

#### Add EnemyStats Script:
```
Add Component > Scripts > BulkUpHeroes.Enemies > EnemyStats

EnemyStats (Script):
├── Enemy Type: BasicMelee
├── Current Health: 50 (auto-set)
├── Max Health: 50 (auto-set)
├── Damage: 5 (auto-set)
└── Move Speed: 2 (auto-set)

Events:
└── On Enemy Death: (empty for now)
```

### 7.3 Add Enemy Visual (Temporary)

```
Select Enemy_Basic > Right Click > 3D Object > Capsule
Name: EnemyModel

Transform:
├── Position: (0, 0, 0) // Local
├── Rotation: (0, 0, 0)
└── Scale: (0.5, 0.8, 0.5)

Capsule Collider: Remove this component

Mesh Renderer:
└── Materials: Create Mat_Enemy (Red color)
```

**Mat_Enemy Material:**
```
Assets/Materials/Mat_Enemy

Shader: URP/Lit
Base Color: #E24A4A (Red)
Metallic: 0
Smoothness: 0.3
```

### 7.4 Test Enemy Placement (Optional)

For testing, you can place 1-2 enemies in scene:
```
1. Duplicate Enemy_Basic
2. Name: Enemy_Test1
3. Position: (3, 0.5, 3)

// They will auto-chase player when game starts
// Delete after testing or before final build
```

---

## 8. UI Setup

### 8.1 Create Canvas
```
Hierarchy > Right Click > UI > Canvas
Name: Canvas_Main

Canvas Component:
├── Render Mode: Screen Space - Overlay
├── Pixel Perfect: ✗
├── Sort Order: 0
└── Target Display: Display 1

Canvas Scaler:
├── UI Scale Mode: Scale With Screen Size
├── Reference Resolution: (1080, 1920) // Portrait
├── Screen Match Mode: Match Width Or Height
├── Match: 0.5
└── Reference Pixels Per Unit: 100

Graphic Raycaster:
└── (Keep defaults)
```

### 8.2 Create Virtual Joystick

#### Create Joystick Panel:
```
Select Canvas > Right Click > UI > Panel
Name: Panel_Joystick

Rect Transform:
├── Anchor Preset: Bottom-Left
├── Anchor: (0, 0) to (0, 0)
├── Pivot: (0, 0)
├── Position: (0, 0, 0)
├── Width: 540 (half screen width)
└── Height: 960 (half screen height)

Image Component:
├── Color: (1, 1, 1, 0) // Fully transparent
└── Raycast Target: ✓
```

#### Create Joystick Container:
```
Select Panel_Joystick > Right Click > UI > Image
Name: VirtualJoystick

Rect Transform:
├── Anchor Preset: Bottom-Left
├── Anchor: (0, 0) to (0, 0)
├── Pivot: (0.5, 0.5)
├── Position: (150, 150, 0)
├── Width: 200
└── Height: 200

Image Component:
├── Source Image: (None - will be invisible)
├── Color: (1, 1, 1, 0.3)
└── Raycast Target: ✓
```

#### Create Joystick Background:
```
Select VirtualJoystick > Right Click > UI > Image
Name: Background

Rect Transform:
├── Anchor Preset: Center
├── Position: (0, 0, 0)
├── Width: 150
└── Height: 150

Image Component:
├── Source Image: UI/Skin/Knob (or create circle sprite)
├── Color: (1, 1, 1, 0.3) // Semi-transparent white
└── Raycast Target: ✗
```

#### Create Joystick Handle:
```
Select Background > Right Click > UI > Image
Name: Handle

Rect Transform:
├── Anchor Preset: Center
├── Position: (0, 0, 0)
├── Width: 60
└── Height: 60

Image Component:
├── Source Image: UI/Skin/Knob
├── Color: (1, 1, 1, 0.6) // More opaque
└── Raycast Target: ✗
```

#### Add VirtualJoystick Script:
```
Select VirtualJoystick GameObject

Add Component > Scripts > BulkUpHeroes.UI > VirtualJoystick

VirtualJoystick (Script):
├── Joystick Container: [Self - VirtualJoystick]
├── Joystick Background: [Background child]
├── Joystick Handle: [Handle child]
├── Handle Range: 50
├── Dead Zone: 0.1
├── Dynamic Joystick: ✓
├── Inactive Color: (1, 1, 1, 0.3)
└── Active Color: (1, 1, 1, 0.6)
```

### 8.3 Create Touch Input Handler

```
Select Canvas > Right Click > Create Empty
Name: TouchInputHandler

Add Component > Scripts > BulkUpHeroes.UI > TouchInputHandler

TouchInputHandler (Script):
├── Screen Division: 0.5 (50% for joystick)
└── Show Debug Zones: ✗ (Enable for testing)
```

### 8.4 Create HUD (Basic - Phase 1)

#### Create HUD Panel:
```
Select Canvas > Right Click > UI > Panel
Name: Panel_HUD

Rect Transform:
├── Anchor Preset: Top Stretch
├── Position: (0, 0, 0)
├── Height: 100
└── Width: (stretch)

Image Component:
└── Color: (0, 0, 0, 0.5) // Semi-transparent black
```

#### Create Health Text (Temporary):
```
Select Panel_HUD > Right Click > UI > Text - TextMeshPro
Name: Text_Health

Rect Transform:
├── Anchor Preset: Top-Left
├── Position: (20, -20, 0)
├── Width: 200
└── Height: 50

TextMeshProUGUI:
├── Text: "Health: 100"
├── Font Size: 24
├── Color: White
├── Alignment: Middle Left
└── Auto Size: ✗
```

---

## 9. Prefab Creation

### 9.1 Create Player Prefab
```
1. Drag Player from Hierarchy to Assets/Prefabs/Player/
2. Name: Player.prefab
3. Delete Player from Hierarchy (GameManager will spawn it)
```

### 9.2 Create Enemy Prefab
```
1. Drag Enemy_Basic from Hierarchy to Assets/Prefabs/Enemies/
2. Name: Enemy_Basic.prefab
3. Delete Enemy_Basic from Hierarchy (PoolManager will use it)
```

### 9.3 Link Prefabs to Managers

**GameManager:**
```
Select Managers > GameManager

Inspector:
├── Player Prefab: [Drag Player.prefab from Project]
└── Player Spawn Point: [Leave as transform, or assign a spawn point]
```

**PoolManager:**
```
Select Managers > PoolManager

Inspector:
├── Enemy Prefab: [Drag Enemy_Basic.prefab from Project]
├── Enemy Pool Size: 20
└── Enemy Pool Parent: (auto-created)
```

### 9.4 Create Arena Prefab (Optional)
```
1. Select Environment/Arena
2. Drag to Assets/Prefabs/Arena/
3. Name: Arena.prefab

// This makes it reusable for multiple scenes
```

---

## 10. Testing Checklist

### 10.1 Scene Validation
- [ ] Arena floor is 10x10 units
- [ ] Camera shows entire arena clearly
- [ ] All 4 walls have colliders on Layer: Wall
- [ ] Spawn points are at corners (±4, 0.5, ±4)
- [ ] Directional Light is positioned and rotated correctly

### 10.2 Manager Validation
- [ ] GameManager exists with script attached
- [ ] PoolManager exists with script attached
- [ ] EventSystem exists in scene
- [ ] Player prefab assigned to GameManager
- [ ] Enemy prefab assigned to PoolManager

### 10.3 Player Validation
- [ ] Player prefab exists in Assets/Prefabs/Player/
- [ ] Has Rigidbody with Continuous collision
- [ ] Has CapsuleCollider
- [ ] Has PlayerController script
- [ ] Has PlayerStats script
- [ ] Tag: Player, Layer: Player
- [ ] Rotation constraints are frozen (X, Y, Z)

### 10.4 Enemy Validation
- [ ] Enemy_Basic prefab exists
- [ ] Has Rigidbody with Continuous collision
- [ ] Has CapsuleCollider
- [ ] Has EnemyAI script
- [ ] Has EnemyStats script
- [ ] Tag: Enemy, Layer: Enemy

### 10.5 UI Validation
- [ ] Canvas exists with Screen Space Overlay
- [ ] Canvas Scaler set to 1080x1920 (Portrait)
- [ ] VirtualJoystick has all child objects (Background, Handle)
- [ ] VirtualJoystick script has references assigned
- [ ] TouchInputHandler script attached to Canvas or child

### 10.6 Runtime Testing (Play Mode)

**In Unity Editor, press Play and verify:**

#### Player Tests:
- [ ] Player spawns at center (0, 0.5, 0)
- [ ] Mouse click/drag creates joystick (left side)
- [ ] Player moves in all 8 directions smoothly
- [ ] Player speed is consistent
- [ ] Player rotates to face movement direction
- [ ] Player cannot pass through walls
- [ ] Player stops at arena boundaries

#### Enemy Tests:
- [ ] Can manually spawn enemy via PoolManager context menu
- [ ] Enemy immediately moves toward player
- [ ] Enemy moves at slower speed than player
- [ ] Enemy rotates smoothly toward player
- [ ] Enemy stops when reaching player (1 unit range)

#### Performance Tests:
- [ ] Game runs at 60 FPS in editor
- [ ] No console errors on Play
- [ ] No memory leaks after 1 minute
- [ ] Joystick responds instantly to input

### 10.7 Mobile Build Testing (Optional for Phase 1)

If building to device:
```
Build Settings > Android/iOS
├── Compression: LZ4
├── Development Build: ✓ (for testing)
└── Build

On Device:
- [ ] Touch controls work on left screen half
- [ ] Player moves smoothly
- [ ] Maintains 30+ FPS
- [ ] No crashes after 5 minutes
```

---

## 11. Common Issues & Solutions

### Issue: Player falls through floor
**Solution:**
- Check Player has Rigidbody with Use Gravity ON
- Check Floor has no collider (plane doesn't need one) - Add MeshCollider if needed
- Ensure Player Y position is 0.5 or higher

### Issue: Joystick doesn't appear
**Solution:**
- Check VirtualJoystick script has all references assigned
- Verify Canvas has EventSystem
- Check Panel_Joystick has Raycast Target enabled
- Ensure dynamic joystick is enabled

### Issue: Enemy doesn't chase player
**Solution:**
- Verify Player has Tag: "Player"
- Check EnemyAI can find player (check console)
- Ensure Enemy has Rigidbody (not kinematic)
- Verify enemy is spawned from pool, not placed manually

### Issue: Touch doesn't work in build
**Solution:**
- Check Player Settings > Resolution > Orientation is Portrait only
- Verify InputSystem package is NOT blocking legacy input
- Test with "Show Debug Zones" enabled on TouchInputHandler

### Issue: GameManager spawns duplicate players
**Solution:**
- Remove Player from Hierarchy (should only be prefab)
- GameManager creates player on Start()
- Only have Player.prefab in Project, not scene

---

## 12. Next Steps (After Phase 1 Setup Complete)

Once this setup is complete and tested:
1. ✅ Save Scene
2. ✅ Create backup of project
3. ✅ Commit to version control
4. 📋 Move to Phase 2: Combat & Wave System
   - Wave spawning
   - Auto-attack system
   - Damage system integration
   - Enemy death and respawning

---

## Quick Setup Summary

**Minimum viable setup:**
```
1. Create Scene: Arena_Main
2. Setup Camera (Orthographic, top-down)
3. Create Arena (Floor + 4 Walls)
4. Create Managers GameObject with GameManager + PoolManager
5. Create Player GameObject → Make Prefab → Delete from scene
6. Create Enemy_Basic GameObject → Make Prefab → Delete from scene
7. Create UI Canvas with VirtualJoystick
8. Assign prefabs to managers
9. Press Play and test movement
```

**Total setup time:** ~30-45 minutes

---

**Document Version:** 1.0
**Last Updated:** Phase 1 Development
**Next Update:** Phase 2 Combat Implementation
