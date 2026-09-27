using System.Globalization;
using Day4.Lab2.Domain;
using Day4.Lab2.Services;

namespace Day4.Lab2.Infrastructure;

public sealed class EmbeddedCsvSubscriptionSource : ISubscriptionSource
{
    private const string Csv = """
        S-01,Dana Levi,PRO,2,30,14,NONE
        S-02,Yossi Cohen,BASIC,0,30,2,NONE
        S-03,ACME Ltd,TEAM,5,30,26,LOYAL
        S-04,Noa Bar,PRO,1,15,1,WELCOME
        S-05,Lior Katz,BASIC,3,30,7,NONE
        S-06,Beta Inc,TEAM,0,10,3,NONE
        S-07,Dana Levi,BASIC,1,30,14,LOYAL
        S-08,Gal Peretz,PRO,0,30,0,WELCOME
        """;

    public IReadOnlyList<Subscription> Load() => CsvSubscriptionParser.Parse(Csv);
}

public static class CsvSubscriptionParser
{
    public static IReadOnlyList<Subscription> Parse(string csv)
        => csv.Split('\n').Select(l => l.Trim()).Where(l => l.Length > 0).Select(ParseLine).ToList();

    public static Subscription ParseLine(string line)
    {
        var f = line.Split(',');
        if (f.Length != 7) throw new FormatException($"Expected 7 fields, got {f.Length}: '{line}'");
        return new Subscription(
            Id: f[0],
            Customer: f[1],
            Plan: Enum.Parse<Plan>(f[2], ignoreCase: true),
            Addons: int.Parse(f[3], CultureInfo.InvariantCulture),
            DaysActive: int.Parse(f[4], CultureInfo.InvariantCulture),
            TenureMonths: int.Parse(f[5], CultureInfo.InvariantCulture),
            Coupon: Enum.Parse<Coupon>(f[6], ignoreCase: true));
    }
}
