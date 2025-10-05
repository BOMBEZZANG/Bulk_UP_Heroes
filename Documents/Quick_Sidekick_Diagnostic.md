# Quick Sidekick Diagnostic & Fix

## Problem
You've set up Sidekick but parts aren't appearing on the player character.

## Root Cause
The part IDs in your PartData assets (like "Helmet_Knight") are examples that don't match the actual part names in your specific Sidekick package.

## Solution: 3-Step Fix

### Step 1: Discover Your Actual Part Names (5 minutes)

**In Unity:**

1. Create empty GameObject in scene: `GameObject > Create Empty`
2. Name it "PartLister"
3. Add component: `SidekickPartLister`
4. In Inspector, check both:
   - ✓ List On Start
   - ✓ Filter By Useful Parts
5. **Press Play**
6. Open Console window
7. **Copy the output** - you'll need these exact part names

**Example output you'll see:**
```
=== PARTS USEFUL FOR YOUR GAME ===

--- HEAD PARTS (use for game 'Head' slot) ---
  Head_Male_01
  Head_Male_02
  Helmet_Soldier_01
  ...

--- TORSO PARTS (use for game 'Torso' slot) ---
  Torso_Shirt_01
  Torso_Armor_Light_01
  ...

--- ARM PARTS (use for game 'Arms' slot) ---
  Gloves_Leather_01
  Gauntlets_Heavy_01
  ...

--- LEG PARTS (use for game 'Legs' slot) ---
  Legs_Pants_01
  Legs_Armor_Heavy_01
  ...
```

### Step 2: Update PartData Assets with Real Names (10 minutes)

**Navigate to:** `Assets/Data/Parts/`

**For each PartData asset:**

1. Click to select it
2. In Inspector, find "Sidekick Part ID"
3. Enter **exact name** from Step 1 output

**Example mappings** (use YOUR actual names from console):

```
Part_Head_Common.asset:
  Sidekick Part ID: "Head_Male_01"    ← Use actual name from YOUR console

Part_Head_Rare.asset:
  Sidekick Part ID: "Helmet_Soldier_01"

Part_Head_Epic.asset:
  Sidekick Part ID: "Helmet_Heavy_01"

Part_Torso_Common.asset:
  Sidekick Part ID: "Torso_Shirt_01"

Part_Torso_Rare.asset:
  Sidekick Part ID: "Torso_Armor_Light_01"

Part_Torso_Epic.asset:
  Sidekick Part ID: "Torso_Armor_Heavy_01"

Part_Arms_Common.asset:
  Sidekick Part ID: "Gloves_Leather_01"

Part_Arms_Rare.asset:
  Sidekick Part ID: "Gauntlets_Light_01"

Part_Arms_Epic.asset:
  Sidekick Part ID: "Gauntlets_Heavy_01"

Part_Legs_Common.asset:
  Sidekick Part ID: "Legs_Pants_01"

Part_Legs_Rare.asset:
  Sidekick Part ID: "Legs_Armor_Light_01"

Part_Legs_Epic.asset:
  Sidekick Part ID: "Legs_Armor_Heavy_01"
```

**CRITICAL:** Part names are case-sensitive! Copy them EXACTLY.

### Step 3: Verify Setup (2 minutes)

**In Unity Hierarchy:**

1. Select **Player** GameObject
2. Verify these components exist:
   - ✓ SidekickCharacterController
   - ✓ SidekickPartSwapper
   - ✓ CharacterPartManager

**In Inspector (Player selected):**

**SidekickCharacterController:**
- ✓ Enable Sidekick: checked
- Character Name: "Player Character"
- Base Model Path: "Meshes/SK_BaseModel"
- Base Material Path: "Materials/M_BaseMaterial"

**SidekickPartSwapper:**
- ✓ Use Sidekick Integration: checked
- Sidekick Controller: [should show reference to component above]

### Step 4: Test It! (1 minute)

**Press Play**

**Check Console for:**
```
[SidekickCharacterController] Initializing Sidekick Runtime API...
[SidekickCharacterController] Initialized successfully. Part library loaded with X categories
[SidekickCharacterController] Character rebuilt with X parts
[SidekickPartSwapper] Sidekick integration enabled and ready!
```

**If you see errors instead, read them carefully** - they'll tell you exactly what's wrong.

**Pick up a part in game:**

Console should show:
```
[SidekickPartSwapper] Swapped Head to 'Head_Male_01' using Sidekick
[SidekickCharacterController] Equipping Head: Head_Male_01
[SidekickCharacterController] Character rebuilt with X parts
```

**Look at Player in Scene view** - you should see the character model change!

---

## Still Not Working?

### Diagnostic Tool

1. Select Player in Hierarchy
2. Add component: `SidekickDebugHelper`
3. Right-click component header → "Log Setup Status"
4. Read console output carefully

### Common Issues

**"Failed to load base model"**
- Check: `Assets/Synty/SidekickCharacters/Resources/Meshes/SK_BaseModel` exists
- If different location, update path in SidekickCharacterController

**"Part 'X' not found in Head library"**
- Part name doesn't match exactly
- Re-run Step 1 to get correct name
- Check spelling and case

**"SidekickCharacterController not initialized"**
- Check console for initialization errors
- Verify Sidekick package imported correctly
- Check Resources folder has Meshes and Materials

**Character created but not visible**
- Check Scene view (not just Game view)
- Look for "Player Character" GameObject under Player in Hierarchy
- Character might be positioned wrong or too small

---

## What You Should See

**Before picking up parts:**
- Basic character body (minimal appearance)

**After picking up Epic Head:**
- Helmet/armor appears on character
- Stats increase

**After collecting all 4 Epic parts:**
- Fully armored character
- Professional appearance
- All stats maxed

---

## Technical Notes

**How it works:**
1. SidekickCharacterController initializes the Sidekick database
2. When you equip a part, it stores the part name
3. It destroys the old character model
4. It rebuilds a new character with all current parts combined
5. Character spawns under Player GameObject

**Performance:**
- Character rebuild takes <0.1 seconds
- Only happens when parts change
- Should be smooth even on mobile

**Part Naming:**
- Part names MUST match exactly (case-sensitive)
- Names come from Sidekick database
- Different Sidekick packages have different part names
- That's why you MUST use Step 1 to discover YOUR part names

---

*Follow these steps in order. Don't skip Step 1 - discovering your actual part names is CRITICAL!*
