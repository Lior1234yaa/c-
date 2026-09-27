using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace Day3.Demo.Events;

public partial class MainWindow : Window
{
    // הרשימה נצפית: כל Add מעדכן את ה-ListBox אוטומטית
    public ObservableCollection<string> LogEntries { get; } = [];

    public ICommand SaveCommand { get; }
    public ICommand ClearLogCommand { get; }

    private readonly DispatcherTimer _timer = new() { Interval = TimeSpan.FromSeconds(1) };
    private bool _dirty;

    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;   // מאפשר {Binding SaveCommand} ו-{Binding LogEntries}

        SaveCommand = new RelayCommand(Save, () => _dirty);
        ClearLogCommand = new RelayCommand(LogEntries.Clear, () => LogEntries.Count > 0);

        // DispatcherTimer מפעיל את Tick על ה-UI thread — מותר לגעת בפקדים
        _timer.Tick += (_, _) => Clock.Text = DateTime.Now.ToString("HH:mm:ss");
    }

    private void Add(string text)
    {
        LogEntries.Add($"{DateTime.Now:HH:mm:ss.fff}  {text}");
        Log.ScrollIntoView(LogEntries[^1]);
    }

    // ---- lifecycle ----
    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        Add("Loaded — החלון מוצג, אפשר לטעון נתונים");
        _timer.Start();
        Input.Focus();
    }

    private void Window_Closing(object sender, CancelEventArgs e)
    {
        if (!_dirty) return;
        var r = MessageBox.Show("יש שינויים שלא נשמרו. לסגור בכל זאת?", "סגירה",
                                MessageBoxButton.YesNo, MessageBoxImage.Warning);
        e.Cancel = r != MessageBoxResult.Yes;   // ביטול הסגירה
    }

    // ---- routed events ----
    private void Outer_PreviewMouseDown(object sender, MouseButtonEventArgs e) =>
        Add($"Border.PreviewMouseDown (tunneling) source={(e.OriginalSource as FrameworkElement)?.Name}");

    private void ButtonA_Click(object sender, RoutedEventArgs e) =>
        Add("ButtonA.Click — sender=" + ((Button)sender).Name);

    private void ButtonB_Click(object sender, RoutedEventArgs e)
    {
        Add("ButtonB.Click — e.Handled = true, הבעבוע נעצר כאן");
        e.Handled = true;
    }

    private void Outer_ButtonClick(object sender, RoutedEventArgs e) =>
        Add($"Border got bubbled Click from {(e.OriginalSource as FrameworkElement)?.Name}");

    // ---- common control events ----
    private void Input_TextChanged(object sender, TextChangedEventArgs e)
    {
        _dirty = true;
        Add($"TextChanged: \"{Input.Text}\"");
    }

    private void Input_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter) { Add("Enter pressed → Save"); Save(); }
        else if (e.Key == Key.Escape) Input.Clear();
    }

    private void Colors_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var item = (ComboBoxItem?)Colors.SelectedItem;
        if (IsLoaded) Add($"SelectionChanged: {item?.Content}");
    }

    // ---- commands ----
    private void Save()
    {
        _dirty = false;
        Add("Saved ✔ (SaveCommand)");
        RelayCommand.Refresh();
    }
}
