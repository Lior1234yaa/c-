using Day2.Lab4;

// Lab 4 — Starter
Console.OutputEncoding = System.Text.Encoding.UTF8;

// TODO (שלב 5): --interval <sec> (ברירת מחדל 5), --once
// TODO (שלב 5): CancellationTokenSource + Console.CancelKeyPress

using var http = new HttpClient { BaseAddress = new Uri("http://localhost:5080"), Timeout = TimeSpan.FromSeconds(10) };
await using var log = new ChannelLogger();
var dashboard = new Dashboard(http, log);

try
{
    await dashboard.RefreshAsync(CancellationToken.None);
    // TODO (שלב 5): PeriodicTimer + לולאה עד ביטול
}
catch (HttpRequestException ex)
{
    Console.WriteLine($"Day2.LocalApi is not reachable: {ex.Message}. Start it: cd Demos/Day2.LocalApi && dotnet run");
}
Console.WriteLine("bye");
