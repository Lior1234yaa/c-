using System.Collections.Concurrent;
using System.Globalization;

namespace Day4.Exercises.Solutions;

/// <summary>
/// תרגיל 10: (1) HttpClient חדש ב-using בכל קריאה → מיצוי sockets; מוזרק במקום.
/// (2) Dictionary לא thread-safe → ConcurrentDictionary.
/// (3) catch { return 0; } מסתיר כשל; מחיר 0 הוא נתון שגוי מסוכן → לוג + זריקה.
/// </summary>
public static class Ex10_Resources
{
    private sealed class FakePriceHandler : HttpMessageHandler
    {
        public int Calls;
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
        {
            Interlocked.Increment(ref Calls);
            var sku = request.RequestUri!.Segments[^1];
            return Task.FromResult(new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            { Content = new StringContent(sku == "BAD" ? "n/a" : "19.90") });
        }
    }

    public sealed class PriceCache(HttpClient http)
    {
        private readonly ConcurrentDictionary<string, decimal> _cache = new();

        public async Task<decimal> GetAsync(string sku, CancellationToken ct = default)
        {
            if (_cache.TryGetValue(sku, out var cached)) return cached;

            var body = await http.GetStringAsync($"https://api.example.invalid/prices/{sku}", ct);
            if (!decimal.TryParse(body, NumberStyles.Number, CultureInfo.InvariantCulture, out var price))
                throw new InvalidDataException($"Price service returned '{body}' for {sku}");

            return _cache.GetOrAdd(sku, price);
        }
    }

    public static async Task RunAsync()
    {
        var handler = new FakePriceHandler();
        var cache = new PriceCache(new HttpClient(handler));

        // 50 קריאות מקבילות לאותו SKU — בטוח עם ConcurrentDictionary
        var results = await Task.WhenAll(Enumerable.Range(0, 50).Select(_ => cache.GetAsync("A-1")));
        Console.WriteLine($"50 parallel reads → all {results.Distinct().Single()}; HTTP calls made: {handler.Calls} (cached after first wave)");

        try { await cache.GetAsync("BAD"); }
        catch (InvalidDataException ex) { Console.WriteLine($"BAD sku → exception instead of silent 0: {ex.Message}"); }
    }
}
