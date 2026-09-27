using System.ComponentModel;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using Day3.Demo.AsyncUi.Services;

namespace Day3.Demo.AsyncUi;

public partial class MainWindow : Window
{
    private readonly FakeWeatherService _service = new();   // בפרויקט אמיתי: IWeatherService דרך constructor
    private readonly AppSettings _settings = SettingsStore.Load();
    private CancellationTokenSource? _cts;
    private IReadOnlyList<WeatherReport> _all = [];

    private bool _isBusy;
    /// <summary>דגל IsBusy: מכבה/מדליק פקדים ומציג התקדמות. מרוכז במקום אחד.</summary>
    private bool IsBusy
    {
        get => _isBusy;
        set
        {
            _isBusy = value;
            LoadButton.IsEnabled = !value;
            CancelButton.IsEnabled = value;
            Progress.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
            if (value) { Progress.Value = 0; ErrorBanner.Visibility = Visibility.Collapsed; }
        }
    }

    public MainWindow()
    {
        InitializeComponent();
        FilterBox.Text = _settings.LastCityFilter;
        SimulateErrors.IsChecked = _settings.SimulateErrors;
        Width = _settings.WindowWidth;
    }

    // async void מותר *רק* ב-event handlers — ותמיד עם try/catch בפנים
    private async void Load_Click(object sender, RoutedEventArgs e)
    {
        _cts?.Cancel();
        _cts = new CancellationTokenSource();
        _service.FailRandomly = SimulateErrors.IsChecked == true;
        IsBusy = true;
        StatusText.Text = "Loading…";

        // Progress<T> נוצר על ה-UI thread ולכן ה-callback רץ עליו — מותר לעדכן פקדים
        var progress = new Progress<int>(p => Progress.Value = p);
        try
        {
            _all = await _service.GetReportsAsync(progress, _cts.Token);
            ApplyFilter();
            StatusText.Text = $"Loaded {_all.Count} reports at {DateTime.Now:T}";
        }
        catch (OperationCanceledException)
        {
            StatusText.Text = "Cancelled";
        }
        catch (HttpRequestException ex)
        {
            // שגיאה צפויה: הודעה ידידותית inline + כפתור Retry
            ShowError($"לא הצלחנו לטעון נתונים: {ex.Message}");
            StatusText.Text = "Failed";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void Cancel_Click(object sender, RoutedEventArgs e) => _cts?.Cancel();

    /// <summary>הדגמה של מה *לא* לעשות: Thread.Sleep על ה-UI thread מקפיא את החלון.</summary>
    private void Block_Click(object sender, RoutedEventArgs e)
    {
        StatusText.Text = "Blocking for 3 seconds… (try to move the window)";
        Thread.Sleep(3000);
        StatusText.Text = "Unblocked. That was bad UX.";
    }

    /// <summary>כשקוד רץ על thread אחר (Task.Run / ספרייה חיצונית) — חוזרים ל-UI דרך Dispatcher.</summary>
    private void BackgroundThread_Click(object sender, RoutedEventArgs e)
    {
        Task.Run(() =>
        {
            var result = HeavyComputation();
            // StatusText.Text = result;  ← היה זורק InvalidOperationException (cross-thread)
            Dispatcher.Invoke(() => StatusText.Text = result);
        });
    }

    private static string HeavyComputation()
    {
        Thread.Sleep(1500);
        return $"Computed on thread {Environment.CurrentManagedThreadId}, shown via Dispatcher";
    }

    private void Filter_TextChanged(object sender, TextChangedEventArgs e) => ApplyFilter();

    private void ApplyFilter()
    {
        var f = FilterBox.Text.Trim();
        Grid.ItemsSource = string.IsNullOrEmpty(f)
            ? _all
            : _all.Where(r => r.City.Contains(f, StringComparison.OrdinalIgnoreCase)).ToList();
    }

    private void ShowError(string message)
    {
        ErrorText.Text = message;
        ErrorBanner.Visibility = Visibility.Visible;
    }

    private void Window_Closing(object sender, CancelEventArgs e)
    {
        _cts?.Cancel();
        _settings.LastCityFilter = FilterBox.Text;
        _settings.SimulateErrors = SimulateErrors.IsChecked == true;
        _settings.WindowWidth = Width;
        try { SettingsStore.Save(_settings); }
        catch (Exception ex) { MessageBox.Show($"Could not save settings: {ex.Message}"); }
    }
}
