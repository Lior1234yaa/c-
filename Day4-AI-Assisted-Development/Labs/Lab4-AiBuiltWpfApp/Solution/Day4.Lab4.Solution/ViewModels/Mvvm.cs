using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Day4.Lab4.ViewModels;

public abstract class ObservableObject : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}

/// <summary>בסיס ל-INotifyDataErrorInfo: שגיאה אחת לכל property, עם עדכון אירועים.</summary>
public abstract class ValidatableObject : ObservableObject, INotifyDataErrorInfo
{
    private readonly Dictionary<string, string> _errors = new();

    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;
    public bool HasErrors => _errors.Count > 0;

    public IEnumerable GetErrors(string? propertyName)
        => propertyName is not null && _errors.TryGetValue(propertyName, out var e) ? new[] { e } : Array.Empty<string>();

    protected void SetError(string? error, [CallerMemberName] string? propertyName = null)
    {
        if (propertyName is null) return;
        var changed = error is null ? _errors.Remove(propertyName) : !(_errors.TryGetValue(propertyName, out var old) && old == error);
        if (error is not null) _errors[propertyName] = error;
        if (changed)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            OnPropertyChanged(nameof(HasErrors));
        }
    }
}

public sealed class RelayCommand(Action execute, Func<bool>? canExecute = null) : ICommand
{
    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    public bool CanExecute(object? parameter) => canExecute?.Invoke() ?? true;
    public void Execute(object? parameter) => execute();
}

/// <summary>פקודה אסינכרונית: לא async void, מונעת הרצה כפולה, מדווחת ל-CanExecute.</summary>
public sealed class AsyncRelayCommand(Func<Task> execute, Func<bool>? canExecute = null) : ICommand
{
    private bool _running;

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    public bool CanExecute(object? parameter) => !_running && (canExecute?.Invoke() ?? true);

    public async void Execute(object? parameter)   // הממשק דורש void; החריגות נתפסות בתוך execute (ה-ViewModel)
    {
        if (!CanExecute(parameter)) return;
        _running = true;
        CommandManager.InvalidateRequerySuggested();
        try { await execute(); }
        finally { _running = false; CommandManager.InvalidateRequerySuggested(); }
    }
}
