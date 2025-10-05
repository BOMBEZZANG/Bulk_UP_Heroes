# Bulk Up Heroes - Phase 4 Development Specification
## Synty Character Integration (ID-Based System)

### Phase Overview

**Phase 4 Goal**: Replace dummy visual system with Synty Sidekick Modular Characters using an ID-based runtime swapping approach, creating professional-quality character visuals while maintaining all Phase 3 gameplay mechanics.

**Duration**: 3-4 Days  
**Prerequisites**: Phase 3 complete with fully functional part system  
**Approach**: ID-based part management (no individual part prefabs)

---

## 1. Technical Architecture

### 1.1 ID-Based System Overview

```
Instead of storing prefabs for each part combination:
- Store part IDs as strings in PartData
- Use Sidekick runtime API to swap parts
- Let Sidekick handle mesh management
- Minimize memory footprint
```

### 1.2 System Flow

```
1. Player starts with BaseCharacter prefab (near-naked)
2. Part pickup triggers ID lookup
3. CharacterPartManager calls Sidekick API with ID
4. Sidekick swaps the mesh at runtime
5. Visual change happens instantly
```

---

## 2. Synty Sidekick Setup

### 2.1 Initial Configuration

**Required Sidekick Settings:**
```
Window > Sidekick > Settings

Runtime Configuration:
├── Allow Runtime Changes: ✓
├── Use Object Pooling: ✓
├── Auto Combine Meshes: ✗ (disable for runtime swapping)
├── Mobile Optimization: ✓
└── Max Bone Weights: 2 (for mobile)
```

### 2.2 Part ID Mapping

**Create Part ID Reference Document:**
```
Location: Assets/Data/Parts/PartIDMapping.txt

Format:
[PartType]_[Rarity] = [Sidekick_Part_ID]

Example Mapping:
Head_Common = MSC_Head_Basic_01
Head_Rare = MSC_Head_Armor_Light_01  
Head_Epic = MSC_Head_Armor_Heavy_01

Arms_Common = MSC_Arms_Basic_01
Arms_Rare = MSC_Arms_Armor_Light_01
Arms_Epic = MSC_Arms_Armor_Heavy_01

Torso_Common = MSC_Torso_Basic_01
Torso_Rare = MSC_Torso_Armor_Light_01
Torso_Epic = MSC_Torso_Armor_Heavy_01

Legs_Common = MSC_Legs_Basic_01
Legs_Rare = MSC_Legs_Armor_Light_01
Legs_Epic = MSC_Legs_Armor_Heavy_01
```

**Note:** MSC = Modular Sidekick Character (prefix may vary based on your version)

---

## 3. Base Character Creation

### 3.1 Create Base Character Prefab

**Using Sidekick Character Creator:**
```
1. Open: Window > Sidekick > Character Creator
2. Click "New Character"
3. Configure Base State:
   ├── Name: "BaseCharacter"
   ├── Type: Humanoid
   ├── Gender: [Your choice]
   └── Parts:
       ├── Head: None or Basic_Hair_01
       ├── Torso: Underwear_Basic
       ├── Arms: None (bare arms)
       ├── Hands: None (bare hands)
       ├── Legs: Underwear_Basic
       └── Feet: None or Basic_Shoes

4. Colors (Skin/Underwear):
   ├── Skin Color: Default
   ├── Primary Color: Gray (#808080)
   └── Secondary Color: Dark Gray (#404040)

5. Export Settings:
   ├── Prefab Name: "BaseCharacter"
   ├── Location: Assets/Prefabs/Player/
   └── Include Components: ✓
```

### 3.2 Configure Base Character Components

**Required Components on BaseCharacter:**
```
BaseCharacter (Prefab)
├── Transform (0, 0, 0)
├── SidekickCharacter (Script)
│   ├── Character ID: "base_character"
│   ├── Enable Runtime Swap: ✓
│   ├── Part Categories: All enabled
│   └── Optimization Mode: Mobile
├── Animator
│   ├── Avatar: Sidekick Humanoid
│   ├── Apply Root Motion: ✗
│   └── Update Mode: Normal
├── Capsule Collider
│   ├── Center: (0, 0.9, 0)
│   ├── Radius: 0.3
│   └── Height: 1.8
└── Rigidbody
    ├── Mass: 1
    ├── Drag: 0
    └── Constraints: Freeze Rotation XYZ
```

---

## 4. Update Existing Systems

### 4.1 Modify PartData ScriptableObject

**Update PartData.cs:**
```csharp
// Add new fields
[Header("Synty Integration")]
public string sidekickPartID;  // The Sidekick part ID
public string dropMeshID;       // ID for drop visual (optional)
public Material rarityMaterial; // Material override for rarity

// Remove old fields
// public GameObject dummyPrefab; // DELETE THIS
// public GameObject dropPrefab;   // DELETE THIS
```

### 4.2 Update All 12 PartData Assets

**Example: Part_Head_Common.asset**
```
Inspector Updates:
├── Sidekick Part ID: "MSC_Head_Basic_01"
├── Drop Mesh ID: "MSC_Head_Basic_01_Drop"
└── Rarity Material: Mat_Common_Glow

Repeat for all 12 part assets with appropriate IDs
```

### 4.3 Create Rarity Materials

**Create 3 materials in Assets/Materials/Parts/:**

**Mat_Common_Glow:**
```
Shader: URP/Lit
Base Color: Gray (#808080)
Metallic: 0.2
Smoothness: 0.3
Emission: None
```

**Mat_Rare_Glow:**
```
Shader: URP/Lit
Base Color: Blue (#4A90E2)
Metallic: 0.5
Smoothness: 0.6
Emission: Blue (low intensity)
```

**Mat_Epic_Glow:**
```
Shader: URP/Lit
Base Color: Purple (#9B59B6)
Metallic: 0.7
Smoothness: 0.8
Emission: Purple (medium intensity)
```

---

## 5. CharacterPartManager Modifications

### 5.1 Script Updates Required

**CharacterPartManager.cs changes:**
```
Key modifications:
1. Add reference to SidekickCharacter component
2. Replace GameObject instantiation with Sidekick API calls
3. Update EquipPart() to use part IDs
4. Modify visual feedback for Synty models
```

### 5.2 Integration Points

```csharp
// Old approach (Phase 3):
GameObject partObject = Instantiate(dummyPrefab, slot);

// New approach (Phase 4):
sidekickCharacter.SwapPart(partType.ToString(), partData.sidekickPartID);
```

### 5.3 Part Removal Handling

```csharp
// For returning to "naked" state:
sidekickCharacter.RemovePart(partType.ToString());
// OR
sidekickCharacter.SwapPart(partType.ToString(), "None");
```

---

## 6. Drop System Visual Updates

### 6.1 Create Universal Drop Prefab

Since we're using ID-based system, create ONE drop prefab that adapts:

**PartDrop_Universal.prefab:**
```
GameObject Structure:
├── Root (Empty GameObject)
│   ├── Transform: Scale (0.5, 0.5, 0.5)
│   ├── PartPickup Script
│   ├── Sphere Collider (Trigger, Radius: 1.0)
│   └── Rigidbody (Kinematic)
├── VisualContainer (Child)
│   ├── MeshFilter (runtime populated)
│   ├── MeshRenderer (runtime material)
│   └── Rotation Script (45°/sec)
└── Effects (Child)
    ├── Particle System (rarity-based)
    └── Light (rarity-based color)
```

### 6.2 Runtime Drop Visual Loading

**PartDropManager modifications:**
```
When spawning a drop:
1. Get Universal Drop Prefab
2. Load part mesh using sidekickPartID
3. Apply rarity material
4. Configure particle color
5. Set light intensity based on rarity
```

---

## 7. Player Prefab Integration

### 7.1 Update Player Prefab

**Steps to modify existing Player prefab:**
```
1. Open: Assets/Prefabs/Player/Player.prefab
2. Delete: Current capsule/dummy visual
3. Add: BaseCharacter as child
4. Update component references:
   ├── CharacterPartManager:
   │   └── Sidekick Character: [BaseCharacter's SidekickCharacter component]
   ├── Animator: [Use BaseCharacter's Animator]
   └── Model Transform: [BaseCharacter transform]
```

### 7.2 Component Hierarchy

```
Player (Prefab Root)
├── BaseCharacter (Synty Character)
│   ├── SidekickCharacter
│   ├── Animator
│   └── Character Mesh (managed by Sidekick)
├── Components (Empty for organization)
│   ├── PlayerController
│   ├── PlayerStats
│   ├── CharacterPartManager
│   ├── CombatManager
│   └── DamageHandler
└── Effects (Empty for VFX)
```

---

## 8. Animation Integration

### 8.1 Animator Setup

**Configure Humanoid animations:**
```
BaseCharacter Animator:
├── Avatar: Sidekick Humanoid Avatar
├── Controller: PlayerAnimatorController
└── Parameters:
    ├── Speed (float): Movement speed
    ├── IsAttacking (bool): Combat state
    ├── IsDead (bool): Death state
    └── AttackTrigger (trigger): Attack animation
```

### 8.2 Animation States

```
Required animations (use Synty's or Unity's):
├── Idle
├── Walk
├── Run  
├── Attack (melee swing)
└── Death
```

---

## 9. UI Updates for Visual Feedback

### 9.1 Part Preview System (Optional Enhancement)

**Show small icon of equipped part:**
```
Part Inventory UI Enhancement:
├── Head: [Icon] Epic Head
├── Arms: [Icon] Rare Arms
├── Torso: [Empty Icon] Empty
└── Legs: [Icon] Common Legs
```

### 9.2 Character Portrait (Optional)

```
Add character preview in corner:
- Small render texture
- Shows current character state
- Updates when parts change
```

---

## 10. Performance Optimization

### 10.1 Mesh Combining Strategy

```
Timing:
- Don't combine during combat
- Combine after 2 seconds of no changes
- Uncombine before part swap
- Recombine after swap complete
```

### 10.2 Mobile-Specific Settings

```
Quality Settings for Mobile:
├── Texture Resolution: 512x512 max
├── Bone Weights: 2 bones per vertex
├── Blend Shapes: Disabled
├── LOD Bias: 1.5
└── Shadow Distance: 20 units
```

### 10.3 Object Pooling

```
Pool Management:
├── Part Drop Pool: 20 objects
├── Effect Pool: 10 objects
├── Damage Number Pool: 20 objects
└── Clear pools between waves
```

---

## 11. Testing Plan

### 11.1 Visual Verification

```
Test Case 1: Base Character
1. Start game
2. Verify player appears nearly naked
3. Check performance (60 FPS target)
4. Verify animations work

Test Case 2: First Part Pickup
1. Pick up Common Head
2. Verify helmet appears instantly
3. Check no frame drops during swap
4. Confirm stats still update correctly

Test Case 3: Full Set
1. Collect all 4 Epic parts
2. Verify character looks fully armored
3. Check draw calls (target: <50)
4. Test all animations with full gear
```

### 11.2 Regression Testing

```
Ensure Phase 3 functionality remains:
□ Parts drop from enemies
□ Stats increase with parts
□ Rarity system works
□ UI updates correctly
□ Wave progression unchanged
□ Combat system unaffected
```

### 11.3 Performance Benchmarks

| Metric | Target | Maximum |
|--------|--------|---------|
| FPS (Base) | 60 | Never below 30 |
| FPS (Full Gear) | 60 | Never below 30 |
| Draw Calls | <30 | 50 |
| Tris Count | <50k | 75k |
| Part Swap Time | <0.1s | 0.2s |
| Memory Usage | <300MB | 400MB |

---

## 12. Common Issues & Solutions

### Issue: "Part ID not found"
```
Solution:
1. Verify sidekickPartID matches exactly
2. Check Sidekick part list in Character Creator
3. Ensure part is included in build
```

### Issue: Character appears black/pink
```
Solution:
1. Check materials assigned
2. Verify shaders compatible with render pipeline
3. Rebuild material references
```

### Issue: Parts not visible after pickup
```
Solution:
1. Verify SidekickCharacter component active
2. Check Enable Runtime Swap is true
3. Confirm part categories enabled
```

### Issue: Performance drops during swap
```
Solution:
1. Disable mesh combining during gameplay
2. Use simpler shaders for mobile
3. Reduce texture sizes
```

---

## 13. Deliverables

### 13.1 Updated Prefabs
- [x] BaseCharacter.prefab (nearly naked character)
- [x] Player.prefab (with integrated BaseCharacter)
- [x] PartDrop_Universal.prefab (adaptive drop visual)

### 13.2 Updated Assets
- [x] 12 PartData assets with sidekickPartIDs
- [x] 3 Rarity materials
- [x] PartIDMapping.txt reference

### 13.3 Modified Scripts
- [x] CharacterPartManager.cs (Sidekick integration)
- [x] PartDropManager.cs (universal drop system)
- [x] PartPickup.cs (visual loading)

### 13.4 Documentation
- [x] Part ID mapping table
- [x] Performance optimization guide
- [x] Troubleshooting guide

---

## 14. Phase 4 Completion Criteria

**Visual Quality:**
- Character looks professional
- Parts clearly distinguishable
- Smooth transitions between states
- Rarity visually apparent

**Performance:**
- Maintains 30+ FPS on target devices
- No stuttering during part swaps
- Memory usage within limits

**Functionality:**
- All Phase 3 features work
- Stats system unchanged
- Game balance maintained

**Polish:**
- Animations play correctly
- No visual glitches
- Consistent art style

---

## Phase 4 Sign-off

| Component | Status | Notes |
|-----------|--------|-------|
| Base Character | | |
| Part ID System | | |
| Runtime Swapping | | |
| Drop Visuals | | |
| Performance | | |
| Mobile Testing | | |

**Next Steps:** Phase 5 - Polish, VFX, and Audio

---

*Note: This phase focuses on visual implementation using Synty's ID system. The gameplay mechanics from Phase 3 should remain exactly the same - only the visuals change from colored primitives to actual 3D character parts.*