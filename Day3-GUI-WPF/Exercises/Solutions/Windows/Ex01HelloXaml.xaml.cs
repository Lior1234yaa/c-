using System.Windows;

namespace Day3.Exercises.Solutions.Windows;

public partial class Ex01HelloXaml : Window
{
    private int _clicks;
    public Ex01HelloXaml() { InitializeComponent(); NameBox.Focus(); }

    private void Greet_Click(object sender, RoutedEventArgs e)
    {
        _clicks++;
        var name = string.IsNullOrWhiteSpace(NameBox.Text) ? "World" : NameBox.Text.Trim();
        Output.Text = $"Hello, {name}! (#{_clicks})";
    }
}
