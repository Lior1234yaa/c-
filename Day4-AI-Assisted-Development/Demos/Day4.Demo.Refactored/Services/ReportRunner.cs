using Day4.Demo.Refactored.Domain;

namespace Day4.Demo.Refactored.Services;

/// <summary>ה-use case: טוען, מתמחר, מסכם וכותב. בלי לדעת מאיפה הנתונים ולאן הפלט.</summary>
public sealed class ReportRunner(IOrderSource source, OrderPricer pricer, ReportFormatter fmt)
{
    public void Run(TextWriter output)
    {
        var orders = source.Load();
        var priced = new List<PricedOrder>();
        var byStatus = new SortedDictionary<string, int>(StringComparer.Ordinal);

        output.WriteLine("=== ORDER REPORT ===");
        output.WriteLine();

        foreach (var order in orders)
        {
            var statusKey = ReportFormatter.StatusCode(order.Status);
            byStatus[statusKey] = byStatus.GetValueOrDefault(statusKey) + 1;

            if (order.Status == OrderStatus.Cancelled)
            {
                output.WriteLine(fmt.CancelledLine(order));
                continue;
            }

            var p = pricer.Price(order);
            priced.Add(p);
            output.WriteLine(fmt.OrderLine(p));
        }

        output.WriteLine();
        output.WriteLine("--- Summary ---");
        output.WriteLine($"Orders processed: {priced.Count,4}");
        output.WriteLine($"Total discounts : {ReportFormatter.Money(priced.Sum(p => p.Discount), 10)}");
        output.WriteLine($"Total shipping  : {ReportFormatter.Money(priced.Sum(p => p.Shipping), 10)}");
        output.WriteLine($"Total VAT       : {ReportFormatter.Money(priced.Sum(p => p.Vat), 10)}");
        output.WriteLine($"Grand total     : {ReportFormatter.Money(priced.Sum(p => p.Total), 10)}");

        output.WriteLine();
        output.WriteLine("--- By customer ---");
        foreach (var g in priced.GroupBy(p => p.Order.Customer).OrderBy(g => g.Key, StringComparer.Ordinal))
            output.WriteLine(fmt.CustomerLine(g.Key, g.Sum(p => p.Total)));

        output.WriteLine();
        output.WriteLine("--- By status ---");
        foreach (var (status, count) in byStatus)
            output.WriteLine($"{status,-12} : {count,4}");

        output.WriteLine();
        // בגרסה הישנה נספרו שורות ה"לוג" (אחת לכל הזמנה + שורה ריקה בסוף). נשמר לתאימות פלט.
        output.WriteLine($"log lines: {orders.Count + 1}");
    }
}
