using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Day3.Lab3.Starter.Models;

namespace Day3.Lab3.Starter.Services;

/// <summary>מימוש מול REST API. ברירת המחדל: Day2.LocalApi (http://localhost:5080/api/products).</summary>
public class HttpProductService(HttpClient http, string url = "http://localhost:5080/api/products") : IProductService
{
    private static readonly JsonSerializerOptions Json = new() { PropertyNameCaseInsensitive = true };

    public string Name => $"HTTP ({url})";

    // TODO 4: לממש בעזרת HttpClient (זוכרים מ-Day 2?):
    //   1. http.GetAsync(url, ct) + EnsureSuccessStatusCode()
    //   2. לקרוא את ה-JSON: מערך של ProductDto (או אובייקט עם products[] — בונוס)
    //   3. progress?.Report(...) בשלבים, ולהמיר ל-Product עם ToProduct()
    public Task<IReadOnlyList<Product>> GetProductsAsync(IProgress<int>? progress, CancellationToken ct) =>
        throw new NotImplementedException();

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
