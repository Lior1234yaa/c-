<div dir="rtl">

# יום 1 — תכנות מונחה-עצמים, אוספים וטיפול בחריגות

היום הראשון בקורס **"C# Programming in the .NET Framework — Updated Practical Training"**. מתחילים מהבסיס של השפה, בונים את המודל האובייקטי, לומדים לנהל נתונים באוספים הנכונים, מוסיפים LINQ ו-delegates, ומסיימים בקוד שלא נופל — עם חריגות, דיבוג וקוד נקי. רוב היום הוא ידיים על המקלדת: 4 מעבדות ו-14 תרגילים.

## מה צריך להתקין לפני היום

ראו [`../00-Setup/INSTALL.md`](../00-Setup/INSTALL.md). בקצרה:

- .NET SDK 10 (`dotnet --version` מדפיס `10.x`)
- Visual Studio 2022+ (workload ".NET desktop development") **או** VS Code עם C# Dev Kit
- Git

בדיקה מהירה:

<div dir="ltr">

```bash
dotnet new console -n Hello && cd Hello && dotnet run
```

</div>

## מטרות למידה

בסוף היום תוכלו:

1. לכתוב תוכנית C# מודרנית: top-level statements, `var`, אינטרפולציה, `switch` expressions, nullable reference types.
2. לעצב מחלקות עם properties, בנאים (כולל primary constructors) ו-`record`, ולהסביר מתי כל אחד מתאים.
3. ליישם את ארבעת עקרונות ה-OOP ב-C#: `private`/`protected`, `virtual`/`override`/`abstract`, ממשקים (כולל default members), `is`/`switch` patterns.
4. לבחור את האוסף הנכון (`List`, `Dictionary`, `HashSet`, `Queue`, `Stack`) לפי Big-O, לכתוב מחלקה גנרית עם constraints, ולחשוף אוספים בבטחה (`IReadOnlyList<T>`).
5. להעביר התנהגות עם `Func`/`Action`, להגדיר ולהפעיל אירועים, ולכתוב שאילתות LINQ (`Where/Select/OrderBy/GroupBy/ToDictionary`) תוך הבנת deferred execution.
6. לטפל בחריגות נכון: `try/catch/finally/when`, חריגות מותאמות, `throw;`, `using`, guard clauses ו-`TryParse`.
7. לדבג ב-Visual Studio: breakpoints (גם מותנים), Watch, Immediate, Call Stack, Exception Settings, Edit & Continue.
8. לזהות ולתקן "ריחות" בקוד: שמות, מספרי קסם, כפילויות, מחלקות גדולות — עם SOLID ו-`dotnet format`.

## סדר יום

| שעה | נושא | חומרים |
|---|---|---|
| 09:00–09:15 | פתיחה, היכרות, הכנת הסביבה | — |
| 09:15–09:50 | **מודול 01** — חימום C#: תוכנית, טיפוסים, בקרת זרימה, מתודות, nullable | [Notes/01](Notes/01-csharp-quickstart.md), [Demo Quickstart](Demos/Day1.Demo.Quickstart/) |
| 09:50–10:30 | **מודול 02** — מחלקות, אובייקטים, properties, בנאים, records | [Notes/02](Notes/02-classes-objects.md), [Demo Classes](Demos/Day1.Demo.Classes/) |
| 10:30–10:45 | הפסקה | |
| 10:45–11:30 | **Lab 1** — מערכת ספרייה | [Labs/Lab1](Labs/Lab1-LibrarySystem/README.md) |
| 11:30–12:15 | **מודול 03** — אנקפסולציה, ירושה, פולימורפיזם, ממשקים, הפשטה | [Notes/03](Notes/03-oop-pillars.md), [Demo Polymorphism](Demos/Day1.Demo.Polymorphism/) |
| 12:15–13:00 | ארוחת צהריים | |
| 13:00–14:00 | **Lab 2** — צורות ועובדים (payroll) | [Labs/Lab2](Labs/Lab2-ShapesAndEmployees/README.md) |
| 14:00–14:30 | **מודול 04** — אוספים, גנריקה, ניהול נתונים יעיל | [Notes/04](Notes/04-collections.md), [Demo Collections](Demos/Day1.Demo.Collections/) |
| 14:30–14:45 | הפסקה | |
| 14:45–15:15 | **מודול 05** — delegates, lambdas, events, LINQ | [Notes/05](Notes/05-delegates-lambdas-linq.md), [Demo LinqDelegates](Demos/Day1.Demo.LinqDelegates/) |
| 15:15–15:40 | **מודול 06 + 07** — חריגות, דיבוג, קוד נקי | [Notes/06](Notes/06-exceptions-debugging.md), [Notes/07](Notes/07-clean-code.md), [Demo Exceptions](Demos/Day1.Demo.Exceptions/) |
| 15:40–16:20 | **Lab 3 או Lab 4** (לבחירה; השני כשיעורי בית) | [Labs/Lab3](Labs/Lab3-InventoryLinq/README.md), [Labs/Lab4](Labs/Lab4-BankRobust/README.md) |
| 16:20–16:30 | סיכום, שאלות, הכנה ליום 2 | |

הערה למרצה: מודולים 06–07 קצרים בהרצאה כי Lab 4 מכסה אותם בפועל. אם הקבוצה מהירה — Lab 3 בכיתה ו-Lab 4 בבית; אם הקבוצה מתקשה בחריגות — להפך.

## מודולים (חומרי הלימוד)

| # | מודול | נושאים |
|---|---|---|
| 01 | [חימום C#](Notes/01-csharp-quickstart.md) | Program.cs, `dotnet new/run`, value vs reference, `var`, מחרוזות, בקרת זרימה, מתודות, nullable, namespaces, VS/VS Code |
| 02 | [מחלקות ואובייקטים](Notes/02-classes-objects.md) | class/object/`new`, שדות מול properties (auto/init/required/computed), בנאים ו-primary constructors, `this`, static, records, object initializers |
| 03 | [עקרונות OOP](Notes/03-oop-pillars.md) | access modifiers, `base`/`virtual`/`override`/`sealed`/`abstract`, פולימורפיזם, ממשקים + default members, הפשטה, composition, `is`/`as`/patterns, `ToString`/`Equals`/`GetHashCode` |
| 04 | [אוספים וגנריקה](Notes/04-collections.md) | מערכים, `List`, `Dictionary`, `HashSet`, `Queue`/`Stack`, `IEnumerable`/`yield`, `Repository<T>` + constraints, טבלת Big-O, `IReadOnlyList`, capacity, `Span<T>` |
| 05 | [Delegates, Lambdas, LINQ](Notes/05-delegates-lambdas-linq.md) | delegates, `Func`/`Action`/`Predicate`, closures, events, LINQ, method vs query syntax, deferred execution, סגנון פונקציונלי |
| 06 | [חריגות ודיבוג](Notes/06-exceptions-debugging.md) | try/catch/finally, `when`, חריגות מותאמות, `throw;`, `using`/IDisposable, guard clauses, TryParse, כלי הדיבאגר, `Debug.Assert` |
| 07 | [קוד נקי](Notes/07-clean-code.md) | שמות, מתודות קטנות, SOLID, DRY, קבועים/enums, הערות, `.editorconfig`/`dotnet format`, refactoring, קוד רב-שימושי |

## דמואים (live coding)

כל דמו הוא פרויקט קונסולה עצמאי: `cd Demos/<Project> && dotnet run`.

| פרויקט | מודול | מה מראה |
|---|---|---|
| [Day1.Demo.Quickstart](Demos/Day1.Demo.Quickstart/) | 01 | ערך/הפניה, `var`, switch expression, מתודות, nullable |
| [Day1.Demo.Classes](Demos/Day1.Demo.Classes/) | 02 | properties, בנאים, primary constructor, static, record מול class |
| [Day1.Demo.Polymorphism](Demos/Day1.Demo.Polymorphism/) | 03 | abstract/virtual/override, ממשקים, pattern matching, Equals, composition |
| [Day1.Demo.Collections](Demos/Day1.Demo.Collections/) | 04 | כל האוספים, `yield`, `Repository<T>`, `IReadOnlyList`, capacity, `Span` |
| [Day1.Demo.LinqDelegates](Demos/Day1.Demo.LinqDelegates/) | 05 | delegates, closures, events, LINQ, deferred execution |
| [Day1.Demo.Exceptions](Demos/Day1.Demo.Exceptions/) | 06 | try/catch/finally/when, `throw;`, using, guard clauses, TryParse, Debug.Assert |

## תרגילים

[Exercises/README.md](Exercises/README.md) — 14 תרגילים קצרים (★–★★★) לפי מודול. פתרונות: `cd Exercises/Solutions && dotnet run -- <מספר>`.

## מעבדות

| מעבדה | משך | נושא | מה מתרגלים |
|---|---|---|---|
| [Lab 1 — מערכת ספרייה](Labs/Lab1-LibrarySystem/README.md) | 45 דק' | `Book`, `Library`, `List<Book>` | properties, בנאים עם ולידציה, חיפוש, השאלה/החזרה, `IReadOnlyList` |
| [Lab 2 — צורות ועובדים](Labs/Lab2-ShapesAndEmployees/README.md) | 60 דק' | היררכיית צורות + payroll עם `IPayable` | abstract, virtual/override, ממשקים, פולימורפיזם, pattern matching |
| [Lab 3 — מלאי עם LINQ](Labs/Lab3-InventoryLinq/README.md) | 60 דק' | `Inventory` + `Reports` | `Dictionary`, `HashSet`, `Func`/`Action` כפרמטרים, אירוע `LowStock`, LINQ, deferred execution |
| [Lab 4 — בנק עמיד](Labs/Lab4-BankRobust/README.md) | 60 דק' | starter מלא באגים לתיקון | חריגות מותאמות, guard clauses, `TryParse`, `using`/`finally`, דיבוג ב-Visual Studio |

לכל מעבדה: `README.md` (הוראות), `Starter/` (מתקמפל, עם `TODO`), `Solution/` (פתרון מלא + `NOTES.md`).

## מצגת

[Slides/Day1.pptx](Slides/Day1.pptx) (נבנית מ-`Slides/Day1.slides.js`, ראו `tools/slides/README.md`).

## הכנה ליום 2

ביום 2 נעבור ל-.NET עצמו: קבצים ו-JSON, async/await, HttpClient, בדיקות יחידה ו-Dependency Injection. ודאו ש-Lab 3 ו-Lab 4 הושלמו — נשתמש במודל של הבנק והמלאי.

</div>
