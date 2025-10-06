# Animator Controller - Super Detailed Guide

This guide explains **exactly** what an Animator Controller is and how to create one, step by step.

---

## What is an Animator Controller?

Think of it like a **flowchart** that controls when animations play:

```
Player standing still? → Play IDLE animation
Player moving? → Play RUNNING animation
Player attacks? → Play PUNCHING animation
```

The Animator Controller is the "brain" that decides which animation to play.

---

## Part 1: Creating the Animator Controller File

### Step 1: Create the File

**In Unity:**

1. **Bottom of screen** → Find **"Project" window**
2. **Navigate to** `Assets/Animations` folder (click on it)
3. **Right side** is empty space → **Right-click** in the empty area
4. Menu appears → Hover over **"Create"**
5. Submenu appears → Click **"Animator Controller"**
6. A new file appears with blue icon and highlighted name
7. **Type**: `PlayerAnimatorController`
8. **Press Enter**

**You now have a file!** It has a blue play button icon.

---

## Part 2: Opening the Animator Window

### Step 2: Open the Animator Window

1. **Double-click** on `PlayerAnimatorController` file
2. A new window/tab opens at the top of Unity
3. **Tab says "Animator"** (next to Scene, Game tabs)

**What you see in Animator window:**
- Gray grid background
- One orange box labeled "Entry"
- One box labeled "Any State"
- Left side: "Parameters" section (empty)
- Left side: "Layers" section

**This is your animation flowchart workspace!**

---

## Part 3: Adding Animation States (The Boxes)

### Step 3: Create "Idle" State

**A "state" = one animation that can play**

1. **In Project window** → Find your **"X Bot@Idle"** file
2. **Click and HOLD** on it
3. **Drag** it into the **Animator window** (gray grid area)
4. **Release mouse** → A new **orange box** appears!
5. Box is labeled **"Idle"** (or "X Bot@Idle")
6. It's **orange** = this is the DEFAULT (plays first when game starts)

### Step 4: Create "Running" State

1. **Project window** → Find **"X Bot@Running"**
2. **Drag** into Animator window (next to Idle box)
3. **Release** → A new **gray box** appears labeled "Running"

### Step 5: Create "Punching" State

1. **Project window** → Find **"X Bot@Punching"** (or your attack animation)
2. **Drag** into Animator window
3. **Release** → Another **gray box** appears labeled "Punching"

**You should now see:**
- Entry (exists automatically)
- **Idle** (orange box) ← you created
- **Running** (gray box) ← you created
- **Punching** (gray box) ← you created
- Any State (exists automatically)

**You now have 3 animations ready to use!**

---

## Part 4: Creating Parameters (The Variables)

### What are Parameters?

**Parameters = variables that tell the animator when to switch animations**

Example:
- `Speed = 0` → Character not moving → Play Idle
- `Speed = 5` → Character moving fast → Play Running

### Step 6: Create "Speed" Parameter

**In Animator window, left side:**

1. Find **"Parameters"** section (might have a "+" button)
2. **Click the "+" button**
3. Menu appears with options:
   - Float
   - Int
   - Bool
   - Trigger
4. **Click "Float"** (a number with decimals)
5. A new parameter appears: "New Float"
6. **Name is highlighted** → Type: `Speed`
7. **Press Enter**

**You now have a Speed parameter!** Shows: `Speed = 0`

### Step 7: Create "Attack" Parameter

1. **Click "+" button again**
2. **Click "Trigger"** (a one-time event)
3. New parameter: "New Trigger"
4. **Type**: `Attack`
5. **Press Enter**

**You now have 2 parameters:**
- Speed (Float) = 0
- Attack (Trigger)

---

## Part 5: Creating Transitions (The Arrows)

### What are Transitions?

**Transitions = arrows that connect states and say "switch from this animation to that animation when X happens"**

Example: Arrow from Idle → Running means "switch from idle to running animation"

---

### Transition 1: Idle → Running (When player starts moving)

**Step 8: Create the Arrow**

1. **In Animator window** → Find the **"Idle"** box
2. **Right-click** on the Idle box
3. Menu appears → Click **"Make Transition"**
4. **Your mouse cursor changes** to a crosshair with an arrow!
5. **Click** on the **"Running"** box
6. **A white arrow** appears from Idle to Running!

**Step 9: Set When This Happens**

The arrow is selected (highlighted). Now tell it WHEN to switch:

1. **Right side** → **Inspector window** appears
2. Shows properties of the selected arrow
3. Find **"Conditions"** section (might be collapsed, click to open)
4. Shows: "List is Empty"
5. **Click the "+" button** in Conditions section
6. A new condition appears with dropdowns:
   ```
   [Speed] [Greater] [0]
   ```
7. **Click the number `0`** → Change to `0.1`
8. Now it says: `Speed Greater 0.1`

**Meaning**: "Switch from Idle to Running when Speed is greater than 0.1"

**Step 10: Make Transition Instant**

Still in Inspector for this arrow:

1. Find **"Settings"** section
2. Find **"Has Exit Time"** checkbox → **UNCHECK IT** ☐
   - (Exit time = waiting for animation to finish, we don't want that)
3. Find **"Transition Duration"** → Change to `0.1`
   - (How long the blend takes, 0.1 seconds = fast)

**Done! First transition complete!**

---

### Transition 2: Running → Idle (When player stops moving)

**Step 11: Create Arrow Back**

1. **Right-click** the **"Running"** box
2. **Make Transition**
3. **Click** the **"Idle"** box
4. **Arrow appears** from Running back to Idle!

**Step 12: Set Condition**

1. **Arrow is selected** → Inspector shows
2. **Conditions** → Click **"+"**
3. Set: `[Speed] [Less] [0.1]`

**Meaning**: "Switch from Running to Idle when Speed drops below 0.1"

**Step 13: Settings**

1. **Has Exit Time**: ☐ (unchecked)
2. **Transition Duration**: `0.2`

**Done!**

---

### Transition 3: Idle → Punching (When player attacks)

**Step 14: Create Arrow**

1. **Right-click "Idle"** box
2. **Make Transition**
3. **Click "Punching"** box
4. Arrow appears

**Step 15: Set Condition**

1. **Conditions** → Click **"+"**
2. Dropdown shows: `[Attack]` (it's a trigger, no comparison needed!)
3. Just shows: `Attack`

**Meaning**: "When Attack trigger fires, switch to Punching"

**Step 16: Settings**

1. **Has Exit Time**: ☐ (unchecked)
2. **Transition Duration**: `0.1`

---

### Transition 4: Running → Punching (Attack while running)

**Step 17: Same Process**

1. **Right-click "Running"**
2. **Make Transition** → **"Punching"**
3. **Condition**: `Attack`
4. **Has Exit Time**: ☐
5. **Transition Duration**: `0.1`

---

### Transition 5: Punching → Idle (Return to idle after attacking)

**This one is DIFFERENT!** We want attack to finish, THEN return to idle.

**Step 18: Create Arrow**

1. **Right-click "Punching"**
2. **Make Transition** → **"Idle"**

**Step 19: Special Settings**

1. **Conditions**: **LEAVE EMPTY!** (don't click +)
   - No conditions = always happens
2. **Has Exit Time**: ✓ **CHECK THIS!**
   - This makes it wait for animation to finish
3. **Exit Time**: `0.9` (90% through animation)
4. **Transition Duration**: `0.1`

**Meaning**: "After punch animation plays 90%, smoothly blend back to Idle"

---

## What You Should Have

**In Animator window:**

**Boxes (States):**
- Entry (orange, built-in)
- Idle (orange)
- Running (gray)
- Punching (gray)
- Any State (gray, built-in)

**Arrows (Transitions):**
1. Entry → Idle (automatic)
2. Idle → Running (white arrow)
3. Running → Idle (white arrow back)
4. Idle → Punching (white arrow)
5. Running → Punching (white arrow)
6. Punching → Idle (white arrow back)

**Parameters (Left side):**
- Speed (Float)
- Attack (Trigger)

**Save:** File → Save (Ctrl+S)

---

## Visual Diagram

```
        Entry
          ↓
       [IDLE] ⟷ [RUNNING]
         ↓ ↑      ↓ ↑
         ↓ ↑      ↓ ↑
         ↓ └─────→↓
         ↓         ↓
       [PUNCHING]──┘
```

**Flow:**
- Start → Idle
- Speed > 0.1 → Running
- Speed < 0.1 → Idle
- Attack pressed → Punching
- Punching finishes → Idle

---

## Testing the Animator (Optional)

**Before using in game, you can test it:**

1. **Animator window** still open
2. **Press Play** in Unity
3. **Click back to Animator tab**
4. You'll see the flowchart **highlighting** which state is active
5. **Left side Parameters** → You can manually change Speed value
   - Set Speed to 1 → Character runs!
   - Set Speed to 0 → Character idles!

**Stop Play mode when done testing.**

---

## Common Questions

**Q: Why is Idle orange?**
A: Orange = default state. Game starts with this animation.

**Q: What are those other boxes (Entry, Any State)?**
A: Built-in Unity helpers. Entry points to default state. Any State is for global transitions (ignore for now).

**Q: Can I rename the boxes?**
A: Click box → Inspector → Change name. But using animation names is fine!

**Q: Arrow is red?**
A: Red = error in condition. Check you typed parameter names correctly.

**Q: Too many arrows, confusing?**
A: That's normal! It's a web. Focus on one arrow at a time.

---

## Next Step

Once you have this setup:
- **File → Save**
- Follow **Complete_Animation_Setup.md Part 3** to connect it to your player

**Your animator is now ready to control animations!**

---

*This is like creating a flowchart - boxes are animations, arrows are "when to switch"*
