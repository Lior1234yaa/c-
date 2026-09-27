using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using System.Windows.Data;
using System.Windows.Input;
using Day3.Lab3.Starter.Models;
using Day3.Lab3.Starter.Mvvm;
using Day3.Lab3.Starter.Services;

namespace Day3.Lab3.Starter.ViewModels;

public class ProductsViewModel : ObservableObject
{
    private readonly IReadOnlyList<IProductService> _services;
    private IProductService _service;
    private CancellationTokenSource? _cts;
    private bool _isBusy;
    private int _progress;
    private string _status = "Ready";
    private string? _error;
    private string _search = "";
    private string? _category;
    private bool _onlyInStock;
    private Product? _selected;

    public ObservableCollection<Product> Products { get; } = [];
    public ICollectionView ProductsView { get; }
    public ObservableCollection<string> Categories { get; } = ["(all)"];
    public IReadOnlyList<IProductService> Services => _services;

    public IProductService Service { get => _service; set => SetProperty(ref _service, value); }

    public bool IsBusy { get => _isBusy; private set { if (SetProperty(ref _isBusy, value)) OnPropertyChanged(nameof(IsIdle)); } }
    public bool IsIdle => !IsBusy;
    public int Progress { get => _progress; private set => SetProperty(ref _progress, value); }
    public string Status { get => _status; private set => SetProperty(ref _status, value); }
    public string? Error { get => _error; private set { if (SetProperty(ref _error, value)) OnPropertyChanged(nameof(HasError)); } }
    public bool HasError => Error is not null;
    public Product? Selected { get => _selected; set => SetProperty(ref _selected, value); }

    // TODO 3: כשאחד משלושת המסננים משתנה — ProductsView.Refresh()
    public string Search { get => _search; set => SetProperty(ref _search, value); }
    public string? Category { get => _category; set => SetProperty(ref _category, value); }
    public bool OnlyInStock { get => _onlyInStock; set => SetProperty(ref _onlyInStock, value); }

    public int VisibleCount => ProductsView.Cast<object>().Count();

    public ICommand LoadCommand { get; }
    public ICommand CancelCommand { get; }
    public ICommand ClearFiltersCommand { get; }

    public ProductsViewModel(IReadOnlyList<IProductService> services)
    {
        _services = services;
        _service = services[0];

        ProductsView = CollectionViewSource.GetDefaultView(Products);
        // TODO 3: ProductsView.Filter = o => o is Product p && Matches(p);
        ProductsView.CollectionChanged += (_, _) => OnPropertyChanged(nameof(VisibleCount));

        LoadCommand = new AsyncRelayCommand(LoadAsync, () => !IsBusy);
        CancelCommand = new RelayCommand(() => _cts?.Cancel(), () => IsBusy);
        ClearFiltersCommand = new RelayCommand(() => { Search = ""; Category = null; OnlyInStock = false; });
    }

    private bool Matches(Product p) =>
        (string.IsNullOrWhiteSpace(Search) || p.Name.Contains(Search, StringComparison.OrdinalIgnoreCase))
        && (Category is null or "(all)" || p.Category == Category)
        && (!OnlyInStock || p.InStock);

    public async Task LoadAsync()
    {
        // TODO 1: טעינה אסינכרונית עם התקדמות:
        //   - ליצור CancellationTokenSource (אפשר עם timeout של 30 שניות)
        //   - IsBusy = true, Error = null, Progress = 0, Status = "Loading…"
        //   - var progress = new Progress<int>(p => Progress = p);
        //   - await Service.GetProductsAsync(progress, _cts.Token) → למלא Products, RebuildCategories(), Status
        // TODO 2: טיפול בשגיאות:
        //   - OperationCanceledException → Status = "Cancelled"
        //   - HttpRequestException → Error = הודעה ידידותית בעברית (+ ex.Message), Status = "Failed"
        //   - finally: IsBusy = false, לשחרר את ה-CTS
        var items = await Service.GetProductsAsync(null, CancellationToken.None);
        Products.Clear();
        foreach (var p in items) Products.Add(p);
        RebuildCategories();
        Status = $"Loaded {items.Count} products";
    }

    private void RebuildCategories()
    {
        var current = Category;
        Categories.Clear();
        Categories.Add("(all)");
        foreach (var c in Products.Select(p => p.Category).Distinct().Order()) Categories.Add(c);
        Category = Categories.Contains(current ?? "") ? current : "(all)";
    }
}
