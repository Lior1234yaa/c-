using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Day4.Demo.DiHostWpf.Services;

public sealed record Order(int Id, string Customer, decimal Total, DateTimeOffset CreatedAt);

public interface IOrderRepository
{
    IReadOnlyList<Order> GetAll();
    void Add(Order order);
}

/// <summary>מימוש בזיכרון — בפרויקט אמיתי יוחלף ב-JSON/DB בלי לגעת ב-OrderService.</summary>
public sealed class InMemoryOrderRepository : IOrderRepository
{
    private readonly List<Order> _orders =
    [
        new(1, "Dana Levi", 375.38m, new DateTimeOffset(2025, 3, 1, 9, 0, 0, TimeSpan.Zero)),
        new(2, "ACME Ltd", 419.64m, new DateTimeOffset(2025, 3, 2, 10, 30, 0, TimeSpan.Zero)),
    ];

    public IReadOnlyList<Order> GetAll() => _orders.AsReadOnly();
    public void Add(Order order) => _orders.Add(order);
}

/// <summary>שכבת Application: לוגיקה שמשתמשת ב-repository, בשעון, באפשרויות ובלוגר — כולם מוזרקים.</summary>
public sealed class OrderService(
    IOrderRepository repository,
    IClock clock,
    IOptions<AppOptions> options,
    ILogger<OrderService> logger)
{
    public IReadOnlyList<Order> GetOrders() => repository.GetAll();

    public Order Create(string customer, decimal total)
    {
        if (string.IsNullOrWhiteSpace(customer))
            throw new ArgumentException("Customer is required.", nameof(customer));
        if (total < 0)
            throw new ArgumentOutOfRangeException(nameof(total), "Total cannot be negative.");

        var id = repository.GetAll().Count == 0 ? 1 : repository.GetAll().Max(o => o.Id) + 1;
        var order = new Order(id, customer.Trim(), total, clock.UtcNow);
        repository.Add(order);

        logger.LogInformation("Created order {OrderId} for {Customer} ({Total} {Currency})",
            order.Id, order.Customer, order.Total, options.Value.Currency);
        return order;
    }
}
