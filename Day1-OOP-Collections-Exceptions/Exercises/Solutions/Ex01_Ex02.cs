namespace Day1.Exercises;

public static class Ex01
{
    static double CelsiusToFahrenheit(double c) => c * 9 / 5 + 32;
    static double FahrenheitToCelsius(double f) => (f - 32) * 5 / 9;

    public static void Run()
    {
        Console.WriteLine($"{"C",6} {"F",8}");
        for (int c = -10; c <= 40; c += 10)
            Console.WriteLine($"{c,6:F1} {CelsiusToFahrenheit(c),8:F1}");
        Console.WriteLine($"100F = {FahrenheitToCelsius(100):F1}C");
    }
}

public static class Ex02
{
    public static void Run()
    {
        for (int n = 1; n <= 20; n++)
        {
            string s = (n % 3, n % 5) switch
            {
                (0, 0) => "FizzBuzz",
                (0, _) => "Fizz",
                (_, 0) => "Buzz",
                _ => n.ToString(),
            };
            Console.Write(s + (n < 20 ? ", " : "\n"));
        }
    }
}
