using System.Diagnostics;
using Day2.Lab4.Sources;

namespace Day2.Lab4;

public class Dashboard(HttpClient http, ChannelLogger log)
{
    private readonly StatsSource _stats = new(http, log);
    // TODO (שלב 1): PendingOrdersSource, HealthSource, WeatherSource

    public async Task RefreshAsync(CancellationToken ct)
    {
        var sw = Stopwatch.StartNew();

        // TODO (שלב 2): FetchSafeAsync<T> שעוטף כל מקור ב-try/catch ומחזיר SourceResult<T>
        // TODO (שלב 3): להפעיל את כל המקורות בלי await, ואז Task.WhenAll
        var stats = await _stats.FetchAsync(ct);

        Console.WriteLine($"\n===== Shop dashboard  {DateTime.Now:HH:mm:ss}  (refresh took {sw.ElapsedMilliseconds} ms) =====");
        Console.WriteLine($"  stats          : {stats.Products} products, {stats.Orders} orders ({stats.Pending} pending), revenue {stats.Revenue:N0}");
        // TODO (שלב 3): שורה לכל מקור: שם, OK/FAILED, משך, תקציר
    }
}
