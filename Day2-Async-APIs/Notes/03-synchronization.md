<div dir="rtl">

# מודול 03 — סנכרון תהליכונים וניהול בטוח של משאבים

## הבעיה: מצב משותף (Shared State)

במודול 01 ראינו ש-`counter++` משני תהליכונים מאבד עדכונים. הכלל הכללי: **כל פעם ששני תהליכונים ניגשים לאותו מקום בזיכרון, ולפחות אחד מהם כותב — צריך סנכרון.** קריאה בלבד משני תהליכונים היא בטוחה; כתיבה אחת בלבד לצד קריאות היא כבר לא (הקורא עלול לראות מצב "חצי מעודכן").

יש שלוש אסטרטגיות עקרוניות, ונעבור עליהן מהפשוטה לחכמה:

1. **מניעת שיתוף** — כל תהליכון עובד על נתונים משלו, ומאחדים בסוף (מערך לפי אינדקס, `WhenAll` שמחזיר תוצאות).
2. **אי-שינוי (Immutability)** — אם אף אחד לא כותב, אין מה לסנכרן.
3. **הדרה הדדית (Mutual Exclusion)** — רק תהליכון אחד בכל רגע בתוך "האזור הקריטי". זה `lock` וחבריו.

## `lock` — הכלי הבסיסי

<div dir="ltr">

```csharp
public class Account
{
    private readonly Lock _lock = new();     // C# 13 / .NET 9+: System.Threading.Lock
    private decimal _balance;

    public decimal Balance
    {
        get { lock (_lock) return _balance; }
    }

    public bool Withdraw(decimal amount)
    {
        lock (_lock)                          // רק תהליכון אחד בפנים
        {
            if (_balance < amount) return false;
            _balance -= amount;               // בדיקה + עדכון = פעולה אחת אטומית
            return true;
        }
    }
}
```

</div>

עד C# 12 נהגו לכתוב `private readonly object _lock = new();` — זה עדיין עובד ונפוץ מאוד. הטיפוס `Lock` החדש מהיר יותר ומונע טעויות (אי אפשר בטעות לנעול על `string` או על `this`). הכללים:

- **נועלים תמיד על אותו אובייקט** כשמגנים על אותם נתונים. שני `lock` על אובייקטים שונים לא מגנים זה מזה.
- **האובייקט פרטי.** `lock (this)` או `lock (typeof(X))` מאפשרים לקוד חיצוני לנעול את האובייקט שלכם ולגרום ל-deadlock.
- **האזור הקריטי קצר.** בלי IO, בלי `await` (המהדר בכלל לא מרשה `await` בתוך `lock`), בלי קריאה לקוד שאתם לא שולטים בו.
- `lock` הוא re-entrant: אותו תהליכון יכול להיכנס שוב לאותו lock (למשל מתודה נעולה שקוראת למתודה נעולה אחרת).

## `Monitor` — מה שמאחורי `lock`

`lock (x) { ... }` הוא סוכר תחבירי ל-`Monitor.Enter` / `Monitor.Exit` בתוך `try/finally`. גישה ישירה ל-`Monitor` נותנת עוד יכולות: המתנה עם timeout (`TryEnter`) והמתנה לאות (`Wait` / `Pulse`):

<div dir="ltr">

```csharp
if (Monitor.TryEnter(_gate, TimeSpan.FromMilliseconds(200)))
{
    try { /* עבודה */ }
    finally { Monitor.Exit(_gate); }
}
else
{
    // הנעילה תפוסה — לא נתקעים לנצח, מדווחים / מנסים שוב
}
```

</div>

## `Interlocked` — פעולות אטומיות בלי נעילה

עבור מונים ודגלים פשוטים, `lock` הוא תותח נגד זבוב. המעבד תומך בפעולות אטומיות ישירות, ו-`Interlocked` חושף אותן:

<div dir="ltr">

```csharp
private int _requests;
private long _bytes;

Interlocked.Increment(ref _requests);            // ++ אטומי
Interlocked.Add(ref _bytes, chunk.Length);       // += אטומי
int snapshot = Interlocked.Exchange(ref _requests, 0);   // קורא ומאפס בפעולה אחת

// CompareExchange: "אם הערך עדיין 0 — שנה ל-1". הבסיס לאלגוריתמים lock-free
if (Interlocked.CompareExchange(ref _state, 1, 0) == 0)
    Console.WriteLine("I was the first to initialize");
```

</div>

`Interlocked` מהיר פי כמה מ-`lock`, אבל מטפל בפעולה **אחת** על משתנה **אחד**. "בדוק את היתרה ואז הפחת" — זו כבר שתי פעולות, ובשביל זה צריך `lock`.

## `SemaphoreSlim` — הגבלת מקביליות (וגם async!)

סמפור מרשה ל-N תהליכונים להיכנס בו-זמנית — לא רק אחד. השימוש הנפוץ: "לכל היותר 3 בקשות HTTP במקביל", "לא יותר מ-10 חיבורים למסד". וחשוב מאוד: ל-`SemaphoreSlim` יש `WaitAsync`, ולכן הוא **הכלי היחיד כאן שמותר להשתמש בו בקוד async** (`lock` לא מרשה `await` בתוכו):

<div dir="ltr">

```csharp
private static readonly SemaphoreSlim _gate = new(3, 3);   // 3 "כרטיסים"

async Task DownloadAsync(string url)
{
    await _gate.WaitAsync();          // מחכה בלי לחסום תהליכון
    try
    {
        await http.GetStringAsync(url);
    }
    finally
    {
        _gate.Release();              // תמיד לשחרר — אחרת הכרטיס אבד לנצח
    }
}
```

</div>

`SemaphoreSlim(1, 1)` הוא בעצם "async lock" — דפוס נפוץ מאוד להגנה על משאב יחיד בקוד אסינכרוני.

## `Mutex` ו-`ReaderWriterLockSlim`

- **`Mutex`** דומה ל-`lock` אבל ברמת מערכת ההפעלה — ולכן יכול לסנכרן בין **תהליכים** שונים (למשל "רק מופע אחד של האפליקציה"). איטי בהרבה. בתוך תהליך אחד — תמיד `lock`.
- **`ReaderWriterLockSlim`** מתאים למבנה שקוראים ממנו הרבה וכותבים אליו מעט (cache, קונפיגורציה): הרבה קוראים נכנסים יחד, כותב נכנס לבד.

<div dir="ltr">

```csharp
private readonly ReaderWriterLockSlim _rw = new();

public string Get(string key)
{
    _rw.EnterReadLock();
    try { return _cache[key]; }
    finally { _rw.ExitReadLock(); }
}

public void Set(string key, string value)
{
    _rw.EnterWriteLock();
    try { _cache[key] = value; }
    finally { _rw.ExitWriteLock(); }
}
```

</div>

## אוספים מקביליים (`System.Collections.Concurrent`)

`List<T>` ו-`Dictionary<K,V>` **אינם בטוחים** לכתיבה מקבילית — תקבלו נתונים חסרים, חריגות מוזרות או לולאה אינסופית. במקום לעטוף כל גישה ב-`lock`, השתמשו באוספים שנבנו לזה:

| אוסף | תפקיד | פעולות מפתח |
|------|-------|--------------|
| `ConcurrentDictionary<K,V>` | מילון בטוח | `GetOrAdd`, `AddOrUpdate`, `TryRemove` — אטומיים |
| `ConcurrentQueue<T>` | תור FIFO | `Enqueue`, `TryDequeue` |
| `ConcurrentStack<T>` | מחסנית | `Push`, `TryPop` |
| `ConcurrentBag<T>` | "שק" ללא סדר | `Add`, `TryTake` |
| `BlockingCollection<T>` | producer/consumer חוסם | `Add`, `Take`, `GetConsumingEnumerable`, `CompleteAdding` |

<div dir="ltr">

```csharp
var wordCounts = new ConcurrentDictionary<string, int>();
Parallel.ForEach(words, w => wordCounts.AddOrUpdate(w, 1, (_, old) => old + 1));

var cache = new ConcurrentDictionary<int, User>();
var user = cache.GetOrAdd(id, key => LoadUser(key));   // הערך ייווצר פעם אחת בלבד*
```

</div>

\* הערה: ה-factory ב-`GetOrAdd` **עלול לרוץ יותר מפעם אחת** בתחרות; רק ההכנסה למילון אטומית. אם היצירה יקרה או עם תופעות לוואי — עטפו ב-`Lazy<T>`.

## `Channel<T>` — Producer/Consumer אסינכרוני

`BlockingCollection` חוסם תהליכונים. הגרסה המודרנית והאסינכרונית היא `System.Threading.Channels`: תור שאפשר לכתוב אליו ולקרוא ממנו עם `await`, כולל **back-pressure** (תור מוגבל שמעכב את היצרן כשהוא מלא):

<div dir="ltr">

```csharp
var channel = Channel.CreateBounded<Order>(capacity: 100);

// Producer
var producer = Task.Run(async () =>
{
    await foreach (var order in ReadOrdersAsync())
        await channel.Writer.WriteAsync(order);     // מחכה אם התור מלא
    channel.Writer.Complete();                       // "אין עוד"
});

// Consumers (אפשר כמה)
var consumers = Enumerable.Range(0, 3).Select(_ => Task.Run(async () =>
{
    await foreach (var order in channel.Reader.ReadAllAsync())  // מסתיים אחרי Complete
        await ProcessAsync(order);
}));

await Task.WhenAll(consumers.Append(producer));
```

</div>

דפוס מצוין ל-logger: כל התהליכונים כותבים ל-Channel, תהליכון אחד מדפיס/כותב לקובץ — בלי `lock` ובלי ערבוב שורות (ראו `Day2.Demo.ProducerConsumer` ומעבדה 4).

## Deadlock — ואיך נמנעים

Deadlock קורה כששני תהליכונים מחכים זה לזה לנצח. התסריט הקלאסי: T1 נועל A ומבקש B; T2 נועל B ומבקש A. אף אחד לא משחרר, התוכנית קופאת בלי שגיאה:

<div dir="ltr">

```csharp
// T1                          // T2
lock (accountA)                lock (accountB)
{                              {
    lock (accountB) { ... }        lock (accountA) { ... }   // deadlock!
}                              }
```

</div>

הגנות:

1. **סדר נעילה קבוע** — תמיד נועלים לפי כלל גלובלי (למשל לפי `Id` מהקטן לגדול). זה הפתרון של מעבדה 2.
2. **נעילה אחת** במקום שתיים, אם אפשר.
3. **`Monitor.TryEnter` עם timeout** — לא נתקעים לנצח, מדווחים.
4. **לא לקרוא לקוד זר בתוך `lock`** — לא callbacks, לא events, לא `await`.
5. **לא לחסום על Task בתוך UI/ASP.NET** (`.Result`) — סוג אחר של deadlock, מודול 06.

## Immutability כאסטרטגיה

הדרך הכי אלגנטית להימנע מבעיות סנכרון היא לא לשנות אובייקטים. `record` (יום 1) הוא אובייקט בלתי-משתנה כברירת מחדל; "שינוי" יוצר עותק חדש עם `with`. אובייקט שאף אחד לא כותב אליו אפשר לשתף בין אלף תהליכונים בלי שום נעילה:

<div dir="ltr">

```csharp
public record PriceSnapshot(string Currency, decimal Rate, DateTime At);

// כל תהליכון מקבל reference לאותו snapshot; "עדכון" = snapshot חדש
private volatile PriceSnapshot _current = new("ILS", 3.7m, DateTime.UtcNow);
public void Update(decimal rate) => _current = _current with { Rate = rate, At = DateTime.UtcNow };
```

</div>

יש גם `System.Collections.Immutable` (`ImmutableList<T>`, `ImmutableDictionary`) ו-`FrozenDictionary` (.NET 8+) לקריאה מהירה.

## Singleton בטוח עם `Lazy<T>`

"האתחל פעם אחת בלבד, גם אם עשרה תהליכונים מבקשים בו-זמנית" — אל תכתבו את זה בעצמכם עם double-check locking. `Lazy<T>` עושה את זה נכון כברירת מחדל:

<div dir="ltr">

```csharp
public sealed class AppConfig
{
    private static readonly Lazy<AppConfig> _instance = new(() => Load());
    public static AppConfig Instance => _instance.Value;   // thread-safe, נוצר פעם אחת
    private AppConfig() { }
    private static AppConfig Load() { /* קריאת קובץ */ return new AppConfig(); }
}
```

</div>

## טעויות נפוצות

- **נעילה על אובייקטים שונים** להגנה על אותם נתונים — או `lock` בכתיבה ושכחה ב**קריאה**.
- **`lock (this)` / `lock ("text")`** — נעילה על משהו שקוד אחר יכול לנעול גם.
- **`await` בתוך `lock`** — המהדר חוסם; הפתרון: `SemaphoreSlim(1,1)`.
- **לשכוח `Release`** לסמפור (או `ExitReadLock`) — תמיד ב-`finally`.
- **`Interlocked` לפעולה מורכבת** ("בדוק ואז עדכן") — צריך `lock`.
- **`ConcurrentDictionary` עם לוגיקה בשני שלבים** (`ContainsKey` ואז `Add`) — לא אטומי. השתמשו ב-`GetOrAdd`/`AddOrUpdate`/`TryAdd`.
- **לוגיקה ארוכה בתוך `lock`** — הופך את התוכנית לסדרתית ומזמין deadlock.

## לסיכום

- שיתוף + כתיבה = צריך סנכרון. עדיף לא לשתף, או לשתף אובייקטים בלתי-משתנים.
- `lock`/`Lock` להדרה הדדית; `Interlocked` למונים; `SemaphoreSlim` להגבלת מקביליות ול-async.
- אוספים מקביליים במקום `List`/`Dictionary` + `lock`.
- `Channel<T>` הוא ה-producer/consumer המודרני, אסינכרוני ועם back-pressure.
- Deadlock נמנע בסדר נעילה קבוע, נעילות קצרות, ו-`TryEnter` עם timeout.
- `Lazy<T>` ל-singleton, `record` לאי-שינוי.

## קריאה נוספת

- [Overview of synchronization primitives](https://learn.microsoft.com/en-us/dotnet/standard/threading/overview-of-synchronization-primitives)
- [The lock statement](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/statements/lock)
- [Interlocked class](https://learn.microsoft.com/en-us/dotnet/api/system.threading.interlocked)
- [Thread-safe collections](https://learn.microsoft.com/en-us/dotnet/standard/collections/thread-safe/)
- [System.Threading.Channels library](https://learn.microsoft.com/en-us/dotnet/core/extensions/channels)
- [Lazy initialization](https://learn.microsoft.com/en-us/dotnet/framework/performance/lazy-initialization)

</div>
