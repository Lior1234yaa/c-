namespace Day4.Lab1;

public interface IDiscountEngine
{
    /// <summary>מחשב את ההנחה להזמנה לפי המפרט ב-README. פונקציה טהורה: ללא I/O, ללא זמן, ללא state.</summary>
    DiscountResult Calculate(Order order);
}
