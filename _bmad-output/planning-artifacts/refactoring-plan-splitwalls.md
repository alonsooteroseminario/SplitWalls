# SplitWalls Refactoring Plan

**Date:** 2026-03-10
**Status:** Approved for Implementation
**Goal:** Reduce 17,676-line monolith to clean, maintainable code with zero behavior changes

---

## Executive Summary

The current codebase is a working Revit API C# addin with **17,676 lines in a single file** (`ThisApplication.cs`). It works correctly but suffers from extreme code duplication, no separation of concerns, and maintenance impossibility. The refactoring will reduce the codebase by an estimated **70-80%** while preserving identical behavior.

---

## Current State Analysis

### Codebase Metrics
| Metric | Value |
|--------|-------|
| Total lines | 17,676 (ThisApplication.cs) |
| Unique methods | ~78 `Revision6_` methods |
| Void/Return duplicate pairs | ~40 pairs (identical logic, only difference: returns Wall or void) |
| Hardcoded-1220 / _mod duplicate pairs | 14 pairs (identical, one uses hardcoded 1220) |
| Window detection code blocks (copy-pasted) | 15+ identical blocks |
| Commented-out dead code | ~2,000+ lines |
| Region blocks | 429 |
| Magic numbers | 1220, 304.8, 4, 2440 scattered everywhere |

### Critical Duplication Patterns

**Pattern 1: Void vs Return pairs (~8,000 lines of waste)**
```
void Revision6_DYNO_Create_New_Wall_EditProfile_I_PUERTA_dVIo(...)      // 130 lines
Wall Revision6_DYNO_Create_New_Wall_EditProfile_I_PUERTA_dVIo_return(...)  // 130 lines IDENTICAL + returns Wall
```
Every wall creation method exists twice. The `_return` version is identical but returns the created `Wall`.

**Pattern 2: Hardcoded vs Parameterized buttons (~400 lines of waste)**
```
void Revision6_BUTTON_1()           // hardcodes anchopanel_UI = 1220
void Revision6_BUTTON_1_mod(int anchopanel_UI)  // takes parameter
```
Every button handler exists twice. The non-`_mod` versions are dead code (never called from the form).

**Pattern 3: Window/Door detection code (~800 lines of waste)**
The same 20-line filter pattern (FilteredElementCollector + BuiltInCategory filter) is copy-pasted 15+ times:
```csharp
BuiltInCategory[] bics_familyIns = new BuiltInCategory[] { OST_Doors, OST_Windows };
List<Element> windows_hosted = new List<Element>();
foreach (BuiltInCategory bic in bics_familyIns) {
    ElementClassFilter familyFilter = new ElementClassFilter(typeof(FamilyInstance));
    ElementCategoryFilter MECategoryfilter = new ElementCategoryFilter(bic);
    // ... 15 more lines ...
}
```

**Pattern 4: Wall join disabling (~200 lines of waste)**
This 4-line block appears 50+ times:
```csharp
if (WallUtils.IsWallJoinAllowedAtEnd(wall, 1))
    WallUtils.DisallowWallJoinAtEnd(wall, 1);
if (WallUtils.IsWallJoinAllowedAtEnd(wall, 0))
    WallUtils.DisallowWallJoinAtEnd(wall, 0);
```

**Pattern 5: OtroLado (flip wall) variants (~300 lines of waste)**
Every button handler has a `_OtroLado` variant that just calls `DarVuelta_Muro` first and then delegates to the same logic.

### Bugs Found

1. **`Form1.cs:41-46`**: `textBox1_TextChanged` sets `textString = "1220"` then checks `if (!(textString == "1220"))` - always false, handler is a no-op.
2. **`Form1.cs:148`**: `textString == ""` check can fail if `textString` is null (no `string.IsNullOrEmpty` used).
3. **Variable name `mierda`** (profanity) used as a variable name in production code (lines ~4002-4057).

---

## Refactoring Strategy

### Principles
1. **Zero behavior change** - identical functionality, just reorganized
2. **Bottom-up** - start with smallest, safest extractions first
3. **One pattern at a time** - each phase eliminates one duplication type
4. **Test at each phase** - verify in Revit after each phase

### Target Architecture

```
SplitWalls/
  App.cs                          (unchanged - ribbon entry point)
  ThisApplication.cs              (slim: Execute + routing only, ~150 lines)
  Services/
    WallSelectionService.cs       (pick single/multiple walls from UI)
    WindowDetectionService.cs     (find hosted windows/doors on a wall)
    WallSplitService.cs           (split wall without windows into panels)
    WallProfileService.cs         (create walls with custom profiles - U, T, I, L, Borde shapes)
    WallFlipService.cs            (flip wall orientation)
    PanelizationOrchestrator.cs   (main logic: OSB, SmartPanel, no-window workflows)
  Models/
    PanelOptions.cs               (replaces Form1 checkbox booleans)
    OpeningInfo.cs                 (window/door data: height, sill, width, position)
    WallSegmentData.cs            (wall curve data: start/end params, points)
  Helpers/
    RevitUnitHelper.cs            (mm-to-feet conversion constants)
    WallJoinHelper.cs             (disable wall joins)
  Forms/
    PanelConfigForm.cs            (renamed Form1, with proper properties)
    PanelConfigForm.Designer.cs   (unchanged designer)
```

---

## Implementation Phases

### Phase 0: Preparation (Safety net)
**Estimated reduction: ~2,500 lines**

- [x] Create `.gitignore` (done)
- [x] Remove `bin/`, `obj/`, `packages/`, `.vs/` from tracking (done)
- [ ] Create git branch `refactor/splitwalls-cleanup`
- [ ] Delete ALL commented-out code (~2,000+ lines)
- [ ] Delete all non-`_mod` button handlers (14 methods with hardcoded 1220 - they are dead code, never called)
- [ ] Fix Form1 bug (`textBox1_TextChanged`)
- [ ] Rename variable `mierda` to `targetWall`
- [ ] Remove empty `finally` blocks, empty event handlers (`splitContainer1_Panel1_Paint`)

**Risk: LOW** - only removes dead/broken code

---

### Phase 1: Extract Constants & Helpers
**Estimated reduction: ~300 lines**

Create `Helpers/RevitUnitHelper.cs`:
```csharp
public static class RevitUnitHelper
{
    public const double MmToFeet = 304.8;
    public const int DefaultPanelWidthMm = 1220;
    public const int SeparatorWidthMm = 4;
    public const int DefaultWallHeightMm = 2440;

    public static double MmToInternal(double mm) => mm / MmToFeet;
    public static double InternalToMm(double feet) => feet * MmToFeet;
}
```

Create `Helpers/WallJoinHelper.cs`:
```csharp
public static class WallJoinHelper
{
    public static void DisableJoins(Wall wall)
    {
        if (WallUtils.IsWallJoinAllowedAtEnd(wall, 0))
            WallUtils.DisallowWallJoinAtEnd(wall, 0);
        if (WallUtils.IsWallJoinAllowedAtEnd(wall, 1))
            WallUtils.DisallowWallJoinAtEnd(wall, 1);
    }
}
```

Replace all 50+ occurrences of the join-disable pattern and all `304.8` / `1220` magic numbers.

**Risk: LOW** - mechanical search-and-replace

---

### Phase 2: Extract Window Detection Service
**Estimated reduction: ~600 lines**

Create `Services/WindowDetectionService.cs`:
```csharp
public class WindowDetectionService
{
    private readonly Document _doc;

    public WindowDetectionService(Document doc) { _doc = doc; }

    public List<FamilyInstance> GetHostedOpenings(Element wall)
    {
        // Single implementation of the 20-line filter pattern
        var results = new List<FamilyInstance>();
        var categories = new[] { BuiltInCategory.OST_Doors, BuiltInCategory.OST_Windows };

        foreach (var bic in categories)
        {
            var collector = new FilteredElementCollector(_doc)
                .OfClass(typeof(FamilyInstance))
                .OfCategory(bic);

            foreach (FamilyInstance fi in collector)
            {
                if (fi.Host?.Id == wall.Id)
                    results.Add(fi);
            }
        }
        return results;
    }
}
```

Replace all 15+ copy-pasted filter blocks with `_windowService.GetHostedOpenings(wall)`.

**Risk: LOW** - pure extraction, no logic change

---

### Phase 3: Eliminate Void/Return Duplication
**Estimated reduction: ~6,000-8,000 lines** (biggest win)

For every void/return pair, keep ONLY the `_return` version (which does everything the void does + returns the Wall). Then change all void call sites to simply discard the return value:

```csharp
// BEFORE: Two methods
void Revision6_DYNO_Create_New_Wall_EditProfile_I_PUERTA_dVIo(...) { /* 130 lines */ }
Wall Revision6_DYNO_Create_New_Wall_EditProfile_I_PUERTA_dVIo_return(...) { /* same 130 lines + return */ }

// AFTER: One method
Wall CreateWallWithProfile_I_Door_LeftEdge(...) { /* 130 lines + return */ }

// Call sites that used void version just ignore the return:
CreateWallWithProfile_I_Door_LeftEdge(wall, height, sill, pointDVIo, pointDVFo, wallHeight);
```

Methods to consolidate (~40 pairs):
- `EditProfile_T` / `EditProfile_T_return`
- `EditProfile_I` / `EditProfile_I_return`
- `EditProfile_U` / `EditProfile_U_return`
- `EditProfile_L` / `EditProfile_L_return`
- `EditProfile_BORDE_*` / `EditProfile_BORDE_*_return`
- `EditProfile_Solitario` / `EditProfile_Solitario_return`
- `EditProfile_CasoEspecial` / `EditProfile_CasoEspecial_return`
- All `_PUERTA` variants
- All `_dVIo` / `_dVFo` variants

**Risk: MEDIUM** - must verify each call site, but logic is identical

---

### Phase 4: Consolidate Button Handlers with Strategy Pattern
**Estimated reduction: ~300 lines**

All 10 `_mod` button handlers follow 3 patterns:

**Pattern A - Single wall pick:**
```csharp
Element wall = PickSingleElement();
if (flipSide) wall = FlipWall(wall);
ProcessWall(panelWidth, wall);
```

**Pattern B - Multi wall pick:**
```csharp
List<Wall> walls = PickMultipleWalls();
foreach (wall in walls) {
    if (flipSide) wall = FlipWall(wall);
    ProcessWall(panelWidth, wall);
}
```

Consolidate into:
```csharp
void ExecutePanelization(int panelWidth, PanelType type, bool flipSide, bool multiSelect)
{
    var walls = multiSelect ? SelectMultipleWalls() : new List<Wall> { SelectSingleWall() };

    foreach (var wall in walls)
    {
        var target = flipSide ? _flipService.FlipWall(wall) : wall;

        switch (type)
        {
            case PanelType.NoWindows:
                _splitService.PanelizeWithoutWindows(target, panelWidth);
                break;
            case PanelType.OSB:
                _panelOrchestrator.PanelizeOSB(panelWidth, target);
                break;
            case PanelType.SmartPanel:
                _panelOrchestrator.PanelizeSmartPanel(panelWidth, target);
                break;
        }
    }
}
```

**Risk: LOW** - all handlers already delegate to the same underlying methods

---

### Phase 5: Extract Wall Profile Builders
**Estimated reduction: ~2,000 lines**

All `Create_New_Wall_EditProfile_*` methods follow the same pattern:
1. Get wall curve, start/end points, height
2. Calculate opening edge points
3. Build a profile (list of `Line` segments forming a closed loop)
4. Call `Wall.Create(doc, profile, wallType.Id, levelId, true)`
5. Disable joins, delete original wall

Create `Services/WallProfileService.cs` with a unified builder:

```csharp
public class WallProfileService
{
    public Wall CreateProfiledWall(Document doc, Wall original, ProfileShape shape, OpeningInfo opening)
    {
        var curve = GetWallCurve(original);
        var profile = BuildProfile(curve, shape, opening);

        using (var trans = new Transaction(doc, "Create profiled wall"))
        {
            trans.Start();
            var newWall = Wall.Create(doc, profile, original.WallType.Id, original.LevelId, true);
            WallJoinHelper.DisableJoins(newWall);
            doc.Delete(original.Id);
            trans.Commit();
            return newWall;
        }
    }

    private IList<Curve> BuildProfile(WallCurveData curve, ProfileShape shape, OpeningInfo opening)
    {
        // Unified profile generation based on shape enum:
        // U, T, I, L, Borde_Left, Borde_Right, Solitario, CasoEspecial
    }
}
```

**Risk: MEDIUM-HIGH** - geometry logic is the core business logic, needs careful verification

---

### Phase 6: Clean Up Form1
**Estimated reduction: ~50 lines**

- Rename `Form1` to `PanelConfigForm`
- Replace 7 boolean properties with a `PanelOptions` model
- Extract duplicated checkbox handler logic
- Fix the `textBox1_TextChanged` bug
- Remove commented-out `checkBox8` code

```csharp
public class PanelOptions
{
    public int PanelWidthMm { get; set; } = 1220;
    public PanelType Type { get; set; }        // NoWindows, OSB, SmartPanel
    public CornerSide Corner { get; set; }     // Side1, Side2
    public bool FullWall { get; set; }
}
```

**Risk: LOW** - UI-only changes

---

### Phase 7: Extract Service Classes
**Estimated reduction: ~200 lines (organization, not reduction)**

Move consolidated code into proper service files:
- `WallSelectionService.cs` - PickObject/PickObjects wrappers
- `WallFlipService.cs` - `DarVuelta_Muro_SinVentanas` + `DarVuelta_Muro_ConVentanas`
- `WallSplitService.cs` - `DividirMuroSinVentana` + `splitWall_agregar_separacion40`
- `PanelizationOrchestrator.cs` - `Button_2_OBS`, `Button_2_SMARTPANEL`, `Button_2_OBS_TODO_WALL`

**Risk: LOW** - moving code between files, no logic changes

---

## Estimated Final Result

| Metric | Before | After |
|--------|--------|-------|
| Lines in ThisApplication.cs | 17,676 | ~150 |
| Total project lines | ~18,400 | ~4,000-5,000 |
| Number of files | 4 .cs | ~12 .cs |
| Duplicate method pairs | 40+ | 0 |
| Copy-pasted blocks | 15+ | 0 |
| Magic numbers | 100+ | 0 (all in constants) |

---

## Execution Order & Testing

Each phase should be:
1. Implemented on the `refactor/splitwalls-cleanup` branch
2. Committed separately with clear message
3. **Tested in Revit** with these scenarios:
   - Split wall without windows (both corners)
   - Split OSB wall with 1 window
   - Split OSB wall with 2+ windows
   - Split SmartPanel wall with windows
   - Full wall panelization (multi-select)
   - All "Otro Lado" (flip) variants

---

## Risk Mitigation

1. **Keep original file**: Tag the current state as `v1.3.0-original` before starting
2. **Phase-by-phase commits**: Each phase is a separate commit, easy to revert
3. **Behavioral equivalence**: No new features, no removed features
4. **Phase 3 is the riskiest**: void/return consolidation touches the most code - test extra thoroughly

---

*Generated by BMAD Method v6 - Developer*
