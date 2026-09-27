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

    public async Task<List<Order>> FetchAsync(CancellationToken ct)
    {
        log.Log("fetching pending orders...");
        return await http.GetFromJsonAsync<List<Order>>("/api/orders?status=Pending", Json.Options, ct) ?? [];
    }
}

public class HealthSource(HttpClient http, ChannelLogger log) : ISource<Health>
{
    public string Name => "health";

    public async Task<Health> FetchAsync(CancellationToken ct)
    {
        log.Log("fetching health...");
        return await http.GetFromJsonAsync<Health>("/api/health", Json.Options, ct)
               ?? throw new InvalidOperationException("empty health");
    }
}
