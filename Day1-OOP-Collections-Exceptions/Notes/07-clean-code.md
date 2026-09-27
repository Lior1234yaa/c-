<div dir="rtl">

# מודול 07 — קוד נקי, תחזוקתי ורב-שימושי

קוד נקרא הרבה יותר פעמים ממה שהוא נכתב. כל שורה שתכתבו היום, מישהו — כנראה אתם בעוד חצי שנה — יצטרך להבין, לשנות ולתקן. "קוד נקי" הוא לא עניין של יופי אלא של **עלות**: כמה זמן לוקח להבין מה הקוד עושה, וכמה בטוח לשנות אותו. במודול הזה נאסוף עקרונות פשוטים ומעשיים, נראה אותם על דוגמאות before/after, ונחבר אותם למה שלמדנו היום: ממשקים, גנריקה ואוספים הם הכלים שהופכים קוד לרב-שימושי.

## שמות

השם הוא התיעוד הראשון והחשוב ביותר. שם טוב חוסך הערה.

- **תארו כוונה**, לא מימוש: `elapsedDays` ולא `d`; `IsEligibleForDiscount` ולא `Check`.
- **מוסכמות C#**: `PascalCase` למחלקות, מתודות, properties; `camelCase` לפרמטרים ומשתנים מקומיים; `_camelCase` לשדות פרטיים; `I` לממשקים; מתודות שמחזירות `bool` בצורת שאלה (`IsEmpty`, `HasItems`, `CanWithdraw`); מתודות async עם סיומת `Async`.
- **בלי קיצורים** שלא כולם מכירים (`cust`, `mgr`) ובלי הונגרית (`strName`, `iCount`).
- **אורך לפי טווח**: `i` בלולאה של 3 שורות בסדר; `i` כשדה במחלקה — לא.

<div dir="ltr">

```csharp
// לפני
static double c(double a, int t, bool m) { ... }

// אחרי
static decimal CalculatePriceWithVat(decimal basePrice, CustomerType customer, bool isMember) { ... }
```

</div>

## מתודות קטנות שעושות דבר אחד

מתודה צריכה להיקרא ולהיות מובנת בלי לגלול. כלל אצבע: עד ~20 שורות, רמת הזחה אחת או שתיים, ושם שמתאר בדיוק מה היא עושה. אם אתם צריכים "ו" בשם (`ValidateAndSave`) — אלה שתי מתודות. חילוץ מתודה (Extract Method, `Ctrl+R, Ctrl+M` ב-Visual Studio) הוא ה-refactoring הנפוץ ביותר.

<div dir="ltr">

```csharp
// לפני: הכל במקום אחד
public void ProcessOrder(Order order)
{
    if (order.Lines.Count == 0) throw new ArgumentException("empty");
    decimal total = 0;
    foreach (var l in order.Lines) total += l.Price * l.Quantity;
    if (order.Customer.IsVip) total *= 0.9m;
    _db.Save(order, total);
    _mailer.Send(order.Customer.Email, $"Order {order.Id}: {total:N2}");
}

// אחרי: שלוש שאלות, שלוש תשובות
public void ProcessOrder(Order order)
{
    EnsureNotEmpty(order);
    decimal total = CalculateTotal(order);
    _db.Save(order, total);
    NotifyCustomer(order, total);
}
```

</div>

## מספרי קסם → קבועים ו-enums

`if (status == 2)` או `price * 1.18` לא אומרים כלום לקורא, וכשהמע"מ ישתנה תצטרכו לחפש `1.18` בכל הפרויקט (ולפספס את ה-`1.18m` בקובץ אחר).

<div dir="ltr">

```csharp
const decimal VatRate = 1.18m;
const int RegularHoursPerMonth = 160;
static readonly TimeSpan LoanPeriod = TimeSpan.FromDays(14);     // readonly לטיפוסים לא-קבועים

enum OrderStatus { Pending, Paid, Shipped, Cancelled }
if (order.Status == OrderStatus.Shipped) { }
```

</div>

`const` לערכים שידועים בקומפילציה (מספרים, מחרוזות); `static readonly` לאובייקטים. `enum` לקבוצה סגורה של אפשרויות — המהדר בודק, ה-IDE משלים, ו-`switch` יכול להזהיר על מקרה חסר.

## DRY — אל תחזרו על עצמכם

כשאותו קטע קוד מופיע פעמיים, באג יתוקן רק במקום אחד. חילוץ מתודה, מחלקת בסיס, או — לרוב הכי טוב — מתודה שמקבלת delegate (מודול 05). אבל היזהרו מהקיצוניות השנייה: שני קטעים שנראים דומים אבל משתנים מסיבות שונות לא צריכים להתאחד. DRY הוא על **ידע**, לא על טקסט.

## SOLID בקצרה

חמישה עקרונות לעיצוב מונחה-עצמים. לא צריך לשנן — צריך לזהות מתי מפרים אותם:

- **S — Single Responsibility**: למחלקה סיבה אחת להשתנות. `Order` לא צריך לדעת לשלוח מייל; `OrderService` לא צריך לדעת לפרמט HTML. סימן אזהרה: מחלקה עם "Manager"/"Helper" בשם ו-40 מתודות.
- **O — Open/Closed**: פתוח להרחבה, סגור לשינוי. הוספת סוג עובד חדש = מחלקה חדשה, לא `case` נוסף ב-`switch` (מודול 03). פולימורפיזם וממשקים.
- **L — Liskov Substitution**: יורש חייב להתנהג כמו האב מבחינת מי שמשתמש בו. אם `Square : Rectangle` שובר `SetWidth` — הירושה שגויה.
- **I — Interface Segregation**: ממשקים קטנים וממוקדים. `IReadOnlyRepository` ו-`IWritableRepository` במקום ממשק אחד עם 12 מתודות שרוב המממשים זורקים `NotSupportedException` מחצי מהן.
- **D — Dependency Inversion**: תלות בהפשטות, לא במימושים. `OrderService(ILogger logger)` ולא `new FileLogger()` בתוך המחלקה. זה מה שמאפשר להחליף מימוש ולבדוק עם fake. ביום 2 נראה Dependency Injection שעושה את זה אוטומטית.

## הערות

הערה טובה מסבירה **למה**, לא **מה**. `i++; // מגדיל את i` היא רעש; `// הספק מחזיר 429 מעל 10 בקשות בשנייה, לכן ההשהיה` היא זהב. אם צריך הערה כדי להסביר מה הקוד עושה — שפרו את הקוד (שם, חילוץ מתודה). הערות `///` (XML doc) על API ציבורי מופיעות ב-IntelliSense ושוות את זה. מחקו קוד בהערה — יש git.

## סגנון קוד: `.editorconfig` ו-`dotnet format`

צוות צריך סגנון אחד, וכלי שאוכף אותו — לא ויכוחים ב-code review על רווחים. קובץ `.editorconfig` בשורש הפרויקט מגדיר את הכללים (הזחות, מיקום סוגריים, `var`, סדר `using`, ואפילו כללי ניתוח סטטי), ו-Visual Studio, VS Code ו-Rider מכבדים אותו אוטומטית.

<div dir="ltr">

```bash
dotnet new editorconfig       # יוצר קובץ עם ברירות המחדל של .NET
dotnet format                 # מעצב את כל הפרויקט לפי הכללים
dotnet format --verify-no-changes   # ב-CI: נכשל אם משהו לא מעוצב
```

</div>

דוגמה לכמה שורות מה-`.editorconfig`:

<div dir="ltr">

```text
[*.cs]
indent_size = 4
csharp_style_var_when_type_is_apparent = true:suggestion
csharp_prefer_braces = true:warning
dotnet_diagnostic.CA2200.severity = error     # throw ex; הופך לשגיאה
```

</div>

הוסיפו גם `<TreatWarningsAsErrors>true</TreatWarningsAsErrors>` ב-`.csproj` בפרויקטים חדשים — אזהרות מצטברות רק אם מרשים להן.

## Refactoring — לפני ואחרי

שכתוב (refactoring) הוא שינוי **מבנה** הקוד בלי לשנות **התנהגות**. עושים אותו בצעדים קטנים, עם בדיקות (או לפחות הרצה) אחרי כל צעד.

<div dir="ltr">

```csharp
// לפני
static double c(double a, int t, bool m)
{
    double r = 0;
    if (t == 1) { r = a * 0.9; if (m) r = r - 5; }
    else if (t == 2) { r = a * 0.8; if (m) r = r - 5; }
    else { r = a; if (m) r = r - 5; }
    if (r < 0) r = 0;
    return r * 1.18;
}

// אחרי
enum CustomerType { Regular, Silver, Gold }
const decimal VatRate = 1.18m, MemberDiscount = 5m, SilverFactor = 0.9m, GoldFactor = 0.8m;

static decimal CalculatePriceWithVat(decimal basePrice, CustomerType customer, bool isMember)
{
    decimal price = ApplyCustomerDiscount(basePrice, customer);
    if (isMember) price -= MemberDiscount;
    price = Math.Max(price, 0);
    return price * VatRate;
}

static decimal ApplyCustomerDiscount(decimal price, CustomerType customer) => customer switch
{
    CustomerType.Silver => price * SilverFactor,
    CustomerType.Gold => price * GoldFactor,
    _ => price,
};
```

</div>

מה השתנה: שמות, `enum` במקום 1/2, קבועים, הכפילות `if (m) r -= 5` הוצאה החוצה, `switch` expression במקום שרשרת `if`, `decimal` לכסף. ההתנהגות זהה — תרגיל 14 בודק את זה.

עוד refactorings יומיומיים: Extract Method, Rename (`F2` / `Ctrl+R, Ctrl+R`), Inline Variable, Replace Conditional with Polymorphism, Introduce Parameter Object (במקום 6 פרמטרים — record אחד), Guard Clauses במקום `if` מקונן.

## קוד רב-שימושי: ממשקים וגנריקה

הכלים שלמדנו היום הם בדיוק מה שהופך קוד ל-reusable:

- **ממשק** מפריד בין "מה" ל-"איך": `Payroll` שעובד עם `IPayable` ישרת גם עובדים, קבלנים ובעתיד רובוטים.
- **גנריקה** מפרידה בין אלגוריתם לטיפוס: `Repository<T>` אחד לכל הישויות; `Find(Func<T, bool>)` אחד לכל התנאים.
- **delegates** מפרידים בין מסגרת לפרטים: `Retry(Action op, int times)` עוטף כל פעולה.
- **records + פונקציות טהורות** קלים להרכבה ולבדיקה.

<div dir="ltr">

```csharp
static T Retry<T>(Func<T> operation, int attempts = 3)       // רב-שימושי: כל פעולה, כל טיפוס
{
    for (int i = 1; ; i++)
    {
        try { return operation(); }
        catch (Exception) when (i < attempts) { Thread.Sleep(100 * i); }
    }
}

var data = Retry(() => client.GetString(url));
```

</div>

## טעויות נפוצות

- **אופטימיזציה מוקדמת** — קוד "חכם" ולא קריא בשביל ביצועים שאף אחד לא מדד.
- **הפשטה מוקדמת** — ממשק לכל מחלקה, גנריקה לדבר שיש לו שימוש אחד. הכלל: אבסטרקציה אחרי המקרה השני, לא לפני הראשון.
- **מחלקות "God"** שיודעות הכל (`AppManager`).
- **פרמטרים בוליאניים** — `Save(order, true, false)`: מה זה אומר? enum או שני שמות.
- **הערות במקום שמות** — `int d; // days since last login` → `int daysSinceLastLogin`.
- **קוד מת** ו-`// TODO` מלפני שנתיים — מחקו. git זוכר.
- **שבירת הסגנון של הפרויקט** כי "ככה אני רגיל" — עקביות חשובה מהעדפה אישית.

## לסיכום

- שמות שמתארים כוונה; מתודות קטנות; קבועים ו-enums במקום מספרי קסם; DRY על ידע.
- SOLID: אחריות אחת, הרחבה בלי שינוי, יורש שמתנהג כמו האב, ממשקים קטנים, תלות בהפשטות.
- הערות ל-"למה"; `.editorconfig` + `dotnet format` לסגנון אחיד.
- refactoring בצעדים קטנים ובדיקה אחרי כל צעד.
- ממשקים, גנריקה ו-delegates הם הכלים לקוד שאפשר להשתמש בו שוב.

## קריאה נוספת

- [C# identifier naming rules and conventions](https://learn.microsoft.com/dotnet/csharp/fundamentals/coding-style/identifier-names)
- [C# coding conventions](https://learn.microsoft.com/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- [Code-style rules (.editorconfig)](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/style-rules/)
- [dotnet format](https://learn.microsoft.com/dotnet/core/tools/dotnet-format)
- [Code analysis in .NET](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/overview)
- [Refactor code in Visual Studio](https://learn.microsoft.com/visualstudio/ide/refactoring-in-visual-studio)
- [Architectural principles (SOLID, DRY)](https://learn.microsoft.com/dotnet/architecture/modern-web-apps-azure/architectural-principles)

</div>
