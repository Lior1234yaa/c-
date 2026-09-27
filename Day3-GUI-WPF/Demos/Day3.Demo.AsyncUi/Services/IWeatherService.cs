namespace Day3.Demo.AsyncUi.Services;

public record WeatherReport(string City, double TempC, string Summary);

/// <summary>ממשק לשירות — ה-UI תלוי בממשק, לא במימוש. קל להחליף ב-HttpClient אמיתי (ראו יום 2).</summary>
public interface IWeatherService
{
    Task<IReadOnlyList<WeatherReport>> GetReportsAsync(IProgress<int>? progress, CancellationToken ct);
}
