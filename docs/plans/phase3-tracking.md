# Phase 3 Tracking — Delete Unused Local Functions

**Branch:** `refactor/phase0-cleanup` (continuing same refactor branch)
**Goal:** Delete all local functions flagged as unused by CS8321 compiler warnings.
**Last Updated:** 2026-03-10
**Status:** COMPLETE

---

## Tasks

| # | Task | Status | Notes |
|---|------|--------|-------|
| P3-1 | Identify unused functions via CS8321 build warnings | ✅ DONE | 20 functions initially; 21 total after cascade |
| P3-2 | Delete 20 unused void/return local function pairs | ✅ DONE | Python brace-counting script |
| P3-3 | Delete `Revision6_InsertOpening_void` (cascade unused) | ✅ DONE | Its only caller was inside a deleted function |
| P3-4 | Build verify (zero errors, zero CS8321) | ✅ DONE | Clean build, no unused-function warnings remain |

---

## Results

| Metric | Before | After |
|--------|--------|-------|
| `ThisApplication.cs` lines | 16,342 | 13,819 (−2,523) |
| Unused local functions | 21 | 0 |
| CS8321 warnings | 20 | 0 |
| CS0168/CS0219 warnings | 19 | 19 (pre-existing, not our concern) |

---

## Functions Deleted (21 total)

All were `void` versions of wall profile builder functions — exact duplicates of
`_return` counterparts but without the `return wall;` line. The `_return` versions
are the ones actually called by the active code.

| Function | Lines |
|----------|-------|
| `Revision6_DYNO_Wall_EditProfile_U_PUERTA_VENTANA_izquierda` | 135 |
| `Revision6_DYNO_Wall_EditProfile_U_PUERTA_PUERTA_izquierda` | 132 |
| `Revision6_DYNO_Wall_EditProfile_3VENT_V_P_V` | 157 |
| `Revision6_DYNO_Wall_EditProfile_3VENT_V_P_P` | 155 |
| `Revision6_DYNO_Wall_EditProfile_3VENT_P_P_V` | 156 |
| `Revision6_DYNO_Wall_EditProfile_3VENT_P_P_P` | 154 |
| `Revision6_DYNO_Create_New_Wall_EditProfile_BORDE_PUERTA_DERECHA` | 119 |
| `Revision6_DYNO_Create_New_Wall_EditProfile_BORDE_PUERTA_IZQUIERDO` | 128 |
| `Revision6_DYNO_Wall_EditProfile_U_PUERTA` | 146 |
| `Revision6_DYNO_Create_New_Wall_EditProfile_T_PUERTA` | 124 |
| `Revision6_DYNO_Create_New_Wall_EditProfile_I_PUERTA_dVIo` | 129 |
| `Revision6_DYNO_Create_New_Wall_EditProfile_I_PUERTA_dVFo` | 129 |
| `Revision6_DYNO_Create_New_Wall_EditProfile_I` | 129 |
| `Revision6_DYNO_Create_New_Wall_EditProfile_dVIo` | 93 |
| `Revision6_DYNO_Create_New_Wall_EditProfile_dVFo` | 94 |
| `Revision6_DYNO_Create_New_Wall_1MURO_Solitario_CasoEspecial` | 96 |
| `Revision6_DYNO_Create_New_Wall_EditProfile_dVIo_dPH_dVFo` | 109 |
| `Revision6_DYNO_Create_New_Wall_EditProfile_dVIo_dPH_dVFo_edit` | 107 |
| `Revision6_InsertOpening` (Opening-returning, unused) | 30 |
| `Revision6_DYNO_Girar180_Muro_ConVentanas` | 172 |
| `Revision6_InsertOpening_void` (cascade: caller was deleted) | 29 |

---

## Next Phase

**Phase 4:** Consolidate button handlers (10 handlers → 1 strategy method).
Tracking: `docs/plans/phase4-tracking.md`
