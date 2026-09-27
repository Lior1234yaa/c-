using System.Globalization;
using System.Windows.Data;

namespace Day3.Demo.Binding.Converters;

public class PriorityToTextConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
        value switch { 1 => "גבוהה", 2 => "רגילה", 3 => "נמוכה", _ => "?" };

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
