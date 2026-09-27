<div dir="rtl">

# יום 4 — פיתוח בעזרת AI וקוד ניתן לתחזוקה

**C# ב-.NET — קורס מעשי | יום 4 (יום הסיום)**

ביום הזה נלמד איך כלי AI (Copilot, Claude, Cursor ואחרים) משתלבים בעבודת מפתח .NET: מ-prompt טוב, דרך סקירה קפדנית של הפלט, ועד ארכיטקטורה ומדיניות ארגונית שמאפשרות להשתמש בהם בבטחה. המושב האחרון של הקורס מוקדש ל-**כלי AI לפיתוח ממשק משתמש**.

## מטרות למידה
בסוף היום תוכלו:
- לבחור את מצב העבודה הנכון (השלמה / צ'אט / סוכן) לכל משימה, ולהגדיר את הכלים ב-Visual Studio ו-VS Code.
- לנסח prompts למפתחים (Role/Context/Task/Constraints/Examples/Output) ולתחזק קובצי הוראות (`CLAUDE.md`, `copilot-instructions.md`).
- להאיץ כתיבת DTOs, LINQ, בדיקות, refactoring ומיגרציות — ולדעת מה לבדוק אחרי.
- לסקור קוד שנוצר ב-AI לפי צ'ק-ליסט, ולזהות באגים עדינים (async, culture, disposal, thread-safety, אבטחה).
- לבנות אפליקציית .NET בשכבות עם DI, configuration ו-logging — גם ב-WPF.
- להגדיר מדיניות ארגונית: פרטיות, סודות, רישוי, שערי סקירה, CI, מדידה.
- לבנות UI ל-WPF (ולא רק) בעזרת AI — מסקיצה ל-XAML מחובר ל-ViewModel.

## לוח זמנים
| שעה | נושא | חומר |
|-----|------|------|
| 09:00–09:45 | מודול 1: תהליכי פיתוח עם AI — השלמה, צ'אט, סוכנים; הגדרת כלים; human in the loop | [Notes/01](Notes/01-ai-dev-workflows.md) |
| 09:45–10:30 | מודול 2: Prompt Engineering למפתחים; קובצי הוראות; MCP | [Notes/02](Notes/02-prompt-engineering-for-devs.md) |
| 10:30–10:45 | הפסקה | |
| 10:45–11:30 | מודול 3: האצת כתיבת קוד — DTOs, LINQ, בדיקות, refactoring, מיגרציות | [Notes/03](Notes/03-code-generation-acceleration.md) |
| 11:30–12:30 | **Lab 1** — פיצ'ר ממפרט עם AI, בדיקות קודם | [Labs/Lab1](Labs/Lab1-FeatureWithAi/README.md) |
| 12:30–13:15 | הפסקת צהריים | |
| 13:15–13:50 | מודול 4: סקירה ואימות של קוד AI | [Notes/04](Notes/04-reviewing-ai-code.md) |
| 13:50–14:35 | **Lab 3** — סקירת קוד AI (45 דק') | [Labs/Lab3](Labs/Lab3-ReviewAiCode/README.md) |
| 14:35–15:05 | מודול 5: ארכיטקטורה ניתנת לתחזוקה — שכבות, DI, options, logging, SOLID | [Notes/05](Notes/05-maintainable-architecture.md) |
| 15:05–15:15 | הפסקה | |
| 15:15–15:35 | מודול 6: שילוב AI בארגון — מדיניות, אבטחה, CI, מדידה | [Notes/06](Notes/06-enterprise-ai-integration.md) |
| 15:35–16:10 | **מודול 7 (מושב הסיום): כלי AI לפיתוח UI** — דמו חי: מ-prompt ל-XAML | [Notes/07](Notes/07-ai-tools-for-ui-development.md) |
| 16:10–16:30 | סיכום הקורס, "מה הלאה", שאלות | |

> Lab 2 (Refactoring legacy, 75 דק') ו-Lab 4 (Capstone WPF, 75 דק') מיועדים לעבודה עצמית / יום תרגול נוסף, או להחלפה עם Lab 1/Lab 3 לפי הקבוצה. מומלץ: Lab 4 כמשימת סיום ביתית.

## מודולים
1. [תהליכי פיתוח עם AI](Notes/01-ai-dev-workflows.md)
2. [Prompt Engineering למפתחים](Notes/02-prompt-engineering-for-devs.md)
3. [האצת כתיבת קוד](Notes/03-code-generation-acceleration.md)
4. [סקירה ואימות של קוד AI](Notes/04-reviewing-ai-code.md)
5. [ארכיטקטורה ניתנת לתחזוקה](Notes/05-maintainable-architecture.md)
6. [שילוב AI בסביבה ארגונית](Notes/06-enterprise-ai-integration.md)
7. [**כלי AI לפיתוח UI — מושב הסיום**](Notes/07-ai-tools-for-ui-development.md)

## מעבדות
| מעבדה | משך | נושא |
|-------|-----|------|
| [Lab 1 — FeatureWithAi](Labs/Lab1-FeatureWithAi/README.md) | 60 דק' | מנוע הנחות ממפרט, בדיקות xUnit קיימות, prompt worksheet |
| [Lab 2 — RefactorLegacy](Labs/Lab2-RefactorLegacy/README.md) | 75 דק' | refactoring של קוד legacy עם golden master, DI ובדיקות |
| [Lab 3 — ReviewAiCode](Labs/Lab3-ReviewAiCode/README.md) | 45 דק' | 7 קטעי קוד "מ-AI" עם באגים מוסתרים — סקירה ותיקון |
| [Lab 4 — AiBuiltWpfApp](Labs/Lab4-AiBuiltWpfApp/README.md) | 75 דק' | Capstone: Expense Tracker ב-WPF מקצה לקצה עם AI |

## דמואים (Demos/)
| פרויקט | מה מדגים |
|--------|----------|
| `Day4.Demo.LegacyMess` | קוד "עובד אבל מבולגן" — נקודת פתיחה ל-refactoring עם AI |
| `Day4.Demo.Refactored` + `.Tests` | אותו פלט, מבנה נקי: ממשקים, DI, decimal, 26 בדיקות + golden master |
| `Day4.Demo.DiHostWpf` | WPF עם `Microsoft.Extensions.Hosting`: DI, `appsettings.json`, `ILogger` |
| `Day4.Demo.AiGeneratedUi` | חלון dashboard כפי שנוצר ב-AI אחרי סקירה: ResourceDictionary, empty state, design-time data |
| `Prompts/` | תבניות prompt (עברית/אנגלית), `CLAUDE.md`, `copilot-instructions.md`, `.cursorrules` |

## תרגילים
[Exercises/README.md](Exercises/README.md) — 12 תרגילים קצרים; פתרונות ב-`Exercises/Solutions` (`dotnet run -- <n>`) ו-`PROMPTS-ANSWERS.md`.

## מה צריך שיהיה מוכן היום
- .NET 10 SDK, Visual Studio 2022 או VS Code + C# Dev Kit — ראו [../00-Setup/INSTALL.md](../00-Setup/INSTALL.md).
- **חשבון וגישה לכלי AI אחד לפחות** (מומלץ שניים): GitHub Copilot (בתוך VS/VS Code), Claude (אתר/Claude Code), ChatGPT, Cursor או JetBrains AI. בדקו מראש שההתחברות עובדת ושאתם יודעים מה מדיניות הארגון לגבי הדבקת קוד.
- git מותקן (ל-diff ול-commit בין צעדי refactoring).
- ל-Lab 4: Windows (WPF). ב-Linux/macOS הפרויקטים מתקמפלים אך אינם רצים.

## הערה על כנות התוכן
כלי ה-AI משתנים מהר. החומר מתאר יכולות **כלליות** לפי קטגוריה; מחירים, גרסאות ותכונות ספציפיות — בדקו בתיעוד הרשמי של הכלי (קישורים בכל מודול).

</div>
