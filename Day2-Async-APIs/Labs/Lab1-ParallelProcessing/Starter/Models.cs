namespace Day2.Lab1;

public record Order(int Id, string Customer, int ImageSize);

public record OrderResult(int OrderId, long Checksum, TimeSpan Duration);

public static class OrderProcessor
{
    /// <summary>עבודת CPU: "מעבד" תמונה — לולאה חישובית שתלויה בגודל התמונה.</summary>
    public static long ProcessImage(Order order)
    {
        long checksum = order.Id;
        for (int i = 0; i < order.ImageSize * 40_000; i++)
            checksum = (checksum * 31 + i) % 1_000_000_007;
        return checksum;
    }

    /// <summary>עבודת IO: "שולח אישור" — המתנה של 50–150ms.</summary>
    public static async Task SendConfirmationAsync(Order order, CancellationToken ct = default)
    {
        await Task.Delay(50 + order.Id % 100, ct);
    }

    public static List<Order> GenerateOrders(int count)
    {
        var names = new[] { "Dana", "Yossi", "Noa", "Avi", "Maya", "Tom" };
        return Enumerable.Range(1, count)
            .Select(i => new Order(i, names[i % names.Length], 200 + (i * 37) % 300))
            .ToList();
    }
}
