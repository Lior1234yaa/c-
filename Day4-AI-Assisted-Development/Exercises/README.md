# יום 4 — תרגילים קצרים

12 תרגילים של 5–15 דקות, מקובצים לפי מודול. דירוג: ★ קל, ★★ בינוני, ★★★ מאתגר.
תרגילי **קוד** (2, 6–11) — הפתרון המלא ב-`Solutions/` ורץ עם `dotnet run -- <מספר>`.
תרגילי **כתיבת prompt / ניתוח** (1, 3, 4, 5, 12) — תשובות לדוגמה ב-`Solutions/PROMPTS-ANSWERS.md`.

מותר (ורצוי) להשתמש בכלי AI בתרגילים — אבל **בדקו כל פלט** לפי הצ'ק-ליסט של מודול 4.

---

## מודול 1 — תהליכי עבודה עם AI

### תרגיל 1 ★ — איזה מצב עבודה?
לכל משימה קבעו: השלמה (autocomplete), צ'אט, או סוכן — ונמקו במשפט:
1. להוסיף `ToString()` למחלקת `Customer`.
2. להבין למה `DataGrid` לא מתעדכן אחרי `Add` לרשימה.
3. להעביר 14 קבצים מ-Newtonsoft.Json ל-System.Text.Json ולהריץ את הבדיקות.
4. לכתוב `for` שממלא מערך.
5. להחליט בין SQLite ל-JSON לשמירת נתונים.

### תרגיל 2 ★ — API מומצא
הקוד הבא "נוצר ב-AI" ולא מתקמפל. מצאו את שלושת ה-APIs המומצאים ותקנו עם APIs אמיתיים של .NET:

```csharp
var names = new List<string> { "Dana", "yossi", "Noa" };
names.RemoveWhere(n => n.Length < 3);
if (names.ContainsIgnoreCase("YOSSI")) Console.WriteLine("found");
var joined = names.JoinWith(", ");
Console.WriteLine(joined);
```

---

## מודול 2 — Prompt Engineering

### תרגיל 3 ★ — שכתוב prompt גרוע
ה-prompt "תכתוב לי service להזמנות" מחזיר ניחוש. שכתבו אותו לפי המבנה Role / Context / Task / Constraints / Examples / Output, עבור `IOrderService` עם `CreateAsync` ו-`GetByCustomerAsync`, ב-.NET 10 עם `IOrderRepository` מוזרק.

### תרגיל 4 ★★ — בדיקות קודם
כתבו prompt שמבקש **רק בדיקות** (xUnit) ל-`PasswordPolicy.Validate(string password)` לפי המפרט: אורך 8–64, לפחות ספרה אחת ואות גדולה אחת, בלי רווחים, מחזיר רשימת שגיאות (ריקה = תקין). ציינו שמות בדיקות ומקרי גבול.

### תרגיל 5 ★★ — קובץ הוראות
כתבו `CLAUDE.md` (או `copilot-instructions.md`) קצר (10–15 שורות) לאפליקציית קונסול .NET 10 שמייבאת CSV ל-JSON: stack, מבנה תיקיות, 4 מוסכמות, פקודות build/test, ו-2 דברים אסורים.

---

## מודול 3 — האצת קוד

### תרגיל 6 ★★ — Records מ-JSON
בקשו מ-AI (או כתבו) records ל-JSON הבא, עם `System.Text.Json`, ו-deserialize אותו. בדקו: כסף ב-`decimal`, `email` יכול להיות `null`, `created_at` הוא `DateTimeOffset`.

```json
{ "order_id": 42, "customer": { "id": 7, "name": "Dana", "email": null },
  "lines": [ { "sku": "A-1", "qty": 2, "unit_price": 19.9 } ],
  "created_at": "2025-03-01T10:15:00+02:00" }
```

### תרגיל 7 ★ — Regex בלי עוגנים
ה-AI הציע `\d{3}-\d{7}` ל"מספר טלפון ישראלי (0XX-XXXXXXX)". הראו קלט שגוי שעובר, תקנו (עוגנים, קידומת 0), והשתמשו ב-`[GeneratedRegex]`.

---

## מודול 4 — סקירת קוד AI

### תרגיל 8 ★★ — async
מצאו שני באגים ותקנו:

```csharp
public class Loader
{
    public async void Load(string url)
    {
        var client = new HttpClient();
        var text = client.GetStringAsync(url).Result;
        Console.WriteLine(text.Length);
    }
}
```

### תרגיל 9 ★★ — תרבות וזמן
מה יקרה על מחשב עם תרבות `de-DE`? ומה הבעיה עם התוקף? תקנו:

```csharp
public static (decimal Amount, DateTime Expires) ParseVoucher(string amountText)
{
    var amount = decimal.Parse(amountText);            // "19.90"
    var expires = DateTime.Now.AddDays(30);
    return (amount, expires);
}
```

### תרגיל 10 ★★★ — משאבים ו-thread safety
מצאו שלוש בעיות (HttpClient, מילון, חריגות) ותקנו:

```csharp
public class PriceCache
{
    private readonly Dictionary<string, decimal> _cache = new();

    public async Task<decimal> GetAsync(string sku)
    {
        if (_cache.ContainsKey(sku)) return _cache[sku];
        try
        {
            using var http = new HttpClient();
            var json = await http.GetStringAsync($"https://api.example.com/prices/{sku}");
            var price = decimal.Parse(json);
            _cache[sku] = price;
            return price;
        }
        catch { return 0; }
    }
}
```

---

## מודול 5 — ארכיטקטורה

### תרגיל 11 ★★ — הזרקת תלויות
המחלקה יוצרת את התלויות שלה בעצמה. הפכו ל-constructor injection עם ממשקים (`IClock`, `INotifier`), ורשמו ב-`ServiceCollection`. הראו בדיקה עם fake:

```csharp
public class ReminderService
{
    public string Remind(string user)
    {
        var now = DateTime.Now;
        var notifier = new EmailNotifier();
        notifier.Send(user, $"Reminder at {now:HH:mm}");
        return "sent";
    }
}
```

---

## מודול 7 — AI ל-UI

### תרגיל 12 ★ — Prompt לחלון
כתבו prompt לחלון WPF "התחברות": שדות שם משתמש וסיסמה, כפתור, הודעת שגיאה, מצב טעינה, RTL ועברית, validation דרך `INotifyDataErrorInfo`, בלי code-behind, ופירוט ה-ViewModel. ציינו 3 דברים שתבדקו בפלט.
