# Phase 9 Tracking — Remove Dead Comment Blocks

**Branch:** `refactor/phase0-cleanup` (continuing same refactor branch)
**Goal:** Delete all multi-line commented-out code blocks (dead C# code left behind from earlier refactoring passes) and compact resulting blank lines.
**Last Updated:** 2026-03-10
**Status:** COMPLETE

---

## Tasks

| # | Task | Status | Notes |
|---|------|--------|-------|
| P9-1 | Survey comment distribution | ✅ DONE | 1,166 comment lines; 119 multi-line blocks (723 lines) identified |
| P9-2 | Remove 119 multi-line `//` blocks | ✅ DONE | All verified as dead C# code |
| P9-3 | Collapse resulting 3+ blank runs → 2 | ✅ DONE | 68 excess blank lines removed |
| P9-4 | Build verify (zero errors) | ✅ DONE | Only pre-existing CS0168/CS0219 warnings |

---

## Results

| Metric | Before | After |
|--------|--------|-------|
| `ThisApplication.cs` lines | 11,576 | 10,785 (−791) |
| Multi-line comment blocks | 119 | 0 |
| Dead comment lines removed | — | 723 |
| Excess blank lines collapsed | — | 68 |

---

## What was in those comment blocks

| Origin | Count |
|--------|-------|
| Old `FilteredElementCollector` window-detection blocks (superseded by `WindowDetectionService` in Phase 2 but left as comments) | ~30 blocks |
| Old wall-geometry coordinate variants (abandoned shape attempts) | ~50 blocks |
| Old profile-builder code replaced by `ReplaceWallWithProfile` (Phase 5) | ~20 blocks |
| Miscellaneous dead experiments | ~19 blocks |

---

## Remaining single-line comments (~443 lines)

These were intentionally left:
- Explanatory inline comments (`// altura primer Wall = 2440`, etc.)
- Labelled section markers (`// INPUTS`, `// Crear linea y corregir…`)
- Isolated commented-out single lines (require manual review per call site)
