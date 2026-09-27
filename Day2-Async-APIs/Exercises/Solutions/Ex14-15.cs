using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

// תרגיל 14 — JSON ידני
static class Ex14
{
    record Current(
        [property: JsonPropertyName("temperature_2m")] double Temperature,
        [property: JsonPropertyName("wind_speed_10m")] double WindSpeed,
        DateTime Time);
    record Forecast(double Latitude, double Longitude, Current Current);

    public static Task RunAsync()
    {
        const string json = """
            {"latitude":32.08,"longitude":34.78,"current":{"time":"2026-06-01T12:00","temperature_2m":27.4,"wind_speed_10m":12.1}}
            """;
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web) { WriteIndented = true };

        var forecast = JsonSerializer.Deserialize<Forecast>(json, options)!;
        Console.WriteLine($"  DTO      : {forecast.Current.Temperature}°C, wind {forecast.Current.WindSpeed} km/h at {forecast.Current.Time:HH:mm}");

        var node = JsonNode.Parse(json)!;
        Console.WriteLine($"  JsonNode : {(double)node["current"]!["temperature_2m"]!}°C, wind {(double)node["current"]!["wind_speed_10m"]!} km/h");

        Console.WriteLine("  re-serialized (camelCase, indented):");
        foreach (var line in JsonSerializer.Serialize(forecast, options).Split('\n')) Console.WriteLine("    " + line);
        return Task.CompletedTask;
    }
}

// תרגיל 15 — sync-over-async והרעבת ThreadPool
static class Ex15
{
    public static async Task RunAsync()
    {
        Console.WriteLine($"  cores={Environment.ProcessorCount}, pool threads before: {ThreadPool.ThreadCount}");

        var sw = Stopwatch.StartNew();
        var blocking = Enumerable.Range(0, 20).Select(_ => Task.Run(() => Task.Delay(200).Wait()));   // רע: חוסם תהליכון Pool
        await Task.WhenAll(blocking);
        Console.WriteLine($"  20 x .Wait() inside Task.Run: {sw.ElapsedMilliseconds} ms, pool threads now: {ThreadPool.ThreadCount}");

        sw.Restart();
        var proper = Enumerable.Range(0, 20).Select(_ => Task.Delay(200));                              // טוב: אין תהליכון תפוס
        await Task.WhenAll(proper);
        Console.WriteLine($"  20 x await Task.Delay      : {sw.ElapsedMilliseconds} ms, pool threads now: {ThreadPool.ThreadCount}");
        Console.WriteLine("  blocking version needs a thread per task; the pool grows ~1 thread/sec beyond the core count -> slow.");
    }
}
