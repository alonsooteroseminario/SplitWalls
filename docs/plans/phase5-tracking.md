# Phase 5 Tracking — Extract Wall Profile Builders

**Branch:** `refactor/phase0-cleanup` (continuing same refactor branch)
**Goal:** Collapse repeated `using (Transaction...) { Wall.Create + DisableJoins + Delete + Commit }` boilerplate into a single `ReplaceWallWithProfile` helper, and strip dead comment lines.
**Last Updated:** 2026-03-10
**Status:** COMPLETE

---

## Tasks

| # | Task | Status | Notes |
|---|------|--------|-------|
| P5-1 | Strip 197 dead single-line comments (`//UIDocument`, `//Document`, `//Application`, `// ESTE FUNCIONA ACTUALMENTE`) | ✅ DONE | Step 1 of script |
| P5-2 | Insert `ReplaceWallWithProfile(Wall source, IList<Curve> profile)` local helper | ✅ DONE | Inserted before first builder method |
| P5-3 | Regex-collapse 19 standard builder methods | ✅ DONE | DOTALL regex with lazy capture groups |
| P5-4 | Verify 5 non-standard methods left unchanged | ✅ DONE | 4 CasoEspecial + 1 2MUROS_Solitario |
| P5-5 | Build verify (zero errors) | ✅ DONE | Only pre-existing CS0168/CS0219 warnings |

---

## Results

| Metric | Before | After |
|--------|--------|-------|
| `ThisApplication.cs` lines | 13,454 | 12,906 (−548) |
| Dead comment lines | 197 | 0 |
| Standard builder methods (collapsed) | 19 | 19 → `ReplaceWallWithProfile` |
| Non-standard methods (unchanged) | 5 | 5 |
| `ReplaceWallWithProfile` helper | 0 | 1 |

---

## Design

Each standard builder had the same 30-line structure:

```csharp
// BEFORE (~30 lines per method):
List<Wall> lista_wall_return = new List<Wall>();
// ... XYZ computations ...
using (Transaction trans = new Transaction(doc, "wall"))
{
    trans.Start();
    IList<Curve> profile = new List<Curve>();
    // ... Line decls + profile.Add calls ...
    Wall wall = Wall.Create(doc, profile, wallType.Id, wall_I.LevelId, true);
    lista_wall_return.Add(wall);
    WallJoinHelper.DisableJoins(wall);
    doc.Delete(wall_I.Id);
    trans.Commit();
}
return lista_wall_return.First();

// AFTER (~same XYZ + profile lines + 2 lines):
// ... XYZ computations kept ...
IList<Curve> profile = new List<Curve>();
// ... Line decls + profile.Add calls kept ...
return ReplaceWallWithProfile(wall_I, profile);
```

The regex used DOTALL with lazy `(.*?)` groups to capture XYZ and profile-building code while requiring an exact footer (`WallJoinHelper.DisableJoins` → `doc.Delete` → `trans.Commit` → `}` → `return .First()`). Methods without this exact footer (CasoEspecial, 2MUROS_Solitario) were safely skipped.

---

## Non-Standard Methods Left Unchanged (5)

| Method | Reason |
|--------|--------|
| `dVFo_PUERTA_CasoEspecial_return` | Extra `Wall.Create` + parameter manipulation before `doc.Delete` |
| `2MUROS_Solitario_return` | Creates walls from lines (not profiles); no `IList<Curve>` |
| `1MURO_Solitario_CasoEspecial_return` | CasoEspecial variant |
| `1MURO_Solitario_CasoEspecial_FinalMuro_return` | CasoEspecial variant |
| `1MURO_Solitario_CasoEspecial_InicialMuro_return` | CasoEspecial variant |

---

## Next Phase

**Phase 6:** Clean Form1 → PanelOptions model.
Tracking: `docs/plans/phase6-tracking.md`
