using System.Globalization;
using System.Windows.Data;

namespace Day3.Demo.Binding.Converters;

public class PriorityToTextConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
        value switch { 1 => "High", 2 => "Normal", 3 => "Low", _ => "?" };

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
