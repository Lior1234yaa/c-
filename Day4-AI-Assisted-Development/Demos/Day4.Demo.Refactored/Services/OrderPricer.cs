using Day4.Demo.Refactored.Domain;

namespace Day4.Demo.Refactored.Services;

/// <summary>מרכיב את שלב התמחור: הנחה → משלוח → מע"מ.</summary>
public sealed class OrderPricer(IDiscountPolicy discounts, IShippingCalculator shipping, IVatCalculator vat)
{
    public PricedOrder Price(Order order)
    {
        var discount = discounts.Calculate(order);
        var afterDiscount = order.Subtotal - discount;
        var ship = shipping.Calculate(order.Country, afterDiscount);
        var tax = vat.Calculate(order.Country, afterDiscount + ship);
        return new PricedOrder(order, discount, ship, tax);
    }
}
