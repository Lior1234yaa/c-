namespace Day4.Demo.DiHostWpf.Services;

/// <summary>Options pattern: מקטע "App" מתוך appsettings.json.</summary>
public sealed class AppOptions
{
    public string Title { get; set; } = "Orders Desktop";
    public string DataFile { get; set; } = "orders.json";
    public string Currency { get; set; } = "ILS";
    public int MaxItemsPerOrder { get; set; } = 100;
}
