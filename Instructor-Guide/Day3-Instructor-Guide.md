<div dir="rtl">

# מדריך למרצה — יום 3: פיתוח GUI מהיר עם WPF

**קורס:** C# Programming in the .NET Framework — Updated Practical Training
**יום 3 מתוך 4** | 09:00–16:30 | תיקיית היום: [`../Day3-GUI-WPF`](../Day3-GUI-WPF/README.md)

## תקציר היום

היום עוברים מקונסולה לחלונות. הטכנולוגיה המרכזית היא **WPF** (XAML, פריסה, אירועים, data binding ו-MVVM-lite),
ובסוף היום יש מבט קצר על **WinForms** לכלים פנימיים מהירים. המסר שחוזר לאורך כל היום: **ה-UI הוא קליפה** מעל
הקוד של ימים 1–2 (מחלקות, LINQ, async, ‏HttpClient, JSON). במהלך היום יש 7 מודולים, 7 דמואים, 5 תרגולים קצרים בכיתה ו-3 מעבדות
(Lab 1, Lab 2, ו-Lab 3 **או** Lab 4). היום מסתיים בהכנה ליום 4 (כלי AI).

## מטרות למידה (מתוך README של היום)

בסוף היום הסטודנטים יוכלו:

1. להסביר את ההבדל בין WinForms, ‏WPF, ‏WinUI, ‏MAUI ו-Avalonia, ולבחור נכון.
2. לכתוב XAML: פריסה עם `Grid`/`StackPanel`/`DockPanel`, פקדים נפוצים, משאבים ו-Styles.
3. לבנות UI רספונסיבי (star sizing, ‏`MinWidth`, פריסה אדפטיבית) שלא קופא (`async`/`await`, ‏`Dispatcher`).
4. לתכנת מונחה-אירועים: routed events, ‏`sender`/`e`, ‏`ICommand`, קיצורי מקלדת, ‏`DispatcherTimer`.
5. להשתמש ב-data binding: ‏`INotifyPropertyChanged`, ‏`ObservableCollection`, ‏`DataTemplate`, converters, ‏MVVM-lite.
6. לחבר GUI ל-backend דרך services וממשקים, עם התקדמות, ביטול וטיפול בשגיאות.
7. לאמת קלט (`ValidationRule`, ‏`INotifyDataErrorInfo`), להשתמש בדיאלוגים ולבנות UI נגיש למקלדת.
8. לבנות פרוטוטייפ מ-wireframe בפחות משעה עם Styles, ‏UserControls ו-Hot Reload.

---

## הכנה לפני היום

### סביבה (יום לפני)

- [ ] מחשב המרצה: **Windows 10/11** + **Visual Studio 2022** עם ה-workload ‏**‎.NET desktop development** (ראו [`../00-Setup/INSTALL.md`](../00-Setup/INSTALL.md)).
- [ ] ‏.NET 10 SDK מותקן: `dotnet --list-sdks`.
- [ ] להריץ את [`../00-Setup/VerifySetup`](../00-Setup/VerifySetup) — וגם לבקש מהסטודנטים להריץ אותו מראש. **מי שאין לו Windows לא יוכל להריץ WPF** (רק לבנות, בזכות `EnableWindowsTargeting`). לתאם זוגות/VM מראש.
- [ ] לבנות את כל הקורס פעם אחת (מוריד חבילות ומחמם את ה-build):

<div dir="ltr">

```powershell
cd C:\c-
.\tools\build-all.ps1
```

</div>

- [ ] להריץ **כל** דמו פעם אחת (`dotnet run`) כדי לוודא שהוא עולה, ולסגור.
- [ ] לפתוח את המצגת [`../Day3-GUI-WPF/Slides/Day3.pptx`](../Day3-GUI-WPF/Slides/Day3.pptx) (55 שקפים; המקור: `Day3.slides.js`). מספרי השקפים במדריך הזה לפי הסדר בקובץ המקור.

### ניקוי מצב שמור (בוקר היום)

שני פרויקטים שומרים קבצים ב-`%AppData%` — כדאי לנקות כדי שהדמו יתחיל "נקי":

<div dir="ltr">

```powershell
Remove-Item -Recurse -Force "$env:APPDATA\Day3.Demo.AsyncUi" -ErrorAction SilentlyContinue
Remove-Item -Recurse -Force "$env:APPDATA\Day3.ContactsManager" -ErrorAction SilentlyContinue
```

</div>

- `Day3.Demo.AsyncUi` זוכר את הסינון, את "Simulate errors" ואת רוחב החלון מההרצה הקודמת.
- Lab 2 (גם Starter וגם Solution) קורא וכותב את **אותו** קובץ `%AppData%\Day3.ContactsManager\contacts.json`.

### מה לפתוח מראש

| חלון | תוכן | מתי |
|------|------|-----|
| Visual Studio | `Day3.Demo.HelloWpf` (ל-XAML Hot Reload ולתצוגת ה-Designer) | כל היום |
| טרמינל 1 | `C:\c-\Day3-GUI-WPF\Demos` — הרצת דמואים | כל היום |
| טרמינל 2 | `C:\c-\Day3-GUI-WPF\Exercises\Solutions` — תפריט פתרונות התרגילים | כל היום |
| טרמינל 3 | `Day2.LocalApi` רץ (ל-Lab 3, אם נבחר) | מ-15:35 |

הפעלת השרת של יום 2 (ל-Lab 3):

<div dir="ltr">

```powershell
cd C:\c-\Day2-Async-APIs\Demos\Day2.LocalApi
dotnet run
# מאזין על http://localhost:5080  — בדיקה: http://localhost:5080/api/products
```

</div>

### מה לתת לסטודנטים

- את כל התיקייה `Day3-GUI-WPF` (או `git pull`), ובמיוחד `Labs/*/Starter` ו-`Exercises/README.md`.
- להזכיר את הכלל מ-[`COURSE-OVERVIEW.md`](../00-Setup/COURSE-OVERVIEW.md): **מעתיקים את `Starter` לתיקיית עבודה** ועובדים על העותק:

<div dir="ltr">

```powershell
Copy-Item -Recurse C:\c-\Day3-GUI-WPF\Labs\Lab1-UnitConverter\Starter C:\my-work\Day3-Lab1
cd C:\my-work\Day3-Lab1
dotnet run
```

</div>

- לתרגילים: פרויקט WPF חדש אחד לכל היום, ומוסיפים לו חלון לכל תרגיל:

<div dir="ltr">

```powershell
dotnet new wpf -n Day3.Exercises
```

</div>

- להזכיר שב-VS **Output window** מציג שגיאות binding (`System.Windows.Data Error`) — כדאי שיהיה פתוח אצל כולם.

---

## לו"ז יומי

| שעה | סוג | נושא | חומרים |
|-----|-----|------|--------|
| 09:00–09:15 | פתיחה | פתיחה, חזרה על יום 2, מה בונים היום | שקפים 1–3 |
| 09:15–09:35 | הרצאה | **מודול 01** — מבוא ל-GUI ב-.NET, ‏WPF מול WinForms, XAML | [Notes/01](../Day3-GUI-WPF/Notes/01-gui-intro-wpf-vs-winforms.md), שקפים 4–10 |
| 09:35–09:45 | דמו | Hello WPF: מבנה פרויקט, x:Name, משאבים, code-behind | [`Demos/Day3.Demo.HelloWpf`](../Day3-GUI-WPF/Demos/Day3.Demo.HelloWpf) |
| 09:45–10:00 | תרגול | תרגיל 1 — Hello XAML ★ | [Exercises](../Day3-GUI-WPF/Exercises/README.md) |
| 10:00–10:20 | הרצאה | **מודול 02** — פריסה, פקדים, UI רספונסיבי, ה-UI thread | [Notes/02](../Day3-GUI-WPF/Notes/02-layouts-controls-responsive.md), שקפים 11–17 |
| 10:20–10:30 | דמו | Layouts: Grid/Panels/Controls/Responsive | [`Demos/Day3.Demo.Layouts`](../Day3-GUI-WPF/Demos/Day3.Demo.Layouts) |
| 10:30–10:45 | תרגול | תרגיל 2 — Grid keypad ★ (מהירים: תרגיל 3) | [Exercises](../Day3-GUI-WPF/Exercises/README.md) |
| 10:45–11:00 | הפסקה | | |
| 11:00–11:45 | מעבדה | **Lab 1** — Unit Converter (5 פתיחה + 35 עבודה + 5 סיכום) | [Lab1](../Day3-GUI-WPF/Labs/Lab1-UnitConverter/README.md), שקף 18 |
| 11:45–11:57 | הרצאה | **מודול 03** — אירועים, routed events, ‏ICommand, timers | [Notes/03](../Day3-GUI-WPF/Notes/03-events-and-event-driven.md), שקפים 19–24 |
| 11:57–12:05 | דמו | Events: bubbling/tunneling, Handled, RelayCommand, InputBindings, DispatcherTimer | [`Demos/Day3.Demo.Events`](../Day3-GUI-WPF/Demos/Day3.Demo.Events) |
| 12:05–12:15 | תרגול | תרגיל 4 — Counter events ★★ | [Exercises](../Day3-GUI-WPF/Exercises/README.md) |
| 12:15–12:30 | הרצאה | **מודול 04** — Data binding ו-MVVM-lite | [Notes/04](../Day3-GUI-WPF/Notes/04-data-binding-mvvm.md), שקפים 25–31 |
| 12:30–12:37 | דמו | Binding: ObservableObject, DataTemplate, converters, RelativeSource | [`Demos/Day3.Demo.Binding`](../Day3-GUI-WPF/Demos/Day3.Demo.Binding) |
| 12:37–12:45 | תרגול | תרגיל 7 — ElementName binding ★ | [Exercises](../Day3-GUI-WPF/Exercises/README.md) |
| 12:45–13:30 | הפסקה | ארוחת צהריים | |
| 13:30–13:40 | הרצאה | **מודול 05** — חיבור ל-backend: services, async, שגיאות, ‏%AppData% | [Notes/05](../Day3-GUI-WPF/Notes/05-connecting-backend.md), שקפים 32–37 |
| 13:40–13:48 | דמו | AsyncUi: IsBusy, Progress, Cancel, Block UI, Dispatcher, באנר שגיאה | [`Demos/Day3.Demo.AsyncUi`](../Day3-GUI-WPF/Demos/Day3.Demo.AsyncUi) |
| 13:48–14:00 | תרגול | תרגיל 10 — Async load ★★ | [Exercises](../Day3-GUI-WPF/Exercises/README.md) |
| 14:00–14:10 | הרצאה | **מודול 06** — ולידציה, דיאלוגים, מקלדת ונגישות | [Notes/06](../Day3-GUI-WPF/Notes/06-validation-and-ux.md), שקפים 38–43 |
| 14:10–14:17 | דמו | Validation: ValidationRule, INotifyDataErrorInfo, CanExecute, דיאלוגים | [`Demos/Day3.Demo.Validation`](../Day3-GUI-WPF/Demos/Day3.Demo.Validation) |
| 14:17–15:25 | מעבדה | **Lab 2** — Contacts Manager (68 דק', כולל 5 סיכום) | [Lab2](../Day3-GUI-WPF/Labs/Lab2-ContactsManager/README.md), שקף 44 |
| 15:25–15:35 | הפסקה | | |
| 15:35–15:43 | הרצאה | **מודול 07** — פרוטוטייפינג מהיר: Styles, UserControls, Hot Reload, WinForms | [Notes/07](../Day3-GUI-WPF/Notes/07-rapid-prototyping.md), שקפים 45–50 |
| 15:43–15:50 | דמו | XAML Hot Reload ב-VS + טופס WinForms בקוד | `Day3.Demo.HelloWpf` (ב-VS), [`Demos/Day3.Demo.WinForms`](../Day3-GUI-WPF/Demos/Day3.Demo.WinForms) |
| 15:50–16:20 | מעבדה | **Lab 4** (מומלץ) **או Lab 3** — השני כשיעורי בית | [Lab4](../Day3-GUI-WPF/Labs/Lab4-RapidDashboard/README.md) / [Lab3](../Day3-GUI-WPF/Labs/Lab3-ProductsApiClient/README.md), שקפים 52–53 |
| 16:20–16:30 | סיכום | שאלות חזרה, שיעורי בית, הצצה ליום 4 | שקפים 51, 54–55 |

> **מה השתנה לעומת ה-README הרשמי:** אותן שעות מסגרת, אותן הפסקות, ואותם גבולות מודולים/מעבדות. בתוך כל מודול
> חילקנו את הזמן ל"הרצאה → דמו → תרגול", והוספנו 5 חלונות תרגול (תרגילים 1, 2, 4, 7, 10). המחיר: המודולים
> 03–06 קוצרו בהרצאה (השקפים תמציתיים; ה-Notes הם החומר המלא), ו-Lab 2 קוצר מ-75 ל-68 דקות על חשבון
> תרגיל 10 (ראו סעיף "אם מאחרים").
> תרגילים שלא שובצו בכיתה (3, 5, 6, 8, 9, 11, 12) — ל"מי שסיים מהר" ולשיעורי בית; שקף 55 מגדיר את **5, 8, 11** כשיעורי בית.

---

## מתי מלמדים כל קובץ Notes

ה-Notes הם "הספר" של הקורס, ויש קובץ אחד לכל מודול. בכיתה **לא מקריאים אותם**. מלמדים מהשקפים ומהדמו, וה-Notes משמשים את המרצה כרשימת נושאים מלאה, ואת הסטודנטים לקריאה אחרי השיעור.
ב-Day 3 קובצי ה-Notes ארוכים וחלונות ההרצאה קצרים (8–20 דק'), ולכן הבחירה כאן סלקטיבית במיוחד: חלק מהסעיפים "נלמדים" רק דרך הדמו, וחלק נשארים לקריאה עצמית.

### מפת Notes ← שעה

| קובץ Notes | מתי מלמדים | זמן הרצאה | שקפים | דמו מיד אחרי | המעבדה שנשענת עליו |
|---|---|---|---|---|---|
| [01 — מבוא ל-GUI, ‏WPF מול WinForms](../Day3-GUI-WPF/Notes/01-gui-intro-wpf-vs-winforms.md) | **09:15–09:35** | 20 דק' | 4–10 | HelloWpf (09:35) | Lab 1 (11:00) |
| [02 — פריסה, פקדים, UI רספונסיבי](../Day3-GUI-WPF/Notes/02-layouts-controls-responsive.md) | **10:00–10:20** | 20 דק' | 11–17 | Layouts (10:20) | Lab 1 (11:00); ‏Lab 3 (פריסה אדפטיבית) |
| [03 — אירועים ותכנות מונחה-אירועים](../Day3-GUI-WPF/Notes/03-events-and-event-driven.md) | **11:45–11:57** (+ הצצה בפתיחת Lab 1, ‏11:00–11:05) | 12 דק' | 19–24 | Events (11:57) | Lab 1 (בדיעבד); ‏Lab 4 (`DispatcherTimer`) |
| [04 — Data Binding ו-MVVM-lite](../Day3-GUI-WPF/Notes/04-data-binding-mvvm.md) | **12:15–12:30** | 15 דק' | 25–31 | Binding (12:30) | Lab 2 (14:17) |
| [05 — חיבור ה-GUI ל-backend](../Day3-GUI-WPF/Notes/05-connecting-backend.md) | **13:30–13:40** | 10 דק' | 32–37 | AsyncUi (13:40) | Lab 3 (15:50 או בית); ‏Lab 2 (JSON ב-`%AppData%`) |
| [06 — ולידציה ואינטראקציה](../Day3-GUI-WPF/Notes/06-validation-and-ux.md) | **14:00–14:10** | 10 דק' | 38–43 | Validation (14:10) | Lab 2 (14:17) |
| [07 — פרוטוטייפינג מהיר](../Day3-GUI-WPF/Notes/07-rapid-prototyping.md) | **15:35–15:43** | 8 דק' | 45–50 | Hot Reload + WinForms (15:43) | Lab 4 (15:50) |

### פירוט לפי סעיפים בתוך כל קובץ

לכל קובץ: אילו סעיפים (לפי הכותרות בקובץ) מלמדים בכיתה, כמה דקות בערך לכל אחד, ומה נשאר לקריאה עצמית.

**Notes/01 — 09:15–09:35**

| שעה | סעיף בקובץ | דק' |
|---|---|---|
| 09:15–09:17 | למה בכלל אפליקציית שולחן עבודה? | 2 |
| 09:17–09:19 | הנוף: מה קיים ב-.NET | 2 |
| 09:19–09:21 | למה WPF בקורס הזה? | 2 |
| 09:21–09:23 | יצירת פרויקט (csproj: ‏`net10.0-windows`, ‏`UseWPF`, ‏`EnableWindowsTargeting`) | 2 |
| 09:23–09:26 | מבנה הפרויקט | 3 |
| 09:26–09:30 | **XAML בשתי דקות** (העיקר) | 4 |
| 09:30–09:31 | הכלים ב-Visual Studio (כולל Output window לשגיאות binding) | 1 |
| 09:31–09:34 | WPF מול WinForms: ההבדל במודל, לא רק במראה | 3 |
| 09:34–09:35 | דבר קטן על Dependency Properties | 1 |

בדמו (09:35): `App.xaml`, ‏`x:Name`, משאבים ו-code-behind מוצגים בקוד חי. לקריאה עצמית: מתי לא לבחור ב-WPF, מה קורה כשמריצים, טעויות נפוצות.

**Notes/02 — 10:00–10:20**

| שעה | סעיף בקובץ | דק' |
|---|---|---|
| 10:00–10:01 | מערכת הפריסה: אין קואורדינטות | 1 |
| 10:01–10:05 | **Grid — הסוס העבודה** (העיקר) | 4 |
| 10:05–10:08 | StackPanel, ‏DockPanel, ‏WrapPanel, ‏Canvas, ‏UniformGrid ו-Viewbox | 3 |
| 10:08–10:10 | Margin, Padding, Alignment | 2 |
| 10:10–10:12 | הפקדים הנפוצים | 2 |
| 10:12–10:14 | עיצוב רספונסיבי | 2 |
| 10:14–10:19 | UI רספונסיבי במובן השני: לא לחסום את ה-UI thread | 5 |
| 10:19–10:20 | Visibility: שלושה מצבים | 1 |

בדמו (10:20): הטאבים Panels ו-Responsive מראים את ה-Panels ואת `SizeChanged` בפועל. לקריאה עצמית: Border, GridSplitter ו-DPI, איך מדבגים פריסה, טעויות נפוצות.

**Notes/03 — 11:45–11:57 (קצב מהיר)**

הצצה מוקדמת ב-11:00–11:05 (פתיחת Lab 1, כדקה, לא נספרת ב-12 הדקות): מתוך "התבנית `sender` / `e`" ו"אירועים שתפגשו כל יום" — החתימה `(object sender, TextChangedEventArgs e)`, רישום `TextChanged="..."` ב-XAML, וה-guard ‏`if (!IsLoaded) return;`.

| שעה | סעיף בקובץ | דק' |
|---|---|---|
| 11:45–11:46 | המודל: התוכנית מחכה, המשתמש מוביל | 1 |
| 11:46–11:47 | התבנית `sender` / `e` (חזרה על ההצצה מ-Lab 1) | 1 |
| 11:47–11:50 | **Routed events: בעבוע ומנהור** (העיקר) | 3 |
| 11:50–11:51 | אירועים שתפגשו כל יום | 1 |
| 11:51–11:54 | ICommand: הפעולה כאובייקט | 3 |
| 11:54–11:55 | קיצורי מקלדת: InputBindings | 1 |
| 11:55–11:56 | טיימרים: DispatcherTimer | 1 |
| 11:56–11:57 | אירוע או Command? טבלת החלטה + Debounce + Lambda handlers והסרת רישום + סדר האירועים בפתיחת חלון (משפט אחד כל אחד) | 1 |

בדמו (11:57): bubbling/tunneling, ‏`Handled`, ‏`RelayCommand`, ‏InputBindings ו-`DispatcherTimer` ביומן האירועים. לקריאה עצמית: מה קורה בלולאת ההודעות, טעויות נפוצות, והפירוט המלא של ארבעת הנושאים שבשורה האחרונה.

**Notes/04 — 12:15–12:30 (קצב מהיר)**

| שעה | סעיף בקובץ | דק' |
|---|---|---|
| 12:15–12:16 | הבעיה עם code-behind | 1 |
| 12:16–12:18 | DataContext ו-{Binding} + Modes | 2 |
| 12:18–12:19 | שגיאות binding לא זורקות חריגה | 1 |
| 12:19–12:22 | **INotifyPropertyChanged: איך הפקד יודע שהערך השתנה** (העיקר) | 3 |
| 12:22–12:24 | ObservableCollection<T>: רשימות שמודיעות + ItemsSource + DataTemplate | 2 |
| 12:24–12:25 | StringFormat + Converters: IValueConverter | 1 |
| 12:25–12:27 | MVVM-lite: ארבע שכבות, בלי דוגמה + RelativeSource: לצאת מ-DataContext פנימי | 2 |
| 12:27–12:28 | CommunityToolkit.Mvvm: אותו דבר עם פחות קוד | 1 |
| 12:28–12:30 | FallbackValue, TargetNullValue ו-ElementName + CollectionView + איך מדבגים binding (בקצרה) | 2 |

בדמו (12:30): converters, ‏`RelativeSource AncestorType=Window` ושגיאת binding ב-Output window (`Sumary`). לקריאה עצמית: SelectedItem, SelectedValue ו-DisplayMemberPath, ‏Binding הוא חוזה — ובדיקות שומרות עליו, טעויות נפוצות.

**Notes/05 — 13:30–13:40 (דחוס בכוונה; הדמו ו-Lab 3 מכסים את השאר)**

| שעה | סעיף בקובץ | דק' |
|---|---|---|
| 13:30–13:31 | העיקרון: ה-UI הוא קליפה | 1 |
| 13:31–13:33 | שירותים מאחורי ממשק + DI-lite: הזרקה דרך הבנאי | 2 |
| 13:33–13:34 | async event handler מול async command (ב-ViewModel) | 1 |
| 13:34–13:36 | **IsBusy, ProgressBar, Cancel — השלישייה** (העיקר) | 2 |
| 13:36–13:37 | לשלב את קוד יום 2 (DTO נפרד מהמודל) | 1 |
| 13:37–13:38 | Dispatcher: כשאתם לא על ה-UI thread | 1 |
| 13:38–13:39 | טיפול בשגיאות: MessageBox או inline? | 1 |
| 13:39–13:40 | שמירה מקומית: JSON ב-%AppData% + במשפט: מבנה תיקיות מומלץ, מחזור חיים של חיבור | 1 |

בדמו (13:40): השלישייה, ‏Dispatcher, באנר השגיאה ו-`settings.json` ב-`%AppData%` בפועל. לקריאה עצמית: לבדוק את ה-ViewModel בלי חלון, לוגים: הפרטים למפתח, ההודעה למשתמש, מה עם offline?, טעויות נפוצות.

**Notes/06 — 14:00–14:10 (דחוס בכוונה; Lab 2 מכסה את השאר)**

| שעה | סעיף בקובץ | דק' |
|---|---|---|
| 14:00–14:01 | למה ולידציה ב-UI? + גישה 1: ולידציה ב-Submit | 1 |
| 14:01–14:02 | גישה 2: ValidationRule בתוך ה-Binding | 1 |
| 14:02–14:05 | **גישה 3: INotifyDataErrorInfo — ה-ViewModel מאמת** (העיקר) + הכפתור מושבת עד שהכל תקין | 3 |
| 14:05–14:06 | איך השגיאה נראית: Validation.ErrorTemplate | 1 |
| 14:06–14:07 | קלט מספרי | 1 |
| 14:07–14:08 | דיאלוגים (קבצים, דיאלוג מותאם, אישור — רק רשימה; מודגמים בדמו) | 1 |
| 14:08–14:09 | UI ידידותי למקלדת + ToolTips ונגישות | 1 |
| 14:09–14:10 | מתי לאמת: מיידי, ב-LostFocus או ב-Submit? + ולידציה בשכבות: UI, VM, שרת (בקצרה) | 1 |

בדמו (14:10): כל שלושת הדיאלוגים ו-`ErrorTemplate` ב-`App.xaml`. לקריאה עצמית: ולידציה אסינכרונית, הודעות בשפת המשתמש, רשימת בדיקה ל-UX של טופס, טעויות נפוצות.

**Notes/07 — 15:35–15:43 (קצב מהיר)**

| שעה | סעיף בקובץ | דק' |
|---|---|---|
| 15:35–15:36 | 1. מסקיצה ל-XAML | 1 |
| 15:36–15:38 | **2. Styles ו-ResourceDictionary: להגדיר פעם אחת** (העיקר) | 2 |
| 15:38–15:40 | 3. UserControl: רכיב לשימוש חוזר (כולל `propdp` מתוך 7. Snippets ו-scaffolding) | 2 |
| 15:40–15:41 | 4. Design-time data + 5. XAML Hot Reload + 6. ספריות UI מוכנות (במשפט כל אחד) | 1 |
| 15:41–15:42 | 8. WinForms: המעצב הגרפי לכלי חד-פעמי | 1 |
| 15:42–15:43 | תהליך עבודה מומלץ לפרוטוטייפ | 1 |

בדמו (15:43): XAML Hot Reload ב-VS וטופס WinForms בקוד (כולל "מתי לזרוק את הפרוטוטייפ" במשפט). לקריאה עצמית: מתי לזרוק את הפרוטוטייפ, שיפורי ביצועים מהירים, דוגמה: מ-wireframe לשלד ב-XAML (מומלץ לפני Lab 4), 9. הצצה ליום 4, טעויות נפוצות.

### מה אומרים לסטודנטים על ה-Notes

- **בפתיחה (09:00):** "יש קובץ Notes לכל מודול. לא צריך לקרוא מראש. בכיתה אני מלמד מהשקפים ומהדמו, וה-Notes הם הספר שלכם לחזרה. היום הם ארוכים במיוחד — בכיתה נעבור רק על העיקר."
- **בכל מעבדה:** "נתקעתם? לפני שאתם מציצים ב-Solution, חפשו את הנושא ב-Notes של המודול." למשל: Lab 1 ← Notes/01 + 02 (ולאירועים: "התבנית `sender` / `e`" ו"אירועים שתפגשו כל יום" ב-Notes/03), Lab 2 ← Notes/04 + 06 (ולשמירה: "שמירה מקומית: JSON ב-%AppData%" ב-Notes/05), Lab 3 ← Notes/05 (ולפריסה האדפטיבית: "עיצוב רספונסיבי" ב-Notes/02), Lab 4 ← Notes/07 (ובמיוחד "דוגמה: מ-wireframe לשלד ב-XAML"; ל-timer: "טיימרים: DispatcherTimer" ב-Notes/03). בתרגיל 10 מותר להעתיק את שלד ה-`LoadAsync` מ-Notes/05.
- **בסיכום (16:20):** קריאה לבית: הסעיפים "לקריאה עצמית" שלמעלה, ובמיוחד Notes 03–07 שעברו מהר. בכל קובץ יש סעיף "טעויות נפוצות" שכדאי לעבור עליו. לפני יום 4 — לקרוא את "9. הצצה ליום 4" ב-Notes/07.

---

## פירוט לפי בלוק

### 09:00–09:15 | פתיחה (שקפים 1–3)

- חזרה קצרה על יום 2: `async`/`await`, ‏`CancellationToken`, ‏`HttpClient` אחד לאפליקציה, ‏`System.Text.Json`. היום כל אלה "יקבלו חלון".
- שקף 2 — לוח הזמנים; שקף 3 — המטרות. להדגיש: 7 מודולים = 7 מטרות, ולכל אחת דמו ומעבדה.
- לוודא שכולם על Windows עם VS פתוח ושהפרויקטים נבנים. מי שאין לו Windows — לשבץ בזוג עכשיו, לא ב-11:00.
- **המסר הפותח:** "ה-UI הוא קליפה מעל הקוד שכבר כתבתם."

---

### 09:15–09:35 | הרצאה — מודול 01: מבוא ל-GUI ב-.NET (שקפים 4–10)

> 📖 **Notes:** [`Notes/01-gui-intro-wpf-vs-winforms.md`](../Day3-GUI-WPF/Notes/01-gui-intro-wpf-vs-winforms.md) · חלוקת הזמן לפי סעיפים: ראו [מתי מלמדים כל קובץ Notes](#מתי-מלמדים-כל-קובץ-notes)

**נקודות להוראה, לפי הסדר:**

1. למה עדיין desktop: כלים פנימיים, חומרה מקומית, offline, כלי אדמין.
2. הנוף (שקף 5): WinForms (2002), ‏WPF (2006), ‏WinUI 3, ‏.NET MAUI, ‏Avalonia — כולן חיות ב-.NET 10.
3. למה WPF בקורס (שקף 6): ה-UI הוא טקסט (XAML — טוב ל-git ול-AI ביום 4), binding ו-MVVM שעוברים ל-WinUI/MAUI/Avalonia, פריסה רספונסיבית מובנית.
4. יצירת פרויקט ו-csproj (שקף 7): ‏`net10.0-windows` (ולא `net10.0`), ‏`UseWPF`, ‏`OutputType=WinExe`, ו-`EnableWindowsTargeting` (בונה בכל מקום, רץ רק ב-Windows).
5. מבנה הפרויקט (שקף 8): `App.xaml` (משאבים גלובליים + `StartupUri`, אין `Main` ידני), ‏`MainWindow.xaml` + code-behind כ-`partial class`; ‏`InitializeComponent()` ו-`MainWindow.g.cs` שמייצר שדה לכל `x:Name`.
6. XAML בשתי דקות (שקף 9): אלמנט = מחלקה, attribute = property; property element syntax; שלושת ה-namespaces (`xmlns`, ‏`xmlns:x`, ‏`xmlns:local`); ‏`x:Name`; ‏`Resources` + `{StaticResource}` כ-markup extension; implicit style (Style בלי `x:Key`).
7. כלי VS (שקף 10): Designer, ‏XAML Hot Reload (נראה במודול 07), ‏Live Visual Tree, ו-**Output window לשגיאות binding**.
8. WPF מול WinForms — ההבדל במודל: מי מצייר (GDI מול DirectX/ControlTemplate), איך מגדירים UI (Designer.cs מול XAML), binding, ‏DPI.
9. בקצרה: Dependency Properties ("מקור הערך לפי עדיפויות, תומך ב-binding") — פרטים במודול 07.

**שאלות לכיתה:**
- "מי כאן תחזק אפליקציית WinForms? מה היה הכי כואב בה?"
- "אם `x:Class` הוא `MyApp.MainWindow` וה-namespace בקוד הוא `MyApp.Views` — מה יקרה?" (שגיאת build על `InitializeComponent`.)

**תפיסות שגויות נפוצות:**
- "WPF מת / WinForms מיושן ולא נתמך" — שתיהן נתמכות ב-.NET 10; הבחירה תלויה בהקשר.
- "`Name` ו-`x:Name` זהים" — כמעט; `x:Name` עובד תמיד, להיות עקביים.
- "ה-Designer הוא הדרך העיקרית לעבוד" — רוב המפתחים כותבים XAML ביד.

---

### 09:35–09:45 | דמו — `Day3.Demo.HelloWpf`

<div dir="ltr">

```powershell
cd C:\c-\Day3-GUI-WPF\Demos\Day3.Demo.HelloWpf
dotnet run
```

</div>

**צעד אחר צעד:**

1. להריץ: להקליד שם, Enter (הכפתור `IsDefault="True"`), ואז Escape (מנקה דרך `NameBox_KeyDown`). שם ריק → "Hello, guest!".
2. לפתוח `App.xaml`: ‏`StartupUri` ו-`AccentBrush` כמשאב **גלובלי**.
3. לפתוח `MainWindow.xaml`: ‏`Window.Resources` עם Style **ללא `x:Key`** לכל ה-Button-ים, שמפנה ל-`{StaticResource AccentBrush}` מ-App; ‏Grid עם `Auto`/`*`; ‏`_Say Hello` (Alt+S).
4. לפתוח `MainWindow.xaml.cs`: ‏`partial`, ‏`InitializeComponent()` ראשון, ‏`Hello_Click(object sender, RoutedEventArgs e)` — "החתימה הזו תחזור כל היום".
5. לפתוח `Greeter.cs`: לוגיקה נפרדת בלי `using System.Windows` → ניתנת לבדיקה ב-xUnit.
6. **שבירה מכוונת** (דקה): למחוק את `InitializeComponent()` ולהריץ — חלון ריק. להחזיר.

**להדגיש:** code-behind = "דבק" בלבד; הלוגיקה במחלקה רגילה. זו ההחלטה הראשונה שחוזרת ב-Lab 1 (`UnitConverter`).

---

### 09:45–10:00 | תרגול — תרגיל 1

| # | כותרת | רמה | מודול |
|---|-------|-----|-------|
| 1 | Hello XAML | ★ | 01–02 |

- **המשימה:** `TextBox`, כפתור `Greet` ו-`TextBlock`; בלחיצה או Enter (`IsDefault`) מציג `Hello, <שם>!` עם מונה לחיצות; גישה לפקדים דרך `x:Name`.
- **תשובה צפויה:** handler אחד שמגדיל מונה ומציב `Output.Text = $"Hello, {name}! (#{_clicks})"`, כאשר שם ריק → `World` (בדיקה: שדה ריק → `Hello, World!`).
- **מהירים:** להתחיל את תרגיל 3 (Login form).
- **הרצת הפתרון:**

<div dir="ltr">

```powershell
cd C:\c-\Day3-GUI-WPF\Exercises\Solutions
dotnet run
# בחלון התפריט: לבחור Ex01 ו-Open (או לחיצה כפולה)
```

</div>

---

### 10:00–10:20 | הרצאה — מודול 02: פריסה, פקדים, UI רספונסיבי (שקפים 11–17)

> 📖 **Notes:** [`Notes/02-layouts-controls-responsive.md`](../Day3-GUI-WPF/Notes/02-layouts-controls-responsive.md) · חלוקת הזמן לפי סעיפים: ראו [מתי מלמדים כל קובץ Notes](#מתי-מלמדים-כל-קובץ-notes)

**נקודות להוראה, לפי הסדר:**

1. אין קואורדינטות: panels מסדרים ילדים; שני מעברים — Measure ו-Arrange.
2. **Grid** (שקף 12): ‏`Auto` / פיקסלים / `*` ו-`2*`; ‏`Grid.Row`/`Grid.Column` כ-attached properties (ברירת מחדל 0); ‏`ColumnSpan`/`RowSpan`.
3. שאר ה-Panels (שקף 13): ‏StackPanel (לא מותח בכיוון הערימה — לא כ-root), ‏DockPanel (`LastChildFill`), ‏WrapPanel, ‏Canvas (ציור בלבד), ‏UniformGrid, ‏Viewbox.
4. Margin (בחוץ) / Padding (בפנים) / Alignment (`Stretch` ברירת מחדל; `Width` מפורש מבטל אותו) (שקף 14). כלל: לא לקבוע Width/Height, להשתמש ב-`MinWidth`.
5. הפקדים הנפוצים (שקף 15): ‏TextBlock מול TextBox, ‏PasswordBox (לא ניתן ל-binding בכוונה), ‏`IsChecked` הוא `bool?`, ‏ComboBox/ListBox/DataGrid עם `ItemsSource`, ‏`Content` יכול להיות כל דבר.
6. רספונסיבי במובן הגודל (שקף 16): star sizing, ‏`MinWidth` על החלון, ‏ScrollViewer (לא סביב DataGrid!), ‏Wrap/Trim, ‏WrapPanel, ‏Viewbox, ‏`SizeChanged` עם סף.
7. רספונסיבי במובן השני (שקף 17): **ה-UI thread** — handler סינכרוני ארוך = "Not Responding". ‏`async void` מותר רק ב-event handlers ועם try/catch; אחרי `await` חוזרים ל-UI thread; מ-`Task.Run` → ‏`Dispatcher.Invoke`; ‏`.Result`/`.Wait()` → deadlock.
8. `Visibility`: ‏`Visible`/`Hidden`/`Collapsed` — ב-99% רוצים `Collapsed` (רלוונטי ל-Lab 1).

**שאלות לכיתה:**
- "יש שלוש עמודות: `Auto`, ‏`2*`, ‏`*`. החלון ברוחב 600 והעמודה הראשונה תופסת 90. כמה מקבלת כל עמודה?" (340 ו-170.)
- "למה `.Result` על Task ב-handler תוקע את החלון לתמיד ולא רק לכמה שניות?"

**תפיסות שגויות נפוצות:**
- "StackPanel כ-root זה בסדר" — התוכן לא נמתח והכפתורים "מרחפים".
- "`Hidden` = לא קיים" — `Hidden` עדיין תופס מקום.
- "ScrollViewer מסביב ל-DataGrid משפר גלילה" — להפך: מבטל וירטואליזציה.
- "`Grid.Row="3"` בגריד של 3 שורות יזרוק שגיאה" — הוא נופל בשקט לשורה האחרונה.

---

### 10:20–10:30 | דמו — `Day3.Demo.Layouts`

<div dir="ltr">

```powershell
cd C:\c-\Day3-GUI-WPF\Demos\Day3.Demo.Layouts
dotnet run
```

</div>

**צעד אחר צעד (לפי הטאבים):**

1. **המסגרת:** ‏`DockPanel` עם `Menu` למעלה (`_File`, ‏`E_xit`, ‏`_Help → _About`) ו-`StatusBar` למטה; ה-`TabControl` ממלא את השאר (last child).
2. **טאב Grid:** עמודות `120` / `*` / `2*` ושורות `Auto` / `*` / `2*`. לגרור את פינת החלון — רק תאי ה-star משתנים.
3. **טאב Panels:** ‏StackPanel אופקי, ‏WrapPanel (להצר את החלון ולראות שבירת שורה), ‏DockPanel עם Top/Left/Right/Fill, ‏Canvas, ‏Viewbox.
4. **טאב Controls:** ‏Slider → ProgressBar ו-TextBlock דרך `ElementName` **בלי שורת קוד** (הכנה לתרגיל 7), ‏DataGrid עם `AutoGenerateColumns` מ-`SampleData.People()`.
5. **טאב Responsive:** להצר מתחת ל-600px — ‏StatusBar מציג `Mode: narrow`, והפאנל הצדדי עובר מעל התוכן. לפתוח `Window_SizeChanged`/`ApplyLayout` ולהראות `Grid.SetColumn`/`SetRow` (attached property מהקוד) ואת הבדיקה "רק כשחוצים את הסף".
6. להראות `MinWidth="420" MinHeight="380"` על ה-Window — "מתחת לזה כל פריסה נשברת, אז לא מאפשרים".

**להדגיש:** פריסה נכונה = רספונסיביות "בחינם"; `SizeChanged` רק כשצריך שינוי מבני.

---

### 10:30–10:45 | תרגול — תרגיל 2 (מהירים: 3)

| # | כותרת | רמה | מודול |
|---|-------|-----|-------|
| 2 | Grid keypad | ★ | 01–02 |
| 3 | Login form (למהירים) | ★ | 01–02 |

- **תרגיל 2 — תשובה צפויה:** ‏Grid 4×4 עם star sizing; תצוגה ב-`ColumnSpan="4"`, ‏`0` על שתי עמודות, ‏`OK` על שתי שורות; **handler אחד** `Button.Click="Key_Click"` על ה-Grid שמשתמש ב-`e.OriginalSource is Button b`. בדיקה: הגדלת החלון מגדילה את כל הכפתורים באופן אחיד.
  - שימו לב: הרמז (bubbling ו-`e.OriginalSource`) שייך בעצם למודול 03 — לתת אותו כ"הצצה", ולהזכיר אותו ב-11:45.
- **תרגיל 3 — תשובה צפויה:** ‏DockPanel (כותרת למעלה, כפתורים למטה) + StackPanel לשדות; `<Label Content="_User name" Target="{Binding ElementName=UserBox}" />` (Alt+U), ‏PasswordBox, ‏`IsDefault`/`IsCancel`, ‏`TabIndex`.
- **הרצת הפתרון:** מתפריט הפתרונות — Ex02 / Ex03.

---

### 10:45–11:00 | הפסקה

---

### 11:00–11:45 | מעבדה — Lab 1: Unit Converter (שקף 18)

[README](../Day3-GUI-WPF/Labs/Lab1-UnitConverter/README.md) · [Solution/NOTES.md](../Day3-GUI-WPF/Labs/Lab1-UnitConverter/Solution/NOTES.md) · משך רשמי 45 דק' · ★

**חלוקת זמן:** 11:00–11:05 פתיחה · 11:05–11:40 עבודה · 11:40–11:45 סיכום.

**מטרה:** ממיר יחידות אורך שעובד בלחיצה על Convert, "חי" בזמן הקלדה ובשינוי יחידה, עם הודעת שגיאה אדומה במקום קריסה.

**פתיחה (5 דק') — חשוב:** המעבדה משתמשת ב-`TextChanged`, ‏`SelectionChanged` ו-`KeyDown`, שמודול 03 מלמד רק אחרי המעבדה.
לכן בפתיחה להראות בדקה את החתימה `(object sender, TextChangedEventArgs e)`, איך מוסיפים `TextChanged="..."` ב-XAML (VS מציע "New Event Handler"), ואת ה-guard ‏`if (!IsLoaded) return;`.

**מה יש ב-Starter:** ‏`LengthUnit.cs` (מוכן, עם `MetersPerUnit` ו-`LengthUnit.All`), ‏`UnitConverter.cs` (TODO 1–2), ‏`MainWindow.xaml` עם Grid זמני (TODO 3), ‏`MainWindow.xaml.cs` (TODO 4–7).

**השלבים בקצרה:**

1. TODO 1 — `Convert`: ‏`value * from.MetersPerUnit / to.MetersPerUnit`.
2. TODO 2 — `TryParseInput`: ‏`double.TryParse(..., NumberStyles.Float, CultureInfo.InvariantCulture, ...)`; ריק / לא-מספר / שלילי → שגיאה בעברית.
3. TODO 3 — Grid של 3 עמודות (`Auto`, ‏`*`, ‏`Auto`) ו-6 שורות (5 `Auto` + אחת `*`), עם `x:Name` זהים לקוד: `ValueBox`, ‏`FromUnit`, ‏`ToUnit`, ‏`ErrorText`, ‏`ResultText`, ‏`ConvertButton`.
4. TODO 4 — `ItemsSource = LengthUnit.All` ו-`SelectedIndex` בבנאי.
5. TODO 5 — ‏`TextChanged` ו-`SelectionChanged` שקוראים ל-`Convert()`.
6. TODO 6 — `Convert()`: ולידציה → `ShowError` או הצגה בפורמט `10 m = 32.8084 ft`; שדה ריק רק מנקה.
7. TODO 7 — Escape מנקה (`KeyDown`); בונוס: כפתור `⇄` (tuple swap).

**קריטריוני קבלה:**
- נבנה ללא אזהרות ורץ.
- `10` עם Meter→Foot מציג `10 m = 32.8084 ft` בלי לחיצה.
- `abc` → הודעה אדומה, אין קריסה, ‏Convert מושבת.
- שינוי יחידה מעדכן מיד; Enter ממיר, Escape מנקה.
- בהרחבת החלון: התוצאה ממורכזת וה-TextBox נמתח.

**איפה נתקעים, ומה הרמז:**

| תקלה | רמז |
|------|-----|
| שגיאת build "The name 'ValueBox' does not exist" | ה-`x:Name` ב-XAML חייב להתאים בדיוק לשמות בקוד. |
| `NullReferenceException` בהפעלה | ‏`SelectionChanged` נורה כבר בבנאי כשמגדירים `SelectedIndex` — ‏`if (!IsLoaded) return;` |
| `1,5` לא עובר / עובר אחרת אצל כל אחד | ‏culture — ‏`InvariantCulture` (והפתרון גם מחליף פסיק בנקודה). |
| הודעת השגיאה "תופסת מקום" גם כשהיא ריקה | ‏`Visibility.Collapsed`, לא `Hidden`. |
| פקד "נעלם" או נערם על אחר | לספור `RowDefinitions`; פקד בשורה שלא קיימת נופל לאחרונה בשקט. |
| שדה ריק צועק שגיאה מיד | להציג שגיאה רק כשהוקלד משהו; ריק = ניקוי תוצאה. |
| מספרים ארוכים נחתכים | ‏`TextWrapping="Wrap"` על `ResultText`. |

**סיכום (5 דק') — החלטות מ-NOTES.md לדיון:**
- `UnitConverter`/`LengthUnit` בלי `using System.Windows` → ניתנים לבדיקת יחידה; code-behind = דבק.
- **כל הדרכים מובילות ל-`Convert()`** (Click, ‏TextChanged, ‏SelectionChanged, ‏Swap) — אין שכפול.
- `IsDefault="True"` במקום טיפול ידני ב-Enter.
- "שדה ריק ≠ שגיאה" — חוויית משתמש.
- `InvariantCulture` + `IsLoaded` guard.
- טיפ מה-NOTES: להראות בכוונה מה קורה **בלי** `IsLoaded` ובלי ולידציה (הקלדת אותיות), ואז לתקן.

**בונוס למהירים:** קטגוריית משקל (g, kg, lb, oz) עם ComboBox קטגוריה; ‏`Clipboard.SetText` בלחיצה כפולה על התוצאה; פרויקט xUnit ל-`Convert`/`TryParseInput`.

**הרצת הפתרון:**

<div dir="ltr">

```powershell
cd C:\c-\Day3-GUI-WPF\Labs\Lab1-UnitConverter\Solution
dotnet run
```

</div>

---

### 11:45–11:57 | הרצאה — מודול 03: אירועים ותכנות מונחה-אירועים (שקפים 19–24)

> 📖 **Notes:** [`Notes/03-events-and-event-driven.md`](../Day3-GUI-WPF/Notes/03-events-and-event-driven.md) · חלוקת הזמן לפי סעיפים: ראו [מתי מלמדים כל קובץ Notes](#מתי-מלמדים-כל-קובץ-notes)

**נקודות להוראה, לפי הסדר:**

1. המודל (שקף 20): התוכנית **מחכה** בלולאת הודעות; אין flow ראשי, יש אוסף תגובות. אירוע = multicast delegate מיום 1.
2. `sender` (מי הפעיל) ו-`e` (מה קרה): ‏`KeyEventArgs.Key`, ‏`CancelEventArgs.Cancel`, ‏`SizeChangedEventArgs.NewSize`.
3. Routed events (שקף 21): **Tunneling** (`Preview*`, מה-root למטה, רץ קודם) → **Bubbling** (`Click`, ‏`KeyDown`, למעלה) ; Direct. ‏`e.Handled = true` עוצר; `sender` מול `e.OriginalSource` (חזרה לתרגיל 2).
4. האירועים היומיומיים (שקף 22): ‏`Loaded` (מקום לטעינת נתונים), ‏`Closing` (ניתן לביטול), ‏`Closed` (ניקוי), ‏`SelectionChanged` נורה גם מהקוד (חזרה ל-Lab 1).
5. ‏`ICommand` (שקף 23): ‏`Execute`/`CanExecute`/`CanExecuteChanged`; ‏`RelayCommand` עם `CommandManager.RequerySuggested`; הכפתור **משבית את עצמו**; ‏`CommandManager.InvalidateRequerySuggested()` אחרי `await`.
6. קיצורים ו-timer (שקף 24): ‏`Window.InputBindings`/`KeyBinding` (עובד רק עם commands), ‏`IsDefault`/`IsCancel`, ‏`_` ל-Alt, ‏`InputGestureText` (תצוגה בלבד); ‏`DispatcherTimer` רץ על ה-UI thread — להבדיל מ-`System.Timers.Timer`.
7. בקצרה מה-Notes: טבלת "אירוע או Command?", ‏debounce עם `DispatcherTimer`, דליפת זיכרון מ-handler על אובייקט ארוך-חיים, וסדר האירועים בפתיחת חלון (בנאי → `Loaded` → `ContentRendered`).

**שאלות לכיתה:**
- "יש `PreviewMouseDown` על ה-Border ו-`Click` על כפתור בתוכו. מי נרשם קודם ביומן?" (ה-Preview — ‏tunneling.)
- "למה לא למדוד שעון עצר לפי מספר ה-Tick-ים של `DispatcherTimer`?" (Tick לא מדויק ומתעכב כשה-UI עמוס → מצטברת סטייה; מודדים עם `Stopwatch`. זו גם השאלה של תרגיל 6.)

**תפיסות שגויות נפוצות:**
- "`sender` הוא תמיד הכפתור שנלחץ" — ב-handler על הורה, `sender` הוא ההורה; הכפתור הוא `e.OriginalSource`.
- "כל timer מתאים ל-UI" — רק `DispatcherTimer` רץ על ה-UI thread.
- "צריך `IsEnabled` ידני על כפתורים" — ‏`CanExecute` עושה את זה.
- "רישום ב-XAML וגם `+=` בקוד לא מזיק" — הפעולה תרוץ פעמיים.

---

### 11:57–12:05 | דמו — `Day3.Demo.Events`

<div dir="ltr">

```powershell
cd C:\c-\Day3-GUI-WPF\Demos\Day3.Demo.Events
dotnet run
```

</div>

**צעד אחר צעד** (יומן האירועים בצד ימין מראה הכל עם חותמת זמן):

1. בפתיחה: היומן מציג `Loaded`, והשעון (`DispatcherTimer`) מתחיל לתקתק.
2. **Button A:** ביומן מופיעים לפי הסדר `Border.PreviewMouseDown (tunneling)` → ‏`ButtonA.Click — sender=BtnA` → ‏`Border got bubbled Click from BtnA`.
3. **Button B (Handled=true):** אין שורת "Border got bubbled" — ‏`e.Handled` עצר את הבעבוע.
4. להקליד בתיבה: ‏`TextChanged` על כל תו; הכפתור **Save (Ctrl+S)** "מתעורר" (`CanExecute` = `_dirty`). ‏Ctrl+S או Enter → ‏`Saved ✔` והכפתור שוב מושבת.
5. ‏Ctrl+L מנקה את היומן (ו-Clear log מושבת כשהיומן ריק).
6. לשנות צבע ב-ComboBox: ‏`SelectionChanged` — ולהראות בקוד את `if (IsLoaded)` (אותו guard כמו ב-Lab 1).
7. להקליד משהו ולסגור את החלון: ‏`Closing` שואל "You have unsaved changes. Close anyway?" — ‏No מבטל את הסגירה (`e.Cancel`).

**להדגיש בקוד:** ‏`DataContext = this` (הקדמה ל-binding), ‏`new RelayCommand(Save, () => _dirty)`, ו-`Window.InputBindings` ב-XAML.

---

### 12:05–12:15 | תרגול — תרגיל 4

| # | כותרת | רמה | מודול |
|---|-------|-----|-------|
| 4 | Counter events | ★★ | 03 |

- **המשימה:** מונה עם `+`/`−`; ‏`KeyDown` על החלון (חיצים), ‏`Loaded` שמציג רמז, ‏`Closing` שמבקש אישור אם המונה ≠ 0.
- **תשובה צפויה:** `Window_KeyDown` שבודק `Key.Up`/`Key.Down`, קורא ל-`Change(±1)` ומסמן `e.Handled = true`; ‏`Window_Closing` עם `e.Cancel = MessageBox.Show(...) != MessageBoxResult.Yes` רק כש-`_count != 0`.
- **מהירים:** תרגיל 5 (אותו מונה עם `RelayCommand`, ‏`CanExecute` בטווח 0–10 ו-Ctrl+↑/↓/R) — הוא גם שיעורי בית.
- **הרצת הפתרון:** מתפריט הפתרונות — Ex04 (וגם Ex05 להמחשה).

---

### 12:15–12:30 | הרצאה — מודול 04: Data Binding ו-MVVM-lite (שקפים 25–31)

> 📖 **Notes:** [`Notes/04-data-binding-mvvm.md`](../Day3-GUI-WPF/Notes/04-data-binding-mvvm.md) · חלוקת הזמן לפי סעיפים: ראו [מתי מלמדים כל קובץ Notes](#מתי-מלמדים-כל-קובץ-notes)

**נקודות להוראה, לפי הסדר:**

1. הבעיה עם code-behind: "סלט" של עדכונים ידניים, אי אפשר לבדוק בלי חלון.
2. `DataContext` (עובר בירושה במורד העץ) ו-`{Binding Path}` (שקף 26); ‏Modes: ‏OneWay/TwoWay/OneTime/OneWayToSource; ‏`UpdateSourceTrigger=PropertyChanged` על `TextBox.Text` (ברירת המחדל היא `LostFocus`).
3. **שגיאות binding לא זורקות חריגה** — רק הודעה ב-Output window (`System.Windows.Data Error: 40`).
4. `INotifyPropertyChanged` דרך `ObservableObject` עם `SetProperty` ו-`[CallerMemberName]` (שקף 27); property מחושב צריך `OnPropertyChanged(nameof(...))` ידני.
5. `ObservableCollection<T>` (שקף 28) — מודיע על Add/Remove, **לא** על שינוי בתוך פריט; רק מה-UI thread. ‏`DataTemplate`, ‏`DisplayMemberPath`.
6. `StringFormat` וה-escape `{}`; ‏Converters (`IValueConverter`, שקף 29), ‏`ConverterParameter`, ‏`BooleanToVisibilityConverter` המובנה.
7. MVVM-lite (שקף 30): View → ViewModel → Service → Model; ה-VM לא מכיר פקדים; services מאחורי ממשקים; DI-lite בבנאי.
8. `RelativeSource AncestorType=Window` כדי לצאת מ-DataContext פנימי (יחזור ב-Lab 2!).
9. `CommunityToolkit.Mvvm` (שקף 31): אותו דבר עם source generators — "בפרויקט אמיתי קחו את ה-toolkit; היום כותבים ידנית כדי להבין".
10. בקצרה: ‏`FallbackValue`/`TargetNullValue`, ‏`ICollectionView` לסינון ומיון (יחזור ב-Lab 2 וב-Lab 3), ו"מה ה-DataContext כאן?" כשאלת דיבאג ראשונה.

**שאלות לכיתה:**
- "הוספתי פריט ל-`List<T>` שקשור ל-ListBox — למה הוא לא מופיע?"
- "בתוך `DataTemplate`, מה ה-DataContext? ואיך אגיע לפקודה של החלון?"

**תפיסות שגויות נפוצות:**
- "`ObservableCollection` מעדכן גם כשמשנים property של פריט" — לא; הפריט צריך להיות `ObservableObject`.
- "אם ה-binding שגוי, התוכנית תקרוס ואדע" — היא תשתוק. Output window.
- "MVVM = אסור שורה אחת ב-code-behind" — מותר "UI טהור" (SizeChanged, פוקוס, דיאלוגים) ויצירת ה-VM.
- "TextBox מעדכן את ה-VM בכל הקשה" — רק עם `UpdateSourceTrigger=PropertyChanged`.

---

### 12:30–12:37 | דמו — `Day3.Demo.Binding`

<div dir="ltr">

```powershell
cd C:\c-\Day3-GUI-WPF\Demos\Day3.Demo.Binding
dotnet run
```

</div>

**צעד אחר צעד:**

1. שלושה פריטים התחלתיים; "Drink coffee" מסומן כבוצע (ירוק דרך `BoolToBrushConverter`). הסיכום למטה: `1 / 3 done`.
2. לסמן CheckBox ברשימה → הצבע והסיכום מתעדכנים. להראות ב-`MainViewModel` את `OnItemsChanged`/`OnItemPropertyChanged` → ‏`RaiseSummary()` (property מחושב = הודעה ידנית).
3. להקליד כותרת חדשה: ‏Add מושבת כל עוד השדה ריק — בזכות `UpdateSourceTrigger=PropertyChanged` + `CanExecute`. ‏Enter מוסיף (`IsDefault`).
4. לבחור פריט: פאנל Details עם `DataContext="{Binding Selected}"`; לשנות Title — הרשימה מתעדכנת בזמן אמת; ‏Slider של Priority → ‏`PriorityToTextConverter` ("High/Normal/Low").
5. להראות ב-XAML את `Remove` עם `RelativeSource AncestorType=Window` ואת `IsEnabled` של הפאנל שקשור ל-`DataContext.HasSelection`.
6. **שבירה מכוונת:** לשנות ב-XAML `{Binding Summary}` ל-`{Binding Sumary}`, להריץ מ-VS ולהראות את ההודעה ב-Output window.
7. לציין: ‏`d:DataContext` / `mc:Ignorable="d"` בראש הקובץ — design-time data (יחזור במודול 07).

**להדגיש:** ‏`MainViewModel` אין בו אף `using System.Windows.Controls` — ניתן לבדיקה בלי חלון.

---

### 12:37–12:45 | תרגול — תרגיל 7

| # | כותרת | רמה | מודול |
|---|-------|-----|-------|
| 7 | ElementName binding | ★ | 04 |

- **המשימה:** אפס קוד C#: ‏Slider שמשנה `Width`/`Height` של Rectangle, ו-TextBox שה-TextBlock מתחתיו מציג את תוכנו ואת מספר התווים.
- **תשובה צפויה:** `Width="{Binding ElementName=Size, Path=Value}"` (וכך `Height`), ו-`Text="{Binding ElementName=Input, Path=Text.Length, StringFormat={}{0} characters}"` — שימו לב ל-`{}` כשהפורמט מתחיל ב-`{0}`.
- **מהירים:** תרגיל 8 (Shopping list) או 9 (Converter) — 8 הוא גם שיעורי בית.
- **הרצת הפתרון:** מתפריט הפתרונות — Ex07.

---

### 12:45–13:30 | ארוחת צהריים

להזכיר למי שבוחר ב-Lab 3 בסוף היום: אפשר להריץ את `Day2.LocalApi` כבר עכשיו (ראו "הכנה").

---

### 13:30–13:40 | הרצאה — מודול 05: חיבור ה-GUI ל-backend (שקפים 32–37)

> 📖 **Notes:** [`Notes/05-connecting-backend.md`](../Day3-GUI-WPF/Notes/05-connecting-backend.md) · חלוקת הזמן לפי סעיפים: ראו [מתי מלמדים כל קובץ Notes](#מתי-מלמדים-כל-קובץ-notes)

**נקודות להוראה, לפי הסדר:**

1. "ה-UI הוא קליפה" (שקף 33): ‏`IProductService` עם `FakeProductService` (פיתוח/בדיקות/כיתה בלי אינטרנט) ו-`HttpProductService` (הקוד מיום 2); DI-lite בבנאי; ‏`HttpClient` אחד סטטי.
2. `async void` ב-handler מול `AsyncRelayCommand` ב-VM (דגל `_running` מונע לחיצה כפולה).
3. **השלישייה** (שקף 34): ‏`IsBusy` + ‏`Progress<int>` (שנוצר על ה-UI thread) + ‏`CancellationTokenSource` (גם timeout); ‏`OperationCanceledException` אינו שגיאה; ‏`finally { IsBusy = false; }`.
4. DTO נפרד מהמודל — כשה-API משתנה, משנים DTO והמרה ולא XAML.
5. Dispatcher (שקף 35): אחרי `await` — UI thread; מ-`Task.Run`/callbacks/`System.Timers.Timer` — ‏`Dispatcher.Invoke`; והכי פשוט: `await Task.Run(...)`.
6. שגיאות (שקף 36): MessageBox לאישורים/קטלני; **באנר inline עם Retry** לשגיאות רשת. הודעה = מה קרה + מה לעשות; פרטים טכניים ללוג.
7. ‏JSON ב-`%AppData%` (שקף 37): ‏`SpecialFolder.ApplicationData`; קובץ פגום → ברירת מחדל, לא קריסה; טוענים בפתיחה, שומרים ב-`Closing`.
8. בקצרה: מבנה תיקיות (Models/Services/ViewModels/...), בדיקת VM בלי חלון (דוגמת xUnit ב-Notes), ומתי טוענים (ב-`Loaded`, לא בבנאי).

**שאלות לכיתה:**
- "איפה צריך ליצור את ה-`Progress<T>` — ולמה זה משנה?"
- "המשתמש לחץ Cancel. להציג לו הודעת שגיאה אדומה?" (לא — "Cancelled" בסטטוס.)

**תפיסות שגויות נפוצות:**
- "`async` = רץ על thread אחר" — ‏`await` משחרר את ה-UI thread, והקוד חוזר אליו.
- "`GetAwaiter().GetResult()` זה בסדר ב-`Closing`" — deadlock (התבנית הנכונה ב-Lab 2).
- "כותבים הגדרות ליד ה-exe" — ב-Program Files אין הרשאות כתיבה.

---

### 13:40–13:48 | דמו — `Day3.Demo.AsyncUi`

<div dir="ltr">

```powershell
cd C:\c-\Day3-GUI-WPF\Demos\Day3.Demo.AsyncUi
dotnet run
```

</div>

**צעד אחר צעד:**

1. **Load (async):** ‏ProgressBar מתקדם (10 ערים × 300ms), ‏Load מושבת ו-Cancel פעיל. **להזיז את החלון בזמן הטעינה** — הוא חי.
2. **Load ואז Cancel** באמצע → סטטוס `Cancelled`.
3. **Block UI (bad!):** ‏`Thread.Sleep(3000)` — לנסות להזיז את החלון: קפוא, ואולי "Not Responding". זה הרגע שנזכרים בו.
4. **Background thread → Dispatcher:** אחרי 1.5 שניות הסטטוס מציג את מספר ה-thread. להראות בקוד את השורה המוערת `StatusText.Text = result;` שהייתה זורקת `InvalidOperationException`.
5. לסמן **Simulate errors** ולטעון כמה פעמים: לעיתים מופיע **באנר אדום inline עם Retry** (לא MessageBox).
6. להקליד סינון (למשל `ha`), לשנות רוחב חלון, לסגור ולפתוח מחדש: הסינון, תיבת הסימון והרוחב **נזכרים** — ‏`%AppData%\Day3.Demo.AsyncUi\settings.json` (לפתוח את הקובץ).

**להדגיש בקוד:** ה-property ‏`IsBusy` שמרכז את מצב הכפתורים וה-ProgressBar במקום אחד, ו-`new Progress<int>(p => Progress.Value = p)` שנוצר לפני ה-`await`.
(הדמו כתוב ב-code-behind בכוונה, לפשטות; ב-Lab 3 אותה תבנית עוברת ל-ViewModel.)

---

### 13:48–14:00 | תרגול — תרגיל 10

| # | כותרת | רמה | מודול |
|---|-------|-----|-------|
| 10 | Async load | ★★ | 05 |

- **המשימה:** כפתור Load שמפעיל `FakeLoadAsync` (10 פריטים, `Task.Delay(300)` ביניהם) עם `IProgress<int>` ל-ProgressBar, כפתור Cancel עם `CancellationTokenSource`, ומצב כפתורים נכון.
- **תשובה צפויה:** `async void Load_Click` עם try / `catch (OperationCanceledException) { Status.Text = "Cancelled"; }` / finally שמחזיר את הכפתורים ועושה `Dispose` ל-CTS; ‏`FakeLoadAsync` מעביר את ה-token ל-`Task.Delay(300, ct)` ומדווח `progress.Report(i * 10)`. בדיקה: אפשר להזיז את החלון בזמן הטעינה; Cancel לא מפיל.
- **טיפ לזמן:** זה תרגיל צפוף ל-12 דקות — לאפשר העתקה של שלד ה-`LoadAsync` מ-Notes/05 ולהתמקד בחיבור ה-token וה-Progress.
- **הרצת הפתרון:** מתפריט הפתרונות — Ex10.

---

### 14:00–14:10 | הרצאה — מודול 06: ולידציה ואינטראקציה (שקפים 38–43)

> 📖 **Notes:** [`Notes/06-validation-and-ux.md`](../Day3-GUI-WPF/Notes/06-validation-and-ux.md) · חלוקת הזמן לפי סעיפים: ראו [מתי מלמדים כל קובץ Notes](#מתי-מלמדים-כל-קובץ-notes)

**נקודות להוראה, לפי הסדר:**

1. למה ולידציה ב-UI: משוב מיידי, ליד השדה, בשפה אנושית (השרת עדיין חייב לאמת).
2. שלוש רמות (שקף 39): ‏(1) ב-Submit — פשוט, שגיאה אחת בכל פעם; ‏(2) `ValidationRule` בתוך ה-Binding — רץ **לפני** שהערך מגיע ל-source, ה-VM לא יודע; ‏(3) `INotifyDataErrorInfo` ב-VM — המומלץ ל-MVVM.
3. `ValidatableObject` (שקף 40): מילון `property → errors`, ‏`SetErrors`, ‏`ErrorsChanged`, ‏`HasErrors` — ‏`CanExecute = () => !HasErrors`.
4. `Validation.ErrorTemplate` (שקף 41): ‏`AdornedElementPlaceholder`, ‏adorner layer (להשאיר Margin תחתון), ‏Trigger על `Validation.HasError` ל-ToolTip; ‏`(Validation.Errors)/ErrorContent`.
5. קלט מספרי: ‏`PreviewTextInput` (tunneling) **וגם** `TryParse` — כי הדבקה עוקפת.
6. דיאלוגים (שקף 42): ‏`OpenFileDialog`/`SaveFileDialog` (`ShowDialog() == true` כי `bool?`), דיאלוג מותאם עם `Owner` ו-`DialogResult`, ‏`MessageBox` עם ברירת מחדל `No` לפעולה הרסנית.
7. מקלדת ונגישות (שקף 43): סדר Tab, ‏`Label.Target` + `_`, ‏`IsDefault`/`IsCancel`, פוקוס ראשוני, ‏ToolTip, ‏`AutomationProperties.Name` לכפתורי אייקון, לא להסתמך על צבע בלבד.
8. בקצרה: מתי לאמת (מיידי / LostFocus / Submit) ו"ולידציה בשכבות" — החוקים במודל (כמו `Contact` ב-Lab 2) ולא שלוש פעמים.

**שאלות לכיתה:**
- "למה `PreviewTextInput` לבד לא מספיק לשדה מספרי?"
- "ב-`ValidationRule` הקלט לא תקין — מה הערך ב-VM?" (הערך הקודם; לא עבר ל-source. רואים את זה בדמו.)

**תפיסות שגויות נפוצות:**
- "כפתור מושבת זה הסבר מספיק" — צריך גם הודעה/ToolTip למה.
- "טופס ריק צריך להיות אדום מההתחלה" — לא לצעוק לפני שהמשתמש התחיל.
- "`MessageBox.Show` בלי Owner זה אותו דבר" — עלול להיפתח מאחורי החלון.

---

### 14:10–14:17 | דמו — `Day3.Demo.Validation`

<div dir="ltr">

```powershell
cd C:\c-\Day3-GUI-WPF\Demos\Day3.Demo.Validation
dotnet run
```

</div>

**צעד אחר צעד (לפי הטאבים):**

1. **ValidationRule:** לנסות להקליד אותיות — חסומות (`PreviewTextInput`). להקליד `150` → מסגרת אדומה + `!` עם ToolTip, והשורה "Value in source" **נשארת** על הערך התקין האחרון. להדביק אותיות (Ctrl+V) — ה-Rule תופס.
2. **INotifyDataErrorInfo:** ‏Save מושבת בטופס ריק (`!HasErrors && Touched`). למלא שם של תו אחד → "Name must be at least 2 characters"; אימייל שגוי; גיל 200. לתקן הכל → ‏Save נדלק; Enter שומר (`IsDefault`) והסטטוס הירוק מופיע. להראות Alt+N / Alt+E / Alt+A (`Label.Target`).
3. להראות ב-`App.xaml` את ה-`ErrorTemplate` המשותף וה-Style עם ה-Trigger על `Validation.HasError`.
4. **Dialogs:** ‏Open file…, ‏Save file…, ‏Custom dialog… (`NameDialog` עם `Owner` ו-`DialogResult = true`), ו-**Delete (confirmation)** — להראות שברירת המחדל היא No (Enter לא מוחק).

**להדגיש:** הטאב השני הוא בדיוק התבנית של Lab 2 (`ValidatableObject` + `CanExecute`).

---

### 14:17–15:25 | מעבדה — Lab 2: Contacts Manager (שקף 44)

[README](../Day3-GUI-WPF/Labs/Lab2-ContactsManager/README.md) · [Solution/NOTES.md](../Day3-GUI-WPF/Labs/Lab2-ContactsManager/Solution/NOTES.md) · משך רשמי 75 דק' (כאן 68) · ★★

**חלוקת זמן מומלצת:**

| שעה | חלק | TODO |
|-----|-----|------|
| 14:17–14:20 | פתיחה: מבנה הפרויקט + ציור עץ ה-DataContext על הלוח (חלון → VM; טופס → `Draft`) | — |
| 14:20–14:32 | א' — המודל מודיע | 1 |
| 14:32–14:45 | ב' — פקודות | 2 |
| 14:45–15:03 | ג' — ולידציה | 3, 4 |
| 15:03–15:14 | ד' — חיפוש ושמירה | 5, 6 |
| 15:14–15:20 | ה' — ה-View | 7 (‏8 בונוס) |
| 15:20–15:25 | סיכום | — |

**מטרה:** ניהול אנשי קשר ב-MVVM-lite: ‏DataGrid משמאל, טופס פרטים מימין, הוספה/עריכה/מחיקה, חיפוש, ולידציה עם `INotifyDataErrorInfo`, ושמירה/טעינה של JSON ב-`%AppData%`.

**מה יש ב-Starter:** ‏`Mvvm/ObservableObject.cs` ו-`Mvvm/RelayCommand.cs` מוכנים; ‏`Services/IContactsStore.cs` (ממשק); ה-Style של שגיאות ב-`App.xaml` מוכן. ה-TODO-ים: `Models/Contact.cs` (1, 4), ‏`Mvvm/ValidatableObject.cs` (3a/3b), ‏`ViewModels/ContactsViewModel.cs` (2, 2b, 2c, 5), ‏`Services/JsonContactsStore.cs` (6a/6b), ‏`MainWindow.xaml` (7a, 7b, 8).

**השלבים בקצרה:**

1. TODO 1 — `SetProperty` בכל ה-setters; ב-`FirstName`/`LastName` גם `OnPropertyChanged(nameof(FullName))`.
2. TODO 2 — ‏`CanExecute`: ‏Delete/Cancel רק עם בחירה, ‏Apply רק כש-`Draft is { HasErrors: false }`, ‏Save רק כש-`IsDirty`; ‏Delete בוחר שכן; ‏Apply = ‏`Selected.CopyFrom(Draft)` + `MarkDirty()` + `ContactsView.Refresh()`.
3. TODO 3 — `GetErrors`/`SetErrors` (הסרת מפתח כשהרשימה ריקה, ‏`ErrorsChanged`, הודעה על `HasErrors`).
4. TODO 4 — ולידציה מה-setters: שם פרטי חובה; טלפון חובה 9–15 ספרות; אימייל אופציונלי אך תקין (`MailAddress.TryCreate`).
5. TODO 5 — `ContactsView.Filter` (שם/טלפון/אימייל, לא תלוי רישיות) ו-`SortDescription` לפי שם.
6. TODO 6 — `LoadAsync`/`SaveAsync` עם `System.Text.Json`; ‏`Directory.CreateDirectory`; ‏`ValidateAll()` אחרי טעינה.
7. TODO 7 — עמודות מפורשות ב-DataGrid (`DataGridCheckBoxColumn` ל-★) וטופס פרטים מלא + Cancel. TODO 8 (בונוס) — Ctrl+N, ‏Ctrl+S, ‏Delete.

**קריטריוני קבלה:**
- Add יוצר איש קשר חדש שנבחר אוטומטית והטופס מתמלא.
- עריכה לא משנה את הרשימה עד Apply; ‏Cancel מחזיר ערכים מקוריים.
- שם ריק / טלפון קצר / אימייל שגוי → מסגרת אדומה, ToolTip, ‏Apply מושבת.
- Delete מעביר בחירה לשכן; הכותרת מציגה `*` כשיש שינויים.
- Save כותב ל-`%AppData%\Day3.ContactsManager\contacts.json`; פתיחה מחדש טוענת.
- Search מסנן בזמן הקלדה; סגירה עם שינויים שואלת אם לשמור.

**איפה נתקעים, ומה הרמז:**

| תקלה | רמז |
|------|-----|
| הרשימה ריקה בהרצה הראשונה ("איפה Dana Levi מה-wireframe?") | אין נתוני seed — הקובץ עוד לא קיים. מתחילים ב-Add. |
| כפתור Apply בטופס "לא עושה כלום" / מושבת תמיד | ה-DataContext של הטופס הוא `Draft`; צריך `{Binding DataContext.ApplyCommand, RelativeSource={RelativeSource AncestorType=Window}}`. לצייר את העץ. |
| כפתורים לא מתעדכנים עד שעוזבים את השדה | ‏`UpdateSourceTrigger=PropertyChanged` על ה-TextBox-ים. |
| שינוי שם לא מופיע בכותרת הטופס | חסר `OnPropertyChanged(nameof(FullName))` (property מחושב). |
| המיון/הסינון לא מתעדכן אחרי Apply | ‏`ContactsView.Refresh()` — ה-view לא מאזין לשינויי property של פריטים. |
| חריגה בסריאליזציה / `HasErrors` בקובץ | ‏`[JsonIgnore]` על `HasErrors` ועל `FullName`. |
| איש קשר עם טלפון ריק נטען "תקין" | ‏`ValidateAll()` אחרי טעינה — `SetProperty` לא מאמת ערך שזהה לברירת המחדל. |
| החלון קופא בסגירה | `.GetAwaiter().GetResult()` ב-`Closing` = deadlock. התבנית: `e.Cancel = true; await SaveAsync(); Close();` (ב-Solution). |
| "הנתונים שלי השתנו מעצמם" | Starter ו-Solution חולקים את אותו `contacts.json` ב-`%AppData%`. |
| שדה ריק ולא קורה כלום | לפתוח Output window ב-VS ולחפש `System.Windows.Data Error`. |

**סיכום (5 דק') — החלטות מ-NOTES.md לדיון:**
- **מודל "חכם"** (`Contact` יורש מ-`ValidatableObject`) — פשרה מודעת של MVVM-lite; בפרויקט גדול מפרידים Model ו-VM לעריכה.
- **Draft** (`Selected.Clone()`) — מה שמאפשר Cancel; Apply מעתיק חזרה.
- **`ICollectionView`** במקום שתי רשימות; ‏`Refresh()` אחרי Apply.
- **`[JsonIgnore]`** על `HasErrors` ו-`FullName`; **`ValidateAll()`** אחרי טעינה.
- **Closing + async** — למה `GetResult()` נתקע (ה-continuation מחכה ל-Dispatcher החסום).
- **DI-lite:** ‏`MainWindow` בונה `JsonContactsStore` ומזריק ל-VM → בבדיקות אפשר store בזיכרון.

**בונוס למהירים:** ייצוא CSV עם `SaveFileDialog`; מיון בלחיצה על כותרת עם `ICollectionView`; מונה תוצאות מסוננות בשורת המצב; TODO 8.

**הרצת הפתרון:**

<div dir="ltr">

```powershell
cd C:\c-\Day3-GUI-WPF\Labs\Lab2-ContactsManager\Solution
dotnet run
```

</div>

---

### 15:25–15:35 | הפסקה

להפעיל את `Day2.LocalApi` בטרמינל 3 אם הולכים ל-Lab 3.

---

### 15:35–15:43 | הרצאה — מודול 07: פרוטוטייפינג מהיר (שקפים 45–50)

> 📖 **Notes:** [`Notes/07-rapid-prototyping.md`](../Day3-GUI-WPF/Notes/07-rapid-prototyping.md) · חלוקת הזמן לפי סעיפים: ראו [מתי מלמדים כל קובץ Notes](#מתי-מלמדים-כל-קובץ-notes)

**נקודות להוראה, לפי הסדר (מהיר — 8 דקות):**

1. מסקיצה ל-XAML (שקף 46): טבלת התרגום (כותרת/סטטוס → DockPanel, טופס → Grid ‏`Auto`/`*`, כרטיסים שווים → UniformGrid, רשימה+פרטים → Grid + GridSplitter). **קודם מבנה, אחר כך צבעים.**
2. Styles + ResourceDictionary (שקף 47): ‏`x:Key` מול implicit, ‏`BasedOn`, ‏`Style.Triggers`, ‏ControlTemplate קצר; הפרדת צבעים (Light/Dark) מסגנונות; ‏`MergedDictionaries[0]` להחלפת theme; ‏**`DynamicResource` לצבעי theme**.
3. UserControl עם DependencyProperty (שקף 48): ‏`x:Name="Root"` + `ElementName=Root`; ‏`propdp` ב-VS.
4. עוד מאיצים (שקף 49): ‏XAML Hot Reload (עובד ל-XAML, לא ל-code-behind), ‏`d:DataContext`, ספריות UI (MaterialDesignInXAML, ‏MahApps.Metro, ‏WPF UI), תיקיית `Mvvm/` לשימוש חוזר.
5. WinForms (שקף 50): ‏`TableLayoutPanel` = ה-Grid של WinForms; מתי כן (כלי פנימי מחר בבוקר) ומתי לא.
6. תהליך עבודה מומלץ לפרוטוטייפ: 10 סקיצה → 15 שלד → 20 נתונים מזויפים → 15 ליטוש.

**שאלה לכיתה:**
- "החלפתי צבע ב-`Resources[...]` וחצי מהחלון לא השתנה. מה החשוד?" (‏`StaticResource`.)

**תפיסות שגויות נפוצות:**
- "UserControl עם property רגילה יתמוך ב-binding" — לא; צריך DP, ואחרת נכשל בשקט.
- "Hot Reload לא עובד" — בדרך כלל שיניתם code-behind.

---

### 15:43–15:50 | דמו — XAML Hot Reload + `Day3.Demo.WinForms`

**Hot Reload (4 דק') — ב-Visual Studio:**
1. לפתוח `Day3.Demo.HelloWpf` ב-VS ולהריץ עם **F5**.
2. בזמן ריצה לשנות ב-`App.xaml` את `Color="#512BD4"` של `AccentBrush` לצבע אחר ולשמור — הכפתורים והברכה משתנים בלי restart.
3. לשנות `Margin`/`FontSize` ב-`MainWindow.xaml` — מתעדכן מיד. לנסות לשנות את `Greeter.cs` — להראות שזה **לא** אותו דבר (C# Hot Reload / restart).

**WinForms (3 דק'):**

<div dir="ltr">

```powershell
cd C:\c-\Day3-GUI-WPF\Demos\Day3.Demo.WinForms
dotnet run
```

</div>

1. להקליד משימה — Add מושבת כשהשדה ריק; Enter מוסיף (`AcceptButton`).
2. לבחור פריט → ‏Remove נדלק → אישור ב-MessageBox.
3. לפתוח `MainForm.cs`: ‏`TableLayoutPanel` עם `ColumnStyles`/`RowStyles`, ‏`Dock`, אירועים עם lambdas, ו-`_list.Items.Add` — **הנתונים חיים בפקד**, בלי binding. "זה מה שה-Designer של VS מייצר ל-`Form1.Designer.cs`."

**להדגיש:** זה מהיר מאוד לכלי פנימי — ובדיוק "סימן לזרוק" את הפרוטוטייפ אם הוא אמור לגדול (Notes/07).

---

### 15:50–16:20 | מעבדה — Lab 4 (מומלץ) או Lab 3 (שקפים 52–53)

**איך לבחור:**

| שיקול | Lab 4 — Rapid Dashboard | Lab 3 — Products API Client |
|-------|-------------------------|-----------------------------|
| קשר למודול שהרגע נלמד | ישיר (מודול 07) | מודולים 02 ו-05 |
| תלות חיצונית | אין | אופציונלי: ‏`Day2.LocalApi` רץ |
| מה אפשר להספיק ב-30 דק' | TODO 1–4 (מתוך 45 דק' ליבה) | חלקים א'–ב' (מתוך 60) |
| מתאים ל... | כיתה עייפה / חזותית; הכנה טובה ליום 4 | כיתה חזקה ו"backend-ית" |

ההמלצה: **Lab 4 בכיתה** (אפשר להדגים שוב Hot Reload), **Lab 3 כשיעורי בית**. השני תמיד נשאר לבית (שקף 55).

#### Lab 4 — Rapid Dashboard

[README](../Day3-GUI-WPF/Labs/Lab4-RapidDashboard/README.md) · [Solution/NOTES.md](../Day3-GUI-WPF/Labs/Lab4-RapidDashboard/Solution/NOTES.md) · משך רשמי 60 דק' (45 + 15 בונוס) · ★★

**חלוקת זמן בכיתה:** 15:50–15:53 הצגת ה-wireframe · 15:53–16:15 עבודה (TODO 1–4, ומהירים 5–6) · 16:15–16:20 הדגמת הפתרון + Hot Reload.

**מטרה:** מ-wireframe ASCII ל-dashboard "חי": ‏Styles, ‏UserControl ‏`StatCard`, ‏TabControl, ‏`DispatcherTimer` שמזין נתונים מזויפים; בונוס — theme בהיר/כהה בזמן ריצה.

**מה יש ב-Starter:** ‏`Themes/Light.xaml` (צבעים, מוכן), ‏`Services/FakeMetricsService.cs` (מוכן), ‏`ViewModels/DashboardViewModel.cs` (מוכן: `Tick()`, ‏`Snapshot`, ‏`RecentOrders`, ‏`UsersHistory`, ‏`IsDark` + אירוע `ThemeChanged`), ‏`StatCard` עם DP של `Title` בלבד. **אין** `Themes/Dark.xaml` — יוצרים אותו בבונוס.

**השלבים בקצרה:**
1. TODO 1 (5 דק') — ב-`Themes/Styles.xaml`: ‏`H1`, ‏`Muted`, ‏`AccentButton` (Border מעוגל + ContentPresenter + Trigger ל-`IsMouseOver`), צבעים ב-`DynamicResource`.
2. TODO 2 (10 דק') — DP ל-`Value`, ‏`Subtitle`, ‏`Accent` (להעתיק את תבנית `Title`), ובניית הכרטיס עם `{Binding X, ElementName=Root}`.
3. TODO 3 (5 דק') — שורת כותרת (`LastUpdated`, ‏CheckBox ‏Live, כפתור theme) ו-`DispatcherTimer` שקורא ל-`_vm.Tick()` כל שנייה, נעצר ב-`Closed`.
4. TODO 4 (10 דק') — `UniformGrid Columns="4"` עם 4 `StatCard` על `Snapshot.*`; ‏Revenue עם `StringFormat={}{0:C0}`.
5. TODO 5 — גרף: ‏ItemsControl על `UsersHistory`, ‏StackPanel אופקי, ‏Rectangle עם `{Binding Height}`.
6. TODO 6 — DataGrid על `RecentOrders` (חדשות למעלה).
7. TODO 7 (בונוס) — `Themes/Dark.xaml` עם אותם מפתחות, ‏`App.ApplyTheme` (החלפת `MergedDictionaries[0]`), ‏`_vm.ThemeChanged += App.ApplyTheme`, ‏Slider ל-`RefreshSeconds` שמשנה את `_timer.Interval`.

**קריטריוני קבלה:** ארבעה כרטיסים שווים שמתעדכנים כל שנייה, ‏Live כבוי עוצר; הגרף זז; טאב Orders מציג חדשות למעלה (עד 25); הקטנת החלון לא שוברת (יש ScrollViewer); בונוס — 🌙/☀ מחליף theme בלי לפתוח חלון מחדש.

**איפה נתקעים, ומה הרמז:**

| תקלה | רמז |
|------|-----|
| הכרטיס מציג ריק / מחפש ב-DataContext של החלון | ‏`x:Name="Root"` על ה-UserControl ו-`ElementName=Root` בכל binding פנימי. |
| `Value="{Binding ...}"` לא עובד על StatCard | חייב להיות DependencyProperty; לבדוק את ה-owner type ב-`Register` (`typeof(StatCard)`). |
| שגיאת XAML ב-Revenue | ‏`StringFormat={}{0:C0}` — ה-`{}` בהתחלה. |
| החלפת theme לא משנה חלק מהצבעים | ‏`StaticResource` במקום `DynamicResource`. |
| החלון לא מתעדכן | הטיימר לא הופעל ב-`Loaded`/בבנאי, או ש-Live כבוי. |
| הכרטיסים לא ברוחב שווה | ‏`UniformGrid` (ולא StackPanel). |

**החלטות מ-NOTES.md לדיון:**
- שלושה מילונים — Light / Dark / Styles; החלפת theme = ‏`MergedDictionaries[0]` (כך עובדות גם ספריות UI).
- ‏`StatCard` עם DP ו-`ElementName=Root` — לא "גונב" את ה-DataContext של ההורה.
- **ה-Timer בחלון, לא ב-VM:** ‏`DispatcherTimer` הוא מושג UI; ה-VM חושף `Tick()` (ניתן לקריאה ידנית בבדיקה); ‏`ThemeChanged` הוא אירוע כי ה-VM לא מכיר את `App`.
- גרף "ידני" (ItemsControl + Rectangle) מספיק לפרוטוטייפ; לגרפים אמיתיים — LiveCharts2, ‏ScottPlot, ‏OxyPlot.
- ‏`Snapshot` הוא record immutable — ‏`OnPropertyChanged(nameof(Snapshot))` אחד מעדכן את כל ה-`Snapshot.X`.
- **שאלת דיון:** "מה היה קורה אם `Tick()` היה נקרא מ-`System.Timers.Timer`?" (cross-thread exception.)

**בונוס למהירים:** TODO 7 (Dark theme + Slider לקצב רענון).

**הרצת הפתרון:**

<div dir="ltr">

```powershell
cd C:\c-\Day3-GUI-WPF\Labs\Lab4-RapidDashboard\Solution
dotnet run
```

</div>

#### Lab 3 — Products API Client

[README](../Day3-GUI-WPF/Labs/Lab3-ProductsApiClient/README.md) · [Solution/NOTES.md](../Day3-GUI-WPF/Labs/Lab3-ProductsApiClient/Solution/NOTES.md) · משך רשמי 60 דק' · ★★

**אם נבחר בכיתה (30 דק'):** 15:50–15:53 פתיחה · 15:53–16:15 חלקים א' (TODO 1/1b/1c) ו-ב' (TODO 2/2b) · 16:15–16:20 הדגמת הפתרון מול `Day2.LocalApi` וכיבוי השרת באמצע. חלקים ג'–ה' לבית.

**מטרה:** לקוח שולחני לקטלוג מוצרים: טעינה אסינכרונית עם התקדמות וביטול, שגיאות inline עם Retry, חיפוש/סינון, ופריסה שמתאימה את עצמה לרוחב. עובד **גם בלי שרת** (Fake).

**מה יש ב-Starter:** ‏`Models/Product.cs`, ‏`Services/IProductService.cs`, ‏`Services/FakeProductService.cs` (מוכן: 60 מוצרים ב-6 מנות, איטי, יכול להיכשל), ‏`AsyncRelayCommand` ב-`Mvvm/RelayCommand.cs`, ‏`BoolToVis` ב-`App.xaml`, ה-Style ‏`Card`. ב-ComboBox של השירותים יש שלושה מימושים: Fake, ‏Fake שנכשל (`FailRandomly = true`), ו-HTTP מול `http://localhost:5080/api/products`.

**השלבים בקצרה:**
1. TODO 1 — `LoadAsync`: ‏CTS, ‏`IsBusy`, ‏`Progress<int>`, ‏`Status`; 1b — ProgressBar בשורת המצב (`BoolToVis`); 1c — שכבת "טוען…" חצי-שקופה עם Cancel.
2. TODO 2 — ‏`OperationCanceledException` → "Cancelled"; ‏`HttpRequestException` → הודעה בעברית ב-`Error`; ‏`finally { IsBusy = false; }`; 2b — באנר inline עם Retry.
3. TODO 3 — `ProductsView.Filter = o => o is Product p && Matches(p)` ו-`Refresh()` בכל שינוי של `Search`/`Category`/`OnlyInStock`.
4. TODO 4 — `HttpProductService`: ‏`GetAsync` + `EnsureSuccessStatusCode` + `ReadFromJsonAsync<List<ProductDto>>`, התקדמות 10/50/100.
5. TODO 5/5b — תצוגת כרטיסים (ItemsControl + WrapPanel) ומעבר אליה מתחת ל-700px ב-`SizeChanged`.
6. TODO 6 — WrapPanel בסרגל העליון; TODO 7 (בונוס) — F5 טוען, Escape מבטל.

**קריטריוני קבלה:** בזמן טעינה Load מושבת, ‏Cancel פעיל, ProgressBar מתקדם, החלון זז; ‏Cancel מציג "Cancelled"; כשל רשת → באנר בעברית + Retry (לא MessageBox); חיפוש + קטגוריה + In stock only יחד עם מונה נכון; HTTP מביא נתונים מ-`Day2.LocalApi` (או באנר אם השרת כבוי); מתחת ל-700px כרטיסים והסרגל נשבר לשורות.

**איפה נתקעים, ומה הרמז:**

| תקלה | רמז |
|------|-----|
| "איזה Fake הוא זה שנכשל?" | לשניהם אותו שם תצוגה ("Fake (in-memory)") — **השני ברשימה** הוא ה-`FailRandomly`. הוא נכשל רק בחלק מהטעינות — לנסות כמה פעמים. |
| ProgressBar לא זז / חריגת cross-thread | ‏`Progress<T>` חייב להיווצר על ה-UI thread, לפני ה-`await` הראשון. |
| אחרי שגיאה החלון "טוען" לנצח | ‏`finally { IsBusy = false; }`. |
| HTTP תמיד נכשל | ‏`Day2.LocalApi` לא רץ (פורט 5080). הבאנר שמופיע **הוא הצלחה** של חלק ב'. |
| הסינון לא מגיב | לקרוא ל-`ProductsView.Refresh()` מה-setters של שלושת המסננים. |
| `Visibility="{Binding IsBusy}"` לא עובד | ‏`Visibility` הוא enum — ‏`Converter={StaticResource BoolToVis}`. |

**החלטות מ-NOTES.md לדיון:**
- ממשק + שני מימושים — ה-VM לא יודע מאיפה הנתונים; בסיס ל-DI אמיתי.
- **Composition root** ב-`MainWindow` בלבד; ‏`HttpClient` סטטי אחד (יום 2).
- **DTO סלחני** (`name`/`title`, ‏`stock`/`quantity`, מערך או `{ products: [] }`) — עובד מול `Day2.LocalApi` ומול dummyjson.com (ב-Solution יש מימוש רביעי מול `https://dummyjson.com/products?limit=40`; בלי אינטרנט — מדלגים).
- ‏`AsyncRelayCommand` — ‏`async void Execute` עטוף ב-try/finally ומונע הפעלה כפולה.
- שגיאה inline במקום MessageBox; **Cancel כפול** — משתמש + timeout של 30 שניות דרך אותו token.
- פריסה אדפטיבית ב-code-behind (החלטת View טהורה); ‏`ICollectionView` כמו ב-Lab 2.
- טיפ מה-NOTES: להריץ את השרת בטרמינל אחד והלקוח בשני, ו**לכבות את השרת באמצע** — הבאנר מופיע.

**בונוס למהירים:** debounce לחיפוש (300ms, ‏`DispatcherTimer`), מיון שורד `Refresh()`, חלון פרטים ב-`MouseDoubleClick`.

**הרצת הפתרון:**

<div dir="ltr">

```powershell
# טרמינל 1 (אופציונלי)
cd C:\c-\Day2-Async-APIs\Demos\Day2.LocalApi
dotnet run
# טרמינל 2
cd C:\c-\Day3-GUI-WPF\Labs\Lab3-ProductsApiClient\Solution
dotnet run
```

</div>

---

### 16:20–16:30 | סיכום (שקפים 51, 54–55)

- שקף 54 — "הטעויות הנפוצות של היום": לעבור מהר ולבקש מהכיתה טעות אחת שהם עשו היום.
- שאלות חזרה (למטה), שיעורי בית, ושקף 51 (ציטוט "הצצה ליום 4") כטיזר לסיום.

---

## אם מאחרים / אם מקדימים

### אם מאחרים — מה לחתוך (לפי סדר עדיפות)

1. **תרגיל 10 (13:48–14:00)** — להפוך אותו לשיעורי בית ולהחזיר את 12 הדקות ל-Lab 2 (שמגיע אז למשך הרשמי, 75 דק' מלאות מ-14:10).
2. **שבירות מכוונות בדמואים** (HelloWpf בלי `InitializeComponent`, ‏Binding עם `Sumary`) — לציין במילים במקום להדגים.
3. **Lab 2, חלק ה'** — להשאיר את `AutoGenerateColumns` ואת TODO 8 לבית; מספיק שהטופס עובד.
4. **מודול 07** — לדלג על ספריות UI ועל `d:DataContext`; להשאיר Styles/DynamicResource, ‏UserControl+DP ו-Hot Reload. דמו WinForms — להריץ בלבד, בלי קוד.
5. **תרגיל 7 (12:37–12:45)** — להראות את טאב Controls בדמו Layouts (אותו רעיון) ולהעביר לבית.
6. **חלון המעבדה 15:50–16:20** — אם נשארו פחות מ-20 דק', להפוך אותו ל-walkthrough של פתרון Lab 4 עם Hot Reload, ושני ה-Labs הולכים הביתה.
- **לא לחתוך:** פתיחת Lab 1 (TextChanged/SelectionChanged/`IsLoaded`), ציור עץ ה-DataContext לפני Lab 2, ודמו "Block UI (bad!)".

### אם מקדימים — מה להוסיף

- תרגילים נוספים לפי מודול: 3 (Login form, ‏מודול 02), 5 ו-6 (Commands, ‏Stopwatch, ‏מודול 03), 8 ו-9 (Shopping list, ‏Converter, ‏מודול 04), 11 (ValidationRule + ErrorTemplate ★★★, ‏מודול 06), 12 (Styles & theme swap, ‏מודול 07).
- בונוסים של המעבדות: Lab 1 — קטגוריית משקל / xUnit; Lab 2 — ייצוא CSV; Lab 3 — debounce; Lab 4 — Dark theme.
- דיון מ-Notes/05: בדיקת xUnit ל-`ProductsViewModel` עם `FakeProductService { DelayPerBatchMs = 0 }` — ולהראות למה זה אפשרי רק בזכות ההפרדה.
- להדגים `PresentationTraceSources.TraceLevel=High` לדיבאג binding (Notes/04).
- להריץ **גם** את Lab 3 וגם את Lab 4 (60 דק' כל אחד לפי ה-README) אם קיבלתם זמן משמעותי.

---

## סיכום היום

### שאלות חזרה (עם תשובות קצרות)

1. **למה אחרי `await` ב-handler מותר לגעת בפקדים, אבל בתוך `Task.Run` לא?**
   אחרי `await` הקוד חוזר ל-UI thread בזכות ה-`SynchronizationContext` של WPF; ‏`Task.Run` רץ על thread-pool, ונגיעה בפקד זורקת `InvalidOperationException` — צריך `Dispatcher.Invoke` (או `await Task.Run(...)`).
2. **מה ההבדל בין `sender` ל-`e.OriginalSource` ב-routed event, ומה עושה `e.Handled = true`?**
   `sender` הוא האלמנט שעליו רשום ה-handler; ‏`OriginalSource` הוא האלמנט שבו האירוע התחיל. ‏`Handled` עוצר את המשך הבעבוע/המנהור.
3. **הוספתי פריט לרשימה ושיניתי שם של פריט קיים — מה צריך כדי ששניהם יופיעו במסך?**
   `ObservableCollection<T>` להוספה/הסרה, ו-`INotifyPropertyChanged` (דרך `ObservableObject`) בפריט עצמו לשינוי property.
4. **`ValidationRule` מול `INotifyDataErrorInfo` — מתי כל אחד?**
   `ValidationRule` לבדיקות מקומיות של פקד (פורמט/טווח), והערך השגוי לא מגיע ל-source; ‏`INotifyDataErrorInfo` ב-VM/מודל — מומלץ ל-MVVM, נבדק ב-unit test, ו-`HasErrors` זמין ל-`CanExecute`.
5. **למה `SaveAsync().GetAwaiter().GetResult()` ב-`Closing` תוקע את האפליקציה, ומה עושים במקום?**
   ה-continuation מחכה ל-UI thread שחסום על ה-Result → deadlock. במקום: `e.Cancel = true; await SaveAsync(); Close();`.
6. **`StaticResource` מול `DynamicResource`?**
   Static נפתר פעם אחת בטעינה; Dynamic עוקב אחרי שינויים — לכן צבעי theme חייבים Dynamic (תרגיל 12, ‏Lab 4).

### שיעורי בית

- **המעבדה שלא נעשתה בכיתה:** Lab 3 (אם עשיתם Lab 4) או Lab 4 (אם עשיתם Lab 3), ולהשלים את מה שלא הספקתם ב-Lab 2.
- **תרגילים 5, 8, 11** (לפי שקף 55): ‏RelayCommand + InputBindings ★★, ‏Shopping list ★★, ‏ValidationRule + ErrorTemplate ★★★.
- רשות: כל שאר התרגילים שלא נעשו (3, 6, 9, 12) ו-10 אם נחתך.
- להשוות כל פתרון ל-`Solution/NOTES.md` — "האם ההחלטות שלי דומות? ואם לא — למה?".

### הצצה ליום 4

"כל ה-boilerplate שראינו היום — DependencyProperty, ‏Styles, ‏ViewModel עם עשרים properties — הוא טקסט מובנה וחזרתי" (שקף 51).
מחר ב-[יום 4](../Day4-AI-Assisted-Development/README.md) נלמד לעבוד עם כלי AI (GitHub Copilot, ‏Claude, ‏ChatGPT) — כולל מפגש סיום על
**AI לפיתוח ממשקי משתמש**: לבקש `StatCard` או מסך MVVM מ-wireframe, ולבדוק שהתוצאה נכונה. להדגיש: שגיאת binding
לא מפילה את התוכנית, היא רק משאירה שדה ריק — ולכן מה שלמדנו היום הוא הבסיס לבדיקת קוד שנוצר ע"י AI.
לבקש מהסטודנטים לוודא שחשבונות ה-AI שלהם פתוחים ועובדים לפני מחר.

</div>
