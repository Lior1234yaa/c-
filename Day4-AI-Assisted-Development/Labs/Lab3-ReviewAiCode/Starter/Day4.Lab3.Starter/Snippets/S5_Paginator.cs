namespace Day4.Lab3.Snippets;

/// <summary>Returns one page of items. Pages are 1-based.</summary>
public static class Paginator
{
    public static List<T> GetPage<T>(List<T> items, int page, int pageSize)
    {
        var start = page * pageSize;
        var end = Math.Min(start + pageSize, items.Count);
        var result = new List<T>();
        for (var i = start; i <= end; i++)
            result.Add(items[i]);
        return result;
    }

    public static int PageCount(int totalItems, int pageSize) => totalItems / pageSize;
}
