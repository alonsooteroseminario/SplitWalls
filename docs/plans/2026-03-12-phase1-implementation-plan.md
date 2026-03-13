# Phase 1 Implementation Plan: WPF Profile Editor

**Design:** `docs/plans/2026-03-12-phase1-wpf-profile-editor-design.md`
**Branch:** `feature/phase1-wpf-profile-editor`

---

## Implementation Steps

Each step is designed to be executed in a separate prompt/session. Steps 1-5 are independent and can run in **parallel worktrees**. Steps 6-8 are sequential (depend on earlier steps).

### Step 1: Data Models (independent)
**Branch:** `feature/phase1-step1-models`
**Files:** `Models/WallProfileConfig.cs`, `Models/ProfileDefaults.cs`, `Models/SplitRule.cs`, `Models/SegmentDef.cs`, `Models/OpeningDef.cs`
**Tasks:**
- [ ] Create `Models/` folder (already exists from PanelOptions.cs)
- [ ] Create `WallProfileConfig.cs` with all properties
- [ ] Create `ProfileDefaults.cs` with default values
- [ ] Create `SplitRule.cs` with Method, PanelWidthMm, SplitPointsMm
- [ ] Create `SegmentDef.cs` with Index, Start/End/Width, Profile, Label, FireRating
- [ ] Create `OpeningDef.cs` with Index, X/Y, Width/Height, Type
- [ ] Add Newtonsoft.Json NuGet package reference to .csproj
- [ ] Build and verify zero errors
**Acceptance:** All 5 model classes compile, Newtonsoft.Json reference added.

### Step 2: ProfileFileService (independent)
**Branch:** `feature/phase1-step2-file-service`
**Files:** `Services/ProfileFileService.cs`
**Tasks:**
- [ ] Create `ProfileFileService.cs` in Services/
- [ ] Implement `Save(config, filePath)` — serialize to JSON, write to file
- [ ] Implement `Load(filePath)` — read file, deserialize to WallProfileConfig
- [ ] Implement `ListProfiles()` — scan default folder for *.txt
- [ ] Implement `CreateTemplate(strategy)` — return starter config for osb/smartPanel/noWindows
- [ ] Default folder constant: `Documents\SplitWalls\Profiles\`
- [ ] Auto-create directory on Save and ListProfiles
- [ ] Build and verify zero errors
**Acceptance:** Service compiles, can serialize/deserialize a template to JSON .txt.
**Depends on:** Step 1 (models) — but can be developed in parallel if models are copied.

### Step 3: WPF UI — RelayCommand + ViewModel (independent)
**Branch:** `feature/phase1-step3-wpf-viewmodel`
**Files:** `UI/RelayCommand.cs`, `UI/ProfileEditorViewModel.cs`
**Tasks:**
- [ ] Create `UI/` folder
- [ ] Create `RelayCommand.cs` — standard ICommand implementation
- [ ] Create `ProfileEditorViewModel.cs` with INotifyPropertyChanged
- [ ] Implement all bound properties delegating to WallProfileConfig
- [ ] Implement ObservableCollection<SegmentDef> and ObservableCollection<OpeningDef>
- [ ] Implement commands: New, Load, Save, SaveAs, AutoSplit, AddOpening, RemoveOpening
- [ ] Implement AutoSplit logic (uniform segment generation)
- [ ] RedrawRequested event for canvas updates
- [ ] Build and verify zero errors
**Acceptance:** ViewModel compiles, all commands wired, AutoSplit generates segments.
**Depends on:** Step 1 (models), Step 2 (file service).

### Step 4: WPF UI — XAML Window + Canvas Renderer (independent)
**Branch:** `feature/phase1-step4-wpf-window`
**Files:** `UI/ProfileEditorWindow.xaml`, `UI/ProfileEditorWindow.xaml.cs`, `UI/ProfileCanvasRenderer.cs`
**Tasks:**
- [ ] Create `ProfileEditorWindow.xaml` — two-column layout (form + canvas)
- [ ] Left panel: all form controls (name, strategy, defaults, split rule, segments grid, openings list, action buttons)
- [ ] Right panel: Canvas with white background, border
- [ ] Create code-behind: wire ViewModel, subscribe to RedrawRequested, handle Canvas.SizeChanged
- [ ] Create `ProfileCanvasRenderer.cs` — static Render method
- [ ] Implement wall outline drawing
- [ ] Implement segment drawing with profile-type color coding (7 colors)
- [ ] Implement opening drawing (blue rectangles)
- [ ] Implement dimension labels (segment widths, total length)
- [ ] Auto-scale to fit canvas with margins
- [ ] Add all new files to .csproj (XAML Page, Compile)
- [ ] Build and verify zero errors
**Acceptance:** Window opens, form controls bind to ViewModel, canvas renders segments and openings.
**Depends on:** Step 3 (ViewModel).

### Step 5: Ribbon Integration — OpenProfileEditorCommand (independent)
**Branch:** `feature/phase1-step5-ribbon-command`
**Files:** `Commands/OpenProfileEditorCommand.cs`, `App.cs` (modified)
**Tasks:**
- [ ] Create `Commands/` folder
- [ ] Create `OpenProfileEditorCommand.cs` implementing IExternalCommand
- [ ] Execute method: instantiate ProfileEditorWindow, ShowDialog()
- [ ] Modify `App.cs`: add "Profile Editor" PushButton to ribbon panel
- [ ] Set button tooltip and image (or text fallback)
- [ ] Add to .csproj
- [ ] Build and verify zero errors
**Acceptance:** New ribbon button appears in Revit, clicking it opens the WPF editor window.
**Depends on:** Step 4 (WPF window).

### Step 6: Form1 "Load Profile" Integration (sequential)
**Branch:** `feature/phase1-step6-form1-load`
**Files:** `Form1.cs` (modified), `Form1.Designer.cs` (modified)
**Tasks:**
- [ ] Add "Load Profile" button to Form1 layout
- [ ] Add label showing loaded profile name
- [ ] Implement btnLoadProfile_Click: OpenFileDialog → ProfileFileService.Load()
- [ ] Expose `LoadedProfile` property (WallProfileConfig)
- [ ] Update AnchoPanel and other form controls from loaded profile
- [ ] Build and verify zero errors
**Acceptance:** Form1 shows "Load Profile" button, loading a .txt populates controls and sets LoadedProfile.
**Depends on:** Step 1 (models), Step 2 (file service).

### Step 7: ProfileExecutionService (sequential)
**Branch:** `feature/phase1-step7-execution-service`
**Files:** `Services/ProfileExecutionService.cs`
**Tasks:**
- [ ] Create `ProfileExecutionService.cs` in Services/
- [ ] Implement `Execute(config, walls)` — iterate walls with Transaction per wall
- [ ] Implement `ExecuteWall(config, wall)` — read wall geometry, compute splits, route by strategy
- [ ] Implement `ExecuteNoWindows` — split wall, apply standard profile to each segment
- [ ] Implement `ExecuteWithOpenings` — detect/use openings, determine profile per segment
- [ ] Implement `ApplyProfile` — switch on profile string → WallProfileBuilder.BuildXxx()
- [ ] Implement `ComputeUniformSplits` — generate split points from panel width
- [ ] Implement `DetermineProfile` — logic to pick U/T/I/L/borde based on opening overlap
- [ ] Build and verify zero errors
**Acceptance:** Service compiles, strategy routing works, profile mapping covers all 7 types.
**Depends on:** Step 1 (models), existing WallProfileBuilder and WindowDetectionService.

### Step 8: ThisApplication.cs Integration (sequential, final)
**Branch:** `feature/phase1-step8-integration`
**Files:** `ThisApplication.cs` (modified)
**Tasks:**
- [ ] Add profile-driven execution path in BUTTON_GENERAL or equivalent entry point
- [ ] Check if `Form1.LoadedProfile != null` → use ProfileExecutionService
- [ ] Else → fall through to existing hardcoded logic (backward compatible)
- [ ] Wire up wall selection → ProfileExecutionService.Execute(loadedProfile, selectedWalls)
- [ ] Build and verify zero errors
- [ ] Test: load a profile .txt → select walls → execute → verify correct profiles applied
**Acceptance:** Full pipeline works: load .txt → select walls → execute with correct strategy and profiles.
**Depends on:** Steps 6, 7.

---

## Parallelization Strategy

```
Steps 1, 2 ──────────────────► can run in parallel (independent)
     │
     ▼
Steps 3, 6, 7 ───────────────► can run in parallel (all depend only on 1+2)
     │
     ▼
Step 4 ───────────────────────► depends on Step 3
     │
     ▼
Step 5 ───────────────────────► depends on Step 4
     │
     ▼
Step 8 ───────────────────────► depends on Steps 6 + 7 (final integration)
```

**Optimal parallel execution:**
- **Wave 1:** Steps 1 + 2 (parallel worktrees)
- **Wave 2:** Steps 3 + 6 + 7 (parallel worktrees, merge wave 1 first)
- **Wave 3:** Step 4 (merge step 3 first)
- **Wave 4:** Step 5 (merge step 4 first)
- **Wave 5:** Step 8 (merge steps 6 + 7 first, final integration)

## Build Verification

After each step, run:
```powershell
powershell.exe -Command "
$src = '\\wsl.localhost\Ubuntu-22.04\home\alonsooteroseminario\source\repos\SplitWalls'
$tmp = 'C:\Temp\SplitWalls_build'
if (Test-Path $tmp) { Remove-Item $tmp -Recurse -Force }
Copy-Item $src $tmp -Recurse -Force
cd $tmp
& 'C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe' SplitWalls.csproj /p:Configuration=Release /nologo /verbosity:minimal 2>&1 | Select-String -NotMatch 'MSB3270|Copyright|^$'
"
```

Expected: zero errors. Acceptable warnings: MSB3270 (arch), CS0168/CS0219/CS8321 (pre-existing unused vars).

## Merge Order

1. Merge step branches into `feature/phase1-wpf-profile-editor` (integration branch)
2. After all steps pass, merge `feature/phase1-wpf-profile-editor` → `refactor/phase0-cleanup`
3. Eventually merge into `master`
