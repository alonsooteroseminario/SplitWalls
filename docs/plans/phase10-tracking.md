# Phase 10 Tracking — WallProfileBuilder Service + Renames

**Branch:** `refactor/phase0-cleanup`
**Goal:** Extract 28 wall-profile-builder local functions into `Services/WallProfileBuilder.cs`, rename `aaaaaa` → `wallPanel`, delete 4 unused CS0219 variable declarations.
**Last Updated:** 2026-03-10
**Status:** COMPLETE

---

## Tasks

| # | Task | Status | Notes |
|---|------|--------|-------|
| P10-1 | Rename `aaaaaa` → `wallPanel`, delete VIo/VFo_previo_vacio | ✅ DONE | 407 renames, 8 lines deleted |
| P10-2 | Create `Services/WallProfileBuilder.cs`, extract 28 methods | ✅ DONE | 2,667 lines removed from ThisApplication.cs |
| P10-3 | Update 298 call sites → `profileBuilder.BuildXxx(` | ✅ DONE | All old Revision6_DYNO_* names gone |
| P10-4 | Register in csproj, build verify (zero errors) | ✅ DONE | Added `using System.Linq` for `.First()` |
| P10-5 | Commit | ✅ DONE | Commits a99332e + 95d6f92 |

---

## Results

| Metric | Before | After |
|--------|--------|-------|
| `ThisApplication.cs` lines | 10,785 | 8,110 (−2,675) |
| `Services/WallProfileBuilder.cs` | — | 2,676 lines (new) |
| `aaaaaa` occurrences | 407 | 0 (→ `wallPanel`) |
| CS0219 vars deleted | 4 | 0 |
| Call sites updated | 0 | 298 |
| Methods extracted | 0 | 28 |

---

## What was extracted

All 27 `Revision6_DYNO_*` profile-builder local functions + `ReplaceWallWithProfile` helper,
moved to `internal class WallProfileBuilder` with `private readonly Document _doc`.

Method renames: see `docs/plans/2026-03-10-phase10-design.md` for full table.

## Bugs fixed during execution

1. **Brace counter exited early on multi-line parameters** — methods whose parameter list
   spans multiple lines have `depth == 0` before the opening `{`. Fixed by tracking
   `entered = True` when `depth > 0` and only exiting when `entered and depth == 0`.

2. **Missing `using System.Linq`** — `List<Wall>.First()` LINQ extension used in 5 methods;
   `ThisApplication.cs` had this via its existing usings; `WallProfileBuilder.cs` needed
   it explicitly added.

3. **Declaration line double-indented** — new_decl_line was set to `\t\t` but re-indent
   pass stripped one more tab. Fixed by using `\t\t\t` so re-indent produces `\t\t`.
