<div dir="rtl">

# Lab 4 — הערות לפתרון ורשימת הבאגים

## רשימת הבאגים ב-Starter (ומה מתקן אותם)

| # | קובץ | הבאג | איך מוצאים | התיקון |
|---|------|------|------------|--------|
| 1 | `Bank.cs` / `Account.Deposit` | `amount < 0` מאפשר הפקדה של 0 | `deposit acc1 0` מצליח ורושם היסטוריה | `amount <= 0` + `InvalidAmountException` |
| 2 | `Account.Withdraw` | היתרה מופחתת **לפני** הבדיקה, והבדיקה משווה לסכום החדש → יתרה שלילית | `deposit 100` ואז `withdraw 80` — "מצליח"; breakpoint על `Balance -= amount`, Watch על `Balance` | לבדוק `amount > Balance` לפני ההפחתה |
| 3 | `Account.RecentHistory` | off-by-one: `Count - count - 1` מחזיר פעולה אחת יותר מדי | `show` אחרי 4 פעולות מדפיס 4 שורות במקום 3; Watch על `start` | `Math.Max(History.Count - count, 0)` |
| 4 | `Bank.Get` | `Open` הופך ל-uppercase אבל `Get` לא — `deposit acc1` נכשל ב-"not found" | Immediate Window: `_accounts.Keys` | `Dictionary` עם `StringComparer.OrdinalIgnoreCase` (או normalize בכל כניסה) |
| 5 | `Bank.Withdraw` | `throw ex;` מוחק את ה-stack trace | Exception Settings → break on `BankException`; Call Stack מתחיל ב-`Withdraw` ולא ב-`Account.Withdraw` | `throw;` |
| 6 | `Bank.Transfer` | copy-paste: `from.Deposit(amount)` במקום `to.Deposit` — הכסף חוזר למקור | `transfer` ואז `list` — היתרות לא השתנו | `to.Deposit(amount)` + rollback בכישלון |
| 7 | `Program.cs` | `TransactionLog` לא נסגר — `transactions.log` נשאר ריק (buffer לא נכתב) | פתחו את הקובץ אחרי ריצה | `using` |
| 8 | `Program.cs` | `decimal.Parse` על קלט לא מספרי → `FormatException` שלא נתפס → קריסה | `deposit acc1 abc` | `TryParse` בתוך `ParseAmount` + `catch (ArgumentException)` |
| 9 | `Program.cs` | פקודה עם פרמטר חסר → `IndexOutOfRangeException` → קריסה | `deposit acc1` | `RequireArgs` (guard clause) + `catch` |
| 10 | `Program.cs` | `processed++` בתוך ה-`try` — לא נספר בכישלון (הספירה לא עקבית) | `processed + failed` לא שווה למספר הפקודות | `finally { processed++; }` |

הבאג ה-11 (רך): ההודעה של insufficient funds ב-Starter מדפיסה יתרה **אחרי** ההפחתה (שלילית) — נעלם עם תיקון 2.

## דגשים

- **היררכיית חריגות**: `BankException` כבסיס, ותת-מחלקות עם **נתונים** (`Shortfall`, `AccountId`). ב-`Program` יש `catch` ספציפי ל-`InsufficientFundsException` שמנצל את `Shortfall`, ואז `catch (BankException)` כללי. הסדר חשוב — ספציפי קודם.
- **חריגה מול ערך חזרה**: קלט לא מספרי מהמשתמש הוא מצב **צפוי**, לכן `TryParse`. אבל ברגע שהקלט לא תקין, אנחנו כן זורקים `ArgumentException` — כי זו הדרך הנוחה לצאת מה-`switch` לנקודה אחת שמטפלת בכל שגיאות השימוש. שני הכלים משלימים.
- **`using` על `TransactionLog`**: `StreamWriter` מחזיק buffer בזיכרון. בלי `Dispose()` (או `Flush()`), הנתונים לא מגיעים לדיסק. `using` מבטיח את זה גם כשיוצאים בחריגה.
- **`finally` לספירה**: `continue` בתוך `catch` **לא** מדלג על `finally` — הוא רץ תמיד.
- **rollback ב-`Transfer`**: אם `Withdraw` הצליח ו-`Deposit` נכשל (למשל סכום 0 — שלא יכול לקרות אחרי הוולידציה, אבל עקרונית), מחזירים את הכסף ו-`throw;`. זו תבנית "compensating action" — ביום 3 נראה טרנזקציות אמיתיות ב-DB.

## הליכת דיבוג מומלצת (למרצה / לתלמיד)

1. הריצו את ה-Starter עם הרצף: `open acc1 Dana` → `deposit acc1 100` → `withdraw acc1 80` → `show acc1`. היתרה שלילית? זה באג 2 (ואם קיבלתם "not found" — זה באג 4, תקנו קודם).
2. breakpoint ב-`Account.Withdraw`, F10 שורה-שורה, Watch על `Balance` ו-`amount`.
3. Exception Settings → סמנו `BankException` → break when thrown. שימו לב ל-Call Stack: אחרי `throw ex;` הפריים של `Account.Withdraw` נעלם.
4. Conditional breakpoint ב-`Transfer` עם התנאי `amount > 40`.
5. Immediate Window: `_accounts.Count`, `_accounts.Keys`, `from.Balance`.
6. Edit & Continue: תקנו את `from.Deposit` ל-`to.Deposit` בזמן שהתוכנית מושהית והמשיכו.

## בדיקה

<div dir="ltr">

```bash
printf 'open acc1 Dana\ndeposit acc1 100\nwithdraw ACC1 30\nwithdraw acc1 500\ndeposit acc1 abc\ndeposit acc1\ndeposit acc1 0\nopen acc2 Yossi\ntransfer acc1 acc2 50\nlist\nshow acc1\nquit\n' | dotnet run
cat transactions.log
```

</div>

צפוי: יתרות סופיות acc1 = 20, acc2 = 50; ארבע שגיאות ידידותיות (insufficient עם short by 430, not a number, needs 2 arguments, amount must be positive); `processed=11, failed=4`; קובץ log עם 6 שורות.

</div>
