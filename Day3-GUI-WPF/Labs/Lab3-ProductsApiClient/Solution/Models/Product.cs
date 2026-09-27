namespace Day3.Lab3.Solution.Models;

/// <summary>המודל של ה-UI — לא תלוי בצורת ה-JSON של ה-API (ראו ProductDto ב-HttpProductService).</summary>
public record Product(int Id, string Name, string Category, decimal Price, int Stock)
{
    public bool InStock => Stock > 0;
}
