using System.Diagnostics;
using Day2.Lab1;

// Lab 1 — Starter: עיבוד הזמנות סדרתי מול מקבילי
Console.OutputEncoding = System.Text.Encoding.UTF8;

int count = args.Length > 0 && int.TryParse(args[0], out var c) ? c : 40;
var orders = OrderProcessor.GenerateOrders(count);
Console.WriteLine($"Processing {orders.Count} orders on {Environment.ProcessorCount} cores\n");

// TODO (שלב 4): צרו CancellationTokenSource ורשמו ל-Console.CancelKeyPress
// TODO (שלב 5): צרו Progress<int> שמדפיס התקדמות

var sw = Stopwatch.StartNew();
var seqResults = Runner.RunSequential(orders);
Console.WriteLine($"Sequential      : {sw.ElapsedMilliseconds} ms, {seqResults.Count} results");

// TODO (שלב 2): הריצו RunWhenAllAsync ומדדו
// TODO (שלב 3): הריצו RunParallelForEach ומדדו
// TODO (שלב 6): הדפיסו טבלת סיכום עם האצה יחסית

static class Runner
{
    public static List<OrderResult> RunSequential(List<Order> orders)
    {
        var results = new List<OrderResult>();
        foreach (var order in orders)
        {
            var sw = Stopwatch.StartNew();
            var checksum = OrderProcessor.ProcessImage(order);
            OrderProcessor.SendConfirmationAsync(order).GetAwaiter().GetResult();   // סדרתי לחלוטין (רע — רק כקו בסיס)
            results.Add(new OrderResult(order.Id, checksum, sw.Elapsed));
        }
        return results;
    }

    public static Task<List<OrderResult>> RunWhenAllAsync(List<Order> orders)
    {
        // TODO (שלב 2): לכל הזמנה Task שמעבד תמונה ואז שולח אישור; await Task.WhenAll
        throw new NotImplementedException();
    }

    public static Task<List<OrderResult>> RunParallelForEachAsync(List<Order> orders)
    {
        // TODO (שלב 3): Parallel.ForEach לעיבוד תמונות (ConcurrentBag לתוצאות), ואז WhenAll לאישורים
        throw new NotImplementedException();
    }
}
