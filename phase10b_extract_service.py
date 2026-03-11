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
    # Brace-count to find closing } — must first enter the body ({) before exiting
    depth = 0
    entered = False
    for i in range(start, len(lines)):
        depth += lines[i].count('{') - lines[i].count('}')
        if depth > 0:
            entered = True
        if entered and depth == 0:
            return start, i
    return None, None

# Extract method blocks
extracted_bodies = []  # list of (new_name, body_text)
ranges_to_delete = []  # list of (start, end) to remove from ThisApplication.cs

for sig_frag, new_name, ret_type, visibility in METHOD_MAP:
    start, end = find_and_extract(lines, sig_frag)
    if start is None:
        print(f"ERROR: could not find '{sig_frag}'", file=sys.stderr)
        sys.exit(1)

    method_lines = list(lines[start:end + 1])

    # Build new declaration line
    decl_line = method_lines[0]
    # The old name is the identifier before the '('
    old_name_in_decl = sig_frag.rstrip('(').split()[-1]  # e.g. "Revision6_DYNO_..."
    new_decl_line = '\t\t\t' + visibility + ' ' + ret_type + ' ' + new_name + decl_line[decl_line.index('('):]
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
