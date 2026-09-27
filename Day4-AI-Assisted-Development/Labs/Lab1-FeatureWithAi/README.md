# Lab 1 — פיצ'ר ממפרט בעזרת AI, בדיקות קודם (60 דקות)

## המטרה
לממש **מנוע כללי הנחה** (`IDiscountEngine`) להזמנות לפי מפרט, בעזרת כלי AI, כשהבדיקות (xUnit) כבר קיימות ונכשלות. תתרגלו: prompt עם הקשר, עבודה מול בדיקות, קריאה ביקורתית של הפלט, ו-refactoring קצר.

## דרישות מקדימות
- .NET 10 SDK, כלי AI אחד לפחות (Copilot / Claude / Cursor / ChatGPT).
- `Starter/` מתקמפל; `dotnet test` **נכשל** (זה המצב ההתחלתי).

## המפרט (זה מה שתעבירו ל-AI)
מודל: `Order(CustomerType Type, IReadOnlyList<OrderLine> Lines, string? CouponCode)`, `OrderLine(string Sku, int Quantity, decimal UnitPrice)`, `Subtotal = Σ Quantity × UnitPrice`.

כללים (ההנחה הגבוהה ביותר מנצחת; **לא** מצטברים, למעט קופון):
1. **Volume**: 10–49 פריטים בסך הכול → 5%; 50 ומעלה → 10%.
2. **Amount**: Subtotal מעל 1,000 → 8%.
3. **VIP**: לקוח `Vip` → 12% תמיד.
4. **Coupon**: `"WELCOME10"` → 10 ₪ קבועים **בנוסף** להנחת האחוזים, אבל רק אם Subtotal ≥ 50; קוד לא מוכר → מתעלמים; השוואת קוד לא רגישה לאותיות.
5. ההנחה הכוללת לעולם לא עולה על ה-Subtotal (סה"כ לא שלילי).
6. הזמנה ריקה → הנחה 0. עיגול ל-2 ספרות (`MidpointRounding.ToEven` — ברירת המחדל של `Math.Round`).
7. הפלט: `DiscountResult(decimal Amount, string AppliedRule)` כאשר `AppliedRule` הוא `"None"`, `"Volume"`, `"Amount"`, `"Vip"` — ואם קופון הופעל מוסיפים `"+Coupon"` (למשל `"Vip+Coupon"`).

## שלבים

### שלב 1 — הכירו את הבדיקות (10 דק')
פתחו `Starter/Day4.Lab1.Starter.Tests/DiscountEngineTests.cs`. הבינו כל בדיקה. הריצו `dotnet test` וראו שהן נכשלות ב-`NotImplementedException`.

### שלב 2 — Prompt ראשון (15 דק')
**דף עבודה — מלאו לפני שאתם שולחים:**

| מרכיב | מה תכתבו |
|-------|----------|
| Role | |
| Context (stack, קבצים שתדביקו) | |
| Task | |
| Constraints (בלי חבילות, בלי static, decimal…) | |
| Examples (2–3 מהבדיקות) | |
| Output format | |

Prompt לדוגמה:

```text
You are a senior C# developer. .NET 10, nullable enabled, file-scoped namespaces.
Implement `DiscountEngine : IDiscountEngine` (interface and models below) so that the attached xUnit tests pass.
Rules: {העתיקו את המפרט}
Constraints: pure function (no I/O, no static state), decimal math, no new packages, keep the interface unchanged,
  each rule in its own small private method or class implementing IDiscountRule.
Output: the class only, then a 3-line summary. Do not modify the tests.
{הדביקו IDiscountEngine.cs, Models.cs, DiscountEngineTests.cs}
```

### שלב 3 — אימות (10 דק')
`dotnet build` → `dotnet test`. אם בדיקות נכשלות: הדביקו את הודעת הכישלון המלאה ב-prompt המשך ("Test X fails with … Fix minimally").

### שלב 4 — סקירה (15 דק')
עברו על הצ'ק-ליסט:
- [ ] אין `double`/`float`.
- [ ] אין `static` mutable state, אין I/O, אין `DateTime.Now`.
- [ ] השוואת קופון לא רגישה לאותיות (`StringComparison.OrdinalIgnoreCase`), `null` בטוח.
- [ ] כללים מופרדים (מתודה/מחלקה לכל כלל) — לא `if` ענק אחד.
- [ ] אין קוד "מיותר" שלא ביקשתם (לוגים, ממשקים נוספים, חבילות).
- [ ] שמות ברורים; מספרי קסם הפכו לקבועים.
בקשו סקירה שנייה: תבנית 13 ב-`Demos/Prompts/templates-en.md`.

### שלב 5 — הרחבה עם בדיקה חדשה (10 דק')
הוסיפו בדיקה: קופון `"WELCOME10"` על הזמנה של 60 ₪ עם 10 פריטים (Volume 5% = 3 ₪) → סה"כ הנחה 13 ₪, `"Volume+Coupon"`. אם היא נכשלת — תקנו (בעצמכם או עם AI) והסבירו מה היה חסר.

## קריטריוני קבלה
- [ ] `dotnet test` ב-`Starter` עובר (כל הבדיקות ירוקות) אחרי המימוש שלכם.
- [ ] `DiscountEngine` עומד בצ'ק-ליסט הסקירה.
- [ ] דף העבודה מלא; אתם יכולים להסביר כל שורה בקוד.
- [ ] הבדיקה מהשלב 5 נוספה ועוברת.

## בונוס
- הפכו כל כלל למחלקה `IDiscountRule` ורשמו אותן ברשימה — הוסיפו כלל "Seasonal 3%" בלי לגעת ב-`DiscountEngine`.
- בקשו מה-AI לייצר `[Theory]` נוספת עם 10 מקרים אקראיים ו**בדקו ידנית** שלושה מהם.

## רמזים
- ה-AI נוטה לצבור הנחות (5% + 8%) — המפרט אומר "הגבוהה מנצחת".
- "10 ₪ בנוסף" הוא סכום קבוע, לא אחוז.
- עיגול פעם אחת בסוף, לא בכל שלב.
