using Day4.Demo.Refactored.Domain;

namespace Day4.Demo.Refactored.Services;

/// <summary>כללי ההנחה — אותה התנהגות כמו הגרסה הישנה, בלי מספרי קסם.</summary>
public sealed class DiscountPolicy : IDiscountPolicy
{
    private const decimal VipHighThreshold = 1000m, VipHighRate = 0.15m;
    private const decimal VipMidThreshold = 250m, VipMidRate = 0.12m, VipBaseRate = 0.10m;
    private const int BusinessBulkQty = 50, BusinessMinQty = 10;
    private const decimal BusinessBulkRate = 0.20m, BusinessRate = 0.10m;
    private const decimal RegularThreshold = 250m; private const int RegularMinQty = 10;
    private const decimal RegularRate = 0.05m;
    private const decimal NewCustomerRate = 0.07m, NewCustomerCap = 30m;

    public decimal Calculate(Order order)
    {
        var sub = order.Subtotal;
        return order.Type switch
        {
            CustomerType.Vip when sub > VipHighThreshold => sub * VipHighRate,
            CustomerType.Vip when sub > VipMidThreshold => sub * VipMidRate,
            CustomerType.Vip => sub * VipBaseRate,

            CustomerType.Business when order.Quantity >= BusinessBulkQty => sub * BusinessBulkRate,
            CustomerType.Business when order.Quantity >= BusinessMinQty => sub * BusinessRate,
            CustomerType.Business => 0m,

            CustomerType.Regular when sub > RegularThreshold || order.Quantity >= RegularMinQty => sub * RegularRate,
            CustomerType.Regular => 0m,

            CustomerType.New => Math.Min(sub * NewCustomerRate, NewCustomerCap),
            _ => 0m,
        };
    }
}
