namespace Day2.Lab3;

public enum OrderStatus { Pending, Paid, Shipped, Cancelled }

public record Product(int Id, string Name, decimal Price, string Category, int Stock);
public record ProductInput(string Name, decimal Price, string? Category, int Stock);

public record OrderItem(int ProductId, string ProductName, int Quantity, decimal UnitPrice);

public record Order(int Id, string Customer, DateTime CreatedAt, OrderStatus Status, List<OrderItem> Items)
{
    // מחושב מקומית; ה-API גם שולח "total" אבל אין לו setter ולכן הוא מתעלם ממנו בקריאה
    public decimal Total => Items.Sum(i => i.Quantity * i.UnitPrice);
}

public record OrderItemInput(int ProductId, int Quantity);
public record OrderInput(string Customer, List<OrderItemInput> Items);
public record OrderStatusInput(OrderStatus Status);

public record PublicPost(int UserId, int Id, string Title, string Body);

// תשובת שגיאה של ה-API: {"error": "..."}
public record ApiError(string? Error);
