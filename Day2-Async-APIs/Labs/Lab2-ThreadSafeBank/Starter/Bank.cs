using System.Collections.Concurrent;

namespace Day2.Lab2;

public interface IBank
{
    string Name { get; }
    int AccountCount { get; }
    bool Transfer(int from, int to, decimal amount);
    decimal GetBalance(int id);
    decimal TotalMoney { get; }
    int FailedTransfers { get; }
}

/// <summary>שלב 1: בלי שום סנכרון — race condition.</summary>
public class UnsafeBank(int accounts, decimal initial) : IBank
{
    private readonly Dictionary<int, decimal> _balances = Enumerable.Range(0, accounts).ToDictionary(i => i, _ => initial);
    private int _failed;

    public string Name => "UnsafeBank";
    public int AccountCount => _balances.Count;
    public int FailedTransfers => _failed;

    public bool Transfer(int from, int to, decimal amount)
    {
        if (_balances[from] < amount) { _failed++; return false; }
        _balances[from] -= amount;      // קריאה + חישוב + כתיבה — לא אטומי!
        _balances[to] += amount;
        return true;
    }

    public decimal GetBalance(int id) => _balances[id];
    public decimal TotalMoney => _balances.Values.Sum();
}

/// <summary>שלב 2: נעילה גלובלית אחת.</summary>
public class LockBank(int accounts, decimal initial) : IBank
{
    private readonly decimal[] _balances = Enumerable.Repeat(initial, accounts).ToArray();   // TODO: החליפו במבנה המתאים

    public string Name => "LockBank";
    public int AccountCount => accounts;
    public int FailedTransfers => throw new NotImplementedException();
    public bool Transfer(int from, int to, decimal amount) => throw new NotImplementedException();   // TODO
    public decimal GetBalance(int id) => _balances[id];   // TODO: האם זה בטוח?
    public decimal TotalMoney => _balances.Sum();          // TODO: האם זה בטוח?
}

/// <summary>שלב 3: Interlocked על long[] (אגורות).</summary>
public class InterlockedBank(int accounts, decimal initial) : IBank
{
    private readonly decimal[] _balances = Enumerable.Repeat(initial, accounts).ToArray();   // TODO: החליפו במבנה המתאים

    public string Name => "InterlockedBank";
    public int AccountCount => accounts;
    public int FailedTransfers => throw new NotImplementedException();
    public bool Transfer(int from, int to, decimal amount) => throw new NotImplementedException();   // TODO: TryWithdraw עם CompareExchange
    public decimal GetBalance(int id) => _balances[id];   // TODO: האם זה בטוח?
    public decimal TotalMoney => _balances.Sum();          // TODO: האם זה בטוח?
}

/// <summary>שלב 4: ConcurrentDictionary.</summary>
public class ConcurrentBank(int accounts, decimal initial) : IBank
{
    private readonly decimal[] _balances = Enumerable.Repeat(initial, accounts).ToArray();   // TODO: החליפו במבנה המתאים

    public string Name => "ConcurrentBank";
    public int AccountCount => accounts;
    public int FailedTransfers => throw new NotImplementedException();
    public bool Transfer(int from, int to, decimal amount) => throw new NotImplementedException();   // TODO: AddOrUpdate
    public decimal GetBalance(int id) => _balances[id];   // TODO: האם זה בטוח?
    public decimal TotalMoney => _balances.Sum();          // TODO: האם זה בטוח?
}

/// <summary>שלבים 5–6: נעילה לכל חשבון. בלי סדר נעילה = deadlock; עם סדר = בטוח ומהיר.</summary>
public class OrderedLockBank(int accounts, decimal initial, bool ordered) : IBank
{
    private readonly decimal[] _balances = Enumerable.Repeat(initial, accounts).ToArray();
    private readonly Lock[] _locks = Enumerable.Range(0, accounts).Select(_ => new Lock()).ToArray();   // נעילה לכל חשבון

    public string Name => ordered ? "OrderedLockBank" : "PerAccountLockBank (unordered)";
    public int AccountCount => accounts;
    public int FailedTransfers => throw new NotImplementedException();
    public bool Transfer(int from, int to, decimal amount) => throw new NotImplementedException();   // TODO: lock(from) ואז lock(to); עם ordered — לפי Id
    public decimal GetBalance(int id) => _balances[id];   // TODO: האם זה בטוח?
    public decimal TotalMoney => _balances.Sum();          // TODO: האם זה בטוח?
}
