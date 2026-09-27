using System.Globalization;
using Day4.Demo.Refactored.Domain;

namespace Day4.Demo.Refactored.Services;

/// <summary>עיצוב שורות הדוח. כל הפורמט במקום אחד.</summary>
public sealed class ReportFormatter
{
    private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;

    public static string Money(decimal value, int width) => value.ToString("0.00", Inv).PadLeft(width);

    public string OrderLine(PricedOrder p)
    {
        var o = p.Order;
        var line =
            $"Order {o.Id} | {o.Customer,-12} | {TypeCode(o.Type),-3} | " +
            $"{o.Quantity,3} x {Money(o.UnitPrice, 7)} | " +
            $"sub {Money(o.Subtotal, 8)} | disc {Money(p.Discount, 7)} | " +
            $"ship {Money(p.Shipping, 6)} | vat {Money(p.Vat, 7)} | TOTAL {Money(p.Total, 9)}";
        return o.Status == OrderStatus.Pending ? line + " (pending)" : line;
    }

    public string CancelledLine(Order o) => $"Order {o.Id} ({o.Customer}) - CANCELLED, skipped";

    public string CustomerLine(string customer, decimal total)
        => $"{customer,-12} : {Money(total, 10)}" + (total > 1000m ? "  *TOP*" : "");

    public string StatusLine(OrderStatus status, int count) => $"{StatusCode(status),-12} : {count,4}";

    public static string TypeCode(CustomerType t) => t switch
    {
        CustomerType.Vip => "VIP",
        CustomerType.Business => "BIZ",
        CustomerType.Regular => "REG",
        CustomerType.New => "NEW",
        _ => "???",
    };

    public static string StatusCode(OrderStatus s) => s.ToString().ToUpperInvariant();
}
