using System.Diagnostics;
using System.Text.Json;
using Day2.Lab4.Sources;

namespace Day2.Lab4;

public class Dashboard(HttpClient http, ChannelLogger log, int maxConcurrency = 4)
{
    private readonly StatsSource _stats = new(http, log);
    private readonly PendingOrdersSource _pending = new(http, log);
    private readonly HealthSource _health = new(http, log);
    private readonly WeatherSource _weather = new(http, log);
    private readonly SemaphoreSlim _gate = new(maxConcurrency, maxConcurrency);   // שלב 7: הגבלת מקביליות

    // בונוס: הערך האחרון שהצליח לכל מקור — מוצג כ-stale כשהמקור נופל
    private readonly Dictionary<string, (string Summary, DateTime At)> _lastGood = new();

    public async Task RefreshAsync(CancellationToken ct)
    {
        var sw = Stopwatch.StartNew();

        // שלב 3: מפעילים את כולם *בלי await*, ואז מחכים לכולם יחד
        var statsTask = FetchSafeAsync(_stats, ct);
        var pendingTask = FetchSafeAsync(_pending, ct);
        var healthTask = FetchSafeAsync(_health, ct);
        var weatherTask = FetchSafeAsync(_weather, ct);
        await Task.WhenAll(statsTask, pendingTask, healthTask, weatherTask);

        // אחרי WhenAll ה-Tasks הושלמו — .Result בטוח כאן (לא חוסם)
        var stats = statsTask.Result; var pending = pendingTask.Result; var health = healthTask.Result; var weather = weatherTask.Result;

        Console.WriteLine($"\n===== Shop dashboard  {DateTime.Now:HH:mm:ss}  (refresh took {sw.ElapsedMilliseconds} ms) =====");
        Render(_stats.Name, stats, s => $"{s.Products} products, {s.Orders} orders ({s.Pending} pending, {s.Shipped} shipped), revenue {s.Revenue:N0}");
        Render(_pending.Name, pending, list => list.Count == 0 ? "none" : string.Join(", ", list.Select(o => $"#{o.Id} {o.Customer} {o.Total:N0}")));
        Render(_health.Name, health, h => $"{h.Status} (server time {h.Time:HH:mm:ss} UTC)");
        Render(_weather.Name, weather, w => $"{w.Temperature:F1}°C, wind {w.WindSpeed:F0} km/h (at {w.Time:HH:mm})", offlineText: "offline");

        int failed = new[] { stats.IsOk, pending.IsOk, health.IsOk, weather.IsOk }.Count(ok => !ok);
        Console.WriteLine(failed == 0 ? "  all sources OK" : $"  {failed} source(s) unavailable — dashboard keeps running");
    }

    // שלב 2: עוטף מקור כך שכישלון הופך לערך, לא לחריגה. ביטול של המשתמש כן עולה למעלה.
    private async Task<SourceResult<T>> FetchSafeAsync<T>(ISource<T> source, CancellationToken ct)
    {
        var sw = Stopwatch.StartNew();
        await _gate.WaitAsync(ct);
        try
        {
            var value = await source.FetchAsync(ct);
            log.Log($"{source.Name}: ok in {sw.ElapsedMilliseconds} ms");
            return SourceResult<T>.Ok(value, sw.Elapsed);
        }
        catch (Exception ex) when (!ct.IsCancellationRequested && ex is HttpRequestException or OperationCanceledException or JsonException or InvalidOperationException)
        {
            var reason = ex switch
            {
                OperationCanceledException => "timeout",
                HttpRequestException h when h.StatusCode is not null => $"HTTP {(int)h.StatusCode}",
                HttpRequestException => "connection failed",
                JsonException => "bad JSON",
                _ => ex.Message,
            };
            log.Log($"{source.Name}: FAILED ({reason}) after {sw.ElapsedMilliseconds} ms");
            return SourceResult<T>.Fail(reason, sw.Elapsed);
        }
        finally { _gate.Release(); }
    }

    private void Render<T>(string name, SourceResult<T> result, Func<T, string> summarize, string offlineText = "unavailable")
    {
        string status = result.IsOk ? "OK    " : "FAILED";
        string text;
        if (result.IsOk)
        {
            text = summarize(result.Value!);
            _lastGood[name] = (text, DateTime.Now);
        }
        else
        {
            text = _lastGood.TryGetValue(name, out var last)
                ? $"{offlineText} ({result.Error}) — stale from {last.At:HH:mm:ss}: {last.Summary}"
                : $"{offlineText} ({result.Error})";
        }
        Console.WriteLine($"  {name,-20} {status} {result.Elapsed.TotalMilliseconds,6:F0} ms  {text}");
    }
}
