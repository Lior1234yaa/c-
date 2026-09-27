using System.Windows;
using System.Windows.Input;
using Day3.Exercises.Solutions.Windows;

namespace Day3.Exercises.Solutions;

public record ExerciseInfo(int Number, string Title, string Stars, Func<Window> Create);

public partial class MainWindow : Window
{
    private static readonly ExerciseInfo[] Exercises =
    [
        new(1, "Hello XAML — TextBox, Button, x:Name", "★", () => new Ex01HelloXaml()),
        new(2, "Grid keypad — star sizing", "★", () => new Ex02GridKeypad()),
        new(3, "Login form — DockPanel/StackPanel, Label.Target", "★", () => new Ex03LoginForm()),
        new(4, "Counter — Click, KeyDown, Loaded, Closing", "★★", () => new Ex04CounterEvents()),
        new(5, "RelayCommand + InputBindings + CanExecute", "★★", () => new Ex05Commands()),
        new(6, "Stopwatch — DispatcherTimer", "★★", () => new Ex06Stopwatch()),
        new(7, "ElementName binding + StringFormat", "★", () => new Ex07ElementBinding()),
        new(8, "Shopping list — ObservableCollection + DataTemplate", "★★", () => new Ex08ShoppingList()),
        new(9, "IValueConverter — temperature to color", "★★", () => new Ex09Converter()),
        new(10, "Async load — progress + cancel", "★★", () => new Ex10AsyncLoad()),
        new(11, "ValidationRule + ErrorTemplate", "★★★", () => new Ex11Validation()),
        new(12, "Styles & theme swap — ResourceDictionary", "★★", () => new Ex12Styles()),
    ];

    public MainWindow()
    {
        InitializeComponent();
        List.ItemsSource = Exercises;
        List.SelectedIndex = 0;
    }

    private void Open_Click(object sender, RoutedEventArgs e) => OpenSelected();
    private void List_MouseDoubleClick(object sender, MouseButtonEventArgs e) => OpenSelected();

    private void OpenSelected()
    {
        if (List.SelectedItem is not ExerciseInfo ex) return;
        var w = ex.Create();
        w.Owner = this;
        w.Show();
    }
}
