using System.Collections.ObjectModel;
using Day3.Lab4.Starter.Mvvm;
using Day3.Lab4.Starter.Services;

namespace Day3.Lab4.Starter.ViewModels;

public record BarPoint(string Label, double Value, double Height);

public class DashboardViewModel : ObservableObject
{
    private readonly FakeMetricsService _metrics;
    private readonly Queue<int> _history = new();
    private MetricsSnapshot _snapshot = new(0, 0, 0, 0, DateTime.Now);
    private bool _isDark;
    private int _refreshSeconds = 1;
    private bool _isLive = true;

    public MetricsSnapshot Snapshot { get => _snapshot; private set => SetProperty(ref _snapshot, value); }
    public ObservableCollection<OrderRow> RecentOrders { get; } = [];
    public ObservableCollection<BarPoint> UsersHistory { get; } = [];

    public bool IsLive { get => _isLive; set => SetProperty(ref _isLive, value); }
    public int RefreshSeconds { get => _refreshSeconds; set => SetProperty(ref _refreshSeconds, Math.Clamp(value, 1, 10)); }

    public bool IsDark
    {
        get => _isDark;
        set { if (SetProperty(ref _isDark, value)) ThemeChanged?.Invoke(value ? "Dark" : "Light"); }
    }

    /// <summary>ה-VM לא מכיר את App; הוא רק מודיע, והחלון מחליף theme.</summary>
    public event Action<string>? ThemeChanged;

    public string LastUpdated => $"Updated {Snapshot.Time:HH:mm:ss}";

    public DashboardViewModel(FakeMetricsService metrics)
    {
        _metrics = metrics;
        for (var i = 0; i < 5; i++) Tick();
    }

    /// <summary>נקרא מה-DispatcherTimer בחלון — רץ על ה-UI thread, לכן מותר לגעת ב-ObservableCollection.</summary>
    public void Tick()
    {
        if (!IsLive) return;
        Snapshot = _metrics.Next();
        OnPropertyChanged(nameof(LastUpdated));

        if (_metrics.MaybeNewOrder() is { } order)
        {
            RecentOrders.Insert(0, order);
            while (RecentOrders.Count > 25) RecentOrders.RemoveAt(RecentOrders.Count - 1);
        }

        _history.Enqueue(Snapshot.ActiveUsers);
        while (_history.Count > 20) _history.Dequeue();
        var max = Math.Max(1, _history.Max());
        UsersHistory.Clear();
        foreach (var (v, i) in _history.Select((v, i) => (v, i)))
            UsersHistory.Add(new BarPoint(i.ToString(), v, 120.0 * v / max));   // גובה בפיקסלים לבר
    }
}
