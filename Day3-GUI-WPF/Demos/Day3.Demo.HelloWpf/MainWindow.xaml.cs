using System.Windows;
using System.Windows.Input;

namespace Day3.Demo.HelloWpf;

/// <summary>
/// ה-code-behind של MainWindow.xaml. המחלקה partial: החלק השני נוצר
/// אוטומטית מה-XAML (MainWindow.g.cs) ומכיל את InitializeComponent והשדות NameBox, Greeting.
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();   // חובה! טוען את ה-XAML ומחבר את האירועים
        NameBox.Focus();
    }

    private void Hello_Click(object sender, RoutedEventArgs e)
    {
        Greeting.Text = Greeter.Greet(NameBox.Text);
    }

    private void Clear_Click(object sender, RoutedEventArgs e)
    {
        NameBox.Clear();
        Greeting.Text = string.Empty;
        NameBox.Focus();
    }

    private void NameBox_KeyDown(object sender, KeyEventArgs e)
    {
        // Enter כבר מטופל ע"י IsDefault="True" על הכפתור; Escape מנקה.
        if (e.Key == Key.Escape) Clear_Click(sender, e);
    }
}
