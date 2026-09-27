using System.Text.Json;

namespace Day4.Lab3.Snippets;

public class AppSettings
{
    public string ApiKey { get; set; } = "sk-live-3f9a1c2e7b8d4e5f6a7b8c9d0e1f2a3b";
    public string Theme { get; set; } = "Light";
    public int RetryCount { get; set; } = 3;
}

/// <summary>Loads and saves settings as JSON next to the executable.</summary>
public class SettingsService
{
    private readonly string _path = Path.Combine(AppContext.BaseDirectory, "settings.json");

    public AppSettings Load()
    {
        if (!File.Exists(_path)) return new AppSettings();
        var json = File.ReadAllText(_path);
        return JsonSerializer.Deserialize<AppSettings>(json);
    }

    public void Save(AppSettings settings)
    {
        var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_path, json);
        Console.WriteLine($"Saved settings with key {settings.ApiKey}");
    }
}
