namespace Day1.Lab4;

public class Account
{
    public string Id { get; }
    public string Owner { get; }
    public decimal Balance { get; private set; }
    public List<string> History { get; } = [];

    public Account(string id, string owner)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(owner);
        Id = id;
        Owner = owner;
    }

    public void Deposit(decimal amount)
    {
        if (amount <= 0) throw new InvalidAmountException(amount);   // FIX 1: גם 0 נדחה
        Balance += amount;
        History.Add($"deposit {amount:N2} -> {Balance:N2}");
    }

    public void Withdraw(decimal amount)
    {
        if (amount <= 0) throw new InvalidAmountException(amount);
        if (amount > Balance)                                           // FIX 2: בודקים לפני שמורידים
            throw new InsufficientFundsException(Id, amount, Balance);
        Balance -= amount;
        History.Add($"withdraw {amount:N2} -> {Balance:N2}");
    }

    public IEnumerable<string> RecentHistory(int count)
    {
        // FIX 3: off-by-one — Count - count, לא Count - count - 1
        int start = Math.Max(History.Count - count, 0);
        for (int i = start; i < History.Count; i++)
            yield return History[i];
    }
}

public class Bank(TransactionLog log)
{
    // FIX 4: מפתחות לא תלויי רישיות — "acc1" ו-"ACC1" הם אותו חשבון
    private readonly Dictionary<string, Account> _accounts = new(StringComparer.OrdinalIgnoreCase);

    public Account Open(string id, string owner)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(owner);
        id = id.ToUpperInvariant();
        if (_accounts.ContainsKey(id)) throw new BankException($"Account {id} already exists");
        var account = new Account(id, owner);
        _accounts[id] = account;
        log.Write($"OPEN {id} {owner}");
        return account;
    }

    public Account Get(string id) =>
        _accounts.TryGetValue(id, out var account) ? account : throw new AccountNotFoundException(id);

    public void Deposit(string id, decimal amount)
    {
        Get(id).Deposit(amount);
        log.Write($"DEPOSIT {id} {amount}");
    }

    public void Withdraw(string id, decimal amount)
    {
        try
        {
            Get(id).Withdraw(amount);
            log.Write($"WITHDRAW {id} {amount}");
        }
        catch (BankException ex)
        {
            log.Write($"FAILED WITHDRAW {id} {amount}: {ex.Message}");
            throw;   // FIX 5: throw; ולא throw ex; — שומר את ה-stack trace המקורי
        }
    }

    public void Transfer(string fromId, string toId, decimal amount)
    {
        var from = Get(fromId);
        var to = Get(toId);
        if (ReferenceEquals(from, to)) throw new BankException("Cannot transfer to the same account");

        from.Withdraw(amount);
        try
        {
            to.Deposit(amount);   // FIX 6: to ולא from
        }
        catch
        {
            from.Deposit(amount);   // rollback — אם ההפקדה נכשלה, מחזירים את הכסף
            throw;
        }
        log.Write($"TRANSFER {fromId} -> {toId} {amount}");
    }

    public IEnumerable<Account> Accounts => _accounts.Values;
}
