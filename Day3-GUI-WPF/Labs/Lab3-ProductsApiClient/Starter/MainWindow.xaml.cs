using System.Net.Http;
using System.Windows;
using Day3.Lab3.Starter.Services;
using Day3.Lab3.Starter.ViewModels;

namespace Day3.Lab3.Starter;

public partial class MainWindow : Window
{
    private static readonly HttpClient Http = new() { Timeout = TimeSpan.FromSeconds(15) };
    private readonly ProductsViewModel _vm;

    public MainWindow()
    {
        InitializeComponent();
        _vm = new ProductsViewModel(
        [
            new FakeProductService(),
            new FakeProductService { FailRandomly = true, DelayPerBatchMs = 400 },
            new HttpProductService(Http),   // Day2.LocalApi — http://localhost:5080/api/products
        ]);
        DataContext = _vm;
    }

    private async void Window_Loaded(object sender, RoutedEventArgs e) => await _vm.LoadAsync();

    // TODO 5b: SizeChanged — מתחת ל-700px להסתיר את ProductsGrid ולהציג את CardsView (ולהפך)
}
