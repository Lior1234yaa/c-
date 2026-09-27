<div dir="rtl">

# Lab 2 — הערות על הפתרון

## המבנה
<div dir="ltr">

```
Domain/Subscription.cs          Subscription, Bill, enums, קבועים
Services/Interfaces.cs          ISubscriptionSource, IPricingService
Services/PricingService.cs      כל כלל = מתודה static קטנה עם קבועים בשם (ניתנות לבדיקה בנפרד)
Services/ReportFormatter.cs     כל העיצוב במקום אחד
Services/BillingReport.cs       ה-use case; כותב ל-TextWriter (בדיקה = StringWriter)
Infrastructure/...CsvSource.cs  מקור נתונים + parser עם InvariantCulture וזריקת FormatException
Program.cs                      composition root: ServiceCollection בלבד
```

</div>

## החלטות
- **Golden master כבדיקה**: `expected-output.txt` מקושר (`Link`) מה-Starter כדי שלא יהיו שני עותקים.
- **decimal במקום double**: הפלט נשאר זהה כי הגרסה הישנה עיצבה ב-`0.00` — אבל שימו לב: `double` היה יכול לתת סנט אחר במקרים אחרים. זה שיפור אמיתי שנעשה בזהירות (diff אחרי כל צעד).
- `Bill.Total` מחושב (record) במקום שדה — אין מצב לא עקבי.
- `TextWriter` במקום `Console` ישירות — שינוי קטן שהופך הכול לבדיק.
- `TreatWarningsAsErrors` בפרויקט המרופקטר.

## טעויות AI טיפוסיות שראינו
| טעות | איך זיהינו | תיקון |
|------|-----------|-------|
| עיגול `Math.Round` בכל שלב "כדי להיות מדויק" | golden master נשבר (סנט) | עיגול רק בעיצוב, כמו במקור |
| שינוי סדר החישוב (`amount / 30 * days`) | הבדלי סנטים ב-partial | `amount * days / 30` כמו במקור |
| `ToString("N2")` "כי זה יפה יותר" | פסיקי אלפים בפלט | `"0.00"` + InvariantCulture |
| מיון תוכניות לפי enum במקום לפי שם | סדר שונה (Basic/Pro/Team לעומת BASIC/PRO/TEAM זהה במקרה; אבל לא מובטח) | מיון לפי `PlanCode` Ordinal |
| הוספת `ILogger` + `Microsoft.Extensions.Hosting` | תלויות מיותרות | רק `DependencyInjection` |
| `catch (Exception)` סביב ה-parse "ליציבות" | שורה פגומה נעלמת בשקט | זריקת `FormatException` עם השורה |
| ממשק לכל מחלקה כולל `IReportFormatter` | ממשק בלי מימוש שני | הושאר כמחלקה; ממשקים רק למה שמוחלף (source, pricing) |

## אימות
<div dir="ltr">

```bash
cd Solution/Day4.Lab2.Solution && dotnet run > actual.txt && diff ../../Starter/Day4.Lab2.Starter/expected-output.txt actual.txt
cd ../Day4.Lab2.Solution.Tests && dotnet test
```

</div>

</div>
