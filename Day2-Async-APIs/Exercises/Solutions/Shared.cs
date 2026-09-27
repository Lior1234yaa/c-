using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

// עזרים משותפים לתרגילי HTTP (11–13)
static class Api
{
    public const string BaseUrl = "http://localhost:5080";

    // מופע אחד לכל התוכנית
    public static readonly HttpClient Http = new() { BaseAddress = new Uri(BaseUrl), Timeout = TimeSpan.FromSeconds(10) };

    public static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() },
    };

    /// <summary>מחזיר false ומדפיס הודעה ידידותית אם ה-API המקומי לא רץ.</summary>
    public static async Task<bool> IsUpAsync()
    {
        try
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));
            using var r = await Http.GetAsync("/api/health", cts.Token);
            return r.IsSuccessStatusCode;
        }
        catch (Exception ex) when (ex is HttpRequestException or OperationCanceledException)
        {
            Console.WriteLine($"Day2.LocalApi is not running at {BaseUrl} — start it with: cd Demos/Day2.LocalApi && dotnet run");
            return false;
        }
    }
}

record Product(int Id, string Name, decimal Price, string Category, int Stock);
record ProductInput(string Name, decimal Price, string? Category, int Stock);
