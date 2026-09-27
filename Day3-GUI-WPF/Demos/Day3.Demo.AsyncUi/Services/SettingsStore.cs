using System.IO;
using System.Text.Json;

namespace Day3.Demo.AsyncUi.Services;

public class AppSettings
{
    public string LastCityFilter { get; set; } = "";
    public bool SimulateErrors { get; set; }
    public double WindowWidth { get; set; } = 720;
}

/// <summary>שמירה/טעינה של הגדרות כקובץ JSON ב-%AppData%\Day3.Demo.AsyncUi\settings.json.</summary>
public static class SettingsStore
{
    private static readonly string Dir =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Day3.Demo.AsyncUi");
    private static readonly string FilePath = Path.Combine(Dir, "settings.json");
    private static readonly JsonSerializerOptions Options = new() { WriteIndented = true };

    public static AppSettings Load()
    {
        try
        {
            if (!File.Exists(FilePath)) return new AppSettings();
            return JsonSerializer.Deserialize<AppSettings>(File.ReadAllText(FilePath)) ?? new AppSettings();
        }
        catch (Exception ex) when (ex is IOException or JsonException)
        {
            return new AppSettings();   // קובץ פגום = ברירת מחדל, לא קריסה
        }
    }

    public static void Save(AppSettings settings)
    {
        Directory.CreateDirectory(Dir);
        File.WriteAllText(FilePath, JsonSerializer.Serialize(settings, Options));
    }
}
