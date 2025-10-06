# Complete Animation Setup Guide

You have the animations downloaded! Now let's get them working in Unity.

---

## Part 1: Import Animations (5 minutes)

### Step 1: Create Animations Folder

**In Unity Project window:**
1. Right-click `Assets` folder
2. `Create > Folder`
3. Name: `Animations`

### Step 2: Import FBX Files

1. **Open File Explorer** → `C:\Users\User\Downloads\`
2. **Find your 3 files**:
   - X Bot@Idle.fbx
   - X Bot@Running.fbx (or similar name)
   - X Bot@Punching.fbx (or attack animation)
3. **Drag all 3** into Unity's `Assets/Animations` folder
4. **Wait for import** (Unity processes them)

### Step 3: Configure EACH Animation

**For X Bot@Idle.fbx:**
1. **Select it** in Project window
2. **Inspector** → **Rig tab**:
   ```
   Animation Type: Humanoid
   Avatar Definition: Create From This Model
   ```
3. **Click Apply**
4. **Animation tab**:
   ```
   ✓ Import Animation
   ✓ Loop Time (checked)
   ```
5. **Click Apply**

**Repeat for X Bot@Running.fbx:**
- Same settings as Idle
- ✓ Loop Time (checked)

**For X Bot@Punching.fbx (or attack):**
- Same Rig settings
- ☐ Loop Time (UNchecked) ← Attack plays once
- Click Apply

---

## Part 2: Create Animator Controller (5 minutes)

### Step 1: Create Controller

1. **Project** → `Assets/Animations` folder
2. **Right-click** → `Create > Animator Controller`
3. **Name**: `PlayerAnimatorController`

### Step 2: Open Animator Window

1. **Double-click** `PlayerAnimatorController`
2. **Animator window** opens (tabs at top)

### Step 3: Add Animation States

**Drag animations into Animator window:**

1. **Find** `X Bot@Idle` in Project
2. **Drag** into Animator window
   - Creates orange "Idle" state (default state)

3. **Find** `X Bot@Running`
4. **Drag** into Animator window
   - Creates "Running" state

5. **Find** `X Bot@Punching`
6. **Drag** into Animator window
   - Creates "Punching" state

**You should see 3 boxes in the Animator window!**

### Step 4: Create Parameters

**In Animator window, find "Parameters" section (usually top-left):**

1. **Click "+" button**
2. Select **"Float"**
3. Name: `Speed`

4. **Click "+" again**
5. Select **"Trigger"**
6. Name: `Attack`

### Step 5: Create Transitions (The Arrows)

**This tells Unity when to switch between animations.**

#### Idle → Running:
1. **Right-click** the "Idle" box
2. **Make Transition**
3. **Click** the "Running" box (arrow appears)
4. **Click the arrow** to select it
5. **Inspector** → **Conditions** section:
   - Click **"+"**
   - Set: `Speed` | `Greater` | `0.1`
6. **Settings** section:
   - **Has Exit Time**: ☐ (UNcheck)
   - **Transition Duration**: `0.1`

#### Running → Idle:
1. **Right-click** "Running" box
2. **Make Transition** → Click "Idle"
3. **Select arrow** → Inspector:
   - Conditions: `Speed` | `Less` | `0.1`
   - Has Exit Time: ☐ (UNcheck)
   - Transition Duration: `0.2`

#### Idle → Punching:
1. **Right-click** "Idle"
2. **Make Transition** → "Punching"
3. **Arrow** → Inspector:
   - Conditions: `Attack` (just select it, no value)
   - Has Exit Time: ☐
   - Transition Duration: `0.1`

#### Running → Punching:
1. **Right-click** "Running"
2. **Make Transition** → "Punching"
3. **Arrow** → Inspector:
   - Conditions: `Attack`
   - Has Exit Time: ☐
   - Transition Duration: `0.1`

#### Punching → Idle (Auto-return):
1. **Right-click** "Punching"
2. **Make Transition** → "Idle"
3. **Arrow** → Inspector:
   - Conditions: **(leave empty!)**
   - Has Exit Time: ✓ **(CHECK this!)**
   - Exit Time: `0.9`
   - Transition Duration: `0.1`

**Save:** File → Save (Ctrl+S)

---

## Part 3: Unity Setup (3 minutes)

### Step 1: Assign Animator Controller to Player

**Select Player GameObject in Hierarchy:**

1. **Inspector** → Find **SidekickCharacterController** component
2. **Animation** section:
   - **Animator Controller**: Drag `PlayerAnimatorController` here

### Step 2: Add Animation Controller Script

**With Player selected:**

1. **Add Component** → Search: `PlayerAnimationController`
2. Inspector auto-fills references
3. Done!

---

## Part 4: Test! (1 minute)

**Press Play:**

**You should see:**
```
[SidekickCharacterController] Animator setup with controller: PlayerAnimatorController
[PlayerAnimationController] Found Animator on Player Character
```

**Move your player around:**
- Standing still → Idle animation plays
- Moving → Running animation plays!
- Smooth transitions!

**Check in Scene view to see animations clearly!**

---

## Troubleshooting

### "Animations not playing"

**Check Console for:**
- `[SidekickCharacterController] No Animator Controller assigned`
  - **Fix**: Assign PlayerAnimatorController in Inspector

- `[PlayerAnimationController] No Animator found`
  - **Fix**: Make sure Sidekick character is created

### "Character sliding/not moving correctly"

**This is normal!** The animation makes the character look like running, but movement comes from PlayerController (Rigidbody velocity).

### "Attack animation doesn't play"

**Not implemented yet** - attack will be added when you integrate with combat system. For now, running animations are enough!

---

## What You Should See

**Before:**
- ❌ Character static T-pose or frozen

**After:**
- ✓ Character idles when standing
- ✓ Runs when moving
- ✓ Smooth transitions
- ✓ Looks alive!

---

## Next Steps

Once animations work:
- ✓ You have a fully animated character
- ✓ Isometric camera shows it clearly
- ✓ Parts equip and change appearance

**Your Phase 4 Sidekick integration is COMPLETE!**

Ready to move to Phase 5:
- VFX (hit effects, part pickups)
- Audio (sound effects, music)
- Polish

---

*Follow the steps in order - should take about 15 minutes total!*
