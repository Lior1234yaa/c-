using Day4.Demo.Refactored.Domain;

namespace Day4.Demo.Refactored.Services;

public interface IOrderSource
{
    IReadOnlyList<Order> Load();
}

public interface IDiscountPolicy
{
    decimal Calculate(Order order);
}

public interface IShippingCalculator
{
    decimal Calculate(string country, decimal amountAfterDiscount);
}

public interface IVatCalculator
{
    decimal Calculate(string country, decimal taxableAmount);
}
