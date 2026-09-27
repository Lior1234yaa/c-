using Day4.Lab2.Domain;

namespace Day4.Lab2.Services;

/// <summary>ה-use case: טוען, מתמחר, מסכם וכותב ל-TextWriter (קונסול, קובץ או StringWriter בבדיקה).</summary>
public sealed class BillingReport(ISubscriptionSource source, IPricingService pricing, ReportFormatter fmt)
{
    public void Write(TextWriter output)
    {
        var bills = source.Load().Select(pricing.Price).ToList();

        output.WriteLine("=== MONTHLY BILLING ===");
        output.WriteLine();
        foreach (var bill in bills) output.WriteLine(fmt.BillLine(bill));

        var grand = bills.Sum(b => b.Total);
        output.WriteLine();
        output.WriteLine("--- Totals ---");
        output.WriteLine($"Subscriptions : {bills.Count,4}");
        output.WriteLine($"Discounts     : {ReportFormatter.Money(bills.Sum(b => b.Discount), 9)}");
        output.WriteLine($"Tax           : {ReportFormatter.Money(bills.Sum(b => b.Tax), 9)}");
        output.WriteLine($"Grand total   : {ReportFormatter.Money(grand, 9)}");

        output.WriteLine();
        output.WriteLine("--- By plan ---");
        foreach (var g in bills.GroupBy(b => b.Subscription.Plan).OrderBy(g => ReportFormatter.PlanCode(g.Key), StringComparer.Ordinal))
            output.WriteLine(fmt.PlanLine(g.Key, g.Sum(b => b.Total), grand));
    }
}
