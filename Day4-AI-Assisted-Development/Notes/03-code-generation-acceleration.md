# מודול 3 — האצת כתיבת קוד עם AI

## הרעיון

רוב זמן הפיתוח היומיומי לא הולך על אלגוריתמים מבריקים אלא על "דבק": DTOs, מיפויים, מחלקות עם `INotifyPropertyChanged`, בדיקות, תיעוד, הודעות commit. זה בדיוק המקום שבו AI חוסך הכי הרבה זמן — בתנאי שאתם יודעים מה לבקש ומה לבדוק אחרי. במודול הזה נעבור על משימות שגרתיות, עם **הבקשה, תוצאה ריאליסטית, ומה היה צריך לתקן**.

## 1. מחלקות/DTOs מ-JSON

**הבקשה:**

```text
Generate C# records (System.Text.Json, .NET 10, nullable enabled) for this API response.
Use PascalCase properties with [JsonPropertyName] where needed. Dates as DateTimeOffset.
{ "order_id": 1023, "customer": { "id": 7, "name": "Dana", "email": null },
  "items": [ { "sku": "A-1", "qty": 2, "unit_price": 19.9 } ],
  "created_at": "2025-03-01T10:15:00Z", "status": "paid" }
```

**תוצאה טיפוסית:**

```csharp
public record OrderDto(
    [property: JsonPropertyName("order_id")] int OrderId,
    [property: JsonPropertyName("customer")] CustomerDto Customer,
    [property: JsonPropertyName("items")] List<OrderItemDto> Items,
    [property: JsonPropertyName("created_at")] DateTimeOffset CreatedAt,
    [property: JsonPropertyName("status")] string Status);

public record CustomerDto(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("email")] string? Email);

public record OrderItemDto(
    [property: JsonPropertyName("sku")] string Sku,
    [property: JsonPropertyName("qty")] int Qty,
    [property: JsonPropertyName("unit_price")] decimal UnitPrice);
```

**מה תיקנו:** בגרסה הראשונה `unit_price` היה `double` — לכסף משתמשים ב-`decimal`. גם `status` עדיף כ-`enum` עם `JsonStringEnumConverter` אם הערכים ידועים. שימו לב שה-AI השתמש נכון ב-`[property: ...]` על פרמטרי record — בדקו זאת, זו טעות נפוצה.

## 2. LINQ משפה טבעית

**הבקשה:** "Given `IEnumerable<Order>` where Order has `CustomerName`, `Date`, `Items` (each with `Qty`, `UnitPrice`): return the top 3 customers by revenue in 2025 as `(Name, Revenue)`, method syntax."

**תוצאה:**

```csharp
var top = orders
    .Where(o => o.Date.Year == 2025)
    .GroupBy(o => o.CustomerName)
    .Select(g => (Name: g.Key,
                  Revenue: g.Sum(o => o.Items.Sum(i => i.Qty * i.UnitPrice))))
    .OrderByDescending(x => x.Revenue)
    .Take(3)
    .ToList();
```

**מה בדקנו:** שההקבצה היא לפי שם (ומה אם לשני לקוחות אותו שם? עדיף `CustomerId`). שהתוצאה מוחזרת כרשימה ולא כ-`IEnumerable` עצל שמחושב שוב בכל מעבר.

## 3. Boilerplate: INotifyPropertyChanged, מיפוי, CRUD

**הבקשה:** "Convert to a WPF ViewModel base: `ObservableObject` with `SetProperty<T>(ref T field, T value, [CallerMemberName] string? name = null)` returning bool."

**תוצאה:**

```csharp
public abstract class ObservableObject : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected bool SetProperty<T>(ref T field, T value,
        [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        return true;
    }
}
```

זה נכון ומקובל. עבור מיפויים ("map `Order` → `OrderDto`") ו-CRUD services ("implement `IRepository<T>` over a JSON file") ה-AI טוב מאוד — הדבר שנבדוק הוא **עקביות** עם הממשקים הקיימים, ושהוא לא הוסיף ספריית מיפוי חיצונית בלי לשאול.

## 4. Regex

**הבקשה:** "Regex for Israeli mobile: 05 followed by 8 digits, optional dash after the prefix. Anchored. Give matching/non-matching examples."

**תוצאה:** `^05\d-?\d{7}$`

**מה תיקנו:** בגרסה הראשונה חסר `^...$`, ולכן `"x0501234567y"` היה מתקבל בתוך טקסט. ב-.NET מומלץ גם `[GeneratedRegex]` (source generator) בקוד שרץ הרבה:

```csharp
public static partial class Phones
{
    [GeneratedRegex(@"^05\d-?\d{7}$")]
    public static partial Regex IsraeliMobile();
}
```

## 5. בדיקות xUnit

**הבקשה:** "Write xUnit tests for `DiscountCalculator.Calculate(decimal subtotal, int quantity)`: 5% over 250, 10% for quantity ≥ 10, larger wins, never negative. `[Theory]` + `[InlineData]`."

**תוצאה (מקוצרת):**

```csharp
public class DiscountCalculatorTests
{
    [Theory]
    [InlineData(100, 1, 100)]
    [InlineData(300, 1, 285)]     // 5%
    [InlineData(100, 10, 90)]     // 10%
    [InlineData(300, 10, 270)]    // larger wins
    [InlineData(0, 0, 0)]
    public void Calculate_ReturnsExpectedTotal(decimal subtotal, int qty, decimal expected)
        => Assert.Equal(expected, DiscountCalculator.Calculate(subtotal, qty));
}
```

**מה תיקנו:** `InlineData` עם `decimal` — המספרים בקוד נכתבים כ-`int`/`double` ומומרים; זה עובד עבור ערכים שלמים אבל `19.9` יגיע כ-`double` ויגרום לשגיאת המרה. פתרון: `[MemberData]` או ערכים שלמים. ה-AI לא תמיד יודע את זה.

## 6. תיעוד ו-XML comments

בקשה: "Add XML doc comments to public members; one sentence each; document exceptions with `<exception>`." בדיקה: שהתיעוד לא משקר (למשל "returns null if not found" כשבפועל נזרקת חריגה). תיעוד שגוי גרוע מהיעדר תיעוד.

## 7. Refactoring עם AI

בקשות ממוקדות עובדות הכי טוב:

- **Extract method**: "Extract the validation block (lines 12–30) into `ValidateOrder(Order)`; keep behavior."
- **Rename**: "Suggest better names for `Process2`, `tmp`, `flag`. Explain each."
- **Apply a pattern**: "Replace the `switch` on `paymentType` with a Strategy: `IPaymentHandler` + one class per type + registration in DI."

הדגמה מלאה ב-`Demos/Day4.Demo.LegacyMess` → `Demos/Day4.Demo.Refactored`. הכלל: **refactoring רק עם רשת ביטחון** — בדיקות או קובץ golden master (פלט "לפני" שמשווים אליו "אחרי"). זה בדיוק מה שנעשה ב-Lab 2.

## 8. מיגרציה: Newtonsoft.Json → System.Text.Json

**הבקשה:** "Migrate to System.Text.Json. Keep JSON output identical (camelCase, ignore nulls, indented). List behavioral differences."

**תוצאה:**

```csharp
private static readonly JsonSerializerOptions Options = new()
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    WriteIndented = true,
};

public static string Serialize(Order o) => JsonSerializer.Serialize(o, Options);
public static Order? Deserialize(string json) => JsonSerializer.Deserialize<Order>(json, Options);
```

**מה ה-AI ציין נכון (ובדקנו):** System.Text.Json רגיש לאותיות גדולות/קטנות בברירת מחדל בקריאה (`PropertyNameCaseInsensitive = true` אם צריך), לא מסדר `Dictionary` עם מפתחות שאינם מחרוזות באותו אופן, ולא תומך ב-`[JsonConverter]` של Newtonsoft. **מה תיקנו:** האפשרויות נוצרו בכל קריאה במקום `static readonly` — זה פוגע בביצועים (מטמון פנימי).

## 9. הודעות commit ותיאורי PR

```text
Write a commit message for this diff (conventional commits). Subject ≤ 72 chars.
Body: what changed and why, not how. Mention the tests added.
```

ב-GitHub, Copilot יכול להציע תיאור PR מהשינויים. תמיד קראו: לעיתים הוא מתאר קבצים במקום כוונה.

## הכלל של המודול: "Generate → Verify → Own"

| שלב | מה עושים |
|-----|----------|
| Generate | בקשה ממוקדת עם הקשר ודוגמאות |
| Verify | build, tests, קריאת ה-diff, analyzers |
| Own | התאמה למוסכמות, הסרת מה שלא צריך, commit קטן |

## טעויות נפוצות

- `double` לכסף, `DateTime` במקום `DateTimeOffset` לזמני שרת, `string` במקום `enum`.
- `JsonSerializerOptions` חדש בכל קריאה.
- בדיקות שבודקות את המימוש ולא את הדרישה (למשל מעתיקות את הנוסחה).
- Regex בלי עוגנים; Regex עבור דוא"ל "מושלם" באורך 200 תווים — לא צריך.
- מיגרציה "שעובדת" אבל משנה את פורמט ה-JSON שנשמר בקבצים קיימים.

## לסיכום

- AI מצוין ב-DTOs, LINQ, boilerplate, regex, בדיקות, תיעוד, refactoring ומיגרציות.
- לכל פלט יש "מה לתקן" — לרוב טיפוסים, אפשרויות, מקרי קצה ועקביות.
- Refactoring רק עם רשת ביטחון (tests / golden master).
- Generate → Verify → Own.

## קריאה נוספת

- System.Text.Json migration guide: https://learn.microsoft.com/dotnet/standard/serialization/system-text-json/migrate-from-newtonsoft
- Source-generated regex: https://learn.microsoft.com/dotnet/standard/base-types/regular-expression-source-generators
- xUnit: https://xunit.net/docs/getting-started/v2/netcore/cmdline
- Refactoring in Visual Studio: https://learn.microsoft.com/visualstudio/ide/refactoring-in-visual-studio
