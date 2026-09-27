using Day3.Demo.Binding.Mvvm;

namespace Day3.Demo.Binding.Models;

/// <summary>Model שמודיע על שינויים כדי שה-CheckBox ברשימה יעדכן את המסך.</summary>
public class TodoItem : ObservableObject
{
    private string _title = "";
    private bool _isDone;
    private int _priority = 2;

    public string Title { get => _title; set => SetProperty(ref _title, value); }
    public bool IsDone { get => _isDone; set => SetProperty(ref _isDone, value); }
    public int Priority { get => _priority; set => SetProperty(ref _priority, value); }
    public DateTime Created { get; init; } = DateTime.Now;
}
