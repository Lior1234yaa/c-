<div dir="rtl">

# Lab 3 — סקירת קוד שנוצר ב-AI (45 דקות)

## המטרה
ב-`Starter/Day4.Lab3.Starter/Snippets/` יש **7 קבצי C#** ש"נוצרו ב-AI". כולם מתקמפלים ונראים סבירים. בכל אחד מוסתרים 1–3 באגים עדינים (אבטחה, async, תרבות, משאבים, thread-safety, null, off-by-one). המשימה: לסקור לפי הצ'ק-ליסט של מודול 4, למצוא, לתקן, ולתעד בטבלת ממצאים — בעזרת AI כסוקר **שני**, לא ראשון.

## דרישות מקדימות
- מודול 4 (צ'ק-ליסט הסקירה). כלי AI לצורך סקירה שנייה.

## הקבצים
| קובץ | מה הוא "אמור" לעשות |
|------|----------------------|
| `S1_FileExport.cs` | שומר דוח לקובץ בתיקיית exports לפי שם שהמשתמש נתן |
| `S2_ReportLoader.cs` | טוען JSON מ-URL ומחזיר אורך |
| `S3_PriceParser.cs` | מפענח מחירים ותאריכים מקובץ ייבוא |
| `S4_TokenCache.cs` | מטמון tokens עם תפוגה |
| `S5_Paginator.cs` | מחזיר עמוד מתוך רשימה |
| `S6_CustomerSearch.cs` | חיפוש לקוחות לפי שם ב-SQL |
| `S7_SettingsService.cs` | קורא/שומר הגדרות ב-JSON עם מפתח API |

## שלבים

### שלב 1 — סקירה ידנית (20 דק')
לכל קובץ, עברו על הצ'ק-ליסט (נכונות, מקרי קצה, אבטחה, ביצועים, async, חריגות, משאבים, תלויות). רשמו ממצאים בטבלה:

| קובץ | שורה | קטגוריה | חומרה (High/Med/Low) | הבעיה | תיקון |
|------|------|---------|----------------------|-------|-------|

**אל תשתמשו ב-AI בשלב הזה** — המטרה היא לאמן את העין.

### שלב 2 — סקירה שנייה עם AI (10 דק')
Prompt (תבנית 13):
<div dir="ltr">

```text
Review the following C# as a strict senior reviewer. Check specifically: null handling, async correctness
(async void, .Result/.Wait), culture-sensitive parsing/formatting, IDisposable/HttpClient usage, thread safety,
exception swallowing, injection/path traversal, secrets, off-by-one. For each finding: severity, line, why, minimal fix.
Do not rewrite the file.
{הדביקו קובץ אחד בכל פעם}
```

</div>
השוו: מה ה-AI מצא שאתם פספסתם? מה **אתם** מצאתם שהוא פספס? מה הוא "מצא" שאינו באג (false positive)?

### שלב 3 — תיקון (15 דק')
תקנו כל קובץ (ידנית או עם prompt "apply the minimal fix for finding #N"). `dotnet build` חייב לעבור. הריצו `dotnet run` — ה-Program מפעיל בדיקות עצמיות קטנות שמדגימות את הבאגים (חלקן ייכשלו ב-Starter ויעברו אחרי התיקון).

## קריטריוני קבלה
- [ ] טבלת ממצאים עם ≥ 12 ממצאים, לפחות אחד בכל קובץ.
- [ ] כל הקבצים מתוקנים, `dotnet build` ללא אזהרות, `dotnet run` מדפיס `ALL CHECKS PASSED`.
- [ ] לכל ממצא: הסבר "למה זה משנה בפרודקשן" במשפט.
- [ ] רשימה של false positives של ה-AI (אם היו) והסבר למה.

## בונוס
- הפעילו `<TreatWarningsAsErrors>true</TreatWarningsAsErrors>` ו-`<AnalysisLevel>latest-recommended</AnalysisLevel>` — כמה מהבאגים ה-analyzers היו תופסים?
- הוסיפו בדיקת xUnit לכל תיקון.

## רמזים
- אם משהו "עובד על המחשב שלי" — שאלו: ומה עם `de-DE`? ומה עם 1,000 threads? ומה עם `..\`?
- `catch {}` הוא כמעט תמיד ממצא.

</div>
