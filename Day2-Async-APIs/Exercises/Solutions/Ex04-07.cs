using System.Diagnostics;
using System.Runtime.CompilerServices;

// תרגיל 4 — Task.Delay + WhenAll
static class Ex04
{
    static async Task<string> FetchAsync(string name, int ms) { await Task.Delay(ms); return $"{name} ready"; }

    public static async Task RunAsync()
    {
        var sw = Stopwatch.StartNew();
        Console.WriteLine($"  {await FetchAsync("a", 300)}, {await FetchAsync("b", 200)}, {await FetchAsync("c", 100)}");
        Console.WriteLine($"  sequential: {sw.ElapsedMilliseconds} ms");

        sw.Restart();
        var results = await Task.WhenAll(FetchAsync("a", 300), FetchAsync("b", 200), FetchAsync("c", 100));
        Console.WriteLine($"  {string.Join(", ", results)}");
        Console.WriteLine($"  WhenAll   : {sw.ElapsedMilliseconds} ms");
    }
}

// תרגיל 5 — WhenAny + timeout
static class Ex05
{
    static async Task<string> DownloadAsync(string name, int ms) { await Task.Delay(ms); return name; }

    public static async Task RunAsync()
    {
        var slow = DownloadAsync("slow (500ms)", 500);
        var fast = DownloadAsync("fast (150ms)", 150);
        var winner = await Task.WhenAny(slow, fast);
        Console.WriteLine($"  winner: {await winner}");

        try
        {
            await DownloadAsync("x", 500).WaitAsync(TimeSpan.FromMilliseconds(100));
        }
        catch (TimeoutException) { Console.WriteLine("  timed out after 100 ms (TimeoutException)"); }
    }
}

// תרגיל 6 — ביטול והתקדמות
static class Ex06
{
    static async Task CountdownAsync(int from, IProgress<int> progress, CancellationToken ct)
    {
        for (int i = from; i >= 0; i--)
        {
            ct.ThrowIfCancellationRequested();
            progress.Report(i);
            await Task.Delay(100, ct);
        }
    }

    public static async Task RunAsync()
    {
        int last = -1;
        var progress = new Progress<int>(i => { last = i; Console.WriteLine($"  {i}..."); });
        using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(450));
        try
        {
            await CountdownAsync(10, progress, cts.Token);
            Console.WriteLine("  liftoff!");
        }
        catch (OperationCanceledException) { Console.WriteLine($"  cancelled at {last}"); }
    }
}

// תרגיל 7 — IAsyncEnumerable
static class Ex07
{
    static async IAsyncEnumerable<int> GenerateAsync(int count, [EnumeratorCancellation] CancellationToken ct = default)
    {
        for (int i = 1; i <= count; i++)
        {
            await Task.Delay(50, ct);
            yield return i;
        }
    }

    public static async Task RunAsync()
    {
        int sum = 0;
        await foreach (var n in GenerateAsync(100))
        {
            if (n % 2 != 0) continue;
            sum += n;
            Console.WriteLine($"  got {n}, sum={sum}");
            if (sum > 20) { Console.WriteLine("  sum > 20 — stopping early (the generator is disposed)"); break; }
        }
    }
}
