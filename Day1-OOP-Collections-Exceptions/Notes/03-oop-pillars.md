# מודול 03 — עקרונות OOP: אנקפסולציה, ירושה, פולימורפיזם, ממשקים והפשטה

במודול הקודם למדנו לבנות מחלקה. עכשיו נלמד לבנות **מערכת** של מחלקות שמשתפות פעולה בלי לדעת יותר מדי אחת על השנייה. ארבעת העקרונות הקלאסיים של OOP הם לא רשימה לשינון אלא ארבע תשובות לשאלה אחת: איך לכתוב קוד שאפשר לשנות בלי לשבור? נלמד גם את הכלים של C# עצמה: access modifiers, `virtual`/`override`, `abstract`, `interface`, pattern matching, ודריסת `ToString`/`Equals`.

## אנקפסולציה — הסתרת מידע

אנקפסולציה אומרת: **המצב של אובייקט משתנה רק דרך המתודות שלו**. מי שמשתמש במחלקה רואה "פנים" (API) קטנות וברורות, ולא יכול להכניס אותה למצב לא חוקי. אם היתרה היא `private set`, אין דרך ליצור חשבון עם יתרה שלילית מבלי לעבור ב-`Withdraw` שבודק. היתרון האמיתי: כשתרצו לשנות את המימוש (למשל לשמור היסטוריה במקום מספר), תשנו במקום אחד.

Access modifiers ב-C#:

| modifier | נגיש מ... |
|---|---|
| `public` | כל מקום |
| `private` | רק המחלקה עצמה (ברירת מחדל לחברים) |
| `protected` | המחלקה ויורשיה |
| `internal` | כל הקוד באותו assembly (פרויקט) — ברירת מחדל למחלקות |
| `protected internal` | יורשים או אותו assembly |
| `private protected` | יורשים בתוך אותו assembly |

כלל: התחילו מ-`private` והרחיבו רק כשצריך. פחות שטח ציבורי = פחות דברים שיכולים להישבר.

## ירושה — "הוא סוג של"

ירושה מאפשרת למחלקה (נגזרת) לקבל את כל החברים של מחלקה אחרת (בסיס) ולהוסיף או לשנות. השתמשו בה כשיש יחס אמיתי של **is-a**: מנהל הוא עובד, ריבוע הוא מלבן. C# תומכת בירושה יחידה בלבד — מחלקה יורשת ממחלקה אחת (אבל יכולה לממש כמה ממשקים).

```csharp
class Employee(int id, string name)
{
    public int Id { get; } = id;
    public string Name { get; } = name;

    public virtual decimal MonthlyPay() => 0;                  // virtual — מותר לדרוס
    public virtual string Describe() => $"#{Id} {Name}";
}

class Manager(int id, string name, decimal salary, decimal bonus) : Employee(id, name)
{
    public override decimal MonthlyPay() => salary / 12 + bonus;
    public override string Describe() => base.Describe() + " (manager)";   // base — המימוש של האב
}

sealed class Intern(int id, string name) : Employee(id, name)   // sealed — אי אפשר לרשת ממנה
{
    public override decimal MonthlyPay() => 3_000;
}
```

- `virtual` במחלקת הבסיס אומר "יורשים רשאים להחליף את המימוש". בלי `virtual`, המתודה קבועה.
- `override` ביורש מחליף. אם תכתבו מתודה באותו שם בלי `override`, תקבלו אזהרה על **hiding** (`new`) — כמעט תמיד טעות.
- `base.X()` קורא למימוש של האב — שימושי כשרוצים "להרחיב" ולא "להחליף".
- `sealed` על מחלקה מונע ירושה; על `override` מונע דריסה נוספת.
- הבנאי של היורש חייב לקרוא לבנאי של האב (`: base(...)` או דרך primary constructor כמו למעלה).

## מחלקות אבסטרקטיות

לפעמים למחלקת הבסיס אין משמעות בפני עצמה: מה זה "סתם צורה"? `abstract class` לא ניתנת ליצירה (`new Shape()` לא מתקמפל), ויכולה להכיל מתודות `abstract` — הצהרה בלי גוף שכל יורש **חייב** לממש.

```csharp
abstract class Shape
{
    public abstract double Area();                        // חובה לממש
    public virtual string Describe() => $"{GetType().Name} with area {Area():F2}";   // משותף
}

class Circle(double r) : Shape
{
    public override double Area() => Math.PI * r * r;
}
```

`Describe` משתמש ב-`Area` שעדיין לא קיים — זה בסדר: בזמן ריצה תיקרא הגרסה של היורש. זו תבנית חזקה: הבסיס מגדיר את השלד, היורשים ממלאים את החורים.

## פולימורפיזם — קריאה אחת, התנהגויות רבות

פולימורפיזם ("ריבוי צורות") הוא התוצאה של `virtual`/`override`: משתנה מטיפוס הבסיס יכול להחזיק כל יורש, והקריאה למתודה מופנית למימוש של הטיפוס **האמיתי** בזמן ריצה.

```csharp
List<Shape> shapes = [new Circle(1), new Rectangle(2, 3), new Square(2)];
foreach (var s in shapes)
    Console.WriteLine(s.Describe());          // כל אחד לפי הטיפוס שלו
double total = shapes.Sum(s => s.Area());
```

הקוד שמדפיס לא יודע ולא צריך לדעת אילו צורות קיימות. כשתוסיפו `Triangle` מחר, הלולאה הזו לא תשתנה. זה ה-**Open/Closed Principle**: פתוח להרחבה, סגור לשינוי. ההפך מזה הוא `switch` על "סוג" בכל מקום — כל צורה חדשה דורשת לעדכן את כל ה-switch-ים.

## ממשקים — חוזה בלי מימוש

ממשק (`interface`) מגדיר **מה** אובייקט יודע לעשות, בלי לומר **איך**. מחלקה יכולה לממש כמה ממשקים, וממשק לא כופה היררכיה: `Contractor` ו-`Employee` יכולים שניהם להיות `IPayable` בלי אב משותף.

```csharp
interface IPayable
{
    string PayeeName { get; }
    decimal MonthlyPay();
    string PaySlip() => $"{PayeeName}: {MonthlyPay():N2}";   // default member (C# 8+)
}

class Contractor(string company, decimal fee) : IPayable
{
    public string PayeeName => company;
    public decimal MonthlyPay() => fee;
}

List<IPayable> payroll = [new Manager(1, "Noa", 360_000, 2_000), new Contractor("Acme", 12_000)];
foreach (var p in payroll) Console.WriteLine(p.PaySlip());
```

- שמות ממשקים מתחילים ב-`I`. חברי ממשק הם ציבוריים אוטומטית.
- **Default interface members** מאפשרים לתת מימוש ברירת מחדל — שימושי כדי להוסיף מתודה לממשק קיים בלי לשבור מממשים. הם נגישים רק דרך טיפוס הממשק. אל תהפכו את זה לתחליף למחלקת בסיס.
- ממשקים נפוצים ב-.NET: `IComparable<T>` (מיון), `IEquatable<T>`, `IEnumerable<T>` (foreach), `IDisposable` (שחרור משאבים).

### ממשק או מחלקה אבסטרקטית?

| | `abstract class` | `interface` |
|---|---|---|
| ירושה | אחת בלבד | כמה שרוצים |
| מכיל | שדות, בנאים, מימוש משותף | חוזה (+ default members) |
| קשר | is-a חזק, קוד משותף | can-do, יכולת |
| דוגמה | `Shape`, `Employee` | `IPayable`, `IDisposable`, `IComparable` |

הרבה פעמים משלבים: ממשק לחוזה, ומחלקה אבסטרקטית שמממשת אותו חלקית לנוחות היורשים.

## הפשטה — לחשוף רק מה שחשוב

הפשטה היא הרעיון שמאחורי ממשקים ומחלקות אבסטרקטיות: **הקוד תלוי בחוזה, לא במימוש**. `Payroll` תלוי ב-`IPayable`, לא ב-`Manager`. התוצאה: אפשר להחליף מימוש (לוגר לקונסולה → לוגר לקובץ → לוגר מזויף לבדיקות) בלי לגעת בקוד שמשתמש בו. זה הבסיס ל-Dependency Injection שנפגוש ביום 2.

## Composition over inheritance

ירושה היא הקשר החזק ביותר בין שתי מחלקות — היורש תלוי בכל פרט של האב. לעיתים קרובות עדיף **הרכבה**: "יש לו" במקום "הוא". במקום `class OrderService : ConsoleLogger` (שירות הזמנות הוא לוגר?!) כותבים `class OrderService(ILogger logger)` — יש לו לוגר, וכל לוגר יתאים.

```csharp
interface ILogger { void Log(string message); }
class ConsoleLogger : ILogger { public void Log(string m) => Console.WriteLine(m); }

class OrderService(ILogger logger)          // has-a
{
    public void Place(string id) { logger.Log($"placing {id}"); }
}
```

כלל אצבע: ירושה למודל דומיין אמיתי (צורות, עובדים) ולמחלקות framework שנועדו לכך; הרכבה לכל השאר.

## `is`, `as` ו-pattern matching

לפעמים כן צריך לדעת את הטיפוס האמיתי. הכלים המודרניים:

```csharp
if (shape is Circle c) Console.WriteLine(c.Radius);     // בדיקה + המרה + משתנה חדש
var rect = shape as Rectangle;                           // null אם לא מתאים (לא זורק)
var forced = (Rectangle)shape;                           // זורק InvalidCastException אם לא מתאים

string label = shape switch
{
    Circle { Radius: > 10 } => "big circle",             // property pattern
    Circle c => $"circle r={c.Radius}",
    Rectangle { Width: var w, Height: var h } when w == h => "square-ish",
    null => "nothing",
    _ => "other",
};
```

הסדר ב-`switch` חשוב: מקרים ספציפיים לפני כלליים, אחרת המהדר יתלונן על case שלא ניתן להגיע אליו. ואם אתם מוצאים את עצמכם עושים `switch` על טיפוסים בכל מקום — אולי חסרה לכם מתודה `virtual`.

## `ToString`, `Equals`, `GetHashCode`

כל מחלקה יורשת מ-`object` שלוש מתודות שכדאי להכיר:

- **`ToString()`** — ברירת המחדל מחזירה את שם הטיפוס. דרסו כדי לקבל הדפסה מועילה ב-`Console.WriteLine(obj)` ובדיבאגר.
- **`Equals(object)`** ו-**`GetHashCode()`** — ברירת המחדל משווה הפניות. אם אובייקטים "שווים לפי תוכן" (למשל `Money(10, "ILS")`), דרסו את **שניהם יחד**: `Dictionary` ו-`HashSet` משתמשים ב-`GetHashCode` כדי למצוא את ה"דלי" ואז ב-`Equals` — אם הם לא עקביים, האוסף יתנהג מוזר.

```csharp
class Money(decimal amount, string currency)
{
    public decimal Amount => amount;
    public string Currency => currency;

    public override bool Equals(object? obj) =>
        obj is Money o && o.Amount == Amount && o.Currency == Currency;
    public override int GetHashCode() => HashCode.Combine(Amount, Currency);
    public override string ToString() => $"{Amount} {Currency}";
}
```

...או פשוט `record Money(decimal Amount, string Currency);` — והמהדר עושה את זה בשבילכם. זו הסיבה העיקרית ש-records קיימים.

## טעויות נפוצות

- **ירושה בשביל שימוש חוזר בקוד** בלי יחס is-a. אם `Car : Engine` נשמע מוזר — זה מוזר. הרכבה.
- **שכחת `virtual`** — ואז `override` לא מתקמפל; או שכחת `override` — ואז המתודה החדשה "מסתירה" ולא דורסת, והפולימורפיזם לא עובד.
- **היררכיות עמוקות** (5 רמות) — קשה לעקוב. 2–3 רמות זה כמעט תמיד מספיק.
- **`Equals` בלי `GetHashCode`** — המהדר מזהיר (CS0659) בצדק.
- **`(Circle)shape` בלי בדיקה** — `InvalidCastException`. העדיפו `is` pattern.
- **ממשק עם 15 מתודות** — מממשים נאלצים לממש הכל. ממשקים קטנים וממוקדים (Interface Segregation, מודול 07).
- **`protected` שדות** — יורשים תלויים במימוש הפנימי. עדיף `protected` מתודות/properties.

## לסיכום

- **אנקפסולציה**: מצב פרטי, API ציבורי קטן. `private` כברירת מחדל.
- **ירושה**: `virtual`/`override`/`base`/`abstract`/`sealed`. רק ליחסי is-a אמיתיים; הרכבה לכל השאר.
- **פולימורפיזם**: משתנה מטיפוס הבסיס, התנהגות לפי הטיפוס האמיתי. מחליף `switch` על סוגים.
- **ממשקים**: חוזה בלי מימוש, ריבוי ממשקים, default members. **הפשטה**: תלות בחוזה, לא במימוש.
- `is`/`as`/`switch` patterns כשצריך את הטיפוס; `ToString`/`Equals`/`GetHashCode` — או record.

## קריאה נוספת

- [Object-Oriented programming (C#)](https://learn.microsoft.com/dotnet/csharp/fundamentals/object-oriented/)
- [Inheritance](https://learn.microsoft.com/dotnet/csharp/fundamentals/object-oriented/inheritance)
- [Polymorphism](https://learn.microsoft.com/dotnet/csharp/fundamentals/object-oriented/polymorphism)
- [Interfaces](https://learn.microsoft.com/dotnet/csharp/fundamentals/types/interfaces)
- [Access modifiers](https://learn.microsoft.com/dotnet/csharp/programming-guide/classes-and-structs/access-modifiers)
- [Pattern matching](https://learn.microsoft.com/dotnet/csharp/fundamentals/functional/pattern-matching)
- [Equality comparisons](https://learn.microsoft.com/dotnet/csharp/programming-guide/statements-expressions-operators/equality-comparisons)
