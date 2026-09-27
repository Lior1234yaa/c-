# Cheat-Sheet — C#, .NET CLI ו-Visual Studio

דף עזר מרוכז לקורס. מומלץ להדפיס (2 עמודים) או להשאיר פתוח בלשונית.

## 1. תחביר C# — הבסיס

| נושא | דוגמה |
|------|-------|
| משתנה עם הסקת טיפוס | `var count = 5;` |
| משתנה עם טיפוס מפורש | `int count = 5;` |
| קבוע | `const double Pi = 3.14;` |
| null-able | `string? name = null;` |
| בדיקת null | `if (name is null) ...` / `name ?? "default"` / `name?.Length` |
| תנאי | `if (x > 0) { } else if (x < 0) { } else { }` |
| ביטוי תנאי | `var sign = x >= 0 ? "+" : "-";` |
| לולאה | `for (int i = 0; i < 10; i++) { }` |
| foreach | `foreach (var item in items) { }` |
| while | `while (cond) { }` / `do { } while (cond);` |
| switch (ביטוי) | `var s = x switch { 0 => "zero", > 0 => "pos", _ => "neg" };` |
| pattern matching | `if (shape is Circle { Radius: > 10 } c) ...` |
| מתודה | `int Add(int a, int b) => a + b;` |
| מחלקה | `public class Person { public string Name { get; set; } = ""; }` |
| record | `public record Point(int X, int Y);` |
| ירושה / מימוש ממשק | `class Dog : Animal, IComparable<Dog>` |
| ממשק | `public interface IShape { double Area(); }` |
| מאפיין לקריאה בלבד | `public int Id { get; init; }` |
| enum | `enum Color { Red, Green, Blue }` |
| חריגה | `throw new ArgumentException("msg", nameof(arg));` |
| try/catch/finally | `try { } catch (IOException ex) when (ex.HResult == 1) { } finally { }` |
| using (שחרור משאב) | `using var file = File.OpenRead(path);` |
| namespace (file-scoped) | `namespace MyApp.Models;` |
| lambda | `x => x * 2` / `(a, b) => a + b` |
| tuple | `(int min, int max) = GetRange();` |
| generics | `T Max<T>(T a, T b) where T : IComparable<T>` |

## 2. טיפוסים נפוצים

| טיפוס | תיאור | ערך ברירת מחדל |
|-------|-------|----------------|
| `int`, `long` | שלמים 32/64 ביט | `0` |
| `double`, `decimal` | נקודה צפה / כספי (מדויק) | `0` |
| `bool` | `true` / `false` | `false` |
| `char` | תו יחיד `'a'` | `'\0'` |
| `string` | מחרוזת (immutable) | `null` |
| `DateTime`, `DateOnly`, `TimeSpan` | תאריך/שעה, תאריך בלבד, משך | `default` |
| `Guid` | מזהה ייחודי `Guid.NewGuid()` | `Guid.Empty` |
| `object` | הבסיס של הכול | `null` |
| `int[]`, `List<int>` | מערך קבוע / רשימה דינמית | `null` |

המרות: `int.Parse("42")`, `int.TryParse(s, out var n)`, `(int)3.7` (חיתוך), `Convert.ToInt32(x)`, `x.ToString()`.

## 3. פורמט מחרוזות

```csharp
var name = "דנה"; var price = 1234.5; var when = DateTime.Now;
Console.WriteLine($"שלום {name}, המחיר {price:C}");        // interpolation + מטבע
Console.WriteLine($"{price:N2} | {price:F0} | {0.256:P1}");  // 1,234.50 | 1235 | 25.6%
Console.WriteLine($"{when:yyyy-MM-dd HH:mm} | {when:d}");    // תאריך מותאם | קצר
Console.WriteLine($"{name,10}|{price,-10}|");                // ריווח (ימין / שמאל)
Console.WriteLine($"{42:D5} | {255:X} | {1_000_000:N0}");   // 00042 | FF | 1,000,000
var multi = """
    טקסט רב-שורתי (raw string literal)
    בלי צורך ב-escape של "מרכאות"
    """;
```

שימושי: `string.Join(", ", list)`, `s.Split(',')`, `s.Trim()`, `s.ToUpper()`, `s.Contains("x")`, `s.StartsWith("a")`, `string.IsNullOrWhiteSpace(s)`, `s.Replace("a", "b")`, `s[..3]` / `s[^2..]` (ranges), `StringBuilder` לחיבורים רבים.

## 4. אוספים (Collections)

| טיפוס | מתי | פעולות עיקריות |
|-------|-----|----------------|
| `T[]` | גודל קבוע, ביצועים | `arr[i]`, `arr.Length` |
| `List<T>` | רשימה גדלה — ברירת המחדל | `Add`, `Remove`, `Insert`, `Count`, `list[i]`, `Contains`, `Sort` |
| `Dictionary<K,V>` | חיפוש לפי מפתח O(1) | `dict[key] = v`, `TryGetValue`, `ContainsKey`, `Keys`, `Values` |
| `HashSet<T>` | ייחודיות, בדיקת חברות מהירה | `Add` (מחזיר bool), `Contains`, `UnionWith` |
| `Queue<T>` | FIFO | `Enqueue`, `Dequeue`, `Peek` |
| `Stack<T>` | LIFO | `Push`, `Pop`, `Peek` |
| `LinkedList<T>` | הוספה/הסרה באמצע | `AddFirst`, `AddLast`, `Remove` |
| `SortedDictionary<K,V>` / `SortedSet<T>` | ממוין תמיד | כמו Dictionary/HashSet |
| `IEnumerable<T>` | "משהו שאפשר לעבור עליו" — לפרמטרים | `foreach`, LINQ |
| `IReadOnlyList<T>` | חשיפה בלי לאפשר שינוי | `Count`, אינדקס |
| `ConcurrentDictionary<K,V>`, `ConcurrentQueue<T>` | multi-threading (יום 2) | `TryAdd`, `AddOrUpdate`, `TryDequeue` |

אתחול מקוצר: `List<int> nums = [1, 2, 3];` (collection expression), `var dict = new Dictionary<string, int> { ["a"] = 1 };`.

## 5. LINQ — 15 המתודות החשובות

```csharp
var people = new List<Person> { /* ... */ };
people.Where(p => p.Age >= 18)                       // סינון
      .Select(p => p.Name)                           // המרה/הקרנה
      .OrderBy(p => p.Age).ThenByDescending(p => p.Name)
      .ToList();                                     // materialize (גם ToArray / ToDictionary)
people.First(p => p.Id == 7);                        // הראשון — חריגה אם אין
people.FirstOrDefault(p => p.Id == 7);               // הראשון — null אם אין
people.Single(p => p.Id == 7);                       // בדיוק אחד
people.Any(p => p.Age > 90);  people.All(p => p.Age > 0);
people.Count(p => p.City == "TLV");
people.Sum(p => p.Salary);  people.Average(p => p.Age);  people.Max(p => p.Age);
people.GroupBy(p => p.City).Select(g => new { g.Key, Count = g.Count() });
people.Skip(10).Take(5);                             // עימוד
people.Select(p => p.City).Distinct();
people.SelectMany(p => p.Phones);                    // שיטוח רשימות מקוננות
people.Zip(scores, (p, s) => (p.Name, s));           // איחוד "צד לצד"
people.Chunk(100);                                   // חלוקה לקבוצות
```

זכרו: LINQ הוא **lazy** — השאילתה רצה רק כשעוברים על התוצאה (`foreach`, `ToList`, `Count`...). אל תעברו על אותה שאילתה פעמיים בלי `ToList()`.

## 6. async / await — תבניות

```csharp
// חתימה: Task (בלי ערך) / Task<T> (עם ערך); שם המתודה מסתיים ב-Async
async Task<string> DownloadAsync(HttpClient http, string url, CancellationToken ct = default)
{
    var response = await http.GetAsync(url, ct);
    response.EnsureSuccessStatusCode();
    return await response.Content.ReadAsStringAsync(ct);
}

// מקביליות: להתחיל הכול, לחכות לכולם
var tasks = urls.Select(u => DownloadAsync(http, u));
string[] pages = await Task.WhenAll(tasks);

// ביטול עם timeout
using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
try { await DownloadAsync(http, url, cts.Token); }
catch (OperationCanceledException) { Console.WriteLine("timeout"); }

// JSON (System.Text.Json)
var user = await http.GetFromJsonAsync<User>(url);          // System.Net.Http.Json
var json = JsonSerializer.Serialize(user, new JsonSerializerOptions { WriteIndented = true });
var back = JsonSerializer.Deserialize<User>(json);

// עבודה כבדה (CPU) — לא לחסום את ה-UI
var result = await Task.Run(() => HeavyCompute(data));
```

כללי אצבע: `await` ולא `.Result` / `.Wait()` (deadlock!); `async void` רק ב-event handlers; העבירו `CancellationToken` הלאה; ב-WPF חזרה ל-UI thread קורית אוטומטית אחרי `await`.

## 7. .NET CLI

| פקודה | מה עושה |
|-------|---------|
| `dotnet new list` | רשימת תבניות |
| `dotnet new console -n App` | פרויקט קונסולה |
| `dotnet new classlib -n Lib` / `dotnet new xunit -n App.Tests` | ספרייה / בדיקות |
| `dotnet new wpf -n App` | פרויקט WPF (Windows) |
| `dotnet run` / `dotnet run -- a b` | הרצה (עם ארגומנטים) |
| `dotnet build` / `dotnet build -c Release` | בנייה |
| `dotnet test` | הרצת בדיקות |
| `dotnet watch run` | הרצה מחדש אוטומטית בכל שינוי |
| `dotnet add package X` / `dotnet add reference ../Lib/Lib.csproj` | תלויות |
| `dotnet format` | עיצוב קוד |
| `dotnet --list-sdks` / `dotnet --info` | מידע על ההתקנה |

## 8. קיצורי מקלדת — Visual Studio ו-VS Code

| פעולה | Visual Studio | VS Code |
|-------|---------------|---------|
| הרצה עם debugger | `F5` | `F5` |
| הרצה בלי debugger | `Ctrl+F5` | `Ctrl+F5` |
| breakpoint (הוספה/הסרה) | `F9` | `F9` |
| Step Over (שורה הבאה) | `F10` | `F10` |
| Step Into (להיכנס למתודה) | `F11` | `F11` |
| Step Out (לצאת מהמתודה) | `Shift+F11` | `Shift+F11` |
| המשך ריצה | `F5` | `F5` |
| Quick Actions / תיקון מהיר (using חסר, refactoring) | `Ctrl+.` | `Ctrl+.` |
| עיצוב מסמך | `Ctrl+K, Ctrl+D` | `Shift+Alt+F` |
| חיפוש קובץ / סימבול בכל הפרויקט | `Ctrl+T` | `Ctrl+P` (קובץ), `Ctrl+T` (סימבול) |
| בנייה | `Ctrl+Shift+B` | `Ctrl+Shift+B` (task) |
| מעבר להגדרה | `F12` | `F12` |
| מציאת כל השימושים | `Shift+F12` | `Shift+F12` |
| שינוי שם (Rename) | `Ctrl+R, Ctrl+R` / `F2` | `F2` |
| הערה / ביטול הערה | `Ctrl+K, Ctrl+C` / `Ctrl+K, Ctrl+U` | `Ctrl+/` |
| חיפוש בכל הקבצים | `Ctrl+Shift+F` | `Ctrl+Shift+F` |
| פלטת פקודות | `Ctrl+Q` (חיפוש) | `Ctrl+Shift+P` |
| טרמינל | Ctrl + ` (גרש הפוך; Developer PowerShell) | Ctrl + ` (גרש הפוך) |
| Solution Explorer / סייר | `Ctrl+Alt+L` | `Ctrl+Shift+E` |
| Copilot Chat | `Ctrl+\, C` | `Ctrl+Alt+I` |
| הצגת IntelliSense | `Ctrl+Space` | `Ctrl+Space` |

ב-macOS החליפו `Ctrl` ב-`Cmd` ברוב הקיצורים של VS Code.

קיצורי קוד (snippets) ב-Visual Studio — הקלידו ואז `Tab` פעמיים: `cw` (Console.WriteLine), `prop` (מאפיין), `ctor` (בנאי), `for`, `foreach`, `try`, `if`, `class`.
