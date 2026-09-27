namespace Day4.Demo.Refactored.Domain;

public enum CustomerType { Regular, Vip, Business, New }

public enum OrderStatus { Paid, Pending, Cancelled }

/// <summary>הזמנה גולמית כפי שהגיעה מהמקור (CSV).</summary>
public sealed record Order(
    int Id,
    string Customer,
    CustomerType Type,
    int Quantity,
    decimal UnitPrice,
    string Country,
    OrderStatus Status,
    DateOnly Date)
{
    public decimal Subtotal => Quantity * UnitPrice;
}

/// <summary>הזמנה אחרי תמחור: הנחה, משלוח, מע"מ וסה"כ.</summary>
public sealed record PricedOrder(Order Order, decimal Discount, decimal Shipping, decimal Vat)
{
    public decimal AfterDiscount => Order.Subtotal - Discount;
    public decimal Total => AfterDiscount + Shipping + Vat;
}
