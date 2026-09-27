namespace Day4.Lab3.Snippets;

/// <summary>Returns one page of items. Pages are 1-based; the last page may be short.</summary>
public static class Paginator
{
    public static List<T> GetPage<T>(IReadOnlyList<T> items, int page, int pageSize)
    {
        ArgumentNullException.ThrowIfNull(items);
        ArgumentOutOfRangeException.ThrowIfLessThan(page, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(pageSize, 1);

        var start = (page - 1) * pageSize;
        var end = Math.Min(start + pageSize, items.Count);   // exclusive
        var result = new List<T>(Math.Max(0, end - start));
        for (var i = start; i < end; i++)
            result.Add(items[i]);
        return result;
    }

    public static int PageCount(int totalItems, int pageSize)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(pageSize, 1);
        return (totalItems + pageSize - 1) / pageSize;   // ceiling
    }
}
