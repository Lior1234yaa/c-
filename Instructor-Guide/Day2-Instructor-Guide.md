<div dir="rtl">

# מדריך למרצה — יום 2: Multithreading, תכנות אסינכרוני ו-APIs

**תיקיית היום:** [`Day2-Async-APIs`](../Day2-Async-APIs/README.md) | **שעות:** 09:00–16:30 | **מצגת:** `Day2-Async-APIs/Slides/Day2.pptx` (נבנית מ-[`Day2.slides.js`](../Day2-Async-APIs/Slides/Day2.slides.js))

## תקציר היום

אתמול התלמידים בנו מודל דומיין עם OOP, אוספים, LINQ וחריגות. היום הם לומדים לגרום לתוכנית לעשות כמה דברים בו-זמנית (Threads, `Parallel`, `Task`, `async`/`await`), להגן על מצב משותף (`lock`, `Interlocked`, `SemaphoreSlim`, אוספים מקביליים, `Channel<T>`), ולדבר עם העולם דרך HTTP ו-JSON (`HttpClient`, `System.Text.Json`). היום נסגר בביצועים ודיבוג של קוד אסינכרוני. שלוש מעבדות רצות בכיתה (Lab 1–3); מעבדה 4 (Async Dashboard) מיועדת למהירים ולשיעורי בית.

**המשפט שהתלמידים צריכים לקחת הביתה** (שקף 50): *מקביליות (`Parallel`) מאיצה חישוב; אסינכרוניות (`async`) משחררת תהליכונים בזמן המתנה.*

### מטרות למידה (מתוך README היום)

בסוף היום התלמידים יוכלו:

1. להסביר את ההבדל בין Process ל-Thread, בין CPU-bound ל-IO-bound, ובין מקביליות לאסינכרוניות.
2. לכתוב קוד `async`/`await` נכון: `Task.WhenAll`, ביטול עם `CancellationToken`, דיווח התקדמות, זרמים אסינכרוניים.
3. לזהות race conditions ולתקן אותם עם `lock`, `Interlocked`, `SemaphoreSlim` ואוספים מקביליים; לזהות deadlock ולמנוע אותו.
4. לצרוך REST API עם `HttpClient` — GET/POST/PUT/DELETE, headers, timeouts, retry וטיפול בשגיאות.
5. לסרלז ולפענח JSON עם `System.Text.Json`, כולל DTOs כ-records ו-JSON דינמי.
6. למדוד ביצועים, להימנע מ-sync-over-async, ולדבג קוד אסינכרוני ב-Visual Studio.

---

## הכנה לפני היום

### ערב לפני / בבוקר לפני 08:45

- [ ] **.NET 10 SDK** — `dotnet --version` מחזיר 10.x. כל הפרויקטים של היום הם `net10.0` (הטיפוס `System.Threading.Lock` דורש .NET 9+).
- [ ] אם יש ספק לגבי מחשבי התלמידים — הריצו את כלי האימות מיום 0: `dotnet run --project 00-Setup/VerifySetup`.
- [ ] `curl` זמין בטרמינל (מגיע עם Windows 10+). אופציונלי: Postman או תוסף VS Code REST Client (`humao.rest-client`).
- [ ] **בנו מראש את כל הפרויקטים של היום** (אין קובץ `.sln` — כל פרויקט נבנה בנפרד). משורש הריפו, ב-PowerShell:

<div dir="ltr">

```powershell
Get-ChildItem Day2-Async-APIs -Recurse -Filter *.csproj | ForEach-Object { dotnet build $_.FullName -v q }
```

</div>

  חשוב לבנות **לפני** שמפעילים את `Day2.LocalApi` — פרויקט רץ נועל את ה-DLL שלו.
- [ ] בדקו אינטרנט בכיתה: אם אין — זה בסדר. חלק ג' של `Day2.Demo.HttpJson` (jsonplaceholder), שלב 7 במעבדה 3 ו-`WeatherSource` במעבדה 4 נכשלים בעדינות. תכננו לומר את זה בקול.
- [ ] ודאו שהפורט 5080 פנוי אצלכם (`netstat -ano | findstr 5080`). הפורט קבוע בקוד: `builder.WebHost.UseUrls("http://localhost:5080")` ב-[`Demos/Day2.LocalApi/Program.cs`](../Day2-Async-APIs/Demos/Day2.LocalApi/Program.cs). אם משנים אותו, צריך לשנות גם בכל הלקוחות (דמו HttpJson, תרגילים 11–13, מעבדות 3–4).

### טרמינלים ל-IDE שצריך לפתוח מראש

| # | מה | פקודה (משורש הריפו) | מתי |
|---|----|---------------------|-----|
| T1 | **Day2.LocalApi — רץ כל היום** | `cd Day2-Async-APIs/Demos/Day2.LocalApi` ואז `dotnet run` | מ-08:50 עד סוף היום |
| T2 | דמואים | `cd Day2-Async-APIs/Demos` | כל היום |
| T3 | פתרונות תרגילים | `cd Day2-Async-APIs/Exercises/Solutions` | אחרי כל חלון תרגול |
| — | Visual Studio / VS Code | פתחו את `Day2.Demo.Threads`, `Day2.Demo.AsyncAwait`, `Day2.Demo.Synchronization` (לקוד על המסך) | — |

בדיקת חיים ל-T1: `curl http://localhost:5080/api/products` אמור להחזיר 6 מוצרים (Laptop, Mouse, Keyboard, Monitor 27", USB-C Hub, Headphones) ו-3 הזמנות (הזמנה 2 במצב `Shipped`). אם הנתונים "התלכלכו" במהלך היום: `curl -X POST http://localhost:5080/api/reset`.

### מה לחלק / לוודא אצל התלמידים

- [ ] לכל תלמיד יש עותק של הריפו ומצליח לבנות.
- [ ] תזכורת לנוהל המעבדות (מ-`00-Setup/COURSE-OVERVIEW.md`): **מעתיקים את `Starter`** לתיקיית עבודה (`my-work/Day2-Lab1` וכו') ועובדים על העותק; לא מציצים ב-`Solution` לפני ניסיון רציני.
- [ ] קישור ל-[`Exercises/README.md`](../Day2-Async-APIs/Exercises/README.md) — ממנו מחלקים את התרגילים הקצרים.
- [ ] ב-09:00 כל תלמיד מפעיל את `Day2.LocalApi` בטרמינל נפרד (שקף 3). מי שלא מצליח עכשיו — ייתקע ב-14:37 ובמעבדה 3.

---

## לו"ז יומי

מספרי השקפים לפי הסדר ב-`Day2.slides.js` (51 שקפים). הלו"ז שומר על כל השעות והמעבדות של ה-README הרשמי ועל ההפסקות; חלונות התרגול הקצרים שובצו בתוך זמן המודולים (על חשבון דקות הרצאה).

| שעה | סוג | נושא | חומרים |
|-----|-----|------|--------|
| 09:00–09:15 | הרצאה | פתיחה, חזרה קצרה על יום 1, סדר היום, הפעלת `Day2.LocalApi` אצל כולם | שקפים 1–3, [README היום](../Day2-Async-APIs/README.md), [LocalApi README](../Day2-Async-APIs/Demos/Day2.LocalApi/README.md) |
| 09:15–09:30 | הרצאה | מודול 01 (א): Process/Thread, `Thread`, ThreadPool, race condition | שקפים 4–8, [Notes/01](../Day2-Async-APIs/Notes/01-threads-intro.md) |
| 09:30–09:37 | דמו | `Day2.Demo.Threads` — דמואים 1, 2, 4 (ו-3 אם יש זמן) | [Demos/Day2.Demo.Threads](../Day2-Async-APIs/Demos/Day2.Demo.Threads/Program.cs) |
| 09:37–09:45 | הרצאה | מודול 01 (ב): CPU-bound מול IO-bound, `Parallel.For/ForEach`, PLINQ | שקפים 9–10 |
| 09:45–09:50 | דמו | `Day2.Demo.Threads` — דמואים 6, 7 (ו-8 אם יש זמן) | כנ"ל |
| 09:50–10:00 | תרגול | תרגיל 1 ★ + תרגיל 2 ★★ | [Exercises](../Day2-Async-APIs/Exercises/README.md) |
| 10:00–10:18 | הרצאה | מודול 02 (א): `Task`, מכונת המצבים של `await`, `Task.Delay`, `WhenAll`/`WhenAny`/`WaitAsync` | שקפים 11–15, [Notes/02](../Day2-Async-APIs/Notes/02-tasks-async-await.md) |
| 10:18–10:25 | דמו | `Day2.Demo.AsyncAwait` — סעיפים 1–4 | [Demos/Day2.Demo.AsyncAwait](../Day2-Async-APIs/Demos/Day2.Demo.AsyncAwait/Program.cs) |
| 10:25–10:33 | הרצאה | מודול 02 (ב): חריגות, `async void`, `CancellationToken`, `IProgress<T>`, `IAsyncEnumerable<T>`, `ValueTask` | שקפים 16–18 |
| 10:33–10:36 | דמו | `Day2.Demo.AsyncAwait` — סעיפים 5–8 (מאותה הרצה) | כנ"ל |
| 10:36–10:45 | תרגול | תרגיל 4 ★ + תרגיל 6 ★★ | [Exercises](../Day2-Async-APIs/Exercises/README.md) |
| 10:45–11:00 | הפסקה | | |
| 11:00–11:45 | מעבדה | **מעבדה 1** — עיבוד מקבילי (תדריך 3 דק' + עבודה) | שקף 19, [Lab1](../Day2-Async-APIs/Labs/Lab1-ParallelProcessing/README.md) |
| 11:45–11:50 | מעבדה | דיון מסכם למעבדה 1 (טבלת זמנים משותפת) | [Lab1 NOTES](../Day2-Async-APIs/Labs/Lab1-ParallelProcessing/Solution/NOTES.md) |
| 11:50–12:04 | הרצאה | מודול 03 (א): shared state, `lock`/`Lock`, `Monitor`, `Interlocked`, `SemaphoreSlim`, אוספים מקביליים | שקפים 20–25, [Notes/03](../Day2-Async-APIs/Notes/03-synchronization.md) |
| 12:04–12:10 | דמו | `Day2.Demo.Synchronization` — סעיפים 1, 3, 4, 6, 7 | [Demos/Day2.Demo.Synchronization](../Day2-Async-APIs/Demos/Day2.Demo.Synchronization/Program.cs) |
| 12:10–12:17 | הרצאה | מודול 03 (ב): `Channel<T>`, deadlock וסדר נעילה, immutability, `Lazy<T>` | שקפים 26–27 |
| 12:17–12:22 | דמו | `Day2.Demo.ProducerConsumer` — סעיפים 2 ו-4 | [Demos/Day2.Demo.ProducerConsumer](../Day2-Async-APIs/Demos/Day2.Demo.ProducerConsumer/Program.cs) |
| 12:22–12:30 | תרגול | תרגיל 8 ★★ | [Exercises](../Day2-Async-APIs/Exercises/README.md) |
| 12:30–13:15 | הפסקה | ארוחת צהריים | |
| 13:15–14:07 | מעבדה | **מעבדה 2** — בנק בטוח לתהליכונים (תדריך 3 דק' + עבודה) | שקף 28, [Lab2](../Day2-Async-APIs/Labs/Lab2-ThreadSafeBank/README.md) |
| 14:07–14:15 | מעבדה | דיון מסכם למעבדה 2 | [Lab2 NOTES](../Day2-Async-APIs/Labs/Lab2-ThreadSafeBank/Solution/NOTES.md) |
| 14:15–14:30 | הרצאה | מודול 04: HTTP, REST, `HttpClient` נכון, CRUD, headers, timeouts, retry, שגיאות, סודות | שקפים 29–35, [Notes/04](../Day2-Async-APIs/Notes/04-rest-apis-http.md) |
| 14:30–14:37 | דמו | `curl` מול LocalApi + `Day2.Demo.HttpJson` חלק B | [Demos/Day2.Demo.HttpJson](../Day2-Async-APIs/Demos/Day2.Demo.HttpJson/Program.cs) |
| 14:37–14:45 | תרגול | תרגיל 11 ★ | [Exercises](../Day2-Async-APIs/Exercises/README.md) |
| 14:45–14:55 | הרצאה | מודול 05: `JsonSerializer`, options, attributes, records כ-DTOs, `JsonNode`/`JsonDocument`, Newtonsoft | שקפים 36–40, [Notes/05](../Day2-Async-APIs/Notes/05-json.md) |
| 14:55–14:58 | דמו | `Day2.Demo.HttpJson` חלק A (JSON) + חלק C | כנ"ל |
| 14:58–15:05 | תרגול | תרגיל 14 ★★ | [Exercises](../Day2-Async-APIs/Exercises/README.md) |
| 15:05–15:15 | הפסקה | | |
| 15:15–16:08 | מעבדה | **מעבדה 3** — לקוח REST מוקלד (מהירים: מעבדה 4) | שקף 41, [Lab3](../Day2-Async-APIs/Labs/Lab3-RestClient/README.md), [Lab4](../Day2-Async-APIs/Labs/Lab4-AsyncDashboard/README.md) |
| 16:08–16:15 | מעבדה | דיון מסכם למעבדה 3 | [Lab3 NOTES](../Day2-Async-APIs/Labs/Lab3-RestClient/Solution/NOTES.md) |
| 16:15–16:25 | הרצאה | מודול 06: sync-over-async, `ConfigureAwait`, מתי לא `Task.Run`, הרעבה, מדידה, כלי דיבוג, `ILogger` (+ הרצת תרגיל 15 כדמו של דקה) | שקפים 42–48, [Notes/06](../Day2-Async-APIs/Notes/06-performance-debugging-async.md) |
| 16:25–16:30 | סיכום | מעבדה 4 כשיעורי בית, ציטוט, סיכום, הצצה ליום 3 | שקפים 49–51 |

**סיכום זמנים (450 דק'):** הרצאה 120 (כולל פתיחה) · דמו 43 · תרגול 42 · מעבדות 170 (כולל דיונים) · הפסקות 70 · סיכום 5.

---

## מתי מלמדים כל קובץ Notes

ה-Notes הם "הספר" של הקורס, ויש קובץ אחד לכל מודול. בכיתה **לא מקריאים אותם**. מלמדים מהשקפים ומהדמו, וה-Notes משמשים את המרצה כרשימת נושאים מלאה, ואת הסטודנטים לקריאה אחרי השיעור. ביום הזה מודולים 01–03 מפוצלים לשני חלקים (א)/(ב) עם דמו ביניהם, ולכן לכל אחד מהם יש שני חלונות זמן.

### מפת Notes ← שעה

| קובץ Notes | מתי מלמדים | זמן הרצאה | שקפים | דמו מיד אחרי | המעבדה שנשענת עליו |
|---|---|---|---|---|---|
| [01 — Multithreading ותכנות מקבילי](../Day2-Async-APIs/Notes/01-threads-intro.md) | **09:15–09:30** + **09:37–09:45** | 23 דק' (15+8) | 4–8, 9–10 | Threads 1, 2, 4 (09:30); Threads 6, 7 (09:45) | Lab 1 (11:00) |
| [02 — Tasks ו-async/await](../Day2-Async-APIs/Notes/02-tasks-async-await.md) | **10:00–10:18** + **10:25–10:33** | 26 דק' (18+8) | 11–15, 16–18 | AsyncAwait 1–4 (10:18); AsyncAwait 5–8 (10:33) | Lab 1 (11:00), Lab 4 (בית) |
| [03 — סנכרון](../Day2-Async-APIs/Notes/03-synchronization.md) | **11:50–12:04** + **12:10–12:17** | 21 דק' (14+7) | 20–25, 26–27 | Synchronization (12:04); ProducerConsumer 2, 4 (12:17) | Lab 2 (13:15), Lab 4 (בית) |
| [04 — REST APIs ו-HTTP](../Day2-Async-APIs/Notes/04-rest-apis-http.md) | **14:15–14:30** | 15 דק' | 29–35 | curl + HttpJson חלק B (14:30) | Lab 3 (15:15) |
| [05 — JSON](../Day2-Async-APIs/Notes/05-json.md) | **14:45–14:55** | 10 דק' | 36–40 | HttpJson חלקים A ו-C (14:55) | Lab 3 (15:15) |
| [06 — ביצועים ודיבוג async](../Day2-Async-APIs/Notes/06-performance-debugging-async.md) | **16:15–16:25** | 10 דק' | 42–48 | תרגיל 15 כדמו (בתוך הבלוק) | Lab 4 (בית), יום 3 (WPF) |

### פירוט לפי סעיפים בתוך כל קובץ

לכל קובץ: אילו סעיפים (לפי הכותרות בקובץ) מלמדים בכיתה, כמה דקות בערך לכל אחד, ומה נשאר לקריאה עצמית.

**Notes/01 — 09:15–09:30 (חלק א) + 09:37–09:45 (חלק ב)**

| שעה | סעיף בקובץ | דק' |
|---|---|---|
| 09:15–09:17 | למה בכלל צריך יותר מתהליכון אחד? | 2 |
| 09:17–09:20 | Process מול Thread | 3 |
| 09:20–09:24 | המחלקה `Thread` | 4 |
| 09:24–09:26 | ThreadPool — לא ליצור, למחזר | 2 |
| 09:26–09:30 | Race Condition — הבעיה המרכזית | 4 |
| *09:30–09:37* | *דמו Threads 1, 2, 4* | — |
| 09:37–09:40 | **CPU-bound מול IO-bound** (העיקר) | 3 |
| 09:40–09:43 | `Parallel.For` ו-`Parallel.ForEach` | 3 |
| 09:43–09:45 | PLINQ — LINQ מקבילי | 2 |

לקריאה עצמית: טעויות נפוצות. עלות תהליכונים מול Tasks מודגמת רק בדמו 5 (אם מקדימים).

**Notes/02 — 10:00–10:18 (חלק א) + 10:25–10:33 (חלק ב)**

| שעה | סעיף בקובץ | דק' |
|---|---|---|
| 10:00–10:05 | מ-Thread ל-Task | 5 |
| 10:05–10:12 | **`async` ו-`await` — איך זה עובד באמת** (ארבעת השלבים, העיקר) | 7 |
| 10:12–10:13 | `Task.Delay` במקום `Thread.Sleep` | 1 |
| 10:13–10:18 | הרכבה: `WhenAll` ו-`WhenAny` (כולל `WaitAsync`) | 5 |
| *10:18–10:25* | *דמו AsyncAwait סעיפים 1–4* | — |
| 10:25–10:27 | חריגות בקוד אסינכרוני | 2 |
| 10:27–10:28 | `async void` — רק ל-Event Handlers | 1 |
| 10:28–10:31 | ביטול: `CancellationToken` | 3 |
| 10:31–10:32 | דיווח התקדמות: `IProgress<T>` | 1 |
| 10:32–10:33 | זרמים אסינכרוניים: `IAsyncEnumerable<T>` + `ValueTask<T>` — בקצרה (משפט אחד) | 1 |

בדמו (10:33): `IAsyncEnumerable` עם ביטול ו-`ValueTask` עם cache. לקריאה עצמית: טעויות נפוצות.

**Notes/03 — 11:50–12:04 (חלק א) + 12:10–12:17 (חלק ב)**

| שעה | סעיף בקובץ | דק' |
|---|---|---|
| 11:50–11:52 | הבעיה: מצב משותף (Shared State) | 2 |
| 11:52–11:56 | **`lock` — הכלי הבסיסי** (כולל `Lock` ו-check-then-act, העיקר) | 4 |
| 11:56–11:57 | `Monitor` — מה שמאחורי `lock` | 1 |
| 11:57–11:59 | `Interlocked` — פעולות אטומיות בלי נעילה | 2 |
| 11:59–12:01 | `SemaphoreSlim` — הגבלת מקביליות (וגם async!) | 2 |
| 12:01–12:02 | `Mutex` ו-`ReaderWriterLockSlim` (אזכור בלבד) | 1 |
| 12:02–12:04 | אוספים מקביליים (`System.Collections.Concurrent`) | 2 |
| *12:04–12:10* | *דמו Synchronization* | — |
| 12:10–12:13 | `Channel<T>` — Producer/Consumer אסינכרוני | 3 |
| 12:13–12:15 | Deadlock — ואיך נמנעים | 2 |
| 12:15–12:16 | Immutability כאסטרטגיה | 1 |
| 12:16–12:17 | Singleton בטוח עם `Lazy<T>` | 1 |

לקריאה עצמית: הפירוט של `Mutex` ו-`ReaderWriterLockSlim` (בדמו: סעיף 5, אם שואלים), טעויות נפוצות. את ה-deadlock עצמו מתרגלים במעבדה 2.

**Notes/04 — 14:15–14:30**

| שעה | סעיף בקובץ | דק' |
|---|---|---|
| 14:15–14:17 | HTTP ב-5 דקות | 2 |
| 14:17–14:18 | REST בקצרה | 1 |
| 14:18–14:21 | **`HttpClient` — נכון** (מופע אחד, העיקר) | 3 |
| 14:21–14:23 | GET, POST, PUT, DELETE ב-C# | 2 |
| 14:23–14:24 | Headers והזדהות | 1 |
| 14:24–14:25 | Timeouts | 1 |
| 14:25–14:27 | ניסיונות חוזרים (Retry) עם backoff | 2 |
| 14:27–14:29 | טיפול בשגיאות — סיכום | 2 |
| 14:29–14:30 | API Keys וסודות — לא בקוד! | 1 |

בדמו (14:30): בדיקת API ידנית (`curl` מול LocalApi; קובץ `.http` — לקריאה). לקריאה עצמית: שירותים חיצוניים לתרגול, טעויות נפוצות.

**Notes/05 — 14:45–14:55 (קצב מהיר)**

| שעה | סעיף בקובץ | דק' |
|---|---|---|
| 14:45–14:46 | מה זה JSON ולמה הוא בכל מקום | 1 |
| 14:46–14:48 | DTO — המחלקה שמייצגת את ה-JSON (records) | 2 |
| 14:48–14:49 | `Serialize` ו-`Deserialize` | 1 |
| 14:49–14:51 | **`JsonSerializerOptions`** (camelCase ו-enum כמחרוזת, העיקר) | 2 |
| 14:51–14:52 | התאמות ברמת המאפיין (`[property: ...]`) | 1 |
| 14:52–14:53 | תאריכים, מספרים ו-null | 1 |
| 14:53–14:54 | JSON דינמי: `JsonNode` ו-`JsonDocument` | 1 |
| 14:54–14:55 | שגיאות + Source Generators + System.Text.Json מול Newtonsoft.Json (משפט אחד כל אחד) | 1 |

בדמו (14:55): עריכת `JsonNode`, `JsonDocument` ו-`JsonException` על קלט שגוי. לקריאה עצמית: Source Generators וטבלת Newtonsoft במלואן, טעויות נפוצות.

**Notes/06 — 16:15–16:25 (דחוס בכוונה; קריאה מלאה בבית לקראת יום 3)**

| שעה | סעיף בקובץ | דק' |
|---|---|---|
| 16:15–16:18 | **הבאג מספר 1: Sync-over-Async** (העיקר) | 3 |
| 16:18–16:19 | `ConfigureAwait(false)` — לספריות | 1 |
| 16:19–16:20 | מתי **לא** להשתמש ב-`Task.Run` | 1 |
| 16:20–16:21 | הימנעות מהרעבת ThreadPool (עם `dotnet run -- 15` כדמו) | 1 |
| 16:21–16:22 | מדידה: `Stopwatch` ו-`dotnet-counters` | 1 |
| 16:22–16:23 | דיבוג async ב-Visual Studio (רק רשימת הכלים) | 1 |
| 16:23–16:24 | לוגים עם `ILogger` | 1 |
| 16:24–16:25 | צ'קליסט באגים נפוצים (4–5 שורות) | 1 |

לקריאה עצמית: שאר הצ'קליסט, טעויות נפוצות, והקובץ כולו לקראת יום 3. את כלי הדיבוג ב-VS מתרגלים בבונוס של מעבדה 2 (Parallel Stacks).

### מה אומרים לסטודנטים על ה-Notes

- **בפתיחה (09:00):** "יש קובץ Notes לכל מודול. לא צריך לקרוא מראש. בכיתה אני מלמד מהשקפים, וה-Notes הם הספר שלכם לחזרה."
- **בכל מעבדה:** "נתקעתם? לפני שאתם מציצים ב-Solution, חפשו את הנושא ב-Notes של המודול." למשל: Lab 1 ← Notes/01 + 02, Lab 2 ← Notes/03, Lab 3 ← Notes/04 + 05, Lab 4 ← Notes/02 + 03 (`Channel`, `SemaphoreSlim`) + 06 (`ILogger`).
- **בסיכום (16:25):** קריאה לבית: הסעיפים "לקריאה עצמית" שלמעלה, ובמיוחד Notes/06 במלואו (sync-over-async ו-`ConfigureAwait`) ו-Notes/05 שעבר מהר. בכל קובץ יש סעיף "טעויות נפוצות" שכדאי לעבור עליו.

---

## פירוט לפי בלוק

### 09:00–09:15 | הרצאה — פתיחה (שקפים 1–3)

- חזרה של 3 דקות על יום 1 — שאלו שאלה אחת: "מה ההבדל בין `record` ל-`class`?" (נחזור ל-records היום פעמיים: DTOs ו-immutability).
- שקף 2 — סדר היום. הדגישו: שלוש מעבדות בכיתה, מעבדה 4 למהירים/בית.
- שקף 3 — **עצרו ותנו לכולם 5 דקות** להפעיל את `Day2.LocalApi` בטרמינל נפרד ולבדוק `curl http://localhost:5080/api/products`. עברו בין השולחנות. בעיות נפוצות: פורט 5080 תפוס (לשנות בשורה אחת ב-`Program.cs`), הרצה מהתיקייה הלא נכונה.
- הסבירו ש-`/api/slow` ו-`/api/flaky` קיימים בכוונה — לתרגול timeout ו-retry.

---

### 09:15–09:30 | הרצאה — מודול 01 (א): Threads (שקפים 4–8)

> 📖 **Notes:** [`Notes/01-threads-intro.md`](../Day2-Async-APIs/Notes/01-threads-intro.md) · חלוקת הזמן לפי סעיפים: ראו [מתי מלמדים כל קובץ Notes](#מתי-מלמדים-כל-קובץ-notes)

**נקודות מפתח, בסדר הוראה:**

1. למה בכלל: בזבוז זמן המתנה (IO) ובזבוז ליבות (CPU). **Concurrency** (לסירוגין) מול **Parallelism** (באמת בו-זמנית).
2. Process = מרחב זיכרון נפרד; Thread = נתיב ביצוע בתוך תהליך — **Stack משלו, Heap משותף**. זה הכוח וזו הסכנה.
3. גם Hello World מכיל כ-10 תהליכונים (GC, Finalizer, JIT).
4. המחלקה `Thread`: `Start`, `Join`, `IsBackground` (תהליכון foreground מונע סגירת התהליך), `Thread.Sleep` משהה רק את התהליכון הנוכחי. ~1MB stack לכל תהליכון.
5. ThreadPool — "לא ליצור, למחזר". מתחיל בכמספר הליבות וגדל **לאט** (~תהליכון לשנייה) — זרעו כאן את מודול 06 (הרעבה). בפועל לא קוראים לו ישירות: `Task.Run`, `Parallel`, `await` משתמשים בו.
6. **Race condition**: `counter++` = קריאה, הוספה, כתיבה. שני תהליכונים קוראים 41, שניהם כותבים 42 — עדכון נעלם. לא דטרמיניסטי, לא משתחזר בבדיקות.

**שאלות לכיתה:**
- "שני תהליכונים מגדילים מונה מיליון פעמים כל אחד. מה תהיה התוצאה?" (תנו להם לנחש לפני הדמו.)
- "אם אני יוצר 1,000 `Thread`-ים, כמה זיכרון הלך עוד לפני שעשו משהו?" (~1GB וירטואלי.)

**תפיסות שגויות נפוצות:**
- "`Thread.Sleep` עוצר את כל התוכנית."
- "אם זה עבד בהרצה אחת — אין באג." (race condition מופיע "לפעמים".)
- "`Task` = תהליכון" (נתקן במודול 02).

---

### 09:30–09:37 | דמו — `Day2.Demo.Threads` (דמואים 1, 2, 4)

<div dir="ltr">

```bash
cd Day2-Async-APIs/Demos/Day2.Demo.Threads
dotnet run -- 1     # Process and Thread
dotnet run -- 2     # Creating threads
dotnet run -- 4     # Race condition
dotnet run -- 3     # ThreadPool (אם יש זמן)
```

</div>

(בלי ארגומנט — מריץ את כל 8 הדמואים ברצף.)

- **דמו 1:** מספר התהליכונים בתהליך "ריק", מזהה ה-main thread, מספר ליבות לוגיות.
- **דמו 2:** הראו את הקוד — `Name`, `IsBackground = true`, `Join`. שימו לב ש-`[main] waiting for worker...` מודפס לפני/בין צעדי ה-worker. הצביעו על `ParameterizedThreadStart` (`t2.Start("hello")`).
- **דמו 4 (העיקר):** שלוש עמודות — `unsafe ++` (בדרך כלל פחות מ-2,000,000), `lock`, `Interlocked`. **הריצו פעמיים** — המספר השגוי משתנה בין ריצות. זה ה-"aha" של הבוקר. אל תסבירו עדיין לעומק את `lock`/`Interlocked` — "נגיע לזה במודול 03".
- **דמו 3 (אופציונלי):** min/max של ה-Pool; `int n = i;` — לכידת עותק של משתנה הלולאה.

---

### 09:37–09:45 | הרצאה — מודול 01 (ב): CPU מול IO, Parallel, PLINQ (שקפים 9–10)

> 📖 **Notes:** [`Notes/01-threads-intro.md`](../Day2-Async-APIs/Notes/01-threads-intro.md) · חלוקת הזמן לפי סעיפים: ראו [מתי מלמדים כל קובץ Notes](#מתי-מלמדים-כל-קובץ-notes)

**נקודות מפתח:**

1. **ההבחנה החשובה ביותר ביום:** CPU-bound (חישוב, הצפנה, עיבוד תמונה) → מקביליות, עד מספר הליבות. IO-bound (HTTP, DB, קבצים) → `async`/`await`, מאות פעולות בלי תהליכונים.
2. טעות נפוצה: `Task.Run`/תהליכון כדי "לחכות" לרשת — תהליכון שלם יושב בטל.
3. `Parallel.For`/`ForEach`: הסדר לא מובטח; **אסור לכתוב ל-`List<T>`/`Dictionary` מתוך הגוף** — מערך לפי אינדקס או אוסף מקבילי; `MaxDegreeOfParallelism`; חריגות → `AggregateException`; קיים `Parallel.ForEachAsync`.
4. PLINQ: `.AsParallel()`, `.AsOrdered()`. משתלם רק כשהעבודה לפריט **יקרה** — תמיד למדוד.

**שאלה לכיתה:** "יש לי 100 קריאות HTTP ו-4 ליבות. `Parallel.For` או `Task.WhenAll`? ומה אם יש לי 100 תמונות לדחוס?"

**תפיסה שגויה:** "יותר תהליכונים = יותר מהר" — לעבודת CPU, מעבר למספר הליבות מקבלים רק context switches.

---

### 09:45–09:50 | דמו — `Day2.Demo.Threads` (דמואים 6, 7)

<div dir="ltr">

```bash
dotnet run -- 6     # CPU-bound vs IO-bound
dotnet run -- 7     # Parallel.For / ForEach
dotnet run -- 8     # PLINQ (אם יש זמן)
```

</div>

- **דמו 6:** ארבע שורות — CPU sequential מול parallel (האצה ≈ מספר ליבות), IO sequential (~400ms, `Thread.Sleep`) מול concurrent (~100ms, `Task.Delay`). זה הגשר למודול 02.
- **דמו 7:** הריבועים ב-`Parallel.For` נכתבים למערך לפי אינדקס (בטוח); ב-`ForEach` עם `MaxDegreeOfParallelism = 3` — הקבצים מודפסים מתהליכונים שונים ובסדר לא צפוי.
- **דמו 8:** LINQ מול PLINQ על 1.5 מיליון מספרים; `AsOrdered` שומר סדר.
- **דמו 5 (עלות תהליכונים)** — שמרו ל"אם מקדימים"; הוא עלול לקחת כמה שניות בגלל גדילת ה-Pool.

---

### 09:50–10:00 | תרגול — תרגילים 1, 2

| # | כותרת | קושי | תשובה צפויה |
|---|-------|------|--------------|
| 1 | שני תהליכונים | ★ | שני `Thread` (מספרים 1–5, אותיות A–E) עם `Sleep(100)`, `Join` לשניהם, "done". הפלט **לא** זהה בין הרצות — ה-scheduler מחליט על השזירה. |
| 2 | Race condition וסידורו | ★★ | 4 תהליכונים × 250,000 → בלי סנכרון פחות מ-1,000,000. עם `lock` ועם `Interlocked.Increment` — בדיוק 1,000,000; `Interlocked` מהיר יותר. |

- מי שמסיים — תרגיל 3 ★★ (`Parallel.For` על ראשוניים עד 2,000,000).
- הצגת פתרון (טרמינל T3):

<div dir="ltr">

```bash
cd Day2-Async-APIs/Exercises/Solutions
dotnet run -- 1
dotnet run -- 2
dotnet run -- 3      # למי שהגיע; הפתרון משתמש ב-Parallel.For עם localInit/localFinally + Interlocked.Add
```

</div>

---

### 10:00–10:18 | הרצאה — מודול 02 (א): Tasks ו-await (שקפים 11–15)

> 📖 **Notes:** [`Notes/02-tasks-async-await.md`](../Day2-Async-APIs/Notes/02-tasks-async-await.md) · חלוקת הזמן לפי סעיפים: ראו [מתי מלמדים כל קובץ Notes](#מתי-מלמדים-כל-קובץ-notes)

"המודול המרכזי של היום" (הערת השקף 11). קחו זמן על מכונת המצבים.

**נקודות מפתח:**

1. `Task` = **הבטחה**, לא תהליכון. `Task<T>` = הבטחה לערך. `Task.Run` שולח עבודת CPU ל-Pool; ל-IO הספריות מחזירות `Task` בעצמן (`GetAsync`, `ReadAllTextAsync`).
2. חוקי תחביר: `async` מאפשר `await`; מחזירים `Task`/`Task<T>`/`ValueTask` (ו-`void` רק ל-event handlers).
3. **ארבעת השלבים של `await`** (שקף 14): (1) הקוד עד ה-`await` הראשון רץ **סינכרונית** על התהליכון הקורא; (2) ב-`await` על Task לא-גמור המתודה מחזירה Task לקורא ומשחררת את התהליכון; (3) כשה-Task הפנימי מסתיים, ה-continuation מתוזמן (בקונסול — תהליכון Pool כלשהו; ב-UI — תהליכון ה-UI); (4) `return` משלים את ה-Task שהוחזר.
4. `await` **לא חוסם**. `Task.Delay` במקום `Thread.Sleep`.
5. `WhenAll`: **קודם מפעילים את כל ה-Tasks, אחר כך `await`**. `await` על כל אחד בנפרד = סדרתי. `WhenAny` — הראשון שמסיים. `WaitAsync(TimeSpan)` — timeout בשורה אחת (`TimeoutException`).

**שאלות לכיתה:**
- "המתודה `SlowGreeting` מדפיסה part 1, עושה `await Task.Delay(200)`, ומדפיסה part 2. מה יודפס קודם — part 1 או 'got a Task immediately'?" (part 1 — הוא רץ סינכרונית.)
- "שלוש קריאות של 300, 200, 100 ms — כמה זמן סדרתי? כמה עם `WhenAll`?" (~600 / ~300.)

**תפיסות שגויות:**
- "`async` גורם למתודה לרוץ על תהליכון אחר." (לא — רק `Task.Run` עושה את זה.)
- "`await` מחכה כמו `.Wait()`." (הוא משחרר את התהליכון.)
- "`foreach (...) await X();` רץ במקביל."

---

### 10:18–10:25 | דמו — `Day2.Demo.AsyncAwait` (סעיפים 1–4)

<div dir="ltr">

```bash
cd Day2-Async-APIs/Demos/Day2.Demo.AsyncAwait
dotnet run
```

</div>

הדמו **אין לו בחירת סעיף** — הוא מריץ את כל 8 הסעיפים ברצף (כמה שניות). הריצו פעם אחת, והשאירו את הפלט על המסך; כרגע עברו על סעיפים 1–4 בפלט ובקוד:

1. **Task basics** — `Status` לפני ואחרי `await` (`WaitingToRun`/`Running` → `RanToCompletion`), `Task.FromResult`.
2. **async/await mechanics** — הדגישו את מספרי התהליכונים: part 1 על אותו thread כמו ה-caller, part 2 על תהליכון אחר. זה מכונת המצבים בפעולה.
3. **WhenAll / WhenAny** — sequential ~600ms, WhenAll ~300ms, WhenAny winner `fast-server` אחרי ~150ms, `WaitAsync` timeout אחרי 300ms.
4. **Exceptions** — `await` זורק `InvalidOperationException` המקורית; `.Wait()` עוטף ב-`AggregateException`; `WhenAll` עם שני כישלונות — `await` זורק את הראשון, כולם ב-`whenAll.Exception.InnerExceptions`.

---

### 10:25–10:33 | הרצאה — מודול 02 (ב): חריגות, ביטול, התקדמות, זרמים (שקפים 16–18)

> 📖 **Notes:** [`Notes/02-tasks-async-await.md`](../Day2-Async-APIs/Notes/02-tasks-async-await.md) · חלוקת הזמן לפי סעיפים: ראו [מתי מלמדים כל קובץ Notes](#מתי-מלמדים-כל-קובץ-notes)

**נקודות מפתח:**

1. חריגה במתודת `async` נשמרת ב-Task ו"מתפוצצת" ב-`await`. Task בלי `await` (fire-and-forget) — החריגה נבלעת. אזהרת CS4014.
2. `async void` — **רק ל-event handlers** (לא ניתן לחכות, לתפוס או לבדוק). ב-handler — `try/catch` בפנים.
3. `CancellationToken` — ביטול **שיתופי**: `CancellationTokenSource` הוא השלט, הטוקן עובר לפונקציות. `ThrowIfCancellationRequested`, `Task.Delay(ms, ct)`, `CancelAfter`/בנאי עם `TimeSpan`, `Register`. `OperationCanceledException` זו יציאה תקינה, לא "שגיאה". **תמיד להעביר את הטוקן הלאה.**
4. `IProgress<T>`/`Progress<T>` — "זוכר" את ה-SynchronizationContext, ולכן ב-WPF ה-callback רץ על תהליכון ה-UI (טיזר ליום 3).
5. `IAsyncEnumerable<T>` + `await foreach` + `[EnumeratorCancellation]`.
6. `ValueTask<T>` — משפט אחד: רק אחרי מדידה, לעולם לא `await` פעמיים.

**שאלה לכיתה:** "תפסתם `catch (Exception ex)` ורשמתם ללוג 'שגיאה'. המשתמש לחץ Cancel. מה יופיע בלוג?" (שגיאה מזויפת — תפסו `OperationCanceledException` בנפרד.)

**תפיסה שגויה:** "`cts.Cancel()` הורג את הפעולה." — לא, הקוד חייב לבדוק את הטוקן.

---

### 10:33–10:36 | דמו — `Day2.Demo.AsyncAwait` (סעיפים 5–8)

מאותה הרצה (אפשר להריץ שוב):

5. **Cancellation** — ביטול אוטומטי אחרי 350ms (step 1..4 ואז הודעת ביטול); ביטול ידני + `[callback] token cancelled` מ-`Register`.
6. **Progress** — 20%…100%.
7. **IAsyncEnumerable** — page-1..3 ואז עצירה אחרי `cts.Cancel()`.
8. **ValueTask** — הקריאה הראשונה `user#7`, הבאות `(cached, no Task allocated)`.

---

### 10:36–10:45 | תרגול — תרגילים 4, 6

| # | כותרת | קושי | תשובה צפויה |
|---|-------|------|--------------|
| 4 | Task.Delay ו-WhenAll | ★ | `FetchAsync(name, ms)` עם `await Task.Delay(ms)`. סדרתי ~600ms, `WhenAll` ~300ms. |
| 6 | ביטול והתקדמות | ★★ | `CountdownAsync(10, progress, ct)` — `ThrowIfCancellationRequested`, `Report(i)`, `Task.Delay(100, ct)`. עם `CancellationTokenSource(450ms)` מודפסים 10…6 ובערך "cancelled at 6" (תלוי תזמון, 5–6). |

- מהירים: תרגיל 5 ★★ (`WhenAny` + `WaitAsync(100ms)` → `TimeoutException`), תרגיל 7 ★★★ (`IAsyncEnumerable`; הזוגיים 2+4+6+8=20, ב-10 הסכום 30 > 20 → `break`).

<div dir="ltr">

```bash
dotnet run -- 4
dotnet run -- 6
```

</div>

---

### 10:45–11:00 | הפסקה

בזמן ההפסקה: ודאו ש-`Labs/Lab1-ParallelProcessing/Starter` נבנה אצלכם, ופתחו את ה-README על המסך.

---

### 11:00–11:50 | מעבדה 1 — עיבוד מקבילי

[README](../Day2-Async-APIs/Labs/Lab1-ParallelProcessing/README.md) · [Solution/NOTES.md](../Day2-Async-APIs/Labs/Lab1-ParallelProcessing/Solution/NOTES.md) · שקף 19

**מטרה:** לעבד 40 הזמנות (תמונה = CPU, אישור = IO) בשלוש דרכים — סדרתי, `Task.WhenAll`, `Parallel.ForEach` — למדוד, להוסיף Ctrl+C והתקדמות, ולהבין מתי כל כלי מתאים.

**מה יש ב-Starter:**
- `Models.cs`: `Order(Id, Customer, ImageSize)`, `OrderResult(OrderId, Checksum, Duration)`, `OrderProcessor.ProcessImage` (CPU), `OrderProcessor.SendConfirmationAsync(order, ct)` (`Task.Delay` 50–150ms), `GenerateOrders(count)`.
- `Program.cs`: `Runner.RunSequential` ממומש (עם `.GetAwaiter().GetResult()` — מסומן "רע, רק קו בסיס"); `RunWhenAllAsync` ו-`RunParallelForEachAsync` זורקים `NotImplementedException`. TODO לשלבים 2–6. מספר ההזמנות כארגומנט: `dotnet run -- 200`.

**שלבים בקצרה (לפי ה-README; 50 דק'):**
1. (5) להריץ, לרשום זמן סדרתי.
2. (10) `RunWhenAllAsync` — Task לכל הזמנה: תמונה ואז `await` אישור; `Task.WhenAll`.
3. (10) `Parallel.ForEach` עם `MaxDegreeOfParallelism = Environment.ProcessorCount`, תוצאות ל-`ConcurrentBag` / מערך לפי אינדקס, ואז `WhenAll` לאישורים.
4. (10) Ctrl+C: `Console.CancelKeyPress += (_, e) => { e.Cancel = true; cts.Cancel(); };` וטוקן לכל שכבה.
5. (10) `IProgress<int>` עם `\r`; `Interlocked.Increment` על המונה.
6. (5) טבלת סיכום — זמן והאצה; 40 מול 200 הזמנות.

> בלו"ז הזה שלב 6 נעשה בעיקר במשותף בדיון (11:45) — התלמידים מדווחים זמנים ואתם כותבים על הלוח.

**קריטריוני קבלה:**
- שלוש הגרסאות מחזירות אותם 40 checksums (סדר לא חשוב).
- `WhenAll` ו-`Parallel` מהירות לפחות x2 על 4 ליבות.
- Ctrl+C עוצר תוך פחות משנייה, בלי crash.
- התקדמות מגיעה ל-100%.
- אין כתיבה ל-`List<T>` מתוך `Parallel.ForEach`.

**איפה נתקעים — ואיזה רמז לתת:**

| תקיעה | רמז |
|-------|-----|
| "גרסת WhenAll שלי כמעט לא מהירה יותר" (**הצפוי ביותר — תנו להם לגלות לבד**, כך ממליצה הערת שקף 19) | "איפה רץ `ProcessImage`? מה קורה בקוד שלפני ה-`await` הראשון ב-lambda `async`?" → לעטוף ב-`Task.Run`. |
| "אי אפשר `await` בתוך `Parallel.ForEach`" | הלולאה סינכרונית — CPU בלולאה, IO אחריה עם `WhenAll`; או הבונוס `Parallel.ForEachAsync`. |
| חריגה / מספר תוצאות שגוי ב-`Parallel.ForEach` | כתבו ל-`List<T>` — `ConcurrentBag` או מערך לפי אינדקס. |
| Ctrl+C הורג את התהליך מיד | שכחו `e.Cancel = true`. |
| `OperationCanceledException` לא נתפסת | `Parallel.ForEach` זורק אותה — לתפוס **מחוץ** ללולאה, פעם אחת ב-`Program`. |
| אחוזי התקדמות קופצים/חוזרים | מונה רגיל מכמה תהליכונים — `Interlocked.Increment`. |
| חתימת המתודה | ה-README קורא לה `RunParallelForEach`, ב-Starter היא `RunParallelForEachAsync` ומחזירה `Task<List<OrderResult>>` — להשתמש בזו של ה-Starter. |

**דיון מסכם (11:45–11:50) — מתוך NOTES.md:**
- `Task.Run` סביב `ProcessImage` בגרסת `WhenAll` — בלעדיו כל החישובים רצים סדרתית בזמן יצירת ה-Tasks. **זו הנקודה הכי חשובה במעבדה.**
- פיצול CPU (`Parallel.ForEach`) ו-IO (`WhenAll`); `Parallel.ForEachAsync` מאחד.
- ביטול שיתופי דרך כל השכבות; תפיסת `OperationCanceledException` פעם אחת.
- תוצאות טיפוסיות (4 ליבות, 40 הזמנות): Sequential ~7000ms, WhenAll ~1200ms (x6), Parallel.ForEach+WhenAll ~1200ms, ForEachAsync ~1300ms. הסבר: סדרתי = CPU (~100ms) + IO (~75ms) לכל הזמנה; מקבילי מוגבל ע"י ליבות (40×100/4 ≈ 1000ms) וה-IO חופף.
- להראות את הפתרון: `cd Day2-Async-APIs/Labs/Lab1-ParallelProcessing/Solution` ואז `dotnet run -- 40` או `dotnet run -- 200 whenall` (מצבים: `seq`, `whenall`, `parallel`, `foreachasync`, `all`).

**בונוס למהירים:** `SemaphoreSlim` שמגביל את האישורים ל-5 בו-זמנית (הקדמה למודול 03); פרמטרי שורת פקודה; `Parallel.ForEachAsync`.

---

### 11:50–12:04 | הרצאה — מודול 03 (א): סנכרון (שקפים 20–25)

> 📖 **Notes:** [`Notes/03-synchronization.md`](../Day2-Async-APIs/Notes/03-synchronization.md) · חלוקת הזמן לפי סעיפים: ראו [מתי מלמדים כל קובץ Notes](#מתי-מלמדים-כל-קובץ-notes)

**נקודות מפתח:**

1. **הכלל:** שני תהליכונים + אותו מקום בזיכרון + לפחות כתיבה אחת = צריך סנכרון. קריאה בלבד — בטוחה.
2. שלוש אסטרטגיות, מהפשוטה לחכמה: **לא לשתף** (מערך לפי אינדקס, `WhenAll` שמחזיר תוצאות) → **לא לשנות** (immutability) → **הדרה הדדית** (`lock`).
3. `lock` והטיפוס `Lock` החדש (C# 13 / .NET 9+). כללים: תמיד אותו אובייקט; אובייקט **פרטי** (לא `this`, לא `string`, לא `typeof`); אזור קריטי קצר — **בלי IO ובלי `await`** (המהדר חוסם); re-entrant. בדיקה + עדכון באותו `lock` ("check-then-act").
4. `Monitor` = מה שמאחורי `lock`; `TryEnter` עם timeout.
5. `Interlocked` — `Increment`, `Add`, `Exchange`, `CompareExchange`. מהיר, אבל פעולה **אחת** על משתנה **אחד**.
6. `SemaphoreSlim` — N בו-זמנית, עם `WaitAsync` — **הכלי שמתאים לקוד async**. `Release` תמיד ב-`finally`. `SemaphoreSlim(1,1)` = async lock.
7. `Mutex` (בין תהליכים, איטי) ו-`ReaderWriterLockSlim` (הרבה קוראים, מעט כותבים) — אזכור בלבד.
8. אוספים מקביליים: `ConcurrentDictionary` (`GetOrAdd`, `AddOrUpdate`, `TryRemove`), `ConcurrentQueue`, `ConcurrentBag`, `BlockingCollection`. הערה: ה-factory של `GetOrAdd` עלול לרוץ יותר מפעם אחת.

**שאלות לכיתה:**
- "`Interlocked` מהיר מ-`lock`. למה לא להשתמש בו תמיד?" (לא יודע לעשות "בדוק יתרה ואז הפחת".)
- "`if (!dict.ContainsKey(k)) dict.TryAdd(k, v)` על `ConcurrentDictionary` — בטוח?" (שתי פעולות, לא אטומי כמכלול.)

**תפיסות שגויות:**
- "`ConcurrentDictionary` הופך כל קוד שמשתמש בו ל-thread-safe."
- "נעלתי בכתיבה, קריאה לא צריכה נעילה." (קורא עלול לראות מצב חצי מעודכן.)
- "`lock` עם `await` בפנים יעבוד אם אזהר." (לא מתקמפל.)

---

### 12:04–12:10 | דמו — `Day2.Demo.Synchronization`

<div dir="ltr">

```bash
cd Day2-Async-APIs/Demos/Day2.Demo.Synchronization
dotnet run
```

</div>

גם כאן אין בחירת סעיף — כל 8 הסעיפים רצים ברצף (כשנייה-שתיים). עברו על הפלט והקוד:

- **1. lock & Lock** — מחלקת `Account` עם `Lock`: 10 תהליכונים × 10 משיכות של 10 → `balance = 0`. הראו את `Thread.SpinWait(50)` שמרחיב את חלון ה-race — "בלי ה-`lock` זה היה נשבר".
- **3. Interlocked** — `counter=100,000`; `CompareExchange`: הניסיון השני רואה 1 ולא מצליח.
- **4. SemaphoreSlim** — 10 עבודות, מקסימום 3: `peak=3`, ~400ms.
- **6. Concurrent collections** — `'the' appears 150 times`; `BlockingCollection` producer/consumer; **`List<int>` תחת `Parallel.For`** — count שגוי או חריגה. רגע חזק.
- **7. Deadlock** — סדר נעילה הפוך → `deadlock detected: True` (בעזרת `TryEnter` 500ms כדי לא לתקוע); סדר קבוע → מסיים. קשרו למעבדה 2.
- 2, 5, 8 (Monitor.TryEnter, RW lock, Lazy/immutable) — אם שואלים.

---

### 12:10–12:17 | הרצאה — מודול 03 (ב): Channel, deadlock, immutability (שקפים 26–27)

> 📖 **Notes:** [`Notes/03-synchronization.md`](../Day2-Async-APIs/Notes/03-synchronization.md) · חלוקת הזמן לפי סעיפים: ראו [מתי מלמדים כל קובץ Notes](#מתי-מלמדים-כל-קובץ-notes)

**נקודות מפתח:**

1. `Channel<T>` — producer/consumer **אסינכרוני**: `CreateBounded`/`CreateUnbounded`, `Writer.WriteAsync`, `Writer.Complete()`, `Reader.ReadAllAsync()`. **Back-pressure** בתור מוגבל: היצרן ממתין (אסינכרונית) כשהתור מלא. `BlockingCollection` חוסם תהליכונים; `Channel` לא.
2. דפוס ה-logger: כולם כותבים ל-Channel, צרכן אחד מדפיס — בלי `lock` ובלי ערבוב שורות (מעבדה 4).
3. Deadlock: T1 נועל A ומחכה ל-B, T2 נועל B ומחכה ל-A. הגנות: **סדר נעילה קבוע** (הפתרון של מעבדה 2), נעילה אחת, `TryEnter` עם timeout, לא לקרוא לקוד זר בתוך `lock`, לא `.Result` ב-UI.
4. Immutability — `record` + `with`; `ImmutableList`, `FrozenDictionary`. `Lazy<T>` ל-singleton במקום double-check locking.

**שאלה לכיתה:** "בהעברה בין חשבונות, איך מבטיחים ששני תהליכונים שמעבירים 1→2 ו-2→1 לא ייתקעו?" (לנעול לפי `Id` עולה — תנו להם לחשוב, התשובה המלאה במעבדה.)

**תפיסה שגויה:** "deadlock זורק חריגה." — לא, התוכנית פשוט קופאת.

---

### 12:17–12:22 | דמו — `Day2.Demo.ProducerConsumer` (סעיפים 2, 4)

<div dir="ltr">

```bash
cd Day2-Async-APIs/Demos/Day2.Demo.ProducerConsumer
dotnet run
```

</div>

(רץ את כל 4 הסעיפים.) התמקדו ב:
- **2. Bounded (קיבולת 2)** — חותמות הזמן: אחרי 2 פריטים היצרן מתקדם רק בקצב הצרכן (100ms). "זה back-pressure — הזיכרון לא מתפוצץ".
- **4. Channel-based logger** — 4 workers כותבים, שורות שלמות עם `[T..]`; `DisposeAsync` מחכה שכל ההודעות יודפסו. "תבנו את זה בעצמכם במעבדה 4."
- 1 ו-3 (unbounded, שלושה צרכנים) — מספיק להראות בפלט.

---

### 12:22–12:30 | תרגול — תרגיל 8

| # | כותרת | קושי | תשובה צפויה |
|---|-------|------|--------------|
| 8 | SemaphoreSlim | ★★ | 10 "בקשות" של `Task.Delay(200)` עם `SemaphoreSlim(3)` + `WaitAsync`/`Release` ב-`finally`; ~800ms (4 גלים × 200), peak = 3 (עם `Interlocked` למונה הריצה). |

<div dir="ltr">

```bash
cd Day2-Async-APIs/Exercises/Solutions
dotnet run -- 8
```

</div>

- מהירים: תרגיל 9 ★★ (`ConcurrentDictionary.AddOrUpdate` + `Parallel.ForEach`; `Dictionary` רגיל — ספירה שגויה או חריגה) ותרגיל 10 ★★★ (`Channel` מוגבל ל-5, יצרן אחד, שני צרכנים, `Complete`). שניהם גם שיעורי בית.

---

### 12:30–13:15 | הפסקה — ארוחת צהריים

לפני שיוצאים: "בחזרה — מעבדה 2. העתיקו את `Labs/Lab2-ThreadSafeBank/Starter` לתיקיית עבודה."

---

### 13:15–14:15 | מעבדה 2 — בנק בטוח לתהליכונים

[README](../Day2-Async-APIs/Labs/Lab2-ThreadSafeBank/README.md) · [Solution/NOTES.md](../Day2-Async-APIs/Labs/Lab2-ThreadSafeBank/Solution/NOTES.md) · שקף 28

**מטרה:** 10 חשבונות, 8 תהליכונים × 20,000 העברות אקראיות. ה-invariant: **סכום הכסף הכולל קבוע**. לשחזר race, לתקן בכמה דרכים, ליצור deadlock אמיתי ולתקן עם סדר נעילה.

**מה יש ב-Starter:**
- `Bank.cs`: `IBank` (`Transfer`, `GetBalance`, `TotalMoney`, `FailedTransfers`); `UnsafeBank` מוכן; `LockBank`, `InterlockedBank`, `ConcurrentBank`, `OrderedLockBank(accounts, initial, ordered)` — שלדים עם `NotImplementedException` ו-TODO ("האם זה בטוח?" על `GetBalance`/`TotalMoney`). ב-`OrderedLockBank` כבר יש מערך `Lock[]` — נעילה לכל חשבון.
- `StressTest.cs`: `StressTest.Run(bank)` — מריץ ומדפיס זמן, סכום לפני/אחרי, invariant.
- `Program.cs`: מריץ רק את `UnsafeBank`; שורות TODO מוכנות להסרת הערה לכל בנק; `DeadlockDemo.Run` — **TODO** (התלמיד כותב את שני התהליכונים ואת ה-watchdog).

**שלבים בקצרה (60 דק'):**
1. (5) להריץ — `UnsafeBank` "מאבד/ממציא" כסף, שונה בכל ריצה.
2. (10) `LockBank` — `Lock` גלובלי; בדיקת יתרה ועדכון באותו `lock`; `FailedTransfers`.
3. (10) `InterlockedBank` — `long[]` באגורות, `Interlocked.Add`, `TryWithdraw` בלולאת `CompareExchange` (הקוד ב-README).
4. (10) `ConcurrentBank` — `ConcurrentDictionary<int, decimal>` + `AddOrUpdate`.
5. (10) `OrderedLockBank` לא מסודר (from ואז to) + `Thread.Sleep(10)` בין הנעילות + `DeadlockDemo` עם שני תהליכונים (`IsBackground = true`) ו-`Join(timeout)` של 3 שניות → "DEADLOCK".
6. (10) לנעול תמיד את ה-`Id` הנמוך קודם → אין deadlock.
7. (5) להריץ את כולם ולהשוות.

**קריטריוני קבלה:**
- `UnsafeBank` מפר את ה-invariant ברוב הריצות + הסבר.
- ארבעת הבנקים האחרים שומרים על הסכום בכל ריצה.
- בדיקת יתרה אטומית (אין יתרות שליליות) ב-`LockBank` וב-`InterlockedBank`.
- `DeadlockDemo` מדגים deadlock בגרסה הלא-מסודרת ועובר במסודרת.
- אין `lock (this)`, אין `await`/IO בתוך `lock`.

**איפה נתקעים — ואיזה רמז לתת:**

| תקיעה | רמז |
|-------|-----|
| "`Interlocked.Add` לא מקבל `decimal`" | `Interlocked` עובד על `int`/`long` בלבד — לכן אגורות (`long`), המרה ב-`GetBalance`/`TotalMoney`. |
| בדיקת יתרה ב-`InterlockedBank` | הקוד של `CompareExchange` ב-README שלב 3; `Volatile.Read`. |
| "ה-deadlock לא קורה" | חסר `Thread.Sleep(10)` בין שתי הנעילות (בפתרון זה מאפיין `SlowMode`). בלעדיו הוא קורה "לפעמים" — וזו בדיוק הסכנה. |
| התוכנית לא נסגרת אחרי "DEADLOCK" | התהליכונים לא `IsBackground = true` (רמז ב-README). |
| "ה-README אומר שה-Starter מריץ עם watchdog" | ב-Starter `DeadlockDemo.Run` הוא TODO — צריך לכתוב `Join(TimeSpan.FromSeconds(3))` בעצמם. |
| ב-`ConcurrentBank` — איך בודקים יתרה | בתוך ה-update factory (ובלי תופעות לוואי — ה-factory עלול לרוץ שוב), או לוותר ולהסביר למה בעייתי. |
| `TotalMoney` ב-`OrderedLockBank` | כדי לקבל snapshot עקבי צריך לנעול את כל החשבונות לפי סדר; מותר גם "בקירוב". |

**דיון מסכם (14:07–14:15) — מתוך NOTES.md:**
- למה `UnsafeBank` נשבר: `-=` = קריאה+חישוב+כתיבה; גם `_failed++`; גם `Dictionary` עצמו לא בטוח. סטייה של אלפי שקלים אחרי 160,000 העברות.
- טבלת הטרייד-אוף: `LockBank` — הכי פשוט ונכון, הכי איטי ("ברירת מחדל כשלא נמדדה בעיה"); `InterlockedBank` — הכי מהיר, רק `int`/`long`; `ConcurrentBank` — טוב, factory ללא תופעות לוואי; `OrderedLockBank` — טוב, העברות בין חשבונות שונים לא חוסמות זו את זו.
- **שאלת הדיון המומלצת (שקף 28):** "למה `ConcurrentBank` שומר על הסכום בלי לנעול שני חשבונות יחד?" — כל `AddOrUpdate` אטומי למפתח שלו; באמצע העברה הכסף "באוויר" ו-`TotalMoney` עלול להיות שגוי לרגע, אבל בסוף מדויק. **אטומיות לכל פעולה ≠ אטומיות לטרנזקציה.**
- תיקון ה-deadlock: `(first, second) = from > to ? (to, from) : (from, to)` — אין מעגל המתנה.
- זמנים טיפוסיים: Unsafe ~50ms BROKEN; Lock ~100–200; Interlocked ~30–60; Concurrent ~60–120; OrderedLock ~80–150.
- פתרון: `cd Day2-Async-APIs/Labs/Lab2-ThreadSafeBank/Solution` ואז `dotnet run` (Unordered → DEADLOCK אחרי 3 שניות; ordered → OK).

**בונוס למהירים:** `GetStatement(id)` עם היסטוריית תנועות (איזה אוסף? איך מגינים?); `ReaderWriterLockSlim` כדי ש-`GetBalance` לא יחסום קוראים אחרים. אם אתם ב-Visual Studio — להסיר את ה-watchdog, להריץ את הגרסה הלא-מסודרת ב-Debug, Break All ולפתוח **Parallel Stacks** — רואים את שני התהליכונים תקועים (הקדמה למודול 06).

---

### 14:15–14:30 | הרצאה — מודול 04: REST ו-HttpClient (שקפים 29–35)

> 📖 **Notes:** [`Notes/04-rest-apis-http.md`](../Day2-Async-APIs/Notes/04-rest-apis-http.md) · חלוקת הזמן לפי סעיפים: ראו [מתי מלמדים כל קובץ Notes](#מתי-מלמדים-כל-קובץ-notes)

"עוברים מהעולם הפנימי לעולם החיצוני" — רוב המשתתפים מכירים HTTP; התמקדו בדרך הנכונה ב-.NET.

**נקודות מפתח:**

1. בקשה = Method + URL (+ query string) + Headers + Body; תשובה = Status + Headers + Body. **2xx טוב, 4xx אשמתכם, 5xx אשמת השרת.** 404 → "אין תוצאה", לא שגיאה; 429/5xx → retry.
2. REST: משאבים ב-URL, פעלים = CRUD. `POST` → 201 + `Location`; `DELETE` → 204. `GET`/`DELETE` idempotent, `POST` לא.
3. **`HttpClient` אחד לכל האפליקציה** — `new` בכל קריאה מדליף sockets. `BaseAddress`, `Timeout`. ב-DI: `IHttpClientFactory` (יום 4).
4. `System.Net.Http.Json`: `GetFromJsonAsync`, `PostAsJsonAsync`, `PutAsJsonAsync`, `ReadFromJsonAsync`, `EnsureSuccessStatusCode`. `HttpRequestMessage` לשליטה מלאה. `Uri.EscapeDataString` לערכי query.
5. Headers: `Accept`, `UserAgent`, `Authorization: Bearer`, API key.
6. Timeouts בשתי רמות: `Http.Timeout` (ברירת מחדל 100 שניות — להוריד) ו-`CancellationTokenSource` לבקשה בודדת.
7. Retry: רק שגיאות **זמניות** (503/429/502/504/408), מספר מוגבל, **exponential backoff**. לא על 400/401/403/404, לא על POST לא idempotent. Polly — להזכיר.
8. טבלת השגיאות: `HttpRequestException` (רשת / `EnsureSuccess`), `TaskCanceledException` (timeout), `JsonException`.
9. **סודות לא בקוד**: `dotnet user-secrets`, משתני סביבה, `.gitignore`.

**שאלות לכיתה:**
- "קיבלתם 404 על `GET /api/products/999`. לזרוק חריגה?" (לרוב לא — `null`.)
- "למה מותר לנסות שוב `DELETE` אבל מסוכן לנסות שוב `POST`?"

**תפיסות שגויות:**
- "`using var http = new HttpClient()` בכל מתודה זה נקי ונכון."
- "retry על כל שגיאה משפר אמינות."
- "`HttpClient` יזרוק חריגה על 404." (`GetAsync` לא; רק `EnsureSuccessStatusCode`/`GetFromJsonAsync`.)

---

### 14:30–14:37 | דמו — curl + `Day2.Demo.HttpJson` חלק B

קודם curl מול ה-API שרץ ב-T1 (הפקודות מ-[LocalApi README](../Day2-Async-APIs/Demos/Day2.LocalApi/README.md)):

<div dir="ltr">

```bash
curl http://localhost:5080/api/products/1
curl -i -X POST http://localhost:5080/api/products -H "Content-Type: application/json" -d '{"name":"Webcam","price":199,"category":"Video","stock":10}'
curl -i "http://localhost:5080/api/flaky?failRate=0.7"
```

</div>

(ב-PowerShell, `curl` עלול להיות alias ל-`Invoke-WebRequest` — השתמשו ב-`curl.exe`.) הראו: 201 + `Location`, 503 אקראי מ-`/api/flaky`.

אחר כך:

<div dir="ltr">

```bash
cd Day2-Async-APIs/Demos/Day2.Demo.HttpJson
dotnet run
```

</div>

הדמו מריץ את שלושת החלקים (A: JSON, B: LocalApi, C: jsonplaceholder) — עכשיו התמקדו ב-**חלק B**:
- `HttpClient` יחיד עם `Timeout` ו-headers (בראש הקובץ) — "מופע אחד, לא `using` בכל קריאה".
- GET רשימה (6 מוצרים), `GET /api/products/999` → 404 בלי חריגה, POST → 201 + Location, PUT → מחיר 249, DELETE → 204, `?status=Shipped` → הזמנה אחת.
- `/api/slow?ms=3000` עם טוקן של 500ms → cancelled.
- `GetWithRetryAsync` מול `/api/flaky?failRate=0.6` — "attempt N: 503 — retrying..." ואז success.
- אם LocalApi לא רץ — הדמו מדפיס הודעה ברורה; טוב להראות גם את זה.

---

### 14:37–14:45 | תרגול — תרגיל 11

| # | כותרת | קושי | תשובה צפויה |
|---|-------|------|--------------|
| 11 | GET ו-DTO | ★ | `record Product(int Id, string Name, decimal Price, string Category, int Stock)`, `GetFromJsonAsync<List<Product>>("http://localhost:5080/api/products")`, `MaxBy(p => p.Price)` → **Laptop (4,500)**. |

<div dir="ltr">

```bash
cd Day2-Async-APIs/Exercises/Solutions
dotnet run -- 11      # דורש ש-Day2.LocalApi ירוץ
```

</div>

- מהירים: תרגיל 12 ★★ (POST 201 + Location → PUT → DELETE 204 → GET 404 בלי חריגה) ותרגיל 13 ★★★ (retry על `/api/flaky` + timeout 500ms על `/api/slow`). שניהם חופפים למעבדה 3 — מי שלא הגיע לא מפסיד.

---

### 14:45–14:55 | הרצאה — מודול 05: JSON (שקפים 36–40)

> 📖 **Notes:** [`Notes/05-json.md`](../Day2-Async-APIs/Notes/05-json.md) · חלוקת הזמן לפי סעיפים: ראו [מתי מלמדים כל קובץ Notes](#מתי-מלמדים-כל-קובץ-notes)

"מודול קצר וטכני — שתי המלכודות: camelCase ו-enum כמספר" (הערת שקף 36).

**נקודות מפתח:**

1. סריאליזציה/דה-סריאליזציה; `System.Text.Json` מובנה, בלי NuGet.
2. **DTO כ-`record`** positional — שמות הפרמטרים ממופים לשמות ב-JSON; אובייקטים מקוננים ורשימות עובדים לכל עומק. מאפיין מחושב (`Total`) מסורלז אך לא נקרא.
3. ברירת המחדל: PascalCase ו-enum כמספר. `JsonSerializerOptions`: `PropertyNamingPolicy = CamelCase`, `PropertyNameCaseInsensitive`, `WriteIndented`, `DefaultIgnoreCondition = WhenWritingNull`, `JsonStringEnumConverter`. **options סטטי — פעם אחת.** `JsonSerializerOptions.Web` / `JsonSerializerDefaults.Web`; `GetFromJsonAsync` משתמש בו כברירת מחדל.
4. Attributes: `[JsonPropertyName("temperature_2m")]`, `[JsonIgnore]`, `[JsonPropertyOrder]`. ב-record — `[property: ...]`!
5. תאריכים ISO 8601 (UTC / `DateTimeOffset`), `decimal` לכסף, שדה חסר → default, שדה עודף → מתעלמים, `required`.
6. JSON דינמי: `JsonNode` (קריאה + עריכה), `JsonDocument` (קריאה מהירה, **חובה `using`**).
7. `JsonException` על קלט שגוי — לתפוס. Source generators ל-AOT/ביצועים; Newtonsoft לקוד ישן (טבלת השוואה בשקף 40).

**שאלה לכיתה:** "ה-API מחזיר `"status": "Shipped"` וה-DTO שלכם מגדיר `OrderStatus Status`. מה יקרה בלי `JsonStringEnumConverter`?" (`JsonException`.)

**תפיסות שגויות:**
- "`[JsonPropertyName]` על פרמטר של record עובד כמו על מאפיין." (צריך `property:`.)
- "`Deserialize<T>` לעולם לא מחזיר null." (קלט `"null"`.)

---

### 14:55–14:58 | דמו — `Day2.Demo.HttpJson` חלק A (ו-C)

מאותה הרצה (או `dotnet run` שוב):
- **Part A** — ה-JSON המסורלז: camelCase, `"status": "Paid"`, `InternalNote` חסר (`[JsonIgnore]`), `total` מופיע. `record equality`: הפריט שווה, ההזמנה כולה לא — **`List` מושווה לפי reference** (חיבור יפה ליום 1). `WeatherDto` עם `[JsonPropertyName]`, עריכת `JsonNode`, `JsonDocument`, ו-`JsonException` על `{ not json }`.
- **Part C** — jsonplaceholder: עם אינטרנט מציג post ו-todos; בלי — "No internet access ... skipping". "ככה קוד אמיתי צריך להתנהג."

---

### 14:58–15:05 | תרגול — תרגיל 14

| # | כותרת | קושי | תשובה צפויה |
|---|-------|------|--------------|
| 14 | JSON ידני | ★★ | `record Current([property: JsonPropertyName("temperature_2m")] double Temperature, [property: JsonPropertyName("wind_speed_10m")] double WindSpeed, DateTime Time)` + `record Forecast(double Latitude, double Longitude, Current Current)`; אותו דבר עם `(double)node["current"]!["temperature_2m"]!`; סריאליזציה חזרה עם `JsonSerializerDefaults.Web` + `WriteIndented`. תוצאה: 27.4°C, רוח 12.1. |

ה-JSON הנתון נמצא בפתרון ([`Ex14-15.cs`](../Day2-Async-APIs/Exercises/Solutions/Ex14-15.cs)) — **הדביקו אותו על המסך / בצ'אט** לפני שמתחילים, כי ה-README מפנה אליו.

<div dir="ltr">

```bash
dotnet run -- 14
```

</div>

---

### 15:05–15:15 | הפסקה

ודאו ש-`Day2.LocalApi` עדיין רץ ב-T1, ואם צריך — `curl -X POST http://localhost:5080/api/reset`.

---

### 15:15–16:15 | מעבדה 3 — לקוח REST מוקלד (מהירים: מעבדה 4)

[README](../Day2-Async-APIs/Labs/Lab3-RestClient/README.md) · [Solution/NOTES.md](../Day2-Async-APIs/Labs/Lab3-RestClient/Solution/NOTES.md) · שקף 41

**מטרה:** מחלקת `ShopApiClient` שעוטפת את `Day2.LocalApi` — DTOs כ-records, CRUD, שגיאות, retry על `/api/flaky`, timeout על `/api/slow`. "SDK קטן".

**דרישה מוקדמת קריטית:** `Day2.LocalApi` רץ. בדקו לפני שמתחילים — יד למעלה מי ש-`curl` עובד לו.

**מה יש ב-Starter:**
- `Dtos.cs`: `Product`, `ProductInput` מוכנים; `OrderStatus`, `OrderItem`, `Order`, `OrderItemInput`, `OrderInput` — TODO.
- `ShopApiClient.cs`: בנאי שמקבל `HttpClient`; `GetProductsAsync` ממומש; TODO ל-options, `GetProductAsync`, Create/Update/Delete, orders, `GetFlakyAsync`, `GetSlowAsync`, `GetPublicPostAsync`.
- `ApiException.cs` — עם `StatusCode`.
- `Program.cs` — `HttpClient` יחיד עם `BaseAddress = http://localhost:5080`, תרחיש בדיקה עם TODO לכל שלב; אם ה-API לא רץ — הודעה ויציאה.

**שלבים בקצרה (60 דק'):**
1. (10) DTOs + `JsonSerializerOptions` סטטי (camelCase, case-insensitive, `JsonStringEnumConverter`).
2. (10) `GetProductAsync` — **404 → `null`**; `GetOrdersAsync(OrderStatus? status)` עם query string.
3. (10) `CreateProductAsync` (ודאו 201), `UpdateProductAsync` (404 → null), `DeleteProductAsync` (204 → true, 404 → false), `CreateOrderAsync`, `SetOrderStatusAsync`.
4. (10) כל סטטוס אחר → `ApiException(statusCode, message)` עם `{"error": "..."}` מה-body; `HttpRequestException` נתפסת ב-`Program`; בדיקה: מחיר שלילי → 400.
5. (10) `GetFlakyAsync(maxAttempts = 5)` — retry על 503, backoff 100/200/400ms, לא על 4xx; מחזיר מספר ניסיונות.
6. (5) `GetSlowAsync(ms, timeout)` — `TimeSpan?`, `null` ב-timeout. 300ms/2s → מצליח; 3000ms/500ms → `null`.
7. (5, אופציונלי) `GetPublicPostAsync` מול jsonplaceholder; offline → הודעה.

**קריטריוני קבלה:**
- כל המתודות ממומשות ו-`Program` רץ מתחילתו לסופו.
- `GetProductAsync(999)` מחזיר `null`.
- `CreateProductAsync` עם קלט שגוי → `ApiException` עם `StatusCode == 400` והודעת השרת.
- `GetFlakyAsync` מצליח ברוב הריצות ומדפיס מספר ניסיונות.
- `GetSlowAsync(3000, 500ms)` מחזיר `null` תוך ~0.5 שניות.
- `HttpClient` אחד ו-`JsonSerializerOptions` סטטי אחד.
- כשה-API לא רץ — הודעה ברורה, לא crash.

**איפה נתקעים — ואיזה רמז לתת:**

| תקיעה | רמז |
|-------|-----|
| `JsonException` על `status` | חסר `JsonStringEnumConverter` ב-options — או שלא העבירו את ה-options ל-`ReadFromJsonAsync<T>(options)` (רמז ראשון ב-README). |
| `GetProductAsync(999)` זורק | `GetFromJsonAsync` זורק על 404 — להשתמש ב-`GetAsync` ולבדוק `StatusCode` לפני `ReadFromJsonAsync`. |
| ה-`ApiException` מכילה רק "Response status code does not indicate success" | לקרוא את ה-body (`{"error": ...}`) **לפני** שזורקים, במקום `EnsureSuccessStatusCode`. |
| `GetSlowAsync` לא נתפס | `TaskCanceledException` יורשת מ-`OperationCanceledException` — לתפוס את האב. |
| `HttpRequestException: Connection refused` | ה-API לא רץ / נסגר. T1! |
| retry "תמיד נכשל" | עם `failRate=0.5` ו-5 ניסיונות, סיכוי כישלון ~3% — קורה. הזדמנות לדבר על circuit breaker / fallback. |
| שכחו `using` על `HttpResponseMessage` | רמז 2 ב-README. |

**דיון מסכם (16:08–16:15) — מתוך NOTES.md:**
- **למה 404 מחזיר `null` ו-400 זורק?** (שאלת הדיון של שקף 41) — "לא קיים" היא תוצאה צפויה של GET/PUT/DELETE לפי id; 400 היא הפרת חוזה.
- `ShopApiClient` **מקבל** `HttpClient` בבנאי — מופע אחד, ניתן לבדיקה עם handler מזויף, מוכן ל-`IHttpClientFactory`.
- `SendAsync` פרטי אחד לכל הבקשות (body כ-`JsonContent`, מדידת זמן, לוג).
- options מבוסס `JsonSerializerDefaults.Web` + `JsonStringEnumConverter`.
- `HttpRequestException` נתפסת ב-`Program`, לא בלקוח — "הלקוח לא יודע מה נכון לעשות; האפליקציה כן".
- Timeout: `CreateLinkedTokenSource(ct)` + `CancelAfter`; `catch ... when (!ct.IsCancellationRequested)` מבחין בין timeout מקומי (`null`) לביטול של הקורא (ממשיך לזרוק).
- הרצת הפתרון: `cd Day2-Async-APIs/Labs/Lab3-RestClient/Solution` ואז `dotnet run` או `dotnet run -- -v` (לוג לכל בקשה: method, url, status, ms).

**בונוס למהירים:** `CancellationToken` בכל מתודה; `ILogger`/`Action<string>` לכל בקשה; `SearchProductsAsync(term)` עם `Uri.EscapeDataString` (ה-API תומך ב-`?search=`). **מי שסיים הכול — עובר למעבדה 4** (ראו להלן).

#### מעבדה 4 — Async Dashboard (למהירים / שיעורי בית)

[README](../Day2-Async-APIs/Labs/Lab4-AsyncDashboard/README.md) · [Solution/NOTES.md](../Day2-Async-APIs/Labs/Lab4-AsyncDashboard/Solution/NOTES.md) · שקף 49 · 75 דקות

- **מטרה:** לוח בקרה בקונסול שמושך **במקביל** 4 מקורות (`StatsSource`, `PendingOrdersSource`, `HealthSource` מול LocalApi + `WeatherSource` מול Open-Meteo), שורד כשלים חלקיים, מתרענן כל N שניות עד Ctrl+C, ורושם דרך `ChannelLogger`.
- **Starter:** `Dtos.cs` (`Stats`, `Order`, `Health` מוכנים; `WeatherResponse` TODO; `SourceResult<T>` עם `Ok`/`Fail`), `Sources/ISource.cs`, `Sources/LocalApiSources.cs` (`StatsSource` ממומש), `Sources/WeatherSource.cs` (TODO עם timeout 4s), `ChannelLogger.cs` (שלד שמדפיס ישירות), `Dashboard.cs` (רק Stats), `Program.cs` (לולאה TODO).
- **שלבים:** מקורות (15) → `FetchSafeAsync<T>` (10) → `WhenAll` (10) → כשלים חלקיים — לעצור את LocalApi באמצע (10) → `PeriodicTimer` + Ctrl+C + `--interval` (10) → `ChannelLogger` ל-`Console.Error` (10) → `SemaphoreSlim(2)` + `--once` (10).
- **נקודת מפתח ל-NOTES:** "כישלון הוא ערך, לא חריגה" — `SourceResult<T>`; ביטול של המשתמש כן עולה (`when (!ct.IsCancellationRequested)`). `.Result` אחרי `WhenAll` — המקום היחיד שבו הוא לגיטימי.
- **תקיעה צפויה:** `PeriodicTimer` לא נלמד ב-Notes — הפנו לרמז ב-README: `using var timer = new PeriodicTimer(TimeSpan.FromSeconds(n));` ו-`await timer.WaitForNextTickAsync(ct)` בלולאה.
- **בדיקה:** בתיקיית `Solution`: `dotnet run -- --once`, `dotnet run -- --interval 2` (ולעצור את LocalApi באמצע), `dotnet run -- --once --max 1` (סדרתי).

---

### 16:15–16:25 | הרצאה — מודול 06: ביצועים ודיבוג (שקפים 42–48)

> 📖 **Notes:** [`Notes/06-performance-debugging-async.md`](../Day2-Async-APIs/Notes/06-performance-debugging-async.md) · חלוקת הזמן לפי סעיפים: ראו [מתי מלמדים כל קובץ Notes](#מתי-מלמדים-כל-קובץ-notes)

"קצר וממוקד — שייצאו עם צ'קליסט ועם ידיעה שיש כלים ב-VS" (הערת שקף 42). אם הזמן לחוץ — שקפים 43 ו-47 הם החובה.

**נקודות מפתח:**

1. **Sync-over-async** (`.Result`, `.Wait()`, `.GetAwaiter().GetResult()`): חוסם תהליכון → הרעבה; ו-**deadlock** בסביבות עם SynchronizationContext (WPF/WinForms/ASP.NET ישן) — תהליכון ה-UI חסום, וה-continuation צריך לחזור אליו. הפתרון: **async all the way** (כולל `async void` ב-handler ו-`async Task Main`). בקונסול אין deadlock — אבל עדיין חסימה.
2. `ConfigureAwait(false)` — בספריות בלבד; ב-WPF הוא אפילו מזיק (אחרי ה-`await` אי אפשר לגעת ב-UI).
3. מתי **לא** `Task.Run`: סביב IO, בשרת ASP.NET Core, כ"תיקון" ל-deadlock, בתוך ספרייה. השימוש הנכון: CPU כבד מה-UI.
4. הרעבת ThreadPool: latency עולה, CPU נמוך, `ThreadPool.ThreadCount` מטפס לאט.
5. מדידה: `Stopwatch` (לא `DateTime.Now`), כמה ריצות + חימום, Release ולא Debug; BenchmarkDotNet; `dotnet-counters monitor -p <pid>`.
6. VS: async call stacks, **Tasks window** (`Ctrl+Shift+D, K`, מציג Deadlocked), **Parallel Stacks** (`Ctrl+Shift+D, S`), Threads window (`Ctrl+Alt+H`, Freeze), breakpoint מותנה לפי `ManagedThreadId`.
7. `ILogger` עם structured logging (`{Source}` ולא `$"..."`); הקשר ל-`ChannelLogger` של מעבדה 4.
8. צ'קליסט הבאגים (שקף 47) — עברו על 4–5 שורות, השאר לקריאה.

**דמו של דקה — תרגיל 15 כהדגמה:**

<div dir="ltr">

```bash
cd Day2-Async-APIs/Exercises/Solutions
dotnet run -- 15
```

</div>

20 × `.Wait()` בתוך `Task.Run` — איטי ו-`ThreadPool.ThreadCount` קופץ; 20 × `await Task.Delay` — ~200ms. "זה ההבדל בין חסימה להמתנה."

**שאלה לכיתה:** "מחר ב-WPF מישהו כותב `var data = LoadAsync().Result;` בתוך Button_Click. מה יקרה?" (החלון קופא לנצח.)

**תפיסה שגויה:** "`ConfigureAwait(false)` בכל מקום זה best practice." / "`Task.Run` הופך קוד לאסינכרוני."

---

### 16:25–16:30 | סיכום (שקפים 49–51)

- שקף 49 — מעבדה 4 = שיעורי בית (או פתיחה ליום 3).
- שקף 50 — הציטוט: *Async is not about making things faster. It is about not wasting threads while you wait.*
- שקף 51 — עברו על שבע נקודות הסיכום במשפט לכל מודול, ושאלו 2–3 משאלות החזרה שלהלן.

---

## אם מאחרים / אם מקדימים

### אם מאחרים — מה לחתוך (לפי סדר עדיפות)

1. **תרגולים 11 ו-14** (14:37, 14:58) — להפוך לשיעורי בית; מעבדה 3 מכסה את אותם מושגים. חוסך 15 דק'.
2. **ValueTask, `ReaderWriterLockSlim`, `Mutex`, source generators, טבלת Newtonsoft** — משפט אחד לכל אחד, להפנות ל-Notes.
3. **דמואים משניים:** Threads 3 ו-8; Synchronization 2, 5, 8; ProducerConsumer 1, 3; HttpJson חלק C.
4. **מעבדה 1:** שלב 6 (טבלת סיכום) נעשה בדיון; שלב 5 (התקדמות) — אופציונלי.
5. **מעבדה 2:** שלב 4 (`ConcurrentBank`) — שיעורי בית, אבל **לשמור** את שלבים 5–6 (deadlock וסדר נעילה) ואת שאלת הדיון.
6. **מעבדה 3:** שלב 7 (API ציבורי) אופציונלי ממילא; שלב 6 אפשר להראות מהפתרון.
7. **מודול 06:** רק שקף 43 (sync-over-async) ושקף 47 (צ'קליסט); השאר לקריאה ב-Notes/06. **אל תחתכו אותו לגמרי** — יום 3 (WPF) נשען עליו.

### אם מקדימים — מה להוסיף

- תרגילים שלא שובצו: 3 ★★, 5 ★★, 7 ★★★, 9 ★★, 10 ★★★, 12 ★★, 13 ★★★ (למהירים בכל חלון תרגול).
- **Threads דמו 5** (`dotnet run -- 5`) — עלות תהליכונים: 200 threads מול 200 `Task.Run` מול 200 `Task.Delay`.
- **דיבוג חי ב-Visual Studio:** לקחת את `OrderedLockBank` הלא-מסודר ממעבדה 2 בלי watchdog, Break All, ולהראות **Parallel Stacks** ו-**Threads window**; להראות **Tasks window** על דמו AsyncAwait עם breakpoint.
- **`dotnet-counters`** על תרגיל 15 (`dotnet tool install -g dotnet-counters`, `dotnet-counters monitor -p <pid>`) — ThreadPool Thread Count ו-Queue Length בזמן אמת.
- הצגת הקוד של `Day2.LocalApi` — Minimal API עם `ConcurrentDictionary` + `Interlocked` (המחסן משותף בין בקשות מקבילות); `/api/slow` מכבד את ה-`CancellationToken` של הבקשה.
- בונוסים של המעבדות (ראו בכל מעבדה), ובמיוחד התחלת מעבדה 4 בכיתה.

---

## סיכום היום

### שאלות חזרה (עם תשובות קצרות)

1. **מה ההבדל בין `Parallel.ForEach` ל-`Task.WhenAll`, ומתי כל אחד?**
   `Parallel` מחלק עבודת **CPU** בין ליבות ומשתמש בתהליכונים; `WhenAll` ממתין לכמה פעולות **IO** בו-זמנית בלי לתפוס תהליכונים בזמן ההמתנה.
2. **למה בגרסת `WhenAll` של מעבדה 1 היה צריך `Task.Run` סביב `ProcessImage`?**
   כי הקוד שלפני ה-`await` הראשון ב-lambda `async` רץ סינכרונית — בלי `Task.Run` כל החישובים רצים בזה אחר זה בזמן יצירת ה-Tasks.
3. **למה `Interlocked` לא מספיק ל"בדוק יתרה ואז הפחת", ומה עושים?**
   הוא אטומי לפעולה אחת על משתנה אחד. צריך `lock` סביב הבדיקה והעדכון, או לולאת `CompareExchange`.
4. **איך מונעים deadlock בהעברה בין שני חשבונות?**
   סדר נעילה קבוע — תמיד לנעול קודם את ה-`Id` הנמוך — ואז אין מעגל המתנה.
5. **למה 404 מחזיר `null` ו-400 זורק `ApiException` בלקוח של מעבדה 3?**
   "לא נמצא" היא תוצאה צפויה; 400 היא שגיאה בבקשה שהקורא צריך לדעת עליה. ו-retry רק על שגיאות זמניות (503/429), לעולם לא על 4xx.
6. **מה קורה ב-`LoadAsync().Result` בתוך event handler של WPF?**
   Deadlock: תהליכון ה-UI חסום, וה-continuation מחכה לחזור אליו. הפתרון: async all the way — `async void` ב-handler ו-`await`.

### שיעורי בית

- **מעבדה 4 — Async Dashboard** ([README](../Day2-Async-APIs/Labs/Lab4-AsyncDashboard/README.md)) — לכל מי שלא סיים בכיתה (זו גם ההמלצה בשקף 51).
- להשלים את מה שלא נגמר ממעבדות 1–3 ולהשוות ל-`Solution/NOTES.md`.
- תרגילים שלא נפתרו בכיתה: 3, 5, 7, 9, 10, 12, 13, 15 (לפחות כל ה-★ וה-★★ — לפי הכלל ב-COURSE-OVERVIEW).
- לקרוא את [Notes/06](../Day2-Async-APIs/Notes/06-performance-debugging-async.md) במלואו — במיוחד sync-over-async ו-`ConfigureAwait` — לקראת מחר.

### הצצה ליום 3

מחר — **WPF** (עם מבט ל-WinForms): XAML, Data Binding, MVVM-lite. שם `async`/`await` פוגש את ה-UI thread: `.Result` באמת מקפיא את החלון, `IProgress<T>` מעדכן `ProgressBar` בלי `Dispatcher`, ו-`ConfigureAwait(false)` בקוד UI שובר את הגישה לפקדים. נחבר את ה-UI לקוד של היום — `HttpClient`, JSON ו-`Day2.LocalApi`. השאירו את `ShopApiClient` שלכם מוכן.

</div>
