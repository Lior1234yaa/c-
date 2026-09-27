<div dir="rtl">

# Lab 3 — הערות על הפתרון

- `FINDINGS.md` מכיל את טבלת הממצאים המלאה (22 ממצאים) ואת ה-false positives.
- הפרויקט המתוקן בונה עם `TreatWarningsAsErrors` + `AnalysisLevel latest-recommended`: ה-analyzers תופסים לבד את CS8603 (null מ-Deserialize) — דוגמה לכך שהכלים משלימים את הסוקר.
- `Program.cs` מריץ את אותן בדיקות עצמיות של ה-Starter (מותאמות לחתימות החדשות) ומדפיס `ALL CHECKS PASSED`; קוד יציאה 0.
- שינויים בחתימות היו **מכוונים**: `PriceParser(TimeProvider)`, `TokenCache(fetch, TimeProvider)`, `ReportLoader(HttpClient)`, `BuildByIdQuery(int)`, `Query` עם פרמטרים — ב-review אמיתי היינו מתעדים אותם כ-breaking changes ב-PR.
- S4: `Lazy<Task<...>>` בתוך `ConcurrentDictionary` היא התבנית המקובלת ל-"fetch פעם אחת גם תחת עומס"; `GetOrAdd` לבדו יכול להריץ את ה-factory יותר מפעם אחת.

## טעויות AI טיפוסיות בשלב התיקון
| טעות | תיקון |
|------|-------|
| `lock` סביב `await` (לא מתקמפל) או `SemaphoreSlim` גלובלי שמסלסל הכול | `ConcurrentDictionary` + `Lazy<Task>` |
| החלפת `catch {}` ב-`catch (Exception ex) { Console.WriteLine(ex); }` — עדיין בולע | זריקה עם הקשר |
| "תיקון" של path traversal ע"י `Replace("..", "")` | `GetFileName` + `GetFullPath` + בדיקת prefix |
| SQL: `name.Replace("'", "''")` במקום פרמטרים | פרמטרים תמיד |
| הסרת `ApiKey` מהמודל אבל הוספת `const string ApiKey` במקום אחר | סוד רק מהסביבה |

</div>
