namespace Day1.Lab3;

/// <summary>מוצר בקטלוג — נתונים בלתי משתנים, לכן record.</summary>
public record Product(string Sku, string Name, string Category, decimal Price);

/// <summary>פריט מלאי: מוצר + כמות במחסן + סף הזמנה מחדש.</summary>
public class StockItem(Product product, int quantity, int reorderLevel)
{
    public Product Product { get; } = product;
    public int Quantity { get; internal set; } = quantity;
    public int ReorderLevel { get; } = reorderLevel;

    public decimal Value => Product.Price * Quantity;
    public bool IsLow => Quantity <= ReorderLevel;

    public override string ToString() =>
        $"{Product.Sku,-6} {Product.Name,-18} {Product.Category,-12} {Product.Price,8:N2} x{Quantity,4}  = {Value,10:N2}{(IsLow ? "  LOW" : "")}";
}

/// <summary>נתוני האירוע "מלאי נמוך".</summary>
public class LowStockEventArgs(StockItem item) : EventArgs
{
    public StockItem Item { get; } = item;
}
