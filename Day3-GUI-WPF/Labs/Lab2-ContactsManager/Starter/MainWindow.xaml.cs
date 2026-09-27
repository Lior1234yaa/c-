using System.ComponentModel;
using System.Windows;
using Day3.Lab2.Starter.Services;
using Day3.Lab2.Starter.ViewModels;

namespace Day3.Lab2.Starter;

public partial class MainWindow : Window
{
    private readonly ContactsViewModel _vm;

    public MainWindow()
    {
        InitializeComponent();
        // DI-lite: החלון בונה את התלויות ומזריק אותן ל-VM
        _vm = new ContactsViewModel(new JsonContactsStore());
        DataContext = _vm;
    }

    private async void Window_Loaded(object sender, RoutedEventArgs e) => await _vm.LoadAsync();

    private bool _closeConfirmed;

    // Closing הוא סינכרוני, אבל שמירה היא async: מבטלים את הסגירה, שומרים, ואז סוגרים שוב.
    private async void Window_Closing(object sender, CancelEventArgs e)
    {
        if (_closeConfirmed || !_vm.IsDirty) return;
        var r = MessageBox.Show(this, "יש שינויים שלא נשמרו. לשמור לפני היציאה?", "Contacts",
                                MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
        if (r == MessageBoxResult.Cancel) { e.Cancel = true; return; }
        if (r == MessageBoxResult.Yes)
        {
            e.Cancel = true;
            await _vm.SaveAsync();
            _closeConfirmed = true;
            Close();
        }
    }
}
