using System.Diagnostics;

namespace Day2.Lab2;

public static class StressTest
{
    public static void Run(IBank bank, int threads = 8, int transfersPerThread = 20_000)
    {
        var before = bank.TotalMoney;
        var sw = Stopwatch.StartNew();

        var workers = Enumerable.Range(0, threads).Select(_ => new Thread(() =>
        {
            for (int i = 0; i < transfersPerThread; i++)
            {
                int from = Random.Shared.Next(bank.AccountCount);
                int to = Random.Shared.Next(bank.AccountCount);
                if (from == to) continue;
                bank.Transfer(from, to, Random.Shared.Next(1, 50));
            }
        })).ToList();

        workers.ForEach(t => t.Start());
        workers.ForEach(t => t.Join());
        sw.Stop();

        var after = bank.TotalMoney;
        bool ok = before == after;
        bool negative = Enumerable.Range(0, bank.AccountCount).Any(i => bank.GetBalance(i) < 0);
        Console.WriteLine($"  {bank.Name,-36} {sw.ElapsedMilliseconds,6} ms  total {before:N0} -> {after:N0}  " +
                          $"{(ok ? "OK" : "BROKEN (diff " + (after - before).ToString("N0") + ")")}" +
                          $"{(negative ? "  NEGATIVE BALANCE!" : "")}  failed={bank.FailedTransfers:N0}");
    }
}
