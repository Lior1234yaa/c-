<div dir="rtl">

# מודול 03 — אירועים ותכנות מונחה-אירועים

## המודל: התוכנית מחכה, המשתמש מוביל

בקונסולה הקוד רץ מלמעלה למטה ו"מושך" קלט (`Console.ReadLine`). ב-GUI זה הפוך: אחרי שהחלון נפתח,
התוכנית **מחכה** בלולאת הודעות (message loop). כל פעולה — לחיצה, הקשה, שינוי גודל, timer — הופכת
ל**אירוע**, ואתם רושמים **handlers** שרצים כשהאירוע קורה. זה תכנות מונחה-אירועים (event-driven):
אין "flow" ראשי, יש אוסף של תגובות.

ב-C# אירוע הוא delegate multicast (יום 1): `button.Click += Handler;`. ב-XAML זה attribute:

<div dir="ltr">

```xml
<Button Content="Save" Click="Save_Click" />
```

</div>

<div dir="ltr">

```csharp
private void Save_Click(object sender, RoutedEventArgs e)
{
    // sender = הכפתור שנלחץ; e = פרטים על האירוע
}
```

</div>

## התבנית `sender` / `e`

כל handler ב-.NET נראה אותו דבר: `(object sender, TEventArgs e)`.

- **`sender`** — האובייקט שהפעיל את האירוע. שימושי כש-handler אחד משרת כמה פקדים:

<div dir="ltr">

```csharp
private void Digit_Click(object sender, RoutedEventArgs e)
{
    var button = (Button)sender;
    Display.Text += button.Content;
}
```

</div>

- **`e`** — מידע ספציפי לאירוע: `KeyEventArgs.Key`, `MouseButtonEventArgs.GetPosition()`,
  `TextChangedEventArgs`, `SelectionChangedEventArgs.AddedItems`, `CancelEventArgs.Cancel`, ‏`SizeChangedEventArgs.NewSize`.

## Routed events: בעבוע ומנהור

אירועים ב-WPF הם **routed** — הם נוסעים בעץ הפקדים:

- **Bubbling** (ברירת מחדל: `Click`, `KeyDown`, `MouseDown`) — מהפקד שבו קרה האירוע **למעלה** להורים.
- **Tunneling** (`PreviewKeyDown`, `PreviewMouseDown`) — מה-root **למטה** אל הפקד. תמיד רץ לפני ה-bubbling.
- **Direct** — רק על הפקד עצמו (`MouseEnter`).

למה זה שימושי? handler אחד על ה-Grid במקום עשרים על כפתורים:

<div dir="ltr">

```xml
<Grid Button.Click="AnyButton_Click">
    <Button Content="1" /> <Button Content="2" /> ...
</Grid>
```

</div>

<div dir="ltr">

```csharp
private void AnyButton_Click(object sender, RoutedEventArgs e)
{
    // sender = ה-Grid; e.OriginalSource = הכפתור שבאמת נלחץ
    if (e.OriginalSource is Button b) Display.Text += b.Content;
}
```

</div>

`e.Handled = true` עוצר את המסע — אף הורה לא יקבל את האירוע. ‏`Preview*` מאפשר "ליירט" לפני שהפקד
מגיב — למשל לחסום תווים ב-`PreviewTextInput` (מודול 06). ראו `Demos/Day3.Demo.Events` שמדפיס את
סדר האירועים ליומן.

## אירועים שתפגשו כל יום

| אירוע | על מי | מתי |
|-------|-------|-----|
| `Click` | Button, MenuItem, CheckBox | לחיצה (עכבר, Enter, רווח) |
| `TextChanged` | TextBox | כל שינוי טקסט |
| `SelectionChanged` | ComboBox, ListBox, DataGrid, TabControl | שינוי בחירה — **גם מהקוד** |
| `KeyDown` / `PreviewKeyDown` | כל פקד/החלון | הקשה. `e.Key == Key.Enter` |
| `Checked` / `Unchecked` | CheckBox, RadioButton | שינוי מצב |
| `ValueChanged` | Slider | שינוי ערך |
| `Loaded` | Window, כל FrameworkElement | הפקד מוכן ומוצג — מקום טוב לטעינת נתונים |
| `Closing` | Window | לפני סגירה, ניתן לביטול (`e.Cancel = true`) |
| `Closed` | Window | אחרי סגירה — ניקוי (timers, קבצים) |
| `SizeChanged` | Window/פקד | שינוי גודל (מודול 02) |
| `MouseDoubleClick` | כל פקד | לחיצה כפולה (פתיחת פריט ברשימה) |

זהירות עם `SelectionChanged`: הוא נורה גם כשמגדירים `SelectedIndex` בבנאי, לפני ש-`Loaded` קרה.
בדיקת `if (!IsLoaded) return;` בתחילת ה-handler חוסכת `NullReferenceException`.

`Closing` עם אישור:

<div dir="ltr">

```csharp
private void Window_Closing(object sender, CancelEventArgs e)
{
    if (!_dirty) return;
    var r = MessageBox.Show("יש שינויים שלא נשמרו. לסגור?", "סגירה", MessageBoxButton.YesNo);
    e.Cancel = r != MessageBoxResult.Yes;
}
```

</div>

## ICommand: הפעולה כאובייקט

`Click` handler עובד, אבל יש לו בעיות: הוא תקוע ב-code-behind, אי אפשר לחבר אותו גם לתפריט וגם
לקיצור מקלדת בלי שכפול, ואין דרך מסודרת להגיד "הפעולה לא זמינה עכשיו". בשביל זה יש `ICommand`:

<div dir="ltr">

```csharp
public interface ICommand
{
    bool CanExecute(object? parameter);
    void Execute(object? parameter);
    event EventHandler? CanExecuteChanged;
}
```

</div>

פקדים כמו `Button`, `MenuItem` ו-`KeyBinding` יודעים לעבוד עם `Command`: הם קוראים ל-`Execute` בלחיצה,
ו**משביתים את עצמם** אוטומטית כש-`CanExecute` מחזיר `false`. המימוש המינימלי שנשתמש בו כל היום:

<div dir="ltr">

```csharp
public sealed class RelayCommand(Action execute, Func<bool>? canExecute = null) : ICommand
{
    public bool CanExecute(object? parameter) => canExecute?.Invoke() ?? true;
    public void Execute(object? parameter) => execute();

    // WPF בודק מחדש CanExecute אחרי כל אינטראקציה (מקלדת/עכבר/פוקוס)
    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
```

</div>

שימוש:

<div dir="ltr">

```csharp
public ICommand SaveCommand { get; }

public MainWindow()
{
    InitializeComponent();
    DataContext = this;
    SaveCommand = new RelayCommand(Save, () => _dirty);
}
```

</div>

<div dir="ltr">

```xml
<Button Content="Save" Command="{Binding SaveCommand}" />
<MenuItem Header="_Save" Command="{Binding SaveCommand}" />
```

</div>

אם המצב שמשפיע על `CanExecute` השתנה בלי אינטראקציית UI (למשל אחרי `await`), קוראים
`CommandManager.InvalidateRequerySuggested()`. במודול 04 ה-commands יעברו ל-ViewModel.

## קיצורי מקלדת: InputBindings

<div dir="ltr">

```xml
<Window.InputBindings>
    <KeyBinding Key="S" Modifiers="Ctrl" Command="{Binding SaveCommand}" />
    <KeyBinding Key="F5" Command="{Binding RefreshCommand}" />
    <KeyBinding Key="Delete" Command="{Binding DeleteCommand}" />
</Window.InputBindings>
```

</div>

זה עובד עם commands בלבד — עוד סיבה להשתמש בהם. בנוסף:

- `IsDefault="True"` על כפתור = Enter מפעיל אותו; `IsCancel="True"` = Escape (ובדיאלוג גם סוגר).
- קו תחתון ב-`Content="_Save"` או `Header="_File"` = מקש Alt+S / Alt+F.
- `InputGestureText="Ctrl+S"` על MenuItem מציג את הקיצור בתפריט (תצוגה בלבד).

## טיימרים: DispatcherTimer

לשעון, לרענון תקופתי, ל-debounce של חיפוש — `DispatcherTimer` מפעיל את `Tick` **על ה-UI thread**,
לכן מותר לגעת בפקדים בלי `Dispatcher.Invoke`:

<div dir="ltr">

```csharp
private readonly DispatcherTimer _timer = new() { Interval = TimeSpan.FromSeconds(1) };

public MainWindow()
{
    InitializeComponent();
    _timer.Tick += (_, _) => Clock.Text = DateTime.Now.ToString("HH:mm:ss");
    Loaded += (_, _) => _timer.Start();
    Closed += (_, _) => _timer.Stop();   // אחרת החלון "חי" לנצח בזיכרון
}
```

</div>

אל תבלבלו עם `System.Timers.Timer` או `System.Threading.Timer` — הם מפעילים callback על thread-pool
thread, ונגיעה בפקד משם תזרוק חריגה. אם צריך דיוק (שעון עצר), מודדים עם `Stopwatch` ומשתמשים
ב-DispatcherTimer רק לרענון התצוגה.

## Lambda handlers והסרת רישום

לא חייבים מתודה נפרדת לכל אירוע. ל-handlers קצרים, lambda בקוד היא לגיטימית וקריאה:

<div dir="ltr">

```csharp
Loaded += (_, _) => _timer.Start();
Closed += (_, _) => _timer.Stop();
```

</div>

הבעיה עם lambda: אי אפשר להסיר אותה (`-=`) כי אין לה שם. ברוב המקרים לא אכפת לנו — החלון
והפקדים מתים יחד. אבל כשאובייקט **ארוך-חיים** (service סטטי, `Application`, timer גלובלי) מחזיק
handler של חלון **קצר-חיים**, החלון לא ישוחרר מהזיכרון עד שמסירים את הרישום. זו דליפת הזיכרון
הקלאסית ב-WPF. כלל: אם רשמתם `+=` על משהו שחי יותר מהחלון — הסירו ב-`Closed`.

## אירוע או Command? טבלת החלטה

| מצב | בחרו |
|-----|------|
| פעולה עסקית (שמור, מחק, טען) שאפשר לבטל/להשבית | `ICommand` |
| אותה פעולה מכפתור, מתפריט ומקיצור מקלדת | `ICommand` |
| תגובה ל-UI טהור (SizeChanged, פוקוס, גלילה) | event handler ב-code-behind |
| אירוע שצריך את `e` (מיקום עכבר, מקש, פריטים שנבחרו) | event handler |
| פרוטוטייפ של חמש דקות | event handler — ואז לשדרג |

ב-WPF אפשר גם לחבר אירוע ישירות ל-command בלי code-behind (`EventTrigger`/behaviors מחבילת
`Microsoft.Xaml.Behaviors.Wpf`), אבל לקורס זה מעבר לצורך.

## Debounce: לא להגיב לכל הקשה

חיפוש שרץ על כל `TextChanged` יפעיל בקשת רשת על כל תו. הפתרון הקלאסי הוא **debounce** עם
`DispatcherTimer`: בכל הקשה מאפסים את הטיימר; רק כשהמשתמש עוצר ל-300ms — מחפשים.

<div dir="ltr">

```csharp
private readonly DispatcherTimer _debounce = new() { Interval = TimeSpan.FromMilliseconds(300) };

public MainWindow()
{
    InitializeComponent();
    _debounce.Tick += (_, _) => { _debounce.Stop(); RunSearch(SearchBox.Text); };
    SearchBox.TextChanged += (_, _) => { _debounce.Stop(); _debounce.Start(); };
}
```

</div>

אותה טכניקה מתאימה לשמירה אוטומטית, לרענון תצוגה מקדימה, ולכל דבר "יקר" שמופעל מהקלדה.

## מה קורה בלולאת ההודעות

שווה להבין את המנגנון פעם אחת. אחרי `Application.Run`, ה-thread הראשי נכנס ללולאה: Windows שולחת
הודעה (עכבר זז, מקש נלחץ, החלון צריך ציור מחדש), WPF מתרגם אותה לאירוע, מפעיל את ה-handlers
שלכם, ואז — רק אז — מצייר את השינויים וחוזר לחכות להודעה הבאה. שתי מסקנות מעשיות: (1) כל עוד
ה-handler שלכם רץ, שום דבר לא מצטייר ושום קלט לא מטופל — לכן handlers חייבים להיות קצרים או
אסינכרוניים; (2) `await` בתוך handler הוא בעצם "תחזור אליי כשיגיעו נתונים, בינתיים המשך ללולאה" —
כך החלון נשאר חי. `DispatcherTimer` הוא בסך הכל הודעה שנכנסת לתור באופן קבוע. ו-`Dispatcher.Invoke`
מ-thread אחר הוא "שים לי הודעה בתור של ה-UI thread". ברגע שרואים את התמונה הזו, כל כללי
ה-threading ב-WPF הופכים להגיוניים.

## סדר האירועים בפתיחת חלון

יש סדר קבוע שכדאי להכיר כשמחליטים איפה לשים קוד אתחול: הבנאי (`InitializeComponent`) → `Initialized`
→ `Loaded` (החלון כבר על המסך, לפקדים יש גודל) → `ContentRendered` (הציור הראשון הסתיים). טעינת
נתונים אסינכרונית שייכת ל-`Loaded`, לא לבנאי — כך החלון מופיע מיד ורק אז מתחיל לטעון. הגדרת
`DataContext`, לעומת זאת, שייכת לבנאי, כדי שה-binding-ים ייפתרו כבר בציור הראשון ולא "יקפצו".

## טעויות נפוצות

- שכחת `e.Handled = true` ב-`KeyDown` → המקש ממשיך לבעבע ומפעיל גם את הפקד וגם את החלון.
- `SelectionChanged` שנורה בבנאי לפני ש-`InitializeComponent` סיים → `NullReferenceException`.
- רישום handler פעמיים (גם ב-XAML וגם `+=` בקוד) → הפעולה רצה פעמיים.
- טיימר שלא נעצר ב-`Closed` → דליפת זיכרון וקוד שרץ על חלון סגור.
- `Thread.Sleep` ב-handler כדי "לחכות" → חלון קפוא. השתמשו ב-`await Task.Delay`.
- שימוש ב-`Click` + `IsEnabled` ידני במקום `Command` + `CanExecute` → שוכחים לעדכן וכפתור נשאר מושבת.

## לסיכום

- GUI = לולאת הודעות + handlers. אין flow ראשי.
- חתימה אחידה: `(sender, e)`. ‏`sender` מי הפעיל, `e` מה קרה.
- Routed events נוסעים בעץ: Preview (tunneling) למטה, ואז bubbling למעלה. `e.Handled` עוצר.
- `ICommand` + `RelayCommand`: הפעולה כאובייקט, `CanExecute` משבית כפתורים לבד, ו-`InputBindings` נותנים קיצורי מקלדת.
- `DispatcherTimer` לכל דבר תקופתי ב-UI.

## קריאה נוספת

- [Routed events overview](https://learn.microsoft.com/dotnet/desktop/wpf/events/routed-events-overview)
- [Commanding overview](https://learn.microsoft.com/dotnet/desktop/wpf/advanced/commanding-overview)
- [ICommand interface](https://learn.microsoft.com/dotnet/api/system.windows.input.icommand)
- [Input overview (keyboard, mouse)](https://learn.microsoft.com/dotnet/desktop/wpf/advanced/input-overview)
- [DispatcherTimer](https://learn.microsoft.com/dotnet/api/system.windows.threading.dispatchertimer)

</div>
