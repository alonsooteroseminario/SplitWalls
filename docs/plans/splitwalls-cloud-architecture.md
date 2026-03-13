# SplitWalls Cloud — Architecture Design Document

**Date:** 2026-03-11  
**Status:** Draft — Pending Review  
**Goal:** Transform the client-specific SplitWalls Revit addin into a scalable, cloud-based web application where any user can define wall split rules via a visual editor and execute them against uploaded `.rvt` files using APS Design Automation.

---

## 1. System Overview

SplitWalls Cloud is a three-tier application:

1. **Frontend** — Next.js (TypeScript) with Redux Toolkit for state management. Includes a 2D wall profile editor, an APS Viewer for 3D wall selection, and job management UI.
2. **Backend** — Next.js API routes that orchestrate APS authentication, OSS storage, Model Derivative translation, and Design Automation WorkItems.
3. **Revit Engine** — A C# `IExternalDBApplication` AppBundle deployed to APS Design Automation. It reads a `skill.json` file defining split rules and modifies the `.rvt` model accordingly.

### High-Level Data Flow

```
User                   Frontend (Next.js)              Backend (API Routes)           APS Cloud
─────                  ─────────────────               ──────────────────             ─────────

1. Upload .rvt    ──→  POST /api/aps/upload       ──→  Upload to OSS Bucket     ──→  Stored in OSS
                                                   ──→  Translate via Model Deriv ──→  SVF2 viewable

2. View model     ──→  APS Viewer SDK loads SVF2   ←──  Viewer token             ←──  SVF2 stream
   Select walls   ──→  Viewer selection events
                       Store wall IDs + metadata
                       in Redux wallSlice

3. Configure      ──→  Wall Profile Editor (2D)
   splits              Define splits, profiles,
                       openings, strategy per wall
                       Store in Redux skillSlice

4. Execute        ──→  POST /api/aps/execute       ──→  Upload skill.json to OSS
                                                   ──→  Create WorkItem:
                                                        input: .rvt + skill.json
                                                        output: result.rvt
                                                   ──→  DA runs AppBundle       ──→  Revit engine
                                                                                     reads skill.json
                                                                                     splits walls
                                                                                     saves result.rvt

5. Download       ──→  GET /api/aps/download        ──→  Fetch from OSS          ←──  result.rvt
   result              or re-translate for viewer
```

---

## 2. Frontend Architecture

### 2.1 Tech Stack

| Layer | Technology |
|-------|-----------|
| Framework | Next.js 14+ (App Router, TypeScript) |
| State | Redux Toolkit with slice files |
| 3D Viewer | APS Viewer SDK (v7) embedded via `@aps_sdk/viewer` |
| 2D Editor | SVG-based wall profile editor (adapted from existing `wall-profile-editor.jsx`) |
| Styling | Tailwind CSS |
| HTTP | RTK Query (built into Redux Toolkit) |

### 2.2 Redux Store Structure

```typescript
// store/
//   index.ts           — configureStore
//   slices/
//     modelSlice.ts    — uploaded model state, OSS URN, translation status
//     wallSlice.ts     — selected walls from viewer, wall metadata
//     skillSlice.ts    — split rules, segment profiles, openings per wall
//     jobSlice.ts      — WorkItem status, progress, results
//   api/
//     apsApi.ts        — RTK Query endpoints for all backend calls
```

**modelSlice** manages:
- `uploadStatus`: idle | uploading | translating | ready | error
- `ossUrn`: the base64-encoded URN for the uploaded model
- `viewerToken`: short-lived APS access token for the viewer

**wallSlice** manages:
- `walls[]`: array of `{ dbId, elementId, name, typeName, lengthMm, heightMm, level, hasOpenings }`
- `selectedWallIds[]`: walls the user has clicked in the viewer
- `wallMetadata`: property data extracted from the viewer's property panel

**skillSlice** manages (per selected wall):
- `wallConfigs{}`: keyed by wall elementId, each containing:
  - `splitRule`: { method: 'uniform' | 'custom', panelWidthMm, splitPointsMm[] }
  - `segments[]`: { startMm, endMm, profile, fireRating, label }
  - `openings[]`: { xMm, yMm, widthMm, heightMm, type }
  - `strategy`: 'noWindows' | 'osb' | 'smartPanel'
  - `execution`: { separatorWidthMm, disableWallJoins, startFromCorner }

**jobSlice** manages:
- `currentJob`: { workItemId, status, progress, reportUrl }
- `jobHistory[]`: past jobs with download links

### 2.3 Page Structure

```
app/
  layout.tsx                    — shell with nav
  page.tsx                      — landing / dashboard
  upload/
    page.tsx                    — drag-and-drop .rvt upload + translation progress
  viewer/
    page.tsx                    — APS 3D Viewer with wall selection panel
  editor/
    page.tsx                    — 2D Wall Profile Editor (per selected wall)
    components/
      WallCanvas.tsx            — SVG canvas (adapted from wall-profile-editor.jsx)
      SegmentProperties.tsx     — profile shape, fire rating, label
      SplitControls.tsx         — auto-split, grid, snap settings
      StrategySelector.tsx      — noWindows / OSB / SmartPanel toggle
      OpeningEditor.tsx         — window/door placement
  execute/
    page.tsx                    — review all wall configs, submit job, monitor progress
  results/
    page.tsx                    — download result, re-view in viewer
```

### 2.4 Two Modes of Operation

**Mode A: .rvt-aware (existing models)**
1. User uploads `.rvt` → translated to SVF2 → displayed in APS Viewer
2. User clicks walls in 3D → wall IDs + properties extracted via Viewer SDK
3. Wall dimensions auto-populate the 2D editor
4. User configures splits per wall → generates `skill.json`
5. WorkItem runs against the uploaded `.rvt`

**Mode B: Parametric (new designs / no .rvt)**
1. User skips upload, goes directly to editor
2. Enters wall dimensions manually (length, height, panel width)
3. Configures splits, profiles, openings
4. Exports `skill.json` only (for use with local Revit addin or later upload)
5. Optionally uploads `.rvt` later to apply the skill

---

## 3. Backend Architecture

### 3.1 API Routes

All routes under `app/api/aps/`:

```
POST   /api/aps/auth/token         — Get 2-legged access token (cached, server-side only)
POST   /api/aps/upload             — Upload .rvt to OSS bucket, start translation
GET    /api/aps/upload/status/:urn — Poll translation status
GET    /api/aps/viewer/token       — Get viewer-scoped token (short TTL, read-only)
POST   /api/aps/execute            — Upload skill.json, create WorkItem
GET    /api/aps/execute/status/:id — Poll WorkItem status
GET    /api/aps/download/:urn      — Get signed download URL for result .rvt
DELETE /api/aps/model/:urn         — Clean up OSS objects
```

### 3.2 APS Services Used

| Service | Purpose | Billing |
|---------|---------|---------|
| **Authentication** | 2-legged OAuth for server-to-server | Free |
| **Data Management (OSS)** | Store .rvt uploads, skill.json, result .rvt | Free |
| **Model Derivative** | Translate .rvt → SVF2 for viewer | Paid (Flex tokens) |
| **Viewer SDK** | 3D visualization + wall selection in browser | Free |
| **Design Automation v3** | Run Revit engine in cloud (AppBundle + Activity + WorkItem) | Paid (Flex tokens) |

### 3.3 OSS Bucket Strategy

```
Bucket: splitwalls-cloud-{APP_ID}
  ├── uploads/
  │     └── {sessionId}/{timestamp}-{filename}.rvt       (input model)
  ├── skills/
  │     └── {sessionId}/{timestamp}-skill.json            (split configuration)
  └── results/
        └── {sessionId}/{timestamp}-result.rvt            (output model)
```

Objects are auto-expired after 24 hours (configurable) to control storage costs.

### 3.4 Environment Variables

```env
APS_CLIENT_ID=<from developer portal>
APS_CLIENT_SECRET=<from developer portal>
APS_BUCKET_KEY=splitwalls-cloud
DA_NICKNAME=splitwalls
DA_ACTIVITY_ID=SplitWallsActivity
DA_ACTIVITY_ALIAS=prod
DA_ENGINE=Autodesk.Revit+2025
```

---

## 4. Design Automation Engine (C# AppBundle)

### 4.1 Key Constraints

- Must implement `IExternalDBApplication` (not `IExternalCommand` — no UI in DA)
- Cannot use `RevitAPIUI` (no dialogs, no user interaction)
- Runs as a headless console — all input via files, all output via files
- Full access to `Revit DB API` (Document, Wall, Transaction, etc.)

### 4.2 AppBundle Structure

```
SplitWallsDA/
  SplitWallsDA.csproj             (.NET 4.8, x64, Revit 2025 references)
  SplitWallsDA.addin              (DA addin manifest)
  Commands/
    SplitWallsApp.cs              (IExternalDBApplication — entry point)
  Services/
    WallSplitService.cs           (split wall without openings — from existing code)
    WallProfileBuilder.cs         (custom profiles U/T/I/L/Borde — from existing code)
    WindowDetectionService.cs     (find hosted openings — from existing code)
    PanelizationOrchestrator.cs   (OSB / SmartPanel / NoWindows routing)
    SkillReader.cs                (deserialize skill.json → strongly typed model)
  Models/
    SplitSkill.cs                 (root model matching skill.json schema)
    WallConfig.cs                 (per-wall configuration)
    SegmentDef.cs                 (segment definition)
    OpeningDef.cs                 (opening definition)
  Helpers/
    RevitUnitHelper.cs            (mm ↔ feet — from existing code)
    WallJoinHelper.cs             (disable joins — from existing code)
  PackageContents.xml             (DA bundle manifest)
```

### 4.3 Entry Point

```csharp
public class SplitWallsApp : IExternalDBApplication
{
    public ExternalDBApplicationResult OnStartup(ControlledApplication app)
    {
        app.ApplicationInitialized += OnApplicationInitialized;
        return ExternalDBApplicationResult.Succeeded;
    }

    private void OnApplicationInitialized(object sender, EventArgs e)
    {
        // 1. Read skill.json from working directory
        var skill = SkillReader.Load("skill.json");

        // 2. Open the input .rvt
        var app = sender as Application;
        var doc = app.OpenDocumentFile("input.rvt");

        // 3. For each wall config in the skill:
        foreach (var wallConfig in skill.WallConfigs)
        {
            // Find wall by ElementId
            var wall = doc.GetElement(new ElementId(wallConfig.ElementId)) as Wall;
            if (wall == null) continue;

            // Route to appropriate strategy
            var orchestrator = new PanelizationOrchestrator(doc);
            orchestrator.Execute(wall, wallConfig);
        }

        // 4. Save as result.rvt
        doc.SaveAs("result.rvt");
        doc.Close(false);
    }

    public ExternalDBApplicationResult OnShutdown(ControlledApplication app)
    {
        return ExternalDBApplicationResult.Succeeded;
    }
}
```

### 4.4 DA Activity Definition

```json
{
  "id": "SplitWallsActivity",
  "commandLine": [
    "$(engine.path)\\\\revitcoreconsole.exe /al \"$(appbundles[SplitWallsDA].path)\""
  ],
  "parameters": {
    "inputFile": {
      "verb": "get",
      "description": "Input Revit model",
      "localName": "input.rvt",
      "required": true
    },
    "inputSkill": {
      "verb": "get",
      "description": "Split skill configuration JSON",
      "localName": "skill.json",
      "required": true
    },
    "outputFile": {
      "verb": "put",
      "description": "Output Revit model with splits applied",
      "localName": "result.rvt",
      "required": true
    }
  },
  "engine": "Autodesk.Revit+2025",
  "appbundles": ["splitwalls.SplitWallsDA+prod"],
  "description": "Split walls into panels based on skill.json configuration"
}
```

### 4.5 WorkItem Submission

```json
{
  "activityId": "splitwalls.SplitWallsActivity+prod",
  "arguments": {
    "inputFile": {
      "verb": "get",
      "url": "https://developer.api.autodesk.com/oss/v2/buckets/.../objects/input.rvt",
      "headers": { "Authorization": "Bearer {token}" }
    },
    "inputSkill": {
      "verb": "get",
      "url": "data:application/json,{...skill JSON inline...}"
    },
    "outputFile": {
      "verb": "put",
      "url": "https://developer.api.autodesk.com/oss/v2/buckets/.../objects/result.rvt",
      "headers": {
        "Authorization": "Bearer {token}",
        "Content-Type": "application/octet-stream"
      }
    }
  }
}
```

Note: For small skill configs, inline JSON via `data:application/json,{...}` avoids an extra OSS upload. For large configs, upload to OSS first.

---

## 5. Skill JSON Schema (The Contract)

This is the central contract between the frontend editor and the Revit engine. Every field maps directly to existing SplitWalls logic.

```typescript
interface SplitSkill {
  version: "2.0";
  created: string;                    // ISO datetime
  name: string;                       // user-defined name for this skill

  // Global defaults (can be overridden per wall)
  defaults: {
    panelWidthMm: number;             // e.g. 1220
    separatorWidthMm: number;         // e.g. 4
    wallHeightMm: number;             // e.g. 2440
    disableWallJoins: boolean;        // typically true
    snapToGridMm: number | null;      // e.g. 100 or null
  };

  // Per-wall configurations
  wallConfigs: WallConfig[];
}

interface WallConfig {
  // Wall identification (for .rvt-aware mode)
  elementId: number | null;           // Revit ElementId — null for parametric mode
  wallName: string;                   // display name

  // Wall dimensions (auto-populated from .rvt or entered manually)
  lengthMm: number;
  heightMm: number;

  // Strategy selection
  strategy: "noWindows" | "osb" | "smartPanel";

  // Which corner to start splitting from
  startFromCorner: 1 | 2;            // 1 = left/start, 2 = right/end (OtroLado)

  // Split definition
  splitRule: {
    method: "uniform" | "custom";
    panelWidthMm: number;             // for uniform: panel width
    splitPointsMm: number[];          // for custom: sorted array of split positions
  };

  // Segment profiles (one per segment between split points)
  segments: SegmentDef[];

  // Openings (windows/doors on this wall)
  openings: OpeningDef[];
}

interface SegmentDef {
  index: number;
  startMm: number;
  endMm: number;
  widthMm: number;
  profile: "standard" | "U" | "L_left" | "L_right" | "T" | "I" | "borde";
  label: string;
  fireRating: "none" | "1hr" | "2hr" | "3hr";
}

interface OpeningDef {
  index: number;
  xMm: number;                       // horizontal position from wall start
  yMm: number;                       // vertical position from floor (sill height)
  widthMm: number;
  heightMm: number;
  type: "window" | "door";
}
```

### 5.1 Mapping Skill Fields → Existing C# Logic

| Skill Field | C# Code It Maps To |
|-------------|-------------------|
| `strategy: "noWindows"` | `BUTTON_GENERAL` → `Muro_sin_Ventanas = true` path |
| `strategy: "osb"` | `BUTTON_GENERAL` → `Muro_OSB_con_Ventanas = true` path |
| `strategy: "smartPanel"` | `BUTTON_GENERAL` → `Muro_SMART_PANEL_con_Ventanas = true` path |
| `startFromCorner: 2` | Calls `DarVuelta_Muro` before processing (OtroLado) |
| `splitRule.panelWidthMm` | `anchopanel_UI` / `numero_final` in existing code |
| `segments[].profile: "U"` | Routes to `WallProfileBuilder.BuildU_*()` |
| `segments[].profile: "T"` | Routes to `WallProfileBuilder.BuildT_*()` |
| `segments[].profile: "I"` | Routes to `WallProfileBuilder.BuildI_*()` |
| `segments[].profile: "borde"` | Routes to `WallProfileBuilder.BuildEdge*()` |
| `openings[].type: "window"` | Detected via `WindowDetectionService` + mapped to profile builder params |
| `openings[].type: "door"` | Same as window but sill = 0 in profile builder |
| `execution.disableWallJoins` | `WallJoinHelper.DisableJoins()` |
| `execution.separatorWidthMm` | The 4mm gap between panels (`4 / RevitUnitHelper.MmToFeet`) |

---

## 6. Viewer Integration Details

### 6.1 Wall Selection Extension

A custom Viewer extension that:
- Filters the model tree to show only `Walls` category
- Highlights walls on hover
- On click: selects the wall, reads its properties (length, height, type, level)
- Supports multi-select (Ctrl+click)
- Sends selected wall data to Redux via callback

```typescript
// viewer/extensions/WallSelectionExtension.ts
class WallSelectionExtension extends Autodesk.Viewing.Extension {
  onSelectionChanged(event: SelectionEvent) {
    const dbIds = event.dbIdArray;
    // Filter to walls only using property 'Category' === 'Walls'
    // Extract: Element ID, Length, Unconnected Height, Wall Type name, Level
    // Dispatch to wallSlice
  }
}
```

### 6.2 Property Extraction

The Viewer SDK provides `getBulkProperties()` to read Revit parameters:

```typescript
viewer.model.getBulkProperties(dbIds, {
  propFilter: ['Category', 'Length', 'Unconnected Height', 'Type', 'Level', 'Element ID']
}, (results) => {
  // Map to WallMetadata objects
  // Length comes in feet → convert to mm (* 304.8)
});
```

This eliminates the need for a separate "extract" Activity — the Viewer gives us wall metadata for free after Model Derivative translation.

---

## 7. Implementation Roadmap

### Phase 1: Foundation (Week 1-2)
- [ ] Next.js project setup with TypeScript + Redux Toolkit
- [ ] All Redux slices with types (modelSlice, wallSlice, skillSlice, jobSlice)
- [ ] RTK Query API service with mock endpoints
- [ ] Skill JSON schema (TypeScript types + JSON Schema for validation)
- [ ] Basic page routing (upload → viewer → editor → execute → results)

### Phase 2: Profile Editor (Week 2-3)
- [ ] Port `wall-profile-editor.jsx` to TypeScript React component
- [ ] Connect to `skillSlice` (replace local state with Redux)
- [ ] Add strategy selector (noWindows / OSB / SmartPanel)
- [ ] Add per-wall configuration tabs (when multiple walls selected)
- [ ] Skill JSON export/import (save/load configurations)

### Phase 3: APS Integration — Upload + Viewer (Week 3-4)
- [ ] APS OAuth 2-legged auth (server-side token management)
- [ ] OSS bucket creation + .rvt upload endpoint
- [ ] Model Derivative translation trigger + polling
- [ ] APS Viewer integration (embed viewer, load translated model)
- [ ] WallSelectionExtension (click walls → extract properties → Redux)

### Phase 4: APS Design Automation (Week 4-6)
- [ ] Port SplitWalls C# to `IExternalDBApplication` (remove all UI references)
- [ ] Implement `SkillReader` to deserialize skill.json
- [ ] Implement `PanelizationOrchestrator` to route skill configs to existing services
- [ ] Package as AppBundle (.zip with PackageContents.xml)
- [ ] Register AppBundle + Activity on APS DA
- [ ] WorkItem submission endpoint + status polling
- [ ] Result download endpoint

### Phase 5: End-to-End Integration (Week 6-7)
- [ ] Connect frontend execute flow → backend → DA → result
- [ ] Job progress UI with status polling
- [ ] Re-translate result for viewer comparison (before/after)
- [ ] Error handling for DA failures (report logs, retry)

### Phase 6: Polish + Parametric Mode (Week 7-8)
- [ ] Parametric mode (skip upload, manual dimensions, export skill only)
- [ ] Skill library (save/load/share skill configurations)
- [ ] Batch processing (apply same skill to multiple walls)
- [ ] UI polish, error states, loading states

---

## 8. Key Technical Decisions

### 8.1 Why a Single Activity (not Extract + Execute)

- Simpler APS setup (one AppBundle, one Activity)
- Wall metadata comes from the Viewer SDK for free — no need for a separate extraction step
- Reduces APS Flex token consumption (one WorkItem instead of two)
- The Viewer + Model Derivative already provide element IDs, dimensions, and properties

### 8.2 Why Redux Toolkit (not Zustand, Jotai, etc.)

- RTK Query provides built-in caching, polling, and invalidation for API calls
- Slice pattern maps naturally to the domain (model, walls, skill, job)
- DevTools for debugging complex state transitions
- Middleware for side effects (e.g. auto-save skill to localStorage)

### 8.3 Why Reuse Existing C# Services

The refactored SplitWalls codebase already has well-separated services:
- `WallProfileBuilder` — 28 profile builder methods for all shape combinations
- `WallSplitService` — wall splitting with separator gaps
- `WindowDetectionService` — finds hosted openings
- `WallJoinHelper` — disables wall joins
- `RevitUnitHelper` — mm ↔ feet conversion

These can be reused almost directly. The main adaptation is replacing `IExternalCommand` + `UIDocument` with `IExternalDBApplication` + headless `Document` access.

### 8.4 Inline skill.json vs OSS Upload

For skill configs under ~4KB (typical for single-wall configs), use inline `data:application/json,{...}` in the WorkItem arguments. For multi-wall batch configs, upload to OSS first. The backend automatically decides based on payload size.

---

## 9. Security Considerations

- APS credentials (`CLIENT_ID`, `CLIENT_SECRET`) are server-side only — never exposed to frontend
- Viewer tokens are scoped to read-only with short TTL (1 hour)
- OSS objects use signed URLs with expiration for upload/download
- WorkItem callbacks use HTTPS webhook URLs
- `.rvt` files are auto-deleted from OSS after 24 hours
- No user authentication in MVP (add later with NextAuth.js or APS 3-legged OAuth)

---

## 10. Cost Estimation (APS Flex Tokens)

| Operation | Token Cost | Frequency |
|-----------|-----------|-----------|
| Model Derivative (translate .rvt → SVF2) | ~5-20 tokens per model | Once per upload |
| Design Automation WorkItem | ~10 tokens per execution | Once per job |
| OSS Storage | Free (within limits) | Continuous |
| Viewer SDK | Free | Continuous |

For MVP/development: the APS Free Tier includes a monthly allowance sufficient for testing. Production use will require Flex token purchases.

---

## 11. Future Extensions

- **3-legged OAuth**: Let users connect their own ACC/BIM360 projects
- **Webhook callbacks**: Replace polling with push notifications for job completion
- **Skill marketplace**: Users share and rate split configurations
- **Multi-wall batch**: Apply different skills to different walls in one WorkItem
- **Preview mode**: Generate a lightweight 3D preview of splits before committing
- **Undo/versioning**: Keep multiple versions of the modified .rvt
- **Real-time collaboration**: Multiple users configure walls simultaneously
