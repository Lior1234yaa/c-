using Day3.Lab3.Starter.Models;

namespace Day3.Lab3.Starter.Services;

/// <summary>ה-UI תלוי בממשק הזה בלבד. FakeProductService לעבודה offline, HttpProductService מול API.</summary>
public interface IProductService
{
    string Name { get; }
    Task<IReadOnlyList<Product>> GetProductsAsync(IProgress<int>? progress, CancellationToken ct);
}
