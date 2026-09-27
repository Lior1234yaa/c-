using System.Text.Json;

namespace Day4.Lab3.Snippets;

/// <summary>Non-secret settings only. The API key comes from environment/user-secrets, never from a default in code or a JSON next to the exe.</summary>
public sealed class AppSettings
{
    public string Theme { get; set; } = "Light";
    public int RetryCount { get; set; } = 3;
}

public sealed class SettingsService(string? path = null)
{
    private static readonly JsonSerializerOptions Options = new() { WriteIndented = true };
    private readonly string _path = path ?? Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Day4Lab3", "settings.json");

    public AppSettings Load()
    {
        if (!File.Exists(_path)) return new AppSettings();
        var json = File.ReadAllText(_path);
        return JsonSerializer.Deserialize<AppSettings>(json, Options)
               ?? throw new InvalidDataException($"Settings file '{_path}' is empty or invalid.");
    }

    public void Save(AppSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);
        Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
        File.WriteAllText(_path, JsonSerializer.Serialize(settings, Options));
        // אין לוג של ערכים — ובוודאי לא של סודות
    }

    /// <summary>הסוד מגיע מהסביבה (בפיתוח: dotnet user-secrets / משתנה סביבה; בפרודקשן: Key Vault וכד').</summary>
    public static string GetApiKey()
        => Environment.GetEnvironmentVariable("DAY4_API_KEY")
           ?? throw new InvalidOperationException("DAY4_API_KEY is not configured.");
}
