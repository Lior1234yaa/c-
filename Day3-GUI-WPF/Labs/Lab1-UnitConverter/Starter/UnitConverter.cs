namespace Day3.Lab1.Starter;

/// <summary>הלוגיקה — בלי שום תלות ב-WPF.</summary>
public static class UnitConverter
{
    // TODO 1: המרה דרך מטר: value * from.MetersPerUnit / to.MetersPerUnit
    public static double Convert(double value, LengthUnit from, LengthUnit to) =>
        throw new NotImplementedException();

    // TODO 2: להחזיר null אם הקלט תקין, אחרת הודעת שגיאה בעברית.
    // רמז: double.TryParse עם CultureInfo.InvariantCulture; קלט ריק / לא מספר / שלילי = שגיאה.
    public static string? TryParseInput(string? text, out double value)
    {
        value = 0;
        throw new NotImplementedException();
    }

    public static string Format(double value) => value.ToString("#,##0.####");
}
