using System.Collections.Concurrent;

namespace Day4.Lab3.Snippets;

/// <summary>Thread-safe token cache with expiry (UTC, injectable clock). Concurrent misses for the same key fetch once.</summary>
public sealed class TokenCache(Func<string, Task<string>> fetch, TimeProvider clock)
{
    private static readonly TimeSpan Lifetime = TimeSpan.FromMinutes(30);
    private readonly ConcurrentDictionary<string, Lazy<Task<(string Token, DateTimeOffset Expires)>>> _cache = new();

    public async Task<string> GetTokenAsync(string clientId)
    {
        while (true)
        {
            var entry = _cache.GetOrAdd(clientId, id => new Lazy<Task<(string, DateTimeOffset)>>(() => FetchAsync(id)));
            try
            {
                var (token, expires) = await entry.Value;
                if (expires > clock.GetUtcNow()) return token;
                _cache.TryRemove(new KeyValuePair<string, Lazy<Task<(string, DateTimeOffset)>>>(clientId, entry));
            }
            catch
            {
                _cache.TryRemove(new KeyValuePair<string, Lazy<Task<(string, DateTimeOffset)>>>(clientId, entry));
                throw;   // כשל ב-fetch לא נשאר במטמון
            }
        }
    }

    private async Task<(string, DateTimeOffset)> FetchAsync(string clientId)
        => (await fetch(clientId), clock.GetUtcNow() + Lifetime);

    public int Count => _cache.Count;
}
