namespace Day4.Lab3.Snippets;

/// <summary>Loads a JSON report from a URL and returns its length.</summary>
public class ReportLoader
{
    public int LastLength { get; private set; }

    public async void Load(string url)
    {
        using var client = new HttpClient();
        var json = client.GetStringAsync(url).Result;
        LastLength = json.Length;
    }

    public async Task<int> LoadManyAsync(IEnumerable<string> urls)
    {
        var total = 0;
        foreach (var url in urls)
        {
            using var client = new HttpClient();
            total += (await client.GetStringAsync(url)).Length;
        }
        return total;
    }
}
