using Day4.Lab2.Domain;
using Day4.Lab2.Infrastructure;
using Day4.Lab2.Services;

namespace Day4.Lab2.Tests;

public class BillingReportTests
{
    [Fact]
    public void Write_MatchesGoldenMasterFromLegacyProgram()
    {
        var expected = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "expected-output.txt")).Replace("\r\n", "\n");
        var writer = new StringWriter { NewLine = "\n" };

        new BillingReport(new EmbeddedCsvSubscriptionSource(), new PricingService(), new ReportFormatter()).Write(writer);

        Assert.Equal(expected, writer.ToString());
    }

    [Fact]
    public void Parser_ThrowsWithLineOnMalformedInput()
    {
        var ex = Assert.Throws<FormatException>(() => CsvSubscriptionParser.ParseLine("S-9,only,three"));
        Assert.Contains("S-9,only,three", ex.Message);
    }

    [Fact]
    public void Parser_IsCaseInsensitiveForEnums()
    {
        var s = CsvSubscriptionParser.ParseLine("S-9,X,pro,1,30,2,loyal");
        Assert.Equal(Plan.Pro, s.Plan);
        Assert.Equal(Coupon.Loyal, s.Coupon);
    }
}
