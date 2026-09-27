// Day1.slides.js — יום 1: תכנות מונחה-עצמים, אוספים וטיפול בחריגות
// בנייה: node tools/slides/build-slides.js Day1-OOP-Collections-Exceptions/Slides/Day1.slides.js Day1-OOP-Collections-Exceptions/Slides/Day1.pptx
module.exports = {
  day: 1,
  course: 'C# ב-.NET — קורס מעשי',
  title: 'יום 1 — OOP, אוספים וחריגות',
  slides: [
    // ---------------- פתיחה ----------------
    {
      type: 'title',
      title: 'תכנות מונחה-עצמים, אוספים וטיפול בחריגות',
      subtitle: 'מהבסיס של C# ועד קוד שלא נופל: מחלקות, ירושה, LINQ, חריגות ודיבוג',
      meta: 'יום 1 מתוך 4  |  09:00–16:30',
      notes: 'ברוכים הבאים ליום הראשון. הצגה קצרה של המרצה והמשתתפים: מי כבר תכנת בשפה אחרת? היום הוא הבסיס לכל שאר הקורס — נתחיל בחימום מהיר על השפה ונבנה ממנו את המודל האובייקטי. ודאו שלכולם dotnet --version מדפיס 10.',
    },
    {
      type: 'bullets',
      title: 'סדר היום',
      icon: 'clock',
      bullets: [
        { text: '09:15 — מודול 01: חימום C#', sub: ['תוכנית ראשונה, טיפוסים, בקרת זרימה, מתודות, nullable'] },
        { text: '09:50 — מודול 02: מחלקות, אובייקטים, properties, בנאים, records' },
        { text: '10:45 — Lab 1: מערכת ספרייה (45 דק\')' },
        { text: '11:30 — מודול 03: אנקפסולציה, ירושה, פולימורפיזם, ממשקים' },
        { text: '13:00 — Lab 2: צורות ועובדים (60 דק\')' },
        { text: '14:00 — מודולים 04–05: אוספים, גנריקה, delegates, LINQ' },
        { text: '15:15 — מודולים 06–07: חריגות, דיבוג, קוד נקי  →  Lab 3 / Lab 4' },
      ],
      notes: 'הפסקות ב-10:30 וב-14:30 (15 דק\'), צהריים 12:15–13:00. שני הלאבים האחרונים — אחד בכיתה ואחד בבית, לפי קצב הקבוצה. כל החומרים בתיקיית Notes, וכל דמו הוא פרויקט שאפשר להריץ.',
    },
    {
      type: 'cards',
      title: 'מה נדע לעשות בסוף היום',
      cards: [
        { icon: 'code', heading: 'לכתוב C# מודרני', text: 'top-level statements, var, אינטרפולציה, switch expressions, nullable' },
        { icon: 'cubes', heading: 'לעצב מחלקות', text: 'properties, בנאים, records, ירושה, ממשקים, פולימורפיזם' },
        { icon: 'db', heading: 'לנהל נתונים', text: 'List, Dictionary, HashSet, גנריקה, LINQ, אירועים' },
        { icon: 'shield', heading: 'לכתוב קוד עמיד', text: 'חריגות, guard clauses, using, דיבאגר, קוד נקי' },
      ],
      notes: 'ארבע יכולות — ארבעה לאבים. הדגישו שהיום הוא מעשי: כ-60% מהזמן על המקלדת. מי שכבר מכיר C# יכול לרוץ קדימה עם הבונוסים בלאבים.',
    },

    // ---------------- 01 חימום ----------------
    { type: 'section', number: '01', title: 'חימום C#', subtitle: 'תוכנית ראשונה, טיפוסים, בקרת זרימה, מתודות, nullable', notes: 'מודול קצר ומהיר — 35 דקות. המטרה: שכולם ידברו באותה שפה לפני OOP. מי שמכיר Java/JS ירגיש בבית; שימו לב במיוחד לערך מול הפניה ול-nullable.' },
    {
      type: 'code',
      title: 'תוכנית ראשונה: dotnet new + top-level statements',
      file: 'Program.cs',
      code: `// dotnet new console -n HelloApp
// cd HelloApp && dotnet run

Console.WriteLine("Hello, .NET!");

int age = 30;
string name = "Dana";
var isActive = true;        // המהדר מסיק bool

Console.WriteLine($"{name} is {age}");

// טיפוסים חייבים לבוא אחרי ההוראות
record Person(string Name, int Age);`,
      bullets: [
        'אין Main — המהדר עוטף בשבילנו',
        'הקובץ .csproj: TargetFramework, Nullable, ImplicitUsings',
        'var = הסקת טיפוס, לא טיפוס דינמי',
        'הצהרות טיפוסים תמיד בסוף הקובץ',
        'dotnet run = build + run',
      ],
      notes: 'הריצו live: dotnet new console, פתחו את ה-csproj והראו את שלוש ההגדרות. הדגישו ש-var עדיין סטטי. ImplicitUsings מסביר למה אין using System בראש הקובץ.',
    },
    {
      type: 'two-col',
      title: 'טיפוסי ערך מול טיפוסי הפניה',
      right: {
        heading: 'ערך (value types)',
        bullets: ['int, double, bool, decimal, DateTime, struct, enum', 'המשתנה מכיל את הערך עצמו', 'השמה = העתקה', 'ברירת מחדל: 0 / false'],
      },
      left: {
        heading: 'הפניה (reference types)',
        bullets: ['string, מערכים, class, record, List<T>', 'המשתנה מכיל "כתובת" לאובייקט', 'השמה = שני שמות לאותו אובייקט', 'ברירת מחדל: null'],
      },
      notes: 'ההבדל החשוב ביותר של היום. הראו ב-Demo.Quickstart: int b = a; b++ לא משפיע על a, אבל arr2 = arr1 ואז arr2[0] = 99 משנה גם את arr1. string הוא הפניה אבל immutable — ToUpper מחזיר מחרוזת חדשה.',
    },
    {
      type: 'code',
      title: 'מחרוזות, אינטרפולציה ובקרת זרימה',
      code: `string full = $"{first} {last}";
string money = $"{1234.5:N2}";          // 1,234.50
string table = $"{name,-10}|{42,5}";    // יישור

if (age >= 18 && isActive) { }

string grade = score switch            // switch expression
{
    >= 90 => "A",
    >= 80 => "B",
    _     => "F",
};

foreach (var item in items) { }
for (int i = 0; i < 3; i++) { }
while (n < 3) { n++; }`,
      bullets: [
        '$"..." — אינטרפולציה עם פורמטים (:N2, :F1, :P0)',
        'switch expression מחזיר ערך ומכסה את כל המקרים',
        'foreach — הדרך המועדפת לעבור על אוסף',
        'decimal לכסף (סיומת m), לא double',
      ],
      notes: 'הראו את switch expression מול switch רגיל — קצר וקריא. הזכירו את הטעות של 0.1 + 0.2 == 0.3 ב-double. raw string literals (""") לטקסט רב-שורתי.',
    },
    {
      type: 'code',
      title: 'מתודות: expression-bodied, out, tuples, אופציונלי',
      code: `static int Add(int x, int y) => x + y;

static string Greet(string who, string greeting = "Hello")
    => $"{greeting}, {who}!";

static bool TryDivide(double x, double y, out double result)
{
    if (y == 0) { result = 0; return false; }
    result = x / y;
    return true;
}

static (int Min, int Max) MinMax(int[] v) => (v.Min(), v.Max());

Greet("Yossi", greeting: "שלום");        // named argument
if (TryDivide(10, 2, out var q)) { }
var (min, max) = MinMax([4, 9, 1]);       // deconstruction`,
      bullets: [
        '=> למתודה של שורה אחת',
        'out — ערך חזרה נוסף (תבנית Try...)',
        'tuple עם שמות במקום מחלקה זמנית',
        'local functions ב-top-level — static לבהירות',
      ],
      notes: 'התבנית TryX/out תחזור במודול 06 (TryParse). tuples נוחים לערכי חזרה מרובים, אבל אם הם נשלחים רחוק — record. הריצו את הדמו והראו את הפלט.',
    },
    {
      type: 'code',
      title: 'Nullable reference types',
      code: `string? input = Console.ReadLine();     // יכול להיות null

int len = input.Length;                 // ⚠ CS8602 possible null

int len2 = input?.Length ?? 0;          // ?. ואז ??

if (input is not null)
    Console.WriteLine(input.Length);    // כאן המהדר יודע

string safe = input ?? "";              // ברירת מחדל
string forced = input!;                 // "אני מבטיח" — להימנע`,
      bullets: [
        'string = לעולם לא null; string? = אולי null',
        'אזהרות nullable הן באגים עתידיים — תקנו, אל תשתיקו',
        '?. ו-?? הם הכלים היומיומיים',
        'ReadLine מחזיר null בסוף הקלט',
      ],
      notes: 'NullReferenceException היא החריגה הנפוצה ביותר ב-.NET. המנגנון הזה תופס את רובן בקומפילציה. הראו את האזהרה ב-IDE ואיך היא נעלמת אחרי בדיקת null. הזהירו מ-! — כמעט תמיד מסתיר באג.',
    },

    // ---------------- 02 מחלקות ----------------
    { type: 'section', number: '02', title: 'מחלקות ואובייקטים', subtitle: 'class, new, properties, בנאים, static, records', notes: 'כאן מתחיל OOP. מחלקה = תבנית, אובייקט = מופע. המסר המרכזי: המצב של אובייקט משתנה רק דרך המתודות שלו.' },
    {
      type: 'code',
      title: 'מחלקה, אובייקט ו-new',
      file: 'BankAccount.cs',
      code: `class BankAccount
{
    private decimal _balance;                 // שדה פרטי — מצב

    public string Owner { get; set; } = "";   // property — API

    public void Deposit(decimal amount)       // מתודה — התנהגות
    {
        if (amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount));
        _balance += amount;
    }

    public decimal Balance => _balance;
}

var acc = new BankAccount { Owner = "Dana" };
acc.Deposit(500);
Console.WriteLine($"{acc.Owner}: {acc.Balance}");`,
      bullets: [
        'מחלקה = תבנית; אובייקט = מופע עם מצב משלו',
        'שדות פרטיים (_camelCase), API ציבורי (PascalCase)',
        'המצב משתנה רק דרך מתודות — אנקפסולציה',
        'object initializer: new X { Prop = ... }',
      ],
      notes: 'שאלו: למה _balance פרטי? כי אחרת כל אחד יכול לכתוב acc._balance = -1000. Deposit היא השומר בשער. זה הרעיון של אנקפסולציה, נחזור אליו במודול 03.',
    },
    {
      type: 'cards',
      title: 'סוגי properties',
      cards: [
        { icon: 'pen', heading: 'auto', text: 'public string Name { get; set; } — המהדר יוצר שדה נסתר' },
        { icon: 'lock', heading: 'private set', text: 'public decimal Balance { get; private set; } — קריאה לכולם, כתיבה רק מבפנים' },
        { icon: 'key', heading: 'init / required', text: 'public required string Sku { get; init; } — חובה ביצירה, ואז read-only' },
        { icon: 'bolt', heading: 'computed', text: 'public decimal PriceWithVat => Price * 1.18m; — מחושב בכל קריאה, בלי שדה' },
        { icon: 'check', heading: 'עם ולידציה', text: 'set => _stock = value >= 0 ? value : throw new ArgumentOutOfRangeException()' },
        { icon: 'eye', heading: 'read-only', text: 'public string Id { get; } — נקבע רק בבנאי' },
      ],
      notes: 'property נראה כמו שדה מבחוץ אבל הוא זוג מתודות. זה מה שמאפשר לשנות מימוש בלי לשבור קוד. required + init הוא השילוב המודרני לאובייקטים שחייבים להיות תקינים מהרגע הראשון. הראו ב-Demo.Classes את Product.',
    },
    {
      type: 'code',
      title: 'בנאים: overloads, this(...), primary constructor',
      code: `class BankAccount
{
    public string Id { get; }
    public string Owner { get; set; }

    public BankAccount(string id, string owner)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        Id = id;
        Owner = owner;
    }

    public BankAccount(string id, string owner, decimal initial)
        : this(id, owner) => Deposit(initial);     // שרשור
}

// C# 12 — primary constructor
class Sensor(string location, double initial)
{
    private readonly List<double> _readings = [initial];
    public string Location => location;
}`,
      bullets: [
        'הבנאי אחראי להביא את האובייקט למצב תקין',
        'ולידציה פעם אחת — בכניסה',
        ': this(...) מונע כפילות בין בנאים',
        'primary constructor: פרמטרים זמינים בכל המחלקה',
      ],
      notes: 'הדגישו: בנאי שמאמת חוסך if-ים בכל שאר המחלקה. primary constructor הוא סוכר תחבירי — הפרמטרים לא הופכים ל-properties אוטומטית (בניגוד ל-record). בנאי צריך להיות מהיר — לא קריאות רשת.',
    },
    {
      type: 'two-col',
      title: 'record מול class',
      right: {
        heading: 'class — ישות עם זהות',
        bullets: ['שוויון לפי הפניה', 'מצב שמשתנה לאורך זמן', 'BankAccount, Order, Library', 'ToString = שם הטיפוס (אלא אם דורסים)'],
      },
      left: {
        heading: 'record — נתונים',
        code: `record Person(string Name, int Age);

var p1 = new Person("Dana", 30);
var p2 = new Person("Dana", 30);
p1 == p2;            // True (לפי ערך)
Console.WriteLine(p1);
// Person { Name = Dana, Age = 30 }

var older = p1 with { Age = 31 };
var (name, age) = older;`,
      },
      notes: 'כלל אצבע: אם שני אובייקטים עם אותם נתונים הם "אותו דבר" — record. אם הזהות חשובה (שני חשבונות עם אותה יתרה) — class. record מקבל בחינם Equals, GetHashCode, ToString, with, deconstruction.',
    },
    {
      type: 'bullets',
      title: 'static: שייך למחלקה, לא לאובייקט',
      icon: 'cube',
      bullets: [
        { text: 'const decimal MinDeposit = 10m; — קבוע בזמן קומפילציה' },
        { text: 'static int Count { get; private set; } — משותף לכל המופעים' },
        { text: 'static BankAccount OpenSavings(string owner) — factory method', sub: ['שם משמעותי במקום בנאי מספר 4'] },
        { text: 'static class MathUtils — ארגז כלים, אי אפשר ליצור מופע', sub: ['Math, Console, File הן כאלה'] },
        { text: 'מתודה סטטית לא יכולה לגשת ל-this' },
        { text: 'this — ההפניה לאובייקט הנוכחי; לרוב לא צריך לכתוב אותו' },
      ],
      notes: 'שאלה טובה לכיתה: מה יקרה אם Count יהיה לא-static? כל אובייקט יספור את עצמו בלבד. הראו ב-Demo.Classes את Product.CreateFreeSample. אחרי השקף — Lab 1.',
    },
    {
      type: 'lab',
      title: 'Lab 1 — מערכת ספרייה',
      goal: 'לבנות Book עם properties ובנאי שמאמת, ו-Library שמנהלת List<Book>: הוספה, חיפוש, השאלה והחזרה.',
      duration: '45 דקות',
      deliverable: 'Starter שעובר את כל קריטריוני הקבלה ב-README; בונוס: BorrowedCount, Oldest, DueDate.',
      tasks: [
        'Book: Isbn (read-only), Title, Author, Year, IsBorrowed (private set), Age מחושב',
        'בנאי עם ArgumentException.ThrowIfNullOrWhiteSpace ובדיקת שנה',
        'Borrow / Return שזורקות InvalidOperationException על מצב לא חוקי',
        'Library: AddBook (ISBN ייחודי), SearchByTitle, SearchByAuthor, FindByIsbn',
        'Books כ-IReadOnlyList<Book>; GetAvailable ממוין לפי כותרת',
        'הסירו את ההערות ב-Seed והריצו את כל התפריט',
      ],
      notes: 'Labs/Lab1-LibrarySystem/README.md. ה-Starter מתקמפל וכל מתודה זורקת NotImplementedException. עברו בין המשתתפים: הטעות הנפוצה היא set ציבורי על IsBorrowed. מי שסיים — בונוס. 45 דקות ואז הפסקה קצרה.',
    },

    // ---------------- 03 OOP ----------------
    { type: 'section', number: '03', title: 'עקרונות OOP', subtitle: 'אנקפסולציה · ירושה · פולימורפיזם · ממשקים · הפשטה', notes: 'ארבעת העקרונות הם ארבע תשובות לשאלה אחת: איך לכתוב קוד שאפשר לשנות בלי לשבור? נלמד אותם דרך הכלים של C#.' },
    {
      type: 'cards',
      title: 'ארבעה עקרונות, שאלה אחת: איך משנים בלי לשבור?',
      cards: [
        { icon: 'lock', heading: 'אנקפסולציה', text: 'המצב פרטי, ה-API קטן. שינוי מימוש לא שובר את המשתמשים.' },
        { icon: 'sitemap', heading: 'ירושה', text: 'מחלקה נגזרת מקבלת ומרחיבה מחלקת בסיס. רק ליחסי "הוא סוג של".' },
        { icon: 'arrows', heading: 'פולימורפיזם', text: 'קריאה אחת, התנהגות לפי הטיפוס האמיתי. מחליף switch על סוגים.' },
        { icon: 'puzzle', heading: 'הפשטה', text: 'תלות בחוזה (ממשק / abstract), לא במימוש. אפשר להחליף מימוש.' },
      ],
      notes: 'שקף מסגרת — נחזור לכל כרטיס בנפרד. הדגישו שאלה לא כללים לשינון אלא כלים לצמצום עלות שינוי. ירושה היא הכלי שהכי קל להשתמש בו לא נכון.',
    },
    {
      type: 'bullets',
      title: 'אנקפסולציה: access modifiers',
      icon: 'lock',
      bullets: [
        { text: 'private — רק המחלקה עצמה (ברירת המחדל לחברים)' },
        { text: 'protected — המחלקה ויורשיה' },
        { text: 'internal — כל הקוד באותו פרויקט (ברירת המחדל למחלקות)' },
        { text: 'public — כולם' },
        { text: 'protected internal / private protected — שילובים נדירים' },
        { text: 'הכלל: התחילו מ-private, הרחיבו רק כשצריך', sub: ['פחות שטח ציבורי = פחות מה שיכול להישבר'] },
      ],
      notes: 'internal שימושי כשבונים ספרייה: מחלקות עזר לא צריכות להיות public. protected שדות — עדיף protected מתודות, כי יורשים אחרת תלויים במימוש הפנימי.',
    },
    {
      type: 'code',
      title: 'ירושה: virtual, override, base, sealed',
      code: `class Employee(int id, string name)
{
    public int Id { get; } = id;
    public string Name { get; } = name;
    public virtual decimal MonthlyPay() => 0;
    public virtual string Describe() => $"#{Id} {Name}";
}

class Manager(int id, string name, decimal salary, decimal bonus)
    : Employee(id, name)
{
    public override decimal MonthlyPay() => salary / 12 + bonus;
    public override string Describe()
        => base.Describe() + " (manager)";
}

sealed class Intern(int id, string name) : Employee(id, name)
{
    public override decimal MonthlyPay() => 3_000;
}`,
      bullets: [
        'virtual — מותר לדרוס; override — דורס',
        'base.X() — המימוש של האב (להרחיב, לא להחליף)',
        'sealed — סוף ההיררכיה',
        'ירושה יחידה בלבד; ממשקים — כמה שרוצים',
        'is-a אמיתי בלבד: מנהל הוא עובד',
      ],
      notes: 'טעות נפוצה: מתודה באותו שם בלי override — זה hiding (new), והפולימורפיזם לא עובד. המהדר מזהיר. הבנאי של היורש חייב לקרוא לבנאי האב — כאן דרך primary constructor.',
    },
    {
      type: 'code',
      title: 'abstract + פולימורפיזם',
      code: `abstract class Shape
{
    public abstract double Area();                  // חובה לממש
    public virtual string Describe()
        => $"{GetType().Name} area={Area():F2}";    // משותף
}

class Circle(double r) : Shape
{
    public override double Area() => Math.PI * r * r;
}

class Rect(double w, double h) : Shape
{
    public override double Area() => w * h;
}

List<Shape> shapes = [new Circle(1), new Rect(2, 3)];
foreach (var s in shapes)
    Console.WriteLine(s.Describe());   // כל אחד לפי הטיפוס שלו
double total = shapes.Sum(s => s.Area());`,
      bullets: [
        'abstract class — אי אפשר new Shape()',
        'abstract method — הצהרה בלי גוף, יורש חייב לממש',
        'Describe משתמש ב-Area שעוד לא קיים — נפתר בזמן ריצה',
        'הלולאה לא תשתנה כשנוסיף Triangle — Open/Closed',
      ],
      notes: 'זה הלב של המודול. הריצו את Demo.Polymorphism. שאלו: מה היה קורה בלי פולימורפיזם? switch על סוג הצורה בכל מקום, וכל צורה חדשה = לעדכן את כולם.',
    },
    {
      type: 'code',
      title: 'ממשקים: חוזה בלי מימוש',
      code: `interface IPayable
{
    string PayeeName { get; }
    decimal MonthlyPay();
    // default member (C# 8+)
    string PaySlip() => $"{PayeeName}: {MonthlyPay():N2}";
}

class Contractor(string company, decimal fee) : IPayable
{
    public string PayeeName => company;
    public decimal MonthlyPay() => fee;
}

List<IPayable> payroll =
[
    new Manager(1, "Noa", 360_000, 2_000),
    new Contractor("Acme", 12_000),
];
foreach (var p in payroll) Console.WriteLine(p.PaySlip());`,
      bullets: [
        'קבלן הוא לא עובד — אבל משלמים לו',
        'מחלקה מממשת כמה ממשקים',
        'default member: מימוש ברירת מחדל, נגיש דרך הממשק',
        'IComparable<T>, IEnumerable<T>, IDisposable — ממשקי .NET',
      ],
      notes: 'הממשק מאפשר ל-Payroll לעבוד עם "כל מה שמשלמים לו" בלי אב משותף. default members — כלי להוסיף מתודה לממשק קיים בלי לשבור מממשים; לא תחליף למחלקת בסיס.',
    },
    {
      type: 'two-col',
      title: 'abstract class או interface?',
      right: {
        heading: 'abstract class',
        bullets: ['ירושה אחת בלבד', 'שדות, בנאים, מימוש משותף', 'יחס is-a חזק', 'Shape, Employee'],
      },
      left: {
        heading: 'interface',
        bullets: ['כמה שרוצים', 'חוזה (+ default members)', 'יכולת: can-do', 'IPayable, IDisposable, IComparable'],
      },
      notes: 'לעיתים קרובות משלבים: ממשק לחוזה, ומחלקה אבסטרקטית שמממשת אותו חלקית לנוחות היורשים. אם מתלבטים — ממשק: הוא מחייב פחות.',
    },
    {
      type: 'code',
      title: 'Composition over inheritance',
      code: `// ❌ שירות הזמנות "הוא" לוגר?
class OrderService : ConsoleLogger { }

// ✅ יש לו לוגר — וכל לוגר יתאים
interface ILogger { void Log(string message); }

class ConsoleLogger : ILogger
{
    public void Log(string m) => Console.WriteLine(m);
}

class OrderService(ILogger logger)
{
    public void Place(string id)
    {
        logger.Log($"placing {id}");
    }
}

var svc = new OrderService(new ConsoleLogger());`,
      bullets: [
        'ירושה = הקשר החזק ביותר; היורש תלוי בכל פרט של האב',
        'has-a במקום is-a לרוב המקרים',
        'אפשר להחליף מימוש: קובץ, fake לבדיקות',
        'זה הבסיס ל-Dependency Injection (יום 2)',
      ],
      notes: 'כלל אצבע: ירושה למודל דומיין אמיתי (צורות, עובדים); הרכבה לכל השאר. NullLogger בדמו מראה החלפה בלי לגעת ב-OrderService.',
    },
    {
      type: 'code',
      title: 'is / as / pattern matching',
      code: `if (shape is Circle c) Console.WriteLine(c.Radius);

var rect = shape as Rectangle;        // null אם לא מתאים
var forced = (Rectangle)shape;        // InvalidCastException

string label = shape switch
{
    Circle { Radius: > 10 } => "big circle",
    Circle c                => $"circle r={c.Radius}",
    Rectangle { Width: var w, Height: var h } when w == h
                            => "square-ish",
    null                    => "nothing",
    _                       => "other",
};`,
      bullets: [
        'is — בדיקה + המרה + משתנה חדש',
        'property pattern { Prop: תנאי }',
        'when — תנאי נוסף',
        'סדר: ספציפי לפני כללי',
        'switch על טיפוסים בכל מקום? אולי חסר virtual',
      ],
      notes: 'המהדר מזהיר על case שלא ניתן להגיע אליו אם הסדר הפוך. הזכירו: pattern matching מצוין לנתונים חיצוניים (JSON, קלט); לדומיין שלכם — פולימורפיזם.',
    },
    {
      type: 'code',
      title: 'ToString, Equals, GetHashCode',
      code: `class Money(decimal amount, string currency)
{
    public decimal Amount => amount;
    public string Currency => currency;

    public override bool Equals(object? obj) =>
        obj is Money o && o.Amount == Amount
                       && o.Currency == Currency;

    public override int GetHashCode()
        => HashCode.Combine(Amount, Currency);

    public override string ToString()
        => $"{Amount} {Currency}";
}

// או פשוט:
record Money(decimal Amount, string Currency);`,
      bullets: [
        'ברירת מחדל: Equals לפי הפניה, ToString = שם הטיפוס',
        'Equals ו-GetHashCode — תמיד יחד',
        'Dictionary / HashSet מסתמכים עליהם',
        'record עושה את כל זה אוטומטית',
      ],
      notes: 'הראו ב-Demo.Polymorphism: HashSet<Money> עם שני אובייקטים שווים מכיל 1. בלי GetHashCode עקבי, האוסף "מאבד" איברים. זו הסיבה העיקרית ש-records קיימים. אחרי השקף — Lab 2.',
    },
    {
      type: 'lab',
      title: 'Lab 2 — צורות ועובדים',
      goal: 'שתי היררכיות: Shape אבסטרקטית (Circle/Rectangle/Triangle/Square) ו-payroll עם Employee, Manager, HourlyEmployee ו-Contractor דרך IPayable.',
      duration: '60 דקות',
      deliverable: 'תלושי שכר לכל IPayable (סה"כ 78,000 עם נתוני הדוגמה), Describe רק לעובדים, ובונוס switch עם patterns.',
      tasks: [
        'Shape abstract: Name, Area(), Perimeter(), ToString משותף',
        'Triangle מאמת אי-שוויון המשולש בבנאי; Square : Rectangle, sealed',
        'IPayable עם default member PaySlip()',
        'Employee abstract : IPayable; Salaried / Hourly (שעות נוספות ×1.5)',
        'Manager : SalariedEmployee עם Bonus ו-Reports; base.CalculateMonthlyPay()',
        'Contractor מממש IPayable בלי לרשת מ-Employee; Payroll עם List<IPayable>',
      ],
      notes: 'Labs/Lab2-ShapesAndEmployees/README.md. חלק א\' (צורות) 15 דקות, חלק ב\' 45. השאלה שהכי שווה לדון בה: למה Contractor לא יורש מ-Employee? אחרי הלאב — הפסקה ואז אוספים.',
    },

    // ---------------- 04 אוספים ----------------
    { type: 'section', number: '04', title: 'אוספים וגנריקה', subtitle: 'מערכים, List, Dictionary, HashSet, Queue/Stack, IEnumerable, Repository<T>', notes: 'המסר: לכל צורך יש אוסף נכון, והבחירה נקבעת לפי הפעולה שחוזרת הכי הרבה. הטבלה עם Big-O היא הכלי.' },
    {
      type: 'code',
      title: 'מערכים: 1D, 2D, jagged',
      code: `int[] scores = [90, 75, 88];           // collection expression
int[] zeros = new int[5];
Console.WriteLine(scores[0]);           // 90
Console.WriteLine(scores[^1]);          // 88 — מהסוף
int[] tail = scores[1..];               // range
Array.Sort(scores);

int[,] grid = { { 1, 2, 3 }, { 4, 5, 6 } };
Console.WriteLine(grid[1, 2]);          // 6
Console.WriteLine(grid.GetLength(0));   // 2 שורות

int[][] jagged = [[1], [2, 3], [4, 5, 6]];
Console.WriteLine(jagged[2].Length);    // 3`,
      bullets: [
        'גודל קבוע, גישה O(1) לפי אינדקס',
        'אינדקס מתחיל ב-0; ^1 מהסוף; a..b טווח',
        '2D למטריצה מלבנית; jagged לשורות באורך שונה',
        'מערך הוא reference type',
      ],
      notes: 'מערכים הם הבסיס — List<T> בנוי עליהם. הראו ^1 ו-ranges, הם חדשים למי שמגיע מ-Java. Length למערך, Count ל-List.',
    },
    {
      type: 'code',
      title: 'List<T> ו-Dictionary<K,V>',
      code: `var names = new List<string> { "Dana", "Yossi" };
names.Add("Noa");
names.Remove("Yossi");
bool has = names.Contains("Noa");       // O(n)
Console.WriteLine(names.Count);

var stock = new Dictionary<string, int>
{
    ["apple"] = 10,
};
stock["cherry"] = 25;                   // הוספה / עדכון
stock["apple"] += 5;
if (stock.TryGetValue("kiwi", out int q)) { }
foreach (var (fruit, qty) in stock)
    Console.WriteLine($"{fruit}: {qty}");

var byName = new Dictionary<string, Product>(
    StringComparer.OrdinalIgnoreCase);`,
      bullets: [
        'List — מערך שיודע לגדול; Count, לא Length',
        'Dictionary — מפתח → ערך, חיפוש O(1)',
        'dict[key] על מפתח חסר → KeyNotFoundException',
        'TryGetValue / GetValueOrDefault / TryAdd',
        'StringComparer למפתחות לא תלויי רישיות',
      ],
      notes: 'שני האוספים שתשתמשו בהם 90% מהזמן. הדגישו את TryGetValue — התבנית Try חוזרת. capacity: new List<int>(100_000) חוסך הכפלות. Demo.Collections חלקים 2–3.',
    },
    {
      type: 'cards',
      title: 'HashSet, Queue, Stack, IEnumerable',
      cards: [
        { icon: 'check', heading: 'HashSet<T>', text: 'קבוצה בלי כפילויות, Contains O(1). Add מחזיר false אם קיים. UnionWith / IntersectWith.' },
        { icon: 'arrows', heading: 'Queue<T> — FIFO', text: 'Enqueue / Dequeue / Peek. עיבוד לפי סדר הגעה: משימות, הדפסה.' },
        { icon: 'thread', heading: 'Stack<T> — LIFO', text: 'Push / Pop / Peek. Undo, ניווט אחורה, פרסינג.' },
        { icon: 'list', heading: 'IEnumerable<T>', text: 'המכנה המשותף: "אפשר לעבור עליי עם foreach". קבלו אותו כפרמטר.' },
      ],
      notes: 'תרגיל 9 בתרגילים מבקש לבחור אוסף לכל תרחיש — שווה לעשות אותו בכיתה בעל-פה. IEnumerable אין לו Count/אינדקס/Add — רק מעבר.',
    },
    {
      type: 'code',
      title: 'yield return — IEnumerable לפי דרישה',
      code: `static IEnumerable<int> Evens(int max)
{
    for (int i = 0; i <= max; i += 2)
        yield return i;      // החזר איבר אחד, המשך בפעם הבאה
}

foreach (var n in Evens(10))
    Console.Write($"{n} ");           // 0 2 4 6 8 10

// מיליון איברים? רק 3 מחושבים
var first3 = Evens(1_000_000).Take(3);`,
      bullets: [
        'המתודה לא רצה עד שעוברים על התוצאה',
        'אין רשימה זמנית בזיכרון',
        'זה הבסיס ל-deferred execution של LINQ',
        'אפשר לשרשר: Where, Take, Select',
      ],
      notes: 'הראו בדיבאגר: breakpoint בתוך Evens ותראו שהוא נכנס רק כשה-foreach מבקש. זה מכין את הקרקע ל-LINQ במודול הבא.',
    },
    {
      type: 'code',
      title: 'גנריקה: Repository<T> עם constraint',
      code: `interface IEntity { int Id { get; } }

class Repository<T> where T : IEntity        // constraint
{
    private readonly List<T> _items = [];
    private readonly Dictionary<int, T> _byId = [];   // אינדקס

    public IReadOnlyList<T> All => _items;

    public void Add(T item)
    {
        if (!_byId.TryAdd(item.Id, item))          // כי T : IEntity
            throw new InvalidOperationException($"Id {item.Id}");
        _items.Add(item);
    }

    public T? GetById(int id) => _byId.GetValueOrDefault(id);
}

record Customer(int Id, string Name) : IEntity;
var repo = new Repository<Customer>();`,
      bullets: [
        'T — placeholder לטיפוס; נקבע בשימוש',
        'where T : IEntity — מה מותר לעשות עם T',
        'constraints: class, struct, new(), IComparable<T>, notnull',
        'List<T> ו-Dictionary<K,V> בנויים בדיוק כך',
        'מתודה גנרית: static T Max<T>(T a, T b) where T : IComparable<T>',
      ],
      notes: 'בלי constraint, T הוא כמו object — אי אפשר לקרוא item.Id. הדגישו את האינדקס: List לסדר + Dictionary לחיפוש — זיכרון זול, סריקה יקרה.',
    },
    {
      type: 'bullets',
      title: 'בחירת האוסף הנכון',
      icon: 'search',
      bullets: [
        { text: 'T[] — גודל קבוע, הכי מהיר: אינדקס O(1), חיפוש O(n)' },
        { text: 'List<T> — רשימה כללית: אינדקס O(1), Add O(1)*, Contains O(n)' },
        { text: 'Dictionary<K,V> — מפתח → ערך: חיפוש/הוספה/הסרה O(1)' },
        { text: 'HashSet<T> — ייחודיות ושייכות: O(1)' },
        { text: 'Queue<T> / Stack<T> — סדר עיבוד: O(1)' },
        { text: 'SortedDictionary<K,V> — מיון תמידי: O(log n)' },
        { text: 'השאלות: סדר? חיפוש לפי מפתח? כפילויות? כמה איברים?', bold: true, sub: ['List.Contains בלולאה על 100K = 10 מיליארד השוואות; HashSet = 100K'] },
      ],
      notes: 'הטבלה המלאה במודול 04. הדוגמה של 100K מסבירה למה זה משנה: ההבדל בין שנייה לדקות. * = amortized — לפעמים הקצאה מחדש.',
    },
    {
      type: 'code',
      title: 'IReadOnlyList<T> וניהול נתונים יעיל',
      code: `class Library
{
    private readonly List<Book> _books = [];

    // ❌ public List<Book> Books => _books;   // כל אחד יכול Clear()
    // ✅ רואים, לא משנים:
    public IReadOnlyList<Book> Books => _books;

    public void Add(Book b) { /* ולידציה */ _books.Add(b); }
}

var big = new List<int>(capacity: 100_000);   // בלי הכפלות

Span<int> window = data.AsSpan(2, 4);          // חלון בלי העתקה`,
      bullets: [
        'List<T> מממש IReadOnlyList<T> — אין העתקה',
        'capacity כשיודעים את הגודל',
        'ToList() רק כשצריך צילום מצב',
        'StringBuilder לבניית מחרוזת בלולאה',
        'Span<T> — לביצועים גבוהים, לא ליום-יום',
        'מדדו לפני שמייעלים (Stopwatch)',
      ],
      notes: 'IReadOnlyList חוזר בכל הלאבים — זו אנקפסולציה של אוספים. Span רק כדי שיכירו את השם; רוב ה-API של .NET מקבל אותו. ImmutableList כשצריך אוסף שבאמת לא משתנה.',
    },

    // ---------------- 05 delegates / LINQ ----------------
    { type: 'section', number: '05', title: 'Delegates, Lambdas, Events ו-LINQ', subtitle: 'להעביר התנהגות, לא רק נתונים', notes: 'עד עכשיו העברנו נתונים למתודות. עכשיו נעביר התנהגות: "סנן לפי התנאי הזה", "כשקורה X — הודע לי". LINQ בנוי על זה.' },
    {
      type: 'code',
      title: 'Delegates, Func/Action ו-lambdas',
      code: `delegate int MathOp(int a, int b);          // טיפוס של מתודה
MathOp add = (a, b) => a + b;
Console.WriteLine(add(2, 3));               // 5

static int Apply(int x, int y, MathOp op) => op(x, y);
Apply(10, 4, (a, b) => a - b);              // התנהגות כפרמטר

// המובנים של .NET — כמעט תמיד מספיקים
Func<int, int, int> power = (b, e) => (int)Math.Pow(b, e);
Action<string> shout = s => Console.WriteLine(s.ToUpper());
Predicate<int> isEven = n => n % 2 == 0;
Func<int> answer = () => 42;

Action pipeline = () => Console.Write("1 ");
pipeline += () => Console.Write("2 ");     // multicast
pipeline();`,
      bullets: [
        'delegate = מתודה כערך',
        'Func<..., TResult> מחזיר; Action לא',
        'lambda: (פרמטרים) => ביטוי',
        'multicast (+=) — הבסיס לאירועים',
      ],
      notes: 'Demo.LinqDelegates חלק 1. שאלו: כמה מתודות FindBy... הייתם כותבים בלי זה? Find(Func<T,bool>) אחת מחליפה את כולן — זה Lab 3.',
    },
    {
      type: 'code',
      title: 'Closures ואירועים',
      code: `static Func<int> MakeCounter()
{
    int count = 0;
    return () => ++count;        // count "נלכד"
}
var next = MakeCounter();
next(); next(); next();          // 1 2 3

class Thermometer
{
    public event EventHandler<TempArgs>? Changed;

    public void Set(double value)
    {
        // ... עדכון ...
        Changed?.Invoke(this, new TempArgs(value));
    }
}

var t = new Thermometer();
t.Changed += (sender, e) => Console.WriteLine(e.Value);
t.Changed += (_, e) => { if (e.Value > 30) TurnOnAC(); };`,
      bullets: [
        'closure לוכד את המשתנה, לא את הערך',
        'event = delegate שמבחוץ רק += / -=',
        'EventHandler<T> — (sender, e) — המוסכמה',
        '?.Invoke — אם אין מנויים',
        'ה-publisher לא מכיר את המנויים',
      ],
      notes: 'אירועים הם הבסיס ל-WPF/WinForms ביום 4 — כל לחיצה היא event. הזכירו לשחרר מנויים (-=) מאובייקטים ארוכי-חיים. Lab 3 משתמש ב-LowStock.',
    },
    {
      type: 'code',
      title: 'LINQ — תיאור מה, לא איך',
      code: `var names = orders.Where(o => o.Total > 100)
                  .Select(o => o.Customer);

var sorted = orders.OrderByDescending(o => o.Total)
                   .ThenBy(o => o.Customer);

foreach (var g in orders.GroupBy(o => o.Category))
    Console.WriteLine($"{g.Key}: {g.Count()} orders");

var first = orders.First(o => o.Category == "Books");   // זורק אם אין
var maybe = orders.FirstOrDefault(o => o.Total > 5000);  // null

bool any = orders.Any(o => o.Total > 1000);
decimal max = orders.Max(o => o.Total);

Dictionary<string, decimal> perCustomer = orders
    .GroupBy(o => o.Customer)
    .ToDictionary(g => g.Key, g => g.Sum(o => o.Total));`,
      bullets: [
        'Where / Select / OrderBy / GroupBy',
        'First / FirstOrDefault / Single / Any / All',
        'Count / Sum / Max / Average / MinBy',
        'ToList / ToDictionary / ToHashSet',
        'Skip / Take לעימוד; Distinct; Chunk',
        'query syntax (from … where … select) — אותו דבר, תחביר SQL',
      ],
      notes: 'שורה אחת במקום לולאה + if + רשימה זמנית. הראו ב-Demo.LinqDelegates חלק 4. First זורק כשאין — FirstOrDefault + בדיקת null.',
    },
    {
      type: 'code',
      title: 'Deferred execution — השאילתה רצה בצריכה',
      code: `var numbers = new List<int> { 1, 2, 3 };

var evens = numbers.Where(n => n % 2 == 0);   // כלום לא קרה

numbers.Add(4);

Console.WriteLine(string.Join(",", evens));    // 2,4 — רץ עכשיו

var snapshot = numbers.Where(n => n % 2 == 0).ToList();  // מקפיא
numbers.Add(6);
Console.WriteLine(string.Join(",", snapshot)); // 2,4`,
      bullets: [
        'Where/Select/OrderBy בונים תוכנית — לא מבצעים',
        'foreach / ToList / Count / First מבצעים',
        'שאילתה שנצרכת פעמיים רצה פעמיים',
        'חריגה ב-lambda נזרקת בזמן הצריכה',
        'ToList() כשצריך תוצאה יציבה',
      ],
      notes: 'הנקודה הכי חשובה ב-LINQ ומקור לבאגים מבלבלים. הראו בדיבאגר breakpoint בתוך ה-lambda — הוא נעצר רק ב-string.Join. Lab 3 מדגים את זה בסוף.',
    },
    {
      type: 'bullets',
      title: 'סגנון פונקציונלי',
      icon: 'wand',
      bullets: [
        { text: 'פונקציות טהורות: אותם קלטים → אותו פלט, בלי תופעות לוואי', sub: ['קל לבדוק, קל להרכיב, בטוח ב-threads'] },
        { text: 'אי-שינוי: record + with, IReadOnlyList, ImmutableList' },
        { text: 'הרכבה: מתודות קטנות שמקבלות ומחזירות Func / IEnumerable' },
        { text: 'הפרדה בין חישוב ל-I/O: חשבו בטהור, הדפיסו בחוץ' },
        { text: 'LINQ לחישוב; foreach לתופעות לוואי' },
        { text: 'לא הכל חייב להיות פונקציונלי — אבל כשאפשר, עדיף' },
      ],
      notes: 'Reports ב-Lab 3 היא static class של פונקציות טהורות. ApplyDiscount בדמו מחזיר Cart חדש בלי לגעת בישן. אחרי השקף — Lab 3 (או דילוג ל-06 ו-Lab 4 לפי הזמן).',
    },
    {
      type: 'lab',
      title: 'Lab 3 — מלאי עם LINQ, delegates ואירועים',
      goal: 'Inventory עם Dictionary לפי SKU ו-HashSet לקטגוריות, Find(Func) ו-ApplyToCategory(Action), אירוע LowStock, ודוחות LINQ טהורים.',
      duration: '60 דקות',
      deliverable: 'הרצה שמדפיסה אזהרות LOW STOCK בזמן מכירות, דוחות לפי קטגוריה / Top 3 / מלאי נמוך, ובונוס deferred execution.',
      tasks: [
        'Dictionary<string, StockItem> ו-HashSet<string> עם StringComparer.OrdinalIgnoreCase',
        'event EventHandler<LowStockEventArgs>? LowStock; הפעלה ב-Sell עם ?.Invoke',
        'Find(Func<StockItem, bool>) ו-ApplyToCategory(string, Action<StockItem>)',
        'Reports: TotalValue, ByCategory (GroupBy), TopByValue, LowStock, PriceIndex (ToDictionary)',
        'מנוי לאירוע ב-Program שמדפיס אזהרה',
        'בונוס: SkusInBoth (IntersectWith), query syntax, הדגמת deferred execution',
      ],
      notes: 'Labs/Lab3-InventoryLinq/README.md. Models.cs ו-Program.cs כבר כתובים — העבודה ב-Inventory.cs ו-Reports.cs. הנקודה הפדגוגית: ApplyToCategory צריך ToList() לפני הלולאה — הסבירו למה.',
    },

    // ---------------- 06 חריגות ודיבוג ----------------
    { type: 'section', number: '06', title: 'חריגות ודיבוג', subtitle: 'try/catch/finally, when, throw;, using, guard clauses, TryParse, הדיבאגר', notes: 'תוכנה נכשלת. השאלה היא איך: הודעה ברורה, מצב עקבי, משאבים משוחררים. ואז — הכלי שיחסוך הכי הרבה שעות: הדיבאגר.' },
    {
      type: 'code',
      title: 'try / catch / finally / when',
      code: `try
{
    var count = int.Parse(File.ReadAllText(path));
    Console.WriteLine(100 / count);
}
catch (FileNotFoundException ex)                 // ספציפי קודם
{
    Console.WriteLine($"missing: {ex.FileName}");
}
catch (InsufficientFundsException ex) when (ex.Shortfall > 1000)
{
    Escalate(ex);                                // exception filter
}
catch (Exception ex)                             // כללי — אחרון
{
    logger.Log(ex);
    throw;                                       // לא throw ex;
}
finally
{
    Console.WriteLine("always");                 // ניקוי
}`,
      bullets: [
        'ה-catch הראשון שמתאים מטפל — ספציפי לפני כללי',
        'finally רץ תמיד: אחרי return, continue, חריגה',
        'when — תפיסה מותנית, בלי לפרוש את ה-stack',
        'throw; שומר stack trace; throw ex; מוחק',
        'catch ריק = בליעת שגיאות',
      ],
      notes: 'Demo.Exceptions חלקים 1–3. הראו את ההבדל בין throw; ל-throw ex; בפלט — ה-stack מתחיל במקום אחר. CA2200 מזהיר על זה. catch (Exception) { } ריק הוא הפשע הנפוץ ביותר.',
    },
    {
      type: 'code',
      title: 'חריגות מותאמות אישית',
      code: `// בסיס לכל חריגות הדומיין
public class BankException(string message) : Exception(message);

public class InsufficientFundsException(
        string accountId, decimal requested, decimal available)
    : BankException(
        $"Insufficient funds in {accountId}: " +
        $"requested {requested:N2}, available {available:N2}")
{
    public string AccountId { get; } = accountId;
    public decimal Requested { get; } = requested;
    public decimal Available { get; } = available;
    public decimal Shortfall => Requested - Available;
}

// עטיפה בלי לאבד מידע
throw new DataAccessException("load failed", innerException: ex);`,
      bullets: [
        'סיומת Exception, ירושה מ-Exception',
        'הודעה ברורה שנבנית בבנאי',
        'properties לנתונים שהמטפל צריך',
        'היררכיה: catch (BankException) אחד לכל השגיאות העסקיות',
        'InnerException כשעוטפים',
      ],
      notes: 'מתי חריגה משלכם? כשלמטפל יש מה לעשות עם המידע (Shortfall) או כשצריך להבחין בין שגיאות עסקיות לטכניות. Lab 4 בונה בדיוק את ההיררכיה הזו.',
    },
    {
      type: 'code',
      title: 'using, guard clauses ו-TryParse',
      code: `using (var writer = new StreamWriter("log.txt"))
{
    writer.WriteLine("hello");
}   // Dispose() — גם אם הייתה חריגה

using var reader = new StreamReader("data.csv");   // עד סוף הבלוק

public void Transfer(Account? from, Account? to, decimal amount)
{
    ArgumentNullException.ThrowIfNull(from);
    ArgumentNullException.ThrowIfNull(to);
    ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amount);
    ArgumentException.ThrowIfNullOrEmpty(from.Id);
    // מכאן — הכל תקין
}

if (int.TryParse(input, out int age) && age is >= 0 and <= 120)
    Console.WriteLine($"ok {age}");
else
    Console.WriteLine("enter a number 0-120");   // בלי חריגה`,
      bullets: [
        'using = try/finally שקורא ל-Dispose',
        'בלי using: קובץ נעול, buffer לא נכתב',
        'guard clauses: נכשלים מוקדם, בכניסה',
        'ThrowIfNull / ThrowIfNullOrEmpty / ThrowIfNegative',
        'קלט צפוי = TryParse; הפרת כלל = throw',
      ],
      notes: 'ההבחנה החשובה: חריגה למצבים חריגים, ערך חזרה לתוצאות שגרתיות (חיפוש שלא מצא, קלט שגוי). Demo.Exceptions חלקים 4–6. TryGetValue, TryAdd, TryDequeue — אותה תבנית.',
    },
    {
      type: 'steps',
      title: 'אסטרטגיית דיבוג',
      steps: [
        { heading: 'שחזרו', text: 'הביאו את הבאג למצב דטרמיניסטי: אותו קלט, אותה תוצאה. בלי זה אין מה לדבג.' },
        { heading: 'שערו', text: 'איפה הערך "מתקלקל"? שימו breakpoint לפני המקום החשוד, לא אחריו.' },
        { heading: 'צעדו', text: 'F10 שורה-שורה עם Watch על המשתנים המעורבים. F11 להיכנס למתודה, Shift+F11 לצאת.' },
        { heading: 'בדקו השערות', text: 'Immediate Window: הקלידו ביטויים (_accounts.Keys, Calc(5)) בלי לשנות קוד.' },
        { heading: 'תקנו ובדקו', text: 'Edit & Continue, הריצו שוב את השחזור. רצוי — כתבו בדיקה (יום 2).' },
      ],
      notes: 'Console.WriteLine הוא לא דיבוג. השיטה: שחזור → השערה → צעדים → אימות. כשיש חריגה — Exception Settings + Call Stack מראים את המקור ולא את הסימפטום.',
    },
    {
      type: 'cards',
      title: 'ארגז הכלים של הדיבאגר (Visual Studio)',
      cards: [
        { icon: 'target', heading: 'Breakpoints', text: 'F9; קליק ימני → Conditions: amount > 1000, Hit Count. Run to Cursor: Ctrl+F10.' },
        { icon: 'eye', heading: 'Watch / Locals / DataTips', text: 'ביטויים חיים תוך כדי צעדים; ריחוף מעל משתנה; הצמדה עם סיכה.' },
        { icon: 'terminal', heading: 'Immediate Window', text: 'Ctrl+Alt+I — הריצו ביטויים ומתודות על המצב הנוכחי.' },
        { icon: 'sitemap', heading: 'Call Stack', text: 'Ctrl+Alt+C — איך הגעתי לכאן? לחיצה כפולה קופצת לפריים.' },
        { icon: 'warning', heading: 'Exception Settings', text: 'Ctrl+Alt+E — break when thrown: לעצור במקום הזריקה, לא ב-catch.' },
        { icon: 'pen', heading: 'Edit & Continue + Debug.Assert', text: 'שנו קוד בזמן השהיה; Assert בודק הנחות ב-Debug ונמחק ב-Release.' },
      ],
      notes: 'Lab 4 מתרגל כל אחד מהכלים האלה על באגים אמיתיים. ב-VS Code: אותם מושגים ב-Run and Debug עם launch.json. Debug.Assert להנחות פנימיות, לא לוולידציית קלט.',
    },

    // ---------------- 07 קוד נקי ----------------
    { type: 'section', number: '07', title: 'קוד נקי', subtitle: 'שמות, מתודות קטנות, SOLID, DRY, dotnet format, refactoring', notes: 'מודול קצר — 10 דקות — כי הרעיונות פשוטים והתרגול הוא בלאבים. קוד נקרא הרבה יותר ממה שהוא נכתב.' },
    {
      type: 'two-col',
      title: 'Refactoring — לפני ואחרי',
      right: {
        heading: 'לפני',
        code: `static double c(double a, int t, bool m)
{
    double r = 0;
    if (t == 1) { r = a * 0.9; if (m) r = r - 5; }
    else if (t == 2) { r = a * 0.8; if (m) r = r - 5; }
    else { r = a; if (m) r = r - 5; }
    if (r < 0) r = 0;
    return r * 1.18;
}`,
      },
      left: {
        heading: 'אחרי',
        code: `enum CustomerType { Regular, Silver, Gold }
const decimal VatRate = 1.18m, MemberDiscount = 5m;

static decimal CalculatePriceWithVat(
    decimal basePrice, CustomerType customer, bool isMember)
{
    decimal price = ApplyCustomerDiscount(basePrice, customer);
    if (isMember) price -= MemberDiscount;
    return Math.Max(price, 0) * VatRate;
}

static decimal ApplyCustomerDiscount(decimal p, CustomerType c)
    => c switch
    {
        CustomerType.Silver => p * 0.9m,
        CustomerType.Gold   => p * 0.8m,
        _ => p,
    };`,
      },
      notes: 'מה השתנה: שמות, enum במקום 1/2, קבועים במקום מספרי קסם, הכפילות if (m) r -= 5 יצאה החוצה, switch expression, decimal לכסף. אותה התנהגות — תרגיל 14 בודק. refactoring = שינוי מבנה בלי שינוי התנהגות, בצעדים קטנים.',
    },
    {
      type: 'cards',
      title: 'SOLID בקצרה',
      cards: [
        { icon: 'target', heading: 'S — Single Responsibility', text: 'סיבה אחת להשתנות. Order לא שולח מייל.' },
        { icon: 'box', heading: 'O — Open/Closed', text: 'סוג עובד חדש = מחלקה חדשה, לא case נוסף.' },
        { icon: 'arrows', heading: 'L — Liskov', text: 'יורש מתנהג כמו האב מבחינת המשתמש בו.' },
        { icon: 'puzzle', heading: 'I — Interface Segregation', text: 'ממשקים קטנים וממוקדים, לא 12 מתודות.' },
        { icon: 'link', heading: 'D — Dependency Inversion', text: 'תלות ב-ILogger, לא ב-new FileLogger().' },
        { icon: 'recycle', heading: 'DRY', text: 'אל תחזרו על ידע. חילוץ מתודה / delegate.' },
      ],
      notes: 'לא לשינון — לזיהוי. שאלו לכל כרטיס: איפה ראינו את זה היום? O = פולימורפיזם (Lab 2), D = OrderService(ILogger), I = IPayable קטן. אזהרה: הפשטה מוקדמת גרועה כמו כפילות.',
    },
    {
      type: 'bullets',
      title: 'הרגלים יומיומיים',
      icon: 'star',
      bullets: [
        { text: 'שמות שמתארים כוונה: elapsedDays, IsEligibleForDiscount, לא d / Check' },
        { text: 'מתודות קטנות (~20 שורות), דבר אחד; "ו" בשם = שתי מתודות' },
        { text: 'מספרי קסם → const / static readonly / enum' },
        { text: 'הערות ל-"למה", לא ל-"מה"; קוד בהערה — למחוק, יש git' },
        { text: '.editorconfig + dotnet format — סגנון אחד לצוות', sub: ['dotnet format --verify-no-changes ב-CI', 'TreatWarningsAsErrors בפרויקטים חדשים'] },
        { text: 'קוד רב-שימושי: ממשקים, גנריקה, delegates — הכלים של היום' },
      ],
      notes: 'dotnet new editorconfig יוצר קובץ ברירת מחדל. Extract Method: Ctrl+R, Ctrl+M; Rename: Ctrl+R, Ctrl+R. עכשיו — Lab 4, שמחבר חריגות, דיבוג וקוד נקי.',
    },
    {
      type: 'lab',
      title: 'Lab 4 — בנק עמיד: לדבג ולתקן',
      goal: 'Starter שמתקמפל ורץ אבל מכיל 8+ באגים: לוגיים, throw ex;, קובץ לוג ריק, וקריסות על קלט. למצוא עם הדיבאגר, לתקן, ולהוסיף היררכיית חריגות.',
      duration: '60 דקות',
      deliverable: 'תוכנית שלא נופלת על שום קלט, transactions.log מלא, BankException + 3 יורשות, ורשימת הבאגים שמצאתם.',
      tasks: [
        'שחזרו: open → deposit 100 → withdraw 80 → show. מה לא בסדר?',
        'Breakpoint ב-Account.Withdraw, Watch על Balance; Immediate: _accounts.Keys',
        'Exception Settings על BankException — איפה נעלם הפריים של Account.Withdraw?',
        'decimal.Parse → TryParse; RequireArgs guard clause; catch ל-ArgumentException',
        'processed++ ב-finally; using על TransactionLog',
        'AccountNotFound / InsufficientFunds (עם Shortfall) / InvalidAmount : BankException',
      ],
      notes: 'Labs/Lab4-BankRobust/README.md. רשימת הבאגים המלאה ב-Solution/NOTES.md — אל תחשפו אותה מראש. עודדו לעבוד עם הדיבאגר ולא עם Console.WriteLine. אפשר כשיעורי בית אם הזמן קצר.',
    },

    // ---------------- סיום ----------------
    {
      type: 'quote',
      text: 'קוד נקרא הרבה יותר פעמים ממה שהוא נכתב. כתבו למי שיקרא אותו בעוד חצי שנה — כנראה אתם.',
      author: 'עקרון מנחה לכל הקורס',
      notes: 'רגע לעצור לפני הסיכום. שאלו: איזה כלי מהיום תשתמשו בו כבר מחר בעבודה?',
    },
    {
      type: 'end',
      title: 'סיכום יום 1',
      bullets: [
        'C# מודרני: top-level, var, nullable, switch expressions, records',
        'OOP: אנקפסולציה, ירושה (virtual/override/abstract), ממשקים, פולימורפיזם',
        'אוספים: List / Dictionary / HashSet / Queue / Stack, גנריקה, IReadOnlyList',
        'delegates, events, LINQ ו-deferred execution',
        'חריגות: try/catch/finally/when, using, guard clauses, TryParse; הדיבאגר',
        'מחר: קבצים ו-JSON, async/await, HttpClient, בדיקות, Dependency Injection',
      ],
      footer: 'שיעורי בית: להשלים את Lab 3 / Lab 4 ואת התרגילים ★★ — נשתמש במודלים מחר',
      notes: 'סיכום ושאלות. ודאו שכולם יודעים איפה החומרים (Notes, Demos, Labs). הכנה ליום 2: Lab 3 ו-Lab 4 מושלמים — נבנה עליהם JSON ו-async.',
    },
  ],
};
