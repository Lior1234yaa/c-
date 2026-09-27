<div dir="rtl">

# מעבדה 1 — הערות לפתרון

## החלטות מרכזיות

- **`Task.Run` סביב `ProcessImage` בגרסת `WhenAll`.** זו הנקודה הכי חשובה במעבדה. `ProcessImage` הוא CPU-bound וסינכרוני; ב-lambda `async` הקוד עד ה-`await` הראשון רץ סינכרונית על התהליכון הקורא. בלי `Task.Run`, `orders.Select(async ...)` היה מבצע את כל החישובים **בזה אחר זה** בזמן יצירת ה-Tasks, ורק האישורים היו מקבילים. נסו להסיר את `Task.Run` ולמדוד — תראו את ההבדל.
- **`Parallel.ForEach` לחלק ה-CPU בלבד.** הלולאה סינכרונית, ולכן אי אפשר `await` בתוכה. פיצלנו: CPU בלולאה (מוגבל למספר הליבות), ואז IO עם `WhenAll`. הבונוס `Parallel.ForEachAsync` מאחד את השניים עם גוף אסינכרוני ו-`MaxDegreeOfParallelism`.
- **`ConcurrentDictionary`/`ConcurrentBag` לתוצאות** מתוך `Parallel.ForEach` — `List<T>` לא בטוח לכתיבה מקבילית (ראו מודול 03). אפשרות שקולה: מערך לפי אינדקס.
- **ביטול שיתופי.** `Console.CancelKeyPress` עם `e.Cancel = true` מונע מהתהליך למות מיד ונותן לנו לבטל דרך ה-`CancellationTokenSource`. הטוקן מועבר לכל שכבה: `ThrowIfCancellationRequested` לפני חישוב, `Task.Delay(..., ct)` ב-IO, `ParallelOptions.CancellationToken` בלולאה. תפיסת `OperationCanceledException` פעם אחת ב-`Measure`.
- **התקדמות עם `Interlocked.Increment`.** הדיווח מגיע מכמה תהליכונים; מונה רגיל היה מאבד עדכונים. `Progress<T>` דואג שה-callback ירוץ על ה-context המקורי (ב-WPF: תהליכון ה-UI).

## תוצאות טיפוסיות (4 ליבות, 40 הזמנות)

| גרסה | זמן | האצה |
|------|-----|------|
| Sequential | ~7000 ms | x1 |
| Task.WhenAll | ~1200 ms | x6 |
| Parallel.ForEach + WhenAll | ~1200 ms | x6 |
| Parallel.ForEachAsync | ~1300 ms | x5.5 |

הסדרתי = סכום ה-CPU (~100ms להזמנה) + IO (~75ms). המקבילי מוגבל בעיקר ע"י CPU / מספר ליבות (40 × 100ms / 4 ליבות ≈ 1000ms), וה-IO חופף לחלוטין.

## מה כדאי להדגיש בכיתה

- ההבדל בין "להפעיל את כל ה-Tasks ואז WhenAll" לבין "await בלולאה" — השני סדרתי.
- IO לא צריך `Task.Run`; CPU כן (כשרוצים מקביליות אמיתית).
- `Parallel.ForEach` זורק `OperationCanceledException` בביטול — לתפוס מחוץ ללולאה.

</div>
