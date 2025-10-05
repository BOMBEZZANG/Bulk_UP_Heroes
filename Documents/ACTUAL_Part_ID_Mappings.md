# ACTUAL Sidekick Part IDs for Your Package

## Your Available Outfits

You have **3 outfit sets** in your Sidekick package:

1. **SK_FANT_KNGT_17** - Fantasy Knight (Heavy Armor) - Use for **EPIC**
2. **SK_SCFI_CIVL_09** - Sci-Fi Civilian (Light Clothing/Armor) - Use for **RARE**
3. **SK_SCFI_CIVL_10** - Sci-Fi Civilian Variant (Accessories) - Use for **RARE/EPIC**
4. **SK_HUMN_BASE_01** - Base Human Body - Use for **COMMON**

## Part Name Format

```
SK_[THEME]_[STYLE]_[SET]_[CODE]_HU01

Example: SK_FANT_KNGT_17_22AHED_HU01
         ↓    ↓     ↓   ↓    ↓
      Fantasy Knight Set17 Head  Human
```

### Part Codes:
- `01HEAD` - Head (face)
- `10TORS` - Torso
- `11AUPL` - Arm Upper Left
- `12AUPR` - Arm Upper Right
- `13ALWL` - Arm Lower Left
- `14ALWR` - Arm Lower Right
- `15HNDL` - Hand Left
- `16HNDR` - Hand Right
- `17HIPS` - Hips
- `18LEGL` - Leg Left
- `19LEGR` - Leg Right
- `20FOTL` - Foot Left
- `21FOTR` - Foot Right
- `22AHED` - Attachment Head (helmets, hats)
- `23AFAC` - Attachment Face (masks, glasses)
- `24ABAC` - Attachment Back (capes, backpacks)

---

## EXACT Part IDs for Each PartData Asset

Copy these **EXACTLY** into your PartData assets in Unity:

### HEAD PARTS

**Part_Head_Common.asset:**
```
Sidekick Part ID: SK_HUMN_BASE_01_01HEAD_HU01
```
(Base human head - naked/plain)

**Part_Head_Rare.asset:**
```
Sidekick Part ID: SK_SCFI_CIVL_09_22AHED_HU01
```
(Sci-Fi civilian helmet/hat)

**Part_Head_Epic.asset:**
```
Sidekick Part ID: SK_FANT_KNGT_17_22AHED_HU01
```
(Fantasy Knight helmet - heavy armor)

---

### TORSO PARTS

**Part_Torso_Common.asset:**
```
Sidekick Part ID: SK_HUMN_BASE_01_10TORS_HU01
```
(Base human torso - naked/plain)

**Part_Torso_Rare.asset:**
```
Sidekick Part ID: SK_SCFI_CIVL_09_10TORS_HU01
```
(Sci-Fi civilian shirt/light armor)

**Part_Torso_Epic.asset:**
```
Sidekick Part ID: SK_FANT_KNGT_17_10TORS_HU01
```
(Fantasy Knight chest armor - heavy armor)

---

### ARM PARTS

**Part_Arms_Common.asset:**
```
Sidekick Part ID: SK_HUMN_BASE_01_11AUPL_HU01
```
(Base human arms - bare skin)

**Part_Arms_Rare.asset:**
```
Sidekick Part ID: SK_SCFI_CIVL_09_11AUPL_HU01
```
(Sci-Fi civilian sleeves/light armor)

**Part_Arms_Epic.asset:**
```
Sidekick Part ID: SK_FANT_KNGT_17_11AUPL_HU01
```
(Fantasy Knight gauntlets - heavy armor)

---

### LEG PARTS

**Part_Legs_Common.asset:**
```
Sidekick Part ID: SK_HUMN_BASE_01_18LEGL_HU01
```
(Base human legs - naked/plain)

**Part_Legs_Rare.asset:**
```
Sidekick Part ID: SK_SCFI_CIVL_09_18LEGL_HU01
```
(Sci-Fi civilian pants/light armor)

**Part_Legs_Epic.asset:**
```
Sidekick Part ID: SK_FANT_KNGT_17_18LEGL_HU01
```
(Fantasy Knight leg armor - heavy armor)

---

## Visual Progression

As player collects parts, they will transform:

**Start (Naked):**
- Base human body parts
- Skin showing

**Common Parts:**
- Still basic human body
- No armor/clothing

**Rare Parts:**
- Sci-Fi civilian clothes
- Light armor
- More covered

**Epic Parts:**
- Full Fantasy Knight armor
- Heavy plates
- Fully armored warrior

---

## Important Notes

1. **The system auto-maps arms/legs:**
   - You specify: `SK_FANT_KNGT_17_11AUPL_HU01` (Arm Upper Left)
   - System uses: `11AUPL`, `12AUPR`, `13ALWL`, `14ALWR`, `15HNDL`, `16HNDR`
   - All from same outfit set (17)

2. **Hips are auto-added:**
   - System adds matching `17HIPS` part automatically
   - Hips connect torso to legs

3. **Case-sensitive!**
   - Must be EXACTLY as shown
   - No spaces, no typos

4. **Alternative: Mix and Match**

If you want different visual progression:

**Option 1: Start Clothed**
```
Common Head: SK_SCFI_CIVL_09_22AHED_HU01  (civilian)
Rare Head: SK_SCFI_CIVL_10_22AHED_HU01    (better civilian)
Epic Head: SK_FANT_KNGT_17_22AHED_HU01    (knight)
```

**Option 2: All Knight Variants**
```
Common: SK_FANT_KNGT_17_... (light knight pieces)
Rare: SK_FANT_KNGT_17_... (medium knight pieces)
Epic: SK_FANT_KNGT_17_... (heavy knight pieces)
```

---

## Complete Part List (For Reference)

**Fantasy Knight (SK_FANT_KNGT_17):**
- 10TORS, 11-16 (arms/hands), 17HIPS, 18-21 (legs/feet)
- 22AHED, 23AFAC, 24ABAC (helmet, face, back)
- 25-34 (various attachments)

**Sci-Fi Civilian 09 (SK_SCFI_CIVL_09):**
- 02HAIR (hair style)
- 10TORS, 11-16 (arms/hands), 17HIPS, 18-21 (legs/feet)
- 22AHED, 23AFAC, 24ABAC
- 25-34 (various attachments)

**Sci-Fi Civilian 10 (SK_SCFI_CIVL_10):**
- 22AHED (different helmet)
- 26-30 (hip/shoulder attachments)

**Base Human (SK_HUMN_BASE_01):**
- 01HEAD, 02HAIR, 03-08 (facial features)
- 10TORS, 11-16 (arms/hands), 17HIPS, 18-21 (legs/feet)
- 35-38 (nose, teeth, tongue, extras)

---

## Next Steps

1. Open Unity
2. Navigate to `Assets/Data/Parts/`
3. For each PartData asset:
   - Click to select
   - Find "Sidekick Part ID" field
   - Copy-paste the EXACT ID from above
4. Save all
5. Press Play
6. Pick up parts - should now use Sidekick!

---

*These are the ACTUAL part names in YOUR package - guaranteed to work!*
