<div dir="rtl">

# מודול 4 — סקירה ואימות של קוד שנוצר ב-AI

## הגישה: Trust but Verify

קוד מ-AI נראה בטוח בעצמו: הוא מעוצב יפה, יש לו שמות טובים ולפעמים אפילו הערות. זו בדיוק הסכנה — הוא **נראה** נכון. הגישה שלנו: מתייחסים לכל פלט כאל PR של מפתח מוכשר שהגיע אתמול לצוות. מקבלים בשמחה, בודקים הכול.

## צ'ק-ליסט הסקירה

| תחום | שאלות |
|------|-------|
| **נכונות** | האם זה עונה על הדרישה? האם יש בדיקה שמוכיחה? |
| **מקרי קצה** | `null`, ריק, אפס, שלילי, אוסף ריק, מחרוזת ארוכה, Unicode/עברית |
| **אבטחה** | הזרקת SQL/פקודות, סודות בקוד, path traversal (`Path.Combine` עם קלט משתמש), deserialization לא בטוח, לוגים עם נתונים אישיים |
| **ביצועים** | N+1, `ToList()` מיותר, מחרוזות בלולאה, Regex בלי cache, `JsonSerializerOptions` חדש בכל קריאה |
| **Async** | `async void`, `.Result`/`.Wait()`, חוסר `ConfigureAwait` בספריות, `CancellationToken` שלא מועבר |
| **חריגות** | `catch (Exception) {}` שבולע, `throw ex` שמוחק stack trace, חריגות לזרימת בקרה |
| **משאבים** | `IDisposable` בלי `using`, `HttpClient` חדש בכל קריאה, קבצים שלא נסגרים |
| **רישוי/העתקה** | קוד שנראה מועתק מספרייה עם רישיון לא תואם; אין ייחוס |
| **תלויות** | חבילות NuGet חדשות — נחוצות? מתוחזקות? גרסה? |
| **עקביות** | תואם למוסכמות, לשכבות ולסגנון של הפרויקט (מודול 5) |

## אימות עם כלים (לא רק עיניים)

### בדיקות
`dotnet test` — אם אין בדיקות, זו ההזדמנות לבקש מה-AI לכתוב, ואז לקרוא **אותן** בקפידה. בדיקה שמאשרת התנהגות שגויה גרועה מכלום.

### בדיקות כאימות — דוגמה
כשה-AI מחזיר מימוש, בקשו ממנו גם בדיקה — ואז **קראו את הבדיקה לפני המימוש**. בדיקה טובה מתארת התנהגות ולא מעתיקה נוסחה:

<div dir="ltr">

```csharp
[Theory]
[InlineData("052-1234567", true)]
[InlineData("x052-1234567y", false)]   // בלי עוגנים — היה עובר
[InlineData("952-1234567", false)]
public void IsraeliMobile_MatchesOnlyFullNumbers(string input, bool expected)
    => Assert.Equal(expected, Phones.IsraeliMobile().IsMatch(input));
```

</div>

אם ה-AI כתב בדיקה שעוברת על קוד שגוי (למשל בודקת רק את המקרה החיובי), הבדיקה עצמה היא הממצא הראשון של הסקירה. כלל אצבע: לכל תיקון באג — בדיקה שנכשלה לפני ועוברת אחרי.

### Analyzers ו-`dotnet format`

<div dir="ltr">

```xml
<!-- Directory.Build.props או ה-csproj -->
<PropertyGroup>
  <Nullable>enable</Nullable>
  <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
  <AnalysisLevel>latest-recommended</AnalysisLevel>
  <EnforceCodeStyleInBuild>true</EnforceCodeStyleInBuild>
</PropertyGroup>
```

</div>

- **Roslyn analyzers** מובנים ב-SDK (`CAxxxx`) — `AnalysisLevel` קובע כמה מהם פעילים.
- **`dotnet format`** מיישר סגנון לפי `.editorconfig`; ב-CI: `dotnet format --verify-no-changes`.
- **SonarLint / SonarQube**, **Roslynator**, **StyleCop.Analyzers** — כלים נפוצים נוספים (בדקו בתיעוד).
- `TreatWarningsAsErrors` הופך "אזהרה שהתעלמנו ממנה" ל"build שנכשל" — עוזר מאוד עם קוד AI שנוטה לייצר אזהרות nullable.

### הרצה
לקוד עם קלט/פלט: הריצו עם קלט אמיתי, כולל עברית, תאריכים ומספרים בפורמט ישראלי.

## דוגמאות לבאגים עדינים שנפוצים בפלט AI (C#)

### 1. Off-by-one

<div dir="ltr">

```csharp
// AI: "return the last n items"
return items.Skip(items.Count - n - 1).ToList();   // באג: מחזיר n+1
return items.Skip(Math.Max(0, items.Count - n)).ToList(); // תיקון
```

</div>

### 2. `async void`

<div dir="ltr">

```csharp
private async void LoadAsync() { await _api.GetAsync(); }        // חריגה תפיל את התהליך
private async Task LoadAsync() { await _api.GetAsync(); }        // תיקון (async void רק ל-event handlers)
```

</div>

### 3. Parsing תלוי-תרבות

<div dir="ltr">

```csharp
decimal.Parse("19.90");                                        // בתרבות עם פסיק עשרוני → חריגה/ערך שגוי
decimal.Parse("19.90", CultureInfo.InvariantCulture);           // תיקון לנתוני מכונה
```

</div>

### 4. `DateTime.Now` לעומת `UtcNow`

<div dir="ltr">

```csharp
var expires = DateTime.Now.AddHours(1);      // שעון קיץ / אזורי זמן → באג
var expires = DateTimeOffset.UtcNow.AddHours(1);   // תיקון; ועדיף להזריק IClock/TimeProvider
```

</div>

### 5. `HttpClient` ב-`using` בכל קריאה

<div dir="ltr">

```csharp
using var client = new HttpClient();     // AI אוהב את זה: מיצוי sockets תחת עומס
// תיקון: IHttpClientFactory או מופע static/מוזרק אחד
```

</div>

### 6. Thread-safety

<div dir="ltr">

```csharp
private readonly Dictionary<string, int> _cache = new();      // גישה ממספר threads → השחתה
private readonly ConcurrentDictionary<string, int> _cache = new();   // תיקון
```

</div>

### 7. בליעת חריגות

<div dir="ltr">

```csharp
try { Save(); } catch { }              // "לא נפל" ≠ "עבד"
try { Save(); } catch (IOException ex) { _logger.LogError(ex, "Save failed"); throw; }
```

</div>

### 8. Path traversal

<div dir="ltr">

```csharp
var path = Path.Combine(root, userFileName);   // "..\..\secrets.txt"
var full = Path.GetFullPath(Path.Combine(root, userFileName));
if (!full.StartsWith(Path.GetFullPath(root) + Path.DirectorySeparatorChar)) throw new UnauthorizedAccessException();
```

</div>

כל הדוגמאות האלה מופיעות בהרחבה ב-Lab 3 ובתרגילים.

## AI סוקר AI: Second-pass prompts

הכלי שכתב את הקוד "מאמין" בו. בקשו סקירה **בשיחה חדשה** או בכלי אחר, עם צ'ק-ליסט:

<div dir="ltr">

```text
Review the following C# code as a strict senior reviewer.
Check specifically: null handling, async correctness (async void, blocking), culture-sensitive
parsing/formatting, IDisposable usage, thread safety, exception swallowing, injection/path traversal.
For each finding: severity, line, why it matters, minimal fix. Do not rewrite the file.
```

</div>

עוד תבניות: "List the assumptions this code makes." / "Write 5 inputs that would break this." / "What would a security auditor flag here?"

## סקירת PR עם Copilot / Claude

- **GitHub Copilot code review**: אפשר לבקש מ-Copilot סקירה על PR (כ-reviewer) — הוא משאיר הערות בשורות. אפשר להגדיר הוראות סקירה ב-`copilot-instructions.md`.
- **Claude Code** יכול לסקור diff מקומי (`git diff`) או PR דרך אינטגרציית GitHub — בדקו בתיעוד.
- בכל מקרה: הערות AI הן **קלט** לסוקר האנושי, לא החלטה. הסוקר האנושי מאשר merge.

## תהליך סקירה מומלץ (10 דקות לקוד קטן)

1. קראו את הדרישה שוב. 2. הריצו build + tests + format. 3. עברו על הצ'ק-ליסט. 4. בקשו סקירת AI שנייה. 5. תקנו, הריצו שוב. 6. commit עם הסבר.

## טעויות נפוצות

- לסקור רק את מה שה-AI הדגיש; לא לפתוח את שאר הקובץ ששונה.
- לסמוך על "I tested this" בפלט של הכלי — אלא אם ראיתם את הבדיקה רצה.
- לתקן באג עם עוד prompt בלי להבין את הסיבה — לפעמים הכלי "מתקן" על ידי הסתרה (`try/catch`).
- לדלג על analyzers "כי זה רק דמו".

## לסיכום

- קוד AI נראה בטוח — לכן צריך צ'ק-ליסט ולא אינטואיציה.
- כלים: בדיקות, analyzers, `TreatWarningsAsErrors`, `dotnet format`, הרצה עם קלט אמיתי.
- הכירו את הבאגים החוזרים: async void, culture, Now/UtcNow, HttpClient, thread-safety, בליעת חריגות, path traversal.
- AI כסוקר שני — בשיחה חדשה, עם צ'ק-ליסט; ההחלטה אנושית.

## קריאה נוספת

- Code analysis in .NET: https://learn.microsoft.com/dotnet/fundamentals/code-analysis/overview
- `dotnet format`: https://learn.microsoft.com/dotnet/core/tools/dotnet-format
- Async guidance (David Fowler): https://github.com/davidfowl/AspNetCoreDiagnosticScenarios/blob/master/AsyncGuidance.md
- IHttpClientFactory: https://learn.microsoft.com/dotnet/core/extensions/httpclient-factory
- Copilot code review: https://docs.github.com/copilot/using-github-copilot/code-review/using-copilot-code-review
- Secure coding guidelines: https://learn.microsoft.com/dotnet/standard/security/secure-coding-guidelines

</div>
