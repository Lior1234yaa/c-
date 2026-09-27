using System.Diagnostics;
using System.Threading.Channels;

// ===============================================================
// Day2.Demo.ProducerConsumer — Channel<T>: יצרן/צרכן אסינכרוני,
// bounded channel (back-pressure), כמה צרכנים, ו-logger מבוסס Channel.
// ===============================================================

Console.OutputEncoding = System.Text.Encoding.UTF8;

Console.WriteLine("======== 1. Unbounded channel: one producer, one consumer ========");
await Demo1_Basic();
Console.WriteLine("\n======== 2. Bounded channel: back-pressure ========");
await Demo2_Bounded();
Console.WriteLine("\n======== 3. One producer, three consumers (work distribution) ========");
await Demo3_MultipleConsumers();
Console.WriteLine("\n======== 4. Channel-based logger ========");
await Demo4_ChannelLogger();
Console.WriteLine("\ndone.");

// ---------------------------------------------------------------
static async Task Demo1_Basic()
{
    var channel = Channel.CreateUnbounded<string>();

    var producer = Task.Run(async () =>
    {
        for (int i = 1; i <= 5; i++)
        {
            await channel.Writer.WriteAsync($"order-{i}");
            Console.WriteLine($"  produced order-{i}");
            await Task.Delay(30);
        }
        channel.Writer.Complete();          // "לא יגיעו עוד פריטים"
    });

    var consumer = Task.Run(async () =>
    {
        await foreach (var item in channel.Reader.ReadAllAsync())   // מסתיים כשה-Writer הושלם והתור ריק
        {
            await Task.Delay(60);                                    // הצרכן איטי יותר מהיצרן
            Console.WriteLine($"      consumed {item}");
        }
    });

    await Task.WhenAll(producer, consumer);
}

// ---------------------------------------------------------------
static async Task Demo2_Bounded()
{
    // קיבולת 2: היצרן ייחסם (אסינכרונית!) כשהתור מלא — כך הזיכרון לא מתפוצץ
    var channel = Channel.CreateBounded<int>(new BoundedChannelOptions(2)
    {
        FullMode = BoundedChannelFullMode.Wait,
        SingleReader = true,
        SingleWriter = true,
    });
    var sw = Stopwatch.StartNew();

    var producer = Task.Run(async () =>
    {
        for (int i = 1; i <= 6; i++)
        {
            await channel.Writer.WriteAsync(i);
            Console.WriteLine($"  [{sw.ElapsedMilliseconds,4} ms] produced {i}");
        }
        channel.Writer.Complete();
    });

    var consumer = Task.Run(async () =>
    {
        await foreach (var item in channel.Reader.ReadAllAsync())
        {
            await Task.Delay(100);
            Console.WriteLine($"  [{sw.ElapsedMilliseconds,4} ms]     consumed {item}");
        }
    });

    await Task.WhenAll(producer, consumer);
    Console.WriteLine("  notice: producer waits once 2 items are queued (back-pressure)");
}

// ---------------------------------------------------------------
static async Task Demo3_MultipleConsumers()
{
    var channel = Channel.CreateUnbounded<int>();
    int processed = 0;

    var consumers = Enumerable.Range(1, 3).Select(id => Task.Run(async () =>
    {
        await foreach (var job in channel.Reader.ReadAllAsync())
        {
            await Task.Delay(Random.Shared.Next(20, 60));
            Interlocked.Increment(ref processed);
            Console.WriteLine($"  worker {id} finished job {job}");
        }
    })).ToArray();

    for (int i = 1; i <= 9; i++) channel.Writer.TryWrite(i);
    channel.Writer.Complete();
    await Task.WhenAll(consumers);
    Console.WriteLine($"  all {processed} jobs processed by 3 workers");
}

// ---------------------------------------------------------------
static async Task Demo4_ChannelLogger()
{
    // ה-logger כותב ל-Channel; תהליכון רקע יחיד מדפיס. הקוראים לא נחסמים ואין ערבוב שורות.
    await using var logger = new ChannelLogger();

    var workers = Enumerable.Range(1, 4).Select(id => Task.Run(async () =>
    {
        for (int i = 1; i <= 3; i++)
        {
            logger.Log($"worker {id} step {i}");
            await Task.Delay(10);
        }
    }));
    await Task.WhenAll(workers);
    logger.Log("all workers done");
    // DisposeAsync ממתין שכל ההודעות יודפסו
}

sealed class ChannelLogger : IAsyncDisposable
{
    private readonly Channel<string> _channel = Channel.CreateUnbounded<string>(new UnboundedChannelOptions { SingleReader = true });
    private readonly Task _pump;

    public ChannelLogger() => _pump = Task.Run(PumpAsync);

    public void Log(string message) =>
        _channel.Writer.TryWrite($"{DateTime.Now:HH:mm:ss.fff} [T{Environment.CurrentManagedThreadId,2}] {message}");

    private async Task PumpAsync()
    {
        await foreach (var line in _channel.Reader.ReadAllAsync())
            Console.WriteLine("  " + line);
    }

    public async ValueTask DisposeAsync()
    {
        _channel.Writer.Complete();
        await _pump;
    }
}
