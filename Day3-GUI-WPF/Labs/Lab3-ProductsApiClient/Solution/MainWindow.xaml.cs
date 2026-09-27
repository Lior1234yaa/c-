using System.Net.Http;
using System.Windows;
using Day3.Lab3.Solution.Services;
using Day3.Lab3.Solution.ViewModels;

namespace Day3.Lab3.Solution;

public partial class MainWindow : Window
{
    private static readonly HttpClient Http = new() { Timeout = TimeSpan.FromSeconds(15) };
    private readonly ProductsViewModel _vm;

    public MainWindow()
    {
        InitializeComponent();
        // composition root: כאן (ורק כאן) בוחרים מימושים קונקרטיים
        _vm = new ProductsViewModel(
        [
            new FakeProductService(),
            new FakeProductService { FailRandomly = true, DelayPerBatchMs = 400 } ,
            new HttpProductService(Http),                                            // Day2.LocalApi
            new HttpProductService(Http, "https://dummyjson.com/products?limit=40"),  // API ציבורי לדוגמה
        ]);
        DataContext = _vm;
    }

    private async void Window_Loaded(object sender, RoutedEventArgs e) => await _vm.LoadAsync();

    /// <summary>פריסה אדפטיבית: מתחת ל-700px עוברים מטבלה לכרטיסים.</summary>
    private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        var narrow = e.NewSize.Width < 700;
        ProductsGrid.Visibility = narrow ? Visibility.Collapsed : Visibility.Visible;
        CardsView.Visibility = narrow ? Visibility.Visible : Visibility.Collapsed;
    }
}
