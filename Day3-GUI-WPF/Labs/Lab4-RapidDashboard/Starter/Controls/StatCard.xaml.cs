using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Day3.Lab4.Starter.Controls;

/// <summary>UserControl לשימוש חוזר: כרטיס סטטיסטיקה.</summary>
public partial class StatCard : UserControl
{
    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(nameof(Title), typeof(string), typeof(StatCard), new PropertyMetadata("Title"));

    public string Title { get => (string)GetValue(TitleProperty); set => SetValue(TitleProperty, value); }

    // TODO 2a: להוסיף DependencyProperty ל-Value (string), Subtitle (string) ו-Accent (Brush)
    //          לפי אותה תבנית. בלי DependencyProperty אי אפשר לעשות Binding מבחוץ!

    public StatCard()
    {
        InitializeComponent();
    }
}
