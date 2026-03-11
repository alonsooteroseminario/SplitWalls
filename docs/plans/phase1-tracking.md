# Phase 1 Tracking — Extract Constants & WallJoin Helper

**Branch:** `refactor/phase0-cleanup` (continuing same refactor branch)
**Goal:** Replace magic numbers and repeated wall-join boilerplate with named helpers.
**Last Updated:** 2026-03-10
**Status:** COMPLETE

---

## Tasks

| # | Task | Status | Notes |
|---|------|--------|-------|
| P1-1 | Create `Helpers/RevitUnitHelper.cs` | ✅ DONE | `MmToFeet=304.8`, `DefaultPanelWidthMm=1220`, `SeparatorWidthMm=4` |
| P1-2 | Create `Helpers/WallJoinHelper.cs` | ✅ DONE | `DisableJoins(Wall wall)` — disables both ends |
| P1-3 | Register both in `SplitWalls.csproj` | ✅ DONE | Added `<Compile Include="Helpers\*.cs" />` entries |
| P1-4 | Replace 113× `304.8` → `RevitUnitHelper.MmToFeet` | ✅ DONE | All in `ThisApplication.cs` |
| P1-5 | Replace 158× 4-line WallJoin block → `WallJoinHelper.DisableJoins(x)` | ✅ DONE | −632 lines |

---

## Results

| Metric | Before | After |
|--------|--------|-------|
| `ThisApplication.cs` lines | 17,287 | 16,655 (−632) |
| `304.8` occurrences | 113 | 0 |
| `WallJoinHelper.DisableJoins` calls | 0 | 158 |
| `IsWallJoinAllowedAtEnd` remaining | 321 | 5 (intentional single-end disables) |
| New files | — | `Helpers/RevitUnitHelper.cs`, `Helpers/WallJoinHelper.cs` |

---

## Not Replaced (Intentional)

5 `IsWallJoinAllowedAtEnd` calls remain:
- 2 are commented-out dead code (lines ~15110, ~15113)
- 3 are single-end-only disables (only end=1, not both ends) — intentionally
  different from the standard 4-line pattern; `WallJoinHelper.DisableJoins`
  would change behavior if applied here.

---

## Next Phase

**Phase 2:** Extract `WindowDetectionService` — the 20-line
`FilteredElementCollector + BuiltInCategory` filter block copy-pasted 15+ times.
Tracking: `docs/plans/phase2-tracking.md`
