using System.IO;
using System.Text.Json;
using Day3.Lab2.Solution.Models;

namespace Day3.Lab2.Solution.Services;

/// <summary>שמירה/טעינה ב-%AppData%\Day3.ContactsManager\contacts.json.</summary>
public class JsonContactsStore : IContactsStore
{
    private static readonly JsonSerializerOptions Options = new() { WriteIndented = true };

    public string Location { get; }

    public JsonContactsStore(string? path = null)
    {
        Location = path ?? Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Day3.ContactsManager", "contacts.json");
    }

    public async Task<List<Contact>> LoadAsync()
    {
        if (!File.Exists(Location)) return [];
        await using var stream = File.OpenRead(Location);
        var list = await JsonSerializer.DeserializeAsync<List<Contact>>(stream, Options) ?? [];
        foreach (var c in list) c.ValidateAll();
        return list;
    }

    public async Task SaveAsync(IEnumerable<Contact> contacts)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(Location)!);
        await using var stream = File.Create(Location);
        await JsonSerializer.SerializeAsync(stream, contacts.ToList(), Options);
    }
}
