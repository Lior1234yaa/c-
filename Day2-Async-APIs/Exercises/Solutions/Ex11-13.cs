using System.Net;
using System.Net.Http.Json;

// תרגיל 11 — GET + DTO
static class Ex11
{
    public static async Task RunAsync()
    {
        if (!await Api.IsUpAsync()) return;
        var products = await Api.Http.GetFromJsonAsync<List<Product>>("/api/products", Api.Json) ?? [];
        var priciest = products.MaxBy(p => p.Price);
        Console.WriteLine($"  {products.Count} products; most expensive: {priciest?.Name} ({priciest?.Price:C})");
    }
}

// תרגיל 12 — POST + PUT + DELETE
static class Ex12
{
    public static async Task RunAsync()
    {
        if (!await Api.IsUpAsync()) return;

        using var created = await Api.Http.PostAsJsonAsync("/api/products", new ProductInput("Exercise Widget", 10m, "Test", 1), Api.Json);
        Console.WriteLine($"  POST -> {(int)created.StatusCode} {created.StatusCode}, Location: {created.Headers.Location}");
        created.EnsureSuccessStatusCode();
        var product = (await created.Content.ReadFromJsonAsync<Product>(Api.Json))!;

        using var updated = await Api.Http.PutAsJsonAsync($"/api/products/{product.Id}", new ProductInput(product.Name, 12.5m, product.Category, product.Stock), Api.Json);
        var after = await updated.Content.ReadFromJsonAsync<Product>(Api.Json);
        Console.WriteLine($"  PUT  -> {(int)updated.StatusCode}, price {product.Price} -> {after?.Price}");

        using var deleted = await Api.Http.DeleteAsync($"/api/products/{product.Id}");
        Console.WriteLine($"  DELETE -> {(int)deleted.StatusCode} {deleted.StatusCode}");

        using var gone = await Api.Http.GetAsync($"/api/products/{product.Id}");
        Console.WriteLine(gone.StatusCode == HttpStatusCode.NotFound
            ? $"  GET after delete -> 404 as expected (handled without exception)"
            : $"  GET after delete -> unexpected {(int)gone.StatusCode}");
    }
}

// תרגיל 13 — retry + timeout
static class Ex13
{
    public static async Task RunAsync()
    {
        if (!await Api.IsUpAsync()) return;

        const int MaxAttempts = 5;
        for (int attempt = 1; attempt <= MaxAttempts; attempt++)
        {
            using var resp = await Api.Http.GetAsync("/api/flaky?failRate=0.5");
            if (resp.IsSuccessStatusCode) { Console.WriteLine($"  flaky: success on attempt {attempt}"); break; }
            if (attempt == MaxAttempts) { Console.WriteLine($"  flaky: gave up after {MaxAttempts} attempts"); break; }
            var delay = TimeSpan.FromMilliseconds(100 * Math.Pow(2, attempt - 1));
            Console.WriteLine($"  flaky: attempt {attempt} -> {(int)resp.StatusCode}, waiting {delay.TotalMilliseconds} ms");
            await Task.Delay(delay);
        }

        using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(500));
        try
        {
            await Api.Http.GetStringAsync("/api/slow?ms=3000", cts.Token);
            Console.WriteLine("  slow: finished (unexpected)");
        }
        catch (OperationCanceledException) { Console.WriteLine("  slow: cancelled after 500 ms (OperationCanceledException)"); }
    }
}
