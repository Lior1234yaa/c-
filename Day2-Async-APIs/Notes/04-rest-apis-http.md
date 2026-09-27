<div dir="rtl">

# מודול 04 — צריכת REST APIs ועבודה עם שירותים חיצוניים

## HTTP ב-5 דקות

כמעט כל תקשורת בין אפליקציה לשירות חיצוני עוברת היום ב-HTTP. בקשה (Request) מורכבת מ:

- **Method (פועל)** — מה רוצים לעשות: `GET` (קרא), `POST` (צור), `PUT` (החלף), `PATCH` (עדכן חלקית), `DELETE` (מחק).
- **URL** — כתובת המשאב, כולל **query string** (`?status=Shipped&page=2`).
- **Headers** — מטא-נתונים: `Content-Type: application/json`, `Authorization: Bearer ...`, `Accept`.
- **Body** — התוכן (ב-POST/PUT), בדרך כלל JSON.

התשובה (Response) מכילה **Status Code**, headers ו-body. קודי הסטטוס החשובים:

| קוד | משמעות | מה עושים בקוד |
|-----|--------|----------------|
| 200 OK / 201 Created / 204 No Content | הצלחה | קוראים את ה-body (ב-204 אין) |
| 400 Bad Request | הבקשה שלכם שגויה | לתקן את הקוד/הקלט, לא לנסות שוב |
| 401 Unauthorized / 403 Forbidden | בעיית הזדהות/הרשאה | לבדוק token / API key |
| 404 Not Found | המשאב לא קיים | לטפל כ"אין תוצאה", לא כשגיאה |
| 429 Too Many Requests | חרגתם מהמכסה | לחכות ולנסות שוב (`Retry-After`) |
| 500 / 502 / 503 / 504 | תקלה בשרת | ניסיון חוזר עם השהיה |

## REST בקצרה

REST הוא סגנון לתכנון API סביב **משאבים** (resources) שמזוהים ב-URL, כשהפעולה נקבעת לפי ה-Method:

| פעולה | Method + URL | תשובה טיפוסית |
|-------|--------------|----------------|
| כל המוצרים | `GET /api/products` | 200 + מערך |
| מוצר 7 | `GET /api/products/7` | 200 + אובייקט / 404 |
| יצירה | `POST /api/products` + body | 201 + `Location` header + האובייקט |
| עדכון מלא | `PUT /api/products/7` + body | 200 / 404 |
| מחיקה | `DELETE /api/products/7` | 204 / 404 |

`GET` ו-`DELETE` צריכים להיות **idempotent** — לבצע אותם פעמיים נותן אותה תוצאה. זה מה שמאפשר לנסות אותם שוב בבטחה. `POST` לא: שני POST = שני משאבים.

## `HttpClient` — נכון

`HttpClient` הוא הכלי ב-.NET לביצוע בקשות. יש כלל אחד שחייבים לזכור: **מופע אחד לכל האפליקציה (או לכל שירות), לא `new HttpClient()` בכל קריאה.** כל מופע מחזיק חיבורי TCP; יצירה ומחיקה חוזרת "מדליפה" sockets עד שהמערכת נחנקת (`SocketException`).

<div dir="ltr">

```csharp
// קונסול / אפליקציה קטנה: static אחד
private static readonly HttpClient Http = new()
{
    BaseAddress = new Uri("http://localhost:5080"),
    Timeout = TimeSpan.FromSeconds(10),
};
```

</div>

באפליקציות ASP.NET Core / עם DI, הדרך המומלצת היא **`IHttpClientFactory`** (`services.AddHttpClient<MyApiClient>()`), שמנהל את מחזור החיים של החיבורים ומאפשר להגדיר typed clients — ניגע בזה ביום 4.

## GET, POST, PUT, DELETE ב-C#

ההרחבות ב-`System.Net.Http.Json` הופכות את העבודה עם JSON לשורה אחת:

<div dir="ltr">

```csharp
using System.Net.Http.Json;

record Product(int Id, string Name, decimal Price, string Category, int Stock);
record ProductInput(string Name, decimal Price, string? Category, int Stock);

// GET רשימה — מבצע GET, בודק סטטוס, ומפענח JSON
List<Product> products = await Http.GetFromJsonAsync<List<Product>>("/api/products") ?? [];

// GET עם query string (Uri.EscapeDataString לערכים שמגיעים מהמשתמש!)
var shipped = await Http.GetFromJsonAsync<List<Order>>($"/api/orders?status={Uri.EscapeDataString("Shipped")}");

// POST — שולח JSON, מקבל את המשאב שנוצר
using var created = await Http.PostAsJsonAsync("/api/products", new ProductInput("Webcam", 199m, "Video", 10));
created.EnsureSuccessStatusCode();                          // זורק HttpRequestException אם לא 2xx
var product = await created.Content.ReadFromJsonAsync<Product>();
Console.WriteLine($"{(int)created.StatusCode} {created.Headers.Location}");   // 201 /api/products/7

// PUT
using var updated = await Http.PutAsJsonAsync($"/api/products/{product!.Id}", new ProductInput("Webcam HD", 249m, "Video", 8));

// DELETE
using var deleted = await Http.DeleteAsync($"/api/products/{product.Id}");
Console.WriteLine(deleted.StatusCode);                      // NoContent
```

</div>

כשרוצים שליטה מלאה (headers לבקשה בודדת, קריאת body גם בשגיאה) משתמשים ב-`HttpRequestMessage`:

<div dir="ltr">

```csharp
using var request = new HttpRequestMessage(HttpMethod.Get, "/api/orders/3");
request.Headers.Add("X-Correlation-Id", Guid.NewGuid().ToString());
using var response = await Http.SendAsync(request, cancellationToken);

if (response.StatusCode == HttpStatusCode.NotFound) return null;
response.EnsureSuccessStatusCode();
var order = await response.Content.ReadFromJsonAsync<Order>(cancellationToken);
```

</div>

## Headers והזדהות

<div dir="ltr">

```csharp
// לכל הבקשות של המופע:
Http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
Http.DefaultRequestHeaders.UserAgent.ParseAdd("MyApp/1.0");

// Bearer token (OAuth / JWT):
Http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

// API key — תלוי בספק; לרוב header מותאם או query string:
Http.DefaultRequestHeaders.Add("X-Api-Key", apiKey);
```

</div>

## Timeouts

בלי timeout, בקשה תקועה יכולה לחכות דקות. יש שתי רמות:

- `Http.Timeout` — ברירת מחדל 100 שניות לכל המופע. הורידו ל-10–30 שניות.
- **לבקשה בודדת** — `CancellationTokenSource` עם זמן, או `.WaitAsync(TimeSpan)`:

<div dir="ltr">

```csharp
using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));
try
{
    var text = await Http.GetStringAsync("/api/slow?ms=5000", cts.Token);
}
catch (OperationCanceledException)     // ב-.NET 5+ TaskCanceledException עם InnerException TimeoutException
{
    Console.WriteLine("timed out");
}
```

</div>

## ניסיונות חוזרים (Retry) עם backoff

שירותים נכשלים לרגע — 503, 429, ניתוק רשת. הפתרון: לנסות שוב, אבל **רק על שגיאות זמניות**, **מספר מוגבל של פעמים**, ועם **השהיה שגדלה** (exponential backoff). ספריית Polly עושה את זה יפה; להבנה נכתוב לולאה פשוטה בעצמנו:

<div dir="ltr">

```csharp
static async Task<HttpResponseMessage> GetWithRetryAsync(HttpClient http, string url, int maxAttempts = 4, CancellationToken ct = default)
{
    for (int attempt = 1; ; attempt++)
    {
        try
        {
            var response = await http.GetAsync(url, ct);
            if (response.IsSuccessStatusCode || !IsTransient(response.StatusCode) || attempt == maxAttempts)
                return response;
            response.Dispose();
        }
        catch (HttpRequestException) when (attempt < maxAttempts) { /* רשת נפלה — ננסה שוב */ }

        var delay = TimeSpan.FromMilliseconds(200 * Math.Pow(2, attempt - 1));   // 200, 400, 800...
        await Task.Delay(delay, ct);
    }

    static bool IsTransient(HttpStatusCode code) => code is
        HttpStatusCode.ServiceUnavailable or HttpStatusCode.TooManyRequests or
        HttpStatusCode.BadGateway or HttpStatusCode.GatewayTimeout or HttpStatusCode.RequestTimeout;
}
```

</div>

מה **לא** לנסות שוב: 400, 401, 403, 404 — הבעיה בבקשה, וניסיון נוסף לא ישנה כלום. וגם: לא לנסות שוב `POST` שאינו idempotent בלי מפתח ייחודי.

## טיפול בשגיאות — סיכום

| מה קרה | מה נזרק | איך מטפלים |
|--------|---------|------------|
| השרת לא זמין / DNS / TLS | `HttpRequestException` | הודעה ידידותית, retry אם הגיוני |
| סטטוס לא-2xx + `EnsureSuccessStatusCode()` | `HttpRequestException` (עם `StatusCode`) | לבדוק את הקוד לפני, או לתפוס |
| timeout / ביטול | `TaskCanceledException` (יורש מ-`OperationCanceledException`) | לתפוס בנפרד מ-Exception כללי |
| JSON לא צפוי | `JsonException` | לוג + fallback |

<div dir="ltr">

```csharp
try
{
    var data = await Http.GetFromJsonAsync<Product>("/api/products/7", ct);
}
catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { /* אין כזה */ }
catch (HttpRequestException ex) { Console.WriteLine($"HTTP error: {ex.StatusCode} {ex.Message}"); }
catch (OperationCanceledException) { Console.WriteLine("timeout / cancelled"); }
catch (JsonException ex) { Console.WriteLine($"bad JSON: {ex.Message}"); }
```

</div>

## שירותים חיצוניים לתרגול

- **Day2.LocalApi** — ה-API המקומי של הקורס (`Demos/Day2.LocalApi`), רץ על `http://localhost:5080`. עובד בלי אינטרנט, כולל `/api/slow` ו-`/api/flaky` לתרגול timeout ו-retry.
- **JSONPlaceholder** — `https://jsonplaceholder.typicode.com` — API ציבורי חינמי "מזויף" עם `/posts`, `/users`, `/todos`. תומך גם ב-POST (מחזיר תשובה אבל לא שומר).
- **Open-Meteo** — `https://api.open-meteo.com/v1/forecast?latitude=32.08&longitude=34.78&current=temperature_2m,wind_speed_10m` — מזג אוויר בלי API key. שימו לב לשמות השדות ב-snake_case (`temperature_2m`) — נצטרך `[JsonPropertyName]` (מודול 05).

**הקוד חייב לעבוד גם כשהשירות הציבורי לא זמין** — תפסו `HttpRequestException` ו-`TaskCanceledException`, והציגו הודעה ידידותית או ערך ברירת מחדל. זו לא "תכונה של הכיתה", זה איך קוד אמיתי צריך להתנהג.

## API Keys וסודות — לא בקוד!

מפתח שנכנס ל-Git נשאר שם לנצח (גם אחרי מחיקה — בהיסטוריה). הכלים:

<div dir="ltr">

```bash
# פיתוח: dotnet user-secrets (נשמר מחוץ לפרויקט, לא עולה ל-Git)
dotnet user-secrets init
dotnet user-secrets set "Weather:ApiKey" "abc123"

# Production: משתני סביבה
export WEATHER__APIKEY=abc123        # __ מפריד היררכיה בקונפיגורציה של .NET
```

</div>

<div dir="ltr">

```csharp
// בקונסול פשוט:
var apiKey = Environment.GetEnvironmentVariable("WEATHER_API_KEY")
             ?? throw new InvalidOperationException("WEATHER_API_KEY is not set");

// עם Microsoft.Extensions.Configuration (יום 4): config["Weather:ApiKey"]
```

</div>

הוסיפו `appsettings.*.json` עם סודות ל-`.gitignore`, והשתמשו ב-`appsettings.json` רק לערכי ברירת מחדל לא רגישים.

## בדיקת API ידנית

לפני שכותבים קוד C#, בודקים שה-API עונה כמו שחושבים:

<div dir="ltr">

```bash
curl http://localhost:5080/api/products/1
curl -i -X POST http://localhost:5080/api/products \
     -H "Content-Type: application/json" \
     -d '{"name":"Webcam","price":199,"category":"Video","stock":10}'
curl -i -X DELETE http://localhost:5080/api/products/7
```

</div>

- **curl** — בכל טרמינל (`-i` מציג headers, `-X` method, `-d` body).
- **Postman** — GUI, אוספי בקשות, סביבות (dev/prod) ומשתנים.
- **VS Code REST Client** (תוסף `humao.rest-client`) — קובץ `.http` בתוך הפרויקט; גם Visual Studio 2022+ תומך בקבצי `.http`:

<div dir="ltr">

```text
### get products
GET http://localhost:5080/api/products

### create
POST http://localhost:5080/api/products
Content-Type: application/json

{ "name": "Webcam", "price": 199, "category": "Video", "stock": 10 }
```

</div>

## טעויות נפוצות

- **`using var http = new HttpClient()` בכל קריאה** — דליפת sockets. מופע אחד / `IHttpClientFactory`.
- **`.Result` על `GetAsync`** — deadlock ב-UI (מודול 06). תמיד `await`.
- **לשכוח `EnsureSuccessStatusCode`** ואז לנסות לפענח JSON של דף שגיאה.
- **Retry על 400/404** או על POST לא idempotent.
- **בלי timeout** — בקשה תקועה = אפליקציה תקועה.
- **בניית query string בשרשור מחרוזות** בלי `Uri.EscapeDataString` — רווחים, `&`, עברית.
- **API key בקוד / ב-Git.**

## לסיכום

- HTTP: Method + URL + Headers + Body ⇄ Status + Headers + Body. 2xx טוב, 4xx אשמתכם, 5xx אשמת השרת.
- REST ממפה CRUD ל-GET/POST/PUT/DELETE על משאבים.
- `HttpClient` אחד לכל האפליקציה; `GetFromJsonAsync` / `PostAsJsonAsync` לעבודה נוחה.
- Timeout תמיד; retry רק על שגיאות זמניות, עם backoff ומספר ניסיונות מוגבל.
- לתפוס `HttpRequestException`, `OperationCanceledException`, `JsonException` — ולהתנהג יפה כשאין רשת.
- סודות ב-user-secrets / משתני סביבה, לא בקוד.

## קריאה נוספת

- [Make HTTP requests with HttpClient](https://learn.microsoft.com/en-us/dotnet/fundamentals/networking/http/httpclient)
- [HttpClient guidelines for .NET](https://learn.microsoft.com/en-us/dotnet/fundamentals/networking/http/httpclient-guidelines)
- [IHttpClientFactory in .NET](https://learn.microsoft.com/en-us/dotnet/core/extensions/httpclient-factory)
- [System.Net.Http.Json extensions](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.json.httpclientjsonextensions)
- [Safe storage of app secrets in development](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets)
- [HTTP response status codes (MDN)](https://developer.mozilla.org/en-US/docs/Web/HTTP/Status)

</div>
