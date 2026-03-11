#!/usr/bin/env python3
"""
Phase 5: Wall profile builder clean-up (regex replace approach).

Safety: the regex only matches methods with the EXACT standard footer.
Non-standard methods (CasoEspecial, DarVuelta, etc.) are silently skipped.

Steps:
  1. Strip dead single-line comments.
  2. Insert ReplaceWallWithProfile helper.
  3. For each standard builder method, collapse the transaction boilerplate
     into 'return ReplaceWallWithProfile(wall_I, profile);'.
"""
import re
import sys

FILE = 'ThisApplication.cs'

with open(FILE, 'r', encoding='utf-8-sig') as f:
    content = f.read()

original_lines = content.count('\n')
print(f"Lines before: {original_lines}")

# ── Step 1: Strip dead single-line comments ──────────────────────────────────
dead = [
    r'[ \t]*//UIDocument uidoc = this\.ActiveUIDocument;[ \t]*\n',
    r'[ \t]*//Document doc = uidoc\.Document;[ \t]*\n',
    r'[ \t]*//Application app = this\.Application;[ \t]*\n',
    r'[ \t]*// ESTE FUNCIONA ACTUALMENTE[ \t]*\n',
]
removed = 0
for pattern in dead:
    new_content, n = re.subn(pattern, '', content)
    removed += n
    content = new_content

print(f"Step 1: removed {removed} dead comment lines")

# ── Step 2: Insert ReplaceWallWithProfile helper ─────────────────────────────
INSERT_MARKER = 'Wall Revision6_DYNO_Wall_EditProfile_U_PUERTA_VENTANA_derecha_return('

helper = (
    '\t\t\t/// <summary>\n'
    '\t\t\t/// Replace <paramref name="source"/> wall with a new wall built from\n'
    '\t\t\t/// <paramref name="profile"/>. Handles transaction, Wall.Create,\n'
    '\t\t\t/// DisableJoins, and deletion of the original wall.\n'
    '\t\t\t/// </summary>\n'
    '\t\t\tWall ReplaceWallWithProfile(Wall source, IList<Curve> profile)\n'
    '\t\t\t{\n'
    '\t\t\t\tusing (Transaction t = new Transaction(doc, "wall"))\n'
    '\t\t\t\t{\n'
    '\t\t\t\t\tt.Start();\n'
    '\t\t\t\t\tWall w = Wall.Create(doc, profile, source.WallType.Id, source.LevelId, true);\n'
    '\t\t\t\t\tWallJoinHelper.DisableJoins(w);\n'
    '\t\t\t\t\tdoc.Delete(source.Id);\n'
    '\t\t\t\t\tt.Commit();\n'
    '\t\t\t\t\treturn w;\n'
    '\t\t\t\t}\n'
    '\t\t\t}\n'
    '\n'
)

idx = content.find(INSERT_MARKER)
if idx < 0:
    print("ERROR: INSERT_MARKER not found", file=sys.stderr)
    sys.exit(1)
content = content[:idx] + helper + content[idx:]
print(f"Step 2: inserted ReplaceWallWithProfile helper")

# ── Step 3: Collapse standard transaction boilerplate ────────────────────────
#
# Pattern matches:
#   List<Wall> lista_wall_return = new List<Wall>();         ← suppress
#   [optional: 2nd List<Wall> decl]                         ← suppress
#   [XYZ computations - captured as group \1]               ← KEEP
#   using (Transaction trans = ...) {                       ← suppress
#   trans.Start();                                          ← suppress
#   [blank lines and //-comments]                           ← suppress
#   IList<Curve> profile = new List<Curve>();               ← suppress (recreated in replacement)
#   [profile-building lines - captured as group \2]         ← KEEP
#   Wall wall = Wall.Create(doc, profile, ...);             ← suppress (done by helper)
#   [optional] lista_wall_return.Add(wall);                 ← suppress
#   WallJoinHelper.DisableJoins(wall);                      ← suppress (done by helper)
#   [optional] lista_wall_return.Add(wall);                 ← suppress
#   doc.Delete(wall_I.Id);                                  ← suppress (done by helper)
#   trans.Commit();                                         ← suppress (done by helper)
#   [blank] }                                               ← suppress (done by helper)
#   return lista_wall_return.First();                       ← REPLACE
#
# Non-standard footers (CasoEspecial etc.) won't match and are left unchanged.

ADD_PAT   = r'(?:[ \t]*(?:lista_wall_return|ultimos_Walls_para_Agregar_40)\.Add\(wall\);[ \t]*\n)?'
BLANK_CMT = r'(?:[ \t]*(?://[^\n]*)?\n)*'  # blank lines or //-comment lines

TRANSFORM = re.compile(
    # --- Trigger: lista_wall_return declaration(s) ---
    r'([ \t]*)List<Wall>\s+(?:lista_wall_return|ultimos_Walls_para_Agregar_40)\s*'
    r'=\s*new\s+(?:List<Wall>|IList<Wall>)\(\);[ \t]*\n'
    r'(?:[ \t]*List<Wall>\s+(?:lista_wall_return|ultimos_Walls_para_Agregar_40)\s*'
    r'=\s*new\s+(?:List<Wall>|IList<Wall>)\(\);[ \t]*\n)?'
    # --- Intervening code (XYZ computations) kept in group 1 ---
    r'(.*?)'
    # --- using (Transaction...) header ---
    r'[ \t]*using\s*\(Transaction\s+trans\s*=\s*new\s+Transaction\(doc,\s*"wall"\)\)[ \t]*\n'
    + BLANK_CMT +
    r'[ \t]*\{[ \t]*\n'
    + BLANK_CMT +
    r'[ \t]*trans\.Start\(\);[ \t]*\n'
    + BLANK_CMT +
    # --- IList<Curve> profile declaration ---
    r'[ \t]*IList<Curve>\s+profile\s*=\s*new\s+List<Curve>\(\);[ \t]*\n'
    # --- Profile-building lines kept in group 2 ---
    r'(.*?)'
    # --- Standard footer: Wall.Create + [Add] + DisableJoins + [Add] + Delete + Commit + } ---
    r'[ \t]*Wall\s+wall\s*=\s*Wall\.Create\(doc,\s*profile,\s*wallType\.Id,\s*wall_I\.LevelId,\s*true\);[ \t]*\n'
    + BLANK_CMT +
    ADD_PAT +
    BLANK_CMT +
    r'[ \t]*WallJoinHelper\.DisableJoins\(wall\);[ \t]*\n'
    + BLANK_CMT +
    ADD_PAT +
    BLANK_CMT +
    r'[ \t]*doc\.Delete\(wall_I\.Id\);[ \t]*\n'
    + BLANK_CMT +
    r'[ \t]*trans\.Commit\(\);[ \t]*\n'
    + BLANK_CMT +
    r'[ \t]*\}[ \t]*\n'
    + BLANK_CMT +
    # --- return lista_wall_return.First() ---
    r'([ \t]*)return\s+(?:lista_wall_return|ultimos_Walls_para_Agregar_40)\.First\(\);',
    re.DOTALL
)

def replacer(m):
    trigger_indent = m.group(1)    # indent of original lista_wall_return decl
    xyz_code       = m.group(2)    # XYZ computations to keep
    profile_code   = m.group(3)    # profile-building lines to keep
    return_indent  = m.group(4)    # indent of original return statement

    return (
        xyz_code                                              # keep XYZ computations
        + return_indent + 'IList<Curve> profile = new List<Curve>();\n'
        + profile_code                                        # keep Line decls + profile.Add
        + return_indent + 'return ReplaceWallWithProfile(wall_I, profile);'
    )

new_content, count = re.subn(TRANSFORM, replacer, content)
content = new_content
print(f"Step 3: transformed {count} standard builder methods")

remaining = len(re.findall(
    r'return\s+(?:lista_wall_return|ultimos_Walls_para_Agregar_40)\.First\(\)',
    content
))
print(f"  Remaining (non-standard, left unchanged): {remaining}")

after_lines = content.count('\n')
print(f"Lines after: {after_lines} (−{original_lines - after_lines})")

with open(FILE, 'w', encoding='utf-8') as f:
    f.write(content)

print("Done.")
