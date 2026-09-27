// =====================================================================
// Day1.Demo.Classes — מחלקות, אובייקטים, בנאים ומאפיינים
// מה הדמו מראה:
//   * class עם שדות פרטיים ו-properties (auto, init, required, computed)
//   * בנאים: overloads, שרשור עם this(...), primary constructor (C# 12)
//   * object initializer, static members (מונה מופעים, factory)
//   * record מול class — שוויון לפי ערך, with-expression, deconstruction
// הרצה:  dotnet run
// =====================================================================

Console.WriteLine("=== 1. מחלקה בסיסית עם properties ===");
var acc = new BankAccount("IL-001", "Dana");
acc.Deposit(500);
acc.Deposit(250);
Console.WriteLine(acc);
Console.WriteLine($"Owner={acc.Owner}, Balance={acc.Balance}, IsEmpty={acc.IsEmpty}");
// acc.Balance = 1000;  // שגיאת קומפילציה: ה-setter פרטי — עקרון האנקפסולציה

Console.WriteLine("\n=== 2. בנאים מרובים ושרשור ===");
var acc2 = new BankAccount("IL-002", "Yossi", initialDeposit: 1_000);
Console.WriteLine(acc2);
Console.WriteLine($"Total accounts created: {BankAccount.Count}");

Console.WriteLine("\n=== 3. required + init + object initializer ===");
var product = new Product { Sku = "P-100", Name = "Keyboard", Price = 149.9m };
// product.Sku = "X";  // שגיאה: init-only — ניתן לקבוע רק בזמן היצירה
Console.WriteLine($"{product.Name} ({product.Sku}) costs {product.Price:C}, with VAT {product.PriceWithVat:C}");

Console.WriteLine("\n=== 4. Primary constructor (C# 12) ===");
var temp = new TemperatureSensor("Kitchen", 22.5);
temp.Read(23.1);
temp.Read(24.0);
Console.WriteLine($"{temp.Location}: last={temp.Last}, avg={temp.Average:F2}");

Console.WriteLine("\n=== 5. static: מונה, קבועים ו-factory ===");
var free = Product.CreateFreeSample("Sticker");
Console.WriteLine($"{free.Name} price={free.Price} (Product.MaxNameLength={Product.MaxNameLength})");

Console.WriteLine("\n=== 6. record מול class ===");
var c1 = new PersonClass("Dana", 30);
var c2 = new PersonClass("Dana", 30);
Console.WriteLine($"class: c1 == c2 ? {c1 == c2}   Equals? {c1.Equals(c2)}   (הפניות שונות)");

var r1 = new PersonRecord("Dana", 30);
var r2 = new PersonRecord("Dana", 30);
Console.WriteLine($"record: r1 == r2 ? {r1 == r2}   (שוויון לפי ערך)");
Console.WriteLine($"ToString של record: {r1}");
var r3 = r1 with { Age = 31 };           // עותק עם שינוי
Console.WriteLine($"with: {r3}");
var (pname, page) = r3;                   // deconstruction
Console.WriteLine($"deconstructed: {pname} / {page}");

// ------------------------------------------------------------------
class BankAccount
{
    // שדה פרטי — המצב הפנימי
    private readonly List<decimal> _transactions = [];

    public static int Count { get; private set; }

    public string Id { get; }                  // read-only property (נקבע בבנאי)
    public string Owner { get; set; }          // auto-property רגיל
    public decimal Balance { get; private set; } // קריאה ציבורית, כתיבה פרטית
    public bool IsEmpty => Balance == 0;      // computed property (expression-bodied)

    public BankAccount(string id, string owner)
    {
        Id = id;
        Owner = owner;
        Count++;
    }

    // overload שמשרשר לבנאי הראשי
    public BankAccount(string id, string owner, decimal initialDeposit) : this(id, owner)
    {
        Deposit(initialDeposit);
    }

    public void Deposit(decimal amount)
    {
        if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount), "הסכום חייב להיות חיובי");
        Balance += amount;
        _transactions.Add(amount);
    }

    public override string ToString() => $"[{Id}] {Owner}: {Balance:N2} ({_transactions.Count} tx)";
}

class Product
{
    public const int MaxNameLength = 50;              // קבוע — static מובנה
    public required string Sku { get; init; }         // חובה באתחול, ולא ניתן לשנות אחר כך
    public required string Name { get; init; }
    public decimal Price { get; set; }
    public decimal PriceWithVat => Price * 1.18m;     // computed

    public static Product CreateFreeSample(string name) => new() { Sku = "FREE", Name = name, Price = 0 };
}

// primary constructor: הפרמטרים זמינים בכל גוף המחלקה
class TemperatureSensor(string location, double initial)
{
    private readonly List<double> _readings = [initial];

    public string Location => location;
    public double Last => _readings[^1];
    public double Average => _readings.Average();

    public void Read(double value) => _readings.Add(value);
}

class PersonClass(string name, int age)
{
    public string Name { get; } = name;
    public int Age { get; } = age;
}

record PersonRecord(string Name, int Age);
