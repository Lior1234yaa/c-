using System.Globalization;
using System.Windows.Controls;

namespace Day3.Demo.Validation.Rules;

/// <summary>ValidationRule: רץ ב-Binding לפני שהערך מגיע ל-source. מתאים לבדיקות "מקומיות" של פקד.</summary>
public class RangeRule : ValidationRule
{
    public int Min { get; set; } = 0;
    public int Max { get; set; } = 100;

    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        var text = value as string ?? "";
        if (!int.TryParse(text, NumberStyles.Integer, cultureInfo, out var n))
            return new ValidationResult(false, "יש להזין מספר שלם");
        if (n < Min || n > Max)
            return new ValidationResult(false, $"הערך חייב להיות בין {Min} ל-{Max}");
        return ValidationResult.ValidResult;
    }
}
