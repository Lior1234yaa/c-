using Day2.Lab2;

// Lab 2 — Solution: race condition -> lock -> Interlocked -> ConcurrentDictionary; deadlock -> lock ordering
Console.OutputEncoding = System.Text.Encoding.UTF8;
const int Accounts = 10;
const decimal Initial = 1000m;

Console.WriteLine("=== Stress test: 8 threads x 20,000 random transfers (invariant: total money is constant) ===");
StressTest.Run(new UnsafeBank(Accounts, Initial));
StressTest.Run(new LockBank(Accounts, Initial));
StressTest.Run(new InterlockedBank(Accounts, Initial));
StressTest.Run(new ConcurrentBank(Accounts, Initial));
StressTest.Run(new OrderedLockBank(Accounts, Initial, ordered: true));

Console.WriteLine("\n=== Deadlock demo: thread A transfers 1->2, thread B transfers 2->1, both in a loop ===");
DeadlockDemo.Run(new OrderedLockBank(Accounts, Initial, ordered: false) { SlowMode = true });
DeadlockDemo.Run(new OrderedLockBank(Accounts, Initial, ordered: true) { SlowMode = true });

static class DeadlockDemo
{
    public static void Run(IBank bank)
    {
        // IsBackground = true: אם באמת נתקעו, לא ימנעו מהתהליך להסתיים
        var a = new Thread(() => { for (int i = 0; i < 20; i++) bank.Transfer(1, 2, 1); }) { IsBackground = true, Name = "A" };
        var b = new Thread(() => { for (int i = 0; i < 20; i++) bank.Transfer(2, 1, 1); }) { IsBackground = true, Name = "B" };
        a.Start(); b.Start();

        // watchdog: Join עם timeout במקום להיתקע יחד איתם
        bool finished = a.Join(TimeSpan.FromSeconds(3)) & b.Join(TimeSpan.FromSeconds(1));
        Console.WriteLine(finished
            ? $"  {bank.Name,-40} finished OK, balances: #1={bank.GetBalance(1)} #2={bank.GetBalance(2)}"
            : $"  {bank.Name,-40} DEADLOCK! (threads still blocked after 3s — A holds lock 1 and waits for 2, B holds 2 and waits for 1)");
    }
}
