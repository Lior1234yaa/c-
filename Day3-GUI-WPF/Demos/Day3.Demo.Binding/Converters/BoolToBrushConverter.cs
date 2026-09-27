using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Day3.Demo.Binding.Converters;

/// <summary>ממיר bool לצבע: true → ירוק, false → אפור. משמש ב-{Binding IsDone, Converter=...}.</summary>
public class BoolToBrushConverter : IValueConverter
{
    public Brush TrueBrush { get; set; } = Brushes.SeaGreen;
    public Brush FalseBrush { get; set; } = Brushes.DimGray;

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
        value is true ? TrueBrush : FalseBrush;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotSupportedException();   // one-way only
}
