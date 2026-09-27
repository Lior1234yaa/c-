using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;
using Day3.Lab2.Starter.Models;
using Day3.Lab2.Starter.Mvvm;
using Day3.Lab2.Starter.Services;

namespace Day3.Lab2.Starter.ViewModels;

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
            Draft = value?.Clone();        // עריכה על עותק — Cancel מחזיר למקור
            OnPropertyChanged(nameof(HasSelection));
        }
    }

    public Contact? Draft { get => _draft; private set => SetProperty(ref _draft, value); }
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
        // TODO 5: להגדיר ContactsView.Filter שמסנן לפי Search (שם/טלפון/אימייל, case-insensitive)
        //         ולהוסיף SortDescription לפי FirstName.

        // TODO 2: לממש את הפקודות. CanExecute:
        //   Delete/Cancel — רק כשיש בחירה; Apply — רק כש-Draft קיים ובלי שגיאות; Save — רק כש-IsDirty.
        AddCommand = new RelayCommand(Add);
        DeleteCommand = new RelayCommand(Delete);
        ApplyCommand = new RelayCommand(Apply);
        CancelCommand = new RelayCommand(() => Draft = Selected?.Clone());
        SaveCommand = new RelayCommand(async () => await SaveAsync());
        LoadCommand = new RelayCommand(async () => await LoadAsync());

        Contacts.CollectionChanged += (_, _) => { MarkDirty(); OnPropertyChanged(nameof(Title)); };
    }

    private void Add()
    {
        var c = new Contact { FirstName = "New", LastName = "Contact", Phone = "050-0000000" };
        Contacts.Add(c);
        Selected = c;
    }

    private void Delete()
    {
        // TODO 2b: להסיר את Selected ולבחור את השכן (או null אם הרשימה ריקה)
        throw new NotImplementedException();
    }

    private void Apply()
    {
        // TODO 2c: להעתיק מ-Draft ל-Selected (CopyFrom), MarkDirty, ContactsView.Refresh(), ולעדכן Status
        throw new NotImplementedException();
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
            Status = $"נטענו {list.Count} אנשי קשר";
        }
        catch (Exception ex) { Status = $"שגיאה בטעינה: {ex.Message}"; }
    }

    public async Task SaveAsync()
    {
        try
        {
            await _store.SaveAsync(Contacts);
            IsDirty = false;
            OnPropertyChanged(nameof(Title));
            Status = $"נשמר ב-{DateTime.Now:T}";
        }
        catch (Exception ex) { Status = $"שגיאה בשמירה: {ex.Message}"; }
    }

    private void MarkDirty() { IsDirty = true; OnPropertyChanged(nameof(Title)); }
}
