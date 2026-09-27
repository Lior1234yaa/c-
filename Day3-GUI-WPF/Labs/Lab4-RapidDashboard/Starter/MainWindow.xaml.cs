using System.Windows;
using System.Windows.Threading;
using Day3.Lab4.Starter.Services;
using Day3.Lab4.Starter.ViewModels;

namespace Day3.Lab4.Starter;

public partial class MainWindow : Window
{
    private readonly DashboardViewModel _vm = new(new FakeMetricsService());

    public MainWindow()
    {
        InitializeComponent();
        DataContext = _vm;

        // TODO 3b: DispatcherTimer כל שנייה שקורא ל-_vm.Tick(); לעצור ב-Closed.
        // TODO 7 (בונוס): _vm.ThemeChanged += App.ApplyTheme;
    }
}
