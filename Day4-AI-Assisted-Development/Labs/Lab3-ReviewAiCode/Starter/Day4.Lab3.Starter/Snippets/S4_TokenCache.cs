namespace Day4.Lab3.Snippets;

/// <summary>Caches access tokens per client id with an expiry.</summary>
public class TokenCache
{
    private readonly Dictionary<string, (string Token, DateTime Expires)> _cache = new();
    private readonly Func<string, Task<string>> _fetch;

    public TokenCache(Func<string, Task<string>> fetch) => _fetch = fetch;

    public async Task<string> GetTokenAsync(string clientId)
    {
        if (_cache.ContainsKey(clientId) && _cache[clientId].Expires > DateTime.Now)
            return _cache[clientId].Token;

        var token = await _fetch(clientId);
        _cache[clientId] = (token, DateTime.Now.AddMinutes(30));
        return token;
    }

    public int Count => _cache.Count;
}
