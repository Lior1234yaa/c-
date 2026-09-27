// Day4.Lab3.Starter — בדיקות עצמיות קטנות שמדגימות את הבאגים. ב-Starter חלקן נכשלות; אחרי התיקון כולן עוברות.
using System.Globalization;
using Day4.Lab3.Snippets;

var failures = 0;
void Check(string name, bool ok, string hint)
{
    Console.WriteLine($"{(ok ? "PASS" : "FAIL")}  {name}{(ok ? "" : "  -> " + hint)}");
    if (!ok) failures++;
}

// S1: path traversal
var root = Path.Combine(Path.GetTempPath(), "lab3-exports-" + Guid.NewGuid().ToString("N"));
try
{
    var saved = new FileExport(root).Save(Path.Combine("..", "escaped.txt"), "x");
    Check("S1 rejects '..' file names", false, $"wrote outside root: {saved}");
}
catch (Exception ex) when (ex is UnauthorizedAccessException or ArgumentException)
{
    Check("S1 rejects '..' file names", true, "");
}
finally
{
    try { Directory.Delete(root, true); File.Delete(Path.Combine(Path.GetTempPath(), "escaped.txt")); } catch { }
}

// S3: culture + swallowed errors
var original = CultureInfo.CurrentCulture;
CultureInfo.CurrentCulture = new CultureInfo("de-DE");
try
{
    var rows = new PriceParser().Parse(["A-1;19.90;2025-03-01"]);
    Check("S3 parses 19.90 as 19.90 under de-DE", rows.Count == 1 && rows[0].Price == 19.90m,
        $"got {(rows.Count == 0 ? "no rows (swallowed)" : rows[0].Price.ToString(CultureInfo.InvariantCulture))}");
    var threw = false;
    try { new PriceParser().Parse(["garbage"]); } catch (FormatException) { threw = true; }
    Check("S3 throws on malformed line instead of skipping silently", threw, "bad lines are swallowed");
}
finally { CultureInfo.CurrentCulture = original; }

// S4: thread safety (1000 parallel gets on a fresh key)
var calls = 0;
var cache = new TokenCache(async id => { Interlocked.Increment(ref calls); await Task.Delay(5); return "tok-" + id; });
var crashed = false;
try
{
    await Task.WhenAll(Enumerable.Range(0, 1000).Select(i => Task.Run(() => cache.GetTokenAsync("client" + (i % 50)))));
}
catch { crashed = true; }
Check("S4 survives 1000 parallel requests", !crashed && cache.Count == 50, crashed ? "dictionary corrupted/exception" : $"count={cache.Count}");

// S5: off-by-one
var items = Enumerable.Range(1, 10).ToList();
List<int>? page1 = null;
try { page1 = Paginator.GetPage(items, 1, 3); } catch (ArgumentOutOfRangeException) { }
Check("S5 page 1 of size 3 is [1,2,3]", page1 is [1, 2, 3], $"got [{(page1 is null ? "exception" : string.Join(",", page1))}]");
Check("S5 PageCount(10,3) == 4", Paginator.PageCount(10, 3) == 4, $"got {Paginator.PageCount(10, 3)}");

// S6: SQL injection
var q = new CustomerSearch().BuildQuery("x' OR '1'='1");
Check("S6 query does not embed raw user input", !q.Contains("'1'='1"), "user input concatenated into SQL");

// S7: secrets
var settingsType = typeof(AppSettings);
var defaultKey = (string?)settingsType.GetProperty("ApiKey")?.GetValue(new AppSettings());
Check("S7 no hardcoded API key default", string.IsNullOrEmpty(defaultKey), "default ApiKey looks like a real secret");

Console.WriteLine();
Console.WriteLine(failures == 0 ? "ALL CHECKS PASSED" : $"{failures} check(s) failed — review the snippets (S2 has no runtime check: review it by reading).");
