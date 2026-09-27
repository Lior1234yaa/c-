using Day2.Lab2;

// Lab 2 — Starter
Console.OutputEncoding = System.Text.Encoding.UTF8;
const int Accounts = 10;
const decimal Initial = 1000m;

Console.WriteLine("=== Stress test: 8 threads x 20,000 random transfers ===");
StressTest.Run(new UnsafeBank(Accounts, Initial));
// TODO (שלב 2): StressTest.Run(new LockBank(Accounts, Initial));
// TODO (שלב 3): StressTest.Run(new InterlockedBank(Accounts, Initial));
// TODO (שלב 4): StressTest.Run(new ConcurrentBank(Accounts, Initial));
// TODO (שלב 6): StressTest.Run(new OrderedLockBank(Accounts, Initial, ordered: true));

Console.WriteLine("\n=== Deadlock demo ===");
// TODO (שלב 5): DeadlockDemo.Run(new OrderedLockBank(Accounts, Initial, ordered: false));
// TODO (שלב 6): DeadlockDemo.Run(new OrderedLockBank(Accounts, Initial, ordered: true));

static class DeadlockDemo
{
    /// <summary>שני תהליכונים: 1->2 ו-2->1 בו-זמנית. מריץ עם watchdog של 3 שניות.</summary>
    public static void Run(IBank bank)
    {
        // TODO (שלב 5): צרו שני Thread-ים (IsBackground = true) שמבצעים העברות הפוכות בלולאה,
        // חכו להם עם Join(timeout) ודווחו "DEADLOCK" אם לא סיימו תוך 3 שניות.
        throw new NotImplementedException();
    }
}
