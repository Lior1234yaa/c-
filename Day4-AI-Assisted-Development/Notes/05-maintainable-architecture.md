# מודול 5 — ארכיטקטורה קריאה, ניתנת להרחבה ולתחזוקה

## למה דווקא היום

כשקוד נכתב מהר (ו-AI כותב מהר מאוד), הארכיטקטורה היא מה שמונע מהפרויקט להפוך לערימה. AI יוסיף בשמחה `HttpClient` בתוך ViewModel, גישה לקובץ בתוך מחלקת דומיין, ו-`static` בכל מקום — אלא אם המבנה שלכם, קובצי ההוראות והבדיקות אומרים אחרת. במודול הזה נבנה את "השלד" שקוד AI צריך להשתלב בו.

## ארכיטקטורת שכבות ל-.NET (clean-ish)

```text
┌────────────────────────────┐
│ UI (WPF / Console / Web)   │  Views, ViewModels — "איך זה נראה"
├────────────────────────────┤
│ Application                │  Services, use-cases, interfaces, DTOs — "מה המערכת עושה"
├────────────────────────────┤
│ Domain                     │  Entities, value objects, rules — "מה נכון עסקית" (ללא תלויות)
├────────────────────────────┤
│ Infrastructure             │  JSON/DB repositories, HTTP clients, file system, clock
└────────────────────────────┘
```

כלל התלות: החיצים מצביעים **פנימה**. Domain לא מכיר אף אחד. Application מכיר Domain ומגדיר ממשקים (`IOrderRepository`). Infrastructure **מממש** את הממשקים. UI מחבר הכול דרך DI.

בפרויקט קטן אפשר להשאיר הכול ב-project אחד עם **תיקיות** לפי השכבות — מה שחשוב הוא הכיוון של התלויות, לא מספר ה-csproj.

## Dependency Injection עם Microsoft.Extensions

DI = במקום שמחלקה תיצור את התלויות שלה (`new JsonOrderRepository()`), היא **מקבלת** אותן בבנאי דרך ממשק. יתרונות: אפשר להחליף מימוש (קובץ ↔ DB ↔ fake לבדיקות), והתלויות גלויות.

```csharp
public interface IOrderRepository
{
    Task<IReadOnlyList<Order>> GetAllAsync(CancellationToken ct = default);
    Task SaveAsync(Order order, CancellationToken ct = default);
}

public sealed class OrderService(IOrderRepository repo, IClock clock, ILogger<OrderService> logger)
{
    public async Task<Order> CreateAsync(NewOrderRequest request, CancellationToken ct = default)
    {
        var order = Order.Create(request.CustomerId, request.Lines, clock.UtcNow);
        await repo.SaveAsync(order, ct);
        logger.LogInformation("Order {OrderId} created", order.Id);
        return order;
    }
}
```

(שימו לב ל-primary constructor של C# 12+ — הפרמטרים זמינים בכל המחלקה.)

### Hosting — גם ב-WPF

`Microsoft.Extensions.Hosting` נותן לנו container, configuration ו-logging במקום אחד. ב-WPF מקימים host ב-`App.xaml.cs`:

```csharp
public partial class App : Application
{
    private readonly IHost _host = Host.CreateDefaultBuilder()
        .ConfigureServices((ctx, services) =>
        {
            services.Configure<AppOptions>(ctx.Configuration.GetSection("App"));
            services.AddSingleton<IClock, SystemClock>();
            services.AddSingleton<IOrderRepository, JsonOrderRepository>();
            services.AddTransient<OrderService>();
            services.AddTransient<MainViewModel>();
            services.AddTransient<MainWindow>();
        })
        .Build();

    protected override async void OnStartup(StartupEventArgs e)
    {
        await _host.StartAsync();
        _host.Services.GetRequiredService<MainWindow>().Show();
        base.OnStartup(e);
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        await _host.StopAsync();
        _host.Dispose();
        base.OnExit(e);
    }
}
```

ב-`App.xaml` מסירים את `StartupUri` (אחרת ייפתחו שני חלונות). `CreateDefaultBuilder` קורא אוטומטית `appsettings.json` (יש להעתיק אותו ל-output: `<Content Include="appsettings.json" CopyToOutputDirectory="PreserveNewest" />`) ומגדיר logging לקונסול/Debug. הדגמה מלאה: `Demos/Day4.Demo.DiHostWpf`.

### Lifetimes
- **Singleton** — מופע אחד לכל האפליקציה (repository עם cache, clock, options).
- **Transient** — חדש בכל בקשה (ViewModels, services קלים).
- **Scoped** — ב-WPF פחות שימושי (אין "request"); רלוונטי ל-ASP.NET Core.

## Configuration: appsettings.json + Options pattern

```json
{
  "App": { "DataFile": "data/orders.json", "Currency": "ILS", "MaxItemsPerOrder": 50 },
  "Logging": { "LogLevel": { "Default": "Information" } }
}
```

```csharp
public sealed class AppOptions
{
    public string DataFile { get; set; } = "orders.json";
    public string Currency { get; set; } = "ILS";
    public int MaxItemsPerOrder { get; set; } = 100;
}

// צריכה:
public sealed class JsonOrderRepository(IOptions<AppOptions> options) : IOrderRepository
{
    private readonly string _file = options.Value.DataFile;
}
```

יתרון: אין קבועים קסומים בקוד, אפשר קובץ `appsettings.Development.json`, ומשתני סביבה דורסים (חשוב לסודות — ראו מודול 6).

## Logging עם ILogger

```csharp
logger.LogInformation("Loaded {Count} orders from {File}", orders.Count, _file);
logger.LogWarning("Order {OrderId} exceeds max items ({Count})", id, count);
logger.LogError(ex, "Failed to save {File}", _file);
```

- **Structured logging**: placeholders בשם, לא string interpolation — מאפשר חיפוש לפי שדה.
- רמות: Trace/Debug/Information/Warning/Error/Critical. אל תרשמו נתונים אישיים או סודות.

## ממשקים ובדיקתיות

כל דבר שמדבר עם העולם (זמן, קבצים, רשת, DB, GUI) מאחורי ממשק. כך הבדיקה מזריקה fake:

```csharp
sealed class FakeClock(DateTimeOffset now) : IClock { public DateTimeOffset UtcNow => now; }

[Fact]
public async Task CreateAsync_SetsCreatedAtFromClock()
{
    var repo = new InMemoryOrderRepository();
    var svc = new OrderService(repo, new FakeClock(new(2025, 1, 1, 0, 0, 0, TimeSpan.Zero)), NullLogger<OrderService>.Instance);
    var order = await svc.CreateAsync(new(7, [new("A-1", 2, 10m)]));
    Assert.Equal(2025, order.CreatedAt.Year);
}
```

(.NET מציע גם `TimeProvider` מובנה — `TimeProvider.System` ו-`FakeTimeProvider` בחבילת `Microsoft.Extensions.TimeProvider.Testing`.)

## מבנה תיקיות ומוסכמות שמות

```text
src/
  Orders.Domain/        Order.cs, OrderLine.cs, Money.cs, Rules/
  Orders.Application/   Services/, Interfaces/, Dtos/
  Orders.Infrastructure/ Json/, Http/, Time/
  Orders.Wpf/           Views/, ViewModels/, Themes/, App.xaml(.cs), appsettings.json
tests/
  Orders.Tests/         (מבנה מקביל: Services/OrderServiceTests.cs)
docs/
  README.md, adr/0001-use-system-text-json.md
```

- שמות: `PascalCase` לטיפוסים ומתודות, `_camelCase` לשדות פרטיים, `I` לממשקים, `Async` למתודות אסינכרוניות, שמות בדיקות `Method_Scenario_Expected`.
- **קבצים קטנים**: מחלקה אחת לקובץ, מתודות עד ~30 שורות. קובץ של 800 שורות הוא סימן אזהרה — גם עבור AI, שיתקשה לערוך אותו נכון.
- file-scoped namespaces, `nullable enable`, `ImplicitUsings`.

## SOLID בקצרה — עם C#

| עיקרון | משמעות | דוגמת הפרה → תיקון |
|--------|---------|----------------------|
| **S**ingle Responsibility | מחלקה = סיבה אחת להשתנות | `OrderProcessor` שמחשב, שומר ומדפיס → `DiscountCalculator`, `OrderRepository`, `ReceiptPrinter` |
| **O**pen/Closed | פתוח להרחבה, סגור לשינוי | `switch` על סוג הנחה → `IDiscountRule` + רשימת כללים |
| **L**iskov | תת-טיפוס לא שובר ציפיות | `ReadOnlyRepository : IRepository` שזורק ב-`Save` → ממשק נפרד `IReadRepository` |
| **I**nterface Segregation | ממשקים קטנים | `IRepository` עם 15 מתודות → `IReadRepository`, `IWriteRepository` |
| **D**ependency Inversion | תלות בהפשטה | `new SmtpClient()` בתוך service → `IEmailSender` מוזרק |

דוגמת Open/Closed:

```csharp
public interface IDiscountRule { decimal Apply(Order order, decimal current); }

public sealed class DiscountEngine(IEnumerable<IDiscountRule> rules)
{
    public decimal Calculate(Order order)
        => rules.Aggregate(order.Subtotal, (total, rule) => rule.Apply(order, total));
}
// כלל חדש = מחלקה חדשה + שורת רישום ב-DI. אפס שינוי ב-DiscountEngine.
```

## חוב טכני וקצב refactoring

- **חוב טכני** = קיצורי דרך שחוסכים היום ועולים מחר. עם AI קל לצבור אותו מהר.
- כלל "הצופה": כל פעם שנוגעים בקובץ, משאירים אותו קצת יותר נקי.
- הקדישו זמן קבוע (למשל 10–15% מהספרינט) ל-refactoring; רשמו חובות ידועים כ-issues.
- AI מצוין בזיהוי: "List code smells in this file and rank by risk."

## תיעוד: README ו-ADRs

- **README** בפרויקט: מה זה, איך בונים/מריצים/בודקים, מבנה התיקיות, החלטות עיקריות.
- **ADR (Architecture Decision Record)**: קובץ קצר לכל החלטה משמעותית — הקשר, החלטה, חלופות, השלכות. דוגמה: `docs/adr/0002-wpf-di-hosting.md`. ADRs עוזרים לבני אדם *ול-AI* להבין למה הדברים כמו שהם.

## לשמור על עקביות קוד AI עם הארכיטקטורה

1. **קובצי הוראות** (`CLAUDE.md`, `copilot-instructions.md`) מתארים את השכבות והכללים (מודול 2).
2. **תבניות**: קובץ `templates/ServiceTemplate.cs` או דוגמה מלאה אחת שה-AI "מחקה". הפניה בבקשה: "Follow the pattern in `Services/CustomerService.cs`."
3. **Analyzers + `.editorconfig`** אוכפים סגנון אוטומטית.
4. **בדיקות ארכיטקטורה**: אפשר לבדוק בבדיקת יחידה שה-Domain לא מפנה ל-Infrastructure (Reflection על assemblies או ספריות ייעודיות).
5. **Review** לפי הצ'ק-ליסט של מודול 4 + שאלה: "האם זה בשכבה הנכונה?"

## טעויות נפוצות

- Service Locator (`_host.Services.GetService<T>()` בכל מקום) במקום הזרקה בבנאי.
- Singleton שמחזיק state של משתמש/מסך.
- `IOptions<T>` שמועבר עמוק לתוך ה-Domain — הדומיין לא צריך להכיר configuration.
- ממשק לכל מחלקה "כי ככה עושים" — ממשק שווה כשיש מימוש שני (אמיתי או fake).
- README שלא מתעדכן — עדיף קצר ונכון.

## לסיכום

- שכבות עם תלויות פנימה: UI → Application → Domain ← Infrastructure.
- DI + Hosting + Options + ILogger — גם ב-WPF, דרך `App.xaml.cs`.
- ממשקים למה שמדבר עם העולם; בדיקות עם fakes.
- SOLID, קבצים קטנים, שמות ברורים, README + ADRs.
- קוד AI נשאר עקבי בזכות קובצי הוראות, תבניות, analyzers ו-review.

## קריאה נוספת

- Dependency injection in .NET: https://learn.microsoft.com/dotnet/core/extensions/dependency-injection
- .NET Generic Host: https://learn.microsoft.com/dotnet/core/extensions/generic-host
- Options pattern: https://learn.microsoft.com/dotnet/core/extensions/options
- Logging in .NET: https://learn.microsoft.com/dotnet/core/extensions/logging
- Architecture guides (Microsoft): https://learn.microsoft.com/dotnet/architecture/
- TimeProvider: https://learn.microsoft.com/dotnet/api/system.timeprovider
