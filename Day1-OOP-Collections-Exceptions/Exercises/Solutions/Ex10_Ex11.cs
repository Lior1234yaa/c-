namespace Day1.Exercises;

public static class Ex10
{
    public static void Run()
    {
        int[] nums = [5, 3, 8, 1, 9, 2, 7, 4, 6, 10];
        Console.WriteLine($"sum of even squares: {nums.Where(n => n % 2 == 0).Sum(n => n * n)}");
        Console.WriteLine($"top 3: {string.Join(", ", nums.OrderByDescending(n => n).Take(3))}");
        Console.WriteLine($"any > 9: {nums.Any(n => n > 9)}");
        Console.WriteLine($"avg of odds: {nums.Where(n => n % 2 != 0).Average():F2}");
        Console.WriteLine($"sorted: {string.Join(" ", nums.Order())}");
    }
}

public static class Ex11
{
    static Func<int, int> Compose(Func<int, int> f, Func<int, int> g) => x => g(f(x));

    class Counter(int threshold)
    {
        public int Value { get; private set; }
        public int Threshold { get; } = threshold;
        public event EventHandler<int>? ThresholdReached;

        public void Increment()
        {
            Value++;
            if (Value == Threshold) ThresholdReached?.Invoke(this, Value);
        }
    }

    public static void Run()
    {
        var addThenDouble = Compose(x => x + 1, x => x * 2);
        var doubleThenAdd = Compose(x => x * 2, x => x + 1);
        Console.WriteLine($"(5+1)*2 = {addThenDouble(5)}, 5*2+1 = {doubleThenAdd(5)}");

        var counter = new Counter(threshold: 3);
        counter.ThresholdReached += (sender, value) => Console.WriteLine($"  threshold reached at {value}!");
        for (int i = 0; i < 5; i++)
        {
            counter.Increment();
            Console.WriteLine($"value = {counter.Value}");
        }
    }
}
