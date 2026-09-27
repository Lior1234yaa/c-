<div dir="rtl">

# מעבדה 1 — עיבוד מקבילי: סדרתי מול `Task.WhenAll` מול `Parallel.ForEach`

**משך:** 50 דקות | **מודולים:** 01, 02 | **פרויקט:** `Starter/Day2.Lab1.Starter`

## המטרה

חנות אונליין מקבלת מאות הזמנות, ולכל הזמנה צריך "לעבד" תמונה של המוצר (עבודת CPU) ו"לשלוח אישור" (עבודת IO). נממש את העיבוד בשלוש דרכים, נמדוד, נוסיף ביטול ב-Ctrl+C ודיווח התקדמות — ונבין מתי כל כלי מתאים.

## דרישות מוקדמות

- מודול 01 (Threads, CPU-bound מול IO-bound, `Parallel.ForEach`) ומודול 02 (`Task.WhenAll`, `CancellationToken`, `IProgress<T>`).
- הפרויקט `Starter` מתקמפל; חפשו `// TODO`.

## המצב ההתחלתי

ב-`Starter` יש:

- `Order` — record עם `Id`, `Customer`, `ImageSize`.
- `OrderProcessor.ProcessImage(order)` — מדמה עבודת CPU (לולאת חישוב שתלויה ב-`ImageSize`) ומחזירה checksum.
- `OrderProcessor.SendConfirmationAsync(order, ct)` — מדמה IO (`Task.Delay` של 50–150 ms).
- `Program.cs` — מייצר 40 הזמנות ומריץ `RunSequential`. שאר הגרסאות זורקות `NotImplementedException`.

## שלבים

### שלב 1 — קו הבסיס (5 דק')
הריצו את ה-Starter. רשמו את הזמן של הגרסה הסדרתית. חשבו: כמה מהזמן הוא CPU וכמה IO? (רמז: `Stopwatch` סביב כל חלק).

### שלב 2 — `Task.WhenAll` (10 דק')
ממשו `RunWhenAllAsync`: לכל הזמנה צרו Task שמבצע `ProcessImage` ואז `await SendConfirmationAsync`. אספו את כל ה-Tasks ל-`List<Task<OrderResult>>` ו-`await Task.WhenAll`. מדדו.

שאלה למחשבה: `ProcessImage` הוא CPU-bound. אם תעטפו אותו ב-`Task.Run` — מה ישתנה? ואם לא?

### שלב 3 — `Parallel.ForEach` (10 דק')
ממשו `RunParallelForEach`: עבדו את התמונות עם `Parallel.ForEach` ו-`MaxDegreeOfParallelism = Environment.ProcessorCount`. אספו את התוצאות ל-`ConcurrentBag<OrderResult>` (או למערך לפי אינדקס). מכיוון שזו לולאה סינכרונית — שלחו את האישורים **אחרי** הלולאה עם `WhenAll`.

בונוס: `Parallel.ForEachAsync` (.NET 6+) מאפשר גוף אסינכרוני — נסו לאחד את שני השלבים.

### שלב 4 — ביטול עם Ctrl+C (10 דק')
צרו `CancellationTokenSource` ורשמו ל-`Console.CancelKeyPress`:

<div dir="ltr">

```csharp
Console.CancelKeyPress += (_, e) => { e.Cancel = true; cts.Cancel(); };
```

</div>

העבירו את הטוקן לכל הגרסאות: `ct.ThrowIfCancellationRequested()` לפני כל תמונה, `Task.Delay(..., ct)` באישור, `ParallelOptions.CancellationToken`. תפסו `OperationCanceledException` ב-`Program` והדפיסו כמה הזמנות הספיקו להסתיים.

### שלב 5 — דיווח התקדמות (10 דק')
הוסיפו פרמטר `IProgress<int>` (אחוזים) לכל הגרסאות. ב-`Program` צרו `Progress<int>` שמדפיס פס התקדמות בשורה אחת (`\r`). שימו לב: ב-`Parallel.ForEach` הדיווח מגיע מכמה תהליכונים — השתמשו ב-`Interlocked.Increment` על מונה.

### שלב 6 — טבלת סיכום (5 דק')
הדפיסו טבלה: שם הגרסה, זמן, האצה יחסית לסדרתי. הריצו עם 40 ו-200 הזמנות. מה ההבדל?

## קריטריוני קבלה

- [ ] שלוש הגרסאות מחזירות את אותם 40 checksums (סדר לא חשוב).
- [ ] `WhenAll` ו-`Parallel` מהירות משמעותית מהסדרתי (לפחות x2 על 4 ליבות).
- [ ] Ctrl+C עוצר את הריצה תוך פחות משנייה, עם הודעה מסודרת ולא crash.
- [ ] התקדמות מוצגת ומגיעה ל-100%.
- [ ] אין כתיבה ל-`List<T>` רגיל מתוך `Parallel.ForEach`.

## בונוס

- הגבילו את שליחת האישורים ל-5 בו-זמנית עם `SemaphoreSlim` (מודול 03).
- הוסיפו `--count N` ו-`--mode seq|whenall|parallel` כפרמטרים לשורת הפקודה.

## רמזים

- `Task.WhenAll(tasks)` על `IEnumerable<Task<T>>` מחזיר `T[]`.
- `Parallel.ForEach` זורק `OperationCanceledException` כשהטוקן מבוטל — תפסו אותה מחוץ ללולאה.
- כדי לראות את ההבדל בין CPU ל-IO: הריצו את `WhenAll` פעם עם `Task.Run` סביב `ProcessImage` ופעם בלי.

</div>
