# Phase 2 Tracking — Extract WindowDetectionService

**Branch:** `refactor/phase0-cleanup` (continuing same refactor branch)
**Goal:** Replace copy-pasted `FilteredElementCollector + BuiltInCategory` window/door detection blocks with a single service class.
**Last Updated:** 2026-03-10
**Status:** COMPLETE

---

## Tasks

| # | Task | Status | Notes |
|---|------|--------|-------|
| P2-1 | Create `Services/WindowDetectionService.cs` | ✅ DONE | Two overloads: `GetHostedOpenings(doc, wall)` and `GetHostedOpenings(doc, view, wall)` |
| P2-2 | Register in `SplitWalls.csproj` | ✅ DONE | Added `<Compile Include="Services\WindowDetectionService.cs" />` |
| P2-3 | Replace 9× detection blocks → service call | ✅ DONE | Python script; 7 use `doc`, 2 use `doc, activeView` |
| P2-4 | Build verify (zero errors) | ✅ DONE | Only pre-existing warnings (unused locals, CS0168/CS0219/CS8321) |

---

## Results

| Metric | Before | After |
|--------|--------|-------|
| `ThisApplication.cs` lines | 16,603 | 16,342 (−261) |
| Detection blocks | 9 | 0 |
| `WindowDetectionService.GetHostedOpenings` calls | 0 | 9 |
| New files | — | `Services/WindowDetectionService.cs` |

---

## Pattern Replaced

Two variants of the same 20-line block, differing only in collector scope:

**Variant A — whole document (7 occurrences):**
```csharp
// BEFORE (20 lines):
BuiltInCategory[] bics_familyIns = new BuiltInCategory[] { OST_Doors, OST_Windows };
List<Element> windows_hosted = new List<Element>();
foreach (BuiltInCategory bic in bics_familyIns) { ... FilteredElementCollector(doc) ... }

// AFTER (1 line):
List<Element> windows_hosted = WindowDetectionService.GetHostedOpenings(doc, wall);
```

**Variant B — active view (2 occurrences):**
```csharp
// BEFORE: FilteredElementCollector(doc, activeView.Id)
// AFTER:
List<Element> windows_hosted = WindowDetectionService.GetHostedOpenings(doc, activeView, wall);
```

---

## Next Phase

**Phase 3:** Eliminate void/`_return` method pairs (~40 pairs, ~6,000–8,000 lines).
Tracking: `docs/plans/phase3-tracking.md`
