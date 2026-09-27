// Day2.Exercises.Solutions — פתרונות לתרגילי יום 2
// הרצה: dotnet run -- <מספר>   או  dotnet run  (תפריט)
Console.OutputEncoding = System.Text.Encoding.UTF8;

var exercises = new (int Number, string Title, Func<Task> Run)[]
{
    (1,  "Two threads",                    Ex01.RunAsync),
    (2,  "Race condition + fixes",         Ex02.RunAsync),
    (3,  "Parallel.For primes",            Ex03.RunAsync),
    (4,  "Task.Delay + WhenAll",           Ex04.RunAsync),
    (5,  "WhenAny + timeout",              Ex05.RunAsync),
    (6,  "Cancellation + progress",        Ex06.RunAsync),
    (7,  "IAsyncEnumerable",               Ex07.RunAsync),
    (8,  "SemaphoreSlim",                  Ex08.RunAsync),
    (9,  "ConcurrentDictionary",           Ex09.RunAsync),
    (10, "Channel producer/consumer",      Ex10.RunAsync),
    (11, "HTTP GET + DTO (needs LocalApi)",Ex11.RunAsync),
    (12, "POST/PUT/DELETE (needs LocalApi)", Ex12.RunAsync),
    (13, "Retry + timeout (needs LocalApi)", Ex13.RunAsync),
    (14, "JSON manual",                    Ex14.RunAsync),
    (15, "Sync-over-async + ThreadPool",   Ex15.RunAsync),
};

if (args.Length > 0 && int.TryParse(args[0], out var n))
{
    await RunOne(n);
    return;
}

while (true)
{
    Console.WriteLine("\n=== Day 2 exercises ===");
    foreach (var (num, title, _) in exercises) Console.WriteLine($"  {num,2}. {title}");
    Console.Write("choose (0 = exit): ");
    var line = Console.ReadLine();
    if (line is null || line.Trim() == "0") break;           // EOF או 0 -> יציאה
    if (int.TryParse(line, out var choice)) await RunOne(choice);
}

async Task RunOne(int number)
{
    var ex = exercises.FirstOrDefault(e => e.Number == number);
    if (ex.Run is null) { Console.WriteLine($"no exercise {number}"); return; }
    Console.WriteLine($"\n--- Exercise {ex.Number}: {ex.Title} ---");
    try { await ex.Run(); }
    catch (Exception e) { Console.WriteLine($"exercise failed: {e.GetType().Name}: {e.Message}"); }
}
