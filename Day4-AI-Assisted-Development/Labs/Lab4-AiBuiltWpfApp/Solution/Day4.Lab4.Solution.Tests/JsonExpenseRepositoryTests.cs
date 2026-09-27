using Day4.Lab4.Core.Models;
using Day4.Lab4.Core.Services;

namespace Day4.Lab4.Tests;

public sealed class JsonExpenseRepositoryTests : IDisposable
{
    private readonly string _dir = Path.Combine(Path.GetTempPath(), "lab4-tests-" + Guid.NewGuid().ToString("N"));
    private string File_ => Path.Combine(_dir, "expenses.json");

    public void Dispose() { try { Directory.Delete(_dir, true); } catch { /* best effort */ } }

    [Fact]
    public async Task LoadAsync_MissingFile_ReturnsEmpty()
    {
        var repo = new JsonExpenseRepository(File_);
        Assert.Empty(await repo.LoadAsync());
    }

    [Fact]
    public async Task SaveAsync_ThenLoadAsync_RoundTrips()
    {
        var repo = new JsonExpenseRepository(File_);
        var expense = new Expense(Guid.NewGuid(), new DateOnly(2025, 3, 1), "מזון", 12.5m, "קפה");

        await repo.SaveAsync([expense]);
        var loaded = await repo.LoadAsync();

        Assert.Equal([expense], loaded);
        Assert.False(File.Exists(File_ + ".tmp"), "temp file must be moved away");
    }

    [Fact]
    public async Task LoadAsync_CorruptFile_ThrowsInvalidData()
    {
        Directory.CreateDirectory(_dir);
        await File.WriteAllTextAsync(File_, "{ not json");
        var repo = new JsonExpenseRepository(File_);

        var ex = await Assert.ThrowsAsync<InvalidDataException>(() => repo.LoadAsync());
        Assert.Contains("פגום", ex.Message);
    }

    [Fact]
    public async Task SaveAsync_WritesInvariantDatesAndNumbers()
    {
        var repo = new JsonExpenseRepository(File_);
        await repo.SaveAsync([new Expense(Guid.Empty, new DateOnly(2025, 12, 31), "אחר", 1234.56m, null)]);
        var json = await File.ReadAllTextAsync(File_);
        Assert.Contains("\"2025-12-31\"", json);
        Assert.Contains("1234.56", json);
    }
}

public class ExpenseValidatorTests
{
    private static readonly DateOnly Today = new(2025, 6, 15);

    [Theory]
    [InlineData("", false)]
    [InlineData("abc", false)]
    [InlineData("0", false)]
    [InlineData("-5", false)]
    [InlineData("12.345", false)]
    [InlineData("12.34", true)]
    [InlineData("100", true)]
    public void ValidateAmountText(string text, bool valid)
        => Assert.Equal(valid, ExpenseValidator.ValidateAmountText(text, out _) is null);

    [Fact]
    public void ValidateAmountText_UsesInvariantCulture()
    {
        Assert.Null(ExpenseValidator.ValidateAmountText("19.90", out var amount));
        Assert.Equal(19.90m, amount);
    }

    [Theory]
    [InlineData("מזון", true)]
    [InlineData("לא קיים", false)]
    [InlineData(null, false)]
    public void ValidateCategory(string? category, bool valid)
        => Assert.Equal(valid, ExpenseValidator.ValidateCategory(category) is null);

    [Fact]
    public void ValidateDate_FutureIsRejected_TodayIsFine()
    {
        Assert.NotNull(ExpenseValidator.ValidateDate(Today.AddDays(1), Today));
        Assert.Null(ExpenseValidator.ValidateDate(Today, Today));
        Assert.NotNull(ExpenseValidator.ValidateDate(null, Today));
    }

    [Fact]
    public void ValidateNote_MaxLength()
    {
        Assert.Null(ExpenseValidator.ValidateNote(new string('a', 100)));
        Assert.NotNull(ExpenseValidator.ValidateNote(new string('a', 101)));
        Assert.Null(ExpenseValidator.ValidateNote(null));
    }
}
