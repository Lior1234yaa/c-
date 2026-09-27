using Day2.Lab3;

// Lab 3 — Solution: תרחיש בדיקה מלא ללקוח ה-REST
Console.OutputEncoding = System.Text.Encoding.UTF8;

using var http = new HttpClient { BaseAddress = new Uri("http://localhost:5080"), Timeout = TimeSpan.FromSeconds(10) };
bool verbose = args.Contains("-v");
var client = new ShopApiClient(http, verbose ? Console.WriteLine : null);

try
{
    // --- GET ---
    var products = await client.GetProductsAsync();
    Console.WriteLine($"GET products -> {products.Count}");
    foreach (var p in products) Console.WriteLine($"  #{p.Id} {p.Name,-14} {p.Price,8:N2} ({p.Category}, stock {p.Stock})");

    var one = await client.GetProductAsync(1);
    var none = await client.GetProductAsync(999);
    Console.WriteLine($"GET product 1 -> {one?.Name}; GET product 999 -> {(none is null ? "null (404 handled)" : "?!")}");

    var found = await client.SearchProductsAsync("mo");
    Console.WriteLine($"search 'mo' -> {string.Join(", ", found.Select(p => p.Name))}");

    // --- POST / PUT / DELETE ---
    var created = await client.CreateProductAsync(new ProductInput("Webcam", 199m, "Video", 10));
    Console.WriteLine($"POST -> created #{created.Id} {created.Name} {created.Price}");
    var updated = await client.UpdateProductAsync(created.Id, new ProductInput("Webcam HD", 249m, "Video", 8));
    Console.WriteLine($"PUT  -> {updated?.Name} {updated?.Price}");
    var deleted = await client.DeleteProductAsync(created.Id);
    var afterDelete = await client.GetProductAsync(created.Id);
    Console.WriteLine($"DELETE -> {deleted}; GET after delete -> {(afterDelete is null ? "null" : "still there?!")}; DELETE again -> {await client.DeleteProductAsync(created.Id)}");

    // --- Orders ---
    var shipped = await client.GetOrdersAsync(OrderStatus.Shipped);
    Console.WriteLine($"GET orders?status=Shipped -> {shipped.Count}: " + string.Join("; ", shipped.Select(o => $"#{o.Id} {o.Customer} {o.Total:N2} ({o.Items.Count} items, {o.CreatedAt:yyyy-MM-dd})")));
    var order = await client.CreateOrderAsync(new OrderInput("Lab3 Student", [new(1, 1), new(2, 3)]));
    Console.WriteLine($"POST order -> #{order.Id} {order.Status} total {order.Total:N2}");
    var paid = await client.SetOrderStatusAsync(order.Id, OrderStatus.Paid);
    Console.WriteLine($"PUT order status -> {paid?.Status}");

    // --- Errors ---
    try
    {
        await client.CreateProductAsync(new ProductInput("Bad", -5m, null, 0));
        Console.WriteLine("ERROR: expected ApiException");
    }
    catch (ApiException ex) { Console.WriteLine($"POST invalid -> ApiException: {ex.Message} (StatusCode={(int)ex.StatusCode})"); }

    // --- Retry ---
    try
    {
        int attempts = await client.GetFlakyAsync(maxAttempts: 5);
        Console.WriteLine($"GET flaky -> succeeded after {attempts} attempt(s)");
    }
    catch (ApiException ex) { Console.WriteLine($"GET flaky -> gave up: {ex.Message}"); }

    // --- Timeout ---
    var fast = await client.GetSlowAsync(300, TimeSpan.FromSeconds(2));
    var slow = await client.GetSlowAsync(3000, TimeSpan.FromMilliseconds(500));
    Console.WriteLine($"GET slow 300ms/2s -> {fast?.TotalMilliseconds:F0} ms; slow 3000ms/500ms -> {(slow is null ? "null (timed out)" : slow)}");

    // --- Public API (optional) ---
    var post = await client.GetPublicPostAsync(1);
    Console.WriteLine(post is null ? "public API -> offline (handled gracefully)" : $"public API -> post #{post.Id}: {post.Title}");
}
catch (HttpRequestException ex)
{
    Console.WriteLine($"Cannot reach Day2.LocalApi at {http.BaseAddress}: {ex.Message}");
    Console.WriteLine("Start it first: cd Demos/Day2.LocalApi && dotnet run");
}
