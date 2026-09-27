using System.Windows;
using Day4.Demo.DiHostWpf.ViewModels;

namespace Day4.Demo.DiHostWpf;

public partial class MainWindow : Window
{
    // ה-ViewModel מוזרק ע"י ה-container; אין לוגיקה ב-code-behind.
    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
