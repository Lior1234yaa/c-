using Day3.Lab3.Solution.Models;

namespace Day3.Lab3.Solution.Services;

/// <summary>ה-UI תלוי בממשק הזה בלבד. FakeProductService לעבודה offline, HttpProductService מול API.</summary>
public interface IProductService
{
    string Name { get; }
    Task<IReadOnlyList<Product>> GetProductsAsync(IProgress<int>? progress, CancellationToken ct);
}
