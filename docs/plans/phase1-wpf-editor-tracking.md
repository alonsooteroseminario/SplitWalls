# Phase 1: WPF Profile Editor — Tracking

**Start Date:** 2026-03-12
**Branch:** `feature/phase1-wpf-profile-editor`
**Design:** `docs/plans/2026-03-12-phase1-wpf-profile-editor-design.md`
**Plan:** `docs/plans/2026-03-12-phase1-implementation-plan.md`

---

## Overview

Replace hardcoded wall profile strategies with data-driven `.txt` (JSON) profiles. Add WPF editor for creating/editing profiles and "Load Profile" to Form1 for execution.

## Step Status

| Step | Description | Branch | Status | Notes |
|------|------------|--------|--------|-------|
| 1 | Data Models (5 classes + Newtonsoft.Json) | `feature/phase1-step1-models` | NOT STARTED | Independent |
| 2 | ProfileFileService (Load/Save/List) | `feature/phase1-step2-file-service` | NOT STARTED | Independent |
| 3 | WPF ViewModel + RelayCommand | `feature/phase1-step3-wpf-viewmodel` | NOT STARTED | Depends: 1,2 |
| 4 | WPF Window XAML + Canvas Renderer | `feature/phase1-step4-wpf-window` | NOT STARTED | Depends: 3 |
| 5 | Ribbon Command (App.cs) | `feature/phase1-step5-ribbon-command` | NOT STARTED | Depends: 4 |
| 6 | Form1 "Load Profile" button | `feature/phase1-step6-form1-load` | NOT STARTED | Depends: 1,2 |
| 7 | ProfileExecutionService | `feature/phase1-step7-execution-service` | NOT STARTED | Depends: 1 |
| 8 | ThisApplication.cs integration | `feature/phase1-step8-integration` | NOT STARTED | Depends: 6,7 |

## Metrics

- **New files:** ~12
- **Modified files:** 3 (Form1.cs, App.cs, ThisApplication.cs)
- **Estimated new lines:** ~1,200-1,500
- **Modified lines:** ~50-80

## Build Results

| Step | Build | Errors | Warnings | Date |
|------|-------|--------|----------|------|
| — | — | — | — | — |

## Merge Log

| Source Branch | Target Branch | Date | Conflicts |
|---------------|---------------|------|-----------|
| — | — | — | — |

## Issues / Blockers

None yet.

## Notes

- Newtonsoft.Json chosen over System.Text.Json (.NET 4.8 compatibility)
- Extended.Wpf.Toolkit already referenced in .csproj (DoubleUpDown controls available)
- Profile files use `.txt` extension but contain JSON (aligns with skill.json schema for cloud migration)
- Existing Form1 workflow preserved — "Load Profile" is additive, not replacing
