# Animator Controller Setup Guide

## Creating the Animator Controller

**In Unity:**

1. **Project window** → `Assets/Animations` folder
2. **Right-click** → `Create > Animator Controller`
3. **Name it**: `PlayerAnimatorController`
4. **Double-click** to open Animator window

---

## Setting Up Animation States

**In the Animator window:**

### Step 1: Add Animations

1. **Drag `X Bot@Idle`** from Project into Animator window
   - This creates "Idle" state (orange = default state)

2. **Drag `X Bot@Running`** into Animator window
   - Creates "Running" state

3. **Drag `X Bot@Punching`** into Animator window
   - Creates "Punching" state

### Step 2: Create Parameters

**In Animator window, find "Parameters" tab (top-left):**

1. **Click "+" button**
2. **Select "Float"**
3. **Name it**: `Speed`

4. **Click "+" again**
5. **Select "Trigger"**
6. **Name it**: `Attack`

### Step 3: Create Transitions

**Make state transitions:**

#### Idle → Running:
1. **Right-click "Idle" state** → `Make Transition`
2. **Click "Running" state**
3. **Select the arrow/transition**
4. **Inspector** → Conditions:
   - Click **"+"**
   - Set: `Speed` `Greater` `0.1`
5. **Settings:**
   - Has Exit Time: ☐ (unchecked)
   - Transition Duration: 0.1

#### Running → Idle:
1. **Right-click "Running" state** → `Make Transition`
2. **Click "Idle" state**
3. **Inspector** → Conditions:
   - `Speed` `Less` `0.1`
4. **Settings:**
   - Has Exit Time: ☐ (unchecked)
   - Transition Duration: 0.2

#### Idle → Punching:
1. **Right-click "Idle"** → `Make Transition`
2. **Click "Punching"**
3. **Conditions:**
   - `Attack` (trigger)
4. **Settings:**
   - Has Exit Time: ☐
   - Transition Duration: 0.1

#### Running → Punching:
1. **Right-click "Running"** → `Make Transition`
2. **Click "Punching"**
3. **Conditions:**
   - `Attack` (trigger)
4. **Settings:**
   - Has Exit Time: ☐
   - Transition Duration: 0.1

#### Punching → Idle:
1. **Right-click "Punching"** → `Make Transition`
2. **Click "Idle"**
3. **Conditions:**
   - (None - leave empty)
4. **Settings:**
   - Has Exit Time: ✓ (checked!)
   - Exit Time: 0.9
   - Transition Duration: 0.1

---

## Summary

**You should have:**
- 3 states: Idle (orange/default), Running, Punching
- 2 parameters: Speed (Float), Attack (Trigger)
- 5 transitions (arrows between states)

**Save** (File → Save or Ctrl+S)
