using System.Net.Mail;
using System.Windows.Input;
using Day3.Demo.Validation.Mvvm;

namespace Day3.Demo.Validation.ViewModels;

/// <summary>טופס עם INotifyDataErrorInfo: כל setter מריץ ולידציה, וה-Save פעיל רק כשאין שגיאות.</summary>
public class PersonFormViewModel : ValidatableObject
{
    private string _name = "";
    private string _email = "";
    private string _ageText = "";
    private string _status = "";

    public string Name
    {
        get => _name;
        set
        {
            if (!SetProperty(ref _name, value)) return;
            SetErrors(Validate());
            IEnumerable<string> Validate()
            {
                if (string.IsNullOrWhiteSpace(value)) yield return "Name is required";
                else if (value.Trim().Length < 2) yield return "Name must be at least 2 characters";
            }
        }
    }

    public string Email
    {
        get => _email;
        set
        {
            if (!SetProperty(ref _email, value)) return;
            SetErrors(MailAddress.TryCreate(value, out _) ? [] : ["Invalid email address"]);
        }
    }

    public string AgeText
    {
        get => _ageText;
        set
        {
            if (!SetProperty(ref _ageText, value)) return;
            SetErrors(int.TryParse(value, out var age) && age is >= 0 and <= 120 ? [] : ["Age must be a number between 0 and 120"]);
        }
    }

    public string Status { get => _status; set => SetProperty(ref _status, value); }

    public ICommand SaveCommand { get; }

    public PersonFormViewModel()
    {
        SaveCommand = new RelayCommand(Save, () => !HasErrors && Touched);
        // מריצים ולידציה ראשונית כדי שהטופס הריק ייחשב לא תקין
        Name = ""; Email = ""; AgeText = "";
    }

    private bool Touched => _name.Length + _email.Length + _ageText.Length > 0;

    private void Save() => Status = $"Saved: {Name} <{Email}>, age {AgeText} at {DateTime.Now:T}";
}
