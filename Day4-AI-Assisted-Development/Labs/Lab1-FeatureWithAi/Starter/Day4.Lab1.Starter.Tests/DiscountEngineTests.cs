namespace Day4.Lab1.Tests;

/// <summary>הבדיקות הן המפרט הניתן להרצה. הן נכשלות עד שתממשו את DiscountEngine.</summary>
public class DiscountEngineTests
{
    private readonly IDiscountEngine _engine = new DiscountEngine();

    private static Order Make(CustomerType type, string? coupon, params (int Qty, decimal Price)[] lines)
        => new(type, lines.Select((l, i) => new OrderLine($"S{i}", l.Qty, l.Price)).ToList(), coupon);

    [Fact]
    public void Calculate_EmptyOrder_NoDiscount()
    {
        var result = _engine.Calculate(new Order(CustomerType.Regular, []));
        Assert.Equal(0m, result.Amount);
        Assert.Equal("None", result.AppliedRule);
    }

    [Fact]
    public void Calculate_SmallRegularOrder_NoDiscount()
    {
        var result = _engine.Calculate(Make(CustomerType.Regular, null, (2, 30m)));
        Assert.Equal(0m, result.Amount);
        Assert.Equal("None", result.AppliedRule);
    }

    [Theory]
    [InlineData(10, 2.0, 1.00, "Volume")]   // 10 פריטים × 2 = 20 → 5% = 1.00
    [InlineData(49, 2.0, 4.90, "Volume")]   // גבול עליון של 5%
    [InlineData(50, 2.0, 10.00, "Volume")]  // 50 פריטים → 10%
    public void Calculate_VolumeRule(int qty, double price, double expected, string rule)
    {
        var result = _engine.Calculate(Make(CustomerType.Regular, null, (qty, (decimal)price)));
        Assert.Equal((decimal)expected, result.Amount);
        Assert.Equal(rule, result.AppliedRule);
    }

    [Fact]
    public void Calculate_AmountOver1000_Eight_Percent()
    {
        var result = _engine.Calculate(Make(CustomerType.Regular, null, (1, 1200m)));
        Assert.Equal(96m, result.Amount);
        Assert.Equal("Amount", result.AppliedRule);
    }

    [Fact]
    public void Calculate_Exactly1000_IsNotOver1000()
    {
        var result = _engine.Calculate(Make(CustomerType.Regular, null, (1, 1000m)));
        Assert.Equal(0m, result.Amount);
        Assert.Equal("None", result.AppliedRule);
    }

    [Fact]
    public void Calculate_RulesDoNotStack_HighestWins()
    {
        // 60 פריטים (10%) וגם סכום 1200 (8%) → 10% בלבד = 120
        var result = _engine.Calculate(Make(CustomerType.Regular, null, (60, 20m)));
        Assert.Equal(120m, result.Amount);
        Assert.Equal("Volume", result.AppliedRule);
    }

    [Fact]
    public void Calculate_Vip_TwelvePercent_EvenOnSmallOrder()
    {
        var result = _engine.Calculate(Make(CustomerType.Vip, null, (1, 10m)));
        Assert.Equal(1.2m, result.Amount);
        Assert.Equal("Vip", result.AppliedRule);
    }

    [Fact]
    public void Calculate_Vip_BeatsVolumeAndAmount()
    {
        var result = _engine.Calculate(Make(CustomerType.Vip, null, (60, 20m)));   // 1200 → 12% = 144
        Assert.Equal(144m, result.Amount);
        Assert.Equal("Vip", result.AppliedRule);
    }

    [Theory]
    [InlineData("WELCOME10")]
    [InlineData("welcome10")]
    [InlineData("Welcome10")]
    public void Calculate_Coupon_AddsFixed10_CaseInsensitive(string code)
    {
        var result = _engine.Calculate(Make(CustomerType.Regular, code, (1, 80m)));
        Assert.Equal(10m, result.Amount);
        Assert.Equal("None+Coupon", result.AppliedRule);
    }

    [Fact]
    public void Calculate_Coupon_StacksOnTopOfPercentRule()
    {
        var result = _engine.Calculate(Make(CustomerType.Vip, "WELCOME10", (1, 100m)));  // 12 + 10
        Assert.Equal(22m, result.Amount);
        Assert.Equal("Vip+Coupon", result.AppliedRule);
    }

    [Fact]
    public void Calculate_Coupon_RequiresSubtotalAtLeast50()
    {
        var result = _engine.Calculate(Make(CustomerType.Regular, "WELCOME10", (1, 49.99m)));
        Assert.Equal(0m, result.Amount);
        Assert.Equal("None", result.AppliedRule);
    }

    [Fact]
    public void Calculate_UnknownCoupon_IsIgnored()
    {
        var result = _engine.Calculate(Make(CustomerType.Regular, "FREESTUFF", (1, 80m)));
        Assert.Equal(0m, result.Amount);
        Assert.Equal("None", result.AppliedRule);
    }

    [Fact]
    public void Calculate_DiscountNeverExceedsSubtotal()
    {
        // VIP על 5 ₪: 0.6 + קופון לא חל (<50) → 0.6. על 50 ₪: 6 + 10 = 16 ≤ 50.
        var result = _engine.Calculate(Make(CustomerType.Vip, "WELCOME10", (1, 50m)));
        Assert.Equal(16m, result.Amount);
        Assert.True(result.Amount <= 50m);
    }

    [Fact]
    public void Calculate_RoundsToTwoDecimals()
    {
        // 3 × 33.33 = 99.99 ; VIP 12% = 11.9988 → 12.00
        var result = _engine.Calculate(Make(CustomerType.Vip, null, (3, 33.33m)));
        Assert.Equal(12.00m, result.Amount);
    }
}
