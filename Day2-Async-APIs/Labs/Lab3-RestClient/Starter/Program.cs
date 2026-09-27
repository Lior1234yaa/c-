using Day2.Lab3;

// Lab 3 — Starter: תרחיש בדיקה ללקוח ה-REST
Console.OutputEncoding = System.Text.Encoding.UTF8;

using var http = new HttpClient { BaseAddress = new Uri("http://localhost:5080"), Timeout = TimeSpan.FromSeconds(10) };
var client = new ShopApiClient(http);

try
{
    var products = await client.GetProductsAsync();
    Console.WriteLine($"GET /api/products -> {products.Count} products");
    foreach (var p in products) Console.WriteLine($"  #{p.Id} {p.Name,-14} {p.Price,8:N2} ({p.Category}, stock {p.Stock})");

    // TODO (שלב 2): GetProductAsync(1) ו-GetProductAsync(999) (צפוי null)
    // TODO (שלב 3): Create -> Update -> Delete -> Get (צפוי null)
    // TODO (שלב 2–3): GetOrdersAsync(OrderStatus.Shipped), CreateOrderAsync, SetOrderStatusAsync
    // TODO (שלב 4): CreateProductAsync עם מחיר שלילי -> ApiException 400
    // TODO (שלב 5): GetFlakyAsync
    // TODO (שלב 6): GetSlowAsync(300, 2s) ו-GetSlowAsync(3000, 500ms)
    // TODO (שלב 7): GetPublicPostAsync(1)
}
catch (HttpRequestException ex)
{
    Console.WriteLine($"Cannot reach Day2.LocalApi at {http.BaseAddress}: {ex.Message}");
    Console.WriteLine("Start it first: cd Demos/Day2.LocalApi && dotnet run");
}
