# מודול 01 — מבוא לפיתוח GUI ב-.NET: ‏WPF מול WinForms

## למה בכלל אפליקציית שולחן עבודה?

אחרי יומיים של קונסולה ו-API, היום נבנה תוכנות עם חלונות, כפתורים וטפסים. אפליקציות שולחניות עדיין
חיות ובועטות: כלים פנימיים בארגונים, מערכות מדידה ובקרה, תוכנות שעובדות offline או עם חומרה מקומית,
ו"כלי אדמין" שמפתחים בונים לעצמם. ב-.NET יש כמה טכנולוגיות לזה, ולכל אחת מקום.

## הנוף: מה קיים ב-.NET

| טכנולוגיה | מאז | פלטפורמות | הגדרת UI | מתאים ל... |
|-----------|-----|-----------|----------|-----------|
| **WinForms** | 2002 | Windows | מעצב גרפי (drag & drop) + קוד | כלים פנימיים מהירים, תחזוקת מערכות ותיקות |
| **WPF** | 2006 | Windows | XAML + data binding | אפליקציות עסקיות עשירות, UI מותאם, MVVM |
| **WinUI 3** | 2021 | Windows 10/11 | XAML (מודרני, Fluent) | אפליקציות Windows חדשות עם מראה עדכני |
| **.NET MAUI** | 2022 | Windows, macOS, iOS, Android | XAML | אפליקציה אחת למובייל ולשולחן העבודה |
| **Avalonia** | 2018 (קהילה) | Windows, macOS, Linux, web | XAML (דומה ל-WPF) | cross-platform שולחני; לא של Microsoft |

כולן חיות ונתמכות ב-.NET 10. WinForms ו-WPF הן ה"ותיקות" — ולכן גם היציבות ביותר, עם הכי הרבה
דוגמאות, ספריות ותשובות ב-Stack Overflow.

## למה WPF בקורס הזה?

1. **ה-UI הוא טקסט (XAML).** קל לקרוא, קל ל-diff ב-git, קל לשתף ב-code review — וחשוב במיוחד ליום 4:
   כלי AI מייצרים ומתקנים XAML מצוין, כי זה טקסט מובנה.
2. **Data binding ו-MVVM.** WPF נבנה סביב הרעיון של הפרדת תצוגה מלוגיקה. מה שתלמדו כאן עובר
   כמעט אחד-לאחד ל-WinUI, MAUI ו-Avalonia.
3. **פריסה רספונסיבית מובנית.** `Grid` עם star sizing מתאים את עצמו לגודל החלון בלי קוד.
4. **עדיין הבחירה הנפוצה** לאפליקציות עסקיות ב-Windows.

WinForms נלמד בקצרה במודול 07 — כי לכלי פנימי חד-פעמי, המעצב הגרפי שלו הוא הדרך המהירה ביותר.

## יצירת פרויקט

מה-CLI:

```bash
dotnet new wpf -n MyFirstWpf
cd MyFirstWpf
dotnet run
```

מ-Visual Studio: **File → New → Project → "WPF Application"** (ודאו שמותקן ה-workload
"‎.NET desktop development" — ראו `00-Setup/INSTALL.md`).

ה-`.csproj` שנוצר:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFramework>net10.0-windows</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <UseWPF>true</UseWPF>
  </PropertyGroup>
</Project>
```

שימו לב ל-`net10.0-windows` (ולא `net10.0`) ול-`UseWPF`. ‏`OutputType=WinExe` אומר "בלי חלון קונסולה".
בקורס אנחנו מוסיפים גם `<EnableWindowsTargeting>true</EnableWindowsTargeting>` — זה מאפשר לקמפל את
הפרויקט גם על Linux/macOS (למשל ב-CI), אבל **להריץ** אפשר רק ב-Windows.

## מבנה הפרויקט

```text
MyFirstWpf/
  App.xaml            ← הגדרת האפליקציה: משאבים גלובליים + StartupUri
  App.xaml.cs         ← code-behind של App (אירועי Startup/Exit)
  MainWindow.xaml     ← ה-UI של החלון הראשי
  MainWindow.xaml.cs  ← ה-code-behind: אירועים ולוגיקת "דבק"
  AssemblyInfo.cs
```

`App.xaml`:

```xml
<Application x:Class="MyFirstWpf.App"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             StartupUri="MainWindow.xaml">
    <Application.Resources>
        <!-- משאבים שזמינים לכל החלונות -->
    </Application.Resources>
</Application>
```

`StartupUri` אומר איזה חלון לפתוח בהפעלה. אין `Main()` בקוד שלכם — הוא נוצר אוטומטית.

`MainWindow.xaml`:

```xml
<Window x:Class="MyFirstWpf.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="Hello" Height="200" Width="320">
    <StackPanel Margin="16">
        <TextBox x:Name="NameBox" />
        <Button Content="Say hello" Click="Hello_Click" />
        <TextBlock x:Name="Greeting" FontSize="20" />
    </StackPanel>
</Window>
```

`MainWindow.xaml.cs`:

```csharp
using System.Windows;

namespace MyFirstWpf;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();   // טוען את ה-XAML — חובה, תמיד ראשון
    }

    private void Hello_Click(object sender, RoutedEventArgs e)
    {
        Greeting.Text = $"שלום, {NameBox.Text}!";
    }
}
```

איך `Greeting` ו-`NameBox` הפכו לשדות? המחלקה היא `partial`: בזמן build, ה-XAML מתורגם לקובץ
`MainWindow.g.cs` שמכיל את `InitializeComponent()` ושדה לכל `x:Name`. לכן:

- `x:Class` ב-XAML **חייב** להתאים ל-namespace + שם המחלקה בקוד.
- אם שכחתם `InitializeComponent()` — החלון ייפתח ריק.

## XAML בשתי דקות

XAML הוא XML שמתאר עץ של אובייקטים. כל אלמנט = מחלקה, כל attribute = property:

```xml
<Button Content="OK" Width="80" Margin="4" />
```

שקול ל:

```csharp
var b = new Button { Content = "OK", Width = 80, Margin = new Thickness(4) };
```

**Property element syntax** — כשהערך מורכב מדי ל-attribute:

```xml
<Button>
    <Button.Content>
        <StackPanel Orientation="Horizontal">
            <TextBlock Text="💾" />
            <TextBlock Text=" Save" />
        </StackPanel>
    </Button.Content>
</Button>
```

**Namespaces** — שתי השורות הקבועות בכל קובץ:

- `xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"` — הפקדים של WPF.
- `xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"` — מילות מפתח של XAML עצמו: `x:Name`, `x:Class`, `x:Key`, `x:Type`.
- `xmlns:local="clr-namespace:MyFirstWpf"` — המחלקות **שלכם** (converters, view models, user controls).

**x:Name** נותן שם לאלמנט כדי לגשת אליו מהקוד או מ-binding (`ElementName=`).

**Resources** — אובייקטים לשימוש חוזר (צבעים, סגנונות, converters) עם מפתח `x:Key`:

```xml
<Window.Resources>
    <SolidColorBrush x:Key="AccentBrush" Color="#512BD4" />
    <Style TargetType="Button">
        <Setter Property="Margin" Value="4" />
        <Setter Property="Padding" Value="12,6" />
    </Style>
</Window.Resources>
...
<TextBlock Foreground="{StaticResource AccentBrush}" />
```

`{StaticResource ...}` הוא **markup extension** — סוגריים מסולסלים בתוך attribute. נפגוש עוד: `{Binding}`,
`{DynamicResource}`, `{x:Type}`. ‏Style בלי `x:Key` חל על כל הפקדים מהסוג בהיקף שלו (implicit style).

## הכלים ב-Visual Studio

- **המעצב (Designer):** תצוגה מפוצלת — XAML למטה, תצוגה מקדימה למעלה. אפשר לגרור פקדים
  מה-Toolbox, אבל רוב המפתחים כותבים XAML ביד ומשתמשים במעצב רק לתצוגה.
- **XAML Hot Reload:** מריצים את האפליקציה (F5), עורכים XAML — והחלון הרץ מתעדכן מיד, בלי restart.
  זה הכלי החשוב ביותר לפרוטוטייפינג מהיר (מודול 07).
- **Live Visual Tree / Live Property Explorer:** לבחון את עץ הפקדים בזמן ריצה ולשנות properties.
- **Output window:** שגיאות binding מודפסות שם (`System.Windows.Data Error`), לא כחריגות!

## WPF מול WinForms: ההבדל במודל, לא רק במראה

קל לחשוב שההבדל הוא "WinForms ישן ומכוער, WPF חדש ויפה". ההבדל האמיתי עמוק יותר:

- **מי מצייר?** WinForms עוטף פקדי Windows מקוריים (GDI/User32). כל כפתור הוא "חלון" של מערכת ההפעלה,
  ולכן קשה לשנות את המראה שלו. WPF מצייר הכל בעצמו דרך DirectX — כפתור הוא רק עץ של צורות וטקסט,
  ואפשר להחליף לו את ה-`ControlTemplate` לגמרי.
- **איך מגדירים UI?** ב-WinForms המעצב מייצר קוד C# (`Form1.Designer.cs`) שמציב פקדים בקואורדינטות.
  ב-WPF ה-UI הוא XAML הצהרתי, והפריסה מחושבת בזמן ריצה.
- **איך מחברים נתונים?** ב-WinForms משתמשים ב-`textBox.Text = model.Name` ולהפך, ידנית. ב-WPF
  ה-binding עושה את זה בשני הכיוונים, וזה מה שמאפשר את MVVM.
- **DPI ומסכים מודרניים.** WPF הוא vector-based ומתאים את עצמו ל-DPI גבוה ולהגדלת טקסט של המערכת;
  WinForms עבר שיפורים בתחום, אבל עדיין דורש תשומת לב.

המשמעות המעשית: ב-WinForms מתחילים מהר יותר (גוררים, לוחצים פעמיים, כותבים קוד), אבל ב-WPF
קל יותר לתחזק, לבדוק ולשנות מראה כשהאפליקציה גדלה. לכן WinForms נשאר בכלי פנימי קטן,
ו-WPF במוצר שיחיה שנים.

## מתי לא לבחור ב-WPF

- צריך macOS או Linux → Avalonia (הכי קרוב ל-WPF) או MAUI.
- צריך מובייל → MAUI.
- צריך מראה Windows 11 מובנה (Fluent, Mica) ואפליקציה חדשה לגמרי → WinUI 3, או WPF עם ספריית עיצוב (מודול 07).
- כלי שורת פקודה עם קצת אינטראקטיביות → אולי בכלל לא GUI; ספריות כמו Spectre.Console מספיקות.

## מה קורה כשמריצים

כשמריצים אפליקציית WPF: ה-`Main` שנוצר אוטומטית יוצר `Application`, קורא ל-`App.xaml` (טוען את המשאבים
הגלובליים), ופותח את החלון מ-`StartupUri`. הבנאי של החלון קורא ל-`InitializeComponent()`, שטוען את
ה-XAML המקומפל (BAML) ובונה את עץ הפקדים — ה-**visual tree**. מכאן והלאה האפליקציה יושבת
בלולאת הודעות ומחכה לאירועים (מודול 03). חלון נסגר → אם זה החלון האחרון, האפליקציה יוצאת
(`ShutdownMode` ב-`App.xaml` שולט בזה).

## דבר קטן על Dependency Properties

תראו את המונח הזה בכל תיעוד של WPF, אז הנה ההסבר בשתי שורות: רוב ה-properties של פקדי WPF
(`Width`, `Background`, `Text`) אינן שדות רגילים אלא **Dependency Properties** — מנגנון שמאפשר להן
לקבל ערך מכמה מקורות בסדר עדיפויות (ערך מקומי > Style > ירושה מההורה > ברירת מחדל), לתמוך
ב-binding ובאנימציה, ולחסוך זיכרון כשלא נקבע ערך. אתם לא צריכים לכתוב כאלה עד שתבנו UserControl
משלכם (מודול 07); עד אז מספיק לדעת ש-`Grid.Row="1"` ו-`{Binding}` עובדים בזכותן.

## טעויות נפוצות

- `x:Class` לא תואם ל-namespace בקוד → שגיאת build מבלבלת ("does not contain a definition for InitializeComponent").
- שכחת `InitializeComponent()` בבנאי → חלון ריק, כל ה-`x:Name` הם `null`.
- ניסיון ליצור פרויקט WPF עם `net10.0` במקום `net10.0-windows`.
- שימוש ב-`Name` במקום `x:Name` — עובד על רוב הפקדים, אבל `x:Name` עובד תמיד. היו עקביים.
- לשים לוגיקה עסקית ב-code-behind. גם ב"Hello" הקטן שלנו הוצאנו את `Greeter` למחלקה נפרדת
  (ראו `Demos/Day3.Demo.HelloWpf`).

## לסיכום

- ל-.NET חמש טכנולוגיות UI; בקורס נעבוד עם WPF כי ה-UI שלו טקסטואלי, מבוסס binding, ומה שלומדים עובר לשאר.
- פרויקט WPF = `App.xaml` (משאבים + StartupUri) + חלונות, כל אחד XAML + code-behind ‏(`partial class`).
- XAML: אלמנט = מחלקה, attribute = property, ‏`x:Name` לגישה מהקוד, `Resources` + `{StaticResource}` לשימוש חוזר.
- `InitializeComponent()` תמיד ראשון בבנאי.

## קריאה נוספת

- [Desktop Guide (WPF .NET)](https://learn.microsoft.com/dotnet/desktop/wpf/overview/)
- [XAML overview](https://learn.microsoft.com/dotnet/desktop/wpf/xaml/)
- [Tutorial: Create a WPF app](https://learn.microsoft.com/dotnet/desktop/wpf/get-started/create-app-visual-studio)
- [Windows Forms overview](https://learn.microsoft.com/dotnet/desktop/winforms/overview/)
- [Choose a .NET UI technology](https://learn.microsoft.com/dotnet/desktop/)
