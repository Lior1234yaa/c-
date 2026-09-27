namespace Day4.Exercises.Solutions;

/// <summary>
/// תרגיל 8: (1) async void — חריגה תפיל את התהליך ואי אפשר ל-await;
/// (2) .Result חוסם ועלול ל-deadlock ב-UI; (3) בונוס: HttpClient חדש בכל קריאה.
/// כדי לא לצאת לרשת, משתמשים ב-HttpMessageHandler מזויף.
/// </summary>
public static class Ex08_Async
{
    private sealed class FakeHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
            => Task.FromResult(new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            { Content = new StringContent("hello from fake server") });
    }

    // מופע אחד משותף (בפרויקט אמיתי: IHttpClientFactory)
    private static readonly HttpClient Http = new(new FakeHandler());

    public sealed class Loader(HttpClient http)
    {
        public async Task<int> LoadAsync(string url, CancellationToken ct = default)
        {
            var text = await http.GetStringAsync(url, ct);
            return text.Length;
        }
    }

    public static async Task RunAsync()
    {
        var loader = new Loader(Http);
        var length = await loader.LoadAsync("https://example.invalid/data");
        Console.WriteLine($"length = {length}");
        Console.WriteLine("תיקונים: async Task במקום async void, await במקום .Result, HttpClient משותף, CancellationToken.");
    }
}
