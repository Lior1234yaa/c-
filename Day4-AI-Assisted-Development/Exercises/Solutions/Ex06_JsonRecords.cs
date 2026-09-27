using System.Text.Json;
using System.Text.Json.Serialization;

namespace Day4.Exercises.Solutions;

/// <summary>תרגיל 6: records ל-JSON — decimal לכסף, string? ל-email, DateTimeOffset לזמן עם offset.</summary>
public static class Ex06_JsonRecords
{
    public sealed record OrderDto(
        [property: JsonPropertyName("order_id")] int OrderId,
        [property: JsonPropertyName("customer")] CustomerDto Customer,
        [property: JsonPropertyName("lines")] List<LineDto> Lines,
        [property: JsonPropertyName("created_at")] DateTimeOffset CreatedAt);

    public sealed record CustomerDto(
        [property: JsonPropertyName("id")] int Id,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("email")] string? Email);

    public sealed record LineDto(
        [property: JsonPropertyName("sku")] string Sku,
        [property: JsonPropertyName("qty")] int Qty,
        [property: JsonPropertyName("unit_price")] decimal UnitPrice);

    private const string Json = """
        { "order_id": 42, "customer": { "id": 7, "name": "Dana", "email": null },
          "lines": [ { "sku": "A-1", "qty": 2, "unit_price": 19.9 } ],
          "created_at": "2025-03-01T10:15:00+02:00" }
        """;

    public static void Run()
    {
        var order = JsonSerializer.Deserialize<OrderDto>(Json)
                    ?? throw new InvalidOperationException("JSON was null");

        Console.WriteLine($"Order {order.OrderId} for {order.Customer.Name} (email: {order.Customer.Email ?? "none"})");
        Console.WriteLine($"Created: {order.CreatedAt:O}  (UTC: {order.CreatedAt.UtcDateTime:O})");
        Console.WriteLine($"Total:   {order.Lines.Sum(l => l.Qty * l.UnitPrice)}  (decimal — 39.8 בדיוק, לא 39.800000000000004)");
    }
}
