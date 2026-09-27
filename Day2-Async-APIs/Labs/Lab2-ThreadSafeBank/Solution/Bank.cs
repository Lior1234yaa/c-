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

/// <summary>שלב 2: נעילה גלובלית אחת. נכון, פשוט, אבל כל ההעברות סדרתיות.</summary>
public class LockBank(int accounts, decimal initial) : IBank
{
    private readonly decimal[] _balances = Enumerable.Repeat(initial, accounts).ToArray();
    private readonly Lock _lock = new();
    private int _failed;

    public string Name => "LockBank (global lock)";
    public int AccountCount => accounts;
    public int FailedTransfers => _failed;

    public bool Transfer(int from, int to, decimal amount)
    {
        lock (_lock)
        {
            if (_balances[from] < amount) { _failed++; return false; }   // בדיקה + עדכון באותה נעילה
            _balances[from] -= amount;
            _balances[to] += amount;
            return true;
        }
    }

    public decimal GetBalance(int id) { lock (_lock) return _balances[id]; }
    public decimal TotalMoney { get { lock (_lock) return _balances.Sum(); } }
}

/// <summary>שלב 3: Interlocked על long[] (אגורות). מהיר מאוד; בדיקת יתרה דרך לולאת CompareExchange.</summary>
public class InterlockedBank(int accounts, decimal initial) : IBank
{
    private readonly long[] _cents = Enumerable.Repeat((long)(initial * 100), accounts).ToArray();
    private int _failed;

    public string Name => "InterlockedBank";
    public int AccountCount => accounts;
    public int FailedTransfers => _failed;

    public bool Transfer(int from, int to, decimal amount)
    {
        long cents = (long)(amount * 100);
        if (!TryWithdraw(from, cents)) { Interlocked.Increment(ref _failed); return false; }
        Interlocked.Add(ref _cents[to], cents);
        return true;
    }

    // "בדוק שיש מספיק ואז הפחת" — אטומי בעזרת CompareExchange: מנסים שוב אם מישהו שינה את הערך בינתיים
    private bool TryWithdraw(int id, long cents)
    {
        while (true)
        {
            long current = Volatile.Read(ref _cents[id]);
            if (current < cents) return false;
            if (Interlocked.CompareExchange(ref _cents[id], current - cents, current) == current) return true;
        }
    }

    public decimal GetBalance(int id) => Volatile.Read(ref _cents[id]) / 100m;
    public decimal TotalMoney => _cents.Sum(c => Volatile.Read(ref c)) / 100m;
}

/// <summary>שלב 4: ConcurrentDictionary. כל AddOrUpdate אטומי למפתח שלו -> הסכום נשמר, אבל בדיקת יתרה דורשת עבודה נוספת.</summary>
public class ConcurrentBank(int accounts, decimal initial) : IBank
{
    private readonly ConcurrentDictionary<int, decimal> _balances = new(Enumerable.Range(0, accounts).ToDictionary(i => i, _ => initial));
    private int _failed;

    public string Name => "ConcurrentBank";
    public int AccountCount => accounts;
    public int FailedTransfers => _failed;

    public bool Transfer(int from, int to, decimal amount)
    {
        // בדיקת יתרה אטומית: ה-update factory מקבל את הערך הנוכחי ומחזיר אותו ללא שינוי אם אין כיסוי.
        // AddOrUpdate עלול להריץ את ה-factory יותר מפעם אחת בתחרות — לכן הוא חייב להיות "טהור" (בלי תופעות לוואי).
        bool withdrawn = false;
        _balances.AddOrUpdate(from, amount, (_, current) =>
        {
            withdrawn = current >= amount;
            return withdrawn ? current - amount : current;
        });
        if (!withdrawn) { Interlocked.Increment(ref _failed); return false; }
        _balances.AddOrUpdate(to, amount, (_, current) => current + amount);
        return true;
    }

    public decimal GetBalance(int id) => _balances[id];
    public decimal TotalMoney => _balances.Values.Sum();
}

/// <summary>שלבים 5–6: נעילה לכל חשבון. ordered=false -> deadlock אפשרי; ordered=true -> נועלים לפי Id.</summary>
public class OrderedLockBank(int accounts, decimal initial, bool ordered) : IBank
{
    private readonly decimal[] _balances = Enumerable.Repeat(initial, accounts).ToArray();
    private readonly Lock[] _locks = Enumerable.Range(0, accounts).Select(_ => new Lock()).ToArray();
    private int _failed;

    public string Name => ordered ? "OrderedLockBank (per-account, ordered)" : "PerAccountLockBank (unordered)";
    public int AccountCount => accounts;
    public int FailedTransfers => _failed;
    public bool SlowMode { get; set; }   // ל-deadlock demo: מרחיב את חלון הזמן בין שתי הנעילות

    public bool Transfer(int from, int to, decimal amount)
    {
        // סדר נעילה קבוע: תמיד ה-Id הנמוך קודם, בלי קשר לכיוון ההעברה
        (int first, int second) = ordered && from > to ? (to, from) : (from, to);

        lock (_locks[first])
        {
            if (SlowMode) Thread.Sleep(10);
            lock (_locks[second])
            {
                if (_balances[from] < amount) { _failed++; return false; }
                _balances[from] -= amount;
                _balances[to] += amount;
                return true;
            }
        }
    }

    public decimal GetBalance(int id) { lock (_locks[id]) return _balances[id]; }

    public decimal TotalMoney
    {
        get
        {
            // נועלים את כולם לפי סדר — כדי לקבל snapshot עקבי
            decimal Sum(int i) { if (i == accounts) return _balances.Sum(); lock (_locks[i]) return Sum(i + 1); }
            return Sum(0);
        }
    }
}
