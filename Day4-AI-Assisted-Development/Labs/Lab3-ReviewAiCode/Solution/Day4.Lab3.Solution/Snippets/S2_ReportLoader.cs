namespace Day4.Lab3.Snippets;

/// <summary>Loads JSON reports. HttpClient is injected (one shared instance / IHttpClientFactory), everything is truly async.</summary>
public sealed class ReportLoader(HttpClient http)
{
    public int LastLength { get; private set; }

    public async Task LoadAsync(string url, CancellationToken ct = default)
    {
        var json = await http.GetStringAsync(url, ct);
        LastLength = json.Length;
    }

    public async Task<int> LoadManyAsync(IEnumerable<string> urls, CancellationToken ct = default)
    {
        var total = 0;
        foreach (var url in urls)
            total += (await http.GetStringAsync(url, ct)).Length;
        return total;
    }
}
