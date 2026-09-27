# מודול 04 — Data Binding ו-MVVM-lite

## הבעיה עם code-behind

עד עכשיו כתבנו `Greeting.Text = ...` ו-`Grid.ItemsSource = ...`. זה עובד, אבל ככל שהמסך גדל, ה-code-behind
הופך לסלט: כל פקד מעודכן ידנית מכמה מקומות, אי אפשר לבדוק את הלוגיקה בלי לפתוח חלון, וכל שינוי
עיצובי (להחליף ListBox ב-DataGrid) דורש לשנות קוד. **Data binding** הופך את הכיוון: הפקד "מסתכל"
על property של אובייקט, ומתעדכן לבד כשהוא משתנה. ה-code-behind מתכווץ, והלוגיקה עוברת לאובייקט רגיל.

## DataContext ו-{Binding}

לכל אלמנט ב-WPF יש `DataContext` — האובייקט שה-binding-ים שלו מתייחסים אליו כברירת מחדל. הוא **עובר
בירושה** במורד העץ: מגדירים פעם אחת על החלון, וכל הילדים רואים אותו.

```csharp
public MainWindow()
{
    InitializeComponent();
    DataContext = new MainViewModel();
}
```

```xml
<TextBox Text="{Binding NewTitle}" />
<TextBlock Text="{Binding Summary}" />
<Button Command="{Binding AddCommand}" />
```

`{Binding NewTitle}` = "קח את `NewTitle` מה-DataContext". ‏`Path` יכול להיות עמוק: `{Binding Selected.Name}`,
`{Binding Items.Count}`, `{Binding Items[0]}`.

### Modes

| Mode | כיוון | ברירת מחדל עבור |
|------|-------|-----------------|
| `OneWay` | מקור → פקד | רוב ה-properties (`Text` של TextBlock, `Content`) |
| `TwoWay` | מקור ↔ פקד | `TextBox.Text`, `CheckBox.IsChecked`, `Slider.Value`, `SelectedItem` |
| `OneTime` | פעם אחת בטעינה | — (טוב לנתונים סטטיים) |
| `OneWayToSource` | פקד → מקור | נדיר |

`UpdateSourceTrigger=PropertyChanged` על `TextBox.Text` — ברירת המחדל מעדכנת את המקור רק ב-`LostFocus`;
עם זה כל הקשה מעדכנת (נחוץ ל-`CanExecute` ולולידציה חיה).

### שגיאות binding לא זורקות חריגה

שגיאת כתיב ב-`Path` לא מפילה את התוכנית — הפקד פשוט נשאר ריק, וההודעה מודפסת ב-**Output window**
של Visual Studio: `System.Windows.Data Error: 40 : BindingExpression path error: 'Nmae' property not found`.
תמיד תסתכלו שם כשמשהו "לא מתעדכן".

## INotifyPropertyChanged: איך הפקד יודע שהערך השתנה

Binding ל-property רגיל עובד פעם אחת. כדי שהפקד יתעדכן כשהערך משתנה, המקור צריך להודיע:

```csharp
public interface INotifyPropertyChanged
{
    event PropertyChangedEventHandler? PropertyChanged;
}
```

במקום לממש את זה בכל מחלקה, כותבים מחלקת בסיס אחת:

```csharp
public abstract class ObservableObject : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? name = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(name);
        return true;
    }
}
```

`[CallerMemberName]` גורם לקומפיילר למלא את שם ה-property שקרא — בלי מחרוזות קסם. ואז:

```csharp
public class TodoItem : ObservableObject
{
    private string _title = "";
    private bool _isDone;

    public string Title { get => _title; set => SetProperty(ref _title, value); }
    public bool IsDone { get => _isDone; set => SetProperty(ref _isDone, value); }
}
```

Property **מחושב** (`Summary => $"{DoneCount}/{Count}"`) לא יודע להודיע לבד — צריך לקרוא
`OnPropertyChanged(nameof(Summary))` כשאחד ממרכיביו משתנה.

## ObservableCollection<T>: רשימות שמודיעות

`List<T>` לא מודיע כשמוסיפים פריט. `ObservableCollection<T>` מממש `INotifyCollectionChanged`, וכל
`Add`/`Remove`/`Clear` מעדכן את הרשימה על המסך:

```csharp
public ObservableCollection<TodoItem> Items { get; } = [];
```

```xml
<ListBox ItemsSource="{Binding Items}" SelectedItem="{Binding Selected}" />
```

שימו לב: האוסף מודיע על **הוספה/הסרה**, לא על שינוי בתוך פריט. בשביל זה הפריט עצמו צריך להיות
`ObservableObject`. ועוד: מותר לשנות `ObservableCollection` רק מה-UI thread.

## ItemsSource + DataTemplate: איך נראה כל פריט

בלי template, ListBox מציג `ToString()`. עם `DataTemplate` מגדירים UI לכל פריט, וה-`DataContext`
בתוכו הוא הפריט:

```xml
<ListBox ItemsSource="{Binding Items}">
    <ListBox.ItemTemplate>
        <DataTemplate>
            <DockPanel>
                <CheckBox IsChecked="{Binding IsDone}" />
                <TextBlock Text="{Binding Title}" Margin="6,0" />
                <TextBlock Text="{Binding Created, StringFormat={}{0:dd/MM HH:mm}}" Foreground="Gray" />
            </DockPanel>
        </DataTemplate>
    </ListBox.ItemTemplate>
</ListBox>
```

אותו רעיון עובד ב-ComboBox, ItemsControl, DataGrid (‏`DataGridTemplateColumn`) ו-TabControl.
`DisplayMemberPath="Name"` הוא קיצור כשרוצים רק טקסט מ-property אחד.

## StringFormat

```xml
<TextBlock Text="{Binding Price, StringFormat={}{0:C}}" />
<TextBlock Text="{Binding Count, StringFormat=Total: {0} items}" />
<TextBlock Text="{Binding Date, StringFormat=yyyy-MM-dd}" />
```

ה-`{}` בהתחלה הוא escape — אחרת XAML חושב ש-`{0:C}` הוא markup extension. אם יש טקסט לפני `{0}`
ה-escape לא נחוץ. `StringFormat` עובד רק כשהיעד הוא `string` (לא על `Content` של Button — שם צריך converter או `ContentStringFormat`).

## Converters: IValueConverter

כשהמקור והיעד מסוגים שונים (bool → צבע, enum → טקסט, מספר → Visibility):

```csharp
public class BoolToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
        value is true ? Brushes.SeaGreen : Brushes.DimGray;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotSupportedException();   // one-way
}
```

```xml
<Window.Resources>
    <conv:BoolToBrushConverter x:Key="DoneBrush" />
</Window.Resources>
<TextBlock Foreground="{Binding IsDone, Converter={StaticResource DoneBrush}}" />
```

`ConverterParameter` מאפשר להעביר ערך מה-XAML (למשל סף). ‏`BooleanToVisibilityConverter` מגיע מובנה
ב-WPF — הנפוץ ביותר: `Visibility="{Binding IsBusy, Converter={StaticResource BoolToVis}}"`.

## MVVM-lite: ארבע שכבות, בלי דוגמה

**MVVM** (Model–View–ViewModel) הוא התבנית הטבעית ל-WPF. בגרסה הפרגמטית שלנו:

```text
View (XAML + code-behind מינימלי)
   │  {Binding}  /  Command
   ▼
ViewModel (ObservableObject: מצב המסך + ICommand-ים)   ← נבדק ב-unit test בלי חלון
   │  קורא ל-
   ▼
Service (IProductService, IContactsStore: I/O, HTTP, קבצים)
   │  מחזיר
   ▼
Model (Product, Contact: נתונים; לעיתים גם ObservableObject)
```

הכללים הפרקטיים:

1. **ה-View לא מכיל לוגיקה.** ה-code-behind יוצר את ה-VM, מגדיר `DataContext`, ומטפל בדברים שהם
   "UI טהור" (SizeChanged, פוקוס, דיאלוגים).
2. **ה-ViewModel לא מכיר פקדים.** אין בו `using System.Windows.Controls`. הוא חושף properties
   ו-commands, וזהו. ככה אפשר לבדוק אותו ב-xUnit.
3. **Services מאחורי ממשקים.** `IProductService` עם `Fake` ו-`Http` — ה-VM לא יודע מי מהם רץ.
4. **DI-lite:** ה-VM מקבל את ה-services בבנאי. ה-View (או `App`) הוא ה-composition root.

```csharp
public class MainViewModel : ObservableObject
{
    private string _newTitle = "";
    public ObservableCollection<TodoItem> Items { get; } = [];
    public string NewTitle { get => _newTitle; set => SetProperty(ref _newTitle, value); }
    public ICommand AddCommand { get; }

    public MainViewModel()
    {
        AddCommand = new RelayCommand(
            () => { Items.Add(new TodoItem { Title = NewTitle }); NewTitle = ""; },
            () => !string.IsNullOrWhiteSpace(NewTitle));
    }
}
```

זה כל ה-VM. אין כאן שום דבר של WPF חוץ מ-`ICommand` (שיושב ב-`System.Windows.Input`, אבל הוא ממשק
פשוט). ראו `Demos/Day3.Demo.Binding` לגרסה המלאה עם פאנל פרטים ו-`RelativeSource`.

### RelativeSource: לצאת מ-DataContext פנימי

בתוך `DataTemplate` או פאנל עם `DataContext="{Binding Selected}"`, ה-binding רואה את הפריט — לא את ה-VM.
כדי להגיע לפקודה של ה-VM:

```xml
<Button Command="{Binding DataContext.RemoveCommand, RelativeSource={RelativeSource AncestorType=Window}}" />
```

## CommunityToolkit.Mvvm: אותו דבר עם פחות קוד

בפרויקטים אמיתיים משתמשים בחבילת NuGet [`CommunityToolkit.Mvvm`](https://learn.microsoft.com/dotnet/communitytoolkit/mvvm/)
של Microsoft. היא מספקת `ObservableObject`, `RelayCommand`, `AsyncRelayCommand` — ו**source generators**
שכותבים את ה-boilerplate בשבילכם:

```csharp
public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private string _newTitle = "";          // מייצר property NewTitle עם SetProperty

    [RelayCommand(CanExecute = nameof(CanAdd))]
    private void Add() { ... }              // מייצר AddCommand

    private bool CanAdd() => !string.IsNullOrWhiteSpace(NewTitle);
}
```

בקורס אנחנו כותבים את המימוש הפשוט ידנית — כדי להבין מה קורה מתחת, ולהישאר ללא תלויות. כשתתחילו
פרויקט אמיתי, קחו את ה-toolkit.

## FallbackValue, TargetNullValue ו-ElementName

שלושה פרמטרים קטנים שחוסכים המון converters:

```xml
<TextBlock Text="{Binding Selected.Name, FallbackValue=No selection}" />
<TextBlock Text="{Binding Notes, TargetNullValue=(none)}" />
<TextBlock Text="{Binding ElementName=Amount, Path=Value}" />
```

`FallbackValue` מוצג כשה-binding **נכשל** (למשל `Selected` הוא null ולכן `Selected.Name` לא קיים) —
ומונע גם את הודעת השגיאה ב-Output. `TargetNullValue` מוצג כשהערך עצמו הוא null. `ElementName` קושר
פקד לפקד אחר באותו חלון, בלי DataContext בכלל — מצוין לדברים כמו "Slider שולט ב-ProgressBar".

## CollectionView: סינון ומיון בלי לשכפל רשימות

כשמציגים `ObservableCollection` ב-ListBox, WPF יוצר מעליה בשקט `ICollectionView` — "תצוגה" עם
סינון, מיון וקיבוץ, בלי לשנות את האוסף עצמו:

```csharp
ContactsView = CollectionViewSource.GetDefaultView(Contacts);
ContactsView.Filter = o => o is Contact c && c.FullName.Contains(Search, StringComparison.OrdinalIgnoreCase);
ContactsView.SortDescriptions.Add(new SortDescription(nameof(Contact.FirstName), ListSortDirection.Ascending));
// כשהסינון משתנה:
ContactsView.Refresh();
```

קושרים את ה-`ItemsSource` ל-`ContactsView` (או ישירות לאוסף — ה-view ברירת המחדל זהה). זה מה
שמאפשר ב-Lab 2 ו-Lab 3 חיפוש מיידי בלי להחזיק "רשימה מסוננת" נפרדת. ה-view גם מחזיק את
`CurrentItem`, ולכן `SelectedItem` של ListBox ו-DataGrid שקשורים לאותו אוסף יסתנכרנו אוטומטית
(`IsSynchronizedWithCurrentItem`).

## איך מדבגים binding

1. **Output window** — כל שגיאת path מודפסת שם. הרגילו את העין לחפש `System.Windows.Data Error`.
2. **`PresentationTraceSources.TraceLevel=High`** על binding ספציפי מדפיס כל שלב בפתרון שלו:
   `{Binding Name, diag:PresentationTraceSources.TraceLevel=High}` (עם `xmlns:diag="clr-namespace:System.Diagnostics;assembly=WindowsBase"`).
3. **Converter "מרגל"** — converter שרק עושה `Debugger.Break()` ומחזיר את הערך, כדי לראות מה באמת עובר.
4. **Live Property Explorer** ב-VS מראה את ערך ה-binding בזמן ריצה ואם הוא בשגיאה.
5. השאלה הראשונה תמיד: **מה ה-DataContext כאן?** בתוך DataTemplate הוא הפריט; בתוך פאנל עם
   `DataContext` מקומי הוא מה שקבעתם; אחרת — של החלון.

## SelectedItem, SelectedValue ו-DisplayMemberPath

בפקדי רשימה יש שלוש דרכים לדעת "מה נבחר", וקל להתבלבל:

- `SelectedItem` — האובייקט עצמו (`Contact`). זה מה שרוצים ב-99% מהמקרים ב-MVVM: `SelectedItem="{Binding Selected}"`.
- `SelectedIndex` — המיקום ברשימה. שימושי לבחירת ברירת מחדל בבנאי, פחות ל-binding.
- `SelectedValue` + `SelectedValuePath` — property מתוך הפריט (למשל `Id`). טוב כשה-VM מחזיק מזהה ולא אובייקט.

`DisplayMemberPath="Name"` הוא הקיצור כשרוצים להציג רק טקסט אחד מהפריט בלי `DataTemplate`.
ב-`ComboBox` של השירותים ב-Lab 3 משתמשים בדיוק בשילוב הזה: `ItemsSource` לרשימת המימושים,
`SelectedItem` למימוש הנבחר, `DisplayMemberPath` לשם.

## Binding הוא חוזה — ובדיקות שומרות עליו

כשה-XAML אומר `{Binding Snapshot.OrdersToday}`, הוא מניח שיש property בשם הזה, מהסוג הזה, שמודיע
על שינויים. שינוי שם ב-VM לא ישבור את הקומפילציה — רק את המסך, בשקט. שתי הגנות זולות:
`nameof` בכל `OnPropertyChanged`, ובדיקת יחידה קצרה שיוצרת את ה-VM ומוודאת ש-`PropertyChanged`
נורה כשמשנים property. עם `CommunityToolkit.Mvvm` ה-source generator מבטיח את זה בקומפילציה.
ביום 4 נראה שזו גם הנקודה שבה קוד שנוצר על ידי AI נוטה לטעות — property בשם קצת שונה בין ה-XAML ל-VM.

## טעויות נפוצות

- שוכחים `DataContext = vm` → כל ה-binding-ים ריקים בשקט. בדקו Output window.
- `List<T>` במקום `ObservableCollection<T>` → `Add` לא מופיע במסך.
- property ללא `OnPropertyChanged` (auto-property) → מתעדכן פעם אחת בלבד.
- `StringFormat` בלי `{}` בהתחלה → שגיאת XAML.
- `Mode=TwoWay` על property עם `private set` → חריגה בזמן ריצה.
- ניסיון לגשת ל-VM דרך `x:Name` בתוך DataTemplate → לא עובד; `RelativeSource` או `ElementName` על אלמנט חיצוני.
- שינוי `ObservableCollection` מ-thread אחר → `NotSupportedException`.

## לסיכום

- `DataContext` + `{Binding Path}` מחברים פקד ל-property; `Mode` קובע כיוון, `UpdateSourceTrigger` מתי.
- `INotifyPropertyChanged` (דרך `ObservableObject`) ו-`ObservableCollection<T>` הם מה שהופך binding ל"חי".
- `DataTemplate` מעצב פריט, `StringFormat` ו-`IValueConverter` מתאימים ערכים לתצוגה.
- MVVM-lite: View מציג, ViewModel מחזיק מצב ופקודות, Service עושה I/O, Model מחזיק נתונים. ה-VM נבדק בלי חלון.
- בעולם האמיתי: `CommunityToolkit.Mvvm`.

## קריאה נוספת

- [Data binding overview](https://learn.microsoft.com/dotnet/desktop/wpf/data/data-binding-overview)
- [Binding declarations](https://learn.microsoft.com/dotnet/desktop/wpf/data/binding-declarations-overview)
- [INotifyPropertyChanged](https://learn.microsoft.com/dotnet/api/system.componentmodel.inotifypropertychanged)
- [ObservableCollection<T>](https://learn.microsoft.com/dotnet/api/system.collections.objectmodel.observablecollection-1)
- [Data templating overview](https://learn.microsoft.com/dotnet/desktop/wpf/data/data-templating-overview)
- [CommunityToolkit.Mvvm](https://learn.microsoft.com/dotnet/communitytoolkit/mvvm/)
