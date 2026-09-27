using System.Net.Http;
using Day3.Lab3.Solution.Models;

namespace Day3.Lab3.Solution.Services;

/// <summary>מימוש in-memory שמדמה רשת איטית (וגם כשל אקראי) — כדי שהמעבדה תעבוד בלי שרת.</summary>
public class FakeProductService : IProductService
{
    private static readonly string[] Categories = ["Laptops", "Phones", "Audio", "Accessories", "Monitors"];
    private readonly Random _rng = new(42);
    private readonly List<Product> _catalog;

    public string Name => "Fake (in-memory)";
    public int DelayPerBatchMs { get; set; } = 250;
    public bool FailRandomly { get; set; }

    public FakeProductService()
    {
        _catalog = Enumerable.Range(1, 60).Select(i => new Product(
            i,
            $"{Categories[i % Categories.Length].TrimEnd('s')} Model {i:00}",
            Categories[i % Categories.Length],
            Math.Round((decimal)(_rng.NextDouble() * 4000 + 50), 2),
            _rng.Next(0, 4) == 0 ? 0 : _rng.Next(1, 100))).ToList();
    }

    public async Task<IReadOnlyList<Product>> GetProductsAsync(IProgress<int>? progress, CancellationToken ct)
    {
        const int batches = 6;
        var result = new List<Product>();
        for (var b = 0; b < batches; b++)
        {
            await Task.Delay(DelayPerBatchMs, ct);
            if (FailRandomly && b == 3 && _rng.Next(2) == 0)
                throw new HttpRequestException("Simulated network failure (503)");
            result.AddRange(_catalog.Skip(b * 10).Take(10));
            progress?.Report((b + 1) * 100 / batches);
        }
        return result;
    }
}
