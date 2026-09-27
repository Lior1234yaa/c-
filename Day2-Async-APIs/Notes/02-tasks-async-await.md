<div dir="rtl">

# מודול 02 — Tasks ו-async/await

## מ-Thread ל-Task

במודול הקודם ראינו ש-`Thread` הוא כלי נמוך ויקר. ב-.NET המודרני העבודה נעשית עם **`Task`** — אובייקט שמייצג "עבודה שתסתיים מתישהו". `Task` לא אומר "תהליכון"; הוא אומר **הבטחה** (Promise). `Task<T>` הוא הבטחה לערך מסוג `T`.

<div dir="ltr">

```csharp
Task<int> compute = Task.Run(() =>
{
    // רץ על תהליכון מה-ThreadPool
    return Enumerable.Range(1, 1000).Sum();
});

Console.WriteLine(compute.Status);      // Running / WaitingToRun
int result = await compute;             // מחכים לתוצאה בלי לחסום
Console.WriteLine(compute.Status);      // RanToCompletion
```

</div>

`Task.Run` שולח עבודת **CPU** ל-ThreadPool. עבור **IO** (רשת, קבצים) לא צריך `Task.Run` בכלל — הספריות מחזירות `Task` בעצמן (`HttpClient.GetAsync`, `File.ReadAllTextAsync`), והמתנה עליהן לא תופסת שום תהליכון. זו נקודה מבלבלת שנחזור אליה במודול 06.

## `async` ו-`await` — איך זה עובד באמת

הכללים התחביריים פשוטים:

1. מתודה שמסומנת `async` יכולה להשתמש ב-`await` בתוכה.
2. היא מחזירה `Task`, `Task<T>` (או `ValueTask`, ובמקרה מיוחד אחד `void`).
3. `await` על `Task<T>` נותן `T`; `await` על `Task` נותן כלום.

<div dir="ltr">

```csharp
static async Task<string> DownloadTitleAsync(string url)
{
    using var http = new HttpClient();
    string html = await http.GetStringAsync(url);   // 1. שולחים בקשה, "משחררים" את התהליכון
    return html.Length > 100 ? html[..100] : html;   // 2. ממשיכים כשהתשובה הגיעה
}
```

</div>

מה קורה מאחורי הקלעים? המהדר הופך את המתודה ל-**מכונת מצבים (state machine)**. כל `await` הוא "נקודת השהיה":

1. הקוד **עד** ה-`await` הראשון רץ סינכרונית, על התהליכון שקרא למתודה.
2. כשמגיעים ל-`await` על Task שעדיין לא הסתיים, המתודה **מחזירה** לקורא Task לא-גמור ומשחררת את התהליכון.
3. כשה-Task הפנימי מסתיים, ה-runtime מזמן את "ההמשך" (continuation) — שאר המתודה — ומריץ אותו (בקונסול: על תהליכון Pool כלשהו; ב-UI: על תהליכון ה-UI).
4. כשהמתודה מגיעה ל-`return`, ה-Task שהוחזר בשלב 2 מסומן כגמור והקורא שלו מתעורר.

<div dir="ltr">

```csharp
Console.WriteLine($"[caller] thread {Environment.CurrentManagedThreadId}");
var task = SlowGreeting("Dana");
Console.WriteLine("[caller] got a Task immediately, doing other work...");
Console.WriteLine(await task);

static async Task<string> SlowGreeting(string name)
{
    Console.WriteLine("[SlowGreeting] part 1 — synchronous");
    await Task.Delay(200);                 // כאן חוזרים לקורא
    Console.WriteLine("[SlowGreeting] part 2 — resumed later");
    return $"Hello, {name}!";
}
```

</div>

המסקנה החשובה: `await` **לא חוסם**. הוא אומר "כשזה יסתיים, תמשיך מכאן". בזמן ההמתנה התהליכון פנוי לעבודות אחרות — לטפל בבקשה נוספת בשרת, או להגיב ללחיצות בממשק המשתמש.

## `Task.Delay` במקום `Thread.Sleep`

`Thread.Sleep(1000)` תופס תהליכון למשך שנייה. `await Task.Delay(1000)` משחרר אותו ומחזיר אחרי שנייה. בקוד אסינכרוני — תמיד `Task.Delay`.

## הרכבה: `WhenAll` ו-`WhenAny`

הכוח האמיתי מגיע כשמריצים כמה פעולות IO **בו-זמנית**:

<div dir="ltr">

```csharp
// סדרתי: ~600 ms
var a = await FetchAsync("a", 300);
var b = await FetchAsync("b", 200);
var c = await FetchAsync("c", 100);

// מקבילי: ~300 ms — כולן יוצאות לדרך מיד, מחכים לכולן
string[] all = await Task.WhenAll(FetchAsync("a", 300), FetchAsync("b", 200), FetchAsync("c", 100));

// הראשונה שמסיימת
Task<string> winner = await Task.WhenAny(FetchAsync("slow", 800), FetchAsync("fast", 150));
Console.WriteLine(await winner);

// timeout בשורה אחת (.NET 6+)
var result = await FetchAsync("x", 5000).WaitAsync(TimeSpan.FromSeconds(1));  // TimeoutException
```

</div>

שימו לב לדפוס: קודם **מפעילים** את כל ה-Tasks (בלי `await`), ורק אז `await Task.WhenAll(...)`. אם תכתבו `await` על כל אחת בנפרד — תקבלו ביצוע סדרתי.

## חריגות בקוד אסינכרוני

חריגה בתוך מתודת `async` לא נזרקת מיד — היא **נשמרת בתוך ה-Task**. היא "מתפוצצת" רק כשמישהו עושה `await`:

<div dir="ltr">

```csharp
static async Task FailAsync()
{
    await Task.Delay(10);
    throw new InvalidOperationException("boom");
}

try { await FailAsync(); }
catch (InvalidOperationException ex) { /* החריגה המקורית, כרגיל */ }

try { FailAsync().Wait(); }              // אל תעשו את זה (ראו מודול 06)...
catch (AggregateException ex)            // ...אבל אם כן — היא עטופה
{ Console.WriteLine(ex.InnerException!.Message); }
```

</div>

- `await` פורס את החריגה המקורית. `.Wait()` ו-`.Result` עוטפים ב-`AggregateException`.
- ב-`Task.WhenAll` עם כמה כישלונות, `await` זורק רק את **הראשון**; כל השאר ב-`task.Exception.InnerExceptions`.
- Task שאף אחד לא עשה עליו `await` ("fire and forget") — החריגה שלו נבלעת בשקט. סכנה.

## `async void` — רק ל-Event Handlers

מתודת `async void` לא מחזירה Task, ולכן: אי אפשר לחכות לה, אי אפשר לתפוס ממנה חריגות (הן מפילות את התהליך), ואי אפשר לבדוק אותה. הסיבה היחידה לקיומה: חתימות של event handlers חייבות להחזיר `void`:

<div dir="ltr">

```csharp
private async void SaveButton_Click(object sender, EventArgs e)   // OK — event handler
{
    try { await SaveAsync(); }
    catch (Exception ex) { ShowError(ex); }   // חובה לתפוס בפנים!
}

private async void DoWork() { ... }          // רע — צריך להיות async Task
```

</div>

## ביטול: `CancellationToken`

פעולות ארוכות חייבות להיות ניתנות לביטול — המשתמש לחץ Cancel, הבקשה בשרת התנתקה, עבר timeout. ב-.NET הביטול **שיתופי**: מי שמבטל מסמן דגל, ומי שעובד בודק אותו.

<div dir="ltr">

```csharp
using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));   // ביטול אוטומטי אחרי 2 שניות
// או: cts.Cancel() ידנית, למשל מ-Console.CancelKeyPress (Ctrl+C)

try
{
    await LongJobAsync(cts.Token);
}
catch (OperationCanceledException)
{
    Console.WriteLine("cancelled");
}

static async Task LongJobAsync(CancellationToken ct)
{
    for (int i = 0; i < 100; i++)
    {
        ct.ThrowIfCancellationRequested();    // בדיקה בין שלבים
        await Task.Delay(100, ct);            // גם פעולות IO מקבלות טוקן
    }
}
```

</div>

- `CancellationTokenSource` הוא "השלט"; `CancellationToken` הוא מה שמעבירים לפונקציות.
- כמעט כל API אסינכרוני ב-.NET מקבל `CancellationToken` כפרמטר אחרון. **תמיד להעביר אותו הלאה.**
- ביטול מסתיים ב-`OperationCanceledException` (או תת-המחלקה `TaskCanceledException`). זה לא "שגיאה" — זה הדרך התקינה לצאת.
- `ct.Register(() => ...)` מריץ callback ברגע הביטול — שימושי לניקוי.

## דיווח התקדמות: `IProgress<T>`

<div dir="ltr">

```csharp
var progress = new Progress<int>(pct => Console.WriteLine($"{pct}%"));   // ב-UI: מעדכן ProgressBar
await ProcessFilesAsync(files, progress);

static async Task ProcessFilesAsync(string[] files, IProgress<int> progress)
{
    for (int i = 0; i < files.Length; i++)
    {
        await ProcessOneAsync(files[i]);
        progress.Report((i + 1) * 100 / files.Length);
    }
}
```

</div>

`Progress<T>` "זוכר" את ה-SynchronizationContext שבו נוצר, ולכן ב-WPF ה-callback רץ על תהליכון ה-UI אוטומטית — בלי `Dispatcher`.

## זרמים אסינכרוניים: `IAsyncEnumerable<T>`

כשהתוצאות מגיעות בהדרגה (עמודים מ-API, שורות מקובץ ענק), לא רוצים לחכות לכולן. `IAsyncEnumerable<T>` הוא `IEnumerable` ש-`MoveNext` שלו אסינכרוני:

<div dir="ltr">

```csharp
await foreach (var page in FetchPagesAsync(cts.Token))
    Console.WriteLine(page);

static async IAsyncEnumerable<string> FetchPagesAsync(
    [EnumeratorCancellation] CancellationToken ct = default)
{
    for (int i = 1; i <= 10; i++)
    {
        await Task.Delay(100, ct);          // "הורדת עמוד"
        yield return $"page-{i}";
    }
}
```

</div>

## `ValueTask<T>` — בקצרה

`Task` הוא אובייקט על ה-Heap. במתודות שנקראות מיליוני פעמים ולרוב מחזירות תשובה מיידית (cache hit), ההקצאה הזו מורגשת. `ValueTask<T>` הוא struct שחוסך אותה במקרה הסינכרוני. כלל אצבע: החזירו `Task` כברירת מחדל; `ValueTask` רק אחרי מדידה, ולעולם אל תעשו עליו `await` פעמיים.

## טעויות נפוצות

- **לשכוח `await`** — `DoAsync();` בלי await מריץ את הפעולה ברקע וממשיך. המהדר מזהיר (CS4014). חריגות נבלעות.
- **`await` בלולאה כשאפשר `WhenAll`** — ביצוע סדרתי במקום מקבילי.
- **`async void`** מחוץ ל-event handler.
- **`.Result` / `.Wait()`** — חסימה, ובחלק מהסביבות deadlock (מודול 06).
- **`Task.Run` סביב IO** — תהליכון שלם שרק מחכה.
- **לא להעביר `CancellationToken` הלאה** — הביטול "נעצר" באמצע השרשרת.
- **`catch (Exception)` שבולע `OperationCanceledException`** — ביטול הופך ל"שגיאה" מוזרה. תפסו אותו בנפרד.

## לסיכום

- `Task` = הבטחה; `await` = "תמשיך מכאן כשזה מוכן" — בלי לחסום תהליכון.
- מתודת `async` הופכת למכונת מצבים; החלק עד ה-`await` הראשון רץ סינכרונית.
- `WhenAll` להרצה מקבילית של IO, `WhenAny` ל"הראשון שמסיים", `WaitAsync` ל-timeout.
- חריגות נשמרות ב-Task ונזרקות ב-`await` (המקורית) או ב-`.Wait()` (עטופה ב-`AggregateException`).
- `CancellationToken` בכל מתודה אסינכרונית; `IProgress<T>` להתקדמות; `IAsyncEnumerable<T>` לזרמים.

## קריאה נוספת

- [Asynchronous programming with async and await](https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/)
- [Task asynchronous programming model](https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/task-asynchronous-programming-model)
- [Async return types](https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/async-return-types)
- [Cancellation in managed threads](https://learn.microsoft.com/en-us/dotnet/standard/threading/cancellation-in-managed-threads)
- [Task-based asynchronous pattern (TAP)](https://learn.microsoft.com/en-us/dotnet/standard/asynchronous-programming-patterns/task-based-asynchronous-pattern-tap)

</div>
