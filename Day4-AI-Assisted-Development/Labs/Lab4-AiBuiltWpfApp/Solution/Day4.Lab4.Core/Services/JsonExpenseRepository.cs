using System.Text.Json;
using Day4.Lab4.Core.Models;

namespace Day4.Lab4.Core.Services;

/// <summary>שמירה ל-JSON: קובץ חסר = רשימה ריקה; קובץ פגום = חריגה ברורה; כתיבה אטומית (temp + move).</summary>
public sealed class JsonExpenseRepository(string? filePath = null) : IExpenseRepository
{
    private static readonly JsonSerializerOptions Options = new() { WriteIndented = true };

    public string FilePath { get; } = filePath ?? Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Day4Lab4", "expenses.json");

    public async Task<IReadOnlyList<Expense>> LoadAsync(CancellationToken ct = default)
    {
        if (!File.Exists(FilePath)) return [];

        await using var stream = File.OpenRead(FilePath);
        try
        {
            return await JsonSerializer.DeserializeAsync<List<Expense>>(stream, Options, ct) ?? [];
        }
        catch (JsonException ex)
        {
            throw new InvalidDataException($"קובץ ההוצאות '{FilePath}' פגום: {ex.Message}", ex);
        }
    }

    public async Task SaveAsync(IReadOnlyList<Expense> expenses, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(expenses);
        Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);

        var temp = FilePath + ".tmp";
        await using (var stream = File.Create(temp))
        {
            await JsonSerializer.SerializeAsync(stream, expenses, Options, ct);
        }
        File.Move(temp, FilePath, overwrite: true);
    }
}
