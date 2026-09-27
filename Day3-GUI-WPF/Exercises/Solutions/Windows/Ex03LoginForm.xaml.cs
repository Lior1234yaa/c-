using System.Windows;

namespace Day3.Exercises.Solutions.Windows;

public partial class Ex03LoginForm : Window
{
    public Ex03LoginForm() { InitializeComponent(); UserBox.Focus(); }

    private void Login_Click(object sender, RoutedEventArgs e)
    {
        if (UserBox.Text.Trim().Length == 0) { Status.Text = "User name is required"; UserBox.Focus(); return; }
        if (PassBox.Password.Length < 4) { Status.Text = "Password must be at least 4 characters"; PassBox.Focus(); return; }
        Status.Foreground = System.Windows.Media.Brushes.SeaGreen;
        Status.Text = $"Welcome, {UserBox.Text}!";
    }
}
