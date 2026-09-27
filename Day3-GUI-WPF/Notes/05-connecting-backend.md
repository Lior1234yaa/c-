# מודול 05 — חיבור ה-GUI ללוגיקה ול-backend

## העיקרון: ה-UI הוא קליפה

הקוד שכתבתם ביום 1 (מחלקות, LINQ, חריגות) וביום 2 (HttpClient, JSON) לא צריך לדעת שהוא רץ מתחת
לחלון. אם הוא כתוב נכון — ב**מחלקות שירות** רגילות — אפשר לחבר אותו ל-WPF, ל-WinForms, ל-Web API
או לבדיקות, בלי לשנות אותו. ה-UI רק קורא לו ומציג את התוצאה.

```text
MainWindow / ViewModel   ──►  IProductService  ◄──  HttpProductService   (HttpClient + JSON, יום 2)
                                                ◄──  FakeProductService   (in-memory, לפיתוח ובדיקות)
```

## שירותים מאחורי ממשק

```csharp
public interface IProductService
{
    Task<IReadOnlyList<Product>> GetProductsAsync(IProgress<int>? progress, CancellationToken ct);
}
```

שלוש סיבות לממשק:

1. **פיתוח בלי שרת** — `FakeProductService` מחזיר נתונים מהזיכרון (עם `Task.Delay` כדי לדמות רשת).
   ה-UI מתפתח במקביל ל-API, וגם עובד בכיתה בלי אינטרנט.
2. **בדיקות** — ה-ViewModel נבדק עם fake דטרמיניסטי.
3. **החלפה** — מחר ה-API עובר ל-gRPC? מחליפים מימוש אחד.

### DI-lite: הזרקה דרך הבנאי

בלי container, בלי קסמים — פשוט מעבירים את התלות בבנאי:

```csharp
public class ProductsViewModel(IProductService service) : ObservableObject { ... }

// ב-MainWindow (composition root):
DataContext = new ProductsViewModel(new HttpProductService(Http));
```

`HttpClient` אחד לכל האפליקציה (`static readonly`) — כפי שלמדנו ביום 2. בפרויקט גדול עוברים ל-
`Microsoft.Extensions.DependencyInjection` — גם ב-WPF זה עובד יפה (`Host.CreateDefaultBuilder` ב-`App.xaml.cs`).

## לקרוא ל-service אסינכרוני מה-UI

### async event handler

```csharp
private async void Load_Click(object sender, RoutedEventArgs e)
{
    IsBusy = true;
    try
    {
        var items = await _service.GetProductsAsync(progress, ct);
        Products.Clear();
        foreach (var p in items) Products.Add(p);
    }
    catch (HttpRequestException ex) { ShowError(ex); }
    finally { IsBusy = false; }
}
```

- ה-`await` משחרר את ה-UI thread; החלון ממשיך להגיב.
- אחרי ה-`await` אנחנו **חוזרים ל-UI thread** — מותר לגעת בפקדים וב-`ObservableCollection`.
- `try/catch` חובה: חריגה מ-`async void` שלא נתפסה מפילה את התהליך.

### async command (ב-ViewModel)

`ICommand.Execute` הוא `void`, ולכן command אסינכרוני נראה כך:

```csharp
public sealed class AsyncRelayCommand(Func<Task> execute, Func<bool>? canExecute = null) : ICommand
{
    private bool _running;
    public bool CanExecute(object? p) => !_running && (canExecute?.Invoke() ?? true);

    public async void Execute(object? p)
    {
        _running = true; CommandManager.InvalidateRequerySuggested();
        try { await execute(); }
        finally { _running = false; CommandManager.InvalidateRequerySuggested(); }
    }
    public event EventHandler? CanExecuteChanged
    { add => CommandManager.RequerySuggested += value; remove => CommandManager.RequerySuggested -= value; }
}
```

הדגל `_running` מונע לחיצה כפולה — הכפתור מושבת עד שהפעולה מסתיימת.

### IsBusy, ProgressBar, Cancel — השלישייה

כל פעולה ארוכה צריכה שלושה דברים: אינדיקציה שמשהו קורה, התקדמות אם אפשר, ודרך לבטל.

```csharp
private CancellationTokenSource? _cts;

public async Task LoadAsync()
{
    _cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));   // גם timeout
    IsBusy = true; Progress = 0;
    var progress = new Progress<int>(p => Progress = p);   // נוצר על ה-UI thread → ה-callback רץ עליו
    try
    {
        var items = await _service.GetProductsAsync(progress, _cts.Token);
        ...
    }
    catch (OperationCanceledException) { Status = "Cancelled"; }
    finally { IsBusy = false; _cts.Dispose(); _cts = null; }
}

public void Cancel() => _cts?.Cancel();
```

```xml
<ProgressBar Value="{Binding Progress}" Visibility="{Binding IsBusy, Converter={StaticResource BoolToVis}}" />
<Button Content="Cancel" Command="{Binding CancelCommand}" />
```

ה-service מכבד את הביטול כי הוא מעביר את ה-token הלאה: `http.GetAsync(url, ct)`, `Task.Delay(ms, ct)`.
`IProgress<T>` הוא הדרך הנכונה לדווח התקדמות מקוד שלא מכיר UI.

## לשלב את קוד יום 2

`HttpProductService` הוא בדיוק מה שכתבתם אתמול:

```csharp
public class HttpProductService(HttpClient http, string url) : IProductService
{
    private static readonly JsonSerializerOptions Json = new() { PropertyNameCaseInsensitive = true };

    public async Task<IReadOnlyList<Product>> GetProductsAsync(IProgress<int>? progress, CancellationToken ct)
    {
        progress?.Report(10);
        using var response = await http.GetAsync(url, ct);
        response.EnsureSuccessStatusCode();
        progress?.Report(50);
        var dtos = await response.Content.ReadFromJsonAsync<List<ProductDto>>(Json, ct) ?? [];
        progress?.Report(100);
        return dtos.Select(d => d.ToProduct()).ToList();
    }
}
```

שימו לב ל-**DTO**: המחלקה שמתאימה ל-JSON של ה-API (`ProductDto`) נפרדת מהמודל של ה-UI (`Product`).
כשה-API משנה שדה, משנים את ה-DTO וההמרה — לא את כל ה-XAML.

## Dispatcher: כשאתם לא על ה-UI thread

אחרי `await` אתם על ה-UI thread. אבל יש מקרים שלא:

- `Task.Run(...)` לחישוב כבד.
- callback של ספרייה (SignalR, serial port, file watcher).
- `System.Timers.Timer`.

שם צריך `Dispatcher`:

```csharp
Task.Run(() =>
{
    var report = BuildHeavyReport();             // רץ ב-thread pool
    Dispatcher.Invoke(() => ReportText.Text = report);        // סינכרוני
    // או: Application.Current.Dispatcher.InvokeAsync(...)   // מכל מקום, אסינכרוני
});
```

הדרך הפשוטה ביותר להימנע מזה: `var report = await Task.Run(BuildHeavyReport);` — ואז אתם כבר חזרה ב-UI thread.

## טיפול בשגיאות: MessageBox או inline?

| | MessageBox | הודעה inline (Border/TextBlock) |
|--|-----------|--------------------------------|
| חוסם את המשתמש | כן | לא |
| מתאים ל... | אישורים, שגיאה קטלנית, החלטה שחייבים | שגיאות רשת, ולידציה, "נסה שוב" |
| מאפשר Retry | לא באמת | כן (כפתור בבאנר) |
| נשאר על המסך להקשר | לא | כן |

הודעה ידידותית = מה קרה + מה לעשות. לא `ex.ToString()`:

```csharp
catch (HttpRequestException ex)
{
    Error = $"לא ניתן להתחבר לשרת ({ex.StatusCode?.ToString() ?? "network"}). בדקו שהשרת רץ ונסו שוב.";
    _logger.LogError(ex, "Load failed");   // הפרטים הטכניים — ללוג, לא למשתמש
}
```

```xml
<Border Background="#FFE6E6" Visibility="{Binding HasError, Converter={StaticResource BoolToVis}}">
    <DockPanel>
        <Button DockPanel.Dock="Right" Content="Retry" Command="{Binding LoadCommand}" />
        <TextBlock Text="{Binding Error}" TextWrapping="Wrap" />
    </DockPanel>
</Border>
```

תפסו חריגות **ספציפיות** (`HttpRequestException`, `JsonException`, `IOException`, `OperationCanceledException`)
ורק בסוף `Exception` כללי לדברים לא צפויים. `OperationCanceledException` הוא לא שגיאה — המשתמש ביקש.

## שמירה מקומית: JSON ב-%AppData%

הגדרות, cache, "הקובץ האחרון" — נשמרים בתיקיית המשתמש, לא ליד ה-exe (שם אין הרשאות כתיבה ב-Program Files):

```csharp
public static class SettingsStore
{
    private static readonly string Dir = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MyApp");
    private static readonly string FilePath = Path.Combine(Dir, "settings.json");

    public static AppSettings Load()
    {
        try
        {
            return File.Exists(FilePath)
                ? JsonSerializer.Deserialize<AppSettings>(File.ReadAllText(FilePath)) ?? new()
                : new();
        }
        catch (Exception ex) when (ex is IOException or JsonException) { return new(); }   // קובץ פגום ≠ קריסה
    }

    public static void Save(AppSettings s)
    {
        Directory.CreateDirectory(Dir);
        File.WriteAllText(FilePath, JsonSerializer.Serialize(s, new JsonSerializerOptions { WriteIndented = true }));
    }
}
```

`SpecialFolder.ApplicationData` = `%AppData%` (roaming), ‏`LocalApplicationData` = `%LocalAppData%` (מקומי, טוב ל-cache גדול).
טוענים ב-`Loaded`/בבנאי, שומרים ב-`Closing`. ראו `Demos/Day3.Demo.AsyncUi` ו-Lab 2.

## מבנה תיקיות מומלץ

כשהפרויקט גדל, המבנה הזה שומר על סדר וגם מכין אתכם ל-MAUI/WinUI שמשתמשים באותה חלוקה:

```text
MyApp/
  Models/          Product.cs, Contact.cs           ← נתונים בלבד
  Services/        IProductService.cs, HttpProductService.cs, FakeProductService.cs
  ViewModels/      ProductsViewModel.cs             ← מצב + פקודות, בלי WPF
  Views/           MainWindow.xaml, ProductDialog.xaml
  Controls/        StatCard.xaml                    ← UserControls
  Converters/      BoolToBrushConverter.cs
  Mvvm/            ObservableObject.cs, RelayCommand.cs
  Themes/          Light.xaml, Dark.xaml, Styles.xaml
  App.xaml         ← composition root + משאבים
```

בפרויקטים גדולים מוציאים את `Models` ו-`Services` ל-class library נפרד (`MyApp.Core`, ‏`net10.0` רגיל
ללא WPF) — כך אותו קוד משרת גם את ה-API ואת בדיקות היחידה, ואי אפשר "בטעות" להכניס לשם `MessageBox`.

## לבדוק את ה-ViewModel בלי חלון

זה הרווח הגדול של ההפרדה. בדיקת xUnit ל-`ProductsViewModel` עם fake:

```csharp
[Fact]
public async Task LoadAsync_fills_products_and_clears_busy()
{
    var vm = new ProductsViewModel([new FakeProductService { DelayPerBatchMs = 0 }]);

    await vm.LoadAsync();

    Assert.False(vm.IsBusy);
    Assert.Equal(60, vm.Products.Count);
    Assert.Null(vm.Error);
}
```

אין כאן `Window`, אין `Dispatcher`, אין STA thread. שימו לב שה-fake מקבל `DelayPerBatchMs = 0` — בדיקות
צריכות לרוץ מהר. אם ה-VM שלכם לא ניתן לבדיקה ככה, כנראה שנכנס לתוכו משהו שצריך להיות ב-View.

## לוגים: הפרטים למפתח, ההודעה למשתמש

המשתמש רואה "לא ניתן להתחבר לשרת"; אתם צריכים לראות את ה-URL, קוד הסטטוס וה-stack trace. הפרידו:
הודעה ידידותית ל-UI, והחריגה המלאה ללוג (`Microsoft.Extensions.Logging` עם provider לקובץ, או בפרוטוטייפ
פשוט `Debug.WriteLine` / `Trace.TraceError`). כלל טוב: כל `catch` שמציג הודעה למשתמש גם כותב ללוג.

## מחזור חיים של חיבור: מתי טוענים, מתי מרעננים

שאלה שחוזרת בכל אפליקציה: מתי לקרוא ל-service? ההמלצות:

- **בפתיחה** — ב-`Loaded` (לא בבנאי), עם `IsBusy` דלוק, כדי שהחלון יופיע מיד ורק אז יטען.
- **ביוזמת המשתמש** — כפתור Refresh/F5. תמיד תנו דרך ידנית, גם אם יש רענון אוטומטי.
- **רענון תקופתי** — `DispatcherTimer` שמפעיל את אותה פקודה. שמרו על מרווח סביר (5–30 שניות
  ל-dashboard), ודלגו על tick אם הקודם עדיין רץ (`if (IsBusy) return;`).
- **בשינוי סינון** — עדיף לסנן בזיכרון (`ICollectionView`) ולא לפנות לשרת על כל תו; אם חייבים
  שרת — debounce (מודול 03).
- **בסגירה** — ביטול פעולות שרצות (`_cts?.Cancel()`) ושמירת הגדרות.

## מה עם offline?

אפליקציית שולחן עבודה חיה גם בלי רשת. שלוש רמות של התמודדות, לפי הצורך: (1) הודעה ברורה +
Retry — המינימום; (2) **cache** של הקריאה האחרונה ב-`%LocalAppData%` כ-JSON, ומציגים אותו עם
תווית "נתונים מ-14:02" עד שהרשת חוזרת; (3) תור פעולות לסנכרון מאוחר — כבר ארכיטקטורה לעצמה.
ל-Lab 3 מספיקה רמה 1; רמה 2 היא בונוס טבעי: `SettingsStore` מהדוגמה למעלה עובד בדיוק אותו דבר
על רשימת מוצרים.

## טעויות נפוצות

- `.Result` / `.Wait()` / `GetAwaiter().GetResult()` על ה-UI thread → deadlock. גם ב-`Closing`! (הפתרון: `e.Cancel = true; await ...; Close();`).
- `async void` בלי try/catch → קריסה שקטה של כל האפליקציה.
- לשכוח `finally { IsBusy = false; }` → אחרי שגיאה החלון נשאר "טוען".
- ליצור `Progress<T>` בתוך `Task.Run` → ה-callback רץ ב-thread הלא נכון.
- `new HttpClient()` בכל קריאה → מיצוי sockets (יום 2).
- להציג `ex.Message` גולמי או stack trace למשתמש.
- לכתוב קבצים ליד ה-exe.

## לסיכום

- הלוגיקה ב-services מאחורי ממשקים; ה-UI מקבל אותם בבנאי. Fake לפיתוח, Http לייצור.
- `async`/`await` ב-handlers ו-commands; `IsBusy` + `ProgressBar` + `CancellationToken` לכל פעולה ארוכה.
- אחרי `await` — UI thread. מ-`Task.Run`/callbacks — `Dispatcher.Invoke`.
- שגיאות: ספציפיות, ידידותיות, inline עם Retry. MessageBox לאישורים.
- הגדרות ו-cache: JSON ב-`%AppData%`.

## קריאה נוספת

- [Asynchronous programming](https://learn.microsoft.com/dotnet/csharp/asynchronous-programming/)
- [Dispatcher class](https://learn.microsoft.com/dotnet/api/system.windows.threading.dispatcher)
- [Progress<T>](https://learn.microsoft.com/dotnet/api/system.progress-1)
- [Cancellation in managed threads](https://learn.microsoft.com/dotnet/standard/threading/cancellation-in-managed-threads)
- [Dependency injection in .NET](https://learn.microsoft.com/dotnet/core/extensions/dependency-injection)
- [Environment.SpecialFolder](https://learn.microsoft.com/dotnet/api/system.environment.specialfolder)
