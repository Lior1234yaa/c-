using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Day3.Lab3.Solution.Models;

namespace Day3.Lab3.Solution.Services;

/// <summary>
/// מימוש מול REST API. ברירת המחדל: Day2.LocalApi (http://localhost:5080/api/products).
/// ה-DTO סלחני: מקבל name או title, ותומך גם בתשובה של מערך וגם באובייקט עם products[] (למשל dummyjson.com).
/// </summary>
public class HttpProductService(HttpClient http, string url = "http://localhost:5080/api/products") : IProductService
{
    private static readonly JsonSerializerOptions Json = new() { PropertyNameCaseInsensitive = true };

    public string Name => $"HTTP ({url})";

    public async Task<IReadOnlyList<Product>> GetProductsAsync(IProgress<int>? progress, CancellationToken ct)
    {
        progress?.Report(10);
        using var response = await http.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, ct);
        response.EnsureSuccessStatusCode();          // 4xx/5xx → HttpRequestException
        progress?.Report(50);

        await using var stream = await response.Content.ReadAsStreamAsync(ct);
        using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: ct);
        var root = doc.RootElement;
        var array = root.ValueKind == JsonValueKind.Array ? root
                  : root.TryGetProperty("products", out var p) ? p
                  : throw new InvalidOperationException("Unexpected JSON shape: expected an array or { products: [] }");

        var dtos = array.Deserialize<List<ProductDto>>(Json) ?? [];
        progress?.Report(100);
        return dtos.Select(d => d.ToProduct()).ToList();
    }

    private sealed class ProductDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Title { get; set; }
        public string? Category { get; set; }
        public decimal Price { get; set; }
        [JsonPropertyName("stock")] public int? Stock { get; set; }
        [JsonPropertyName("quantity")] public int? Quantity { get; set; }

        public Product ToProduct() => new(Id, Name ?? Title ?? $"#{Id}", Category ?? "General", Price, Stock ?? Quantity ?? 0);
    }
}
