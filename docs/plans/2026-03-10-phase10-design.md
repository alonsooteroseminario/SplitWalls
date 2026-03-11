# Phase 10 Design — WallProfileBuilder Service + Renames

**Date:** 2026-03-10
**Status:** Approved

## Goal

Extract the 27 wall-profile-builder local functions into a proper service class (`Services/WallProfileBuilder.cs`), and rename the `aaaaaa` variable + delete unused variable declarations that generate CS0219 warnings.

## Section 1 — WallProfileBuilder service class

### What moves
- `ReplaceWallWithProfile(Wall source, IList<Curve> profile)` — becomes `private`
- All 27 `Revision6_DYNO_*EditProfile*` and `Revision6_DYNO_Create_New_Wall_*` methods

### New class shape
```csharp
namespace SplitWalls
{
    using Autodesk.Revit.DB;
    using System.Collections.Generic;

    internal class WallProfileBuilder
    {
        private readonly Document _doc;
        internal WallProfileBuilder(Document doc) { _doc = doc; }

        private Wall ReplaceWallWithProfile(Wall source, IList<Curve> profile) { ... }

        public Wall BuildU_DoorWindowRight(Wall wall, ...) { ... }
        // ... all builders
    }
}
```

### Usage in ThisApplication.cs
One declaration added near the top of `Execute` (before local functions that call it):
```csharp
var profileBuilder = new WallProfileBuilder(doc);
```
Remaining local panelizer functions capture `profileBuilder` from outer scope.
All call sites change from `Revision6_DYNO_Create_New_Wall_EditProfile_dVFo_PUERTA_return(wall, ...)`
to `profileBuilder.BuildDoorRight(wall, ...)`.

### Scope change inside the service
- `doc` references in method bodies become `_doc`
- Method bodies otherwise unchanged (parameter names like `_wall_`, `wall_I` left as-is)

## Section 2 — Renames and deletions

### Variable renames
| Old | New | Occurrences |
|-----|-----|-------------|
| `aaaaaa` | `wallPanel` | 403 |

### Deletions (CS0219 — assigned but never read)
| Variable | Locations |
|----------|-----------|
| `VIo_previo_vacio` | 2 declaration sites (4 lines each with set) |
| `VFo_previo_vacio` | 2 declaration sites (4 lines each with set) |

### Method renames (old local fn → new service method)
| Old | New |
|-----|-----|
| `Revision6_DYNO_Wall_EditProfile_U_PUERTA_VENTANA_derecha_return` | `BuildU_DoorWindowRight` |
| `Revision6_DYNO_Wall_EditProfile_U_PUERTA_PUERTA_derecha_return` | `BuildU_DoorDoorRight` |
| `Revision6_DYNO_Wall_EditProfile_U_PUERTA_VENTANA_izquierda_return` | `BuildU_DoorWindowLeft` |
| `Revision6_DYNO_Wall_EditProfile_U_PUERTA_PUERTA_izquierda_return` | `BuildU_DoorDoorLeft` |
| `Revision6_DYNO_Wall_EditProfile_3VENT_V_P_V_return` | `Build3Opening_WindowDoorWindow` |
| `Revision6_DYNO_Wall_EditProfile_3VENT_V_P_P_return` | `Build3Opening_WindowDoorDoor` |
| `Revision6_DYNO_Wall_EditProfile_3VENT_P_P_V_return` | `Build3Opening_DoorDoorWindow` |
| `Revision6_DYNO_Wall_EditProfile_3VENT_P_P_P_return` | `Build3Opening_DoorDoorDoor` |
| `Revision6_DYNO_Create_New_Wall_EditProfile_BORDE_PUERTA_DERECHA_return` | `BuildEdgeDoorRight` |
| `Revision6_DYNO_Create_New_Wall_EditProfile_BORDE_PUERTA_IZQUIERDO_return` | `BuildEdgeDoorLeft` |
| `Revision6_DYNO_Wall_EditProfile_U_PUERTA_return` | `BuildU_Door` |
| `Revision6_DYNO_Create_New_Wall_EditProfile_T_PUERTA_return` | `BuildT_Door` |
| `Revision6_DYNO_Create_New_Wall_EditProfile_I_PUERTA_dVIo_return` | `BuildI_DoorLeft` |
| `Revision6_DYNO_Create_New_Wall_EditProfile_I_PUERTA_dVFo_return` | `BuildI_DoorRight` |
| `Revision6_DYNO_Create_New_Wall_EditProfile_I_return` | `BuildI` |
| `Revision6_DYNO_Create_New_Wall_EditProfile_dVIo_return` | `BuildWindowLeft` |
| `Revision6_DYNO_Create_New_Wall_EditProfile_dVFo_return` | `BuildWindowRight` |
| `Revision6_DYNO_Create_New_Wall_EditProfile_dVIo_PUERTA_return` | `BuildDoorLeft` |
| `Revision6_DYNO_Create_New_Wall_EditProfile_dVFo_PUERTA_return` | `BuildDoorRight` |
| `Revision6_DYNO_Create_New_Wall_EditProfile_dVFo_PUERTA_CasoEspecial_return` | `BuildDoorRightSpecialCase` |
| `Revision6_DYNO_Create_New_Wall_EditProfile_Solitario` | `BuildSolitario` |
| `Revision6_DYNO_Create_New_Wall_2MUROS_Solitario` | `BuildTwoWallSolitario` |
| `Revision6_DYNO_Create_New_Wall_2MUROS_Solitario_return` | `BuildTwoWallSolitarioReturn` |
| `Revision6_DYNO_Create_New_Wall_1MURO_Solitario` | `BuildOneWallSolitario` |
| `Revision6_DYNO_Create_New_Wall_1MURO_Solitario_CasoEspecial_return` | `BuildOneWallSolitarioSpecialCase` |
| `Revision6_DYNO_Create_New_Wall_1MURO_Solitario_CasoEspecial_FinalMuro_return` | `BuildOneWallSolitarioSpecialEndWall` |
| `Revision6_DYNO_Create_New_Wall_1MURO_Solitario_CasoEspecial_InicialMuro_return` | `BuildOneWallSolitarioSpecialStartWall` |

## Constraints
- Zero behavior change — compile and behavior identical before and after
- Build must be green (zero errors) before commit
- Method bodies inside WallProfileBuilder left as-is except `doc` → `_doc`
