# SplitWalls Beta MVP — Product & Architecture Plan

**Date:** 2026-03-13
**Updated:** 2026-03-13 (v4 — added 2D Profile Editor + custom splits)
**Status:** Approved — Ready for Implementation
**Mode:** SCOPE REDUCTION (user-facing) + SCOPE EXPANSION (admin workbench)
**Goal:** Ship a working beta demo where users upload a `.rvt` file, configure uniform wall splits, execute via APS Design Automation, and download the modified result. Free to use; users "pay" by sharing on X/LinkedIn. Includes an admin developer workbench for debugging APS DA AppBundles without a Revit desktop license — designed to become a standalone product for AEC cloud developers.

---

## 1. Product Vision (Beta)

### Two Products in One App

```
┌─────────────────────────────────────────────────────────────────┐
│  SPLITWALLS.VERCEL.APP                                          │
│                                                                 │
│  ┌─────────────────────────┐  ┌──────────────────────────────┐ │
│  │  USER-FACING PRODUCT    │  │  ADMIN DA WORKBENCH          │ │
│  │                         │  │  (role: admin only)           │ │
│  │  Upload .rvt            │  │                              │ │
│  │  Configure splits       │  │  Upload .zip AppBundle       │ │
│  │  Execute                │  │  Register on APS DA          │ │
│  │  Share → Download       │  │  Submit test WorkItems       │ │
│  │                         │  │  View logs + console output  │ │
│  │  Audience: Architects   │  │  Before/After 3D Viewer      │ │
│  │  & builders             │  │  Job history + diff          │ │
│  │                         │  │                              │ │
│  │                         │  │  Audience: APS DA developers │ │
│  └─────────────────────────┘  └──────────────────────────────┘ │
└─────────────────────────────────────────────────────────────────┘
```

### User-Facing Core Loop

```
Sign up (Clerk) → Upload .rvt → View model in 3D → Configure splits → Execute → View result in 3D (verify) → Share on X/LinkedIn → Download result.rvt
```

### Admin Workbench Core Loop

```
Sign in (admin role) → Upload .zip bundle → Register AppBundle + Activity → Upload test .rvt → Submit WorkItem → View logs → View before/after in 3D Viewer → Iterate
```

### Value Proposition

**For end users:** Upload a Revit file, define wall panel split rules, get back a modified `.rvt` — no Revit desktop needed, works in the browser.

**For developers (future product):** Debug your APS Design Automation AppBundles without a Revit desktop license. Upload bundles, run tests, view structured logs, see 3D results — all in the browser.

### Business Model (Beta)

- **Free to use** — zero cost to the user
- **Social payment** — user shares a result-focused post on X or LinkedIn before downloading
- **Intent-based verification** — share dialog opens with pre-filled text; on return/close, download unlocks
- **Pre-filled message template:**
  > "Just split [X] wall panels in my Revit model in under a minute with SplitWalls — free tool for architects & builders. Try it: splitwalls.vercel.app"

### Target Platforms for Growth

| Platform | Why | Priority |
|----------|-----|----------|
| **X (Twitter)** | Tech-savvy architects, BIM managers, Revit community | P1 |
| **LinkedIn** | Firms, decision makers, AEC professionals | P1 |
| **YouTube** | Tutorials — how AEC people discover tools | P2 (post-beta) |

---

## 2. Tech Stack

| Layer | Technology | Cost |
|-------|-----------|------|
| **Frontend + API** | Next.js 14+ (App Router, TypeScript) | Free (Vercel) |
| **Auth** | Clerk (Google + Microsoft + email) | Free (10K MAU) |
| **Database** | MongoDB Atlas + Prisma ORM | Free tier |
| **Hosting** | Vercel (splitwalls.vercel.app) | Free tier |
| **Styling** | Tailwind CSS + shadcn/ui | Free |
| **3D Viewer** | APS Viewer SDK v7 | Free |
| **Model Translation** | APS Model Derivative (SVF2) | Flex tokens |
| **Cloud Engine** | APS Design Automation v3 | Flex tokens |
| **File Storage** | APS OSS (24hr auto-expiry) | Free |
| **Revit Engines** | 2022, 2023, 2024 (3 AppBundles) | — |

### Future Migration Path

Next.js API routes (Vercel) → NestJS API (Azure) when scaling requires it. REST + JSON contract stays identical.

---

## 3. System Architecture

```
┌──────────────────────────────────────────────────────────────────────┐
│                    FRONTEND (Next.js on Vercel)                       │
│                                                                      │
│  PUBLIC ROUTES                          ADMIN ROUTES (role: admin)    │
│  ─────────────                          ─────────────────────────     │
│  /                 Landing page         /admin                       │
│  /sign-in          Clerk auth           /admin/bundles    Bundle Mgr │
│  /sign-up          Clerk auth           /admin/test       Test Runner│
│  /dashboard        Upload + history     /admin/logs/:id   Log Viewer │
│  /viewer/:id       3D model viewer      /admin/viewer/:id 3D Viewer  │
│  /configure/:id    Split config         /admin/history    Job History│
│  /status/:id       Job progress                                      │
│  /result/:id       3D result + download                              │
│                                                                      │
└──────────┬───────────────────┬───────────────────────────────────────┘
           │                   │
           ▼                   ▼
┌──────────────┐    ┌──────────────────────────────────────────────────┐
│   Clerk      │    │  Next.js API Routes (/api/...)                   │
│   - Google   │    │                                                  │
│   - Microsoft│    │  USER ROUTES:                                    │
│   - Email    │    │  POST /api/upload           Upload .rvt          │
│              │    │  POST /api/execute           Create WorkItem     │
│   Metadata:  │    │  GET  /api/status/:id        Poll job           │
│   { role }   │    │  GET  /api/download/:id      Signed URL         │
│              │    │  POST /api/social/verify      Mark shared        │
│              │    │  GET  /api/viewer/token        Viewer token      │
│              │    │  POST /api/viewer/translate    .rvt → SVF2       │
│              │    │  GET  /api/viewer/status/:urn  Transl. status    │
└──────────────┘    │                                                  │
                    │  ADMIN ROUTES (role check middleware):            │
                    │  POST /api/admin/bundles          Upload .zip    │
                    │  POST /api/admin/bundles/register  Register DA   │
                    │  GET  /api/admin/bundles           List bundles  │
                    │  POST /api/admin/activities        Create/update │
                    │  POST /api/admin/test              Submit WorkItem│
                    │  GET  /api/admin/test/:id          Poll + report │
                    │  GET  /api/admin/test/:id/logs     Parsed logs   │
                    │  POST /api/admin/viewer/translate  Trigger transl│
                    │  GET  /api/admin/viewer/token      Viewer token  │
                    │  GET  /api/admin/viewer/status/:urn  Transl status│
                    │  GET  /api/admin/history           All test jobs │
                    │                                                  │
                    │  SHARED (internal):                               │
                    │  POST /api/internal/aps/auth       2-legged OAuth│
                    │  POST /api/internal/aps/oss        OSS operations│
                    └──────────┬───────────────────────────────────────┘
                               │
                    ┌──────────▼───────────────────────────────────────┐
                    │  APS Cloud                                       │
                    │                                                  │
                    │  OAuth 2-legged ─→ Access Token (cached)         │
                    │  OSS ─→ Store .rvt, .zip bundles, skill.json     │
                    │  Design Automation:                               │
                    │    - AppBundle registration                       │
                    │    - Activity creation                            │
                    │    - WorkItem submission                          │
                    │    - Report/log retrieval                         │
                    │  Model Derivative ─→ Translate .rvt → SVF2       │
                    │  Viewer SDK ─→ 3D visualization in browser       │
                    └──────────────────────────────────────────────────┘
                               │
                    ┌──────────▼───────────────────────────────────────┐
                    │  MongoDB Atlas                                    │
                    │                                                  │
                    │  User       { clerkId, email, role, createdAt }  │
                    │  Job        { userId, status, revitVersion,      │
                    │               ossInputUrn, ossResultUrn, config, │
                    │               panelCount, createdAt }            │
                    │  Share      { userId, jobId, platform, sharedAt }│
                    │  Bundle     { name, version, engine, ossUrn,     │
                    │               daAppBundleId, status, createdAt } │
                    │  Activity   { name, bundleId, engine, daId,      │
                    │               parameters, createdAt }            │
                    │  TestJob    { bundleId, activityId, workItemId,  │
                    │               status, inputUrn, resultUrn,       │
                    │               inputViewableUrn, resultViewableUrn│
                    │               reportCached, config, duration,    │
                    │               createdAt }                        │
                    │  ViewCache  { rvtFileHash, ossUrn, viewableUrn,  │
                    │               translationStatus, createdAt }     │
                    └──────────────────────────────────────────────────┘
```

---

## 4. User Flow — End User (Step by Step)

```
1. LAND           User visits splitwalls.vercel.app
                  Sees: hero section, 3-step explainer, "Get Started" CTA
                        │
2. SIGN UP        Clerk modal (Google / Microsoft / email)
                  Creates User record in MongoDB (role: "user")
                        │
3. DASHBOARD      User sees: upload area + past jobs list
                  Drags .rvt file → selects Revit version (2022/2023/2024)
                        │
4. UPLOAD         Frontend sends file to POST /api/upload
                  Backend: uploads to APS OSS bucket
                  Triggers Model Derivative translation (hash-cached)
                  Returns: ossUrn + jobId
                        │
5. VIEW MODEL     /viewer/:jobId
                  ┌────────────────────────────────────────────┐
                  │  "Your model is being prepared..."         │
                  │  [████████░░░░░░] 60% translating          │
                  │                                            │
                  │  ┌──────────────────────────────────────┐  │
                  │  │                                      │  │
                  │  │         [APS Viewer]                  │  │
                  │  │                                      │  │
                  │  │    3D view of uploaded .rvt           │  │
                  │  │    User confirms: "yes, right file"  │  │
                  │  │                                      │  │
                  │  └──────────────────────────────────────┘  │
                  │                                            │
                  │  Model info: 24 walls · 3 levels           │
                  │                                            │
                  │  [Continue to Configure →]                 │
                  └────────────────────────────────────────────┘
                  Translation uses ViewCache (hash-based)
                  If same .rvt uploaded before → instant load
                        │
6. CONFIGURE      /configure/:jobId
                  ┌────────────────────────────────────────────┐
                  │  2D PROFILE EDITOR (SVG + React)           │
                  │                                            │
                  │  Wall Length: [5200] mm  Height: [2440] mm │
                  │                                            │
                  │   1220mm   1220mm   1220mm   1540mm        │
                  │  ┌───────┬───────┬───────┬────────┐        │
                  │  │       │       │       │        │        │
                  │  │  std  │  std  │  std  │  std   │ 2440mm │
                  │  │       │       │       │        │        │
                  │  └───────┴───────┴───────┴────────┘        │
                  │            5200mm total                    │
                  │                                            │
                  │  [+ Add Split] [Reset Uniform] [Snap:100mm]│
                  │  Profile: ■ std ■ U ■ L ■ T ■ I ■ borde  │
                  │                                            │
                  │  Draggable split lines:                    │
                  │    - Drag any split point left/right       │
                  │    - Auto-switches to "custom" mode        │
                  │    - Snap-to-grid optional (100mm)         │
                  │    - Min segment width: 100mm              │
                  │    - Max 100 segments                      │
                  └────────────────────────────────────────────┘

                  Form controls below canvas:
                  - Panel width (mm) [default: 1220] → recalculates uniform splits
                  - Separator width (mm) [default: 4]
                  - Disable wall joins [default: checked]
                  - Apply to: "All walls" (MVP — no per-wall selection)

                  Canvas tech: SVG + React (DOM-based hit testing, Tailwind colors)
                  Hook: useProfileCalculator(wallLength, panelWidth, separatorWidth)
                    → Returns SegmentDef[] for rendering
                    → On drag: method switches "uniform" → "custom"
                    → Generates splitPointsMm[] for skill.json

                  User clicks "Execute"
                        │
7. EXECUTE        Frontend sends config to POST /api/execute
                  Backend:
                    a) Builds skill.json from form values
                    b) Selects Activity alias matching chosen Revit version
                    c) Creates WorkItem (input: .rvt + skill.json)
                    d) Returns workItemId
                        │
8. WAIT           Frontend polls GET /api/status/:jobId every 5s
                  Shows: progress bar + status text
                  DA states: pending → inProgress → success | failed
                        │
9. RESULT         /result/:jobId (on success)
   VIEWER         ┌────────────────────────────────────────────┐
                  │  "Your walls have been split!"             │
                  │  96 panels created from 24 walls            │
                  │                                            │
                  │  ┌─ BEFORE ────────┐ ┌─ AFTER ──────────┐ │
                  │  │                 │ │                   │ │
                  │  │  [APS Viewer]   │ │  [APS Viewer]     │ │
                  │  │                 │ │                   │ │
                  │  │  Original model │ │  Split walls      │ │
                  │  │  24 walls       │ │  96 panels        │ │
                  │  │                 │ │                   │ │
                  │  └─────────────────┘ └───────────────────┘ │
                  │                                            │
                  │  [Sync cameras]                            │
                  │                                            │
                  │  [Share to Download ↓]                     │
                  └────────────────────────────────────────────┘
                  Result .rvt translated on success (auto)
                  Split-screen before/after (reuses admin component)
                        │
10. SOCIAL GATE   "Share to download your result:"
                  [Share on X]  [Share on LinkedIn]
                  Click opens new window with pre-filled post
                  On window close/return → POST /api/social/verify
                  Download button unlocks
                        │
11. DOWNLOAD      GET /api/download/:jobId → signed OSS URL
                  Browser downloads result.rvt
```

---

## 5. User Flow — Admin DA Workbench

```
1. SIGN IN        Admin signs in (Clerk — role: "admin" in metadata)
                  Redirected to /admin
                        │
2. BUNDLE         /admin/bundles
   MANAGER        ┌────────────────────────────────────────────┐
                  │  [Upload New Bundle]                       │
                  │                                            │
                  │  Bundle: SplitWallsDA_2024.zip             │
                  │  Engine: Autodesk.Revit+2024               │
                  │  Version: 3  │  Status: Registered         │
                  │  Uploaded: 2026-03-13 14:30                │
                  │  [Re-register] [Download] [Delete]         │
                  │                                            │
                  │  Bundle: SplitWallsDA_2023.zip             │
                  │  Engine: Autodesk.Revit+2023               │
                  │  Version: 1  │  Status: Registered         │
                  │  ...                                       │
                  └────────────────────────────────────────────┘
                        │
                  Upload .zip → POST /api/admin/bundles
                    a) Upload .zip to APS OSS
                    b) Register/update AppBundle on APS DA
                    c) Create Activity if not exists
                    d) Store Bundle + Activity records in MongoDB
                        │
3. TEST           /admin/test
   RUNNER         ┌────────────────────────────────────────────┐
                  │  Select Bundle:  [SplitWallsDA_2024 v3 ▼] │
                  │  Select Activity: [SplitWalls2024    ▼]    │
                  │                                            │
                  │  Input .rvt:  [Upload file] or [Recent ▼]  │
                  │  Input JSON:  ┌─────────────────────────┐  │
                  │               │ {                       │  │
                  │               │   "version": "2.0",     │  │
                  │               │   "defaults": {         │  │
                  │               │     "panelWidthMm": 1220│  │
                  │               │   },                    │  │
                  │               │   ...                    │  │
                  │               │ }                       │  │
                  │               └─────────────────────────┘  │
                  │  [JSON editor with syntax highlighting]    │
                  │                                            │
                  │  [▶ Run Test]                              │
                  └────────────────────────────────────────────┘
                        │
                  Run Test → POST /api/admin/test
                    a) Upload input .rvt to OSS (or reuse existing)
                    b) Upload input JSON as skill.json to OSS
                    c) Create WorkItem with selected Activity
                    d) Create TestJob record
                    e) Start polling
                        │
4. LOG            /admin/logs/:testJobId
   VIEWER         ┌────────────────────────────────────────────┐
                  │  TestJob #42  │  Status: ● SUCCESS         │
                  │  Bundle: SplitWallsDA_2024 v3              │
                  │  Duration: 34.2s                           │
                  │  ─────────────────────────────────────────  │
                  │                                            │
                  │  ┌─ Console Output ──────────────────────┐ │
                  │  │ [00:00.0] Starting SplitWallsApp...   │ │
                  │  │ [00:01.2] Loaded skill.json (v2.0)    │ │
                  │  │ [00:01.3] Opening input.rvt...        │ │
                  │  │ [00:05.8] Found 24 walls              │ │
                  │  │ [00:06.1] Splitting wall #1 (5.2m)... │ │
                  │  │ [00:06.3]   → 4 panels created        │ │
                  │  │ [00:06.5] Splitting wall #2 (3.1m)... │ │
                  │  │ ...                                   │ │
                  │  │ [00:28.4] Total: 96 panels created    │ │
                  │  │ [00:28.5] Saving result.rvt...        │ │
                  │  │ [00:34.2] Done.                       │ │
                  │  └───────────────────────────────────────┘ │
                  │                                            │
                  │  ┌─ Input JSON ───────┐ ┌─ Errors ──────┐ │
                  │  │ { "version": "2.0" │ │ (none)        │ │
                  │  │   ...              │ │               │ │
                  │  │ }                  │ │               │ │
                  │  └────────────────────┘ └───────────────┘ │
                  │                                            │
                  │  [View in 3D]  [Re-run]  [Re-run with edit]│
                  └────────────────────────────────────────────┘
                        │
                  Report fetched from APS DA → parsed into structured sections
                  Cached in MongoDB (reports expire on APS after ~2 weeks)
                        │
5. 3D VIEWER      /admin/viewer/:testJobId
   (BEFORE/AFTER) ┌────────────────────────────────────────────┐
                  │  ┌─ INPUT .rvt ────┐ ┌─ RESULT .rvt ────┐ │
                  │  │                 │ │                   │ │
                  │  │   [APS Viewer]  │ │   [APS Viewer]    │ │
                  │  │                 │ │                   │ │
                  │  │   24 walls      │ │   24 walls →      │ │
                  │  │   (original)    │ │   96 panels       │ │
                  │  │                 │ │                   │ │
                  │  └─────────────────┘ └───────────────────┘ │
                  │                                            │
                  │  [Sync cameras]  [Reset view]              │
                  │                                            │
                  │  Translation Status:                       │
                  │  Input:  ✅ Cached (hash: abc123)          │
                  │  Result: ✅ Translated (34s ago)           │
                  └────────────────────────────────────────────┘
                        │
                  Smart caching:
                    - Hash input .rvt → check ViewCache
                    - If cached viewable exists → load immediately
                    - If not → translate via Model Derivative → cache
                    - Result .rvt → translate only on "View in 3D" click
                        │
6. JOB            /admin/history
   HISTORY        ┌────────────────────────────────────────────┐
                  │  Filter: [All ▼] [Success ▼] [2024 ▼]     │
                  │  Search: [________________]                │
                  │                                            │
                  │  #42 │ ● SUCCESS │ v3 │ 2024 │ 34.2s │ 5m │
                  │  #41 │ ✕ FAILED  │ v3 │ 2024 │ 12.1s │ 8m │
                  │  #40 │ ● SUCCESS │ v2 │ 2024 │ 31.0s │ 1h │
                  │  #39 │ ● SUCCESS │ v2 │ 2023 │ 38.5s │ 1h │
                  │  ...                                       │
                  │                                            │
                  │  [Compare #42 vs #41]                      │
                  └────────────────────────────────────────────┘
                        │
                  Compare view shows:
                    - JSON diff of inputs (what changed?)
                    - Status diff (what fixed it?)
                    - Duration diff (faster or slower?)
```

---

## 6. API Routes — Complete

### User Routes

```
POST   /api/upload
  Auth: Clerk (userId from session)
  Body: multipart/form-data { file: .rvt, revitVersion: "2022"|"2023"|"2024" }
  Action: Upload to APS OSS, create Job record (status: uploaded)
  Returns: { jobId, ossUrn }

POST   /api/execute
  Auth: Clerk
  Body: { jobId, config: { panelWidthMm, separatorWidthMm, disableWallJoins } }
  Action:
    1. Build skill.json (strategy: "noWindows", method: "uniform", all walls)
    2. Select DA Activity alias by revitVersion
    3. Create WorkItem on APS DA
    4. Update Job record (status: executing, workItemId)
  Returns: { jobId, workItemId }

GET    /api/status/:jobId
  Auth: Clerk
  Action: Poll APS DA WorkItem status, update Job record
  Returns: { status, progress, reportUrl? }

POST   /api/social/verify
  Auth: Clerk
  Body: { jobId, platform: "x"|"linkedin" }
  Action: Create Share record, mark job as downloadable
  Returns: { downloadReady: true }

GET    /api/download/:jobId
  Auth: Clerk
  Action: Verify Share exists for this job, generate signed OSS URL
  Returns: { downloadUrl } (or 403 if not shared)

GET    /api/viewer/token
  Auth: Clerk
  Action: Generate short-lived Viewer-scoped token (read-only, 1hr TTL)
  Returns: { accessToken, expiresIn }

POST   /api/viewer/translate
  Auth: Clerk
  Body: { ossUrn, fileHash? }
  Action:
    1. Check ViewCache by fileHash (if provided)
    2. If cached → return existing viewableUrn
    3. Else → trigger Model Derivative translation
    4. Create/update ViewCache record
  Returns: { viewableUrn?, status: "cached"|"translating", translationId? }

GET    /api/viewer/status/:urn
  Auth: Clerk
  Action: Poll Model Derivative translation status
  Returns: { status, progress, viewableUrn? }
```

### Admin Routes (role: admin middleware)

```
POST   /api/admin/bundles
  Auth: Clerk (admin role)
  Body: multipart/form-data { file: .zip, engine: "Autodesk.Revit+2024", name: "SplitWallsDA" }
  Action:
    1. Upload .zip to APS OSS
    2. POST to DA /appbundles to register (or update version)
    3. Create/update Bundle record
  Returns: { bundleId, daAppBundleId, version }

GET    /api/admin/bundles
  Auth: Clerk (admin role)
  Returns: { bundles: Bundle[] }

POST   /api/admin/activities
  Auth: Clerk (admin role)
  Body: { bundleId, name, engine, parameters: {...} }
  Action:
    1. POST to DA /activities to create
    2. POST to DA /activities/:id/aliases to create "test" alias
    3. Create Activity record
  Returns: { activityId, daActivityId }

POST   /api/admin/test
  Auth: Clerk (admin role)
  Body: multipart/form-data { activityId, rvtFile?: File, rvtUrn?: string, inputJson: string }
  Action:
    1. Upload .rvt to OSS (or reuse rvtUrn)
    2. Upload inputJson as skill.json to OSS (inline if <4KB)
    3. Create WorkItem with selected Activity
    4. Create TestJob record (status: pending)
  Returns: { testJobId, workItemId }

GET    /api/admin/test/:testJobId
  Auth: Clerk (admin role)
  Action: Poll DA WorkItem status, update TestJob record
  Returns: { status, progress, duration, reportUrl? }

GET    /api/admin/test/:testJobId/logs
  Auth: Clerk (admin role)
  Action:
    1. If reportCached → return from MongoDB
    2. Else fetch from DA report URL → parse → cache → return
  Returns: { lines: ParsedLogLine[], errors: string[], duration: number }

  # NOTE: Admin viewer uses shared /api/viewer/* routes (Decision #12)
  # No separate /api/admin/viewer/* needed

GET    /api/admin/history
  Auth: Clerk (admin role)
  Query: ?status=failed&engine=2024&page=1&limit=20
  Returns: { testJobs: TestJob[], total, page }
```

---

## 7. Skill JSON Schema (MVP Subset)

The MVP uses a simplified version of the full skill.json contract — only uniform splitting, noWindows strategy, standard profile, applied to ALL walls.

```typescript
// MVP skill.json — what the backend generates from the config form
interface MvpSkill {
  version: "2.0";
  created: string;
  name: string;

  defaults: {
    panelWidthMm: number;       // from form (default 1220)
    separatorWidthMm: number;   // from form (default 4)
    disableWallJoins: boolean;  // from form (default true)
  };

  // MVP: empty array = "apply to ALL walls in the model"
  // The AppBundle interprets empty wallConfigs as "process every Wall element"
  wallConfigs: [];

  // MVP global config (applies to all walls)
  globalConfig: {
    strategy: "noWindows";
    splitRule: {
      method: "uniform" | "custom";
      panelWidthMm: number;       // used when method = "uniform"
      splitPointsMm?: number[];   // used when method = "custom" (drag points)
    };
  };
}
```

### Why `wallConfigs: []` for MVP

Selecting specific walls requires either:
- APS Viewer integration (deferred for user-facing; available in admin) to pick walls by clicking in 3D
- User knowing Revit ElementIds (unrealistic)

Instead, the MVP AppBundle collects ALL `Wall` elements from the document and applies the same uniform split to each. This is the simplest useful behavior: "split every wall in my model into 1220mm panels."

---

## 8. C# AppBundle (MVP)

### 8.1 Structure — 3 Bundles

```
SplitWallsDA/
  SplitWallsDA.csproj          (.NET 4.8, multi-target via build config)
  SplitWallsDA.addin
  PackageContents.xml

  App/
    SplitWallsApp.cs           (IExternalDBApplication entry point)

  Services/
    SkillReader.cs             (deserialize skill.json)
    UniformSplitService.cs     (split wall into equal panels — extracted from ThisApplication.cs)

  Helpers/
    RevitUnitHelper.cs         (mm ↔ feet — reuse from existing)
    WallJoinHelper.cs          (disable joins — reuse from existing)

  Models/
    MvpSkill.cs                (C# model matching MVP skill.json)
```

Build produces 3 zip bundles:
- `SplitWallsDA_2022.zip` (references Revit 2022 SDK)
- `SplitWallsDA_2023.zip` (references Revit 2023 SDK)
- `SplitWallsDA_2024.zip` (references Revit 2024 SDK)

### 8.2 Entry Point (MVP)

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
        var app = sender as Application;

        // 1. Read skill.json
        var skill = SkillReader.Load("skill.json");

        // 2. Open input model
        var doc = app.OpenDocumentFile("input.rvt");

        // 3. Collect ALL walls in the document
        var collector = new FilteredElementCollector(doc);
        var walls = collector.OfClass(typeof(Wall)).Cast<Wall>().ToList();

        // 4. Split each wall uniformly
        var splitter = new UniformSplitService(doc);
        int panelCount = 0;

        using (var tx = new Transaction(doc, "SplitWalls"))
        {
            tx.Start();

            foreach (var wall in walls)
            {
                int count = splitter.SplitWall(
                    wall,
                    skill.Defaults.PanelWidthMm,
                    skill.Defaults.SeparatorWidthMm,
                    skill.Defaults.DisableWallJoins
                );
                panelCount += count;
            }

            tx.Commit();
        }

        // 5. Write panel count to report (for social share message)
        File.WriteAllText("report.txt", panelCount.ToString());

        // 6. Save result
        doc.SaveAs("result.rvt");
        doc.Close(false);
    }

    public ExternalDBApplicationResult OnShutdown(ControlledApplication app)
        => ExternalDBApplicationResult.Succeeded;
}
```

### 8.3 UniformSplitService — What to Extract

The core wall-splitting logic lives in `ThisApplication.cs` inside the `BUTTON_GENERAL()` function, specifically the `Muro_sin_Ventanas = true` path. This path:

1. Gets wall location curve (start/end points)
2. Calculates number of panels: `wallLength / panelWidth`
3. For each split point: creates a new wall segment using `Wall.Create()` or splits using `ElementTransformUtils.SplitElement()`
4. Optionally disables wall joins via `WallJoinHelper`
5. Adds separator gaps (4mm)

The `UniformSplitService` extracts this exact logic, parameterized by `panelWidthMm` and `separatorWidthMm`, with NO UI dependencies.

Supports two modes:
- **uniform:** `SplitWallUniform(wall, panelWidthMm, separatorWidthMm)` — original behavior
- **custom:** `SplitWallAt(wall, splitPointsMm[])` — split at explicit positions (from 2D editor drag)

```csharp
// In OnApplicationInitialized, after collecting walls:
if (skill.GlobalConfig.SplitRule.Method == "custom"
    && skill.GlobalConfig.SplitRule.SplitPointsMm?.Count > 0)
{
    panelCount += splitter.SplitWallAt(wall, skill.GlobalConfig.SplitRule.SplitPointsMm);
}
else
{
    panelCount += splitter.SplitWallUniform(wall,
        skill.Defaults.PanelWidthMm, skill.Defaults.SeparatorWidthMm);
}
```

### 8.4 DA Registration (3 Activities)

```
AppBundles:
  splitwalls.SplitWallsDA2022+prod  → engine: Autodesk.Revit+2022
  splitwalls.SplitWallsDA2023+prod  → engine: Autodesk.Revit+2023
  splitwalls.SplitWallsDA2024+prod  → engine: Autodesk.Revit+2024

Activities:
  splitwalls.SplitWalls2022+prod    → uses SplitWallsDA2022 bundle
  splitwalls.SplitWalls2023+prod    → uses SplitWallsDA2023 bundle
  splitwalls.SplitWalls2024+prod    → uses SplitWallsDA2024 bundle

Activity Parameters (same for all):
  inputFile:  { verb: "get", localName: "input.rvt" }
  inputSkill: { verb: "get", localName: "skill.json" }
  outputFile: { verb: "put", localName: "result.rvt" }
  outputReport: { verb: "put", localName: "report.txt" }
```

Backend maps user's version selection → correct Activity alias.

---

## 9. MongoDB Schema (Prisma)

```prisma
datasource db {
  provider = "mongodb"
  url      = env("DATABASE_URL")
}

generator client {
  provider = "prisma-client-js"
}

// ─── Shared ───────────────────────────────────────────

model User {
  id        String   @id @default(auto()) @map("_id") @db.ObjectId
  clerkId   String   @unique
  email     String
  name      String?
  role      String   @default("user") // "user" | "admin"
  createdAt DateTime @default(now())
  jobs      Job[]
  shares    Share[]
}

// ─── User-facing ──────────────────────────────────────

model Job {
  id            String   @id @default(auto()) @map("_id") @db.ObjectId
  userId        String   @db.ObjectId
  user          User     @relation(fields: [userId], references: [id])
  status        String   // uploaded | executing | success | failed
  revitVersion  String   // "2022" | "2023" | "2024"
  ossInputUrn   String
  ossResultUrn  String?
  workItemId    String?
  panelCount    Int?
  config        Json     // { panelWidthMm, separatorWidthMm, disableWallJoins }
  reportUrl     String?
  errorMessage  String?
  createdAt     DateTime @default(now())
  updatedAt     DateTime @updatedAt
  shares        Share[]
}

model Share {
  id        String   @id @default(auto()) @map("_id") @db.ObjectId
  userId    String   @db.ObjectId
  user      User     @relation(fields: [userId], references: [id])
  jobId     String   @db.ObjectId
  job       Job      @relation(fields: [jobId], references: [id])
  platform  String   // "x" | "linkedin"
  sharedAt  DateTime @default(now())
}

// ─── Admin DA Workbench ───────────────────────────────

model Bundle {
  id             String     @id @default(auto()) @map("_id") @db.ObjectId
  name           String     // "SplitWallsDA"
  engine         String     // "Autodesk.Revit+2024"
  version        Int        @default(1)
  ossUrn         String     // where the .zip lives in OSS
  daAppBundleId  String?    // APS DA registered ID
  daAlias        String?    // "prod" or "test"
  status         String     // pending | registered | failed
  fileName       String     // original filename
  fileSizeBytes  Int
  createdAt      DateTime   @default(now())
  updatedAt      DateTime   @updatedAt
  activities     Activity[]
  testJobs       TestJob[]
}

model Activity {
  id             String    @id @default(auto()) @map("_id") @db.ObjectId
  name           String    // "SplitWalls2024"
  bundleId       String    @db.ObjectId
  bundle         Bundle    @relation(fields: [bundleId], references: [id])
  engine         String    // "Autodesk.Revit+2024"
  daActivityId   String?   // APS DA registered ID
  daAlias        String?   // "test"
  parameters     Json      // DA activity parameter definitions
  status         String    // pending | registered | failed
  createdAt      DateTime  @default(now())
  updatedAt      DateTime  @updatedAt
  testJobs       TestJob[]
}

model TestJob {
  id                  String   @id @default(auto()) @map("_id") @db.ObjectId
  bundleId            String   @db.ObjectId
  bundle              Bundle   @relation(fields: [bundleId], references: [id])
  activityId          String   @db.ObjectId
  activity            Activity @relation(fields: [activityId], references: [id])
  workItemId          String?
  status              String   // pending | inProgress | success | failed | cancelled
  ossInputRvtUrn      String
  ossInputJsonUrn     String?
  ossResultUrn        String?
  inputConfig         Json     // the skill.json content (for display + re-run)
  reportCached        String?  // cached report text (DA reports expire)
  reportParsed        Json?    // structured parsed log lines
  errorMessage        String?
  durationSeconds     Float?
  inputFileHash       String?  // SHA-256 of input .rvt for ViewCache lookup
  inputViewableUrn    String?  // SVF2 URN for input .rvt (from ViewCache)
  resultViewableUrn   String?  // SVF2 URN for result .rvt
  createdAt           DateTime @default(now())
  updatedAt           DateTime @updatedAt
}

model ViewCache {
  id                String   @id @default(auto()) @map("_id") @db.ObjectId
  rvtFileHash       String   @unique // SHA-256 of the .rvt file
  ossUrn            String   // where the .rvt lives in OSS
  viewableUrn       String?  // SVF2 URN after translation
  translationStatus String   // pending | inProgress | success | failed
  translationId     String?  // Model Derivative job ID
  fileSizeBytes     Int?
  createdAt         DateTime @default(now())
  updatedAt         DateTime @updatedAt
}
```

---

## 10. Admin Workbench — DA Report Parser

APS Design Automation reports are raw text dumps. The parser extracts structure:

```typescript
// lib/da-report-parser.ts

interface ParsedLogLine {
  timestamp: string;      // "[00:06.1]" or raw line number
  level: "info" | "warn" | "error" | "system";
  source: "revit" | "appbundle" | "da-engine";
  message: string;
  raw: string;
}

interface ParsedReport {
  lines: ParsedLogLine[];
  errors: ParsedLogLine[];
  warnings: ParsedLogLine[];
  summary: {
    totalDuration: number;    // seconds
    startupTime: number;      // time to ApplicationInitialized
    processingTime: number;   // time in user code
    wallCount: number | null; // parsed from output if available
    panelCount: number | null;
    exitCode: string;
  };
}

// Parser rules:
// - Lines starting with "Error" or containing exception names → level: error
// - Lines containing "warning" → level: warn
// - Lines from Revit engine (pre-ApplicationInitialized) → source: da-engine
// - Lines from user Console.Write* calls → source: appbundle
// - Timestamps extracted from "[HH:MM:SS.mmm]" patterns
```

---

## 11. Admin Workbench — Viewer Integration

### Smart Translation Caching

```
Developer uploads test.rvt
        │
        ▼
  Hash file (SHA-256, client-side)
        │
        ▼
  Check ViewCache by hash
        │
   ┌────┴────┐
   │ CACHED  │ NOT CACHED
   │         │
   ▼         ▼
  Load     Translate via
  existing  Model Derivative
  viewable   │
   │         ▼
   │       Wait (30-120s)
   │         │
   │         ▼
   │       Store in ViewCache
   │         │
   └────┬────┘
        │
        ▼
  Load APS Viewer with viewableUrn
```

### Split-Screen Viewer Component

```typescript
// components/admin/SplitViewer.tsx

interface SplitViewerProps {
  inputViewableUrn: string;
  resultViewableUrn: string;
  accessToken: string;
}

// Two <div> containers side by side
// Each initializes its own Autodesk.Viewing.GuiViewer3D instance
// "Sync cameras" button: copies camera position/target from left → right
// Both viewers share the same access token
```

---

## 12. Landing Page Structure

```
/  (landing page — public, no auth required)

┌─────────────────────────────────────────────┐
│  HERO                                       │
│  "Split Revit Walls in Seconds"             │
│  "Upload your .rvt → configure panels →     │
│   download the result. No Revit needed."    │
│                                             │
│  [Get Started — Free]                       │
└─────────────────────────────────────────────┘

┌──────────────────────────────────────────────────────────────┐
│  4-STEP EXPLAINER                                            │
│                                                              │
│  1. Upload      2. View 3D     3. Design       4. Verify     │
│  Drop your      See your       Draw splits     See before/   │
│  .rvt file      model in 3D    on 2D canvas    after in 3D   │
│  [icon]         [icon]         [icon]          [icon]        │
└──────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────┐
│  SUPPORTED VERSIONS                         │
│  Revit 2022 · 2023 · 2024                   │
└─────────────────────────────────────────────┘

┌─────────────────────────────────────────────┐
│  SOCIAL PROOF / TESTIMONIALS                │
│  (empty at launch — fills from Share posts) │
└─────────────────────────────────────────────┘

┌─────────────────────────────────────────────┐
│  FOOTER                                     │
│  Built by [your name] · X · LinkedIn        │
└─────────────────────────────────────────────┘
```

---

## 13. Social Gate Implementation

### Share Dialog Flow

```
User clicks [Share on X]
  │
  ▼
window.open("https://x.com/intent/tweet?text=...&url=...")
  │                                              │
  ▼                                              ▼
User posts (or doesn't)                    Window closes / user returns
  │
  ▼
Frontend detects window closed (polling window.closed)
  │
  ▼
POST /api/social/verify { jobId, platform: "x" }
  │
  ▼
Backend creates Share record, returns { downloadReady: true }
  │
  ▼
Download button enables → GET /api/download/:jobId
```

### Pre-filled Messages

**X (Twitter):**
```
Just split {panelCount} wall panels in my Revit model in under a minute with SplitWalls — free tool for architects & builders

Try it: splitwalls.vercel.app
```

**LinkedIn:**
```
Just used SplitWalls to automatically split {panelCount} wall panels in my Revit model — took under a minute, no Revit desktop needed.

Free tool for architects, structural engineers, and builders working with panelized wall systems.

splitwalls.vercel.app
```

---

## 14. Environment Variables

```env
# Clerk
NEXT_PUBLIC_CLERK_PUBLISHABLE_KEY=pk_...
CLERK_SECRET_KEY=sk_...
NEXT_PUBLIC_CLERK_SIGN_IN_URL=/sign-in
NEXT_PUBLIC_CLERK_SIGN_UP_URL=/sign-up
NEXT_PUBLIC_CLERK_AFTER_SIGN_IN_URL=/dashboard
NEXT_PUBLIC_CLERK_AFTER_SIGN_UP_URL=/dashboard

# MongoDB
DATABASE_URL=mongodb+srv://...

# APS (Autodesk Platform Services)
APS_CLIENT_ID=
APS_CLIENT_SECRET=
APS_BUCKET_KEY=splitwalls-beta

# DA Activity aliases (one per Revit version)
DA_NICKNAME=splitwalls
DA_ACTIVITY_2022=SplitWalls2022
DA_ACTIVITY_2023=SplitWalls2023
DA_ACTIVITY_2024=SplitWalls2024
DA_ACTIVITY_ALIAS=prod
```

---

## 15. Implementation Roadmap (MVP + Admin Workbench)

### Phase A: Project Scaffold (2-3 days)

- [ ] Create Next.js 14 project with TypeScript
- [ ] Install + configure Clerk (Google + Microsoft + email)
- [ ] Install + configure Prisma with MongoDB Atlas
- [ ] Set up Tailwind CSS + shadcn/ui
- [ ] Create full Prisma schema (User, Job, Share, Bundle, Activity, TestJob, ViewCache)
- [ ] Landing page (`/`)
- [ ] Auth pages (`/sign-in`, `/sign-up`)
- [ ] Protected dashboard layout (user)
- [ ] Clerk middleware.ts — admin role gate for /admin/* and /api/admin/*
- [ ] lib/aps-auth.ts — 2-legged OAuth token cache with lazy refresh
- [ ] lib/aps-client.ts — apsRequest() with auto-token + 401 retry
- [ ] lib/api-helpers.ts — withAuth(handler) Clerk wrapper
- [ ] Vitest setup + first test (aps-auth token caching)

### Phase B: Upload + Configure Flow (5-7 days)

- [ ] Dashboard page with file upload dropzone
- [ ] Revit version selector (2022/2023/2024)
- [ ] POST /api/upload — generate signed OSS URL + create Job record
- [ ] POST /api/upload/complete — notify upload done, trigger translation
- [ ] Client-side direct-to-OSS upload via signed URL (no size limit)
- [ ] **2D Profile Editor (SVG + React):**
  - [ ] `useProfileCalculator` hook — uniform split math (TS port of WPF logic)
  - [ ] `ProfileCanvas.tsx` — SVG wall rendering (segments, labels, dimensions)
  - [ ] `SplitPointDragger.tsx` — draggable split lines with pointer events
  - [ ] `ProfileLegend.tsx` — color legend for profile types
  - [ ] Uniform → custom mode switch on drag
  - [ ] Snap-to-grid (100mm configurable)
  - [ ] Min segment width enforcement (100mm)
  - [ ] Max segment cap (100)
  - [ ] [Reset to Uniform] button
  - [ ] Editable wall length + height inputs
  - [ ] ResizeObserver + SVG viewBox for responsive scaling
- [ ] Configuration form (panel width, separator, joins toggle) — connected to canvas
- [ ] Basic form validation (panelWidth ≥ 50mm, wallLength ≥ 100mm, file is .rvt)
- [ ] skill.json generation from canvas state (uniform or custom splitPointsMm[])

### Phase C: C# AppBundle (5-7 days) — CRITICAL PATH

- [ ] New `SplitWallsDA` project (IExternalDBApplication)
- [ ] Extract `UniformSplitService` from ThisApplication.cs noWindows path
  - [ ] `SplitWallUniform(wall, panelWidthMm, separatorWidthMm)` — uniform mode
  - [ ] `SplitWallAt(wall, splitPointsMm[])` — custom mode (from 2D editor drag)
- [ ] Port `RevitUnitHelper` + `WallJoinHelper` (copy, remove UI refs)
- [ ] Implement `SkillReader` (JSON deserialization)
- [ ] Implement `MvpSkill` model classes (with `splitPointsMm[]` support)
- [ ] Add structured Console.WriteLine logging for debug visibility
- [ ] Build 3 AppBundle zips (2022, 2023, 2024)
- [ ] Register AppBundles on APS DA (use admin workbench!)
- [ ] Register 3 Activities (one per version)
- [ ] Test with a sample .rvt via admin workbench (both uniform + custom splits)

### Phase D: Admin DA Workbench (5-7 days)

- [ ] `/admin` dashboard page
- [ ] Bundle Manager UI (upload .zip, list bundles, progress steps)
- [ ] POST /api/admin/bundles/upload — upload .zip to OSS
- [ ] POST /api/admin/bundles/register — register AppBundle on DA
- [ ] POST /api/admin/bundles/alias — create/update alias
- [ ] Activity Manager (create/update from bundle)
- [ ] POST /api/admin/activities — create + alias
- [ ] Test Runner UI (select bundle, upload .rvt, edit JSON, run)
- [ ] POST /api/admin/test — submit WorkItem
- [ ] Log Viewer UI (structured output, error highlighting)
- [ ] GET /api/admin/test/:id/logs — fetch + parse + cache report
- [ ] DA Report Parser (lib/da-report-parser.ts)
- [ ] Job History page (filter, search, pagination)
- [ ] GET /api/admin/history — list with filters
- [ ] Compare view (diff two test job inputs/outputs)

### Phase E: Viewer Integration — Shared (4-6 days)

Viewer infrastructure is shared between admin workbench and user flow.

- [ ] ViewerProvider component ("use client" + `<Script>` + `Autodesk.Viewing.Initializer`)
- [ ] Shared API routes: /api/viewer/token, /api/viewer/translate, /api/viewer/status/:urn
- [ ] Model Derivative translation trigger + polling (exponential backoff 3s→10s)
- [ ] ViewCache logic (hash-based, skip hash for files >200MB)
- [ ] Client-side SHA-256 hashing (crypto.subtle.digest)
- [ ] SingleViewer component (headless Viewer3D + React controls — for /viewer/:jobId)
- [ ] SplitViewer component (two headless Viewer3D + React controls — for /result/:jobId and /admin/viewer/:id)
- [ ] Camera sync between SplitViewer instances
- [ ] "View in 3D" button on admin Log Viewer page
- [ ] Lazy-load input viewer in SplitViewer ("Compare with original" button)
- [ ] Fallback UI when Viewer SDK CDN fails
- [ ] 5-min translation timeout with retry button

### Phase F: Execute + Status + Result — User Flow (4-5 days)

- [ ] POST /api/execute — build skill.json, create WorkItem
- [ ] GET /api/status/:jobId — poll DA, update MongoDB
- [ ] Job status page with progress indicator
- [ ] /viewer/:jobId page — show uploaded model in 3D (uses SingleViewer)
- [ ] Auto-translate input .rvt on upload (via ViewCache)
- [ ] /result/:jobId page — before/after split-screen (uses SplitViewer)
- [ ] Auto-translate result .rvt on job success
- [ ] GET /api/download/:jobId — signed OSS URL
- [ ] Error handling for DA failures (surface report logs)

### Phase G: Social Gate + Polish (2-3 days)

- [ ] Social share dialog (X + LinkedIn)
- [ ] POST /api/social/verify
- [ ] Download gating logic
- [ ] Dynamic panel count in share message (from report.txt)
- [ ] Job history on dashboard
- [ ] Loading states, error states, empty states
- [ ] Mobile-responsive landing page

### Total: ~27-39 days

### Recommended Build Order

```
Phase A (scaffold)
    │
    ▼
Phase D (admin workbench — bundle mgr, test runner, logs, history)
    │
    ▼
Phase E (viewer — shared infra for admin + user)
    │
    ├──→ Admin Viewer now works (before/after in workbench)
    │
    ▼
Phase C (C# AppBundle — developed + tested via admin workbench + viewer)
    │
    ▼
Phase B (user upload flow)
    │
    ▼
Phase F (user execute + viewer pages — reuses shared viewer components)
    │
    ▼
Phase G (social gate + polish)
```

**Why this order:** Build the admin workbench + viewer FIRST because you need them to develop and test the C# AppBundle. The workbench is your development tool — use it to iterate on the AppBundle and visually confirm results before wiring up the user-facing flow. The viewer components built in Phase E are reused directly in the user flow (Phase F).

---

## 16. What's NOT in Scope (Deferred)

| Feature | Why Deferred | When |
|---------|-------------|------|
| Per-wall configuration (user) | MVP applies to all walls; wall selection UI is Phase 2 | Phase 2 |
| Profile shapes (U/T/I/L/borde) | Start with standard rectangular splits only | Phase 2 |
| Opening-aware routing (osb/smartPanel) | noWindows strategy only for beta | Phase 2 |
| Revit 2025 engine | Add when user demand appears | Phase 2 |
| WPF desktop editor | Can't test without Revit license | Deferred |
| Parametric mode (no .rvt) | Need .rvt upload to prove pipeline | Phase 3 |
| Skill library / marketplace | Premature — need users first | Phase 3 |
| NestJS API migration | Vercel API routes are fine for beta | When scaling |
| Custom domain | Use splitwalls.vercel.app for beta | When branding matters |
| Webhook callbacks | Polling is simpler for MVP | Phase 2 |
| Multi-user admin workbench | Only you (admin) for beta | When productizing workbench |
| AppBundle version diffing | Nice-to-have, not MVP | Phase 2 workbench |
| Automated test suites for DA | Manual testing sufficient for beta | Phase 2 workbench |

---

## 17. Risk Registry

| Risk | Likelihood | Impact | Mitigation |
|------|-----------|--------|------------|
| C# AppBundle fails on APS DA — can't debug locally | HIGH | CRITICAL | Admin workbench provides structured logs + 3D viewer. Iterate via workbench, not curl. |
| Revit API differences between 2022/2023/2024 break the bundle | MEDIUM | HIGH | Start with 2024 only via workbench, add others once 2024 works. |
| APS Flex token costs spike (Viewer translations) | MEDIUM | MEDIUM | Smart ViewCache (hash-based). Result translated only on explicit click. Rate limit user jobs (5/day). |
| Social gate is too annoying — users bounce | MEDIUM | MEDIUM | Track conversion rate. Add "Skip" option if <30%. |
| .rvt files too large for OSS upload (>100MB) | LOW | MEDIUM | Client-side file size check. OSS supports up to 5GB with multipart. |
| Wall splitting produces incorrect geometry | MEDIUM | HIGH | Admin workbench before/after Viewer catches this during development. |
| DA report format changes | LOW | LOW | Parser is tolerant (falls back to raw text). Report is cached. |
| Vercel serverless function timeout (10s default) | MEDIUM | MEDIUM | APS OSS upload uses signed URLs (direct browser→OSS). DA operations are async (fire + poll). |

---

## 18. Credentials Setup Guide

### 18.1 Clerk (Auth)

1. Go to https://clerk.com → Create application
2. Enable sign-in methods: **Google**, **Microsoft**, **Email**
3. For Google: Clerk auto-provisions OAuth credentials (no setup needed on free tier)
4. For Microsoft: Clerk auto-provisions via Azure AD (no setup needed on free tier)
5. Copy keys to `.env`:
   ```
   NEXT_PUBLIC_CLERK_PUBLISHABLE_KEY=pk_test_...
   CLERK_SECRET_KEY=sk_test_...
   ```
6. Set your own user as admin: Clerk Dashboard → Users → your user → Metadata → set `{ "role": "admin" }`

### 18.2 APS (Autodesk Platform Services)

1. Go to https://aps.autodesk.com → Sign in with Autodesk account
2. Create new application → Select APIs:
   - **Data Management** (OSS)
   - **Model Derivative** (Viewer translation)
   - **Design Automation** (Revit cloud execution)
3. Note: Design Automation requires **Flex tokens** — request access if not enabled
4. Copy credentials to `.env`:
   ```
   APS_CLIENT_ID=your_client_id
   APS_CLIENT_SECRET=your_client_secret
   ```
5. Set DA nickname (one-time):
   ```bash
   curl -X PATCH https://developer.api.autodesk.com/da/us-east/v3/forgeapps/me \
     -H "Authorization: Bearer {token}" \
     -H "Content-Type: application/json" \
     -d '{ "nickname": "splitwalls" }'
   ```

### 18.3 MongoDB Atlas

1. Go to https://cloud.mongodb.com → Create free cluster (M0)
2. Create database user (username + password)
3. Whitelist IP: `0.0.0.0/0` (for Vercel serverless — restrict later)
4. Get connection string → add to `.env`:
   ```
   DATABASE_URL=mongodb+srv://user:pass@cluster.mongodb.net/splitwalls?retryWrites=true&w=majority
   ```
5. Run `npx prisma db push` to create collections

### 18.4 Vercel

1. Go to https://vercel.com → Import Git repository
2. Add all `.env` variables in Vercel dashboard → Settings → Environment Variables
3. Deploy — Vercel auto-detects Next.js
4. Your app is live at `splitwalls.vercel.app`

---

## 19. Engineering Decisions (from Eng Review)

### Architecture Decisions

| # | Issue | Decision | Rationale |
|---|-------|----------|-----------|
| 1 | .rvt upload exceeds Vercel 4.5MB limit | **Signed URL direct-to-OSS** — API generates signed URL, browser uploads directly to APS OSS | Eliminates size limit entirely, standard serverless pattern |
| 2 | APS OAuth token caching | **In-memory module-level cache** in `lib/aps-auth.ts` with lazy refresh at <5min remaining | Tokens are cheap to regenerate (~200ms), no Redis needed |
| 3 | APS Viewer SDK in Next.js App Router | **`"use client"` ViewerProvider** with `<Script>` tag + `onLoad` + `dynamic(() => ..., { ssr: false })` | Clean SSR separation, avoids hydration issues |
| 4 | Dual Viewer for before/after | **Headless `Viewer3D`** (not GuiViewer3D) with shared React controls | Lower memory, no toolbar conflicts, consistent with Tailwind UI |
| 5 | Vercel 10s timeout vs DA bundle registration | **Three chained API routes** — upload → register → alias, called sequentially from frontend | Each call stays under 10s, explicit progress feedback |
| 7 | Prisma/MongoDB relation integrity | **Accept app-level relations + soft deletes** (`deletedAt` on Bundle, Activity) | No cascade needed for single-admin beta |
| 8 | Model Derivative translation polling | **Frontend polling with exponential backoff** (3s → 10s after 30s) | Simple, no infrastructure, works on Vercel serverless |

### Code Quality Decisions

| # | Issue | Decision | Rationale |
|---|-------|----------|-----------|
| 9 | API route boilerplate (auth + APS) | **Shared helpers**: `withAuth(handler)` in `lib/api-helpers.ts` + `apsRequest()` in `lib/aps-client.ts` | DRY — each route is 10-15 lines of business logic |
| 10 | Admin role enforcement | **Clerk middleware matcher** in `middleware.ts` for `/api/admin/*` and `/admin/*` | Zero code in routes, impossible to forget on new admin route |
| 11 | DA report parser robustness | **Tolerant by default** — unmatched lines become `level: "info", source: "unknown"`. Never throw. Raw text always preserved. | DA report format is undocumented, varies between Revit versions |
| 12 | Viewer routes user vs admin duplication | **Single `/api/viewer/*` route set** for all authenticated users. Remove `/api/admin/viewer/*`. | DRY — eliminates 3 duplicate routes, no security reason to restrict |
| 13 | 2D Profile Editor rendering technology | **SVG + React** (not HTML5 Canvas or konva.js) | DOM-based hit testing, CSS/Tailwind colors, React event handlers per segment. <200 segments = no perf concern |
| 14 | Wall dimensions for preview | **Generic preview wall** with editable length/height (default 5200×2440mm) | No Model Derivative property extraction needed. User adjusts manually. Simplest path |
| 17 | Custom split support | **Extend AppBundle** with `SplitWallAt(wall, splitPointsMm[])` | Makes 2D drag editor actually functional. SplitRule model already has SplitPointsMm field |
| 18 | 2D editor interaction model | **Uniform auto-calculate + draggable split points** | Starts with uniform, user drags to customize, method auto-switches, Reset button available |

### Performance Decisions

| # | Issue | Decision | Rationale |
|---|-------|----------|-----------|
| 15 | Signed URL expiration for slow uploads | **60-minute expiration** on OSS signed URLs | Minimal security risk (single-use PUT), handles slow connections |
| 16 | Dual viewer memory (400-800MB) | **Lazy-load input viewer** — show result by default, "Compare with original" button loads input alongside | Halves default memory, most users only check result |

### Critical Failure Modes (5 gaps identified + fixes)

| Gap | Failure | Fix |
|-----|---------|-----|
| Report >10MB | Browser freeze during parsing | Truncate at 1MB, show "truncated" warning |
| SplitViewer partial failure | One model translation fails | Error state per panel, don't block the other |
| File >500MB hashing | Browser OOM during SHA-256 | Skip hashing for files >200MB, always translate |
| Translation stuck | Model Derivative never completes | 5-min client timeout + "Taking longer than expected" + retry button |
| Viewer SDK CDN down | Blank viewer area | Fallback message "3D viewer unavailable" + "Download file instead" button |

### Test Strategy

- **Unit tests (Vitest):** `da-report-parser`, `aps-auth` token caching, skill.json builder, signed URL generation, `useProfileCalculator` (uniform splits, custom drag, snap-to-grid, edge cases: panelWidth > wallLength, wallLength = 0, min segment width enforcement)
- **Integration tests (msw):** API routes with mocked APS responses
- **Manual E2E:** Admin workbench tested live (single admin)
- **No Playwright E2E for beta** — defer to Phase 2

### Key Library Choices

```
lib/
  aps-auth.ts              — Token cache + lazy refresh (singleton)
  aps-client.ts            — apsRequest(method, url, body) with 401 retry
  api-helpers.ts           — withAuth(handler) Clerk wrapper
  da-report-parser.ts      — Raw text → ParsedReport (tolerant)

hooks/
  useProfileCalculator.ts  — Uniform/custom split math (TS port of WPF logic)
                              Returns SegmentDef[] from wallLength + panelWidth + separatorWidth
                              Handles drag → custom mode switch + splitPointsMm[] generation

components/
  ViewerProvider.tsx       — Script loader + Autodesk.Viewing.Initializer
  SingleViewer.tsx         — One headless Viewer3D instance
  SplitViewer.tsx          — Two headless Viewer3D + React controls + camera sync
  ProfileEditor.tsx        — 2D wall profile editor (container)
    ProfileCanvas.tsx      — SVG rendering: segments, labels, dimensions, wall outline
    SplitPointDragger.tsx  — Draggable split lines (pointer events, snap-to-grid)
    ProfileLegend.tsx      — Color legend for profile types
```

### Upload Flow (Revised for Signed URLs)

```
Browser                          API Route                    APS OSS
───────                          ─────────                    ───────

1. Hash file (SHA-256)
   Check ViewCache
   ────────────────────→  POST /api/upload
                          Generate signed URL ──→  (prepares bucket)
                          ←── signed URL + ossUrn
                          Create Job record
   ←── { signedUrl, jobId, ossUrn }

2. Upload .rvt directly   ──────────────────────────────────→  PUT (signed URL)
   (no size limit)                                             Stored in OSS

3. Notify completion
   ────────────────────→  POST /api/upload/complete
                          Trigger Model Derivative
                          ←── { status: "translating" }
```

---

## 20. Success Metrics (Beta)

| Metric | Target (first month) |
|--------|---------------------|
| Sign-ups | 50+ |
| Jobs executed successfully | 100+ |
| Social share conversion rate | >50% |
| Return users (>1 job) | >30% |
| X/LinkedIn impressions from shares | 5,000+ |
| DA failure rate | <10% |
| Admin debug iterations per day | 10+ (during development) |
| Time per debug cycle (workbench) | <1 minute (vs 3-5 min with curl) |

---

## 21. Completion Summary

```
+====================================================================+
|         CEO + ENG PLAN REVIEW — COMPLETION SUMMARY                 |
+====================================================================+
| Mode selected        | REDUCTION (user) + EXPANSION (admin)        |
| System Audit         | 8K-line monolith, no local Revit debug,     |
|                      | no tests, admin workbench fills debug gap    |
| Step 0               | Two-product strategy in one Next.js app      |
+--------------------------------------------------------------------+
| CEO Decisions (12):                                                |
|   1.  Admin access: Clerk metadata role flag                       |
|   2.  Full workbench: 5 sections (Bundle, Test, Logs, Viewer, Hx)  |
|   3.  Viewer caching: hash-based, result on explicit click         |
|   4.  Bundle upload: browser drag-and-drop                         |
|   5.  Before/after: split-screen dual APS Viewer                   |
|   6.  Auth: Google + Microsoft + email (Clerk)                     |
|   7.  Social gate: Share-to-Download on X + LinkedIn               |
|   8.  Verification: Intent-based (window close detection)          |
|   9.  Database: MongoDB Atlas + Prisma                             |
|   10. Revit versions: 2022, 2023, 2024 (3 bundles)                |
|   11. Hosting: Vercel (splitwalls.vercel.app)                      |
|   12. Share message: Result-focused with dynamic panel count       |
+--------------------------------------------------------------------+
| Eng Decisions (14):                                                |
|   1.  Upload: Signed URL direct-to-OSS (bypasses Vercel limit)    |
|   2.  APS auth: In-memory token cache with lazy refresh            |
|   3.  Viewer SDK: "use client" ViewerProvider + <Script> + dynamic |
|   4.  Dual Viewer: Headless Viewer3D + React controls              |
|   5.  Bundle registration: 3 chained API routes (< 10s each)      |
|   7.  DB relations: App-level + soft deletes (no cascade needed)   |
|   8.  Translation polling: Exponential backoff (3s → 10s)          |
|   9.  API boilerplate: withAuth() + apsRequest() shared helpers    |
|   10. Admin role: Clerk middleware matcher (zero per-route code)    |
|   12. Viewer routes: Single /api/viewer/* for all users            |
|   13. 2D Editor: SVG + React (DOM hit-testing, Tailwind colors)   |
|   14. Wall dims: Generic preview wall (editable, no MD extraction)|
|   15. Signed URL expiry: 60 minutes (handles slow connections)     |
|   16. Memory: Lazy-load input viewer on "Compare" click            |
|   17. Custom splits: AppBundle SplitWallAt(splitPointsMm[])       |
|   18. Interaction: Uniform auto-calc + draggable split points      |
+--------------------------------------------------------------------+
| Eng Review:                                                        |
|   Architecture:   7 issues found, all resolved                     |
|   Code Quality:   4 issues found, all resolved                     |
|   Tests:          Strategy defined, 5 critical gaps fixed          |
|   Performance:    2 issues found, all resolved                     |
+--------------------------------------------------------------------+
| NOT in scope         | 13 items deferred (see Section 16)          |
| What already exists  | WallProfileBuilder, RevitUnitHelper,        |
|                      | WallJoinHelper, WindowDetectionService,     |
|                      | skill.json data models                       |
| Critical path        | Scaffold (A) → Admin (D) → Viewer (E)      |
|                      | → AppBundle (C) → User flow (B+F+G)          |
| Estimated timeline   | 27-39 days                                   |
| Risks                | 8 identified, 2 HIGH likelihood              |
| Critical failure gaps| 5 identified, all have fixes defined         |
| Test strategy        | Vitest unit + msw integration + manual E2E   |
+====================================================================+
```
