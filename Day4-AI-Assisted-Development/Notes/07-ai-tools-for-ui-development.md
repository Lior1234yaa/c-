<div dir="rtl">

# מודול 7 — כלי AI לפיתוח ממשק משתמש (מושב הסיום)

## למה זה המושב האחרון

UI הוא המקום שבו AI חוסך הכי הרבה זמן — ומייצר הכי הרבה "כמעט נכון". XAML הוא מפורט, חוזר על עצמו, ומלא בשמות של controls ו-namespaces שקל לטעות בהם. במושב הזה נחבר את כל מה שלמדנו היום (prompting, review, ארכיטקטורה) לתהליך עבודה מלא: **סקיצה → prompt → XAML → חידוד → binding ל-ViewModel → review**, נראה דוגמה מלאה, ונסגור את הקורס עם "מה הלאה".

## קטגוריות של כלי AI ל-UI

| קטגוריה | דוגמאות (בדקו בתיעוד העדכני) | מה מקבלים | מתאים ל- |
|----------|-------------------------------|-----------|-----------|
| **עוזרים בתוך ה-IDE** | GitHub Copilot (VS/VS Code), Claude Code / Claude, Cursor, JetBrains AI | XAML / WinForms / Blazor / React ישירות בפרויקט, עם הקשר של ה-ViewModels | מפתחי C# — הדרך הראשית שלנו היום |
| **Prompt-to-UI באינטרנט** | v0 by Vercel, Lovable, Bolt.new | אפליקציית web (React/Next) מתיאור טקסטואלי, כולל preview | אב-טיפוס מהיר, אפליקציות web |
| **כלי עיצוב עם AI** | Figma AI / Figma Make, Uizard, Galileo AI | mockups, מסכים, לפעמים קוד ראשוני | עיצוב לפני קוד, תיאום עם מעצבים |
| **תמונה/סקיצה → קוד** | כלים מסוג screenshot-to-code; Claude/Copilot עם תמונה כקלט | HTML/React/XAML מצילום מסך או סקיצה | שחזור מסך קיים, מעבר ממערכת ישנה |
| **תוספי Design-to-code** | תוספי Figma לייצוא קוד, Figma Dev Mode + MCP | קוד מקומפוננטות מעוצבות | צוותים עם design system ב-Figma |

קישורים רשמיים: v0 https://v0.app · Lovable https://lovable.dev · Bolt https://bolt.new · Figma https://www.figma.com · Uizard https://uizard.io · Galileo AI https://www.usegalileo.ai · Copilot https://github.com/features/copilot · Claude Code https://docs.claude.com/en/docs/claude-code/overview

**הערה כנה:** רוב כלי ה-Prompt-to-UI מייצרים **web** (React/HTML). ל-WPF/WinForms, הכלים המעשיים הם העוזרים בתוך ה-IDE ומודלי צ'אט — הם מכירים XAML היטב. אפשר להשתמש ב-v0/Figma ל**עיצוב** ואז לבקש מהעוזר "translate this layout to WPF XAML".

## תהליך עבודה ל-C# desktop

<div dir="ltr">

```text
1. סקיצה   — נייר/Figma/ASCII: אילו אזורים, מה בכל אזור
2. Prompt   — תיאור + אילוצים (MVVM, RTL, resources, sizes, states)
3. XAML     — הכלי מייצר; build מיד
4. חידוד    — prompt שני: מרווחים, סטיילים, מצבים (loading/empty/error)
5. Binding  — ViewModel קיים/חדש, design-time data
6. Review   — צ'ק-ליסט UI (למטה) + הרצה + נגישות + RTL
```

</div>

## דוגמה מלאה: חלון "Orders Dashboard"

### סקיצה (ASCII)

<div dir="ltr">

```text
+------------------------------------------------------------------+
| [חיפוש........] [סטטוס v] [רענן]                 לוח הזמנות      |
+---------------+---------------+----------------+-----------------+
| הזמנות היום   | הכנסות היום   | ממתינות        | לקוחות פעילים   |
+------------------------------------------------------------------+
| DataGrid: מס' | לקוח | תאריך | סכום | סטטוס                     |
|                                                                  |
+------------------------------------------------------------------+
| סטטוס: נטענו 42 הזמנות                    [ייצוא] [הזמנה חדשה]   |
+------------------------------------------------------------------+
```

</div>

### Prompt ראשון

<div dir="ltr">

```text
You are a WPF/XAML expert. Generate MainWindow.xaml only (no code-behind logic).
Context: .NET 10 WPF, MVVM without frameworks. DataContext is OrdersDashboardViewModel with:
  string SearchText; ObservableCollection<string> Statuses; string? SelectedStatus;
  ICommand RefreshCommand, ExportCommand, NewOrderCommand;
  int TodayOrders; decimal TodayRevenue; int PendingOrders; int ActiveCustomers;
  ObservableCollection<OrderRow> Orders (OrderRow: int Id, string Customer, DateTime Date, decimal Total, string Status);
  OrderRow? SelectedOrder; bool IsBusy; string StatusMessage.
Layout: Grid with 4 rows: toolbar (search TextBox, status ComboBox, Refresh Button) + title;
  4 KPI cards in a UniformGrid; DataGrid (read-only, auto columns off, currency format for Total);
  status bar with StatusMessage on one side and Export/New Order buttons on the other.
Constraints: FlowDirection="RightToLeft", Hebrew labels, MinWidth 900, MinHeight 600,
  colors/brushes via StaticResource keys (define them in Window.Resources),
  show a ProgressBar IsIndeterminate bound to IsBusy with BooleanToVisibilityConverter,
  standard namespaces only (no third-party controls).
```

</div>

### התוצאה (מקוצרת; הגרסה המלאה ב-`Demos/Day4.Demo.AiGeneratedUi`)

<div dir="ltr">

```xml
<Window x:Class="Day4.Demo.AiGeneratedUi.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="לוח הזמנות" Width="1000" Height="650" MinWidth="900" MinHeight="600"
        FlowDirection="RightToLeft" FontFamily="Segoe UI" FontSize="14">
  <Window.Resources>
    <BooleanToVisibilityConverter x:Key="BoolToVis" />
    <SolidColorBrush x:Key="AccentBrush" Color="#512BD4" />
    <SolidColorBrush x:Key="CardBrush" Color="#F3F0FF" />
    <Style x:Key="Card" TargetType="Border">
      <Setter Property="Background" Value="{StaticResource CardBrush}" />
      <Setter Property="CornerRadius" Value="8" />
      <Setter Property="Padding" Value="16" />
      <Setter Property="Margin" Value="6" />
    </Style>
  </Window.Resources>
  <Grid Margin="16">
    <Grid.RowDefinitions>
      <RowDefinition Height="Auto" />
      <RowDefinition Height="Auto" />
      <RowDefinition Height="*" />
      <RowDefinition Height="Auto" />
    </Grid.RowDefinitions>

    <DockPanel Grid.Row="0" Margin="0,0,0,12">
      <TextBlock Text="לוח הזמנות" FontSize="24" FontWeight="SemiBold" DockPanel.Dock="Right" />
      <StackPanel Orientation="Horizontal" DockPanel.Dock="Left">
        <TextBox Width="220" Margin="0,0,8,0"
                 Text="{Binding SearchText, UpdateSourceTrigger=PropertyChanged}" />
        <ComboBox Width="140" Margin="0,0,8,0" ItemsSource="{Binding Statuses}"
                  SelectedItem="{Binding SelectedStatus}" />
        <Button Content="רענן" Command="{Binding RefreshCommand}" Padding="12,4" />
      </StackPanel>
    </DockPanel>

    <UniformGrid Grid.Row="1" Columns="4">
      <Border Style="{StaticResource Card}">
        <StackPanel>
          <TextBlock Text="הזמנות היום" Foreground="Gray" />
          <TextBlock Text="{Binding TodayOrders}" FontSize="28" FontWeight="Bold" />
        </StackPanel>
      </Border>
      <!-- ...עוד 3 כרטיסים... -->
    </UniformGrid>

    <DataGrid Grid.Row="2" ItemsSource="{Binding Orders}" SelectedItem="{Binding SelectedOrder}"
              AutoGenerateColumns="False" IsReadOnly="True" Margin="0,12,0,0">
      <DataGrid.Columns>
        <DataGridTextColumn Header="מס'" Binding="{Binding Id}" Width="70" />
        <DataGridTextColumn Header="לקוח" Binding="{Binding Customer}" Width="*" />
        <DataGridTextColumn Header="תאריך" Binding="{Binding Date, StringFormat=d}" Width="110" />
        <DataGridTextColumn Header="סכום" Binding="{Binding Total, StringFormat=C}" Width="110" />
        <DataGridTextColumn Header="סטטוס" Binding="{Binding Status}" Width="110" />
      </DataGrid.Columns>
    </DataGrid>

    <DockPanel Grid.Row="3" Margin="0,12,0,0">
      <StackPanel Orientation="Horizontal" DockPanel.Dock="Left">
        <Button Content="הזמנה חדשה" Command="{Binding NewOrderCommand}" Margin="0,0,8,0" />
        <Button Content="ייצוא" Command="{Binding ExportCommand}" />
      </StackPanel>
      <ProgressBar IsIndeterminate="True" Width="120" Height="6" DockPanel.Dock="Left" Margin="12,0"
                   Visibility="{Binding IsBusy, Converter={StaticResource BoolToVis}}" />
      <TextBlock Text="{Binding StatusMessage}" VerticalAlignment="Center" />
    </DockPanel>
  </Grid>
</Window>
```

</div>

**מה בדקנו בסיבוב הראשון:** ה-build עבר. אבל: (1) בגרסה המקורית הופיע `xmlns:controls="..."` של ספרייה חיצונית שלא ביקשנו — הוסר. (2) `StringFormat=C` תלוי ב-culture של התהליך — צריך לקבוע `CultureInfo` בהפעלה או פורמט מפורש `{}{0:N2} ₪`. (3) חסר מצב "אין הזמנות" (empty state).

### Prompt שני (חידוד)

<div dir="ltr">

```text
Refine the XAML you produced:
1. Add an empty-state TextBlock "אין הזמנות להצגה" centered over the DataGrid, visible when Orders.Count == 0
   (use a DataTrigger on Orders.Count in a Style — no converter).
2. Move the brushes and the Card style to a ResourceDictionary Themes/Dashboard.xaml and merge it in Window.Resources.
3. Make the KPI value TextBlocks use a shared style "KpiValue" (FontSize 28, Bold, AccentBrush).
4. DataGrid: alternating row background, no row headers, column headers bold.
5. Add AutomationProperties.Name to the search TextBox and the buttons for accessibility.
Show only the changed parts.
```

</div>

זה סיבוב טיפוסי: הראשון נותן מבנה, השני נותן איכות. הסיבוב השלישי (אם יש) עוסק במצבי שגיאה ובמסכים קטנים.

## טיפים ל-prompts טובים ל-UI

- **Layout ראשון**: "Grid with N rows/cols", מה ב-Dock, מה נמתח (`*`) ומה קבוע (`Auto`).
- **מידות**: Min/Max של החלון, רוחב עמודות, `Margin`/`Padding` עקביים (8/12/16).
- **מצבים**: loading, empty, error, disabled — בקשו אותם במפורש; ה-AI מדלג עליהם.
- **Validation**: "Show validation errors under the field using `Validation.ErrorTemplate` and `ValidatesOnNotifyDataErrors`" (חיבור ליום 3).
- **נגישות**: `AutomationProperties.Name`, ניגודיות, ניווט במקלדת, `TabIndex`, גדלי גופן מ-resources.
- **RTL ועברית!**: `FlowDirection="RightToLeft"` על החלון; טקסטים בעברית; שימו לב ש-`DockPanel.Dock="Left"` מתהפך ויזואלית; מספרים/תאריכים — `FlowDirection="LeftToRight"` על התא אם צריך; אל תשכחו גופן שתומך בעברית.
- **ViewModel קיים**: הדביקו את חתימות ה-properties — אחרת הכלי ימציא שמות.
- **הגבלה**: "standard WPF controls only" / "no code-behind logic" / "no new packages".

## מלכודות נפוצות ב-XAML שנוצר ב-AI

| מלכודת | סימן | תיקון |
|--------|------|-------|
| Namespace חסר/מומצא | `xmlns:mah="..."`, `xmlns:sys` שלא מוגדר | הסירו או הוסיפו `xmlns:sys="clr-namespace:System;assembly=System.Runtime"` |
| Control שלא קיים | `<Card>`, `<Icon>`, `<NumericUpDown>` (אין ב-WPF הבסיסי) | `Border` + `TextBlock`, או ספרייה במודע |
| Property לא קיים | `CornerRadius` על `Button`, `Padding` על `Grid` | `Border` עוטף / Template |
| Binding לשם שגוי | `{Binding TotalAmount}` כשה-VM חושף `Total` | הדביקו את ה-VM ב-prompt; בדקו Output window (binding errors) |
| סגנונות לא עקביים | צבעים "קשיחים" בכל מקום | ResourceDictionary + הוראה "use StaticResource" |
| Converter שלא הוגדר | `Converter={StaticResource BoolToVis}` ללא הגדרה | הוסיפו ל-Resources |
| Code-behind עם לוגיקה | `Button_Click` שקורא ל-API | Command ב-ViewModel |
| RTL שבור | טקסט מיושר שמאלה, אייקונים הפוכים | `FlowDirection` על ה-Window; בדיקה ויזואלית |

## לשמור על עיצוב עקבי

1. **ResourceDictionary** אחד לצבעים/גופנים/סטיילים (`Themes/Colors.xaml`, `Themes/Controls.xaml`) — ממוזג ב-`App.xaml`.
2. **קובץ הוראות**: ב-`CLAUDE.md`/`copilot-instructions.md`: "All views use styles from Themes/*.xaml; never hardcode colors; FlowDirection RTL; spacing scale 4/8/12/16."
3. **חלון דוגמה** אחד "מושלם" שמפנים אליו: "Match the look of Views/CustomersView.xaml."
4. **Review ויזואלי**: צילום מסך ב-PR.

## מתי Web UI (Blazor) + AI הוא המסלול המהיר יותר

- כשהאפליקציה צריכה לרוץ בדפדפן/מובייל, או כשהצוות מעורב (web + .NET).
- כלי ה-Prompt-to-UI (v0, Lovable, Bolt) מייצרים HTML/CSS/React — הרבה יותר בשל מאשר XAML — ואפשר לבקש מהעוזר ב-IDE להמיר ל-Blazor components (`.razor`).
- Blazor Hybrid (MAUI/WPF host) מאפשר לשלב Razor בתוך אפליקציית desktop.
- WPF עדיין הבחירה הנכונה ל-desktop-only, אינטגרציה עמוקה עם Windows, ותוכנות קיימות.

## מה הלאה — לאחר הקורס

- **.NET MAUI** — desktop + mobile מאותו קוד: https://learn.microsoft.com/dotnet/maui/
- **Blazor** — web UI ב-C#: https://learn.microsoft.com/aspnet/core/blazor/
- **Avalonia** — XAML חוצה-פלטפורמות (קוד פתוח): https://avaloniaui.net
- **WPF docs**: https://learn.microsoft.com/dotnet/desktop/wpf/
- **CommunityToolkit.Mvvm** — source generators ל-MVVM: https://learn.microsoft.com/dotnet/communitytoolkit/mvvm/
- **ASP.NET Core** — צד השרת: https://learn.microsoft.com/aspnet/core/
- **C# docs ומה חדש**: https://learn.microsoft.com/dotnet/csharp/whats-new/
- **Microsoft Learn paths** (חינם): https://learn.microsoft.com/training/dotnet/

## סיכום הקורס

- **יום 1**: OOP, collections, LINQ, חריגות — הבסיס לחשיבה ב-C#.
- **יום 2**: async, HttpClient, JSON — לדבר עם העולם.
- **יום 3**: WPF, MVVM, validation, styles — לבנות ממשק.
- **יום 4**: AI ככלי האצה — עם prompt טוב, review קפדני, ארכיטקטורה שמחזיקה, ומדיניות ארגונית.

המסר האחרון: הכלים ישתנו כל כמה חודשים. מה שיישאר הוא היכולת שלכם **לנסח, לקרוא, לבדוק ולהסביר** קוד. תרגלו את זה — עם AI ובלעדיו.

## טעויות נפוצות

- להתחיל מ-XAML בלי ViewModel מוגדר — ואז לתקן bindings שעה.
- לקבל ספריית controls חיצונית "כי ה-AI הציע".
- לשכוח RTL ועברית עד הסוף — ואז הכול זז.
- לא לבקש מצבי loading/empty/error.
- להאמין שהעיצוב "יסתדר" — בלי ResourceDictionary אין עקביות.

## קריאה נוספת

- WPF XAML overview: https://learn.microsoft.com/dotnet/desktop/wpf/xaml/
- Data binding overview (WPF): https://learn.microsoft.com/dotnet/desktop/wpf/data/
- FlowDirection / bidirectional: https://learn.microsoft.com/dotnet/desktop/wpf/advanced/bidirectional-features-in-wpf-overview
- Accessibility (UI Automation) in WPF: https://learn.microsoft.com/dotnet/desktop/wpf/advanced/accessibility
- Copilot in Visual Studio: https://learn.microsoft.com/visualstudio/ide/visual-studio-github-copilot-extension

</div>
