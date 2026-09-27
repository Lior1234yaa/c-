# Lab 2 — הערות לפתרון

## חלק א' — צורות

- `Shape` אבסטרקטית: אין דבר כזה "סתם צורה", ולכן אסור ליצור `new Shape()`. המחלקה מגדירה **חוזה** (`Area`, `Perimeter`, `Name`) ומספקת מימוש משותף אחד — `ToString` — שמשתמש בחוזה. זהו ה-Template Method הפשוט ביותר.
- `Square : Rectangle` עם primary constructor שמעביר `side, side` ל-base. סימנו `sealed` כי אין טעם לרשת מריבוע, וזה גם מאפשר למהדר אופטימיזציות קטנות.
- `Triangle` מוודא את אי-שוויון המשולש **בבנאי** — אובייקט שנוצר הוא תמיד תקין. זו אותה תבנית שראינו ב-Lab 1.

## חלק ב' — עובדים

- **`IPayable` נפרד מ-`Employee`.** לקבלן משלמים, אבל הוא לא עובד: אין לו Id, אין לו מנהל, ולא הגיוני שיירש מ-`Employee`. הממשק מאפשר ל-`Payroll` לעבוד עם `List<IPayable>` בלי לדעת מי בפנים — זו התועלת של הפשטה: **הקוד תלוי בחוזה, לא במימוש**.
- **default interface member (`PaySlip`)** — פורמט אחיד לכולם בלי לשכפל קוד. שימו לב שהוא נגיש רק דרך הטיפוס `IPayable`, לא דרך `SalariedEmployee` ישירות.
- **`Manager : SalariedEmployee`** — מנהל *הוא* עובד עם משכורת קבועה, פלוס בונוס. `base.CalculateMonthlyPay() + Bonus` מראה איך להרחיב התנהגות במקום להעתיק אותה.
- **`HourlyEmployee`** — הקבועים `RegularHours` ו-`OvertimeFactor` במקום "מספרי קסם" (מודול 7).
- **`Payroll.Employees => _payees.OfType<Employee>()`** — סינון לפי טיפוס ב-LINQ. הקבלן פשוט לא ייכלל.
- **pattern matching ב-`switch`** בסוף ה-Program — הסדר חשוב! `Manager` לפני `Employee`, אחרת ה-case הכללי "יבלע" את המנהל (המהדר אפילו יזהיר על case שלא ניתן להגיע אליו).

## מה לא עשינו בכוונה

- לא הוספנו `Employee.Salary` set ציבורי — המשכורת נקבעת בבנאי. אם רוצים העלאה, מוסיפים מתודה `GiveRaise(percent)` שמוודאת ערכים.
- לא יצרנו `enum EmployeeType` ו-`switch` בתוך `CalculateMonthlyPay` — זה בדיוק מה שפולימורפיזם מחליף. הוספת סוג עובד חדש = מחלקה חדשה, בלי לגעת בקוד קיים (Open/Closed).

## בדיקה

```bash
dotnet run
```

צפוי: 4 צורות עם שטחים (Circle 3.14, Rectangle 6, Triangle 6, Square 4; סה"כ 19.14), שגיאה על Triangle(1,1,10), טבלת משכורות: Dana 20,000, Yossi 14,000 (160×80 + 10×120), Noa 32,000, Acme 12,000, סה"כ 78,000.
