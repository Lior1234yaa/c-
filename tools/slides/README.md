# בניית המצגות (Slides)

המצגות של הקורס נבנות מקובצי הגדרה (JavaScript) בעזרת `pptxgenjs`, כך שקל לערוך אותן כטקסט ולבנות מחדש.

## בנייה

```bash
cd tools/slides
npm install            # פעם אחת
npm run build          # בונה את כל המצגות: Day*/Slides/*.slides.js  ->  Day*/Slides/*.pptx
# או מצגת בודדת:
node build-slides.js ../../Day1-OOP-Collections-Exceptions/Slides/Day1.slides.js ../../Day1-OOP-Collections-Exceptions/Slides/Day1.pptx
```

## מבנה קובץ הגדרה (deck spec)

```js
module.exports = {
  day: 1,
  course: 'C# ב-.NET — קורס מעשי',
  title: 'יום 1 — ...',
  slides: [ /* אובייקטי שקף */ ],
};
```

לכל שקף אפשר להוסיף `notes: '...'` — הערות למרצה (Speaker Notes).

| type | שדות | תיאור |
|------|------|-------|
| `title` | `title`, `subtitle?`, `meta?` | שקף פתיחה כהה |
| `section` | `number`, `title`, `subtitle?` | מפריד מקטע |
| `bullets` | `title`, `bullets[]`, `icon?` | נקודות. פריט יכול להיות מחרוזת או `{ text, bold?, sub: [] }` |
| `two-col` | `title`, `right{heading,bullets\|code}`, `left{heading,bullets\|code}` | שתי עמודות (הראשונה מימין) |
| `code` | `title`, `code`, `file?`, `bullets?` | קוד (LTR) משמאל והסברים מימין |
| `cards` | `title`, `cards[{icon?, heading, text}]` | 2–6 כרטיסים |
| `steps` | `title`, `steps[{heading, text}]` | תהליך ממוספר (3–6 שלבים) |
| `lab` | `title`, `goal`, `duration`, `deliverable`, `tasks[]` | פתיחת מעבדה |
| `quote` | `text`, `author?` | ציטוט |
| `end` | `title`, `bullets?`, `footer?` | סיכום / סיום |

אייקונים זמינים (`icon`): code, gear, list, bug, rocket, lab, clock, robot, window, cloud, lock, chat, check, warning, bulb, db, thread, globe, book, user, users, box, puzzle, shield, bolt, sitemap, arrows, eye, pen, wand, brain, mouse, paint, link, terminal, git, star, question, target, key, search, timer, chart, tools, handshake, cube, cubes, file, laptop, ban, recycle, tag, trophy, hammer.

## כללי אצבע
- קוד: עד ~22 שורות ועד ~70 תווים לשורה בשקף `code` עם הסברים, עד ~95 תווים בלי הסברים.
- עד 7 נקודות בשקף `bullets`.
- כל שקף עם `notes` למרצה.
