using System.Globalization;
using System.Net.Mail;
using System.Windows;
using System.Windows.Controls;
using Day3.Exercises.Solutions.Mvvm;

namespace Day3.Exercises.Solutions.Windows;

public partial class Ex11Validation : Window
{
    private readonly EmailViewModel _vm = new();
    public Ex11Validation() { InitializeComponent(); DataContext = _vm; }

    private void Subscribe_Click(object sender, RoutedEventArgs e) => Status.Text = $"Subscribed: {_vm.Email}";
}

public class EmailViewModel : ObservableObject
{
    private string _email = "";
    public string Email { get => _email; set => SetProperty(ref _email, value); }
}

public class EmailRule : ValidationRule
{
    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        var s = value as string ?? "";
        if (s.Length == 0) return new ValidationResult(false, "Email is required");
        return MailAddress.TryCreate(s, out _) ? ValidationResult.ValidResult : new ValidationResult(false, "Invalid email address");
    }
}
