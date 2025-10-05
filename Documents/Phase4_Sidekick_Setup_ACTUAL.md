# Phase 4 - Actual Sidekick Integration Setup
## Simple Steps to Enable Sidekick Characters

**This replaces the previous guide. Follow these steps exactly.**

---

## What Changed?

The Sidekick system uses **SidekickRuntime API** (not simple components). I've now implemented the actual working integration. Here's how to set it up:

---

## Unity Setup (5 Minutes)

### Step 1: Add Components to Player

**Select Player GameObject in Hierarchy**

**Add these TWO components:**

1. **Add Component** → Search: `SidekickCharacterController`
2. **Add Component** → Search: `SidekickPartSwapper` (if not already there)

### Step 2: Configure SidekickCharacterController

**On Player GameObject, find SidekickCharacterController component:**

```
SidekickCharacterController:
├── Enable Sidekick: ✓ (checked)
├── Character Name: "Player Character"
├── Character Parent: [Leave empty or assign Player transform]
├── Base Model Path: "Meshes/SK_BaseModel" (default)
└── Base Material Path: "Materials/M_BaseMaterial" (default)
```

**Leave paths as default unless you moved Sidekick assets.**

### Step 3: Configure SidekickPartSwapper

**On Player GameObject, find SidekickPartSwapper component:**

```
SidekickPartSwapper:
├── Use Sidekick Integration: ✓ (checked)
├── Sidekick Controller: [Should auto-fill from same GameObject]
└── Fallback slots: [Keep your existing slot transforms]
```

### Step 4: Fill in Part IDs

**This is CRITICAL!** Open each PartData asset and fill in Sidekick Part IDs.

**Go to:** `Assets/Data/Parts/`

**For each part (Part_Head_Common, etc.):**

1. Click to select
2. Find "Sidekick Part ID" field
3. Enter the part name exactly as it appears in Sidekick

**Example Part IDs (adjust based on your Sidekick package):**

```
Part_Head_Common:
  Sidekick Part ID: Helmet_Basic

Part_Head_Rare:
  Sidekick Part ID: Helmet_Soldier

Part_Head_Epic:
  Sidekick Part ID: Helmet_Knight

Part_Arms_Common:
  Sidekick Part ID: Gloves_Leather

Part_Arms_Rare:
  Sidekick Part ID: Gauntlets_Light

Part_Arms_Epic:
  Sidekick Part ID: Gauntlets_Heavy

Part_Torso_Common:
  Sidekick Part ID: Torso_Shirt

Part_Torso_Rare:
  Sidekick Part ID: Torso_Armor_Light

Part_Torso_Epic:
  Sidekick Part ID: Torso_Armor_Heavy

Part_Legs_Common:
  Sidekick Part ID: Legs_Pants

Part_Legs_Rare:
  Sidekick Part ID: Legs_Armor_Light

Part_Legs_Epic:
  Sidekick Part ID: Legs_Armor_Heavy
```

**To find correct part names:**
1. Open Synty > Sidekick Character Tool
2. Browse parts in each category
3. Note the exact part names
4. Use those names in PartData assets

---

## Step 5: Test It!

**Press Play**

**Check Console for:**
```
[SidekickCharacterController] Initializing Sidekick Runtime API...
[SidekickCharacterController] Initialized successfully. Part library loaded with X categories
[SidekickCharacterController] Character rebuilt with X parts
[SidekickPartSwapper] Sidekick integration enabled and ready!
```

**If you see these messages: ✓ SUCCESS!**

**Pick up a part and check console:**
```
[SidekickPartSwapper] Swapped Head to 'Helmet_Knight' using Sidekick
[SidekickCharacterController] Equipping Head: Helmet_Knight
[SidekickCharacterController] Character rebuilt with X parts
```

---

## Troubleshooting

### Issue: "Failed to load base model"

**Console shows:** `Failed to load base model at 'Meshes/SK_BaseModel'`

**Solution:**
1. Check if file exists: `Assets/Synty/SidekickCharacters/Resources/Meshes/SK_BaseModel`
2. If in different location, update path in SidekickCharacterController
3. Path is relative to Resources folder

### Issue: "Part 'X' not found in library"

**Console shows:** `Part 'Helmet_Knight' not found in Head library`

**Solution:**
1. Part name doesn't match exactly
2. Open Synty > Sidekick Character Tool
3. Find the correct part name
4. Update PartData asset with exact name

### Issue: "Sidekick integration enabled and ready!" but no character visible

**Possible causes:**
1. Character created but positioned wrong
2. Check Scene view (not just Game view)
3. Look for "Player Character" GameObject in Hierarchy
4. It should be created under Player

**Solution:**
- In SidekickCharacterController, assign "Character Parent" to Player transform
- Check character is not behind camera

### Issue: "SidekickCharacterController not initialized"

**Console shows errors during initialization**

**Solutions:**
- Check Sidekick package is properly imported
- Verify Resources folder has Meshes and Materials
- Check Unity console for specific error message

---

## What You Should See

**Before picking up parts:**
- Minimal/basic character (base body)
- Character should be visible in Scene view

**After picking up Common Head:**
- Helmet/head armor appears
- Character rebuilds with new part
- Stats increase

**After picking up all 4 Epic parts:**
- Fully armored character
- All parts visible
- Character looks professional

---

## Important Notes

### How This System Works

**Different from previous approach:**
- ❌ No BaseCharacter prefab needed
- ❌ No manual Sidekick Character Creator setup
- ✓ Character built at runtime from code
- ✓ Automatically combines meshes
- ✓ Parts swap by rebuilding character

**When you equip a part:**
1. System stores part name
2. Destroys old character
3. Builds new character with all current parts
4. Spawns under Player

### Part Naming

**CRITICAL:** Part IDs must match EXACTLY (case-sensitive!)

**Examples:**
- ✓ "Helmet_Knight" matches "Helmet_Knight"
- ❌ "helmet_knight" does NOT match "Helmet_Knight"
- ❌ "HelmetKnight" does NOT match "Helmet_Knight"

**The system tries partial matching, but exact is best.**

### Performance

**Character rebuilding is fast (<0.1 seconds)**
- Only happens when part changes
- Not every frame
- Sidekick handles mesh combining
- Should be smooth even on mobile

---

## Fallback Mode

**If Sidekick fails to initialize:**
- System automatically falls back to dummy visuals
- All gameplay still works
- You'll see colored cubes instead of character models
- Check console to see why Sidekick failed

---

## Debug Commands

**Use SidekickDebugHelper:**

1. Select Player in Hierarchy
2. Find SidekickDebugHelper component
3. Right-click component header
4. Choose "Log Setup Status"
5. Read console output

**This tells you:**
- Is Sidekick initialized?
- Is controller found?
- Is character created?
- Are part IDs set?

---

## Next Steps

**Once working:**
1. ✓ Test all part types (Head, Arms, Torso, Legs)
2. ✓ Test all rarities (Common, Rare, Epic)
3. ✓ Check character looks good
4. ✓ Verify stats still work
5. ✓ Test performance (should be smooth)

**Then move to:**
- Polish character appearance
- Adjust part IDs for better models
- Tune visual effects
- Move to Phase 5 (VFX, Audio)

---

## Quick Checklist

Before asking for help, verify:

- [ ] SidekickCharacterController component on Player
- [ ] SidekickPartSwapper component on Player
- [ ] Both components configured (checkboxes enabled)
- [ ] All 12 PartData assets have Sidekick Part IDs filled in
- [ ] Part IDs match actual Sidekick part names
- [ ] No errors in console during initialization
- [ ] Character created (check Hierarchy for "Player Character")

---

*This is the real Sidekick integration! Your setup from the previous guide is no longer needed - the system builds characters at runtime now.*
