# מעבדה 2 — בנק בטוח לתהליכונים (Thread-Safe Bank)

**משך:** 60 דקות | **מודולים:** 01, 03 | **פרויקט:** `Starter/Day2.Lab2.Starter`

## המטרה

בנק עם 10 חשבונות ו-8 תהליכונים שמבצעים אלפי העברות אקראיות בו-זמנית. **סכום הכסף הכולל חייב להישאר קבוע** — זה ה-invariant שלנו. נשחזר race condition, נתקן אותו בשלוש דרכים שונות, ניצור deadlock אמיתי ונתקן אותו עם סדר נעילה.

## דרישות מוקדמות

מודול 03: `lock`, `Interlocked`, `ConcurrentDictionary`, deadlock וסדר נעילה.

## המצב ההתחלתי

ב-`Starter`:

- `IBank` — `Transfer(from, to, amount)`, `GetBalance(id)`, `TotalMoney`, `FailedTransfers`.
- `UnsafeBank` — מימוש נאיבי עם `Dictionary<int, decimal>` בלי שום סנכרון. **מוכן.**
- `LockBank`, `InterlockedBank`, `ConcurrentBank`, `OrderedLockBank` — זורקים `NotImplementedException`.
- `StressTest.Run(bank, threads, transfersPerThread)` — מריץ העברות אקראיות ומדפיס: זמן, סכום לפני/אחרי, האם ה-invariant נשמר.
- `DeadlockDemo` — TODO.

## שלבים

### שלב 1 — לשחזר את הבאג (5 דק')
הריצו את ה-Starter. `UnsafeBank` "מאבד" או "ממציא" כסף — הסכום הכולל משתנה. הריצו כמה פעמים: הסכום שונה בכל ריצה. הסבירו לעצמכם **איפה בדיוק** ה-race (רמז: `balances[from] -= amount` הוא קריאה + חישוב + כתיבה).

### שלב 2 — `LockBank` עם נעילה גלובלית (10 דק')
ממשו `LockBank` עם `Lock` אחד (או `object`) לכל הבנק. כל `Transfer` ו-`GetBalance` בתוך `lock`. הריצו: ה-invariant נשמר. שימו לב לזמן — כל ההעברות סדרתיות עכשיו.

הוסיפו בדיקת יתרה: אם `balance < amount` — לא מעבירים, מגדילים `FailedTransfers`. הבדיקה והעדכון חייבים להיות באותו `lock`.

### שלב 3 — `InterlockedBank` (10 דק')
ממשו בנק שמחזיק יתרות כ-`long[]` (באגורות) ומשתמש ב-`Interlocked.Add` להפחתה ולהוספה. ה-invariant נשמר (למה?). אבל: איך עושים "בדוק שיש מספיק ואז הפחת" אטומית? ממשו `TryWithdraw` עם לולאת `Interlocked.CompareExchange`:

```csharp
long current;
do
{
    current = Volatile.Read(ref balances[id]);
    if (current < amount) return false;
} while (Interlocked.CompareExchange(ref balances[id], current - amount, current) != current);
return true;
```

### שלב 4 — `ConcurrentBank` (10 דק')
ממשו עם `ConcurrentDictionary<int, decimal>` ו-`AddOrUpdate`. שימו לב: `AddOrUpdate` אטומי לכל **מפתח בנפרד**, אבל העברה נוגעת בשני מפתחות — האם ה-invariant נשמר? (כן — כל עדכון בודד אטומי, והסכום של שניהם קבוע.) ומה עם בדיקת יתרה? (בעייתי — צריך `lock` או CompareExchange בלולאה.)

### שלב 5 — Deadlock אמיתי (10 דק')
ממשו `OrderedLockBank` בגרסה **ראשונה**: `Lock` לכל חשבון, ו-`Transfer` שנועל קודם את `from` ואז את `to`. הריצו את `DeadlockDemo`: שני תהליכונים, אחד מעביר 1→2 והשני 2→1, עם `Thread.Sleep(10)` בין הנעילות. התוכנית תיתקע (ה-Starter מריץ אותה עם watchdog של 3 שניות שמדווח "DEADLOCK").

### שלב 6 — סדר נעילה (10 דק')
תקנו: נעלו תמיד את החשבון עם ה-`Id` הנמוך קודם, בלי קשר לכיוון ההעברה. הריצו שוב — אין deadlock, ה-invariant נשמר, והביצועים טובים מ-`LockBank` (למה? העברות בין חשבונות שונים לא חוסמות זו את זו).

### שלב 7 — השוואה (5 דק')
הריצו את ה-`StressTest` על כל הבנקים והשוו: נכונות, זמן, `FailedTransfers`.

## קריטריוני קבלה

- [ ] `UnsafeBank` מפר את ה-invariant (לפחות ברוב הריצות) — והסבר קצר למה.
- [ ] `LockBank`, `InterlockedBank`, `ConcurrentBank`, `OrderedLockBank` שומרים על הסכום הכולל בכל ריצה.
- [ ] בדיקת יתרה אטומית (אין יתרות שליליות) ב-`LockBank` וב-`InterlockedBank`.
- [ ] `DeadlockDemo` מדגים deadlock בגרסה הלא-מסודרת ועובר בגרסה המסודרת.
- [ ] אין `lock (this)`, אין `await`/IO בתוך `lock`.

## בונוס

- הוסיפו `GetStatement(id)` שמחזיר היסטוריית תנועות — איזה אוסף תבחרו ואיך תגנו עליו?
- החליפו את `Lock` ב-`ReaderWriterLockSlim` כדי ש-`GetBalance` (קריאה) לא יחסום קריאות אחרות.

## רמזים

- הרנדומליות ב-`StressTest` משתמשת ב-`Random.Shared` — הוא thread-safe.
- ב-deadlock demo, סמנו את התהליכונים `IsBackground = true` — אחרת התוכנית לא תסתיים גם אחרי ה-watchdog.
- `Interlocked` עובד על `int`/`long`, לא על `decimal` — לכן אגורות.
