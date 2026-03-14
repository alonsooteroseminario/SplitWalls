# SplitWalls — Deferred Work Items

Items explicitly considered and deferred during plan review. Each has context sufficient for someone to pick up the work in 3 months.

---

## TODO-1: 2D Profile Editor — Undo/Redo

**What:** Add Ctrl+Z / Ctrl+Y undo/redo for split point drag operations on the 2D canvas.

**Why:** Users experimenting with custom splits need a way to revert accidental drags without resetting the entire layout to uniform.

**Pros:** Small effort, high usability improvement, standard UX expectation for any design tool.

**Cons:** Adds state management complexity (history stack). Need to decide scope: just drags, or also profile type changes?

**Context:** The canvas state is a simple `SegmentDef[]` array. Implementing undo/redo means storing previous states in a stack (max ~50 entries). The `useProfileCalculator` hook returns new state on every change — push old state onto undo stack before replacing. React's `useReducer` is a natural fit. The [Reset to Uniform] button already exists as a full reset fallback.

**Effort:** S (2 hours)
**Priority:** P3
**Phase:** 2
**Depends on:** Phase B (2D Profile Editor) must be built first.

---

## TODO-2: Extract Real Wall Dimensions from Model Derivative

**What:** After .rvt upload + Model Derivative translation, query the properties API to extract actual wall element lengths. Show a wall picker dropdown on the Configure page.

**Why:** The MVP uses a generic preview wall (default 5200mm). Real wall dimensions let users preview the exact split pattern for each wall in their model — much more useful than a placeholder.

**Pros:** Transforms the 2D editor from "approximate preview" to "exact preview per wall." Enables per-wall configuration (Phase 2 prerequisite).

**Cons:** Requires Model Derivative properties API integration (+1 API route, +1 day). Wall elements may have complex geometry (curved walls, angled). Properties API returns large JSON payloads.

**Context:** Model Derivative properties API: `GET /modelderivative/v2/designdata/{urn}/metadata/{guid}/properties`. Filter for elements where `objectid` matches `Wall` category. Extract `Length` property from element properties. The ViewCache already stores the `viewableUrn` — extend it with extracted wall metadata. Store as JSON array on the Job record or a new WallMetadata model.

**Effort:** M (1-2 days)
**Priority:** P2
**Phase:** 2
**Depends on:** Phase E (Viewer integration) — needs Model Derivative translation to complete first.

---

## TODO-3: Profile Type Execution in C# AppBundle

**What:** Extend the AppBundle to create profile-specific wall configurations (U, L_left, L_right, T, I, borde) instead of only standard rectangular splits.

**Why:** The 2D editor lets users assign profile types visually, but the AppBundle ignores them. This is the core differentiator of SplitWalls — the existing desktop app supports 7+ profile types with complex routing logic.

**Pros:** Makes SplitWalls unique in the market. Leverages existing `WallProfileBuilder.cs` (28 methods, 2,676 lines) and `ProfileExecutionService.cs` stubs. The WPF Phase 1 data models already define all profile types.

**Cons:** Complex — profile types interact with opening-aware routing (`osb`, `smartPanel` strategies). `ProfileExecutionService.RouteSegmentProfile()` is currently a TODO stub. Need real Revit wall geometry manipulation per profile type. Effort: L (3-5 days + testing).

**Context:** The existing `WallProfileBuilder.cs` has 28 extracted methods that build specific wall profiles. The `ProfileExecutionService.cs` has 4 TODO stubs for `ApplySegmentProfiles()` and `RouteSegmentProfile()`. The skill.json `globalConfig.strategy` field already supports `"osb"` and `"smartPanel"` in addition to `"noWindows"`. Start with `standard` + `U` profiles, then add `L`, `T`, `I` incrementally.

**Effort:** L (3-5 days)
**Priority:** P1
**Phase:** 2
**Depends on:** TODO-2 (real wall dims) for per-wall profile assignment. Phase C (AppBundle) must be working with uniform splits first.

---

## TODO-4: Opening (Window/Door) Overlay on 2D Canvas

**What:** Add visual rendering of windows and doors on the 2D profile editor canvas, matching the WPF `ProfileCanvasRenderer` behavior.

**Why:** Openings affect how wall segments should be profiled. Visualizing them prepares users for opening-aware routing (Phase 2) and provides more accurate split previews.

**Pros:** The WPF renderer already has opening rendering logic (`OpeningDef` model, semi-transparent blue rectangles with type labels). Direct port to SVG. Small effort. Visual value even without backend support.

**Cons:** For MVP, openings are visual-only — no effect on execution. Users may be confused that openings appear but don't affect the split result.

**Context:** `OpeningDef` model has `XMm`, `YMm`, `WidthMm`, `HeightMm`, `Type` (window/door). WPF renders them as semi-transparent blue rectangles overlaid on wall segments. In the web editor: add SVG `<rect>` elements with `opacity="0.5"` and `fill="#93c5fd"`. Users can add openings manually (click + drag) or they could be extracted from Model Derivative properties (depends on TODO-2). For MVP, manual placement only.

**Effort:** S (1 day)
**Priority:** P2
**Phase:** 2
**Depends on:** Phase B (2D Profile Editor), TODO-2 (for auto-detection from .rvt).

---

## TODO-5: Profile Library — Save/Load/Share Configurations

**What:** Let users save named split configurations (e.g., "OSB Standard 1220mm", "SmartPanel 610mm") and reuse them across jobs. Foundation for a profile marketplace.

**Why:** Returning users re-enter the same settings every time. Named profiles enable one-click reuse and sharing. This is where SplitWalls becomes a platform.

**Pros:** Creates network effects (shared profiles attract new users). Enables template library. Foundation for marketplace monetization (Phase 3+). Small DB schema addition.

**Cons:** Premature for a beta with 0 users. Profile compatibility across Revit versions is uncertain. Needs CRUD UI + API routes. Sharing/public profiles need moderation.

**Context:** New Prisma model: `Profile { id, userId, name, config: Json, isPublic: Boolean, usageCount: Int, createdAt, updatedAt }`. CRUD API: `POST /api/profiles`, `GET /api/profiles`, `PUT /api/profiles/:id`, `DELETE /api/profiles/:id`. UI: profile picker dropdown on Configure page, "Save as Profile" button after editing, optional "Browse Public Profiles" page. The `config` field stores the full `WallProfileConfig` equivalent (segments, split rule, defaults).

**Effort:** M (2-3 days)
**Priority:** P3
**Phase:** 3
**Depends on:** Phase B (2D Profile Editor), TODO-3 (profile types in AppBundle) for meaningful saved profiles.
