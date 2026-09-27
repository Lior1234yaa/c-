using System.Windows;

namespace Day3.Exercises.Solutions.Windows;

public partial class Ex10AsyncLoad : Window
{
    private CancellationTokenSource? _cts;

    public Ex10AsyncLoad() => InitializeComponent();

    private async void Load_Click(object sender, RoutedEventArgs e)
    {
        _cts = new CancellationTokenSource();
        LoadButton.IsEnabled = false; CancelButton.IsEnabled = true;
        Results.Items.Clear(); Progress.Value = 0; Status.Text = "Loading…";
        var progress = new Progress<int>(p => Progress.Value = p);
        try
        {
            var items = await FakeLoadAsync(progress, _cts.Token);
            foreach (var i in items) Results.Items.Add(i);
            Status.Text = $"Done: {items.Count} items";
        }
        catch (OperationCanceledException) { Status.Text = "Cancelled"; }
        finally
        {
            LoadButton.IsEnabled = true; CancelButton.IsEnabled = false;
            _cts.Dispose(); _cts = null;
        }
    }

    private void Cancel_Click(object sender, RoutedEventArgs e) => _cts?.Cancel();

    // "שירות" מזויף — בפרויקט אמיתי זה HttpClient
    private static async Task<List<string>> FakeLoadAsync(IProgress<int> progress, CancellationToken ct)
    {
        var list = new List<string>();
        for (var i = 1; i <= 10; i++)
        {
            await Task.Delay(300, ct);
            list.Add($"Item {i} loaded at {DateTime.Now:HH:mm:ss.f}");
            progress.Report(i * 10);
        }
        return list;
    }
}
