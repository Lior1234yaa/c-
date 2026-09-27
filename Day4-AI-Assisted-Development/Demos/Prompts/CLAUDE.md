# Orders Desktop — project instructions for AI assistants

This file is read automatically by Claude Code. Keep it short, accurate and up to date.

## Stack
- .NET 10, C# 14, WPF (`net10.0-windows`), MVVM **without** third-party MVVM frameworks.
- Serialization: `System.Text.Json` only. HTTP: `HttpClient` via `IHttpClientFactory`.
- DI/Hosting: `Microsoft.Extensions.Hosting` (host built in `App.xaml.cs`).
- Logging: `Microsoft.Extensions.Logging` (`ILogger<T>`). Tests: xUnit.

## Solution structure
```
src/Orders.Domain          entities, value objects, business rules  (no project references)
src/Orders.Application     services, interfaces (IOrderRepository, IClock), DTOs
src/Orders.Infrastructure  JSON repositories, HTTP clients, SystemClock
src/Orders.Wpf             Views/ (XAML), ViewModels/, Themes/, App.xaml.cs (DI), appsettings.json
tests/Orders.Tests         mirrors src structure
```
Dependency direction: Wpf → Application → Domain; Infrastructure → Application. Never the reverse.

## Conventions
- File-scoped namespaces, `nullable` enabled, `var` when the type is obvious, one type per file.
- Private fields `_camelCase`; async methods end with `Async` and take a `CancellationToken`.
- Public API gets XML doc comments. Test names: `Method_Scenario_Expected`.
- Money is `decimal`. Timestamps are `DateTimeOffset`. Business logic never calls `DateTime.Now` — inject `IClock`.
- Parse/format machine data with `CultureInfo.InvariantCulture`; UI formatting uses the current culture.
- Never swallow exceptions. Log with structured placeholders (`{OrderId}`), never string interpolation.
- No secrets in code or in this file. Configuration comes from `appsettings.json` / environment.

## WPF rules
- No logic in code-behind: Views bind to ViewModels (`ObservableObject` + `RelayCommand` in `ViewModels/Mvvm.cs`).
- Every window: `FlowDirection="RightToLeft"`, Hebrew labels, `MinWidth`/`MinHeight` set.
- Colors, fonts and styles come from `Themes/*.xaml` via `StaticResource`. Never hardcode colors in views.
- Spacing scale: 4 / 8 / 12 / 16. Standard WPF controls only unless asked.
- Include loading / empty / error states and `AutomationProperties.Name` on inputs and buttons.

## Commands
```
dotnet build
dotnet test
dotnet format --verify-no-changes
```

## Workflow for changes
1. Read the relevant tests and the interface first; follow the pattern in `Services/CustomerService.cs`.
2. Add or extend tests in `tests/Orders.Tests` before implementing.
3. Keep changes inside the layer that owns them; do not add NuGet packages without asking.
4. Run build + tests + format before reporting done. Show a short summary of what changed and why.
