namespace Day1.Lab2;

// ============ חלק ב' — עובדים ומשכורות ============

public interface IPayable
{
    string PayeeName { get; }
    decimal CalculateMonthlyPay();

    // default interface member — כל מממש מקבל אותו בחינם ויכול לדרוס
    string PaySlip() => $"{PayeeName,-24} {CalculateMonthlyPay(),12:N2}";
}

public abstract class Employee(int id, string name) : IPayable
{
    public int Id { get; } = id;
    public string Name { get; } = !string.IsNullOrWhiteSpace(name) ? name : throw new ArgumentException("name required", nameof(name));

    public string PayeeName => Name;
    public abstract decimal CalculateMonthlyPay();
    public virtual string Describe() => $"#{Id} {Name} ({GetType().Name})";
}

public class SalariedEmployee(int id, string name, decimal annualSalary) : Employee(id, name)
{
    public decimal AnnualSalary { get; } = annualSalary;
    public override decimal CalculateMonthlyPay() => AnnualSalary / 12;
}

public class HourlyEmployee(int id, string name, decimal hourlyRate, int hoursWorked) : Employee(id, name)
{
    private const int RegularHours = 160;
    private const decimal OvertimeFactor = 1.5m;

    public decimal HourlyRate { get; } = hourlyRate;
    public int HoursWorked { get; } = hoursWorked;

    public override decimal CalculateMonthlyPay()
    {
        int regular = Math.Min(HoursWorked, RegularHours);
        int overtime = Math.Max(HoursWorked - RegularHours, 0);
        return regular * HourlyRate + overtime * HourlyRate * OvertimeFactor;
    }

    public override string Describe() => base.Describe() + $"  hours={HoursWorked}";
}

public class Manager(int id, string name, decimal annualSalary, decimal bonus) : SalariedEmployee(id, name, annualSalary)
{
    public decimal Bonus { get; } = bonus;
    public List<Employee> Reports { get; } = [];

    public override decimal CalculateMonthlyPay() => base.CalculateMonthlyPay() + Bonus;
    public override string Describe() => base.Describe() + $"  manages {Reports.Count}";
}

// קבלן: לא Employee, אבל כן IPayable — הממשק מאפשר לתשלומים לעבוד על "כל מה שמשלמים לו"
public class Contractor(string name, decimal monthlyFee) : IPayable
{
    public string PayeeName => name + " (contractor)";
    public decimal CalculateMonthlyPay() => monthlyFee;
}

public class Payroll
{
    private readonly List<IPayable> _payees = [];

    public void Add(IPayable payee)
    {
        ArgumentNullException.ThrowIfNull(payee);
        _payees.Add(payee);
    }

    public decimal TotalMonthly() => _payees.Sum(p => p.CalculateMonthlyPay());

    public void PrintSlips()
    {
        foreach (var p in _payees) Console.WriteLine(p.PaySlip());   // פולימורפיזם דרך הממשק
        Console.WriteLine(new string('-', 37));
        Console.WriteLine($"{"TOTAL",-24} {TotalMonthly(),12:N2}");
    }

    public IEnumerable<Employee> Employees => _payees.OfType<Employee>();
}
