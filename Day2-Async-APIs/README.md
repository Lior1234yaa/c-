# יום 2 — Multithreading, תכנות אסינכרוני ו-APIs

היום השני של הקורס "C# ב-.NET — קורס מעשי". אתמול למדנו OOP, אוספים, LINQ וחריגות; היום נלמד איך לגרום לתוכנית לעשות כמה דברים בו-זמנית, לדבר עם שירותים חיצוניים דרך HTTP, ולעבוד עם JSON — בלי לאבד נתונים, בלי להיתקע, ועם כלים לדבג את זה כשמשהו משתבש.

## לפני שמתחילים: מריצים את Day2.LocalApi

רוב המעבדות היום מדברות עם REST API. כדי שהכיתה תעבוד גם בלי אינטרנט, יש API מקומי קטן בתיקיית `Demos/Day2.LocalApi`. **פתחו טרמינל נפרד והשאירו אותו רץ כל היום:**

```bash
cd Day2-Async-APIs/Demos/Day2.LocalApi
dotnet run
# Day2.LocalApi listening on http://localhost:5080  (Ctrl+C to stop)
```

בדיקה מהירה מטרמינל אחר (או בדפדפן): `curl http://localhost:5080/api/products`. רשימת נקודות הקצה ודוגמאות JSON — ב-[Demos/Day2.LocalApi/README.md](Demos/Day2.LocalApi/README.md).

## מה צריך שיהיה מותקן

- .NET 10 SDK (`dotnet --version`)
- Visual Studio 2022+ / VS Code עם C# Dev Kit / Rider
- `curl` (מגיע עם Windows 10+, macOS, Linux). אופציונלי: Postman או תוסף REST Client ל-VS Code.
- אינטרנט — רצוי אבל **לא חובה** (הדוגמאות מול APIs ציבוריים נכשלות בעדינות).

## מטרות למידה

בסוף היום תוכלו:

1. להסביר את ההבדל בין Process ל-Thread, בין CPU-bound ל-IO-bound, ובין מקביליות לאסינכרוניות.
2. לכתוב קוד `async`/`await` נכון: `Task.WhenAll`, ביטול עם `CancellationToken`, דיווח התקדמות, זרמים אסינכרוניים.
3. לזהות race conditions ולתקן אותם עם `lock`, `Interlocked`, `SemaphoreSlim` ואוספים מקביליים; לזהות deadlock ולמנוע אותו.
4. לצרוך REST API עם `HttpClient` — GET/POST/PUT/DELETE, headers, timeouts, retry וטיפול בשגיאות.
5. לסרלז ולפענח JSON עם `System.Text.Json`, כולל DTOs כ-records ו-JSON דינמי.
6. למדוד ביצועים, להימנע מ-sync-over-async, ולדבג קוד אסינכרוני ב-Visual Studio.

## סדר היום

| שעה | נושא | חומר |
|-----|------|------|
| 09:00–09:15 | פתיחה, חזרה קצרה על יום 1, הרצת Day2.LocalApi | README |
| 09:15–10:00 | מודול 01 — Threads, ThreadPool, race conditions, CPU מול IO, `Parallel` | [Notes/01](Notes/01-threads-intro.md), `Demos/Day2.Demo.Threads` |
| 10:00–10:45 | מודול 02 — Tasks, `async`/`await`, `WhenAll`, ביטול, התקדמות | [Notes/02](Notes/02-tasks-async-await.md), `Demos/Day2.Demo.AsyncAwait` |
| 10:45–11:00 | הפסקה | |
| 11:00–11:50 | **מעבדה 1** — עיבוד מקבילי | [Labs/Lab1](Labs/Lab1-ParallelProcessing/README.md) |
| 11:50–12:30 | מודול 03 — סנכרון: `lock`, `Interlocked`, `SemaphoreSlim`, אוספים מקביליים, `Channel<T>`, deadlock | [Notes/03](Notes/03-synchronization.md), `Demos/Day2.Demo.Synchronization`, `Demos/Day2.Demo.ProducerConsumer` |
| 12:30–13:15 | ארוחת צהריים | |
| 13:15–14:15 | **מעבדה 2** — בנק בטוח לתהליכונים | [Labs/Lab2](Labs/Lab2-ThreadSafeBank/README.md) |
| 14:15–14:45 | מודול 04 — HTTP, REST, `HttpClient`, retry/timeout, סודות | [Notes/04](Notes/04-rest-apis-http.md), `Demos/Day2.Demo.HttpJson` |
| 14:45–15:05 | מודול 05 — JSON עם `System.Text.Json` | [Notes/05](Notes/05-json.md) |
| 15:05–15:15 | הפסקה | |
| 15:15–16:15 | **מעבדה 3** — לקוח REST מוקלד (מי שמסיים: תחילת **מעבדה 4**) | [Labs/Lab3](Labs/Lab3-RestClient/README.md), [Labs/Lab4](Labs/Lab4-AsyncDashboard/README.md) |
| 16:15–16:30 | מודול 06 — ביצועים ודיבוג async, סיכום, שאלות | [Notes/06](Notes/06-performance-debugging-async.md) |

**מעבדה 4 (Async Dashboard)** היא מעבדת האינטגרציה של היום: היא מתאימה לתלמידים מהירים בזמן מעבדה 3, כשיעורי בית, או כפתיחה ליום 3.

## חומרי הלימוד (Notes)

| # | מודול | נושאים |
|---|-------|--------|
| 01 | [מבוא ל-Multithreading](Notes/01-threads-intro.md) | Process/Thread, `Thread`, ThreadPool, race condition, עלות תהליכונים, CPU/IO-bound, `Parallel.For/ForEach`, PLINQ |
| 02 | [Tasks ו-async/await](Notes/02-tasks-async-await.md) | `Task`/`Task<T>`, מכונת המצבים, `WhenAll/WhenAny`, חריגות, `async void`, `CancellationToken`, `IProgress<T>`, `IAsyncEnumerable<T>`, `ValueTask` |
| 03 | [סנכרון](Notes/03-synchronization.md) | `lock`/`Lock`, `Monitor`, `Interlocked`, `SemaphoreSlim`, `Mutex`, `ReaderWriterLockSlim`, אוספים מקביליים, `Channel<T>`, deadlock, immutability, `Lazy<T>` |
| 04 | [REST APIs ו-HTTP](Notes/04-rest-apis-http.md) | פעלים וסטטוסים, REST, `HttpClient` נכון, CRUD, headers/auth, timeout, retry, שגיאות, סודות, curl/Postman |
| 05 | [JSON](Notes/05-json.md) | `JsonSerializer`, `JsonSerializerOptions`, attributes, enums/dates, records כ-DTOs, `JsonNode`/`JsonDocument`, source generators, Newtonsoft |
| 06 | [ביצועים ודיבוג](Notes/06-performance-debugging-async.md) | sync-over-async, `ConfigureAwait`, מתי לא `Task.Run`, הרעבה, `Stopwatch`, `dotnet-counters`, כלי הדיבוג ב-VS, `ILogger` |

## דמואים (Demos)

| פרויקט | מה מדגים |
|--------|----------|
| `Day2.Demo.Threads` | Thread, ThreadPool, race condition, עלות תהליכונים, CPU/IO, Parallel, PLINQ (`dotnet run -- <1..8>`) |
| `Day2.Demo.AsyncAwait` | Task, מכונת מצבים, WhenAll/WhenAny, חריגות, ביטול, התקדמות, async streams, ValueTask |
| `Day2.Demo.Synchronization` | lock/Lock, Monitor, Interlocked, SemaphoreSlim, RW lock, אוספים מקביליים, deadlock, Lazy |
| `Day2.Demo.ProducerConsumer` | `Channel<T>`: bounded/unbounded, כמה צרכנים, logger מבוסס Channel |
| `Day2.Demo.HttpJson` | System.Text.Json + HttpClient מול LocalApi ו-jsonplaceholder (עם fallback) |
| `Day2.LocalApi` | ה-REST API המקומי (Minimal API) — [README](Demos/Day2.LocalApi/README.md) |

## תרגילים

[Exercises/README.md](Exercises/README.md) — 15 תרגילים קצרים לפי מודול. פתרונות: `Exercises/Solutions` → `dotnet run -- <n>`.

## מעבדות

| מעבדה | משך | נושא |
|-------|-----|------|
| [Lab 1 — Parallel Processing](Labs/Lab1-ParallelProcessing/README.md) | 50 דק' | סדרתי מול `WhenAll` מול `Parallel.ForEach`, Stopwatch, Ctrl+C, התקדמות |
| [Lab 2 — Thread-Safe Bank](Labs/Lab2-ThreadSafeBank/README.md) | 60 דק' | race condition → `lock` → `Interlocked` → `ConcurrentDictionary`; deadlock וסדר נעילה |
| [Lab 3 — REST Client](Labs/Lab3-RestClient/README.md) | 60 דק' | לקוח מוקלד מול Day2.LocalApi: DTOs, CRUD, שגיאות, retry, timeout |
| [Lab 4 — Async Dashboard](Labs/Lab4-AsyncDashboard/README.md) | 75 דק' | לוח בקרה שמושך כמה מקורות במקביל, כשלים חלקיים, רענון מחזורי, logger על `Channel<T>` |

לכל מעבדה: `README.md` (הנחיות), `Starter/` (מתקמפל, עם TODO) ו-`Solution/` (פתרון מלא + `NOTES.md`).

## מצגת

`Slides/Day2.pptx` (נבנית מ-`Slides/Day2.slides.js` — ראו `tools/slides/README.md`).
