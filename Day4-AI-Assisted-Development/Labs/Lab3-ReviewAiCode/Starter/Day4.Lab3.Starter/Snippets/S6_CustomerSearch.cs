namespace Day4.Lab3.Snippets;

/// <summary>Builds the SQL used to search customers by name (executed elsewhere).</summary>
public class CustomerSearch
{
    public string BuildQuery(string name)
    {
        return "SELECT Id, Name FROM Customers WHERE Name LIKE '%" + name + "%' ORDER BY Name";
    }

    public string BuildByIdQuery(string id)
    {
        return $"SELECT * FROM Customers WHERE Id = {id}";
    }
}
