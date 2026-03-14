# SplitWalls Cloud — New Project Kickoff

**Date:** 2026-03-14
**Purpose:** Everything a new LLM session needs to start building the SplitWalls Cloud web app from scratch.
**Context:** This is the cloud companion to the SplitWalls Revit desktop addin. The desktop addin lives at `~/source/repos/SplitWalls` (C# .NET 4.8). The cloud project is a NEW git repository: `splitwalls-cloud`.

---

## What We're Building

A browser-based wall-splitting tool. Users upload a `.rvt` (Revit) file, design how to split the walls on a 2D canvas, execute via Autodesk Platform Services Design Automation (cloud Revit), verify the result in 3D, share on X/LinkedIn, and download the modified `.rvt`. Free to use.

**Also includes:** An admin DA Workbench for debugging the C# AppBundle without a Revit desktop license.

**Full plan (v5):** `docs/plans/splitwalls-beta-mvp-plan.md` in the SplitWalls C# repo (or the new cloud repo once created).

---

## 3-Software Architecture

```
splitwalls-cloud/          ← NEW git repository
  apps/
    web/                   ← Next.js 14 (Vercel) — pure UI, zero secrets
    api/                   ← NestJS (Railway) — all business logic + APS credentials
    da-bundle/             ← C# .NET 4.8 DA AppBundle (built + deployed via admin workbench)
  packages/
    types/                 ← Shared TypeScript interfaces used by both web and api
    tsconfig/              ← Shared tsconfig.base.json
  turbo.json
  pnpm-workspace.yaml
  package.json
```

### Data Flow

```
Browser (Next.js)
  │ fetch(NEXT_PUBLIC_API_URL + '/api/...', {
  │   headers: { Authorization: 'Bearer ' + clerkToken }
  │ })
  ▼
NestJS (Railway)                     MongoDB Atlas
  │ ClerkAuthGuard verifies JWT  ──→  Prisma ORM
  │ APS credentials (env vars)
  │ All business logic
  ▼
APS Cloud (Autodesk Platform Services)
  ├─ OSS (file storage): .rvt files, skill.json, result.rvt
  ├─ Design Automation: Revit cloud execution
  ├─ Model Derivative: .rvt → SVF2 translation
  └─ Viewer SDK: browser calls APS DIRECTLY using token from NestJS

C# DA AppBundle (isolated APS compute)
  Reads: skill.json + input.rvt from OSS
  Writes: result.rvt + report.txt to OSS
  NO network access to web or api — OSS only
```

---

## Tech Stack

| Layer | Technology | Hosting | Cost |
|-------|-----------|---------|------|
| Frontend | Next.js 14+ App Router (TypeScript) | Vercel | Free |
| Backend API | NestJS (Node.js + TypeScript) | Railway | Free tier |
| Auth | Clerk (Google + Microsoft + email) | Clerk Cloud | Free 10K MAU |
| Database | MongoDB Atlas + Prisma ORM | MongoDB Atlas | Free tier |
| Styling | Tailwind CSS + shadcn/ui | — | Free |
| 3D Viewer | APS Viewer SDK v7 (client-side) | APS CDN | Free |
| Model Translation | APS Model Derivative (SVF2) | APS Cloud | Flex tokens |
| Cloud Engine | APS Design Automation v3 | APS Cloud | Flex tokens |
| File Storage | APS OSS (24hr auto-expiry signed URLs) | APS Cloud | Free |
| Monorepo | Turborepo + pnpm workspaces | — | Free |
| C# AppBundle | .NET 4.8, builds to .zip | Local → APS | — |

---

## Why NestJS (Not Next.js API Routes)

The plan originally used Next.js API routes on Vercel. This was changed to NestJS on Railway for:

1. **Vercel 10s timeout** — APS Model Derivative translation takes 30-120s. NestJS on Railway has no timeout.
2. **Persistent token cache** — In-memory APS OAuth token cache works reliably on a persistent server vs serverless cold starts.
3. **WebSocket-ready** — Phase 2 will stream DA log lines in real-time. NestJS supports SSE/WebSocket natively; Vercel serverless cannot maintain open connections.
4. **Clean secrets boundary** — Vercel (Next.js) has ZERO secrets. Railway (NestJS) has ALL secrets.

---

## Environment Variables

### Next.js — Vercel (apps/web/.env)

```env
NEXT_PUBLIC_CLERK_PUBLISHABLE_KEY=pk_...
NEXT_PUBLIC_CLERK_SIGN_IN_URL=/sign-in
NEXT_PUBLIC_CLERK_SIGN_UP_URL=/sign-up
NEXT_PUBLIC_CLERK_AFTER_SIGN_IN_URL=/dashboard
NEXT_PUBLIC_CLERK_AFTER_SIGN_UP_URL=/dashboard
NEXT_PUBLIC_API_URL=https://api.splitwalls.up.railway.app

# Local dev: NEXT_PUBLIC_API_URL=http://localhost:3001
```

### NestJS — Railway (apps/api/.env)

```env
# Clerk
CLERK_SECRET_KEY=sk_...

# MongoDB
DATABASE_URL=mongodb+srv://user:pass@cluster.mongodb.net/splitwalls?retryWrites=true&w=majority

# APS (Autodesk Platform Services) — ALL secrets here, NEVER in Vercel
APS_CLIENT_ID=
APS_CLIENT_SECRET=
APS_BUCKET_KEY=splitwalls-beta

# DA Activity aliases
DA_NICKNAME=splitwalls
DA_ACTIVITY_2022=SplitWalls2022
DA_ACTIVITY_2023=SplitWalls2023
DA_ACTIVITY_2024=SplitWalls2024
DA_ACTIVITY_ALIAS=prod

# Server
CORS_ORIGIN=https://splitwalls.vercel.app,http://localhost:3000
PORT=3001
NODE_ENV=production
```

---

## API Routes (NestJS, global prefix `/api`)

All routes require `Authorization: Bearer {clerkJWT}` header. Admin routes additionally require `role: "admin"` in Clerk JWT metadata.

### User Routes

```
POST /api/jobs/upload-url          Get signed OSS URL for .rvt upload; create Job record
POST /api/jobs/:id/execute         Build skill.json, upload to OSS, create WorkItem on APS DA
GET  /api/jobs/:id/status          Poll DA WorkItem status, update Job record
POST /api/jobs/:id/social/verify   Mark job as shared (creates Share record)
GET  /api/jobs/:id/download        Get signed OSS URL for result.rvt (403 if not shared)
GET  /api/viewer/token             Get short-lived APS Viewer token (1hr, read-only scope)
POST /api/viewer/translate         Trigger Model Derivative translation (hash-cached)
GET  /api/viewer/status/:urn       Poll translation status
GET  /api/health                   Public health check { status, db, apsToken }
```

### Admin Routes (ClerkAuthGuard + AdminGuard)

```
POST /api/admin/bundles/upload     Upload AppBundle .zip to APS OSS; create Bundle record
POST /api/admin/bundles/register   Register AppBundle on APS DA
POST /api/admin/bundles/alias      Create/update AppBundle alias
GET  /api/admin/bundles            List all bundles
POST /api/admin/activities         Create Activity + alias on APS DA
POST /api/admin/test               Submit test WorkItem; create TestJob record
GET  /api/admin/test/:id           Poll test WorkItem status
GET  /api/admin/test/:id/logs      Fetch, parse, and cache DA report
GET  /api/admin/history            List test jobs (filter: status, engine, page)
```

---

## MongoDB Schema (Prisma — lives in apps/api/prisma/schema.prisma)

```prisma
datasource db {
  provider = "mongodb"
  url      = env("DATABASE_URL")
}

generator client {
  provider = "prisma-client-js"
}

model User {
  id        String   @id @default(auto()) @map("_id") @db.ObjectId
  clerkId   String   @unique
  email     String
  name      String?
  role      String   @default("user")   // "user" | "admin"
  createdAt DateTime @default(now())
  jobs      Job[]
  shares    Share[]
}

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
  config        Json     // { panelWidthMm, separatorWidthMm, disableWallJoins, splitRule }
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

model Bundle {
  id             String     @id @default(auto()) @map("_id") @db.ObjectId
  name           String
  engine         String     // "Autodesk.Revit+2024"
  version        Int        @default(1)
  ossUrn         String
  daAppBundleId  String?
  daAlias        String?
  status         String     // pending | registered | failed
  fileName       String
  fileSizeBytes  Int
  createdAt      DateTime   @default(now())
  updatedAt      DateTime   @updatedAt
  activities     Activity[]
  testJobs       TestJob[]
}

model Activity {
  id             String    @id @default(auto()) @map("_id") @db.ObjectId
  name           String
  bundleId       String    @db.ObjectId
  bundle         Bundle    @relation(fields: [bundleId], references: [id])
  engine         String
  daActivityId   String?
  daAlias        String?
  parameters     Json
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
  status              String   // pending | inProgress | success | failed
  ossInputRvtUrn      String
  ossInputJsonUrn     String?
  ossResultUrn        String?
  inputConfig         Json
  reportCached        String?
  reportParsed        Json?
  errorMessage        String?
  durationSeconds     Float?
  inputFileHash       String?
  inputViewableUrn    String?
  resultViewableUrn   String?
  createdAt           DateTime @default(now())
  updatedAt           DateTime @updatedAt
}

model ViewCache {
  id                String   @id @default(auto()) @map("_id") @db.ObjectId
  rvtFileHash       String   @unique
  ossUrn            String
  viewableUrn       String?
  translationStatus String   // pending | inProgress | success | failed
  translationId     String?
  fileSizeBytes     Int?
  createdAt         DateTime @default(now())
  updatedAt         DateTime @updatedAt
}
```

---

## Shared TypeScript Types (packages/types/src/)

```typescript
// skill.ts — matches skill.json schema sent to APS DA
export interface MvpSkill {
  version: "2.0";
  created: string;
  name: string;
  defaults: {
    panelWidthMm: number;
    separatorWidthMm: number;
    disableWallJoins: boolean;
  };
  wallConfigs: [];                   // MVP: empty = apply to ALL walls
  globalConfig: {
    strategy: "noWindows";
    splitRule: {
      method: "uniform" | "custom";
      panelWidthMm: number;
      splitPointsMm?: number[];      // custom drag positions from 2D editor
    };
  };
}

// job.ts
export type JobStatus = "uploaded" | "executing" | "success" | "failed";
export interface JobConfig {
  panelWidthMm: number;
  separatorWidthMm: number;
  disableWallJoins: boolean;
  splitRule: { method: "uniform" | "custom"; panelWidthMm: number; splitPointsMm?: number[] };
}

// log.ts — DA report parser output
export interface ParsedLogLine {
  timestamp: string;
  level: "info" | "warn" | "error" | "system";
  source: "revit" | "appbundle" | "da-engine" | "unknown";
  message: string;
  raw: string;
}
export interface ParsedReport {
  lines: ParsedLogLine[];
  errors: ParsedLogLine[];
  warnings: ParsedLogLine[];
  summary: {
    totalDuration: number;
    startupTime: number;
    processingTime: number;
    wallCount: number | null;
    panelCount: number | null;
    exitCode: string;
  };
}

// api.ts — typed request/response shapes for all API routes
export interface CreateJobUploadUrlRequest {
  revitVersion: "2022" | "2023" | "2024";
  fileName: string;
  fileSizeBytes: number;
}
export interface CreateJobUploadUrlResponse {
  jobId: string;
  signedUrl: string;
  ossUrn: string;
}
export interface ExecuteJobRequest {
  config: JobConfig;
}
export interface ExecuteJobResponse {
  jobId: string;
  workItemId: string;
}
export interface GetJobStatusResponse {
  status: JobStatus;
  progress: number;
  panelCount?: number;
  errorMessage?: string;
}
export interface GetViewerTokenResponse {
  accessToken: string;
  expiresIn: number;
}
```

---

## NestJS Module Structure

```
apps/api/src/
  main.ts                CORS (CORS_ORIGIN), Helmet, ValidationPipe, global prefix '/api', PORT
  app.module.ts

  auth/
    clerk.guard.ts       @Injectable CanActivate — verifies Clerk JWT via clerk-sdk-node
                         Sets req.userId (sub claim) + req.userRole (metadata.role || 'user')
    admin.guard.ts       @Injectable CanActivate — checks req.userRole === 'admin'

  aps/
    aps-auth.service.ts  2-legged OAuth (client_credentials grant)
                         In-memory token cache: { token, expiresAt }
                         getToken(): if expiresAt - now < 5min → refresh
    aps-oss.service.ts   getSignedUrl(key, verb, ttl), upload(key, buffer), delete(key)
    aps-da.service.ts    registerBundle(), updateBundle(), createAlias(), createActivity()
                         submitWorkItem(activityId, inputs, outputs), getWorkItemStatus(id)
                         getReport(reportUrl)
    aps-md.service.ts    translate(ossUrn), getStatus(urn), getViewerToken()

  jobs/
    jobs.controller.ts   POST /jobs/upload-url, POST /jobs/:id/execute,
                         GET /jobs/:id/status, POST /jobs/:id/social/verify,
                         GET /jobs/:id/download
    jobs.service.ts      createUploadUrl(), executeJob(), getStatus(), verifySocial(), getDownload()

  admin/
    admin.controller.ts  All /admin/* routes
    bundles.service.ts   uploadBundle(), registerBundle(), createAlias()
    test-runner.service.ts submitTest(), getTestStatus(), getTestLogs()
    da-report-parser.ts  parse(rawText: string): ParsedReport
                         NEVER throws — unmatched lines → level: 'info', source: 'unknown'
                         Truncate at 1MB to prevent browser freeze

  viewer/
    viewer.controller.ts GET /viewer/token, POST /viewer/translate, GET /viewer/status/:urn
    view-cache.service.ts checkByHash(hash), store(hash, ossUrn, viewableUrn)

  social/
    social.controller.ts POST /jobs/:id/social/verify

  prisma/
    prisma.service.ts    extends PrismaClient, implements OnModuleInit/OnModuleDestroy

  health/
    health.controller.ts GET /health (public — no auth) → { status: 'ok', db: 'connected', apsToken: 'cached' }
```

---

## Next.js App Structure (apps/web)

```
apps/web/
  middleware.ts          Clerk middleware — protect /dashboard, /configure/:id,
                         /viewer/:id, /result/:id, /status/:id, /admin, /admin/*
                         Note: role check for /admin still works via Clerk sessionClaims

  app/
    layout.tsx           ClerkProvider wrapper
    page.tsx             Landing page (public)
    sign-in/             Clerk SignIn component
    sign-up/             Clerk SignUp component
    dashboard/           File upload + job history
    viewer/[id]/         APS Viewer (single model, after upload)
    configure/[id]/      2D Profile Editor page
    status/[id]/         Job progress polling page
    result/[id]/         Before/after split-screen + social gate + download
    admin/               Admin layout (role: admin check)
      bundles/           Bundle manager
      test/              Test runner
      logs/[id]/         Log viewer
      viewer/[id]/       Before/after split viewer (admin)
      history/           Job history

  lib/
    api-client.ts        Typed fetch wrapper:
                         async function api<T>(path, opts?): Promise<T>
                         Auto-attaches: Authorization: Bearer {clerkToken}
                         Base URL: NEXT_PUBLIC_API_URL

  components/
    ViewerProvider.tsx   <Script src="APS Viewer SDK CDN" onLoad=... />
                         + Autodesk.Viewing.Initializer
    SingleViewer.tsx     One headless Viewer3D instance
    SplitViewer.tsx      Two headless Viewer3D + camera sync + "Compare with original" lazy-load
    ProfileEditor.tsx    2D wall profile editor container
      ProfileCanvas.tsx  SVG: segments, labels, dimensions, wall outline
      SplitPointDragger.tsx  Draggable split lines (pointer events, snap-to-grid)
      ProfileLegend.tsx  Color legend (std, U, L-left, L-right, T, I, borde)

  hooks/
    useProfileCalculator.ts  Uniform split math + custom drag → SegmentDef[] + splitPointsMm[]
                              Port of WPF ProfileCanvasRenderer.cs logic
```

---

## C# AppBundle (apps/da-bundle)

```
apps/da-bundle/
  SplitWallsDA.csproj    .NET 4.8, multi-target via build config
  SplitWallsDA.addin
  PackageContents.xml

  App/
    SplitWallsApp.cs     IExternalDBApplication entry point

  Services/
    SkillReader.cs       Deserialize skill.json (System.Web.Script.Serialization)
    UniformSplitService.cs  SplitWallUniform() + SplitWallAt() — extracted from SplitWalls addin

  Helpers/
    RevitUnitHelper.cs   MmToFeet(mm) = mm / 304.8
    WallJoinHelper.cs    DisableJoins(wall, doc)

  Models/
    MvpSkill.cs          C# model matching MvpSkill TypeScript interface
```

**Build produces 3 zip bundles:**
- `SplitWallsDA_2022.zip` (Revit 2022 SDK refs)
- `SplitWallsDA_2023.zip` (Revit 2023 SDK refs)
- `SplitWallsDA_2024.zip` (Revit 2024 SDK refs)

**AppBundle only talks to the outside world via OSS.** APS DA injects the OSS files at startup. No HTTP calls in the C# code.

---

## Implementation Roadmap

Build phases are designed so you use the admin workbench to develop and test the AppBundle before wiring up the user flow.

### Phase A: Project Scaffold (3-4 days) ← START HERE

1. `pnpm create turbo@latest splitwalls-cloud` → pnpm workspaces
2. Create `apps/web` (Next.js 14), `apps/api` (NestJS), `packages/types`
3. NestJS: `ClerkAuthGuard`, `AdminGuard`, `PrismaService`, `ApsAuthService`, `GET /api/health`
4. Next.js: Clerk middleware, landing page, auth pages, `lib/api-client.ts`
5. Deploy: NestJS → Railway, Next.js → Vercel
6. Verify: health endpoint works + Vercel loads

### Phase D: Admin DA Workbench (5-7 days) ← Build this SECOND

Build the workbench so you can develop the C# AppBundle with visual feedback.

### Phase E: Viewer Integration (4-6 days) ← Third

Viewer is shared — admin workbench needs it. Build once, reuse everywhere.

### Phase C: C# AppBundle (5-7 days) — CRITICAL PATH

Develop + test using admin workbench + viewer. Do NOT proceed to user flow until AppBundle works.

### Phase B: Upload + Configure Flow (5-7 days)

User upload, 2D Profile Editor, execute button.

### Phase F: Execute + Status + Result (4-5 days)

User execute flow, status polling, result viewer.

### Phase G: Social Gate + Polish (2-3 days)

Share dialog, download gating, error states.

**Total: ~30-43 days**

---

## Key Implementation Details

### ClerkAuthGuard (NestJS)

```typescript
// apps/api/src/auth/clerk.guard.ts
import { createClerkClient } from '@clerk/clerk-sdk-node';

@Injectable()
export class ClerkAuthGuard implements CanActivate {
  private clerk = createClerkClient({ secretKey: process.env.CLERK_SECRET_KEY });

  async canActivate(context: ExecutionContext): Promise<boolean> {
    const req = context.switchToHttp().getRequest();
    const token = req.headers.authorization?.split('Bearer ')[1];
    if (!token) throw new UnauthorizedException();
    try {
      const payload = await this.clerk.verifyToken(token);
      req.userId = payload.sub;
      req.userRole = (payload as any).metadata?.role || 'user';
      return true;
    } catch {
      throw new UnauthorizedException();
    }
  }
}
```

### ApsAuthService (NestJS — token cache)

```typescript
// apps/api/src/aps/aps-auth.service.ts
@Injectable()
export class ApsAuthService {
  private cache: { token: string; expiresAt: number } | null = null;

  async getToken(): Promise<string> {
    const now = Date.now();
    if (this.cache && this.cache.expiresAt - now > 5 * 60 * 1000) {
      return this.cache.token;
    }
    const res = await fetch('https://developer.api.autodesk.com/authentication/v2/token', {
      method: 'POST',
      headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
      body: new URLSearchParams({
        grant_type: 'client_credentials',
        client_id: process.env.APS_CLIENT_ID!,
        client_secret: process.env.APS_CLIENT_SECRET!,
        scope: 'data:read data:write data:create bucket:read bucket:create viewables:read',
      }),
    });
    const data = await res.json();
    this.cache = { token: data.access_token, expiresAt: now + data.expires_in * 1000 };
    return this.cache.token;
  }
}
```

### api-client.ts (Next.js — typed fetch wrapper)

```typescript
// apps/web/lib/api-client.ts
const BASE_URL = process.env.NEXT_PUBLIC_API_URL!;

export async function apiRequest<T>(
  path: string,
  token: string,
  opts: RequestInit = {}
): Promise<T> {
  const res = await fetch(BASE_URL + path, {
    ...opts,
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`,
      ...opts.headers,
    },
  });
  if (!res.ok) {
    const err = await res.json().catch(() => ({ message: res.statusText }));
    throw new Error(err.message || `API error ${res.status}`);
  }
  return res.json();
}
```

### Upload flow (browser → NestJS → APS OSS)

```
1. Browser calls: POST /api/jobs/upload-url { revitVersion, fileName, fileSizeBytes }
   → NestJS generates signed PUT URL via ApsOssService.getSignedUrl(key, 'upload', 3600)
   → NestJS creates Job record (status: 'uploaded')
   → Returns { jobId, signedUrl, ossUrn }

2. Browser PUT signedUrl (direct to APS OSS, no size limit, no timeout)

3. Browser calls: POST /api/jobs/:jobId/execute { config: { panelWidthMm, ... } }
   → NestJS builds skill.json from config
   → NestJS uploads skill.json to OSS
   → NestJS creates WorkItem on APS DA
   → Returns { jobId, workItemId }

4. Browser polls: GET /api/jobs/:jobId/status every 5s
   → NestJS polls APS DA WorkItem status
   → Updates Job record in MongoDB
   → Returns { status, progress, panelCount? }
```

### Social Gate (download unlock)

```typescript
// User clicks [Share on X] button:
const shareUrl = `https://x.com/intent/tweet?text=${encodeURIComponent(message)}`;
const w = window.open(shareUrl, '_blank', 'width=600,height=400');

// Poll for window close:
const timer = setInterval(async () => {
  if (w?.closed) {
    clearInterval(timer);
    await apiRequest('/api/jobs/' + jobId + '/social/verify', token, {
      method: 'POST',
      body: JSON.stringify({ platform: 'x' }),
    });
    setDownloadReady(true);
  }
}, 500);
```

---

## APS Viewer SDK Integration (Next.js)

The APS Viewer SDK is a client-side JavaScript library loaded from Autodesk's CDN. It calls APS directly from the browser using a short-lived token from NestJS.

```typescript
// apps/web/components/ViewerProvider.tsx
// Loads Viewer SDK script once, initializes Autodesk.Viewing

// apps/web/components/SingleViewer.tsx
// Props: { viewableUrn: string }
// 1. GET /api/viewer/token → { accessToken }
// 2. Autodesk.Viewing.Initializer({ env: 'AutodeskProduction', accessToken })
// 3. new Autodesk.Viewing.Viewer3D(container, { headless: true }) — no toolbar
// 4. viewer.loadModel(viewableUrn)

// apps/web/components/SplitViewer.tsx
// Two Viewer3D instances side by side (before / after)
// "Sync cameras" button copies camera state left → right
// "Compare with original" button lazy-loads the input viewer
```

---

## DA Report Parser

```typescript
// apps/api/src/admin/da-report-parser.ts
// Never throws — tolerant by design (DA report format is undocumented)
// Truncates at 1MB to prevent memory issues
// Returns ParsedReport (from packages/types)

function parse(rawText: string): ParsedReport {
  const text = rawText.length > 1_000_000 ? rawText.slice(0, 1_000_000) + '\n[TRUNCATED]' : rawText;
  const lines: ParsedLogLine[] = text.split('\n').map(parseOneLine);
  return {
    lines,
    errors: lines.filter(l => l.level === 'error'),
    warnings: lines.filter(l => l.level === 'warn'),
    summary: extractSummary(lines),
  };
}
```

---

## Credentials Setup

1. **Clerk** → https://clerk.com → Create app → Enable Google + Microsoft + Email
   - Copy `NEXT_PUBLIC_CLERK_PUBLISHABLE_KEY` (Vercel) + `CLERK_SECRET_KEY` (Railway)
   - Set yourself as admin: Dashboard → Users → your user → Metadata → `{ "role": "admin" }`

2. **APS** → https://aps.autodesk.com → Create app → Enable Data Management + Model Derivative + Design Automation
   - Request Design Automation access (may need to submit form for Flex tokens)
   - Copy `APS_CLIENT_ID` + `APS_CLIENT_SECRET` (Railway only)
   - Set DA nickname (one-time):
     ```bash
     curl -X PATCH https://developer.api.autodesk.com/da/us-east/v3/forgeapps/me \
       -H "Authorization: Bearer {token}" \
       -H "Content-Type: application/json" \
       -d '{ "nickname": "splitwalls" }'
     ```

3. **MongoDB Atlas** → https://cloud.mongodb.com → Create M0 free cluster
   - Create DB user, whitelist `0.0.0.0/0` (Vercel serverless IPs are dynamic)
   - Copy connection string → `DATABASE_URL` (Railway only)
   - Run `cd apps/api && npx prisma db push` to create collections

4. **Railway** → https://railway.app → New project → Deploy from GitHub (`splitwalls-cloud`)
   - Set root directory: `apps/api`
   - Add all Railway env vars (see env vars section above)
   - Set healthcheck: `GET /api/health`

5. **Vercel** → https://vercel.com → Import `splitwalls-cloud`
   - Set root directory: `apps/web`
   - Add all Vercel env vars (see env vars section above)

---

## Repository Relationship

```
SplitWalls (existing repo — C# addin)      splitwalls-cloud (NEW repo — web app)
──────────────────────────────────────      ──────────────────────────────────────
Revit desktop addin (.NET 4.8)              apps/web (Next.js 14 on Vercel)
WPF Profile Editor                          apps/api (NestJS on Railway)
No cloud code                               apps/da-bundle (C# .NET 4.8 AppBundle)
                                            packages/types (shared TS types)
```

The `apps/da-bundle` C# code is a **new, separate project** from the SplitWalls desktop addin — it shares logic (RevitUnitHelper, WallJoinHelper, split service) but is built as an IExternalDBApplication (no UI, no ribbon, no WPF) specifically for APS Design Automation headless execution.

---

## What the AppBundle Does

The AppBundle runs inside APS DA cloud Revit engine. Entry point: `SplitWallsApp : IExternalDBApplication`.

```csharp
// Execution flow:
// 1. Read skill.json (via SkillReader.Load("skill.json"))
// 2. Open input.rvt (app.OpenDocumentFile("input.rvt"))
// 3. Collect ALL Wall elements (FilteredElementCollector)
// 4. For each wall:
//    if method == "custom" && splitPointsMm.Length > 0:
//      splitter.SplitWallAt(wall, skill.GlobalConfig.SplitRule.SplitPointsMm)
//    else:
//      splitter.SplitWallUniform(wall, panelWidthMm, separatorWidthMm)
// 5. Write panel count to report.txt
// 6. doc.SaveAs("result.rvt")
```

The UniformSplitService logic is extracted from `ThisApplication.cs` `BUTTON_GENERAL()` → `Muro_sin_Ventanas = true` path in the SplitWalls desktop addin.

---

## Key Decisions Made (for reference)

| # | Decision | What was chosen | Why |
|---|----------|----------------|-----|
| 19 | Backend | NestJS from day 1 | Timeouts, persistent cache, WebSocket-ready |
| 20 | Repo structure | Turborepo monorepo | Shared TypeScript types, one CI pipeline |
| 21 | NestJS hosting | Railway | Simplest persistent server hosting, free tier |
| 22 | Client↔API | Browser calls NestJS directly | No proxy, no Vercel timeout on any path |
| 23 | Type safety | packages/types shared TS interfaces | Compile-time API contract enforcement |
| 24 | Auth in NestJS | ClerkAuthGuard verifies JWT | NestJS owns all auth logic |
| 25 | Real-time status | Deferred to Phase 2 | SSE trivial on NestJS Railway; polling is fine for MVP |
| Swagger | API docs | Deferred to Phase 2 | ~30 min to enable once API is stable |

---

## Full Plan

See `docs/plans/splitwalls-beta-mvp-plan.md` (v5) for:
- Complete user flow (step-by-step)
- Admin DA Workbench flow
- Full skill.json schema
- C# AppBundle entry point + split service spec
- ViewCache smart translation caching
- DA Report Parser spec
- Social gate implementation
- Success metrics
- Risk registry
- All engineering decisions (#1–#25)
