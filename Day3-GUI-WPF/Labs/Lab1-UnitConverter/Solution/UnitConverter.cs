using System.Globalization;

namespace Day3.Lab1.Solution;

/// <summary>הלוגיקה — בלי שום תלות ב-WPF. אפשר לבדוק ב-unit test.</summary>
public static class UnitConverter
{
    public static double Convert(double value, LengthUnit from, LengthUnit to) =>
        value * from.MetersPerUnit / to.MetersPerUnit;

    /// <summary>
    /// מנסה לפרסר קלט של משתמש. מחזיר הודעת שגיאה (או null אם תקין).
    /// מקבל גם פסיק כנקודה עשרונית כדי להיות ידידותי.
    /// </summary>
    public static string? TryParseInput(string? text, out double value)
    {
        value = 0;
        var t = (text ?? "").Trim().Replace(',', '.');
        if (t.Length == 0) return "הזינו ערך";
        if (!double.TryParse(t, NumberStyles.Float, CultureInfo.InvariantCulture, out value))
            return "הערך חייב להיות מספר";
        if (value < 0) return "הערך חייב להיות חיובי";
        return null;
    }

    public static string Format(double value) =>
        value switch
        {
            0 => "0",
            < 0.001 or >= 1_000_000_000 => value.ToString("0.###E0", CultureInfo.InvariantCulture),
            _ => value.ToString("#,##0.####", CultureInfo.InvariantCulture),
        };
}
