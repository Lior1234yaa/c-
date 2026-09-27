# Lab 4 — בנק עמיד: חריגות, ולידציה ודיבוג (60 דקות)

## מטרה

הפעם לא כותבים מאפס — **מתקנים**. ה-Starter הוא בנק קטן שמתקמפל ורץ, אבל מכיל לפחות 8 באגים: לוגיים, אחד שמאבד stack trace, אחד שמשאיר קובץ ריק, ושניים שמפילים את התוכנית על קלט לא צפוי. תתרגלו את כלי הדיבוג של Visual Studio (breakpoints, Watch, Call Stack, Exception Settings), תבנו היררכיית חריגות מותאמות, ותהפכו את התוכנית לכזו שלא נופלת על שום קלט.

## מה צריך לדעת לפני

מודול 06 (חריגות ודיבוג) ומודול 07 (קוד נקי). Lab 1 עוזר — מודל דומה.

## התחלה

```bash
cd Labs/Lab4-BankRobust/Starter
dotnet run
```

ואז הקלידו:

```text
open acc1 Dana
deposit acc1 100
withdraw acc1 80
show acc1
```

משהו לא בסדר? יופי. מתחילים.

## שלבים

### שלב 1 — ציד באגים (25 דק')

עבדו עם דיבאגר, לא עם `Console.WriteLine`. לכל באג רשמו: איפה, מה הסימפטום, מה התיקון.

1. הריצו את הרצף שלמעלה. אם קיבלתם `not found` — למה `Open` מצא את החשבון ו-`Get` לא? (רמז: Immediate Window → `_accounts.Keys`).
2. שימו breakpoint ב-`Account.Withdraw` ועברו שורה-שורה (F10) עם Watch על `Balance`. מה הסדר הנכון של הפעולות?
3. בצעו 4 הפקדות ואז `show` — כמה שורות היסטוריה מודפסות? (אמורות להיות 3.)
4. פתחו שני חשבונות, בצעו `transfer`, ואז `list`. הכסף עבר?
5. נסו `deposit acc1 0`. האם זה אמור להצליח?
6. סיימו עם `quit` ופתחו את `transactions.log`. מה יש בו?
7. Exception Settings → סמנו `BankException`. בצעו משיכה שנכשלת והביטו ב-Call Stack. איפה נעלם `Account.Withdraw`? (רמז: `throw ex;`)

### שלב 2 — עמידות לקלט (15 דק')

8. **TODO B** — נסו `deposit acc1 abc`. התוכנית קרסה עם `FormatException`. החליפו את `decimal.Parse` בפונקציית עזר `ParseAmount` שמשתמשת ב-`TryParse`.
9. **TODO C** — נסו `deposit acc1` (בלי סכום). `IndexOutOfRangeException`. הוסיפו guard clause `RequireArgs(parts, n)` ו-`catch` מתאים.
10. ודאו ש-`processed + failed` שווה תמיד למספר הפקודות שהוקלדו. איפה צריך לשבת `processed++`? (רמז: `finally`.)

### שלב 3 — חריגות מותאמות ו-`using` (20 דק')

11. **TODO A** — ב-`Exceptions.cs` בנו את ההיררכיה: `AccountNotFoundException`, `InsufficientFundsException` (עם `Requested`, `Available`, `Shortfall`), `InvalidAmountException`. השתמשו בהן ב-`Account` ו-`Bank`.
12. ב-`Program` הוסיפו `catch (InsufficientFundsException ex)` **לפני** ה-`catch (BankException)` שמדפיס גם `short by ...`.
13. עטפו את `TransactionLog` ב-`using` כך שהקובץ ייסגר תמיד.
14. הוסיפו guard clauses (`ArgumentException.ThrowIfNullOrWhiteSpace`) בבנאי `Account` וב-`Bank.Open`.

## קריטריוני קבלה

- [ ] `deposit 100` → `withdraw 80` משאיר יתרה 20 (לא -60).
- [ ] `withdraw 500` על יתרה 70 מדפיס `error: Insufficient funds ... (short by 430.00)`.
- [ ] `deposit acc1 abc`, `deposit acc1`, `deposit acc1 0`, `foo` — כולם מדפיסים הודעה ולא מפילים את התוכנית.
- [ ] `transfer acc1 acc2 50` מעביר באמת (בדקו עם `list`).
- [ ] `show` מדפיס לכל היותר 3 שורות היסטוריה.
- [ ] `deposit ACC1 ...` ו-`deposit acc1 ...` פונים לאותו חשבון.
- [ ] אחרי `quit`, `transactions.log` מכיל את כל הפעולות.
- [ ] ב-`Bank.Withdraw` יש `throw;` ולא `throw ex;`.
- [ ] `processed + failed` = מספר הפקודות.
- [ ] יש לפחות 3 מחלקות חריגה שיורשות מ-`BankException`, ולפחות אחת עם property נוסף.

## בונוס

- rollback ב-`Transfer`: אם ההפקדה לחשבון היעד נכשלת, החזירו את הכסף למקור (`try/catch` + `throw;`).
- מנעו העברה מחשבון לעצמו.
- Conditional breakpoint: עצרו ב-`Transfer` רק כש-`amount > 40`.
- הוסיפו `Debug.Assert(Balance >= 0)` בסוף `Withdraw` — ובדקו שהוא לא רץ ב-`dotnet run -c Release`.

## רמזים

- `decimal.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out var amount)` — כדי ש-`10.5` יעבוד בכל מחשב.
- `new Dictionary<string, Account>(StringComparer.OrdinalIgnoreCase)`.
- `continue` בתוך `catch` **לא** מדלג על `finally`.
- ב-Visual Studio: **Debug → Windows → Exception Settings**, חפשו את שם החריגה שלכם או סמנו "Common Language Runtime Exceptions".
- `using (var log = new TransactionLog(...)) { ... }` — כל הלולאה בפנים.
