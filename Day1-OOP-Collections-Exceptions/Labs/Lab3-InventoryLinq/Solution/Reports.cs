namespace Day1.Lab3;

/// <summary>דוחות — פונקציות טהורות מעל IEnumerable&lt;StockItem&gt; (לא משנות כלום).</summary>
public static class Reports
{
    public static decimal TotalValue(IEnumerable<StockItem> items) => items.Sum(i => i.Value);

    public static IEnumerable<CategorySummary> ByCategory(IEnumerable<StockItem> items) =>
        items.GroupBy(i => i.Product.Category)
             .Select(g => new CategorySummary(
                 g.Key,
                 g.Count(),
                 g.Sum(i => i.Quantity),
                 g.Sum(i => i.Value)))
             .OrderByDescending(c => c.TotalValue);

    public static IEnumerable<StockItem> TopByValue(IEnumerable<StockItem> items, int n) =>
        items.OrderByDescending(i => i.Value).Take(n);

    public static IEnumerable<StockItem> LowStock(IEnumerable<StockItem> items) =>
        items.Where(i => i.IsLow).OrderBy(i => i.Quantity).ThenBy(i => i.Product.Name);

    public static Dictionary<string, decimal> PriceIndex(IEnumerable<StockItem> items) =>
        items.ToDictionary(i => i.Product.Sku, i => i.Product.Price);

    public static HashSet<string> SkusInBoth(IEnumerable<StockItem> a, IEnumerable<StockItem> b)
    {
        var set = a.Select(i => i.Product.Sku).ToHashSet();
        set.IntersectWith(b.Select(i => i.Product.Sku));
        return set;
    }

    // בונוס: אותו דוח ב-query syntax
    public static IEnumerable<string> NamesByCategoryQuery(IEnumerable<StockItem> items) =>
        from i in items
        orderby i.Product.Category, i.Product.Name
        select $"{i.Product.Category}/{i.Product.Name}";
}

public record CategorySummary(string Category, int ProductCount, int TotalQuantity, decimal TotalValue);
