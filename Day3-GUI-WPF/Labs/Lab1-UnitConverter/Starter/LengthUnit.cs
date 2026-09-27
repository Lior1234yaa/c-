namespace Day3.Lab1.Starter;

/// <summary>יחידת אורך + מקדם המרה למטר. record כדי שיהיה שוויון לפי ערך ו-ToString נוח.</summary>
public record LengthUnit(string Name, string Symbol, double MetersPerUnit)
{
    public override string ToString() => $"{Name} ({Symbol})";

    public static readonly IReadOnlyList<LengthUnit> All =
    [
        new("Millimeter", "mm", 0.001),
        new("Centimeter", "cm", 0.01),
        new("Meter", "m", 1),
        new("Kilometer", "km", 1000),
        new("Inch", "in", 0.0254),
        new("Foot", "ft", 0.3048),
        new("Mile", "mi", 1609.344),
    ];
}
