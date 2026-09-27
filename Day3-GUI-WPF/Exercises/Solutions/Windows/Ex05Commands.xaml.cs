using System.Windows;
using System.Windows.Input;
using Day3.Exercises.Solutions.Mvvm;

namespace Day3.Exercises.Solutions.Windows;

public partial class Ex05Commands : Window
{
    public Ex05Commands() { InitializeComponent(); DataContext = new CounterViewModel(); }
}

public class CounterViewModel : ObservableObject
{
    private int _value;
    public int Value { get => _value; private set => SetProperty(ref _value, value); }

    public ICommand IncrementCommand { get; }
    public ICommand DecrementCommand { get; }
    public ICommand ResetCommand { get; }

    public CounterViewModel()
    {
        IncrementCommand = new RelayCommand(() => Value++, () => Value < 10);
        DecrementCommand = new RelayCommand(() => Value--, () => Value > 0);
        ResetCommand = new RelayCommand(() => Value = 0, () => Value != 0);
    }
}
