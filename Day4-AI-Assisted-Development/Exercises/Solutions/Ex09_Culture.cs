using System.Globalization;

namespace Day4.Exercises.Solutions;

/// <summary>תרגיל 9: parsing תלוי-תרבות ו-DateTime.Now לתוקף.</summary>
public static class Ex09_Culture
{
    public static (decimal Amount, DateTimeOffset Expires) ParseVoucher(string amountText, TimeProvider clock)
    {
        if (!decimal.TryParse(amountText, NumberStyles.Number, CultureInfo.InvariantCulture, out var amount))
            throw new FormatException($"Invalid amount '{amountText}' (expected e.g. 19.90)");

        var expires = clock.GetUtcNow().AddDays(30);     // UTC, ו-clock מוזרק → ניתן לבדיקה
        return (amount, expires);
    }

    public static void Run()
    {
        var original = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = new CultureInfo("de-DE");
            Console.WriteLine($"de-DE: decimal.Parse(\"19.90\") = {decimal.Parse("19.90")}   <-- 1990! נקודה נחשבת מפריד אלפים");
            var (amount, expires) = ParseVoucher("19.90", TimeProvider.System);
            Console.WriteLine($"fixed: amount = {amount.ToString(CultureInfo.InvariantCulture)}, expires (UTC) = {expires:u}");
        }
        finally
        {
            CultureInfo.CurrentCulture = original;
        }
    }
}
