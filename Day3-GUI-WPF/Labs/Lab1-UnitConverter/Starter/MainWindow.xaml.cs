using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Day3.Lab1.Starter;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        // TODO 4: למלא את ה-ComboBox-ים מ-LengthUnit.All ולבחור ברירת מחדל (SelectedIndex)
        ValueBox.Focus();
    }

    private void Convert_Click(object sender, RoutedEventArgs e) => Convert();

    // TODO 5: לחבר TextChanged של ValueBox ו-SelectionChanged של שני ה-ComboBox-ים ל-Convert()
    //         (המרה חיה). ב-XAML: TextChanged="ValueBox_TextChanged" וכו'.

    // TODO 7 (בונוס): KeyDown — Escape מנקה; כפתור Swap שמחליף בין היחידות.

    private void Clear_Click(object sender, RoutedEventArgs e)
    {
        ValueBox.Clear();
        ValueBox.Focus();
    }

    /// <summary>ולידציה → המרה → הצגה.</summary>
    private void Convert()
    {
        if (!IsLoaded) return;

        // TODO 6: להשתמש ב-UnitConverter.TryParseInput; אם יש שגיאה — ShowError(error) ולצאת.
        //         אחרת לקרוא ל-UnitConverter.Convert עם היחידות הנבחרות ולהציג ב-ResultText:
        //         "10 m = 32.8084 ft"
        ResultText.Text = ValueBox.Text;
    }

    private void ShowError(string? message)
    {
        ErrorText.Text = message ?? "";
        ErrorText.Visibility = message is null ? Visibility.Collapsed : Visibility.Visible;
        ResultText.Text = "";
        ConvertButton.IsEnabled = message is null;
    }
}
