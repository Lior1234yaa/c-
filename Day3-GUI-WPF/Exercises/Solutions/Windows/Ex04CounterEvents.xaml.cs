using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace Day3.Exercises.Solutions.Windows;

public partial class Ex04CounterEvents : Window
{
    private int _count;
    public Ex04CounterEvents() => InitializeComponent();

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        Hint.Text = "Use +/− buttons or ↑/↓ keys";
        Render();
    }

    private void Plus_Click(object sender, RoutedEventArgs e) => Change(+1);
    private void Minus_Click(object sender, RoutedEventArgs e) => Change(-1);

    private void Window_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Up) Change(+1);
        else if (e.Key == Key.Down) Change(-1);
        else return;
        e.Handled = true;
    }

    private void Change(int delta) { _count += delta; Render(); }
    private void Render() => CountText.Text = _count.ToString();

    private void Window_Closing(object sender, CancelEventArgs e)
    {
        if (_count == 0) return;
        e.Cancel = MessageBox.Show(this, $"Counter is {_count}. Close anyway?", "Closing",
                                   MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes;
    }
}
