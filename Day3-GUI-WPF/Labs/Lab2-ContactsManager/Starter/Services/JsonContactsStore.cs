using System.IO;
using System.Text.Json;
using Day3.Lab2.Starter.Models;

namespace Day3.Lab2.Starter.Services;

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

    // TODO 6a: אם הקובץ לא קיים — רשימה ריקה. אחרת: File.OpenRead + JsonSerializer.DeserializeAsync<List<Contact>>.
    //          אחרי הטעינה לקרוא ל-ValidateAll() על כל איש קשר.
    public Task<List<Contact>> LoadAsync() => throw new NotImplementedException();

    // TODO 6b: Directory.CreateDirectory + File.Create + JsonSerializer.SerializeAsync
    public Task SaveAsync(IEnumerable<Contact> contacts) => throw new NotImplementedException();
}
