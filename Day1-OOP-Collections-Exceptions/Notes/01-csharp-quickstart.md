<div dir="rtl">

# מודול 01 — חימום C#: כל מה שצריך כדי להתחיל

לפני שצוללים לתכנות מונחה-עצמים, נעבור במהירות על "השפה עצמה": איך נראית תוכנית C#, אילו טיפוסים יש, איך כותבים תנאים, לולאות ומתודות, ומה זה `?` שמופיע אחרי טיפוסים. אם כבר תכנתתם בשפה אחרת (Java, JavaScript, Python), רוב הדברים ייראו מוכרים — שימו לב במיוחד להבדל בין טיפוסי ערך להפניה ול-nullable reference types, כי שם מסתתרים רוב הבאגים של מתחילים.

## תוכנית ראשונה: `Program.cs` ו-top-level statements

<div dir="ltr">

```bash
dotnet new console -n HelloApp
cd HelloApp
dotnet run
```

</div>

`dotnet new` יוצר שני קבצים: `HelloApp.csproj` (הגדרות הפרויקט — איזו גרסת .NET, אילו חבילות) ו-`Program.cs`. מאז C# 9 אין צורך במחלקה `Program` ובמתודה `Main` — כותבים את הקוד ישירות:

<div dir="ltr">

```csharp
Console.WriteLine("Hello, .NET!");
```

</div>

זה נקרא **top-level statements**. המהדר עוטף את הקוד ב-`Main` בשבילנו. הכלל היחיד: הצהרות של טיפוסים (מחלקות, records) חייבות לבוא **אחרי** כל ההוראות. בפרויקטים גדולים עדיין נפגוש `static void Main(string[] args)` — זה אותו דבר, רק מפורש.

ה-`.csproj` שלנו נראה כך:

<div dir="ltr">

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>
</Project>
```

</div>

`ImplicitUsings` מייבא אוטומטית `System`, `System.Collections.Generic`, `System.Linq` ועוד — לכן לא נראה `using System;` בראש הקבצים. `Nullable` נסביר בהמשך.

## משתנים וטיפוסים בסיסיים

C# היא שפה עם **טיפוסים סטטיים**: לכל משתנה יש טיפוס שנקבע בזמן הקומפילציה.

<div dir="ltr">

```csharp
int age = 30;                 // שלם 32 ביט
long big = 10_000_000_000;    // שלם 64 ביט (קו תחתון לקריאות)
double price = 19.90;         // נקודה צפה 64 ביט
decimal money = 19.90m;       // עשרוני מדויק — לכסף! (סיומת m)
bool isActive = true;
char initial = 'D';
string name = "Dana";
```

</div>

**`var`** מבקש מהמהדר להסיק את הטיפוס מהערך. הטיפוס עדיין קבוע — `var x = 5;` ואז `x = "hi";` לא יתקמפל. משתמשים ב-`var` כשהטיפוס ברור מהצד הימני (`var list = new List<int>();`) ומציינים טיפוס מפורש כשהוא לא (`int count = GetCount();`).

### טיפוסי ערך מול טיפוסי הפניה

זהו ההבדל החשוב ביותר להבין ביום הראשון:

| | טיפוסי ערך (value types) | טיפוסי הפניה (reference types) |
|---|---|---|
| דוגמאות | `int`, `double`, `bool`, `char`, `decimal`, `DateTime`, `struct`, `enum` | `string`, מערכים, `class`, `record`, `List<T>`, `object` |
| המשתנה מכיל | את הערך עצמו | "כתובת" של אובייקט ב-heap |
| השמה `b = a` | מעתיקה את הערך | מעתיקה את ההפניה — שני שמות לאותו אובייקט |
| ברירת מחדל | `0`, `false` וכו' | `null` |

<div dir="ltr">

```csharp
int a = 5;
int b = a;      // העתקה
b++;            // a עדיין 5

int[] arr1 = [1, 2, 3];
int[] arr2 = arr1;   // אותו מערך!
arr2[0] = 99;        // arr1[0] הוא עכשיו 99
```

</div>

`string` הוא טיפוס הפניה, אבל **בלתי-משתנה** (immutable): כל פעולה כמו `ToUpper()` מחזירה מחרוזת חדשה. לכן `s.ToUpper();` בלי השמה לא עושה כלום.

## מחרוזות ואינטרפולציה

<div dir="ltr">

```csharp
string first = "Dana", last = "Cohen";
string full = $"{first} {last}";                  // string interpolation
string padded = $"{first,-10}|{42,5}";             // יישור: רוחב 10 לשמאל, 5 לימין
string money = $"{1234.5:N2}";                     // "1,234.50"
string pct = $"{0.256:P1}";                        // "25.6%"
string multi = """
    שורה ראשונה
    שורה שנייה
    """;                                           // raw string literal (C# 11)
bool same = string.Equals(first, "dana", StringComparison.OrdinalIgnoreCase);
```

</div>

פעולות שימושיות: `Length`, `Contains`, `StartsWith`, `Split`, `Trim`, `Replace`, `Substring`, `string.Join(", ", list)`, `string.IsNullOrWhiteSpace(s)`.

## בקרת זרימה

<div dir="ltr">

```csharp
if (age >= 18 && isActive) Console.WriteLine("adult");
else if (age >= 13) Console.WriteLine("teen");
else Console.WriteLine("child");

// switch expression — מחזיר ערך, חייב לכסות את כל המקרים (_ = ברירת מחדל)
string grade = score switch
{
    >= 90 => "A",
    >= 80 => "B",
    _ => "F",
};

for (int i = 0; i < 3; i++) { }
foreach (var item in items) { }        // הדרך המועדפת לעבור על אוסף
while (condition) { }
do { } while (condition);
```

</div>

`break` יוצא מהלולאה, `continue` מדלג לאיטרציה הבאה. אופרטור טרנרי: `var label = x > 0 ? "pos" : "neg";`.

## מתודות

<div dir="ltr">

```csharp
static int Add(int x, int y) => x + y;                         // expression-bodied
static string Greet(string who, string greeting = "Hello")      // פרמטר אופציונלי
    => $"{greeting}, {who}!";

static bool TryDivide(double x, double y, out double result)   // out — ערך חזרה נוסף
{
    if (y == 0) { result = 0; return false; }
    result = x / y;
    return true;
}

static (int Min, int Max) MinMax(int[] values) => (values.Min(), values.Max());  // tuple

Greet("Yossi");                        // Hello, Yossi!
Greet("Yossi", greeting: "שלום");      // named argument
var (min, max) = MinMax([4, 9, 1]);    // deconstruction
```

</div>

ב-top-level statements אפשר להגדיר מתודות ישירות בקובץ (local functions). `static` אומר שהמתודה לא ניגשת למשתנים מבחוץ — הרגל טוב לבהירות.

## Nullable reference types

ב-.NET המודרני, `string name` פירושו "מחרוזת שלעולם לא `null`", ו-`string? name` פירושו "אולי `null`". המהדר עוקב ומזהיר אם ניגשים למשהו שעלול להיות `null` בלי בדיקה:

<div dir="ltr">

```csharp
string? input = Console.ReadLine();        // ReadLine מחזיר null בסוף הקלט
int len = input.Length;                    // אזהרה CS8602: dereference of a possibly null reference

int len2 = input?.Length ?? 0;             // ?. מחזיר null אם input null; ?? נותן ברירת מחדל
if (input is not null) Console.WriteLine(input.Length);   // אחרי הבדיקה — אין אזהרה
string safe = input ?? "";                 // ?? — "אם null, אז"
string forced = input!;                    // ! — "אני מבטיח שזה לא null" (להימנע!)
```

</div>

זו לא רק אזהרה קוסמטית: `NullReferenceException` היא החריגה הנפוצה ביותר ב-.NET, והמנגנון הזה מונע את רובן עוד לפני ההרצה. הכלל: אל תשתיקו אזהרות nullable — תקנו אותן.

## Namespaces ו-using

<div dir="ltr">

```csharp
namespace MyShop.Inventory;     // file-scoped namespace — כל הקובץ שייך אליו

public class Product { }
```

</div>

מקובל שמבנה התיקיות משקף את ה-namespace. כדי להשתמש בטיפוס מ-namespace אחר: `using MyShop.Inventory;` בראש הקובץ, או שם מלא `MyShop.Inventory.Product`.

## Visual Studio / VS Code — הבסיס

- **Visual Studio**: File → New → Project → Console App. `F5` ריצה עם דיבאגר, `Ctrl+F5` בלי. `Ctrl+.` להצעות תיקון (Quick Actions), `Ctrl+K, Ctrl+D` לעיצוב קוד, `F12` למעבר להגדרה.
- **VS Code**: התקינו את הרחבת **C# Dev Kit**. `dotnet new console`, פתחו את התיקייה, `F5` (ייווצר `launch.json` אוטומטית). הטרמינל המשולב (`` Ctrl+` ``) ל-`dotnet run`.
- שני הכלים משתמשים באותו SDK ובאותו `dotnet` CLI — הפרויקט זהה.

פקודות CLI שכדאי לזכור: `dotnet new console -n Name`, `dotnet run`, `dotnet build`, `dotnet add package X`, `dotnet --version`.

## `enum` ו-`struct` בקצרה

שני טיפוסי ערך שתגדירו בעצמכם ותפגשו כבר היום. **`enum`** הוא קבוצה סגורה של ערכים בעלי שם — במקום "מספרי קסם" כמו `status == 2` כותבים `status == OrderStatus.Shipped`. המהדר מונע ערכים לא חוקיים, ה-IDE משלים אוטומטית, והקוד מסביר את עצמו. **`struct`** הוא כמו מחלקה קטנה שמועתקת לפי ערך (כמו `int`), ומתאים לנתונים קטנים ובלתי-משתנים כמו נקודה או צבע. ברוב המקרים תכתבו `class` או `record`; `struct` שמור למקרים שבהם הביצועים משנים.

<div dir="ltr">

```csharp
enum OrderStatus { Pending, Paid, Shipped, Cancelled }

OrderStatus status = OrderStatus.Paid;
if (status is OrderStatus.Paid or OrderStatus.Shipped) Console.WriteLine("money received");
Console.WriteLine((int)status);                 // 1 — הערך המספרי מאחורי הקלעים
Console.WriteLine(Enum.Parse<OrderStatus>("Shipped"));

readonly record struct Point(int X, int Y);     // טיפוס ערך קטן ובלתי-משתנה
```

</div>

## איך קוראים שגיאת קומפילציה

המהדר של C# הוא החבר הכי טוב שלכם: רוב הטעויות נתפסות לפני שהתוכנית רצה בכלל. הודעת שגיאה נראית כך: `Program.cs(12,9): error CS0029: Cannot implicitly convert type 'string' to 'int'`. קראו אותה מימין לשמאל: מה הבעיה (המרה לא חוקית), מה הקוד (`CS0029` — אפשר לחפש אותו בדיוק כך במנוע חיפוש או בתיעוד), ואיפה (שורה 12, עמודה 9). ב-Visual Studio לחיצה כפולה על השגיאה ב-Error List מקפיצה לשורה, ו-`Ctrl+.` מציע לעיתים קרובות תיקון אוטומטי. אזהרות (warning) לא עוצרות את הבנייה, אבל כדאי להתייחס אליהן כאל שגיאות — במיוחד אזהרות nullable.

## טעויות נפוצות

- **`==` בין `double`** — `0.1 + 0.2 == 0.3` הוא `false`. לכסף השתמשו ב-`decimal`; להשוואת `double` השוו הפרש לסף קטן.
- **שינוי מחרוזת "במקום"** — `name.ToUpper();` לא משנה את `name`. צריך `name = name.ToUpper();`.
- **`int` מתמלא** — `int.MaxValue + 1` מתגלגל למספר שלילי בלי שגיאה (אלא אם `checked`). ל-מספרים גדולים: `long`.
- **`Console.ReadLine()` מחזיר `string?`** — תמיד טפלו ב-`null` (סוף קלט) ובקלט לא מספרי (`int.TryParse`, מודול 06).
- **השתקת אזהרות nullable עם `!`** — כמעט תמיד מסתירה באג אמיתי.
- **`var` לכל דבר** — `var result = Process();` לא אומר לקורא כלום. ציינו טיפוס כשהוא לא ברור מהשורה.
- **שכחת `m` על `decimal`** — `decimal x = 19.90;` לא מתקמפל; צריך `19.90m`.

## לסיכום

- `dotnet new console` + `dotnet run` — זה כל מה שצריך כדי להתחיל. top-level statements חוסכים את הטקס של `Main`.
- הבדל ערך/הפניה: `int` מועתק, מערך/מחלקה משותפים. זה יסביר הרבה התנהגויות בהמשך היום.
- אינטרפולציה `$"..."` עם פורמטים (`:N2`, `,-10`) היא הדרך לבנות מחרוזות.
- `switch` expression, `foreach`, מתודות expression-bodied, `out` ו-tuples — התחביר המודרני קצר וקריא.
- `string?` מול `string`: המהדר עוזר לכם להימנע מ-`NullReferenceException`. תנו לו.

## קריאה נוספת

- [A tour of the C# language](https://learn.microsoft.com/dotnet/csharp/tour-of-csharp/)
- [Top-level statements](https://learn.microsoft.com/dotnet/csharp/fundamentals/program-structure/top-level-statements)
- [Value types and reference types](https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/value-types)
- [Nullable reference types](https://learn.microsoft.com/dotnet/csharp/nullable-references)
- [String interpolation](https://learn.microsoft.com/dotnet/csharp/language-reference/tokens/interpolated)
- [dotnet CLI overview](https://learn.microsoft.com/dotnet/core/tools/)

</div>
