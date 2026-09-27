<div dir="rtl">

# מודול 04 — אוספים, גנריקה וניהול נתונים יעיל

כמעט כל תוכנית מנהלת "הרבה מ-משהו": רשימת ספרים, מלאי לפי מק"ט, תור של משימות. .NET מגיע עם סט אוספים עשיר ב-`System.Collections.Generic`, וכל אחד מהם טוב למשהו אחר. במודול הזה נלמד את החשובים, נבין **איך לבחור** (הטבלה עם ה-Big-O היא הכלי), נכתוב מחלקה גנרית משלנו, ונדבר על איך לחשוף אוספים החוצה בלי לאבד שליטה.

## מערכים

מערך הוא בלוק זיכרון רציף בגודל **קבוע**. הוא הכי מהיר לגישה לפי אינדקס, אבל אי אפשר להוסיף איברים.

<div dir="ltr">

```csharp
int[] scores = [90, 75, 88];             // collection expression (C# 12)
int[] zeros = new int[5];                // 5 אפסים
Console.WriteLine(scores[0]);            // 90 — אינדקס מתחיל ב-0
Console.WriteLine(scores[^1]);           // 88 — מהסוף
int[] middle = scores[1..];              // range: מאינדקס 1 עד הסוף
Array.Sort(scores);

int[,] grid = { { 1, 2, 3 }, { 4, 5, 6 } };      // דו-ממדי (מטריצה): 2 שורות, 3 עמודות
Console.WriteLine(grid[1, 2]);                   // 6
Console.WriteLine(grid.GetLength(0));            // 2

int[][] jagged = [[1], [2, 3], [4, 5, 6]];       // "משונן": מערך של מערכים באורכים שונים
Console.WriteLine(jagged[2].Length);             // 3
```

</div>

מערך דו-ממדי מתאים למטריצה מלבנית (לוח משחק); jagged — כשלכל שורה אורך אחר. מערכים הם reference types: העברה למתודה לא מעתיקה אותם.

## `List<T>` — הרשימה הדינמית

`List<T>` הוא מערך שיודע לגדול. זה האוסף שתשתמשו בו הכי הרבה.

<div dir="ltr">

```csharp
var names = new List<string> { "Dana", "Yossi" };
names.Add("Noa");
names.Insert(0, "Avi");
names.Remove("Yossi");                  // לפי ערך (true/false)
names.RemoveAt(0);                      // לפי אינדקס
bool has = names.Contains("Noa");       // O(n) — סריקה
int idx = names.IndexOf("Noa");
names.Sort();
Console.WriteLine($"{names.Count} items: {string.Join(", ", names)}");
```

</div>

`Count` (לא `Length`) הוא מספר האיברים; `Capacity` הוא כמה מקום הוקצה. כשה-List מתמלא הוא מקצה מערך כפול ומעתיק — לכן אם יודעים מראש כמה איברים יהיו, `new List<int>(capacity: 100_000)` חוסך הקצאות.

## `Dictionary<TKey, TValue>` — חיפוש לפי מפתח

מילון ממפה מפתח לערך עם חיפוש בזמן קבוע (בממוצע) — לא משנה אם יש 10 או 10 מיליון איברים. זה **ה**כלי לחיפוש "לפי מזהה".

<div dir="ltr">

```csharp
var stock = new Dictionary<string, int>
{
    ["apple"] = 10,
    ["banana"] = 0,
};
stock["cherry"] = 25;                             // הוספה או עדכון
stock["apple"] += 5;
if (stock.TryGetValue("kiwi", out int qty)) { }   // חיפוש בטוח — בלי חריגה
bool exists = stock.ContainsKey("apple");
stock.Remove("banana");
foreach (var (fruit, count) in stock) Console.WriteLine($"{fruit}: {count}");

var byName = new Dictionary<string, Product>(StringComparer.OrdinalIgnoreCase);   // מפתח לא תלוי רישיות
```

</div>

`stock["kiwi"]` על מפתח שלא קיים זורק `KeyNotFoundException` — לכן `TryGetValue`. המפתח חייב להיות יציב: אל תשתמשו באובייקט ש-`GetHashCode` שלו משתנה (מודול 03).

## `HashSet<T>`, `Queue<T>`, `Stack<T>`

- **`HashSet<T>`** — קבוצה בלי כפילויות, בדיקת שייכות O(1). "האם כבר ראינו את המזהה הזה?" `Add` מחזיר `false` אם האיבר כבר קיים. פעולות קבוצה: `UnionWith`, `IntersectWith`, `ExceptWith`.
- **`Queue<T>`** — תור: ראשון נכנס, ראשון יוצא (FIFO). `Enqueue`, `Dequeue`, `Peek`. עיבוד משימות לפי סדר הגעה.
- **`Stack<T>`** — מחסנית: אחרון נכנס, ראשון יוצא (LIFO). `Push`, `Pop`, `Peek`. Undo, ניווט "אחורה", פרסינג.

<div dir="ltr">

```csharp
var seen = new HashSet<int>();
foreach (var id in [17, 3, 17]) if (!seen.Add(id)) Console.WriteLine($"duplicate {id}");

var tasks = new Queue<string>(["build", "test", "deploy"]);
while (tasks.Count > 0) Console.WriteLine(tasks.Dequeue());

var undo = new Stack<string>();
undo.Push("typed a"); undo.Push("bold");
Console.WriteLine(undo.Pop());      // bold
```

</div>

## `IEnumerable<T>` ו-`foreach`

כל האוספים מממשים `IEnumerable<T>` — הממשק שאומר "אפשר לעבור עליי עם `foreach`". זה גם הטיפוס שכדאי לקבל כפרמטר כשכל מה שהמתודה צריכה הוא לעבור על האיברים: היא תעבוד עם מערך, `List`, `HashSet`, תוצאת LINQ — הכל.

אפשר גם לייצר `IEnumerable<T>` בעצמכם עם `yield return` — האיברים מיוצרים **לפי דרישה**, אחד בכל פעם:

<div dir="ltr">

```csharp
static IEnumerable<int> Evens(int max)
{
    for (int i = 0; i <= max; i += 2)
        yield return i;                        // "החזר את זה, ותמשיך מכאן בפעם הבאה"
}

foreach (var n in Evens(1_000_000).Take(3)) Console.WriteLine(n);   // רק 3 מחושבים
```

</div>

זהו הבסיס ל-deferred execution של LINQ (מודול 05). חשוב: `IEnumerable<T>` הוא רק "היכולת לעבור" — אין לו `Count`, אינדקס או `Add`. אם צריך אותם, בקשו `IReadOnlyList<T>` או `List<T>`.

## גנריקה — קוד שעובד לכל טיפוס

`List<T>` הוא **גנרי**: `T` הוא placeholder לטיפוס שנבחר בשימוש. בלי גנריקה היינו צריכים `IntList`, `StringList`... או `List` של `object` עם casting בכל מקום (כמו ב-.NET 1.0). גנריקה נותנת גם type safety וגם ביצועים.

אפשר לכתוב מחלקות ומתודות גנריות בעצמכם:

<div dir="ltr">

```csharp
interface IEntity { int Id { get; } }

class Repository<T> where T : IEntity            // constraint: T חייב לממש IEntity
{
    private readonly List<T> _items = [];
    private readonly Dictionary<int, T> _byId = [];   // אינדקס — חיפוש O(1) לפי Id

    public IReadOnlyList<T> All => _items;

    public void Add(T item)
    {
        if (!_byId.TryAdd(item.Id, item))           // אפשר כי T הוא IEntity
            throw new InvalidOperationException($"Id {item.Id} exists");
        _items.Add(item);
    }

    public T? GetById(int id) => _byId.GetValueOrDefault(id);
}

record Customer(int Id, string Name) : IEntity;
var repo = new Repository<Customer>();
```

</div>

Constraints נפוצים: `where T : class` (רק reference types), `where T : struct`, `where T : new()` (יש בנאי ריק), `where T : IComparable<T>` (אפשר להשוות), `where T : notnull`. בלי constraint, כל מה שאפשר לעשות עם `T` הוא מה שאפשר לעשות עם `object`.

מתודה גנרית: `static T Max<T>(T a, T b) where T : IComparable<T> => a.CompareTo(b) >= 0 ? a : b;` — המהדר מסיק את `T` מהארגומנטים.

## בחירת האוסף הנכון

| צורך | אוסף | גישה לפי אינדקס | חיפוש לפי ערך/מפתח | הוספה בסוף | הסרה |
|---|---|---|---|---|---|
| גודל קבוע, מהיר | `T[]` | O(1) | O(n) | — | — |
| רשימה כללית עם סדר | `List<T>` | O(1) | O(n) | O(1)* | O(n) |
| מפתח → ערך | `Dictionary<K,V>` | — | O(1) | O(1)* | O(1) |
| ייחודיות / שייכות | `HashSet<T>` | — | O(1) | O(1)* | O(1) |
| FIFO | `Queue<T>` | — | — | O(1)* | O(1) מהראש |
| LIFO | `Stack<T>` | — | — | O(1)* | O(1) מהסוף |
| מיון תמידי לפי מפתח | `SortedDictionary<K,V>` | — | O(log n) | O(log n) | O(log n) |

`*` — amortized: לפעמים יש הקצאה מחדש, אבל בממוצע קבוע.

השאלות שמכריעות: האם צריך סדר? האם צריך חיפוש לפי מפתח? האם יש כפילויות? כמה איברים? `List<T>` עם `Contains` בלולאה על 100,000 איברים = 10 מיליארד השוואות. `HashSet` = 100,000. זה ההבדל בין שנייה לדקות.

## Immutability ו-`IReadOnlyList<T>`

כשמחלקה מחזיקה `List<T>` פרטי וחושפת אותו כ-`public List<T> Items { get; }`, כל אחד יכול לקרוא `obj.Items.Clear()` — האנקפסולציה נשברה. הפתרון: לחשוף **ממשק לקריאה בלבד**.

<div dir="ltr">

```csharp
class Library
{
    private readonly List<Book> _books = [];
    public IReadOnlyList<Book> Books => _books;       // רואים, לא משנים
    public void Add(Book b) { /* ולידציה */ _books.Add(b); }
}
```

</div>

`List<T>` מממש `IReadOnlyList<T>`, אז אין העתקה — רק הגבלת ה-API. אפשרויות נוספות: `IReadOnlyCollection<T>`, `IReadOnlyDictionary<K,V>`, `_books.AsReadOnly()` (עוטף), ו-`ImmutableList<T>` מ-`System.Collections.Immutable` כשצריך אוסף שבאמת לא ניתן לשינוי (כל "שינוי" מחזיר אוסף חדש). מערכים ו-records גם הם חלק מהסיפור: `record` עם `IReadOnlyList<T>` הוא נתון בלתי-משתנה לחלוטין.

## ניהול נתונים יעיל

כמה עקרונות שחוסכים הרבה זמן ריצה — ובאגים:

1. **אינדקס בזיכרון**: אם מחפשים לפי מפתח לעיתים קרובות, החזיקו `Dictionary` ליד ה-`List` (כמו ב-`Repository<T>` למעלה). זיכרון זול; סריקות יקרות.
2. **Capacity**: `new List<T>(n)` / `new Dictionary<K,V>(n)` כשיודעים את הגודל. חוסך הכפלות והעתקות.
3. **אל תעתיקו סתם**: `ToList()` בכל שורה יוצר עותק. השתמשו בו רק כשצריך "צילום מצב" או כשעוברים על האוסף יותר מפעם אחת.
4. **`StringBuilder`** לבניית מחרוזת בלולאה — `+=` על string יוצר מחרוזת חדשה בכל פעם.
5. **`Span<T>`** — "חלון" על מערך (או חלק ממנו) בלי העתקה. שימושי בעיבוד טקסט/בינארי בביצועים גבוהים: `data.AsSpan(2, 4)`. לא תצטרכו אותו ביום-יום, אבל כדאי לדעת שהוא שם — ושהרבה מ-API של .NET מקבל אותו.
6. **מדדו לפני שמייעלים**: `Stopwatch` או BenchmarkDotNet. רוב הקוד לא צריך אופטימיזציה — צריך אוסף נכון.

## טעויות נפוצות

- **שינוי אוסף בתוך `foreach`** עליו — `InvalidOperationException: Collection was modified`. עברו על עותק (`.ToList()`) או השתמשו ב-`RemoveAll(predicate)`.
- **`List.Contains` / `IndexOf` בלולאה** על אוספים גדולים — O(n²). `HashSet`/`Dictionary`.
- **`dict[key]` בלי לבדוק** — `KeyNotFoundException`. `TryGetValue` / `GetValueOrDefault`.
- **חשיפת `List<T>` ציבורי** — כל אחד יכול לרוקן אותו. `IReadOnlyList<T>`.
- **מפתח שמשתנה** ב-`Dictionary`/`HashSet` — האיבר "נעלם". מפתחות immutable (string, int, record).
- **`IEnumerable` שנצרך פעמיים** — אם הוא תוצאת LINQ, השאילתה תרוץ פעמיים (ולפעמים תיתן תוצאות שונות). `ToList()` פעם אחת.
- **`Count()` של LINQ במקום `Count`** של List — הראשון עלול לסרוק את כל האוסף.

## לסיכום

- מערך לגודל קבוע; `List<T>` לרשימה כללית; `Dictionary` לחיפוש לפי מפתח; `HashSet` לייחודיות; `Queue`/`Stack` לסדר עיבוד.
- `IEnumerable<T>` הוא המכנה המשותף — קבלו אותו כפרמטר, ייצרו אותו עם `yield`.
- גנריקה (`Repository<T> where T : IEntity`) = קוד אחד לכל הטיפוסים, עם בטיחות טיפוסים.
- הטבלה של Big-O היא הכלי לבחירה. חיפוש חוזר = אינדקס (`Dictionary`).
- חשפו אוספים כ-`IReadOnlyList<T>`; קבעו capacity כשיודעים; אל תעתיקו סתם.

## קריאה נוספת

- [Collections (C#)](https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/collections)
- [Arrays](https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/arrays)
- [Generic classes and methods](https://learn.microsoft.com/dotnet/csharp/fundamentals/types/generics)
- [Constraints on type parameters](https://learn.microsoft.com/dotnet/csharp/programming-guide/generics/constraints-on-type-parameters)
- [Iterators (yield)](https://learn.microsoft.com/dotnet/csharp/iterators)
- [Selecting a collection class](https://learn.microsoft.com/dotnet/standard/collections/selecting-a-collection-class)
- [Memory<T> and Span<T> usage guidelines](https://learn.microsoft.com/dotnet/standard/memory-and-spans/memory-t-usage-guidelines)

</div>
