namespace Day4.Lab1;

/// <summary>כלל הנחה באחוזים. מחזיר 0 כשלא חל.</summary>
public interface IDiscountRule
{
    string Name { get; }
    decimal Rate(Order order);
}

public sealed class VolumeRule : IDiscountRule
{
    private const int Tier1Qty = 10, Tier2Qty = 50;
    private const decimal Tier1Rate = 0.05m, Tier2Rate = 0.10m;

    public string Name => "Volume";
    public decimal Rate(Order order) => order.TotalQuantity switch
    {
        >= Tier2Qty => Tier2Rate,
        >= Tier1Qty => Tier1Rate,
        _ => 0m,
    };
}

public sealed class AmountRule : IDiscountRule
{
    private const decimal Threshold = 1000m, RateValue = 0.08m;
    public string Name => "Amount";
    public decimal Rate(Order order) => order.Subtotal > Threshold ? RateValue : 0m;
}

public sealed class VipRule : IDiscountRule
{
    private const decimal RateValue = 0.12m;
    public string Name => "Vip";
    public decimal Rate(Order order) => order.Type == CustomerType.Vip ? RateValue : 0m;
}

/// <summary>
/// מנוע ההנחות: בוחר את כלל האחוזים הגבוה ביותר (לא מצטבר), מוסיף קופון קבוע אם זכאי,
/// מגביל לסכום ההזמנה ומעגל פעם אחת בסוף. פונקציה טהורה.
/// </summary>
public sealed class DiscountEngine(IEnumerable<IDiscountRule> rules) : IDiscountEngine
{
    private const string CouponCode = "WELCOME10";
    private const decimal CouponAmount = 10m, CouponMinSubtotal = 50m;

    private readonly IReadOnlyList<IDiscountRule> _rules = rules.ToList();

    public DiscountEngine() : this([new VolumeRule(), new AmountRule(), new VipRule()]) { }

    public DiscountResult Calculate(Order order)
    {
        ArgumentNullException.ThrowIfNull(order);
        var subtotal = order.Subtotal;
        if (subtotal <= 0m) return new DiscountResult(0m, "None");

        var (rate, ruleName) = BestPercentRule(order);
        var amount = subtotal * rate;

        var coupon = CouponDiscount(order);
        if (coupon > 0m)
        {
            amount += coupon;
            ruleName += "+Coupon";
        }

        amount = Math.Min(amount, subtotal);
        return new DiscountResult(Math.Round(amount, 2), ruleName);
    }

    private (decimal Rate, string Name) BestPercentRule(Order order)
    {
        var best = (Rate: 0m, Name: "None");
        foreach (var rule in _rules)
        {
            var rate = rule.Rate(order);
            if (rate > best.Rate) best = (rate, rule.Name);
        }
        return best;
    }

    private static decimal CouponDiscount(Order order)
        => order.Subtotal >= CouponMinSubtotal
           && string.Equals(order.CouponCode?.Trim(), CouponCode, StringComparison.OrdinalIgnoreCase)
            ? CouponAmount
            : 0m;
}
