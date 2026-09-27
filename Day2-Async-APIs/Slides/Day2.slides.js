// Day2.slides.js — מצגת יום 2: Multithreading, Async ו-APIs
// בנייה: node tools/slides/build-slides.js Day2-Async-APIs/Slides/Day2.slides.js Day2-Async-APIs/Slides/Day2.pptx
module.exports = {
  day: 2,
  course: 'C# ב-.NET — קורס מעשי',
  title: 'יום 2 — Multithreading, Async ו-APIs',
  slides: [
    // ---------------- פתיחה ----------------
    {
      type: 'title',
      title: 'יום 2 — Multithreading, תכנות אסינכרוני ו-APIs',
      subtitle: 'Threads, Tasks ו-async/await · סנכרון · HttpClient · JSON · ביצועים ודיבוג',
      meta: '09:00–16:30 · 6 מודולים · 4 מעבדות',
      notes: 'ברוכים הבאים ליום השני. אתמול בנינו את היסודות — מחלקות, אוספים, LINQ וחריגות. היום נלמד איך לגרום לתוכנית לעשות כמה דברים בו-זמנית ולדבר עם העולם דרך HTTP. בקשו מכולם לוודא ש-.NET 10 מותקן ושהם הצליחו לשכפל את הריפו.',
    },
    {
      type: 'bullets', title: 'סדר היום', icon: 'clock',
      bullets: [
        { text: '09:15 — מודול 01: Threads, ThreadPool, race conditions', sub: ['10:00 — מודול 02: Tasks ו-async/await'] },
        { text: '11:00 — מעבדה 1: עיבוד מקבילי (50 דק\')' },
        { text: '11:50 — מודול 03: סנכרון, אוספים מקביליים, Channel<T>' },
        { text: '12:30 — צהריים · 13:15 — מעבדה 2: בנק בטוח (60 דק\')' },
        { text: '14:15 — מודול 04: REST ו-HttpClient · 14:45 — מודול 05: JSON' },
        { text: '15:15 — מעבדה 3: לקוח REST (60 דק\') · מעבדה 4 למהירים / בית' },
        { text: '16:15 — מודול 06: ביצועים ודיבוג async, סיכום' },
      ],
      notes: 'עברו על לוח הזמנים. מעבדה 4 היא מעבדת אינטגרציה של 75 דקות — היא מיועדת למי שמסיים מוקדם את מעבדה 3, לשיעורי בית או לפתיחת יום 3. הדגישו שהמעבדות הן הלב של היום.',
    },
    {
      type: 'cards', title: 'לפני שמתחילים: מריצים את Day2.LocalApi',
      cards: [
        { icon: 'terminal', heading: '1. טרמינל נפרד', text: 'cd Demos/Day2.LocalApi ואז dotnet run. השאירו פתוח כל היום.' },
        { icon: 'globe', heading: '2. בדיקה', text: 'curl http://localhost:5080/api/products — אמור להחזיר 6 מוצרים ב-JSON.' },
        { icon: 'cloud', heading: '3. בלי אינטרנט? בסדר', text: 'כל המעבדות עובדות מול ה-API המקומי. ה-APIs הציבוריים הם תוספת.' },
        { icon: 'book', heading: '4. תיעוד', text: 'Demos/Day2.LocalApi/README.md — כל נקודות הקצה, כולל /api/slow ו-/api/flaky.' },
      ],
      notes: 'תנו לכולם 5 דקות להריץ את ה-API המקומי עכשיו, כדי שלא נאבד זמן במעבדה 3. אם הפורט 5080 תפוס אצל מישהו — הפורט מוגדר בשורה אחת ב-Program.cs. הסבירו ש-/api/slow ו-/api/flaky קיימים בכוונה כדי לתרגל timeout ו-retry.',
    },

    // ---------------- מודול 01 ----------------
    { type: 'section', number: '01', title: 'מבוא ל-Multithreading', subtitle: 'Process, Thread, ThreadPool, race conditions, CPU מול IO', notes: 'המודול הראשון מניח את המושגים. המטרה: שכולם יבינו למה צריך מקביליות, מה ההבדל בין Thread ל-Process, ומה זה race condition — עוד לפני שנדבר על async.' },
    {
      type: 'two-col', title: 'Process מול Thread',
      right: { heading: 'Process (תהליך)', bullets: ['תוכנית שרצה: זיכרון משלה, קבצים, הרשאות', 'תהליכים לא רואים את הזיכרון זה של זה', 'יקר ליצור, בטוח לבודד'] },
      left: { heading: 'Thread (תהליכון)', bullets: ['נתיב ביצוע בתוך תהליך', 'Stack משלו — אבל Heap משותף!', 'קל לשתף נתונים — וקל להרוס אותם', 'גם Hello World מכיל ~10 תהליכונים (GC, JIT...)'] },
      notes: 'השיתוף של ה-Heap הוא הנקודה הכי חשובה בשקף: זה מה שהופך תהליכונים לשימושיים ומסוכנים בו-זמנית. הריצו את Day2.Demo.Threads דמו 1 כדי להראות כמה תהליכונים יש בתהליך פשוט.',
    },
    {
      type: 'code', title: 'המחלקה Thread',
      code: `var worker = new Thread(() =>
{
    for (int i = 1; i <= 3; i++)
    {
        Console.WriteLine($"[worker] step {i}");
        Thread.Sleep(100);
    }
})
{
    Name = "MyWorker",
    IsBackground = true
};

worker.Start();
Console.WriteLine("main continues...");
worker.Join();   // חוסם עד שה-worker מסיים`,
      bullets: ['Start מפעיל, Join מחכה', 'IsBackground: לא מונע מהתהליך להסתיים', 'כל Thread ≈ 1MB stack + זמן יצירה', 'ברוב הקוד המודרני לא ניצור Thread ידנית — נשתמש ב-Task'],
      notes: 'הראו את הדמו החי. הדגישו ש-Thread.Sleep משהה רק את התהליכון הנוכחי. שאלו: מה יקרה אם נשכח את Join? התוכנית עלולה להסתיים לפני שה-worker הדפיס משהו.',
    },
    {
      type: 'bullets', title: 'ThreadPool — לא ליצור, למחזר', icon: 'recycle',
      bullets: [
        'יצירת תהליכון יקרה → .NET מחזיק בריכה של תהליכונים מוכנים',
        'ThreadPool.QueueUserWorkItem — אבל בפועל Task.Run / Parallel / async משתמשים בו בשבילנו',
        { text: 'הבריכה גדלה לאט (~תהליכון לשנייה) כשכולם תפוסים', sub: ['חסימת תהליכוני Pool (.Result) = "הרעבה" — מודול 06'] },
        'פריטי עבודה צריכים להיות קצרים; עבודה ארוכה → LongRunning / Thread',
        'לכידת משתנה לולאה: תמיד int n = i; בתוך for',
      ],
      notes: 'הדמו מס\' 5 (cost of threads) מראה את ההבדל: 200 תהליכונים ידניים, 200 Task.Run, ו-200 Task.Delay. שימו לב לזמן של Task.Run — הבריכה גדלה לאט כשהעבודות חוסמות. זה הזרע לשיחה על הרעבה במודול 06.',
    },
    {
      type: 'code', title: 'Race Condition — הבאג שלא משתחזר',
      code: `int counter = 0;

var t1 = new Thread(() =>
{
    for (int i = 0; i < 1_000_000; i++) counter++;
});
var t2 = new Thread(() =>
{
    for (int i = 0; i < 1_000_000; i++) counter++;
});

t1.Start(); t2.Start();
t1.Join();  t2.Join();

Console.WriteLine(counter);
// expected 2,000,000
// actual   1,176,871  (changes every run!)`,
      bullets: ['counter++ = קריאה → הוספה → כתיבה', 'שני תהליכונים קוראים 41, שניהם כותבים 42', 'התוצאה תלויה בתזמון — לא דטרמיניסטית', 'לא מופיע בבדיקות, מופיע אצל הלקוח', 'פתרונות במודול 03: lock, Interlocked'],
      notes: 'זה השקף הכי חשוב במודול. הריצו את הדמו כמה פעמים והראו שהמספר שונה בכל פעם. ציירו על הלוח את שלושת השלבים של ++ ואיך שני תהליכונים "דורסים" זה את זה.',
    },
    {
      type: 'two-col', title: 'CPU-bound מול IO-bound — ההבחנה שקובעת הכל',
      right: { heading: 'CPU-bound', bullets: ['עיבוד תמונה, הצפנה, מיון, חישוב', 'המעבד עסוק — הצוואר הוא הליבות', 'הפתרון: מקביליות — Parallel.For, PLINQ, Task.Run', 'האצה מקסימלית = מספר הליבות'] },
      left: { heading: 'IO-bound', bullets: ['HTTP, מסד נתונים, קבצים, Task.Delay', 'מחכים למשהו חיצוני — המעבד פנוי', 'הפתרון: אסינכרוניות — async/await', 'מאות פעולות במקביל בלי תהליכונים בכלל'] },
      notes: 'טעות נפוצה: לפתוח תהליכון כדי לחכות לרשת. התהליכון יושב ומחכה ולא עושה כלום. עבור IO רוצים await שמשחרר את התהליכון. דמו 6 מראה 4 המתנות של 100ms: 400ms סדרתי מול 100ms עם Task.Delay + WhenAll.',
    },
    {
      type: 'code', title: 'Parallel.For / ForEach ו-PLINQ',
      code: `var squares = new int[data.Length];
Parallel.For(0, data.Length,
    i => squares[i] = data[i] * data[i]);

Parallel.ForEach(files,
    new ParallelOptions { MaxDegreeOfParallelism = 3 },
    file => Process(file));

// PLINQ
var primes = Enumerable.Range(1, 1_500_000)
    .AsParallel()
    .Where(IsPrime)
    .Count();`,
      bullets: ['הסדר לא מובטח', 'אסור לכתוב ל-List/Dictionary מתוך הגוף!', 'מערך לפי אינדקס או אוספים מקביליים', 'PLINQ משתלם רק כשהעבודה על כל פריט יקרה', 'תמיד למדוד — Stopwatch'],
      notes: 'דמו 8 מראה ש-PLINQ עם עבודה זולה איטי יותר מ-LINQ רגיל — ה-overhead של החלוקה גדול מהרווח. זו נקודה חשובה: מקביליות היא לא קסם. Parallel.ForEachAsync (.NET 6) מאפשר גוף אסינכרוני.',
    },

    // ---------------- מודול 02 ----------------
    { type: 'section', number: '02', title: 'Tasks ו-async/await', subtitle: 'Task<T>, מכונת המצבים, WhenAll, חריגות, ביטול, התקדמות', notes: 'המודול המרכזי של היום. כל השאר נבנה עליו. קחו את הזמן על מכונת המצבים — מי שמבין מה קורה ב-await לא יכתוב .Result.' },
    {
      type: 'bullets', title: 'Task = הבטחה, לא תהליכון', icon: 'box',
      bullets: [
        'Task — "עבודה שתסתיים מתישהו"; Task<T> — הבטחה לערך',
        { text: 'Task.Run — שולח עבודת CPU ל-ThreadPool', sub: ['עבור IO לא צריך Task.Run — הספריות מחזירות Task בעצמן'] },
        'await — "כשזה יסתיים, תמשיך מכאן" — בלי לחסום תהליכון',
        'Task.Delay במקום Thread.Sleep בקוד אסינכרוני',
        'Task.FromResult / Task.CompletedTask לתשובות מיידיות',
      ],
      notes: 'המשפט לזכור: Task הוא הבטחה, לא תהליכון. Task.Delay לא תופס שום תהליכון במשך ההמתנה — זה ההבדל המהותי מ-Thread.Sleep.',
    },
    {
      type: 'code', title: 'async/await — מה קורה באמת',
      code: `Console.WriteLine("[caller] before");
var task = SlowGreeting("Dana");
Console.WriteLine("[caller] got a Task, doing other work");
Console.WriteLine(await task);

static async Task<string> SlowGreeting(string name)
{
    Console.WriteLine("[part 1] synchronous");
    await Task.Delay(200);   // כאן חוזרים לקורא
    Console.WriteLine("[part 2] resumed later");
    return $"Hello, {name}!";
}

// [caller] before
// [part 1] synchronous
// [caller] got a Task, doing other work
// [part 2] resumed later
// Hello, Dana!`,
      bullets: ['המהדר יוצר מכונת מצבים', 'עד ה-await הראשון: סינכרוני', 'await על Task לא גמור → מחזירים Task לקורא', 'ההמשך רץ כשה-Task הפנימי מסתיים'],
      notes: 'הריצו את הדמו והראו את סדר ההדפסות. השאלה שכולם שואלים: "על איזה תהליכון רץ part 2?" — בקונסול: תהליכון Pool כלשהו; ב-WPF: תהליכון ה-UI. זה יהיה חשוב ביום 3.',
    },
    {
      type: 'steps', title: 'ארבעת השלבים של await',
      steps: [
        { heading: 'הקוד עד ה-await הראשון רץ סינכרונית', text: 'על התהליכון שקרא למתודה — כמו כל מתודה רגילה' },
        { heading: 'await על Task שלא הסתיים → המתודה מחזירה Task לקורא', text: 'התהליכון משוחרר לעבודות אחרות (UI, בקשות נוספות)' },
        { heading: 'כשה-Task הפנימי מסתיים, ה-runtime מזמן את ההמשך', text: 'בקונסול: תהליכון Pool. ב-UI: תהליכון ה-UI (SynchronizationContext)' },
        { heading: 'return מסמן את ה-Task שהוחזר כגמור', text: 'הקורא שעשה await עליו מתעורר' },
      ],
      notes: 'ציירו את זה על הלוב כ"ציר זמן" עם שני שחקנים: הקורא והמתודה. השלב השני הוא המפתח: המתודה "מחזירה" באמצע, וממשיכה מאוחר יותר.',
    },
    {
      type: 'code', title: 'WhenAll, WhenAny ו-timeout',
      code: `// סדרתי: ~600 ms
var a = await Fetch("a", 300);
var b = await Fetch("b", 200);
var c = await Fetch("c", 100);

// מקבילי: ~300 ms
string[] all = await Task.WhenAll(
    Fetch("a", 300), Fetch("b", 200), Fetch("c", 100));

// הראשון שמסיים
Task<string> winner = await Task.WhenAny(slow, fast);

// timeout בשורה אחת (.NET 6+)
var r = await Fetch("x", 5000)
    .WaitAsync(TimeSpan.FromSeconds(1));   // TimeoutException`,
      bullets: ['קודם מפעילים את כולם (בלי await), ואז WhenAll', 'await בלולאה = סדרתי!', 'WhenAll על Task<T> מחזיר T[]', 'WhenAny ל"המהיר ביותר" / fallback'],
      notes: 'הדפוס "await בלולאה" הוא הבאג הכי נפוץ אצל מתחילים — הקוד נראה אסינכרוני אבל רץ סדרתית. במעבדה 1 הם ימדדו את זה בעצמם.',
    },
    {
      type: 'bullets', title: 'חריגות בקוד אסינכרוני', icon: 'bug',
      bullets: [
        'חריגה נשמרת בתוך ה-Task ו"מתפוצצת" ב-await',
        { text: 'await זורק את החריגה המקורית', sub: ['.Wait() / .Result עוטפים ב-AggregateException'] },
        'WhenAll: await זורק את הראשונה; כולן ב-task.Exception.InnerExceptions',
        'Task בלי await ("fire and forget") — החריגה נבלעת בשקט',
        { text: 'async void — רק ל-event handlers!', sub: ['אי אפשר לחכות, אי אפשר לתפוס, מפיל את התהליך'] },
        'OperationCanceledException היא לא שגיאה — לתפוס בנפרד',
      ],
      notes: 'דמו 4 ב-Day2.Demo.AsyncAwait מראה את שלושת המקרים. הדגישו את async void: הסיבה היחידה לקיומו היא חתימות של event handlers. בכל מקום אחר — async Task.',
    },
    {
      type: 'code', title: 'ביטול שיתופי: CancellationToken',
      code: `using var cts = new CancellationTokenSource(
    TimeSpan.FromSeconds(2));   // או cts.Cancel() ידנית

try
{
    await LongJobAsync(cts.Token);
}
catch (OperationCanceledException)
{
    Console.WriteLine("cancelled");
}

static async Task LongJobAsync(CancellationToken ct)
{
    for (int i = 0; i < 100; i++)
    {
        ct.ThrowIfCancellationRequested();
        await Task.Delay(100, ct);
    }
}`,
      bullets: ['CancellationTokenSource = השלט', 'CancellationToken = מה שמעבירים', 'הביטול שיתופי: מי שעובד בודק', 'כמעט כל API אסינכרוני מקבל טוקן — תמיד להעביר הלאה', 'Console.CancelKeyPress → cts.Cancel() ל-Ctrl+C'],
      notes: 'ביטול הוא לא "שגיאה" — זו הדרך התקינה לצאת. הטעות הנפוצה: catch (Exception) שבולע גם את הביטול. במעבדה 1 הם יחברו Ctrl+C לטוקן.',
    },
    {
      type: 'two-col', title: 'IProgress<T> ו-IAsyncEnumerable<T>',
      right: { heading: 'דיווח התקדמות', code: `var progress = new Progress<int>(
    pct => bar.Value = pct);

await ProcessAsync(files, progress);

// בתוך המתודה:
progress.Report(i * 100 / total);

// Progress<T> זוכר את ה-context
// -> ב-WPF ה-callback על UI thread` },
      left: { heading: 'זרם אסינכרוני', code: `await foreach (var page in FetchPagesAsync(ct))
    Console.WriteLine(page);

static async IAsyncEnumerable<string> FetchPagesAsync(
    [EnumeratorCancellation] CancellationToken ct = default)
{
    for (int i = 1; i <= 10; i++)
    {
        await Task.Delay(100, ct);   // "הורדת עמוד"
        yield return $"page-{i}";
    }
}` },
      notes: 'שני כלים משלימים: IProgress לדיווח "כמה נשאר", IAsyncEnumerable לתוצאות שמגיעות בהדרגה. ValueTask נזכיר רק בקצרה — אופטימיזציה למקרים חמים אחרי מדידה.',
    },
    {
      type: 'lab', title: 'מעבדה 1 — עיבוד מקבילי',
      goal: 'להשוות עיבוד הזמנות סדרתי, Task.WhenAll ו-Parallel.ForEach; להבין מתי כל כלי מתאים.',
      duration: '50 דקות',
      deliverable: 'תוכנית שמריצה את שלוש הגרסאות, מודדת עם Stopwatch, נעצרת ב-Ctrl+C ומציגה התקדמות.',
      tasks: ['הריצו את הגרסה הסדרתית — קו הבסיס', 'ממשו RunWhenAllAsync (CPU ב-Task.Run, IO ב-await)', 'ממשו Parallel.ForEach + WhenAll לאישורים', 'חברו Ctrl+C ל-CancellationTokenSource', 'הוסיפו IProgress<int> עם Interlocked', 'טבלת סיכום: זמן והאצה לכל גרסה'],
      notes: 'הנקודה שהכי שווה לחכות לה: מי שלא עוטף את ProcessImage ב-Task.Run יגלה שגרסת WhenAll שלו כמעט לא מהירה יותר — כי החישוב רץ סדרתית לפני ה-await הראשון. תנו להם לגלות את זה לבד ואז הסבירו.',
    },

    // ---------------- מודול 03 ----------------
    { type: 'section', number: '03', title: 'סנכרון וניהול בטוח של משאבים', subtitle: 'lock, Interlocked, SemaphoreSlim, אוספים מקביליים, Channel<T>, deadlock', notes: 'עכשיו שראינו את הבעיה (race condition) — הפתרונות. סדר ההצגה: קודם "לא לשתף", אז "לא לשנות", ורק אז נעילות.' },
    {
      type: 'bullets', title: 'הכלל: שיתוף + כתיבה = צריך סנכרון', icon: 'shield',
      bullets: [
        'קריאה בלבד מכמה תהליכונים — בטוחה. כתיבה אחת לצד קריאות — כבר לא',
        { text: 'אסטרטגיה 1: לא לשתף', sub: ['כל תהליכון עובד על נתונים משלו, מאחדים בסוף (מערך לפי אינדקס, WhenAll)'] },
        { text: 'אסטרטגיה 2: לא לשנות (Immutability)', sub: ['record + with — אם אף אחד לא כותב, אין מה לסנכרן'] },
        { text: 'אסטרטגיה 3: הדרה הדדית', sub: ['lock, Interlocked, SemaphoreSlim, אוספים מקביליים'] },
      ],
      notes: 'הסדר הזה מכוון: נעילות הן המוצא האחרון, לא הראשון. הרבה בעיות סנכרון נפתרות בשינוי תכנון — למשל להחזיר תוצאות מ-Task במקום לכתוב לאוסף משותף.',
    },
    {
      type: 'code', title: 'lock — והטיפוס Lock החדש (C# 13)',
      code: `public class Account
{
    private readonly Lock _lock = new();  // .NET 9+
    private decimal _balance;

    public decimal Balance
    {
        get { lock (_lock) return _balance; }
    }

    public bool Withdraw(decimal amount)
    {
        lock (_lock)
        {
            if (_balance < amount) return false;
            _balance -= amount;   // בדיקה + עדכון = יחידה אחת
            return true;
        }
    }
}`,
      bullets: ['נועלים תמיד על אותו אובייקט פרטי', 'לא lock(this), לא lock("text")', 'האזור הקריטי קצר — בלי IO, בלי await', 'גם הקריאה בתוך lock!', 'עד C# 12: private readonly object _lock'],
      notes: 'הטעות הקלאסית: לנעול בכתיבה ולשכוח בקריאה. הסבירו ש-lock הוא סוכר תחבירי ל-Monitor.Enter/Exit ב-try/finally, ושהמהדר לא מרשה await בתוכו.',
    },
    {
      type: 'cards', title: 'ארגז הכלים של הסנכרון',
      cards: [
        { icon: 'bolt', heading: 'Interlocked', text: 'Increment, Add, Exchange, CompareExchange. אטומי ומהיר — אבל פעולה אחת על משתנה אחד.' },
        { icon: 'users', heading: 'SemaphoreSlim', text: 'N נכנסים בו-זמנית. WaitAsync — הכלי היחיד שמותר בקוד async. תמיד Release ב-finally.' },
        { icon: 'eye', heading: 'ReaderWriterLockSlim', text: 'הרבה קוראים יחד, כותב אחד לבד. ל-cache וקונפיגורציה.' },
        { icon: 'lock', heading: 'Mutex', text: 'ברמת מערכת ההפעלה — סנכרון בין תהליכים ("מופע יחיד"). איטי; בתוך תהליך — lock.' },
        { icon: 'timer', heading: 'Monitor.TryEnter', text: 'lock עם timeout: לא נתקעים לנצח, מדווחים או מנסים שוב.' },
        { icon: 'star', heading: 'Lazy<T>', text: 'אתחול עצל ובטוח פעם אחת בלבד — singleton בלי double-check locking.' },
      ],
      notes: 'עברו במהירות — הפרטים בחומר. הדגישו את SemaphoreSlim.WaitAsync: כשמישהו שואל "איך עושים lock בקוד async?" זו התשובה: SemaphoreSlim(1,1).',
    },
    {
      type: 'code', title: 'SemaphoreSlim — הגבלת מקביליות בקוד async',
      code: `private static readonly SemaphoreSlim _gate = new(3, 3);

async Task DownloadAsync(string url)
{
    await _gate.WaitAsync();   // לא חוסם תהליכון
    try
    {
        await http.GetStringAsync(url);
    }
    finally
    {
        _gate.Release();       // תמיד! אחרת הכרטיס אבד
    }
}

// 10 הורדות, לכל היותר 3 בו-זמנית:
await Task.WhenAll(urls.Select(DownloadAsync));`,
      bullets: ['"לכל היותר 3 בקשות במקביל"', 'מגן על שירותים עם rate limit', 'SemaphoreSlim(1,1) = async lock', 'Release ב-finally — חובה'],
      notes: 'הדמו ב-Day2.Demo.Synchronization מודד: 10 עבודות של 100ms עם 3 כרטיסים = ~400ms, ושיא המקביליות בדיוק 3. במעבדה 4 הם ישתמשו בזה להגבלת מקורות.',
    },
    {
      type: 'two-col', title: 'אוספים מקביליים — System.Collections.Concurrent',
      right: { heading: 'מה יש', bullets: ['ConcurrentDictionary — GetOrAdd, AddOrUpdate, TryRemove', 'ConcurrentQueue / ConcurrentStack / ConcurrentBag', 'BlockingCollection — producer/consumer חוסם', 'ImmutableList / FrozenDictionary לקריאה'] },
      left: { heading: 'מה לזכור', bullets: ['List / Dictionary רגילים אינם בטוחים לכתיבה מקבילית', 'ContainsKey ואז Add = לא אטומי → GetOrAdd', 'ה-factory של GetOrAdd עלול לרוץ יותר מפעם אחת', 'לוגיקה בשני שלבים? עדיין צריך lock'] },
      notes: 'הדמו מראה List<int> תחת Parallel.For: לפעמים 31,000 במקום 100,000, לפעמים חריגה. זה ממחיש טוב יותר מכל הסבר. ה-API המקומי שלנו משתמש ב-ConcurrentDictionary — אפשר להראות את הקוד.',
    },
    {
      type: 'code', title: 'Channel<T> — Producer/Consumer אסינכרוני',
      code: `var channel = Channel.CreateBounded<Order>(100);

// Producer
var producer = Task.Run(async () =>
{
    await foreach (var order in ReadOrdersAsync())
        await channel.Writer.WriteAsync(order); // מחכה אם מלא
    channel.Writer.Complete();                  // "אין עוד"
});

// Consumers
var consumers = Enumerable.Range(0, 3).Select(_ =>
    Task.Run(async () =>
    {
        await foreach (var o in channel.Reader.ReadAllAsync())
            await ProcessAsync(o);
    }));

await Task.WhenAll(consumers.Append(producer));`,
      bullets: ['תור שכותבים וקוראים אליו עם await', 'Bounded = back-pressure: היצרן מחכה כשמלא', 'ReadAllAsync מסתיים אחרי Complete', 'דפוס מצוין ל-logger — מעבדה 4'],
      notes: 'Day2.Demo.ProducerConsumer מראה 4 תרחישים כולל לוגר. ההבדל מ-BlockingCollection: Channel לא חוסם תהליכונים. שאלו: מה קורה בלי Complete()? הצרכנים מחכים לנצח.',
    },
    {
      type: 'code', title: 'Deadlock — ואיך נמנעים',
      code: `// T1                       // T2
lock (accountA)             lock (accountB)
{                           {
    lock (accountB) {...}       lock (accountA) {...}
}                           }
// T1 holds A, waits for B; T2 holds B, waits for A

// the fix: a fixed lock order (always by Id)
var (first, second) = from.Id < to.Id
    ? (from, to)
    : (to, from);

lock (first)
{
    lock (second)
    {
        Transfer(from, to, amount);
    }
}`,
      bullets: ['אין שגיאה, אין חריגה — התוכנית פשוט קופאת', 'סדר נעילה גלובלי קבוע', 'Monitor.TryEnter עם timeout', 'לא לקרוא לקוד זר בתוך lock', 'מעבדה 2: תיצרו deadlock אמיתי ותתקנו'],
      notes: 'הדמו מזהה deadlock עם TryEnter כדי לא לתקוע את התוכנית. במעבדה 2 הם יראו את התוכנית באמת נתקעת (עם watchdog). ההגנה מספר 1 היא סדר נעילה.',
    },
    {
      type: 'lab', title: 'מעבדה 2 — בנק בטוח לתהליכונים',
      goal: 'לשחזר race condition בהעברות כספים, לתקן בשלוש דרכים, ליצור deadlock אמיתי ולתקן עם סדר נעילה.',
      duration: '60 דקות',
      deliverable: 'חמישה מימושים של IBank שעוברים stress test של 160,000 העברות מקביליות; deadlock demo שמדגים ומתקן.',
      tasks: ['הריצו UnsafeBank — הכסף "נעלם"', 'LockBank: נעילה גלובלית + בדיקת יתרה אטומית', 'InterlockedBank: long באגורות + CompareExchange', 'ConcurrentBank: ConcurrentDictionary.AddOrUpdate', 'נעילה לכל חשבון → deadlock', 'סדר נעילה לפי Id → תיקון'],
      notes: 'ה-invariant "סכום הכסף קבוע" הוא המדד. תזכורת: Interlocked לא עובד על decimal — לכן אגורות. השאלה הטובה לדיון: למה ConcurrentBank שומר על הסכום בלי לנעול שני חשבונות יחד?',
    },

    // ---------------- מודול 04 ----------------
    { type: 'section', number: '04', title: 'צריכת REST APIs', subtitle: 'HTTP, REST, HttpClient, timeouts, retry, שגיאות, סודות', notes: 'עוברים מהעולם הפנימי לעולם החיצוני. רוב המשתתפים כבר נתקלו ב-HTTP — התמקדו בדרך הנכונה לעשות את זה ב-.NET.' },
    {
      type: 'two-col', title: 'HTTP ב-5 דקות',
      right: { heading: 'הבקשה', bullets: ['Method: GET (קרא), POST (צור), PUT (החלף), DELETE', 'URL + query string (?status=Shipped)', 'Headers: Content-Type, Authorization, Accept', 'Body: JSON (ב-POST/PUT)'] },
      left: { heading: 'התשובה — Status Code', bullets: ['2xx הצלחה: 200 OK, 201 Created, 204 No Content', '4xx אשמתכם: 400, 401/403, 404, 429', '5xx אשמת השרת: 500, 502, 503, 504', 'GET ו-DELETE idempotent — בטוח לנסות שוב'] },
      notes: 'הכלל שעוזר לזכור: 4xx = תקנו את הבקשה ואל תנסו שוב; 5xx / 429 = נסו שוב אחרי המתנה. 404 הוא לרוב תשובה לגיטימית ("אין כזה"), לא שגיאה.',
    },
    {
      type: 'cards', title: 'REST: משאבים + פעלים',
      cards: [
        { icon: 'list', heading: 'GET /api/products', text: 'כל המוצרים → 200 + מערך. עם ?search= לסינון.' },
        { icon: 'search', heading: 'GET /api/products/7', text: 'מוצר בודד → 200 + אובייקט, או 404.' },
        { icon: 'pen', heading: 'POST /api/products', text: 'יצירה עם body → 201 + Location header + האובייקט.' },
        { icon: 'arrows', heading: 'PUT /api/products/7', text: 'עדכון מלא → 200 (או 404). PATCH לעדכון חלקי.' },
        { icon: 'ban', heading: 'DELETE /api/products/7', text: '→ 204 No Content (או 404).' },
        { icon: 'globe', heading: 'Day2.LocalApi', text: 'בדיוק המבנה הזה, על localhost:5080. פלוס /api/slow ו-/api/flaky.' },
      ],
      notes: 'הראו curl חי מול ה-API המקומי: GET, POST עם -d, DELETE עם -i כדי לראות את הסטטוס. זה מכין אותם למעבדה 3.',
    },
    {
      type: 'code', title: 'HttpClient + System.Net.Http.Json',
      code: `private static readonly HttpClient Http = new()
{
    BaseAddress = new Uri("http://localhost:5080"),
    Timeout = TimeSpan.FromSeconds(10),
};

// GET: מבצע, בודק סטטוס, מפענח JSON
var products = await Http
    .GetFromJsonAsync<List<Product>>("/api/products") ?? [];

// POST
using var resp = await Http.PostAsJsonAsync("/api/products",
    new ProductInput("Webcam", 199m, "Video", 10));
resp.EnsureSuccessStatusCode();           // זורק אם לא 2xx
var created = await resp.Content.ReadFromJsonAsync<Product>();
Console.WriteLine(resp.Headers.Location); // /api/products/7

// PUT / DELETE
await Http.PutAsJsonAsync($"/api/products/{id}", input);
await Http.DeleteAsync($"/api/products/{id}");`,
      bullets: ['מופע HttpClient אחד לכל האפליקציה!', 'new HttpClient() בכל קריאה = דליפת sockets', 'ב-DI: IHttpClientFactory (יום 4)', 'HttpRequestMessage לשליטה מלאה'],
      notes: 'הכלל של מופע אחד הוא הדבר הכי חשוב בשקף. הסבירו למה: כל מופע מחזיק חיבורי TCP, ו-using בכל קריאה משאיר sockets ב-TIME_WAIT עד שהמערכת נחנקת.',
    },
    {
      type: 'code', title: 'Retry עם backoff — בלי ספרייה',
      code: `for (int attempt = 1; ; attempt++)
{
    try
    {
        var resp = await http.GetAsync(url, ct);
        if (resp.IsSuccessStatusCode
            || !IsTransient(resp.StatusCode)
            || attempt == maxAttempts)
            return resp;
    }
    catch (HttpRequestException) when (attempt < maxAttempts)
    { /* הרשת נפלה לרגע */ }

    var delay = TimeSpan.FromMilliseconds(
        200 * Math.Pow(2, attempt - 1));  // 200, 400, 800...
    await Task.Delay(delay, ct);
}

static bool IsTransient(HttpStatusCode c) => c is
    HttpStatusCode.ServiceUnavailable or
    HttpStatusCode.TooManyRequests or
    HttpStatusCode.GatewayTimeout;`,
      bullets: ['רק על שגיאות זמניות: 503, 429, 502, 504', 'לא על 400/401/404 — הבעיה בבקשה', 'מספר ניסיונות מוגבל', 'השהיה שגדלה (exponential backoff)', 'בייצור: ספריית Polly'],
      notes: 'ה-/api/flaky של ה-API המקומי נכשל ב-50% מהפעמים — מקום מושלם לתרגל. הזכירו ש-POST לא idempotent: retry עלול ליצור שני משאבים.',
    },
    {
      type: 'bullets', title: 'Timeouts ושגיאות', icon: 'warning',
      bullets: [
        { text: 'Http.Timeout — ברירת מחדל 100 שניות. הורידו ל-10–30', sub: ['לבקשה בודדת: CancellationTokenSource(TimeSpan) או .WaitAsync()'] },
        { text: 'HttpRequestException — השרת לא זמין / DNS / EnsureSuccessStatusCode', sub: ['יש לה StatusCode כשהגיעה תשובה'] },
        'TaskCanceledException (יורשת מ-OperationCanceledException) — timeout או ביטול',
        'JsonException — ה-body לא מה שציפינו',
        'הקוד חייב לעבוד גם כשהשירות לא זמין: הודעה ידידותית / ערך ברירת מחדל',
        '404 → null, לא חריגה',
      ],
      notes: 'ארבע חריגות לתפוס — ובסדר הזה. הדגישו: תפיסת OperationCanceledException בנפרד, לפני Exception כללי. ובלי timeout, בקשה תקועה = אפליקציה תקועה.',
    },
    {
      type: 'cards', title: 'סודות וכלי בדיקה',
      cards: [
        { icon: 'key', heading: 'לא בקוד, לא ב-Git', text: 'מפתח שנכנס ל-Git נשאר בהיסטוריה לנצח.' },
        { icon: 'lock', heading: 'dotnet user-secrets', text: 'לפיתוח: user-secrets init / set "Weather:ApiKey" "..." — נשמר מחוץ לפרויקט.' },
        { icon: 'gear', heading: 'משתני סביבה', text: 'לייצור: WEATHER__APIKEY. Environment.GetEnvironmentVariable או IConfiguration.' },
        { icon: 'terminal', heading: 'curl', text: 'curl -i -X POST url -H "Content-Type: application/json" -d "{...}"' },
        { icon: 'window', heading: 'Postman', text: 'GUI, אוספים, סביבות ומשתנים. נוח לשיתוף בצוות.' },
        { icon: 'file', heading: 'קבצי .http', text: 'VS Code REST Client / Visual Studio 2022: בקשות כטקסט בתוך הריפו.' },
      ],
      notes: 'הראו קובץ .http אם יש זמן — זה הכלי שהכי קל לאמץ. ולגבי סודות: אם מישהו כבר דחף מפתח ל-Git — לסובב אותו (rotate), לא רק למחוק.',
    },

    // ---------------- מודול 05 ----------------
    { type: 'section', number: '05', title: 'JSON עם System.Text.Json', subtitle: 'Serialize / Deserialize, options, attributes, JsonNode, Newtonsoft', notes: 'מודול קצר וטכני. רוב המשתתפים ראו JSON; המטרה היא הדרך הנכונה ב-.NET המודרני ושתי המלכודות: camelCase ו-enum כמספר.' },
    {
      type: 'code', title: 'Serialize / Deserialize + JsonSerializerOptions',
      code: `public record Order(int Id, string Customer,
    DateTime CreatedAt, OrderStatus Status,
    List<OrderItem> Items);

static readonly JsonSerializerOptions Options = new()
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    PropertyNameCaseInsensitive = true,
    WriteIndented = true,
    Converters = { new JsonStringEnumConverter() },
};

string json = JsonSerializer.Serialize(order, Options);
// {"id":42,"customer":"Dana","status":"Paid",...}

Order back = JsonSerializer.Deserialize<Order>(json, Options)!;`,
      bullets: ['מובנה ב-.NET, מהיר, בלי NuGet', 'records = DTOs מושלמים', 'Options אחד static — יצירה בכל קריאה יקרה', 'JsonSerializerOptions.Web = ברירת המחדל של ASP.NET', 'GetFromJsonAsync משתמש ב-Web כברירת מחדל'],
      notes: 'שתי המלכודות: בלי CamelCase השדות יוצאים PascalCase ורוב ה-APIs לא יאהבו; בלי JsonStringEnumConverter ה-enum יוצא כמספר. הראו את הפלט לפני ואחרי ב-Day2.Demo.HttpJson.',
    },
    {
      type: 'code', title: 'התאמות ברמת המאפיין',
      code: `public record Weather(
    [property: JsonPropertyName("temperature_2m")]
    double Temperature,
    [property: JsonPropertyName("wind_speed_10m")]
    double WindSpeed,
    DateTime Time);

public class User
{
    public int Id { get; set; }
    public required string Name { get; set; }

    [JsonIgnore]
    public string? PasswordHash { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int LoginCount { get; set; }
}`,
      bullets: ['[JsonPropertyName] כשהשם ב-JSON שונה (snake_case)', 'ב-record: [property: ...]', '[JsonIgnore] לשדות פנימיים', 'required — חריגה אם השדה חסר', 'DateTime ב-ISO 8601; decimal לכסף', 'שדה חסר → default; שדה עודף → מתעלמים'],
      notes: 'Open-Meteo (מעבדה 4) מחזיר temperature_2m — בדיוק המקרה ל-JsonPropertyName. הזכירו: מאפיין בלי set/init לא ידה-סורלז, ושדות (fields) דורשים IncludeFields.',
    },
    {
      type: 'code', title: 'JSON דינמי: JsonNode ו-JsonDocument',
      code: `// JsonNode — עץ שאפשר לקרוא ולשנות
JsonNode root = JsonNode.Parse(json)!;
string? name = (string?)root["user"]?["name"];
double temp = (double)root["current"]!["temperature_2m"]!;
root["count"] = 4;
root["user"]!["email"] = "noa@example.com";
string updated = root.ToJsonString();

// JsonDocument — קריאה בלבד, הכי מהיר, חובה using
using JsonDocument doc = JsonDocument.Parse(json);
foreach (JsonElement item in
         doc.RootElement.GetProperty("items").EnumerateArray())
{
    Console.WriteLine(item.GetProperty("productName").GetString());
}

// JSON שגוי -> JsonException (עם LineNumber)`,
      bullets: ['DTO כשהמבנה ידוע — 99% מהמקרים', 'JsonNode לעריכה / מבנה גמיש', 'JsonDocument לסריקה מהירה של JSON גדול', 'Source generators: ביצועים + Native AOT'],
      notes: 'רוב הזמן DTO. JsonNode כשצריך שני שדות מתשובה ענקית או כשהמבנה משתנה. Source generators — רק להזכיר: המהדר מייצר את קוד הסריאליזציה, בלי reflection.',
    },
    {
      type: 'two-col', title: 'System.Text.Json מול Newtonsoft.Json',
      right: { heading: 'System.Text.Json', bullets: ['מובנה ב-.NET (Core 3.0+)', 'מהיר וחסכוני יותר', 'Case-sensitive כברירת מחדל', '[JsonPropertyName], JsonStringEnumConverter', 'JsonNode / JsonDocument', 'Source generators, AOT', 'ברירת המחדל לקוד חדש'] },
      left: { heading: 'Newtonsoft.Json (Json.NET)', bullets: ['חבילת NuGet, ותיקה מאוד', 'סלחנית וגמישה: מחזורי הפניות, private setters, dynamic', 'Case-insensitive תמיד', '[JsonProperty], StringEnumConverter', 'JObject / JToken', 'תפגשו אותה בהמון קוד קיים', 'לקוד ישן או פיצ\'ר ספציפי'] },
      notes: 'המסר: קוד חדש — System.Text.Json. קוד קיים עם Newtonsoft — לא לשכתב סתם. יש מדריך מעבר רשמי ב-learn.microsoft.com (בקריאה נוספת של מודול 05).',
    },
    {
      type: 'lab', title: 'מעבדה 3 — לקוח REST מוקלד',
      goal: 'לבנות ShopApiClient מעל Day2.LocalApi: DTOs, CRUD מלא, טיפול בשגיאות, retry על /api/flaky ו-timeout על /api/slow.',
      duration: '60 דקות',
      deliverable: 'מחלקת לקוח שכל מתודותיה ממומשות; Program שרץ מתחילתו לסופו; הודעה ידידותית כשה-API לא רץ.',
      tasks: ['השלימו DTOs (Order, OrderStatus) + JsonSerializerOptions', 'GetProductAsync: 404 → null', 'Create (201) / Update / Delete (204)', 'ApiException עם StatusCode והודעת השרת', 'GetFlakyAsync: retry + backoff, לא על 4xx', 'GetSlowAsync: CancellationTokenSource(timeout) → null'],
      notes: 'ודאו שה-API המקומי רץ אצל כולם. מי שמסיים מוקדם — מתחיל את מעבדה 4. שאלת דיון טובה: למה 404 מחזיר null ו-400 זורק חריגה?',
    },

    // ---------------- מודול 06 ----------------
    { type: 'section', number: '06', title: 'ביצועים ודיבוג של קוד אסינכרוני', subtitle: 'sync-over-async, ConfigureAwait, מדידה, כלי הדיבוג ב-VS, ILogger', notes: 'המודול האחרון — קצר וממוקד. המטרה: שייצאו עם צ\'קליסט של הבאגים הנפוצים ועם ידיעה שיש כלים ב-Visual Studio לדבג את זה.' },
    {
      type: 'code', title: 'הבאג מספר 1: Sync-over-Async',
      code: `// WPF — הכפתור קופא לנצח
private void Button_Click(object sender, RoutedEventArgs e)
{
    var data = LoadAsync().Result;  // 1. UI thread חסום
    textBlock.Text = data;
}

private async Task<string> LoadAsync()
{
    await Task.Delay(1000);          // 2. ההמשך צריך את ה-UI thread
    return "done";                   // 3. ...שחסום ב-.Result. deadlock
}

// התיקון: async all the way
private async void Button_Click(object sender, RoutedEventArgs e)
{
    textBlock.Text = await LoadAsync();
}`,
      bullets: ['.Result / .Wait() / GetAwaiter().GetResult()', 'חוסמים תהליכון — ובשרת: הרעבת ThreadPool', 'ב-UI: deadlock', 'בקונסול / ASP.NET Core אין deadlock — אבל עדיין חסימה', 'async Task Main, async void רק ב-handler'],
      notes: 'ציירו את המעגל: ה-UI thread מחכה ל-Task, ה-Task מחכה ל-UI thread. זה הבאג שרואים הכי הרבה בקוד של מתחילים ב-WPF — יום 3 יראה את זה חי.',
    },
    {
      type: 'bullets', title: 'ConfigureAwait, Task.Run והרעבה', icon: 'gear',
      bullets: [
        { text: 'ConfigureAwait(false) — "לא אכפת לי לאיזה תהליכון לחזור"', sub: ['בספריות: על כל await. באפליקציה (WPF/קונסול/ASP.NET Core): לא צריך, ב-UI אף יזיק'] },
        { text: 'Task.Run — רק לעבודת CPU מה-UI', sub: ['לא סביב IO, לא בשרת, לא כ"תיקון" ל-deadlock, לא בספרייה'] },
        { text: 'הרעבת ThreadPool: latency עולה, CPU נמוך, תהליכונים מטפסים לאט', sub: ['הפתרון: להסיר חסימות. SetMinThreads הוא טיפול בסימפטום'] },
        'למדוד לפני שמייעלים',
      ],
      notes: 'ConfigureAwait(false) הוא הדבר שכולם "שמעו שצריך". התשובה המדויקת: בספריות כן, באפליקציה לא. Task.Run סביב HttpClient הוא הטעות השנייה בתפוצה אחרי .Result.',
    },
    {
      type: 'two-col', title: 'מדידה: Stopwatch, dotnet-counters, BenchmarkDotNet',
      right: { heading: 'Stopwatch', code: `var sw = Stopwatch.StartNew();
await ProcessAllAsync();
sw.Stop();
Console.WriteLine(
    $"{sw.ElapsedMilliseconds} ms");

// לא DateTime.Now!
// כמה ריצות, חימום, Release build

Console.WriteLine(
    ThreadPool.ThreadCount);
Console.WriteLine(
    ThreadPool.PendingWorkItemCount);` },
      left: { heading: 'כלים', bullets: ['dotnet-counters monitor -p <pid> — ThreadPool count, queue length, GC, חריגות בזמן אמת', 'BenchmarkDotNet — השוואות micro מדויקות עם סטטיסטיקה', 'תור שגדל + תהליכונים שמטפסים לאט = הרעבה', 'למדוד ב-Release, לא ב-Debug'] },
      notes: 'תרגיל 15 מדגים את ההרעבה: 20 עבודות שחוסמות ב-.Wait() לוקחות פי 2–3 יותר זמן מ-await, וה-ThreadPool גדל. אם יש זמן — הריצו dotnet-counters מול אחד הדמואים.',
    },
    {
      type: 'cards', title: 'דיבוג async ב-Visual Studio',
      cards: [
        { icon: 'sitemap', heading: 'Async Call Stacks', text: 'ה-Call Stack מציג את השרשרת הלוגית גם כשהתהליכון התחלף. כבו "Show external code".' },
        { icon: 'list', heading: 'Tasks window', text: 'Ctrl+Shift+D, K — כל ה-Tasks: Scheduled / Running / Awaiting / Blocked / Deadlocked.' },
        { icon: 'cubes', heading: 'Parallel Stacks', text: 'Ctrl+Shift+D, S — תרשים של כל התהליכונים וה-Tasks ומי מחכה למי.' },
        { icon: 'thread', heading: 'Threads window', text: 'Ctrl+Alt+H — להקפיא (Freeze) תהליכון ולשחזר race conditions.' },
        { icon: 'target', heading: 'Breakpoint מותנה', text: 'Thread.CurrentThread.ManagedThreadId == 7 — לעצור רק בתהליכון מסוים.' },
        { icon: 'laptop', heading: 'VS Code', text: 'async stacks ו-Threads כן; Tasks window אין.' },
      ],
      notes: 'אם אתם על Windows עם Visual Studio — פתחו את Day2.Demo.AsyncAwait, שימו breakpoint אחרי await והראו את Tasks window ואת ה-async call stack. זה שווה יותר מכל הסבר.',
    },
    {
      type: 'bullets', title: 'צ\'קליסט באגים נפוצים', icon: 'check',
      bullets: [
        'החלון קופא → .Result על UI thread → await עד למעלה',
        '"עובד בדיבאג, נכשל בריצה" → race condition → lock / Interlocked / Concurrent*',
        'חריגה נעלמת → Task בלי await / async void → await הכל',
        'הכל סדרתי למרות async → await בלולאה → לאסוף Tasks ואז WhenAll',
        'ObjectDisposedException אחרי await → using שנסגר מוקדם / HttpClient ב-using',
        'ביטול נרשם כשגיאה → catch (Exception) בולע OperationCanceledException',
        'UI לא מתעדכן אחרי await → ConfigureAwait(false) בקוד UI',
      ],
      notes: 'הטבלה המלאה בחומר של מודול 06. הציעו להדפיס ולתלות ליד המסך. כל שורה כאן היא באג שראינו היום בדמו או במעבדה.',
    },
    {
      type: 'code', title: 'לוגים: ILogger במקום Console.WriteLine',
      code: `using Microsoft.Extensions.Logging;

using var factory = LoggerFactory.Create(b => b
    .AddSimpleConsole(o => o.TimestampFormat = "HH:mm:ss.fff ")
    .SetMinimumLevel(LogLevel.Debug));

ILogger logger = factory.CreateLogger("Dashboard");

logger.LogInformation("fetching {Source} (attempt {Attempt})",
    "weather", 2);              // structured — לא $"..."

try { await FetchAsync(); }
catch (HttpRequestException ex)
{
    logger.LogWarning(ex, "fetch failed for {Source}", "weather");
}`,
      bullets: ['רמות: Trace…Critical', 'קטגוריה לכל מחלקה', 'תבנית {Name} שומרת את הפרמטרים כשדות', 'string interpolation מאבד את המבנה', 'במעבדה 4: logger על Channel<T> — אותו רעיון'],
      notes: 'Microsoft.Extensions.Logging.Console היא חבילת NuGet בקונסול; ב-ASP.NET Core הכל מובנה. ביום 4 נחבר את זה ל-DI. הדגישו את ההבדל בין {Source} ל-$"{source}".',
    },
    {
      type: 'lab', title: 'מעבדה 4 — לוח בקרה אסינכרוני',
      goal: 'לוח בקרה בקונסול שמושך 4 מקורות במקביל (API מקומי + Open-Meteo), שורד כשלים חלקיים, מתרענן כל N שניות עד Ctrl+C, ורושם לוגים דרך Channel<T>.',
      duration: '75 דקות (למהירים / בית)',
      deliverable: 'תוכנית שמציגה טבלה עם 4 מקורות, OK/FAILED ומשך לכל אחד; ממשיכה לעבוד כשמקור נופל; נעצרת נקי.',
      tasks: ['3 מקורות מול ה-API המקומי + WeatherSource עם timeout', 'FetchSafeAsync<T> — כישלון הופך ל-SourceResult', 'הפעלה במקביל + Task.WhenAll; זמן = המקור האיטי', 'עצרו את ה-API באמצע — הלוח ממשיך', 'PeriodicTimer + Ctrl+C + --once', 'ChannelLogger: TryWrite, pump ל-stderr, DisposeAsync'],
      notes: 'זו מעבדת האינטגרציה — היא נוגעת בכל ששת המודולים. בלי אינטרנט מזג האוויר יציג offline אחרי ה-timeout וזה בסדר: זו בדיוק ההתנהגות שרוצים. הפתרון כולל גם "stale values" כבונוס.',
    },
    {
      type: 'quote',
      text: 'Async is not about making things faster. It is about not wasting threads while you wait.',
      author: 'כלל אצבע לקוד אסינכרוני',
      notes: 'רגע לעצור: מקביליות (Parallel) מאיצה חישוב; אסינכרוניות (async) משחררת תהליכונים בזמן המתנה. שני כלים, שתי בעיות שונות. אם לוקחים משפט אחד מהיום — זה.',
    },
    {
      type: 'end', title: 'סיכום יום 2',
      bullets: [
        'Thread יקר, ThreadPool ממחזר, Task הוא הבטחה — await לא חוסם',
        'CPU-bound → Parallel / Task.Run; IO-bound → async/await + WhenAll',
        'שיתוף + כתיבה = סנכרון: lock, Interlocked, SemaphoreSlim, Concurrent*, Channel<T>',
        'Deadlock נמנע בסדר נעילה; sync-over-async נמנע ב-async all the way',
        'HttpClient אחד, timeout תמיד, retry רק על שגיאות זמניות, סודות מחוץ לקוד',
        'System.Text.Json: options סטטי, records כ-DTOs, camelCase + enum כמחרוזת',
        'מחר: WPF ו-Windows Forms — ושם async/await פוגש את ה-UI thread',
      ],
      footer: 'שיעורי בית: לסיים את מעבדה 4 · לעבור על Exercises/README.md · שאלות בערוץ הקורס',
      notes: 'סכמו בקצרה כל מודול במשפט. הזכירו שמעבדה 4 היא שיעורי הבית למי שלא הגיע אליה, ושמחר נראה את async/await בהקשר של ממשק משתמש — שם .Result באמת תוקע את החלון.',
    },
  ],
};
