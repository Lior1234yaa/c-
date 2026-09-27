using Day4.Demo.Refactored.Domain;
using Day4.Demo.Refactored.Services;

namespace Day4.Demo.Refactored.Tests;

public class DiscountPolicyTests
{
    private static Order Make(CustomerType type, int qty, decimal price)
        => new(1, "c", type, qty, price, "IL", OrderStatus.Paid, new DateOnly(2025, 1, 1));

    [Theory]
    [InlineData(CustomerType.Vip, 1, 2000, 300)]     // 15% מעל 1000
    [InlineData(CustomerType.Vip, 1, 500, 60)]       // 12% מעל 250
    [InlineData(CustomerType.Vip, 1, 100, 10)]       // 10% בסיס
    [InlineData(CustomerType.Business, 50, 10, 100)] // 20% מ-50 יח'
    [InlineData(CustomerType.Business, 10, 10, 10)]  // 10% מ-10 יח'
    [InlineData(CustomerType.Business, 9, 10, 0)]
    [InlineData(CustomerType.Regular, 1, 300, 15)]   // 5% מעל 250
    [InlineData(CustomerType.Regular, 10, 10, 5)]    // 5% מ-10 יח'
    [InlineData(CustomerType.Regular, 2, 50, 0)]
    [InlineData(CustomerType.New, 1, 100, 7)]        // 7%
    [InlineData(CustomerType.New, 1, 1000, 30)]      // תקרה 30
    public void Calculate_MatchesBusinessRules(CustomerType type, int qty, decimal price, decimal expected)
    {
        var policy = new DiscountPolicy();
        Assert.Equal(expected, policy.Calculate(Make(type, qty, price)));
    }

    [Fact]
    public void Calculate_NeverNegative()
    {
        var policy = new DiscountPolicy();
        Assert.True(policy.Calculate(Make(CustomerType.Vip, 0, 0)) >= 0);
    }
}
