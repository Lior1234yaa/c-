using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Day2.Lab4.Sources;

public static class Json
{
    public static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web) { Converters = { new JsonStringEnumConverter() } };
}

public class StatsSource(HttpClient http, ChannelLogger log) : ISource<Stats>
{
    public string Name => "stats";

    public async Task<Stats> FetchAsync(CancellationToken ct)
    {
        log.Log("fetching stats...");
        return await http.GetFromJsonAsync<Stats>("/api/stats", Json.Options, ct)
               ?? throw new InvalidOperationException("empty stats");
    }
}

public class PendingOrdersSource(HttpClient http, ChannelLogger log) : ISource<List<Order>>
{
    public string Name => "pending orders";

    public Task<List<Order>> FetchAsync(CancellationToken ct)
    {
        log.Log("fetching pending orders...");
        _ = http;   // TODO (שלב 1): GET /api/orders?status=Pending  (הסירו את השורה הזו כשמממשים)
        throw new NotImplementedException();
    }
}

public class HealthSource(HttpClient http, ChannelLogger log) : ISource<Health>
{
    public string Name => "health";

    public Task<Health> FetchAsync(CancellationToken ct)
    {
        log.Log("fetching health...");
        _ = http;   // TODO (שלב 1): GET /api/health
        throw new NotImplementedException();
    }
}
