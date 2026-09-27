using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Day3.Exercises.Solutions.Windows;

public partial class Ex09Converter : Window
{
    public Ex09Converter() => InitializeComponent();
}

public class TempToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
        value is double t ? t switch
        {
            < 10 => Brushes.SteelBlue,
            < 25 => Brushes.SeaGreen,
            < 35 => Brushes.Orange,
            _ => Brushes.Crimson,
        } : Brushes.Gray;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotSupportedException();
}

public class ThresholdToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var threshold = double.Parse(parameter?.ToString() ?? "0", CultureInfo.InvariantCulture);
        return value is double v && v >= threshold ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotSupportedException();
}
