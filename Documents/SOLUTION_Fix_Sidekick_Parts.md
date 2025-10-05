# SOLUTION: Fix Sidekick Part Equipping

## Current Situation

**What's Working:**
- ✓ SidekickCharacterController initializes successfully (38 categories loaded)
- ✓ Creates "Player Character" with base mesh
- ✓ Part pickup and stats work

**What's Broken:**
- ❌ Part names in PartData don't match your actual Sidekick package
- ❌ You have duplicate characters (BaseCharacter + Player Character)
- ❌ Dummy visuals show instead of Sidekick parts

## Why It's Failing

Your PartData assets have example part names like:
```
SK_FANT_KNGT_17_16HNDR_HU01  ← This is from Fantasy Knight package
SK_FANT_KNGT_09_10TORS_HU01  ← These don't match YOUR package
```

But YOUR Sidekick package has 38 categories with different part names. When the system tries to equip "SK_FANT_KNGT_17_16HNDR_HU01", it can't find it, so it falls back to dummy visuals.

---

## STEP-BY-STEP FIX

### Step 1: Discover Your Actual Part Names (5 minutes)

You already have SidekickPartLister.cs. Now use it:

**In Unity:**

1. Create empty GameObject: `GameObject > Create Empty`
2. Name it "PartLister"
3. Add component: `SidekickPartLister`
4. In Inspector, check both:
   - ✓ List On Start
   - ✓ Filter By Useful Parts
5. **Press Play**
6. **Copy ALL the output from Console**

**Example of what you'll see:**
```
=== LISTING ALL SIDEKICK PARTS ===
Found 38 part categories

=== PARTS USEFUL FOR YOUR GAME ===

--- HEAD PARTS (use for game 'Head' slot) ---
  Head_Male_01
  Head_Male_02
  Helmet_Warrior_01
  Helmet_Knight_Heavy
  ...

--- TORSO PARTS (use for game 'Torso' slot) ---
  Torso_Naked
  Torso_Shirt_Basic
  Torso_Armor_Light_01
  ...

--- ARM PARTS (use for game 'Arms' slot) ---
NOTE: You'll use same name for Upper/Lower Left/Right arms
  Arm_Bare
  Gloves_Leather_01
  Gauntlets_Heavy_01
  ...

--- LEG PARTS (use for game 'Legs' slot) ---
NOTE: You'll use same name for both left and right legs
  Legs_Naked
  Legs_Pants_Basic
  Legs_Armor_Heavy_01
  ...

--- HIPS PARTS (also useful for 'Legs' slot) ---
  Hips_Underwear
  Hips_Belt_Basic
  ...
```

**COPY ALL THESE NAMES** - you'll need them for Step 2.

---

### Step 2: Update PartData Assets (10 minutes)

**Navigate to:** `Assets/Data/Parts/`

You'll see 12 PartData assets. For EACH ONE:

1. **Click to select the asset**
2. **In Inspector, find "Sidekick Part ID"**
3. **Replace with EXACT name from Step 1**

**Example mapping** (use YOUR names from console!):

```
Part_Head_Common.asset:
  Sidekick Part ID: "Head_Male_01"    ← Use basic head from YOUR list

Part_Head_Rare.asset:
  Sidekick Part ID: "Helmet_Warrior_01"    ← Use better helmet from YOUR list

Part_Head_Epic.asset:
  Sidekick Part ID: "Helmet_Knight_Heavy"    ← Use best helmet from YOUR list

Part_Torso_Common.asset:
  Sidekick Part ID: "Torso_Shirt_Basic"

Part_Torso_Rare.asset:
  Sidekick Part ID: "Torso_Armor_Light_01"

Part_Torso_Epic.asset:
  Sidekick Part ID: "Torso_Armor_Heavy_01"

Part_Arms_Common.asset:
  Sidekick Part ID: "Arm_Bare"

Part_Arms_Rare.asset:
  Sidekick Part ID: "Gauntlets_Light_01"

Part_Arms_Epic.asset:
  Sidekick Part ID: "Gauntlets_Heavy_01"

Part_Legs_Common.asset:
  Sidekick Part ID: "Legs_Pants_Basic"

Part_Legs_Rare.asset:
  Sidekick Part ID: "Legs_Armor_Light_01"

Part_Legs_Epic.asset:
  Sidekick Part ID: "Legs_Armor_Heavy_01"
```

**CRITICAL:**
- Part names are **case-sensitive**
- Must match **EXACTLY** (copy-paste from console)
- No extra spaces or typos

---

### Step 3: Remove Duplicate Characters (2 minutes)

You have two characters showing up. Remove the manual one:

**In Unity Hierarchy, select Player:**

1. Expand Player GameObject
2. Find **"BaseCharacter"** child object
3. **Delete it** (this is your old manual character)
4. Keep **"PartSlots"** (needed for fallback)
5. Keep **"Player Character"** (auto-created by Sidekick)

**After deletion, you should have:**
```
Player
├── PartSlots (fallback only)
│   ├── HeadSlot
│   ├── ArmsSlot
│   ├── TorsoSlot
│   └── LegsSlot
└── Player Character (Sidekick - created at runtime)
    └── SK_HUMN_BASE_01_17HIPS_HU01
```

---

### Step 4: Test It! (1 minute)

**Press Play**

**Check Console - you should see:**
```
[SidekickCharacterController] Initializing Sidekick Runtime API...
[SidekickCharacterController] Initialized successfully. Part library loaded with 38 categories
[SidekickCharacterController] Character rebuilt with 1 parts
[SidekickPartSwapper] Sidekick integration enabled and ready!
```

**Pick up a Common Leg part in game**

**Console should show:**
```
[SidekickPartSwapper] Swapped Legs to 'Legs_Pants_Basic' using Sidekick
[SidekickCharacterController] Equipping Legs: Legs_Pants_Basic
[SidekickCharacterController] Character rebuilt with 3 parts    ← Parts increased!
[CharacterPartManager] Equipped Common Legs using Sidekick: Legs_Pants_Basic
```

**Look at Player Character in Scene view:**
- Should see leg armor/pants appear on character
- Character mesh should update visually
- NO dummy cubes should appear in PartSlots

---

## Verification Checklist

After following all steps:

- [ ] Ran SidekickPartLister and copied part names
- [ ] Updated all 12 PartData assets with real part names
- [ ] Deleted "BaseCharacter" GameObject from Player
- [ ] Pressed Play - no errors in console
- [ ] Picked up part - saw "[SidekickPartSwapper] Swapped ... using Sidekick"
- [ ] Saw "Character rebuilt with X parts" (X should increase)
- [ ] Character visual changed in Scene view
- [ ] NO dummy meshes appeared in PartSlots

---

## If Still Not Working

**Check for these errors in Console:**

**Error:** `[SidekickCharacterController] Part 'X' not found in Y library`
- **Cause:** Part name doesn't match exactly
- **Fix:** Check spelling, case, and spaces. Re-run PartLister.

**Error:** `[SidekickPartSwapper] Sidekick swap failed for X, falling back to dummy`
- **Cause:** Either Sidekick not available OR part name wrong
- **Fix:** Check both Step 1 (part names) and that controller initialized

**No Sidekick messages at all:**
- **Cause:** Sidekick not initializing
- **Fix:** Check for errors during initialization. Verify Resources folder has base model.

**Character rebuilds but looks wrong:**
- **Cause:** Part names might be correct but wrong type (e.g., using a torso part for head slot)
- **Fix:** Make sure you're using HEAD parts for head slot, TORSO parts for torso slot, etc.

---

## Understanding the System

**How Sidekick integration works:**

1. **Initialization (Start):**
   - Loads Sidekick database (38 categories)
   - Creates dictionary of all available parts
   - Builds base character with underwear/hips

2. **When you pick up a part:**
   - CharacterPartManager gets PartData
   - Reads `sidekickPartID` field (e.g., "Helmet_Knight_Heavy")
   - Calls SidekickCharacterController.EquipPart()
   - Stores the part name for that slot

3. **Character rebuild:**
   - **Destroys old character completely**
   - **Builds NEW character from scratch** with:
     - Base body parts (hips, underwear)
     - Head part (if equipped)
     - Torso part (if equipped)
     - Arm parts (upper/lower left/right)
     - Leg parts (left/right)
   - Spawns under Player GameObject
   - Takes <0.1 seconds

4. **Sidekick part mapping:**
   - Your game has 4 slots: Head, Torso, Arms, Legs
   - Sidekick has granular parts: ArmUpperLeft, ArmUpperRight, ArmLowerLeft, etc.
   - System automatically maps:
     - Arms → ArmUpperLeft + ArmUpperRight + ArmLowerLeft + ArmLowerRight + HandLeft + HandRight
     - Legs → LegLeft + LegRight + FootLeft + FootRight
     - Same part name used for all related slots

**Why names must match exactly:**
- Sidekick stores parts in dictionaries by name
- If name doesn't match: `dictionary["Wrong_Name"]` = not found
- If not found: warning logged, part skipped, dummy used instead

---

## After It Works

**Once Sidekick is working:**

1. **Adjust part assignments:**
   - Pick better-looking parts for Epic tier
   - Choose visually distinct parts for Common/Rare/Epic

2. **Visual tuning:**
   - Adjust character scale if needed
   - Position "Player Character" if it's offset
   - Test all part combinations

3. **Clean up:**
   - Remove SidekickPartLister GameObject
   - Remove old BaseCharacter references
   - Test performance (should be smooth)

4. **Move forward:**
   - Your Phase 4 is complete!
   - Ready for Phase 5 (VFX, Audio, Polish)

---

*Follow Step 1 first - discovering your actual part names is CRITICAL!*
