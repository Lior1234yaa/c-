# מודול 01 — מבוא ל-Multithreading ותכנות מקבילי

## למה בכלל צריך יותר מתהליכון אחד?

עד עכשיו כל התוכניות שכתבנו רצו "בקו ישר": שורה אחרי שורה, פעולה אחרי פעולה. זה פשוט, אבל יש לזה שני מחירים גדולים:

1. **בזבוז זמן המתנה** — כשהתוכנית מחכה לרשת, לדיסק או למסד נתונים, המעבד יושב בטל. בקשת HTTP ממוצעת לוקחת 50–500 מילישניות; בזמן הזה המעבד יכול לבצע מיליוני פעולות.
2. **בזבוז ליבות** — כמעט לכל מחשב היום יש 4, 8 או 16 ליבות. תוכנית סדרתית משתמשת באחת מהן בלבד.

**Concurrency** (מקביליות "לוגית") פירושה שכמה משימות מתקדמות *לסירוגין* — למשל, מחכים לשלוש בקשות רשת בו-זמנית. **Parallelism** (מקביליות פיזית) פירושו שכמה משימות רצות *באותו רגע ממש* על ליבות שונות. ב-.NET שני המונחים נפגשים באותם כלים, ולכן חשוב להבין את ההבדל כבר בהתחלה: מקביליות עוזרת לעבודת **CPU-bound**; מקביליות לוגית (ובעיקר `async`) עוזרת לעבודת **IO-bound**.

## Process מול Thread

- **Process (תהליך)** — תוכנית שרצה: מרחב זיכרון משלה, קבצים פתוחים, הרשאות. שני תהליכים לא רואים את הזיכרון זה של זה.
- **Thread (תהליכון)** — "נתיב ביצוע" בתוך תהליך. לכל תהליכון יש מחסנית (Stack) משלו ומצביע להוראה הנוכחית, אבל **כל התהליכונים באותו תהליך חולקים את אותו Heap** — אותם אובייקטים, אותם שדות סטטיים.

השיתוף הזה הוא גם הכוח וגם הסכנה: קל להעביר מידע בין תהליכונים (פשוט משתנה משותף), אבל קל באותה מידה להרוס אותו.

```csharp
using System.Diagnostics;

var proc = Process.GetCurrentProcess();
Console.WriteLine($"Process {proc.Id} has {proc.Threads.Count} threads");
Console.WriteLine($"Main thread id: {Environment.CurrentManagedThreadId}");
Console.WriteLine($"Cores: {Environment.ProcessorCount}");
```

גם תוכנית "Hello World" מכילה כבר כ-10 תהליכונים: ה-Garbage Collector, ה-Finalizer, ה-JIT ועוד עובדים ברקע.

## המחלקה `Thread`

הדרך הכי "נמוכה" ליצור תהליכון היא המחלקה `System.Threading.Thread`. נותנים לה delegate, קוראים ל-`Start`, ובסוף ל-`Join` כדי לחכות לסיום:

```csharp
var worker = new Thread(() =>
{
    for (int i = 1; i <= 3; i++)
    {
        Console.WriteLine($"[worker {Environment.CurrentManagedThreadId}] step {i}");
        Thread.Sleep(100);
    }
})
{
    Name = "MyWorker",
    IsBackground = true   // תהליכון רקע לא מונע מהתהליך להסתיים
};

worker.Start();
Console.WriteLine("main continues...");
worker.Join();            // חוסם את main עד שה-worker מסיים
```

כמה דברים שכדאי לדעת:

- `Thread.Sleep(ms)` משהה את **התהליכון הנוכחי** — לא את התוכנית כולה.
- `IsBackground = false` (ברירת המחדל) פירושו שהתהליך ימשיך לרוץ עד שהתהליכון יסיים. שכחתם לסיים לולאה אינסופית? התוכנית לא תיסגר.
- כל תהליכון מקבל כברירת מחדל מחסנית של ~1MB. אלף תהליכונים = ~1GB זיכרון וירטואלי, עוד לפני שעשו משהו.

## ThreadPool — לא ליצור, למחזר

יצירת תהליכון היא פעולה יקרה (הקצאת מחסנית, רישום במערכת ההפעלה, context switch). לכן .NET מחזיק **ThreadPool**: בריכה של תהליכונים מוכנים שמקבלים "פריטי עבודה" קצרים:

```csharp
ThreadPool.QueueUserWorkItem(_ =>
{
    Console.WriteLine($"on pool thread {Environment.CurrentManagedThreadId}, " +
                      $"IsThreadPoolThread={Thread.CurrentThread.IsThreadPoolThread}");
});
```

בפועל כמעט לעולם לא נקרא ל-`ThreadPool` ישירות — `Task.Run`, `Parallel.For` ו-`async/await` (מודול 02) משתמשים בו בשבילנו. מה שכן חשוב להבין:

- ה-Pool מתחיל עם מספר תהליכונים כמספר הליבות, וגדל **לאט** (בערך תהליכון אחד לשנייה) כשכולם תפוסים. לכן חסימת תהליכוני Pool (למשל עם `.Result`) גורמת ל"הרעבה" — ראו מודול 06.
- פריטי עבודה אמורים להיות קצרים. עבודה ארוכה מאוד? `new Thread` או `Task.Factory.StartNew(..., TaskCreationOptions.LongRunning)`.

## Race Condition — הבעיה המרכזית

הנה הדוגמה הקלאסית. שני תהליכונים מגדילים מונה משותף מיליון פעמים כל אחד:

```csharp
int counter = 0;
var t1 = new Thread(() => { for (int i = 0; i < 1_000_000; i++) counter++; });
var t2 = new Thread(() => { for (int i = 0; i < 1_000_000; i++) counter++; });
t1.Start(); t2.Start();
t1.Join();  t2.Join();
Console.WriteLine(counter);   // 2,000,000? כמעט אף פעם! למשל 1,176,871
```

למה? כי `counter++` הוא **לא פעולה אחת**. המעבד מבצע שלושה שלבים: קורא את הערך לרגיסטר, מוסיף 1, כותב חזרה. אם שני תהליכונים קוראים את אותו ערך (נניח 41) לפני שמישהו כתב, שניהם יכתבו 42 — ועדכון אחד "נעלם". זה **Race Condition**: התוצאה תלויה במי הגיע קודם, והיא לא דטרמיניסטית. הבאג הזה לא מופיע בבדיקות, מופיע אצל הלקוח, ולא ניתן לשחזור. מודול 03 מוקדש כולו לפתרונות (`lock`, `Interlocked`, אוספים מקביליים).

## CPU-bound מול IO-bound

זו ההבחנה הכי חשובה ביום הזה, כי היא קובעת איזה כלי לבחור:

| | CPU-bound | IO-bound |
|---|---|---|
| דוגמאות | עיבוד תמונה, הצפנה, מיון גדול, חישוב מדעי | HTTP, מסד נתונים, קבצים, `Task.Delay` |
| הצוואר | המעבד עסוק | מחכים למשהו חיצוני; המעבד פנוי |
| הפתרון | **מקביליות**: `Parallel.For`, PLINQ, `Task.Run` | **אסינכרוניות**: `async/await`, `HttpClient.GetAsync` |
| כמה מהר אפשר? | עד מספר הליבות | מאות/אלפי פעולות במקביל — בלי תהליכונים בכלל |

טעות נפוצה: לפתוח תהליכון (או `Task.Run`) כדי "לחכות" לרשת. התהליכון יושב ומחכה, תופס זיכרון ולא עושה כלום. עבור IO רוצים `await`, שמשחרר את התהליכון בזמן ההמתנה.

```csharp
// IO-bound: 4 המתנות של 100ms
var sw = Stopwatch.StartNew();
for (int i = 0; i < 4; i++) Thread.Sleep(100);        // ~400 ms, תהליכון תפוס
Console.WriteLine(sw.ElapsedMilliseconds);

sw.Restart();
await Task.WhenAll(Task.Delay(100), Task.Delay(100), Task.Delay(100), Task.Delay(100));
Console.WriteLine(sw.ElapsedMilliseconds);            // ~100 ms, אפס תהליכונים תפוסים
```

## `Parallel.For` ו-`Parallel.ForEach`

לעבודת CPU על אוסף, ה-TPL (Task Parallel Library) נותן לולאות מקביליות מוכנות. הן מחלקות את הטווח בין תהליכוני ה-Pool ומחכות לסיום כולם:

```csharp
var data = Enumerable.Range(1, 20).ToArray();
var squares = new int[data.Length];

Parallel.For(0, data.Length, i => squares[i] = data[i] * data[i]);
// כל אינדקס נכתב פעם אחת בלבד -> בטוח, בלי נעילה

var files = new[] { "a.jpg", "b.jpg", "c.jpg", "d.jpg" };
Parallel.ForEach(files, new ParallelOptions { MaxDegreeOfParallelism = 2 }, file =>
{
    Console.WriteLine($"{file} on thread {Environment.CurrentManagedThreadId}");
});
```

כללים חשובים:

- **הסדר לא מובטח.** אם צריך סדר — אספו תוצאות למערך לפי אינדקס.
- **אסור לכתוב לאוסף רגיל** (`List<T>`, `Dictionary`) מתוך הגוף. תקבלו נתונים חסרים או חריגה. השתמשו במערך לפי אינדקס או באוספים מקביליים (מודול 03).
- `MaxDegreeOfParallelism` שולט בכמה תהליכונים ירוצו בו-זמנית — חשוב כשהעבודה נוגעת במשאב מוגבל.
- חריגה בתוך הלולאה נזרקת כ-`AggregateException` שמכילה את כל השגיאות.
- יש גם `Parallel.ForEachAsync` (.NET 6+) לגרסה אסינכרונית עם `CancellationToken`.

## PLINQ — LINQ מקבילי

מוסיפים `.AsParallel()` לשאילתת LINQ, וה-runtime מחלק את העבודה בין הליבות:

```csharp
var primes = Enumerable.Range(1, 1_500_000)
    .AsParallel()
    .Where(IsPrime)
    .Count();

// לשמירת הסדר המקורי (במחיר ביצועים):
var firstFive = Enumerable.Range(1, 1_000_000).AsParallel().AsOrdered()
    .Where(n => n % 1000 == 0).Take(5);
```

PLINQ משתלם רק כשהעבודה על כל פריט **יקרה**. עבור `Where(n => n % 7 == 0)` על מיליון מספרים, הגרסה הסדרתית תהיה מהירה יותר — ה-overhead של חלוקת העבודה ומיזוג התוצאות גדול מהרווח. תמיד למדוד (`Stopwatch`, מודול 06).

## טעויות נפוצות

- **לכידת משתנה לולאה** — `for (int i...) ThreadPool.QueueUserWorkItem(_ => Console.WriteLine(i))` ידפיס ערך שגוי. תמיד `int n = i;` בתוך הלולאה (ב-`foreach` הבעיה תוקנה ב-C# 5, ב-`for` לא).
- **לשכוח `Join`** — התוכנית מסיימת לפני שהתהליכון עשה משהו (או להפך, לא מסיימת בגלל `IsBackground = false`).
- **גישה ל-UI מתהליכון אחר** — ב-WPF/WinForms רק תהליכון ה-UI רשאי לגעת בפקדים. הפתרון: `Dispatcher.Invoke` / `Control.Invoke`, או פשוט `async/await` (יום 3).
- **הנחה שהקוד רץ בסדר מסוים** — אם התוכנית נכונה רק כש-t1 מסיים לפני t2, היא לא נכונה.
- **`Parallel.For` על גוף זול מאוד** — ה-overhead גדול מהרווח. למדוד.

## לסיכום

- Process = זיכרון נפרד; Thread = נתיב ביצוע שחולק את הזיכרון עם אחיו.
- `Thread` יקר; `ThreadPool` ממחזר. ברוב הקוד המודרני נשתמש ב-`Task` ולא ב-`Thread`.
- Race condition נוצר כשכמה תהליכונים כותבים לאותו מקום בלי סנכרון. `counter++` הוא שלוש פעולות.
- CPU-bound → מקביליות (`Parallel`, PLINQ). IO-bound → `async/await` (המודול הבא).
- הסדר בלולאות מקביליות לא מובטח, ואסור לכתוב לאוספים רגילים מתוכן.

## קריאה נוספת

- [Threads and threading](https://learn.microsoft.com/en-us/dotnet/standard/threading/threads-and-threading)
- [The managed thread pool](https://learn.microsoft.com/en-us/dotnet/standard/threading/the-managed-thread-pool)
- [Parallel programming in .NET](https://learn.microsoft.com/en-us/dotnet/standard/parallel-programming/)
- [Data parallelism (Parallel.For/ForEach)](https://learn.microsoft.com/en-us/dotnet/standard/parallel-programming/data-parallelism-task-parallel-library)
- [Parallel LINQ (PLINQ)](https://learn.microsoft.com/en-us/dotnet/standard/parallel-programming/introduction-to-plinq)
