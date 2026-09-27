using System.Collections.Concurrent;
using System.Diagnostics;

// ===============================================================
// Day2.Demo.Synchronization — lock / Lock, Monitor, Interlocked,
// SemaphoreSlim, ReaderWriterLockSlim, אוספים מקביליים, deadlock,
// immutability ו-Lazy<T> singleton.
// ===============================================================

Console.OutputEncoding = System.Text.Encoding.UTF8;

Console.WriteLine("======== 1. lock & the C# 13 Lock type ========");
Demo1_Lock();
Console.WriteLine("\n======== 2. Monitor.TryEnter ========");
Demo2_Monitor();
Console.WriteLine("\n======== 3. Interlocked ========");
Demo3_Interlocked();
Console.WriteLine("\n======== 4. SemaphoreSlim (async throttling) ========");
await Demo4_SemaphoreSlim();
Console.WriteLine("\n======== 5. ReaderWriterLockSlim ========");
Demo5_ReaderWriterLock();
Console.WriteLine("\n======== 6. Concurrent collections ========");
Demo6_ConcurrentCollections();
Console.WriteLine("\n======== 7. Deadlock & lock ordering ========");
Demo7_Deadlock();
Console.WriteLine("\n======== 8. Immutability & Lazy<T> singleton ========");
Demo8_ImmutableAndLazy();
Console.WriteLine("\ndone.");

// ---------------------------------------------------------------
static void Demo1_Lock()
{
    var account = new Account(1000);
    // 10 תהליכונים מושכים 10 פעמים 10 ש"ח — היתרה חייבת להגיע בדיוק ל-0
    var threads = Enumerable.Range(0, 10).Select(_ => new Thread(() =>
    {
        for (int i = 0; i < 10; i++) account.Withdraw(10);
    })).ToArray();
    foreach (var t in threads) t.Start();
    foreach (var t in threads) t.Join();
    Console.WriteLine($"  balance = {account.Balance} (expected 0), failed withdrawals = {account.Failed}");
}

// ---------------------------------------------------------------
static void Demo2_Monitor()
{
    // lock(x) { ... } == Monitor.Enter(x); try { ... } finally { Monitor.Exit(x); }
    // Monitor.TryEnter מאפשר timeout במקום המתנה אינסופית
    var gate = new object();
    var holder = new Thread(() => { lock (gate) Thread.Sleep(300); });
    holder.Start();
    Thread.Sleep(20);

    if (Monitor.TryEnter(gate, TimeSpan.FromMilliseconds(100)))
    {
        try { Console.WriteLine("  got the lock (unexpected)"); }
        finally { Monitor.Exit(gate); }
    }
    else Console.WriteLine("  TryEnter timed out after 100ms — lock is busy, do something else instead of hanging");
    holder.Join();
}

// ---------------------------------------------------------------
static void Demo3_Interlocked()
{
    int counter = 0;
    long total = 0;
    Parallel.For(0, 100_000, _ =>
    {
        Interlocked.Increment(ref counter);
        Interlocked.Add(ref total, 5);
    });
    Console.WriteLine($"  counter={counter:N0}, total={total:N0}");

    // CompareExchange: "עדכן רק אם הערך עדיין X" — הבסיס לאלגוריתמים lock-free
    int state = 0;
    int previous = Interlocked.CompareExchange(ref state, 1, 0);   // 0 -> 1
    int again = Interlocked.CompareExchange(ref state, 2, 0);      // לא יצליח — state כבר 1
    Console.WriteLine($"  CompareExchange: previous={previous}, state={state}, second attempt saw {again}");
}

// ---------------------------------------------------------------
static async Task Demo4_SemaphoreSlim()
{
    // הגבלת מקביליות: לכל היותר 3 "הורדות" בו-זמנית מתוך 10
    using var gate = new SemaphoreSlim(3, 3);
    int running = 0, maxRunning = 0;
    var sw = Stopwatch.StartNew();

    var tasks = Enumerable.Range(1, 10).Select(async i =>
    {
        await gate.WaitAsync();                 // async — לא חוסם תהליכון בזמן ההמתנה
        try
        {
            int now = Interlocked.Increment(ref running);
            InterlockedMax(ref maxRunning, now);
            await Task.Delay(100);
            Interlocked.Decrement(ref running);
        }
        finally { gate.Release(); }             // תמיד לשחרר!
    });
    await Task.WhenAll(tasks);
    Console.WriteLine($"  10 jobs, max 3 concurrent: peak={maxRunning}, took {sw.ElapsedMilliseconds} ms (~400)");

    static void InterlockedMax(ref int target, int value)
    {
        int current;
        while ((current = target) < value && Interlocked.CompareExchange(ref target, value, current) != current) { }
    }
}

// ---------------------------------------------------------------
static void Demo5_ReaderWriterLock()
{
    // הרבה קוראים במקביל, כותב אחד בלבד
    using var rw = new ReaderWriterLockSlim();
    var config = new Dictionary<string, string> { ["mode"] = "dev" };
    int reads = 0;

    var readers = Enumerable.Range(0, 4).Select(r => new Thread(() =>
    {
        for (int i = 0; i < 1000; i++)
        {
            rw.EnterReadLock();
            try { _ = config["mode"]; Interlocked.Increment(ref reads); }
            finally { rw.ExitReadLock(); }
        }
    })).ToList();
    var writer = new Thread(() =>
    {
        for (int i = 0; i < 10; i++)
        {
            rw.EnterWriteLock();
            try { config["mode"] = i % 2 == 0 ? "prod" : "dev"; }
            finally { rw.ExitWriteLock(); }
            Thread.Sleep(1);
        }
    });
    readers.ForEach(t => t.Start()); writer.Start();
    readers.ForEach(t => t.Join()); writer.Join();
    Console.WriteLine($"  reads={reads}, final mode={config["mode"]}, no exceptions");
}

// ---------------------------------------------------------------
static void Demo6_ConcurrentCollections()
{
    // ConcurrentDictionary — AddOrUpdate / GetOrAdd אטומיים
    var wordCounts = new ConcurrentDictionary<string, int>();
    var words = "the quick brown fox jumps over the lazy dog the end".Split(' ');
    Parallel.ForEach(Enumerable.Repeat(words, 50).SelectMany(w => w),
        w => wordCounts.AddOrUpdate(w, 1, (_, old) => old + 1));
    Console.WriteLine($"  'the' appears {wordCounts["the"]} times (50 x 3 = 150)");

    // ConcurrentQueue — תור בטוח
    var queue = new ConcurrentQueue<int>();
    Parallel.For(0, 1000, i => queue.Enqueue(i));
    int dequeued = 0;
    while (queue.TryDequeue(out _)) dequeued++;
    Console.WriteLine($"  ConcurrentQueue: enqueued 1000, dequeued {dequeued}");

    // BlockingCollection — producer/consumer קלאסי (חוסם תהליכון!)
    using var bc = new BlockingCollection<string>(boundedCapacity: 5);
    var consumer = new Thread(() =>
    {
        foreach (var item in bc.GetConsumingEnumerable())   // מסתיים כש-CompleteAdding נקרא והתור ריק
            Console.WriteLine($"    consumed {item}");
    });
    consumer.Start();
    for (int i = 1; i <= 8; i++) bc.Add($"job-{i}");
    bc.CompleteAdding();
    consumer.Join();

    // אזהרה: List<T> / Dictionary<K,V> רגילים אינם בטוחים לכתיבה מקבילית!
    var unsafeList = new List<int>();
    try
    {
        Parallel.For(0, 100_000, i => unsafeList.Add(i));
        Console.WriteLine($"  List<int> under Parallel.For: count={unsafeList.Count} (expected 100000 — may be wrong or throw!)");
    }
    catch (Exception ex) { Console.WriteLine($"  List<int> under Parallel.For threw {ex.GetType().Name} — not thread-safe!"); }
}

// ---------------------------------------------------------------
static void Demo7_Deadlock()
{
    var lockA = new object();
    var lockB = new object();

    // גרסה שמתה: T1 נועל A ואז B; T2 נועל B ואז A -> כל אחד מחכה לשני לנצח.
    // כאן נשתמש ב-Monitor.TryEnter עם timeout כדי *להראות* את הבעיה בלי לתקוע את התוכנית.
    bool deadlockDetected = false;
    var t1 = new Thread(() =>
    {
        lock (lockA) { Thread.Sleep(50); if (!Monitor.TryEnter(lockB, 500)) deadlockDetected = true; else Monitor.Exit(lockB); }
    });
    var t2 = new Thread(() =>
    {
        lock (lockB) { Thread.Sleep(50); if (!Monitor.TryEnter(lockA, 500)) deadlockDetected = true; else Monitor.Exit(lockA); }
    });
    t1.Start(); t2.Start(); t1.Join(); t2.Join();
    Console.WriteLine($"  opposite lock order -> deadlock detected: {deadlockDetected}");

    // התיקון: סדר נעילה קבוע (תמיד A לפני B)
    var t3 = new Thread(() => { lock (lockA) { Thread.Sleep(50); lock (lockB) { } } });
    var t4 = new Thread(() => { lock (lockA) { Thread.Sleep(50); lock (lockB) { } } });
    var sw = Stopwatch.StartNew();
    t3.Start(); t4.Start(); t3.Join(); t4.Join();
    Console.WriteLine($"  consistent lock order -> finished in {sw.ElapsedMilliseconds} ms, no deadlock");
}

// ---------------------------------------------------------------
static void Demo8_ImmutableAndLazy()
{
    // אובייקט בלתי-משתנה (record) בטוח לשיתוף בין תהליכונים — אין מה לסנכרן
    var price = new PriceSnapshot("ILS", 3.7m, DateTime.UtcNow);
    Parallel.For(0, 4, i => { var updated = price with { Rate = price.Rate + i }; _ = updated; });
    Console.WriteLine($"  original snapshot untouched: {price.Rate}");

    // Lazy<T> — אתחול עצל ובטוח (thread-safe by default)
    Parallel.For(0, 20, i => { var cfg = AppConfig.Instance; });
    Console.WriteLine($"  AppConfig.Instance created {AppConfig.CreatedCount} time(s) (expected 1)");
}

class Account(decimal initial)
{
    private readonly Lock _lock = new();       // C# 13 / .NET 9+: System.Threading.Lock
    private decimal _balance = initial;
    private int _failed;

    public decimal Balance { get { lock (_lock) return _balance; } }
    public int Failed => _failed;

    public bool Withdraw(decimal amount)
    {
        lock (_lock)                            // רק תהליכון אחד בתוך הבלוק בכל רגע
        {
            if (_balance < amount) { _failed++; return false; }
            Thread.SpinWait(50);                // מדמה עבודה — מגדיל את חלון ה-race בלי נעילה
            _balance -= amount;
            return true;
        }
    }
}

record PriceSnapshot(string Currency, decimal Rate, DateTime At);

sealed class AppConfig
{
    public static int CreatedCount;
    private static readonly Lazy<AppConfig> _instance = new(() =>
    {
        Interlocked.Increment(ref CreatedCount);
        Thread.Sleep(20);
        return new AppConfig();
    });
    public static AppConfig Instance => _instance.Value;
    private AppConfig() { }
}
