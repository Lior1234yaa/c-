# מודול 06 — טיפול בחריגות ואסטרטגיות דיבוג

תוכנה נכשלת: קובץ לא קיים, המשתמש הקליד "abc" במקום מספר, הרשת נפלה, ומישהו ניסה למשוך יותר ממה שיש בחשבון. ההבדל בין תוכנה טובה לרעה הוא לא שהראשונה לא נכשלת, אלא שהיא נכשלת **בצורה מבוקרת**: הודעה ברורה, מצב עקבי, משאבים משוחררים. במודול הזה נלמד את מנגנון החריגות של .NET, מתי להשתמש בו ומתי לא, ואז את הכלי שיחסוך לכם הכי הרבה שעות בקריירה — הדיבאגר.

## מה זו חריגה

חריגה (exception) היא אובייקט שמתאר שגיאה, ו"נזרק" (`throw`) מהמקום שבו התגלתה. הזריקה עוצרת את הריצה הרגילה ומטפסת במעלה ה-call stack עד שמישהו "תופס" אותה (`catch`). אם אף אחד לא תופס — התוכנית קורסת עם stack trace. כל החריגות יורשות מ-`System.Exception`, ויש לכל אחת `Message`, `StackTrace` ו-`InnerException` (החריגה שגרמה לה).

חריגות נפוצות שתפגשו: `NullReferenceException` (גישה ל-null), `ArgumentException` / `ArgumentNullException` / `ArgumentOutOfRangeException` (פרמטר לא תקין), `InvalidOperationException` (הפעולה לא חוקית במצב הנוכחי), `FormatException` (פרסינג), `KeyNotFoundException`, `IndexOutOfRangeException`, `IOException`, `HttpRequestException`.

## `try` / `catch` / `finally`

```csharp
try
{
    var text = File.ReadAllText(path);
    var count = int.Parse(text);
    Console.WriteLine(100 / count);
}
catch (FileNotFoundException ex)             // ספציפי קודם
{
    Console.WriteLine($"missing file: {ex.FileName}");
}
catch (FormatException)                       // אפשר בלי משתנה
{
    Console.WriteLine("file does not contain a number");
}
catch (Exception ex)                          // כללי — אחרון
{
    Console.WriteLine($"unexpected: {ex.GetType().Name}: {ex.Message}");
    throw;                                    // ראו למטה
}
finally
{
    Console.WriteLine("runs always — with or without exception");   // ניקוי
}
```

- ה-`catch` הראשון שמתאים (לפי סדר, כולל ירושה) מטפל. לכן ספציפי לפני כללי — המהדר אפילו יזהיר אם הסדר הפוך.
- `finally` רץ תמיד: אחרי הצלחה, אחרי `catch`, אחרי `return` בתוך ה-`try`, ואפילו אחרי `continue`. זה המקום לשחרר משאבים.
- אפשר `try/finally` בלי `catch` — "לא מטפל, אבל מנקה".

### Exception filters — `when`

פילטר מאפשר לתפוס רק כשמתקיים תנאי, בלי לתפוס ולזרוק מחדש:

```csharp
catch (InsufficientFundsException ex) when (ex.Shortfall > 1000)
{
    EscalateToManager(ex);
}
catch (InsufficientFundsException ex)
{
    Console.WriteLine(ex.Message);
}
catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { }
```

יתרון עדין: כשהפילטר מחזיר `false`, ה-stack לא "נפרש" — ובדיבאגר תראו את מקום הזריקה המקורי. טריק נפוץ ללוגים: `catch (Exception ex) when (Log(ex))` כש-`Log` מחזיר `false` — רושם ולא תופס.

## `throw;` מול `throw ex;`

כשתופסים חריגה ורוצים להעביר אותה הלאה (אחרי לוג, למשל):

```csharp
catch (Exception ex)
{
    logger.Log(ex);
    throw;          // נכון: שומר את ה-stack trace המקורי
    // throw ex;    // שגוי: ה-stack trace מתחיל מכאן — איבדנו איפה זה באמת קרה (CA2200)
}
```

אם רוצים לעטוף בחריגה משלכם, העבירו את המקורית כ-`InnerException`: `throw new DataAccessException("failed to load orders", ex);`. כך לא מאבדים מידע.

## חריגות מותאמות אישית

הגדירו חריגה משלכם כשלמטפל יש **מה לעשות** עם המידע: סוג שגיאה עסקית, נתונים נלווים.

```csharp
public class BankException(string message) : Exception(message);   // בסיס לכל החריגות של הדומיין

public class InsufficientFundsException(string accountId, decimal requested, decimal available)
    : BankException($"Insufficient funds in {accountId}: requested {requested:N2}, available {available:N2}")
{
    public string AccountId { get; } = accountId;
    public decimal Requested { get; } = requested;
    public decimal Available { get; } = available;
    public decimal Shortfall => Requested - Available;
}
```

מוסכמות: סיומת `Exception`, ירושה מ-`Exception` (לא `ApplicationException`), הודעה ברורה שנבנית בבנאי, properties לנתונים. היררכיה (`BankException` כבסיס) מאפשרת `catch (BankException)` אחד לכל השגיאות העסקיות, ו-`catch` ספציפי כשצריך.

## `using` ו-`IDisposable`

אובייקטים שמחזיקים משאב חיצוני — קובץ, חיבור DB, socket, `HttpClient` — מממשים `IDisposable`. חייבים לקרוא ל-`Dispose()` כשמסיימים, **גם אם הייתה חריגה**. `using` עושה בדיוק את זה: `try/finally` שקורא ל-`Dispose`.

```csharp
using (var writer = new StreamWriter("log.txt"))
{
    writer.WriteLine("hello");
}   // Dispose() כאן — הקובץ נסגר וה-buffer נכתב

using var reader = new StreamReader("data.csv");   // using declaration (C# 8): Dispose בסוף הבלוק המקיף
var header = reader.ReadLine();
```

בלי `using`, קובץ עלול להישאר פתוח (ותוכנית אחרת לא תוכל לגשת אליו) או שה-buffer לא ייכתב לדיסק (הלוג "ריק"). כשאתם כותבים מחלקה שמחזיקה `IDisposable` — ממשו `IDisposable` בעצמכם והעבירו את ה-`Dispose` הלאה.

## Guard clauses — נכשלים מוקדם

בדקו פרמטרים **בכניסה** למתודה וזרקו מיד. השגיאה נתפסת קרוב למקור, ושאר המתודה נקי מ-`if`-ים:

```csharp
public void Transfer(Account? from, Account? to, decimal amount)
{
    ArgumentNullException.ThrowIfNull(from);
    ArgumentNullException.ThrowIfNull(to);
    ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amount);
    if (ReferenceEquals(from, to)) throw new ArgumentException("same account", nameof(to));
    // מכאן — הכל תקין
}

public Book(string isbn, string title)
{
    ArgumentException.ThrowIfNullOrWhiteSpace(isbn);
    ArgumentException.ThrowIfNullOrEmpty(title);
    // ...
}
```

המתודות הסטטיות `ThrowIfNull`, `ThrowIfNullOrEmpty`, `ThrowIfNullOrWhiteSpace`, `ThrowIfNegative`, `ThrowIfNegativeOrZero`, `ThrowIfGreaterThan` וכו' (מ-.NET 6–8) חוסכות שורות ומשתמשות אוטומטית בשם הפרמטר בהודעה.

## מתי **לא** לזרוק: ה-TryParse pattern

חריגות יקרות (יחסית) ומיועדות למצבים **חריגים**. קלט לא תקין מהמשתמש הוא לא חריג — הוא צפוי. לכן ל-.NET יש זוגות `Parse`/`TryParse`:

```csharp
Console.Write("age: ");
var line = Console.ReadLine();
if (int.TryParse(line, out int age) && age is >= 0 and <= 120)
    Console.WriteLine($"ok, {age}");
else
    Console.WriteLine("please enter a number between 0 and 120");
```

אותו רעיון: `Dictionary.TryGetValue`, `Dictionary.TryAdd`, `Queue.TryDequeue`, `Enum.TryParse`, `DateTime.TryParse`. ולמתודות שלכם: אם כישלון הוא תוצאה שגרתית (חיפוש שלא מצא), החזירו `bool`/`null`; אם הוא הפרת כללים (משיכה מעל היתרה, פרמטר לא חוקי) — זרקו.

לולאת קלט טיפוסית: לקרוא, `TryParse`, אם לא תקין — הודעה ולקרוא שוב; אם `ReadLine()` מחזיר `null` (סוף הקלט) — לצאת בצורה מסודרת.

## אסטרטגיות דיבוג ב-Visual Studio

`Console.WriteLine` הוא לא דיבוג. הדיבאגר מאפשר לעצור את התוכנית, להסתכל על כל משתנה, ולהתקדם שורה-שורה.

| כלי | איך | מתי |
|---|---|---|
| **Breakpoint** | `F9` על שורה, `F5` להריץ עד אליה | "מה קורה כאן?" |
| **Step Over / Into / Out** | `F10` / `F11` / `Shift+F11` | שורה-שורה; להיכנס למתודה; לצאת ממנה |
| **Conditional breakpoint** | קליק ימני על הנקודה → Conditions: `amount > 1000` או Hit Count | הבאג קורה רק באיטרציה ה-347 |
| **Watch / Locals / Autos** | Debug → Windows → Watch; הוסיפו ביטוי (`from.Balance`, `list.Count`) | לעקוב אחרי ערכים תוך כדי צעדים |
| **Immediate Window** | `Ctrl+Alt+I`; הקלידו ביטויים: `_accounts.Keys`, `Calc(5)` | לבדוק השערה בלי לשנות קוד |
| **Call Stack** | `Ctrl+Alt+C` | "איך הגעתי לכאן?" — לחיצה כפולה קופצת לפריים |
| **Exception Settings** | `Ctrl+Alt+E`; סמנו סוג חריגה → break when thrown | לעצור **במקום הזריקה**, לא ב-catch |
| **Edit & Continue** | שנו קוד בזמן שהתוכנית מושהית, המשיכו | לתקן ולבדוק בלי להריץ מחדש |
| **DataTips / Pin** | ריחוף מעל משתנה; הצמדה עם הסיכה | הצצה מהירה |
| **Run to Cursor** | `Ctrl+F10` | "רוץ עד כאן" בלי נקודת עצירה |

אסטרטגיה: (1) **שחזרו** את הבאג באופן דטרמיניסטי; (2) שערו איפה הוא — נקודת עצירה **לפני** המקום החשוד; (3) `F10` עם Watch על המשתנים המעורבים עד שהערך "מתקלקל"; (4) תקנו, הריצו שוב, ורצוי — כתבו בדיקה (יום 2). כשיש חריגה — Exception Settings + Call Stack מראים את מקור הבעיה, לא רק את הסימפטום.

ב-VS Code: אותם מושגים ב-Run and Debug (`F5`), עם `launch.json` שנוצר אוטומטית. `dotnet run` לבד לא מריץ דיבאגר.

## `Debug.Assert` ולוגים

- **`Debug.Assert(condition, "message")`** — בודק הנחה בזמן פיתוח. אם התנאי `false`, הדיבאגר עוצר. ב-Release השורה **נמחקת** לחלוטין — אפס עלות. השתמשו ל-invariants פנימיים ("היתרה לא יכולה להיות שלילית כאן"), לא לוולידציית קלט.
- **`Debug.WriteLine`** — הדפסה לחלון Output של הדיבאגר, רק ב-Debug.
- **לוגים** (`ILogger` מ-`Microsoft.Extensions.Logging`, יום 2) — לייצור. רמות: Trace/Debug/Information/Warning/Error/Critical. רשמו את החריגה כולה (`logger.LogError(ex, "transfer failed")`), לא רק `ex.Message`.

## טעויות נפוצות

- **`catch (Exception) { }` ריק** — "בולע" שגיאות; התוכנה ממשיכה במצב שבור ומישהו יגלה את זה שבוע אחר כך. לפחות לוג, ובדרך כלל `throw;`.
- **`throw ex;`** — איבוד stack trace.
- **חריגות לזרימת בקרה** — `try { int.Parse } catch` בלולאה. `TryParse`.
- **`catch` רחב מדי ברמה נמוכה** — תפסו מה שאתם יודעים לטפל בו; תנו לשאר לעלות למי שיודע.
- **בלי `using`** על קבצים/חיבורים — דליפת משאבים, קבצים נעולים, לוג ריק.
- **הודעה חסרת מידע** — `throw new Exception("error")`. כתבו מה, איפה, ואיזה ערך.
- **`Debug.Assert` לוולידציית קלט** — נעלם ב-Release והבדיקה לא תרוץ.
- **דיבוג עם `Console.WriteLine`** כשיש דיבאגר — איטי, מלכלך את הקוד, ונשכח בפרודקשן.

## לסיכום

- `try/catch/finally`: ספציפי לפני כללי, `finally` תמיד רץ, `when` לסינון.
- `throw;` שומר stack; חריגות מותאמות עם נתונים ובסיס משותף.
- `using` לכל `IDisposable` — שחרור מובטח.
- Guard clauses (`ArgumentException.ThrowIfNullOrEmpty`...) בכניסה; `TryParse` לקלט צפוי; חריגות למצבים חריגים.
- דיבאגר: breakpoints (גם מותנים), Step, Watch, Immediate, Call Stack, Exception Settings, Edit & Continue.
- `Debug.Assert` להנחות פנימיות בפיתוח; לוגים לייצור.

## קריאה נוספת

- [Exceptions and exception handling](https://learn.microsoft.com/dotnet/csharp/fundamentals/exceptions/)
- [Best practices for exceptions](https://learn.microsoft.com/dotnet/standard/exceptions/best-practices-for-exceptions)
- [Exception filters (when)](https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/when)
- [using statement](https://learn.microsoft.com/dotnet/csharp/language-reference/statements/using)
- [Implement a Dispose method](https://learn.microsoft.com/dotnet/standard/garbage-collection/implementing-dispose)
- [Debugging in Visual Studio](https://learn.microsoft.com/visualstudio/debugger/)
- [Navigate through code with the debugger](https://learn.microsoft.com/visualstudio/debugger/navigating-through-code-with-the-debugger)
- [Manage exceptions with the debugger](https://learn.microsoft.com/visualstudio/debugger/managing-exceptions-with-the-debugger)
