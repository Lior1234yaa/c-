namespace Day4.Lab3.Snippets;

/// <summary>
/// Builds parameterized queries. The SQL never contains user input; values travel as parameters
/// (the caller binds them, e.g. with SqlCommand.Parameters or Dapper).
/// </summary>
public static class CustomerSearch
{
    public sealed record Query(string Sql, IReadOnlyDictionary<string, object> Parameters);

    public static Query BuildQuery(string name)
    {
        ArgumentNullException.ThrowIfNull(name);
        // LIKE: גם תווי wildcard של המשתמש (% _) מנוטרלים
        var pattern = "%" + name.Replace("[", "[[]").Replace("%", "[%]").Replace("_", "[_]") + "%";
        return new Query(
            "SELECT Id, Name FROM Customers WHERE Name LIKE @pattern ORDER BY Name",
            new Dictionary<string, object> { ["@pattern"] = pattern });
    }

    public static Query BuildByIdQuery(int id)
        => new("SELECT Id, Name FROM Customers WHERE Id = @id", new Dictionary<string, object> { ["@id"] = id });
}
