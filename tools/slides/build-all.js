// בונה את כל המצגות בקורס: Day*/Slides/*.slides.js -> .pptx
const path = require('path'), fs = require('fs');
const { build } = require('./build-slides');
const root = path.resolve(__dirname, '..', '..');
(async () => {
  for (const day of fs.readdirSync(root).filter(d => /^Day\d/.test(d))) {
    const dir = path.join(root, day, 'Slides');
    if (!fs.existsSync(dir)) continue;
    for (const f of fs.readdirSync(dir).filter(f => f.endsWith('.slides.js'))) {
      const spec = require(path.join(dir, f));
      await build(spec, path.join(dir, f.replace(/\.slides\.js$/, '.pptx')));
    }
  }
})().catch(e => { console.error(e); process.exit(1); });
