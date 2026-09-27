# תשובות לדוגמה — תרגילי prompt וניתוח (1, 3, 4, 5, 12)

אין תשובה "יחידה נכונה"; אלה דוגמאות שעומדות בקריטריונים של מודול 2.

## תרגיל 1 — איזה מצב עבודה?
1. `ToString()` ל-`Customer` — **השלמה**: משימה מקומית, ההקשר על המסך, קל לבדוק.
2. `DataGrid` לא מתעדכן — **צ'אט**: צריך הסבר (List ↔ ObservableCollection), לא קוד רב.
3. מיגרציה ב-14 קבצים + הרצת בדיקות — **סוכן**: רב-קבצי, מכני, עם אימות אוטומטי (tests). חובה: git נקי לפני, review של ה-diff אחרי.
4. `for` שממלא מערך — **השלמה**.
5. SQLite לעומת JSON — **צ'אט**: "give 2–3 options with trade-offs" — החלטה אנושית.

## תרגיל 3 — שכתוב prompt
```text
Role: You are a senior .NET developer who writes small, testable services.
Context: .NET 10 console/library, C# 14, nullable enabled, file-scoped namespaces.
  Existing: IOrderRepository { Task<Order?> GetAsync(int id, CancellationToken ct); Task AddAsync(Order o, CancellationToken ct);
  Task<IReadOnlyList<Order>> GetByCustomerAsync(int customerId, CancellationToken ct); }
  Order(int Id, int CustomerId, decimal Total, DateTimeOffset CreatedAt). IClock { DateTimeOffset UtcNow { get; } }
Task: Implement OrderService : IOrderService with
  Task<Order> CreateAsync(int customerId, decimal total, CancellationToken ct)
  Task<IReadOnlyList<Order>> GetByCustomerAsync(int customerId, CancellationToken ct)
Constraints: constructor injection (IOrderRepository, IClock, ILogger<OrderService>); throw ArgumentOutOfRangeException
  for total < 0; no DateTime.Now; no new NuGet packages; no static state.
Examples: CreateAsync(7, 100m) → Order with CustomerId 7, Total 100, CreatedAt == clock.UtcNow, saved via repository.
Output: the service class only, then xUnit tests using a fake repository and fake clock (no mocking library).
```

## תרגיל 4 — בדיקות קודם
```text
Do not implement yet. Write xUnit tests only for `PasswordPolicy.Validate(string password)` returning IReadOnlyList<string> errors
(empty list = valid). Spec: length 8–64 inclusive; at least one digit; at least one uppercase letter; no whitespace anywhere.
Tests (Method_Scenario_Expected):
- Validate_ValidPassword_ReturnsEmpty ("Abcdefg1")
- Validate_TooShort_ReturnsLengthError (7 chars) and Validate_MinLength_IsValid (8) — boundary
- Validate_TooLong_ReturnsLengthError (65) and Validate_MaxLength_IsValid (64) — boundary
- Validate_NoDigit_ReturnsDigitError; Validate_NoUppercase_ReturnsUppercaseError
- Validate_ContainsSpace_ReturnsWhitespaceError (also tab, and Hebrew letters should NOT count as uppercase)
- Validate_MultipleViolations_ReturnsAllErrors ("abc" → 3 errors)
- Validate_Null_ThrowsArgumentNullException
Use [Theory]/[InlineData] where it reads well. Assert on error count and on message keywords, not exact sentences.
```

## תרגיל 5 — קובץ הוראות (CSV → JSON)
```text
# CsvToJson — instructions for AI assistants
Stack: .NET 10 console app, C# 14, nullable enabled, System.Text.Json only, xUnit tests. No third-party CSV/JSON packages.
Structure: src/CsvToJson (Program.cs, Parsing/, Output/), tests/CsvToJson.Tests.
Conventions:
- File-scoped namespaces; one type per file; `_camelCase` private fields; async methods end with Async + CancellationToken.
- All parsing uses CultureInfo.InvariantCulture. Money = decimal, dates = DateOnly/DateTimeOffset.
- Errors: throw FormatException with line number; never swallow exceptions; exit code 1 on failure.
- Public members get XML docs; tests named Method_Scenario_Expected.
Commands: dotnet build; dotnet test; dotnet format --verify-no-changes.
Never: read files outside the input path given on the command line; add NuGet packages without asking.
```

## תרגיל 12 — prompt לחלון התחברות
```text
Generate LoginWindow.xaml only (no code-behind logic). .NET 10 WPF, MVVM.
DataContext: LoginViewModel : INotifyPropertyChanged, INotifyDataErrorInfo with
  string UserName; string Password (bound via PasswordBox helper is NOT allowed — use a TextBox with PasswordChar for this exercise);
  ICommand LoginCommand; bool IsBusy; string? ErrorMessage.
Layout: Grid, 2 columns (labels/inputs), rows: title, username, password, error text, button row.
Constraints: FlowDirection="RightToLeft", Hebrew labels ("שם משתמש", "סיסמה", "התחבר"), MinWidth 360,
  ValidatesOnNotifyDataErrors=True + UpdateSourceTrigger=PropertyChanged on both inputs,
  validation message under each field via Validation.ErrorTemplate,
  ErrorMessage TextBlock in red (StaticResource DangerBrush) visible only when not null,
  ProgressBar IsIndeterminate bound to IsBusy, button disabled while busy (Command CanExecute),
  AutomationProperties.Name on inputs and button, standard WPF controls only, brushes via StaticResource in Window.Resources.
```
שלושה דברים לבדוק בפלט: (1) שאין namespace/ספרייה חיצונית ושאין `Button_Click` ב-code-behind; (2) ש-bindings תואמים בדיוק לשמות ב-ViewModel ו-`ValidatesOnNotifyDataErrors` קיים; (3) שה-RTL נכון ויזואלית (תוויות מימין, טקסט סיסמה/מספרים ב-LTR אם צריך) ושכל ה-StaticResource מוגדרים.
