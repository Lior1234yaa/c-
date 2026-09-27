namespace Day4.Lab3.Snippets;

public record PriceRow(string Sku, decimal Price, DateTime ValidFrom);

/// <summary>Parses "SKU;price;yyyy-MM-dd" rows from an import file.</summary>
public class PriceParser
{
    public List<PriceRow> Parse(IEnumerable<string> lines)
    {
        var rows = new List<PriceRow>();
        foreach (var line in lines)
        {
            try
            {
                var parts = line.Split(';');
                var price = double.Parse(parts[1]);
                var date = DateTime.Parse(parts[2]);
                rows.Add(new PriceRow(parts[0], (decimal)price, date));
            }
            catch
            {
                // skip bad lines
            }
        }
        return rows;
    }

    public bool IsValidNow(PriceRow row) => row.ValidFrom <= DateTime.Now;
}
