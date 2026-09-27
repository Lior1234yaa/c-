// Day4.Exercises.Solutions — פתרונות לתרגילי הקוד של יום 4.
// הרצה: dotnet run -- <מספר תרגיל>   (2, 6, 7, 8, 9, 10, 11)
// תרגילי ה-prompt (1, 3, 4, 5, 12) — ראו PROMPTS-ANSWERS.md
using Day4.Exercises.Solutions;

var exercises = new Dictionary<int, (string Title, Func<Task> Run)>
{
    [2] = ("API מומצא → APIs אמיתיים", () => { Ex02_FakeApi.Run(); return Task.CompletedTask; }),
    [6] = ("Records מ-JSON", () => { Ex06_JsonRecords.Run(); return Task.CompletedTask; }),
    [7] = ("Regex עם עוגנים", () => { Ex07_Regex.Run(); return Task.CompletedTask; }),
    [8] = ("async void ו-.Result", Ex08_Async.RunAsync),
    [9] = ("תרבות וזמן", () => { Ex09_Culture.Run(); return Task.CompletedTask; }),
    [10] = ("HttpClient, thread-safety, חריגות", Ex10_Resources.RunAsync),
    [11] = ("הזרקת תלויות", () => { Ex11_Di.Run(); return Task.CompletedTask; }),
};

int? choice = args.Length > 0 && int.TryParse(args[0], out var n) ? n : null;

if (choice is null)
{
    Console.WriteLine("תרגילי יום 4 — בחרו מספר:");
    foreach (var (k, v) in exercises.OrderBy(kv => kv.Key)) Console.WriteLine($"  {k,2}. {v.Title}");
    Console.Write("> ");
    var line = Console.ReadLine();          // EOF → null → יציאה מסודרת
    if (line is null || !int.TryParse(line, out var m)) return;
    choice = m;
}

if (!exercises.TryGetValue(choice.Value, out var ex))
{
    Console.WriteLine($"אין תרגיל קוד מספר {choice}. תרגילי prompt נמצאים ב-PROMPTS-ANSWERS.md");
    return;
}

Console.WriteLine($"=== תרגיל {choice}: {ex.Title} ===");
await ex.Run();
