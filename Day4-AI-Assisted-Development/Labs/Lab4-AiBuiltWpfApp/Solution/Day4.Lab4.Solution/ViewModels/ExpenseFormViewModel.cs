using Day4.Lab4.Core.Models;

namespace Day4.Lab4.ViewModels;

/// <summary>טופס ההוספה עם validation לפי המפרט. הכללים עצמם ב-Core (ExpenseValidator) — כאן רק חיבור ל-UI.</summary>
public sealed class ExpenseFormViewModel(TimeProvider clock) : ValidatableObject
{
    private DateTime? _date = clock.GetLocalNow().Date;
    private string? _category;
    private string _amountText = "";
    private string? _note;

    public IReadOnlyList<string> Categories => Core.Models.Categories.All;

    public DateTime? Date
    {
        get => _date;
        set { if (SetProperty(ref _date, value)) Validate(); }
    }

    public string? Category
    {
        get => _category;
        set { if (SetProperty(ref _category, value)) Validate(); }
    }

    public string AmountText
    {
        get => _amountText;
        set { if (SetProperty(ref _amountText, value)) Validate(); }
    }

    public string? Note
    {
        get => _note;
        set { if (SetProperty(ref _note, value)) Validate(); }
    }

    public bool IsValid => !HasErrors && !string.IsNullOrWhiteSpace(AmountText) && Category is not null;

    public void Validate()
    {
        var today = DateOnly.FromDateTime(clock.GetLocalNow().Date);
        SetError(ExpenseValidator.ValidateDate(_date is null ? null : DateOnly.FromDateTime(_date.Value), today), nameof(Date));
        SetError(ExpenseValidator.ValidateCategory(_category), nameof(Category));
        SetError(ExpenseValidator.ValidateAmountText(_amountText, out _), nameof(AmountText));
        SetError(ExpenseValidator.ValidateNote(_note), nameof(Note));
        OnPropertyChanged(nameof(IsValid));
    }

    public Expense ToExpense()
    {
        Validate();
        if (!IsValid) throw new InvalidOperationException("Form is not valid.");
        ExpenseValidator.ValidateAmountText(_amountText, out var amount);
        return new Expense(Guid.NewGuid(), DateOnly.FromDateTime(_date!.Value), _category!, amount, string.IsNullOrWhiteSpace(_note) ? null : _note.Trim());
    }

    public void Reset()
    {
        AmountText = "";
        Note = null;
        // תאריך וקטגוריה נשארים — נוח להזנה רצופה
    }
}
