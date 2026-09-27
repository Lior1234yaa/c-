using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Day3.Lab1.Solution;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        // ItemsSource מהקוד; ה-ComboBox מציג ToString() של כל LengthUnit
        FromUnit.ItemsSource = LengthUnit.All;
        ToUnit.ItemsSource = LengthUnit.All;
        FromUnit.SelectedIndex = 2;   // Meter
        ToUnit.SelectedIndex = 5;     // Foot

        ValueBox.Focus();
    }

    // ---- אירועים: כולם מובילים לאותה פונקציה ----
    private void Convert_Click(object sender, RoutedEventArgs e) => Convert();
    private void ValueBox_TextChanged(object sender, TextChangedEventArgs e) => Convert();
    private void Unit_SelectionChanged(object sender, SelectionChangedEventArgs e) => Convert();

    private void ValueBox_KeyDown(object sender, KeyEventArgs e)
    {
        // Enter כבר מטופל ע"י IsDefault; Escape מנקה
        if (e.Key == Key.Escape) Clear_Click(sender, e);
    }

    private void Swap_Click(object sender, RoutedEventArgs e)
    {
        (FromUnit.SelectedItem, ToUnit.SelectedItem) = (ToUnit.SelectedItem, FromUnit.SelectedItem);
        // SelectionChanged כבר יפעיל Convert
    }

    private void Clear_Click(object sender, RoutedEventArgs e)
    {
        ValueBox.Clear();
        ValueBox.Focus();
    }

    /// <summary>ולידציה → המרה → הצגה. רץ על כל שינוי (המרה חיה).</summary>
    private void Convert()
    {
        if (!IsLoaded) return;   // SelectionChanged יכול לירות לפני שהכל מוכן

        var error = UnitConverter.TryParseInput(ValueBox.Text, out var value);
        if (error is not null)
        {
            ShowError(ValueBox.Text.Trim().Length == 0 ? null : error);   // שדה ריק = לא שגיאה, רק אין תוצאה
            return;
        }
        if (FromUnit.SelectedItem is not LengthUnit from || ToUnit.SelectedItem is not LengthUnit to)
            return;

        ShowError(null);
        var result = UnitConverter.Convert(value, from, to);
        ResultText.Text = $"{UnitConverter.Format(value)} {from.Symbol} = {UnitConverter.Format(result)} {to.Symbol}";
    }

    private void ShowError(string? message)
    {
        ErrorText.Text = message ?? "";
        ErrorText.Visibility = message is null ? Visibility.Collapsed : Visibility.Visible;
        ResultText.Text = "";
        ConvertButton.IsEnabled = message is null;
    }
}
