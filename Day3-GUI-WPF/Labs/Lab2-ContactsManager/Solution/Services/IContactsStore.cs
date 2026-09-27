using Day3.Lab2.Solution.Models;

namespace Day3.Lab2.Solution.Services;

public interface IContactsStore
{
    Task<List<Contact>> LoadAsync();
    Task SaveAsync(IEnumerable<Contact> contacts);
    string Location { get; }
}
