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
