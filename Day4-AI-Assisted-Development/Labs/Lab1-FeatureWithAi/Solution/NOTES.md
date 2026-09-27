<div dir="rtl">

# Lab 1 — הערות על הפתרון

## החלטות עיצוב
- **כלל = מחלקה** (`IDiscountRule` עם `Name` ו-`Rate`). המנוע בוחר את האחוז הגבוה ביותר; קופון הוא סכום קבוע ולכן לא `IDiscountRule` אלא שלב נפרד. פתוח להרחבה (בונוס: `SeasonalRule` בבדיקות) בלי לשנות את המנוע.
- **בנאי ללא פרמטרים** רושם את שלושת הכללים של המפרט — כדי שהבדיקות המקוריות (`new DiscountEngine()`) יעבדו; בנאי עם `IEnumerable<IDiscountRule>` מתאים ל-DI.
- **עיגול פעם אחת בסוף** (`Math.Round(amount, 2)`), אחרי ההגבלה ל-Subtotal.
- `decimal` בלבד. הבדיקות משתמשות ב-`double` ב-`InlineData` (מגבלת attributes) וממירות — ערכים "עגולים" ולכן בטוח.

## טעויות AI טיפוסיות שראינו (ואיך תיקנו)
| טעות | סימן | תיקון |
|------|------|-------|
| צבירת הנחות (5% + 8%) | `Calculate_RulesDoNotStack_HighestWins` נכשל | בחירת מקסימום במקום סכימה |
| קופון כאחוז (10%) | `Calculate_Coupon_AddsFixed10` מחזיר 8 במקום 10 | סכום קבוע 10 |
| השוואת קופון רגישה לאותיות (`==`) | `welcome10` נכשל | `StringComparison.OrdinalIgnoreCase` |
| עיגול בכל שלב | 12.00 יוצא 11.99 במקרה ה-VIP | עיגול פעם אחת |
| `double` במקום `decimal` | הבדלי סנטים | decimal בכל מקום |
| `AppliedRule` "Coupon" בלבד כשיש גם VIP | `Vip+Coupon` נכשל | שרשור שם הכלל + "+Coupon" |
| הוספת `ILogger` ו-`DateTime.Now` שלא ביקשנו | קוד עודף | הוסר — פונקציה טהורה |
| `order.Lines` יכול להיות null לדעת ה-AI | `?.` בכל מקום | ה-record לא מאפשר null; הוסר רעש |

## איך לאמת
<div dir="ltr">

```bash
cd Solution/Day4.Lab1.Solution.Tests
dotnet test
```

</div>

</div>
