namespace Day1.Exercises;

public static class Ex12
{
    class ValidationException(string input, string message) : Exception(message)
    {
        public string Input { get; } = input;
    }

    static int ReadInt(string prompt, int min, int max)
    {
        while (true)
        {
            Console.Write($"{prompt} [{min}-{max}]: ");
            var line = Console.ReadLine() ?? throw new EndOfStreamException("no more input");
            if (int.TryParse(line, out int value) && value >= min && value <= max) return value;
            Console.WriteLine($"  '{line}' is not a number between {min} and {max}, try again");
        }
    }

    static void Validate(int age)
    {
        if (age is < 0 or > 120) throw new ValidationException(age.ToString(), $"age {age} is out of range");
    }

    public static void Run()
    {
        try
        {
            int age = ReadInt("age", -50, 200);   // טווח רחב בכוונה כדי ש-Validate יהיה משמעותי
            Validate(age);
            Console.WriteLine($"valid age: {age}");
        }
        catch (ValidationException ex)
        {
            Console.WriteLine($"validation failed for '{ex.Input}': {ex.Message}");
        }
        catch (EndOfStreamException)
        {
            Console.WriteLine("input ended — bye");
        }
    }
}

public static class Ex13
{
    class Step(string name) : IDisposable
    {
        public Step Init() { Console.WriteLine($"  ctor {name}"); return this; }
        public void Dispose() => Console.WriteLine($"  dispose {name}");
    }

    static bool Log(Exception ex) { Console.WriteLine($"  filter saw: {ex.GetType().Name}"); return false; }

    public static void Run()
    {
        // צפוי: ctor A, ctor B, filter, dispose B, catch, finally, dispose A
        // (הפילטר רץ לפני ש-B משוחרר! ה-using של B הוא try/finally פנימי, ופילטרים
        //  רצים בשלב הראשון של החיפוש אחר catch — לפני unwinding)
        using var a = new Step("A").Init();
        try
        {
            using var b = new Step("B").Init();
            throw new InvalidOperationException();
        }
        catch (Exception ex) when (Log(ex)) { Console.WriteLine("  never here (filter returned false)"); }
        catch { Console.WriteLine("  catch"); }
        finally { Console.WriteLine("  finally"); }
        Console.WriteLine("  end of method (A disposed after this line)");
    }
}

public static class Ex14
{
    // ---------- לפני ----------
    static double c(double a, int t, bool m)
    {
        double r = 0;
        if (t == 1) { r = a * 0.9; if (m) r = r - 5; }
        else if (t == 2) { r = a * 0.8; if (m) r = r - 5; }
        else { r = a; if (m) r = r - 5; }
        if (r < 0) r = 0;
        return r * 1.18;
    }

    // ---------- אחרי ----------
    enum CustomerType { Regular = 0, Silver = 1, Gold = 2 }

    const double VatRate = 1.18;
    const double MemberDiscount = 5;
    const double SilverFactor = 0.9;
    const double GoldFactor = 0.8;

    static double CalculatePriceWithVat(double basePrice, CustomerType customer, bool isMember)
    {
        double price = ApplyCustomerDiscount(basePrice, customer);
        if (isMember) price -= MemberDiscount;
        price = Math.Max(price, 0);
        return price * VatRate;
    }

    static double ApplyCustomerDiscount(double price, CustomerType customer) => customer switch
    {
        CustomerType.Silver => price * SilverFactor,
        CustomerType.Gold => price * GoldFactor,
        _ => price,
    };

    public static void Run()
    {
        foreach (var (amount, type, member) in new[] { (100.0, 0, false), (100.0, 1, true), (100.0, 2, true), (3.0, 2, true) })
        {
            double before = c(amount, type, member);
            double after = CalculatePriceWithVat(amount, (CustomerType)type, member);
            Console.WriteLine($"amount={amount,5} type={(CustomerType)type,-7} member={member,-5} before={before:F2} after={after:F2} same={before == after}");
        }
    }
}
