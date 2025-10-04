# Phase 3 Unity Setup Guide - Parts System
## Complete Unity Configuration for Part Collection, Equipment, and Stat Modification

---

## Table of Contents
1. [Overview](#overview)
2. [Prerequisites](#prerequisites)
3. [Part 1: Creating Part Data Assets](#part-1-creating-part-data-assets)
4. [Part 2: Setting Up Part Drop System](#part-2-setting-up-part-drop-system)
5. [Part 3: Setting Up Player Part Slots](#part-3-setting-up-player-part-slots)
6. [Part 4: Setting Up Part UI](#part-4-setting-up-part-ui)
7. [Part 5: Creating Dummy Part Prefabs](#part-5-creating-dummy-part-prefabs)
8. [Part 6: Testing](#part-6-testing)
9. [Troubleshooting](#troubleshooting)

---

## Overview

Phase 3 implements the core progression system: parts that drop from enemies, can be picked up by the player, and provide stat bonuses. This uses **dummy visuals** (colored primitives) to focus on gameplay mechanics before visual polish.

**What you'll set up:**
- 12 Part Data ScriptableObjects (4 types × 3 rarities)
- Part Drop Manager with object pooling
- Character Part Manager on player
- Part Inventory UI display
- Pickup notification system
- Dummy part prefabs

**Estimated Setup Time:** 45-60 minutes

---

## Prerequisites

✅ **Phase 2 must be complete** - Combat and wave systems working
✅ **All Phase 3 scripts compiled** without errors
✅ **Scene open:** Your main game scene
✅ **Backup saved:** Save a copy of your project before proceeding

---

## Part 1: Creating Part Data Assets

Part Data ScriptableObjects define the properties of each part type and rarity.

### 1.1 Create Part Data Folder

1. In **Project** window, navigate to `Assets/`
2. Create folder structure: `Assets/Data/Parts/`
3. You should have:
   ```
   Assets/
   └── Data/
       └── Parts/
   ```

### 1.2 Create Common Parts (4 parts)

**Common Head:**
1. Right-click in `Assets/Data/Parts/` → **Create** → **Bulk Up Heroes** → **Part Data**
2. Name: `Part_Head_Common`
3. Select it, configure in Inspector:
   ```
   Part Identification:
   ├── Part Type: Head
   ├── Rarity: Common
   └── Display Name: "Common Head"

   Stat Modifiers:
   ├── Health Bonus: 0
   ├── Damage Multiplier: 0
   ├── Attack Speed Multiplier: 0.5 (50% bonus)
   └── Move Speed Multiplier: 0

   Dummy Visual Data:
   ├── Drop Prefab: (leave empty for now)
   ├── Equipped Prefab: (leave empty for now)
   ├── Part Color: Red (255, 100, 100)
   └── Shape Type: Sphere
   ```

**Common Arms:**
1. Create new Part Data: `Part_Arms_Common`
2. Configure:
   ```
   Part Identification:
   ├── Part Type: Arms
   ├── Rarity: Common
   └── Display Name: "Common Arms"

   Stat Modifiers:
   ├── Health Bonus: 0
   ├── Damage Multiplier: 1.0 (100% bonus)
   ├── Attack Speed Multiplier: 0
   └── Move Speed Multiplier: 0

   Dummy Visual Data:
   ├── Part Color: Blue (100, 150, 255)
   └── Shape Type: Cube
   ```

**Common Torso:**
1. Create new Part Data: `Part_Torso_Common`
2. Configure:
   ```
   Part Identification:
   ├── Part Type: Torso
   ├── Rarity: Common
   └── Display Name: "Common Torso"

   Stat Modifiers:
   ├── Health Bonus: 1.0 (100% bonus)
   ├── Damage Multiplier: 0
   ├── Attack Speed Multiplier: 0
   └── Move Speed Multiplier: 0

   Dummy Visual Data:
   ├── Part Color: Green (100, 255, 100)
   └── Shape Type: Cylinder
   ```

**Common Legs:**
1. Create new Part Data: `Part_Legs_Common`
2. Configure:
   ```
   Part Identification:
   ├── Part Type: Legs
   ├── Rarity: Common
   └── Display Name: "Common Legs"

   Stat Modifiers:
   ├── Health Bonus: 0
   ├── Damage Multiplier: 0
   ├── Attack Speed Multiplier: 0
   └── Move Speed Multiplier: 0.3 (30% bonus)

   Dummy Visual Data:
   ├── Part Color: Yellow (255, 255, 100)
   └── Shape Type: Capsule
   ```

### 1.3 Create Rare Parts (4 parts)

Follow same process as Common, but:
- Set **Rarity: Rare**
- Same stat values (rarity multiplier is automatic: 1.5x)
- Brighter colors:
  - Head: Red (255, 150, 150)
  - Arms: Blue (150, 200, 255)
  - Torso: Green (150, 255, 150)
  - Legs: Yellow (255, 255, 150)

**Files to create:**
- `Part_Head_Rare`
- `Part_Arms_Rare`
- `Part_Torso_Rare`
- `Part_Legs_Rare`

### 1.4 Create Epic Parts (4 parts)

Follow same process, but:
- Set **Rarity: Epic**
- Same stat values (rarity multiplier is automatic: 2.0x)
- Brightest colors:
  - Head: Red (255, 200, 200)
  - Arms: Blue (200, 220, 255)
  - Torso: Green (200, 255, 200)
  - Legs: Yellow (255, 255, 200)

**Files to create:**
- `Part_Head_Epic`
- `Part_Arms_Epic`
- `Part_Torso_Epic`
- `Part_Legs_Epic`

### 1.5 Verify Part Data Assets

You should now have **12 Part Data assets** in `Assets/Data/Parts/`:
```
Part_Head_Common.asset    Part_Head_Rare.asset    Part_Head_Epic.asset
Part_Arms_Common.asset    Part_Arms_Rare.asset    Part_Arms_Epic.asset
Part_Torso_Common.asset   Part_Torso_Rare.asset   Part_Torso_Epic.asset
Part_Legs_Common.asset    Part_Legs_Rare.asset    Part_Legs_Epic.asset
```

---

## Part 2: Setting Up Part Drop System

### 2.1 Create Part Drop Manager Object

1. In **Hierarchy**, create empty GameObject
2. Name: `PartDropManager`
3. Position: (0, 0, 0)
4. **Add Component** → `PartDropManager` script

### 2.2 Configure Part Drop Manager

Select `PartDropManager`, configure in Inspector:

**Drop Chances by Wave:**
```
Element 0:
├── Min Wave: 1
├── Max Wave: 2
├── Drop Chance: 1.0 (100%)
├── Common Weight: 90
├── Rare Weight: 10
└── Epic Weight: 0

Element 1:
├── Min Wave: 3
├── Max Wave: 4
├── Drop Chance: 0.8 (80%)
├── Common Weight: 70
├── Rare Weight: 25
└── Epic Weight: 5

Element 2:
├── Min Wave: 5
├── Max Wave: 6
├── Drop Chance: 0.7 (70%)
├── Common Weight: 60
├── Rare Weight: 30
└── Epic Weight: 10

Element 3:
├── Min Wave: 7
├── Max Wave: 999
├── Drop Chance: 0.6 (60%)
├── Common Weight: 50
├── Rare Weight: 35
└── Epic Weight: 15
```

**Part Data:**
- Size: 12
- Drag all 12 Part Data assets from `Assets/Data/Parts/` into array

**Pickup Prefab:**
- (We'll create and assign this in Part 5)

**Pool Settings:**
```
├── Initial Pool Size: 20
├── Max Parts On Ground: 10
└── Pool Parent: (leave empty, auto-creates)
```

### 2.3 Verify Drop Manager

- Check Console for: `[PartDropManager] Initialized pickup pool with 20 objects`
- If error appears, make sure all Part Data assets are assigned

---

## Part 3: Setting Up Player Part Slots

### 3.1 Create Part Slot Hierarchy

1. Select **Player** GameObject in Hierarchy
2. Right-click Player → **Create Empty**
3. Name: `PartSlots`
4. Position: (0, 0, 0) relative to Player
5. Under `PartSlots`, create 4 empty GameObjects:
   - `HeadSlot` - Position: (0, 1.2, 0)
   - `ArmsSlot` - Position: (0, 0.5, 0)
   - `TorsoSlot` - Position: (0, 0, 0)
   - `LegsSlot` - Position: (0, -0.7, 0)

**Your hierarchy should look like:**
```
Player
├── PlayerModel (your existing player visual)
├── PartSlots
│   ├── HeadSlot
│   ├── ArmsSlot
│   ├── TorsoSlot
│   └── LegsSlot
└── (other player components)
```

### 3.2 Add Character Part Manager

1. Select **Player** GameObject
2. **Add Component** → `CharacterPartManager` script
3. Configure:
   ```
   Part Slots:
   ├── Head Slot: Drag HeadSlot from Hierarchy
   ├── Arms Slot: Drag ArmsSlot from Hierarchy
   ├── Torso Slot: Drag TorsoSlot from Hierarchy
   └── Legs Slot: Drag LegsSlot from Hierarchy

   References:
   └── Player Stats: Drag Player's PlayerStats component
   ```

### 3.3 Verify Player Setup

- Player should have both `PlayerStats` and `CharacterPartManager` components
- All 4 part slots assigned
- Check Console for: `[CharacterPartManager] Part slots initialized`

---

## Part 4: Setting Up Part UI

### 4.1 Create Part Inventory Display

1. Select **HUD Canvas** in Hierarchy
2. Right-click HUD Canvas → **UI** → **Panel**
3. Name: `PartInventoryPanel`
4. Configure RectTransform:
   ```
   Anchor Preset: Top-Left (hold SHIFT+ALT while clicking)
   Position X: 120
   Position Y: -120 (below health bar)
   Width: 180
   Height: 120
   ```
5. Panel **Image** component:
   ```
   Color: (0, 0, 0, 80) // Semi-transparent black
   ```

### 4.2 Create Part Slot Text Elements

Under `PartInventoryPanel`, create 4 text elements:

**Head Text:**
1. Right-click PartInventoryPanel → **UI** → **TextMeshPro - Text**
2. Name: `HeadText`
3. Configure:
   ```
   RectTransform:
   ├── Anchor: Top-Stretch
   ├── Position Y: -15
   ├── Height: 25
   └── Left/Right: 10

   TextMeshPro:
   ├── Text: "Head: Empty"
   ├── Font Size: 14
   ├── Color: Gray (128, 128, 128)
   ├── Alignment: Left, Middle
   └── Wrapping: Disabled
   ```

**Arms Text:**
1. Duplicate `HeadText` (CTRL+D)
2. Name: `ArmsText`
3. Position Y: -45
4. Text: "Arms: Empty"

**Torso Text:**
1. Duplicate `ArmsText`
2. Name: `TorsoText`
3. Position Y: -75
4. Text: "Torso: Empty"

**Legs Text:**
1. Duplicate `TorsoText`
2. Name: `LegsText`
3. Position Y: -105
4. Text: "Legs: Empty"

### 4.3 Add Part Inventory UI Script

1. Select `PartInventoryPanel`
2. **Add Component** → `PartInventoryUI` script
3. Configure:
   ```
   Part Slot Text Elements:
   ├── Head Text: Drag HeadText
   ├── Arms Text: Drag ArmsText
   ├── Torso Text: Drag TorsoText
   └── Legs Text: Drag LegsText

   Display Settings:
   ├── Empty Slot Text: "Empty"
   └── Empty Slot Color: Gray (128, 128, 128)
   ```

### 4.4 Create Pickup Notification

1. Select **HUD Canvas**
2. Right-click HUD Canvas → **UI** → **Panel**
3. Name: `PickupNotificationPanel`
4. Configure RectTransform:
   ```
   Anchor Preset: Top-Center
   Position Y: -200
   Width: 400
   Height: 80
   ```
5. Panel **Image** component:
   ```
   Color: (0, 0, 0, 0) // Transparent (we only want text visible)
   Raycast Target: OFF (uncheck)
   ```

**Create Title Text:**
1. Right-click PickupNotificationPanel → **UI** → **TextMeshPro - Text**
2. Name: `TitleText`
3. Configure:
   ```
   RectTransform:
   ├── Anchor: Top-Stretch
   ├── Position Y: -15
   ├── Height: 40

   TextMeshPro:
   ├── Text: "ARMS ACQUIRED!"
   ├── Font Size: 32
   ├── Color: White
   ├── Alignment: Center, Middle
   ├── Font Style: Bold
   └── Auto Size: Disabled
   ```

**Create Subtitle Text:**
1. Right-click PickupNotificationPanel → **UI** → **TextMeshPro - Text**
2. Name: `SubtitleText`
3. Configure:
   ```
   RectTransform:
   ├── Anchor: Bottom-Stretch
   ├── Position Y: 10
   ├── Height: 30

   TextMeshPro:
   ├── Text: "+100% Damage"
   ├── Font Size: 20
   ├── Color: White
   └── Alignment: Center, Middle
   ```

### 4.5 Add Pickup Notification Script

1. Select `PickupNotificationPanel`
2. **Add Component** → `PartPickupNotification` script
3. Configure:
   ```
   UI References:
   ├── Title Text: Drag TitleText
   ├── Subtitle Text: Drag SubtitleText
   └── Canvas Group: (auto-added, leave empty)

   Animation Settings:
   ├── Display Duration: 2.0
   ├── Fade In Duration: 0.2
   ├── Fade Out Duration: 0.5
   └── Slide Up Distance: 50
   ```
4. **Add Component** → `Canvas Group` (if not auto-added)

### 4.6 Create Stat Popup Manager

1. In **Hierarchy**, find **HUD Canvas**
2. Create empty child: **Right-click HUD Canvas** → **Create Empty**
3. Name: `PartStatPopupManager`
4. **Add Component** → `PartStatPopupManager` script
5. Configure:
   ```
   References:
   ├── Canvas: Drag HUD Canvas
   └── Player Transform: Drag Player from Hierarchy

   Popup Settings:
   ├── Popup Offset: (0, 2, 0)
   └── Show Popups: ✓ (checked)
   ```

---

## Part 5: Creating Dummy Part Prefabs

### 5.1 Create Pickup Prefab

**Create Base Pickup Object:**
1. In **Hierarchy**, create **3D Object** → **Sphere**
2. Name: `PartPickup`
3. Configure Transform:
   ```
   Position: (0, 10, 0) // Out of view temporarily
   Scale: (0.3, 0.3, 0.3)
   ```

**Configure Components:**
1. Select `PartPickup`
2. Remove **Sphere Collider** component (delete it)
3. **Add Component** → **Capsule Collider**
4. Configure Capsule Collider:
   ```
   ├── Is Trigger: ✓ (checked)
   ├── Radius: 0.5
   └── Height: 1.0
   ```
5. **Add Component** → `PartPickup` script
6. Configure PartPickup:
   ```
   Visual Settings:
   ├── Hover Height: 0.5
   ├── Hover Speed: 1.0
   ├── Rotation Speed: 45
   └── Bounce Amplitude: 0.1

   Despawn Settings:
   ├── Despawn Time: 5.0
   └── Fade Duration: 0.5
   ```

**Create Prefab:**
1. In **Project**, create folder: `Assets/Prefabs/Parts/`
2. Drag `PartPickup` from Hierarchy into `Assets/Prefabs/Parts/`
3. Delete `PartPickup` from Hierarchy (we only need the prefab)

### 5.2 Assign Pickup Prefab to Drop Manager

1. Select `PartDropManager` in Hierarchy
2. In Inspector, find **Pickup Prefab** field
3. Drag `PartPickup` prefab from `Assets/Prefabs/Parts/` into the field

### 5.3 Create Equipped Part Prefabs (Optional)

If you want custom visuals for equipped parts, create 4 prefabs:
- `Head_Equipped` (Red Sphere, scale 0.3)
- `Arms_Equipped` (Blue Cube, scale 0.6x0.3x0.3)
- `Torso_Equipped` (Green Cylinder, scale 0.5x0.7x0.4)
- `Legs_Equipped` (Yellow Capsule, scale 0.3x0.5x0.3)

Otherwise, CharacterPartManager will auto-create primitives.

---

## Part 6: Testing

### 6.1 Basic Functionality Test

**Test Drop System:**
1. Enter **Play Mode**
2. Kill an enemy
3. **Expected:** Part should drop at enemy's death location
4. **Expected:** Part should hover, rotate, and bounce
5. **Expected Console:** `[PartDropManager] Dropped [Rarity] [PartType] at [position]`

**Test Pickup:**
1. Walk player over dropped part
2. **Expected:** Part disappears
3. **Expected:** Part Inventory UI updates (e.g., "Head: Common")
4. **Expected:** Notification appears: "HEAD ACQUIRED!" with stat bonus
5. **Expected Console:** `[PartPickup] Player picked up [part name]`

**Test Stat Application:**
1. Open **Player** Inspector during Play Mode
2. Check **PlayerStats** component
3. Look at Damage, Health, Attack Speed, Move Speed values
4. Pick up an Arms part
5. **Expected:** Damage value should increase
6. **Example:** Base Damage 3 → 6 with Common Arms (100% bonus)

### 6.2 Rarity System Test

1. Kill 10 enemies in Wave 1
2. **Expected:** ~90% Common, ~10% Rare, 0% Epic drops
3. Progress to Wave 5
4. Kill 10 enemies
5. **Expected:** ~60% Common, ~30% Rare, ~10% Epic drops

### 6.3 Upgrade Test

1. Pick up a **Common Arms** part
2. Check Damage stat (should increase)
3. Pick up a **Rare Arms** part
4. **Expected:** Common Arms replaced with Rare Arms
5. **Expected:** Damage increases again (Rare = 1.5x multiplier)

### 6.4 Downgrade Prevention Test

1. Pick up an **Epic Head** part
2. Try to pick up a **Common Head** part
3. **Expected:** Common Head NOT picked up
4. **Expected Console:** `[PartPickup] Player already has better Head`

### 6.5 Wave Balance Test

**Wave 1 (No Parts):**
- Player Base Damage: 3
- Enemy Health: 20
- **Expected:** 7 hits to kill (20 / 3 ≈ 7)
- Should be challenging but doable

**Wave 3 (1-2 Parts):**
- With Common Arms: Damage = 6
- Enemy Health: 30
- **Expected:** 5 hits to kill (30 / 6 = 5)
- Should feel more powerful

**Wave 5+ (Full Set):**
- With full Common set: ~2x stats
- **Expected:** Waves become manageable with parts

### 6.6 UI Test

**Part Inventory UI:**
1. Check top-left corner below health bar
2. Should show: "Head: Empty", "Arms: Empty", etc.
3. Pick up parts
4. **Expected:** UI updates immediately
5. **Expected:** Text color matches rarity (Gray=Common, Blue=Rare, Purple=Epic)

**Pickup Notifications:**
1. Pick up a part
2. **Expected:** Center screen notification appears
3. **Expected:** Shows "ARMS ACQUIRED!" (or similar)
4. **Expected:** Shows stat bonus "+100% Damage"
5. **Expected:** Slides up and fades out after 2 seconds

**Floating Stat Text:**
1. Pick up a part
2. **Expected:** Text appears above player's head
3. **Expected:** Shows "+100% Damage" (or relevant stat)
4. **Expected:** Floats upward and fades out

---

## Troubleshooting

### Problem: Parts not dropping

**Symptoms:** Kill enemy, no part appears

**Solutions:**
1. Check `PartDropManager` is in scene
2. Verify Part Data array (Size: 12) is filled
3. Check Pickup Prefab is assigned
4. Check Console for errors
5. Verify Wave 1 Drop Chance is 1.0 (100%)

### Problem: "PartData not found" error

**Symptoms:** Console shows: `[PartDropManager] No part data found for [Rarity] [PartType]`

**Solutions:**
1. Make sure you created all 12 Part Data assets
2. Verify all Part Data assigned to PartDropManager's Part Data array
3. Check each Part Data has correct PartType and Rarity set

### Problem: Can't pick up parts

**Symptoms:** Walk over part, nothing happens

**Solutions:**
1. Check Player has `CharacterPartManager` component
2. Verify Player tag is "Player" (in Inspector)
3. Check PartPickup prefab has:
   - Collider with "Is Trigger" checked
   - PartPickup script attached
4. Check Console for: `[PartPickup] Player has no CharacterPartManager!`

### Problem: Stats not changing

**Symptoms:** Pick up part, stats don't increase

**Solutions:**
1. Select Player during Play Mode
2. Check `PlayerStats` component values in Inspector
3. Verify `CharacterPartManager` has PlayerStats reference assigned
4. Check Part Data has stat modifiers > 0
5. Look for Console message: `[PlayerStats] Damage updated: X (Base: Y, Modifier: +Z%)`

### Problem: UI not updating

**Symptoms:** Part Inventory UI stays "Empty" after pickup

**Solutions:**
1. Check `PartInventoryUI` component has all 4 text references assigned
2. Verify text objects are using TextMeshProUGUI (not legacy Text)
3. Check Console for: `[PartInventoryUI] Subscribed to GameEvents`
4. During Play Mode, look for: `[PartInventoryUI] Part equipped: [type] - [name]`

### Problem: Pickup notification not showing

**Symptoms:** Pick up part, no center screen notification

**Solutions:**
1. Check `PickupNotificationPanel` is active in Hierarchy
2. Verify `PartPickupNotification` component has:
   - Title Text assigned
   - Subtitle Text assigned
   - Canvas Group component present
3. Check Console for: `[PartPickupNotification] Showing notification for [part name]`

### Problem: Health bar breaks after pickup

**Symptoms:** Health bar shows wrong values after picking up Torso part

**Solutions:**
1. This is expected behavior - Max Health increased
2. Current health percentage is maintained
3. Example: 30/30 → 60/60 (full health maintained at 100%)
4. Example: 15/30 → 30/60 (50% health maintained)

### Problem: Parts fall through floor

**Symptoms:** Dropped parts appear then immediately disappear

**Solutions:**
1. Check PartPickup prefab has Hover Height: 0.5
2. Verify parts are spawning at correct Y position
3. Check floor collider exists and is not set as trigger
4. Look for Console message: `[PartDropManager] Dropped [part] at [position]`
   - Y position should be > 0

### Problem: Too many parts on ground

**Symptoms:** Parts everywhere, performance drops

**Solutions:**
1. Check `Max Parts On Ground` setting (should be 10)
2. Verify parts despawn after 5 seconds
3. Older parts should auto-remove when limit exceeded
4. Check Console for: `[PartPickup] Pickup effect for [rarity] part`

### Problem: Enemy stats feel wrong

**Symptoms:** Wave 1 too hard or too easy

**Solutions:**
1. Wave 1 enemies should have:
   - Health: 20
   - Damage: 8
2. Player without parts should have:
   - Health: 30
   - Damage: 3
3. Check `WaveConfig.cs` GenerateForWave() formula
4. Verify GameConstants values updated to Phase 3 values

---

## Phase 3 Completion Checklist

Before moving to Phase 4, verify:

✅ **Part System:**
- [ ] 12 Part Data assets created (4 types × 3 rarities)
- [ ] Parts drop from enemies based on wave
- [ ] Parts can be picked up by player
- [ ] Stats increase when parts equipped
- [ ] Rarity upgrade system works (Rare replaces Common)
- [ ] Can't downgrade (Epic stays over Common)

✅ **Visual System:**
- [ ] Parts hover and rotate when dropped
- [ ] Parts despawn after 5 seconds
- [ ] Dummy visuals attach to player when equipped
- [ ] Rarity affects visual scale

✅ **UI System:**
- [ ] Part Inventory UI displays equipped parts
- [ ] Pickup notifications appear at center screen
- [ ] Floating stat text appears above player
- [ ] All text uses TextMeshPro
- [ ] Colors match rarity (Common=Gray, Rare=Blue, Epic=Purple)

✅ **Balance:**
- [ ] Wave 1 challenging without parts
- [ ] Wave 3+ requires parts to survive
- [ ] Full Common set gives ~2x stats
- [ ] Full Epic set gives ~3x stats
- [ ] Player feels power progression

✅ **Performance:**
- [ ] Object pooling working (no instantiate during gameplay)
- [ ] Max 10 parts on ground at once
- [ ] No lag when multiple parts drop
- [ ] 60 FPS maintained on target device

---

## Next Steps: Phase 4

Once Phase 3 is complete and tested:
1. Replace dummy visuals with Synty character parts
2. Add smooth part swap animations
3. Implement advanced VFX for pickups
4. Add sound effects for each rarity
5. Polish UI with icons and better layouts

**Phase 3 Goal:** Make the game FUN through meaningful progression, not pretty. Colored cubes that make you powerful beat beautiful models that don't impact gameplay!

---

## Support

If you encounter issues not covered in troubleshooting:
1. Check Console for error messages
2. Verify all references are assigned (no "None" in Inspector)
3. Test with Context Menu commands (right-click component → Debug commands)
4. Review Phase3_plan.md for system specifications

Remember: Phase 3 is about mechanics, not visuals. If parts drop, player gets stronger, and progression feels good - you're done! ✅
