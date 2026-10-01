using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Input;
using Day3.Demo.Binding.Models;
using Day3.Demo.Binding.Mvvm;

namespace Day3.Demo.Binding.ViewModels;

/// <summary>
/// ה-ViewModel: מצב המסך + פקודות. אין כאן שום using של System.Windows.Controls —
/// אפשר לבדוק אותו ב-unit test בלי חלון.
/// </summary>
public class MainViewModel : ObservableObject
{
    private string _newTitle = "";
    private TodoItem? _selected;

    public ObservableCollection<TodoItem> Items { get; } = [];

    public string NewTitle
    {
        get => _newTitle;
        set => SetProperty(ref _newTitle, value);
    }

    public TodoItem? Selected
    {
        get => _selected;
        set { if (SetProperty(ref _selected, value)) OnPropertyChanged(nameof(HasSelection)); }
    }

    public bool HasSelection => Selected is not null;

    // Computed properties — צריך להודיע עליהן ידנית כשהמקור משתנה
    public int DoneCount => Items.Count(i => i.IsDone);
    public string Summary => $"{DoneCount} / {Items.Count} done";

    public ICommand AddCommand { get; }
    public ICommand RemoveCommand { get; }

    public MainViewModel()
    {
        AddCommand = new RelayCommand(Add, () => !string.IsNullOrWhiteSpace(NewTitle));
        RemoveCommand = new RelayCommand(Remove, () => Selected is not null);

        Items.CollectionChanged += OnItemsChanged;

        foreach (var t in new[] { "Learn XAML", "Build a ViewModel", "Drink coffee" })
            Items.Add(new TodoItem { Title = t, Priority = t.Contains("coffee") ? 1 : 2 });
        Items[2].IsDone = true;
    }

    private void Add()
    {
        Items.Add(new TodoItem { Title = NewTitle.Trim() });
        NewTitle = "";
    }

    private void Remove()
    {
        if (Selected is null) return;
        Items.Remove(Selected);
        Selected = null;
    }

    private void OnItemsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        // מאזינים ל-IsDone של כל פריט כדי לעדכן את הסיכום
        if (e.NewItems is not null)
            foreach (TodoItem item in e.NewItems) item.PropertyChanged += OnItemPropertyChanged;
        if (e.OldItems is not null)
            foreach (TodoItem item in e.OldItems) item.PropertyChanged -= OnItemPropertyChanged;
        RaiseSummary();
    }

    private void OnItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(TodoItem.IsDone)) RaiseSummary();
    }

    private void RaiseSummary()
    {
        OnPropertyChanged(nameof(DoneCount));
        OnPropertyChanged(nameof(Summary));
    }
}
