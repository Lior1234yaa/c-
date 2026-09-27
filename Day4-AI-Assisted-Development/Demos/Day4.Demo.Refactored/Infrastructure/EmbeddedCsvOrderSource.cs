using System.Globalization;
using Day4.Demo.Refactored.Domain;
using Day4.Demo.Refactored.Services;

namespace Day4.Demo.Refactored.Infrastructure;

/// <summary>מקור נתונים: CSV מוטבע. קל להחליף בקובץ/DB כי ReportRunner מכיר רק את IOrderSource.</summary>
public sealed class EmbeddedCsvOrderSource : IOrderSource
{
    private const string Csv = """
        1001,Dana Levi,VIP,3,120.5,IL,PAID,2025-03-01
        1002,Yossi Cohen,REG,12,15,IL,PAID,2025-03-01
        1003,ACME Ltd,BIZ,40,9.99,US,PENDING,2025-03-02
        1004,Dana Levi,VIP,1,999,IL,PAID,2025-03-02
        1005,Noa Bar,REG,2,49.9,DE,CANCELLED,2025-03-03
        1006,Yossi Cohen,REG,1,15,IL,PENDING,2025-03-03
        1007,ACME Ltd,BIZ,100,4.5,US,PAID,2025-03-04
        1008,Lior Katz,NEW,5,20,IL,PAID,2025-03-04
        1009,Noa Bar,REG,7,33.3,DE,PAID,2025-03-05
        1010,Lior Katz,NEW,1,250,FR,PAID,2025-03-05
        """;

    public IReadOnlyList<Order> Load() => CsvOrderParser.Parse(Csv);
}

public static class CsvOrderParser
{
    public static IReadOnlyList<Order> Parse(string csv)
    {
        var result = new List<Order>();
        foreach (var raw in csv.Split('\n'))
        {
            var line = raw.Trim();
            if (line.Length == 0) continue;
            result.Add(ParseLine(line));
        }
        return result;
    }

    public static Order ParseLine(string line)
    {
        var f = line.Split(',');
        if (f.Length != 8)
            throw new FormatException($"Expected 8 fields, got {f.Length}: '{line}'");

        return new Order(
            Id: int.Parse(f[0], CultureInfo.InvariantCulture),
            Customer: f[1],
            Type: ParseType(f[2]),
            Quantity: int.Parse(f[3], CultureInfo.InvariantCulture),
            UnitPrice: decimal.Parse(f[4], CultureInfo.InvariantCulture),
            Country: f[5],
            Status: Enum.Parse<OrderStatus>(f[6], ignoreCase: true),
            Date: DateOnly.ParseExact(f[7], "yyyy-MM-dd", CultureInfo.InvariantCulture));
    }

    private static CustomerType ParseType(string code) => code switch
    {
        "VIP" => CustomerType.Vip,
        "BIZ" => CustomerType.Business,
        "REG" => CustomerType.Regular,
        "NEW" => CustomerType.New,
        _ => throw new FormatException($"Unknown customer type '{code}'"),
    };
}
