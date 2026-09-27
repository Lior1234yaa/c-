using System.Diagnostics;
using System.Runtime.CompilerServices;

// ===============================================================
// Day2.Demo.AsyncAwait — Task, async/await, WhenAll/WhenAny, חריגות,
// CancellationToken, IProgress<T>, IAsyncEnumerable<T>, ValueTask.
// הרצה: dotnet run
// ===============================================================

Console.OutputEncoding = System.Text.Encoding.UTF8;

Console.WriteLine("======== 1. Task basics ========");
await Demo1_TaskBasics();
Console.WriteLine("\n======== 2. async/await mechanics ========");
await Demo2_AwaitMechanics();
Console.WriteLine("\n======== 3. WhenAll / WhenAny / Delay ========");
await Demo3_Combinators();
Console.WriteLine("\n======== 4. Exceptions ========");
await Demo4_Exceptions();
Console.WriteLine("\n======== 5. Cancellation ========");
await Demo5_Cancellation();
Console.WriteLine("\n======== 6. Progress ========");
await Demo6_Progress();
Console.WriteLine("\n======== 7. IAsyncEnumerable ========");
await Demo7_AsyncStreams();
Console.WriteLine("\n======== 8. ValueTask ========");
await Demo8_ValueTask();
Console.WriteLine("\ndone.");

// ---------------------------------------------------------------
static async Task Demo1_TaskBasics()
{
    // Task = "הבטחה" לעבודה שתסתיים בעתיד. Task<T> = הבטחה לערך.
    Task<int> compute = Task.Run(() =>
    {
        Console.WriteLine($"  computing on thread {Environment.CurrentManagedThreadId}...");
        return Enumerable.Range(1, 1000).Sum();
    });

    Console.WriteLine($"  status before await: {compute.Status}");
    int result = await compute;                 // ממתין בלי לחסום את התהליכון
    Console.WriteLine($"  result = {result}, status after: {compute.Status}");

    // Task שכבר הושלם — שימושי ב-cache / stubs
    Task<string> ready = Task.FromResult("cached");
    Console.WriteLine($"  Task.FromResult -> {await ready} (IsCompleted={ready.IsCompleted})");
}

// ---------------------------------------------------------------
static async Task Demo2_AwaitMechanics()
{
    // המהדר הופך מתודת async ל-state machine: כל await הוא "נקודת השהיה".
    // הקוד לפני ה-await הראשון רץ סינכרונית על התהליכון הקורא!
    Console.WriteLine($"  [caller] thread {Environment.CurrentManagedThreadId}");
    var task = SlowGreeting("Dana");
    Console.WriteLine("  [caller] SlowGreeting returned a Task immediately — I can do other work");
    var greeting = await task;
    Console.WriteLine($"  [caller] got: {greeting} on thread {Environment.CurrentManagedThreadId}");

    static async Task<string> SlowGreeting(string name)
    {
        Console.WriteLine($"  [SlowGreeting] part 1 runs synchronously, thread {Environment.CurrentManagedThreadId}");
        await Task.Delay(200);   // כאן המתודה "מחזירה" Task לקורא וממשיכה מאוחר יותר
        Console.WriteLine($"  [SlowGreeting] part 2 resumed on thread {Environment.CurrentManagedThreadId}");
        return $"Hello, {name}!";
    }
}

// ---------------------------------------------------------------
static async Task Demo3_Combinators()
{
    var sw = Stopwatch.StartNew();

    // סדרתי: 300+200+100 = ~600ms
    await FakeDownload("a", 300); await FakeDownload("b", 200); await FakeDownload("c", 100);
    Console.WriteLine($"  sequential: {sw.ElapsedMilliseconds} ms");

    // מקבילי: WhenAll מחכה לכולם, ~300ms
    sw.Restart();
    string[] all = await Task.WhenAll(FakeDownload("a", 300), FakeDownload("b", 200), FakeDownload("c", 100));
    Console.WriteLine($"  WhenAll: {sw.ElapsedMilliseconds} ms -> [{string.Join(", ", all)}]");

    // WhenAny — הראשון שמסיים (למשל: timeout או "המראה" הכי מהירה)
    sw.Restart();
    var slow = FakeDownload("slow-server", 800);
    var fast = FakeDownload("fast-server", 150);
    Task<string> winner = await Task.WhenAny(slow, fast);
    Console.WriteLine($"  WhenAny winner: {await winner} after {sw.ElapsedMilliseconds} ms");

    // timeout עם WaitAsync (.NET 6+)
    try
    {
        await FakeDownload("very-slow", 2000).WaitAsync(TimeSpan.FromMilliseconds(300));
    }
    catch (TimeoutException) { Console.WriteLine("  WaitAsync: timed out after 300 ms (as expected)"); }

    static async Task<string> FakeDownload(string name, int ms)
    {
        await Task.Delay(ms);
        return $"{name}:{ms}ms";
    }
}

// ---------------------------------------------------------------
static async Task Demo4_Exceptions()
{
    // await "פורס" את החריגה המקורית — לא AggregateException
    try { await Fail("boom"); }
    catch (InvalidOperationException ex) { Console.WriteLine($"  await -> caught {ex.GetType().Name}: {ex.Message}"); }

    // .Wait()/.Result עוטפים ב-AggregateException (וגם מסוכנים — ראו מודול 06)
    try { Fail("boom2").Wait(); }
    catch (AggregateException ex) { Console.WriteLine($"  .Wait() -> caught {ex.GetType().Name} with {ex.InnerExceptions.Count} inner: {ex.InnerException?.Message}"); }

    // WhenAll: await זורק רק את הראשונה; כולן ב-task.Exception
    var whenAll = Task.WhenAll(Fail("x"), Fail("y"), Task.Delay(10));
    try { await whenAll; }
    catch (InvalidOperationException)
    {
        Console.WriteLine($"  WhenAll -> await threw the first; all errors: " +
            string.Join(", ", whenAll.Exception!.InnerExceptions.Select(e => e.Message)));
    }

    // async void — רק ל-event handlers! חריגה ממנו לא ניתנת לתפיסה ע"י הקורא.
    Console.WriteLine("  (async void: exceptions escape to the SynchronizationContext / crash the process — avoid)");

    static async Task Fail(string msg)
    {
        await Task.Delay(10);
        throw new InvalidOperationException(msg);
    }
}

// ---------------------------------------------------------------
static async Task Demo5_Cancellation()
{
    using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(350)); // ביטול אוטומטי אחרי 350ms
    try
    {
        await LongJob(cts.Token);
        Console.WriteLine("  job finished (unexpected)");
    }
    catch (OperationCanceledException)
    {
        Console.WriteLine("  job was cancelled (OperationCanceledException) — cooperative cancellation works!");
    }

    // ביטול ידני + Register
    using var cts2 = new CancellationTokenSource();
    cts2.Token.Register(() => Console.WriteLine("  [callback] token cancelled"));
    var job = LongJob(cts2.Token);
    await Task.Delay(120);
    cts2.Cancel();
    try { await job; } catch (OperationCanceledException) { Console.WriteLine("  manual cancel confirmed"); }

    static async Task LongJob(CancellationToken ct)
    {
        for (int i = 1; i <= 10; i++)
        {
            ct.ThrowIfCancellationRequested();          // בדיקה שיתופית
            Console.WriteLine($"  step {i}/10");
            await Task.Delay(100, ct);                   // גם Delay מכבד את הטוקן
        }
    }
}

// ---------------------------------------------------------------
static async Task Demo6_Progress()
{
    // IProgress<T> — דיווח התקדמות בטוח ל-UI thread (Progress<T> תופס את ה-SynchronizationContext)
    var progress = new Progress<int>(pct => Console.WriteLine($"  progress: {pct}%"));
    await ProcessFiles(5, progress);

    static async Task ProcessFiles(int count, IProgress<int> progress)
    {
        for (int i = 1; i <= count; i++)
        {
            await Task.Delay(60);
            progress.Report(i * 100 / count);
        }
    }
}

// ---------------------------------------------------------------
static async Task Demo7_AsyncStreams()
{
    // IAsyncEnumerable<T>: תוצאות "מטפטפות" — לא צריך לחכות לכל הרשימה
    using var cts = new CancellationTokenSource();
    await foreach (var page in FetchPages(4, cts.Token))
    {
        Console.WriteLine($"  received {page}");
        if (page == "page-3") cts.Cancel(); // ניתן לעצור באמצע
    }

    static async IAsyncEnumerable<string> FetchPages(int count, [EnumeratorCancellation] CancellationToken ct = default)
    {
        for (int i = 1; i <= count; i++)
        {
            if (ct.IsCancellationRequested) yield break;
            await Task.Delay(80, CancellationToken.None);
            yield return $"page-{i}";
        }
    }
}

// ---------------------------------------------------------------
static async Task Demo8_ValueTask()
{
    // ValueTask<T> חוסך הקצאת Task כשהתשובה לרוב זמינה מיידית (cache hit)
    var cache = new Dictionary<int, string>();
    for (int i = 0; i < 3; i++)
        Console.WriteLine($"  GetUser(7) -> {await GetUser(7)}");

    async ValueTask<string> GetUser(int id)
    {
        if (cache.TryGetValue(id, out var hit)) return hit + " (cached, no Task allocated)";
        await Task.Delay(50);
        return cache[id] = $"user#{id}";
    }
}
