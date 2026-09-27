using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using System.Windows.Data;
using System.Windows.Input;
using Day3.Lab3.Solution.Models;
using Day3.Lab3.Solution.Mvvm;
using Day3.Lab3.Solution.Services;

namespace Day3.Lab3.Solution.ViewModels;

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

    public IProductService Service
    {
        get => _service;
        set => SetProperty(ref _service, value);
    }

    public bool IsBusy { get => _isBusy; private set { if (SetProperty(ref _isBusy, value)) OnPropertyChanged(nameof(IsIdle)); } }
    public bool IsIdle => !IsBusy;
    public int Progress { get => _progress; private set => SetProperty(ref _progress, value); }
    public string Status { get => _status; private set => SetProperty(ref _status, value); }
    public string? Error { get => _error; private set { if (SetProperty(ref _error, value)) OnPropertyChanged(nameof(HasError)); } }
    public bool HasError => Error is not null;
    public Product? Selected { get => _selected; set => SetProperty(ref _selected, value); }

    public string Search { get => _search; set { if (SetProperty(ref _search, value)) ProductsView.Refresh(); } }
    public string? Category { get => _category; set { if (SetProperty(ref _category, value)) ProductsView.Refresh(); } }
    public bool OnlyInStock { get => _onlyInStock; set { if (SetProperty(ref _onlyInStock, value)) ProductsView.Refresh(); } }

    public int VisibleCount => ProductsView.Cast<object>().Count();
    public decimal VisibleTotal => ProductsView.Cast<Product>().Sum(p => p.Price * p.Stock);

    public ICommand LoadCommand { get; }
    public ICommand CancelCommand { get; }
    public ICommand ClearFiltersCommand { get; }

    public ProductsViewModel(IReadOnlyList<IProductService> services)
    {
        _services = services;
        _service = services[0];

        ProductsView = CollectionViewSource.GetDefaultView(Products);
        ProductsView.Filter = o => o is Product p && Matches(p);
        ProductsView.CollectionChanged += (_, _) => { OnPropertyChanged(nameof(VisibleCount)); OnPropertyChanged(nameof(VisibleTotal)); };

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
        _cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));   // timeout כללי
        IsBusy = true; Error = null; Progress = 0;
        Status = $"Loading from {Service.Name}…";
        var progress = new Progress<int>(p => Progress = p);

        try
        {
            var items = await Service.GetProductsAsync(progress, _cts.Token);
            Products.Clear();
            foreach (var p in items) Products.Add(p);
            RebuildCategories();
            Status = $"Loaded {items.Count} products at {DateTime.Now:T}";
        }
        catch (OperationCanceledException)
        {
            Status = "Cancelled";
        }
        catch (HttpRequestException ex)
        {
            Error = $"לא ניתן לטעון מוצרים מהשרת ({ex.StatusCode?.ToString() ?? "network"}). בדקו שהשרת רץ ונסו שוב.\n{ex.Message}";
            Status = "Failed";
        }
        catch (Exception ex)
        {
            Error = $"שגיאה לא צפויה: {ex.Message}";
            Status = "Failed";
        }
        finally
        {
            IsBusy = false;
            _cts.Dispose();
            _cts = null;
        }
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
