using System.Globalization;
using Day4.Lab2.Domain;

namespace Day4.Lab2.Services;

public sealed class ReportFormatter
{
    private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;

    public static string Money(decimal v, int width) => v.ToString("0.00", Inv).PadLeft(width);

    public string BillLine(Bill b)
    {
        var s = b.Subscription;
        var line =
            $"{s.Id} | {s.Customer,-12} | {PlanCode(s.Plan),-5} | addons {s.Addons,2} | days {s.DaysActive,2} | " +
            $"sub {Money(b.Subtotal, 7)} | pror {Money(b.Prorated, 7)} | disc {Money(b.Discount, 6)} | " +
            $"tax {Money(b.Tax, 6)} | TOTAL {Money(b.Total, 8)}";
        if (s.Coupon != Coupon.None) line += $" [{s.Coupon.ToString().ToUpperInvariant()}]";
        if (b.IsPartial) line += " (partial)";
        return line;
    }

    public string PlanLine(Plan plan, decimal total, decimal grand)
    {
        var pct = grand == 0m ? 0m : total / grand * 100m;
        return $"{PlanCode(plan),-6} : {Money(total, 9)}  ({pct.ToString("0.0", Inv)}%)";
    }

    public static string PlanCode(Plan plan) => plan.ToString().ToUpperInvariant();
}
