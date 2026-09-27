<div dir="rtl">

# Lab 4 — Capstone: אפליקציית WPF שלמה בעזרת AI (75 דקות)

## המטרה
לבנות **"מעקב הוצאות" (Expense Tracker)** — אפליקציית WPF קטנה — ממפרט של עמוד אחד, מקצה לקצה עם כלי AI: XAML + ViewModel + שמירה ל-JSON + validation. ואז לסקור, לתקן ולשפר. זה מסכם את כל היום: prompt טוב, review, ארכיטקטורה, ו-UI.

## דרישות מקדימות
- ימים 3 ו-4. Windows עם .NET 10 SDK (ל-WPF). כלי AI עם גישה לקבצי הפרויקט (מומלץ).
- `Starter/` מכיל: csproj, `App.xaml`, `Themes/Colors.xaml`, `ViewModels/Mvvm.cs` (ObservableObject + RelayCommand), `SPEC.md`, ו-`CLAUDE.md` + `.github/copilot-instructions.md` מוכנים.

## המפרט (SPEC.md — זה מה שנותנים ל-AI)
ראו `Starter/Day4.Lab4.Starter/SPEC.md`. תמצית:
- חלון ראשי: טופס הוספה (תאריך, קטגוריה, סכום, הערה), טבלת הוצאות, סינון לפי חודש/קטגוריה, סיכום (סה"כ החודש, לפי קטגוריה), כפתורי מחיקה/שמירה.
- Validation עם `INotifyDataErrorInfo`: סכום > 0, קטגוריה חובה, תאריך לא עתידי, הערה עד 100 תווים.
- שמירה/טעינה מקובץ JSON ב-`%LOCALAPPDATA%\Day4Lab4\expenses.json` (System.Text.Json), דרך `IExpenseRepository`.
- RTL, עברית, סטיילים מ-`Themes/Colors.xaml`, מצבי loading/empty/error, `AutomationProperties`.

## שלבים

### שלב 1 — תכנון עם AI (10 דק')
Prompt:
<div dir="ltr">

```text
Read SPEC.md and CLAUDE.md. Propose the file list (Models, Services, ViewModels, Views) with one line each,
and the ViewModel's public members (properties + commands with types). Do not write code yet.
```

</div>
**דף עבודה:** תקנו את התוכנית: מה חסר (IExpenseRepository? validation base class?), מה מיותר.

### שלב 2 — מודל + repository + בדיקות (15 דק')
Prompt:
<div dir="ltr">

```text
Implement Models/Expense.cs (record), Services/IExpenseRepository.cs and Services/JsonExpenseRepository.cs
(System.Text.Json, async, atomic write via temp file + move, creates folder). Then write xUnit tests for the repository
using a temp folder. Follow CLAUDE.md. No new packages.
```

</div>
בדקו: `decimal`, `DateOnly`, `InvariantCulture`/JSON defaults, `CancellationToken`, אין `catch {}`.

### שלב 3 — ViewModel עם validation (15 דק')
<div dir="ltr">

```text
Implement ViewModels/ExpenseFormViewModel (INotifyDataErrorInfo) with the rules in SPEC.md, and
ViewModels/MainViewModel (ObservableCollection<Expense>, filters, totals, Add/Delete/Save/Load commands, IsBusy, ErrorMessage).
Inject IExpenseRepository. Commands must not be async void; use async Task methods wrapped by an AsyncRelayCommand you add to Mvvm.cs.
```

</div>
בדקו: `CanExecute` מתעדכן, אין `DateTime.Now` ב-ViewModel (הזריקו `TimeProvider`), חישובי סיכום ב-`decimal`.

### שלב 4 — XAML (15 דק')
<div dir="ltr">

```text
Generate Views/MainWindow.xaml for MainViewModel (members below). Grid layout: form on top (2 columns), filters row,
DataGrid, summary panel on the side, status bar. FlowDirection RightToLeft, Hebrew labels, styles from Themes/Colors.xaml
via StaticResource, Validation.ErrorTemplate showing the error under each field, empty state, ProgressBar for IsBusy,
AutomationProperties.Name everywhere. Standard WPF controls only. No code-behind logic.
{הדביקו את חתימות ה-ViewModel}
```

</div>
הריצו. תקנו binding errors (חלון Output ב-VS מציג אותם).

### שלב 5 — סקירה וליטוש (20 דק')
- [ ] צ'ק-ליסט מודול 4 על כל הקבצים + סקירת AI שנייה (תבנית 13).
- [ ] צ'ק-ליסט UI ממודול 7: RTL, מצבים, נגישות, עקביות סטיילים, אין namespace/control מומצא.
- [ ] `dotnet build` בלי אזהרות; בדיקות ה-repository עוברות.
- [ ] בקשו מה-AI הודעת commit; ערכו אותה.

## קריטריוני קבלה
- [ ] האפליקציה נבנית ורצה; הוספה/מחיקה/שמירה/טעינה עובדות; הנתונים שורדים הפעלה מחדש.
- [ ] Validation מוצג ליד השדה; כפתור "הוסף" מושבת כשיש שגיאות.
- [ ] סינון וסיכומים נכונים (בדקו ידנית 3 מקרים).
- [ ] אין לוגיקה ב-code-behind; `IExpenseRepository` מוזרק; אין `async void` פרט ל-event handlers.
- [ ] RTL ועברית תקינים; empty state מוצג כשאין הוצאות.
- [ ] דף העבודה + רשימת "מה ה-AI טעה ומה תיקנתי" (לפחות 5 פריטים).

## בונוס
- ייצוא ל-CSV (עם `InvariantCulture`) דרך `IExporter`.
- החלפת ה-repository ב-SQLite (`Microsoft.Data.Sqlite`) בלי לשנות את ה-ViewModel.
- DI מלא עם `Microsoft.Extensions.Hosting` כמו ב-`Demos/Day4.Demo.DiHostWpf`.

## רמזים
- אם ה-AI מייצר `PasswordBox`/`DatePicker` עם binding שגוי — `DatePicker.SelectedDate` הוא `DateTime?`; המירו ל-`DateOnly` ב-ViewModel.
- `ValidatesOnNotifyDataErrors=True` הוא ברירת המחדל ב-WPF מודרני, אבל `UpdateSourceTrigger=PropertyChanged` לא — בקשו במפורש.
- אם ה-DataGrid לא מתעדכן: `ObservableCollection`, לא `List`.

</div>
