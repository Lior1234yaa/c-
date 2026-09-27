# C# Programming in the .NET Framework — קורס מעשי (4 ימים)

חומרי קורס מלאים לפי הסילבוס: הרצאות (Notes), מצגות (PPTX), דמואים, תרגילים קצרים, מעבדות עם פרויקט התחלה ופתרון מלא, ומדריך התקנה — הכול בעברית, עם קוד ב-C# 13 / .NET 10.

## מפת הקורס

| יום | נושא | תיקייה |
|-----|------|--------|
| 0 | מה צריך להתקין, סקירת הקורס, Cheat-sheet | [`00-Setup/`](00-Setup/) |
| 1 | תכנות מונחה עצמים, אוספים וטיפול בחריגות | [`Day1-OOP-Collections-Exceptions/`](Day1-OOP-Collections-Exceptions/) |
| 2 | Multithreading, תכנות אסינכרוני ו-REST APIs + JSON | [`Day2-Async-APIs/`](Day2-Async-APIs/) |
| 3 | פיתוח GUI מהיר עם WPF (והשוואה ל-WinForms) | [`Day3-GUI-WPF/`](Day3-GUI-WPF/) |
| 4 | פיתוח בעזרת AI וקוד תחזוקתי — מסתיים במפגש על כלי AI לפיתוח UI | [`Day4-AI-Assisted-Development/`](Day4-AI-Assisted-Development/) |

## איך להתחיל

1. עברו על [`00-Setup/INSTALL.md`](00-Setup/INSTALL.md) והתקינו את הכלים (.NET 10 SDK, Visual Studio, Git, ולקראת יום 4 — חשבונות לכלי AI).
2. הריצו את בדיקת הסביבה:

```bash
cd 00-Setup/VerifySetup
dotnet run
```

3. קראו את [`00-Setup/COURSE-OVERVIEW.md`](00-Setup/COURSE-OVERVIEW.md) כדי להבין איך החומר מאורגן ואיך עובדים על מעבדות.

## מה יש בכל תיקיית יום

```text
DayN-<Name>/
  README.md          לו"ז היום (09:00–16:30), מטרות למידה, קישורים
  Slides/DayN.pptx   המצגת של היום (+ קובץ המקור DayN.slides.js)
  Notes/             חומרי הלימוד — קובץ לכל מודול
  Demos/             פרויקטים קטנים להדגמה חיה בכיתה
  Exercises/         תרגילים קצרים + Solutions/
  Labs/LabK-<Name>/  מעבדה: README.md (הנחיות), Starter/ (פרויקט התחלה), Solution/ (פתרון מלא + NOTES.md)
```

## בנייה ובדיקה של כל הקוד

```bash
./tools/build-all.sh        # Linux / macOS
.\tools\build-all.ps1       # Windows (PowerShell)
```

פרויקטי WPF/WinForms (יום 3 ויום 4) מתקמפלים בכל מערכת הפעלה בזכות `EnableWindowsTargeting`, אבל **רצים רק ב-Windows**.

## בניית המצגות מחדש

המצגות נבנות מקובצי טקסט (`Slides/DayN.slides.js`) — קל לערוך אותן ולבנות מחדש:

```bash
cd tools/slides
npm install
npm run build
```

פירוט בפורמט ובסוגי השקפים: [`tools/slides/README.md`](tools/slides/README.md).

## רישיון ושימוש

החומרים נועדו להוראה. קוד הדוגמאות חופשי לשימוש ולשינוי.
