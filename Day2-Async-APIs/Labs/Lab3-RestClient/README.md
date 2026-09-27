<div dir="rtl">

# מעבדה 3 — לקוח REST מוקלד (Typed REST Client)

**משך:** 60 דקות | **מודולים:** 04, 05 | **פרויקט:** `Starter/Day2.Lab3.Starter`

## המטרה

לבנות מחלקה `ShopApiClient` שעוטפת את `Day2.LocalApi` — עם DTOs כ-records, פעולות GET/POST/PUT/DELETE, טיפול נכון בשגיאות, retry על נקודת קצה לא יציבה ו-timeout על נקודת קצה איטית. בסוף המעבדה יהיה לכם "SDK" קטן שאפשר להשתמש בו מכל אפליקציה.

## דרישות מוקדמות

- **`Day2.LocalApi` רץ** על `http://localhost:5080` (`Demos/Day2.LocalApi` → `dotnet run`). ראו את ה-README שלו לרשימת נקודות הקצה.
- מודול 04 (`HttpClient`, `System.Net.Http.Json`, סטטוסים, retry) ומודול 05 (`JsonSerializerOptions`, records, `[JsonPropertyName]`).

## המצב ההתחלתי

ב-`Starter`:

- `Dtos.cs` — `Product` ו-`ProductInput` מוכנים; `Order`, `OrderItem`, `OrderInput`, `OrderStatus` — TODO.
- `ShopApiClient.cs` — בנאי שמקבל `HttpClient`, `GetProductsAsync` ממומש; שאר המתודות `NotImplementedException`.
- `ApiException.cs` — חריגה מותאמת עם `StatusCode`.
- `Program.cs` — תרחיש בדיקה שקורא לכל המתודות ומדפיס תוצאות. אם ה-API לא רץ — מדפיס הודעה ויוצא.

## שלבים

### שלב 1 — DTOs (10 דק')
השלימו את ה-records ב-`Dtos.cs` לפי ה-JSON של `/api/orders` (הסתכלו ב-README של ה-API או ב-`curl`):

- `OrderStatus` — enum; ה-API מחזיר מחרוזת (`"Shipped"`), לכן צריך `JsonStringEnumConverter` ב-options.
- `OrderItem(ProductId, ProductName, Quantity, UnitPrice)`.
- `Order(Id, Customer, CreatedAt, Status, Items)` + מאפיין מחושב `Total`.
- `OrderInput(Customer, Items)` ו-`OrderItemInput(ProductId, Quantity)`.

הגדירו `JsonSerializerOptions` סטטי אחד ב-`ShopApiClient`: camelCase, case-insensitive, enum כמחרוזת.

### שלב 2 — GET (10 דק')
- `GetProductAsync(int id)` — מחזיר `Product?`; **404 → `null`**, לא חריגה. (רמז: `GetAsync` + בדיקת `StatusCode` לפני `ReadFromJsonAsync`.)
- `GetOrdersAsync(OrderStatus? status = null)` — עם query string כשיש סינון.

### שלב 3 — POST / PUT / DELETE (10 דק')
- `CreateProductAsync(ProductInput)` — `PostAsJsonAsync`, ודאו 201, החזירו את המוצר שנוצר.
- `UpdateProductAsync(int id, ProductInput)` — PUT, 404 → `null`.
- `DeleteProductAsync(int id)` — מחזיר `bool` (204 → true, 404 → false).
- `CreateOrderAsync(OrderInput)` ו-`SetOrderStatusAsync(int id, OrderStatus)`.

### שלב 4 — שגיאות (10 דק')
- לכל תשובה שאינה 2xx/404 צפויים — זרקו `ApiException(statusCode, message)`; קראו את ה-body של השגיאה (`{"error": "..."}`) לתוך ההודעה.
- תפסו `HttpRequestException` (השרת לא זמין) ב-`Program` והדפיסו הודעה ידידותית.
- בדקו: `CreateProductAsync` עם מחיר שלילי → 400 → `ApiException` עם ההודעה של השרת.

### שלב 5 — Retry (10 דק')
ממשו `GetFlakyAsync(int maxAttempts = 5)` שקורא ל-`/api/flaky?failRate=0.5` ומנסה שוב על 503 עם backoff מעריכי (100, 200, 400... ms). החזירו את מספר הניסיונות שנדרשו. אל תנסו שוב על 4xx.

### שלב 6 — Timeout (5 דק')
ממשו `GetSlowAsync(int ms, TimeSpan timeout)` שקורא ל-`/api/slow?ms=` עם `CancellationTokenSource(timeout)` ומחזיר `TimeSpan?` — `null` אם עבר ה-timeout (תפסו `OperationCanceledException`). בדקו: 300 ms עם timeout 2s → מצליח; 3000 ms עם 500 ms → `null`.

### שלב 7 — (אופציונלי) API ציבורי (5 דק')
אם יש אינטרנט: `GetPublicPostAsync(int id)` מול `https://jsonplaceholder.typicode.com/posts/{id}`. אם אין — תפסו את השגיאה והדפיסו "offline".

## קריטריוני קבלה

- [ ] כל המתודות ב-`ShopApiClient` ממומשות ו-`Program` רץ מתחילתו לסופו בלי חריגות לא מטופלות.
- [ ] `GetProductAsync(999)` מחזיר `null` (לא זורק).
- [ ] `CreateProductAsync` עם קלט שגוי זורק `ApiException` עם `StatusCode == 400` והודעת השרת.
- [ ] `GetFlakyAsync` מצליח ברוב הריצות ומדפיס את מספר הניסיונות.
- [ ] `GetSlowAsync(3000, 500ms)` מחזיר `null` תוך ~0.5 שניות.
- [ ] יש מופע `HttpClient` **אחד** לכל התוכנית, ו-`JsonSerializerOptions` אחד סטטי.
- [ ] כשה-API לא רץ, התוכנית מדפיסה הודעה ברורה ולא crash.

## בונוס

- הוסיפו `CancellationToken` לכל מתודה והעבירו אותו הלאה.
- הוסיפו `ILogger` (או `Action<string>`) לרישום כל בקשה: method, url, status, ms.
- כתבו `SearchProductsAsync(string term)` עם `Uri.EscapeDataString`.

## רמזים

- `response.Content.ReadFromJsonAsync<T>(options)` — אל תשכחו להעביר את ה-options.
- `HttpResponseMessage` הוא `IDisposable` — `using var`.
- `TaskCanceledException` יורשת מ-`OperationCanceledException` — תפסו את האב.

</div>
