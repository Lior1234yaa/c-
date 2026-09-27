namespace Day4.Lab1;

public enum CustomerType { Regular, Vip }

public sealed record OrderLine(string Sku, int Quantity, decimal UnitPrice);

public sealed record Order(CustomerType Type, IReadOnlyList<OrderLine> Lines, string? CouponCode = null)
{
    public decimal Subtotal => Lines.Sum(l => l.Quantity * l.UnitPrice);
    public int TotalQuantity => Lines.Sum(l => l.Quantity);
}

/// <summary>תוצאת חישוב ההנחה: הסכום והכלל שהופעל ("None" / "Volume" / "Amount" / "Vip", ואופציונלית "+Coupon").</summary>
public sealed record DiscountResult(decimal Amount, string AppliedRule);
