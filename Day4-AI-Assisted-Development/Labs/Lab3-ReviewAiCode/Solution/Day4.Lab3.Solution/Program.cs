// Day4.Lab3.Solution — אותן בדיקות עצמיות כמו ב-Starter, מותאמות לחתימות המתוקנות. כולן עוברות.
using System.Globalization;
using Day4.Lab3.Snippets;

var failures = 0;
void Check(string name, bool ok, string hint)
{
    Console.WriteLine($"{(ok ? "PASS" : "FAIL")}  {name}{(ok ? "" : "  -> " + hint)}");
    if (!ok) failures++;
}

// S1
var root = Path.Combine(Path.GetTempPath(), "lab3-exports-" + Guid.NewGuid().ToString("N"));
try
{
    new FileExport(root).Save(Path.Combine("..", "escaped.txt"), "x");
    Check("S1 rejects '..' file names", false, "wrote outside root");
}
catch (Exception ex) when (ex is UnauthorizedAccessException or ArgumentException)
{
    Check("S1 rejects '..' file names", true, "");
}
var ok1 = new FileExport(root).Save("report.txt", "hello");
Check("S1 saves a plain file name inside root", File.Exists(ok1) && ok1.StartsWith(Path.GetFullPath(root), StringComparison.Ordinal), ok1);
try { Directory.Delete(root, true); } catch { }

// S3
var original = CultureInfo.CurrentCulture;
CultureInfo.CurrentCulture = new CultureInfo("de-DE");
try
{
    var parser = new PriceParser(TimeProvider.System);
    Check("S3 IsValidNow uses injected clock", parser.IsValidNow(new PriceRow("A", 1m, new DateOnly(2000, 1, 1))), "clock");
    var rows = PriceParser.Parse(["A-1;19.90;2025-03-01"]);
    Check("S3 parses 19.90 as 19.90 under de-DE", rows.Count == 1 && rows[0].Price == 19.90m, "culture-dependent parse");
    var threw = false;
    try { PriceParser.Parse(["garbage"]); } catch (FormatException) { threw = true; }
    Check("S3 throws on malformed line instead of skipping silently", threw, "swallowed");
}
finally { CultureInfo.CurrentCulture = original; }

// S4
var calls = 0;
var cache = new TokenCache(async id => { Interlocked.Increment(ref calls); await Task.Delay(5); return "tok-" + id; }, TimeProvider.System);
var crashed = false;
try
{
    await Task.WhenAll(Enumerable.Range(0, 1000).Select(i => Task.Run(() => cache.GetTokenAsync("client" + (i % 50)))));
}
catch { crashed = true; }
Check("S4 survives 1000 parallel requests", !crashed && cache.Count == 50, crashed ? "exception" : $"count={cache.Count}");
Check("S4 fetches each key once under contention", calls == 50, $"fetch calls = {calls}");

// S5
var items = Enumerable.Range(1, 10).ToList();
Check("S5 page 1 of size 3 is [1,2,3]", Paginator.GetPage(items, 1, 3) is [1, 2, 3], "off-by-one");
Check("S5 last page is short [10]", Paginator.GetPage(items, 4, 3) is [10], "index out of range");
Check("S5 PageCount(10,3) == 4", Paginator.PageCount(10, 3) == 4, $"got {Paginator.PageCount(10, 3)}");

// S6
var q = CustomerSearch.BuildQuery("x' OR '1'='1");
Check("S6 query does not embed raw user input", !q.Sql.Contains("'1'='1") && q.Parameters.ContainsKey("@pattern"), "concatenated SQL");

// S7
Check("S7 no ApiKey property with a default secret", typeof(AppSettings).GetProperty("ApiKey") is null, "hardcoded secret");

Console.WriteLine();
Console.WriteLine(failures == 0 ? "ALL CHECKS PASSED" : $"{failures} check(s) failed");
return failures == 0 ? 0 : 1;
