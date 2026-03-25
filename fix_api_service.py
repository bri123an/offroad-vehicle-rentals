#!/usr/bin/env python3
import re

# Read the original file
with open('ApiService.cs', 'r') as f:
    content = f.read()

# Replace GET calls with literal strings
content = re.sub(r'GetAsync\("([^"]+)"\)', r'GetAsync(AddFunctionKey("\1"))', content)
# Replace GET calls with string interpolation
content = re.sub(r'GetAsync\(\$"([^"]+)"\)', r'GetAsync(AddFunctionKey($"\1"))', content)

# Replace POST calls with literal strings
content = re.sub(r'PostAsJsonAsync\("([^"]+)",', r'PostAsJsonAsync(AddFunctionKey("\1"),', content)
# Replace POST calls with string interpolation
content = re.sub(r'PostAsJsonAsync\(\$"([^"]+)",', r'PostAsJsonAsync(AddFunctionKey($"\1"),', content)

# Replace PUT calls with literal strings
content = re.sub(r'PutAsJsonAsync\("([^"]+)",', r'PutAsJsonAsync(AddFunctionKey("\1"),', content)
# Replace PUT calls with string interpolation
content = re.sub(r'PutAsJsonAsync\(\$"([^"]+)",', r'PutAsJsonAsync(AddFunctionKey($"\1"),', content)

# Replace DELETE calls with literal strings
content = re.sub(r'DeleteAsync\("([^"]+)"\)', r'DeleteAsync(AddFunctionKey("\1"))', content)
# Replace DELETE calls with string interpolation
content = re.sub(r'DeleteAsync\(\$"([^"]+)"\)', r'DeleteAsync(AddFunctionKey($"\1"))', content)

# Write back
with open('ApiService.cs', 'w') as f:
    f.write(content)

print("✓ API Service updated successfully!")
print("All HTTP calls now include function key support")
