<div dir="rtl">

# מודול 02 — מחלקות, אובייקטים, בנאים ומאפיינים

תכנות מונחה-עצמים (OOP) הוא דרך לארגן תוכנה סביב **עצמים**: יחידות שמאגדות יחד נתונים (מצב) והתנהגות (מתודות) שפועלת על הנתונים האלה. במקום פונקציה `Deposit(account, amount)` שמקבלת "מבנה נתונים" ומשנה אותו, יש לנו אובייקט `account` שיודע להפקיד לעצמו — ורק הוא מחליט אם ההפקדה חוקית. במודול הזה נלמד את אבני הבניין: מחלקות, אובייקטים, בנאים ו-properties. במודול הבא נראה את ארבעת העקרונות (אנקפסולציה, ירושה, פולימורפיזם, הפשטה) שהופכים את זה לעיצוב טוב.

## מחלקה ואובייקט

**מחלקה** (`class`) היא תבנית — "כך נראה חשבון בנק". **אובייקט** (מופע, instance) הוא חשבון בנק ספציפי שנוצר מהתבנית עם `new`. מחלקה אחת, אינסוף אובייקטים, לכל אחד מצב משלו.

<div dir="ltr">

```csharp
class BankAccount
{
    private decimal _balance;                 // שדה (field) — המצב הפנימי, פרטי

    public string Owner { get; set; } = "";   // property — הפנים הציבוריות

    public void Deposit(decimal amount)       // מתודה — התנהגות
    {
        if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount));
        _balance += amount;
    }

    public decimal GetBalance() => _balance;
}

var acc = new BankAccount { Owner = "Dana" };   // יצירה + object initializer
acc.Deposit(500);
Console.WriteLine($"{acc.Owner}: {acc.GetBalance()}");
```

</div>

מוסכמות: שדות פרטיים ב-`_camelCase`, כל מה שציבורי ב-`PascalCase`. מחלקה לכל קובץ, שם הקובץ כשם המחלקה.

## שדות מול Properties

**שדה** הוא משתנה בתוך האובייקט. **Property** נראה כמו שדה מבחוץ (`acc.Balance`), אבל מאחוריו יש מתודות `get`/`set` שאפשר לשלוט בהן. למה לא פשוט שדות ציבוריים? כי property מאפשר:

- לאמת ערכים (`set` שבודק);
- לחשב ערך בזמן קריאה במקום לשמור אותו;
- לחשוף קריאה בלבד;
- לשנות את המימוש בעתיד בלי לשבור את מי שמשתמש במחלקה.

<div dir="ltr">

```csharp
class Product
{
    // auto-property: המהדר יוצר שדה נסתר
    public string Name { get; set; } = "";

    // קריאה ציבורית, כתיבה רק מתוך המחלקה
    public decimal Price { get; private set; }

    // property מלא עם ולידציה
    private int _stock;
    public int Stock
    {
        get => _stock;
        set => _stock = value >= 0 ? value : throw new ArgumentOutOfRangeException(nameof(value));
    }

    // computed property — אין שדה, מחושב בכל קריאה
    public decimal PriceWithVat => Price * 1.18m;

    // init-only: ניתן לקבוע רק ביצירה (object initializer / בנאי), אחר כך read-only
    public string Sku { get; init; } = "";

    // required: חובה לקבוע ביצירה, אחרת שגיאת קומפילציה
    public required string Category { get; init; }
}

var p = new Product { Name = "Keyboard", Sku = "K-1", Category = "Peripherals" };
// p.Sku = "X";   // שגיאה: init-only
```

</div>

`required` + `init` הם השילוב המודרני ל"אובייקט שחייב להיות תקין מהרגע שנוצר, ואז לא משתנה" — בלי לכתוב בנאי עם 6 פרמטרים.

## בנאים (constructors)

בנאי הוא המתודה שרצה ב-`new`. תפקידו: להביא את האובייקט למצב תקין. אם לא כותבים בנאי, יש בנאי ריק ברירת מחדל.

<div dir="ltr">

```csharp
class BankAccount
{
    public string Id { get; }          // read-only — נקבע רק בבנאי
    public string Owner { get; set; }
    public decimal Balance { get; private set; }

    public BankAccount(string id, string owner)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        Id = id;
        Owner = owner;
    }

    // overload שמשרשר לבנאי הראשון עם this(...)
    public BankAccount(string id, string owner, decimal initialDeposit) : this(id, owner)
    {
        Deposit(initialDeposit);
    }

    public void Deposit(decimal amount) { /* ... */ }
}

var a1 = new BankAccount("IL-1", "Dana");
var a2 = new BankAccount("IL-2", "Yossi", 1_000);
```

</div>

### Primary constructors (C# 12)

כשהבנאי רק מעתיק פרמטרים לשדות, אפשר לקצר: הפרמטרים נכתבים בכותרת המחלקה וזמינים בכל גוף המחלקה.

<div dir="ltr">

```csharp
class TemperatureSensor(string location, double initial)
{
    private readonly List<double> _readings = [initial];

    public string Location => location;          // הפרמטר "נלכד"
    public double Last => _readings[^1];
    public void Read(double value) => _readings.Add(value);
}
```

</div>

שימו לב: הפרמטרים של primary constructor הם לא properties — הם לא נראים מבחוץ אלא אם חושפים אותם. ב-`record` (בהמשך) הם כן הופכים ל-properties אוטומטית.

## `this`

`this` הוא ההפניה לאובייקט הנוכחי. משתמשים בו כשיש התנגשות שמות (`this.name = name;`), לשרשור בנאים (`: this(...)`), או כדי להעביר את האובייקט עצמו למקום אחר (`registry.Add(this)`). ברוב הקוד המודרני לא צריך לכתוב אותו.

## חברים סטטיים

`static` שייך **למחלקה** ולא לאובייקט ספציפי. מונה מופעים, קבועים, מתודות עזר "טהורות" ו-factory methods הם המקרים הקלאסיים:

<div dir="ltr">

```csharp
class BankAccount
{
    public const decimal MinDeposit = 10m;              // קבוע — static מובנה
    public static int Count { get; private set; }       // משותף לכל המופעים

    public BankAccount(string id, string owner) { /*...*/ Count++; }

    public static BankAccount OpenSavings(string owner) => new(NextId(), owner);   // factory
    private static string NextId() => $"IL-{Count + 1:D4}";
}

Console.WriteLine(BankAccount.Count);       // דרך המחלקה, לא דרך אובייקט
```

</div>

מחלקה שכולה static (`static class MathUtils`) לא ניתנת ליצירה — היא רק "ארגז כלים" של מתודות. `Math`, `Console`, `File` הן כאלה.

## Object initializers

תחביר נוח לקבוע properties מיד אחרי היצירה. משתלב עם `init`/`required`:

<div dir="ltr">

```csharp
var order = new Order
{
    Id = 42,
    Customer = "Dana",
    Lines = [new("pen", 2), new("book", 1)],   // collection expression (C# 12)
};
```

</div>

## `record` מול `class`

`record` הוא מחלקה שה-מהדר כותב עבורה אוטומטית: בנאי, properties, `Equals`/`GetHashCode` **לפי ערך**, `ToString` קריא, `with` ו-deconstruction. הוא מיועד ל"נתונים" — אובייקטים שמזוהים לפי התוכן שלהם ולא לפי הזהות.

<div dir="ltr">

```csharp
record Person(string Name, int Age);          // positional record — שורה אחת

var p1 = new Person("Dana", 30);
var p2 = new Person("Dana", 30);
Console.WriteLine(p1 == p2);                   // True — שוויון לפי ערך
Console.WriteLine(p1);                         // Person { Name = Dana, Age = 30 }
var older = p1 with { Age = 31 };              // עותק עם שינוי; p1 לא השתנה
var (name, age) = older;                       // deconstruction
```

</div>

| | `class` | `record` |
|---|---|---|
| שוויון | לפי הפניה (אותו אובייקט) | לפי ערך (אותם נתונים) |
| מיועד ל | ישויות עם זהות ומצב שמשתנה (`BankAccount`, `Order`) | נתונים/ערכים (`Point`, `Money`, DTO, תוצאת שאילתה) |
| שינוי | properties עם `set` | בדרך כלל immutable, `with` ליצירת עותק |
| `ToString` | שם הטיפוס (אלא אם דורסים) | כל ה-properties |

כלל אצבע: אם שני אובייקטים עם אותם נתונים הם "אותו דבר" — record. אם הזהות חשובה (שני חשבונות בנק עם אותה יתרה הם עדיין חשבונות שונים) — class. יש גם `record struct` לטיפוסי ערך.

## מחזור החיים של אובייקט

מה קורה בפועל כשכותבים `var acc = new BankAccount("IL-1", "Dana");`? קודם מוקצה זיכרון ב-heap בגודל שמתאים לכל השדות של המחלקה. השדות מקבלים ערכי ברירת מחדל (`0`, `false`, `null`), ואז רצים המאתחלים שכתבתם בשורת ההצהרה (`= [];`, `= "";`), ורק אז גוף הבנאי. בסוף מוחזרת ההפניה ונשמרת ב-`acc`. האובייקט חי כל עוד מישהו מחזיק הפניה אליו; כשאין יותר הפניות, ה-**Garbage Collector** משחרר את הזיכרון בזמן שנוח לו — אין `delete` ב-C#. זו הסיבה שאנחנו לא דואגים לשחרור זיכרון, אבל כן דואגים לשחרור **משאבים** חיצוניים (קבצים, חיבורים) דרך `IDisposable` — על כך במודול 06.

חשוב להבין את ההשלכה של "הפניה": אם נעביר את `acc` למתודה והמתודה תקרא `acc.Deposit(100)`, השינוי יראה גם אצל הקורא — יש רק אובייקט אחד. לעומת זאת, `record struct` או `int` שמועברים למתודה מועתקים, והמתודה עובדת על עותק.

## עוד על `required` מול בנאי

מתי לבחור בנאי ומתי `required`? בנאי מתאים כשיש **לוגיקה** ביצירה — ולידציה, חישוב, רישום — או כשיש מעט פרמטרים חובה. `required` + `init` מתאים לאובייקטים שהם בעיקר "מכולת נתונים" עם הרבה שדות, שבהם object initializer קריא יותר מרשימת פרמטרים ארוכה שקל לבלבל בסדר שלהם. אפשר גם לשלב: בנאי לפרמטרים החיוניים ו-properties עם `init` לכל השאר.

## טעויות נפוצות

- **שדות ציבוריים** (`public int Age;`) — מאבדים שליטה. תמיד property.
- **בנאי שלא מאמת** ואז מתודות שבודקות `if (Name == null)` בכל מקום. אמתו פעם אחת, בכניסה.
- **`set` ציבורי על כל דבר** — אם ערך לא אמור להשתנות אחרי היצירה, `{ get; }` או `init`.
- **בלבול בין `static` למופע** — `Count++` בתוך מתודה סטטית לא יכול לגשת ל-`this`; ומתודת מופע יכולה לגשת ל-static, לא להפך.
- **record עם `set` ציבורי** — מאבד את היתרון (immutability + hash יציב). אם צריך שינוי — class.
- **בנאי שעושה עבודה כבדה** (קריאה לרשת, DB) — בנאי צריך להיות מהיר וצפוי. עבודה כבדה — במתודה נפרדת או factory.
- **`==` בין מחלקות** כשמתכוונים לתוכן — עבור `class` זו השוואת הפניות. או record, או `Equals` דרוס (מודול 03).

## לסיכום

- מחלקה = תבנית; אובייקט = מופע עם מצב משלו. `new` מפעיל בנאי שאחראי להביא את האובייקט למצב תקין.
- Properties במקום שדות ציבוריים: `{ get; set; }`, `{ get; private set; }`, `{ get; init; }`, `required`, ו-computed (`=>`).
- בנאים: overloads עם `: this(...)`, ו-primary constructors כשהבנאי טריוויאלי.
- `static` — שייך למחלקה: קבועים, מונים, factory methods.
- `record` לנתונים (שוויון לפי ערך, `with`), `class` לישויות עם זהות.

## קריאה נוספת

- [Classes, structs, and records](https://learn.microsoft.com/dotnet/csharp/fundamentals/types/classes)
- [Properties](https://learn.microsoft.com/dotnet/csharp/programming-guide/classes-and-structs/properties)
- [Constructors](https://learn.microsoft.com/dotnet/csharp/programming-guide/classes-and-structs/constructors)
- [Primary constructors](https://learn.microsoft.com/dotnet/csharp/whats-new/tutorials/primary-constructors)
- [Records](https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/record)
- [required modifier](https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/required)

</div>
