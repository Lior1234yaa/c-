namespace Day1.Lab3;

public class Inventory
{
    // TODO 1: שדה פרטי Dictionary<string, StockItem> לפי SKU (מפתח לא תלוי רישיות:
    //         new Dictionary<string, StockItem>(StringComparer.OrdinalIgnoreCase))
    //         ושדה HashSet<string> של קטגוריות (גם הוא OrdinalIgnoreCase).

    // TODO 2: event LowStock מטיפוס EventHandler<LowStockEventArgs>

    // TODO 3: Items — IEnumerable<StockItem> של כל הפריטים; Categories — IReadOnlySet<string>
    public IEnumerable<StockItem> Items => throw new NotImplementedException();
    public IReadOnlySet<string> Categories => throw new NotImplementedException();

    // TODO 4: Add — מוסיף פריט חדש. SKU כפול → InvalidOperationException. מעדכן את סט הקטגוריות.
    public void Add(Product product, int quantity, int reorderLevel = 5) => throw new NotImplementedException();

    // TODO 5: Get — מחזיר StockItem לפי SKU או זורק KeyNotFoundException עם הודעה ברורה.
    public StockItem Get(string sku) => throw new NotImplementedException();

    // TODO 6: Receive(sku, qty) — קליטת סחורה (qty חייב להיות חיובי).
    public void Receive(string sku, int quantity) => throw new NotImplementedException();

    // TODO 7: Sell(sku, qty) — מכירה. אין מספיק → InvalidOperationException.
    //         אחרי ההפחתה: אם הפריט ירד לסף (IsLow) — הפעילו את האירוע LowStock.
    public void Sell(string sku, int quantity) => throw new NotImplementedException();

    // TODO 8: Find — מקבל Func<StockItem, bool> ומחזיר את כל הפריטים שעומדים בתנאי.
    public IEnumerable<StockItem> Find(Func<StockItem, bool> predicate) => throw new NotImplementedException();

    // TODO 9: ApplyToCategory — מקבל שם קטגוריה ו-Action<StockItem> ומפעיל אותו על כל פריט בקטגוריה.
    public void ApplyToCategory(string category, Action<StockItem> action) => throw new NotImplementedException();
}
