namespace Day4.Lab4.Core.Models;

public sealed record Expense(Guid Id, DateOnly Date, string Category, decimal Amount, string? Note);

public static class Categories
{
    public static readonly IReadOnlyList<string> All = ["מזון", "תחבורה", "דיור", "בילויים", "בריאות", "אחר"];
}
