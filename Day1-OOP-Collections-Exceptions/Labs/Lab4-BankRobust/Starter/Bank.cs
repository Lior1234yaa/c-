namespace Day1.Lab4;

public class Account(string id, string owner)
{
    public string Id { get; } = id;
    public string Owner { get; } = owner;
    public decimal Balance { get; private set; }
    public List<string> History { get; } = [];

    public void Deposit(decimal amount)
    {
        // BUG? בדקו: אילו סכומים אמורים להידחות?
        if (amount < 0) throw new BankException($"Invalid amount {amount}");
        Balance += amount;
        History.Add($"deposit {amount:N2} -> {Balance:N2}");
    }

    public void Withdraw(decimal amount)
    {
        if (amount <= 0) throw new BankException($"Invalid amount {amount}");
        Balance -= amount;
        if (amount > Balance)
            throw new BankException($"Insufficient funds: balance {Balance:N2}, requested {amount:N2}");
        History.Add($"withdraw {amount:N2} -> {Balance:N2}");
    }

    public IEnumerable<string> RecentHistory(int count)
    {
        // מחזיר את count הפעולות האחרונות
        int start = History.Count - count - 1;
        if (start < 0) start = 0;
        for (int i = start; i < History.Count; i++)
            yield return History[i];
    }
}

public class Bank(TransactionLog log)
{
    private readonly Dictionary<string, Account> _accounts = [];

    public Account Open(string id, string owner)
    {
        id = id.ToUpperInvariant();
        if (_accounts.ContainsKey(id)) throw new BankException($"Account {id} already exists");
        var account = new Account(id, owner);
        _accounts[id] = account;
        log.Write($"OPEN {id} {owner}");
        return account;
    }

    public Account Get(string id)
    {
        if (!_accounts.TryGetValue(id, out var account))
            throw new BankException($"Account {id} not found");
        return account;
    }

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
            throw ex;
        }
    }

    public void Transfer(string fromId, string toId, decimal amount)
    {
        var from = Get(fromId);
        var to = Get(toId);
        from.Withdraw(amount);
        from.Deposit(amount);
        log.Write($"TRANSFER {fromId} -> {toId} {amount}");
    }

    public IEnumerable<Account> Accounts => _accounts.Values;
}
