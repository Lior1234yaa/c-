using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Day2.Lab3;

public class ShopApiClient(HttpClient http)
{
    // TODO (שלב 1): camelCase, case-insensitive, enum כמחרוזת
    private static readonly JsonSerializerOptions JsonOptions = new();

    public async Task<List<Product>> GetProductsAsync()
    {
        return await http.GetFromJsonAsync<List<Product>>("/api/products", JsonOptions) ?? [];
    }

    public Task<Product?> GetProductAsync(int id)
    {
        // TODO (שלב 2): 404 -> null; סטטוס אחר שאינו 2xx -> ApiException
        throw new NotImplementedException();
    }

    public Task<Product> CreateProductAsync(ProductInput input)
    {
        // TODO (שלב 3): PostAsJsonAsync; לוודא 201; להחזיר את המוצר שנוצר
        throw new NotImplementedException();
    }

    public Task<Product?> UpdateProductAsync(int id, ProductInput input)
    {
        // TODO (שלב 3): PutAsJsonAsync; 404 -> null
        throw new NotImplementedException();
    }

    public Task<bool> DeleteProductAsync(int id)
    {
        // TODO (שלב 3): DeleteAsync; 204 -> true; 404 -> false
        throw new NotImplementedException();
    }

    // TODO (שלב 2): GetOrdersAsync(OrderStatus? status = null)
    // TODO (שלב 3): CreateOrderAsync(OrderInput), SetOrderStatusAsync(int id, OrderStatus status)

    public Task<int> GetFlakyAsync(int maxAttempts = 5)
    {
        // TODO (שלב 5): retry על 503 עם backoff מעריכי; להחזיר את מספר הניסיונות
        throw new NotImplementedException();
    }

    public Task<TimeSpan?> GetSlowAsync(int ms, TimeSpan timeout)
    {
        // TODO (שלב 6): CancellationTokenSource(timeout); null אם בוטל
        throw new NotImplementedException();
    }

    public Task<PublicPost?> GetPublicPostAsync(int id)
    {
        // TODO (שלב 7, אופציונלי): jsonplaceholder.typicode.com/posts/{id}; null אם אין רשת
        throw new NotImplementedException();
    }
}
