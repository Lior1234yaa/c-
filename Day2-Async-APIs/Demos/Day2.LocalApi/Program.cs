using System.Collections.Concurrent;
using System.Text.Json.Serialization;

// ---------------------------------------------------------------
// Day2.LocalApi — REST API קטן בזיכרון (Products + Orders)
// משמש את המעבדות של יום 2 כדי שהכיתה תעבוד גם בלי אינטרנט.
// הרצה:  dotnet run   ->  http://localhost:5080
// ---------------------------------------------------------------

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://localhost:5080");
builder.Services.ConfigureHttpJsonOptions(o =>
{
    o.SerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    o.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
builder.Logging.SetMinimumLevel(LogLevel.Warning);

var app = builder.Build();
var store = new InMemoryStore();
var random = new Random();

app.MapGet("/", () => Results.Ok(new
{
    name = "Day2.LocalApi",
    endpoints = new[]
    {
        "GET /api/health", "GET /api/products", "GET /api/products/{id}", "POST /api/products",
        "PUT /api/products/{id}", "DELETE /api/products/{id}",
        "GET /api/orders?status=", "GET /api/orders/{id}", "POST /api/orders", "PUT /api/orders/{id}",
        "DELETE /api/orders/{id}", "GET /api/slow?ms=", "GET /api/flaky?failRate=", "GET /api/stats",
    },
}));

app.MapGet("/api/health", () => Results.Ok(new { status = "ok", time = DateTime.UtcNow }));

// ---------------- Products ----------------
app.MapGet("/api/products", (string? search) =>
{
    var items = store.Products.Values.OrderBy(p => p.Id).AsEnumerable();
    if (!string.IsNullOrWhiteSpace(search))
        items = items.Where(p => p.Name.Contains(search, StringComparison.OrdinalIgnoreCase));
    return Results.Ok(items);
});

app.MapGet("/api/products/{id:int}", (int id) =>
    store.Products.TryGetValue(id, out var p) ? Results.Ok(p) : Results.NotFound(new { error = $"product {id} not found" }));

app.MapPost("/api/products", (ProductInput input) =>
{
    if (string.IsNullOrWhiteSpace(input.Name) || input.Price < 0)
        return Results.BadRequest(new { error = "name is required and price must be >= 0" });
    var product = store.AddProduct(input);
    return Results.Created($"/api/products/{product.Id}", product);
});

app.MapPut("/api/products/{id:int}", (int id, ProductInput input) =>
{
    if (!store.Products.ContainsKey(id)) return Results.NotFound(new { error = $"product {id} not found" });
    if (string.IsNullOrWhiteSpace(input.Name) || input.Price < 0)
        return Results.BadRequest(new { error = "name is required and price must be >= 0" });
    var updated = new Product(id, input.Name, input.Price, input.Category ?? "General", input.Stock);
    store.Products[id] = updated;
    return Results.Ok(updated);
});

app.MapDelete("/api/products/{id:int}", (int id) =>
    store.Products.TryRemove(id, out _) ? Results.NoContent() : Results.NotFound(new { error = $"product {id} not found" }));

// ---------------- Orders ----------------
app.MapGet("/api/orders", (OrderStatus? status) =>
{
    var items = store.Orders.Values.OrderBy(o => o.Id).AsEnumerable();
    if (status is not null) items = items.Where(o => o.Status == status);
    return Results.Ok(items);
});

app.MapGet("/api/orders/{id:int}", (int id) =>
    store.Orders.TryGetValue(id, out var o) ? Results.Ok(o) : Results.NotFound(new { error = $"order {id} not found" }));

app.MapPost("/api/orders", (OrderInput input) =>
{
    if (input.Items is null || input.Items.Count == 0)
        return Results.BadRequest(new { error = "order must contain at least one item" });
    foreach (var item in input.Items)
        if (!store.Products.ContainsKey(item.ProductId))
            return Results.BadRequest(new { error = $"unknown product {item.ProductId}" });
    var order = store.AddOrder(input);
    return Results.Created($"/api/orders/{order.Id}", order);
});

app.MapPut("/api/orders/{id:int}", (int id, OrderStatusInput input) =>
{
    if (!store.Orders.TryGetValue(id, out var existing))
        return Results.NotFound(new { error = $"order {id} not found" });
    var updated = existing with { Status = input.Status };
    store.Orders[id] = updated;
    return Results.Ok(updated);
});

app.MapDelete("/api/orders/{id:int}", (int id) =>
    store.Orders.TryRemove(id, out _) ? Results.NoContent() : Results.NotFound(new { error = $"order {id} not found" }));

// ---------------- Helpers for labs ----------------
// נקודת קצה איטית בכוונה — לתרגול timeout ו-cancellation
app.MapGet("/api/slow", async (int? ms, CancellationToken ct) =>
{
    var delay = Math.Clamp(ms ?? 3000, 0, 60_000);
    await Task.Delay(delay, ct);
    return Results.Ok(new { message = $"slept {delay} ms", at = DateTime.UtcNow });
});

// נקודת קצה שנכשלת אקראית — לתרגול retry
app.MapGet("/api/flaky", (double? failRate) =>
{
    var rate = Math.Clamp(failRate ?? 0.5, 0, 1);
    if (random.NextDouble() < rate)
        return Results.Json(new { error = "random failure, try again" }, statusCode: 503);
    return Results.Ok(new { message = "success!", at = DateTime.UtcNow });
});

app.MapGet("/api/stats", () =>
{
    var orders = store.Orders.Values.ToList();
    return Results.Ok(new
    {
        products = store.Products.Count,
        orders = orders.Count,
        pending = orders.Count(o => o.Status == OrderStatus.Pending),
        shipped = orders.Count(o => o.Status == OrderStatus.Shipped),
        revenue = orders.Where(o => o.Status != OrderStatus.Cancelled).Sum(o => o.Total),
    });
});

app.MapPost("/api/reset", () => { store.Reset(); return Results.Ok(new { message = "store reset" }); });

Console.WriteLine("Day2.LocalApi listening on http://localhost:5080  (Ctrl+C to stop)");
app.Run();

// ---------------- Models ----------------
public enum OrderStatus { Pending, Paid, Shipped, Cancelled }

public record Product(int Id, string Name, decimal Price, string Category, int Stock);
public record ProductInput(string Name, decimal Price, string? Category, int Stock);

public record OrderItem(int ProductId, string ProductName, int Quantity, decimal UnitPrice);
public record Order(int Id, string Customer, DateTime CreatedAt, OrderStatus Status, List<OrderItem> Items)
{
    public decimal Total => Items.Sum(i => i.Quantity * i.UnitPrice);
}
public record OrderItemInput(int ProductId, int Quantity);
public record OrderInput(string Customer, List<OrderItemInput> Items);
public record OrderStatusInput(OrderStatus Status);

public class InMemoryStore
{
    public ConcurrentDictionary<int, Product> Products { get; } = new();
    public ConcurrentDictionary<int, Order> Orders { get; } = new();
    private int _productId;
    private int _orderId;

    public InMemoryStore() => Reset();

    public Product AddProduct(ProductInput input)
    {
        var id = Interlocked.Increment(ref _productId);
        var p = new Product(id, input.Name, input.Price, input.Category ?? "General", input.Stock);
        Products[id] = p;
        return p;
    }

    public Order AddOrder(OrderInput input)
    {
        var id = Interlocked.Increment(ref _orderId);
        var items = input.Items.Select(i =>
        {
            var p = Products[i.ProductId];
            return new OrderItem(p.Id, p.Name, Math.Max(1, i.Quantity), p.Price);
        }).ToList();
        var order = new Order(id, string.IsNullOrWhiteSpace(input.Customer) ? "anonymous" : input.Customer,
            DateTime.UtcNow, OrderStatus.Pending, items);
        Orders[id] = order;
        return order;
    }

    public void Reset()
    {
        Products.Clear(); Orders.Clear();
        _productId = 0; _orderId = 0;
        AddProduct(new("Laptop", 4500m, "Computers", 12));
        AddProduct(new("Mouse", 89.9m, "Accessories", 200));
        AddProduct(new("Keyboard", 249m, "Accessories", 75));
        AddProduct(new("Monitor 27\"", 1290m, "Displays", 30));
        AddProduct(new("USB-C Hub", 159m, "Accessories", 120));
        AddProduct(new("Headphones", 399m, "Audio", 60));
        AddOrder(new("Dana", [new(1, 1), new(2, 2)]));
        AddOrder(new("Yossi", [new(4, 2)]));
        AddOrder(new("Noa", [new(3, 1), new(5, 1), new(6, 1)]));
        Orders[2] = Orders[2] with { Status = OrderStatus.Shipped };
    }
}
