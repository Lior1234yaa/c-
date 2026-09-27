using System.Globalization;

namespace Day4.Lab4.Core.Models;

/// <summary>כללי ה-validation של המפרט — טהורים וניתנים לבדיקה, בלי תלות ב-WPF.</summary>
public static class ExpenseValidator
{
    public const int MaxNoteLength = 100;

    public static string? ValidateAmountText(string? text, out decimal amount)
    {
        amount = 0m;
        if (string.IsNullOrWhiteSpace(text)) return "יש להזין סכום";
        if (!decimal.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out amount)) return "סכום לא תקין (למשל 12.50)";
        if (amount <= 0m) return "הסכום חייב להיות גדול מאפס";
        if (decimal.Round(amount, 2) != amount) return "עד 2 ספרות אחרי הנקודה";
        return null;
    }

    public static string? ValidateCategory(string? category)
        => string.IsNullOrWhiteSpace(category) || !Categories.All.Contains(category) ? "יש לבחור קטגוריה" : null;

    public static string? ValidateDate(DateOnly? date, DateOnly today)
        => date is null ? "יש לבחור תאריך" : date > today ? "התאריך לא יכול להיות בעתיד" : null;

    public static string? ValidateNote(string? note)
        => note is { Length: > MaxNoteLength } ? $"הערה עד {MaxNoteLength} תווים" : null;
}
