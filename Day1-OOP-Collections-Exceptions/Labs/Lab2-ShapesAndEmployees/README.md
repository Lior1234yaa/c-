<div dir="rtl">

# Lab 2 — צורות ועובדים: ירושה, הפשטה ופולימורפיזם (60 דקות)

## מטרה

לבנות שתי היררכיות קטנות ולהרגיש בידיים את ארבעת המושגים: מחלקה אבסטרקטית שמגדירה חוזה, ירושה שמרחיבה התנהגות (`base`), ממשק שמאפשר לטפל באובייקטים שונים באופן אחיד, ופולימורפיזם — קריאה אחת, התנהגויות שונות לפי הטיפוס האמיתי.

## מה צריך לדעת לפני

מודולים 02 ו-03. Lab 1 מומלץ.

## התחלה

<div dir="ltr">

```bash
cd Labs/Lab2-ShapesAndEmployees/Starter
dotnet run
```

</div>

## שלבים

### חלק א' — צורות (15 דק')

1. **TODO 1** — הפכו את `Shape` ל-`abstract` עם `Name`, `Area()`, `Perimeter()` אבסטרקטיים ו-`ToString()` משותף.
2. **TODO 2** — `Circle`, `Rectangle`, `Triangle`. במשולש, ודאו בבנאי שהצלעות תקינות (אי-שוויון המשולש) — אחרת `ArgumentException`.
3. **TODO 3** — `Square : Rectangle`, מסומן `sealed`. שימו לב כמה מעט קוד צריך.
4. **TODO 11** — ב-`Program.cs`: רשימת `List<Shape>`, הדפסה, סכום שטחים, וניסיון ליצור משולש לא חוקי בתוך `try/catch`.

### חלק ב' — משכורות (45 דק')

5. **TODO 4** — ממשק `IPayable` עם `PayeeName`, `CalculateMonthlyPay()`, ו-default member `PaySlip()`.
6. **TODO 5** — `Employee` אבסטרקטית שמממשת `IPayable`. `CalculateMonthlyPay` נשאר אבסטרקטי, `Describe()` הוא `virtual`.
7. **TODO 6** — `SalariedEmployee`: שנתי / 12.
8. **TODO 7** — `HourlyEmployee`: 160 שעות רגילות, מעבר לזה פי 1.5. דרסו את `Describe()` והשתמשו ב-`base.Describe()`.
9. **TODO 8** — `Manager : SalariedEmployee` עם `Bonus` ו-`Reports`. `CalculateMonthlyPay` = `base` + בונוס.
10. **TODO 9** — `Contractor` שמממש `IPayable` **בלי** לרשת מ-`Employee`. שאלו את עצמכם: למה?
11. **TODO 10** — `Payroll` עם `List<IPayable>`: `Add`, `TotalMonthly`, `PrintSlips`.
12. **TODO 12** — ב-`Program.cs`: צרו את העובדים והקבלן, הדפיסו תלושים, ואז הדפיסו `Describe()` רק לעובדים (`is Employee e`).

## קריטריוני קבלה

- [ ] `new Shape()` לא מתקמפל (המחלקה אבסטרקטית).
- [ ] `Triangle(1, 1, 10)` זורק `ArgumentException`; `Triangle(3, 4, 5)` נותן שטח 6.
- [ ] `Square(2)` מדפיס `Square: area=4.00, perimeter=8.00` — בלי לכתוב `Area()` מחדש ב-Square.
- [ ] `Yossi` (80 ₪/שעה, 170 שעות) מקבל 14,000 (160×80 + 10×80×1.5).
- [ ] `Noa` (360,000 שנתי + 2,000 בונוס) מקבלת 32,000.
- [ ] `Payroll` מקבל גם `Contractor` וגם `Employee` באותה רשימה, והסכום הכולל נכון (78,000 עם הנתונים שבדוגמה).
- [ ] `PaySlip()` לא מומש באף מחלקה — רק בממשק.
- [ ] בלולאת `Describe()` הקבלן לא מופיע.

## בונוס

- `switch` עם pattern matching שמסווג כל `IPayable` ל: "senior manager" (יותר מכפוף אחד), "manager", "hourly with overtime", "regular employee", "external". שימו לב לסדר ה-cases.
- הוסיפו `interface IDrawable { void Draw(); }` ומימוש ASCII ל-`Rectangle` ו-`Square` בלבד — צורה יכולה לממש כמה ממשקים.
- `Payroll.Employees` — property שמחזיר רק את העובדים (`OfType<Employee>()`).

## רמזים

- primary constructor שמעביר ל-base: `class Square(double side) : Rectangle(side, side)`.
- default member בממשק: `string PaySlip() => $"...";` — פשוט גוף בתוך הממשק.
- כדי לקרוא ל-`PaySlip()` על `SalariedEmployee` צריך משתנה מטיפוס `IPayable`.
- `Math.Min` / `Math.Max` עוזרים לחשב שעות רגילות/נוספות בלי `if`.

</div>
