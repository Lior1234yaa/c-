#!/usr/bin/env node
/**
 * build-slides.js — בונה מצגת PPTX מקובץ הגדרה (deck spec) בעברית (RTL).
 *
 * שימוש:  node tools/slides/build-slides.js <deck-spec.js> <output.pptx>
 *
 * קובץ ה-spec הוא מודול Node שמייצא אובייקט:
 *   { day: 1, course: '...', title: '...', subtitle: '...', slides: [ ...slide objects ] }
 *
 * סוגי שקפים נתמכים (ראו README.md בתיקייה זו לפירוט המלא):
 *   title | section | bullets | two-col | code | cards | steps | lab | quote | end
 */
const path = require('path');
const fs = require('fs');
const os = require('os');

const resolveFrom = (m) => require.resolve(m, { paths: [__dirname, path.join(__dirname, '..', '..'), process.cwd()] });
const pptxgen = require(resolveFrom('pptxgenjs'));

// ---------- palette ----------
const C = {
  dark: '1B1340',      // רקע כהה (שקף פתיחה / מקטע)
  primary: '512BD4',   // סגול .NET
  primary2: '7C5CE6',
  light: 'F3F0FF',     // רקע כרטיסים
  accent: 'FFB020',    // ענבר — הדגשות
  text: '1F1B2E',
  muted: '6B6480',
  white: 'FFFFFF',
  codeBg: '1E1E2E',
  codeFg: 'E6E6F0',
  codeKw: 'C792EA',
};
const FONT = 'Arial';
const MONO = 'Courier New';
const W = 13.333, H = 7.5, M = 0.6;

// ---------- icons ----------
let iconCache = null;
function iconPng(name, color, size = 256) {
  const cacheDir = path.join(os.tmpdir(), 'course-slide-icons');
  fs.mkdirSync(cacheDir, { recursive: true });
  const file = path.join(cacheDir, `${name}-${color}-${size}.png`);
  if (fs.existsSync(file)) return fs.readFileSync(file);
  if (!iconCache) {
    const React = require(resolveFrom('react'));
    const Server = require(resolveFrom('react-dom/server'));
    const fa = require(resolveFrom('react-icons/fa6'));
    const map = {
      code: fa.FaCode, gear: fa.FaGear, list: fa.FaListUl, bug: fa.FaBug, rocket: fa.FaRocket,
      lab: fa.FaFlask, clock: fa.FaClock, robot: fa.FaRobot, window: fa.FaWindowMaximize,
      cloud: fa.FaCloud, lock: fa.FaLock, chat: fa.FaComments, check: fa.FaCheck,
      warning: fa.FaTriangleExclamation, bulb: fa.FaLightbulb, db: fa.FaDatabase,
      thread: fa.FaLayerGroup, globe: fa.FaGlobe, book: fa.FaBook, user: fa.FaUser,
      users: fa.FaUsers, box: fa.FaBoxOpen, puzzle: fa.FaPuzzlePiece, shield: fa.FaShieldHalved,
      bolt: fa.FaBolt, sitemap: fa.FaSitemap, arrows: fa.FaArrowsRotate, eye: fa.FaEye,
      pen: fa.FaPenToSquare, wand: fa.FaWandMagicSparkles, brain: fa.FaBrain, mouse: fa.FaArrowPointer,
      paint: fa.FaPaintbrush, link: fa.FaLink, terminal: fa.FaTerminal, git: fa.FaCodeBranch,
      star: fa.FaStar, question: fa.FaCircleQuestion, target: fa.FaBullseye, key: fa.FaKey,
      search: fa.FaMagnifyingGlass, timer: fa.FaHourglassHalf, chart: fa.FaChartLine, tools: fa.FaScrewdriverWrench,
      handshake: fa.FaHandshake, cube: fa.FaCube, cubes: fa.FaCubes, file: fa.FaFileCode, laptop: fa.FaLaptopCode,
      ban: fa.FaBan, recycle: fa.FaRecycle, tag: fa.FaTag, trophy: fa.FaTrophy, hammer: fa.FaHammer,
    };
    iconCache = { React, Server, map, sharp: require(resolveFrom('sharp')) };
  }
  const Comp = iconCache.map[name] || iconCache.map.star;
  const svg = iconCache.Server.renderToStaticMarkup(iconCache.React.createElement(Comp, { color: '#' + color, size }));
  // sharp is async; use deasync-free approach via spawnSync of a tiny node script
  const script = `const s=require(${JSON.stringify(resolveFrom('sharp'))});s(Buffer.from(process.argv[1])).png().toFile(process.argv[2]).then(()=>{})`;
  require('child_process').execFileSync(process.execPath, ['-e', script, svg, file]);
  return fs.readFileSync(file);
}
function addIconCircle(slide, name, x, y, d, opts = {}) {
  const bg = opts.bg || C.primary, fg = opts.fg || C.white;
  slide.addShape('ellipse', { x, y, w: d, h: d, fill: { color: bg }, line: { color: bg } });
  const pad = d * 0.25;
  slide.addImage({ data: 'image/png;base64,' + iconPng(name, fg).toString('base64'), x: x + pad, y: y + pad, w: d - 2 * pad, h: d - 2 * pad });
}

// ---------- text helpers ----------
const rtl = (o = {}) => ({ fontFace: FONT, rtlMode: true, align: 'right', lang: 'he-IL', color: C.text, isTextBox: true, valign: 'top', ...o });
const ltr = (o = {}) => ({ fontFace: MONO, align: 'left', lang: 'en-US', color: C.codeFg, isTextBox: true, valign: 'top', ...o });

function bulletRuns(items, size, color = C.text) {
  const runs = [];
  const flat = [];
  for (const it of items) {
    if (typeof it === 'string') flat.push({ text: it, lvl: 0 });
    else { flat.push({ text: it.text, lvl: 0, bold: it.bold }); (it.sub || []).forEach(s => flat.push({ text: s, lvl: 1 })); }
  }
  flat.forEach((f, i) => {
    runs.push({
      text: f.text,
      options: {
        bullet: f.lvl ? { indent: 20 } : true, indentLevel: f.lvl, fontSize: f.lvl ? size - 3 : size, bold: !!f.bold,
        color: f.lvl ? C.muted : color, breakLine: i < flat.length - 1, paraSpaceAfter: f.lvl ? 4 : 8,
      },
    });
  });
  return runs;
}
function autoSize(items, base = 20, box = { w: 12, h: 5 }) {
  const flat = items.flatMap(i => typeof i === 'string' ? [i] : [i.text, ...(i.sub || [])]);
  const n = flat.length, chars = flat.reduce((a, s) => a + s.length, 0);
  let s = base;
  if (n > 6 || chars > 320) s = base - 2;
  if (n > 8 || chars > 480) s = base - 4;
  if (n > 10 || chars > 640) s = base - 6;
  return Math.max(s, 12);
}
function codeSize(code, boxW) {
  const lines = code.split('\n');
  const maxLen = Math.max(...lines.map(l => l.length));
  // Courier New: char width ≈ 0.6 * fontsize(pt)/72 inch
  const fitW = (boxW - 0.4) * 72 / 0.6 / maxLen;
  const fitH = (5.2 * 72) / (lines.length * 1.25);
  return Math.max(9, Math.min(15, Math.floor(Math.min(fitW, fitH))));
}

// ---------- chrome ----------
function footer(slide, spec, n, dark = false) {
  const col = dark ? 'A79FD6' : C.muted;
  slide.addText(`${spec.course}  |  יום ${spec.day}`, rtl({ x: W - M - 5, y: H - 0.45, w: 5, h: 0.3, fontSize: 10, color: col, margin: 0 }));
  slide.addText(String(n), { x: M, y: H - 0.45, w: 1, h: 0.3, fontSize: 10, color: col, fontFace: FONT, align: 'left', isTextBox: true, margin: 0 });
}
function titleBar(slide, text, opts = {}) {
  slide.addText(text, rtl({ x: M, y: 0.4, w: W - 2 * M, h: 0.9, fontSize: opts.size || 32, bold: true, color: C.dark, valign: 'middle', margin: 0 }));
}
function decor(slide) {
  // מוטיב חוזר: עיגולים סגולים שקופים בפינה השמאלית-תחתונה
  slide.addShape('ellipse', { x: -1.2, y: H - 2.2, w: 3.2, h: 3.2, fill: { color: C.primary, transparency: 82 }, line: { color: C.primary, transparency: 100 } });
  slide.addShape('ellipse', { x: 0.9, y: H - 1.1, w: 1.6, h: 1.6, fill: { color: C.accent, transparency: 75 }, line: { color: C.accent, transparency: 100 } });
}

// ---------- slide renderers ----------
const R = {};
R.title = (s, sl, spec) => {
  sl.background = { color: C.dark };
  sl.addShape('ellipse', { x: -2.5, y: -2.5, w: 7, h: 7, fill: { color: C.primary, transparency: 70 }, line: { color: C.primary, transparency: 100 } });
  sl.addShape('ellipse', { x: 1.5, y: 3.5, w: 4.5, h: 4.5, fill: { color: C.primary2, transparency: 75 }, line: { color: C.primary2, transparency: 100 } });
  sl.addText('{ }', { x: 0.9, y: 1.7, w: 4.5, h: 3.2, fontSize: 150, bold: true, color: C.accent, fontFace: MONO, align: 'center', valign: 'middle', isTextBox: true, transparency: 15 });
  sl.addText(spec.course, rtl({ x: 5.6, y: 1.1, w: 7.1, h: 0.6, fontSize: 18, color: 'CFC7F5', margin: 0 }));
  sl.addText(s.title, rtl({ x: 5.6, y: 1.8, w: 7.1, h: 2.2, fontSize: 44, bold: true, color: C.white, valign: 'middle', margin: 0 }));
  if (s.subtitle) sl.addText(s.subtitle, rtl({ x: 5.6, y: 4.1, w: 7.1, h: 1.4, fontSize: 22, color: 'CFC7F5', margin: 0 }));
  if (s.meta) sl.addText(s.meta, rtl({ x: 5.6, y: 5.7, w: 7.1, h: 0.5, fontSize: 14, color: C.accent, margin: 0 }));
};
R.section = (s, sl, spec, n) => {
  sl.background = { color: C.dark };
  sl.addShape('ellipse', { x: 8.5, y: -3, w: 8, h: 8, fill: { color: C.primary, transparency: 72 }, line: { color: C.primary, transparency: 100 } });
  sl.addText(s.number || '', { x: 0.9, y: 1.3, w: 4, h: 3, fontSize: 130, bold: true, color: C.accent, fontFace: FONT, align: 'left', valign: 'middle', isTextBox: true, margin: 0 });
  sl.addText(s.title, rtl({ x: 4.8, y: 2.0, w: 7.9, h: 1.6, fontSize: 40, bold: true, color: C.white, valign: 'middle', margin: 0 }));
  if (s.subtitle) sl.addText(s.subtitle, rtl({ x: 4.8, y: 3.7, w: 7.9, h: 1.6, fontSize: 20, color: 'CFC7F5', margin: 0 }));
  footer(sl, spec, n, true);
};
R.bullets = (s, sl, spec, n) => {
  titleBar(sl, s.title);
  const hasIcon = !!s.icon;
  const size = autoSize(s.bullets, 20);
  const x = hasIcon ? M + 3.2 : M, w = W - 2 * M - (hasIcon ? 3.2 : 0);
  sl.addText(bulletRuns(s.bullets, size), rtl({ x, y: 1.5, w, h: 5.2, fontSize: size, margin: 0.1 }));
  if (hasIcon) {
    addIconCircle(sl, s.icon, M + 0.5, 2.6, 2.0, { bg: C.light, fg: C.primary });
  } else decor(sl);
  footer(sl, spec, n);
};
R['two-col'] = (s, sl, spec, n) => {
  titleBar(sl, s.title);
  const colW = (W - 2 * M - 0.5) / 2;
  // בעברית: העמודה "הראשונה" (right) מימין
  const cols = [{ c: s.right, x: M + colW + 0.5 }, { c: s.left, x: M }];
  for (const { c, x } of cols) {
    if (!c) continue;
    sl.addShape('roundRect', { x, y: 1.5, w: colW, h: 5.3, fill: { color: C.light }, line: { color: C.light }, rectRadius: 0.15 });
    if (c.heading) sl.addText(c.heading, rtl({ x: x + 0.25, y: 1.65, w: colW - 0.5, h: 0.6, fontSize: 20, bold: true, color: C.primary, margin: 0 }));
    const size = autoSize(c.bullets || [], 17, { w: colW, h: 4 });
    if (c.bullets) sl.addText(bulletRuns(c.bullets, size), rtl({ x: x + 0.25, y: 2.3, w: colW - 0.5, h: 4.3, fontSize: size, margin: 0 }));
    if (c.code) {
      const cs = codeSize(c.code, colW - 0.4);
      sl.addShape('roundRect', { x: x + 0.2, y: 2.3, w: colW - 0.4, h: 4.3, fill: { color: C.codeBg }, line: { color: C.codeBg }, rectRadius: 0.1 });
      sl.addText(c.code, ltr({ x: x + 0.3, y: 2.4, w: colW - 0.6, h: 4.1, fontSize: cs, margin: 0.05 }));
    }
  }
  footer(sl, spec, n);
};
R.code = (s, sl, spec, n) => {
  titleBar(sl, s.title);
  const hasSide = s.bullets && s.bullets.length;
  const codeW = hasSide ? 7.9 : W - 2 * M;
  const codeX = M;  // הקוד משמאל, ההסברים מימין
  const cs = codeSize(s.code, codeW);
  sl.addShape('roundRect', { x: codeX, y: 1.5, w: codeW, h: 5.3, fill: { color: C.codeBg }, line: { color: C.codeBg }, rectRadius: 0.12 });
  if (s.file) sl.addText(s.file, ltr({ x: codeX + 0.2, y: 1.55, w: codeW - 0.4, h: 0.3, fontSize: 10, color: '9A96B8', margin: 0 }));
  sl.addText(s.code, ltr({ x: codeX + 0.2, y: s.file ? 1.9 : 1.65, w: codeW - 0.4, h: s.file ? 4.8 : 5.05, fontSize: cs, margin: 0.05 }));
  if (hasSide) {
    const bx = codeX + codeW + 0.4, bw = W - M - bx;
    const size = autoSize(s.bullets, 16, { w: bw, h: 5 });
    sl.addText(bulletRuns(s.bullets, size), rtl({ x: bx, y: 1.5, w: bw, h: 5.3, fontSize: size, margin: 0 }));
  }
  footer(sl, spec, n);
};
R.cards = (s, sl, spec, n) => {
  titleBar(sl, s.title);
  const cards = s.cards.slice(0, 6);
  const cols = cards.length <= 4 ? cards.length : 3;
  const rows = Math.ceil(cards.length / cols);
  const gap = 0.35, availW = W - 2 * M, availH = 5.3;
  const cw = (availW - gap * (cols - 1)) / cols, ch = (availH - gap * (rows - 1)) / rows;
  cards.forEach((c, i) => {
    const r = Math.floor(i / cols), col = i % cols;
    // RTL: הכרטיס הראשון בצד ימין
    const x = W - M - cw - col * (cw + gap), y = 1.5 + r * (ch + gap);
    sl.addShape('roundRect', { x, y, w: cw, h: ch, fill: { color: C.light }, line: { color: C.light }, rectRadius: 0.15, shadow: { type: 'outer', blur: 6, offset: 2, angle: 90, color: '000000', opacity: 0.12 } });
    const d = rows > 1 ? 0.55 : 0.7;
    let cy = y + 0.25;
    if (c.icon) { addIconCircle(sl, c.icon, x + cw - 0.25 - d, cy, d); cy += d + 0.15; }
    const headSize = rows > 1 ? 15 : (cols >= 4 ? 16 : 18);
    sl.addText(c.heading || '', rtl({ x: x + 0.25, y: cy, w: cw - 0.5, h: 0.5, fontSize: headSize, bold: true, color: C.dark, valign: 'middle', margin: 0, fit: 'shrink' }));
    cy += 0.55;
    const bodySize = rows > 1 ? 12 : (cols >= 4 ? 13 : 15);
    const body = Array.isArray(c.text) ? bulletRuns(c.text, bodySize) : c.text || '';
    sl.addText(body, rtl({ x: x + 0.25, y: cy, w: cw - 0.5, h: y + ch - cy - 0.25, fontSize: bodySize, color: C.text, margin: 0 }));
  });
  footer(sl, spec, n);
};
R.steps = (s, sl, spec, n) => {
  titleBar(sl, s.title);
  const steps = s.steps.slice(0, 6);
  const rowH = Math.min(1.0, 5.2 / steps.length), y0 = 1.55;
  steps.forEach((st, i) => {
    const y = y0 + i * rowH;
    const d = Math.min(0.7, rowH - 0.2);
    sl.addShape('ellipse', { x: W - M - d, y: y + (rowH - d) / 2 - 0.05, w: d, h: d, fill: { color: C.primary }, line: { color: C.primary } });
    sl.addText(String(i + 1), { x: W - M - d, y: y + (rowH - d) / 2 - 0.05, w: d, h: d, fontSize: 18, bold: true, color: C.white, fontFace: FONT, align: 'center', valign: 'middle', isTextBox: true, margin: 0 });
    const tw = W - 2 * M - d - 0.3;
    const heading = typeof st === 'string' ? st : st.heading;
    const text = typeof st === 'string' ? '' : st.text;
    const runs = [{ text: heading, options: { bold: true, fontSize: 18, color: C.dark, breakLine: !!text } }];
    if (text) runs.push({ text, options: { fontSize: 14, color: C.muted } });
    sl.addText(runs, rtl({ x: M, y, w: tw, h: rowH - 0.1, valign: 'middle', margin: 0 }));
  });
  decor(sl);
  footer(sl, spec, n);
};
R.lab = (s, sl, spec, n) => {
  sl.background = { color: C.light };
  addIconCircle(sl, 'lab', W - M - 0.9, 0.45, 0.9, { bg: C.accent, fg: C.dark });
  sl.addText(s.title, rtl({ x: M, y: 0.4, w: W - 2 * M - 1.1, h: 0.9, fontSize: 32, bold: true, color: C.dark, valign: 'middle', margin: 0 }));
  const rightW = 7.6, leftW = W - 2 * M - rightW - 0.4;
  const rx = W - M - rightW;
  sl.addShape('roundRect', { x: rx, y: 1.5, w: rightW, h: 5.3, fill: { color: C.white }, line: { color: C.white }, rectRadius: 0.15 });
  sl.addText('המשימות', rtl({ x: rx + 0.25, y: 1.65, w: rightW - 0.5, h: 0.5, fontSize: 18, bold: true, color: C.primary, margin: 0 }));
  const size = autoSize(s.tasks, 16, { w: rightW, h: 4.5 });
  sl.addText(s.tasks.map((t, i) => ({ text: t, options: { bullet: { type: 'number' }, fontSize: size, breakLine: i < s.tasks.length - 1, paraSpaceAfter: 6 } })), rtl({ x: rx + 0.25, y: 2.2, w: rightW - 0.5, h: 4.5, fontSize: size, margin: 0 }));
  const boxes = [['המטרה', s.goal, 'target'], ['משך', s.duration, 'clock'], ['תוצר', s.deliverable, 'trophy']].filter(b => b[1]);
  const bh = (5.3 - 0.3 * (boxes.length - 1)) / boxes.length;
  boxes.forEach(([h, t, ic], i) => {
    const y = 1.5 + i * (bh + 0.3);
    sl.addShape('roundRect', { x: M, y, w: leftW, h: bh, fill: { color: C.white }, line: { color: C.white }, rectRadius: 0.15 });
    addIconCircle(sl, ic, M + leftW - 0.7, y + 0.2, 0.5);
    sl.addText(h, rtl({ x: M + 0.2, y: y + 0.2, w: leftW - 1.1, h: 0.5, fontSize: 14, bold: true, color: C.primary, valign: 'middle', margin: 0 }));
    sl.addText(t, rtl({ x: M + 0.2, y: y + 0.75, w: leftW - 0.4, h: bh - 0.9, fontSize: 13, color: C.text, margin: 0 }));
  });
  footer(sl, spec, n);
};
R.quote = (s, sl, spec, n) => {
  sl.background = { color: C.dark };
  sl.addText('”', { x: 0.8, y: 0.6, w: 3, h: 3, fontSize: 200, color: C.accent, fontFace: 'Cambria', align: 'left', isTextBox: true, transparency: 30, margin: 0 });
  sl.addText(s.text, rtl({ x: 2.5, y: 1.8, w: W - 2.5 - M, h: 3.2, fontSize: 30, color: C.white, valign: 'middle', margin: 0 }));
  if (s.author) sl.addText('— ' + s.author, rtl({ x: 2.5, y: 5.1, w: W - 2.5 - M, h: 0.6, fontSize: 18, color: 'CFC7F5', margin: 0 }));
  footer(sl, spec, n, true);
};
R.end = (s, sl, spec, n) => {
  sl.background = { color: C.dark };
  sl.addShape('ellipse', { x: -2, y: 3.5, w: 6, h: 6, fill: { color: C.primary, transparency: 70 }, line: { color: C.primary, transparency: 100 } });
  sl.addText(s.title || 'סיכום', rtl({ x: M, y: 0.6, w: W - 2 * M, h: 1.2, fontSize: 40, bold: true, color: C.white, valign: 'middle', margin: 0 }));
  if (s.bullets) {
    const size = autoSize(s.bullets, 20);
    sl.addText(bulletRuns(s.bullets, size, C.white), rtl({ x: M + 3, y: 2.0, w: W - 2 * M - 3, h: 4.5, fontSize: size, margin: 0 }));
  }
  if (s.footer) sl.addText(s.footer, rtl({ x: M, y: H - 1.3, w: W - 2 * M, h: 0.6, fontSize: 16, color: C.accent, margin: 0 }));
  footer(sl, spec, n, true);
};

// ---------- main ----------
function build(spec, out) {
  const pres = new pptxgen();
  pres.layout = 'LAYOUT_WIDE';
  pres.rtlMode = true;
  pres.lang = 'he-IL';
  pres.title = spec.title;
  spec.slides.forEach((s, i) => {
    const sl = pres.addSlide();
    const fn = R[s.type];
    if (!fn) throw new Error(`Unknown slide type "${s.type}" at slide ${i + 1}`);
    fn(s, sl, spec, i + 1);
    if (s.notes) sl.addNotes(s.notes);
  });
  return pres.writeFile({ fileName: out }).then(() => console.log(`wrote ${out} (${spec.slides.length} slides)`));
}

if (require.main === module) {
  const [specPath, out] = process.argv.slice(2);
  if (!specPath || !out) { console.error('usage: build-slides.js <deck-spec.js> <output.pptx>'); process.exit(1); }
  const spec = require(path.resolve(specPath));
  build(spec, path.resolve(out)).catch(e => { console.error(e); process.exit(1); });
}
module.exports = { build };
