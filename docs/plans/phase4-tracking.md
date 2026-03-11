# Phase 4 Tracking — Consolidate Button Handlers

**Branch:** `refactor/phase0-cleanup` (continuing same refactor branch)
**Goal:** Replace 10 near-identical button handler functions with a single `DispatchButton` helper.
**Last Updated:** 2026-03-10
**Status:** COMPLETE

---

## Tasks

| # | Task | Status | Notes |
|---|------|--------|-------|
| P4-1 | Replace 10-branch if/else with `DispatchButton` calls | ✅ DONE | dispatcher now 32 lines vs 55 |
| P4-2 | Add `DispatchButton(multiSelect, flip, strategy)` helper | ✅ DONE | 16-line helper using `Func<Element,Element>` + `Action<Element>` |
| P4-3 | Delete `#region BUTTONS MODIFICADOS PARA ADDIN` (10 `_mod` functions) | ✅ DONE | ~330 lines removed |
| P4-4 | Delete `BUTTON_2_OBS_TODO_WALL_INPUT` thin wrapper (now unused) | ✅ DONE | Callers were inside deleted region |
| P4-5 | Build verify (zero errors) | ✅ DONE | One fix: orphan `catch` block from partial regex deletion removed |

---

## Results

| Metric | Before | After |
|--------|--------|-------|
| `ThisApplication.cs` lines | 13,819 | 13,454 (−365) |
| Handler functions (`_mod`) | 10 | 0 |
| `DispatchButton` helper | 0 | 1 (16 lines) |
| Thin wrapper (`INPUT`) | 1 | 0 |

---

## Design

The 10 original handlers varied on three axes:

| Axis | Values |
|------|--------|
| Selection | single `PickObject` / multi `PickObjects` |
| Flip | none / `DarVuelta_Muro_ConVentanas` / `DarVuelta_Muro_SinVentanas` |
| Strategy | `PanelizarMuroInicial_SMARTPANEL_0_VENTANA` / `Button_2_OBS` / `Button_2_OBS_TODO_WALL` / `Button_2_SMARTPANEL` |

`DispatchButton` takes `Func<Element,Element> flip` and `Action<Element> strategy` delegates,
reducing the 10 methods + dispatcher to 10 call-site lines + one 16-line helper.

---

## Next Phase

**Phase 5:** Extract wall profile builders (U/T/I/L/Borde shapes) — a large block of
similar `Wall.Create(doc, profile, ...)` methods that share the same profile-construction structure.
Tracking: `docs/plans/phase5-tracking.md`
