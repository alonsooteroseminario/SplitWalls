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
