using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Day4.Demo.AiGeneratedUi.ViewModels;

public sealed record OrderRow(int Id, string Customer, DateTime Date, decimal Total, string Status);

/// <summary>ה-ViewModel שה-prompt תיאר. הנתונים כאן מדומים — במערכת אמיתית יגיעו משירות מוזרק.</summary>
public class OrdersDashboardViewModel : ObservableObject
{
    private static readonly IReadOnlyList<OrderRow> Sample =
    [
        new(1001, "Dana Levi", new DateTime(2025, 3, 1), 375.38m, "Paid"),
        new(1002, "Yossi Cohen", new DateTime(2025, 3, 1), 231.28m, "Paid"),
        new(1003, "ACME Ltd", new DateTime(2025, 3, 2), 419.64m, "Pending"),
        new(1004, "Dana Levi", new DateTime(2025, 3, 2), 1037.36m, "Paid"),
        new(1006, "Yossi Cohen", new DateTime(2025, 3, 3), 47.20m, "Pending"),
        new(1008, "Lior Katz", new DateTime(2025, 3, 4), 139.24m, "Paid"),
    ];

    private string _searchText = "";
    private string? _selectedStatus;
    private OrderRow? _selectedOrder;
    private bool _isBusy;
    private string _statusMessage = "";

    public OrdersDashboardViewModel()
    {
        Statuses = ["All", "Paid", "Pending", "Cancelled"];
        _selectedStatus = Statuses[0];
        Orders = [];
        RefreshCommand = new RelayCommand(Refresh, () => !IsBusy);
        ExportCommand = new RelayCommand(() => StatusMessage = "Export: not implemented in this demo", () => Orders.Count > 0);
        NewOrderCommand = new RelayCommand(() => StatusMessage = "New order: not implemented in this demo");
        Refresh();
    }

    public ObservableCollection<string> Statuses { get; }
    public ObservableCollection<OrderRow> Orders { get; }

    public ICommand RefreshCommand { get; }
    public ICommand ExportCommand { get; }
    public ICommand NewOrderCommand { get; }

    public string SearchText
    {
        get => _searchText;
        set { if (SetProperty(ref _searchText, value)) Refresh(); }
    }

    public string? SelectedStatus
    {
        get => _selectedStatus;
        set { if (SetProperty(ref _selectedStatus, value)) Refresh(); }
    }

    public OrderRow? SelectedOrder { get => _selectedOrder; set => SetProperty(ref _selectedOrder, value); }
    public bool IsBusy { get => _isBusy; set => SetProperty(ref _isBusy, value); }
    public string StatusMessage { get => _statusMessage; set => SetProperty(ref _statusMessage, value); }

    public int TodayOrders => Sample.Count(o => o.Date == Sample.Max(x => x.Date));
    public decimal TodayRevenue => Sample.Where(o => o.Date == Sample.Max(x => x.Date)).Sum(o => o.Total);
    public int PendingOrders => Sample.Count(o => o.Status == "Pending");
    public int ActiveCustomers => Sample.Select(o => o.Customer).Distinct().Count();

    private void Refresh()
    {
        IsBusy = true;
        var query = Sample.AsEnumerable();
        if (!string.IsNullOrWhiteSpace(SearchText))
            query = query.Where(o => o.Customer.Contains(SearchText, StringComparison.CurrentCultureIgnoreCase));
        if (SelectedStatus is not null && SelectedStatus != "All")
            query = query.Where(o => o.Status == SelectedStatus);

        Orders.Clear();
        foreach (var row in query.OrderByDescending(o => o.Date)) Orders.Add(row);

        StatusMessage = $"Loaded {Orders.Count} orders";
        IsBusy = false;
    }
}

/// <summary>נתוני עיצוב (design-time) — מוצגים במעצב של Visual Studio בלי להריץ.</summary>
public sealed class DesignOrdersDashboardViewModel : OrdersDashboardViewModel
{
    public DesignOrdersDashboardViewModel()
    {
        StatusMessage = "Design view — 6 sample orders";
    }
}
