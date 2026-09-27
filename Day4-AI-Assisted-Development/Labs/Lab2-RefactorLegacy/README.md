<div dir="rtl">

# Lab 2 — Refactoring של קוד legacy בעזרת AI, עם Golden Master (75 דקות)

## המטרה
לקחת אפליקציית קונסול "עובדת אבל מבולגנת" (`SubscriptionBiller` — חיוב חודשי למנויים) ולהפוך אותה, בעזרת AI, למבנה נקי: services עם ממשקים, DI, `decimal`, ובדיקות — **בלי לשנות את הפלט אפילו בתו אחד**. קובץ `expected-output.txt` הוא ה-Golden Master.

## דרישות מקדימות
- Lab 1 הושלם (עבודה מול בדיקות).
- כלי AI עם יכולת עריכה רב-קבצית מומלץ (Copilot agent / Claude Code / Cursor), אבל אפשר גם צ'אט רגיל.

## הקוד ההתחלתי
`Starter/Day4.Lab2.Starter/SubscriptionBiller.cs` — כ-230 שורות: מתודה אחת, מספרי קסם, כפילויות, `double` לכסף, אפס טיפול בשגיאות. `dotnet run` מדפיס דוח; `expected-output.txt` מכיל בדיוק את הפלט הזה.

## שלבים

### שלב 0 — רשת ביטחון (10 דק')
1. הריצו: `dotnet run > actual.txt` והשוו: `diff expected-output.txt actual.txt` (ב-Windows: `fc` או `Compare-Object`). חייב להיות זהה.
2. צרו commit (או העתק) של המצב ההתחלתי.
3. כתבו **בדיקת golden master** ראשונה (בפרויקט tests חדש או בסוף Program): מריצה את הביל ומשווה ל-`expected-output.txt`. זו הבדיקה שתלווה אתכם.

### שלב 1 — ניתוח עם AI (10 דק')
Prompt:
<div dir="ltr">

```text
Analyze this C# class. List the code smells ranked by risk (god method, magic numbers, duplicated formatting,
double for money, no error handling, mixed responsibilities). Propose a target structure: which classes/interfaces,
what each is responsible for. Do NOT write code yet.
{הדביקו SubscriptionBiller.cs}
```

</div>
**דף עבודה:** רשמו את המבנה שה-AI הציע ומה **אתם** הייתם משנים בו (למשל: האם באמת צריך ממשק ל-formatter?).

### שלב 2 — Refactoring בצעדים קטנים (35 דק')
כלל: אחרי **כל** צעד — `dotnet run` + diff מול golden master (או הבדיקה). צעדים מומלצים, כל אחד prompt נפרד:
1. "Extract the pricing rules (plan base price, add-ons, proration, discount, tax) into small private methods with named constants. Keep output identical."
2. "Introduce a `Subscription` record and a parser `CsvSubscriptionParser` with `CultureInfo.InvariantCulture`. Keep output identical."
3. "Replace `double` with `decimal`. Verify formatting stays identical (0.00 with InvariantCulture)."
4. "Move pricing into `IPricingService` / `PricingService`, formatting into `ReportFormatter`, the loop into `BillingReport`. Wire with `Microsoft.Extensions.DependencyInjection` in Program.cs."
5. "Add xUnit tests for `PricingService` (each rule) and a golden-master test for the whole report."

### שלב 3 — סקירה (10 דק')
- [ ] הפלט זהה (golden master ירוק).
- [ ] אין `double` לכסף; parsing עם `InvariantCulture`.
- [ ] כל מספר קסם הפך לקבוע עם שם.
- [ ] `Program.cs` רק מחבר (composition root); הלוגיקה בשירותים.
- [ ] שגיאות קלט (שורה פגומה) נזרקות עם הודעה ברורה — לא נבלעות.
- [ ] הבדיקות בודקות **כללים** ולא רק את הפלט הכולל.

### שלב 4 — סקירה שנייה ע"י AI (10 דק')
תבנית 13 ב-`Demos/Prompts`. השוו את הממצאים לצ'ק-ליסט שלכם. תקנו מה שמוצדק.

## קריטריוני קבלה
- [ ] `dotnet run` בפרויקט המרופקטר מדפיס פלט **זהה** ל-`expected-output.txt`.
- [ ] יש לפחות 3 מחלקות שירות עם ממשקים ורישום DI.
- [ ] יש ≥ 8 בדיקות xUnit עוברות, כולל golden master.
- [ ] דף העבודה מלא: מה ה-AI הציע, מה שיניתם ולמה.

## בונוס
- החליפו את מקור הנתונים בקובץ CSV חיצוני (`IFileSystem`/`Path`) בלי לשנות את `BillingReport`.
- הוסיפו `ILogger` ורשמו אזהרה על מנוי עם הנחה מעל 50%.

## רמזים
- אם ה-diff נשבר אחרי מעבר ל-`decimal`: בדקו את סדר הפעולות (הכפלה לפני חלוקה) והעיגול — הגרסה הישנה לא עיגלה בכלל, רק עיצבה ב-`0.00`.
- `dotnet run > actual.txt` ב-PowerShell עשוי לכתוב UTF-16 — השוו תוכן, לא bytes, או השתמשו ב-`| Out-File -Encoding utf8`.

</div>
