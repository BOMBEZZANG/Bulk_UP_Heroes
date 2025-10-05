# Phase 4 Troubleshooting Guide
## Parts Not Appearing on Player

If parts are being picked up but not visually appearing on the player, follow these steps:

---

## Quick Diagnostic Steps

### Step 1: Add Debug Helper

1. **Select Player GameObject** in Hierarchy
2. **Add Component** > Search for "Sidekick Debug Helper"
3. **In Inspector**, right-click on component header
4. **Select "Log Setup Status"**
5. **Check Console** for diagnostic messages

---

## Common Issues & Solutions

### Issue 1: SidekickPartSwapper Not Found

**Console shows:** `❌ SidekickPartSwapper NOT FOUND on Player!`

**Solution:**
1. Select Player GameObject
2. Add Component > SidekickPartSwapper
3. Assign references in Inspector:
   - **Sidekick Character**: Drag BaseCharacter's SidekickCharacter component
   - **Use Sidekick Integration**: ✓ (checked)
   - **Head/Arms/Torso/Legs Slots**: Assign the slot transforms

---

### Issue 2: SidekickCharacter Reference Missing

**Console shows:** `⚠ SidekickCharacter component reference is NULL`

**Solution:**
1. **Find BaseCharacter** in scene (should be child of Player)
2. **Select Player** GameObject
3. **In SidekickPartSwapper component:**
   - Find "Sidekick Character" field
   - Drag BaseCharacter GameObject to this field
   - Unity should automatically grab the SidekickCharacter component

**If BaseCharacter doesn't exist:**
1. Create it using Synty > Sidekick Character Tool
2. Export as prefab: `BaseCharacter.prefab`
3. Drag into scene as child of Player

---

### Issue 3: Part IDs Not Set

**Console shows:** `❌ Missing Sidekick Part ID for: [Part Name]`

**Solution:**
1. Go to `Assets/Data/Parts/` folder
2. For each PartData asset (Part_Head_Common, etc.):
   - Click to select
   - In Inspector, scroll to "Synty Sidekick Integration" section
   - Fill in "Sidekick Part ID" field
   - Example: `MSC_Head_Basic_01`

**To find correct IDs:**
1. Open Synty > Sidekick Character Tool
2. Browse parts in each category
3. Select a part and note its name/ID
4. Copy exact ID to PartData asset

---

### Issue 4: SIDEKICK_CHARACTERS Define Not Set

**Console shows:** `SIDEKICK_CHARACTERS scripting define not set`

**Solution:**
1. Go to `Edit > Project Settings > Player`
2. Expand "Other Settings"
3. Find "Scripting Define Symbols"
4. Add: `SIDEKICK_CHARACTERS`
5. Click Apply
6. Wait for Unity to recompile

---

### Issue 5: Sidekick Not Available (Using Fallback)

**Console shows:** `⚠ Sidekick not available - will use fallback dummy visuals`

**This is actually OK!** The system will work with colored primitives instead of Sidekick meshes.

**To enable Sidekick mode:**
1. Make sure SIDEKICK_CHARACTERS define is set (see Issue 4)
2. Ensure BaseCharacter exists with SidekickCharacter component
3. Check "Use Sidekick Integration" is enabled on SidekickPartSwapper

---

## Manual Setup Checklist

If automatic setup didn't work, manually verify:

### On Player GameObject:

```
Player (GameObject)
├── PlayerController ✓
├── PlayerStats ✓
├── CharacterPartManager ✓
│   └── Part Swapper: [SidekickPartSwapper reference] ← CHECK THIS
├── SidekickPartSwapper ← MUST EXIST
│   ├── Use Sidekick Integration: ✓
│   ├── Sidekick Character: [BaseCharacter reference] ← CHECK THIS
│   ├── Head Slot: [Transform]
│   ├── Arms Slot: [Transform]
│   ├── Torso Slot: [Transform]
│   └── Legs Slot: [Transform]
├── CombatManager ✓
├── DamageHandler ✓
└── BaseCharacter (Child GameObject) ← MUST EXIST
    ├── SidekickCharacter ← MUST EXIST
    │   └── Enable Runtime Swap: ✓ ← CRITICAL!
    └── Animator ✓
```

---

## Step-by-Step Fix (If Nothing Works)

### 1. Verify BaseCharacter Setup

**In Scene Hierarchy:**
1. Expand Player GameObject
2. Find "BaseCharacter" child
3. Select it

**In Inspector (BaseCharacter selected):**
1. Verify "SidekickCharacter" component exists
2. Check "Enable Runtime Swap" is ✓
3. Note this GameObject reference

### 2. Setup SidekickPartSwapper

**Select Player GameObject**

**Add SidekickPartSwapper if missing:**
1. Add Component > SidekickPartSwapper

**Configure it:**
1. ✓ Use Sidekick Integration
2. Drag BaseCharacter to "Sidekick Character" field
3. Assign slot transforms:
   - Head Slot → Player/Slots/HeadSlot (or similar)
   - Arms Slot → Player/Slots/ArmsSlot
   - Torso Slot → Player/Slots/TorsoSlot
   - Legs Slot → Player/Slots/LegsSlot

### 3. Update CharacterPartManager

**Still on Player GameObject:**

1. Find CharacterPartManager component
2. Check "Part Swapper" field
3. If empty, drag SidekickPartSwapper component to it
4. (Usually auto-assigned, but check anyway)

### 4. Set Part IDs

**For EACH PartData asset:**

Open: `Assets/Data/Parts/Part_Head_Common`
```
Inspector:
├── Part Type: Head
├── Rarity: Common
└── Synty Sidekick Integration:
    └── Sidekick Part ID: [FILL THIS IN]
        Example: MSC_Head_Basic_01
```

Repeat for all 12 parts (4 types × 3 rarities).

### 5. Test in Play Mode

1. Start game
2. Pick up a part
3. **Check Console** for messages:
   - `✓ [SidekickPartSwapper] Swapped Head to 'MSC_Head_Basic_01'` = WORKING!
   - `⚠ Using fallback mode` = Using dummy visuals (OK, but not Sidekick)
   - `❌ Error` = Something wrong, read error message

---

## Console Message Reference

### Good Messages (Everything Working):

```
✓ [SidekickPartSwapper] Sidekick integration enabled
✓ [CharacterPartManager] Equipped Epic Head using Sidekick: MSC_Head_Armor_Heavy_01
✓ [SidekickPartSwapper] Swapped Head to 'MSC_Head_Armor_Heavy_01'
```

### Fallback Mode (Works, but uses dummy visuals):

```
⚠ [SidekickPartSwapper] Using fallback mode
✓ [CharacterPartManager] Created fallback visual for Epic Head
```

### Error Messages (Need fixing):

```
❌ [SidekickPartSwapper] Failed to swap part: [error message]
❌ [CharacterPartManager] SidekickPartSwapper not found!
❌ SidekickCharacter component reference is NULL
```

---

## Testing Commands

**Using Debug Helper (Context Menu):**

1. Select Player in Hierarchy
2. Right-click on SidekickDebugHelper component
3. Choose:
   - "Log Setup Status" - Check configuration
   - "Check All PartData Assets" - Verify Part IDs
   - "Test Part Swap" - Test swap functionality

---

## Still Not Working?

### Check Unity Console for:

1. **Red errors** - Must fix these first
2. **Yellow warnings** - May indicate setup issues
3. **Missing component errors** - Add missing components

### Verify Scene Setup:

1. Is Player prefab in scene? (not just in Project)
2. Is BaseCharacter a child of Player instance in scene?
3. Did you press Play? (Setup only visible in Play mode)

### Nuclear Option (Reset and Rebuild):

1. **Backup** your current Player prefab
2. Delete Player from scene
3. Create new empty GameObject named "Player"
4. Add all required components manually
5. Drag BaseCharacter prefab as child
6. Configure all references step by step
7. Save as new prefab

---

## Expected Behavior When Working

**Before picking up parts:**
- Player appears nearly naked (underwear only)
- BaseCharacter visible with default skin

**After picking up Common Head:**
- Helmet appears on head
- Health increases by 10%
- Console shows successful equip message

**After picking up all 4 Epic parts:**
- Character fully armored
- Purple glow/emission on parts
- All stats significantly increased
- Character looks badass!

---

## Need More Help?

1. Copy entire Console log output
2. Take screenshot of Player GameObject Inspector
3. Note which step failed
4. Check that all prerequisites from Phase 3 still work
