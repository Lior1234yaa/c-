using System.Net.Mail;
using System.Text.Json.Serialization;
using Day3.Lab2.Solution.Mvvm;

namespace Day3.Lab2.Solution.Models;

/// <summary>
/// איש קשר. מודל "חכם": מודיע על שינויים (לצורך binding) ומאמת את עצמו (INotifyDataErrorInfo).
/// ל-JSON נשמרים רק ה-properties הציבוריים; HasErrors מסומן [JsonIgnore].
/// </summary>
public class Contact : ValidatableObject
{
    private string _firstName = "";
    private string _lastName = "";
    private string _phone = "";
    private string _email = "";
    private string _notes = "";
    private bool _isFavorite;

    public Guid Id { get; init; } = Guid.NewGuid();

    public string FirstName
    {
        get => _firstName;
        set { if (SetProperty(ref _firstName, value)) { ValidateName(value); OnPropertyChanged(nameof(FullName)); } }
    }

    public string LastName
    {
        get => _lastName;
        set { if (SetProperty(ref _lastName, value)) OnPropertyChanged(nameof(FullName)); }
    }

    public string Phone
    {
        get => _phone;
        set { if (SetProperty(ref _phone, value)) ValidatePhone(value); }
    }

    public string Email
    {
        get => _email;
        set { if (SetProperty(ref _email, value)) ValidateEmail(value); }
    }

    public string Notes { get => _notes; set => SetProperty(ref _notes, value); }
    public bool IsFavorite { get => _isFavorite; set => SetProperty(ref _isFavorite, value); }

    [JsonIgnore] public string FullName => $"{FirstName} {LastName}".Trim();

    public override void ValidateAll()
    {
        ValidateName(FirstName);
        ValidatePhone(Phone);
        ValidateEmail(Email);
    }

    private void ValidateName(string value) =>
        SetErrors(string.IsNullOrWhiteSpace(value) ? ["שם פרטי הוא שדה חובה"] : [], nameof(FirstName));

    private void ValidatePhone(string value)
    {
        var digits = value.Count(char.IsDigit);
        SetErrors(string.IsNullOrWhiteSpace(value) ? ["טלפון הוא שדה חובה"]
                : digits is < 9 or > 15 ? ["טלפון חייב להכיל 9–15 ספרות"] : [], nameof(Phone));
    }

    private void ValidateEmail(string value) =>
        SetErrors(value.Length > 0 && !MailAddress.TryCreate(value, out _) ? ["כתובת אימייל לא תקינה"] : [], nameof(Email));

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
