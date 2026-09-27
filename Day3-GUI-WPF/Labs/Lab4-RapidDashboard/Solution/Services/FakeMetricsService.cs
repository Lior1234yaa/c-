namespace Day3.Lab4.Solution.Services;

public record MetricsSnapshot(int OrdersToday, decimal RevenueToday, int ActiveUsers, int Errors, DateTime Time);
public record OrderRow(int Id, string Customer, decimal Amount, string Status, DateTime Time);

/// <summary>מקור נתונים "חי" מזויף: כל קריאה ל-Next() מזיזה את המספרים קצת, כמו מערכת אמיתית.</summary>
public class FakeMetricsService
{
    private static readonly string[] Customers = ["Dana", "Yossi", "Noa", "Amir", "Lior", "Maya", "Tom", "Shira"];
    private static readonly string[] Statuses = ["Paid", "Paid", "Paid", "Pending", "Refunded"];
    private readonly Random _rng = new();
    private int _orders = 120, _users = 38, _errors = 2, _nextOrderId = 1000;
    private decimal _revenue = 18_450m;

    public MetricsSnapshot Next()
    {
        var newOrders = _rng.Next(0, 4);
        _orders += newOrders;
        _revenue += newOrders * (decimal)(_rng.NextDouble() * 300 + 40);
        _users = Math.Clamp(_users + _rng.Next(-3, 4), 5, 200);
        if (_rng.Next(10) == 0) _errors++;
        return new MetricsSnapshot(_orders, Math.Round(_revenue, 2), _users, _errors, DateTime.Now);
    }

    public OrderRow? MaybeNewOrder() =>
        _rng.Next(3) == 0
            ? new OrderRow(_nextOrderId++, Customers[_rng.Next(Customers.Length)],
                           Math.Round((decimal)(_rng.NextDouble() * 300 + 40), 2), Statuses[_rng.Next(Statuses.Length)], DateTime.Now)
            : null;
}
