# Copilot instructions — Expense Tracker (Lab 4)

We build a WPF (.NET 10, C# 14) desktop app with MVVM and no third-party MVVM frameworks.
Read `CLAUDE.md`-style rules below and follow them for every suggestion, chat answer and agent task.

## Architecture
- Single WPF project (Lab 4 Expense Tracker): `Models/` (Expense, Categories), `Services/` (IExpenseRepository, JsonExpenseRepository), `ViewModels/` (Mvvm.cs, ExpenseFormViewModel, MainViewModel), `MainWindow.xaml`, `Themes/Colors.xaml`.
- Keep Models free of WPF types and Services free of ViewModels. Constructor injection (IExpenseRepository, TimeProvider); `Microsoft.Extensions.Hosting` only as the bonus step. No service locator.

## Code style
- File-scoped namespaces, nullable enabled, one type per file, `_camelCase` private fields.
- `decimal` for money, `DateTimeOffset` for timestamps, inject `IClock` instead of `DateTime.Now`.
- `CultureInfo.InvariantCulture` for machine parsing/formatting.
- `System.Text.Json` only; cache `JsonSerializerOptions` in a static field.
- Async methods end with `Async`, accept `CancellationToken`, never `async void` (except WPF event handlers), never `.Result`/`.Wait()`.
- Never swallow exceptions; log with `ILogger<T>` structured placeholders.
- Never hardcode secrets; configuration via `appsettings.json`/environment.

## WPF
- No logic in code-behind. ViewModels derive from `ObservableObject`; commands are `RelayCommand`.
- `FlowDirection="RightToLeft"`, Hebrew labels, styles and brushes from `Themes/*.xaml` via `StaticResource`.
- Standard WPF controls only. Include loading/empty/error states and `AutomationProperties.Name`.

## Tests
- xUnit, `Method_Scenario_Expected`, `[Theory]` with `[InlineData]` for tabular cases. Write tests before implementation when adding features.

## Build & verify
`dotnet build`, `dotnet test`, `dotnet format --verify-no-changes`. Do not add NuGet packages without asking.
