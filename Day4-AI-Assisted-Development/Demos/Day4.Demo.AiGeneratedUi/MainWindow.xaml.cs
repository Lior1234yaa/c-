using System.Windows;
using Day4.Demo.AiGeneratedUi.ViewModels;

namespace Day4.Demo.AiGeneratedUi;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new OrdersDashboardViewModel();
    }
}
