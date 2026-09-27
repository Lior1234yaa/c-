using System.Net.Http;
namespace Day3.Demo.AsyncUi.Services;

/// <summary>מימוש מזויף שמדמה רשת איטית: 10 "בקשות" של 300ms, מדווח התקדמות, מכבד ביטול, ולעיתים נכשל.</summary>
public class FakeWeatherService : IWeatherService
{
    private static readonly string[] Cities = ["Tel Aviv", "Haifa", "Jerusalem", "Eilat", "Beer Sheva", "Nazareth", "Tiberias", "Ashdod", "Netanya", "Herzliya"];
    private readonly Random _rng = new();

    public bool FailRandomly { get; set; }

    public async Task<IReadOnlyList<WeatherReport>> GetReportsAsync(IProgress<int>? progress, CancellationToken ct)
    {
        var list = new List<WeatherReport>();
        for (var i = 0; i < Cities.Length; i++)
        {
            await Task.Delay(300, ct);                      // מדמה I/O; זורק OperationCanceledException בביטול
            if (FailRandomly && _rng.Next(4) == 0)
                throw new HttpRequestException($"Simulated network error while loading {Cities[i]}");

            list.Add(new WeatherReport(Cities[i], Math.Round(15 + _rng.NextDouble() * 20, 1), _rng.Next(2) == 0 ? "Sunny" : "Cloudy"));
            progress?.Report((i + 1) * 100 / Cities.Length);   // IProgress מריץ את ה-callback על ה-UI thread
        }
        return list;
    }
}
