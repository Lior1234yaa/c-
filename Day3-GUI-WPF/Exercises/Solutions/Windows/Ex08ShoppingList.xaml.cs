using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using Day3.Exercises.Solutions.Mvvm;

namespace Day3.Exercises.Solutions.Windows;

public partial class Ex08ShoppingList : Window
{
    public Ex08ShoppingList() { InitializeComponent(); DataContext = new ShoppingViewModel(); }
}

public class ShoppingItem : ObservableObject
{
    private bool _isBought;
    public required string Name { get; init; }
    public decimal Price { get; init; }
    public bool IsBought { get => _isBought; set => SetProperty(ref _isBought, value); }
}

public class ShoppingViewModel : ObservableObject
{
    private string _newName = "";
    private string _newPrice = "";
    private ShoppingItem? _selected;

    public ObservableCollection<ShoppingItem> Items { get; } =
    [
        new() { Name = "Milk", Price = 6.9m },
        new() { Name = "Bread", Price = 12.5m },
    ];

    public string NewName { get => _newName; set => SetProperty(ref _newName, value); }
    public string NewPrice { get => _newPrice; set => SetProperty(ref _newPrice, value); }
    public ShoppingItem? Selected { get => _selected; set => SetProperty(ref _selected, value); }
    public decimal Total => Items.Sum(i => i.Price);

    public ICommand AddCommand { get; }
    public ICommand RemoveCommand { get; }

    public ShoppingViewModel()
    {
        AddCommand = new RelayCommand(Add, () => NewName.Trim().Length > 0 && TryPrice(out _));
        RemoveCommand = new RelayCommand(() => { Items.Remove(Selected!); Selected = null; }, () => Selected is not null);
        Items.CollectionChanged += (_, _) => OnPropertyChanged(nameof(Total));
    }

    private bool TryPrice(out decimal p) =>
        decimal.TryParse(NewPrice, NumberStyles.Number, CultureInfo.InvariantCulture, out p) && p >= 0;

    private void Add()
    {
        TryPrice(out var p);
        Items.Add(new ShoppingItem { Name = NewName.Trim(), Price = p });
        NewName = ""; NewPrice = "";
    }
}
