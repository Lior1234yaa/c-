// יום 3 — פיתוח GUI מהיר עם WPF
// build: node tools/slides/build-slides.js Day3-GUI-WPF/Slides/Day3.slides.js Day3-GUI-WPF/Slides/Day3.pptx
module.exports = {
  day: 3,
  course: 'C# ב-.NET — קורס מעשי',
  title: 'יום 3 — פיתוח GUI מהיר עם WPF',
  slides: [
    {
      type: 'title',
      title: 'יום 3 — פיתוח GUI מהיר',
      subtitle: 'WPF, XAML, אירועים, Data Binding, חיבור ל-backend, ולידציה ופרוטוטייפינג',
      meta: '09:00–16:30 | 7 מודולים | 4 מעבדות',
      notes: 'ברוכים הבאים ליום 3. היום עוברים מקונסולה לחלונות אמיתיים. נזכיר שהכל בנוי על יום 1 (מחלקות, LINQ) ויום 2 (async, HttpClient, JSON). המסר המרכזי: ה-UI הוא קליפה מעל הקוד שכבר כתבתם.',
    },
    {
      type: 'bullets', title: 'לוח הזמנים להיום', icon: 'clock',
      bullets: [
        '09:15 — מודול 01: מבוא ל-GUI, WPF מול WinForms, XAML',
        '10:00 — מודול 02: פריסה, פקדים, UI רספונסיבי',
        '11:00 — Lab 1: Unit Converter (45 דק\')',
        '11:45 — מודולים 03–04: אירועים, Data Binding ו-MVVM-lite',
        '13:30 — מודולים 05–06: חיבור ל-backend, ולידציה ו-UX',
        '14:10 — Lab 2: Contacts Manager (75 דק\')',
        '15:35 — מודול 07: פרוטוטייפינג מהיר, ואז Lab 3 / Lab 4',
      ],
      notes: 'הפסקות ב-10:45 וב-15:25, צהריים 12:45–13:30. Lab 3 ו-Lab 4 לבחירה בסוף היום, השני כשיעורי בית. תרגילים קצרים בתיקיית Exercises.',
    },
    {
      type: 'bullets', title: 'מה תדעו בסוף היום', icon: 'target',
      bullets: [
        'לבחור טכנולוגיית UI ב-.NET ולהסביר למה WPF',
        'לכתוב XAML: פריסה, פקדים, משאבים, Styles',
        'לבנות UI רספונסיבי שלא קופא (async, Dispatcher)',
        'לתכנת מונחה-אירועים: routed events, ICommand, קיצורים, timers',
        'Data binding + MVVM-lite: INotifyPropertyChanged, ObservableCollection',
        'לחבר GUI ל-services עם התקדמות, ביטול ושגיאות ידידותיות',
        'לאמת קלט, להשתמש בדיאלוגים, ולבנות פרוטוטייפ תוך שעה',
      ],
      notes: 'שבע מטרות = שבעה מודולים. הדגישו שכל מטרה נתמכת בדמו ובמעבדה. לא צריך לזכור הכל — צריך לדעת איפה לחפש ב-Notes.',
    },

    // ================= 01 =================
    { type: 'section', number: '01', title: 'מבוא ל-GUI ב-.NET', subtitle: 'WinForms, WPF, WinUI, MAUI, Avalonia — ולמה WPF',
      notes: 'מודול קצר של הקשר: מה קיים, מה נבחר ולמה. הדמו: Day3.Demo.HelloWpf.' },
    {
      type: 'cards', title: 'הנוף: טכנולוגיות UI ב-.NET',
      cards: [
        { icon: 'window', heading: 'WinForms (2002)', text: 'Windows בלבד. מעצב גרפי drag & drop. כלים פנימיים, מערכות ותיקות.' },
        { icon: 'code', heading: 'WPF (2006)', text: 'Windows. XAML + data binding + MVVM. אפליקציות עסקיות עשירות.' },
        { icon: 'star', heading: 'WinUI 3', text: 'Windows 10/11. XAML מודרני, מראה Fluent. אפליקציות חדשות.' },
        { icon: 'globe', heading: '.NET MAUI', text: 'Windows, macOS, iOS, Android. אפליקציה אחת לכל הפלטפורמות.' },
        { icon: 'cubes', heading: 'Avalonia', text: 'קהילתי. Windows/macOS/Linux/web. XAML דומה מאוד ל-WPF.' },
        { icon: 'check', heading: 'כולן חיות ב-.NET 10', text: 'הבחירה תלויה בפלטפורמה, בצוות ובאורך חיי המוצר.' },
      ],
      notes: 'טבלת ההשוואה המלאה ב-Notes/01. שאלו את הכיתה מי עבד עם מה. הדגישו ש-WinForms ו-WPF הן הוותיקות ולכן היציבות ביותר עם הכי הרבה חומר ברשת.',
    },
    {
      type: 'bullets', title: 'למה WPF בקורס הזה?', icon: 'bulb',
      bullets: [
        { text: 'ה-UI הוא טקסט (XAML)', sub: ['קל ל-diff, ל-code review — וכלי AI (יום 4) מייצרים אותו מצוין'] },
        { text: 'Data binding ו-MVVM מובנים', sub: ['מה שלומדים כאן עובר ל-WinUI, MAUI ו-Avalonia'] },
        { text: 'פריסה רספונסיבית "בחינם"', sub: ['Grid + star sizing מתאים את עצמו לגודל החלון'] },
        'עדיין הבחירה הנפוצה לאפליקציות עסקיות ב-Windows',
        'WinForms — במודול 07, ככלי לפרוטוטייפ פנימי מהיר',
      ],
      notes: 'הנקודה על AI היא חשובה להמשך הקורס: XAML הוא טקסט מובנה, ולכן ביום 4 נראה איך כלי AI כותבים ומתקנים אותו. WinForms לא נזנח — יש לו מקום ברור.',
    },
    {
      type: 'code', title: 'פרויקט WPF: מבנה ו-csproj', file: 'MyFirstWpf.csproj',
      code: `<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFramework>net10.0-windows</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <UseWPF>true</UseWPF>
  </PropertyGroup>
</Project>

MyFirstWpf/
  App.xaml / App.xaml.cs        <- משאבים + StartupUri
  MainWindow.xaml               <- ה-UI
  MainWindow.xaml.cs            <- code-behind`,
      bullets: [
        'dotnet new wpf -n MyFirstWpf',
        'net10.0-windows + UseWPF',
        'WinExe = בלי חלון קונסולה',
        'EnableWindowsTargeting מאפשר build גם ב-Linux (הרצה רק ב-Windows)',
        'אין Main() — נוצר אוטומטית',
      ],
      notes: 'הראו יצירה מה-CLI ומ-VS. הסבירו את ההבדל בין net10.0 ל-net10.0-windows. ציינו שבמאגר הקורס כל הפרויקטים מכילים EnableWindowsTargeting כדי שייבנו ב-CI.',
    },
    {
      type: 'code', title: 'XAML + code-behind: שני חצאים של מחלקה אחת', file: 'MainWindow.xaml / .xaml.cs',
      code: `<Window x:Class="MyFirstWpf.MainWindow"
  xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
  xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
  Title="Hello" Height="200" Width="320">
    <StackPanel Margin="16">
        <TextBox x:Name="NameBox" />
        <Button Content="Say hello" Click="Hello_Click" />
        <TextBlock x:Name="Greeting" FontSize="20" />
    </StackPanel>
</Window>

public partial class MainWindow : Window
{
    public MainWindow() { InitializeComponent(); }

    private void Hello_Click(object sender, RoutedEventArgs e)
        => Greeting.Text = $"שלום, {NameBox.Text}!";
}`,
      bullets: [
        'x:Class חייב להתאים ל-namespace + מחלקה',
        'partial: החצי השני נוצר מה-XAML (g.cs)',
        'x:Name → שדה במחלקה',
        'InitializeComponent() תמיד ראשון',
        'Click="..." = רישום אירוע',
      ],
      notes: 'זה השקף החשוב של המודול. הסבירו את הקסם של partial: ה-XAML מקומפל לקובץ g.cs שמכיל את InitializeComponent והשדות. שכחת InitializeComponent = חלון ריק. הדמו HelloWpf מראה גם Greeter כמחלקה נפרדת מה-UI.',
    },
    {
      type: 'bullets', title: 'XAML בשתי דקות', icon: 'file',
      bullets: [
        { text: 'אלמנט = מחלקה, attribute = property', sub: ['<Button Content="OK" Width="80" /> == new Button { Content = "OK", Width = 80 }'] },
        { text: 'Property element syntax לערכים מורכבים', sub: ['<Button.Content> <StackPanel>...</StackPanel> </Button.Content>'] },
        { text: 'Namespaces', sub: ['xmlns = פקדי WPF, xmlns:x = מילות XAML, xmlns:local = המחלקות שלכם'] },
        { text: 'Resources + {StaticResource Key}', sub: ['צבעים, Styles, converters לשימוש חוזר; Style בלי x:Key חל על כל הפקדים מהסוג'] },
        'Markup extensions: {Binding}, {StaticResource}, {DynamicResource}, {x:Type}',
      ],
      notes: 'אל תעמיקו יותר מדי — הכל יחזור בהקשר במודולים הבאים. המטרה: שיוכלו לקרוא XAML. הראו ב-HelloWpf את ה-Style ב-Window.Resources וה-Brush ב-App.Resources.',
    },
    {
      type: 'cards', title: 'הכלים ב-Visual Studio',
      cards: [
        { icon: 'eye', heading: 'Designer', text: 'תצוגה מפוצלת: XAML + תצוגה מקדימה. גרירה מה-Toolbox אפשרית, אבל רוב המפתחים כותבים XAML ביד.' },
        { icon: 'bolt', heading: 'XAML Hot Reload', text: 'F5, עורכים XAML, שומרים — החלון הרץ מתעדכן מיד. הכלי המרכזי לפרוטוטייפינג.' },
        { icon: 'sitemap', heading: 'Live Visual Tree', text: 'עץ הפקדים בזמן ריצה + Live Property Explorer: מי קבע כל property.' },
        { icon: 'bug', heading: 'Output window', text: 'שגיאות binding מודפסות כאן — לא כחריגות! System.Windows.Data Error.' },
      ],
      notes: 'הדגימו Hot Reload חי אם יש Windows בכיתה: שנו Margin ותראו את החלון משתנה. ה-Output window יחזור במודול 04 — שגיאת binding לא מפילה את התוכנית.',
    },

    // ================= 02 =================
    { type: 'section', number: '02', title: 'פריסה, פקדים ו-UI רספונסיבי', subtitle: 'Grid, StackPanel, DockPanel, WrapPanel — ולא לחסום את ה-UI thread',
      notes: 'הדמו: Day3.Demo.Layouts עם TabControl לכל panel. שינוי גודל החלון מדגים את הפריסה האדפטיבית.' },
    {
      type: 'code', title: 'Grid: הסוס העבודה', file: 'Grid + star sizing',
      code: `<Grid>
    <Grid.RowDefinitions>
        <RowDefinition Height="Auto" />   <!-- כגובה התוכן -->
        <RowDefinition Height="*" />      <!-- כל השאר -->
        <RowDefinition Height="40" />     <!-- פיקסלים -->
    </Grid.RowDefinitions>
    <Grid.ColumnDefinitions>
        <ColumnDefinition Width="Auto" />
        <ColumnDefinition Width="2*" />   <!-- פי 2 -->
        <ColumnDefinition Width="*" />
    </Grid.ColumnDefinitions>

    <TextBlock Grid.Row="0" Grid.Column="0" Text="Name:" />
    <TextBox   Grid.Row="0" Grid.Column="1" Grid.ColumnSpan="2" />
    <ListBox   Grid.Row="1" Grid.ColumnSpan="3" />
    <Button    Grid.Row="2" Grid.Column="2" Content="OK" />
</Grid>`,
      bullets: [
        'אין קואורדינטות — panels וחוקים',
        'Auto / פיקסלים / * (יחסי)',
        'Grid.Row, Grid.Column = attached properties',
        'ColumnSpan / RowSpan',
        'Measure → Arrange בכל שינוי גודל',
      ],
      notes: 'הסבירו star sizing עם דוגמה מספרית: 2* ו-* = שני שליש ושליש. attached property = property שההורה מצמיד לילד. הראו ב-Layouts demo את הטאב Grid ושנו את גודל החלון.',
    },
    {
      type: 'cards', title: 'שאר ה-Panels',
      cards: [
        { icon: 'list', heading: 'StackPanel', text: 'ערימה אנכית/אופקית. לכפתורים ולטפסים קצרים. לא מותח ילדים בכיוון הערימה.' },
        { icon: 'window', heading: 'DockPanel', text: 'Top/Bottom/Left/Right + הילד האחרון ממלא. מסגרת חלון: תפריט למעלה, סטטוס למטה.' },
        { icon: 'arrows', heading: 'WrapPanel', text: 'שובר שורה כשנגמר המקום. סרגלי כלים, תגיות, כרטיסים.' },
        { icon: 'paint', heading: 'Canvas', text: 'מיקום מוחלט (Left/Top). ציור ומשחקים — לא טפסים.' },
        { icon: 'cubes', heading: 'UniformGrid', text: 'תאים שווים בלי הגדרות. Columns="4" — מצוין לכרטיסים.' },
        { icon: 'search', heading: 'Viewbox', text: 'מגדיל/מקטין את התוכן כולו כמו תמונה. לוגו, מסך קיוסק.' },
      ],
      notes: 'שאלה טובה: "מה ה-root של חלון טיפוסי?" — DockPanel או Grid, אף פעם לא StackPanel. הראו את הטאב Panels בדמו.',
    },
    {
      type: 'bullets', title: 'Margin, Padding, Alignment', icon: 'box',
      bullets: [
        { text: 'Margin — מרווח מחוץ לפקד', sub: ['"8" | "left,top,right,bottom" | "horizontal,vertical"'] },
        { text: 'Padding — מרווח בתוך הפקד', sub: ['Border, Button, TextBox...'] },
        { text: 'HorizontalAlignment / VerticalAlignment', sub: ['Stretch (ברירת מחדל: למלא), Left, Center, Right'] },
        'Width קבוע מבטל את ה-Stretch — השתמשו ב-MinWidth / MaxWidth',
        'Visibility: Visible / Hidden (תופס מקום) / Collapsed (לא תופס)',
        'Border = מסגרת + רקע + CornerRadius סביב ילד אחד',
      ],
      notes: 'כלל אצבע: אל תקבעו Width/Height אלא אם חייבים. Visibility.Collapsed כמעט תמיד. Border הוא הבסיס לכל "כרטיס" ב-UI מודרני.',
    },
    {
      type: 'two-col', title: 'הפקדים הנפוצים',
      right: { heading: 'קלט ופעולה', bullets: [
        'TextBox (Text, TextChanged), PasswordBox',
        'Button (Content, Click, Command, IsDefault, IsCancel)',
        'CheckBox / RadioButton (IsChecked — bool?)',
        'ComboBox (ItemsSource, SelectedItem)',
        'Slider / ProgressBar (Minimum, Maximum, Value)',
      ] },
      left: { heading: 'תצוגה ומבנה', bullets: [
        'TextBlock (TextWrapping, TextTrimming), Image',
        'ListBox / ListView (ItemTemplate)',
        'DataGrid (Columns, AutoGenerateColumns)',
        'TabControl, Menu, ContextMenu, StatusBar, ToolBar',
        'ScrollViewer, GridSplitter, Expander',
      ] },
      notes: 'טבלה מלאה ב-Notes/02. הדגישו ש-Content של Button יכול להיות כל דבר, ו-IsChecked הוא nullable. הראו את הטאב Controls בדמו — כולל ElementName binding בין Slider ל-ProgressBar בלי קוד.',
    },
    {
      type: 'bullets', title: 'עיצוב רספונסיבי (במובן של גודל)', icon: 'arrows',
      bullets: [
        'Star sizing במקום פיקסלים; Auto לתוויות',
        'MinWidth / MinHeight על החלון — מתחת לזה הכל נשבר',
        'ScrollViewer סביב תוכן ארוך (לא סביב DataGrid!)',
        'TextWrapping="Wrap", TextTrimming="CharacterEllipsis"',
        'WrapPanel לסרגלים שיישברו לשורה שנייה',
        { text: 'פריסה אדפטיבית: SizeChanged', sub: ['מתחת ל-600px: הפאנל הצדדי עובר מעל התוכן (Grid.SetColumn)'] },
        'DPI: WPF עובד ביחידות לוגיות — 100 = 100 ב-100%, 150 ב-150%',
      ],
      notes: 'הראו את הטאב Responsive בדמו: הצרו את החלון מתחת ל-600px. ציינו את ה-guard "רק כשחוצים את הסף" בקוד. ScrollViewer סביב DataGrid מבטל וירטואליזציה — טעות נפוצה.',
    },
    {
      type: 'code', title: 'רספונסיבי במובן השני: לא לחסום את ה-UI thread', file: 'async event handler',
      code: `// רע: החלון קופא ל-3 שניות
private void Load_Click(object sender, RoutedEventArgs e)
{
    var data = _service.GetData();   // HTTP סינכרוני / Thread.Sleep
    Grid.ItemsSource = data;
}

// טוב: ה-UI thread משוחרר בזמן ההמתנה
private async void Load_Click(object sender, RoutedEventArgs e)
{
    LoadButton.IsEnabled = false;
    try
    {
        var data = await _service.GetDataAsync();
        Grid.ItemsSource = data;     // חזרנו ל-UI thread אוטומטית
    }
    catch (HttpRequestException ex) { ShowError(ex.Message); }
    finally { LoadButton.IsEnabled = true; }
}`,
      bullets: [
        'UI thread אחד מצייר ומטפל בקלט',
        'handler שמחכה = "Not Responding"',
        'async void מותר רק ב-event handlers',
        'תמיד try/catch בפנים',
        'אחרי await — חזרה ל-UI thread',
        'מ-Task.Run: Dispatcher.Invoke',
      ],
      notes: 'זה ממשיך ישירות מיום 2. הדגימו את הכפתור "Block UI (bad!)" ב-Day3.Demo.AsyncUi ונסו להזיז את החלון. .Result/.Wait על ה-UI thread = deadlock. Dispatcher יורחב במודול 05.',
    },
    {
      type: 'lab', title: 'Lab 1 — Unit Converter',
      goal: 'האפליקציה הראשונה ב-WPF: פריסה עם Grid, ComboBox ליחידות, המרה חיה עם ולידציה.',
      duration: '45 דקות',
      deliverable: 'ממיר יחידות אורך שעובד בלחיצה, בזמן הקלדה ועם Enter; קלט שגוי מציג הודעה ולא קורס.',
      tasks: [
        'ממשו UnitConverter.Convert ו-TryParseInput (לוגיקה בלי WPF)',
        'בנו את ה-Grid: 3 עמודות (Auto, *, Auto) ו-6 שורות',
        'מלאו את ה-ComboBox-ים מ-LengthUnit.All',
        'חברו TextChanged + SelectionChanged ל-Convert() — המרה חיה',
        'הציגו שגיאה אדומה על קלט לא תקין והשביתו את הכפתור',
        'Enter מפעיל (IsDefault), Escape מנקה; בונוס: כפתור Swap',
      ],
      notes: 'Starter ב-Labs/Lab1-UnitConverter/Starter. עברו בכיתה על ה-TODO-ים בסדר. נקודות למעקב: IsLoaded guard ב-Convert (SelectionChanged נורה בבנאי), ו-CultureInfo בפרסור.',
    },

    // ================= 03 =================
    { type: 'section', number: '03', title: 'אירועים ותכנות מונחה-אירועים', subtitle: 'routed events, sender/e, ICommand, קיצורי מקלדת, DispatcherTimer',
      notes: 'הדמו: Day3.Demo.Events — יומן אירועים שמראה את סדר tunneling/bubbling.' },
    {
      type: 'bullets', title: 'המודל: התוכנית מחכה, המשתמש מוביל', icon: 'mouse',
      bullets: [
        'קונסולה: הקוד "מושך" קלט מלמעלה למטה',
        'GUI: לולאת הודעות; כל פעולה = אירוע; אתם רושמים handlers',
        { text: 'חתימה אחידה: (object sender, TEventArgs e)', sub: ['sender = מי הפעיל (שימושי ל-handler משותף)', 'e = מה קרה: Key, GetPosition(), Cancel, NewSize...'] },
        'רישום ב-XAML: Click="Save_Click" | בקוד: button.Click += Save_Click',
        'lambda handlers לגיטימיים — אבל אי אפשר להסיר; זהירות עם אובייקטים ארוכי-חיים',
      ],
      notes: 'קשרו ל-delegates מיום 1: אירוע הוא delegate multicast. דליפת הזיכרון הקלאסית: service סטטי שמחזיק handler של חלון. הסירו ב-Closed.',
    },
    {
      type: 'code', title: 'Routed events: מנהור ובעבוע', file: 'Day3.Demo.Events',
      code: `<!-- handler אחד על ה-Grid במקום עשרים על כפתורים -->
<Grid Button.Click="AnyButton_Click"
      PreviewMouseDown="Grid_PreviewMouseDown">
    <Button Content="1" /> <Button Content="2" /> ...
</Grid>

private void AnyButton_Click(object sender, RoutedEventArgs e)
{
    // sender = ה-Grid; OriginalSource = הכפתור שבאמת נלחץ
    if (e.OriginalSource is Button b) Display.Text += b.Content;
}

private void ButtonB_Click(object sender, RoutedEventArgs e)
{
    e.Handled = true;   // עוצר את הבעבוע — ההורים לא יקבלו
}`,
      bullets: [
        'Bubbling: מהפקד למעלה (Click, KeyDown)',
        'Tunneling: מה-root למטה (Preview*), תמיד קודם',
        'e.OriginalSource = המקור האמיתי',
        'e.Handled = true עוצר את המסע',
        'Preview* = ליירט לפני שהפקד מגיב',
      ],
      notes: 'הריצו את הדמו ולחצו על Button A ואז Button B — היומן מראה Preview ב-Border, Click בכפתור, ואז Click מבעבע ל-Border (רק עבור A). תרגיל 2 (keypad) משתמש בדיוק בזה.',
    },
    {
      type: 'two-col', title: 'אירועים שתפגשו כל יום',
      right: { heading: 'פקדים', bullets: [
        'Click — Button, MenuItem (עכבר, Enter, רווח)',
        'TextChanged — TextBox, כל שינוי',
        'SelectionChanged — ComboBox, ListBox, DataGrid (גם מהקוד!)',
        'KeyDown / PreviewKeyDown — e.Key == Key.Enter',
        'Checked / Unchecked, ValueChanged, MouseDoubleClick',
      ] },
      left: { heading: 'חלון ומחזור חיים', bullets: [
        'Loaded — הפקד מוכן; מקום לטעינת נתונים',
        'Closing — לפני סגירה; e.Cancel = true מבטל',
        'Closed — אחרי סגירה; ניקוי timers וקבצים',
        'SizeChanged — פריסה אדפטיבית',
        'if (!IsLoaded) return; — נגד SelectionChanged מוקדם',
      ] },
      notes: 'SelectionChanged שנורה בבנאי הוא הבאג הראשון שכולם פוגשים. Closing עם MessageBox YesNo — הראו בדמו: הקלידו משהו, סגרו, ראו את השאלה.',
    },
    {
      type: 'code', title: 'ICommand: הפעולה כאובייקט', file: 'RelayCommand.cs',
      code: `public sealed class RelayCommand(Action execute,
                                 Func<bool>? canExecute = null)
    : ICommand
{
    public bool CanExecute(object? p) => canExecute?.Invoke() ?? true;
    public void Execute(object? p) => execute();

    // WPF בודק מחדש CanExecute אחרי כל אינטראקציה
    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
// שימוש:
SaveCommand = new RelayCommand(Save, () => _dirty);

<Button Content="Save" Command="{Binding SaveCommand}" />
<MenuItem Header="_Save" Command="{Binding SaveCommand}" />`,
      bullets: [
        'CanExecute=false → הכפתור מושבת לבד',
        'אותה פעולה מכפתור, תפריט וקיצור',
        'חי ב-ViewModel, לא ב-code-behind',
        'CommandManager.InvalidateRequerySuggested() אחרי await',
        'CommunityToolkit.Mvvm: [RelayCommand]',
      ],
      notes: 'זו המחלקה שנשתמש בה כל היום. הדגישו את ההבדל מ-Click + IsEnabled ידני: אי אפשר לשכוח לעדכן. טבלת "אירוע או Command" ב-Notes/03.',
    },
    {
      type: 'code', title: 'קיצורי מקלדת ו-DispatcherTimer', file: 'InputBindings + timer',
      code: `<Window.InputBindings>
    <KeyBinding Key="S" Modifiers="Ctrl"
                Command="{Binding SaveCommand}" />
    <KeyBinding Key="F5" Command="{Binding RefreshCommand}" />
    <KeyBinding Key="Delete" Command="{Binding DeleteCommand}" />
</Window.InputBindings>

<Button Content="_Save" IsDefault="True" />   <!-- Enter, Alt+S -->
<Button Content="_Cancel" IsCancel="True" />  <!-- Escape -->

// DispatcherTimer: Tick רץ על ה-UI thread
private readonly DispatcherTimer _timer =
    new() { Interval = TimeSpan.FromSeconds(1) };

_timer.Tick += (_, _) => Clock.Text = DateTime.Now.ToString("T");
Loaded += (_, _) => _timer.Start();
Closed += (_, _) => _timer.Stop();   // אחרת החלון חי לנצח`,
      bullets: [
        'InputBindings עובדים רק עם Commands',
        'IsDefault / IsCancel / _mnemonic',
        'DispatcherTimer ≠ System.Timers.Timer',
        'Stopwatch למדידה, timer רק לציור',
        'Debounce: Stop + Start בכל הקשה',
      ],
      notes: 'System.Timers.Timer מפעיל callback ב-thread pool — נגיעה בפקד משם זורקת חריגה. תרגיל 6 (stopwatch) ו-Lab 4 (dashboard) משתמשים ב-DispatcherTimer. Debounce לחיפוש — קוד מלא ב-Notes/03.',
    },

    // ================= 04 =================
    { type: 'section', number: '04', title: 'Data Binding ו-MVVM-lite', subtitle: 'DataContext, INotifyPropertyChanged, ObservableCollection, DataTemplate, converters',
      notes: 'הדמו: Day3.Demo.Binding — רשימת משימות עם פאנל פרטים, converters ו-RelativeSource.' },
    {
      type: 'bullets', title: 'DataContext ו-{Binding}', icon: 'link',
      bullets: [
        'הבעיה: Greeting.Text = ... מכל מקום → code-behind סלט, בלתי ניתן לבדיקה',
        { text: 'DataContext = האובייקט שה-binding מסתכל עליו; עובר בירושה במורד העץ', sub: ['DataContext = new MainViewModel(); פעם אחת על החלון'] },
        { text: '{Binding Path}: NewTitle, Selected.Name, Items.Count', sub: ['ElementName= לפקד אחר; RelativeSource= להורה'] },
        { text: 'Mode: OneWay (ברירת מחדל) | TwoWay (TextBox.Text, IsChecked, SelectedItem) | OneTime', sub: ['UpdateSourceTrigger=PropertyChanged — כל הקשה, לא רק LostFocus'] },
        'FallbackValue / TargetNullValue — בלי converter',
        'שגיאת binding לא זורקת! → Output window: System.Windows.Data Error',
      ],
      notes: 'הדגישו את הנקודה האחרונה שוב: שגיאת כתיב ב-Path משאירה שדה ריק בשקט. UpdateSourceTrigger=PropertyChanged נחוץ כשרוצים CanExecute חי על TextBox.',
    },
    {
      type: 'code', title: 'INotifyPropertyChanged דרך מחלקת בסיס', file: 'Mvvm/ObservableObject.cs',
      code: `public abstract class ObservableObject : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged(
        [CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this,
               new PropertyChangedEventArgs(name));

    protected bool SetProperty<T>(ref T field, T value,
        [CallerMemberName] string? name = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;
        field = value; OnPropertyChanged(name); return true;
    }
}
public class TodoItem : ObservableObject
{
    private string _title = "";
    public string Title
    { get => _title; set => SetProperty(ref _title, value); }
}`,
      bullets: [
        'binding ל-property רגיל = פעם אחת בלבד',
        '[CallerMemberName] — בלי מחרוזות קסם',
        'SetProperty מחזיר true אם השתנה',
        'property מחושב: OnPropertyChanged(nameof(Summary)) ידנית',
        'CommunityToolkit: [ObservableProperty]',
      ],
      notes: 'כתבו את המחלקה הזו פעם אחת והעתיקו לכל פרויקט. השאלה שתמיד עולה: למה property מחושב לא מתעדכן? כי אין מי שיודיע עליו. הראו Summary בדמו.',
    },
    {
      type: 'code', title: 'ObservableCollection + DataTemplate', file: 'MainViewModel + XAML',
      code: `// ViewModel: הרשימה מודיעה על Add/Remove
public ObservableCollection<TodoItem> Items { get; } = [];
public TodoItem? Selected
{ get => _selected; set => SetProperty(ref _selected, value); }

<ListBox ItemsSource="{Binding Items}"
         SelectedItem="{Binding Selected}">
    <ListBox.ItemTemplate>
        <DataTemplate>   <!-- DataContext כאן = הפריט -->
            <DockPanel>
                <CheckBox IsChecked="{Binding IsDone}" />
                <TextBlock Text="{Binding Title}" Margin="6,0" />
                <TextBlock Foreground="Gray"
                  Text="{Binding Created, StringFormat=d}" />
            </DockPanel>
        </DataTemplate>
    </ListBox.ItemTemplate>
</ListBox>

<!-- מתוך template, חזרה ל-VM של החלון: -->
<Button Command="{Binding DataContext.RemoveCommand,
    RelativeSource={RelativeSource AncestorType=Window}}" />`,
      bullets: [
        'List<T> לא מודיע; ObservableCollection כן',
        'האוסף מודיע על הוספה/הסרה — הפריט על שינוי בתוכו',
        'DataTemplate = איך נראה פריט',
        'StringFormat: {} בהתחלה = escape',
        'רק מה-UI thread!',
      ],
      notes: 'שני מקורות הודעה: האוסף (Add/Remove) והפריט (INotifyPropertyChanged). CheckBox ב-template מדגים TwoWay לתוך הפריט. RelativeSource מבלבל — ציירו את עץ ה-DataContext על הלוח.',
    },
    {
      type: 'code', title: 'Converters: כשהמקור והיעד מסוגים שונים', file: 'BoolToBrushConverter.cs',
      code: `public class BoolToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType,
                          object parameter, CultureInfo culture)
        => value is true ? Brushes.SeaGreen : Brushes.DimGray;

    public object ConvertBack(object value, Type targetType,
                              object parameter, CultureInfo culture)
        => throw new NotSupportedException();   // one-way
}

<Window.Resources>
    <conv:BoolToBrushConverter x:Key="DoneBrush" />
    <BooleanToVisibilityConverter x:Key="BoolToVis" /> <!-- מובנה -->
</Window.Resources>

<TextBlock Foreground="{Binding IsDone,
                        Converter={StaticResource DoneBrush}}" />
<ProgressBar Visibility="{Binding IsBusy,
                          Converter={StaticResource BoolToVis}}" />`,
      bullets: [
        'bool → Brush, enum → טקסט, double → Visibility',
        'רשום כ-resource, משמש ב-{Binding Converter=}',
        'ConverterParameter — ערך מה-XAML',
        'BooleanToVisibilityConverter מובנה',
        'Visibility הוא enum, לא bool',
      ],
      notes: 'תרגיל 9 בונה שני converters. הזכירו ש-StringFormat עובד רק כשהיעד string — ל-Content של Button צריך converter או ContentStringFormat.',
    },
    {
      type: 'steps', title: 'MVVM-lite: ארבע שכבות, בלי דוגמה',
      steps: [
        { heading: 'View — XAML + code-behind מינימלי', text: 'יוצר את ה-VM, קובע DataContext, מטפל ב-UI טהור (SizeChanged, פוקוס, דיאלוגים).' },
        { heading: 'ViewModel — ObservableObject', text: 'מצב המסך + ICommand-ים. אין using System.Windows.Controls. נבדק ב-xUnit בלי חלון.' },
        { heading: 'Service — מאחורי ממשק', text: 'IProductService: I/O, HTTP, קבצים. Fake לפיתוח ובדיקות, Http לייצור.' },
        { heading: 'Model — נתונים', text: 'Product, Contact. records או ObservableObject כשצריך עריכה חיה.' },
        { heading: 'DI-lite', text: 'ה-VM מקבל services בבנאי; ה-View (או App) הוא ה-composition root.' },
      ],
      notes: 'זו התבנית של Labs 2–4. אל תהפכו את זה לדת: "lite" אומר פרגמטי. הכלל היחיד שלא מתפשרים עליו: ה-VM לא מכיר פקדים.',
    },
    {
      type: 'two-col', title: 'ידני מול CommunityToolkit.Mvvm',
      right: { heading: 'מה שכתבנו היום (אפס תלויות)', code: `public class VM : ObservableObject
{
    private string _title = "";
    public string Title
    {
        get => _title;
        set => SetProperty(ref _title,
                           value);
    }

    public ICommand AddCommand { get; }
    public VM()
    {
        AddCommand =
            new RelayCommand(Add, CanAdd);
    }
}` },
      left: { heading: 'עם source generators (NuGet)', code: `public partial class VM : ObservableObject
{
    [ObservableProperty]
    private string _title = "";

    [RelayCommand(
        CanExecute = nameof(CanAdd))]
    private void Add() { ... }

    private bool CanAdd()
        => !string.IsNullOrWhiteSpace(Title);
}

// גם: AsyncRelayCommand, Messenger, Ioc` },
      notes: 'CommunityToolkit.Mvvm של Microsoft מייצר בקומפילציה את מה שכתבנו ידנית. בקורס נשארים ידניים כדי להבין ולבנות ללא רשת; בפרויקט אמיתי — קחו את ה-toolkit.',
    },

    // ================= 05 =================
    { type: 'section', number: '05', title: 'חיבור ה-GUI ל-backend', subtitle: 'services וממשקים, async commands, IsBusy + Progress + Cancel, Dispatcher, שגיאות, %AppData%',
      notes: 'הדמו: Day3.Demo.AsyncUi. כאן מתחבר הקוד של יום 2 (HttpClient + JSON) לחלון.' },
    {
      type: 'bullets', title: 'ה-UI הוא קליפה: services מאחורי ממשק', icon: 'puzzle',
      bullets: [
        'הקוד מימים 1–2 לא צריך לדעת שהוא רץ מתחת לחלון',
        { text: 'IProductService: Task<IReadOnlyList<Product>> GetProductsAsync(progress, ct)', sub: ['FakeProductService — in-memory, Task.Delay, יודע להיכשל', 'HttpProductService — HttpClient + System.Text.Json (יום 2)'] },
        'למה ממשק: פיתוח בלי שרת, בדיקות עם fake, החלפה עתידית',
        { text: 'DI-lite: הזרקה דרך הבנאי', sub: ['DataContext = new ProductsViewModel(new HttpProductService(Http));'] },
        'DTO נפרד מהמודל של ה-UI — ה-API משתנה, ה-XAML לא',
        'HttpClient אחד static לכל האפליקציה',
      ],
      notes: 'קשרו ל-Lab 3 שבו יש בדיוק את המבנה הזה. שאלו: איפה בונים את ה-service הקונקרטי? רק ב-composition root. בפרויקט גדול — Microsoft.Extensions.DependencyInjection עובד גם ב-WPF.',
    },
    {
      type: 'code', title: 'השלישייה: IsBusy, Progress, Cancel', file: 'ProductsViewModel.LoadAsync',
      code: `public async Task LoadAsync()
{
    _cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
    IsBusy = true; Error = null; Progress = 0;
    var progress = new Progress<int>(p => Progress = p); // UI thread
    try
    {
        var items = await _service.GetProductsAsync(progress,
                                                    _cts.Token);
        Products.Clear();
        foreach (var p in items) Products.Add(p);
        Status = $"Loaded {items.Count}";
    }
    catch (OperationCanceledException) { Status = "Cancelled"; }
    catch (HttpRequestException ex)
    {
        Error = "לא ניתן להתחבר לשרת. בדקו שהשרת רץ ונסו שוב.";
    }
    finally { IsBusy = false; _cts.Dispose(); _cts = null; }
}

public void Cancel() => _cts?.Cancel();`,
      bullets: [
        'IsBusy: מכבה כפתורים, מציג ProgressBar',
        'Progress<T> נוצר על ה-UI thread → callback רץ שם',
        'CancellationToken עובר עד ל-HttpClient/Task.Delay',
        'OperationCanceledException ≠ שגיאה',
        'finally חובה — אחרת "טוען" לנצח',
        'AsyncRelayCommand מונע לחיצה כפולה',
      ],
      notes: 'כל פעולה ארוכה צריכה את שלושתם. ה-timeout ב-CTS הוא בונוס נחמד. הדגימו ב-AsyncUi: Load, Cancel באמצע, ו-Simulate errors.',
    },
    {
      type: 'code', title: 'Dispatcher: כשאתם לא על ה-UI thread', file: 'Task.Run + Dispatcher',
      code: `// אחרי await — אתם על ה-UI thread. אבל כאן לא:
Task.Run(() =>
{
    var report = BuildHeavyReport();         // thread pool

    // ReportText.Text = report;
    // ↑ InvalidOperationException:
    //   The calling thread cannot access this object

    Dispatcher.Invoke(() => ReportText.Text = report);  // סינכרוני
    // Application.Current.Dispatcher.InvokeAsync(...) // מכל מקום
});

// הכי פשוט: תנו ל-await לעשות את העבודה
var report = await Task.Run(BuildHeavyReport);
ReportText.Text = report;   // כבר חזרנו ל-UI thread`,
      bullets: [
        'מתי: Task.Run, callbacks של ספריות, System.Timers.Timer',
        'Invoke — מחכה; InvokeAsync/BeginInvoke — לא',
        'ObservableCollection גם דורש UI thread',
        'עדיף: await Task.Run(...)',
      ],
      notes: 'הכפתור "Background thread → Dispatcher" בדמו. הגרסה עם await Task.Run היא כמעט תמיד הנכונה — Dispatcher.Invoke נשאר ל-callbacks שאין לכם שליטה עליהם.',
    },
    {
      type: 'two-col', title: 'שגיאות: MessageBox או inline?',
      right: { heading: 'MessageBox', bullets: [
        'חוסם את המשתמש',
        'לאישורים (מחיקה) ולשגיאה קטלנית',
        'ברירת מחדל בטוחה: MessageBoxResult.No',
        'תמיד עם Owner — אחרת נפתח מאחורי החלון',
        'לא מאפשר Retry אמיתי',
      ] },
      left: { heading: 'באנר inline + Retry', bullets: [
        'לא חוסם; נשאר על המסך להקשר',
        'לשגיאות רשת, ולידציה, "נסה שוב"',
        'Border אדמדם + TextBlock {Binding Error} + Button Retry',
        'Visibility={Binding HasError, Converter=BoolToVis}',
        'הודעה = מה קרה + מה לעשות; ex.ToString() ללוג בלבד',
      ] },
      notes: 'תפסו חריגות ספציפיות (HttpRequestException, JsonException, IOException) ורק בסוף Exception. הפרדה: הודעה ידידותית למשתמש, פרטים טכניים ללוג.',
    },
    {
      type: 'code', title: 'הגדרות ו-cache: JSON ב-%AppData%', file: 'SettingsStore.cs',
      code: `public static class SettingsStore
{
    static readonly string Dir = Path.Combine(
        Environment.GetFolderPath(
            Environment.SpecialFolder.ApplicationData), "MyApp");
    static readonly string FilePath = Path.Combine(Dir, "app.json");

    public static AppSettings Load()
    {
        try
        {
            if (!File.Exists(FilePath)) return new();
            var json = File.ReadAllText(FilePath);
            return JsonSerializer.Deserialize<AppSettings>(json)!;
        }
        catch (Exception ex) when (ex is IOException or JsonException)
        { return new(); }   // קובץ פגום = ברירת מחדל
    }

    public static void Save(AppSettings s)
    {
        Directory.CreateDirectory(Dir);
        File.WriteAllText(FilePath, JsonSerializer.Serialize(s));
    }
}`,
      bullets: [
        'לא ליד ה-exe (אין הרשאות ב-Program Files)',
        'ApplicationData = %AppData% (roaming)',
        'LocalApplicationData = cache גדול',
        'קובץ פגום → ברירת מחדל, לא קריסה',
        'Load בבנאי/Loaded, Save ב-Closing',
        'Closing + async: e.Cancel; await; Close()',
      ],
      notes: 'Lab 2 שומר אנשי קשר באותה דרך. הטריק של Closing אסינכרוני חשוב: .GetAwaiter().GetResult() ב-Closing = deadlock. הפתרון מוסבר ב-NOTES של Lab 2.',
    },

    // ================= 06 =================
    { type: 'section', number: '06', title: 'ולידציה ואינטראקציה עם המשתמש', subtitle: 'ValidationRule, INotifyDataErrorInfo, ErrorTemplate, דיאלוגים, מקלדת ונגישות',
      notes: 'הדמו: Day3.Demo.Validation — שלושה טאבים: ValidationRule, INotifyDataErrorInfo, Dialogs.' },
    {
      type: 'cards', title: 'שלוש רמות של ולידציה',
      cards: [
        { icon: 'check', heading: '1. ב-Submit', text: 'בלחיצה בודקים הכל, מציגים את השגיאה הראשונה, מחזירים פוקוס. פשוט; מתאים לטפסים קטנים וכלים פנימיים.' },
        { icon: 'file', heading: '2. ValidationRule ב-Binding', text: 'הבדיקה על ה-binding, לפני שהערך מגיע ל-source. טוב לפורמט/טווח של פקד בודד. הכלל חי ב-XAML.' },
        { icon: 'brain', heading: '3. INotifyDataErrorInfo', text: 'ה-VM/מודל מאמת בכל setter ומדווח. HasErrors ל-CanExecute, כמה שדות בו-זמנית, נבדק ב-unit test. המומלץ ל-MVVM.' },
      ],
      notes: 'IDataErrorInfo הוא הממשק הישן (string this[string]) — עדיין נתמך. Lab 2 משתמש ב-INotifyDataErrorInfo דרך ValidatableObject. תרגיל 11 — ValidationRule.',
    },
    {
      type: 'code', title: 'INotifyDataErrorInfo: מחלקת בסיס + setter מאמת', file: 'Mvvm/ValidatableObject.cs',
      code: `public abstract class ValidatableObject
    : ObservableObject, INotifyDataErrorInfo
{
    private readonly Dictionary<string, List<string>> _errors = new();
    public bool HasErrors => _errors.Count > 0;
    public event EventHandler<DataErrorsChangedEventArgs>?
        ErrorsChanged;
    public IEnumerable GetErrors(string? name) =>
        name != null && _errors.TryGetValue(name, out var l)
            ? l : Array.Empty<string>();

    protected void SetErrors(IEnumerable<string> errors,
                             [CallerMemberName] string? name = null)
    {
        var list = errors.ToList();
        if (list.Count == 0) _errors.Remove(name!);
        else _errors[name!] = list;
        ErrorsChanged?.Invoke(this,
            new DataErrorsChangedEventArgs(name));
        OnPropertyChanged(nameof(HasErrors));
    }
}`,
      bullets: [
        'מילון property → שגיאות',
        'ValidatesOnNotifyDataErrors = ברירת מחדל',
        'SaveCommand = new RelayCommand(Save, () => !HasErrors)',
        'ולידציה אסינכרונית: setter מפעיל בדיקה, SetErrors כשחוזרת',
        'UpdateSourceTrigger=PropertyChanged על ה-TextBox',
      ],
      notes: 'זה ה-TODO המרכזי ב-Lab 2. השאלה: מה ההבדל מ-ValidationRule? כאן ה-VM יודע שיש שגיאה — ולכן CanExecute יכול להשבית את Save.',
    },
    {
      type: 'code', title: 'איך זה נראה: Validation.ErrorTemplate', file: 'App.xaml',
      code: `<ControlTemplate x:Key="ErrorTemplate">
    <StackPanel>
        <Border BorderBrush="Red" BorderThickness="2">
            <!-- כאן יושב ה-TextBox המקורי -->
            <AdornedElementPlaceholder x:Name="Adorner" />
        </Border>
        <TextBlock Foreground="Red" FontSize="11"
          Text="{Binding ElementName=Adorner,
            Path=AdornedElement.(Validation.Errors)/ErrorContent}" />
    </StackPanel>
</ControlTemplate>
<Style TargetType="TextBox">
    <Setter Property="Validation.ErrorTemplate"
            Value="{StaticResource ErrorTemplate}" />
    <Style.Triggers>
        <Trigger Property="Validation.HasError" Value="True">
            <Setter Property="ToolTip"
                Value="{Binding RelativeSource={RelativeSource Self},
                        Path=(Validation.Errors)/ErrorContent}" />
        </Trigger>
    </Style.Triggers>
</Style>`,
      bullets: [
        'ברירת מחדל: מסגרת אדומה דקה',
        'Adorner layer — מעל הפקד; השאירו Margin תחתון',
        '/ = הפריט הנוכחי ברשימת השגיאות',
        'Trigger על HasError → ToolTip',
        'קלט מספרי: PreviewTextInput + TryParse (הדבקה עוקפת!)',
      ],
      notes: 'ה-Style הזה ב-App.xaml משרת את כל ה-TextBox-ים באפליקציה. הטאב הראשון בדמו מראה PreviewTextInput שחוסם אותיות — אבל Ctrl+V עדיין מכניס, לכן צריך גם TryParse.',
    },
    {
      type: 'bullets', title: 'דיאלוגים', icon: 'window',
      bullets: [
        { text: 'OpenFileDialog / SaveFileDialog (Microsoft.Win32)', sub: ['Filter="JSON (*.json)|*.json", ShowDialog(this) == true (bool?)', 'OpenFolderDialog מ-.NET 8'] },
        { text: 'דיאלוג מותאם = Window רגיל', sub: ['new NameDialog { Owner = this }.ShowDialog()', 'בפנים: DialogResult = true; כפתור IsCancel סוגר לבד', 'CenterOwner, NoResize, ShowInTaskbar=False'] },
        { text: 'אישור: MessageBox.Show(this, ..., YesNo, Warning, MessageBoxResult.No)', sub: ['ברירת מחדל "לא" — Enter בטעות לא ימחק'] },
      ],
      notes: 'הטאב Dialogs בדמו. Owner חשוב פעמיים: מרכוז וחסימה נכונה, ובלי Owner ה-MessageBox עלול להיפתח מאחורי החלון.',
    },
    {
      type: 'bullets', title: 'UI ידידותי למקלדת ונגיש', icon: 'users',
      bullets: [
        'סדר Tab הגיוני (TabIndex או סדר ה-XAML); פוקוס ראשוני בשדה הראשון',
        'Label עם Target ו-_ → Alt+אות מקפיץ לשדה',
        'IsDefault / IsCancel; InputBindings לפעולות תכופות',
        'ToolTip על כל כפתור-אייקון; AutomationProperties.Name לקורא-מסך',
        'לא רק צבע לשגיאה — גם אייקון/טקסט; כבדו גדלי גופן של המערכת',
        { text: 'הבדיקה המהירה: לעבור על הטופס כולו בלי עכבר', sub: ['רשימת בדיקה מלאה ל-UX של טופס ב-Notes/06'] },
      ],
      notes: 'תנו לסטודנטים דקה לעבור על Lab 1 שלהם בלי עכבר. מי שנתקע — יש לו באג נגישות. ההודעות בשפת המשתמש: "מה, איפה, מה לעשות".',
    },
    {
      type: 'lab', title: 'Lab 2 — Contacts Manager',
      goal: 'MVVM-lite מלא: רשימה + טופס, ולידציה עם INotifyDataErrorInfo, שמירה/טעינה מ-JSON ב-%AppData%.',
      duration: '75 דקות',
      deliverable: 'ניהול אנשי קשר: הוספה/עריכה/מחיקה עם Draft ו-Cancel, חיפוש, שגיאות ליד השדה, Save/Load, אישור בסגירה.',
      tasks: [
        'Contact: SetProperty + OnPropertyChanged(nameof(FullName))',
        'פקודות עם CanExecute: Delete/Cancel/Apply/Save',
        'ValidatableObject: GetErrors + SetErrors',
        'ולידציה: שם חובה, טלפון 9–15 ספרות, אימייל תקין',
        'ICollectionView: Filter לפי Search + SortDescription',
        'JsonContactsStore: LoadAsync/SaveAsync (יום 2!)',
        'DataGrid עם עמודות מפורשות + טופס פרטים מלא; בונוס: InputBindings',
      ],
      notes: 'המעבדה הארוכה של היום. חלקו לחלקים א–ה לפי ה-README ועצרו אחרי כל חלק לבדיקה משותפת. הנקודות הקשות: RelativeSource לפקודות מתוך הטופס, ו-Closing אסינכרוני.',
    },

    // ================= 07 =================
    { type: 'section', number: '07', title: 'פרוטוטייפינג מהיר', subtitle: 'מסקיצה ל-XAML, Styles, UserControls, Hot Reload, ספריות UI, WinForms — והצצה ליום 4',
      notes: 'מודול קצר ומעשי לפני Lab 4. הדמו: Day3.Demo.WinForms.' },
    {
      type: 'steps', title: 'מ-wireframe לחלון שעובד',
      steps: [
        { heading: '10 דק\' — סקיצה', text: 'נייר או ASCII. מסכים, מה בכל מסך, הפעולה העיקרית.' },
        { heading: '15 דק\' — שלד', text: 'dotnet new wpf; הפריסה בלבד עם Border-ים אפורים. תרגום: כותרת/סטטוס→DockPanel, טופס→Grid, כרטיסים→UniformGrid/WrapPanel, טבלה→DataGrid AutoGenerate.' },
        { heading: '20 דק\' — נתונים מזויפים', text: 'FakeService + ViewModel דק + binding. עכשיו יש מה להראות.' },
        { heading: '15 דק\' — ליטוש', text: 'Styles, צבע מודגש אחד, ריווח. עוצרים ב"נראה בסדר".' },
        { heading: 'הצגה ומשוב', text: 'לפני שממשיכים לבנות. מבנה קודם, צבעים אחר כך.' },
      ],
      notes: 'מי שמתחיל מהליטוש נשאר עם כפתור יפה ובלי אפליקציה. ה-wireframe של Lab 4 הוא ASCII ב-README.',
    },
    {
      type: 'code', title: 'Styles + ResourceDictionary = theme בשורה אחת', file: 'Themes/*.xaml + App.xaml.cs',
      code: `<!-- Themes/Light.xaml: רק צבעים -->
<SolidColorBrush x:Key="Accent" Color="#512BD4" />
<SolidColorBrush x:Key="CardBg" Color="#FFFFFF" />

<!-- Themes/Styles.xaml: סגנונות שמפנים לצבעים -->
<Style x:Key="Card" TargetType="Border">
    <Setter Property="Background"
            Value="{DynamicResource CardBg}" />
    <Setter Property="CornerRadius" Value="8" />
    <Setter Property="Padding" Value="14" />
</Style>

<!-- App.xaml -->
<ResourceDictionary.MergedDictionaries>
    <ResourceDictionary Source="Themes/Light.xaml" />   <!-- [0] -->
    <ResourceDictionary Source="Themes/Styles.xaml" />  <!-- [1] -->
</ResourceDictionary.MergedDictionaries>

// החלפת theme בזמן ריצה:
var dark = new Uri("Themes/Dark.xaml", UriKind.Relative);
Application.Current.Resources.MergedDictionaries[0] =
    new ResourceDictionary { Source = dark };`,
      bullets: [
        'Style = אוסף Setter-ים; BasedOn = ירושה',
        'Style בלי x:Key = implicit לכל הפקדים מהסוג',
        'ControlTemplate משנה מבנה (כפתור מעוגל)',
        'צבעים נפרד מסגנונות',
        'DynamicResource עוקב אחרי שינויים; Static — לא',
      ],
      notes: 'זה בדיוק מה ש-Lab 4 בונה (בונוס theme). ספריות כמו MaterialDesignInXAML עובדות באותה טכניקה. תרגיל 12 עושה גרסה מוקטנת עם Resources[...] על החלון.',
    },
    {
      type: 'code', title: 'UserControl לשימוש חוזר: DependencyProperty', file: 'Controls/StatCard.xaml.cs',
      code: `public partial class StatCard : UserControl
{
    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register(nameof(Value), typeof(string),
            typeof(StatCard), new PropertyMetadata("0"));

    public string Value
    {
        get => (string)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public StatCard() { InitializeComponent(); }
}

<!-- StatCard.xaml: x:Name="Root" על ה-UserControl -->
<TextBlock Text="{Binding Value, ElementName=Root}" FontSize="30" />

<!-- שימוש בחלון: -->
<c:StatCard Title="Orders" Value="{Binding Snapshot.OrdersToday}"
            Accent="{DynamicResource Accent}" />`,
      bullets: [
        'בלי DP — binding מבחוץ לא עובד (בשקט)',
        'ElementName=Root, לא DataContext',
        'snippet propdp ב-VS',
        'boilerplate = מועמד מושלם ל-AI (יום 4)',
      ],
      notes: 'הבאג הקלאסי: property רגילה ב-UserControl, Value="{Binding X}" ושום דבר לא קורה. הסבירו למה ElementName=Root: הפקד לא "גונב" את ה-DataContext של ההורה.',
    },
    {
      type: 'cards', title: 'עוד מאיצים',
      cards: [
        { icon: 'bolt', heading: 'XAML Hot Reload', text: 'F5, ערכו XAML, שמרו. עובד ל-layout, styles, templates. לא ל-code-behind.' },
        { icon: 'eye', heading: 'd:DataContext', text: 'd:DesignInstance Type=vm:DashboardViewModel — המעצב מציג נתונים אמיתיים. mc:Ignorable="d".' },
        { icon: 'cubes', heading: 'ספריות UI מוכנות', text: 'MaterialDesignInXAML, MahApps.Metro, WPF UI (Fluent). NuGet + ResourceDictionary ב-App.xaml. בדקו תיעוד עדכני.' },
        { icon: 'terminal', heading: 'Snippets & scaffolding', text: 'propdp, ctor; dotnet new wpf/wpfusercontrollib; תיקיית Mvvm/ להעתקה; או CommunityToolkit.Mvvm.' },
      ],
      notes: 'בקורס לא תלויים בספריות UI כדי שהכל ייבנה ללא רשת — אבל לפרוטוטייפ שצריך להרשים זה קיצור דרך אדיר. הראו screenshot מהאתר של אחת מהן אם יש אינטרנט.',
    },
    {
      type: 'code', title: 'WinForms: המעצב הגרפי לכלי חד-פעמי', file: 'Day3.Demo.WinForms/MainForm.cs',
      code: `public class MainForm : Form
{
    readonly TextBox _nameBox = new() { Dock = DockStyle.Fill };
    readonly Button _addButton = new() { Text = "Add" };
    readonly ListBox _list = new() { Dock = DockStyle.Fill };

    public MainForm()
    {
        var layout = new TableLayoutPanel
        { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 2 };
        layout.Controls.Add(_nameBox, 0, 0);
        layout.Controls.Add(_addButton, 1, 0);
        layout.Controls.Add(_list, 0, 1);
        layout.SetColumnSpan(_list, 2);
        Controls.Add(layout);
        AcceptButton = _addButton;   // Enter

        _addButton.Click += (_, _) =>
        { _list.Items.Add(_nameBox.Text); _nameBox.Clear(); };
    }
}`,
      bullets: [
        'פקדים = אובייקטים, פריסה = properties',
        'המעצב מייצר בדיוק קוד כזה (Designer.cs)',
        'אין XAML, binding, MVVM — ומהר מאוד',
        'מתי: כלי פנימי, throwaway, מערכת קיימת',
        'מתי לא: UI מותאם, יחיה שנים',
      ],
      notes: 'הדמו בנוי בקוד בלי Designer כדי להראות את המודל. ב-VS: גוררים, לוחצים פעמיים על הכפתור, כותבים. TableLayoutPanel = ה-Grid של WinForms. תנו למרצה לבחור אם להרחיב.',
    },
    {
      type: 'quote',
      text: 'כל ה-boilerplate שראינו היום — DependencyProperty, Styles, ViewModel עם עשרים properties — הוא טקסט מובנה וחזרתי. מחר נראה איך כלי AI כותבים אותו בשניות, ואיך בודקים שהם צדקו.',
      author: 'הצצה ליום 4',
      notes: 'הטיזר ליום 4. הדגישו: שגיאת binding לא מפילה את התוכנית — לכן דווקא עם AI חשוב להבין מה נכון. כל מה שלמדנו היום הוא הבסיס לבדיקת קוד שנוצר.',
    },
    {
      type: 'lab', title: 'Lab 3 — Products API Client',
      goal: 'לקוח WPF מעל REST API: טעינה אסינכרונית עם התקדמות וביטול, שגיאות inline, סינון, פריסה רספונסיבית.',
      duration: '60 דקות',
      deliverable: 'קטלוג מוצרים שעובד עם FakeProductService (offline) ועם HttpProductService מול Day2.LocalApi.',
      tasks: [
        'LoadAsync: CTS + IsBusy + Progress<int> + Status; ProgressBar ושכבת "טוען…"',
        'שגיאות: OperationCanceled → "Cancelled"; HttpRequestException → באנר + Retry',
        'ICollectionView.Filter: חיפוש, קטגוריה, In stock only',
        'HttpProductService עם HttpClient + JSON (קוד מיום 2)',
        'תצוגת כרטיסים (ItemsControl + WrapPanel) מתחת ל-700px',
        'WrapPanel לסרגל העליון; בונוס: F5 / Escape',
      ],
      notes: 'המעבדה עובדת בלי שרת בזכות ה-Fake. מי שהריץ את Day2.LocalApi יראה נתונים אמיתיים. ה-Fake ה"נכשל" ב-ComboBox מדגים Retry.',
    },
    {
      type: 'lab', title: 'Lab 4 — Rapid Dashboard',
      goal: 'מ-wireframe (ASCII) ל-dashboard חי ב-45 דקות: Styles, UserControl, TabControl, DispatcherTimer.',
      duration: '60 דקות (45 + 15 בונוס)',
      deliverable: 'Ops Dashboard עם 4 StatCard-ים חיים, גרף עמודות, טאב הזמנות ו-Settings; בונוס: theme בהיר/כהה.',
      tasks: [
        'Styles: H1, Muted, AccentButton (ControlTemplate קצר)',
        'StatCard: DependencyProperty ל-Value, Subtitle, Accent',
        'כותרת + DispatcherTimer שקורא ל-vm.Tick()',
        'UniformGrid עם 4 כרטיסים קשורים ל-Snapshot.*',
        'ItemsControl + Rectangle = גרף; DataGrid להזמנות',
        'בונוס: Dark.xaml + App.ApplyTheme + Slider ל-RefreshSeconds',
      ],
      notes: 'עבדו עם Hot Reload פתוח. הסוד: המבנה (Border-ים אפורים) קודם, ליטוש אחר כך. שאלה לדיון בסוף: מה היה קורה עם System.Timers.Timer במקום DispatcherTimer?',
    },
    {
      type: 'bullets', title: 'הטעויות הנפוצות של היום', icon: 'warning',
      bullets: [
        'x:Class לא תואם / שכחת InitializeComponent → חלון ריק',
        'StackPanel כ-root, Width קבועים → לא רספונסיבי',
        '.Result / Thread.Sleep על ה-UI thread → קפוא או deadlock',
        'List<T> במקום ObservableCollection; property בלי OnPropertyChanged',
        'שכחת DataContext / שגיאת כתיב ב-Path → ריק בשקט (Output window!)',
        'נגיעה בפקד מ-Task.Run בלי Dispatcher → InvalidOperationException',
        'MessageBox לכל שגיאה; ולידציה רק ב-Submit; StaticResource לצבעי theme',
      ],
      notes: 'עברו מהר — כל אחת מופיעה בהרחבה ב"טעויות נפוצות" של המודול המתאים. בקשו מהכיתה להוסיף טעות שהם עשו היום.',
    },
    {
      type: 'end', title: 'סיכום יום 3',
      bullets: [
        'WPF = XAML + binding + MVVM; WinForms לכלים מהירים',
        'פריסה ב-panels, star sizing, ולעולם לא לחסום את ה-UI thread',
        'אירועים → Commands; INotifyPropertyChanged + ObservableCollection',
        'services מאחורי ממשק; IsBusy + Progress + Cancel; שגיאות ידידותיות',
        'ולידציה ב-VM; דיאלוגים; מקלדת ונגישות',
        'פרוטוטייפ: מבנה קודם, Styles, UserControls, Hot Reload',
        'מחר: כלי AI לפיתוח — עם כל מה שלמדנו כבסיס לבדיקה',
      ],
      footer: 'שיעורי בית: Lab 3 או Lab 4 (מה שלא הספקתם) + תרגילים 5, 8, 11',
      notes: 'סגרו עם החזרה למסר הפותח: ה-UI הוא קליפה מעל הקוד של ימים 1–2. הזכירו את Exercises/README.md ואת NOTES.md בכל Solution. שאלות פתוחות — לפורום הקורס.',
    },
  ],
};
