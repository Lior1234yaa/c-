using Day3.Lab2.Starter.Models;

namespace Day3.Lab2.Starter.Services;

public interface IContactsStore
{
    Task<List<Contact>> LoadAsync();
    Task SaveAsync(IEnumerable<Contact> contacts);
    string Location { get; }
}
