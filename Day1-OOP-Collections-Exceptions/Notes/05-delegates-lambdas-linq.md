<div dir="rtl">

# מודול 05 — Delegates, Lambdas, Events ו-LINQ

עד עכשיו העברנו למתודות **נתונים**. במודול הזה נלמד להעביר **התנהגות**: "סנן לי את הרשימה לפי התנאי הזה", "כשהמלאי יורד — הודע לי", "מיין לפי השדה הזה". ב-C# זה נעשה עם delegates ו-lambdas, ומעליהם בנויים אירועים ו-LINQ — הכלי שהופך 20 שורות של לולאות לשורה אחת קריאה. נסיים בכמה עקרונות של תכנות פונקציונלי שהופכים קוד לכזה שקל לבדוק ולהבין.

## Delegates — מתודה כערך

delegate הוא טיפוס שמתאר **חתימה** של מתודה: אילו פרמטרים, איזה ערך חזרה. משתנה מטיפוס delegate מחזיק הפניה למתודה, ואפשר "לקרוא" לו.

<div dir="ltr">

```csharp
delegate int MathOp(int a, int b);            // הגדרת הטיפוס

static int Add(int a, int b) => a + b;

MathOp op = Add;                              // המתודה היא ערך
Console.WriteLine(op(2, 3));                  // 5

static int Apply(int x, int y, MathOp op) => op(x, y);
Console.WriteLine(Apply(10, 4, (a, b) => a - b));   // 6 — העברת התנהגות
```

</div>

בפועל כמעט לא מגדירים delegates משלנו, כי .NET מספק גנריים מוכנים:

| טיפוס | חתימה | דוגמה |
|---|---|---|
| `Action` | בלי פרמטרים, בלי ערך חזרה | `Action say = () => Console.WriteLine("hi");` |
| `Action<T1, T2...>` | עד 16 פרמטרים, בלי ערך חזרה | `Action<string> log = m => Console.WriteLine(m);` |
| `Func<T1..., TResult>` | פרמטרים + ערך חזרה (האחרון) | `Func<int, int, int> add = (a, b) => a + b;` |
| `Predicate<T>` | `T` → `bool` | `Predicate<int> isEven = n => n % 2 == 0;` |

delegate הוא **multicast**: `+=` מוסיף מתודה נוספת, והקריאה מפעילה את כולן לפי הסדר. זה הבסיס לאירועים.

## Lambdas

lambda היא מתודה אנונימית שנכתבת במקום: `(פרמטרים) => ביטוי` או `(פרמטרים) => { גוף }`. המהדר מסיק את טיפוסי הפרמטרים מה-delegate שהיא מושמת אליו.

<div dir="ltr">

```csharp
Func<int, int> square = x => x * x;                 // פרמטר אחד — בלי סוגריים
Func<int, int, int> max = (a, b) => a > b ? a : b;
Action<string> shout = s => { var u = s.ToUpper(); Console.WriteLine(u + "!"); };
Func<int> answer = () => 42;                        // בלי פרמטרים
```

</div>

### Closures

lambda יכולה להשתמש במשתנים מהסביבה שבה נוצרה — ו"לזכור" אותם גם אחרי שהמתודה שיצרה אותה סיימה:

<div dir="ltr">

```csharp
static Func<int> MakeCounter()
{
    int count = 0;
    return () => ++count;      // count "נלכד" — חי כל עוד ה-lambda חיה
}

var next = MakeCounter();
Console.WriteLine($"{next()} {next()} {next()}");   // 1 2 3
```

</div>

זה חזק, אבל שימו לב: ה-lambda לוכדת את **המשתנה**, לא את הערך. אם משתנה משתנה אחרי יצירת ה-lambda, היא תראה את הערך החדש.

## Events — publisher/subscriber

אירוע הוא delegate multicast עם הגנה: מבחוץ אפשר רק להירשם (`+=`) ולהסיר (`-=`), ורק המחלקה שהגדירה אותו יכולה להפעיל. זו הדרך של אובייקט להודיע "קרה משהו" בלי לדעת מי מקשיב.

<div dir="ltr">

```csharp
class LowStockEventArgs(string sku, int quantity) : EventArgs
{
    public string Sku { get; } = sku;
    public int Quantity { get; } = quantity;
}

class Inventory
{
    public event EventHandler<LowStockEventArgs>? LowStock;     // המוסכמה: (sender, e)

    public void Sell(string sku, int qty)
    {
        // ... הפחתה ...
        if (remaining <= threshold)
            LowStock?.Invoke(this, new LowStockEventArgs(sku, remaining));   // ?. — אם אין מנויים
    }
}

var inv = new Inventory();
inv.LowStock += (sender, e) => Console.WriteLine($"reorder {e.Sku}! only {e.Quantity} left");
inv.LowStock += (_, e) => emailService.Notify(e.Sku);       // מנוי שני — Inventory לא יודע עליו
```

</div>

המוסכמה של .NET: `EventHandler<TEventArgs>`, פרמטרים `(object? sender, TEventArgs e)`, שם האירוע בזמן עבר או הווה (`Changed`, `Clicked`, `LowStock`). כשתעבדו עם WPF/WinForms ביום 4, כל לחיצת כפתור היא אירוע כזה.

זכרו להסיר מנויים (`-=`) מאובייקטים ארוכי-חיים: publisher שמחזיק הפניה למנוי מונע ממנו להיאסף ב-GC.

## Extension methods — איך LINQ "נדבק" לכל אוסף

`Where`, `Select` ושאר האופרטורים אינם מתודות של `List<T>` — הם **extension methods**: מתודות סטטיות שהמהדר מרשה לקרוא כאילו הן שייכות לטיפוס. כך LINQ עובד על כל `IEnumerable<T>` בלי לשנות אף מחלקה. אתם יכולים לכתוב כאלה בעצמכם כדי להוסיף פעולות לטיפוסים שאינם שלכם (למשל `string` או `DateTime`), או כדי לשמור על מחלקות קטנות. הכללים: מחלקה `static`, מתודה `static`, והפרמטר הראשון עם `this`.

<div dir="ltr">

```csharp
static class StringExtensions
{
    public static string Truncate(this string s, int max) =>
        s.Length <= max ? s : s[..max] + "…";

    public static bool IsBlank(this string? s) => string.IsNullOrWhiteSpace(s);
}

Console.WriteLine("Hello, world".Truncate(5));    // Hello…
if (input.IsBlank()) { }                          // עובד גם על null
```

</div>

אל תגזימו: extension method מתאימה לפעולות עזר כלליות, לא ללוגיקה עסקית שצריכה להיות במחלקה עצמה.

## delegate או ממשק?

שניהם מאפשרים "להזריק התנהגות". כלל אצבע: כשצריך **פעולה אחת** (תנאי סינון, callback, השוואה) — delegate (`Func`/`Action`), כי הוא קל להעברה כ-lambda במקום. כשצריך **קבוצת פעולות קשורות** או מצב (לוגר עם `Log` ו-`Flush`, מאגר עם `Get`/`Add`/`Remove`) — ממשק. LINQ בחר ב-delegates כי כל אופרטור צריך בדיוק פונקציה אחת; `IComparer<T>` הוא ממשק כי מיון הוא מושג שמקבל שם ומימושים לשימוש חוזר. ואם אתם מתלבטים — התחילו מ-`Func`, ועברו לממשק כשמופיעה הפעולה השנייה.

## LINQ — שאילתות על אוספים

LINQ (Language Integrated Query) הוא אוסף של extension methods על `IEnumerable<T>` שמקבלות lambdas. במקום לולאה + `if` + רשימה זמנית, מתארים **מה** רוצים:

<div dir="ltr">

```csharp
record Order(int Id, string Customer, string Category, decimal Total);
List<Order> orders = [ /* ... */ ];

// סינון והטלה
var names = orders.Where(o => o.Total > 100).Select(o => o.Customer);

// מיון
var sorted = orders.OrderByDescending(o => o.Total).ThenBy(o => o.Customer);

// קיבוץ
foreach (var g in orders.GroupBy(o => o.Category))
    Console.WriteLine($"{g.Key}: {g.Count()} orders, {g.Sum(o => o.Total):N2}");

// איבר בודד
var first = orders.First(o => o.Category == "Books");            // זורק אם אין
var maybe = orders.FirstOrDefault(o => o.Total > 5000);          // null אם אין
var only = orders.Single(o => o.Id == 3);                        // זורק אם אין או אם יש יותר מאחד

// בדיקות ואגרגציה
bool any = orders.Any(o => o.Total > 1000);
bool all = orders.All(o => o.Total > 0);
int n = orders.Count(o => o.Customer == "Dana");
decimal max = orders.Max(o => o.Total), avg = orders.Average(o => o.Total);

// המרה לאוספים
Dictionary<string, decimal> perCustomer = orders
    .GroupBy(o => o.Customer)
    .ToDictionary(g => g.Key, g => g.Sum(o => o.Total));
List<int> ids = orders.Select(o => o.Id).ToList();
HashSet<string> categories = orders.Select(o => o.Category).ToHashSet();

// עוד שימושיים: Skip/Take (עימוד), Distinct, OrderBy(x => x) , Reverse, Zip, Chunk, MinBy/MaxBy, Aggregate
var page2 = orders.Skip(10).Take(10);
```

</div>

### Method syntax מול query syntax

יש שני תחבירים לאותו דבר. **Method syntax** (למעלה) הוא הנפוץ; **query syntax** דומה ל-SQL ולפעמים קריא יותר ב-joins ובקיבוצים מורכבים:

<div dir="ltr">

```csharp
var q = from o in orders
        where o.Category == "Electronics"
        orderby o.Total descending
        select new { o.Id, o.Customer };      // anonymous type
```

</div>

המהדר מתרגם query syntax ל-method calls. בחרו אחד ותהיו עקביים בפרויקט.

### Deferred execution

זו הנקודה שהכי חשוב להבין ב-LINQ: `Where`, `Select`, `OrderBy` וכו' **לא רצים** כשכותבים אותם. הם בונים "תוכנית", והיא מבוצעת רק כשמישהו עובר על התוצאה (`foreach`, `ToList`, `Count`, `First`...).

<div dir="ltr">

```csharp
var numbers = new List<int> { 1, 2, 3 };
var evens = numbers.Where(n => n % 2 == 0);    // כלום לא קרה עדיין
numbers.Add(4);
Console.WriteLine(string.Join(",", evens));     // 2,4 — השאילתה רצה עכשיו ורואה את 4

var snapshot = numbers.Where(n => n % 2 == 0).ToList();   // ToList מבצע ומקפיא
```

</div>

השלכות: (1) שאילתה שנצרכת פעמיים רצה פעמיים; (2) שינוי במקור אחרי ההגדרה משפיע על התוצאה; (3) חריגה ב-lambda תיזרק בזמן הצריכה, לא בזמן ההגדרה. כשצריך תוצאה יציבה — `ToList()` / `ToArray()`. אופרטורים כמו `Count()`, `Sum()`, `First()`, `ToDictionary()` הם "immediate" — מבצעים מיד.

## סגנון פונקציונלי

LINQ ו-lambdas מזמינים סגנון תכנות שמקל על בדיקות והבנה:

- **פונקציות טהורות** (pure): אותם קלטים → אותו פלט, בלי תופעות לוואי (לא משנות מצב, לא כותבות לקונסולה/קובץ). קל לבדוק, קל להרכיב, בטוח ב-threads. `Reports.TotalValue(items)` בלאב 3 היא כזו.
- **אי-שינוי** (immutability): במקום לשנות אובייקט, ליצור חדש. `record` עם `with`, `IReadOnlyList<T>`, `ImmutableList<T>`. פחות "מי שינה את זה?".
- **הרכבה**: מתודות קטנות שמקבלות ומחזירות `Func`/`IEnumerable`, ומשורשרות: `orders.Where(IsRecent).Select(ToSummary).OrderBy(s => s.Date)`.
- **הפרדה בין חישוב ל-I/O**: חשבו את התוצאה בפונקציה טהורה, הדפיסו/שמרו בשכבה חיצונית.

<div dir="ltr">

```csharp
record Cart(IReadOnlyList<Item> Items) { public decimal Total => Items.Sum(i => i.Price); }

static Cart ApplyDiscount(Cart cart, decimal pct) =>            // טהורה: לא נוגעת ב-cart
    cart with { Items = cart.Items.Select(i => i with { Price = i.Price * (1 - pct) }).ToList() };
```

</div>

לא צריך להפוך את כל הקוד לפונקציונלי; אבל כשמתודה יכולה להיות טהורה — עדיף שתהיה.

## טעויות נפוצות

- **שאילתה שנצרכת כמה פעמים** — רצה כמה פעמים. שמרו ב-`ToList()` פעם אחת.
- **`First()` בלי בדיקה** — `InvalidOperationException: Sequence contains no elements`. `FirstOrDefault` + בדיקת `null`, או `Any()` קודם.
- **הפעלת אירוע בלי `?.`** — `NullReferenceException` כשאין מנויים.
- **lambda שמשנה משתנה חיצוני** בתוך `Select`/`Where` — תופעות לוואי בשאילתה מבלבלות ורצות כמה פעמים. LINQ הוא לחישוב; לולאה `foreach` לתופעות לוואי.
- **לכידת משתנה לולאה** ב-`for` ישן — כל ה-lambdas רואות את הערך האחרון. ב-`foreach` המודרני כל איטרציה מקבלת משתנה חדש, ב-`for` לא.
- **`OrderBy` בתוך לולאה** — מיון O(n log n) בכל איטרציה. מיינו פעם אחת.
- **שרשראות LINQ בנות 12 שלבים בשורה אחת** — פרקו למשתני ביניים עם שמות.

## לסיכום

- delegate = מתודה כערך. `Func`/`Action`/`Predicate` מכסים את כל הצרכים; lambda היא הדרך לכתוב אותם במקום.
- closures לוכדות משתנים, לא ערכים.
- `event EventHandler<T>` + `?.Invoke` — publisher שלא מכיר את המנויים.
- LINQ: `Where/Select/OrderBy/GroupBy/First/Any/ToDictionary` — תיאור **מה**, לא **איך**.
- deferred execution: השאילתה רצה בצריכה. `ToList()` להקפאה.
- פונקציות טהורות + immutability = קוד שקל לבדוק ולהבין.

## קריאה נוספת

- [Delegates](https://learn.microsoft.com/dotnet/csharp/programming-guide/delegates/)
- [Lambda expressions](https://learn.microsoft.com/dotnet/csharp/language-reference/operators/lambda-expressions)
- [Events](https://learn.microsoft.com/dotnet/csharp/programming-guide/events/)
- [LINQ overview](https://learn.microsoft.com/dotnet/csharp/linq/)
- [Standard query operators](https://learn.microsoft.com/dotnet/csharp/linq/standard-query-operators/)
- [Deferred execution](https://learn.microsoft.com/dotnet/csharp/linq/get-started/introduction-to-linq-queries#deferred-execution)
- [Functional programming in C#](https://learn.microsoft.com/dotnet/csharp/fundamentals/functional/)

</div>
