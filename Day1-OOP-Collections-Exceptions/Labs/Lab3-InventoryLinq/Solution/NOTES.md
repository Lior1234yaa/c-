<div dir="rtl">

# Lab 3 — הערות לפתרון

## בחירת האוספים

| צורך | אוסף | למה |
|------|------|-----|
| פריט לפי SKU | `Dictionary<string, StockItem>` | חיפוש O(1); SKU הוא מפתח טבעי וייחודי |
| רשימת קטגוריות | `HashSet<string>` | בלי כפילויות, `Add` מחזיר `false` אם כבר קיים |
| כל הפריטים החוצה | `IEnumerable<StockItem>` (`_items.Values`) | הקורא יכול לעבור/לסנן, לא להוסיף |
| SKU→מחיר לדוח | `ToDictionary` | בונים אינדקס חד-פעמי מתוך שאילתה |

שני האוספים משתמשים ב-`StringComparer.OrdinalIgnoreCase` — כך `"k-100"` ו-`"K-100"` הם אותו מפתח בלי `ToUpper()` בכל מקום.

## Delegates

- `Find(Func<StockItem, bool>)` — במקום לכתוב `FindCheap`, `FindByCategory`, `FindLow`... מתודה אחת מקבלת **את התנאי עצמו** כפרמטר. זו הרעיון מאחורי כל LINQ.
- `ApplyToCategory(string, Action<StockItem>)` — "עשה משהו לכל פריט בקטגוריה". שימו לב ל-`.ToList()` לפני הלולאה: ה-action עלול לשנות את האוסף (למשל `Receive` שמשנה `Quantity` זה בסדר, אבל אם היה מוחק פריט מה-Dictionary היינו מקבלים `InvalidOperationException: Collection was modified`). לקחת "צילום" לפני זה הרגל טוב.
- הרכבת פילטרים: `i => cheap(i) && peripherals(i)` — delegates הם ערכים, אפשר להרכיב אותם.

## אירועים

- `event EventHandler<LowStockEventArgs>? LowStock` — התבנית הסטנדרטית של .NET: `(sender, e)`. `EventArgs` מותאם מאפשר להוסיף נתונים בעתיד בלי לשבור מנויים.
- `LowStock?.Invoke(this, ...)` — ה-`?.` חיוני: בלי מנויים ה-event הוא `null`.
- ה-`Inventory` **לא יודע** מי מאזין — ה-Program מחליט להדפיס, ומנוי שני אוסף רשימת הזמנות. זו הפרדה בין "מה קרה" ל-"מה עושים עם זה".

## LINQ

- `ByCategory`: `GroupBy` → `Select` ל-record → `OrderByDescending`. שלוש שורות שמחליפות 20 שורות של לולאות ו-Dictionary ידני.
- `Reports` היא `static class` של **פונקציות טהורות**: מקבלות `IEnumerable`, מחזירות תוצאה, לא משנות מצב. קל לבדוק, קל להרכיב.
- **Deferred execution** — הדגמה בסוף ה-Program: `lowQuery` הוגדר לפני המכירה של המסכים, אבל בזמן ההדפסה כבר כולל אותם. אם רוצים "צילום" — `ToList()`.

## בדיקה

<div dir="ltr">

```bash
dotnet run
```

</div>

צפוי: אירועי LOW STOCK ל-Keyboard (4), Mouse (2), USB-C Cable (5); דחייה של מכירת 999 ושל SKU לא קיים; אחרי restock הכבלים: USB-C 25, HDMI 28; ערך כולל 11,359.30 אחרי ה-restock; Top 3: Monitor, HDMI Cable, Mouse Pad; ובסוף המסך מופיע ב-Low stock עם כמות 1.

</div>
