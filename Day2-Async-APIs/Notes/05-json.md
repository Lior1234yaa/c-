<div dir="rtl">

# מודול 05 — JSON: סריאליזציה ודה-סריאליזציה עם System.Text.Json

## מה זה JSON ולמה הוא בכל מקום

JSON (JavaScript Object Notation) הוא פורמט טקסט פשוט לייצוג נתונים: אובייקטים (`{}`), מערכים (`[]`), מחרוזות, מספרים, `true/false` ו-`null`. הוא הפך לשפה המשותפת של REST APIs, קבצי קונפיגורציה ותקשורת בין שירותים. **סריאליזציה** = מאובייקט C# לטקסט JSON; **דה-סריאליזציה** = ההפך.

<div dir="ltr">

```json
{
  "id": 42,
  "customer": "Dana",
  "createdAt": "2026-03-01T10:30:00Z",
  "status": "Paid",
  "items": [
    { "productId": 1, "productName": "Laptop", "quantity": 1, "unitPrice": 4500 }
  ]
}
```

</div>

ב-.NET המודרני הספרייה המובנית היא **`System.Text.Json`** (מרחבי השמות `System.Text.Json` ו-`System.Text.Json.Serialization`). היא מהירה, חסכונית בזיכרון, ולא צריכה חבילת NuGet.

## DTO — המחלקה שמייצגת את ה-JSON

הדרך הנוחה ביותר היא להגדיר **DTO (Data Transfer Object)** — טיפוס שמשקף את מבנה ה-JSON. `record` (יום 1) מושלם לזה: קצר, בלתי-משתנה, עם שוויון לפי ערך ו-`ToString` נחמד:

<div dir="ltr">

```csharp
public enum OrderStatus { Pending, Paid, Shipped, Cancelled }

public record OrderItem(int ProductId, string ProductName, int Quantity, decimal UnitPrice);

public record Order(int Id, string Customer, DateTime CreatedAt, OrderStatus Status, List<OrderItem> Items)
{
    public decimal Total => Items.Sum(i => i.Quantity * i.UnitPrice);   // מחושב — יסורלז, לא ידה-סורלז
}
```

</div>

`System.Text.Json` יודע לעבוד עם records דרך הבנאי הראשי (positional constructor): שמות הפרמטרים מותאמים לשמות ב-JSON. אובייקטים מקוננים ומערכים (`List<OrderItem>`) עובדים אוטומטית לכל עומק.

## `Serialize` ו-`Deserialize`

<div dir="ltr">

```csharp
using System.Text.Json;

var order = new Order(42, "Dana", DateTime.UtcNow, OrderStatus.Paid, [new(1, "Laptop", 1, 4500m)]);

string json = JsonSerializer.Serialize(order);
// {"Id":42,"Customer":"Dana","CreatedAt":"2026-...","Status":1,"Items":[...],"Total":4500}

Order back = JsonSerializer.Deserialize<Order>(json)!;   // null אם ה-JSON הוא "null"
```

</div>

שימו לב לשתי בעיות בפלט ברירת המחדל: שמות השדות ב-PascalCase (רוב ה-APIs מצפים ל-camelCase), וה-enum יצא כמספר `1`. את שתיהן פותרים עם אפשרויות.

## `JsonSerializerOptions`

<div dir="ltr">

```csharp
using System.Text.Json.Serialization;

var options = new JsonSerializerOptions
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,   // Customer -> customer
    PropertyNameCaseInsensitive = true,                  // בקריאה: "customer"/"Customer" שניהם OK
    WriteIndented = true,                                // פלט "יפה" עם רווחים (לקבצים/דיבוג, לא לרשת)
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,   // לא לכתוב שדות null
    Converters = { new JsonStringEnumConverter() },      // enum כמחרוזת: "Paid"
};

string json = JsonSerializer.Serialize(order, options);
var back = JsonSerializer.Deserialize<Order>(json, options);
```

</div>

- **צרו את ה-options פעם אחת** (שדה `static readonly`) והשתמשו בו שוב ושוב — הוא מטמון מידע על הטיפוסים, ויצירה בכל קריאה יקרה.
- יש קיצור: `JsonSerializerOptions.Web` — הגדרות ברירת המחדל של ASP.NET (camelCase, case-insensitive, מספרים גם ממחרוזות).
- `HttpClient.GetFromJsonAsync` (מודול 04) משתמש כברירת מחדל ב-`JsonSerializerOptions.Web`, ולכן לרוב camelCase "פשוט עובד". אפשר להעביר לו options משלכם.

## התאמות ברמת המאפיין

<div dir="ltr">

```csharp
public record Weather(
    [property: JsonPropertyName("temperature_2m")] double Temperature,   // שם שונה ב-JSON
    [property: JsonPropertyName("wind_speed_10m")] double WindSpeed,
    DateTime Time);

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = "";

    [JsonIgnore]                                       // לא לסרלז ולא לקרוא
    public string? PasswordHash { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int LoginCount { get; set; }                // מושמט כשהוא 0

    [JsonPropertyOrder(-1)]                            // ראשון בפלט
    public string Type => "user";
}
```

</div>

ב-`record` עם בנאי ראשי, ה-attribute חייב להתחיל ב-`[property: ...]` כדי שיוצמד למאפיין ולא לפרמטר.

## תאריכים, מספרים ו-null

- **`DateTime` / `DateTimeOffset`** — נכתבים ב-ISO 8601 (`"2026-03-01T10:30:00Z"`). העדיפו `DateTimeOffset` או `DateTime` ב-UTC כדי לא להסתבך עם אזורי זמן. `DateOnly` ו-`TimeOnly` נתמכים גם (`"2026-03-01"`).
- **`decimal`** לכסף. JSON לא מבחין בין int ל-float; `4500` ייקרא ל-`decimal` בלי בעיה.
- **שדה חסר ב-JSON** → ברירת מחדל של הטיפוס (`0`, `null`, `false`). שדה **עודף ב-JSON** → מתעלמים בשקט (אלא אם `UnmappedMemberHandling = Disallow`).
- **`required`** (C# 11) על מאפיין — ה-deserializer יזרוק אם השדה חסר. שימושי ל-DTOs "קפדניים".
- **`null`** למאפיין לא-nullable (`string Name`) — נכנס בשקט ב-`Nullable` disable, אבל עם `<Nullable>enable</Nullable>` עדיף לסמן `string?` או `required`.

## JSON דינמי: `JsonNode` ו-`JsonDocument`

לא תמיד יש (או רוצים) DTO — למשל תשובת API ענקית שצריך ממנה שני שדות, או מבנה שמשתנה. שתי אפשרויות:

<div dir="ltr">

```csharp
using System.Text.Json.Nodes;

// JsonNode — עץ שאפשר לקרוא *ולשנות*
JsonNode root = JsonNode.Parse(json)!;
string? name = (string?)root["user"]?["name"];
double temp  = (double)root["current"]!["temperature_2m"]!;
root["count"] = 4;                                  // שינוי
root["user"]!["email"] = "noa@example.com";         // הוספה
string updated = root.ToJsonString();

// JsonDocument — קריאה בלבד, הכי מהיר וחסכוני, חובה using
using JsonDocument doc = JsonDocument.Parse(json);
JsonElement items = doc.RootElement.GetProperty("items");
foreach (JsonElement item in items.EnumerateArray())
    Console.WriteLine(item.GetProperty("productName").GetString());

if (doc.RootElement.TryGetProperty("optional", out var opt)) { /* קיים */ }
```

</div>

כלל אצבע: DTO כשהמבנה ידוע ויציב (99% מהמקרים); `JsonNode` לעריכה/מבנה גמיש; `JsonDocument` לסריקה מהירה של JSON גדול.

## שגיאות

JSON לא תקין או לא תואם זורק `JsonException` עם מיקום השגיאה (`LineNumber`, `BytePositionInLine`). תמיד לתפוס אותו כשהקלט מגיע מבחוץ:

<div dir="ltr">

```csharp
try
{
    var order = JsonSerializer.Deserialize<Order>(input, options);
}
catch (JsonException ex)
{
    Console.WriteLine($"Invalid JSON at line {ex.LineNumber}: {ex.Message}");
}
```

</div>

## Source Generators (בקצרה)

כברירת מחדל `System.Text.Json` משתמש ב-reflection כדי "ללמוד" את הטיפוסים בזמן ריצה. עם **source generator** המהדר מייצר את קוד הסריאליזציה מראש — מהיר יותר בהפעלה, פחות זיכרון, ועובד ב-Native AOT (שם reflection לא זמין):

<div dir="ltr">

```csharp
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(Order))]
[JsonSerializable(typeof(List<Product>))]
internal partial class AppJsonContext : JsonSerializerContext { }

// שימוש:
string json = JsonSerializer.Serialize(order, AppJsonContext.Default.Order);
var back = JsonSerializer.Deserialize(json, AppJsonContext.Default.Order);
```

</div>

לאפליקציות רגילות זה אופציונלי; כשעוברים ל-AOT או לשירותים עתירי JSON — כדאי.

## System.Text.Json מול Newtonsoft.Json

Newtonsoft.Json (Json.NET) הייתה במשך שנים הספרייה הסטנדרטית, ותפגשו אותה בהמון קוד קיים. השוואה קצרה:

| | System.Text.Json | Newtonsoft.Json |
|---|---|---|
| מקור | מובנה ב-.NET (Core 3.0+) | חבילת NuGet |
| ביצועים | מהיר וחסכוני יותר | טוב, אבל יותר הקצאות |
| ברירת מחדל שמות | PascalCase (כמו ב-C#) | PascalCase |
| Case-insensitive | רק עם `PropertyNameCaseInsensitive` | תמיד |
| enum כמחרוזת | `JsonStringEnumConverter` | `StringEnumConverter` |
| שינוי שם | `[JsonPropertyName]` | `[JsonProperty]` |
| JSON דינמי | `JsonNode` / `JsonDocument` | `JObject` / `JToken` / `dynamic` |
| Source generators / AOT | כן | לא |
| גמישות (מקרי קצה, מחזורי הפניות, private setters) | קפדנית יותר | סלחנית ועשירה מאוד |
| מתי לבחור | ברירת המחדל לקוד חדש | קוד קיים, או פיצ'ר ספציפי שחסר |

## טעויות נפוצות

- **PascalCase מול camelCase** — ה-JSON יוצא/נקרא עם השמות הלא-נכונים. `PropertyNamingPolicy = CamelCase` + `PropertyNameCaseInsensitive = true`.
- **enum כמספר** — API מקבל `1` במקום `"Paid"`. `JsonStringEnumConverter`.
- **מאפיין בלי `set`/`init`** או שדה במקום מאפיין — לא ידה-סורלז (שדות דורשים `IncludeFields = true`).
- **יצירת `JsonSerializerOptions` בכל קריאה** — איטי. `static readonly`.
- **שכחת `using` על `JsonDocument`** — דליפת זיכרון מה-ArrayPool.
- **`Deserialize<T>` מחזיר null** — תוצאה של קלט `"null"`; בדקו לפני שימוש (`?? throw`).
- **הנחה שמספר גדול ייכנס ל-`int`** — `JsonException` בהצפה; השתמשו ב-`long`/`decimal`.

## לסיכום

- `System.Text.Json` מובנה, מהיר, ומספיק ל-95% מהמקרים. `JsonSerializer.Serialize/Deserialize` + `JsonSerializerOptions` סטטי.
- records כ-DTOs; `[JsonPropertyName]` לשמות שונים; `[JsonIgnore]` לשדות פנימיים; `JsonStringEnumConverter` ל-enums.
- `JsonNode` לעריכה דינמית, `JsonDocument` לקריאה מהירה.
- `JsonException` על קלט שגוי — לתפוס.
- Source generators לביצועים/AOT; Newtonsoft לקוד ישן או מקרים מיוחדים.

## קריאה נוספת

- [JSON serialization and deserialization in .NET](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/overview)
- [How to customize property names and values](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/customize-properties)
- [How to use a JSON DOM (JsonNode / JsonDocument)](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/use-dom)
- [Source generation in System.Text.Json](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/source-generation)
- [Migrate from Newtonsoft.Json to System.Text.Json](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/migrate-from-newtonsoft)

</div>
