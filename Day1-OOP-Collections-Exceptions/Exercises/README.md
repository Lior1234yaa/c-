# תרגילים — יום 1

תרגילים קצרים (5–15 דקות כל אחד), מקובצים לפי מודול. דרגת קושי: ★ קל, ★★ בינוני, ★★★ מאתגר.
הפתרונות בפרויקט `Solutions/` — הרצה של תרגיל בודד:

```bash
cd Exercises/Solutions
dotnet run -- 3        # מריץ את תרגיל 3
dotnet run             # תפריט
```

מומלץ לפתור כל תרגיל בפרויקט חדש משלכם (`dotnet new console -n Ex03`) ורק אז להשוות לפתרון.

## מודול 01 — חימום C#

### תרגיל 1 ★ — ממיר טמפרטורות
כתבו שתי מתודות: `CelsiusToFahrenheit(double c)` ו-`FahrenheitToCelsius(double f)` (expression-bodied). הדפיסו טבלה מ-−10 עד 40 מעלות צלזיוס בקפיצות של 10, עם אינטרפולציה ופורמט של ספרה אחת אחרי הנקודה (`{x:F1}`).

### תרגיל 2 ★ — FizzBuzz עם switch expression
לכל מספר מ-1 עד 20: "FizzBuzz" אם מתחלק ב-15, "Fizz" ב-3, "Buzz" ב-5, אחרת המספר. השתמשו ב-`switch` expression עם tuple pattern: `(n % 3, n % 5) switch { (0, 0) => ..., (0, _) => ... }`.

## מודול 02 — מחלקות ואובייקטים

### תרגיל 3 ★ — מחלקת `Rectangle`
מחלקה עם properties `Width`, `Height` (קריאה בלבד, נקבעות בבנאי שמוודא ערכים חיוביים), property מחושב `Area`, מתודה `Scale(double factor)` שמחזירה מלבן **חדש**, ו-`ToString()`. צרו שני מלבנים והדפיסו.

### תרגיל 4 ★★ — `record` מול `class` ו-static counter
א. הגדירו `record Point(int X, int Y)` ו-`class PointClass` עם אותם שדות. צרו שני מופעים שווים מכל אחד והדפיסו `==` ו-`Equals`. הסבירו את ההבדל בהערה.
ב. הוסיפו ל-`PointClass` מונה סטטי `Created` שגדל בכל בנאי, ומתודה סטטית `Origin()` שמחזירה (0,0).
ג. השתמשו ב-`with` כדי ליצור נקודה מוזזת מה-record.

## מודול 03 — עקרונות OOP

### תרגיל 5 ★★ — כלי רכב
מחלקה אבסטרקטית `Vehicle` (`Name`, `Wheels` אבסטרקטי, `virtual Describe()`), יורשים `Car`, `Motorcycle`, `Truck` (`LoadCapacity`). ממשק `IElectric { int BatteryKwh { get; } }` ש-`Car` אחד מממש (`ElectricCar : Car, IElectric`). עברו על `List<Vehicle>` והדפיסו `Describe()`; ואז הדפיסו רק את החשמליים (`OfType<IElectric>()`).

### תרגיל 6 ★★ — pattern matching
מתודה `string Classify(object o)` שמחזירה תיאור ל: `int` שלילי / `int` חיובי / `string` ריק / `string` אחר / `double` / `int[]` עם 0 איברים / `int[]` אחר / `null` / כל דבר אחר. השתמשו ב-`switch` expression עם type patterns, property patterns (`{ Length: 0 }`) ו-`when`. הריצו על מערך `object[]` מעורב.

## מודול 04 — אוספים וגנריקה

### תרגיל 7 ★ — ספירת מילים
קבלו משפט (קבוע בקוד), פצלו למילים (`Split`), וספרו כמה פעמים כל מילה מופיעה ב-`Dictionary<string, int>` (לא תלוי רישיות). הדפיסו ממוין לפי שכיחות יורדת ואז אלפביתי.

### תרגיל 8 ★★ — `MyStack<T>` גנרי
מימוש מחסנית על גבי מערך פנימי: `Push`, `Pop` (זורק `InvalidOperationException` כשריק), `Peek`, `Count`, `IsEmpty`. המערך גדל פי 2 כשמתמלא. הוסיפו constraint `where T : notnull`. בדקו עם `int` ועם `string`.

### תרגיל 9 ★★ — האוסף הנכון
לכל תרחיש בחרו אוסף ונמקו בהערה, ואז ממשו: (א) מזהי מבקרים ייחודיים מתוך רשימה עם כפילויות — כמה ייחודיים? (ב) תור הדפסה — מסמכים מטופלים לפי סדר הגעה. (ג) היסטוריית "Undo". (ד) חיפוש מהיר של סטודנט לפי ת"ז. (ה) רשימה של ציונים לצורך ממוצע.

## מודול 05 — Delegates, Lambdas, LINQ

### תרגיל 10 ★ — LINQ על מספרים
עבור `int[] nums = [5, 3, 8, 1, 9, 2, 7, 4, 6, 10]`: סכום ריבועי הזוגיים, שלושת הגדולים, האם יש מספר > 9, ממוצע האי-זוגיים, ומחרוזת של המספרים ממוינים (`string.Join`). כל תשובה בשורת LINQ אחת.

### תרגיל 11 ★★ — delegates ואירוע
א. מתודה `Func<int, int> Compose(Func<int, int> f, Func<int, int> g)` שמחזירה `x => g(f(x))`. בדקו עם `x + 1` ו-`x * 2`.
ב. מחלקה `Counter` עם `Increment()` ואירוע `ThresholdReached` (`EventHandler<int>`) שמופעל כשהמונה מגיע ל-`Threshold`. הירשמו עם lambda והדפיסו.

## מודול 06 — חריגות ודיבוג

### תרגיל 12 ★★ — קלט בטוח
מתודה `int ReadInt(string prompt, int min, int max)` שמבקשת מהמשתמש מספר בלולאה עד שהקלט תקין (`TryParse` + טווח). על EOF (`ReadLine()` מחזיר `null`) — זרקו `EndOfStreamException`. הגדירו `class ValidationException : Exception` עם property `Input`, וזרקו אותה מ-`Validate(int age)` אם הגיל לא בין 0 ל-120. תפסו ב-`Main` והדפיסו הודעה ידידותית.

### תרגיל 13 ★★★ — סדר הריצה
מחלקה `Step(string name) : IDisposable` שמדפיסה בבנאי ובדיספוז. כתבו מתודה שמכילה: `using var a = new Step("A");` ואז `try { using var b = new Step("B"); throw new InvalidOperationException(); } catch { Console.WriteLine("catch"); } finally { Console.WriteLine("finally"); }`. **לפני** ההרצה, כתבו בהערה מה סדר ההדפסות הצפוי. הריצו ובדקו. הוסיפו `catch (Exception ex) when (Log(ex))` עם `Log` שמדפיסה ומחזירה `false` — מתי היא רצה?

## מודול 07 — קוד נקי

### תרגיל 14 ★★ — refactoring
נתונה המתודה הבאה. שכתבו אותה: שמות ברורים, קבועים במקום מספרי קסם, `enum` לסוג הלקוח, חילוץ מתודות קטנות, והסרת כפילות. התנהגות זהה.

```csharp
static double c(double a, int t, bool m)
{
    double r = 0;
    if (t == 1) { r = a * 0.9; if (m) r = r - 5; }
    else if (t == 2) { r = a * 0.8; if (m) r = r - 5; }
    else { r = a; if (m) r = r - 5; }
    if (r < 0) r = 0;
    return r * 1.18;
}
```
