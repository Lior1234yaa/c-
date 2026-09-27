namespace Day1.Lab4;

/// <summary>בסיס לכל חריגות הבנק — מאפשר catch אחד לכל "שגיאה עסקית".</summary>
public class BankException(string message) : Exception(message);

public class AccountNotFoundException(string accountId)
    : BankException($"Account '{accountId}' not found")
{
    public string AccountId { get; } = accountId;
}

public class InsufficientFundsException(string accountId, decimal requested, decimal available)
    : BankException($"Insufficient funds in {accountId}: requested {requested:N2}, available {available:N2}")
{
    public string AccountId { get; } = accountId;
    public decimal Requested { get; } = requested;
    public decimal Available { get; } = available;
    public decimal Shortfall => Requested - Available;
}

public class InvalidAmountException(decimal amount)
    : BankException($"Amount must be positive, got {amount:N2}")
{
    public decimal Amount { get; } = amount;
}
