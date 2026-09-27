using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

// ===============================================================
// Day2.Demo.HttpJson — HttpClient + System.Text.Json
// חלק א: JSON בלבד (עובד תמיד).  חלק ב: מול Day2.LocalApi (localhost:5080).
// חלק ג: מול API ציבורי (jsonplaceholder) — נכשל בעדינות בלי אינטרנט.
// ===============================================================

Console.OutputEncoding = System.Text.Encoding.UTF8;

// HttpClient: מופע אחד לכל התוכנית (לא using בכל קריאה!) — אחרת נגמרים ה-sockets.
using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };
http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
http.DefaultRequestHeaders.UserAgent.ParseAdd("Day2Demo/1.0");

var jsonOptions = new JsonSerializerOptions
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    PropertyNameCaseInsensitive = true,
    WriteIndented = true,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    Converters = { new JsonStringEnumConverter() },
};

Console.WriteLine("======== Part A: System.Text.Json ========");
JsonDemo();

Console.WriteLine("\n======== Part B: Day2.LocalApi (http://localhost:5080) ========");
await LocalApiDemo();

Console.WriteLine("\n======== Part C: public API (jsonplaceholder.typicode.com) ========");
await PublicApiDemo();

Console.WriteLine("\ndone.");

// ---------------------------------------------------------------
void JsonDemo()
{
    var order = new OrderDto(42, "Dana", DateTime.Parse("2026-03-01T10:30:00Z").ToUniversalTime(), OrderStatus.Paid,
        [new(1, "Laptop", 1, 4500m), new(2, "Mouse", 2, 89.9m)]) { InternalNote = "vip" };

    // Serialize
    string json = JsonSerializer.Serialize(order, jsonOptions);
    Console.WriteLine("  Serialize (camelCase, enum as string, [JsonIgnore] note hidden):");
    Console.WriteLine(Indent(json));

    // Deserialize (case-insensitive, missing optional -> default)
    var back = JsonSerializer.Deserialize<OrderDto>(json, jsonOptions)!;
    Console.WriteLine($"  Deserialize -> #{back.Id} {back.Customer} {back.Status} total={back.Total}");
    // שוויון של record הוא "רדוד": שדות פשוטים מושווים לפי ערך, אבל List מושווה לפי reference
    Console.WriteLine($"  record equality: items[0] equal = {back.Items[0] == order.Items[0]}, whole order equal = {back == order} (List compared by reference)");

    // [JsonPropertyName] — כשהשם ב-JSON שונה משם המאפיין
    var weather = JsonSerializer.Deserialize<WeatherDto>("""{"temperature_2m": 27.4, "wind_speed_10m": 12.1, "time": "2026-06-01T12:00"}""", jsonOptions)!;
    Console.WriteLine($"  [JsonPropertyName] -> temp={weather.Temperature}°C wind={weather.WindSpeed} at {weather.Time}");

    // JSON דינמי: JsonNode כשאין (או לא רוצים) מחלקה
    var node = JsonNode.Parse("""{"user":{"name":"Noa","tags":["a","b"]},"count":3}""")!;
    Console.WriteLine($"  JsonNode -> name={node["user"]?["name"]}, second tag={node["user"]?["tags"]?[1]}, count={(int)node["count"]!}");
    node["count"] = 4; node["user"]!["email"] = "noa@example.com";
    Console.WriteLine($"  JsonNode edited -> {node.ToJsonString()}");

    // JsonDocument — קריאה בלבד, מהיר וחסכוני
    using var doc = JsonDocument.Parse("""[{"id":1},{"id":2},{"id":3}]""");
    Console.WriteLine($"  JsonDocument -> array length={doc.RootElement.GetArrayLength()}, ids=" +
        string.Join(",", doc.RootElement.EnumerateArray().Select(e => e.GetProperty("id").GetInt32())));

    // שגיאת JSON -> JsonException
    try { JsonSerializer.Deserialize<OrderDto>("{ not json }", jsonOptions); }
    catch (JsonException ex) { Console.WriteLine($"  bad JSON -> JsonException: {ex.Message[..Math.Min(60, ex.Message.Length)]}..."); }
}

// ---------------------------------------------------------------
async Task LocalApiDemo()
{
    const string BaseUrl = "http://localhost:5080";
    try
    {
        // GET list — GetFromJsonAsync עושה GET + EnsureSuccess + Deserialize בשורה אחת
        var products = await http.GetFromJsonAsync<List<ProductDto>>($"{BaseUrl}/api/products", jsonOptions) ?? [];
        Console.WriteLine($"  GET /api/products -> {products.Count} products: {string.Join(", ", products.Select(p => p.Name))}");

        // GET by id עם טיפול בסטטוס
        using var resp404 = await http.GetAsync($"{BaseUrl}/api/products/999");
        Console.WriteLine($"  GET /api/products/999 -> {(int)resp404.StatusCode} {resp404.StatusCode}");

        // POST
        using var created = await http.PostAsJsonAsync($"{BaseUrl}/api/products",
            new ProductInput("Webcam", 199m, "Video", 10), jsonOptions);
        created.EnsureSuccessStatusCode();
        var newProduct = (await created.Content.ReadFromJsonAsync<ProductDto>(jsonOptions))!;
        Console.WriteLine($"  POST -> {(int)created.StatusCode} Location={created.Headers.Location} id={newProduct.Id}");

        // PUT
        var change = new ProductInput(newProduct.Name, 249m, newProduct.Category, newProduct.Stock);
        using var updated = await http.PutAsJsonAsync($"{BaseUrl}/api/products/{newProduct.Id}", change, jsonOptions);
        Console.WriteLine($"  PUT  -> {(int)updated.StatusCode} price now {(await updated.Content.ReadFromJsonAsync<ProductDto>(jsonOptions))!.Price}");

        // DELETE
        using var deleted = await http.DeleteAsync($"{BaseUrl}/api/products/{newProduct.Id}");
        Console.WriteLine($"  DELETE -> {(int)deleted.StatusCode} {deleted.StatusCode}");

        // Query string + enum
        var shipped = await http.GetFromJsonAsync<List<OrderDto>>($"{BaseUrl}/api/orders?status=Shipped", jsonOptions) ?? [];
        Console.WriteLine($"  GET /api/orders?status=Shipped -> {shipped.Count} order(s), total={shipped.Sum(o => o.Total)}");

        // Timeout per request via CancellationToken
        using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(500));
        try
        {
            await http.GetStringAsync($"{BaseUrl}/api/slow?ms=3000", cts.Token);
        }
        catch (OperationCanceledException) { Console.WriteLine("  GET /api/slow?ms=3000 with 500ms token -> cancelled (timeout) as expected"); }

        // Retry loop (Polly-style, בלי תלות חיצונית)
        var result = await GetWithRetryAsync($"{BaseUrl}/api/flaky?failRate=0.6", maxAttempts: 5);
        Console.WriteLine($"  GET /api/flaky with retry -> {result}");
    }
    catch (HttpRequestException ex)
    {
        Console.WriteLine($"  Day2.LocalApi is not running ({ex.Message}).");
        Console.WriteLine("  Start it with: cd Demos/Day2.LocalApi && dotnet run");
    }
}

async Task<string> GetWithRetryAsync(string url, int maxAttempts)
{
    for (int attempt = 1; ; attempt++)
    {
        try
        {
            using var resp = await http.GetAsync(url);
            if (resp.IsSuccessStatusCode) return $"success on attempt {attempt}";
            if (attempt >= maxAttempts || !IsTransient(resp.StatusCode)) return $"gave up after {attempt} attempts ({(int)resp.StatusCode})";
            Console.WriteLine($"    attempt {attempt}: {(int)resp.StatusCode} — retrying...");
        }
        catch (HttpRequestException) when (attempt < maxAttempts) { Console.WriteLine($"    attempt {attempt}: network error — retrying..."); }
        await Task.Delay(TimeSpan.FromMilliseconds(100 * Math.Pow(2, attempt - 1)));  // exponential backoff
    }

    static bool IsTransient(HttpStatusCode code) =>
        code is HttpStatusCode.ServiceUnavailable or HttpStatusCode.TooManyRequests or HttpStatusCode.RequestTimeout
            or HttpStatusCode.BadGateway or HttpStatusCode.GatewayTimeout;
}

// ---------------------------------------------------------------
async Task PublicApiDemo()
{
    try
    {
        var post = await http.GetFromJsonAsync<PostDto>("https://jsonplaceholder.typicode.com/posts/1", jsonOptions);
        Console.WriteLine($"  post #{post?.Id} by user {post?.UserId}: \"{post?.Title}\"");

        var todos = await http.GetFromJsonAsync<List<TodoDto>>("https://jsonplaceholder.typicode.com/todos?userId=1", jsonOptions) ?? [];
        Console.WriteLine($"  user 1 has {todos.Count} todos, {todos.Count(t => t.Completed)} completed");
    }
    catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
    {
        Console.WriteLine($"  No internet access ({ex.GetType().Name}) — skipping public API demo. That's fine in class: use Day2.LocalApi instead.");
    }
}

static string Indent(string text) => string.Join("\n", text.Split('\n').Select(l => "    " + l));

// ---------------- DTOs ----------------
enum OrderStatus { Pending, Paid, Shipped, Cancelled }

record ProductDto(int Id, string Name, decimal Price, string Category, int Stock);
record ProductInput(string Name, decimal Price, string? Category, int Stock);
record OrderItemDto(int ProductId, string ProductName, int Quantity, decimal UnitPrice);
record OrderDto(int Id, string Customer, DateTime CreatedAt, OrderStatus Status, List<OrderItemDto> Items)
{
    public decimal Total => Items.Sum(i => i.Quantity * i.UnitPrice);
    [JsonIgnore] public string? InternalNote { get; init; }
}

record WeatherDto(
    [property: JsonPropertyName("temperature_2m")] double Temperature,
    [property: JsonPropertyName("wind_speed_10m")] double WindSpeed,
    DateTime Time);

record PostDto(int UserId, int Id, string Title, string Body);
record TodoDto(int UserId, int Id, string Title, bool Completed);
