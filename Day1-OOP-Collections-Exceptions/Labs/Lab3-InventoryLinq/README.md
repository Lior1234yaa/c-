<div dir="rtl">

# Lab 3 — מלאי: Dictionary, HashSet, LINQ, delegates ואירועים (60 דקות)

## מטרה

לבנות מודול ניהול מלאי קטן שמשתמש בכלים "הנכונים" לכל צורך: `Dictionary` לחיפוש לפי SKU, `HashSet` לקטגוריות, `Func`/`Action` כפרמטרים כדי לכתוב מתודה אחת גמישה במקום עשר, אירוע `LowStock` שמפריד בין "מה קרה" ל-"מי מגיב", ודוחות LINQ שהם פונקציות טהורות.

## מה צריך לדעת לפני

מודולים 04 ו-05. Lab 1–2 מומלצים.

## התחלה

<div dir="ltr">

```bash
cd Labs/Lab3-InventoryLinq/Starter
dotnet run
```

</div>

`Models.cs` (record `Product`, `StockItem`, `LowStockEventArgs`) ו-`Program.cs` כבר כתובים. עבודתכם ב-`Inventory.cs` ו-`Reports.cs`.

## שלבים

### שלב 1 — `Inventory` (25 דק')

1. **TODO 1** — שדות: `Dictionary<string, StockItem>` ו-`HashSet<string>`, שניהם עם `StringComparer.OrdinalIgnoreCase`.
2. **TODO 2** — `public event EventHandler<LowStockEventArgs>? LowStock;`
3. **TODO 3** — `Items` ו-`Categories` (חשיפה לקריאה בלבד).
4. **TODO 4–6** — `Add` (SKU כפול → חריגה), `Get` (לא קיים → `KeyNotFoundException`), `Receive`.
5. **TODO 7** — `Sell`: בדיקת כמות, הפחתה, ואם `IsLow` — הפעלת האירוע. אל תשכחו `?.Invoke`.
6. **TODO 8** — `Find(Func<StockItem, bool>)` — שורה אחת עם `Where`.
7. **TODO 9** — `ApplyToCategory(string, Action<StockItem>)`.

### שלב 2 — `Reports` (20 דק')

8. **TODO 10** — `TotalValue` (`Sum`).
9. **TODO 11** — `ByCategory` — `GroupBy` + `Select` ל-`CategorySummary` + `OrderByDescending`.
10. **TODO 12** — `TopByValue` (`OrderByDescending` + `Take`).
11. **TODO 13** — `LowStock` (`Where` + `OrderBy` + `ThenBy`).
12. **TODO 14** — `PriceIndex` (`ToDictionary`).

### שלב 3 — חיבור (15 דק')

13. **TODO 16** — ב-`Program.cs`: הירשמו ל-`LowStock` עם lambda שמדפיסה אזהרה. הריצו ובדקו שהאזהרות מופיעות בזמן המכירות.

## קריטריוני קבלה

- [ ] `inv.Sell("k-100", 1)` עובד גם באותיות קטנות.
- [ ] מכירה של Keyboard 8 יחידות (מ-12 ל-4, סף 5) מדפיסה `!! LOW STOCK: Keyboard (4 left, reorder at 5)`.
- [ ] מכירה של יותר ממה שיש זורקת `InvalidOperationException` עם הודעה שכוללת את הכמות הזמינה — והתוכנית ממשיכה.
- [ ] `Find(i => i.Product.Price < 50)` מחזיר בדיוק את שלושת הפריטים הזולים (Cable ×2, Mouse Pad).
- [ ] `ByCategory` ממוין לפי ערך יורד, ו-`Displays` ראשון.
- [ ] `TopByValue(items, 3)` מחזיר 3 פריטים.
- [ ] `Reports` לא משנה שום דבר במלאי (אין `set` / `Add` / `Remove` בתוכה).
- [ ] אין `ToUpper()`/`ToLower()` בקוד — ההשוואה נעשית דרך `StringComparer`.

## בונוס

- **TODO 15** — `SkusInBoth(a, b)` עם `HashSet.IntersectWith`.
- מנוי שני לאירוע שאוסף רשימת הזמנות (`List<string>`), והדפסתה בסוף.
- הדגימו deferred execution: הגדירו `var low = Reports.LowStock(inv.Items);` **לפני** מכירה, הדפיסו **אחרי** — מה קורה? ומה אם מוסיפים `.ToList()`?
- כתבו אחד מהדוחות ב-query syntax (`from ... where ... select`).

## רמזים

- `Dictionary.TryGetValue(key, out var item)` — חיפוש בלי חריגה.
- `new Dictionary<string, StockItem>(StringComparer.OrdinalIgnoreCase)` — מפתחות לא תלויי רישיות.
- `LowStock?.Invoke(this, new LowStockEventArgs(item));`
- `items.GroupBy(i => i.Product.Category).Select(g => new CategorySummary(g.Key, g.Count(), g.Sum(...), g.Sum(...)))`.
- ב-`ApplyToCategory`, קראו ל-`.ToList()` לפני ה-`foreach` — הסבר ב-NOTES של הפתרון.

</div>
