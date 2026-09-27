namespace Day1.Lab2;

// ============ חלק ב' — עובדים ומשכורות (~45 דקות) ============

// TODO 4: ממשק IPayable עם:
//   - string PayeeName { get; }
//   - decimal CalculateMonthlyPay();
//   - default member: string PaySlip() => $"{PayeeName,-24} {CalculateMonthlyPay(),12:N2}";
public interface IPayable
{
}

// TODO 5: מחלקה אבסטרקטית Employee שמממשת IPayable:
//   - Id (int), Name (string) — read-only, נקבעים בבנאי
//   - PayeeName מחזיר את Name
//   - CalculateMonthlyPay אבסטרקטי
//   - virtual string Describe() => $"#{Id} {Name} ({GetType().Name})"
public abstract class Employee
{
}

// TODO 6: SalariedEmployee(id, name, annualSalary) — משכורת חודשית = annualSalary / 12
public class SalariedEmployee
{
}

// TODO 7: HourlyEmployee(id, name, hourlyRate, hoursWorked):
//   עד 160 שעות — תעריף רגיל; כל שעה מעבר — פי 1.5.
//   Describe() דורס ומוסיף "  hours=..." בסוף (השתמשו ב-base.Describe()).
public class HourlyEmployee
{
}

// TODO 8: Manager יורש מ-SalariedEmployee ומוסיף:
//   - List<Employee> Reports (הכפופים)
//   - Bonus (decimal) קבוע לחודש
//   - CalculateMonthlyPay = base + Bonus
//   - Describe() מוסיף "  manages N"
public class Manager
{
}

// TODO 9: Contractor(name, monthlyFee) — לא עובד! לא יורש מ-Employee,
//   אבל מממש IPayable ישירות (חשבונית חודשית קבועה).
public class Contractor
{
}

// TODO 10: Payroll — מחזיק List<IPayable>; Add(IPayable), TotalMonthly(), PrintSlips()
//   (מדפיס PaySlip של כל אחד + שורת סיכום). שימו לב: Payroll לא יודע ולא צריך
//   לדעת אם מדובר בעובד או בקבלן — זה הפולימורפיזם.
public class Payroll
{
    public void Add(IPayable payee) => throw new NotImplementedException();
    public decimal TotalMonthly() => throw new NotImplementedException();
    public void PrintSlips() => throw new NotImplementedException();
}
