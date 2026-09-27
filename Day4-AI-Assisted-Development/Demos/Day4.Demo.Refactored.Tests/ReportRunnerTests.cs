using Day4.Demo.Refactored.Domain;
using Day4.Demo.Refactored.Infrastructure;
using Day4.Demo.Refactored.Services;

namespace Day4.Demo.Refactored.Tests;

public class ReportRunnerTests
{
    private sealed class FakeSource(params Order[] orders) : IOrderSource
    {
        public IReadOnlyList<Order> Load() => orders;
    }

    private static ReportRunner CreateRunner(IOrderSource source)
        => new(source,
               new OrderPricer(new DiscountPolicy(), new ShippingCalculator(), new IsraelVatCalculator()),
               new ReportFormatter());

    [Fact]
    public void Run_SkipsCancelledOrders_ButCountsThemInStatus()
    {
        var cancelled = new Order(5, "Noa", CustomerType.Regular, 1, 10m, "IL", OrderStatus.Cancelled, new(2025, 1, 1));
        var writer = new StringWriter();

        CreateRunner(new FakeSource(cancelled)).Run(writer);
        var text = writer.ToString();

        Assert.Contains("Order 5 (Noa) - CANCELLED, skipped", text);
        Assert.Contains("Orders processed:    0", text);
        Assert.Contains("CANCELLED    :    1", text);
    }

    [Fact]
    public void Run_WithEmbeddedData_MatchesGoldenMaster()
    {
        // ה-golden master הופק מהגרסה הישנה (Day4.Demo.LegacyMess) ונשמר כקובץ בפרויקט הבדיקות.
        var expected = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "golden-master.txt"))
                           .Replace("\r\n", "\n");
        var writer = new StringWriter { NewLine = "\n" };

        CreateRunner(new EmbeddedCsvOrderSource()).Run(writer);

        Assert.Equal(expected, writer.ToString());
    }

    [Fact]
    public void CsvParser_ThrowsOnMalformedLine()
    {
        var ex = Assert.Throws<FormatException>(() => CsvOrderParser.ParseLine("1,only,three"));
        Assert.Contains("Expected 8 fields", ex.Message);
    }

    [Fact]
    public void CsvParser_ParsesPriceWithInvariantCulture()
    {
        var order = CsvOrderParser.ParseLine("1,Dana,VIP,2,19.9,IL,PAID,2025-03-01");
        Assert.Equal(19.9m, order.UnitPrice);
        Assert.Equal(new DateOnly(2025, 3, 1), order.Date);
    }
}
