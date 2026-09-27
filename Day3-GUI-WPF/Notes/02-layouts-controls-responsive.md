<div dir="rtl">

# מודול 02 — פריסה, פקדים ו-UI רספונסיבי

## מערכת הפריסה: אין קואורדינטות

ב-WPF (בניגוד ל-WinForms הקלאסי) לא אומרים "הכפתור ב-x=120, y=40". במקום זה שמים פקדים בתוך
**panels**, וכל panel מסדר את הילדים שלו לפי חוקים משלו. כשהחלון משנה גודל — ה-panel מחשב מחדש.
זה מה שהופך את ה-UI לרספונסיבי "בחינם". הפריסה מתבצעת בשני מעברים: **Measure** (כל ילד אומר כמה
מקום הוא רוצה) ו-**Arrange** (ההורה מחליט כמה הוא מקבל).

### Grid — הסוס העבודה

<div dir="ltr">

```xml
<Grid>
    <Grid.RowDefinitions>
        <RowDefinition Height="Auto" />   <!-- כגובה התוכן -->
        <RowDefinition Height="*" />      <!-- כל השאר -->
        <RowDefinition Height="40" />     <!-- פיקסלים קבועים -->
    </Grid.RowDefinitions>
    <Grid.ColumnDefinitions>
        <ColumnDefinition Width="Auto" />
        <ColumnDefinition Width="2*" />   <!-- פי 2 מהעמודה הבאה -->
        <ColumnDefinition Width="*" />
    </Grid.ColumnDefinitions>

    <TextBlock Grid.Row="0" Grid.Column="0" Text="Name:" />
    <TextBox   Grid.Row="0" Grid.Column="1" Grid.ColumnSpan="2" />
    <ListBox   Grid.Row="1" Grid.ColumnSpan="3" />
    <Button    Grid.Row="2" Grid.Column="2" Content="OK" />
</Grid>
```

</div>

שלושה סוגי גודל: `Auto` (לפי התוכן), מספר (פיקסלים) ו-`*` (**star sizing** — חלוקה יחסית של מה
שנשאר). `2*` ו-`*` = שני שליש ושליש. `Grid.Row`/`Grid.Column` הם **attached properties** — properties
שה-Grid "מצמיד" לילדים שלו. ברירת המחדל היא 0.

### StackPanel — ערימה

מסדר ילדים בשורה או בטור. פשוט, אבל **לא** מותח את הילדים למילוי המקום בכיוון הערימה:

<div dir="ltr">

```xml
<StackPanel Orientation="Horizontal">
    <Button Content="Save" />
    <Button Content="Cancel" />
</StackPanel>
```

</div>

טוב לכפתורים, לטפסים קצרים ולתוכן שגולל. לא טוב כ-root של חלון שאמור למלא את עצמו.

### DockPanel — תפריט למעלה, סטטוס למטה

<div dir="ltr">

```xml
<DockPanel LastChildFill="True">
    <Menu DockPanel.Dock="Top">...</Menu>
    <StatusBar DockPanel.Dock="Bottom">...</StatusBar>
    <TreeView DockPanel.Dock="Left" Width="200" />
    <ContentControl />   <!-- הילד האחרון ממלא את השאר -->
</DockPanel>
```

</div>

### WrapPanel — שובר שורה

כמו StackPanel, אבל כשנגמר המקום ממשיך בשורה הבאה. מצוין לסרגלי כלים, תגיות וכרטיסים.

### Canvas — מיקום מוחלט

`Canvas.Left`/`Canvas.Top`. משתמשים בו לציור, משחקים ודיאגרמות — **לא** לטפסים, כי הוא לא רספונסיבי.

### UniformGrid ו-Viewbox

`UniformGrid Columns="4"` — תאים שווים בלי הגדרות. `Viewbox` מגדיל/מקטין את התוכן כולו כמו תמונה
(שימושי ללוגו או למסך "קיוסק").

## Margin, Padding, Alignment

- **Margin** — מרווח *מחוץ* לפקד (`Margin="8"` או `"left,top,right,bottom"` או `"horizontal,vertical"`).
- **Padding** — מרווח *בתוך* הפקד, בין הגבול לתוכן (קיים ב-Border, Button, TextBox...).
- **HorizontalAlignment / VerticalAlignment** — `Stretch` (ברירת מחדל: למלא את התא), `Left`, `Center`, `Right`.
  אם נותנים `Width` מפורש — ה-Stretch מפסיק לעבוד.

כלל אצבע: אל תקבעו `Width`/`Height` אלא אם חייבים. תנו לפריסה לעבוד, והשתמשו ב-`MinWidth`/`MaxWidth`.

## הפקדים הנפוצים

| פקד | למה | property/אירוע עיקריים |
|-----|-----|-------------------------|
| `TextBlock` | טקסט לתצוגה | `Text`, `TextWrapping`, `TextTrimming` |
| `TextBox` | קלט טקסט | `Text`, `TextChanged`, `AcceptsReturn` |
| `PasswordBox` | סיסמה | `Password` (לא ניתן ל-binding בכוונה) |
| `Button` | פעולה | `Content`, `Click`, `Command`, `IsDefault`, `IsCancel` |
| `CheckBox` / `RadioButton` | בוליאני / בחירה אחת מקבוצה (`GroupName`) | `IsChecked` (nullable bool!) |
| `ComboBox` | בחירה מרשימה נפתחת | `ItemsSource`, `SelectedItem`, `SelectionChanged` |
| `ListBox` | רשימה | `ItemsSource`, `ItemTemplate`, `SelectedItem` |
| `ListView` | רשימה עם עמודות (`GridView`) | כמו ListBox + `View` |
| `DataGrid` | טבלה עם עריכה, מיון, עמודות | `ItemsSource`, `AutoGenerateColumns`, `Columns` |
| `Slider` / `ProgressBar` | ערך בטווח | `Minimum`, `Maximum`, `Value` |
| `TabControl` | לשוניות | `TabItem Header="..."` |
| `Menu` / `ContextMenu` | תפריטים | `MenuItem Header="_File"` (קו תחתון = Alt-mnemonic) |
| `StatusBar` / `ToolBar` | סרגלים | `StatusBarItem` |
| `Image` | תמונה | `Source`, `Stretch` |
| `Border` | מסגרת/רקע/פינות מעוגלות סביב ילד אחד | `CornerRadius`, `Padding` |
| `ScrollViewer` | גלילה | `VerticalScrollBarVisibility="Auto"` |

`Content` של Button (ושל כל `ContentControl`) יכול להיות כל דבר — טקסט, תמונה, panel שלם.

דוגמה שמשלבת כמה מהם, כולל binding בין פקדים (`ElementName`) בלי שורת קוד:

<div dir="ltr">

```xml
<StackPanel>
    <Slider x:Name="Amount" Minimum="0" Maximum="100" Value="40" />
    <ProgressBar Height="18" Value="{Binding ElementName=Amount, Path=Value}" />
    <TextBlock Text="{Binding ElementName=Amount, Path=Value, StringFormat=Value: {0:F0}}" />
</StackPanel>
```

</div>

## עיצוב רספונסיבי

"רספונסיבי" ב-desktop אומר: החלון נראה טוב ב-800×600 וגם ב-4K, וגם כשהמשתמש גורר את הפינה.

1. **Star sizing** במקום פיקסלים. `Width="*"` לתוכן העיקרי, `Auto` לתוויות.
2. **MinWidth / MinHeight על החלון** — מתחת לגודל מסוים כל פריסה נשברת; פשוט אל תאפשרו.
3. **ScrollViewer** סביב תוכן ארוך (טפסים, רשימות) — אבל לא סביב `DataGrid`/`ListBox` שכבר גוללים לבד.
4. **TextWrapping="Wrap"** ו-**TextTrimming="CharacterEllipsis"** לטקסט שעלול להיחתך.
5. **WrapPanel** לסרגלי כלים שיישברו לשורה שנייה בחלון צר.
6. **Viewbox** כשרוצים שהתוכן יגדל פרופורציונלית (לוח מחוונים על מסך גדול).
7. **פריסה אדפטיבית** — `SizeChanged` על החלון ושינוי הפריסה מעל/מתחת לסף. ב-`Demos/Day3.Demo.Layouts`
   הפאנל הצדדי עובר מעל התוכן כשהחלון צר מ-600px:

<div dir="ltr">

```csharp
private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
{
    var narrow = e.NewSize.Width < 600;
    if (narrow == _narrow) return;      // רק כשחוצים את הסף
    _narrow = narrow;
    SideCol.Width = narrow ? new GridLength(0) : new GridLength(220);
    Grid.SetColumn(SidePanel, narrow ? 1 : 0);
    Grid.SetRow(MainPanel, narrow ? 1 : 0);
}
```

</div>

`Grid.SetColumn(element, n)` היא הדרך לקבוע attached property מהקוד.

## UI רספונסיבי במובן השני: לא לחסום את ה-UI thread

ל-WPF יש thread אחד שמצייר ומטפל בקלט — **ה-UI thread**. כל event handler רץ עליו. אם ה-handler
עושה `Thread.Sleep(3000)` או קורא ל-API באופן סינכרוני — החלון "קופא": לא מצטייר, לא מגיב לעכבר,
ו-Windows מציג "Not Responding". זו הסיבה הכי נפוצה לאפליקציות שולחניות שמרגישות גרועות.

הפתרון הוא בדיוק מה שלמדנו ביום 2 — `async`/`await`:

<div dir="ltr">

```csharp
private async void Load_Click(object sender, RoutedEventArgs e)
{
    LoadButton.IsEnabled = false;
    try
    {
        var data = await _service.GetDataAsync();   // ה-UI thread משוחרר בזמן ההמתנה
        Grid.ItemsSource = data;                     // חוזרים אוטומטית ל-UI thread
    }
    finally { LoadButton.IsEnabled = true; }
}
```

</div>

`async void` הוא "אסור" בדרך כלל — אבל ל-event handlers הוא ההוצאה מן הכלל, כי חתימת האירוע
דורשת `void`. הכלל: תמיד `try/catch` בפנים, כי חריגה מ-`async void` מפילה את התהליך.

אחרי `await`, הקוד ממשיך על ה-UI thread (בזכות `SynchronizationContext` של WPF). אבל אם אתם
ב-thread אחר — למשל ב-`Task.Run` או ב-callback של ספרייה — אסור לגעת בפקדים ישירות:

<div dir="ltr">

```csharp
Task.Run(() =>
{
    var result = HeavyComputation();
    Dispatcher.Invoke(() => StatusText.Text = result);   // חזרה ל-UI thread
});
```

</div>

`Dispatcher.Invoke` (סינכרוני) או `Dispatcher.InvokeAsync`/`BeginInvoke` (אסינכרוני) מריצים delegate על
ה-UI thread. נגיעה בפקד מ-thread אחר זורקת `InvalidOperationException: The calling thread cannot access this object`.

`Progress<T>` פותר את זה אלגנטית לדיווח התקדמות: יוצרים אותו על ה-UI thread, וה-callback שלו תמיד רץ שם.
ראו `Demos/Day3.Demo.AsyncUi` — כולל כפתור "Block UI (bad!)" שמדגים בדיוק מה לא לעשות.

## Border, GridSplitter ו-DPI

**Border** הוא הפקד שעוטף פקד אחד ומוסיף לו מסגרת, רקע, פינות מעוגלות ו-Padding. ב-WPF אין
"Panel עם מסגרת" — עוטפים ב-Border. כמעט כל "כרטיס" ב-UI מודרני הוא `Border` עם `CornerRadius`
ובתוכו `StackPanel` או `Grid`.

**GridSplitter** מאפשר למשתמש לגרור את הגבול בין שתי עמודות/שורות של Grid — כמו בין
Solution Explorer לעורך ב-Visual Studio. שמים אותו בעמודה משלו ברוחב `Auto`, וה-Grid מטפל בשאר.

**DPI:** WPF עובד ב"יחידות לוגיות" (1/96 אינץ') ולא בפיקסלים. `Width="100"` הוא 100 פיקסלים במסך
100% ו-150 פיקסלים במסך 150%. זה קורה אוטומטית — אבל תמונות bitmap ייראו מטושטשות אם הן קטנות
מדי; העדיפו אייקונים וקטוריים (`Path`, גופני אייקונים) לפקדים.

## איך מדבגים פריסה

כשמשהו "לא במקום", הצעד הראשון הוא לראות את הגבולות: תנו ל-panel רקע זמני (`Background="LightYellow"`)
או עטפו ב-Border אדום. ב-Visual Studio, **Live Visual Tree** מציג את עץ הפקדים בזמן ריצה ו**Live Property
Explorer** מראה מי קבע כל property (Style? Local? Inherited?). שני חשודים קבועים: `HorizontalAlignment`
שמנע מתיחה, ו-`Margin` שהצטבר מ-Style ומהפקד יחד. ולבסוף — אם ה-Grid לא מתנהג, ספרו את
ה-`RowDefinitions`: פקד עם `Grid.Row="3"` בגריד של 3 שורות ייפול לשורה האחרונה בשקט.

## Visibility: שלושה מצבים

`Visibility` הוא לא bool. `Visible` מוצג, `Hidden` נסתר **אבל תופס מקום**, `Collapsed` נסתר ולא תופס מקום.
ב-99% מהמקרים רוצים `Collapsed`. זו הסיבה שצריך `BooleanToVisibilityConverter` ב-binding (מודול 04).

## טעויות נפוצות

- `StackPanel` כ-root של חלון → תוכן לא נמתח, כפתורים "מרחפים" למעלה. השתמשו ב-Grid/DockPanel.
- `Width`/`Height` קבועים על הכל → החלון לא רספונסיבי. תנו `*`, `Auto`, `MinWidth`.
- `ScrollViewer` מסביב ל-`DataGrid` → הטבלה מקבלת גובה אינסופי, הווירטואליזציה מתבטלת והכל איטי.
- `Margin` שלילי או ענק כדי "לזוז" למקום — סימן שהפריסה לא נכונה.
- קריאה סינכרונית ל-HTTP/DB ב-handler → חלון קפוא. תמיד `await`.
- `.Result` או `.Wait()` על Task ב-UI thread → **deadlock** (ה-continuation מחכה ל-UI thread שחסום על ה-Result).

## לסיכום

- פריסה ב-WPF = panels ולא קואורדינטות. `Grid` + star sizing לרוב החלון, `StackPanel` לקבוצות קטנות,
  `DockPanel` למסגרת, `WrapPanel` לסרגלים.
- `Margin` בחוץ, `Padding` בפנים, `Stretch` ברירת מחדל — ואל תקבעו גדלים קבועים.
- רספונסיבי = star sizing, MinWidth, ScrollViewer, Wrap/Trim, ואם צריך — `SizeChanged`.
- ה-UI thread קדוש: `async`/`await` בכל handler שמחכה למשהו, `Dispatcher` כשחוזרים מ-thread אחר.

## קריאה נוספת

- [Panels overview](https://learn.microsoft.com/dotnet/desktop/wpf/controls/panels-overview)
- [Grid](https://learn.microsoft.com/dotnet/api/system.windows.controls.grid)
- [Alignment, margins and padding](https://learn.microsoft.com/dotnet/desktop/wpf/advanced/alignment-margins-and-padding-overview)
- [Controls by category](https://learn.microsoft.com/dotnet/desktop/wpf/controls/controls-by-category)
- [Threading model](https://learn.microsoft.com/dotnet/desktop/wpf/advanced/threading-model)

</div>
