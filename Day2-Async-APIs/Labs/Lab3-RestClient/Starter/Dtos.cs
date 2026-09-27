namespace Day2.Lab3;

public record Product(int Id, string Name, decimal Price, string Category, int Stock);
public record ProductInput(string Name, decimal Price, string? Category, int Stock);

// TODO (שלב 1): OrderStatus enum (Pending, Paid, Shipped, Cancelled)
// TODO (שלב 1): OrderItem(ProductId, ProductName, Quantity, UnitPrice)
// TODO (שלב 1): Order(Id, Customer, CreatedAt, Status, Items) + Total
// TODO (שלב 1): OrderItemInput(ProductId, Quantity), OrderInput(Customer, Items)

public record PublicPost(int UserId, int Id, string Title, string Body);
