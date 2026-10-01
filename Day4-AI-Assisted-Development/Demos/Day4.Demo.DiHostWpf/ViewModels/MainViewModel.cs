using System.Collections.ObjectModel;
using System.Windows.Input;
using Day4.Demo.DiHostWpf.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Day4.Demo.DiHostWpf.ViewModels;

/// <summary>ה-ViewModel מקבל את כל התלויות בבנאי — ה-container בונה אותו.</summary>
public sealed class MainViewModel : ObservableObject
{
    private readonly OrderService _orders;
    private readonly ILogger<MainViewModel> _logger;

    private string _customer = "";
    private string _totalText = "";
    private string _status = "";

    public MainViewModel(OrderService orders, IOptions<AppOptions> options, ILogger<MainViewModel> logger)
    {
        _orders = orders;
        _logger = logger;
        Title = options.Value.Title;
        Currency = options.Value.Currency;
        Orders = new ObservableCollection<Order>(orders.GetOrders());
        AddCommand = new RelayCommand(Add, () => !string.IsNullOrWhiteSpace(Customer));
    }

    public string Title { get; }
    public string Currency { get; }
    public ObservableCollection<Order> Orders { get; }
    public ICommand AddCommand { get; }

    public string Customer { get => _customer; set => SetProperty(ref _customer, value); }
    public string TotalText { get => _totalText; set => SetProperty(ref _totalText, value); }
    public string Status { get => _status; set => SetProperty(ref _status, value); }

    private void Add()
    {
        if (!decimal.TryParse(TotalText, System.Globalization.NumberStyles.Number,
                System.Globalization.CultureInfo.InvariantCulture, out var total))
        {
            Status = "Invalid amount";
            return;
        }

        try
        {
            var order = _orders.Create(Customer, total);
            Orders.Add(order);
            Status = $"Added order #{order.Id}";
            Customer = "";
            TotalText = "";
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Validation failed");
            Status = ex.Message;
        }
    }
}
