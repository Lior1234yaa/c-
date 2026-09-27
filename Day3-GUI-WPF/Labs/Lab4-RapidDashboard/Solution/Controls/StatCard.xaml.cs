using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Day3.Lab4.Solution.Controls;

/// <summary>
/// UserControl לשימוש חוזר: כרטיס סטטיסטיקה. ה-properties הן DependencyProperty כדי שאפשר יהיה
/// לעשות עליהן Binding מבחוץ (למשל Value="{Binding Snapshot.OrdersToday}").
/// </summary>
public partial class StatCard : UserControl
{
    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(nameof(Title), typeof(string), typeof(StatCard), new PropertyMetadata("Title"));
    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register(nameof(Value), typeof(string), typeof(StatCard), new PropertyMetadata("0"));
    public static readonly DependencyProperty SubtitleProperty =
        DependencyProperty.Register(nameof(Subtitle), typeof(string), typeof(StatCard), new PropertyMetadata(""));
    public static readonly DependencyProperty AccentProperty =
        DependencyProperty.Register(nameof(Accent), typeof(Brush), typeof(StatCard), new PropertyMetadata(Brushes.SlateBlue));

    public string Title { get => (string)GetValue(TitleProperty); set => SetValue(TitleProperty, value); }
    public string Value { get => (string)GetValue(ValueProperty); set => SetValue(ValueProperty, value); }
    public string Subtitle { get => (string)GetValue(SubtitleProperty); set => SetValue(SubtitleProperty, value); }
    public Brush Accent { get => (Brush)GetValue(AccentProperty); set => SetValue(AccentProperty, value); }

    public StatCard()
    {
        InitializeComponent();
    }
}
