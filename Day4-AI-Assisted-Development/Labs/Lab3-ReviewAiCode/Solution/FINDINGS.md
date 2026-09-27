<div dir="rtl">

# Lab 3 — טבלת ממצאים (פתרון)

| # | קובץ | קטגוריה | חומרה | הבעיה | למה זה משנה בפרודקשן | תיקון |
|---|------|---------|-------|-------|----------------------|-------|
| 1 | S1_FileExport | אבטחה — path traversal | High | `Path.Combine(root, userFileName)` עם `..\` או נתיב מוחלט יוצא מהתיקייה | דריסת קבצים מחוץ ל-exports (config, קבצי מערכת) | `Path.GetFileName` + בדיקת `GetFullPath` שמתחיל ב-root |
| 2 | S1_FileExport | קלט | Med | אין בדיקת null/ריק/תווים אסורים | חריגות לא ברורות | `ThrowIfNullOrWhiteSpace`, `GetInvalidFileNameChars` |
| 3 | S2_ReportLoader | async | High | `async void Load` | חריגה מפילה את התהליך; אי אפשר ל-await/לבדוק | `async Task LoadAsync` |
| 4 | S2_ReportLoader | async | High | `.Result` בתוך async | deadlock ב-UI/ASP.NET, חסימת thread | `await` |
| 5 | S2_ReportLoader | משאבים | Med | `new HttpClient()` ב-`using` בכל קריאה (וגם בלולאה) | מיצוי sockets (TIME_WAIT) תחת עומס | HttpClient מוזרק / IHttpClientFactory |
| 6 | S2_ReportLoader | async | Low | אין `CancellationToken` | אי אפשר לבטל קריאות איטיות | פרמטר ct |
| 7 | S3_PriceParser | תרבות | High | `double.Parse(parts[1])` בלי culture | ב-de-DE "19.90" → 1990; מחירים שגויים ×100 | `decimal.TryParse(..., InvariantCulture)` |
| 8 | S3_PriceParser | טיפוס | Med | `double` לכסף ואז cast ל-decimal | שגיאות עיגול | decimal ישירות |
| 9 | S3_PriceParser | חריגות | High | `catch {}` מדלג על שורות פגומות בשקט | ייבוא "מצליח" עם חצי מהנתונים | זריקת `FormatException` עם מספר שורה |
| 10 | S3_PriceParser | תרבות/זמן | Med | `DateTime.Parse` תלוי-culture, `DateTime.Now` לתוקף | תאריך 03/01 מתפרש הפוך; אזורי זמן | `DateOnly.TryParseExact` + `TimeProvider` |
| 11 | S4_TokenCache | thread-safety | High | `Dictionary` נכתב ממספר threads | השחתת מבנה, חריגות אקראיות | `ConcurrentDictionary` + `Lazy<Task>` (fetch פעם אחת) |
| 12 | S4_TokenCache | זמן | Med | `DateTime.Now` לתפוגה | שעון קיץ / אזור זמן מזיזים תפוגה; לא בדיק | `TimeProvider.GetUtcNow()` |
| 13 | S4_TokenCache | חריגות | Low | כשל ב-fetch נשאר… (בגרסה המתוקנת) — לוודא שלא נשמר במטמון | token ריק לנצח | הסרה מהמטמון ב-catch + rethrow |
| 14 | S5_Paginator | off-by-one | High | `page * pageSize` עבור עמודים 1-based; `i <= end` | עמוד 1 מדלג על פריטים; חריגה בעמוד האחרון | `(page-1)*pageSize`, `i < end` |
| 15 | S5_Paginator | נכונות | Med | `PageCount = total / pageSize` (רצפה) | העמוד האחרון "נעלם" | ceiling |
| 16 | S6_CustomerSearch | אבטחה — SQL injection | High | שרשור קלט משתמש ל-SQL | גניבת/מחיקת נתונים | פרמטרים (`@pattern`, `@id`), נטרול wildcards |
| 17 | S6_CustomerSearch | טיפוס | Low | `id` כמחרוזת | injection + שגיאות | `int id` |
| 18 | S7_SettingsService | סודות | High | מפתח API כברירת מחדל בקוד | דולף בגיט/בינארי | הסרה; קריאה מסביבה/user-secrets/Key Vault |
| 19 | S7_SettingsService | סודות/לוגים | High | `Console.WriteLine` של המפתח | סודות בלוגים | לא לרשום ערכים |
| 20 | S7_SettingsService | null | Med | `Deserialize` יכול להחזיר null (אזהרה CS8603) | NullReferenceException מאוחר | `?? throw InvalidDataException` |
| 21 | S7_SettingsService | ביצועים | Low | `new JsonSerializerOptions` בכל שמירה | אובדן cache פנימי | `static readonly` |
| 22 | S7_SettingsService | תפעול | Low | כתיבה ליד ה-exe (`BaseDirectory`) | אין הרשאות ב-Program Files | `LocalApplicationData` |

## False positives טיפוסיים של AI (שראינו)
- "S5: `List<T>` צריך להיות `IEnumerable<T>`" — סגנון, לא באג (אם כי `IReadOnlyList` עדיף לאינדוקס).
- "S1: `Directory.CreateDirectory` לא בטוח ל-threads" — הוא בטוח לקריאה חוזרת.
- "S3: צריך `try/catch` סביב כל שורה" — ההפך: בליעה הייתה הבאג.

</div>
