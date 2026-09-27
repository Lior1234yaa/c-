namespace Day4.Demo.Refactored.Services;

/// <summary>דמי משלוח לפי מדינה, עם סף למשלוח חינם. טבלה במקום שרשרת if.</summary>
public sealed class ShippingCalculator : IShippingCalculator
{
    private sealed record Rule(decimal FreeAbove, decimal Fee);

    private static readonly Dictionary<string, Rule> Rules = new(StringComparer.OrdinalIgnoreCase)
    {
        ["IL"] = new(FreeAbove: 200m, Fee: 25m),
        ["US"] = new(FreeAbove: 500m, Fee: 60m),
        ["DE"] = new(FreeAbove: 400m, Fee: 45m),
        ["FR"] = new(FreeAbove: 400m, Fee: 45m),
    };

    private const decimal DefaultFee = 80m;

    public decimal Calculate(string country, decimal amountAfterDiscount)
    {
        if (!Rules.TryGetValue(country, out var rule)) return DefaultFee;
        return amountAfterDiscount >= rule.FreeAbove ? 0m : rule.Fee;
    }
}

public sealed class IsraelVatCalculator : IVatCalculator
{
    private const decimal Rate = 0.18m;

    public decimal Calculate(string country, decimal taxableAmount)
        => string.Equals(country, "IL", StringComparison.OrdinalIgnoreCase) ? taxableAmount * Rate : 0m;
}
