<div dir="rtl">

# מדריך למרצה — יום 1: OOP, אוספים וטיפול בחריגות

> תיקיית היום: [`../Day1-OOP-Collections-Exceptions/`](../Day1-OOP-Collections-Exceptions/README.md) · מצגת: [`Slides/Day1.pptx`](../Day1-OOP-Collections-Exceptions/Slides/Day1.pptx) · תרגילים: [`Exercises/README.md`](../Day1-OOP-Collections-Exceptions/Exercises/README.md)

## תקציר היום

זה היום שעליו נשען כל הקורס. בבוקר עוברים על השפה עצמה (מודול 01), ואז בונים מחלקות (02) ועקרונות OOP (03). כל אחד מהם נסגר במעבדה: Lab 1 אחרי מודול 02 ו-Lab 2 אחרי מודול 03. אחר הצהריים הקצב מהיר יותר: אוספים וגנריקה (04), delegates/LINQ (05), וחריגות, דיבוג וקוד נקי (06–07). היום נגמר במעבדה אחת לבחירה, Lab 3 או Lab 4, והשנייה הולכת הביתה. בערך מחצית מהיום היא עבודה מעשית: 4 מעבדות ו-14 תרגילים קצרים. 8 מהתרגילים משובצים בלו"ז הכיתתי שלמטה.

**המסר של היום:** מצב של אובייקט משתנה רק דרך המתודות שלו. בוחרים את האוסף לפי הפעולה שחוזרת הכי הרבה. תוכנה טובה נכשלת בצורה מבוקרת.

## מטרות למידה (מתוך README של היום)

בסוף היום המשתתפים יוכלו:

1. לכתוב תוכנית C# מודרנית: top-level statements, `var`, אינטרפולציה, `switch` expressions, nullable reference types.
2. לעצב מחלקות עם properties, בנאים (כולל primary constructors) ו-`record`, ולהסביר מתי כל אחד מתאים.
3. ליישם את ארבעת עקרונות ה-OOP: `private`/`protected`, `virtual`/`override`/`abstract`, ממשקים (כולל default members), ו-`is`/`switch` patterns.
4. לבחור את האוסף הנכון לפי Big-O, לכתוב מחלקה גנרית עם constraints ולחשוף אוספים כ-`IReadOnlyList<T>`.
5. להעביר התנהגות עם `Func`/`Action`, להגדיר אירועים ולכתוב LINQ, כולל הבנה של deferred execution.
6. לטפל בחריגות נכון: `try/catch/finally/when`, חריגות מותאמות, `throw;`, `using`, guard clauses ו-`TryParse`.
7. לדבג ב-Visual Studio עם breakpoints (גם מותנים), Watch, Immediate, Call Stack, Exception Settings ו-Edit & Continue.
8. לזהות ריחות קוד ולתקן אותם (שמות, מספרי קסם, כפילויות) בעזרת SOLID ו-`dotnet format`.

---

## הכנה לפני היום (checklist למרצה)

### יום-יומיים לפני

- [ ] לשלוח למשתתפים את [`00-Setup/INSTALL.md`](../00-Setup/INSTALL.md) ולבקש שיריצו את `00-Setup/VerifySetup` לפני שמגיעים.
- [ ] לקרוא את `Solution/NOTES.md` של ארבע המעבדות. ב-Lab 4 חשוב במיוחד להכיר את טבלת 10 הבאגים, כי תצטרכו לתת רמזים בלי לחשוף אותה.
- [ ] להחליט מראש איזו מעבדה תרוץ בסוף היום, Lab 3 או Lab 4 (ראו את הבלוק של 15:40 בהמשך). אפשר לעדכן את ההחלטה בצהריים לפי קצב הקבוצה.

### בבוקר, לפני 09:00

- [ ] לוודא שהפקודה `dotnet --version` מדפיסה `10.x` במחשב המרצה.
- [ ] להריץ את VerifySetup:

<div dir="ltr">

```powershell
cd C:\c-\00-Setup\VerifySetup
dotnet run
```

</div>

- [ ] **לבנות מראש את כל הפרויקטים של היום.** כך נמנעים מהמתנה ל-restore באמצע דמו:

<div dir="ltr">

```powershell
cd C:\c-\Day1-OOP-Collections-Exceptions
Get-ChildItem -Recurse -Filter *.csproj | ForEach-Object { dotnet build $_.FullName }
```

</div>

- [ ] לפתוח את [`Slides/Day1.pptx`](../Day1-OOP-Collections-Exceptions/Slides/Day1.pptx). המצגת כוללת 55 שקפים. מספרי השקפים במדריך נספרו לפי הסדר ב-`Day1.slides.js`.
- [ ] להשאיר שני טרמינלים פתוחים לאורך כל היום:
  - **טרמינל A (דמואים):** `cd C:\c-\Day1-OOP-Collections-Exceptions\Demos`
  - **טרמינל B (פתרונות תרגילים):** `cd C:\c-\Day1-OOP-Collections-Exceptions\Exercises\Solutions`, שם מריצים `dotnet run -- <n>`.
- [ ] לפתוח ב-Visual Studio את [`Labs/Lab4-BankRobust/Starter`](../Day1-OOP-Collections-Exceptions/Labs/Lab4-BankRobust/Starter/) (קובץ `Day1.Lab4.Starter.csproj`), גם אם Lab 4 הולך הביתה. הוא משמש להדגמת הדיבאגר בבלוק 06. אם יש בתיקייה `transactions.log` מריצה קודמת, מוחקים אותו.
- [ ] להגדיל את גופן העורך והטרמינל (לפחות 16pt). יש פלט בעברית, אז כדאי לוודא שהטרמינל מציג אותו כראוי.

### מה המשתתפים צריכים לקבל

- [ ] את המאגר כולו (clone או zip).
- [ ] הנחיה ל**העתיק כל `Starter` לתיקיית עבודה** ולא לעבוד על המקור (ראו "איך עובדים על מעבדה" ב-[`COURSE-OVERVIEW.md`](../00-Setup/COURSE-OVERVIEW.md)):

<div dir="ltr">

```powershell
Copy-Item -Recurse C:\c-\Day1-OOP-Collections-Exceptions\Labs\Lab1-LibrarySystem\Starter C:\my-work\Day1-Lab1
```

</div>

- [ ] כלל עבודה שכדאי להגיד בקול: "לא מציצים ב-`Solution` לפני שניסיתם ברצינות. קודם הרמזים ב-README, אחר כך השכן, אחר כך המרצה."

---

## לו"ז יומי

ה-README הרשמי קובע את מסגרת 09:00–16:30 ואת ההפסקות (10:30, 12:15–13:00, 14:30). בלו"ז הזה כל בלוק הרצאה מחולק להרצאה, דמו ותרגול, ואליו שובצו תרגילים ספציפיים.

| שעה | סוג | נושא | חומרים |
|---|---|---|---|
| 09:00–09:15 | פתיחה | היכרות, סדר היום, בדיקת סביבה (`dotnet --version`) | שקפים 1–3 |
| 09:15–09:35 | הרצאה | **מודול 01**: חימום C#: `dotnet new`, top-level, ערך/הפניה, מחרוזות, `switch` expression, מתודות, nullable | [Notes/01](../Day1-OOP-Collections-Exceptions/Notes/01-csharp-quickstart.md), שקפים 4–9 |
| 09:35–09:42 | דמו | Quickstart | [Day1.Demo.Quickstart](../Day1-OOP-Collections-Exceptions/Demos/Day1.Demo.Quickstart/Program.cs) |
| 09:42–09:50 | תרגול | **תרגיל 2** ★ FizzBuzz עם switch expression (למי שסיים: **תרגיל 1** ★) | [Exercises](../Day1-OOP-Collections-Exceptions/Exercises/README.md) |
| 09:50–10:10 | הרצאה | **מודול 02**: class/object, properties (auto/init/required/computed), בנאים, primary constructor, static, record | [Notes/02](../Day1-OOP-Collections-Exceptions/Notes/02-classes-objects.md), שקפים 10–15 |
| 10:10–10:20 | דמו | Classes | [Day1.Demo.Classes](../Day1-OOP-Collections-Exceptions/Demos/Day1.Demo.Classes/Program.cs) |
| 10:20–10:30 | תרגול | **תרגיל 3** ★ מחלקת `Rectangle` (למי שסיים: **תרגיל 4** ★★) | [Exercises](../Day1-OOP-Collections-Exceptions/Exercises/README.md) |
| 10:30–10:45 | הפסקה | | |
| 10:45–11:25 | מעבדה | **Lab 1**: מערכת ספרייה | [Lab1 README](../Day1-OOP-Collections-Exceptions/Labs/Lab1-LibrarySystem/README.md), שקף 16 |
| 11:25–11:30 | סיכום | דיון על Lab 1: למה `private set`, ומתי זורקים חריגה ומתי מחזירים `false` | [Lab1 NOTES](../Day1-OOP-Collections-Exceptions/Labs/Lab1-LibrarySystem/Solution/NOTES.md) |
| 11:30–11:55 | הרצאה | **מודול 03**: אנקפסולציה, ירושה, abstract, פולימורפיזם, ממשקים, composition, patterns, Equals | [Notes/03](../Day1-OOP-Collections-Exceptions/Notes/03-oop-pillars.md), שקפים 17–26 |
| 11:55–12:05 | דמו | Polymorphism | [Day1.Demo.Polymorphism](../Day1-OOP-Collections-Exceptions/Demos/Day1.Demo.Polymorphism/Program.cs) |
| 12:05–12:15 | תרגול | **תרגיל 6** ★★ pattern matching | [Exercises](../Day1-OOP-Collections-Exceptions/Exercises/README.md) |
| 12:15–13:00 | הפסקה | ארוחת צהריים | |
| 13:00–13:55 | מעבדה | **Lab 2**: צורות ועובדים (payroll) | [Lab2 README](../Day1-OOP-Collections-Exceptions/Labs/Lab2-ShapesAndEmployees/README.md), שקף 27 |
| 13:55–14:00 | סיכום | דיון על Lab 2: למה `Contractor` לא יורש מ-`Employee` | [Lab2 NOTES](../Day1-OOP-Collections-Exceptions/Labs/Lab2-ShapesAndEmployees/Solution/NOTES.md) |
| 14:00–14:15 | הרצאה | **מודול 04**: מערכים, List, Dictionary, HashSet, Queue/Stack, `yield`, `Repository<T>`, Big-O, `IReadOnlyList` | [Notes/04](../Day1-OOP-Collections-Exceptions/Notes/04-collections.md), שקפים 28–35 |
| 14:15–14:22 | דמו | Collections | [Day1.Demo.Collections](../Day1-OOP-Collections-Exceptions/Demos/Day1.Demo.Collections/Program.cs) |
| 14:22–14:30 | תרגול | **תרגיל 9** ★★ האוסף הנכון (בעל-פה, בכיתה) | [Exercises](../Day1-OOP-Collections-Exceptions/Exercises/README.md) |
| 14:30–14:45 | הפסקה | | |
| 14:45–15:00 | הרצאה | **מודול 05**: delegates, `Func`/`Action`, lambdas, closures, events, LINQ, deferred execution | [Notes/05](../Day1-OOP-Collections-Exceptions/Notes/05-delegates-lambdas-linq.md), שקפים 36–41 |
| 15:00–15:07 | דמו | LinqDelegates | [Day1.Demo.LinqDelegates](../Day1-OOP-Collections-Exceptions/Demos/Day1.Demo.LinqDelegates/Program.cs) |
| 15:07–15:15 | תרגול | **תרגיל 10** ★ LINQ על מספרים | [Exercises](../Day1-OOP-Collections-Exceptions/Exercises/README.md) |
| 15:15–15:24 | הרצאה | **מודול 06**: try/catch/finally/when, חריגות מותאמות, `throw;`, `using`, guard clauses, TryParse, הדיבאגר | [Notes/06](../Day1-OOP-Collections-Exceptions/Notes/06-exceptions-debugging.md), שקפים 43–48 |
| 15:24–15:30 | דמו | Exceptions | [Day1.Demo.Exceptions](../Day1-OOP-Collections-Exceptions/Demos/Day1.Demo.Exceptions/Program.cs) |
| 15:30–15:33 | תרגול | **תרגיל 13** ★★★ "נחשו את סדר ההדפסות" (ניחוש בקול, ואז הרצת הפתרון) | [Exercises](../Day1-OOP-Collections-Exceptions/Exercises/README.md) |
| 15:33–15:40 | הרצאה | **מודול 07**: קוד נקי: שמות, מספרי קסם, SOLID, `dotnet format`, refactoring (עם **תרגיל 14** כדוגמת before/after) | [Notes/07](../Day1-OOP-Collections-Exceptions/Notes/07-clean-code.md), שקפים 49–52 |
| 15:40–16:20 | מעבדה | **Lab 3 או Lab 4** (לבחירה; השנייה הולכת הביתה) | [Lab3](../Day1-OOP-Collections-Exceptions/Labs/Lab3-InventoryLinq/README.md), [Lab4](../Day1-OOP-Collections-Exceptions/Labs/Lab4-BankRobust/README.md), שקפים 42 / 53 |
| 16:20–16:30 | סיכום | שאלות חזרה, שיעורי בית, הצצה ליום 2 | שקפים 54–55 |

**תרגילים שלא נכנסו לכיתה והולכים הביתה:** 1 (אם לא נפתר), 4, 5, 7, 8, 11, 12, 14 (את 14 מדגימים בכיתה, אבל מבקשים לפתור לבד).
תרגיל 5 הושאר בחוץ בכוונה כי הוא חופף ל-Lab 2. תרגילים 7 ו-8 חופפים ל-Lab 3.

---

## פירוט לפי בלוק

### 09:00–09:15 · פתיחה

- הציגו את עצמכם וערכו סבב קצר: "באיזו שפה תכנתתם עד היום?" התשובות יעזרו לכוון את מודול 01. למי שמגיע מ-Java/JS רוב החומר מוכר. למי שמגיע מ-Python כדאי להדגיש טיפוסים סטטיים.
- שקף 2 (סדר היום), שקף 3 (מה נדע לעשות בסוף היום).
- כולם מריצים `dotnet --version`. מי שלא מקבל `10.x` עובר לסעיף "פתרון בעיות" ב-[`INSTALL.md`](../00-Setup/INSTALL.md), ובינתיים עובד עם שכן.
- הסבירו את מבנה התיקייה (Notes / Demos / Exercises / Labs) ואת כלל ההעתקה של `Starter`.

---

### 09:15–09:35 · הרצאה: מודול 01, חימום C# (שקפים 4–9)

**נקודות לפי סדר ההוראה:**

1. `dotnet new console -n HelloApp` ואז `dotnet run`. מה יש ב-`.csproj`: `net10.0`, `Nullable`, `ImplicitUsings`. זה מסביר למה אין `using System;` בראש הקובץ.
2. top-level statements: המהדר מייצר את `Main` בעצמו. הצהרות טיפוסים חייבות לבוא **אחרי** ההוראות.
3. טיפוסים: `int`, `long`, `double`, `decimal` (עם סיומת `m`, ומשמש לכסף), `bool`, `char`, `string`. `var` = הסקת טיפוס, והטיפוס עדיין סטטי.
4. **ערך מול הפניה** (הנקודה הכי חשובה בבוקר): `b = a` מעתיק `int`, אבל עבור מערך מעתיק רק את ההפניה. `string` הוא reference type בלתי-משתנה, ולכן `s.ToUpper();` לבד לא משנה כלום.
5. אינטרפולציה ופורמטים: `{x:N2}`, `{x,-10}`, `{x:P1}`, raw string literals.
6. בקרת זרימה: `switch` expression עם `_`, `foreach` כברירת מחדל.
7. מתודות: expression-bodied, פרמטר אופציונלי, named argument, `out`, tuple ו-deconstruction.
8. Nullable reference types: `string?`, `?.`, `??`, `is not null`, ולמה `!` הוא ריח רע. `Console.ReadLine()` מחזיר `string?`.
9. בקצרה: `enum`, `struct`, ואיך קוראים שגיאת קומפילציה (`CS0029` + שורה/עמודה, ו-`Ctrl+.`).

**שאלות לכיתה:**
- "`int[] arr2 = arr1; arr2[0] = 99;` — מה יש עכשיו ב-`arr1[0]`? ואם זה היה `int`?"
- "`0.1 + 0.2 == 0.3` — true או false? באיזה טיפוס תשמרו מחיר?"

**טעויות נפוצות אצל סטודנטים:**
- חושבים ש-`var` הופך את המשתנה לדינמי.
- `name.ToUpper();` בלי השמה.
- משתיקים אזהרות nullable עם `!` במקום לטפל ב-`null`.
- כותבים `decimal x = 19.90;` בלי `m`, וזה לא מתקמפל.

---

### 09:35–09:42 · דמו: Day1.Demo.Quickstart

<div dir="ltr">

```powershell
cd C:\c-\Day1-OOP-Collections-Exceptions\Demos\Day1.Demo.Quickstart
dotnet run
```

</div>

הדמו רץ ברצף, בלי ארגומנטים, ומדפיס 5 סעיפים. עברו עליהם עם הקוד פתוח לצד הפלט:

1. **סעיף 1:** `var isActive = true` ואינטרפולציה עם `:F2`.
2. **סעיף 2 (עצרו כאן):** `a=5, b=6` מול `arr1[0]=99`. אחר כך ה-`struct Point`: `p1.X=1, p2.X=100`, כלומר struct מועתק. כדאי לשאול: "מה יקרה אם נשנה את `Point` ל-`class`?" ולשנות בחי אם יש זמן.
3. **סעיף 3:** `Grade(85)` = B דרך `switch` expression עם relational patterns.
4. **סעיף 4:** `TryDivide(10, 0, out _)` מחזיר `false`. ה-`out _` הוא discard. `MinMax` מחזיר tuple.
5. **סעיף 5:** `FindUser(1)` מחזיר `null`, ולכן `?.Length` ריק ו-`??` מחליף ב-"(unknown)". אחרי `if (found is not null)` אין אזהרה.

**מה להדגיש:** שהמתודות העזר (local functions) וה-`struct` נמצאים **בסוף** הקובץ. זה כלל ה-top-level.

---

### 09:42–09:50 · תרגול: תרגילים 2 (ו-1)

| # | כותרת | קושי | תשובה צפויה |
|---|---|---|---|
| 2 | FizzBuzz עם switch expression | ★ | `(n % 3, n % 5) switch { (0, 0) => "FizzBuzz", (0, _) => "Fizz", (_, 0) => "Buzz", _ => n.ToString() }`. הסדר חשוב: `(0,0)` ראשון. |
| 1 (למי שסיים) | ממיר טמפרטורות | ★ | `static double CelsiusToFahrenheit(double c) => c * 9 / 5 + 32;` והפוך. לולאה `for (c = -10; c <= 40; c += 10)` עם `{x:F1}`. |

הרצת הפתרון (בטרמינל B):

<div dir="ltr">

```powershell
dotnet run -- 2
dotnet run -- 1
```

</div>

---

### 09:50–10:10 · הרצאה: מודול 02, מחלקות ואובייקטים (שקפים 10–15)

**נקודות לפי סדר ההוראה:**

1. מחלקה היא תבנית, אובייקט הוא מופע. `new` מפעיל בנאי. דוגמת `BankAccount`: `Deposit` מאמת בעצמו, והאובייקט "יודע להפקיד לעצמו".
2. מוסכמות: `_camelCase` לשדות פרטיים, `PascalCase` לכל מה שציבורי, ומחלקה אחת לכל קובץ.
3. **שדות מול properties.** למה לא שדה ציבורי: ולידציה, ערך מחושב, קריאה בלבד, והיכולת לשנות מימוש בעתיד.
4. סוגי properties (שקף 12): `{ get; set; }`, `{ get; private set; }`, property מלא עם ולידציה, computed `=>`, `init`, `required`.
5. בנאים: תפקידם להביא את האובייקט למצב תקין. overload עם `: this(...)`. guard `ArgumentException.ThrowIfNullOrWhiteSpace`.
6. Primary constructor (C# 12): הפרמטרים **אינם** properties, בניגוד ל-record.
7. `this`, ו-`static` (מונה, `const`, factory method, `static class`).
8. Object initializer ו-collection expression `[...]`.
9. **`record` מול `class`** (שקף 14): שוויון לפי ערך, `ToString`, `with`, deconstruction. כלל האצבע: אם הזהות חשובה משתמשים ב-class, ואם רק התוכן חשוב ב-record.
10. מחזור חיים: heap, ערכי ברירת מחדל, מאתחלים, גוף הבנאי, GC. זה מכין את הקרקע ל-`IDisposable` במודול 06.

**שאלות לכיתה:**
- "שני חשבונות בנק עם אותו בעלים ואותה יתרה: אותו חשבון? אז class או record?"
- "מתי תעדיפו `required` + `init` על פני בנאי?" (תשובה: כשאין לוגיקה ביצירה ויש הרבה שדות.)

**טעויות נפוצות:**
- `public int Age;` כשדה ציבורי.
- `set` ציבורי על כל דבר.
- `==` בין שני מופעי class כשמתכוונים להשוואת תוכן.
- מנסים לגשת ל-`this` מתוך מתודה `static`.
- record עם `set` ציבורי.
- בנאי שלא מאמת, ואז בדיקות `null` שמפוזרות בכל הקוד.

---

### 10:10–10:20 · דמו: Day1.Demo.Classes

<div dir="ltr">

```powershell
cd C:\c-\Day1-OOP-Collections-Exceptions\Demos\Day1.Demo.Classes
dotnet run
```

</div>

1. **סעיף 1:** `BankAccount` עם `Balance { get; private set; }`. **הסירו את ההערה** מ-`// acc.Balance = 1000;` והראו את שגיאת הקומפילציה, ואז החזירו את ההערה.
2. **סעיף 2:** בנאי שני עם `: this(id, owner)` ו-`initialDeposit`. `BankAccount.Count` הוא static ומדפיס 2.
3. **סעיף 3:** `Product` עם `required`/`init`. הסירו את ההערה מ-`// product.Sku = "X";` כדי להראות שגיאת init-only. אפשר גם למחוק את `Sku = ...` מה-initializer כדי להראות את שגיאת `required`.
4. **סעיף 4:** `TemperatureSensor(string location, double initial)`, primary constructor שהפרמטרים שלו זמינים בגוף המחלקה.
5. **סעיף 5:** `Product.CreateFreeSample` כ-factory ו-`const MaxNameLength`.
6. **סעיף 6 (השיא):** `PersonClass` מדפיס `c1 == c2 ? False`, ו-`PersonRecord` מדפיס `True`, יחד עם `ToString` אוטומטי, `with { Age = 31 }` ו-deconstruction.

---

### 10:20–10:30 · תרגול: תרגיל 3 (ו-4)

| # | כותרת | קושי | תשובה צפויה |
|---|---|---|---|
| 3 | מחלקת `Rectangle` | ★ | `Width`/`Height` כ-`{ get; }` שנקבעים בבנאי, עם `ArgumentOutOfRangeException` אם הערך ≤ 0. `Area => Width * Height`. `Scale(f)` מחזיר `new Rectangle(Width*f, Height*f)`, כלומר לא משנה את המלבן הקיים. |
| 4 (למי שסיים) | `record` מול `class` + static counter | ★★ | record: `==` ו-`Equals` שניהם `True`. class: שניהם `False`, כי ברירת המחדל משווה הפניות. `static int Created` גדל בבנאי. `p with { X = p.X + 1 }`. |

<div dir="ltr">

```powershell
dotnet run -- 3
dotnet run -- 4
```

</div>

---

### 10:30–10:45 · הפסקה

---

### 10:45–11:25 · מעבדה: Lab 1, מערכת ספרייה (שקף 16)

**מטרה:** לבנות `Book` עם properties ובנאי שמאמת, ו-`Library` שמנהלת `List<Book>` (הוספה, חיפוש, השאלה, החזרה). הרעיון המרכזי: מצב האובייקט משתנה רק דרך המתודות שלו.

**מה יש ב-Starter:** הפרויקט מתקמפל, וכל מתודה זורקת `NotImplementedException`. תפריט הקונסולה ב-`Program.cs` כבר מוכן, כולל `catch` ל-`InvalidOperationException`/`ArgumentException` שמדפיס `Error: ...`. TODO 1–4 נמצאים ב-`Book.cs`, TODO 5–11 ב-`Library.cs`, ו-TODO 12 הוא הסרת ההערות מ-`Seed` ב-`Program.cs`.

<div dir="ltr">

```powershell
cd Labs\Lab1-LibrarySystem\Starter
dotnet run
```

</div>

**השלבים בקצרה:**
1. `Book` (15 דק'): properties (`Isbn` קריאה בלבד, `IsBorrowed` עם `private set`, `Age` מחושב), בנאי עם guards, `Borrow()`/`Return()` שזורקים `InvalidOperationException` כשהמצב לא חוקי, ו-`ToString()`.
2. `Library` (20 דק'): `IReadOnlyList<Book> Books`, `AddBook` שמונע ISBN כפול, `SearchByTitle`/`SearchByAuthor`/`FindByIsbn`, `Borrow(isbn)`/`Return(isbn)` שמחזירים `false` כשהספר לא נמצא, ו-`GetAvailable()` ממוין.
3. חיבור (10 דק'): הסרת ההערות ב-`Seed` ובדיקת כל התפריט.

**קריטריוני קבלה (מתוך ה-README):** build בלי אזהרות. `new Book("", ...)` זורק `ArgumentException`. שנה 3000 זורקת `ArgumentOutOfRangeException`. השאלה כפולה מדפיסה `Error: ... already borrowed` בלי לקרוס. `SearchByTitle("CLEAN")` מוצא את "Clean Code". `GetAvailable()` ממוין ולא כולל ספרים מושאלים. `library.Books.Add(...)` לא מתקמפל.

**איפה נתקעים ואיזה רמז לתת:**

| מקום | רמז |
|---|---|
| `IsBorrowed { get; set; }` ציבורי (הטעות הנפוצה לפי הערות השקף) | "מי מורשה לשנות אותו? רק `Borrow`/`Return`, ולכן `private set`." |
| חיפוש שתלוי ברישיות, או `ToLower()` על שני הצדדים | `title.Contains(text, StringComparison.OrdinalIgnoreCase)` |
| `FindByIsbn` מחזיר `Book` ולא `Book?`, ומופיעה אזהרת nullable | `_books.FirstOrDefault(b => b.Isbn == isbn)` עם טיפוס חזרה `Book?` |
| בדיקת השנה | `DateTime.Now.Year` כגבול עליון |
| "למה לא `List<Book>`?" (TODO 5) | `List<T>` מממש `IReadOnlyList<T>`, ולכן `public IReadOnlyList<Book> Books => _books;` עובד בלי להעתיק |
| "הרצתי ואין ספרים" | שכחו את TODO 12, הסרת ההערות ב-`Seed` |
| עוד לא מכירים LINQ | אפשר לכתוב `foreach` רגיל, והתוצאה זהה |

**בדיקה מהירה של הפתרון** (לפי NOTES):

<div dir="ltr">

```bash
cd Labs/Lab1-LibrarySystem/Solution
printf '1\n2\nclean\n4\n978-0132350884\n4\n978-0132350884\n6\n5\n978-0132350884\n0\n' | dotnet run
```

</div>

**בונוס למהירים:** `BorrowedCount`, `Oldest` (עם `MinBy`), ו-`DueDate` + `IsOverdue` (14 יום מההשאלה).

### 11:25–11:30 · סיכום Lab 1

החלטות מ-[`Solution/NOTES.md`](../Day1-OOP-Collections-Exceptions/Labs/Lab1-LibrarySystem/Solution/NOTES.md) לדיון:
- **`Book.Borrow()` זורק, אבל `Library.Borrow(isbn)` מחזיר `false`.** השאלה כפולה היא הפרת כלל, ולכן חריגה. "לא נמצא" היא תוצאה לגיטימית של חיפוש, ולכן ערך חזרה. ההבחנה הזו תחזור ב-Lab 4.
- `Age` הוא computed property ולא שדה, כי ערך שנשמר בשדה "מתיישן".
- האם `Book` צריך להיות `record`? לא, כי יש לו מצב שמשתנה (`IsBorrowed`).

---

### 11:30–11:55 · הרצאה: מודול 03, עקרונות OOP (שקפים 17–26)

**נקודות לפי סדר ההוראה:**

1. מסגרת: ארבעת העקרונות הם ארבע תשובות לשאלה אחת: "איך משנים בלי לשבור?" (שקף 18).
2. **אנקפסולציה:** טבלת access modifiers (`public`, `private`, `protected`, `internal`, `protected internal`, `private protected`). כלל: מתחילים מ-`private`.
3. **ירושה (is-a):** `virtual`, `override`, `base.X()`, `sealed`. ירושה יחידה. אם כותבים מתודה בלי `override` מקבלים hiding, וזו כמעט תמיד טעות.
4. **abstract:** אי אפשר ליצור `new Shape()`, ומתודה `abstract` חייבת מימוש ביורשים. תבנית Template Method: `Describe()` בבסיס קורא ל-`Area()` של היורש.
5. **פולימורפיזם:** `List<Shape>` ו-`foreach` שקורא ל-`Describe()`. זה Open/Closed: מוסיפים `Triangle` בלי לגעת בלולאה, בניגוד ל-`switch` על סוג.
6. **ממשקים:** `IPayable`, מימוש של כמה ממשקים, default interface members (נגישים רק דרך טיפוס הממשק). טבלת abstract class מול interface (שקף 23).
7. **הפשטה:** הקוד תלוי בחוזה ולא במימוש. זה הבסיס ל-DI.
8. **Composition over inheritance:** `OrderService(ILogger logger)` ולא `OrderService : ConsoleLogger`.
9. `is` / `as` / cast. `switch` עם type patterns, property patterns ו-`when`. הסדר חשוב: מקרה ספציפי לפני כללי.
10. `ToString`, `Equals`, `GetHashCode`: דורסים את שניהם יחד, או פשוט משתמשים ב-record.

**שאלות לכיתה:**
- "`Car : Engine` — נשמע נכון? איך הייתם מתכננים את זה?" (תשובה: הרכבה, has-a.)
- "הוספנו סוג עובד חדש. כמה קבצים צריך לשנות עם פולימורפיזם, וכמה עם `switch` על `EmployeeType`?"

**טעויות נפוצות:**
- שוכחים `virtual`, ואז `override` לא מתקמפל.
- שוכחים `override`, מקבלים hiding, והפולימורפיזם "לא עובד".
- ירושה רק בשביל שימוש חוזר בקוד.
- מממשים `Equals` בלי `GetHashCode` (אזהרה CS0659).
- `(Circle)shape` בלי בדיקה, שזורק `InvalidCastException`.
- בונים היררכיות עמוקות מדי.

---

### 11:55–12:05 · דמו: Day1.Demo.Polymorphism

<div dir="ltr">

```powershell
cd C:\c-\Day1-OOP-Collections-Exceptions\Demos\Day1.Demo.Polymorphism
dotnet run
```

</div>

1. **סעיף 1:** `List<Shape>` עם Circle, Rectangle ו-Square. `Circle.Describe()` משתמש ב-`base.Describe()`. `Square` הוא `sealed` ויורש מ-`Rectangle(side, side)` בלי לממש `Area()` בעצמו. בסוף מודפס סכום השטחים.
2. **סעיף 2:** `IDescribable` מוחזק גם ב-`Shape` וגם ב-`Invoice`, שאין להן אב משותף. `DescribeLoud()` הוא default member שנגיש רק דרך משתנה מטיפוס `IDescribable`.
3. **סעיף 3:** `switch` על `object[]` מעורב: `Circle { Radius: > 1.5 }` מופיע לפני `Circle c`, ו-`Rectangle ... when w == h`. נסו להזיז את `Circle c` מעל ה-property pattern כדי להראות את שגיאת "case שלא ניתן להגיע אליו".
4. **סעיף 4:** `Money` עם `Equals`/`GetHashCode` דרוסים, ולכן `HashSet` מכיל איבר **אחד**. אפשר להעיר את `GetHashCode` בחי ולהראות שה-count קופץ ל-2 (ומופיעה אזהרה).
5. **סעיף 5:** `OrderService` עם `ConsoleLogger` מול `NullLogger`. זו composition: מחליפים התנהגות בלי ירושה.

---

### 12:05–12:15 · תרגול: תרגיל 6

| # | כותרת | קושי | תשובה צפויה |
|---|---|---|---|
| 6 | pattern matching | ★★ | `switch` expression שבו `int n when n < 0`, `int`, `string { Length: 0 }`, `string`, `double`, `int[] { Length: 0 }`, `int[]`, `null` ו-`_`. **הסדר קובע:** קודם המקרים הספציפיים. |

<div dir="ltr">

```powershell
dotnet run -- 6
```

</div>

תרגיל 5 (כלי רכב, ★★) חופף מאוד ל-Lab 2, ולכן נשאר לבית.

---

### 12:15–13:00 · ארוחת צהריים

---

### 13:00–13:55 · מעבדה: Lab 2, צורות ועובדים (שקף 27)

**מטרה:** שתי היררכיות. צורות: מחלקה אבסטרקטית, ירושה ו-`sealed`. משכורות: ממשק `IPayable` עם default member, `base`, ופולימורפיזם דרך `List<IPayable>`.

**מה יש ב-Starter:** קבצי `Shapes.cs` (TODO 1–3) ו-`Employees.cs` (TODO 4–10) עם שלדים בהערות. ב-`Program.cs` יש את TODO 11 (צורות) ו-TODO 12 (payroll), כולל נתוני הדוגמה בהערה.

**השלבים בקצרה:**
- **חלק א' — צורות (15 דק'):** `abstract Shape` עם `Name`, `Area()` ו-`Perimeter()` אבסטרקטיים ו-`ToString()` משותף. `Circle`, `Rectangle`, `Triangle` (שבודק אי-שוויון משולש בבנאי). `sealed Square : Rectangle`. ב-Program: רשימה, סכום שטחים ו-`try/catch` על `Triangle(1,1,10)`.
- **חלק ב' — משכורות (45 דק'):** `IPayable` עם `PaySlip()` כ-default member. `abstract Employee : IPayable`. `SalariedEmployee` מחשב שנתי חלקי 12. `HourlyEmployee` מחשב 160 שעות רגילות ומעליהן פי 1.5, ו-`Describe()` שלו משתמש ב-`base`. `Manager : SalariedEmployee` מוסיף `Bonus` ו-`Reports`. `Contractor : IPayable` **בלי** `Employee`. `Payroll` מחזיק `List<IPayable>`.

**קריטריוני קבלה:** `new Shape()` לא מתקמפל. `Triangle(3,4,5)` מחזיר שטח 6. `Square(2)` מדפיס `Square: area=4.00, perimeter=8.00`. Yossi מקבל 14,000 ונועה 32,000. הסכום הכולל הוא 78,000. `PaySlip()` ממומש רק בממשק. הקבלן לא מופיע בלולאת `Describe()`.

**הפלט הצפוי של הפתרון** (לפי NOTES): Circle 3.14, Rectangle 6, Triangle 6, Square 4, סה"כ 19.14. שגיאה על Triangle(1,1,10). Dana 20,000, Yossi 14,000, Noa 32,000, Acme 12,000, סה"כ 78,000.

<div dir="ltr">

```powershell
cd Labs\Lab2-ShapesAndEmployees\Solution
dotnet run
```

</div>

**איפה נתקעים ואיזה רמז לתת:**

| מקום | רמז |
|---|---|
| `Square` מממש `Area()` מחדש | `class Square(double side) : Rectangle(side, side)`, וזה כל הקוד שצריך |
| `dana.PaySlip()` לא מתקמפל | default member נגיש רק דרך משתנה מטיפוס `IPayable` |
| חישוב שעות נוספות עם `if` מסובך | `Math.Min(hours, 160)` ו-`Math.Max(hours - 160, 0)`, עם קבועים `RegularHours`/`OvertimeFactor` |
| Yossi מקבל 13,600 (בלי תוספת) או 14,800 (שעות נוספות נספרו פעמיים) | 160×80 + 10×(80×1.5) = 14,000. השעות הרגילות מוגבלות ל-160 |
| `Manager` מחשב את המשכורת מחדש | `base.CalculateMonthlyPay() + Bonus` |
| `Contractor : Employee` | "יש לו Id? מנהל? הוא עובד?" הקבלן צריך לממש רק את `IPayable` |
| אי-שוויון המשולש | כל צלע צריכה להיות קטנה מסכום השתיים האחרות. אחרת זורקים `ArgumentException` בבנאי |

**בונוס למהירים:** סיווג עם pattern matching ("senior manager", "manager", "hourly with overtime", "regular employee", "external"), כשסדר ה-cases חשוב. `IDrawable` עם ציור ASCII רק ל-`Rectangle`/`Square`. `Payroll.Employees` עם `OfType<Employee>()`.

### 13:55–14:00 · סיכום Lab 2

מתוך [`Solution/NOTES.md`](../Day1-OOP-Collections-Exceptions/Labs/Lab2-ShapesAndEmployees/Solution/NOTES.md):
- **למה `Contractor` לא יורש מ-`Employee`?** לפי הערות השקף זו השאלה הכי שווה לדיון. `IPayable` מאפשר ל-`Payroll` לעבוד עם כל מי שמקבל תשלום. הקוד תלוי בחוזה ולא במימוש.
- **"מה לא עשינו בכוונה":** אין `enum EmployeeType` עם `switch` בתוך `CalculateMonthlyPay`, כי בדיוק את זה פולימורפיזם מחליף (Open/Closed). אין גם `Salary` עם set ציבורי. במקומו אפשר מתודה כמו `GiveRaise`.
- `Shape.ToString()` שקורא ל-`Area()` האבסטרקטי הוא Template Method.

---

### 14:00–14:15 · הרצאה: מודול 04, אוספים וגנריקה (שקפים 28–35)

**נקודות לפי סדר ההוראה (בקצב מהיר, כי הזמן קצר):**

1. מערכים: גודל קבוע, `[^1]`, ranges, דו-ממדי מול jagged.
2. `List<T>`: `Add`, `Insert`, `Remove`, `Contains` (O(n)). ההבדל בין `Count` ל-`Capacity`.
3. `Dictionary<K,V>`: חיפוש O(1). `TryGetValue` במקום גישה ישירה שזורקת `KeyNotFoundException`. `StringComparer.OrdinalIgnoreCase` (כלי מרכזי ב-Lab 3 וב-Lab 4).
4. `HashSet` (ייחודיות, `Add` מחזיר `bool`), `Queue` (FIFO), `Stack` (LIFO).
5. `IEnumerable<T>` הוא המכנה המשותף. `yield return` מייצר ערכים לפי דרישה, וזה הבסיס ל-deferred execution.
6. גנריקה: `Repository<T> where T : IEntity`. בלי constraint, `T` מתנהג כמו `object`. רשימת constraints נפוצים.
7. **טבלת Big-O (שקף 34):** ההשוואה בין `List.Contains` בלולאה על 100K איברים (10 מיליארד השוואות) לבין `HashSet` (100K).
8. `IReadOnlyList<T>` לחשיפה בטוחה. capacity, "אל תעתיקו סתם", `StringBuilder`, `Span<T>` (רק להכיר שקיים).

**שאלות לכיתה:**
- "צריך לבדוק לכל אחד מ-100,000 מזהים אם כבר ראינו אותו. איזה אוסף?"
- "`foreach (var x in list) if (...) list.Remove(x);` — מה יקרה?" (תשובה: `InvalidOperationException: Collection was modified`. הפתרון: `RemoveAll` או `ToList()`.)

**טעויות נפוצות:**
- `dict[key]` בלי בדיקה.
- חשיפת `List<T>` ציבורי.
- מפתח שמשתנה בתוך `Dictionary`/`HashSet`.
- `IEnumerable` שנצרך פעמיים.
- `Count()` של LINQ במקום ה-property `Count`.

---

### 14:15–14:22 · דמו: Day1.Demo.Collections

<div dir="ltr">

```powershell
cd C:\c-\Day1-OOP-Collections-Exceptions\Demos\Day1.Demo.Collections
dotnet run
```

</div>

1. **סעיף 1:** `grid[1,2]` מחזיר 6, ו-`jagged[2].Length` מחזיר 3.
2. **סעיפים 2–3:** List ו-Dictionary. `TryGetValue("banana")`, ו-`foreach (var (key, value) in stock)` עם deconstruction.
3. **סעיף 4:** HashSet בולע את הכפילות `"c#"`. ה-`Add("oop")` הראשון מחזיר `True` והשני `False`. אחר כך Queue מול Stack.
4. **סעיף 5:** `EvenNumbers(1_000_000).Take(3)` מחשב רק 3 ערכים. אפשר לשים breakpoint על `yield return` כדי להראות את זה.
5. **סעיף 6:** `Repository<Customer>` עם אינדקס `Dictionary` לצד `List`. `GetById(99)` מחזיר `null`. `all.Add(...)` לא קיים על `IReadOnlyList`.
6. **סעיף 7:** `Capacity` אחרי 100K איברים, ו-`Span` שכותב למערך המקורי (`30` מופיע ב-`data`). הסבירו שזה "חלון" על המערך ולא העתקה.

---

### 14:22–14:30 · תרגול: תרגיל 9 (בעל-פה)

לפי הערות השקף, את התרגיל הזה עושים בכיתה בעל-פה: מקריאים כל תרחיש והכיתה עונה.

| # | כותרת | קושי | תשובה צפויה |
|---|---|---|---|
| 9 | האוסף הנכון | ★★ | (א) מבקרים ייחודיים: `HashSet<T>`. (ב) תור הדפסה: `Queue<T>`. (ג) Undo: `Stack<T>`. (ד) סטודנט לפי ת"ז: `Dictionary<K,V>`. (ה) ציונים לממוצע: `List<T>` (או מערך). |

<div dir="ltr">

```powershell
dotnet run -- 9
```

</div>

תרגילים 7 (ספירת מילים, ★) ו-8 (`MyStack<T>`, ★★) הולכים הביתה.

---

### 14:30–14:45 · הפסקה

---

### 14:45–15:00 · הרצאה: מודול 05, Delegates, Lambdas, Events ו-LINQ (שקפים 36–41)

**נקודות לפי סדר ההוראה:**

1. delegate הוא "מתודה כערך". `delegate int MathOp(int a, int b)` מול הטיפוסים המוכנים `Func`, `Action` ו-`Predicate` (בטבלה). delegates הם multicast (`+=`).
2. Lambdas: `x => x * x`, `(a, b) => ...`, `() => 42`.
3. **Closures:** `MakeCounter()`. ה-lambda לוכדת את **המשתנה**, לא את הערך.
4. **Events:** `event EventHandler<TArgs>? X`, הפעלה עם `X?.Invoke(this, args)`. מבחוץ אפשר רק `+=`/`-=`. ה-publisher לא מכיר את המנויים. מומלץ להסיר מנויים מאובייקטים שחיים הרבה זמן.
5. בקצרה: extension methods הם מה שמאפשר ל-LINQ "להיצמד" לכל `IEnumerable`. delegate מול ממשק: פעולה אחת מתאימה ל-`Func`, וקבוצת פעולות קשורות מתאימה לממשק.
6. **LINQ:** `Where`, `Select`, `OrderByDescending().ThenBy()`, `GroupBy`, `First`/`FirstOrDefault`/`Single`, `Any`/`All`/`Count`, `Max`/`Average`, `ToDictionary`/`ToList`/`ToHashSet`, `Skip`/`Take`.
7. Method syntax מול query syntax: המהדר מתרגם את השני לראשון. בוחרים סגנון אחד ונשארים עקביים.
8. **Deferred execution (הנקודה החשובה ביותר במודול):** השאילתה רצה רק כשצורכים אותה. `ToList()` מקפיא את התוצאה. חריגה בתוך lambda נזרקת בזמן הצריכה.
9. סגנון פונקציונלי: פונקציות טהורות, immutability (`record` + `with`), והפרדה בין חישוב ל-I/O.

**שאלות לכיתה:**
- "`var evens = numbers.Where(n => n % 2 == 0); numbers.Add(4);` — האם 4 יופיע ב-`evens`?"
- "מה ההבדל בין `First` ל-`FirstOrDefault` כשאין התאמה?"

**טעויות נפוצות:**
- מפעילים אירוע בלי `?.`, וכשאין מנויים מקבלים `NullReferenceException`.
- `First()` על רצף ריק.
- תופעות לוואי בתוך `Select`.
- שאילתה שנצרכת פעמיים.
- שרשרת LINQ ארוכה בשורה אחת.
- לכידת משתנה של לולאת `for`.

---

### 15:00–15:07 · דמו: Day1.Demo.LinqDelegates

<div dir="ltr">

```powershell
cd C:\c-\Day1-OOP-Collections-Exceptions\Demos\Day1.Demo.LinqDelegates
dotnet run
```

</div>

1. **סעיף 1:** `MathOp` מותאם, `Func`/`Action`/`Predicate`, ו-`pipeline` multicast שמדפיס `step1 step2 step3`.
2. **סעיף 2:** closure שמדפיס `1 2 3`.
3. **סעיף 3:** `Thermometer.TemperatureChanged` עם שני מנויים. מנוי B מגיב רק מעל 30. השמה שנייה של `32` **לא** מפעילה את האירוע, כי אין שינוי.
4. **סעיף 4:** LINQ על `orders`: expensive, מיון, GroupBy, `First`, `Any`/`All`, `FirstOrDefault` שמחזיר "none", `ToDictionary` ו-`Aggregate`.
5. **סעיף 5:** אותה שאילתה ב-query syntax.
6. **סעיף 6 (עצרו כאן):** `evens = 2,4`, כלומר 4 נכלל כי השאילתה רצה רק בהדפסה. `snapshot = 2,4` בלי 6, כי `ToList()` הקפיא את התוצאה.
7. **סעיף 7:** `ApplyDiscount` היא פונקציה טהורה. ה-cart המקורי לא השתנה.

---

### 15:07–15:15 · תרגול: תרגיל 10

| # | כותרת | קושי | תשובה צפויה |
|---|---|---|---|
| 10 | LINQ על מספרים | ★ | עבור `[5,3,8,1,9,2,7,4,6,10]`: סכום ריבועי הזוגיים 220 (`Where(...).Sum(n => n*n)`). שלושת הגדולים 10, 9, 8. `Any(n => n > 9)` מחזיר true. ממוצע האי-זוגיים 5.00. `string.Join(" ", nums.Order())`. |

<div dir="ltr">

```powershell
dotnet run -- 10
```

</div>

תרגיל 11 (Compose ואירוע `Counter`, ★★) הולך הביתה.

---

### 15:15–15:24 · הרצאה: מודול 06, חריגות ודיבוג (שקפים 43–48)

הבלוק דחוס בכוונה. לפי הערת המרצה ב-README, Lab 4 מכסה את החומר בפועל.

1. חריגה היא אובייקט שנזרק ומטפס במעלה ה-call stack. יש לה `Message`, `StackTrace` ו-`InnerException`. כדאי להכיר את החריגות הנפוצות.
2. `try/catch/finally`: catch ספציפי לפני כללי. `finally` רץ תמיד (גם אחרי `return` או `continue`).
3. `when` filters. הפילטר רץ **לפני** ה-unwinding, ולכן אפשר להשתמש בטריק `when (Log(ex))`.
4. `throw;` מול `throw ex;` (CA2200). עטיפת חריגה עם `InnerException`.
5. חריגות מותאמות: `BankException` כבסיס, ו-`InsufficientFundsException` עם properties (`Shortfall`).
6. `using` ו-`IDisposable`: using statement ו-using declaration. בלי Dispose, ה-buffer של הקובץ לא נכתב לדיסק.
7. Guard clauses: `ThrowIfNull`, `ThrowIfNullOrWhiteSpace`, `ThrowIfNegativeOrZero`.
8. **מתי לא לזרוק:** `TryParse`, `TryGetValue`. קלט לא תקין הוא מצב צפוי, לא חריג.
9. הדיבאגר (שקפים 47–48): אסטרטגיה בארבעה צעדים (לשחזר, לשער, F10 עם Watch, לתקן). הכלים: F9/F10/F11, conditional breakpoint, Watch, Immediate (`Ctrl+Alt+I`), Call Stack, Exception Settings (`Ctrl+Alt+E`), Edit & Continue. `Debug.Assert` נמחק ב-Release.

**שאלה לכיתה:** "`catch (Exception) { }` ריק. מה הבעיה?" (תשובה: התוכנה ממשיכה לרוץ במצב שבור, ומישהו יגלה את זה שבוע אחר כך.)

**טעויות נפוצות:**
- `throw ex;`
- שימוש בחריגות לזרימת בקרה (`Parse` בתוך `try` בלולאה).
- `Debug.Assert` לוולידציית קלט.
- שכחת `using`.
- הודעות שגיאה בלי מידע שימושי.

---

### 15:24–15:30 · דמו: Day1.Demo.Exceptions

<div dir="ltr">

```powershell
cd C:\c-\Day1-OOP-Collections-Exceptions\Demos\Day1.Demo.Exceptions
dotnet run
dotnet run -c Release     # אופציונלי: Debug.Assert/Debug.WriteLine נעלמים
```

</div>

1. **סעיף 1:** `arr[5]` נתפס ב-`IndexOutOfRangeException` הספציפי, ואחריו רץ `finally`.
2. **סעיף 2:** `Withdraw(30)` מצליח (נשאר 70). `Withdraw(500)` מחזיר shortfall של 430, ולכן הפילטר `when (ex.Shortfall > 100)` תופס. `Withdraw(-5)` מגיע ל-`ArgumentOutOfRangeException` מה-guard.
3. **סעיף 3 (עצרו כאן):** `throw;` מדפיס `Level2 (good)`, ו-`throw ex;` מדפיס `Level1 (origin lost)`.
4. **סעיף 4:** `Dispose(A)`, אחר כך `Dispose(B)`, ואחר כך `Dispose(C)` **לפני** ההודעה "caught after C disposed".
5. **סעיף 5:** guard clauses שדוחים `""` ו-`-1`, עם `ParamName`.
6. **סעיף 6:** `TryParse` על `"42"`, `"abc"`, `"3.5"` ו-`""`, בלי אף חריגה.
7. **סעיף 7:** `Debug.Assert`. אם יש זמן, הריצו `-c Release` והסבירו.

אם נשאר זמן (או כחלק מ-Lab 4): ב-Visual Studio, עם ה-Starter של Lab 4 פתוח, הראו Exception Settings. **לא** לחשוף את רשימת הבאגים.

---

### 15:30–15:33 · תרגול: תרגיל 13 ("נחשו את הפלט")

| # | כותרת | קושי | תשובה צפויה |
|---|---|---|---|
| 13 | סדר הריצה | ★★★ | בלי פילטר: `ctor A`, `ctor B`, `dispose B`, `catch`, `finally`, `dispose A`. עם `catch (Exception ex) when (Log(ex))`: **הפילטר רץ לפני `dispose B`**, כי פילטרים רצים בשלב החיפוש, לפני ה-unwinding. |

הכתיבו את הקוד מה-README על הלוח, בקשו מהכיתה לנחש, ואז הריצו:

<div dir="ltr">

```powershell
dotnet run -- 13
```

</div>

הפלט הצפוי של הפתרון: `ctor A`, `ctor B`, `filter saw: InvalidOperationException`, `dispose B`, `catch`, `finally`, `end of method`, `dispose A`.
תרגיל 12 (קלט בטוח + `ValidationException`, ★★) הולך הביתה.

---

### 15:33–15:40 · הרצאה: מודול 07, קוד נקי (שקפים 49–52)

1. **שמות** שמתארים כוונה, ומוסכמות C# (`PascalCase`, `_camelCase`, `I`, מתודות `bool` בצורת שאלה).
2. מתודות קטנות שעושות דבר אחד (Extract Method היא `Ctrl+R, Ctrl+M`). מספרי קסם מוחלפים ב-`const`/`static readonly`/`enum`. DRY מתייחס לידע, לא לטקסט.
3. **Refactoring לפני/אחרי (שקף 50):** הריצו את תרגיל 14 כדי להראות שההתנהגות זהה:

<div dir="ltr">

```powershell
dotnet run -- 14
```

</div>

   הפלט מציג `same=True` לכל ארבעת המקרים. מה השתנה: שמות, `enum CustomerType`, קבועים, הוצאת הכפילות `if (m) r -= 5`, ו-`switch` expression.
4. SOLID בקצרה (שקף 51). כל אות מקושרת לדוגמה מהיום: O = פולימורפיזם ב-Lab 2, D = `OrderService(ILogger)`.
5. `.editorconfig`, `dotnet format` ו-`--verify-no-changes` ב-CI. הערות מסבירות "למה", לא "מה".

**שאלה לכיתה:** "`Save(order, true, false)` — מה זה אומר?" (תשובה: פרמטרים בוליאניים לא קריאים. עדיף enum או שתי מתודות עם שמות.)

---

### 15:40–16:20 · מעבדה: Lab 3 או Lab 4

**איך לבחור** (לפי הערת המרצה ב-README): אם הקבוצה מהירה, Lab 3 בכיתה ו-Lab 4 בבית. אם הקבוצה מתקשה בחריגות, להפך.
לפי ה-README שתי המעבדות מתוכננות ל-60 דקות, אבל בכיתה יש רק 40. הגדירו יעד חלקי ברור (למטה), והשאר הולך הביתה.

#### אפשרות א': Lab 3, מלאי עם LINQ (שקף 42)

**מטרה:** `Dictionary` לחיפוש לפי SKU, `HashSet` לקטגוריות, `Func`/`Action` כפרמטרים, אירוע `LowStock` ודוחות LINQ טהורים.

**מה יש ב-Starter:** `Models.cs` (`record Product`, `StockItem` עם `IsLow`/`Value`, `LowStockEventArgs`) ו-`Program.cs` כבר כתובים. העבודה נעשית ב-`Inventory.cs` (TODO 1–9) וב-`Reports.cs` (TODO 10–14, ו-TODO 15 כבונוס). ב-`Program.cs` נשאר רק TODO 16, הרישום לאירוע.

**השלבים בקצרה:** `Inventory` (25 דק'): שדות עם `StringComparer.OrdinalIgnoreCase`, event, `Add`, `Get`, `Receive`, `Sell` עם `?.Invoke`, `Find(Func)` ו-`ApplyToCategory(Action)`. `Reports` (20 דק'): `TotalValue`, `ByCategory`, `TopByValue`, `LowStock` ו-`PriceIndex`. חיבור (15 דק').
**יעד ל-40 הדקות בכיתה:** `Inventory` מלא + TODO 16 + `TotalValue`/`ByCategory`.

**קריטריוני קבלה:** `Sell("k-100", 1)` עובד גם באותיות קטנות. מכירה של 8 Keyboards מדפיסה `!! LOW STOCK: Keyboard (4 left, reorder at 5)`. מכירה מעבר למלאי זורקת `InvalidOperationException` והתוכנית ממשיכה. `Find(i => i.Product.Price < 50)` מחזיר 3 פריטים. `ByCategory` מציג את `Displays` ראשון. `Reports` לא משנה את המלאי. אין `ToUpper`/`ToLower` בקוד.

**איפה נתקעים ואיזה רמז לתת:**

| מקום | רמז |
|---|---|
| `"k-100"` לא נמצא | `new Dictionary<string, StockItem>(StringComparer.OrdinalIgnoreCase)`, וכך גם ב-HashSet |
| `NullReferenceException` ב-`Sell` | `LowStock?.Invoke(this, new LowStockEventArgs(item));` |
| `Categories` נחשף כ-`HashSet` | TODO 3 מבקש `IReadOnlySet<string>` |
| `ByCategory` | `GroupBy(i => i.Product.Category).Select(g => new CategorySummary(g.Key, g.Count(), g.Sum(...), g.Sum(...))).OrderByDescending(...)` |
| `ApplyToCategory` | `.ToList()` לפני ה-`foreach` (ראו NOTES) |

**הפלט הצפוי של הפתרון** (לפי NOTES): אירועי LOW STOCK ל-Keyboard (4), Mouse (2) ו-USB-C Cable (5). דחייה של מכירת 999 ושל SKU שלא קיים. אחרי restock: USB-C 25, HDMI 28. ערך כולל 11,359.30. Top 3: Monitor, HDMI Cable, Mouse Pad. בסוף המסך מופיע ב-Low stock עם כמות 1.

**החלטות לדיון בסיכום** (מתוך [`NOTES.md`](../Day1-OOP-Collections-Exceptions/Labs/Lab3-InventoryLinq/Solution/NOTES.md)):
- טבלת בחירת האוספים: `Dictionary` לפי SKU, `HashSet` לקטגוריות, `IEnumerable` כלפי חוץ, ו-`ToDictionary` לאינדקס חד-פעמי.
- `Find(Func)` אחד במקום עשר מתודות `FindXxx`. זה הרעיון שמאחורי LINQ.
- `ToList()` ב-`ApplyToCategory`: אם ה-action מוחק פריט, בלי ה"צילום" מקבלים `Collection was modified`.
- `Inventory` לא יודע מי מאזין. זו הפרדה בין "מה קרה" לבין "מה עושים עם זה".
- `Reports` היא `static class` של פונקציות טהורות.

**בונוס:** `SkusInBoth` עם `IntersectWith`. מנוי שני שאוסף רשימת הזמנות. הדגמת deferred execution (שאילתה שמוגדרת לפני מכירה ומודפסת אחריה, עם ובלי `ToList()`). דוח אחד ב-query syntax.

#### אפשרות ב': Lab 4, בנק עמיד (שקף 53)

**מטרה:** הפעם לא כותבים מאפס אלא **מתקנים**. ה-Starter הוא בנק שמתקמפל ורץ, אבל מכיל באגים. המשתתפים מוצאים אותם עם הדיבאגר, מוסיפים היררכיית חריגות, ו-`using`/`finally`/`TryParse`.

**מה יש ב-Starter:** `Bank.cs` (`Account` + `Bank`), `Exceptions.cs` (רק `BankException` ו-TODO A), `TransactionLog.cs` (`IDisposable` מעל `StreamWriter`) ו-`Program.cs` (לולאת פקודות: `open`, `deposit`, `withdraw`, `transfer`, `show`, `list`, `help`, `quit`), עם TODO B ו-C.

**השלבים בקצרה:** ציד באגים עם הדיבאגר (25 דק', 7 הנחיות ב-README). עמידות לקלט (15 דק': `ParseAmount` עם `TryParse`, `RequireArgs`, `processed++` בתוך `finally`). חריגות מותאמות ו-`using` (20 דק').
**יעד ל-40 הדקות בכיתה:** שלב 1 (ציד באגים) + שלב 2.

**קריטריוני קבלה:** deposit 100 ואחריו withdraw 80 משאירים 20. `withdraw 500` על יתרה 70 מדפיס `short by 430.00`. `deposit acc1 abc`, `deposit acc1`, `deposit acc1 0` ו-`foo` לא מפילים את התוכנית. `transfer` באמת מעביר. `show` מציג עד 3 שורות. `ACC1` ו-`acc1` הם אותו חשבון. `transactions.log` מלא. יש `throw;`. `processed + failed` שווה למספר הפקודות. לפחות 3 חריגות יורשות מ-`BankException`.

**הבאגים שצריך להכיר** (רק למרצה, [`NOTES.md`](../Day1-OOP-Collections-Exceptions/Labs/Lab4-BankRobust/Solution/NOTES.md). **לא לחשוף מראש**):

| # | באג | הרמז שנותנים |
|---|---|---|
| 1 | `Deposit` בודק `amount < 0`, ולכן 0 עובר | "נסו `deposit acc1 0`. זה אמור להצליח?" |
| 2 | `Withdraw` מפחית לפני הבדיקה | "breakpoint ב-`Account.Withdraw`, F10, ו-Watch על `Balance`" |
| 3 | off-by-one ב-`RecentHistory` | "4 פעולות ואז `show`. כמה שורות? Watch על `start`" |
| 4 | `Open` הופך ל-uppercase ו-`Get` לא | "Immediate Window: `_accounts.Keys`" |
| 5 | `throw ex;` ב-`Bank.Withdraw` | "Exception Settings על `BankException`. איפה נעלם `Account.Withdraw` מה-Call Stack?" |
| 6 | `from.Deposit` במקום `to.Deposit` ב-`Transfer` | "`transfer` ואז `list`. הכסף עבר?" |
| 7 | `TransactionLog` לא נסגר, ולכן הלוג ריק | "פתחו את `transactions.log` אחרי `quit`" |
| 8 | `decimal.Parse` על `abc` | TODO B, `TryParse` |
| 9 | פרמטר חסר גורם ל-`IndexOutOfRangeException` | TODO C, guard `RequireArgs` |
| 10 | `processed++` בתוך ה-`try` | "`continue` בתוך `catch` לא מדלג על `finally`" |

**הליכת דיבוג מומלצת להדגמה** (מתוך NOTES): הרצף open, deposit 100, withdraw 80, show. ואז breakpoint ב-`Withdraw` עם Watch, Exception Settings ו-Call Stack, conditional breakpoint ב-`Transfer` (`amount > 40`), Immediate (`_accounts.Keys`), ולבסוף Edit & Continue לתיקון `from.Deposit` ל-`to.Deposit`.

**בדיקת הפתרון:**

<div dir="ltr">

```bash
cd Labs/Lab4-BankRobust/Solution
printf 'open acc1 Dana\ndeposit acc1 100\nwithdraw ACC1 30\nwithdraw acc1 500\ndeposit acc1 abc\ndeposit acc1\ndeposit acc1 0\nopen acc2 Yossi\ntransfer acc1 acc2 50\nlist\nshow acc1\nquit\n' | dotnet run
cat transactions.log
```

</div>

הצפוי: acc1 = 20 ו-acc2 = 50. ארבע שגיאות ידידותיות. `processed=11, failed=4`. ה-log מכיל 6 שורות.

**החלטות לדיון בסיכום:** היררכיית חריגות עם נתונים, כש-`catch (InsufficientFundsException)` בא **לפני** `catch (BankException)`. `TryParse` לקלט צפוי, אבל זריקת `ArgumentException` כדי לצאת מה-`switch` לנקודת טיפול אחת. `using` מבטיח flush. `finally` לספירה. rollback ב-`Transfer` הוא compensating action.

**בונוס:** rollback ב-`Transfer`, מניעת העברה לעצמו, conditional breakpoint, ו-`Debug.Assert(Balance >= 0)` שנבדק מול `-c Release`.

---

### 16:20–16:30 · סיכום היום

ראו את הסעיף [סיכום היום](#סיכום-היום) בהמשך: שאלות חזרה, שיעורי בית והצצה ליום 2. שקף 54 (ציטוט: "קוד נקרא הרבה יותר פעמים ממה שהוא נכתב") ושקף 55 (סיכום). שאלה לסגירה מתוך הערות השקף: "איזה כלי מהיום תשתמשו בו כבר מחר בעבודה?"

---

## אם מאחרים / אם מקדימים

### אם מאחרים (לפי סדר עדיפות, מה לקצץ ראשון)

1. **תרגולים שמשובצים אחרי דמו:** להפוך את תרגילים 3, 6 ו-10 לשיעורי בית. כל אחד חוסך 8–10 דקות. את תרגיל 9 להשאיר, כי הוא בעל-פה ומהיר.
2. **דמו Quickstart:** להריץ רק את סעיפים 2 (ערך/הפניה) ו-5 (nullable).
3. **מודול 04:** לדלג על `Span<T>`, capacity ומערכים דו-ממדיים. הם מופיעים ב-Notes.
4. **מודול 07:** להסתפק בהרצת `dotnet run -- 14` ובשקף SOLID (3 דקות).
5. **Lab 2:** לקצר את חלק א' (צורות). ה-Solution כבר מדגים אותו, ואפשר להתמקד בחלק ב' (IPayable/Payroll), שהוא העיקר.
6. **בלוק המעבדה האחרון:** אם נשארו פחות מ-25 דקות, לא להתחיל מעבדה חדשה. עדיף הדגמת דיבוג חיה על ה-Starter של Lab 4 (הליכת הדיבוג מ-NOTES, בלי לחשוף את כל הרשימה), ולשלוח את שתי המעבדות הביתה.
7. **לא לקצץ:** ערך/הפניה, record מול class, פולימורפיזם, deferred execution, `throw;` מול `throw ex;`. כל אלה חוזרים בימים הבאים.

### אם מקדימים

- **בבוקר:** תרגיל 4 (★★) לכולם, ובונוס Lab 1 (`DueDate`/`IsOverdue`).
- **אחרי מודול 03:** תרגיל 5 (★★) כחימום לפני Lab 2.
- **אחרי מודול 04:** תרגיל 8 (`MyStack<T>`, ★★): גנריקה, constraint `notnull` וגדילה פי 2.
- **אחרי מודול 05:** תרגיל 11 (★★): Compose ואירוע `Counter`.
- **אחרי מודול 06:** תרגיל 12 (★★) עם הרצה אינטראקטיבית (`dotnet run -- 12`).
- **בסוף היום:** להריץ את **שתי** המעבדות. מי שסיים את Lab 3 עובר ל-Lab 4, או להפך. בונוסים: deferred execution ב-Lab 3, rollback ב-Lab 4.
- **העמקה:** להראות `dotnet new editorconfig` ו-`dotnet format` על אחד הפרויקטים, או לפתור את תרגיל 14 בחי עם כלי ה-refactoring של Visual Studio (`Ctrl+R, Ctrl+M`, `F2`).

---

## סיכום היום

### שאלות חזרה (עם תשובות קצרות)

1. **`int[] b = a; b[0] = 9;` — מה קרה ל-`a`? ואם היה מדובר ב-`int`?**
   מערך הוא reference type, ולכן `a[0]` שווה 9 (שני שמות לאותו אובייקט). `int` מועתק לפי ערך, ולכן `a` לא היה משתנה.
2. **מתי `record` ומתי `class`?**
   `record` מתאים לנתונים שמזוהים לפי התוכן: שוויון לפי ערך, `with`, immutable. `class` מתאים לישויות עם זהות ומצב שמשתנה, כמו `BankAccount` או `Book`.
3. **למה `Contractor` מממש `IPayable` ולא יורש מ-`Employee`?**
   קבלן הוא לא עובד (אין לו Id ואין לו מנהל). הממשק מאפשר ל-`Payroll` לעבוד עם כל מי שמקבל תשלום. הקוד תלוי בחוזה ולא במימוש.
4. **צריך לבדוק שייכות של 100K מזהים. איזה אוסף, ולמה?**
   `HashSet<T>` (או `Dictionary` כשיש ערך מקושר), כי הבדיקה היא O(1). `List.Contains` הוא O(n), ובלולאה זה O(n²).
5. **מה זה deferred execution, ואיך "מקפיאים" תוצאה?**
   `Where`/`Select` רק בונים שאילתה, והיא רצה כשצורכים אותה (`foreach`, `Count`, `First`). אם המקור השתנה בינתיים, התוצאה תשקף את השינוי. `ToList()`/`ToArray()` מבצעים את השאילתה ומקפיאים את התוצאה.
6. **`throw;` מול `throw ex;`, ומתי `TryParse` ולא `try/catch`?**
   `throw;` שומר על ה-stack trace המקורי, ו-`throw ex;` מאפס אותו. `TryParse` מתאים לקלט צפוי שעלול להיות שגוי. חריגות שמורות להפרת כללים או למצבים חריגים באמת.

### שיעורי בית

- **המעבדה שלא נעשתה בכיתה:** Lab 3 או Lab 4, עד כל קריטריוני הקבלה. מי שעשה בכיתה רק חלק ממעבדה, משלים אותה.
- **תרגילים:** 4, 5, 7, 8, 11, 12, 14 (ו-1 אם לא נפתר). ההנחיה בשקף הסיום היא להשלים את כל התרגילים ★★.
- **קריאה:** Notes 04–07 למי שהרגיש שהקצב אחר הצהריים היה מהיר מדי.
- להשוות כל פתרון מול `Solution/NOTES.md`: "ההחלטות שלי דומות? ואם לא, אני יודע למה?"

### הצצה ליום 2

ביום 2 ([`Day2-Async-APIs`](../Day2-Async-APIs/README.md)) עוברים לעבודה במקביל ולעולם שמחוץ לתוכנית. הנושאים הם threads ו-ThreadPool, `Task` ו-`async`/`await`, סנכרון (`lock`, `Interlocked`, `SemaphoreSlim`, `Channel<T>`), קריאות REST עם `HttpClient` ו-JSON עם `System.Text.Json`.
הצצה שתעבוד טוב: "הבנק מ-Lab 4 עובד מצוין עם משתמש אחד. מה יקרה כשמאה threads ימשכו כסף מאותו חשבון בו-זמנית?" זו בדיוק Lab 2 של מחר, "בנק בטוח לתהליכונים". ביום 2 גם ירוץ API מקומי (`Demos/Day2.LocalApi`) בטרמינל נפרד לאורך כל היום.

</div>
