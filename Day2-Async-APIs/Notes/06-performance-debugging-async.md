# מודול 06 — ביצועים ודיבוג של קוד אסינכרוני

## הבאג מספר 1: Sync-over-Async

הטעות הכי נפוצה (והכי כואבת) בקוד אסינכרוני היא לחסום על Task: `.Result`, `.Wait()`, `.GetAwaiter().GetResult()`. למה זה רע?

1. **חסימת תהליכון** — התהליכון יושב ומחכה במקום לעבוד. בשרת עם 100 בקשות מקבילות, 100 תהליכונים חסומים = ThreadPool ריק = "הרעבה" (thread starvation) וה-latency קופץ לשניות.
2. **Deadlock ב-UI ובסביבות עם SynchronizationContext** (WPF, WinForms, ASP.NET הישן). התסריט:

```csharp
// WPF button click — קופא לנצח!
private void Button_Click(object sender, RoutedEventArgs e)
{
    var data = LoadAsync().Result;          // 1. תהליכון ה-UI נחסם וממתין ל-Task
    textBlock.Text = data;
}

private async Task<string> LoadAsync()
{
    await Task.Delay(1000);                 // 2. ההמשך (continuation) צריך לחזור לתהליכון ה-UI...
    return "done";                          // 3. ...אבל הוא חסום ב-.Result. אף אחד לא זז.
}
```

הפתרון: **async all the way**. מהרגע שיש `await` בתחתית, כל השרשרת עד למעלה חייבת להיות `async` — כולל ה-event handler (`async void`) ו-`Main` (`static async Task Main`).

```csharp
private async void Button_Click(object sender, RoutedEventArgs e)
{
    textBlock.Text = await LoadAsync();     // תהליכון ה-UI משוחרר בזמן ההמתנה
}
```

בקונסול ובשרתי ASP.NET Core **אין** SynchronizationContext, ולכן `.Result` לא יגרום ל-deadlock — אבל עדיין יחסום תהליכון. אל תתרגלו לזה.

## `ConfigureAwait(false)` — לספריות

אחרי `await`, ברירת המחדל היא לחזור ל-**context** המקורי (תהליכון ה-UI ב-WPF). זה מה שמאפשר לעדכן פקדים אחרי `await` בלי `Dispatcher`. אבל בקוד **ספרייה** — שלא נוגע ב-UI — החזרה הזו מיותרת, יקרה, והיא הצד השני של ה-deadlock שראינו:

```csharp
// בתוך ספרייה / קוד תשתית — לא ב-UI ולא ב-controller:
public async Task<string> GetDataAsync()
{
    var response = await _http.GetAsync(url).ConfigureAwait(false);   // "לא אכפת לי על איזה תהליכון להמשיך"
    return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
}
```

כללים: בספריות NuGet ובקוד שיתופי — `ConfigureAwait(false)` על כל `await`. באפליקציה עצמה (WPF, קונסול, ASP.NET Core) — לא צריך; ב-WPF זה אפילו יזיק, כי אחרי `await` לא תוכלו לגעת ב-UI.

## מתי **לא** להשתמש ב-`Task.Run`

`Task.Run` לוקח תהליכון מה-Pool. זה נכון לעבודת CPU כבדה שלא רוצים שתחסום את ה-UI. זה **שגוי** ב:

- **סביב IO** — `Task.Run(() => http.GetStringAsync(url))` תופס תהליכון רק כדי שיחכה. פשוט `await http.GetStringAsync(url)`.
- **בשרת ASP.NET Core** — הבקשה כבר רצה על תהליכון Pool; `Task.Run` רק מוסיף context switch ומתחרה על אותם תהליכונים.
- **כ"תיקון" ל-deadlock** — `Task.Run(() => X().Result)` "עובד" אבל שורף שני תהליכונים במקום אפס.
- **בתוך ספרייה** — תנו לקורא להחליט; ספרייה חושפת `async` אמיתי, לא "fake async".

השימוש הנכון: `var result = await Task.Run(() => HeavyCpuWork(data));` מה-UI, כדי שהחלון לא יקפא.

## הימנעות מהרעבת ThreadPool

ה-ThreadPool מוסיף תהליכונים לאט (בערך אחד לשנייה מעבר למספר הליבות). אם 50 בקשות עושות `.Result` בו-זמנית, המערכת "נחנקת" לעשרות שניות. סימנים: latency שעולה בהדרגה תחת עומס, CPU נמוך, `ThreadPool.ThreadCount` שמטפס לאט. הפתרונות, לפי סדר עדיפות: (1) לבטל את החסימה — `await`; (2) עבודות ארוכות באמת ב-`TaskCreationOptions.LongRunning` או `new Thread`; (3) כמוצא אחרון `ThreadPool.SetMinThreads` — טיפול בסימפטום.

## מדידה: `Stopwatch` ו-`dotnet-counters`

לפני שמייעלים — מודדים. הכלי הבסיסי:

```csharp
var sw = Stopwatch.StartNew();
await ProcessAllAsync();
sw.Stop();
Console.WriteLine($"took {sw.ElapsedMilliseconds} ms ({sw.Elapsed.TotalSeconds:F2}s)");

// השוואה הוגנת: להריץ כל גרסה כמה פעמים, "חימום" ראשון לא נספר, ולהשוות חציונים
```

- `DateTime.Now` **לא** מתאים למדידה — רזולוציה גסה ותלוי בשעון המערכת. `Stopwatch` (או `Stopwatch.GetTimestamp()` + `GetElapsedTime`) הוא הכלי.
- למדידות micro (מיקרו-שניות, השוואת שתי מימושים) — ספריית **BenchmarkDotNet**.
- למעקב אחרי תהליך חי: **`dotnet-counters`** (`dotnet tool install -g dotnet-counters`, ואז `dotnet-counters monitor -p <pid>`) מציג בזמן אמת את `ThreadPool Thread Count`, `ThreadPool Queue Length`, GC, חריגות לשנייה. תור שגדל ומספר תהליכונים שמטפס לאט = הרעבה.

```csharp
// "מונים" זולים שאפשר להדפיס בעצמכם:
Console.WriteLine($"pool threads: {ThreadPool.ThreadCount}, pending: {ThreadPool.PendingWorkItemCount}");
```

## דיבוג async ב-Visual Studio

קוד אסינכרוני נראה "קופץ" בדיבאגר: `await` משחרר תהליכון, וההמשך רץ על תהליכון אחר. הכלים שעוזרים (Debug ▸ Windows):

- **Call Stack עם "async call stacks"** — VS מציג את השרשרת הלוגית (`[Async] Main → LoadAsync → ...`) גם כשהתהליכון הפיזי התחלף. אם רואים רק `ThreadPoolWorkQueue.Dispatch` — ודאו ש-"Show external code" כבוי.
- **Tasks window** (`Ctrl+Shift+D, K`) — כל ה-Tasks החיים: סטטוס (Scheduled / Running / Awaiting / Blocked), מה הם מחכים לו, ו-**Deadlocked** כשיש מעגל המתנה. זו הדרך הכי מהירה לאתר sync-over-async.
- **Parallel Stacks** (`Ctrl+Shift+D, S`) — תרשים של כל התהליכונים/ה-Tasks ומחסניות הקריאה שלהם; מצב "Tasks" מראה איזה Task מחכה לאיזה.
- **Threads window** (`Ctrl+Alt+H`) — רשימת תהליכונים; אפשר להקפיא (Freeze) תהליכון ולראות איך השני מתנהג — מצוין לשחזור race conditions.
- **Breakpoint מותנה** עם `System.Threading.Thread.CurrentThread.ManagedThreadId == 7` כדי לעצור רק בתהליכון מסוים.
- ב-VS Code: `.NET Debugger` תומך ב-async stacks ו-Threads; Tasks window אין.

## לוגים עם `ILogger`

בקוד מקבילי `Console.WriteLine` מערבב שורות ולא אומר מי כתב. `Microsoft.Extensions.Logging` נותן רמות (Trace/Debug/Information/Warning/Error/Critical), קטגוריות, ופלט מובנה:

```csharp
// dotnet add package Microsoft.Extensions.Logging.Console   (בקונסול; ב-ASP.NET Core מובנה)
using Microsoft.Extensions.Logging;

using var factory = LoggerFactory.Create(b => b.AddSimpleConsole(o => o.TimestampFormat = "HH:mm:ss.fff ").SetMinimumLevel(LogLevel.Debug));
ILogger logger = factory.CreateLogger("Dashboard");

logger.LogInformation("fetching {Source} (attempt {Attempt})", "weather", 2);   // structured: לא string interpolation!
try { await FetchAsync(); }
catch (HttpRequestException ex) { logger.LogWarning(ex, "fetch failed for {Source}", "weather"); }
```

שימו לב לתבנית `{Source}` במקום `$"..."` — כך הלוגר שומר את הפרמטרים כשדות (structured logging) שאפשר לסנן ולחפש. במעבדה 4 נבנה logger פשוט על `Channel<T>` — אותו רעיון: הכתיבה לא חוסמת, ותהליכון אחד מסדר את הפלט.

## צ'קליסט באגים נפוצים

| סימפטום | הסיבה הסבירה | תיקון |
|---------|--------------|-------|
| התוכנית/החלון קופא | `.Result` / `.Wait()` על תהליכון UI | `await` עד למעלה |
| "עובד בדיבאג, נכשל בריצה" | race condition / הנחה על סדר | `lock`, `Interlocked`, אוספים מקביליים |
| חריגה "נעלמת" | Task בלי `await` / `async void` | `await` הכל, `async Task` |
| אזהרה CS4014 | קריאה ל-async בלי `await` | להוסיף `await` (או `_ = ` + טיפול בחריגות במכוון) |
| `ObjectDisposedException` אחרי await | `using` שנסגר לפני שההמשך רץ / `HttpClient` ב-using | להאריך חיים / מופע אחד |
| הכל סדרתי למרות async | `await` בלולאה במקום `WhenAll` | לאסוף Tasks ואז `WhenAll` |
| Latency עולה תחת עומס, CPU נמוך | הרעבת ThreadPool | להסיר חסימות; `dotnet-counters` |
| `OperationCanceledException` בלוג כשגיאה | ביטול נתפס ב-`catch (Exception)` | לתפוס בנפרד |
| `await` בתוך `lock` לא מתקמפל | `lock` לא תומך | `SemaphoreSlim(1,1)` |
| UI לא מתעדכן אחרי `ConfigureAwait(false)` | ההמשך על תהליכון Pool | להסיר `ConfigureAwait` בקוד UI |

## טעויות נפוצות

- **`Task.Run` "כדי שיהיה אסינכרוני"** סביב IO.
- **מדידה עם `DateTime.Now`** או ריצה בודדת בלי חימום.
- **`ConfigureAwait(false)` בכל מקום** "כי ככה אמרו" — כולל בקוד UI שאחר כך נוגע בפקדים.
- **לוג ב-string interpolation** (`$"..."`) — מאבד את המבנה ומחשב את המחרוזת גם כשהרמה כבויה.
- **בדיקת ביצועים ב-Debug** במקום Release — JIT בלי אופטימיזציות.

## לסיכום

- Sync-over-async (`.Result`/`.Wait()`) = חסימה + deadlock ב-UI. הפתרון: async all the way.
- `ConfigureAwait(false)` בספריות בלבד; `Task.Run` רק לעבודת CPU מה-UI.
- למדוד עם `Stopwatch` (וכמה ריצות), לעקוב עם `dotnet-counters`, להשוות עם BenchmarkDotNet.
- ב-Visual Studio: Tasks window, Parallel Stacks, async call stacks, Threads window.
- `ILogger` עם structured logging במקום `Console.WriteLine`.

## קריאה נוספת

- [Async/await best practices (Stephen Cleary, MSDN Magazine)](https://learn.microsoft.com/en-us/archive/msdn-magazine/2013/march/async-await-best-practices-in-asynchronous-programming)
- [ConfigureAwait FAQ (.NET Blog)](https://devblogs.microsoft.com/dotnet/configureawait-faq/)
- [Debug an async application (Tasks window)](https://learn.microsoft.com/en-us/visualstudio/debugger/using-the-tasks-window)
- [Using the Parallel Stacks window](https://learn.microsoft.com/en-us/visualstudio/debugger/using-the-parallel-stacks-window)
- [dotnet-counters](https://learn.microsoft.com/en-us/dotnet/core/diagnostics/dotnet-counters)
- [Logging in .NET](https://learn.microsoft.com/en-us/dotnet/core/extensions/logging)
- [Diagnosing ThreadPool starvation](https://learn.microsoft.com/en-us/dotnet/core/diagnostics/debug-threadpool-starvation)
