# Prompt templates for C# / .NET developers (English)

Replace `{...}` placeholders. Always paste the relevant code/interfaces after the prompt.

## 1. Records/DTOs from JSON
```text
Generate C# records for the JSON below. Target .NET 10, System.Text.Json, nullable enabled.
PascalCase properties with [JsonPropertyName] only where the name differs.
Use decimal for money, DateTimeOffset for timestamps, enums for fixed string values (with JsonStringEnumConverter).
Output: code only.
{paste JSON}
```

## 2. LINQ from a sentence
```text
Write a LINQ query (method syntax) over {IEnumerable<T> description} that returns {result}.
Explain the complexity in one line and point out any assumption (e.g. duplicates, nulls).
```

## 3. MVVM ViewModel
```text
Convert this class into a WPF ViewModel: inherit ObservableObject (INotifyPropertyChanged + SetProperty),
expose ICommand properties using RelayCommand. No third-party MVVM libraries. No logic in code-behind.
{paste class + ObservableObject/RelayCommand if you have them}
```

## 4. Unit tests (xUnit)
```text
Write xUnit tests for the class below. Cover: happy path, boundary values, null/empty input, one exception case.
Use [Theory]/[InlineData] where natural. Name tests Method_Scenario_Expected. Do not change the class.
{paste class}
```

## 5. Tests first (TDD)
```text
Do NOT implement yet. Write xUnit tests for {interface/method} from this spec:
{spec bullets}
Then wait for my approval before implementing.
```

## 6. Refactor with behavior preserved
```text
Refactor the method below: extract well-named private methods, replace magic numbers with named constants,
remove duplication. Behavior and public signature must stay identical. Show the full refactored class,
then a 3-line summary of what changed.
{paste code}
```

## 7. Explain code
```text
Explain this code to a junior developer: what it does, line by line where non-obvious.
Then list the assumptions it makes about its inputs and what could fail in production.
{paste code}
```

## 8. Find the bug
```text
This code has a bug related to {async | culture | disposal | thread-safety | null | off-by-one}.
Symptom: {describe}. Find the most likely cause, explain it, and propose the minimal fix. Do not rewrite everything.
{paste code + failing test/error}
```

## 9. Regex
```text
Write a .NET regex for {pattern description}. Anchor it. Provide 5 matching and 5 non-matching examples,
and an xUnit [Theory] that verifies them. Use [GeneratedRegex] if appropriate.
```

## 10. Migration
```text
Migrate this code from {Newtonsoft.Json} to {System.Text.Json}. Keep the produced JSON identical.
List every behavioral difference you know of (casing, nulls, converters, dictionaries) and how you handled it.
{paste code}
```

## 11. XML docs
```text
Add XML documentation comments to public members: one clear sentence per summary, <param>/<returns> where useful,
<exception> for thrown exceptions. Do not document obvious getters. Do not change code.
{paste code}
```

## 12. Commit message / PR description
```text
Write a conventional-commit message for this diff. Subject ≤ 72 chars, imperative mood.
Body: what and why (not how), tests added, breaking changes if any.
{paste git diff}
```

## 13. Strict code review (second pass)
```text
Review the following C# as a strict senior reviewer. Check: null handling, async correctness (async void, .Result),
culture-sensitive parsing, IDisposable/HttpClient usage, thread safety, swallowed exceptions, injection/path traversal,
unnecessary dependencies. For each finding: severity, location, why, minimal fix. Do not rewrite the file.
{paste code}
```

## 14. WPF window (XAML)
```text
Generate {WindowName}.xaml only, no code-behind logic. .NET 10 WPF, MVVM.
DataContext: {ViewModel name} with these members: {list properties/commands with types}.
Layout: {rows/columns, which regions}. FlowDirection="RightToLeft", Hebrew labels.
Colors/styles via StaticResource keys defined in Window.Resources. Standard WPF controls only.
Include loading (IsBusy), empty and error states. Add AutomationProperties.Name to inputs and buttons.
```
