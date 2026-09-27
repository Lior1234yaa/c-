using System.Windows;
using System.Windows.Media;

namespace Day3.Exercises.Solutions.Windows;

public partial class Ex12Styles : Window
{
    private bool _dark;
    public Ex12Styles() => InitializeComponent();

    private void ToggleTheme_Click(object sender, RoutedEventArgs e)
    {
        _dark = !_dark;
        // החלפת המשאבים במקום: כל DynamicResource מתעדכן מיד
        Resources["Bg"] = new SolidColorBrush(_dark ? Color.FromRgb(0x1B, 0x13, 0x40) : Color.FromRgb(0xF4, 0xF5, 0xF9));
        Resources["Fg"] = new SolidColorBrush(_dark ? Color.FromRgb(0xF3, 0xF0, 0xFF) : Color.FromRgb(0x1F, 0x1B, 0x2E));
        Resources["Accent"] = new SolidColorBrush(_dark ? Color.FromRgb(0xFF, 0xB0, 0x20) : Color.FromRgb(0x51, 0x2B, 0xD4));
    }
}
