namespace Day1.Exercises;

public static class Ex03
{
    class Rectangle
    {
        public double Width { get; }
        public double Height { get; }
        public double Area => Width * Height;

        public Rectangle(double width, double height)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(width);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(height);
            Width = width;
            Height = height;
        }

        public Rectangle Scale(double factor) => new(Width * factor, Height * factor);   // אובייקט חדש — לא משנה את הקיים
        public override string ToString() => $"Rectangle {Width}x{Height} (area {Area})";
    }

    public static void Run()
    {
        var r = new Rectangle(3, 4);
        var big = r.Scale(2);
        Console.WriteLine(r);
        Console.WriteLine(big);
        try { _ = new Rectangle(0, 5); }
        catch (ArgumentOutOfRangeException ex) { Console.WriteLine($"rejected: {ex.ParamName}"); }
    }
}

public static class Ex04
{
    record Point(int X, int Y);

    class PointClass
    {
        public static int Created { get; private set; }   // משותף לכל המופעים
        public int X { get; }
        public int Y { get; }

        public PointClass(int x, int y)
        {
            X = x;
            Y = y;
            Created++;
        }

        public static PointClass Origin() => new(0, 0);
        public override string ToString() => $"({X},{Y})";
    }

    public static void Run()
    {
        var r1 = new Point(1, 2);
        var r2 = new Point(1, 2);
        // record: שוויון לפי ערך — המהדר יוצר Equals/GetHashCode/== לפי כל ה-properties
        Console.WriteLine($"record:  r1 == r2 → {r1 == r2}, Equals → {r1.Equals(r2)}");

        var c1 = new PointClass(1, 2);
        var c2 = new PointClass(1, 2);
        // class: שוויון לפי הפניה — שני אובייקטים שונים בזיכרון
        Console.WriteLine($"class:   c1 == c2 → {c1 == c2}, Equals → {c1.Equals(c2)}");

        var origin = PointClass.Origin();
        Console.WriteLine($"Created = {PointClass.Created}, origin = {origin}");

        var moved = r1 with { X = r1.X + 10 };
        Console.WriteLine($"with: {r1} → {moved}");
    }
}
