using System.Net.Http.Json;

namespace Day2.Lab4.Sources;

/// <summary>מזג אוויר בתל אביב מ-Open-Meteo (API ציבורי, בלי מפתח). timeout קצר משלו — כדי שרשת איטית לא תעכב את הלוח.</summary>
public class WeatherSource(HttpClient http, ChannelLogger log, TimeSpan? timeout = null) : ISource<WeatherCurrent>
{
    public const string Url = "https://api.open-meteo.com/v1/forecast?latitude=32.08&longitude=34.78&current=temperature_2m,wind_speed_10m";

    public string Name => "weather (Tel Aviv)";

    public async Task<WeatherCurrent> FetchAsync(CancellationToken ct)
    {
        log.Log("fetching weather...");
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);   // מגיב גם ל-Ctrl+C וגם ל-timeout
        cts.CancelAfter(timeout ?? TimeSpan.FromSeconds(4));
        var response = await http.GetFromJsonAsync<WeatherResponse>(Url, Json.Options, cts.Token)
                       ?? throw new InvalidOperationException("empty weather response");
        return response.Current;
    }
}
