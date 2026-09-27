using Day4.Lab2.Domain;

namespace Day4.Lab2.Services;

/// <summary>כל כללי התמחור, כל אחד במתודה קטנה עם קבועים בשם. אותה התנהגות כמו הגרסה הישנה.</summary>
public sealed class PricingService : IPricingService
{
    private const decimal BasicPrice = 29.9m, ProPrice = 79.0m, TeamPrice = 199.0m;
    private const decimal BasicAddon = 9.9m, ProAddon = 14.9m, TeamAddon = 19.9m, TeamAddonAfterBulk = 14.9m;
    private const int TeamBulkAddons = 3;

    private const int LoyalTier2Months = 24, LoyalTier1Months = 12;
    private const decimal LoyalTier2Rate = 0.15m, LoyalTier1Rate = 0.10m;
    private const decimal WelcomeNewRate = 0.5m, WelcomeRate = 0.2m;
    private const decimal LoyalCouponTier2 = 10m, LoyalCouponTier1 = 5m;
    private const decimal TaxRate = 0.18m;

    public Bill Price(Subscription s)
    {
        var subtotal = BasePrice(s.Plan) + AddonPrice(s.Plan, s.Addons);
        var prorated = Prorate(subtotal, s.DaysActive);
        var discount = Math.Min(TenureDiscount(prorated, s.TenureMonths) + CouponDiscount(prorated, s), prorated);
        var tax = (prorated - discount) * TaxRate;
        return new Bill(s, subtotal, prorated, discount, tax);
    }

    public static decimal BasePrice(Plan plan) => plan switch
    {
        Plan.Basic => BasicPrice,
        Plan.Pro => ProPrice,
        Plan.Team => TeamPrice,
        _ => 0m,
    };

    public static decimal AddonPrice(Plan plan, int addons) => plan switch
    {
        Plan.Basic => addons * BasicAddon,
        Plan.Pro => addons * ProAddon,
        Plan.Team when addons > TeamBulkAddons => TeamBulkAddons * TeamAddon + (addons - TeamBulkAddons) * TeamAddonAfterBulk,
        Plan.Team => addons * TeamAddon,
        _ => 0m,
    };

    public static decimal Prorate(decimal amount, int daysActive)
        => daysActive < BillingConstants.DaysInMonth ? amount * daysActive / BillingConstants.DaysInMonth : amount;

    public static decimal TenureDiscount(decimal prorated, int months) => months switch
    {
        >= LoyalTier2Months => prorated * LoyalTier2Rate,
        >= LoyalTier1Months => prorated * LoyalTier1Rate,
        _ => 0m,
    };

    public static decimal CouponDiscount(decimal prorated, Subscription s) => s.Coupon switch
    {
        Coupon.Welcome => prorated * (s.TenureMonths < 1 ? WelcomeNewRate : WelcomeRate),
        Coupon.Loyal when s.TenureMonths >= LoyalTier2Months => LoyalCouponTier2,
        Coupon.Loyal when s.TenureMonths >= LoyalTier1Months => LoyalCouponTier1,
        _ => 0m,
    };
}
