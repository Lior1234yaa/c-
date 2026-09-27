using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace Day3.Lab2.Solution.Mvvm;

/// <summary>בסיס עם INotifyDataErrorInfo. הנגזרות קוראות ל-SetErrors(...) מתוך ה-setters.</summary>
public abstract class ValidatableObject : ObservableObject, INotifyDataErrorInfo
{
    private readonly Dictionary<string, List<string>> _errors = new();

    [JsonIgnore] public bool HasErrors => _errors.Count > 0;
    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

    public IEnumerable GetErrors(string? propertyName) =>
        propertyName is not null && _errors.TryGetValue(propertyName, out var list) ? list : Array.Empty<string>();

    protected void SetErrors(IEnumerable<string> errors, [CallerMemberName] string? propertyName = null)
    {
        if (propertyName is null) return;
        var list = errors.ToList();
        if (list.Count == 0) _errors.Remove(propertyName);
        else _errors[propertyName] = list;
        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        OnPropertyChanged(nameof(HasErrors));
    }

    /// <summary>מריץ מחדש את כל ה-setters כדי לאכלס שגיאות (למשל אחרי טעינה או יצירה).</summary>
    public abstract void ValidateAll();
}
