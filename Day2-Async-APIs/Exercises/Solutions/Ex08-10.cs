using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Channels;

// תרגיל 8 — SemaphoreSlim
static class Ex08
{
    public static async Task RunAsync()
    {
        using var gate = new SemaphoreSlim(3, 3);
        int running = 0, peak = 0;
        var sw = Stopwatch.StartNew();

        var tasks = Enumerable.Range(1, 10).Select(async i =>
        {
            await gate.WaitAsync();
            try
            {
                int now = Interlocked.Increment(ref running);
                int seen; do { seen = peak; } while (now > seen && Interlocked.CompareExchange(ref peak, now, seen) != seen);
                await Task.Delay(200);
                Interlocked.Decrement(ref running);
            }
            finally { gate.Release(); }
        });
        await Task.WhenAll(tasks);
        Console.WriteLine($"  10 requests, max 3 at a time: {sw.ElapsedMilliseconds} ms (expected ~800), peak concurrency = {peak}");
    }
}

// תרגיל 9 — ConcurrentDictionary
static class Ex09
{
    public static Task RunAsync()
    {
        var text = string.Join(' ', Enumerable.Repeat("the quick brown fox jumps over the lazy dog and the fox runs away from the dog", 500));
        var words = text.Split(' ');

        var counts = new ConcurrentDictionary<string, int>();
        Parallel.ForEach(words, w => counts.AddOrUpdate(w, 1, (_, c) => c + 1));
        Console.WriteLine("  top 3: " + string.Join(", ", counts.OrderByDescending(kv => kv.Value).Take(3).Select(kv => $"{kv.Key}={kv.Value}")));

        // בונוס: Dictionary רגיל — לא בטוח. לרוב ספירה שגויה או חריגה.
        var plain = new Dictionary<string, int>();
        try
        {
            Parallel.ForEach(words, w => { plain.TryGetValue(w, out var c); plain[w] = c + 1; });
            Console.WriteLine($"  plain Dictionary: 'the' = {plain.GetValueOrDefault("the")} (correct = {counts["the"]}) — wrong or lucky");
        }
        catch (Exception ex) { Console.WriteLine($"  plain Dictionary threw {ex.GetType().Name} — not thread-safe!"); }
        return Task.CompletedTask;
    }
}

// תרגיל 10 — Channel producer/consumer
static class Ex10
{
    public static async Task RunAsync()
    {
        var channel = Channel.CreateBounded<int>(5);

        var producer = Task.Run(async () =>
        {
            for (int i = 1; i <= 20; i++) { await channel.Writer.WriteAsync(i); await Task.Delay(10); }
            channel.Writer.Complete();
        });

        var consumers = Enumerable.Range(1, 2).Select(id => Task.Run(async () =>
        {
            await foreach (var n in channel.Reader.ReadAllAsync())
            {
                await Task.Delay(40);
                Console.WriteLine($"  worker {id} got {n}");
            }
        }));

        await Task.WhenAll(consumers.Append(producer));
        Console.WriteLine("  all consumed, program ends because the channel was completed");
    }
}
