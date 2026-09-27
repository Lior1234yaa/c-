using Day4.Lab2.Domain;
using Day4.Lab2.Services;

namespace Day4.Lab2.Tests;

public class PricingServiceTests
{
    private static Subscription Make(Plan plan, int addons = 0, int days = 30, int months = 0, Coupon coupon = Coupon.None)
        => new("S-1", "c", plan, addons, days, months, coupon);

    [Theory]
    [InlineData(Plan.Basic, 29.9)]
    [InlineData(Plan.Pro, 79.0)]
    [InlineData(Plan.Team, 199.0)]
    public void BasePrice_ByPlan(Plan plan, double expected)
        => Assert.Equal((decimal)expected, PricingService.BasePrice(plan));

    [Theory]
    [InlineData(Plan.Basic, 2, 19.8)]
    [InlineData(Plan.Pro, 1, 14.9)]
    [InlineData(Plan.Team, 3, 59.7)]
    [InlineData(Plan.Team, 5, 89.5)]   // 3×19.9 + 2×14.9
    public void AddonPrice_ByPlanWithTeamBulk(Plan plan, int addons, double expected)
        => Assert.Equal((decimal)expected, PricingService.AddonPrice(plan, addons));

    [Fact]
    public void Prorate_HalfMonth_HalvesAmount()
        => Assert.Equal(50m, PricingService.Prorate(100m, 15));

    [Fact]
    public void Prorate_FullMonth_Unchanged()
        => Assert.Equal(100m, PricingService.Prorate(100m, 30));

    [Theory]
    [InlineData(0, 0)]
    [InlineData(11, 0)]
    [InlineData(12, 10)]
    [InlineData(24, 15)]
    public void TenureDiscount_Tiers(int months, double expected)
        => Assert.Equal((decimal)expected, PricingService.TenureDiscount(100m, months));

    [Fact]
    public void Welcome_NewCustomer_HalfOff()
        => Assert.Equal(50m, PricingService.CouponDiscount(100m, Make(Plan.Pro, months: 0, coupon: Coupon.Welcome)));

    [Fact]
    public void Welcome_ExistingCustomer_TwentyPercent()
        => Assert.Equal(20m, PricingService.CouponDiscount(100m, Make(Plan.Pro, months: 1, coupon: Coupon.Welcome)));

    [Fact]
    public void Loyal_Under12Months_Nothing()
        => Assert.Equal(0m, PricingService.CouponDiscount(100m, Make(Plan.Pro, months: 6, coupon: Coupon.Loyal)));

    [Fact]
    public void Price_DiscountNeverExceedsProrated()
    {
        var bill = new PricingService().Price(Make(Plan.Basic, days: 1, months: 30, coupon: Coupon.Loyal)); // pror ≈ 1, disc 15% + 10
        Assert.True(bill.Discount <= bill.Prorated);
        Assert.True(bill.Total >= 0);
    }

    [Fact]
    public void Price_TaxIsEighteenPercentAfterDiscount()
    {
        var bill = new PricingService().Price(Make(Plan.Pro));   // 79, no discount
        Assert.Equal(79m * 0.18m, bill.Tax);
    }
}
