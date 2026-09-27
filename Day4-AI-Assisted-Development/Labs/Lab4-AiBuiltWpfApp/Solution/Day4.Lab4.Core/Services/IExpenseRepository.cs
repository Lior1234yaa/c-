using Day4.Lab4.Core.Models;

namespace Day4.Lab4.Core.Services;

public interface IExpenseRepository
{
    Task<IReadOnlyList<Expense>> LoadAsync(CancellationToken ct = default);
    Task SaveAsync(IReadOnlyList<Expense> expenses, CancellationToken ct = default);
}
