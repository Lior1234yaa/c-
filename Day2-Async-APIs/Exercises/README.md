# תרגילים — יום 2: Multithreading, Async ו-APIs

תרגילים קצרים (5–15 דקות כל אחד) לפי מודול. דרגת קושי: ★ קל, ★★ בינוני, ★★★ מאתגר.
הפתרונות בפרויקט `Solutions/` — `dotnet run -- <מספר תרגיל>` (או `dotnet run` לתפריט).
תרגילים 11–13 דורשים ש-`Day2.LocalApi` ירוץ (`Demos/Day2.LocalApi` → `dotnet run`).

## מודול 01 — Threads

### תרגיל 1 ★ — שני תהליכונים
כתבו תוכנית שמפעילה שני `Thread`-ים: אחד מדפיס את המספרים 1–5, השני את האותיות A–E, כל אחד עם `Thread.Sleep(100)` בין הדפסות. חכו לשניהם עם `Join` והדפיסו "done". הריצו כמה פעמים — האם הפלט זהה?

### תרגיל 2 ★★ — Race condition וסידורו
צרו מונה `int` משותף. הפעילו 4 תהליכונים שכל אחד מגדיל אותו 250,000 פעמים. הדפיסו את התוצאה (צפוי: 1,000,000). תקנו את הבאג פעם עם `lock` ופעם עם `Interlocked.Increment`, והשוו זמנים עם `Stopwatch`.

### תרגיל 3 ★★ — Parallel.For
חשבו את סכום המספרים הראשוניים עד 2,000,000: פעם ב-`for` רגיל ופעם ב-`Parallel.For` (רמז: צברו לתוך `long[]` לפי אינדקס, או `Interlocked.Add`). מדדו את ההאצה.

## מודול 02 — Tasks ו-async/await

### תרגיל 4 ★ — Task.Delay ו-WhenAll
כתבו `async Task<string> FetchAsync(string name, int ms)` שממתין `ms` מילישניות ומחזיר `"{name} ready"`. קראו לה 3 פעמים (300, 200, 100 ms) פעם סדרתית ופעם עם `Task.WhenAll`, ומדדו.

### תרגיל 5 ★★ — WhenAny ו-timeout
הריצו שתי "הורדות" (500 ms ו-150 ms) והדפיסו מי ניצחה (`Task.WhenAny`). אחר כך הוסיפו timeout של 100 ms עם `WaitAsync` ותפסו `TimeoutException`.

### תרגיל 6 ★★ — ביטול והתקדמות
כתבו `async Task CountdownAsync(int from, IProgress<int> progress, CancellationToken ct)` שסופר לאחור בקצב של 100 ms לצעד, מדווח כל צעד, ומכבד ביטול. הפעילו עם `CancellationTokenSource(TimeSpan.FromMilliseconds(450))` והדפיסו "cancelled at N".

### תרגיל 7 ★★★ — IAsyncEnumerable
כתבו `async IAsyncEnumerable<int> GenerateAsync(int count, CancellationToken ct)` שמחזיר מספר כל 50 ms. צרכו אותו ב-`await foreach`, סכמו רק זוגיים, ועצרו (`break`) כשהסכום עובר 20.

## מודול 03 — סנכרון

### תרגיל 8 ★★ — SemaphoreSlim
הריצו 10 "בקשות" (כל אחת `Task.Delay(200)`), אבל הגבילו ל-3 בו-זמנית עם `SemaphoreSlim`. הדפיסו את זמן הריצה הכולל (צפוי ~800 ms) ואת מספר הבקשות המרבי שרצו יחד.

### תרגיל 9 ★★ — ConcurrentDictionary
ספרו הופעות של מילים בטקסט ארוך עם `Parallel.ForEach` ו-`ConcurrentDictionary<string,int>.AddOrUpdate`. הציגו את 3 המילים הנפוצות. בונוס: מה קורה אם משתמשים ב-`Dictionary` רגיל?

### תרגיל 10 ★★★ — Channel producer/consumer
צרו `Channel<int>` מוגבל ל-5. יצרן כותב 20 מספרים (10 ms כל אחד), שני צרכנים קוראים (`ReadAllAsync`, 40 ms כל אחד) ומדפיסים "worker X got N". ודאו שהתוכנית מסתיימת (`Complete`).

## מודול 04–05 — HTTP ו-JSON

### תרגיל 11 ★ — GET ו-DTO
מול `Day2.LocalApi`: הגדירו `record Product(...)`, קראו `GET /api/products` עם `GetFromJsonAsync`, והדפיסו את המוצר היקר ביותר.

### תרגיל 12 ★★ — POST + PUT + DELETE
צרו מוצר חדש ב-POST (בדקו שהסטטוס 201 וקראו את ה-`Location`), עדכנו את מחירו ב-PUT, ומחקו אותו ב-DELETE (צפוי 204). לבסוף GET לפי ה-id — צפוי 404. טפלו ב-404 בלי חריגה.

### תרגיל 13 ★★★ — Retry ו-timeout
קראו ל-`/api/flaky?failRate=0.5` עם עד 5 ניסיונות ו-backoff מעריכי, והדפיסו באיזה ניסיון הצלחתם. אחר כך קראו ל-`/api/slow?ms=3000` עם `CancellationToken` של 500 ms ותפסו את הביטול.

### תרגיל 14 ★★ — JSON ידני
נתון JSON של תחזית (ראו בפתרון). פענחו אותו ל-record עם `[JsonPropertyName]` עבור `temperature_2m` ו-`wind_speed_10m`, ואז עשו את אותו דבר עם `JsonNode` בלי מחלקה. סרלזו חזרה עם `WriteIndented` ו-camelCase.

## מודול 06 — ביצועים ודיבוג

### תרגיל 15 ★★ — sync-over-async ו-ThreadPool
הריצו 20 משימות שכל אחת חוסמת עם `.Wait()` על `Task.Delay(200)` דרך `Task.Run`, ומדדו. הריצו שוב עם `await` נכון. הדפיסו `ThreadPool.ThreadCount` לפני ואחרי כל גרסה והסבירו את ההבדל.
