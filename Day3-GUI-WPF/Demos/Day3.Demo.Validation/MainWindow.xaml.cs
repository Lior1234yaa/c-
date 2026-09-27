using System.IO;
using System.Windows;
using System.Windows.Input;
using Day3.Demo.Validation.Mvvm;
using Day3.Demo.Validation.ViewModels;
using Microsoft.Win32;

namespace Day3.Demo.Validation;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel();
    }

    /// <summary>מאפשר רק ספרות (ולא אותיות/רווחים). הדבקה עדיין אפשרית — לכן צריך גם ולידציה.</summary>
    private void Numeric_PreviewTextInput(object sender, TextCompositionEventArgs e) =>
        e.Handled = !e.Text.All(char.IsDigit);

    private void OpenFile_Click(object sender, RoutedEventArgs e)
    {
        var dlg = new OpenFileDialog { Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*", Title = "בחרו קובץ" };
        if (dlg.ShowDialog(this) == true)
            DialogResultText.Text = $"נבחר: {dlg.FileName} ({new FileInfo(dlg.FileName).Length} bytes)";
    }

    private void SaveFile_Click(object sender, RoutedEventArgs e)
    {
        var dlg = new SaveFileDialog { Filter = "JSON (*.json)|*.json", FileName = "export.json", DefaultExt = ".json" };
        if (dlg.ShowDialog(this) == true)
        {
            File.WriteAllText(dlg.FileName, """{ "saved": true }""");
            DialogResultText.Text = $"נשמר ל-{dlg.FileName}";
        }
    }

    private void CustomDialog_Click(object sender, RoutedEventArgs e)
    {
        var dlg = new NameDialog { Owner = this };   // Owner: ממורכז מעל החלון וחוסם אותו
        DialogResultText.Text = dlg.ShowDialog() == true ? $"שם הפרויקט: {dlg.ProjectName}" : "בוטל";
    }

    private void Delete_Click(object sender, RoutedEventArgs e)
    {
        var r = MessageBox.Show(this, "למחוק את הפריט? הפעולה אינה הפיכה.", "אישור מחיקה",
                                MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);
        DialogResultText.Text = r == MessageBoxResult.Yes ? "נמחק" : "המחיקה בוטלה";
    }
}

/// <summary>ה-VM של החלון: Quantity לטאב הראשון + Form לטאב השני.</summary>
public class MainViewModel : ObservableObject
{
    private int _quantity = 10;
    public int Quantity { get => _quantity; set => SetProperty(ref _quantity, value); }
    public PersonFormViewModel Form { get; } = new();
}
