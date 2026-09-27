using System.Windows;
using System.Windows.Threading;
using Day3.Lab4.Solution.Services;
using Day3.Lab4.Solution.ViewModels;

namespace Day3.Lab4.Solution;

public partial class MainWindow : Window
{
    private readonly DashboardViewModel _vm = new(new FakeMetricsService());
    private readonly DispatcherTimer _timer = new() { Interval = TimeSpan.FromSeconds(1) };

    public MainWindow()
    {
        InitializeComponent();
        DataContext = _vm;

        _vm.ThemeChanged += App.ApplyTheme;
        _timer.Tick += (_, _) => _vm.Tick();   // Tick רץ על ה-UI thread
        _timer.Start();
        Closed += (_, _) => _timer.Stop();
    }

    private void ToggleTheme_Click(object sender, RoutedEventArgs e) => _vm.IsDark = !_vm.IsDark;

    private void Refresh_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (!IsLoaded) return;
        _timer.Interval = TimeSpan.FromSeconds(Math.Max(1, e.NewValue));
    }
}
