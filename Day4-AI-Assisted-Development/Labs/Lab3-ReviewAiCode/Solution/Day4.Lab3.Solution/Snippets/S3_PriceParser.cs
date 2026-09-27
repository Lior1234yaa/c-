using System.Globalization;

namespace Day4.Lab3.Snippets;

public sealed record PriceRow(string Sku, decimal Price, DateOnly ValidFrom);

/// <summary>Parses "SKU;price;yyyy-MM-dd" rows. Machine data → InvariantCulture; bad lines fail loudly with the line number.</summary>
public sealed class PriceParser(TimeProvider clock)
{
    public static List<PriceRow> Parse(IEnumerable<string> lines)
    {
        var rows = new List<PriceRow>();
        var lineNo = 0;
        foreach (var line in lines)
        {
            lineNo++;
            var parts = line.Split(';');
            if (parts.Length != 3)
                throw new FormatException($"Line {lineNo}: expected 3 fields, got {parts.Length}.");
            if (!decimal.TryParse(parts[1], NumberStyles.Number, CultureInfo.InvariantCulture, out var price))
                throw new FormatException($"Line {lineNo}: invalid price '{parts[1]}'.");
            if (!DateOnly.TryParseExact(parts[2], "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                throw new FormatException($"Line {lineNo}: invalid date '{parts[2]}' (expected yyyy-MM-dd).");

            rows.Add(new PriceRow(parts[0].Trim(), price, date));
        }
        return rows;
    }

    public bool IsValidNow(PriceRow row)
        => row.ValidFrom <= DateOnly.FromDateTime(clock.GetUtcNow().UtcDateTime);
}
