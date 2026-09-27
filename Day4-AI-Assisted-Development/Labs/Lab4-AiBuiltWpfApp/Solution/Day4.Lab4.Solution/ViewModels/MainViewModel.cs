using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Windows.Input;
using Day4.Lab4.Core.Models;
using Day4.Lab4.Core.Services;

namespace Day4.Lab4.ViewModels;

public sealed record CategorySummary(string Category, decimal Amount, double Percent);

public sealed class MainViewModel : ObservableObject
{
    private const string AllFilter = "הכול";

    private readonly IExpenseRepository _repository;
    private readonly TimeProvider _clock;
    private readonly List<Expense> _all = [];

    private string _selectedMonth = AllFilter;
    private string _selectedCategory = AllFilter;
    private Expense? _selectedExpense;
    private bool _isBusy;
    private string _statusMessage = "";
    private string? _errorMessage;

    public MainViewModel(IExpenseRepository repository, TimeProvider clock)
    {
        _repository = repository;
        _clock = clock;
        Form = new ExpenseFormViewModel(clock);

        AddCommand = new AsyncRelayCommand(AddAsync, () => Form.IsValid);
        DeleteCommand = new AsyncRelayCommand(DeleteAsync, () => SelectedExpense is not null);
        SaveCommand = new AsyncRelayCommand(SaveAsync);
        LoadCommand = new AsyncRelayCommand(LoadAsync);
    }

    public ExpenseFormViewModel Form { get; }
    public ObservableCollection<Expense> Expenses { get; } = [];
    public ObservableCollection<string> Months { get; } = [AllFilter];
    public ObservableCollection<string> CategoryFilters { get; } = [AllFilter, .. Categories.All];
    public ObservableCollection<CategorySummary> ByCategory { get; } = [];

    public ICommand AddCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand LoadCommand { get; }

    public string SelectedMonth { get => _selectedMonth; set { if (SetProperty(ref _selectedMonth, value)) ApplyFilter(); } }
    public string SelectedCategory { get => _selectedCategory; set { if (SetProperty(ref _selectedCategory, value)) ApplyFilter(); } }
    public Expense? SelectedExpense { get => _selectedExpense; set => SetProperty(ref _selectedExpense, value); }
    public bool IsBusy { get => _isBusy; set => SetProperty(ref _isBusy, value); }
    public string StatusMessage { get => _statusMessage; set => SetProperty(ref _statusMessage, value); }
    public string? ErrorMessage { get => _errorMessage; set { if (SetProperty(ref _errorMessage, value)) OnPropertyChanged(nameof(HasError)); } }
    public bool HasError => ErrorMessage is not null;

    public decimal FilteredTotal => Expenses.Sum(e => e.Amount);
    public decimal AveragePerDay
    {
        get
        {
            if (Expenses.Count == 0) return 0m;
            if (SelectedMonth == AllFilter) return FilteredTotal / Math.Max(1, Expenses.Select(e => e.Date).Distinct().Count());
            var first = Expenses[0].Date;
            return FilteredTotal / DateTime.DaysInMonth(first.Year, first.Month);
        }
    }

    public async Task LoadAsync()
    {
        await RunAsync(async () =>
        {
            _all.Clear();
            _all.AddRange(await _repository.LoadAsync());
            RebuildMonths();
            ApplyFilter();
            StatusMessage = $"נטענו {_all.Count} הוצאות";
        });
    }

    private async Task AddAsync()
    {
        if (!Form.IsValid) return;
        var expense = Form.ToExpense();
        _all.Add(expense);
        Form.Reset();
        RebuildMonths();
        ApplyFilter();
        await SaveAsync();
        StatusMessage = $"נוספה הוצאה: {expense.Amount.ToString("N2", CultureInfo.CurrentCulture)} ₪ ({expense.Category})";
    }

    private async Task DeleteAsync()
    {
        if (SelectedExpense is null) return;
        _all.RemoveAll(e => e.Id == SelectedExpense.Id);
        SelectedExpense = null;
        RebuildMonths();
        ApplyFilter();
        await SaveAsync();
        StatusMessage = "ההוצאה נמחקה";
    }

    private Task SaveAsync() => RunAsync(async () =>
    {
        await _repository.SaveAsync(_all.OrderByDescending(e => e.Date).ToList());
        StatusMessage = "נשמר";
    });

    /// <summary>עוטף כל פעולה אסינכרונית: IsBusy, ניקוי/הצגת שגיאה — בלי לקרוס.</summary>
    private async Task RunAsync(Func<Task> action)
    {
        IsBusy = true;
        ErrorMessage = null;
        try { await action(); }
        catch (Exception ex) when (ex is IOException or InvalidDataException or UnauthorizedAccessException)
        {
            ErrorMessage = ex.Message;
        }
        finally { IsBusy = false; }
    }

    private void RebuildMonths()
    {
        var months = _all.Select(e => MonthKey(e.Date)).Distinct().OrderByDescending(m => m).ToList();
        var keep = months.Contains(SelectedMonth) ? SelectedMonth : AllFilter;
        Months.Clear();
        Months.Add(AllFilter);
        foreach (var m in months) Months.Add(m);
        SelectedMonth = keep;
    }

    private void ApplyFilter()
    {
        IEnumerable<Expense> q = _all;
        if (SelectedMonth != AllFilter) q = q.Where(e => MonthKey(e.Date) == SelectedMonth);
        if (SelectedCategory != AllFilter) q = q.Where(e => e.Category == SelectedCategory);

        Expenses.Clear();
        foreach (var e in q.OrderByDescending(e => e.Date)) Expenses.Add(e);

        ByCategory.Clear();
        var total = FilteredTotal;
        foreach (var g in Expenses.GroupBy(e => e.Category).OrderByDescending(g => g.Sum(e => e.Amount)))
        {
            var sum = g.Sum(e => e.Amount);
            ByCategory.Add(new CategorySummary(g.Key, sum, total == 0m ? 0 : (double)(sum / total * 100m)));
        }

        OnPropertyChanged(nameof(FilteredTotal));
        OnPropertyChanged(nameof(AveragePerDay));
    }

    private static string MonthKey(DateOnly d) => d.ToString("yyyy-MM", CultureInfo.InvariantCulture);
}
