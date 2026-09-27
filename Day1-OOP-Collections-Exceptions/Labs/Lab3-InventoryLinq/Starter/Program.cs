// Day1.Lab3.Starter — מלאי עם LINQ, delegates ואירועים
// השלימו את ה-TODO ב-Inventory.cs ו-Reports.cs; ה-Program כבר כתוב ויעבוד ברגע שתסיימו.
using Day1.Lab3;

var inv = new Inventory();

// TODO 16: הירשמו לאירוע LowStock והדפיסו: "  !! LOW STOCK: <name> (<qty> left, reorder at <level>)"

Seed(inv);

Console.WriteLine("=== All items ===");
foreach (var item in inv.Items) Console.WriteLine(item);
Console.WriteLine($"Categories: {string.Join(", ", inv.Categories)}");

Console.WriteLine("\n=== Selling ===");
inv.Sell("K-100", 8);
inv.Sell("M-200", 2);
inv.Sell("C-300", 45);
try { inv.Sell("K-100", 999); }
catch (InvalidOperationException ex) { Console.WriteLine($"  rejected: {ex.Message}"); }
try { inv.Sell("NOPE", 1); }
catch (KeyNotFoundException ex) { Console.WriteLine($"  rejected: {ex.Message}"); }

Console.WriteLine("\n=== Custom filters (Func) ===");
Func<StockItem, bool> cheap = i => i.Product.Price < 50;
Func<StockItem, bool> peripherals = i => i.Product.Category == "Peripherals";
Console.WriteLine("cheap: " + string.Join(", ", inv.Find(cheap).Select(i => i.Product.Name)));
Console.WriteLine("cheap peripherals: " + string.Join(", ", inv.Find(i => cheap(i) && peripherals(i)).Select(i => i.Product.Name)));

Console.WriteLine("\n=== Action on a category (restock all Cables by +20) ===");
inv.ApplyToCategory("Cables", i => inv.Receive(i.Product.Sku, 20));
foreach (var item in inv.Find(i => i.Product.Category == "Cables")) Console.WriteLine(item);

Console.WriteLine("\n=== Reports ===");
Console.WriteLine($"Total inventory value: {Reports.TotalValue(inv.Items):N2}");
foreach (var c in Reports.ByCategory(inv.Items))
    Console.WriteLine($"  {c.Category,-12} products={c.ProductCount} qty={c.TotalQuantity,4} value={c.TotalValue,10:N2}");
Console.WriteLine("Top 3 by value:");
foreach (var item in Reports.TopByValue(inv.Items, 3)) Console.WriteLine("  " + item);
Console.WriteLine("Low stock:");
foreach (var item in Reports.LowStock(inv.Items)) Console.WriteLine("  " + item);
var prices = Reports.PriceIndex(inv.Items);
Console.WriteLine($"Price of M-200 via index: {prices["M-200"]:N2}");

static void Seed(Inventory inv)
{
    inv.Add(new Product("K-100", "Keyboard", "Peripherals", 149.90m), 12, reorderLevel: 5);
    inv.Add(new Product("M-200", "Mouse", "Peripherals", 79.00m), 4, reorderLevel: 3);
    inv.Add(new Product("C-300", "USB-C Cable", "Cables", 29.90m), 50, reorderLevel: 10);
    inv.Add(new Product("C-301", "HDMI Cable", "Cables", 39.90m), 8, reorderLevel: 10);
    inv.Add(new Product("S-400", "27\" Monitor", "Displays", 1_199.00m), 6, reorderLevel: 2);
    inv.Add(new Product("S-401", "Monitor Arm", "Displays", 249.00m), 3, reorderLevel: 2);
    inv.Add(new Product("P-500", "Mouse Pad", "Peripherals", 19.90m), 40, reorderLevel: 10);
}
