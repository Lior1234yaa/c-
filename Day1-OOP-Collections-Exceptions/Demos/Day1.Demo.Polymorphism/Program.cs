// =====================================================================
// Day1.Demo.Polymorphism — ירושה, פולימורפיזם, ממשקים והפשטה
// מה הדמו מראה:
//   * מחלקה אבסטרקטית Shape עם מתודה אבסטרקטית ומתודה virtual
//   * override / base / sealed
//   * ממשקים: IDescribable עם default member, מימוש מרובה
//   * פולימורפיזם: List<Shape> — כל אובייקט מתנהג לפי הטיפוס האמיתי שלו
//   * is / as / pattern matching (type pattern, property pattern, switch)
//   * Equals/GetHashCode/ToString ב-class
//   * composition over inheritance (Logger מוזרק במקום ירושה)
// הרצה:  dotnet run
// =====================================================================

Console.WriteLine("=== 1. פולימורפיזם דרך מחלקת בסיס אבסטרקטית ===");
List<Shape> shapes =
[
    new Circle(1.5),
    new Rectangle(3, 4),
    new Square(2),
];
foreach (var s in shapes)
    Console.WriteLine($"{s.Name,-10} area={s.Area():F2}  {s.Describe()}");

Console.WriteLine($"\nTotal area: {shapes.Sum(s => s.Area()):F2}");

Console.WriteLine("\n=== 2. ממשקים ו-default members ===");
IDescribable[] things = [new Circle(1), new Invoice(3, 99.9m)];
foreach (var t in things) Console.WriteLine(t.Describe());
IDescribable inv = new Invoice(1, 10m);
Console.WriteLine(inv.DescribeLoud());  // default member — נגיש רק דרך טיפוס הממשק

Console.WriteLine("\n=== 3. is / as / pattern matching ===");
object[] mixed = [new Circle(2), new Rectangle(1, 1), "hello", 42, null!];
foreach (var o in mixed)
{
    string msg = o switch
    {
        Circle { Radius: > 1.5 } c => $"big circle r={c.Radius}",
        Circle c                    => $"circle r={c.Radius}",
        Rectangle { Width: var w, Height: var h } when w == h => "rectangle that is actually a square",
        Rectangle r                 => $"rectangle {r.Width}x{r.Height}",
        string str                  => $"string of length {str.Length}",
        int i                       => $"int {i}",
        null                        => "null!",
        _                           => "something else",
    };
    Console.WriteLine(msg);
}

if (shapes[0] is Circle first) Console.WriteLine($"shapes[0] is a Circle with radius {first.Radius}");
var maybeRect = shapes[0] as Rectangle;   // null אם ההמרה לא מתאימה — לא זורק חריגה
Console.WriteLine($"shapes[0] as Rectangle → {(maybeRect is null ? "null" : "rect")}");

Console.WriteLine("\n=== 4. Equals / GetHashCode ===");
var m1 = new Money(10, "ILS");
var m2 = new Money(10, "ILS");
Console.WriteLine($"m1.Equals(m2) = {m1.Equals(m2)}, same hash = {m1.GetHashCode() == m2.GetHashCode()}");
var set = new HashSet<Money> { m1, m2 };
Console.WriteLine($"HashSet count = {set.Count} (הודות ל-Equals/GetHashCode)");

Console.WriteLine("\n=== 5. Composition over inheritance ===");
var svc = new OrderService(new ConsoleLogger());
svc.Place("ORD-1");
var quiet = new OrderService(new NullLogger());
quiet.Place("ORD-2");

// ------------------------------------------------------------------
interface IDescribable
{
    string Describe();
    // default implementation — מי שמממש את הממשק מקבל אותה "בחינם"
    string DescribeLoud() => Describe().ToUpperInvariant() + "!";
}

abstract class Shape : IDescribable
{
    public abstract string Name { get; }
    public abstract double Area();                 // חייבים לממש ביורשים
    public virtual string Describe() => $"I am a {Name}";  // אפשר לדרוס
}

class Circle(double radius) : Shape
{
    public double Radius { get; } = radius;
    public override string Name => "Circle";
    public override double Area() => Math.PI * Radius * Radius;
    public override string Describe() => base.Describe() + $" with radius {Radius}";
}

class Rectangle(double width, double height) : Shape
{
    public double Width { get; } = width;
    public double Height { get; } = height;
    public override string Name => "Rectangle";
    public override double Area() => Width * Height;
}

// sealed — אי אפשר לרשת ממנה הלאה
sealed class Square(double side) : Rectangle(side, side)
{
    public override string Name => "Square";
}

class Invoice(int items, decimal total) : IDescribable
{
    public string Describe() => $"Invoice with {items} items, total {total:N2}";
}

class Money
{
    public decimal Amount { get; }
    public string Currency { get; }
    public Money(decimal amount, string currency) { Amount = amount; Currency = currency; }

    public override bool Equals(object? obj) =>
        obj is Money other && Amount == other.Amount && Currency == other.Currency;
    public override int GetHashCode() => HashCode.Combine(Amount, Currency);
    public override string ToString() => $"{Amount} {Currency}";
}

interface ILogger { void Log(string message); }
class ConsoleLogger : ILogger { public void Log(string message) => Console.WriteLine($"[LOG] {message}"); }
class NullLogger : ILogger { public void Log(string message) { } }

// OrderService "יש לו" logger (has-a) במקום "הוא" logger (is-a)
class OrderService(ILogger logger)
{
    public void Place(string orderId)
    {
        logger.Log($"placing {orderId}");
        Console.WriteLine($"Order {orderId} placed");
    }
}
