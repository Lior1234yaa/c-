using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Day4.Demo.AiGeneratedUi.ViewModels;

public sealed record OrderRow(int Id, string Customer, DateTime Date, decimal Total, string Status);

/// <summary>ה-ViewModel שה-prompt תיאר. הנתונים כאן מדומים — במערכת אמיתית יגיעו משירות מוזרק.</summary>
public class OrdersDashboardViewModel : ObservableObject
{
    private static readonly IReadOnlyList<OrderRow> Sample =
    [
        new(1001, "דנה לוי", new DateTime(2025, 3, 1), 375.38m, "שולם"),
        new(1002, "יוסי כהן", new DateTime(2025, 3, 1), 231.28m, "שולם"),
        new(1003, "ACME Ltd", new DateTime(2025, 3, 2), 419.64m, "ממתין"),
        new(1004, "דנה לוי", new DateTime(2025, 3, 2), 1037.36m, "שולם"),
        new(1006, "יוסי כהן", new DateTime(2025, 3, 3), 47.20m, "ממתין"),
        new(1008, "ליאור כץ", new DateTime(2025, 3, 4), 139.24m, "שולם"),
    ];

    private string _searchText = "";
    private string? _selectedStatus;
    private OrderRow? _selectedOrder;
    private bool _isBusy;
    private string _statusMessage = "";

    public OrdersDashboardViewModel()
    {
        Statuses = ["הכול", "שולם", "ממתין", "בוטל"];
        _selectedStatus = Statuses[0];
        Orders = [];
        RefreshCommand = new RelayCommand(Refresh, () => !IsBusy);
        ExportCommand = new RelayCommand(() => StatusMessage = "ייצוא: לא ממומש בדמו", () => Orders.Count > 0);
        NewOrderCommand = new RelayCommand(() => StatusMessage = "הזמנה חדשה: לא ממומש בדמו");
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
    public int PendingOrders => Sample.Count(o => o.Status == "ממתין");
    public int ActiveCustomers => Sample.Select(o => o.Customer).Distinct().Count();

    private void Refresh()
    {
        IsBusy = true;
        var query = Sample.AsEnumerable();
        if (!string.IsNullOrWhiteSpace(SearchText))
            query = query.Where(o => o.Customer.Contains(SearchText, StringComparison.CurrentCultureIgnoreCase));
        if (SelectedStatus is not null && SelectedStatus != "הכול")
            query = query.Where(o => o.Status == SelectedStatus);

        Orders.Clear();
        foreach (var row in query.OrderByDescending(o => o.Date)) Orders.Add(row);

        StatusMessage = $"נטענו {Orders.Count} הזמנות";
        IsBusy = false;
    }
}

/// <summary>נתוני עיצוב (design-time) — מוצגים במעצב של Visual Studio בלי להריץ.</summary>
public sealed class DesignOrdersDashboardViewModel : OrdersDashboardViewModel
{
    public DesignOrdersDashboardViewModel()
    {
        StatusMessage = "תצוגת עיצוב — 6 הזמנות לדוגמה";
    }
}
