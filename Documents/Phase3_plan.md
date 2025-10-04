# Bulk Up Heroes - Phase 3 Development Specification
## Parts System Implementation (Dummy Visual Version)

### Phase Overview

**Phase 3 Goal**: Implement the complete parts drop, pickup, and stat modification system using placeholder visuals (colored primitive objects) to establish core gameplay mechanics before visual polish.

**Duration**: 2-3 Days  
**Prerequisites**: Phase 2 fully complete with combat and wave systems operational  
**Priority**: Critical - This creates the core progression loop that defines the game

---

## Phase 3 Core Concepts

### System Architecture
```
This phase focuses on MECHANICS over VISUALS:
- Parts drop from enemies
- Players collect parts by walking over them
- Parts immediately modify stats
- Visual representation using colored primitives
- Full stat rebalancing for weak→strong progression
```

### Why Dummy Visuals First
1. **Faster iteration** on gameplay balance
2. **Clear debugging** - separate logic from visual issues
3. **Validate fun factor** before visual investment
4. **Easier stat tuning** without asset complications

---

## 1. Part System Architecture

### 1.1 Part Data Structure

**PartData.cs** (ScriptableObject)
```
Location: Assets/Scripts/Parts/PartData.cs

Properties:
├── Part Identification
│   ├── partID: string (unique identifier)
│   ├── partType: enum (Head, Arms, Torso, Legs)
│   └── rarity: enum (Common, Rare, Epic)
├── Stat Modifiers
│   ├── healthBonus: float
│   ├── damageMultiplier: float
│   ├── attackSpeedMultiplier: float
│   └── moveSpeedMultiplier: float
└── Visual Data (Dummy)
    ├── dummyPrefab: GameObject (colored primitive)
    ├── dropPrefab: GameObject (pickup version)
    └── particleColor: Color
```

### 1.2 Part Type Specifications

| Part Type | Primary Stat | Secondary Stat | Dummy Visual | Color |
|-----------|-------------|---------------|--------------|-------|
| **Head** | Attack Speed +50% | Vision Range +10% | Sphere | Red |
| **Arms** | Damage +100% | Attack Range +10% | Cube | Blue |
| **Torso** | Max Health +100% | Defense (future) | Cylinder | Green |
| **Legs** | Move Speed +30% | Jump (future) | Capsule | Yellow |

### 1.3 Rarity System

| Rarity | Drop Chance | Stat Multiplier | Visual Indicator | Size Scale |
|--------|------------|-----------------|------------------|------------|
| **Common** | 70% | 1.0x | Solid color | 1.0x |
| **Rare** | 25% | 1.5x | Glowing outline | 1.2x |
| **Epic** | 5% | 2.0x | Particle effect | 1.4x |

---

## 2. Part Drop System

### 2.1 Drop Manager Implementation

**PartDropManager.cs**
```
Location: Assets/Scripts/Parts/PartDropManager.cs

Responsibilities:
├── Calculate drop chance on enemy death
├── Determine part type and rarity
├── Spawn part at enemy death location
├── Manage part cleanup (5-second timer)
└── Handle drop pool for performance
```

### 2.2 Drop Mechanics

| Wave | Drop Chance | Rarity Weights | Notes |
|------|------------|---------------|-------|
| 1-2 | 100% | C:90% R:10% E:0% | Guaranteed drops to teach system |
| 3-4 | 80% | C:70% R:25% E:5% | Standard rates |
| 5-6 | 70% | C:60% R:30% E:10% | Better rarity chances |
| 7+ | 60% | C:50% R:35% E:15% | Lower quantity, higher quality |

### 2.3 Drop Behavior

**Visual Presentation**:
```
1. Enemy dies → Small explosion effect
2. Part appears at death location
3. Part hovers 0.5 units above ground
4. Rotates continuously (45°/second)
5. Gentle bounce animation (0.1 unit amplitude)
6. Glowing effect based on rarity
7. Auto-despawn after 5 seconds (fade out)
```

---

## 3. Part Pickup System

### 3.1 Pickup Detection

**PartPickup.cs**
```
Location: Assets/Scripts/Parts/PartPickup.cs

Detection Method:
├── Trigger Collider (radius: 0.5 units)
├── Layer: "Pickup"
├── Only responds to Player layer
├── Auto-collect on contact
└── No manual interaction needed
```

### 3.2 Pickup Flow

```
1. Player enters trigger zone
2. Check if part slot is empty OR lower rarity
3. If valid:
   ├── Apply part to character
   ├── Update stats immediately
   ├── Show pickup feedback
   ├── Destroy pickup object
4. If invalid (same/lower rarity):
   └── Show "Already have better part" message
```

### 3.3 Pickup Feedback

**Immediate Feedback**:
- Screen flash (0.1 seconds, color based on part type)
- Stat change popup: "+100% Damage!" 
- Sound effect (different pitch per rarity)
- Particle burst at pickup location

---

## 4. Character Part System

### 4.1 Player Part Slots

**CharacterPartManager.cs**
```
Location: Assets/Scripts/Player/CharacterPartManager.cs

Structure:
PlayerGameObject
├── PartSlots (Empty GameObject)
│   ├── HeadSlot (Transform)
│   ├── ArmsSlot (Transform)
│   ├── TorsoSlot (Transform)
│   └── LegsSlot (Transform)
```

### 4.2 Dummy Visual Representation

**Part Visual Hierarchy**:
```
HeadSlot
└── Head_Common (Red Sphere)
    ├── MeshRenderer
    ├── Local Position: (0, 1.2, 0)
    ├── Scale: (0.3, 0.3, 0.3)
    └── Rotation: (0, 0, 0)

ArmsSlot
└── Arms_Rare (Blue Cube with glow)
    ├── MeshRenderer
    ├── Local Position: (0, 0.5, 0)
    ├── Scale: (0.6, 0.3, 0.3) * 1.2 (rare bonus)
    └── Particle System (rare glow)
```

### 4.3 Part Attachment Points

| Slot | Local Position | Base Scale | Rotation |
|------|---------------|------------|----------|
| Head | (0, 1.2, 0) | (0.3, 0.3, 0.3) | None |
| Arms | (0, 0.5, 0) | (0.6, 0.3, 0.3) | None |
| Torso | (0, 0, 0) | (0.5, 0.7, 0.4) | None |
| Legs | (0, -0.7, 0) | (0.3, 0.5, 0.3) | None |

---

## 5. Stat Modification System

### 5.1 Base Stats Rebalance

**PlayerStats.cs Updates**:
```
Previous (Phase 2):          New (Phase 3):
├── Health: 100              ├── Health: 30
├── Damage: 10               ├── Damage: 3
├── Attack Speed: 1.0        ├── Attack Speed: 0.5
└── Move Speed: 5.0          └── Move Speed: 4.0
```

### 5.2 Stat Calculation Formula

```
Final Stat = Base Stat * (1 + Part Bonus) * Rarity Multiplier

Example (Arms Part):
├── Base Damage: 3
├── Arms Bonus: +100% (2x multiplier)
├── Rare Multiplier: 1.5x
└── Final: 3 * 2 * 1.5 = 9 damage
```

### 5.3 Full Set Comparison

| Stat | No Parts | Common Set | Rare Set | Epic Set |
|------|----------|------------|----------|----------|
| Health | 30 | 60 | 90 | 120 |
| Damage | 3 | 6 | 9 | 12 |
| Attack Speed | 0.5/s | 0.75/s | 1.125/s | 1.5/s |
| Move Speed | 4.0 | 5.2 | 6.24 | 7.28 |

---

## 6. UI Updates

### 6.1 Part Inventory Display

**Location**: Top-left corner below health bar
```
Visual Layout:
┌─────────────────┐
│ [Head: Empty]   │
│ [Arms: Common]  │
│ [Torso: Rare]   │
│ [Legs: Empty]   │
└─────────────────┘
```

**PartInventoryUI.cs**:
```
Features:
├── Shows current equipped parts
├── Color-coded by rarity
├── Updates instantly on pickup
└── Shows "Empty" for missing parts
```

### 6.2 Pickup Notifications

**Location**: Center screen, above announcements
```
Format: "[PART TYPE] ACQUIRED!"
Subtext: "+[STAT CHANGE]"
Duration: 2 seconds
Animation: Slide up + fade out
```

### 6.3 Stat Change Indicators

**Floating Text**:
```
Position: Above player head
Format: "+50% Attack Speed"
Color: Matches part type
Movement: Float up 1 unit over 1 second
Fade: Start after 0.5 seconds
```

---

## 7. Enemy Rebalancing

### 7.1 Enemy Stats Adjustment

To make parts feel impactful:

| Wave | Enemy Health | Enemy Damage | Notes |
|------|-------------|--------------|-------|
| 1 | 20 | 8 | Player needs 7 hits without parts |
| 2 | 25 | 10 | Requires first part to manage |
| 3 | 30 | 12 | Difficult without 2+ parts |
| 5 | 40 | 15 | Impossible without good parts |
| 10 | 60 | 25 | Requires near-complete set |

### 7.2 Difficulty Curve

**Design Philosophy**:
- Wave 1-2: Tutorial, guaranteed part drops
- Wave 3-4: Challenge ramps up, parts essential
- Wave 5+: Parts quality matters more than quantity

---

## 8. Implementation Checklist

### 8.1 Core Systems
- [ ] Create PartData ScriptableObjects
- [ ] Implement PartDropManager
- [ ] Create PartPickup prefabs with triggers
- [ ] Build CharacterPartManager
- [ ] Update PlayerStats for part modifiers
- [ ] Create colored dummy prefabs

### 8.2 Drop System
- [ ] Enemy death triggers drop roll
- [ ] Parts spawn at death location
- [ ] Rotation and hover animation
- [ ] 5-second despawn timer
- [ ] Object pooling for pickups

### 8.3 Pickup System  
- [ ] Trigger detection on player
- [ ] Stat application on pickup
- [ ] Visual feedback (particles, flash)
- [ ] Rarity comparison logic
- [ ] Sound effects (if time permits)

### 8.4 UI Systems
- [ ] Part inventory display
- [ ] Pickup notifications
- [ ] Stat change popups
- [ ] Update health bar for new max health

### 8.5 Balance Changes
- [ ] Reduce player base stats
- [ ] Adjust enemy stats per wave
- [ ] Test progression curve
- [ ] Verify parts feel impactful

---

## 9. Testing Scenarios

### 9.1 Part Drop Testing
```
Test Case 1: Basic Drop
1. Kill enemy in Wave 1
2. Verify part drops 100% of time
3. Check hover and rotation
4. Confirm 5-second despawn

Test Case 2: Rarity Distribution
1. Kill 100 enemies in Wave 5
2. Track rarity distribution
3. Should be ~60% Common, 30% Rare, 10% Epic
```

### 9.2 Pickup Testing
```
Test Case 1: First Part
1. Start with no parts
2. Pickup any part
3. Verify immediate stat change
4. Check visual attachment

Test Case 2: Upgrade Part
1. Have Common Arms
2. Pickup Rare Arms
3. Common should be replaced
4. Stats should increase

Test Case 3: Downgrade Prevention
1. Have Epic Head
2. Try to pickup Common Head
3. Should show "Already have better"
4. Epic should remain
```

### 9.3 Balance Testing
```
Test Case 1: No Parts Challenge
1. Try to complete Wave 3 without parts
2. Should be extremely difficult
3. Player should want parts desperately

Test Case 2: Full Set Power
1. Equip all Epic parts
2. Should easily handle Wave 10
3. Player should feel like a god
```

---

## 10. Performance Considerations

### Object Pooling
- Pool 20 part pickups at start
- Reuse for all drops
- Never instantiate during gameplay

### Optimization Limits
- Maximum 10 parts on ground simultaneously
- Oldest parts despawn if limit exceeded
- Particle effects only for Rare/Epic

### Mobile Performance
- Simple colored materials (no textures)
- Minimal particle effects
- No complex shaders

---

## 11. Debug Tools

### Context Menu Commands
```
CharacterPartManager:
├── "Give Random Part"
├── "Give Specific Part"
├── "Clear All Parts"
└── "Give Full Epic Set"

PartDropManager:
├── "Force Drop Next Kill"
├── "Set Drop Rate"
└── "Spawn Part At Cursor"
```

### Debug Visualization
- Show part slots as wireframe boxes
- Display current stat multipliers
- Show drop chance on enemy hover
- Part rarity percentage overlay

---

## 12. Phase 3 Completion Criteria

**The phase is complete when:**

1. **Parts drop from enemies** based on wave and chance
2. **Pickups work instantly** with proper feedback
3. **Stats change dramatically** with parts
4. **Visual slots show equipped parts** (colored primitives)
5. **Game difficulty requires parts** to progress
6. **Player feels progression** from weak to strong
7. **All systems are stable** for 10+ minute sessions

---

## 13. Handoff to Phase 4

### What Phase 4 Will Add
- Synty character integration
- Real 3D model parts
- Smooth part swapping animations
- Character customization persistence
- Advanced visual effects

### What Must Be Ready
- [ ] All part logic fully functional
- [ ] Stat system completely balanced
- [ ] Drop rates finalized
- [ ] UI systems working
- [ ] Performance optimized

---

## Phase 3 Sign-off

| Role | Name | Date | Notes |
|------|------|------|-------|
| Developer | | | Core systems complete |
| QA Tester | | | Balance validated |
| Designer | | | Fun factor confirmed |

**Critical Success Metric**: Players should feel dramatic power growth from parts and desperately want to collect them.

---

**Remember**: This phase is about making the game FUN, not pretty. Colored cubes that make you powerful are better than beautiful models that don't affect gameplay. Validate the core loop before moving to visual polish.