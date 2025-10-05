# Isometric Camera Setup (Diablo-Style)

## Current Setup
Your camera is:
- Position: (0, 10, 0) - directly above
- Rotation: (90, 0, 0) - looking straight down
- Parent: Arena_Main (static)

This is why you can't see 3D models well!

---

## Setup Instructions (5 Minutes)

### Step 1: Modify Main Camera

**In Unity Hierarchy:**

1. **Expand Arena_Main**
2. **Select "Main Camera"**
3. **Right-click** on Main Camera → **Unparent** (or drag it out to root level)
   - This disconnects camera from Arena_Main
4. **Add Component** → Search: `IsometricCameraController`

### Step 2: Configure Isometric Settings

**With Main Camera selected, in Inspector:**

**IsometricCameraController settings:**

#### **For Diablo-Style View:**
```
Target: [Drag Player prefab/object here]

Isometric Settings:
├── Angle X: 50           ← Looking down angle
├── Angle Y: 45           ← Diagonal view angle
├── Distance: 12          ← How far back
└── Height Offset: 2      ← Extra height

Follow Settings:
├── Follow Smoothing: 5   ← Smooth camera movement
└── Target Offset: (0, 0, 0)

Optional: Dynamic Zoom:
└── Enable Dynamic Zoom: ☐ (unchecked for now)
```

**That's it!** Press Play and the camera will follow your player at a nice angle.

---

## Camera Preset Angles

Try these presets to find your preferred view:

### **Diablo III Style** (Recommended)
```
Angle X: 50
Angle Y: 45
Distance: 12
Height Offset: 2
```
- Shows character clearly
- Good tactical view
- Can see armor details

### **Hades Style** (More Character Focus)
```
Angle X: 45
Angle Y: 45
Distance: 10
Height Offset: 1.5
```
- Closer to character
- Better for seeing equipped parts
- More dramatic

### **Path of Exile Style** (Wider View)
```
Angle X: 60
Angle Y: 45
Distance: 14
Height Offset: 3
```
- See more of arena
- Better for enemy awareness
- Slightly less character detail

### **Classic Isometric** (Tactical)
```
Angle X: 45
Angle Y: 45
Distance: 15
Height Offset: 0
```
- True isometric
- Maximum battlefield view

---

## Testing Your View

**After setup, Press Play:**

1. **Move your character around**
   - Camera should smoothly follow
   - Keeps player centered

2. **Pick up a part**
   - You should clearly see the 3D armor/helmet appear!
   - Character model visible from angle

3. **Check enemy visibility**
   - Can you see enemies approaching?
   - Adjust Distance if needed

4. **Fine-tune if needed:**
   - **Too close?** Increase Distance (e.g., 14)
   - **Can't see character?** Decrease Angle X (e.g., 40)
   - **Want more dramatic?** Decrease Distance (e.g., 10)

---

## Optional: Enable Mouse Zoom

**If you want players to zoom in/out:**

1. Select Main Camera
2. In IsometricCameraController:
   ```
   Enable Dynamic Zoom: ✓ (checked)
   Min Distance: 8
   Max Distance: 15
   Zoom Speed: 2
   ```

3. **In-game:** Mouse scroll wheel zooms camera

---

## Troubleshooting

### "Camera doesn't follow player"

**Check:**
1. Is "Target" field set to Player? (drag Player from Hierarchy)
2. Is Player tagged as "Player"? (Select Player → Inspector → Tag → Player)

### "Camera is jerky/stuttering"

**Solution:** Increase Follow Smoothing to 8-10 for smoother motion

### "Can't see character, too far away"

**Solution:** Reduce Distance to 10 and Angle X to 45

### "Camera inside arena floor"

**Solution:** Increase Height Offset to 3-5

### "Want to see character parts better"

**Use Hades preset:**
```
Angle X: 40
Distance: 9
Height Offset: 1
```

---

## Advanced: Camera Shake on Hit

Want camera shake when player takes damage? Let me know and I can add it to IsometricCameraController!

---

## What You'll See After Setup

**Before (Top-Down):**
- ❌ Only see top of character's head
- ❌ Can't see equipped armor
- ❌ 3D models not visible

**After (Isometric):**
- ✓ See full character at angle
- ✓ Equipped armor/helmets clearly visible
- ✓ Character transforms as you collect Epic parts
- ✓ Camera smoothly follows player

---

## Quick Setup Checklist

- [ ] Main Camera unparented from Arena_Main
- [ ] IsometricCameraController component added
- [ ] Target set to Player
- [ ] Angle X: 50, Angle Y: 45, Distance: 12
- [ ] Follow Smoothing: 5
- [ ] Press Play and test movement
- [ ] Pick up a part and see 3D armor appear!

---

*Recommended: Start with Diablo preset, then adjust Distance to your preference!*
