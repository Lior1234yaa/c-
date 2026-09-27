// Setup.VerifySetup — בדיקת סביבת העבודה לקורס C# / .NET
// הרצה: dotnet run
//
// התוכנית בודקת שהמחשב מוכן לקורס: גרסת .NET, מערכת הפעלה, SDK מותקן,
// גישה לאינטרנט (NuGet), ושתי בדיקות שפיות קטנות (JSON ו-async).

using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

// ב-Windows חלון הקונסולה לא תמיד מציג עברית/אמוג'י כברירת מחדל
try { Console.OutputEncoding = System.Text.Encoding.UTF8; } catch { /* לא קריטי */ }

var results = new List<CheckResult>();

Console.WriteLine("=====================================================");
Console.WriteLine("  בדיקת סביבת עבודה — קורס C# / .NET (4 ימים)");
Console.WriteLine("=====================================================");
Console.WriteLine();

// ---------- 1. גרסת .NET ----------
Console.WriteLine("[1] גרסת .NET");
Console.WriteLine($"    Environment.Version      : {Environment.Version}");
Console.WriteLine($"    FrameworkDescription     : {RuntimeInformation.FrameworkDescription}");
Console.WriteLine($"    RuntimeIdentifier        : {RuntimeInformation.RuntimeIdentifier}");
bool runtimeOk = Environment.Version.Major >= 10;
results.Add(new("Runtime של .NET 10 ומעלה", runtimeOk,
    runtimeOk ? $"רץ על .NET {Environment.Version.Major}" : $"נמצאה גרסה {Environment.Version} — נדרשת 10 ומעלה"));
Console.WriteLine();

// ---------- 2. מערכת הפעלה ----------
Console.WriteLine("[2] מערכת הפעלה");
Console.WriteLine($"    OSDescription            : {RuntimeInformation.OSDescription}");
Console.WriteLine($"    OSArchitecture           : {RuntimeInformation.OSArchitecture}");
bool isWindows = OperatingSystem.IsWindows();
Console.WriteLine($"    Windows?                 : {(isWindows ? "כן" : "לא")}");
results.Add(new("מערכת הפעלה מזוהה", true, RuntimeInformation.OSDescription));
results.Add(new("Windows (נדרש ל-WPF ביום 3)", isWindows,
    isWindows
        ? "אפשר להריץ WPF ו-WinForms"
        : "ימים 1, 2 ו-4 יעבדו; ליום 3 (WPF) צריך Windows או מכונה וירטואלית",
    Mandatory: false));
Console.WriteLine();

// ---------- 3. dotnet SDK מותקן ----------
Console.WriteLine("[3] dotnet SDK (dotnet --list-sdks)");
var (sdkOk, sdkDetail) = CheckDotnetSdk();
Console.WriteLine($"    {sdkDetail}");
results.Add(new("dotnet SDK 10.x מותקן", sdkOk, sdkDetail));
Console.WriteLine();

// ---------- 4. גישה לאינטרנט (NuGet) ----------
Console.WriteLine("[4] גישה ל-api.nuget.org (timeout של 5 שניות)");
var (netOk, netDetail) = await CheckNuGetAsync();
Console.WriteLine($"    {netDetail}");
results.Add(new("גישה ל-NuGet (להורדת חבילות)", netOk, netDetail, Mandatory: false));
Console.WriteLine();

// ---------- 5. System.Text.Json ----------
Console.WriteLine("[5] בדיקת System.Text.Json (serialize -> deserialize)");
var (jsonOk, jsonDetail) = CheckJsonRoundTrip();
Console.WriteLine($"    {jsonDetail}");
results.Add(new("System.Text.Json עובד", jsonOk, jsonDetail));
Console.WriteLine();

// ---------- 6. async/await ----------
Console.WriteLine("[6] בדיקת async/await (Task.WhenAll)");
var (asyncOk, asyncDetail) = await CheckAsyncAsync();
Console.WriteLine($"    {asyncDetail}");
results.Add(new("async/await עובד", asyncOk, asyncDetail));
Console.WriteLine();

// ---------- סיכום ----------
Console.WriteLine("=====================================================");
Console.WriteLine("  סיכום");
Console.WriteLine("=====================================================");
foreach (var r in results)
{
    string icon = r.Passed ? "✅" : r.Mandatory ? "❌" : "⚠️";
    Console.WriteLine($"  {icon} {r.Name}");
    if (!r.Passed)
        Console.WriteLine($"       -> {r.Detail}");
}
Console.WriteLine();

// בדיקות "חובה": runtime + SDK + JSON + async. אינטרנט ו-Windows הן אזהרות בלבד.
bool[] mandatory = [runtimeOk, sdkOk, jsonOk, asyncOk];
int failedMandatory = mandatory.Count(ok => !ok);

if (failedMandatory == 0 && netOk && isWindows)
{
    Console.WriteLine("✅ הכול מוכן! הסביבה שלך מתאימה לכל ארבעת ימי הקורס.");
}
else if (failedMandatory == 0)
{
    Console.WriteLine("✅ הבסיס תקין. שימו לב לאזהרות למעלה:");
    if (!netOk) Console.WriteLine("   - בלי גישה ל-NuGet לא תוכלו להוסיף חבילות (ראו INSTALL.md, סעיף פתרון בעיות).");
    if (!isWindows) Console.WriteLine("   - ביום 3 (WPF) נדרש Windows.");
}
else
{
    Console.WriteLine($"❌ {failedMandatory} בדיקות חובה נכשלו. עברו על INSTALL.md ותקנו לפני תחילת הקורס.");
}

return failedMandatory == 0 ? 0 : 1;

// =====================================================================
// פונקציות עזר
// =====================================================================

static (bool ok, string detail) CheckDotnetSdk()
{
    try
    {
        var psi = new ProcessStartInfo("dotnet", "--list-sdks")
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };
        using var process = Process.Start(psi);
        if (process is null)
            return (false, "לא הצלחתי להריץ את הפקודה dotnet — האם היא ב-PATH?");

        string output = process.StandardOutput.ReadToEnd();
        if (!process.WaitForExit(10_000))
        {
            try { process.Kill(); } catch { }
            return (false, "הפקודה dotnet --list-sdks לא הסתיימה בזמן");
        }

        var sdks = output
            .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToList();

        if (sdks.Count == 0)
            return (false, "לא נמצאו SDKs מותקנים (רק Runtime?). התקינו .NET 10 SDK.");

        bool has10 = sdks.Any(s => s.StartsWith("10.", StringComparison.Ordinal));
        string list = string.Join(", ", sdks.Select(s => s.Split(' ')[0]));
        return has10
            ? (true, $"נמצא SDK 10 (מותקנים: {list})")
            : (false, $"לא נמצא SDK 10 (מותקנים: {list})");
    }
    catch (Exception ex)
    {
        return (false, $"שגיאה בהרצת dotnet: {ex.GetType().Name} — {ex.Message}");
    }
}

static async Task<(bool ok, string detail)> CheckNuGetAsync()
{
    try
    {
        using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };
        var sw = Stopwatch.StartNew();
        using var response = await http.GetAsync("https://api.nuget.org/v3/index.json");
        sw.Stop();
        return response.IsSuccessStatusCode
            ? (true, $"HTTP {(int)response.StatusCode} תוך {sw.ElapsedMilliseconds} ms")
            : (false, $"התקבלה תשובה לא צפויה: HTTP {(int)response.StatusCode}");
    }
    catch (TaskCanceledException)
    {
        return (false, "timeout — אין תשובה תוך 5 שניות (אין אינטרנט? proxy ארגוני?)");
    }
    catch (HttpRequestException ex)
    {
        return (false, $"שגיאת רשת: {ex.Message}");
    }
    catch (Exception ex)
    {
        return (false, $"שגיאה לא צפויה: {ex.GetType().Name} — {ex.Message}");
    }
}

static (bool ok, string detail) CheckJsonRoundTrip()
{
    try
    {
        var original = new Student("דנה", 3, ["OOP", "Async", "WPF", "AI"]);
        var options = new JsonSerializerOptions { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };
        string json = JsonSerializer.Serialize(original, options);
        var copy = JsonSerializer.Deserialize<Student>(json, options);

        bool ok = copy is not null
                  && copy.Name == original.Name
                  && copy.Day == original.Day
                  && copy.Topics.SequenceEqual(original.Topics);

        return ok
            ? (true, $"round-trip תקין: {json}")
            : (false, "האובייקט אחרי deserialize שונה מהמקור");
    }
    catch (Exception ex)
    {
        return (false, $"שגיאה: {ex.GetType().Name} — {ex.Message}");
    }
}

static async Task<(bool ok, string detail)> CheckAsyncAsync()
{
    try
    {
        var sw = Stopwatch.StartNew();
        // שלוש משימות "מקביליות" של 100ms — אמורות להסתיים יחד בסביבות 100ms ולא 300ms
        var tasks = Enumerable.Range(1, 3)
            .Select(async i => { await Task.Delay(100); return i * 10; });
        int[] values = await Task.WhenAll(tasks);
        sw.Stop();

        int sum = values.Sum();
        bool ok = sum == 60 && sw.ElapsedMilliseconds < 1000;
        return ok
            ? (true, $"3 משימות הסתיימו במקביל תוך {sw.ElapsedMilliseconds} ms (סכום = {sum})")
            : (false, $"תוצאה לא צפויה: סכום = {sum}, זמן = {sw.ElapsedMilliseconds} ms");
    }
    catch (Exception ex)
    {
        return (false, $"שגיאה: {ex.GetType().Name} — {ex.Message}");
    }
}

// =====================================================================
// טיפוסים
// =====================================================================

record CheckResult(string Name, bool Passed, string Detail, bool Mandatory = true);

record Student(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("day")] int Day,
    [property: JsonPropertyName("topics")] string[] Topics);
