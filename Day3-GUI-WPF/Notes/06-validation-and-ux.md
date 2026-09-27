# מודול 06 — ולידציה של קלט ואינטראקציה עם המשתמש

## למה ולידציה ב-UI?

השרת (או השכבה העסקית) חייב לאמת — אבל המשתמש לא צריך לגלות על שגיאה אחרי שלחץ Submit וחיכה
לרשת. ולידציה ב-UI היא עניין של חוויית משתמש: משוב מיידי, ליד השדה, בשפה אנושית, ובלי לחסום.
WPF נותן כמה מנגנונים; בוחרים לפי המקרה.

## גישה 1: ולידציה ב-Submit

הפשוטה ביותר: בלחיצה בודקים הכל, מציגים את השגיאה הראשונה, ומחזירים פוקוס:

```csharp
private void Save_Click(object sender, RoutedEventArgs e)
{
    if (string.IsNullOrWhiteSpace(NameBox.Text)) { ShowError("שם הוא שדה חובה"); NameBox.Focus(); return; }
    if (!int.TryParse(AgeBox.Text, out var age) || age is < 0 or > 120) { ShowError("גיל לא תקין"); AgeBox.Focus(); return; }
    ...
}
```

מתאים לטפסים קטנים ולכלים פנימיים. חסרון: המשתמש רואה שגיאה אחת בכל פעם, ורק אחרי לחיצה.

## גישה 2: ValidationRule בתוך ה-Binding

הבדיקה יושבת על ה-binding עצמו, ורצה **לפני** שהערך מגיע ל-source:

```csharp
public class RangeRule : ValidationRule
{
    public int Min { get; set; }
    public int Max { get; set; } = 100;

    public override ValidationResult Validate(object value, CultureInfo culture)
    {
        if (!int.TryParse(value as string, out var n)) return new ValidationResult(false, "יש להזין מספר שלם");
        if (n < Min || n > Max) return new ValidationResult(false, $"הערך חייב להיות בין {Min} ל-{Max}");
        return ValidationResult.ValidResult;
    }
}
```

```xml
<TextBox>
    <TextBox.Text>
        <Binding Path="Quantity" UpdateSourceTrigger="PropertyChanged">
            <Binding.ValidationRules>
                <rules:RangeRule Min="0" Max="100" />
            </Binding.ValidationRules>
        </Binding>
    </TextBox.Text>
</TextBox>
```

טוב לבדיקות "מקומיות" של פקד (פורמט, טווח). חסרון: הכלל חי ב-XAML, ה-ViewModel לא יודע שיש שגיאה.

## גישה 3: INotifyDataErrorInfo — ה-ViewModel מאמת

הגישה המומלצת ל-MVVM. ה-VM (או המודל) מממש ממשק שאומר "לאילו properties יש שגיאות":

```csharp
public interface INotifyDataErrorInfo
{
    bool HasErrors { get; }
    event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;
    IEnumerable GetErrors(string? propertyName);
}
```

מימוש בסיס לשימוש חוזר (ראו Lab 2):

```csharp
public abstract class ValidatableObject : ObservableObject, INotifyDataErrorInfo
{
    private readonly Dictionary<string, List<string>> _errors = new();

    public bool HasErrors => _errors.Count > 0;
    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

    public IEnumerable GetErrors(string? name) =>
        name is not null && _errors.TryGetValue(name, out var list) ? list : Array.Empty<string>();

    protected void SetErrors(IEnumerable<string> errors, [CallerMemberName] string? name = null)
    {
        var list = errors.ToList();
        if (list.Count == 0) _errors.Remove(name!); else _errors[name!] = list;
        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(name));
        OnPropertyChanged(nameof(HasErrors));
    }
}
```

ואז כל setter מאמת:

```csharp
public string Email
{
    get => _email;
    set
    {
        if (!SetProperty(ref _email, value)) return;
        SetErrors(MailAddress.TryCreate(value, out _) ? [] : ["כתובת אימייל לא תקינה"]);
    }
}
```

ה-binding מציג את השגיאה אוטומטית (`ValidatesOnNotifyDataErrors` הוא `true` כברירת מחדל ב-.NET Core ומעלה).
היתרון הגדול: `HasErrors` זמין ל-`CanExecute`, כל הכללים בקוד C# שנבדק ב-unit test, ושגיאות מכמה שדות בו-זמנית.

(`IDataErrorInfo` הוא הממשק הישן יותר — `string this[string columnName]` — עדיין נתמך, אבל
`INotifyDataErrorInfo` גמיש יותר ותומך בולידציה אסינכרונית.)

## איך השגיאה נראית: Validation.ErrorTemplate

ברירת המחדל: מסגרת אדומה דקה. כדי להוסיף הודעה:

```xml
<ControlTemplate x:Key="ErrorTemplate">
    <StackPanel>
        <Border BorderBrush="Red" BorderThickness="2">
            <AdornedElementPlaceholder x:Name="Adorner" />   <!-- כאן יושב ה-TextBox המקורי -->
        </Border>
        <TextBlock Foreground="Red" FontSize="11"
                   Text="{Binding ElementName=Adorner, Path=AdornedElement.(Validation.Errors)/ErrorContent}" />
    </StackPanel>
</ControlTemplate>

<Style TargetType="TextBox">
    <Setter Property="Validation.ErrorTemplate" Value="{StaticResource ErrorTemplate}" />
    <Style.Triggers>
        <Trigger Property="Validation.HasError" Value="True">
            <Setter Property="ToolTip" Value="{Binding RelativeSource={RelativeSource Self}, Path=(Validation.Errors)/ErrorContent}" />
        </Trigger>
    </Style.Triggers>
</Style>
```

ה-template מצויר ב-**adorner layer** מעל הפקד, לכן טקסט מתחת לשדה עלול לחפוף לפקד הבא — השאירו
`Margin` תחתון. ה-`/` ב-`(Validation.Errors)/ErrorContent` אומר "הפריט הנוכחי ברשימה" (הראשון).

## הכפתור מושבת עד שהכל תקין

עם `ICommand`:

```csharp
SaveCommand = new RelayCommand(Save, () => !HasErrors);
```

בלי VM, עם trigger על פקד ספציפי:

```xml
<Button Content="Save">
    <Button.Style>
        <Style TargetType="Button">
            <Style.Triggers>
                <DataTrigger Binding="{Binding ElementName=EmailBox, Path=(Validation.HasError)}" Value="True">
                    <Setter Property="IsEnabled" Value="False" />
                </DataTrigger>
            </Style.Triggers>
        </Style>
    </Button.Style>
</Button>
```

## קלט מספרי

`TextBox` הוא טקסט. שתי שכבות:

1. **חסימת תווים** ב-`PreviewTextInput` (tunneling — לפני שהפקד מקבל את התו):

```csharp
private void Numeric_PreviewTextInput(object sender, TextCompositionEventArgs e) =>
    e.Handled = !e.Text.All(char.IsDigit);
```

2. **ולידציה אמיתית** עם `TryParse` — כי הדבקה (Ctrl+V) עוקפת את `PreviewTextInput`, וכי צריך לבדוק טווח.

לפרסור השתמשו ב-`CultureInfo.InvariantCulture` או ב-`CultureInfo.CurrentCulture` באופן **מודע** —
בעברית/צרפתית הנקודה העשרונית שונה.

## דיאלוגים

### קבצים

```csharp
using Microsoft.Win32;

var dlg = new OpenFileDialog { Filter = "JSON (*.json)|*.json|All files|*.*", Title = "Import" };
if (dlg.ShowDialog(this) == true)
    Import(dlg.FileName);

var save = new SaveFileDialog { FileName = "export.json", DefaultExt = ".json", Filter = "JSON|*.json" };
if (save.ShowDialog(this) == true)
    File.WriteAllText(save.FileName, json);
```

`ShowDialog` מחזיר `bool?` — לכן `== true`. ‏`OpenFolderDialog` קיים מ-.NET 8.

### דיאלוג מותאם

חלון רגיל עם `ShowDialog()`:

```csharp
var dlg = new NameDialog { Owner = this };        // Owner: ממורכז מעל ההורה וחוסם אותו
if (dlg.ShowDialog() == true)
    ProjectName = dlg.ProjectName;
```

בתוך הדיאלוג: `DialogResult = true;` סוגר ומחזיר true. כפתור עם `IsCancel="True"` סוגר עם false בלי קוד.
`WindowStartupLocation="CenterOwner"`, ‏`ResizeMode="NoResize"`, ‏`ShowInTaskbar="False"` נותנים מראה של דיאלוג.

### אישור

```csharp
var r = MessageBox.Show(this, "למחוק את הפריט? הפעולה אינה הפיכה.", "אישור מחיקה",
                        MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);
if (r == MessageBoxResult.Yes) Delete();
```

ברירת המחדל (`MessageBoxResult.No`) חשובה: Enter בטעות לא ימחק.

## UI ידידותי למקלדת

- **סדר Tab** הגיוני: `TabIndex`, או פשוט סדר ה-XAML.
- `Label` עם `Target` ו-`_` בטקסט: `<Label Content="_Email" Target="{Binding ElementName=EmailBox}" />` — Alt+E מקפיץ לשדה.
- `IsDefault`/`IsCancel` על הכפתורים; `InputBindings` לפעולות תכופות.
- פוקוס ראשוני: `FirstBox.Focus()` בבנאי או `FocusManager.FocusedElement="{Binding ElementName=FirstBox}"` ב-XAML.
- `SelectAll()` ב-`GotFocus` לשדות מספריים.

## ToolTips ונגישות

```xml
<Button Content="⟳" ToolTip="Refresh (F5)" AutomationProperties.Name="Refresh" />
```

- `ToolTip` על כל כפתור-אייקון. יכול להיות פאנל שלם, לא רק טקסט.
- `AutomationProperties.Name` — מה שקורא-מסך יקריא. לכפתורים עם טקסט זה אוטומטי; לאייקונים חובה.
- `AutomationProperties.LabeledBy` מקשר שדה לתווית.
- ניגודיות: אל תסמכו על צבע בלבד לשגיאה — הוסיפו אייקון או טקסט.
- כבדו את `SystemParameters.HighContrast` וגדלי גופן של המערכת (אל תקבעו `FontSize` זעיר).
- בדיקה מהירה: נסו לעבור על הטופס כולו **בלי עכבר**.

## ולידציה אסינכרונית

לפעמים הבדיקה דורשת שרת: "האם שם המשתמש תפוס?". `INotifyDataErrorInfo` תומך בזה באופן טבעי —
ה-setter מפעיל בדיקה אסינכרונית, וכשהיא חוזרת קוראים ל-`SetErrors` ומודיעים. בינתיים השדה תקין
(או מציג "בודק…"). חשוב: לבטל בדיקה קודמת אם המשתמש המשיך להקליד (`CancellationTokenSource`,
בדיוק כמו ב-debounce), ולשלב debounce כדי לא לשלוח בקשה על כל תו.

## הודעות בשפת המשתמש

הודעת שגיאה טובה עונה על שלוש שאלות: **מה** לא בסדר, **איפה**, ו**מה לעשות**. השוו:

| גרוע | טוב |
|------|-----|
| "Invalid input" | "טלפון חייב להכיל 9–15 ספרות" |
| "Error 400" | "לא ניתן לשמור: כתובת האימייל כבר קיימת במערכת" |
| "Object reference not set..." | "משהו השתבש. נסו שוב; אם הבעיה נמשכת פנו לתמיכה (קוד 1042)" |

ההודעה ליד השדה, לא בראש הטופס; בצבע **וגם** בטקסט/אייקון; ובשפה שהמשתמש קורא — אם האפליקציה
בעברית, גם השגיאות.

## רשימת בדיקה ל-UX של טופס

לפני שמסיימים טופס, עברו על הרשימה:

- [ ] אפשר למלא ולשלוח את כל הטופס **בלי עכבר** (Tab, Enter, Escape, Alt+אות).
- [ ] הפוקוס מתחיל בשדה הראשון.
- [ ] שדות חובה מסומנים (כוכבית) — לפני שהמשתמש טועה.
- [ ] שגיאה מופיעה ליד השדה, מיד או ב-LostFocus, ונעלמת כשמתקנים.
- [ ] כפתור השמירה מושבת כשיש שגיאות — **ויש הסבר למה** (tooltip או סיכום).
- [ ] פעולה הרסנית (מחיקה) מבקשת אישור, וברירת המחדל היא "לא".
- [ ] פעולה ארוכה מציגה התקדמות ומאפשרת ביטול (מודול 05).
- [ ] הטופס נראה תקין כשמרחיבים/מצרים את החלון וכשהמערכת מוגדרת ל-125% טקסט.

## ולידציה בשכבות: UI, VM, שרת

חשוב להבין שה-UI הוא רק השכבה הראשונה. אותו כלל ("טלפון 9–15 ספרות") צריך להתקיים גם בשרת —
כי ה-UI אפשר לעקוף, וכי יש עוד לקוחות. הדרך לא לכתוב אותו שלוש פעמים: לשים את חוקי הולידציה
במודל/ב-`Core` (למשל `Contact.Validate()` שמחזיר רשימת שגיאות), ולתת גם ל-VM וגם ל-API לקרוא לו.
`INotifyDataErrorInfo` ב-VM הוא אז רק "מתאם" שמעביר את התוצאות ל-binding. ב-Lab 2 הולידציה יושבת
במודל `Contact` בדיוק מהסיבה הזו — אפשר לבדוק אותה ב-xUnit בלי WPF, ולהעתיק אותה ל-API של יום 2.

## מתי לאמת: מיידי, ב-LostFocus או ב-Submit?

- **מיידי (כל הקשה)** — לשדות קצרים עם כלל פשוט (מספר, אורך). המשתמש מקבל משוב תוך כדי הקלדה.
  חסרון: שדה חובה ריק "צועק" עוד לפני שהתחלתם — לכן ב-Lab 1 שדה ריק לא נחשב שגיאה עד שהוקלד משהו.
- **ב-LostFocus** — לשדות ארוכים (אימייל, כתובת): מציגים שגיאה רק כשהמשתמש סיים עם השדה. זו ברירת
  המחדל של `TextBox.Text` binding, ולפעמים היא דווקא הנכונה.
- **ב-Submit** — לכללים שתלויים בכמה שדות ("תאריך סיום אחרי תאריך התחלה") או בשרת.

השילוב הטוב ביותר: מיידי לפורמט, Submit לחוקים צולבים, והכפתור מושבת רק כשיש שגיאות **ידועות** —
לא כשהטופס פשוט עוד ריק.

## טעויות נפוצות

- ולידציה רק ב-Submit + MessageBox לכל שגיאה → משתמשים מתוסכלים.
- `ErrorTemplate` עם טקסט מתחת לשדה ובלי Margin → ההודעה מכסה את השדה הבא.
- `UpdateSourceTrigger` ברירת מחדל (`LostFocus`) על TextBox → הכפתור "לא מתעדכן" עד שעוזבים את השדה.
- `PreviewTextInput` בלבד בלי `TryParse` → הדבקה מכניסה זבל.
- `MessageBox.Show` בלי `Owner` → הדיאלוג נפתח מאחורי החלון.
- להישען על `Enabled=false` כטיפול יחיד: המשתמש לא מבין *למה* הכפתור אפור. הוסיפו הודעה.

## לסיכום

- שלוש רמות: Submit (פשוט), `ValidationRule` (בפקד), `INotifyDataErrorInfo` (ב-VM — המומלץ ל-MVVM).
- `Validation.ErrorTemplate` + Trigger על `Validation.HasError` מציגים את השגיאה; `CanExecute` משבית שמירה.
- קלט מספרי: `PreviewTextInput` + `TryParse` עם culture מודע.
- דיאלוגים: `OpenFileDialog`/`SaveFileDialog`, חלון עם `ShowDialog` ו-`DialogResult`, ‏`MessageBox` עם ברירת מחדל בטוחה.
- מקלדת ונגישות: Tab, Alt-mnemonics, `IsDefault`/`IsCancel`, ‏ToolTip, ‏`AutomationProperties`.

## קריאה נוספת

- [Data validation (Binding.ValidationRules, INotifyDataErrorInfo)](https://learn.microsoft.com/dotnet/desktop/wpf/data/data-binding-overview#data-validation)
- [Validation.ErrorTemplate](https://learn.microsoft.com/dotnet/api/system.windows.controls.validation.errortemplate)
- [INotifyDataErrorInfo](https://learn.microsoft.com/dotnet/api/system.componentmodel.inotifydataerrorinfo)
- [Dialog boxes overview](https://learn.microsoft.com/dotnet/desktop/wpf/windows/dialog-boxes-overview)
- [Accessibility best practices](https://learn.microsoft.com/dotnet/framework/ui-automation/accessibility-best-practices)
- [Focus overview](https://learn.microsoft.com/dotnet/desktop/wpf/advanced/focus-overview)
