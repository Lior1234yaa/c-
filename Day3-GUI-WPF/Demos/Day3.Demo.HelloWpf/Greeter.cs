namespace Day3.Demo.HelloWpf;

/// <summary>
/// לוגיקה "עסקית" קטנה שמופרדת מה-UI: קל לבדוק אותה ב-unit test בלי חלון.
/// </summary>
public static class Greeter
{
    public static string Greet(string? name)
    {
        var trimmed = (name ?? string.Empty).Trim();
        return trimmed.Length == 0 ? "Hello, guest!" : $"Hello, {trimmed}!";
    }
}
