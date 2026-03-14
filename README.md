# SplitWalls

Split Revit walls into equal panels — automate prefabricated panel fabrication.

---

## What is SplitWalls?

SplitWalls is a two-product system for splitting Revit wall elements into uniform or custom-width panels:

1. **Desktop Addin** — A Revit 2022–2024 C# API plugin (WPF profile editor + execution engine)
2. **Cloud Web App** *(in development)* — Browser-based tool using APS Design Automation; no Revit desktop license required

---

## 1. Revit Desktop Addin

### Features

- Split any wall or `Walls` category element into equal panels by length
- WPF Profile Editor: visually design wall split configurations on a 2D canvas
- Save/load split profiles as `.txt` (JSON) files
- Supports profile types: standard, U, L, T, I, borde
- Disables wall joins automatically on split segments
- Ribbon button + Form UI for batch execution

### Tech Stack

- C# / .NET 4.8
- Revit API 2022, 2023, 2024
- WPF (XAML) for profile editor UI
- `System.Web.Script.Serialization` for profile JSON (no NuGet)

### Project Structure

```
SplitWalls/
  App.cs                        Ribbon command entry point
  ThisApplication.cs            Main execution logic (IExternalCommand)
  Form1.cs                      UI form with Load Profile button
  Commands/
    OpenProfileEditorCommand.cs  Opens WPF profile editor
  Models/
    WallProfileConfig.cs        Root profile configuration model
    ProfileDefaults.cs          Panel/separator/height defaults
    SplitRule.cs                Uniform or custom split rule
    SegmentDef.cs               Individual panel segment definition
    OpeningDef.cs               Window/door opening definition
  Services/
    ProfileFileService.cs       Load/save .txt profile files
    ProfileExecutionService.cs  Executes profile against Revit walls
    WallProfileBuilder.cs       28 profile builder methods
    WindowDetectionService.cs   Detects hosted openings on walls
  UI/
    ProfileEditorWindow.xaml    WPF 2D profile editor window
    ProfileEditorViewModel.cs   Editor ViewModel + RelayCommand
    ProfileCanvasRenderer.cs    SVG-style 2D wall canvas renderer
  Helpers/
    RevitUnitHelper.cs          mm ↔ feet conversion (304.8)
    WallJoinHelper.cs           Disable/enable wall joins
```

### Build

Requires Visual Studio 2022 and Revit SDK references (2022, 2023, or 2024).

> **Note (WSL):** Build must copy to `C:\Temp\SplitWalls_build` before MSBuild — `Form1.resx` has mark-of-the-web that blocks WSL direct builds. See `MEMORY.md` for the full build command.

---

## 2. SplitWalls Cloud Web App *(in development)*

### Overview

Browser-based wall splitting — upload a `.rvt`, design your split profile, execute via APS Design Automation, verify in 3D, download the result. No Revit desktop license required.

**Free to use.** Users "pay" by sharing their result on X or LinkedIn before downloading.

### Core User Flow

```
Sign up → Upload .rvt → View in 3D → Design splits on 2D canvas
       → Execute (APS DA) → Verify before/after in 3D → Share → Download
```

### Tech Stack

| Layer | Technology |
|-------|-----------|
| Frontend + API | Next.js 14+ (App Router, TypeScript) |
| Auth | Clerk (Google + Microsoft + email) |
| Database | MongoDB Atlas + Prisma ORM |
| Hosting | Vercel |
| Styling | Tailwind CSS + shadcn/ui |
| 3D Viewer | APS Viewer SDK v7 (headless Viewer3D) |
| Cloud Engine | APS Design Automation v3 (Revit 2022/2023/2024) |
| File Storage | APS OSS (signed URLs, 24hr auto-expiry) |

### Key Features

- **2D Profile Editor** — Interactive SVG canvas with draggable split points; uniform or custom splits; snap-to-grid; profile type color legend
- **3D Viewer** — Before/after split-screen using APS Viewer SDK
- **Admin DA Workbench** — Upload AppBundles, register on APS DA, run test WorkItems, view structured logs, debug via before/after 3D viewer — all in the browser
- **Social gate** — Share on X or LinkedIn to unlock download
- **Smart ViewCache** — SHA-256 hash-based caching of Model Derivative translations

### Supported Revit Versions

Revit 2022, 2023, 2024 (three separate AppBundle `.zip` files)

### Architecture

See [`docs/plans/splitwalls-beta-mvp-plan.md`](docs/plans/splitwalls-beta-mvp-plan.md) for the full architecture, API routes, Prisma schema, implementation roadmap, and credentials setup guide.

### UI Mockup

See [`docs/mockups/splitwalls-ui-mockup.html`](docs/mockups/splitwalls-ui-mockup.html) — open in browser for a full interactive demo with fake static data (11 pages: landing, dashboard, 3D viewer, 2D profile editor, status, result, admin bundles, test runner, logs, 3D viewer, history).

---

## Refactoring Progress (Desktop Addin)

The original 17,676-line monolith has been progressively refactored:

| Phase | Work | Lines removed |
|-------|------|--------------|
| 0 | Dead code, rename `mierda`→`targetWall` | −389 |
| 1 | Extract `RevitUnitHelper`, `WallJoinHelper` | −632 |
| 2 | Extract `WindowDetectionService` | −313 |
| 3 | Delete 21 unused local functions | −2,523 |
| 4 | DispatchButton helper (10 handlers) | −365 |
| 5 | ReplaceWallWithProfile helper | −548 |
| 6 | Extract `PanelOptions` model | — |
| 7 | Delete duplicate void/return functions | −186 |
| 8 | Remove 423 region/endregion pairs | −1,144 |
| 9 | Remove 119 dead comment blocks | −791 |
| 10 | Extract `WallProfileBuilder` (28 methods) | −2,675 |
| **Phase 1** | **WPF Profile Editor (8 steps, 13 new files)** | **+1,450** |

**Current:** `ThisApplication.cs` at ~8,110 lines (was 17,676).

---

## Deferred / TODO

See [`TODOS.md`](TODOS.md) for deferred work items with full context.
