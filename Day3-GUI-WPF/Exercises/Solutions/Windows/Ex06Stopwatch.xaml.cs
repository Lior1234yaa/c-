using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;

namespace Day3.Exercises.Solutions.Windows;

public partial class Ex06Stopwatch : Window
{
    // Stopwatch מודד זמן מדויק; DispatcherTimer רק "מצייר" כל 100ms
    private readonly Stopwatch _sw = new();
    private readonly DispatcherTimer _timer = new() { Interval = TimeSpan.FromMilliseconds(100) };
    private readonly List<string> _laps = [];

    public Ex06Stopwatch()
    {
        InitializeComponent();
        _timer.Tick += (_, _) => Render();
        Closed += (_, _) => _timer.Stop();
    }

    private void StartStop_Click(object sender, RoutedEventArgs e)
    {
        if (_sw.IsRunning) { _sw.Stop(); _timer.Stop(); StartStop.Content = "Start"; }
        else { _sw.Start(); _timer.Start(); StartStop.Content = "Stop"; }
    }

    private void Lap_Click(object sender, RoutedEventArgs e)
    {
        if (!_sw.IsRunning) return;
        _laps.Add(Format(_sw.Elapsed));
        Laps.Text = string.Join("   ", _laps.TakeLast(4));
    }

    private void Reset_Click(object sender, RoutedEventArgs e)
    {
        _sw.Reset(); _timer.Stop(); _laps.Clear();
        StartStop.Content = "Start"; Laps.Text = ""; Render();
    }

    private void Render() => TimeText.Text = Format(_sw.Elapsed);
    private static string Format(TimeSpan t) => $"{(int)t.TotalMinutes:00}:{t.Seconds:00}.{t.Milliseconds / 100}";
}
