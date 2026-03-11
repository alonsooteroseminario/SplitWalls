# Phase 10 Implementation Plan

> **For Claude:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Extract 28 wall-profile-builder local functions into `Services/WallProfileBuilder.cs`, rename `aaaaaa` → `wallPanel`, and delete the 4 unused CS0219 variable declarations.

**Architecture:** Python scripts (matching the approach used in phases 3–9) manipulate the file content deterministically. `WallProfileBuilder` takes `Document doc` in its constructor and all methods are `public` except `ReplaceWallWithProfile` (private). The remaining local functions in `Execute` capture `profileBuilder` from the outer scope, exactly like they currently capture `doc`.

**Tech Stack:** C# .NET 4.8, Revit 2022 API, Python 3 scripts for surgical edits, MSBuild via PowerShell bridge.

---

### Task 1: Rename `aaaaaa` → `wallPanel` + delete CS0219 vars

**Files:**
- Modify: `ThisApplication.cs`
- Create: `phase10a_renames.py`

**Step 1: Write the rename script**

```python
#!/usr/bin/env python3
"""Phase 10a: rename aaaaaa → wallPanel, delete VIo/VFo_previo_vacio."""
import re, sys

FILE = 'ThisApplication.cs'
with open(FILE, 'r', encoding='utf-8-sig') as f:
    content = f.read()

original_lines = content.count('\n')
print(f"Lines before: {original_lines}")

# 1. Rename aaaaaa → wallPanel
count = content.count('aaaaaa')
content = content.replace('aaaaaa', 'wallPanel')
print(f"Step 1: renamed {count} occurrences of aaaaaa → wallPanel")

# 2. Delete VIo_previo_vacio / VFo_previo_vacio declarations and assignments
# These are CS0219: assigned but never read. Pattern:
#   bool VIo_previo_vacio = false;
#   bool VFo_previo_vacio = false;
# and later:
#   VIo_previo_vacio = true;
#   VFo_previo_vacio = true;
dead_patterns = [
    r'[ \t]*bool VIo_previo_vacio = false;[ \t]*\n',
    r'[ \t]*bool VFo_previo_vacio = false;[ \t]*\n',
    r'[ \t]*VIo_previo_vacio = true;[ \t]*\n',
    r'[ \t]*VFo_previo_vacio = true;[ \t]*\n',
]
removed = 0
for pat in dead_patterns:
    new_content, n = re.subn(pat, '', content)
    removed += n
    content = new_content
print(f"Step 2: deleted {removed} VIo/VFo_previo_vacio lines")

after_lines = content.count('\n')
print(f"Lines after: {after_lines} (−{original_lines - after_lines})")

with open(FILE, 'w', encoding='utf-8') as f:
    f.write(content)
print("Done.")
```

**Step 2: Run it**

```bash
python3 phase10a_renames.py
```

Expected output:
```
Lines before: 10785
Step 1: renamed 403 occurrences of aaaaaa → wallPanel
Step 2: deleted 8 VIo/VFo_previo_vacio lines
Lines after: 10777 (−8)
Done.
```

**Step 3: Build verify**

```powershell
powershell.exe -Command "
\$src = '\\\\wsl.localhost\\Ubuntu-22.04\\home\\alonsooteroseminario\\source\\repos\\SplitWalls'
\$tmp = 'C:\\Temp\\SplitWalls_build'
if (Test-Path \$tmp) { Remove-Item \$tmp -Recurse -Force }
Copy-Item \$src \$tmp -Recurse -Force
cd \$tmp
& 'C:\\Program Files\\Microsoft Visual Studio\\2022\\Community\\MSBuild\\Current\\Bin\\MSBuild.exe' SplitWalls.csproj /p:Configuration=Release /nologo /verbosity:minimal 2>&1 | Select-String -NotMatch 'MSB3270|Copyright|^$'
"
```

Expected: `SplitWalls -> ...bin\Release\SplitWalls.dll` with no errors. The 4 CS0219 warnings for VIo/VFo_previo_vacio should be gone. Other pre-existing warnings remain.

**Step 4: Commit**

```bash
git add ThisApplication.cs
git commit -m "refactor(phase10a): rename aaaaaa→wallPanel, delete 4 unused CS0219 vars"
```

---

### Task 2: Create `Services/WallProfileBuilder.cs` + remove methods from ThisApplication.cs

**Files:**
- Create: `Services/WallProfileBuilder.cs`
- Modify: `ThisApplication.cs`
- Create: `phase10b_extract_service.py`

**Step 1: Write the extraction script**

The script uses brace counting (same pattern as phase3) to find each method's start and end in `ThisApplication.cs`, then builds `WallProfileBuilder.cs`.

```python
#!/usr/bin/env python3
"""
Phase 10b: Extract 28 wall-profile-builder local functions from
ThisApplication.cs into Services/WallProfileBuilder.cs.

For each method:
  - Strip the Revision6_DYNO_ prefix and _return suffix
  - Rename to the clean name
  - Replace `doc` word-boundary → `_doc` in the body
  - Make private for ReplaceWallWithProfile, public for everything else
"""
import re, sys, os

SRC  = 'ThisApplication.cs'
DEST = 'Services/WallProfileBuilder.cs'

# (old_signature_fragment, new_method_name, return_type, visibility)
# old_signature_fragment = enough of the declaration line to uniquely identify it
# Declarations start with 3 tabs + return_type + ' ' + old_name + '('
METHOD_MAP = [
    # private helper first
    ('Wall ReplaceWallWithProfile(',              'ReplaceWallWithProfile',               'Wall',  'private'),
    # profile builders
    ('Wall Revision6_DYNO_Wall_EditProfile_U_PUERTA_VENTANA_derecha_return(',   'BuildU_DoorWindowRight',             'Wall',  'public'),
    ('Wall Revision6_DYNO_Wall_EditProfile_U_PUERTA_PUERTA_derecha_return(',    'BuildU_DoorDoorRight',               'Wall',  'public'),
    ('Wall Revision6_DYNO_Wall_EditProfile_U_PUERTA_VENTANA_izquierda_return(', 'BuildU_DoorWindowLeft',              'Wall',  'public'),
    ('Wall Revision6_DYNO_Wall_EditProfile_U_PUERTA_PUERTA_izquierda_return(',  'BuildU_DoorDoorLeft',                'Wall',  'public'),
    ('Wall Revision6_DYNO_Wall_EditProfile_3VENT_V_P_V_return(',                'Build3Opening_WindowDoorWindow',     'Wall',  'public'),
    ('Wall Revision6_DYNO_Wall_EditProfile_3VENT_V_P_P_return(',                'Build3Opening_WindowDoorDoor',       'Wall',  'public'),
    ('Wall Revision6_DYNO_Wall_EditProfile_3VENT_P_P_V_return(',                'Build3Opening_DoorDoorWindow',       'Wall',  'public'),
    ('Wall Revision6_DYNO_Wall_EditProfile_3VENT_P_P_P_return(',                'Build3Opening_DoorDoorDoor',         'Wall',  'public'),
    ('Wall Revision6_DYNO_Create_New_Wall_EditProfile_BORDE_PUERTA_DERECHA_return(',   'BuildEdgeDoorRight',          'Wall',  'public'),
    ('Wall Revision6_DYNO_Create_New_Wall_EditProfile_BORDE_PUERTA_IZQUIERDO_return(', 'BuildEdgeDoorLeft',           'Wall',  'public'),
    ('Wall Revision6_DYNO_Wall_EditProfile_U_PUERTA_return(',                   'BuildU_Door',                        'Wall',  'public'),
    ('Wall Revision6_DYNO_Create_New_Wall_EditProfile_T_PUERTA_return(',        'BuildT_Door',                        'Wall',  'public'),
    ('Wall Revision6_DYNO_Create_New_Wall_EditProfile_I_PUERTA_dVIo_return(',   'BuildI_DoorLeft',                    'Wall',  'public'),
    ('Wall Revision6_DYNO_Create_New_Wall_EditProfile_I_PUERTA_dVFo_return(',   'BuildI_DoorRight',                   'Wall',  'public'),
    ('Wall Revision6_DYNO_Create_New_Wall_EditProfile_I_return(',               'BuildI',                             'Wall',  'public'),
    ('Wall Revision6_DYNO_Create_New_Wall_EditProfile_dVIo_return(',            'BuildWindowLeft',                    'Wall',  'public'),
    ('Wall Revision6_DYNO_Create_New_Wall_EditProfile_dVFo_return(',            'BuildWindowRight',                   'Wall',  'public'),
    ('Wall Revision6_DYNO_Create_New_Wall_EditProfile_dVIo_PUERTA_return(',     'BuildDoorLeft',                      'Wall',  'public'),
    ('Wall Revision6_DYNO_Create_New_Wall_EditProfile_dVFo_PUERTA_return(',     'BuildDoorRight',                     'Wall',  'public'),
    ('Wall Revision6_DYNO_Create_New_Wall_EditProfile_dVFo_PUERTA_CasoEspecial_return(', 'BuildDoorRightSpecialCase', 'Wall',  'public'),
    ('void Revision6_DYNO_Create_New_Wall_EditProfile_Solitario(',              'BuildSolitario',                     'void',  'public'),
    ('void Revision6_DYNO_Create_New_Wall_2MUROS_Solitario(',                   'BuildTwoWallSolitario',               'void',  'public'),
    ('Wall Revision6_DYNO_Create_New_Wall_2MUROS_Solitario_return(',            'BuildTwoWallSolitarioReturn',         'Wall',  'public'),
    ('void Revision6_DYNO_Create_New_Wall_1MURO_Solitario(',                    'BuildOneWallSolitario',               'void',  'public'),
    ('Wall Revision6_DYNO_Create_New_Wall_1MURO_Solitario_CasoEspecial_return(',         'BuildOneWallSolitarioSpecialCase',     'Wall', 'public'),
    ('Wall Revision6_DYNO_Create_New_Wall_1MURO_Solitario_CasoEspecial_FinalMuro_return(','BuildOneWallSolitarioSpecialEndWall', 'Wall', 'public'),
    ('Wall Revision6_DYNO_Create_New_Wall_1MURO_Solitario_CasoEspecial_InicialMuro_return(','BuildOneWallSolitarioSpecialStartWall','Wall','public'),
]

with open(SRC, 'r', encoding='utf-8-sig') as f:
    lines = f.readlines()

original_count = len(lines)
print(f"Lines before: {original_count}")

def find_and_extract(lines, sig_fragment):
    """Return (start_idx, end_idx) of the method containing sig_fragment."""
    start = None
    for i, line in enumerate(lines):
        if sig_fragment in line and not line.strip().startswith('//'):
            start = i
            break
    if start is None:
        return None, None
    # Brace-count to find closing }
    depth = 0
    for i in range(start, len(lines)):
        depth += lines[i].count('{') - lines[i].count('}')
        if depth == 0 and i > start:
            return start, i
    return None, None

# Extract method blocks
extracted_bodies = []  # list of (new_name, visibility, return_type, body_lines)
ranges_to_delete = []  # list of (start, end) to remove from ThisApplication.cs

for sig_frag, new_name, ret_type, visibility in METHOD_MAP:
    start, end = find_and_extract(lines, sig_frag)
    if start is None:
        print(f"ERROR: could not find '{sig_frag}'", file=sys.stderr)
        sys.exit(1)

    method_lines = lines[start:end + 1]

    # Replace the declaration line: change signature to use new name and visibility
    decl_line = method_lines[0]
    # Strip leading tabs (3 tabs for local function inside Execute)
    stripped = decl_line.lstrip('\t')
    # Build new declaration: remove old name fragment, insert new with visibility
    old_ret_and_name = ret_type + ' ' + sig_frag.split('(')[0].strip().split()[-1]
    new_decl = '\t\t' + visibility + ' ' + ret_type + ' ' + new_name + stripped[len(stripped.split('(')[0]):].lstrip()
    # More robust: just replace the old name fragment with the new name
    # The sig fragment includes the opening paren: sig_frag = "Wall OldName("
    old_name_in_decl = sig_frag.rstrip('(').split()[-1]  # e.g. "Revision6_DYNO_..."
    new_decl_line = '\t\t' + visibility + ' ' + ret_type + ' ' + new_name + decl_line[decl_line.index('('):]
    method_lines[0] = new_decl_line

    # Re-indent body: was at 3 tabs (local fn in Execute), move to 2 tabs (method in class)
    reindented = []
    for line in method_lines:
        if line.startswith('\t\t\t'):
            reindented.append('\t\t' + line[3:])
        elif line.startswith('\t\t'):
            reindented.append('\t' + line[2:])
        else:
            reindented.append(line)

    # Replace `doc` word-boundary → `_doc` in the body
    body_text = ''.join(reindented)
    body_text = re.sub(r'\bdoc\b', '_doc', body_text)
    # Restore the field name in case it over-replaced something (unlikely but safe)

    extracted_bodies.append((new_name, body_text))
    ranges_to_delete.append((start, end))
    print(f"  Extracted: {old_name_in_decl} → {new_name} (lines {start+1}–{end+1})")

# Build WallProfileBuilder.cs
class_methods = '\n'.join(body for _, body in extracted_bodies)

service_file = '''\
using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace SplitWalls
{
\tinternal class WallProfileBuilder
\t{
\t\tprivate readonly Document _doc;

\t\tinternal WallProfileBuilder(Document doc)
\t\t{
\t\t\t_doc = doc;
\t\t}

''' + class_methods + '''
\t}
}
'''

os.makedirs('Services', exist_ok=True)
with open(DEST, 'w', encoding='utf-8') as f:
    f.write(service_file)
print(f"\nWrote {DEST}")

# Delete extracted ranges from ThisApplication.cs (process in reverse order)
ranges_to_delete.sort(key=lambda x: x[0], reverse=True)
for start, end in ranges_to_delete:
    # Also eat trailing blank lines
    while end + 1 < len(lines) and lines[end + 1].strip() == '':
        end += 1
    del lines[start:end + 1]

# Collapse any resulting 3+ blank lines → 2
final = []
blanks = 0
for line in lines:
    if line.strip() == '':
        blanks += 1
        if blanks <= 2:
            final.append(line)
    else:
        blanks = 0
        final.append(line)

after_count = len(final)
print(f"ThisApplication.cs: {original_count} → {after_count} lines (−{original_count - after_count})")

with open(SRC, 'w', encoding='utf-8') as f:
    f.writelines(final)
print("Done.")
```

**Step 2: Run it**

```bash
python3 phase10b_extract_service.py
```

Expected output shows 28 methods extracted, `Services/WallProfileBuilder.cs` written, and a significant line reduction in `ThisApplication.cs`.

**Step 3: Verify `WallProfileBuilder.cs` was created**

```bash
wc -l Services/WallProfileBuilder.cs
head -15 Services/WallProfileBuilder.cs
```

---

### Task 3: Update call sites in `ThisApplication.cs` + add `profileBuilder` declaration

**Files:**
- Modify: `ThisApplication.cs`
- Create: `phase10c_callsites.py`

**Step 1: Write the call-site update script**

```python
#!/usr/bin/env python3
"""
Phase 10c: In ThisApplication.cs —
  1. Replace all old Revision6_DYNO_*_return( calls with profileBuilder.NewName(
  2. Add `var profileBuilder = new WallProfileBuilder(doc);` declaration
"""
import sys

FILE = 'ThisApplication.cs'

# Must match the METHOD_MAP from phase10b (all except ReplaceWallWithProfile
# which is now private in WallProfileBuilder and has no call sites left)
CALL_MAP = [
    ('Revision6_DYNO_Wall_EditProfile_U_PUERTA_VENTANA_derecha_return(',   'profileBuilder.BuildU_DoorWindowRight('),
    ('Revision6_DYNO_Wall_EditProfile_U_PUERTA_PUERTA_derecha_return(',    'profileBuilder.BuildU_DoorDoorRight('),
    ('Revision6_DYNO_Wall_EditProfile_U_PUERTA_VENTANA_izquierda_return(', 'profileBuilder.BuildU_DoorWindowLeft('),
    ('Revision6_DYNO_Wall_EditProfile_U_PUERTA_PUERTA_izquierda_return(',  'profileBuilder.BuildU_DoorDoorLeft('),
    ('Revision6_DYNO_Wall_EditProfile_3VENT_V_P_V_return(',                'profileBuilder.Build3Opening_WindowDoorWindow('),
    ('Revision6_DYNO_Wall_EditProfile_3VENT_V_P_P_return(',                'profileBuilder.Build3Opening_WindowDoorDoor('),
    ('Revision6_DYNO_Wall_EditProfile_3VENT_P_P_V_return(',                'profileBuilder.Build3Opening_DoorDoorWindow('),
    ('Revision6_DYNO_Wall_EditProfile_3VENT_P_P_P_return(',                'profileBuilder.Build3Opening_DoorDoorDoor('),
    ('Revision6_DYNO_Create_New_Wall_EditProfile_BORDE_PUERTA_DERECHA_return(',   'profileBuilder.BuildEdgeDoorRight('),
    ('Revision6_DYNO_Create_New_Wall_EditProfile_BORDE_PUERTA_IZQUIERDO_return(', 'profileBuilder.BuildEdgeDoorLeft('),
    ('Revision6_DYNO_Wall_EditProfile_U_PUERTA_return(',                   'profileBuilder.BuildU_Door('),
    ('Revision6_DYNO_Create_New_Wall_EditProfile_T_PUERTA_return(',        'profileBuilder.BuildT_Door('),
    ('Revision6_DYNO_Create_New_Wall_EditProfile_I_PUERTA_dVIo_return(',   'profileBuilder.BuildI_DoorLeft('),
    ('Revision6_DYNO_Create_New_Wall_EditProfile_I_PUERTA_dVFo_return(',   'profileBuilder.BuildI_DoorRight('),
    ('Revision6_DYNO_Create_New_Wall_EditProfile_I_return(',               'profileBuilder.BuildI('),
    ('Revision6_DYNO_Create_New_Wall_EditProfile_dVIo_return(',            'profileBuilder.BuildWindowLeft('),
    ('Revision6_DYNO_Create_New_Wall_EditProfile_dVFo_return(',            'profileBuilder.BuildWindowRight('),
    ('Revision6_DYNO_Create_New_Wall_EditProfile_dVIo_PUERTA_return(',     'profileBuilder.BuildDoorLeft('),
    ('Revision6_DYNO_Create_New_Wall_EditProfile_dVFo_PUERTA_return(',     'profileBuilder.BuildDoorRight('),
    ('Revision6_DYNO_Create_New_Wall_EditProfile_dVFo_PUERTA_CasoEspecial_return(', 'profileBuilder.BuildDoorRightSpecialCase('),
    ('Revision6_DYNO_Create_New_Wall_EditProfile_Solitario(',              'profileBuilder.BuildSolitario('),
    ('Revision6_DYNO_Create_New_Wall_2MUROS_Solitario_return(',            'profileBuilder.BuildTwoWallSolitarioReturn('),
    ('Revision6_DYNO_Create_New_Wall_2MUROS_Solitario(',                   'profileBuilder.BuildTwoWallSolitario('),
    ('Revision6_DYNO_Create_New_Wall_1MURO_Solitario_CasoEspecial_FinalMuro_return(', 'profileBuilder.BuildOneWallSolitarioSpecialEndWall('),
    ('Revision6_DYNO_Create_New_Wall_1MURO_Solitario_CasoEspecial_InicialMuro_return(', 'profileBuilder.BuildOneWallSolitarioSpecialStartWall('),
    ('Revision6_DYNO_Create_New_Wall_1MURO_Solitario_CasoEspecial_return(', 'profileBuilder.BuildOneWallSolitarioSpecialCase('),
    ('Revision6_DYNO_Create_New_Wall_1MURO_Solitario(',                    'profileBuilder.BuildOneWallSolitario('),
]

with open(FILE, 'r', encoding='utf-8-sig') as f:
    content = f.read()

# 1. Replace all call sites
total = 0
for old, new in CALL_MAP:
    n = content.count(old)
    if n > 0:
        content = content.replace(old, new)
        total += n
        print(f"  {n:3d}× {old.split('(')[0].split('_')[-1]} → {new.split('.')[1].split('(')[0]}")
print(f"Total call sites updated: {total}")

# 2. Add profileBuilder declaration after `Document doc = ...` line
ANCHOR = 'Document doc = uiApp.ActiveUIDocument.Document;\n'
INJECT = '\t\t\tvar profileBuilder = new WallProfileBuilder(doc);\n'
if ANCHOR not in content:
    print("ERROR: anchor not found", file=sys.stderr)
    sys.exit(1)
if INJECT in content:
    print("profileBuilder declaration already present, skipping")
else:
    content = content.replace(ANCHOR, ANCHOR + INJECT, 1)
    print("Injected: var profileBuilder = new WallProfileBuilder(doc);")

# 3. Verify no old Revision6_DYNO_*EditProfile* calls remain
import re
remaining = re.findall(r'Revision6_DYNO_(?:Wall_EditProfile|Create_New_Wall)', content)
if remaining:
    print(f"WARNING: {len(remaining)} old Revision6_DYNO_ calls still present:", file=sys.stderr)
    for r in remaining[:5]:
        print(f"  {r}", file=sys.stderr)
else:
    print("Verified: no old Revision6_DYNO_EditProfile calls remain")

with open(FILE, 'w', encoding='utf-8') as f:
    f.write(content)
print("Done.")
```

**Step 2: Run it**

```bash
python3 phase10c_callsites.py
```

Expected: all call sites replaced, no old names remaining.

---

### Task 4: Register in csproj and build

**Files:**
- Modify: `SplitWalls.csproj`

**Step 1: Add `WallProfileBuilder.cs` to csproj**

In `SplitWalls.csproj`, after the `Models\PanelOptions.cs` entry:
```xml
<Compile Include="Models\PanelOptions.cs" />
<Compile Include="Services\WallProfileBuilder.cs" />   ← add this
<Compile Include="Services\WindowDetectionService.cs" />
```

Use the same Python one-liner pattern as previous phases:
```bash
python3 -c "
with open('SplitWalls.csproj','r',encoding='utf-8-sig') as f: c=f.read()
old='    <Compile Include=\"Models\\\\PanelOptions.cs\" />\n    <Compile Include=\"Services\\\\WindowDetectionService.cs\" />'
new='    <Compile Include=\"Models\\\\PanelOptions.cs\" />\n    <Compile Include=\"Services\\\\WallProfileBuilder.cs\" />\n    <Compile Include=\"Services\\\\WindowDetectionService.cs\" />'
assert old in c
c=c.replace(old,new,1)
with open('SplitWalls.csproj','w',encoding='utf-8') as f: f.write(c)
print('OK')
"
```

**Step 2: Build**

```powershell
powershell.exe -Command "
\$src = '\\\\wsl.localhost\\Ubuntu-22.04\\home\\alonsooteroseminario\\source\\repos\\SplitWalls'
\$tmp = 'C:\\Temp\\SplitWalls_build'
if (Test-Path \$tmp) { Remove-Item \$tmp -Recurse -Force }
Copy-Item \$src \$tmp -Recurse -Force
cd \$tmp
& 'C:\\Program Files\\Microsoft Visual Studio\\2022\\Community\\MSBuild\\Current\\Bin\\MSBuild.exe' SplitWalls.csproj /p:Configuration=Release /nologo /verbosity:minimal 2>&1 | Select-String -NotMatch 'MSB3270|Copyright|^$'
"
```

Expected: `SplitWalls.dll` — zero errors. Remaining pre-existing warnings (CS0168 `e`, CS0219 `VIo/VFo_previo_vacio` gone) only.

**If build errors occur:** Check the error line numbers in `ThisApplication.cs`. Common issues:
- A `Revision6_DYNO_` call site was missed in CALL_MAP — add it and re-run `phase10c_callsites.py`
- An indentation issue in `WallProfileBuilder.cs` — check the extracted method around the reported line

---

### Task 5: Commit

```bash
git add Services/WallProfileBuilder.cs ThisApplication.cs SplitWalls.csproj
git commit -m "refactor(phase10): extract WallProfileBuilder service, rename aaaaaa→wallPanel"
```

Then write the phase10-tracking doc and update MEMORY.md.

---

## Quick Reference: Method name map

| Old (local fn in Execute) | New (WallProfileBuilder method) |
|---------------------------|----------------------------------|
| `ReplaceWallWithProfile` | `ReplaceWallWithProfile` (private) |
| `…U_PUERTA_VENTANA_derecha_return` | `BuildU_DoorWindowRight` |
| `…U_PUERTA_PUERTA_derecha_return` | `BuildU_DoorDoorRight` |
| `…U_PUERTA_VENTANA_izquierda_return` | `BuildU_DoorWindowLeft` |
| `…U_PUERTA_PUERTA_izquierda_return` | `BuildU_DoorDoorLeft` |
| `…3VENT_V_P_V_return` | `Build3Opening_WindowDoorWindow` |
| `…3VENT_V_P_P_return` | `Build3Opening_WindowDoorDoor` |
| `…3VENT_P_P_V_return` | `Build3Opening_DoorDoorWindow` |
| `…3VENT_P_P_P_return` | `Build3Opening_DoorDoorDoor` |
| `…BORDE_PUERTA_DERECHA_return` | `BuildEdgeDoorRight` |
| `…BORDE_PUERTA_IZQUIERDO_return` | `BuildEdgeDoorLeft` |
| `…U_PUERTA_return` | `BuildU_Door` |
| `…T_PUERTA_return` | `BuildT_Door` |
| `…I_PUERTA_dVIo_return` | `BuildI_DoorLeft` |
| `…I_PUERTA_dVFo_return` | `BuildI_DoorRight` |
| `…I_return` | `BuildI` |
| `…dVIo_return` | `BuildWindowLeft` |
| `…dVFo_return` | `BuildWindowRight` |
| `…dVIo_PUERTA_return` | `BuildDoorLeft` |
| `…dVFo_PUERTA_return` | `BuildDoorRight` |
| `…dVFo_PUERTA_CasoEspecial_return` | `BuildDoorRightSpecialCase` |
| `…EditProfile_Solitario` | `BuildSolitario` |
| `…2MUROS_Solitario` (void) | `BuildTwoWallSolitario` |
| `…2MUROS_Solitario_return` | `BuildTwoWallSolitarioReturn` |
| `…1MURO_Solitario` (void) | `BuildOneWallSolitario` |
| `…1MURO_Solitario_CasoEspecial_return` | `BuildOneWallSolitarioSpecialCase` |
| `…1MURO_Solitario_CasoEspecial_FinalMuro_return` | `BuildOneWallSolitarioSpecialEndWall` |
| `…1MURO_Solitario_CasoEspecial_InicialMuro_return` | `BuildOneWallSolitarioSpecialStartWall` |
