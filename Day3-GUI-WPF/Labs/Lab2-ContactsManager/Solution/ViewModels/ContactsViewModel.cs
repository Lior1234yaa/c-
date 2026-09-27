using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;
using Day3.Lab2.Solution.Models;
using Day3.Lab2.Solution.Mvvm;
using Day3.Lab2.Solution.Services;

namespace Day3.Lab2.Solution.ViewModels;

/// <summary>
/// ה-ViewModel הראשי. הרשימה נצפית; הפריט הנבחר נערך דרך עותק (Draft) כדי לאפשר Cancel.
/// </summary>
public class ContactsViewModel : ObservableObject
{
    private readonly IContactsStore _store;
    private Contact? _selected;
    private Contact? _draft;
    private string _search = "";
    private string _status = "";
    private bool _isDirty;

    public ObservableCollection<Contact> Contacts { get; } = [];
    public ICollectionView ContactsView { get; }

    public Contact? Selected
    {
        get => _selected;
        set
        {
            if (!SetProperty(ref _selected, value)) return;
            Draft = value?.Clone();        // עריכה על עותק
            OnPropertyChanged(nameof(HasSelection));
        }
    }

    /// <summary>העותק שהטופס קשור אליו. Apply מעתיק חזרה ל-Selected.</summary>
    public Contact? Draft
    {
        get => _draft;
        private set => SetProperty(ref _draft, value);
    }

    public bool HasSelection => Selected is not null;

    public string Search
    {
        get => _search;
        set { if (SetProperty(ref _search, value)) ContactsView.Refresh(); }
    }

    public string Status { get => _status; set => SetProperty(ref _status, value); }
    public bool IsDirty { get => _isDirty; private set => SetProperty(ref _isDirty, value); }
    public string Title => $"Contacts ({Contacts.Count})" + (IsDirty ? " *" : "");

    public ICommand AddCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand ApplyCommand { get; }
    public ICommand CancelCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand LoadCommand { get; }

    public ContactsViewModel(IContactsStore store)
    {
        _store = store;

        ContactsView = CollectionViewSource.GetDefaultView(Contacts);
        ContactsView.Filter = o => o is Contact c && Matches(c);
        ContactsView.SortDescriptions.Add(new SortDescription(nameof(Contact.FirstName), ListSortDirection.Ascending));

        AddCommand = new RelayCommand(Add);
        DeleteCommand = new RelayCommand(Delete, () => HasSelection);
        ApplyCommand = new RelayCommand(Apply, () => Draft is { HasErrors: false });
        CancelCommand = new RelayCommand(() => Draft = Selected?.Clone(), () => HasSelection);
        SaveCommand = new RelayCommand(async () => await SaveAsync(), () => IsDirty);
        LoadCommand = new RelayCommand(async () => await LoadAsync());

        Contacts.CollectionChanged += (_, _) => { MarkDirty(); OnPropertyChanged(nameof(Title)); };
    }

    private bool Matches(Contact c) =>
        string.IsNullOrWhiteSpace(Search)
        || c.FullName.Contains(Search, StringComparison.OrdinalIgnoreCase)
        || c.Phone.Contains(Search, StringComparison.OrdinalIgnoreCase)
        || c.Email.Contains(Search, StringComparison.OrdinalIgnoreCase);

    private void Add()
    {
        var c = new Contact { FirstName = "New", LastName = "Contact", Phone = "050-0000000" };
        Contacts.Add(c);
        Selected = c;
    }

    private void Delete()
    {
        if (Selected is null) return;
        var idx = Contacts.IndexOf(Selected);
        Contacts.Remove(Selected);
        Selected = Contacts.Count == 0 ? null : Contacts[Math.Min(idx, Contacts.Count - 1)];
    }

    private void Apply()
    {
        if (Selected is null || Draft is null) return;
        Selected.CopyFrom(Draft);
        MarkDirty();
        ContactsView.Refresh();   // עדכון מיון/סינון אחרי שינוי שם
        Status = $"עודכן: {Selected.FullName}";
    }

    public async Task LoadAsync()
    {
        try
        {
            var list = await _store.LoadAsync();
            Contacts.Clear();
            foreach (var c in list) Contacts.Add(c);
            IsDirty = false;
            OnPropertyChanged(nameof(Title));
            Status = list.Count == 0 ? "אין קובץ שמור עדיין — התחילו להוסיף אנשי קשר" : $"נטענו {list.Count} אנשי קשר מ-{_store.Location}";
        }
        catch (Exception ex)
        {
            Status = $"שגיאה בטעינה: {ex.Message}";
        }
    }

    public async Task SaveAsync()
    {
        try
        {
            await _store.SaveAsync(Contacts);
            IsDirty = false;
            OnPropertyChanged(nameof(Title));
            Status = $"נשמר ({Contacts.Count}) ב-{DateTime.Now:T}";
        }
        catch (Exception ex)
        {
            Status = $"שגיאה בשמירה: {ex.Message}";
        }
    }

    private void MarkDirty()
    {
        IsDirty = true;
        OnPropertyChanged(nameof(Title));
    }
}
