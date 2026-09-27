# יום 3 — פיתוח GUI מהיר עם WPF

**קורס:** C# Programming in the .NET Framework — Updated Practical Training
**יום 3 מתוך 4** | 09:00–16:30

## על מה היום?

אחרי יומיים של קונסולה, מחלקות, LINQ, async ו-REST — היום נבנה **אפליקציות שולחניות** עם חלונות
אמיתיים. הטכנולוגיה המרכזית היא **WPF** (XAML + data binding + MVVM-lite), עם מבט קצר ל-**WinForms**
ככלי לפרוטוטייפים פנימיים. נחבר את ה-UI לקוד של הימים הקודמים (services, HttpClient, JSON), נלמד
לבנות טפסים עם ולידציה, ונסיים בטכניקות לבניית GUI מהר — הכנה ליום 4 (AI tools).

## מטרות למידה

בסוף היום תוכלו:

- להסביר את ההבדל בין WinForms, WPF, WinUI, MAUI ו-Avalonia, ולבחור נכון.
- לכתוב XAML: פריסה עם `Grid`/`StackPanel`/`DockPanel`, פקדים נפוצים, משאבים ו-Styles.
- לבנות UI רספונסיבי (star sizing, MinWidth, פריסה אדפטיבית) שלא קופא (`async`/`await`, `Dispatcher`).
- לתכנת מונחה-אירועים: routed events, `sender`/`e`, ‏`ICommand`, קיצורי מקלדת, `DispatcherTimer`.
- להשתמש ב-data binding: `INotifyPropertyChanged`, `ObservableCollection`, `DataTemplate`, converters, MVVM-lite.
- לחבר GUI ל-backend דרך services וממשקים, עם התקדמות, ביטול וטיפול בשגיאות.
- לאמת קלט (`ValidationRule`, `INotifyDataErrorInfo`), להשתמש בדיאלוגים ולבנות UI נגיש למקלדת.
- לבנות פרוטוטייפ מ-wireframe תוך פחות משעה עם Styles, UserControls ו-Hot Reload.

## דרישות מוקדמות

- ידע מימים 1–2: מחלקות וממשקים, `List<T>`/LINQ, חריגות, `async`/`await`, `HttpClient`, `System.Text.Json`.
- **Windows 10/11** + **Visual Studio 2022** עם ה-workload ‏**"‎.NET desktop development"** ו-.NET 10 SDK.
  הוראות התקנה: [`../00-Setup/INSTALL.md`](../00-Setup/INSTALL.md).
- אופציונלי: פרויקט `Day2-Async-APIs/Demos/Day2.LocalApi` מיום 2 (ל-Lab 3).

> הערה: כל הפרויקטים כאן **נבנים** גם ב-Linux/macOS (`EnableWindowsTargeting`), אבל **רצים** רק ב-Windows.

## לוח זמנים

| שעה | נושא | חומרים |
|-----|------|--------|
| 09:00–09:15 | פתיחה, חזרה על יום 2, מה בונים היום | — |
| 09:15–10:00 | **מודול 01** — מבוא ל-GUI ב-.NET, ‏WPF מול WinForms, XAML | [Notes/01](Notes/01-gui-intro-wpf-vs-winforms.md), `Demos/Day3.Demo.HelloWpf` |
| 10:00–10:45 | **מודול 02** — פריסה, פקדים, UI רספונסיבי | [Notes/02](Notes/02-layouts-controls-responsive.md), `Demos/Day3.Demo.Layouts` |
| 10:45–11:00 | הפסקה | |
| 11:00–11:45 | **Lab 1** — Unit Converter | [Labs/Lab1](Labs/Lab1-UnitConverter/README.md) |
| 11:45–12:15 | **מודול 03** — אירועים, routed events, ‏ICommand, timers | [Notes/03](Notes/03-events-and-event-driven.md), `Demos/Day3.Demo.Events` |
| 12:15–12:45 | **מודול 04** — Data binding ו-MVVM-lite | [Notes/04](Notes/04-data-binding-mvvm.md), `Demos/Day3.Demo.Binding` |
| 12:45–13:30 | ארוחת צהריים | |
| 13:30–13:50 | **מודול 05** — חיבור ל-backend: services, async, שגיאות, %AppData% | [Notes/05](Notes/05-connecting-backend.md), `Demos/Day3.Demo.AsyncUi` |
| 13:50–14:10 | **מודול 06** — ולידציה, דיאלוגים, מקלדת ונגישות | [Notes/06](Notes/06-validation-and-ux.md), `Demos/Day3.Demo.Validation` |
| 14:10–15:25 | **Lab 2** — Contacts Manager (MVVM + ולידציה + JSON) | [Labs/Lab2](Labs/Lab2-ContactsManager/README.md) |
| 15:25–15:35 | הפסקה | |
| 15:35–15:50 | **מודול 07** — פרוטוטייפינג מהיר: Styles, UserControls, Hot Reload, WinForms | [Notes/07](Notes/07-rapid-prototyping.md), `Demos/Day3.Demo.WinForms` |
| 15:50–16:20 | **Lab 3** או **Lab 4** (לבחירה; השני כשיעורי בית) | [Lab3](Labs/Lab3-ProductsApiClient/README.md) / [Lab4](Labs/Lab4-RapidDashboard/README.md) |
| 16:20–16:30 | סיכום, תרגילים לבית, הצצה ליום 4 | [Exercises](Exercises/README.md) |

## מודולים

1. [מבוא ל-GUI ב-.NET: ‏WPF מול WinForms](Notes/01-gui-intro-wpf-vs-winforms.md)
2. [פריסה, פקדים ו-UI רספונסיבי](Notes/02-layouts-controls-responsive.md)
3. [אירועים ותכנות מונחה-אירועים](Notes/03-events-and-event-driven.md)
4. [Data Binding ו-MVVM-lite](Notes/04-data-binding-mvvm.md)
5. [חיבור ה-GUI ללוגיקה ול-backend](Notes/05-connecting-backend.md)
6. [ולידציה ואינטראקציה עם המשתמש](Notes/06-validation-and-ux.md)
7. [טכניקות לפרוטוטייפינג מהיר](Notes/07-rapid-prototyping.md)

## דמואים (Demos/)

| פרויקט | מדגים |
|--------|-------|
| `Day3.Demo.HelloWpf` | מבנה פרויקט, XAML, x:Name, משאבים, code-behind, לוגיקה נפרדת |
| `Day3.Demo.Layouts` | Grid/Stack/Dock/Wrap/Canvas, פקדים, Menu/StatusBar, פריסה אדפטיבית ב-SizeChanged |
| `Day3.Demo.Events` | routed events (bubbling/tunneling), אירועים נפוצים, RelayCommand, InputBindings, DispatcherTimer |
| `Day3.Demo.Binding` | ObservableObject, ObservableCollection, DataTemplate, StringFormat, converters, MVVM-lite |
| `Day3.Demo.AsyncUi` | IsBusy, ProgressBar, ביטול, Dispatcher, שגיאות inline, הגדרות ב-%AppData% |
| `Day3.Demo.Validation` | ValidationRule, INotifyDataErrorInfo, ErrorTemplate, CanExecute, קלט מספרי, דיאלוגים |
| `Day3.Demo.WinForms` | טופס WinForms שנבנה בקוד (TableLayoutPanel, אירועים) |

הרצה (ב-Windows): `cd Demos/Day3.Demo.HelloWpf && dotnet run`.

## מעבדות (Labs/)

| מעבדה | משך | נושא |
|-------|-----|------|
| [Lab 1 — Unit Converter](Labs/Lab1-UnitConverter/README.md) | 45 דק' | אפליקציה ראשונה: Grid, ComboBox, TextChanged, ולידציה, Enter |
| [Lab 2 — Contacts Manager](Labs/Lab2-ContactsManager/README.md) | 75 דק' | MVVM-lite, DataGrid + טופס, INotifyDataErrorInfo, JSON |
| [Lab 3 — Products API Client](Labs/Lab3-ProductsApiClient/README.md) | 60 דק' | לקוח REST: Fake/Http service, async + התקדמות + ביטול, סינון, רספונסיבי |
| [Lab 4 — Rapid Dashboard](Labs/Lab4-RapidDashboard/README.md) | 60 דק' | מ-wireframe ל-dashboard: Styles, UserControl, DispatcherTimer, theme |

לכל מעבדה: `README.md` (הוראות), `Starter/` (שלד עם TODO), `Solution/` (פתרון + `NOTES.md`).

## תרגילים

[Exercises/README.md](Exercises/README.md) — 12 תרגילים קצרים לפי מודול. פתרונות ב-`Exercises/Solutions` (חלון תפריט שפותח כל תרגיל).

## מצגת

`Slides/Day3.pptx` (נבנית מ-`Slides/Day3.slides.js`, ראו `tools/slides/README.md`).

## בניית הכל (בדיקה)

```bash
# מכל תיקיית פרויקט:
dotnet build
# או לכל הפרויקטים של היום:
find Day3-GUI-WPF -name "*.csproj" -exec dotnet build {} \;
```
