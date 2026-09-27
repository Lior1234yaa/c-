namespace Day1.Lab3;

/// <summary>דוחות — פונקציות טהורות מעל IEnumerable&lt;StockItem&gt; (לא משנות כלום).</summary>
public static class Reports
{
    // TODO 10: TotalValue — סכום Value של כל הפריטים
    public static decimal TotalValue(IEnumerable<StockItem> items) => throw new NotImplementedException();

    // TODO 11: ByCategory — לכל קטגוריה: מספר פריטים, סה"כ כמות, סה"כ ערך. ממוין לפי ערך יורד.
    //          החזירו IEnumerable של record CategorySummary (מוגדר למטה).
    public static IEnumerable<CategorySummary> ByCategory(IEnumerable<StockItem> items) => throw new NotImplementedException();

    // TODO 12: TopByValue(items, n) — n הפריטים היקרים ביותר במלאי (לפי Value)
    public static IEnumerable<StockItem> TopByValue(IEnumerable<StockItem> items, int n) => throw new NotImplementedException();

    // TODO 13: LowStock — כל הפריטים ש-IsLow, ממוינים לפי כמות עולה ואז לפי שם
    public static IEnumerable<StockItem> LowStock(IEnumerable<StockItem> items) => throw new NotImplementedException();

    // TODO 14: PriceIndex — Dictionary<string, decimal> מ-SKU למחיר (ToDictionary)
    public static Dictionary<string, decimal> PriceIndex(IEnumerable<StockItem> items) => throw new NotImplementedException();

    // TODO 15 (בונוס): SkusInBoth — HashSet של ה-SKU שמופיעים גם ברשימה a וגם ב-b (IntersectWith / Intersect)
    public static HashSet<string> SkusInBoth(IEnumerable<StockItem> a, IEnumerable<StockItem> b) => throw new NotImplementedException();
}

public record CategorySummary(string Category, int ProductCount, int TotalQuantity, decimal TotalValue);
