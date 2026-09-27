using System.Diagnostics;

// ===============================================================
// Day2.Demo.Threads — תהליכונים (Threads), ThreadPool, race condition,
// CPU-bound מול IO-bound, Parallel.For ו-PLINQ.
// הרצה: dotnet run   (או dotnet run -- <שם דמו>)
// ===============================================================

Console.OutputEncoding = System.Text.Encoding.UTF8;
var demos = new (string Key, string Name, Action Run)[]
{
    ("1", "Process and Thread", Demo1_ProcessAndThread),
    ("2", "Creating threads", Demo2_CreateThreads),
    ("3", "ThreadPool", Demo3_ThreadPool),
    ("4", "Race condition", Demo4_RaceCondition),
    ("5", "Cost of threads", Demo5_CostOfThreads),
    ("6", "CPU-bound vs IO-bound", Demo6_CpuVsIo),
    ("7", "Parallel.For / ForEach", Demo7_ParallelFor),
    ("8", "PLINQ", Demo8_Plinq),
};

var selected = args.Length > 0 ? demos.Where(d => d.Key == args[0]).ToArray() : demos;
if (selected.Length == 0) { Console.WriteLine("usage: dotnet run -- <1..8>"); return; }
foreach (var (key, name, run) in selected)
{
    Console.WriteLine($"\n======== Demo {key}: {name} ========");
    run();
}

// ---------------------------------------------------------------
static void Demo1_ProcessAndThread()
{
    var proc = Process.GetCurrentProcess();
    Console.WriteLine($"Process id: {proc.Id}, name: {proc.ProcessName}");
    Console.WriteLine($"Threads in this process right now: {proc.Threads.Count}");
    Console.WriteLine($"Main thread id: {Environment.CurrentManagedThreadId}");
    Console.WriteLine($"Logical processors: {Environment.ProcessorCount}");
}

// ---------------------------------------------------------------
static void Demo2_CreateThreads()
{
    // יצירת Thread ידנית: מקבל delegate, מתחיל ב-Start, מחכים ב-Join
    var worker = new Thread(() =>
    {
        for (int i = 1; i <= 3; i++)
        {
            Console.WriteLine($"  [worker {Environment.CurrentManagedThreadId}] step {i}");
            Thread.Sleep(100);
        }
    })
    { Name = "MyWorker", IsBackground = true };

    worker.Start();
    Console.WriteLine($"  [main {Environment.CurrentManagedThreadId}] waiting for worker...");
    worker.Join();   // חוסם עד שהתהליכון מסיים
    Console.WriteLine("  [main] worker finished");

    // ParameterizedThreadStart — העברת פרמטר
    var t2 = new Thread(obj => Console.WriteLine($"  got parameter: {obj}"));
    t2.Start("hello");
    t2.Join();
}

// ---------------------------------------------------------------
static void Demo3_ThreadPool()
{
    ThreadPool.GetMinThreads(out var minWorkers, out var minIo);
    ThreadPool.GetMaxThreads(out var maxWorkers, out var maxIo);
    Console.WriteLine($"  ThreadPool min workers={minWorkers}, io={minIo}; max workers={maxWorkers}, io={maxIo}");

    using var done = new CountdownEvent(5);
    for (int i = 1; i <= 5; i++)
    {
        int n = i; // לכידת עותק! (אחרת כל העבודות יראו את אותו i)
        ThreadPool.QueueUserWorkItem(_ =>
        {
            Console.WriteLine($"  work item {n} on pool thread {Environment.CurrentManagedThreadId} (IsThreadPoolThread={Thread.CurrentThread.IsThreadPoolThread})");
            Thread.Sleep(50);
            done.Signal();
        });
    }
    done.Wait();
    Console.WriteLine("  all pool work items finished");
}

// ---------------------------------------------------------------
static void Demo4_RaceCondition()
{
    // שני תהליכונים מגדילים מונה משותף. counter++ הוא לא אטומי:
    // קריאה -> הוספה -> כתיבה. בין הקריאה לכתיבה תהליכון אחר יכול "לגנוב" עדכון.
    const int Iterations = 1_000_000;

    int unsafeCounter = 0;
    int lockedCounter = 0;
    int interlockedCounter = 0;
    var gate = new object();

    Thread[] Make(ThreadStart body) => [new Thread(body), new Thread(body)];

    void Run(Thread[] threads) { foreach (var t in threads) t.Start(); foreach (var t in threads) t.Join(); }

    Run(Make(() => { for (int i = 0; i < Iterations; i++) unsafeCounter++; }));
    Run(Make(() => { for (int i = 0; i < Iterations; i++) lock (gate) lockedCounter++; }));
    Run(Make(() => { for (int i = 0; i < Iterations; i++) Interlocked.Increment(ref interlockedCounter); }));

    Console.WriteLine($"  expected     : {2 * Iterations:N0}");
    Console.WriteLine($"  unsafe ++    : {unsafeCounter:N0}   <-- race condition (usually smaller!)");
    Console.WriteLine($"  lock         : {lockedCounter:N0}");
    Console.WriteLine($"  Interlocked  : {interlockedCounter:N0}");
}

// ---------------------------------------------------------------
static void Demo5_CostOfThreads()
{
    // כל Thread עולה: ~1MB stack + זמן יצירה + context switching.
    const int N = 200;
    var sw = Stopwatch.StartNew();
    var threads = new List<Thread>();
    for (int i = 0; i < N; i++)
    {
        var t = new Thread(() => Thread.Sleep(10));
        t.Start();
        threads.Add(t);
    }
    threads.ForEach(t => t.Join());
    Console.WriteLine($"  {N} dedicated threads: {sw.ElapsedMilliseconds} ms");

    sw.Restart();
    var tasks = Enumerable.Range(0, N).Select(_ => Task.Run(() => Thread.Sleep(10))).ToArray();
    Task.WaitAll(tasks);
    Console.WriteLine($"  {N} Task.Run on pool: {sw.ElapsedMilliseconds} ms (pool grows slowly - ~1-2 threads/sec when starved)");

    sw.Restart();
    var delays = Enumerable.Range(0, N).Select(_ => Task.Delay(10)).ToArray();
    Task.WaitAll(delays);
    Console.WriteLine($"  {N} Task.Delay (no thread at all!): {sw.ElapsedMilliseconds} ms");
}

// ---------------------------------------------------------------
static void Demo6_CpuVsIo()
{
    // CPU-bound: חישוב. הקבלה עוזרת עד מספר הליבות.
    var sw = Stopwatch.StartNew();
    long sum = 0;
    for (int i = 0; i < 4; i++) sum += CountPrimes(200_000);
    Console.WriteLine($"  CPU-bound sequential (4 x primes): {sw.ElapsedMilliseconds} ms, sum={sum}");

    sw.Restart();
    var results = new long[4];
    Parallel.For(0, 4, i => results[i] = CountPrimes(200_000));
    Console.WriteLine($"  CPU-bound parallel   (4 x primes): {sw.ElapsedMilliseconds} ms, sum={results.Sum()} ({Environment.ProcessorCount} cores)");

    // IO-bound: המתנה (רשת/דיסק). לא צריך תהליכון כדי לחכות — צריך async.
    sw.Restart();
    for (int i = 0; i < 4; i++) Thread.Sleep(100);   // מדמה 4 קריאות IO סדרתיות
    Console.WriteLine($"  IO-bound sequential (4 x 100ms): {sw.ElapsedMilliseconds} ms");

    sw.Restart();
    Task.WaitAll(Enumerable.Range(0, 4).Select(_ => Task.Delay(100)).ToArray());
    Console.WriteLine($"  IO-bound concurrent (4 x 100ms): {sw.ElapsedMilliseconds} ms");

    static long CountPrimes(int max)
    {
        long count = 0;
        for (int n = 2; n < max; n++)
        {
            bool prime = true;
            for (int d = 2; d * d <= n; d++) if (n % d == 0) { prime = false; break; }
            if (prime) count++;
        }
        return count;
    }
}

// ---------------------------------------------------------------
static void Demo7_ParallelFor()
{
    var data = Enumerable.Range(1, 20).ToArray();
    var squares = new int[data.Length];

    Parallel.For(0, data.Length, i => squares[i] = data[i] * data[i]);
    Console.WriteLine("  Parallel.For squares: " + string.Join(", ", squares));

    var files = new[] { "a.jpg", "b.jpg", "c.jpg", "d.jpg", "e.jpg", "f.jpg" };
    Parallel.ForEach(files, new ParallelOptions { MaxDegreeOfParallelism = 3 }, file =>
    {
        Thread.Sleep(50);
        Console.WriteLine($"  processed {file} on thread {Environment.CurrentManagedThreadId}");
    });

    // אזהרה: הסדר לא מובטח, ואסור לכתוב לאוסף רגיל (List) מתוך הגוף בלי נעילה!
}

// ---------------------------------------------------------------
static void Demo8_Plinq()
{
    // PLINQ משתלם רק כשהעבודה על כל פריט "יקרה" — לעבודה זולה ה-overhead של החלוקה גדול מהרווח
    var numbers = Enumerable.Range(1, 1_500_000);

    var sw = Stopwatch.StartNew();
    var seq = numbers.Where(IsPrime).Count();
    Console.WriteLine($"  LINQ  : {seq} primes in {sw.ElapsedMilliseconds} ms");

    sw.Restart();
    var par = numbers.AsParallel().Where(IsPrime).Count();
    Console.WriteLine($"  PLINQ : {par} primes in {sw.ElapsedMilliseconds} ms");

    // AsOrdered שומר על הסדר המקורי (במחיר ביצועים)
    var firstFive = numbers.AsParallel().AsOrdered().Where(n => n % 1000 == 0).Take(5);
    Console.WriteLine("  PLINQ ordered: " + string.Join(", ", firstFive));

    static bool IsPrime(int n)
    {
        if (n < 2) return false;
        for (int d = 2; d * d <= n; d++) if (n % d == 0) return false;
        return true;
    }
}
