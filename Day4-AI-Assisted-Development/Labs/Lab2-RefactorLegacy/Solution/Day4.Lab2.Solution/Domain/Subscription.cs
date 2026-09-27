namespace Day4.Lab2.Domain;

public enum Plan { Basic, Pro, Team }

public enum Coupon { None, Welcome, Loyal }

public sealed record Subscription(
    string Id,
    string Customer,
    Plan Plan,
    int Addons,
    int DaysActive,
    int TenureMonths,
    Coupon Coupon);

/// <summary>תוצאת התמחור של מנוי אחד לחודש.</summary>
public sealed record Bill(Subscription Subscription, decimal Subtotal, decimal Prorated, decimal Discount, decimal Tax)
{
    public decimal Total => Prorated - Discount + Tax;
    public bool IsPartial => Subscription.DaysActive < BillingConstants.DaysInMonth;
}

public static class BillingConstants
{
    public const int DaysInMonth = 30;
}
