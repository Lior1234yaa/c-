using System.Collections.Concurrent;
using System.Diagnostics;
using Day2.Lab1;

// Lab 1 — Solution: סדרתי מול Task.WhenAll מול Parallel.ForEach, עם ביטול (Ctrl+C) והתקדמות
// הרצה: dotnet run [-- <count> [seq|whenall|parallel|all]]
Console.OutputEncoding = System.Text.Encoding.UTF8;

int count = args.Length > 0 && int.TryParse(args[0], out var c) ? c : 40;
string mode = args.Length > 1 ? args[1].ToLowerInvariant() : "all";
var orders = OrderProcessor.GenerateOrders(count);
Console.WriteLine($"Processing {orders.Count} orders on {Environment.ProcessorCount} cores (mode: {mode}). Press Ctrl+C to cancel.\n");

// שלב 4: ביטול ב-Ctrl+C. e.Cancel = true מונע מהתהליך למות מיד — אנחנו נסיים יפה.
using var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) => { e.Cancel = true; cts.Cancel(); Console.WriteLine("\n[Ctrl+C] cancelling..."); };

// שלב 5: התקדמות. Progress<T> מריץ את ה-callback על ה-context שבו נוצר (בקונסול: pool thread).
var progress = new Progress<int>(pct => Console.Write($"\r  progress: [{new string('#', pct / 5),-20}] {pct,3}%"));

var timings = new List<(string Name, long Ms, int Results)>();

async Task Measure(string name, Func<Task<List<OrderResult>>> run)
{
    var sw = Stopwatch.StartNew();
    try
    {
        var results = await run();
        Console.WriteLine($"\r  {name,-16}: {sw.ElapsedMilliseconds,6} ms, {results.Count} results, checksum-sum={results.Sum(r => r.Checksum)}");
        timings.Add((name, sw.ElapsedMilliseconds, results.Count));
    }
    catch (OperationCanceledException)
    {
        Console.WriteLine($"\r  {name,-16}: cancelled after {sw.ElapsedMilliseconds} ms ({Runner.Completed} orders completed)");
    }
}

if (mode is "seq" or "all") await Measure("Sequential", () => Task.FromResult(Runner.RunSequential(orders, progress, cts.Token)));
if (!cts.IsCancellationRequested && mode is "whenall" or "all") await Measure("Task.WhenAll", () => Runner.RunWhenAllAsync(orders, progress, cts.Token));
if (!cts.IsCancellationRequested && mode is "parallel" or "all") await Measure("Parallel.ForEach", () => Runner.RunParallelForEachAsync(orders, progress, cts.Token));
if (!cts.IsCancellationRequested && mode is "foreachasync" or "all") await Measure("ForEachAsync", () => Runner.RunForEachAsyncAsync(orders, progress, cts.Token));

// שלב 6: טבלת סיכום
if (timings.Count > 1)
{
    var baseline = timings[0].Ms;
    Console.WriteLine("\n  Summary");
    Console.WriteLine("  " + new string('-', 44));
    foreach (var (name, ms, _) in timings)
        Console.WriteLine($"  {name,-18} {ms,7} ms   x{(double)baseline / Math.Max(1, ms),5:F2}");
}

static class Runner
{
    private static int _completed;
    public static int Completed => _completed;

    private static void ReportOne(int total, IProgress<int> progress)
    {
        // כמה תהליכונים מדווחים בו-זמנית -> Interlocked
        int done = Interlocked.Increment(ref _completed);
        progress.Report(done * 100 / total);
    }

    public static List<OrderResult> RunSequential(List<Order> orders, IProgress<int> progress, CancellationToken ct)
    {
        _completed = 0;
        var results = new List<OrderResult>();
        foreach (var order in orders)
        {
            ct.ThrowIfCancellationRequested();
            var sw = Stopwatch.StartNew();
            var checksum = OrderProcessor.ProcessImage(order);
            OrderProcessor.SendConfirmationAsync(order, ct).GetAwaiter().GetResult();   // קו בסיס בלבד
            results.Add(new OrderResult(order.Id, checksum, sw.Elapsed));
            ReportOne(orders.Count, progress);
        }
        return results;
    }

    // שלב 2: כל ההזמנות יוצאות לדרך יחד. ProcessImage הוא CPU-bound ולכן עטוף ב-Task.Run —
    // בלי זה, כל ה"Tasks" היו מבצעים את החישוב סדרתית על התהליכון הקורא לפני ה-await הראשון.
    public static async Task<List<OrderResult>> RunWhenAllAsync(List<Order> orders, IProgress<int> progress, CancellationToken ct)
    {
        _completed = 0;
        var tasks = orders.Select(async order =>
        {
            var sw = Stopwatch.StartNew();
            var checksum = await Task.Run(() => { ct.ThrowIfCancellationRequested(); return OrderProcessor.ProcessImage(order); }, ct);
            await OrderProcessor.SendConfirmationAsync(order, ct);
            ReportOne(orders.Count, progress);
            return new OrderResult(order.Id, checksum, sw.Elapsed);
        });
        var results = await Task.WhenAll(tasks);
        return results.ToList();
    }

    // שלב 3: Parallel.ForEach לחלק ה-CPU (מוגבל למספר הליבות), ואז WhenAll לחלק ה-IO
    public static async Task<List<OrderResult>> RunParallelForEachAsync(List<Order> orders, IProgress<int> progress, CancellationToken ct)
    {
        _completed = 0;
        var checksums = new ConcurrentDictionary<int, (long Checksum, Stopwatch Sw)>();   // לא List<T>!
        var options = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount, CancellationToken = ct };

        Parallel.ForEach(orders, options, order =>
        {
            var sw = Stopwatch.StartNew();
            checksums[order.Id] = (OrderProcessor.ProcessImage(order), sw);
        });

        var sends = orders.Select(async order =>
        {
            await OrderProcessor.SendConfirmationAsync(order, ct);
            var (checksum, sw) = checksums[order.Id];
            ReportOne(orders.Count, progress);
            return new OrderResult(order.Id, checksum, sw.Elapsed);
        });
        return (await Task.WhenAll(sends)).ToList();
    }

    // בונוס: Parallel.ForEachAsync — גוף אסינכרוני עם הגבלת מקביליות מובנית
    public static async Task<List<OrderResult>> RunForEachAsyncAsync(List<Order> orders, IProgress<int> progress, CancellationToken ct)
    {
        _completed = 0;
        var bag = new ConcurrentBag<OrderResult>();
        await Parallel.ForEachAsync(orders, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount * 2, CancellationToken = ct },
            async (order, token) =>
            {
                var sw = Stopwatch.StartNew();
                var checksum = OrderProcessor.ProcessImage(order);
                await OrderProcessor.SendConfirmationAsync(order, token);
                bag.Add(new OrderResult(order.Id, checksum, sw.Elapsed));
                ReportOne(orders.Count, progress);
            });
        return bag.ToList();
    }
}
