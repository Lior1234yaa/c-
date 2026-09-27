<div dir="rtl">

# מודול 2 — Prompt Engineering למפתחים

## למה זה חשוב

איכות הקוד שתקבלו מכלי AI תלויה כמעט לגמרי באיכות הבקשה. "תכתוב לי שירות הזמנות" מחזיר ניחוש; "כתוב `OrderService` שמממש את `IOrderService` המצורף, משתמש ב-`IOrderRepository` דרך DI, זורק `ValidationException` על סכום שלילי, ומגיע עם בדיקות xUnit" מחזיר קוד שאפשר להתחיל לבדוק. במודול הזה נלמד לנסח בקשות כמו מפרט טכני קצר.

## האנטומיה של Prompt טוב למפתח

Prompt טוב מכיל שישה מרכיבים (לא כולם חובה בכל פעם):

| מרכיב | שאלה שהוא עונה עליה | דוגמה |
|-------|----------------------|-------|
| **Role** (תפקיד) | מי "מדבר"? | "You are a senior .NET developer who values readable code." |
| **Context** (הקשר) | באיזה פרויקט/גרסה/ארכיטקטורה? | ".NET 10, WPF, MVVM, System.Text.Json, no third-party MVVM libs." |
| **Task** (משימה) | מה בדיוק לעשות? | "Implement `CalculateDiscount` in the class below." |
| **Constraints** (אילוצים) | מה אסור/חובה? | "No new NuGet packages. Must be thread-safe. Keep public API unchanged." |
| **Examples** (דוגמאות) | איך נראה קלט/פלט נכון? | "Input: 3 items of 100 → 285 (5% off over 250)." |
| **Output format** (פורמט) | מה להחזיר? | "Return only the C# file, then a 3-line summary of decisions." |

דוגמה מלאה:

<div dir="ltr">

```text
You are a senior C# developer.
Context: .NET 10 console app, nullable enabled, file-scoped namespaces.
Task: Write a static method `ParseCsvLine(string line)` returning `string[]`
that supports quoted fields with commas and escaped quotes ("").
Constraints: no Regex, no external packages, O(n).
Examples:
  a,b,c        -> ["a","b","c"]
  "x, y",z     -> ["x, y","z"]
  "say ""hi""" -> ["say \"hi\""]
Output: the method only, with XML doc comments, plus 5 xUnit test cases.
```

</div>

## תנו לכלי את הקוד — "Give the code context"

הכלי לא יודע מה יש בפרויקט שלכם אלא אם הראיתם לו. דרכים לתת הקשר:

- **הדבקת הקוד הרלוונטי** (הממשק, המודל, הבדיקה שנכשלת) — לא את כל הפרויקט.
- **בחירת קובץ/מקטע ב-IDE**: Copilot Chat ו-Cursor מוסיפים את הבחירה להקשר; ב-Copilot אפשר לציין `#file` וב-Cursor `@file`.
- **סוכנים** (Claude Code, Copilot agent) קוראים בעצמם — אבל כדאי לכוון: "Look at `Services/OrderService.cs` and its tests first."
- **הודעות שגיאה מלאות**: הדביקו את השגיאה של הקומפיילר/הבדיקה כמו שהיא, כולל stack trace.

## שיפור איטרטיבי

אל תצפו לפלט מושלם בפעם הראשונה. לולאה טובה:

1. בקשה ראשונה קצרה עם הקשר.
2. build/test → הדבקת השגיאה: "This fails with CS0246: The type 'X' could not be found. Fix without adding packages."
3. חידוד: "Good. Now make it handle empty input by returning an empty array instead of throwing."
4. ניקוי: "Remove the comments that restate the code, keep only the XML summary."

טיפ: כשהשיחה מתארכת והכלי "שוכח", פתחו שיחה חדשה עם סיכום קצר של ההחלטות עד כה.

## בקשו בדיקות קודם (Tests First)

תבנית חזקה במיוחד: לבקש קודם בדיקות, לקרוא אותן (קל יותר לבדוק בדיקות מאשר מימוש), ורק אז לבקש מימוש שעובר אותן.

<div dir="ltr">

```text
Before implementing, write xUnit tests for `IDiscountRule.Apply(Order)` based on this spec:
- 5% off when subtotal > 250
- 10% off when quantity >= 10
- rules never stack; the larger discount wins
- never a negative total
Use [Theory] with InlineData. Do not implement yet.
```

</div>

אחרי שאישרתם את הבדיקות: "Now implement the rules so the tests pass. Show the diff only."

## בקשו חלופות ו-trade-offs

<div dir="ltr">

```text
Give me 2–3 ways to persist app settings in a WPF app (JSON file, registry, user settings).
For each: 3 lines of pros/cons and when you would choose it. No code yet.
```

</div>

כך אתם מקבלים החלטת עיצוב במקום מימוש שרירותי.

## Prompts של הבנה ואבחון

- **הסבר קוד**: "Explain this method line by line. Then list any assumptions it makes about its input."
- **מצא את הבאג**: "This test fails with `Expected 285, got 300`. Here is the code and the test. Find the most likely cause; do not rewrite the whole method."
- **ברווז גומי (Rubber duck)**: "I'll describe my approach; ask me questions that reveal holes before I write code." — הכלי מצוין בלשאול "מה קורה אם…?".
- **ניתוח שגיאה**: הדביקו stack trace ובקשו "Explain what this exception means and the 3 most common causes in WPF."

## ספריית תבניות (מוכנות לשימוש)

התבניות המלאות (עברית + אנגלית) נמצאות ב-`Demos/Prompts/`. הנה 12 בקיצור:

1. **מחלקה מ-JSON**: "Generate C# records for this JSON. Use `System.Text.Json` attributes only where the property name differs. Nullable where the value can be null."
2. **שאילתת LINQ**: "Write a LINQ query (method syntax) over `IEnumerable<Order>` that returns the top 5 customers by total amount in 2025, with (CustomerName, Total). Explain complexity."
3. **INotifyPropertyChanged**: "Convert this class to an MVVM ViewModel with `INotifyPropertyChanged`, a `SetProperty` helper and a `RelayCommand` — no external MVVM libraries."
4. **בדיקות**: "Write xUnit tests for the class below. Cover happy path, boundary values, null/empty input, and one exception case. Name tests `Method_Scenario_Expected`."
5. **Refactor**: "Refactor this method: extract smaller methods with intention-revealing names, replace magic numbers with named constants, keep behavior identical. Show only the changed code."
6. **Explain**: "Explain this code to a junior developer. Then list what could go wrong in production."
7. **Find the bug**: "The following code has at least one bug related to {async/culture/disposal}. Find it, explain, and fix minimally."
8. **Regex**: "Write a .NET regex for Israeli phone numbers (05X-XXXXXXX with optional dash). Provide 5 matching and 5 non-matching examples and an xUnit test."
9. **Migration**: "Migrate this class from Newtonsoft.Json to System.Text.Json. Keep property names in the output identical. Point out behaviors that differ."
10. **XML docs**: "Add XML documentation comments to all public members. Be concise; do not document obvious getters."
11. **Commit message**: "Write a conventional-commit message for this diff. Subject ≤ 72 chars, body explains why."
12. **XAML**: "Generate a WPF window (XAML only) for {description}: Grid layout, FlowDirection=RightToLeft, bindings to `{ViewModel}` properties, no code-behind logic. Use `{StaticResource}` for colors."

## קובצי הוראות לפרויקט (Instruction files)

במקום לחזור על ההקשר בכל בקשה, שמים אותו בקובץ שהכלי קורא אוטומטית:

| כלי | קובץ |
|-----|------|
| Claude Code | `CLAUDE.md` בשורש הפרויקט (ואפשר גם בתת-תיקיות) |
| GitHub Copilot | `.github/copilot-instructions.md` (ואפשר קובצי `*.instructions.md` נוספים) |
| Cursor | `.cursorrules` או `.cursor/rules/*.mdc` |
| Windsurf, JetBrains AI | יש מנגנון דומה — בדקו בתיעוד העדכני |

מה שמים בקובץ: מבנה הפרויקט, טכנולוגיות וגרסאות, מוסכמות קוד, מה אסור, איך בונים ובודקים, ודוגמת קוד "כמו שאנחנו כותבים".

דוגמה מלאה ל-`CLAUDE.md` עבור פתרון WPF (הגרסה המלאה ב-`Demos/Prompts/CLAUDE.md`):

<div dir="ltr">

```text
# Orders Desktop — project instructions

## Stack
- .NET 10, C# 14, WPF (net10.0-windows), MVVM without third-party frameworks.
- Serialization: System.Text.Json only. HTTP: HttpClient via IHttpClientFactory.
- Tests: xUnit. Logging: Microsoft.Extensions.Logging.

## Structure
- src/Orders.Domain      -> entities, value objects, rules (no dependencies)
- src/Orders.Application -> services, interfaces, DTOs
- src/Orders.Infrastructure -> JSON repositories, HTTP clients
- src/Orders.Wpf         -> Views (XAML), ViewModels, App.xaml.cs (DI host)
- tests/Orders.Tests

## Conventions
- File-scoped namespaces, nullable enabled, `var` when type is obvious.
- Public API gets XML docs. Private fields `_camelCase`.
- No logic in code-behind; ViewModels expose ICommand + INotifyPropertyChanged.
- Never use DateTime.Now for business logic; inject IClock.
- Never swallow exceptions; log and rethrow or translate to a domain exception.
- All windows set FlowDirection="RightToLeft" and use resources from Themes/Colors.xaml.

## Commands
- Build: dotnet build
- Test:  dotnet test
- Format: dotnet format --verify-no-changes

## When adding a feature
1. Write/extend tests in tests/Orders.Tests first.
2. Keep changes inside the layer that owns them.
3. Do not add NuGet packages without asking.
```

</div>

הקובץ הזה חוסך זמן, אבל חשוב יותר: הוא **מתעד את המוסכמות לבני אדם** באותה הזדמנות.

## MCP — Model Context Protocol (בקצרה)

MCP הוא פרוטוקול פתוח שמאפשר לכלי AI להתחבר ל"שרתי הקשר": מסד נתונים, GitHub, מערכת טיקטים, תיעוד פנימי, או כלי build. במקום להדביק ידנית, הכלי שואל את המקור. Claude Code, Copilot, Cursor ואחרים תומכים בו (בדקו את התיעוד של הכלי שלכם). פרטים: https://modelcontextprotocol.io

מבחינת המפתח: MCP = עוד דרך לתת הקשר אמין, אך גם עוד ערוץ שדרכו נתונים יוצאים — ראו מדיניות במודול 6.

## טעויות נפוצות

- **בקשה בלי הקשר** ("תכתוב repository") → הכלי ממציא מודל, גרסה וספרייה.
- **בקשה בלי קריטריון הצלחה** → אין דרך לדעת אם הפלט נכון. תמיד: דוגמה או בדיקה.
- **"תתקן הכול"** → שינויים ענקיים שאי אפשר לסקור. בקשו "minimal change".
- **להתעלם משאלות של הכלי** — כשהוא שואל "האם X או Y?" זו ההזדמנות לדייק.
- **אותה שיחה לנצח** — שיחה ארוכה מצטברת "רעש"; פתחו חדשה עם סיכום.

## לסיכום

- Prompt = Role + Context + Task + Constraints + Examples + Output.
- תנו קוד, שגיאות ודוגמאות; עבדו באיטרציות קטנות.
- בדיקות קודם, חלופות לפני מימוש, "הסבר" ו"מצא באג" הם כלי עבודה יומיומיים.
- קובצי הוראות (`CLAUDE.md`, `copilot-instructions.md`, `.cursorrules`) הופכים את ההקשר לקבוע — ומתעדים מוסכמות.
- MCP מרחיב את ההקשר למקורות חיצוניים.

## קריאה נוספת

- Copilot prompt engineering: https://docs.github.com/copilot/using-github-copilot/prompt-engineering-for-github-copilot
- Copilot custom instructions: https://docs.github.com/copilot/customizing-copilot/adding-repository-custom-instructions-for-github-copilot
- Claude Code — CLAUDE.md and memory: https://docs.claude.com/en/docs/claude-code/memory
- Prompt engineering (Anthropic docs): https://docs.claude.com/en/docs/build-with-claude/prompt-engineering/overview
- MCP: https://modelcontextprotocol.io

</div>
