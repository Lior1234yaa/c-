namespace Day1.Lab3;

public class Inventory
{
    // Dictionary לפי SKU — חיפוש O(1). ההשוואה לא תלוית רישיות כדי ש-"k-100" ו-"K-100" יהיו אותו מפתח.
    private readonly Dictionary<string, StockItem> _items = new(StringComparer.OrdinalIgnoreCase);
    private readonly HashSet<string> _categories = new(StringComparer.OrdinalIgnoreCase);

    public event EventHandler<LowStockEventArgs>? LowStock;

    public IEnumerable<StockItem> Items => _items.Values;
    public IReadOnlySet<string> Categories => _categories;

    public void Add(Product product, int quantity, int reorderLevel = 5)
    {
        ArgumentNullException.ThrowIfNull(product);
        ArgumentOutOfRangeException.ThrowIfNegative(quantity);
        if (_items.ContainsKey(product.Sku))
            throw new InvalidOperationException($"SKU {product.Sku} already exists");

        _items[product.Sku] = new StockItem(product, quantity, reorderLevel);
        _categories.Add(product.Category);   // HashSet מתעלם מכפילויות
    }

    public StockItem Get(string sku) =>
        _items.TryGetValue(sku, out var item)
            ? item
            : throw new KeyNotFoundException($"Unknown SKU '{sku}'");

    public void Receive(string sku, int quantity)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity);
        Get(sku).Quantity += quantity;
    }

    public void Sell(string sku, int quantity)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity);
        var item = Get(sku);
        if (item.Quantity < quantity)
            throw new InvalidOperationException($"Cannot sell {quantity} x {item.Product.Name}: only {item.Quantity} in stock");

        item.Quantity -= quantity;
        if (item.IsLow)
            LowStock?.Invoke(this, new LowStockEventArgs(item));   // ?. — אם אין מנויים, לא קורה כלום
    }

    public IEnumerable<StockItem> Find(Func<StockItem, bool> predicate) => _items.Values.Where(predicate);

    public void ApplyToCategory(string category, Action<StockItem> action)
    {
        // ToList() — כי action עלול לשנות את האוסף שעליו אנחנו עוברים
        foreach (var item in Find(i => string.Equals(i.Product.Category, category, StringComparison.OrdinalIgnoreCase)).ToList())
            action(item);
    }
}
