using System.Text.Json.Serialization;

namespace Day2.Lab4;

public enum OrderStatus { Pending, Paid, Shipped, Cancelled }

public record Stats(int Products, int Orders, int Pending, int Shipped, decimal Revenue);
public record OrderItem(int ProductId, string ProductName, int Quantity, decimal UnitPrice);
public record Order(int Id, string Customer, DateTime CreatedAt, OrderStatus Status, List<OrderItem> Items, decimal Total);
public record Health(string Status, DateTime Time);

// Open-Meteo: {"current": {"time": "2026-06-01T12:00", "temperature_2m": 27.4, "wind_speed_10m": 12.1}}
public record WeatherCurrent(
    DateTime Time,
    [property: JsonPropertyName("temperature_2m")] double Temperature,
    [property: JsonPropertyName("wind_speed_10m")] double WindSpeed);
public record WeatherResponse(WeatherCurrent Current);

/// <summary>תוצאה של מקור: הצלחה עם ערך, או כישלון עם הודעה. תמיד עם משך.</summary>
public record SourceResult<T>(T? Value, string? Error, TimeSpan Elapsed)
{
    public bool IsOk => Error is null;
    public static SourceResult<T> Ok(T value, TimeSpan elapsed) => new(value, null, elapsed);
    public static SourceResult<T> Fail(string error, TimeSpan elapsed) => new(default, error, elapsed);
}
