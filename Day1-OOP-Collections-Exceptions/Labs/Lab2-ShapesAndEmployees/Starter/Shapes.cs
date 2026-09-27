namespace Day1.Lab2;

// ============ חלק א' — צורות (חימום, ~15 דקות) ============

// TODO 1: הפכו את Shape למחלקה אבסטרקטית עם:
//   - property אבסטרקטי Name (string)
//   - מתודות אבסטרקטיות Area() ו-Perimeter() שמחזירות double
//   - override ל-ToString שמחזיר למשל: "Circle: area=3.14, perimeter=6.28"
public class Shape
{
}

// TODO 2: Circle(radius), Rectangle(width, height), Triangle(a, b, c) — יורשים מ-Shape.
//   Triangle: ודאו בבנאי שהצלעות מקיימות אי-שוויון המשולש, אחרת ArgumentException.
//   שטח משולש לפי נוסחת הרון: s=(a+b+c)/2, area=sqrt(s(s-a)(s-b)(s-c)).
// TODO 3: Square יורש מ-Rectangle (צלע אחת), ומסומן sealed.
