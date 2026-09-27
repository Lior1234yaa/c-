using System.Windows;
using Day3.Demo.Binding.ViewModels;

namespace Day3.Demo.Binding;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel();   // כל ה-{Binding} בחלון מסתכלים על ה-VM
    }
}
