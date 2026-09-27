using System.Windows;

namespace Day3.Demo.Validation;

public partial class NameDialog : Window
{
    public string ProjectName => NameBox.Text.Trim();

    public NameDialog()
    {
        InitializeComponent();
        NameBox.Focus();
    }

    private void Ok_Click(object sender, RoutedEventArgs e)
    {
        if (ProjectName.Length == 0) { NameBox.Focus(); return; }
        DialogResult = true;   // סוגר את הדיאלוג ומחזיר true מ-ShowDialog()
    }
}
