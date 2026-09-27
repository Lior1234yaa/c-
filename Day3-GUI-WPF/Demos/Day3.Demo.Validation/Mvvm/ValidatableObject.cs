using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Day3.Demo.Validation.Mvvm;

/// <summary>
/// בסיס ל-ViewModel עם INotifyDataErrorInfo: שומר מילון של שגיאות לכל property.
/// ה-Binding (ValidatesOnNotifyDataErrors=True, ברירת מחדל ב-.NET Core+) מציג אותן אוטומטית.
/// </summary>
public abstract class ValidatableObject : ObservableObject, INotifyDataErrorInfo
{
    private readonly Dictionary<string, List<string>> _errors = new();

    public bool HasErrors => _errors.Count > 0;
    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

    public IEnumerable GetErrors(string? propertyName) =>
        propertyName is not null && _errors.TryGetValue(propertyName, out var list) ? list : Array.Empty<string>();

    protected void SetErrors(IEnumerable<string> errors, [CallerMemberName] string? propertyName = null)
    {
        if (propertyName is null) return;
        var list = errors.ToList();
        var changed = list.Count > 0 ? !_errors.TryGetValue(propertyName, out var old) || !old.SequenceEqual(list)
                                     : _errors.Remove(propertyName);
        if (list.Count > 0) _errors[propertyName] = list;
        if (changed)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            OnPropertyChanged(nameof(HasErrors));
        }
    }
}
