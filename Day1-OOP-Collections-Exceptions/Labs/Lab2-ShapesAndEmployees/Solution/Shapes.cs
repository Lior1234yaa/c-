namespace Day1.Lab2;

// ============ חלק א' — צורות ============

public abstract class Shape
{
    public abstract string Name { get; }
    public abstract double Area();
    public abstract double Perimeter();

    public override string ToString() => $"{Name}: area={Area():F2}, perimeter={Perimeter():F2}";
}

public class Circle(double radius) : Shape
{
    public double Radius { get; } = radius > 0 ? radius : throw new ArgumentOutOfRangeException(nameof(radius));
    public override string Name => "Circle";
    public override double Area() => Math.PI * Radius * Radius;
    public override double Perimeter() => 2 * Math.PI * Radius;
}

public class Rectangle(double width, double height) : Shape
{
    public double Width { get; } = width;
    public double Height { get; } = height;
    public override string Name => "Rectangle";
    public override double Area() => Width * Height;
    public override double Perimeter() => 2 * (Width + Height);
}

// sealed: ריבוע הוא "עלה" בהיררכיה — אי אפשר לרשת ממנו
public sealed class Square(double side) : Rectangle(side, side)
{
    public override string Name => "Square";
}

public class Triangle : Shape
{
    public double A { get; }
    public double B { get; }
    public double C { get; }

    public Triangle(double a, double b, double c)
    {
        if (a <= 0 || b <= 0 || c <= 0) throw new ArgumentOutOfRangeException("sides must be positive");
        if (a + b <= c || a + c <= b || b + c <= a)
            throw new ArgumentException($"Sides {a},{b},{c} violate the triangle inequality");
        (A, B, C) = (a, b, c);
    }

    public override string Name => "Triangle";
    public override double Perimeter() => A + B + C;
    public override double Area()
    {
        double s = Perimeter() / 2;   // נוסחת הרון
        return Math.Sqrt(s * (s - A) * (s - B) * (s - C));
    }
}
