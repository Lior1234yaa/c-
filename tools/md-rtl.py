#!/usr/bin/env python3
"""עוטף קובצי Markdown בעברית ב-<div dir="rtl"> כדי שיוצגו נכון ב-GitHub / VS Code,
ומחזיר בלוקי קוד (```) לכיוון LTR. הסקריפט אידמפוטנטי — אפשר להריץ שוב ושוב.

שימוש:  python3 tools/md-rtl.py [path ...]   (ברירת מחדל: כל ה-*.md בריפו)
"""
import re, sys, pathlib

RTL_OPEN, RTL_CLOSE = '<div dir="rtl">', '</div>'
LTR_OPEN = '<div dir="ltr">'
FENCE = re.compile(r'^(```|~~~)')

def has_hebrew(text):
    return re.search(r'[֐-׿]', text) is not None

def wrap(text):
    lines = text.split('\n')
    if lines and lines[0].strip() == RTL_OPEN:
        return text  # כבר עטוף
    out, in_fence, fence_mark = [], False, None
    for line in lines:
        m = FENCE.match(line)
        if m and not in_fence:
            in_fence, fence_mark = True, m.group(1)
            out += [LTR_OPEN, '', line]
            continue
        if in_fence and line.startswith(fence_mark) and line.strip() == fence_mark:
            in_fence = False
            out += [line, '', RTL_CLOSE]
            continue
        out.append(line)
    body = '\n'.join(out).strip('\n')
    return f'{RTL_OPEN}\n\n{body}\n\n{RTL_CLOSE}\n'

def main(paths):
    root = pathlib.Path(__file__).resolve().parent.parent
    files = [pathlib.Path(p) for p in paths] if paths else [
        p for p in root.rglob('*.md') if 'node_modules' not in p.parts and 'bin' not in p.parts and 'obj' not in p.parts]
    n = 0
    for f in files:
        t = f.read_text(encoding='utf-8')
        if not has_hebrew(t):
            continue
        w = wrap(t)
        if w != t:
            f.write_text(w, encoding='utf-8'); n += 1
    print(f'wrapped {n} file(s)')

if __name__ == '__main__':
    main(sys.argv[1:])
