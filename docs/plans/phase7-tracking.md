# Phase 7 Tracking — Delete Remaining Void/Return Duplicates

**Branch:** `refactor/phase0-cleanup` (continuing same refactor branch)
**Goal:** Eliminate the last void functions that are behaviorally identical to their `_return` counterparts, by redirecting callers and deleting the dead implementations.
**Last Updated:** 2026-03-10
**Status:** COMPLETE

---

## Tasks

| # | Task | Status | Notes |
|---|------|--------|-------|
| P7-1 | Audit remaining void functions for void/return equivalence | ✅ DONE | 2 of 5 are equivalent; 3 have different behavior or no counterpart |
| P7-2 | Redirect 3× `dVIo_PUERTA` void callers → `dVIo_PUERTA_return` | ✅ DONE | Result discarded at call site |
| P7-3 | Redirect 3× `dVFo_PUERTA` void callers → `dVFo_PUERTA_return` | ✅ DONE | Result discarded at call site |
| P7-4 | Delete `Revision6_DYNO_Create_New_Wall_EditProfile_dVIo_PUERTA` (void, 99 lines) | ✅ DONE | Now unused after step P7-2 |
| P7-5 | Delete `Revision6_DYNO_Create_New_Wall_EditProfile_dVFo_PUERTA` (void, 87 lines) | ✅ DONE | Now unused after step P7-3 |
| P7-6 | Build verify (zero errors) | ✅ DONE | Only pre-existing CS0168/CS0219 warnings |

---

## Results

| Metric | Before | After |
|--------|--------|-------|
| `ThisApplication.cs` lines | 12,906 | 12,720 (−186) |
| Void/return duplicate pairs | 2 | 0 |
| Call sites redirected | 6 | 6 |

---

## Void Functions Audited (5 total remaining before this phase)

| Function | Verdict |
|----------|---------|
| `dVIo_PUERTA` (void) | Deleted — identical behavior to `dVIo_PUERTA_return` |
| `dVFo_PUERTA` (void) | Deleted — identical behavior to `dVFo_PUERTA_return` |
| `Solitario` (void) | Kept — no `_return` counterpart; unique algorithm |
| `2MUROS_Solitario` (void) | Kept — **different** behavior from `2MUROS_Solitario_return` (mutates wall in-place vs creates new walls + deletes) |
| `1MURO_Solitario` (void) | Kept — `CasoEspecial_return` is a different algorithm, not a counterpart |

---

## Next Phase

**Phase 8:** Remove `#region`/`#endregion` noise (empty or single-statement regions).
Tracking: `docs/plans/phase8-tracking.md`
