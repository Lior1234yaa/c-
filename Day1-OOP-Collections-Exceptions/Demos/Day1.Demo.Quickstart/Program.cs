// =====================================================================
// Day1.Demo.Quickstart — חימום C#
// מה הדמו מראה:
//   * top-level statements (אין Main מפורש)
//   * טיפוסי ערך מול טיפוסי הפניה (struct/int מול class/array)
//   * var, מחרוזות ואינטרפולציה, בקרת זרימה (if / switch expression / loops)
//   * מתודות עם פרמטרים אופציונליים, out ו-tuples
//   * nullable reference types (string?) והפעולות ?. ?? !
// הרצה:  dotnet run
// =====================================================================

Console.WriteLine("=== 1. משתנים, var ואינטרפולציה ===");
int age = 30;                    // טיפוס ערך
double price = 19.90;
string name = "Dana";            // טיפוס הפניה (אך immutable)
var isActive = true;             // המהדר מסיק bool
Console.WriteLine($"{name} is {age} years old, price={price:F2}, active={isActive}");
Console.WriteLine($"Rounded: {Math.Round(price)}  |  Upper: {name.ToUpper()}  |  Len: {name.Length}");

Console.WriteLine("\n=== 2. ערך מול הפניה ===");
int a = 5;
int b = a;          // העתקה של הערך
b++;
Console.WriteLine($"a={a}, b={b}   (int הוא value type — b לא השפיע על a)");

int[] arr1 = { 1, 2, 3 };
int[] arr2 = arr1;  // העתקה של ההפניה — שני השמות מצביעים לאותו מערך
arr2[0] = 99;
Console.WriteLine($"arr1[0]={arr1[0]}   (מערך הוא reference type — השינוי נראה דרך שתי ההפניות)");

Point p1 = new(1, 2);
Point p2 = p1;      // struct → העתקה
p2.X = 100;
Console.WriteLine($"p1.X={p1.X}, p2.X={p2.X}   (struct מועתק)");

Console.WriteLine("\n=== 3. בקרת זרימה ===");
for (int i = 1; i <= 3; i++) Console.Write($"{i} ");
Console.WriteLine();

string[] days = ["Sun", "Mon", "Tue"];
foreach (var d in days) Console.Write($"{d} ");
Console.WriteLine();

int n = 0;
while (n < 3) { n++; }
Console.WriteLine($"after while: n={n}");

string Grade(int score) => score switch
{
    >= 90 => "A",
    >= 80 => "B",
    >= 70 => "C",
    _ => "F",
};
Console.WriteLine($"Grade(85) = {Grade(85)}, Grade(50) = {Grade(50)}");

Console.WriteLine("\n=== 4. מתודות ===");
Console.WriteLine(Add(2, 3));
Console.WriteLine(Greet("Yossi"));
Console.WriteLine(Greet("Yossi", "שלום"));
if (TryDivide(10, 2, out double result)) Console.WriteLine($"10/2 = {result}");
if (!TryDivide(10, 0, out _)) Console.WriteLine("10/0 — חלוקה באפס לא מותרת");
var (min, max) = MinMax([4, 9, 1, 7]);
Console.WriteLine($"min={min}, max={max}");

Console.WriteLine("\n=== 5. Nullable reference types ===");
string? maybeName = FindUser(1);   // יכול להיות null
Console.WriteLine($"Length via ?. : {maybeName?.Length}");
Console.WriteLine($"With ?? : {maybeName ?? "(unknown)"}");
string? found = FindUser(0);
Console.WriteLine($"found = {found ?? "null"}");
if (found is not null)
    Console.WriteLine($"found has {found.Length} chars"); // המהדר יודע שכאן found אינו null

// ---------- מתודות עזר (local functions ברמת הקובץ) ----------
static int Add(int x, int y) => x + y;

static string Greet(string who, string greeting = "Hello") => $"{greeting}, {who}!";

static bool TryDivide(double x, double y, out double quotient)
{
    if (y == 0) { quotient = 0; return false; }
    quotient = x / y;
    return true;
}

static (int Min, int Max) MinMax(int[] values) => (values.Min(), values.Max());

static string? FindUser(int id) => id == 0 ? "admin" : null;

// struct — טיפוס ערך שהגדרנו בעצמנו
struct Point
{
    public int X;
    public int Y;
    public Point(int x, int y) { X = x; Y = y; }
}
