// =====================================================================
// Day1.Demo.LinqDelegates — Delegates, Lambdas, Events ו-LINQ
// מה הדמו מראה:
//   * delegate מותאם אישית, Func/Action/Predicate, multicast
//   * lambdas ו-closures (משתנה שנלכד)
//   * event עם EventHandler<T> — publisher/subscriber
//   * LINQ: Where/Select/OrderBy/GroupBy/First/Any/ToDictionary/Aggregate
//   * method syntax מול query syntax, deferred execution
//   * סגנון פונקציונלי: פונקציות טהורות ואי-שינוי (records)
// הרצה:  dotnet run
// =====================================================================

Console.WriteLine("=== 1. delegates ===");
MathOp add = (a, b) => a + b;
MathOp mul = (a, b) => a * b;
Console.WriteLine($"add(2,3)={add(2, 3)}, mul(2,3)={mul(2, 3)}");
Console.WriteLine($"Apply(10, 4, subtract) = {Apply(10, 4, (a, b) => a - b)}");

Func<int, int, int> power = (b, e) => (int)Math.Pow(b, e);
Action<string> shout = s => Console.WriteLine(s.ToUpper() + "!");
Predicate<int> isEven = n => n % 2 == 0;
Console.WriteLine($"power(2,5)={power(2, 5)}, isEven(4)={isEven(4)}");
shout("delegates are fun");

Action pipeline = () => Console.Write("step1 ");
pipeline += () => Console.Write("step2 ");
pipeline += () => Console.WriteLine("step3");
pipeline();   // multicast — כולם רצים לפי הסדר

Console.WriteLine("\n=== 2. closures ===");
var counter = MakeCounter();
Console.WriteLine($"{counter()} {counter()} {counter()}   (ה-lambda 'זוכרת' את המשתנה count)");

Console.WriteLine("\n=== 3. events ===");
var sensor = new Thermometer();
sensor.TemperatureChanged += (sender, e) =>
    Console.WriteLine($"  [subscriber A] {e.OldValue} → {e.NewValue}");
sensor.TemperatureChanged += (_, e) =>
{
    if (e.NewValue > 30) Console.WriteLine("  [subscriber B] HOT! turning on AC");
};
sensor.Temperature = 25;
sensor.Temperature = 32;
sensor.Temperature = 32;   // אין שינוי → אין אירוע

Console.WriteLine("\n=== 4. LINQ ===");
List<Order> orders =
[
    new(1, "Dana", "Books", 120m),
    new(2, "Yossi", "Electronics", 2_400m),
    new(3, "Dana", "Electronics", 800m),
    new(4, "Noa", "Books", 60m),
    new(5, "Yossi", "Toys", 150m),
];

var expensive = orders.Where(o => o.Total > 100).Select(o => $"{o.Customer}:{o.Total}");
Console.WriteLine("expensive: " + string.Join(", ", expensive));

var byTotalDesc = orders.OrderByDescending(o => o.Total).ThenBy(o => o.Customer).Select(o => o.Id);
Console.WriteLine("ids by total desc: " + string.Join(",", byTotalDesc));

Console.WriteLine("group by category:");
foreach (var g in orders.GroupBy(o => o.Category))
    Console.WriteLine($"  {g.Key}: {g.Count()} orders, sum={g.Sum(o => o.Total)}");

Console.WriteLine($"first Books order: #{orders.First(o => o.Category == "Books").Id}");
Console.WriteLine($"any > 5000? {orders.Any(o => o.Total > 5000)}, all > 50? {orders.All(o => o.Total > 50)}");
Console.WriteLine($"FirstOrDefault Toys>1000: {orders.FirstOrDefault(o => o.Category == "Toys" && o.Total > 1000)?.Id.ToString() ?? "none"}");

Dictionary<string, decimal> totalPerCustomer = orders
    .GroupBy(o => o.Customer)
    .ToDictionary(g => g.Key, g => g.Sum(o => o.Total));
foreach (var (cust, total) in totalPerCustomer) Console.WriteLine($"  {cust} spent {total}");

Console.WriteLine($"max={orders.Max(o => o.Total)}, avg={orders.Average(o => o.Total):F1}, count={orders.Count()}");
Console.WriteLine($"Aggregate (sum manually) = {orders.Aggregate(0m, (acc, o) => acc + o.Total)}");

Console.WriteLine("\n=== 5. query syntax (אותו דבר, תחביר אחר) ===");
var q = from o in orders
        where o.Category == "Electronics"
        orderby o.Total descending
        select new { o.Id, o.Customer };
foreach (var item in q) Console.WriteLine($"  #{item.Id} {item.Customer}");

Console.WriteLine("\n=== 6. deferred execution ===");
var numbers = new List<int> { 1, 2, 3 };
var evens = numbers.Where(n => n % 2 == 0);   // עדיין לא רץ!
numbers.Add(4);
Console.WriteLine($"evens = {string.Join(",", evens)}   (4 נכלל כי השאילתה רצה רק עכשיו)");
var snapshot = numbers.Where(n => n % 2 == 0).ToList();   // ToList() מקפיא
numbers.Add(6);
Console.WriteLine($"snapshot = {string.Join(",", snapshot)}   (6 לא נכלל)");

Console.WriteLine("\n=== 7. סגנון פונקציונלי ===");
var cart = new Cart([new("pen", 5m), new("book", 40m)]);
var discounted = ApplyDiscount(cart, 0.10m);   // פונקציה טהורה — לא משנה את cart
Console.WriteLine($"original total={cart.Total}, discounted total={discounted.Total}");

// ------------------------------------------------------------------
static int Apply(int a, int b, MathOp op) => op(a, b);

static Func<int> MakeCounter()
{
    int count = 0;
    return () => ++count;   // closure על count
}

static Cart ApplyDiscount(Cart cart, decimal percent) =>
    cart with { Items = cart.Items.Select(i => i with { Price = i.Price * (1 - percent) }).ToList() };

// הצהרת delegate היא הצהרת טיפוס — לכן היא אחרי ה-local functions
delegate int MathOp(int a, int b);

record Order(int Id, string Customer, string Category, decimal Total);
record Item(string Name, decimal Price);
record Cart(List<Item> Items)
{
    public decimal Total => Items.Sum(i => i.Price);
}

class TemperatureChangedEventArgs(double oldValue, double newValue) : EventArgs
{
    public double OldValue { get; } = oldValue;
    public double NewValue { get; } = newValue;
}

class Thermometer
{
    private double _temperature;

    public event EventHandler<TemperatureChangedEventArgs>? TemperatureChanged;

    public double Temperature
    {
        get => _temperature;
        set
        {
            if (value == _temperature) return;
            var old = _temperature;
            _temperature = value;
            TemperatureChanged?.Invoke(this, new TemperatureChangedEventArgs(old, value));
        }
    }
}
