using System.Windows;
using Day4.Lab4.Core.Services;
using Day4.Lab4.ViewModels;

namespace Day4.Lab4;

public partial class MainWindow : Window
{
    private readonly MainViewModel _viewModel;

    public MainWindow()
    {
        InitializeComponent();
        _viewModel = new MainViewModel(new JsonExpenseRepository(), TimeProvider.System);
        DataContext = _viewModel;
        Loaded += OnLoaded;
    }

    // event handler — המקום היחיד שבו async void מקובל; הלוגיקה עצמה ב-ViewModel.
    private async void OnLoaded(object sender, RoutedEventArgs e) => await _viewModel.LoadAsync();
}
