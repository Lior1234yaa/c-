using System.Diagnostics;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Day2.Lab3;

/// <summary>לקוח מוקלד מעל Day2.LocalApi. מקבל HttpClient מבחוץ (מופע אחד לכל האפליקציה).</summary>
public class ShopApiClient(HttpClient http, Action<string>? log = null)
{
    // אחד לכל התוכנית — יצירה בכל קריאה יקרה
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)   // camelCase + case-insensitive
    {
        Converters = { new JsonStringEnumConverter() },
    };

    // ---------- Products ----------
    public async Task<List<Product>> GetProductsAsync(CancellationToken ct = default)
    {
        using var response = await SendAsync(HttpMethod.Get, "/api/products", ct: ct);
        await EnsureSuccessAsync(response, ct);
        return await response.Content.ReadFromJsonAsync<List<Product>>(JsonOptions, ct) ?? [];
    }

    public async Task<List<Product>> SearchProductsAsync(string term, CancellationToken ct = default)
    {
        using var response = await SendAsync(HttpMethod.Get, $"/api/products?search={Uri.EscapeDataString(term)}", ct: ct);
        await EnsureSuccessAsync(response, ct);
        return await response.Content.ReadFromJsonAsync<List<Product>>(JsonOptions, ct) ?? [];
    }

    public async Task<Product?> GetProductAsync(int id, CancellationToken ct = default)
    {
        using var response = await SendAsync(HttpMethod.Get, $"/api/products/{id}", ct: ct);
        if (response.StatusCode == HttpStatusCode.NotFound) return null;      // 404 הוא תשובה לגיטימית, לא חריגה
        await EnsureSuccessAsync(response, ct);
        return await response.Content.ReadFromJsonAsync<Product>(JsonOptions, ct);
    }

    public async Task<Product> CreateProductAsync(ProductInput input, CancellationToken ct = default)
    {
        using var response = await SendAsync(HttpMethod.Post, "/api/products", input, ct);
        await EnsureSuccessAsync(response, ct, HttpStatusCode.Created);
        return await response.Content.ReadFromJsonAsync<Product>(JsonOptions, ct)
               ?? throw new ApiException(response.StatusCode, "empty body");
    }

    public async Task<Product?> UpdateProductAsync(int id, ProductInput input, CancellationToken ct = default)
    {
        using var response = await SendAsync(HttpMethod.Put, $"/api/products/{id}", input, ct);
        if (response.StatusCode == HttpStatusCode.NotFound) return null;
        await EnsureSuccessAsync(response, ct);
        return await response.Content.ReadFromJsonAsync<Product>(JsonOptions, ct);
    }

    public async Task<bool> DeleteProductAsync(int id, CancellationToken ct = default)
    {
        using var response = await SendAsync(HttpMethod.Delete, $"/api/products/{id}", ct: ct);
        if (response.StatusCode == HttpStatusCode.NotFound) return false;
        await EnsureSuccessAsync(response, ct, HttpStatusCode.NoContent);
        return true;
    }

    // ---------- Orders ----------
    public async Task<List<Order>> GetOrdersAsync(OrderStatus? status = null, CancellationToken ct = default)
    {
        var url = status is null ? "/api/orders" : $"/api/orders?status={status}";
        using var response = await SendAsync(HttpMethod.Get, url, ct: ct);
        await EnsureSuccessAsync(response, ct);
        return await response.Content.ReadFromJsonAsync<List<Order>>(JsonOptions, ct) ?? [];
    }

    public async Task<Order> CreateOrderAsync(OrderInput input, CancellationToken ct = default)
    {
        using var response = await SendAsync(HttpMethod.Post, "/api/orders", input, ct);
        await EnsureSuccessAsync(response, ct, HttpStatusCode.Created);
        return (await response.Content.ReadFromJsonAsync<Order>(JsonOptions, ct))!;
    }

    public async Task<Order?> SetOrderStatusAsync(int id, OrderStatus status, CancellationToken ct = default)
    {
        using var response = await SendAsync(HttpMethod.Put, $"/api/orders/{id}", new OrderStatusInput(status), ct);
        if (response.StatusCode == HttpStatusCode.NotFound) return null;
        await EnsureSuccessAsync(response, ct);
        return await response.Content.ReadFromJsonAsync<Order>(JsonOptions, ct);
    }

    // ---------- Resilience ----------
    /// <summary>קורא ל-/api/flaky עם retry על שגיאות זמניות. מחזיר את מספר הניסיונות שנדרשו.</summary>
    public async Task<int> GetFlakyAsync(int maxAttempts = 5, double failRate = 0.5, CancellationToken ct = default)
    {
        for (int attempt = 1; ; attempt++)
        {
            try
            {
                using var response = await SendAsync(HttpMethod.Get, $"/api/flaky?failRate={failRate}", ct: ct);
                if (response.IsSuccessStatusCode) return attempt;
                if (!IsTransient(response.StatusCode) || attempt == maxAttempts)
                    throw new ApiException(response.StatusCode, await ReadErrorAsync(response, ct) + $" (after {attempt} attempts)");
            }
            catch (HttpRequestException) when (attempt < maxAttempts) { /* רשת נפלה לרגע — ננסה שוב */ }

            var delay = TimeSpan.FromMilliseconds(100 * Math.Pow(2, attempt - 1));   // 100, 200, 400, 800...
            log?.Invoke($"  retry {attempt}/{maxAttempts} in {delay.TotalMilliseconds} ms");
            await Task.Delay(delay, ct);
        }
    }

    /// <summary>קורא ל-/api/slow עם timeout. null אם עבר הזמן.</summary>
    public async Task<TimeSpan?> GetSlowAsync(int ms, TimeSpan timeout, CancellationToken ct = default)
    {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);   // מכבד גם ביטול חיצוני
        cts.CancelAfter(timeout);
        var sw = Stopwatch.StartNew();
        try
        {
            using var response = await SendAsync(HttpMethod.Get, $"/api/slow?ms={ms}", ct: cts.Token);
            await EnsureSuccessAsync(response, cts.Token);
            return sw.Elapsed;
        }
        catch (OperationCanceledException) when (!ct.IsCancellationRequested)   // ה-timeout שלנו, לא ביטול של הקורא
        {
            return null;
        }
    }

    /// <summary>API ציבורי — null כשאין רשת.</summary>
    public async Task<PublicPost?> GetPublicPostAsync(int id, CancellationToken ct = default)
    {
        try
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            cts.CancelAfter(TimeSpan.FromSeconds(5));
            return await http.GetFromJsonAsync<PublicPost>($"https://jsonplaceholder.typicode.com/posts/{id}", JsonOptions, cts.Token);
        }
        catch (Exception ex) when (ex is HttpRequestException or OperationCanceledException)
        {
            log?.Invoke($"  public API unavailable: {ex.GetType().Name}");
            return null;
        }
    }

    // ---------- Helpers ----------
    private async Task<HttpResponseMessage> SendAsync(HttpMethod method, string url, object? body = null, CancellationToken ct = default)
    {
        using var request = new HttpRequestMessage(method, url);
        if (body is not null) request.Content = JsonContent.Create(body, options: JsonOptions);
        var sw = Stopwatch.StartNew();
        var response = await http.SendAsync(request, ct);
        log?.Invoke($"  {method.Method,-6} {url,-40} -> {(int)response.StatusCode} ({sw.ElapsedMilliseconds} ms)");
        return response;
    }

    private static async Task EnsureSuccessAsync(HttpResponseMessage response, CancellationToken ct, HttpStatusCode? expected = null)
    {
        if (response.IsSuccessStatusCode && (expected is null || response.StatusCode == expected)) return;
        throw new ApiException(response.StatusCode, await ReadErrorAsync(response, ct));
    }

    private static async Task<string> ReadErrorAsync(HttpResponseMessage response, CancellationToken ct)
    {
        try
        {
            var error = await response.Content.ReadFromJsonAsync<ApiError>(JsonOptions, ct);
            if (!string.IsNullOrWhiteSpace(error?.Error)) return error.Error;
        }
        catch (JsonException) { }
        var text = await response.Content.ReadAsStringAsync(ct);
        return string.IsNullOrWhiteSpace(text) ? response.ReasonPhrase ?? "unknown error" : text;
    }

    private static bool IsTransient(HttpStatusCode code) => code is
        HttpStatusCode.ServiceUnavailable or HttpStatusCode.TooManyRequests or HttpStatusCode.RequestTimeout or
        HttpStatusCode.BadGateway or HttpStatusCode.GatewayTimeout;
}
