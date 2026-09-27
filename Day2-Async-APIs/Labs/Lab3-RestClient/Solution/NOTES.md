<div dir="rtl">

# מעבדה 3 — הערות לפתרון

## מבנה

- **`ShopApiClient` מקבל `HttpClient` בבנאי** ולא יוצר אחד. כך יש מופע אחד לכל האפליקציה (או `IHttpClientFactory` בהמשך), והמחלקה ניתנת לבדיקה עם `HttpMessageHandler` מזויף.
- **`SendAsync` פרטי אחד** לכל הבקשות: בונה `HttpRequestMessage`, מוסיף body כ-`JsonContent`, מודד זמן ומרשם ללוג. כל המתודות הציבוריות קצרות ועקביות.
- **`JsonSerializerOptions` סטטי** מבוסס `JsonSerializerDefaults.Web` (camelCase + case-insensitive + מספרים ממחרוזות) בתוספת `JsonStringEnumConverter` — כי ה-API מחזיר `"status": "Shipped"`.

## החלטות על שגיאות

- **404 ⇒ `null` / `false`**, לא חריגה. "המשאב לא קיים" הוא תוצאה צפויה של GET/PUT/DELETE לפי id, ולקורא נוח יותר `if (product is null)` מאשר `try/catch`.
- **כל סטטוס אחר שאינו 2xx ⇒ `ApiException`** עם ה-`StatusCode` וההודעה מה-body (`{"error": "..."}`). כך הקורא רואה "400 BadRequest: name is required and price must be >= 0" ולא רק "Response status code does not indicate success".
- **`EnsureSuccessAsync` עם סטטוס צפוי** (201 ל-POST, 204 ל-DELETE) — בדיקה קפדנית שמזהה API שהתנהג אחרת מהחוזה.
- **`HttpRequestException`** (השרת לא רץ) נתפסת ב-`Program`, לא בלקוח — הלקוח לא יודע מה נכון לעשות; האפליקציה כן.

## Retry

לולאה פשוטה בסגנון Polly: מנסים שוב רק על סטטוסים זמניים (503/429/408/502/504) ועל `HttpRequestException`; backoff מעריכי (100, 200, 400, 800 ms); אחרי `maxAttempts` זורקים `ApiException` שמציינת כמה ניסיונות היו. 4xx לא מנסים שוב — הבעיה בבקשה. עם `failRate=0.5` ו-5 ניסיונות, ההסתברות להיכשל היא 3%; זה קורה לפעמים בכיתה וזו הזדמנות לדבר על מה עושים אז (circuit breaker, fallback).

## Timeout

`CancellationTokenSource.CreateLinkedTokenSource(ct)` + `CancelAfter(timeout)`: הטוקן המקומי מבטל גם כשהקורא ביטל וגם כשעבר הזמן. ה-`catch` מבחין ביניהם עם `when (!ct.IsCancellationRequested)` — timeout מקומי מחזיר `null`, ביטול של הקורא ממשיך לזרוק (הקורא ביקש להפסיק, לא לקבל `null`).

## API ציבורי

`GetPublicPostAsync` מחזיר `null` על כל בעיית רשת (`HttpRequestException` או `OperationCanceledException` מה-timeout) ורושם ללוג. כך התוכנית רצה עד הסוף גם בכיתה בלי אינטרנט.

## הרצה

<div dir="ltr">

```bash
dotnet run          # פלט תמציתי
dotnet run -- -v    # עם לוג של כל בקשה: method, url, status, ms
```

</div>

</div>
