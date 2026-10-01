// =====================================================================
// Day1.Demo.Exceptions — חריגות, ניקוי משאבים ודיבוג
// מה הדמו מראה:
//   * try / catch / finally, סדר catch (ספציפי לפני כללי)
//   * exception filters (when), חריגה מותאמת אישית עם נתונים
//   * throw; מול throw ex; (שמירה על ה-stack trace)
//   * using / IDisposable (using declaration)
//   * guard clauses: ArgumentException.ThrowIfNullOrEmpty, ArgumentOutOfRangeException.ThrowIfNegative
//   * TryParse pattern במקום חריגות לזרימה רגילה
//   * Debug.Assert ו-Debug.WriteLine (רצים רק ב-Debug)
// הרצה:  dotnet run   (או dotnet run -c Release כדי לראות ש-Debug.Assert נעלם)
// =====================================================================
using System.Diagnostics;

Console.WriteLine("=== 1. try / catch / finally ===");
try
{
    int[] arr = [1, 2, 3];
    Console.WriteLine(arr[5]);
}
catch (IndexOutOfRangeException ex)
{
    Console.WriteLine($"caught specific: {ex.GetType().Name} — {ex.Message}");
}
catch (Exception ex)
{
    Console.WriteLine($"caught general: {ex.Message}");
}
finally
{
    Console.WriteLine("finally always runs (cleanup)");
}

Console.WriteLine("\n=== 2. Custom exception + exception filter ===");
var account = new Account("IL-1", 100);
foreach (var amount in new[] { 30m, 500m, -5m })
{
    try
    {
        account.Withdraw(amount);
        Console.WriteLine($"withdrew {amount}, balance={account.Balance}");
    }
    catch (InsufficientFundsException ex) when (ex.Shortfall > 100)
    {
        Console.WriteLine($"filter matched (big shortfall {ex.Shortfall}): {ex.Message}");
    }
    catch (InsufficientFundsException ex)
    {
        Console.WriteLine($"small shortfall {ex.Shortfall}: {ex.Message}");
    }
    catch (ArgumentOutOfRangeException ex)
    {
        Console.WriteLine($"bad argument '{ex.ParamName}': {ex.Message}");
    }
}

Console.WriteLine("\n=== 3. throw; vs throw ex; ===");
try { Level1(rethrowCorrectly: true); }
catch (Exception ex) { Console.WriteLine("throw;    → stack keeps origin: " + FirstFrame(ex)); }
try { Level1(rethrowCorrectly: false); }
catch (Exception ex) { Console.WriteLine("throw ex; → stack starts at rethrow: " + FirstFrame(ex)); }

Console.WriteLine("\n=== 4. using / IDisposable ===");
using (var r1 = new Resource("A"))
{
    Console.WriteLine("  working with A");
}   // Dispose נקרא כאן — גם אם הייתה חריגה
{
    using var r2 = new Resource("B");   // using declaration — Dispose בסוף הבלוק
    Console.WriteLine("  working with B");
}
try
{
    using var r3 = new Resource("C");
    throw new InvalidOperationException("boom inside using");
}
catch (InvalidOperationException ex) { Console.WriteLine($"  caught after C disposed: {ex.Message}"); }

Console.WriteLine("\n=== 5. guard clauses ===");
foreach (var (name, qty) in new[] { ("pen", 3), ("", 3), ("ink", -1) })
{
    try { Console.WriteLine($"  {Describe(name, qty)}"); }
    catch (ArgumentException ex) { Console.WriteLine($"  rejected: {ex.GetType().Name} ({ex.ParamName}): {ex.Message.Split(Environment.NewLine)[0]}"); }
}

Console.WriteLine("\n=== 6. TryParse instead of exceptions ===");
foreach (var input in new[] { "42", "abc", "3.5", "" })
{
    if (int.TryParse(input, out int value)) Console.WriteLine($"  '{input}' → {value}");
    else Console.WriteLine($"  '{input}' is not a valid int (no exception thrown)");
}
Console.WriteLine($"  Parse('abc') would throw: {SafeParseMessage("abc")}");

Console.WriteLine("\n=== 7. Debug.Assert / Debug.WriteLine ===");
int total = Sum([1, 2, 3]);
Debug.Assert(total == 6, "Sum is wrong!");   // ב-Release השורה נמחקת
Debug.WriteLine("this line appears only in the Debug output window");
Console.WriteLine($"  total={total} (Assert passed in Debug; Assert compiled away in Release)");
Console.WriteLine("\nDone.");

// ------------------------------------------------------------------
static void Level1(bool rethrowCorrectly)
{
    try { Level2(); }
    catch (Exception ex)
    {
        if (rethrowCorrectly) throw;   // שומר את ה-stack trace המקורי
#pragma warning disable CA2200 // מדגימים בכוונה את הטעות
        else throw ex;                 // מוחק! ה-stack מתחיל מכאן (CA2200)
#pragma warning restore CA2200
    }
}
static void Level2() => throw new InvalidOperationException("origin is Level2");
static string FirstFrame(Exception ex)
{
    var line = ex.StackTrace?.Split('\n').FirstOrDefault()?.Trim() ?? "";
    return line.Contains("Level2") ? "Level2 (good)" : "Level1 (origin lost)";
}

static string Describe(string name, int quantity)
{
    ArgumentException.ThrowIfNullOrEmpty(name);
    ArgumentOutOfRangeException.ThrowIfNegative(quantity);
    return $"{quantity} x {name}";
}

static string SafeParseMessage(string s)
{
    try { int.Parse(s); return "no exception"; }
    catch (FormatException ex) { return ex.Message; }
}

static int Sum(int[] values) { int s = 0; foreach (var v in values) s += v; return s; }

class InsufficientFundsException : Exception
{
    public decimal Shortfall { get; }
    public InsufficientFundsException(decimal shortfall)
        : base($"Insufficient funds: short by {shortfall:N2}")
    {
        Shortfall = shortfall;
    }
}

class Account(string id, decimal balance)
{
    public string Id { get; } = id;
    public decimal Balance { get; private set; } = balance;

    public void Withdraw(decimal amount)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amount);
        if (amount > Balance) throw new InsufficientFundsException(amount - Balance);
        Balance -= amount;
    }
}

class Resource(string name) : IDisposable
{
    public void Dispose() => Console.WriteLine($"  Dispose({name})");
}
