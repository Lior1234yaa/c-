using System.Diagnostics;

// תרגיל 1 — שני תהליכונים
static class Ex01
{
    public static Task RunAsync()
    {
        var numbers = new Thread(() => { for (int i = 1; i <= 5; i++) { Console.WriteLine($"  {i}"); Thread.Sleep(100); } });
        var letters = new Thread(() => { for (char c = 'A'; c <= 'E'; c++) { Console.WriteLine($"  {c}"); Thread.Sleep(100); } });
        numbers.Start(); letters.Start();
        numbers.Join(); letters.Join();
        Console.WriteLine("  done (the interleaving changes between runs — the scheduler decides)");
        return Task.CompletedTask;
    }
}

// תרגיל 2 — race condition ותיקונים
static class Ex02
{
    public static Task RunAsync()
    {
        const int Threads = 4, PerThread = 250_000;

        int unsafeCounter = 0, lockedCounter = 0, interlockedCounter = 0;
        var gate = new Lock();

        long Measure(string name, Action body, Func<int> read)
        {
            var sw = Stopwatch.StartNew();
            var ts = Enumerable.Range(0, Threads).Select(_ => new Thread(() => body())).ToList();
            ts.ForEach(t => t.Start()); ts.ForEach(t => t.Join());
            Console.WriteLine($"  {name,-12}: {read():N0} in {sw.ElapsedMilliseconds} ms");
            return sw.ElapsedMilliseconds;
        }

        Console.WriteLine($"  expected    : {Threads * PerThread:N0}");
        Measure("unsafe ++", () => { for (int i = 0; i < PerThread; i++) unsafeCounter++; }, () => unsafeCounter);
        Measure("lock", () => { for (int i = 0; i < PerThread; i++) lock (gate) lockedCounter++; }, () => lockedCounter);
        Measure("Interlocked", () => { for (int i = 0; i < PerThread; i++) Interlocked.Increment(ref interlockedCounter); }, () => interlockedCounter);
        return Task.CompletedTask;
    }
}

// תרגיל 3 — Parallel.For
static class Ex03
{
    public static Task RunAsync()
    {
        const int Max = 2_000_000;
        var sw = Stopwatch.StartNew();
        long seq = 0;
        for (int n = 2; n < Max; n++) if (IsPrime(n)) seq += n;
        var seqMs = sw.ElapsedMilliseconds;
        Console.WriteLine($"  sequential  : sum={seq:N0} in {seqMs} ms");

        sw.Restart();
        long par = 0;
        Parallel.For(2, Max,
            () => 0L,                                        // ערך התחלתי לכל תהליכון (thread-local)
            (n, _, local) => IsPrime(n) ? local + n : local,  // צבירה מקומית — בלי תחרות
            local => Interlocked.Add(ref par, local));        // מיזוג פעם אחת לכל תהליכון
        var parMs = sw.ElapsedMilliseconds;
        Console.WriteLine($"  Parallel.For: sum={par:N0} in {parMs} ms  (speedup x{(double)seqMs / Math.Max(1, parMs):F1} on {Environment.ProcessorCount} cores)");
        return Task.CompletedTask;
    }

    static bool IsPrime(int n)
    {
        if (n < 2) return false;
        if (n % 2 == 0) return n == 2;
        for (int d = 3; d * d <= n; d += 2) if (n % d == 0) return false;
        return true;
    }
}
