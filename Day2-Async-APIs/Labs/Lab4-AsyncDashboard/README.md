<div dir="rtl">

# מעבדה 4 — לוח בקרה אסינכרוני (Async Dashboard)

**משך:** 75 דקות | **מודולים:** 02, 03, 04, 05, 06 | **פרויקט:** `Starter/Day2.Lab4.Starter`

## המטרה

מעבדת האינטגרציה של היום: אפליקציית קונסול שמציגה "לוח בקרה" למנהל החנות. בכל רענון היא מושכת **במקביל** כמה מקורות — סטטיסטיקות מ-`Day2.LocalApi`, הזמנות ממתינות, בריאות השרת, ומזג האוויר בתל אביב מ-Open-Meteo (API ציבורי) — מאחדת אותם עם `Task.WhenAll`, מציגה טבלה, וחוזרת על זה כל N שניות עד Ctrl+C. מקור שנכשל לא מפיל את הלוח: הוא מוצג כ-"unavailable". כל הלוגים עוברים דרך logger מבוסס `Channel<T>`.

## דרישות מוקדמות

- `Day2.LocalApi` רץ.
- מעבדה 3 (או לפחות מודול 04): `HttpClient`, DTOs, טיפול בשגיאות.
- מודול 03: `Channel<T>`; מודול 02: `WhenAll`, `CancellationToken`, `PeriodicTimer`.

## המצב ההתחלתי

ב-`Starter`:

- `Dtos.cs` — `Stats`, `Order`, `Health` (מוכנים), `WeatherResponse` (TODO — שמות snake_case).
- `Sources/` — `ISource<T>` עם `Name` ו-`FetchAsync(ct)`; `StatsSource` ממומש; `PendingOrdersSource`, `HealthSource`, `WeatherSource` — TODO.
- `SourceResult<T>` — record שמייצג הצלחה (`Value`) או כישלון (`Error`) + משך.
- `ChannelLogger` — שלד: `Log(string)` ו-`DisposeAsync`, TODO.
- `Dashboard.cs` — `RefreshAsync(ct)` מריץ כרגע רק את `StatsSource`; `RenderAsync` מדפיס.
- `Program.cs` — לולאת רענון TODO.

## שלבים

### שלב 1 — המקורות (15 דק')
1. `PendingOrdersSource` — `GET /api/orders?status=Pending`, מחזיר `List<Order>`.
2. `HealthSource` — `GET /api/health`.
3. `WeatherSource` — `GET https://api.open-meteo.com/v1/forecast?latitude=32.08&longitude=34.78&current=temperature_2m,wind_speed_10m`. השלימו את `WeatherResponse` עם `[JsonPropertyName("temperature_2m")]` וכו'. **חובה** timeout של 3–5 שניות (`CancellationTokenSource` מקושר).

### שלב 2 — `FetchSafeAsync` (10 דק')
כתבו מתודה גנרית `Task<SourceResult<T>> FetchSafeAsync<T>(ISource<T> source, CancellationToken ct)`: מודדת זמן (`Stopwatch`), קוראת ל-`source.FetchAsync`, ומחזירה `SourceResult.Ok(value, elapsed)` או `SourceResult.Fail(errorMessage, elapsed)`. תפסו `HttpRequestException`, `TaskCanceledException` (timeout) ו-`JsonException`. **אל תתפסו** ביטול שמגיע מה-`ct` של המשתמש — תנו לו לעלות (`when (!ct.IsCancellationRequested)`).

### שלב 3 — איסוף במקביל (10 דק')
ב-`Dashboard.RefreshAsync`: הפעילו את ארבעת ה-`FetchSafeAsync` **בלי `await`**, ואז `await Task.WhenAll(...)`. מדדו את זמן הרענון הכולל — הוא צריך להיות בערך כמו המקור **האיטי ביותר**, לא הסכום. הציגו לכל מקור: שם, סטטוס (OK / FAILED), משך, ותקציר הערך.

### שלב 4 — כשלים חלקיים (10 דק')
עצרו את `Day2.LocalApi` באמצע הריצה — הלוח צריך להמשיך לעבוד, עם 3 מקורות FAILED ומזג אוויר OK (או ההפך אם אין אינטרנט). הפעילו מחדש את ה-API — הרענון הבא חוזר ל-OK. אם מזג האוויר לא זמין, הציגו "weather: offline" ולא שגיאה מפחידה.

### שלב 5 — לולאת רענון וביטול (10 דק')
ב-`Program`: `PeriodicTimer` עם המרווח מ-`--interval` (ברירת מחדל 5 שניות). `await timer.WaitForNextTickAsync(ct)` בלולאה; Ctrl+C מבטל דרך `CancellationTokenSource`. הרענון הראשון מיד. תפסו `OperationCanceledException` פעם אחת ובסוף הדפיסו "bye".

### שלב 6 — `ChannelLogger` (10 דק')
ממשו את הלוגר: `Channel.CreateUnbounded<string>()`, `Log` עושה `TryWrite` (לא חוסם), `Task` רקע קורא `ReadAllAsync` וכותב ל-`Console.Error` (כדי לא להתערבב עם הלוח) עם חותמת זמן ו-thread id. `DisposeAsync` — `Complete()` ואז `await` על ה-Task. השתמשו בו מכל המקורות ("fetching stats...", "weather failed: timeout").

### שלב 7 — הגבלת מקביליות ו-`--once` (10 דק')
- הוסיפו `SemaphoreSlim(2)` כך שלכל היותר 2 מקורות ימשכו בו-זמנית (דמיינו API עם rate limit). מדדו את ההשפעה על זמן הרענון.
- הוסיפו דגל `--once` שמריץ רענון אחד ויוצא (נוח לבדיקות ולסקריפטים).

## קריטריוני קבלה

- [ ] רענון אחד מציג 4 מקורות; זמן הרענון ≈ המקור האיטי ביותר.
- [ ] כשה-API המקומי לא רץ, הלוח מציג FAILED למקורות שלו וממשיך; אין crash ואין stack trace.
- [ ] כשאין אינטרנט, מזג האוויר מוצג כ-"offline" תוך ≤ 5 שניות (timeout), ושאר המקורות תקינים.
- [ ] Ctrl+C עוצר את הלולאה תוך פחות משנייה, הלוגר מרוקן את התור, מודפס "bye".
- [ ] `--interval 2` ו-`--once` עובדים.
- [ ] כל הלוגים עוברים דרך `ChannelLogger`; אין `Console.WriteLine` ישיר מתוך המקורות.
- [ ] אין `.Result`/`.Wait()`; `CancellationToken` מועבר לכל שכבה.

## בונוס

- שמרו את הרענון האחרון שהצליח לכל מקור והציגו "stale (from 12:30:05)" במקום FAILED.
- הוסיפו מקור חמישי מ-`https://jsonplaceholder.typicode.com/todos?userId=1&completed=false` ("open tasks").
- `IAsyncEnumerable<DashboardSnapshot>` שמייצר snapshot בכל tick, ו-`await foreach` ב-`Program`.

## רמזים

- `PeriodicTimer` (.NET 6+): `using var timer = new PeriodicTimer(TimeSpan.FromSeconds(n));`.
- `Task.WhenAll` על Tasks מטיפוסים שונים: `await Task.WhenAll(t1, t2, t3, t4)` ואז `t1.Result` (בטוח אחרי WhenAll — ה-Task הושלם).
- Open-Meteo מחזיר `{"current": {"time": "...", "temperature_2m": 27.4, "wind_speed_10m": 12.1}}`.
- `Console.Error` ללוגים, `Console.Out` ללוח — כך אפשר להפריד: `dotnet run 2> log.txt`.

</div>
