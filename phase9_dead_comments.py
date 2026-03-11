#!/usr/bin/env python3
"""
Phase 9: Remove dead commented-out code blocks and cleanup.

Steps:
  1. Remove all multi-line comment blocks (2+ consecutive // lines).
     These are exclusively dead code (old collectors, old geometry, etc.).
  2. Remove standalone empty // lines (line contains only // with no text).
  3. Collapse 3+ consecutive blank lines → 2.
"""
FILE = 'ThisApplication.cs'

with open(FILE, 'r', encoding='utf-8-sig') as f:
    lines = f.readlines()

original = len(lines)
print(f"Lines before: {original}")

# ── Step 1: Remove multi-line comment blocks ─────────────────────────────────
def is_comment(line):
    return line.strip().startswith('//')

result = []
i = 0
removed_blocks = 0
removed_block_lines = 0
while i < len(lines):
    if is_comment(lines[i]):
        # Peek ahead: is this the start of a 2+ line run?
        j = i + 1
        while j < len(lines) and is_comment(lines[j]):
            j += 1
        if j - i >= 2:
            # Multi-line block — drop it entirely
            removed_blocks += 1
            removed_block_lines += (j - i)
            i = j
        else:
            # Single comment line — keep for now
            result.append(lines[i])
            i += 1
    else:
        result.append(lines[i])
        i += 1

print(f"Step 1: removed {removed_blocks} multi-line comment blocks ({removed_block_lines} lines)")

# ── Step 2: Remove standalone empty // lines ─────────────────────────────────
removed_empty = 0
cleaned = []
for line in result:
    if line.strip() == '//':
        removed_empty += 1
    else:
        cleaned.append(line)

print(f"Step 2: removed {removed_empty} standalone empty // lines")

# ── Step 3: Collapse 3+ blank lines → 2 ─────────────────────────────────────
final = []
blanks = 0
removed_blanks = 0
for line in cleaned:
    if line.strip() == '':
        blanks += 1
        if blanks <= 2:
            final.append(line)
        else:
            removed_blanks += 1
    else:
        blanks = 0
        final.append(line)

print(f"Step 3: collapsed {removed_blanks} excess blank lines")

after = len(final)
print(f"Lines after: {after} (−{original - after})")

with open(FILE, 'w', encoding='utf-8') as f:
    f.writelines(final)

print("Done.")
