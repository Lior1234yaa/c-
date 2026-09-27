using System.Windows;
using System.Windows.Controls;

namespace Day3.Exercises.Solutions.Windows;

public partial class Ex02GridKeypad : Window
{
    public Ex02GridKeypad() => InitializeComponent();

    // handler אחד לכל הכפתורים דרך bubbling של Button.Click על ה-Grid
    private void Key_Click(object sender, RoutedEventArgs e)
    {
        if (e.OriginalSource is not Button b) return;
        switch (b.Content?.ToString())
        {
            case "C": Display.Text = ""; break;
            case "⌫": if (Display.Text.Length > 0) Display.Text = Display.Text[..^1]; break;
            case "OK": MessageBox.Show(this, $"Value: {Display.Text}", "Keypad"); break;
            case var key: Display.Text += key; break;
        }
    }
}
