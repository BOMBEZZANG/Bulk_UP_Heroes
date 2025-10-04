# Phase 2 Unity Setup Guide - Combat & Wave System
**Bulk Up Heroes - Phase 2 Implementation**

This guide provides step-by-step instructions for setting up all Phase 2 systems in Unity, including Combat, Wave Management, and HUD.

---

## Table of Contents
1. [Overview](#overview)
2. [Prerequisites](#prerequisites)
3. [Combat System Setup](#combat-system-setup)
4. [Wave Management Setup](#wave-management-setup)
5. [HUD System Setup](#hud-system-setup)
6. [Game Over Screen Setup](#game-over-screen-setup)
7. [Updating Existing Objects](#updating-existing-objects)
8. [Testing Checklist](#testing-checklist)
9. [Troubleshooting](#troubleshooting)

---

## Overview

Phase 2 adds the following new systems:
- **Combat System**: Auto-attack for player and enemies
- **Wave System**: Progressive wave spawning with difficulty scaling
- **HUD System**: Health bar, wave counter, enemy counter
- **Game Over Screen**: Death screen with statistics

**New Scripts Created:**
- Core: `GameEvents.cs`
- Combat: `CombatManager.cs`, `DamageHandler.cs`
- Enemies: `WaveManager.cs`, `EnemySpawner.cs`, `WaveConfig.cs`
- UI: `HUDManager.cs`, `HealthBar.cs`, `AnnouncementSystem.cs`, `GameOverScreen.cs`

---

## Prerequisites

**Required from Phase 1:**
- ✅ Complete Phase 1 setup (see Phase1_Unity_Setup_Guide.md)
- ✅ Player prefab with PlayerController and PlayerStats
- ✅ Enemy prefab with EnemyAI and EnemyStats
- ✅ PoolManager set up and working
- ✅ Virtual Joystick functioning

**Phase 2 Scripts Location:**
All new scripts should be in their respective folders:
- `Assets/Scripts/Core/`
- `Assets/Scripts/Combat/`
- `Assets/Scripts/Enemies/`
- `Assets/Scripts/UI/`

**TextMeshPro Setup:**
- ✅ Import TextMeshPro Essentials (Window → TextMeshPro → Import TMP Essential Resources)
- All UI text will use TextMeshPro instead of legacy Text components

---

## Combat System Setup

### 1. Update Player Prefab

**a) Add CombatManager Component:**
1. Open Player prefab (in `Assets/Prefabs/`)
2. Click **Add Component** → Search: `CombatManager`
3. Configure:
   ```
   Attack Range: 1.5
   Target Update Interval: 0.2
   Target Layer: Enemy (select from dropdown)
   Enable Visual Feedback: ✓
   Attack Scale Punch: 1.2
   Attack Anim Duration: 0.2
   ```

**b) Add DamageHandler Component:**
1. On same Player prefab, **Add Component** → Search: `DamageHandler`
2. Configure:
   ```
   ╔══ Damage Feedback ══╗
   Enable Damage Feedback: ✓
   Damage Flash Color: (255, 0, 0, 255) ← Red
   Damage Flash Duration: 0.1
   Damage Shake Duration: 0.1
   Damage Shake Intensity: 0.1

   ╔══ Death Settings ══╗
   Death Fade Duration: 0.3
   Death Scale Duration: 0.3
   Death Delay: 0.5
   ```

**c) Update PlayerStats Component:**
1. PlayerStats should already exist from Phase 1
2. Verify it has:
   ```
   Max Health: 100
   Base Damage: 10
   Base Attack Speed: 1
   Base Move Speed: 5
   ```

### 2. Update Enemy Prefab

**a) Add CombatManager Component:**
1. Open Enemy prefab (in `Assets/Prefabs/`)
2. Click **Add Component** → Search: `CombatManager`
3. Configure:
   ```
   Attack Range: 1.0
   Target Update Interval: 0.2
   Target Layer: Player (select from dropdown)
   Enable Visual Feedback: ✓
   Attack Scale Punch: 1.2
   Attack Anim Duration: 0.2
   ```

**b) Add DamageHandler Component:**
1. On same Enemy prefab, **Add Component** → Search: `DamageHandler`
2. Configure:
   ```
   ╔══ Damage Feedback ══╗
   Enable Damage Feedback: ✓
   Damage Flash Color: (255, 100, 100, 255) ← Light red
   Damage Flash Duration: 0.1
   Damage Shake Duration: 0.1
   Damage Shake Intensity: 0.1

   ╔══ Death Settings ══╗
   Death Fade Duration: 0.3
   Death Scale Duration: 0.3
   Death Delay: 0.5
   ```

**c) Verify EnemyStats Component:**
1. EnemyStats should already exist
2. Leave default values (will be set by wave spawner)

**Important Note:** After updating prefabs, click **Apply** at the top of the Inspector to save changes!

---

## Wave Management Setup

### 3. Create Wave Manager

**a) Create GameObject:**
1. In Hierarchy, right-click → **Create Empty**
2. Rename to: `WaveManager`
3. Position: (0, 0, 0)

**b) Add WaveManager Component:**
1. Select WaveManager object
2. **Add Component** → Search: `WaveManager`
3. Configure:
   ```
   ╔══ Wave Settings ══╗
   Starting Wave: 1
   Wave Prepare Delay: 3
   Wave Complete Delay: 2
   Auto Start First Wave: ✓

   ╔══ References ══╗
   Enemy Spawner: [Assign in next step]
   ```

### 4. Create Enemy Spawner

**a) Create GameObject:**
1. In Hierarchy, right-click → **Create Empty**
2. Rename to: `EnemySpawner`
3. Position: (0, 0, 0)

**b) Add EnemySpawner Component:**
1. Select EnemySpawner object
2. **Add Component** → Search: `EnemySpawner`
3. Configure:
   ```
   ╔══ Spawn Points ══╗
   Spawn Points: [Leave empty - will auto-create]
   Auto Find Spawn Points: ✓

   ╔══ Spawn Settings ══╗
   Spawn Effect Duration: 0.3
   Rotate Spawn Points: ✓
   ```

**c) Link to WaveManager:**
1. Select `WaveManager` object
2. In Inspector, find **Enemy Spawner** field
3. Drag `EnemySpawner` object from Hierarchy to this field

### 5. Create Spawn Points (Manual - Optional)

If you want custom spawn point positions instead of auto-generated:

**Create 4 Spawn Points:**
1. In Hierarchy, right-click → **Create Empty**
2. Rename to: `SpawnPoint_0`
3. Tag: `SpawnPoint` (create tag if needed)
4. Position: `(4, 0.5, 4)` ← Northeast corner

Repeat for:
- `SpawnPoint_1`: Position `(-4, 0.5, 4)` ← Northwest
- `SpawnPoint_2`: Position `(4, 0.5, -4)` ← Southeast
- `SpawnPoint_3`: Position `(-4, 0.5, -4)` ← Southwest

**Organize Spawn Points:**
1. Create empty GameObject: `SpawnPoints`
2. Drag all 4 SpawnPoint objects under it

**Update EnemySpawner:**
1. Select `EnemySpawner`
2. Set `Auto Find Spawn Points: ✓`
3. Or manually drag spawn points to array

### 6. Update PoolManager

**Remove TestEnemySpawner (if exists):**
1. Select `PoolManager` object
2. If `TestEnemySpawner` component exists, remove it
3. Wave system will handle all spawning now

---

## HUD System Setup

### 7. Create HUD Canvas

**a) Create Canvas:**
1. Hierarchy → Right-click → **UI** → **Canvas**
2. Rename to: `HUD_Canvas`
3. Configure Canvas component:
   ```
   Render Mode: Screen Space - Overlay
   Pixel Perfect: ✓
   Canvas Scaler:
     ├── UI Scale Mode: Scale With Screen Size
     ├── Reference Resolution: (1080, 1920) ← Portrait
     └── Match: 0.5 (Width & Height)
   ```

### 8. Create HUD Manager

**a) Add HUDManager Component:**
1. Select `HUD_Canvas` object
2. **Add Component** → Search: `HUDManager`
3. Leave fields empty for now (will assign after creating UI)

### 9. Create Health Bar

**a) Create Health Bar Panel:**
1. Right-click `HUD_Canvas` → **UI** → **Panel**
2. Rename to: `HealthBar_Panel`
3. Configure RectTransform:
   ```
   Anchor Preset: Top-Left
   Position X: 120
   Position Y: -60
   Width: 200
   Height: 30
   ```
4. Panel Image:
   ```
   Color: (0, 0, 0, 180) ← Semi-transparent black
   ```

**b) Create Health Bar Background:**
1. Right-click `HealthBar_Panel` → **UI** → **Image**
2. Rename to: `HealthBar_Background`
3. Configure:
   ```
   RectTransform:
     ├── Anchor: Stretch-Stretch (both axes)
     ├── Left: 5, Right: 5
     ├── Top: 5, Bottom: 5

   Image Component:
     ├── Color: (50, 50, 50, 255) ← Dark gray
     └── Image Type: Sliced (optional)
   ```

**c) Create Health Bar Fill:**
1. Right-click `HealthBar_Background` → **UI** → **Image**
2. Rename to: `HealthBar_Fill`
3. Configure:
   ```
   RectTransform:
     ├── Anchor: Stretch-Stretch
     ├── Left: 0, Right: 0
     ├── Top: 0, Bottom: 0

   Image Component:
     ├── Color: (0, 255, 0, 255) ← Green
     ├── Image Type: Filled
     ├── Fill Method: Horizontal
     ├── Fill Origin: Left
     └── Fill Amount: 1.0
   ```

**d) Create Health Text (Optional):**
1. Right-click `HealthBar_Panel` → **UI** → **TextMeshPro - Text**
2. Rename to: `HealthBar_Text`
3. If prompted, import TMP Essentials
4. Configure:
   ```
   RectTransform:
     ├── Anchor: Center
     ├── Width: 180
     ├── Height: 30

   TextMeshProUGUI Component:
     ├── Text: "100 / 100"
     ├── Font Size: 16
     ├── Alignment: Center-Middle
     ├── Color: White
     ├── Enable Auto Sizing: ✓ (optional)
     ├── Min: 10, Max: 20
     └── Wrapping: Disabled
   ```

**e) Add HealthBar Component:**
1. Select `HealthBar_Panel`
2. **Add Component** → Search: `HealthBar`
3. Configure:
   ```
   ╔══ Health Bar Components ══╗
   Fill Image: [Drag HealthBar_Fill here]
   Background Image: [Drag HealthBar_Background here]
   Health Text: [Drag HealthBar_Text here]

   ╔══ Visual Settings ══╗
   Smooth Fill: ✓
   Fill Speed: 5
   Enable Color Transition: ✓
   Show Health Text: ✓

   ╔══ Color Thresholds ══╗
   Healthy Color: (0, 255, 0, 255) ← Green
   Warning Color: (255, 255, 0, 255) ← Yellow
   Critical Color: (255, 0, 0, 255) ← Red
   Warning Threshold: 0.5
   Critical Threshold: 0.25
   ```

### 10. Create Wave & Enemy Counters

**a) Create Wave Text:**
1. Right-click `HUD_Canvas` → **UI** → **TextMeshPro - Text**
2. Rename to: `Wave_Text`
3. Configure:
   ```
   RectTransform:
     ├── Anchor Preset: Top-Right
     ├── Position X: -120
     ├── Position Y: -40
     ├── Width: 200
     ├── Height: 40

   TextMeshProUGUI Component:
     ├── Text: "Wave: 1"
     ├── Font Size: 24
     ├── Alignment: Right-Middle
     ├── Color: White
     ├── Enable Auto Sizing: ✓
     ├── Min: 18, Max: 28
     └── Wrapping: Disabled
   ```

**b) Create Enemy Counter Text:**
1. Right-click `HUD_Canvas` → **UI** → **TextMeshPro - Text**
2. Rename to: `EnemyCount_Text`
3. Configure:
   ```
   RectTransform:
     ├── Anchor Preset: Top-Right
     ├── Position X: -120
     ├── Position Y: -80
     ├── Width: 200
     ├── Height: 40

   TextMeshProUGUI Component:
     ├── Text: "Enemies: 0/0"
     ├── Font Size: 20
     ├── Alignment: Right-Middle
     ├── Color: (255, 200, 200, 255) ← Light red
     ├── Enable Auto Sizing: ✓
     ├── Min: 16, Max: 24
     └── Wrapping: Disabled
   ```

**c) Create Kill Counter Text:**
1. Right-click `HUD_Canvas` → **UI** → **TextMeshPro - Text**
2. Rename to: `KillCount_Text`
3. Configure:
   ```
   RectTransform:
     ├── Anchor Preset: Top-Right
     ├── Position X: -120
     ├── Position Y: -120
     ├── Width: 200
     ├── Height: 40

   TextMeshProUGUI Component:
     ├── Text: "Kills: 0"
     ├── Font Size: 20
     ├── Alignment: Right-Middle
     ├── Color: (255, 215, 0, 255) ← Gold
     ├── Enable Auto Sizing: ✓
     ├── Min: 16, Max: 24
     └── Wrapping: Disabled
   ```

### 11. Link HUD Manager

**Configure HUDManager Component:**
1. Select `HUD_Canvas`
2. Find `HUDManager` component
3. Assign references:
   ```
   ╔══ HUD Panels ══╗
   HUD Panel: [Drag HUD_Canvas itself]
   Health Bar: [Drag HealthBar_Panel]
   Announcement System: [Will assign after next step]

   ╔══ Wave Display ══╗
   Wave Text: [Drag Wave_Text]
   Wave Text Format: "Wave: {0}"

   ╔══ Enemy Counter ══╗
   Enemy Count Text: [Drag EnemyCount_Text]
   Enemy Count Format: "Enemies: {0}"
   Show Remaining Total: ✓
   Enemy Count Detail Format: "Enemies: {0}/{1}"

   ╔══ Stats Display ══╗
   Kill Count Text: [Drag KillCount_Text]
   Kill Count Format: "Kills: {0}"
   ```

---

## Announcement System Setup

### 12. Create Announcement Panel

**a) Create Announcement Panel:**
1. Right-click `HUD_Canvas` → **UI** → **Panel**
2. Rename to: `Announcement_Panel`
3. Configure:
   ```
   RectTransform:
     ├── Anchor Preset: Center-Middle
     ├── Position: (0, 0)
     ├── Width: 800
     ├── Height: 300

   Panel Image:
     └── Color: (0, 0, 0, 0) ← Fully transparent

   Canvas Group (Add Component):
     ├── Alpha: 0
     ├── Interactable: ✗
     └── Block Raycasts: ✗
   ```

**b) Create Main Announcement Text:**
1. Right-click `Announcement_Panel` → **UI** → **TextMeshPro - Text**
2. Rename to: `Announcement_Text`
3. Configure:
   ```
   RectTransform:
     ├── Anchor: Stretch-Stretch
     ├── Left: 50, Right: 50
     ├── Top: 100, Bottom: 150

   TextMeshProUGUI Component:
     ├── Text: "WAVE 1"
     ├── Font Size: 60
     ├── Font Style: Bold
     ├── Alignment: Center-Middle
     ├── Color: White
     ├── Enable Auto Sizing: ✓
     ├── Min: 40, Max: 80
     └── Wrapping: Disabled
   ```

**c) Create Sub Text:**
1. Right-click `Announcement_Panel` → **UI** → **TextMeshPro - Text**
2. Rename to: `Announcement_SubText`
3. Configure:
   ```
   RectTransform:
     ├── Anchor: Stretch-Stretch
     ├── Left: 50, Right: 50
     ├── Top: 150, Bottom: 100

   TextMeshProUGUI Component:
     ├── Text: "Get Ready!"
     ├── Font Size: 30
     ├── Alignment: Center-Top
     ├── Color: (200, 200, 200, 255)
     ├── Enable Auto Sizing: ✓
     ├── Min: 20, Max: 40
     └── Wrapping: Disabled
   ```

**d) Add AnnouncementSystem Component:**
1. Select `Announcement_Panel`
2. **Add Component** → Search: `AnnouncementSystem`
3. Configure:
   ```
   ╔══ Announcement Components ══╗
   Announcement Panel: [Drag Announcement_Panel itself]
   Announcement Text: [Drag Announcement_Text]
   Sub Text: [Drag Announcement_SubText]

   ╔══ Animation Settings ══╗
   Fade In Duration: 0.5
   Display Duration: 2
   Fade Out Duration: 0.5
   Scale Animation: ✓
   Scale From: 0.5
   Scale To: 1

   ╔══ Wave Announcement Settings ══╗
   Wave Start Prefix: "WAVE"
   Wave Complete Text: "WAVE COMPLETE!"
   Wave Start Color: (255, 255, 255, 255) ← White
   Wave Complete Color: (0, 255, 0, 255) ← Green
   ```

**e) Link to HUDManager:**
1. Select `HUD_Canvas`
2. Find `HUDManager` component
3. Assign:
   ```
   Announcement System: [Drag Announcement_Panel]
   ```

---

## Game Over Screen Setup

### 13. Create Game Over Panel

**a) Create Game Over Canvas (New Canvas for overlay):**
1. Hierarchy → Right-click → **UI** → **Canvas**
2. Rename to: `GameOver_Canvas`
3. Configure:
   ```
   Render Mode: Screen Space - Overlay
   Sort Order: 10 ← Above HUD
   Canvas Scaler:
     ├── UI Scale Mode: Scale With Screen Size
     ├── Reference Resolution: (1080, 1920)
     └── Match: 0.5
   ```

**b) Create Background Panel:**
1. Right-click `GameOver_Canvas` → **UI** → **Panel**
2. Rename to: `GameOver_Panel`
3. Configure:
   ```
   RectTransform:
     ├── Anchor: Stretch-Stretch (full screen)
     ├── Left: 0, Right: 0, Top: 0, Bottom: 0

   Panel Image:
     └── Color: (0, 0, 0, 200) ← Semi-transparent black

   Canvas Group (Add Component):
     ├── Alpha: 0
     ├── Interactable: ✗
     └── Block Raycasts: ✗
   ```

**c) Create Game Over Title:**
1. Right-click `GameOver_Panel` → **UI** → **TextMeshPro - Text**
2. Rename to: `GameOver_Title`
3. Configure:
   ```
   RectTransform:
     ├── Anchor: Top-Center
     ├── Position X: 0
     ├── Position Y: -300
     ├── Width: 800
     ├── Height: 120

   TextMeshProUGUI Component:
     ├── Text: "GAME OVER"
     ├── Font Size: 80
     ├── Font Style: Bold
     ├── Alignment: Center-Middle
     ├── Color: (255, 100, 100, 255) ← Light red
     ├── Enable Auto Sizing: ✓
     ├── Min: 60, Max: 100
     └── Wrapping: Disabled
   ```

**d) Create Stats Container:**
1. Right-click `GameOver_Panel` → **UI** → **Panel**
2. Rename to: `Stats_Container`
3. Configure:
   ```
   RectTransform:
     ├── Anchor: Center
     ├── Position: (0, 0)
     ├── Width: 600
     ├── Height: 400

   Panel Image:
     └── Color: (0, 0, 0, 0) ← Transparent
   ```

**e) Create Wave Reached Text:**
1. Right-click `Stats_Container` → **UI** → **TextMeshPro - Text**
2. Rename to: `WaveReached_Text`
3. Configure:
   ```
   RectTransform:
     ├── Anchor: Top-Center
     ├── Position Y: -50
     ├── Width: 500
     ├── Height: 60

   TextMeshProUGUI Component:
     ├── Text: "Wave Reached: 5"
     ├── Font Size: 36
     ├── Alignment: Center-Middle
     ├── Color: White
     ├── Enable Auto Sizing: ✓
     ├── Min: 28, Max: 40
     └── Wrapping: Disabled
   ```

**f) Create Kills Text:**
1. Right-click `Stats_Container` → **UI** → **TextMeshPro - Text**
2. Rename to: `Kills_Text`
3. Configure:
   ```
   RectTransform:
     ├── Anchor: Top-Center
     ├── Position Y: -130
     ├── Width: 500
     ├── Height: 60

   TextMeshProUGUI Component:
     ├── Text: "Total Kills: 42"
     ├── Font Size: 32
     ├── Alignment: Center-Middle
     ├── Color: (255, 215, 0, 255) ← Gold
     ├── Enable Auto Sizing: ✓
     ├── Min: 24, Max: 36
     └── Wrapping: Disabled
   ```

**g) Create Survival Time Text:**
1. Right-click `Stats_Container` → **UI** → **TextMeshPro - Text**
2. Rename to: `SurvivalTime_Text`
3. Configure:
   ```
   RectTransform:
     ├── Anchor: Top-Center
     ├── Position Y: -200
     ├── Width: 500
     ├── Height: 60

   TextMeshProUGUI Component:
     ├── Text: "Survival Time: 123.5s"
     ├── Font Size: 28
     ├── Alignment: Center-Middle
     ├── Color: (200, 200, 200, 255)
     ├── Enable Auto Sizing: ✓
     ├── Min: 20, Max: 32
     └── Wrapping: Disabled
   ```

**h) Create Restart Button:**
1. Right-click `GameOver_Panel` → **UI** → **Button - TextMeshPro**
2. Rename to: `Restart_Button`
3. Configure:
   ```
   RectTransform:
     ├── Anchor: Bottom-Center
     ├── Position X: 0
     ├── Position Y: 350
     ├── Width: 400
     ├── Height: 100

   Button Component:
     └── Colors: Default

   Button → Text (TMP):
     ├── Text: "RESTART"
     ├── Font Size: 40
     ├── Font Style: Bold
     ├── Alignment: Center-Middle
     ├── Color: White
     └── Enable Auto Sizing: ✓ (optional)
   ```

**i) Create Quit Button:**
1. Right-click `GameOver_Panel` → **UI** → **Button - TextMeshPro**
2. Rename to: `Quit_Button`
3. Configure:
   ```
   RectTransform:
     ├── Anchor: Bottom-Center
     ├── Position X: 0
     ├── Position Y: 220
     ├── Width: 400
     ├── Height: 100

   Button Component:
     └── Colors: Default

   Button → Text (TMP):
     ├── Text: "QUIT"
     ├── Font Size: 40
     ├── Alignment: Center-Middle
     ├── Color: (200, 200, 200, 255)
     └── Enable Auto Sizing: ✓ (optional)
   ```

**j) Add GameOverScreen Component:**
1. Select `GameOver_Canvas`
2. **Add Component** → Search: `GameOverScreen`
3. Configure:
   ```
   ╔══ Game Over Panel ══╗
   Game Over Panel: [Drag GameOver_Panel]
   Canvas Group: [Should auto-assign from GameOver_Panel]

   ╔══ Text Elements ══╗
   Game Over Title: [Drag GameOver_Title]
   Wave Reached Text: [Drag WaveReached_Text]
   Kills Text: [Drag Kills_Text]
   Survival Time Text: [Drag SurvivalTime_Text]

   ╔══ Buttons ══╗
   Restart Button: [Drag Restart_Button]
   Quit Button: [Drag Quit_Button]

   ╔══ Display Settings ══╗
   Fade In Duration: 0.5
   Delay Before Show: 1

   ╔══ Text Formats ══╗
   Wave Format: "Wave Reached: {0}"
   Kills Format: "Total Kills: {0}"
   Survival Time Format: "Survival Time: {0:F1}s"
   ```

---

## Updating Existing Objects

### 14. Update GameManager

**Add Event Integration:**
1. Select `GameManager` object in scene
2. GameManager component should already exist
3. No changes needed - Phase 2 updates are in the script

### 15. Disable TestEnemySpawner (if exists)

**Remove old spawner:**
1. Find `TestEnemySpawner` script on any object
2. Remove the component or disable it
3. WaveManager now handles all spawning

---

## Final Hierarchy Structure

Your scene hierarchy should look like:

```
Scene
├── GameManager
├── PoolManager
├── WaveManager
├── EnemySpawner
├── SpawnPoints (optional)
│   ├── SpawnPoint_0
│   ├── SpawnPoint_1
│   ├── SpawnPoint_2
│   └── SpawnPoint_3
├── Player (spawned at runtime)
├── Main Camera
├── Directional Light
├── Arena (floor)
├── UI_Canvas (Phase 1 - Joystick)
│   ├── JoystickContainer
│   │   ├── Background
│   │   └── Handle
├── HUD_Canvas
│   ├── HUDManager (component)
│   ├── HealthBar_Panel
│   │   ├── HealthBar (component)
│   │   ├── HealthBar_Background
│   │   │   └── HealthBar_Fill
│   │   └── HealthBar_Text
│   ├── Wave_Text
│   ├── EnemyCount_Text
│   ├── KillCount_Text
│   └── Announcement_Panel
│       ├── AnnouncementSystem (component)
│       ├── Announcement_Text
│       └── Announcement_SubText
└── GameOver_Canvas
    └── GameOver_Panel
        ├── GameOverScreen (component)
        ├── GameOver_Title
        ├── Stats_Container
        │   ├── WaveReached_Text
        │   ├── Kills_Text
        │   └── SurvivalTime_Text
        ├── Restart_Button
        └── Quit_Button
```

---

## Testing Checklist

### ✓ Combat System Test
- [ ] Player attacks nearby enemies automatically
- [ ] Enemies attack player when in range
- [ ] Player flashes red when taking damage
- [ ] Enemies flash red when taking damage
- [ ] Player health bar decreases when hit
- [ ] Enemies die and fade out after health depleted
- [ ] Attack animations play (scale punch)

### ✓ Wave System Test
- [ ] Wave 1 starts automatically after 1 second
- [ ] Wave announcement appears "WAVE 1"
- [ ] Enemies spawn sequentially with 0.5s delay
- [ ] Wave counter shows current wave number
- [ ] Enemy counter shows "X/Y" format
- [ ] After all enemies killed, "WAVE COMPLETE!" appears
- [ ] Wave 2 starts after 2 second delay
- [ ] Wave 2 enemies are stronger (more HP, damage, speed)
- [ ] Spawn points rotate (NE → NW → SE → SW)

### ✓ HUD System Test
- [ ] Health bar starts at 100%
- [ ] Health bar is green when full
- [ ] Health bar turns yellow at 50%
- [ ] Health bar turns red at 25%
- [ ] Health bar smoothly animates down
- [ ] Health text shows "X / Y" format
- [ ] Wave counter updates each wave
- [ ] Enemy counter updates as enemies spawn/die
- [ ] Kill counter increments on enemy death

### ✓ Game Over Screen Test
- [ ] Player dies when health reaches 0
- [ ] Player fades out and shrinks
- [ ] Game Over screen appears after 1 second
- [ ] Statistics display correctly (wave, kills, time)
- [ ] Restart button restarts the game
- [ ] All systems reset properly on restart

### ✓ Event System Test
- [ ] PlayerStats triggers OnPlayerHealthChanged
- [ ] DamageHandler triggers OnPlayerDeath
- [ ] CombatManager triggers OnPlayerAttack/OnEnemyAttack
- [ ] WaveManager triggers OnWaveStart/OnWaveComplete
- [ ] No null reference errors in console

---

## Troubleshooting

### Common Issues:

**1. "No enemies spawning"**
- ✓ Check WaveManager has EnemySpawner assigned
- ✓ Check PoolManager has Enemy prefab assigned
- ✓ Check spawn points exist or auto-create enabled
- ✓ Verify Player has "Player" tag
- ✓ Check console for spawn errors

**2. "Health bar not updating"**
- ✓ Verify PlayerStats has health values set
- ✓ Check HealthBar has Fill Image assigned
- ✓ Make sure PlayerStats is triggering health change events
- ✓ Check GameEvents subscriptions in HealthBar

**3. "Combat not working"**
- ✓ Player layer is "Player", Enemy layer is "Enemy"
- ✓ CombatManager Target Layer matches enemy/player layer
- ✓ Attack Range is appropriate (1.5 for player, 1.0 for enemy)
- ✓ Stats components have Damage values set
- ✓ Both entities have Rigidbody and Collider

**4. "Wave announcements not showing"**
- ✓ AnnouncementSystem has all text fields assigned
- ✓ Canvas Group exists on Announcement_Panel
- ✓ Check alpha starts at 0
- ✓ Verify GameEvents.OnWaveStart is triggered

**5. "Game Over screen doesn't appear"**
- ✓ GameOverScreen component assigned to canvas
- ✓ All text fields assigned in inspector
- ✓ Buttons assigned (Restart, Quit)
- ✓ Check player actually dies (health reaches 0)
- ✓ Verify OnPlayerDeath event is triggered

**6. "Enemies too weak/strong"**
- ✓ Check WaveConfig.cs formulas:
  - Health: 50 + (wave * 10)
  - Damage: 5 + (wave * 2)
  - Speed: 2 + (wave * 0.1)
- ✓ Adjust values in WaveConfig.GenerateForWave()

**7. "Performance issues"**
- ✓ Ensure object pooling is active
- ✓ Check PoolManager pool size (default 20)
- ✓ Reduce spawn count per wave if needed
- ✓ Disable Gizmos in Game view

**8. "Null Reference Exceptions"**
- ✓ Check all Inspector references are assigned
- ✓ Verify script execution order (Edit → Project Settings → Script Execution Order)
- ✓ Make sure GameManager exists in scene
- ✓ Ensure PoolManager initialized before spawning

### Debug Commands:

Open Unity console to see debug logs:
- `[WaveManager]` - Wave state changes
- `[EnemySpawner]` - Spawn events
- `[CombatManager]` - Attack events
- `[DamageHandler]` - Damage and death
- `[GameEvents]` - Event triggers

**Context Menu Debug Options:**
- WaveManager → Right-click → "Start Next Wave"
- WaveManager → Right-click → "Skip to Wave 5"
- WaveManager → Right-click → "Log Wave Stats"
- EnemySpawner → Right-click → "Test Spawn Single Enemy"
- GameOverScreen → Right-click → "Test Game Over Screen"

---

## Advanced Configuration

### Customizing Wave Difficulty:

Edit `WaveConfig.cs` line 30-40:
```csharp
public static WaveConfig GenerateForWave(int waveNumber)
{
    return new WaveConfig
    {
        waveNumber = waveNumber,
        enemyCount = Mathf.Min(waveNumber, 10),  // Cap at 10
        enemyHealth = 50f + (waveNumber * 10f),  // +10 HP per wave
        enemySpeed = 2f + (waveNumber * 0.1f),   // +0.1 speed per wave
        enemyDamage = 5f + (waveNumber * 2f),    // +2 damage per wave
        spawnDelay = 0.5f                        // Delay between spawns
    };
}
```

### Customizing Colors:

**Health Bar Colors:**
- Healthy: RGB(0, 255, 0) - Green
- Warning: RGB(255, 255, 0) - Yellow
- Critical: RGB(255, 0, 0) - Red

**Damage Flash:**
- Player: RGB(255, 0, 0) - Red
- Enemy: RGB(255, 100, 100) - Light Red

**UI Text:**
- Wave: RGB(255, 255, 255) - White
- Enemy Count: RGB(255, 200, 200) - Light Red
- Kills: RGB(255, 215, 0) - Gold

---

## Next Steps

After completing Phase 2 setup:

1. **Test thoroughly** using checklist above
2. **Tune balance** by adjusting wave formulas
3. **Gather feedback** on difficulty curve
4. **Prepare for Phase 3** (Part Collection System)

**Phase 2 Complete! ✅**

You now have:
- ✅ Functional auto-combat system
- ✅ Progressive wave spawning
- ✅ Real-time HUD with statistics
- ✅ Game over flow with restart

---

## Support

If you encounter issues:
1. Check console for error messages
2. Verify all references in Inspector
3. Review troubleshooting section
4. Test with Context Menu debug commands
5. Check that all prefabs are updated and applied

**Remember:** Always click **Apply** on prefabs after making changes!
