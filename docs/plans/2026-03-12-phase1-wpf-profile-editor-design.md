# Phase 1 Design: WPF Profile Editor + Data-Driven Execution

**Date:** 2026-03-12
**Branch:** `feature/phase1-wpf-profile-editor`
**Status:** Design Approved — Awaiting Implementation

---

## Goal

Replace hardcoded wall profile strategies with a data-driven system:
1. A **WPF editor** lets users define wall split profiles via form inputs + 2D canvas preview
2. Profiles are saved as **`.txt` files** (JSON content, `skill.json` schema) in `Documents\SplitWalls\Profiles\`
3. Users **load** a `.txt` file from Form1 to configure execution
4. `ProfileExecutionService` reads the config and drives existing `WallProfileBuilder` methods
5. Aligns 1:1 with the cloud `skill.json` contract for future migration

## Decisions

| Decision | Choice | Rationale |
|----------|--------|-----------|
| File format | JSON in `.txt` (skill.json schema) | Zero rework for cloud migration |
| UI interactivity | Simplified visual preview + form inputs | Faster to build; full drag-and-drop in later phase |
| Default file location | `%USERPROFILE%\Documents\SplitWalls\Profiles\` | Intuitive, no permission issues |
| Integration model | WPF editor = separate ribbon button; Form1 gains "Load Profile" | Safe coexistence, no breakage |
| Profiles per file | One profile template per file | Simpler, more reusable |
| Architecture | MVVM WPF + Shared Models | Clean separation, testable, cloud-ready |
| JSON library | Newtonsoft.Json | .NET 4.8 standard, reliable |

## File & Folder Structure

```
SplitWalls/
├── Models/
│   ├── WallProfileConfig.cs      # Root config (maps to skill.json)
│   ├── SplitRule.cs               # Split method + panel width + split points
│   ├── SegmentDef.cs              # Per-segment: profile shape, label, fire rating
│   ├── OpeningDef.cs              # Window/door: position, size, type
│   └── ProfileDefaults.cs         # Default values (panelWidth, separator, snapGrid)
├── Services/
│   ├── ProfileFileService.cs      # Load/Save/List .txt profiles
│   ├── ProfileExecutionService.cs # Maps WallProfileConfig → WallProfileBuilder calls
│   ├── WallProfileBuilder.cs      # (existing, unchanged)
│   └── WindowDetectionService.cs  # (existing, unchanged)
├── UI/
│   ├── ProfileEditorWindow.xaml   # WPF window: form inputs + Canvas preview
│   ├── ProfileEditorWindow.xaml.cs
│   ├── ProfileEditorViewModel.cs  # MVVM ViewModel
│   ├── ProfileCanvasRenderer.cs   # 2D visualization on Canvas
│   └── RelayCommand.cs            # ICommand helper for MVVM
├── Commands/
│   └── OpenProfileEditorCommand.cs # Revit IExternalCommand for ribbon button
├── Form1.cs                       # (modified: "Load Profile" button)
├── ThisApplication.cs             # (modified: profile-driven execution path)
└── App.cs                         # (modified: new ribbon button)
```

## Data Models

```csharp
public class WallProfileConfig
{
    public string Name { get; set; }           // "OSB Standard"
    public string Version { get; set; }        // "2.0"
    public string Created { get; set; }        // ISO date
    public string Strategy { get; set; }       // "noWindows" | "osb" | "smartPanel"
    public ProfileDefaults Defaults { get; set; }
    public SplitRule SplitRule { get; set; }
    public List<SegmentDef> Segments { get; set; }
    public List<OpeningDef> Openings { get; set; }
}

public class ProfileDefaults
{
    public double PanelWidthMm { get; set; }    // 1220
    public double SeparatorWidthMm { get; set; } // 4
    public double WallHeightMm { get; set; }     // 2440
    public bool DisableWallJoins { get; set; }    // true
    public double? SnapToGridMm { get; set; }     // 100 or null
}

public class SplitRule
{
    public string Method { get; set; }           // "uniform" | "custom"
    public double PanelWidthMm { get; set; }
    public List<double> SplitPointsMm { get; set; }
}

public class SegmentDef
{
    public int Index { get; set; }
    public double StartMm { get; set; }
    public double EndMm { get; set; }
    public double WidthMm { get; set; }
    public string Profile { get; set; }         // "standard"|"U"|"L_left"|"L_right"|"T"|"I"|"borde"
    public string Label { get; set; }
    public string FireRating { get; set; }      // "none"|"1hr"|"2hr"|"3hr"
}

public class OpeningDef
{
    public int Index { get; set; }
    public double XMm { get; set; }
    public double YMm { get; set; }
    public double WidthMm { get; set; }
    public double HeightMm { get; set; }
    public string Type { get; set; }            // "window" | "door"
}
```

## ProfileFileService

- **Save:** Serialize `WallProfileConfig` → JSON → `.txt` file
- **Load:** Deserialize `.txt` → `WallProfileConfig`
- **ListProfiles:** Scan default folder for `*.txt` files
- **CreateTemplate:** Generate starter profile for a given strategy
- Default folder: `Documents\SplitWalls\Profiles\`, auto-created on first save

## ProfileExecutionService

- Bridge between `WallProfileConfig` and existing `WallProfileBuilder`
- `Execute(config, walls)` — iterates walls, routes by strategy
- Strategy routing: `noWindows` → split only; `osb`/`smartPanel` → split + opening detection + profile application
- Profile mapping: `segment.Profile` string → `WallProfileBuilder.BuildXxx()` method call
- Openings: use config-defined openings OR auto-detect from wall via `WindowDetectionService`
- Each wall wrapped in its own `Transaction`

## WPF ProfileEditorWindow

**Layout:** Two-column — left form panel (300px), right 2D canvas preview (fill)

**Form Controls:**
- Profile name (TextBox)
- Strategy selector (ComboBox: noWindows, osb, smartPanel)
- Defaults group: panel width, separator, wall height (DoubleUpDown), disable joins (CheckBox)
- Split rule: method selector + Auto-Split button
- Segments: DataGrid with index, width, profile type, label columns
- Openings: list with add/remove, showing type + dimensions
- Actions: New, Load, Save, Save As buttons

**Canvas Preview (read-only):**
- Wall outline (gray rectangle)
- Segments color-coded by profile type (7 colors)
- Openings drawn as blue rectangles
- Dimension labels (width per segment, total wall length)
- Auto-scales to fit canvas, redraws on any property change

## ViewModel (MVVM)

- `ProfileEditorViewModel : INotifyPropertyChanged`
- Properties delegate to `WallProfileConfig` fields
- `ObservableCollection<SegmentDef>` and `ObservableCollection<OpeningDef>` for list binding
- Commands: New, Load, Save, SaveAs, AutoSplit, AddOpening, RemoveOpening
- `RedrawRequested` event triggers canvas re-render on any change

## Form1 Integration

- New "Load Profile" button with `OpenFileDialog` targeting default profiles folder
- `LoadedProfile` property exposes loaded `WallProfileConfig` to `ThisApplication.cs`
- Form controls update to reflect loaded profile values (panel width, etc.)

## Ribbon Integration (App.cs)

- New "Profile Editor" ribbon button → `OpenProfileEditorCommand`
- Existing "Split Walls" button unchanged
- `OpenProfileEditorCommand : IExternalCommand` opens `ProfileEditorWindow.ShowDialog()`

## Sample .txt Output

```json
{
  "Name": "OSB Standard",
  "Version": "2.0",
  "Created": "2026-03-12T10:30:00",
  "Strategy": "osb",
  "Defaults": {
    "PanelWidthMm": 1220.0,
    "SeparatorWidthMm": 4.0,
    "WallHeightMm": 2440.0,
    "DisableWallJoins": true,
    "SnapToGridMm": 100.0
  },
  "SplitRule": {
    "Method": "uniform",
    "PanelWidthMm": 1220.0,
    "SplitPointsMm": []
  },
  "Segments": [
    { "Index": 0, "StartMm": 0, "EndMm": 1220, "WidthMm": 1220, "Profile": "standard", "Label": "Panel A", "FireRating": "none" },
    { "Index": 1, "StartMm": 1224, "EndMm": 2444, "WidthMm": 1220, "Profile": "U", "Label": "Window Panel", "FireRating": "1hr" }
  ],
  "Openings": [
    { "Index": 0, "XMm": 1400, "YMm": 900, "WidthMm": 600, "HeightMm": 1200, "Type": "window" }
  ]
}
```

## Component Summary

| Component | Purpose | New/Modified |
|-----------|---------|-------------|
| 5 Model classes | Data-driven config matching skill.json | New |
| ProfileFileService | Load/Save/List .txt profiles | New |
| ProfileExecutionService | Maps config → WallProfileBuilder calls | New |
| ProfileEditorWindow.xaml | WPF form + canvas preview | New |
| ProfileEditorViewModel | MVVM bindings + commands | New |
| ProfileCanvasRenderer | 2D visualization on Canvas | New |
| RelayCommand | ICommand helper for MVVM | New |
| OpenProfileEditorCommand | Revit ribbon command | New |
| Form1.cs | "Load Profile" button + LoadedProfile property | Modified |
| App.cs | New ribbon button for editor | Modified |
| ThisApplication.cs | Profile-driven execution path alongside existing | Modified |
