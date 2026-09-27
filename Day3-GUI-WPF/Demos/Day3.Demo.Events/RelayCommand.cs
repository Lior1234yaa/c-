using System.Windows.Input;

namespace Day3.Demo.Events;

/// <summary>
/// מימוש מינימלי של ICommand: עוטף Action + Func&lt;bool&gt;.
/// CommandManager.RequerySuggested גורם ל-WPF לבדוק מחדש CanExecute אחרי כל אינטראקציה.
/// </summary>
public sealed class RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null) : ICommand
{
    public RelayCommand(Action execute, Func<bool>? canExecute = null)
        : this(_ => execute(), canExecute is null ? null : _ => canExecute()) { }

    public bool CanExecute(object? parameter) => canExecute?.Invoke(parameter) ?? true;
    public void Execute(object? parameter) => execute(parameter);

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    /// <summary>לקרוא כשמצב שמשפיע על CanExecute השתנה מחוץ לאינטראקציית UI.</summary>
    public static void Refresh() => CommandManager.InvalidateRequerySuggested();
}
