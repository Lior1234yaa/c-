using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace Day3.Lab2.Starter.Mvvm;

/// <summary>בסיס עם INotifyDataErrorInfo. הנגזרות קוראות ל-SetErrors(...) מתוך ה-setters.</summary>
public abstract class ValidatableObject : ObservableObject, INotifyDataErrorInfo
{
    private readonly Dictionary<string, List<string>> _errors = new();

    [JsonIgnore] public bool HasErrors => _errors.Count > 0;
#pragma warning disable CS0067 // האירוע ישמש אחרי מימוש SetErrors
    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;
#pragma warning restore CS0067

    // TODO 3a: להחזיר את רשימת השגיאות של propertyName (או ריק)
    public IEnumerable GetErrors(string? propertyName) => throw new NotImplementedException();

    // TODO 3b: לעדכן את המילון (להסיר כשהרשימה ריקה), להפעיל ErrorsChanged ולהודיע על HasErrors
    protected void SetErrors(IEnumerable<string> errors, [CallerMemberName] string? propertyName = null) =>
        throw new NotImplementedException();

    public abstract void ValidateAll();
}
