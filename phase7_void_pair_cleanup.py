#!/usr/bin/env python3
"""
Phase 7: Redirect void-function callers to their _return counterparts,
then delete the now-unused void functions.

Targets:
  - Revision6_DYNO_Create_New_Wall_EditProfile_dVIo_PUERTA  (void, 3 callers)
  - Revision6_DYNO_Create_New_Wall_EditProfile_dVFo_PUERTA  (void, 3 callers)

Both void functions are behaviorally identical to their _return counterparts;
they only differ by using the old transaction pattern instead of ReplaceWallWithProfile.
"""
import re, sys

FILE = 'ThisApplication.cs'

with open(FILE, 'r', encoding='utf-8-sig') as f:
    content = f.read()

original_lines = content.count('\n')
print(f"Lines before: {original_lines}")

# ── Step 1: Redirect call sites ─────────────────────────────────────────────
# Call sites use wall_i / wall_ii as first arg; declarations use _wall_.
# So replacing dVIo_PUERTA(wall_i, and dVFo_PUERTA(wall_ii, is unambiguous.

replacements = [
    # (old_call_fragment, new_call_fragment, expected_count)
    ('Revision6_DYNO_Create_New_Wall_EditProfile_dVIo_PUERTA(wall_i,',
     'Revision6_DYNO_Create_New_Wall_EditProfile_dVIo_PUERTA_return(wall_i,',
     3),
    ('Revision6_DYNO_Create_New_Wall_EditProfile_dVFo_PUERTA(wall_ii,',
     'Revision6_DYNO_Create_New_Wall_EditProfile_dVFo_PUERTA_return(wall_ii,',
     3),
]

for old, new, expected in replacements:
    count = content.count(old)
    if count != expected:
        print(f"ERROR: expected {expected} occurrences of '{old}', found {count}", file=sys.stderr)
        sys.exit(1)
    content = content.replace(old, new)
    print(f"Step 1: redirected {count}× '{old.split('(')[0].rsplit('_',1)[-1]+' callers'}'")

# ── Step 2: Delete the two void functions ────────────────────────────────────
# Pattern: match from the `void FunctionName(` declaration through the
# closing `}` at the same brace depth, then eat trailing blank lines.

VOID_FUNCS = [
    'void Revision6_DYNO_Create_New_Wall_EditProfile_dVIo_PUERTA(',
    'void Revision6_DYNO_Create_New_Wall_EditProfile_dVFo_PUERTA(',
]

lines = content.splitlines(keepends=True)
deleted_total = 0

for func_sig in VOID_FUNCS:
    # Find start line
    start = None
    for i, line in enumerate(lines):
        if func_sig in line:
            start = i
            break
    if start is None:
        print(f"ERROR: could not find '{func_sig}'", file=sys.stderr)
        sys.exit(1)

    # Count braces to find end
    depth = 0
    end = None
    for i in range(start, len(lines)):
        depth += lines[i].count('{') - lines[i].count('}')
        if depth == 0 and i > start:
            end = i
            break
    if end is None:
        print(f"ERROR: could not find closing brace for '{func_sig}'", file=sys.stderr)
        sys.exit(1)

    # Consume trailing blank lines
    while end + 1 < len(lines) and lines[end + 1].strip() == '':
        end += 1

    deleted = end - start + 1
    deleted_total += deleted
    func_name = func_sig.split('(')[0].strip().split()[-1]
    print(f"Step 2: deleted {func_name} ({deleted} lines, {start+1}–{end+1})")

    # Delete lines (rebuild each time since indices shift)
    lines = lines[:start] + lines[end + 1:]

content = ''.join(lines)

after_lines = content.count('\n')
print(f"Lines after: {after_lines} (−{original_lines - after_lines})")

with open(FILE, 'w', encoding='utf-8') as f:
    f.write(content)

print("Done.")
