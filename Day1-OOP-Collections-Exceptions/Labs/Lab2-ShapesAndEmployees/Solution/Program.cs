// Day1.Lab2.Solution — צורות ועובדים (פתרון מלא)
// הרצה: dotnet run
using Day1.Lab2;

Console.WriteLine("=== Part A: Shapes ===");
List<Shape> shapes = [new Circle(1), new Rectangle(2, 3), new Triangle(3, 4, 5), new Square(2)];
foreach (var s in shapes) Console.WriteLine(s);
Console.WriteLine($"Total area: {shapes.Sum(s => s.Area()):F2}");
Console.WriteLine($"Largest: {shapes.MaxBy(s => s.Area())?.Name}");
try
{
    _ = new Triangle(1, 1, 10);
}
catch (ArgumentException ex)
{
    Console.WriteLine($"Invalid triangle rejected: {ex.Message}");
}

Console.WriteLine("\n=== Part B: Payroll ===");
var dana = new SalariedEmployee(1, "Dana", 240_000m);
var yossi = new HourlyEmployee(2, "Yossi", 80m, 170);
var noa = new Manager(3, "Noa", 360_000m, bonus: 2_000m);
noa.Reports.Add(dana);
noa.Reports.Add(yossi);
var freelancer = new Contractor("Acme Ltd.", 12_000m);

var payroll = new Payroll();
payroll.Add(dana);
payroll.Add(yossi);
payroll.Add(noa);
payroll.Add(freelancer);
payroll.PrintSlips();

Console.WriteLine("\nEmployees only (pattern matching):");
foreach (var e in payroll.Employees)
    Console.WriteLine("  " + e.Describe());

Console.WriteLine("\nBonus — switch on type:");
IPayable[] all = [dana, yossi, noa, freelancer];
foreach (var p in all)
{
    string kind = p switch
    {
        Manager m when m.Reports.Count > 1 => "senior manager",
        Manager                            => "manager",
        HourlyEmployee { HoursWorked: > 160 } => "hourly with overtime",
        Employee                           => "regular employee",
        _                                  => "external",
    };
    Console.WriteLine($"  {p.PayeeName,-22} → {kind}");
}
