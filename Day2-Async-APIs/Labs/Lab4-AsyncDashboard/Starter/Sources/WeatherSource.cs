using System.Net.Http.Json;

namespace Day2.Lab4.Sources;

/// <summary>מזג אוויר בתל אביב מ-Open-Meteo (API ציבורי, בלי מפתח).</summary>
public class WeatherSource(HttpClient http, ChannelLogger log) : ISource<WeatherCurrent>
{
    public const string Url = "https://api.open-meteo.com/v1/forecast?latitude=32.08&longitude=34.78&current=temperature_2m,wind_speed_10m";

    public string Name => "weather (Tel Aviv)";

    public Task<WeatherCurrent> FetchAsync(CancellationToken ct)
    {
        log.Log("fetching weather...");
        _ = http;   // TODO (שלב 1): GET Url עם timeout של 4 שניות (CancellationTokenSource מקושר + CancelAfter); להחזיר response.Current
        throw new NotImplementedException();
    }
}
