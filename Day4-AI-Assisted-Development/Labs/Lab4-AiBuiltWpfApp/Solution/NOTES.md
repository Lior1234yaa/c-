<div dir="rtl">

# Lab 4 — הערות על הפתרון

## מבנה
<div dir="ltr">

```
Day4.Lab4.Core/            net10.0 — Models (Expense, Categories, ExpenseValidator), Services (IExpenseRepository, JsonExpenseRepository)
Day4.Lab4.Solution/        net10.0-windows — WPF: ViewModels (Mvvm, ExpenseForm, Main), MainWindow.xaml, Themes
Day4.Lab4.Solution.Tests/  net10.0 — בדיקות ל-Core (repository + validator) — רצות גם ב-CI לינוקס
```

</div>
הפיצול ל-Core הוא החלטה מודעת: כללי ה-validation והאחסון לא תלויים ב-WPF, ולכן נבדקים בלי חלון. ה-ViewModel רק מחבר.

## החלטות
- **`ValidatableObject`** קטן (INotifyDataErrorInfo) ב-Mvvm.cs, שגיאה אחת ל-property. `Validation.ErrorTemplate` משותף ב-`Themes/Colors.xaml` מציג אותה מתחת לשדה.
- **`AsyncRelayCommand`**: `Execute` הוא `async void` (הממשק מחייב), אבל כל החריגות נתפסות ב-`MainViewModel.RunAsync` — לכן זה בטוח. מונע לחיצה כפולה.
- **`TimeProvider`** מוזרק → "תאריך לא בעתיד" ניתן לבדיקה.
- **כתיבה אטומית** (temp + `File.Move(overwrite)`) — קובץ לא נשאר חצי-כתוב אם האפליקציה נסגרת באמצע.
- **שגיאות I/O ← UI**: `RunAsync` תופס רק סוגי חריגות צפויים (`IOException`, `InvalidDataException`, `UnauthorizedAccessException`) ומציג `ErrorMessage`; באגים אמיתיים לא מוסתרים.
- **Culture**: JSON תמיד Invariant (ברירת המחדל של System.Text.Json ל-DateOnly/decimal); UI מעצב `N2` לפי culture נוכחית; שדה הסכום `FlowDirection=LeftToRight`.
- `DatePicker.SelectedDate` הוא `DateTime?` — ההמרה ל-`DateOnly` ב-ViewModel.

## טעויות AI טיפוסיות שראינו
| טעות | תיקון |
|------|-------|
| `async void` בפקודות + `try/catch` ריק | `AsyncRelayCommand` + `RunAsync` מרכזי |
| `DateTime.Now` בבדיקת "לא בעתיד" | `TimeProvider` מוזרק |
| `double Amount` ו-`decimal.Parse(text)` בלי culture | `decimal` + `InvariantCulture` |
| `File.WriteAllText` ישיר (לא אטומי), ללא יצירת תיקייה | temp + move, `CreateDirectory` |
| `catch (Exception) { }` ב-Load "כדי שלא יקרוס" | חריגה ברורה → `ErrorMessage` |
| binding ל-`Form.AddCommand` (לא קיים) | `RelativeSource AncestorType=Window` |
| Validation רק בלחיצה על "הוסף" | `UpdateSourceTrigger=PropertyChanged` + `ValidatesOnNotifyDataErrors` |
| `ObservableCollection` שמוחלפת (`Expenses = new ...`) בלי PropertyChanged | Clear/Add על אותו מופע |
| xmlns של ספריית controls חיצונית ל-DatePicker "יפה" | `DatePicker` סטנדרטי |
| חישוב אחוזים ב-`decimal` עם חלוקה באפס | בדיקת `total == 0` |

## אימות
<div dir="ltr">

```bash
cd Solution/Day4.Lab4.Solution.Tests && dotnet test      # רץ בכל מערכת הפעלה
cd ../Day4.Lab4.Solution && dotnet build                 # WPF; הרצה רק ב-Windows
```

</div>

</div>
