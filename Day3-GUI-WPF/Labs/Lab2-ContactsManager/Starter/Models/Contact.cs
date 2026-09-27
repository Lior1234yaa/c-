using System.Net.Mail;
using System.Text.Json.Serialization;
using Day3.Lab2.Starter.Mvvm;

namespace Day3.Lab2.Starter.Models;

/// <summary>איש קשר: מודיע על שינויים + מאמת את עצמו.</summary>
public class Contact : ValidatableObject
{
    private string _firstName = "";
    private string _lastName = "";
    private string _phone = "";
    private string _email = "";
    private string _notes = "";
    private bool _isFavorite;

    public Guid Id { get; init; } = Guid.NewGuid();

    // TODO 1: להפוך את ה-properties ל-"נצפים" בעזרת SetProperty (ראו Notes).
    //         FirstName/LastName צריכים גם OnPropertyChanged(nameof(FullName)).
    public string FirstName { get => _firstName; set => _firstName = value; }
    public string LastName { get => _lastName; set => _lastName = value; }
    public string Phone { get => _phone; set => _phone = value; }
    public string Email { get => _email; set => _email = value; }
    public string Notes { get => _notes; set => _notes = value; }
    public bool IsFavorite { get => _isFavorite; set => _isFavorite = value; }

    [JsonIgnore] public string FullName => $"{FirstName} {LastName}".Trim();

    // TODO 4: לקרוא ל-ValidateName/ValidatePhone/ValidateEmail מתוך ה-setters המתאימים,
    //         ולממש את שלוש הפונקציות: שם פרטי חובה; טלפון חובה עם 9–15 ספרות;
    //         אימייל אופציונלי אבל אם קיים חייב להיות תקין (MailAddress.TryCreate).
    public override void ValidateAll()
    {
        ValidateName(FirstName);
        ValidatePhone(Phone);
        ValidateEmail(Email);
    }

    private void ValidateName(string value) { }
    private void ValidatePhone(string value) { }
    private void ValidateEmail(string value) { }

    public Contact Clone() => new()
    {
        Id = Id, FirstName = FirstName, LastName = LastName, Phone = Phone, Email = Email, Notes = Notes, IsFavorite = IsFavorite,
    };

    public void CopyFrom(Contact other)
    {
        FirstName = other.FirstName; LastName = other.LastName; Phone = other.Phone;
        Email = other.Email; Notes = other.Notes; IsFavorite = other.IsFavorite;
    }
}
