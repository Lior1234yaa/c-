<div dir="rtl">

# מודול 07 — טכניקות לפרוטוטייפינג מהיר של GUI

## המטרה: מסקיצה לחלון שעובד תוך שעה

פרוטוטייפ טוב לא צריך להיות יפה — הוא צריך **להראות את הרעיון** ולאפשר משוב מהר. הטכניקות
כאן חוסכות זמן בכל פרויקט, אבל בפרוטוטייפ הן ההבדל בין "יום" ל"שעה".

## 1. מסקיצה ל-XAML

התחילו מ-wireframe — על נייר, בלוח, או ASCII ב-markdown (כמו ב-Lab 4). ואז תרגמו מכנית:

| בסקיצה | ב-XAML |
|--------|--------|
| כותרת למעלה, סטטוס למטה | `DockPanel` עם `Dock="Top"/"Bottom"` |
| טופס "תווית: שדה" | `Grid` עם 2 עמודות (`Auto`, `*`) |
| שורת כפתורים | `StackPanel Orientation="Horizontal"` |
| כרטיסים ברוחב שווה | `UniformGrid Columns="N"` |
| כרטיסים שנשברים לשורות | `WrapPanel` |
| טבלה | `DataGrid` עם `AutoGenerateColumns="True"` (לפרוטוטייפ!) |
| לשוניות | `TabControl` |
| רשימה + פרטים | `Grid` 2 עמודות + `GridSplitter` |

כלל: **קודם המבנה, אחר כך הצבעים.** פריסה נכונה עם Border אפור נראית כמו פרוטוטייפ מקצועי;
צבעים יפים על פריסה שבורה נראים כמו באג.

## 2. Styles ו-ResourceDictionary: להגדיר פעם אחת

`Style` = אוסף `Setter`-ים שחל על סוג פקד:

<div dir="ltr">

```xml
<Style x:Key="H1" TargetType="TextBlock">
    <Setter Property="FontSize" Value="22" />
    <Setter Property="FontWeight" Value="Bold" />
    <Setter Property="Margin" Value="0,0,0,10" />
</Style>

<Style TargetType="Button">                      <!-- בלי x:Key: חל על כל הכפתורים -->
    <Setter Property="Padding" Value="12,6" />
    <Setter Property="Margin" Value="4" />
</Style>

<Style x:Key="Danger" TargetType="Button" BasedOn="{StaticResource {x:Type Button}}">
    <Setter Property="Background" Value="#D64545" />
    <Setter Property="Foreground" Value="White" />
</Style>
```

</div>

`BasedOn` = ירושה. `Style.Triggers` משנים property לפי מצב (`IsMouseOver`, `IsEnabled`) בלי קוד.

**ControlTemplate** משנה את *המבנה* של הפקד (למשל כפתור מעוגל). לפרוטוטייפ — קצר ולעניין:

<div dir="ltr">

```xml
<Setter Property="Template">
    <Setter.Value>
        <ControlTemplate TargetType="Button">
            <Border Background="{TemplateBinding Background}" CornerRadius="6" Padding="{TemplateBinding Padding}">
                <ContentPresenter HorizontalAlignment="Center" VerticalAlignment="Center" />
            </Border>
        </ControlTemplate>
    </Setter.Value>
</Setter>
```

</div>

**ResourceDictionary** מוציא את זה לקובץ נפרד וממזג ב-`App.xaml`:

<div dir="ltr">

```xml
<Application.Resources>
    <ResourceDictionary>
        <ResourceDictionary.MergedDictionaries>
            <ResourceDictionary Source="Themes/Light.xaml" />   <!-- צבעים -->
            <ResourceDictionary Source="Themes/Styles.xaml" />  <!-- סגנונות שמפנים לצבעים -->
        </ResourceDictionary.MergedDictionaries>
    </ResourceDictionary>
</Application.Resources>
```

</div>

הפרדת **צבעים** מ**סגנונות** + `{DynamicResource}` = החלפת theme בשורת קוד אחת:

<div dir="ltr">

```csharp
Application.Current.Resources.MergedDictionaries[0] =
    new ResourceDictionary { Source = new Uri("Themes/Dark.xaml", UriKind.Relative) };
```

</div>

`StaticResource` נפתר פעם אחת בטעינה; `DynamicResource` עוקב אחרי שינויים. לצבעי theme — Dynamic.

## 3. UserControl: רכיב לשימוש חוזר

כשאותו בלוק XAML חוזר (כרטיס סטטיסטיקה, שורת "תווית+שדה", כותרת עם אייקון) — הופכים אותו ל-`UserControl`.
כדי שאפשר יהיה לעשות עליו binding מבחוץ, ה-properties חייבות להיות `DependencyProperty`:

<div dir="ltr">

```csharp
public partial class StatCard : UserControl
{
    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register(nameof(Value), typeof(string), typeof(StatCard), new PropertyMetadata("0"));

    public string Value { get => (string)GetValue(ValueProperty); set => SetValue(ValueProperty, value); }
}
```

</div>

<div dir="ltr">

```xml
<!-- בתוך StatCard.xaml: x:Name="Root" על ה-UserControl -->
<TextBlock Text="{Binding Value, ElementName=Root}" FontSize="30" />

<!-- שימוש: -->
<c:StatCard Title="Orders" Value="{Binding Snapshot.OrdersToday}" />
```

</div>

ה-boilerplate של DP ארוך — זה בדיוק מה ש-snippets (`propdp` + Tab ב-VS) וכלי AI (יום 4) עושים בשבילכם.

## 4. Design-time data: לראות את המסך בלי להריץ

<div dir="ltr">

```xml
<Window ...
        xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
        xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
        mc:Ignorable="d"
        d:DataContext="{d:DesignInstance Type=vm:DashboardViewModel, IsDesignTimeCreatable=True}">
```

</div>

`d:DataContext` נותן למעצב ב-VS מופע של ה-VM, כך שה-binding-ים מוצגים עם נתונים אמיתיים (הבנאי חייב
להיות ללא פרמטרים — או שיוצרים `DesignViewModel` יורש). `d:` מתעלמים ממנו בזמן ריצה. אפשר גם
`d:Text="Sample text"` על פקד בודד.

## 5. XAML Hot Reload: הלולאה המהירה

הריצו (F5), ערכו XAML, שמרו — החלון הרץ מתעדכן. אין restart, המצב נשמר (הנתונים שטענתם עדיין שם).
זה עובד ל-layout, styles, resources, DataTemplates. **לא** עובד ל-code-behind ולשינויים מבניים
בבנאי (שם Hot Reload של C# מנסה, ולפעמים צריך restart). עבודה טיפוסית: שני מסכים — VS משמאל,
האפליקציה מימין, ומכווננים Margin/Width/צבעים עד שזה נראה נכון.

## 6. ספריות UI מוכנות

במקום לעצב כל כפתור, לוקחים ערכת עיצוב שלמה. שלוש ספריות קוד-פתוח נפוצות (בדקו את התיעוד
העדכני של כל אחת לפני שימוש):

- **MaterialDesignInXAML** — [github.com/MaterialDesignInXAML/MaterialDesignInXamlToolkit](https://github.com/MaterialDesignInXAML/MaterialDesignInXamlToolkit) — עיצוב Material של Google, הרבה פקדים ואייקונים.
- **MahApps.Metro** — [github.com/MahApps/MahApps.Metro](https://github.com/MahApps/MahApps.Metro) — ותיקה, חלון "Metro" עם title bar מותאם, דיאלוגים, flyouts.
- **WPF UI** — [github.com/lepoco/wpfui](https://github.com/lepoco/wpfui) — מראה Fluent/Windows 11, ניווט צדדי, ערכות בהיר/כהה.

כולן מותקנות כ-NuGet, מוסיפות ResourceDictionary ל-`App.xaml`, ומאותו רגע כל הפקדים הסטנדרטיים
נראים אחרת. בקורס לא נתלה בהן (כדי שהכל יבנה ללא רשת), אבל לפרוטוטייפ שצריך להרשים — זה קיצור דרך אדיר.

## 7. Snippets ו-scaffolding

- Visual Studio: `propdp` (DependencyProperty), `propfull`, `ctor`; ב-XAML — IntelliSense על attributes ואירועים
  (הקלדת `Click="` מציעה "New Event Handler" שיוצר את המתודה).
- `dotnet new wpf`, `dotnet new wpfusercontrollib` — שלד מיידי.
- שמרו לעצמכם תיקיית `Mvvm/` עם `ObservableObject`, `RelayCommand`, `ValidatableObject` — העתקה
  לפרויקט חדש לוקחת דקה. (או NuGet של `CommunityToolkit.Mvvm`.)

## 8. WinForms: המעצב הגרפי לכלי חד-פעמי

לפעמים צריך "חלון עם שלושה שדות וכפתור שמריץ סקריפט" — לשימוש פנימי, מחר בבוקר. כאן המעצב
של WinForms עדיין הכי מהיר: גוררים פקדים מה-Toolbox, לוחצים פעמיים על הכפתור, כותבים את הקוד.
בלי XAML, בלי binding, בלי MVVM.

המודל של WinForms פשוט: פקדים הם אובייקטים, הפריסה היא properties (`Dock`, `Anchor`,
`TableLayoutPanel`), ואירועים הם delegates. ב-`Demos/Day3.Demo.WinForms` הטופס בנוי **בקוד** בלי
קובץ Designer — כדי להראות בדיוק מה המעצב מייצר:

<div dir="ltr">

```csharp
public class MainForm : Form
{
    private readonly TextBox _nameBox = new() { Dock = DockStyle.Fill };
    private readonly Button _addButton = new() { Text = "Add", Dock = DockStyle.Fill };
    private readonly ListBox _list = new() { Dock = DockStyle.Fill };

    public MainForm()
    {
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 2 };
        layout.Controls.Add(_nameBox, 0, 0);
        layout.Controls.Add(_addButton, 1, 0);
        layout.Controls.Add(_list, 0, 1);
        layout.SetColumnSpan(_list, 2);
        Controls.Add(layout);

        _addButton.Click += (_, _) => { _list.Items.Add(_nameBox.Text); _nameBox.Clear(); };
    }
}
```

</div>

מתי WinForms: כלי פנימי, throwaway, צוות שכבר מכיר, תחזוקת מערכת קיימת. מתי לא: UI מותאם,
data binding מורכב, DPI גבוה ואנימציות, כל דבר שיחיה שנים.

## 9. הצצה ליום 4

כל מה שראינו — DP boilerplate, Styles, DataTemplates, ViewModel עם עשרים properties — הוא טקסט
מובנה וחזרתי. זה בדיוק מה שכלי AI (GitHub Copilot, Claude, ChatGPT) עושים היטב: "צור לי UserControl
בשם StatCard עם Title, Value ו-Accent" מחזיר את כל ה-boilerplate בשניות. מחר נלמד לעבוד איתם
נכון: איך לתאר wireframe, איך לבקש MVVM, ואיך **לבדוק** שהתוצאה נכונה — כי כפי שראינו היום,
שגיאת binding לא מפילה את התוכנית, היא רק משאירה שדה ריק.

## תהליך עבודה מומלץ לפרוטוטייפ

1. **10 דקות — סקיצה.** נייר או ASCII. מה המסכים, מה בכל מסך, מה הפעולה העיקרית.
2. **15 דקות — שלד.** `dotnet new wpf`, חלון עם הפריסה בלבד: Border-ים אפורים עם טקסט placeholder.
   הריצו ותראו שהפריסה שורדת שינוי גודל.
3. **20 דקות — נתונים מזויפים.** `FakeService` שמחזיר רשימה קבועה או אקראית, ViewModel דק, binding.
   עכשיו יש משהו להראות.
4. **15 דקות — ליטוש.** Styles, צבע אחד מודגש, אייקונים, ריווח. עצרו כשזה "נראה בסדר", לא "מושלם".
5. **הצגה ומשוב.** לפני שממשיכים לבנות.

הסדר חשוב: מי שמתחיל מהליטוש נשאר עם כפתור יפה ובלי אפליקציה.

## מתי לזרוק את הפרוטוטייפ

פרוטוטייפ טוב עונה על שאלה ("האם הזרימה הזו הגיונית למשתמשים?"). ברגע שהשאלה נענתה, יש שתי אפשרויות:
**לזרוק** ולבנות מחדש נכון, או **לשדרג** בהדרגה. הסימנים שכדאי לזרוק: ה-code-behind מלא לוגיקה,
אין ViewModel, הנתונים "חיים" בפקדים (`ListBox.Items.Add`) ולא במודל. הסימנים שאפשר לשדרג:
כבר יש `IService`+Fake, VM עם `ObservableObject`, ו-XAML שמפריד צבעים ל-resources — כלומר,
בדיוק מה שתרגלנו ב-Labs 2–4. ההשקעה הקטנה במבנה נכון כבר בפרוטוטייפ היא מה שמאפשרת "לשדרג" במקום "לזרוק".

## שיפורי ביצועים מהירים

פרוטוטייפ שמרגיש איטי מאבד את הקהל גם אם הרעיון טוב. שלושה תיקונים של דקה:

- **וירטואליזציה:** `ListBox` ו-`DataGrid` מציירים רק שורות גלויות — אבל רק אם הם **לא** בתוך `ScrollViewer`
  או `StackPanel` בלי גובה. תנו להם גובה מה-Grid (`Height="*"`).
- **`Task.Delay` ב-Fake:** אם ה-fake מחזיר מיד, לא תראו את מצבי ה-loading ולא תגלו שהחלון קופא. השאירו 200–500ms.
- **Binding ל-`DataContext` כבד:** אל תבנו את כל ה-VM בבנאי של החלון עם קריאות סינכרוניות; טענו ב-`Loaded` עם `await`.

## דוגמה: מ-wireframe לשלד ב-XAML

כדי להמחיש את טבלת התרגום, הנה ה-wireframe של Lab 4 ומה שנוצר ממנו בשלב "שלד" — לפני נתונים,
לפני צבעים:

<div dir="ltr">

```xml
<DockPanel Margin="12">
    <DockPanel DockPanel.Dock="Top">                        <!-- כותרת + כפתורים מימין -->
        <StackPanel DockPanel.Dock="Right" Orientation="Horizontal" />
        <TextBlock Text="Ops Dashboard" />
    </DockPanel>
    <TabControl>
        <TabItem Header="Overview">
            <StackPanel>
                <UniformGrid Rows="1" Columns="4">          <!-- 4 כרטיסים -->
                    <Border Style="{StaticResource Card}" /> <Border Style="{StaticResource Card}" />
                    <Border Style="{StaticResource Card}" /> <Border Style="{StaticResource Card}" />
                </UniformGrid>
                <Border Style="{StaticResource Card}" Height="160" />   <!-- גרף -->
            </StackPanel>
        </TabItem>
        <TabItem Header="Orders" />
        <TabItem Header="Settings" />
    </TabControl>
</DockPanel>
```

</div>

עשרים שורות, והמסך כבר "קיים": אפשר להריץ, לשנות גודל, ולהראות למישהו. כל מה שנשאר הוא להחליף
כל `Border` ריק בתוכן אמיתי — וזה בדיוק סדר העבודה של Lab 4.

## טעויות נפוצות

- להתחיל מצבעים ואנימציות לפני שהפריסה עובדת.
- `StaticResource` לצבעי theme → החלפת theme לא משפיעה.
- UserControl עם properties רגילות (לא DP) → binding מבחוץ לא עובד, בשקט.
- לשכוח `mc:Ignorable="d"` → שגיאת build על `d:DataContext`.
- Hot Reload "לא עובד" — בדרך כלל כי שיניתם code-behind, לא XAML.
- להכניס ספריית UI כבדה לפרוטוטייפ שצריך לרוץ על מחשב בלי גישה ל-NuGet.

## לסיכום

- wireframe → טבלת תרגום ל-panels → מבנה קודם, צבעים אחר כך.
- Styles + ResourceDictionary + DynamicResource = עיצוב אחיד והחלפת theme בשורה אחת.
- UserControl עם DependencyProperty לכל בלוק שחוזר; `d:DataContext` ו-Hot Reload ללולאה מהירה.
- ספריות UI מוכנות כשצריך להרשים; WinForms כשצריך כלי פנימי מחר בבוקר.
- יום 4: כל ה-boilerplate הזה — עם AI.

## קריאה נוספת

- [Styles and templates](https://learn.microsoft.com/dotnet/desktop/wpf/controls/styles-templates-overview)
- [Merged resource dictionaries](https://learn.microsoft.com/dotnet/desktop/wpf/systems/xaml-resources-merged-dictionaries)
- [UserControl and custom dependency properties](https://learn.microsoft.com/dotnet/desktop/wpf/properties/custom-dependency-properties)
- [XAML Hot Reload](https://learn.microsoft.com/visualstudio/xaml-tools/xaml-hot-reload)
- [Design-time attributes (d:DataContext)](https://learn.microsoft.com/visualstudio/xaml-tools/xaml-designer-design-time-attributes)
- [Windows Forms: TableLayoutPanel](https://learn.microsoft.com/dotnet/desktop/winforms/controls/tablelayoutpanel-control-overview-windows-forms)

</div>
