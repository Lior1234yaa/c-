using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Day3.Demo.Binding.Mvvm;

/// <summary>
/// מחלקת בסיס ל-ViewModel/Model: מממשת INotifyPropertyChanged פעם אחת.
/// [CallerMemberName] ממלא את שם ה-property אוטומטית — אין מחרוזות קסם.
/// (ב-CommunityToolkit.Mvvm יש ObservableObject + [ObservableProperty] שעושים את זה בקומפילציה.)
/// </summary>
public abstract class ObservableObject : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    /// <summary>מעדכן שדה ומודיע רק אם הערך באמת השתנה. מחזיר true אם השתנה.</summary>
    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? name = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(name);
        return true;
    }
}
