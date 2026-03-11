#!/usr/bin/env python3
"""
Phase 8: Remove all #region/#endregion directives and collapse any
resulting runs of 3+ consecutive blank lines down to 2.
"""
FILE = 'ThisApplication.cs'

with open(FILE, 'r', encoding='utf-8-sig') as f:
    lines = f.readlines()

original = len(lines)
print(f"Lines before: {original}")

# Step 1: drop region directives
stripped = [l for l in lines
            if not (l.strip().startswith('#region') or l.strip().startswith('#endregion'))]
removed_regions = original - len(stripped)
print(f"Step 1: removed {removed_regions} #region/#endregion lines")

# Step 2: collapse 3+ consecutive blank lines → 2
result = []
blanks = 0
for l in stripped:
    if l.strip() == '':
        blanks += 1
        if blanks <= 2:
            result.append(l)
    else:
        blanks = 0
        result.append(l)

removed_blanks = len(stripped) - len(result)
print(f"Step 2: collapsed {removed_blanks} excess blank lines")

after = len(result)
print(f"Lines after: {after} (−{original - after})")

with open(FILE, 'w', encoding='utf-8') as f:
    f.writelines(result)

print("Done.")
