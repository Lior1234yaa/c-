using Day2.Lab4;

// Lab 4 — Solution: dashboard שמרענן כל N שניות, מושך 4 מקורות במקביל, ושורד כשלים חלקיים.
// הרצה: dotnet run [-- --interval 5] [--once] [--quiet] [--max 2]
Console.OutputEncoding = System.Text.Encoding.UTF8;

int interval = ArgInt("--interval", 5);
int maxConcurrency = ArgInt("--max", 4);
bool once = args.Contains("--once");
bool quiet = args.Contains("--quiet");

// שלב 5: Ctrl+C -> ביטול שיתופי (e.Cancel = true כדי שהתהליך לא ימות מיד ונספיק לנקות)
using var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) => { e.Cancel = true; cts.Cancel(); };

using var http = new HttpClient { BaseAddress = new Uri("http://localhost:5080"), Timeout = TimeSpan.FromSeconds(10) };
await using var log = new ChannelLogger(enabled: !quiet);
var dashboard = new Dashboard(http, log, maxConcurrency);

Console.WriteLine($"Async dashboard — refresh every {interval}s, max {maxConcurrency} concurrent fetches. Ctrl+C to stop. (logs on stderr)");

try
{
    await dashboard.RefreshAsync(cts.Token);                       // רענון ראשון מיד
    if (!once)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(interval));
        while (await timer.WaitForNextTickAsync(cts.Token))        // מחזיר false / זורק כשמבוטל
            await dashboard.RefreshAsync(cts.Token);
    }
}
catch (OperationCanceledException)
{
    log.Log("cancelled by user");
}

Console.WriteLine("bye");

int ArgInt(string name, int fallback)
{
    int i = Array.IndexOf(args, name);
    return i >= 0 && i + 1 < args.Length && int.TryParse(args[i + 1], out var v) && v > 0 ? v : fallback;
}
