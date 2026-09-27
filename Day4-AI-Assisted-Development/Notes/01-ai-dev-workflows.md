# מודול 1 — תהליכי פיתוח בעזרת AI (AI-Assisted Development Workflows)

## למה אנחנו כאן

בשלושת הימים הקודמים כתבנו קוד "ביד": מחלקות, LINQ, `async/await`, `HttpClient`, ואפליקציות WPF עם MVVM. היום נלמד איך כלי AI משנים את **אופן העבודה** של מפתח .NET — לא במקום הידע שרכשתם, אלא **על גביו**. הנקודה החשובה ביותר של היום: כלי AI מייצרים קוד מהר, אבל **האחריות על הקוד נשארת שלכם**. מפתח שלא מבין מה ה-AI כתב לא יכול לבדוק אותו, לתחזק אותו או להסביר אותו ב-Code Review.

## שלושת הדורות של עזרה מ-AI

כלי ה-AI לפיתוח התפתחו בשלושה שלבים, וכולם קיימים היום זה לצד זה:

| דור | מה זה | דוגמה | מתי משתמשים |
|-----|-------|-------|-------------|
| **השלמה (Autocomplete)** | הכלי מציע את השורה/הפונקציה הבאה תוך כדי הקלדה | GitHub Copilot inline suggestions | כתיבת boilerplate, מחזורי `for`, מיפויים |
| **צ'אט (Chat)** | שיחה על הקוד: "הסבר", "מצא באג", "כתוב בדיקה" | Copilot Chat, Claude, ChatGPT, JetBrains AI Assistant | הבנה, עיצוב, בדיקות, refactoring ממוקד |
| **סוכן (Agent)** | הכלי מקבל משימה, קורא קבצים, עורך, מריץ build/tests ומתקן | Claude Code, Copilot agent mode, Cursor Agent, Codex | משימות רב-קבציות: פיצ'ר שלם, מיגרציה, refactoring גדול |

ההבדל המהותי: בהשלמה ובצ'אט **אתם** מחליטים על כל שינוי; בסוכן אתם מגדירים משימה ובודקים **תוצאה**. ככל שהאוטונומיה גדלה, כך גדל הצורך בבדיקות אוטומטיות, ב-git ובביקורת קפדנית.

## מפת הכלים (נכון להיום — בדקו בתיעוד העדכני)

| כלי | היכן רץ | מתאים במיוחד ל- |
|-----|---------|-----------------|
| GitHub Copilot | Visual Studio, VS Code, JetBrains, CLI | השלמות, צ'אט על הקוד, agent mode, code review ב-PR |
| Claude / Claude Code | אתר/אפליקציה, CLI (`claude`), תוספים ל-VS Code ו-JetBrains | משימות רב-קבציות, הסברים ארוכים, refactoring, סקירת קוד |
| ChatGPT / Codex | אתר, CLI, תוסף IDE | צ'אט כללי, יצירת קוד, סוכן למשימות |
| Cursor | IDE עצמאי (מבוסס VS Code) | עריכה מונחית-AI בתוך העורך, Agent, כללי פרויקט |
| Windsurf | IDE עצמאי | חוויית סוכן בתוך העורך |
| JetBrains AI Assistant | Rider / IntelliJ | מפתחי Rider — צ'אט, השלמה, הסבר commit |

קישורים רשמיים:
- GitHub Copilot: https://github.com/features/copilot ותיעוד https://docs.github.com/copilot
- Claude Code: https://docs.claude.com/en/docs/claude-code/overview
- Cursor: https://cursor.com
- Windsurf: https://windsurf.com
- JetBrains AI Assistant: https://www.jetbrains.com/ai/
- OpenAI Codex: https://openai.com/codex/

התכונות המדויקות, המודלים והמחירים משתנים לעיתים קרובות — **אל תתבססו על מצגת; בדקו את התיעוד**.

## התקנה והגדרה בסיסית

### Visual Studio 2022 + GitHub Copilot
1. ב-Visual Studio Installer ודאו שרכיב **GitHub Copilot** מסומן (בגרסאות עדכניות הוא מגיע כחלק מההתקנה).
2. היכנסו עם חשבון GitHub שיש לו רישיון Copilot (אישי או ארגוני).
3. חלון הצ'אט: **View → GitHub Copilot Chat**. ניסיון ראשון: פתחו קובץ C# ובקשו "Explain this file".
4. בקוד: התחילו לכתוב הערה (`// parse the csv line into an Order`) ותנו ל-Copilot להציע. `Tab` מקבל, `Esc` דוחה.

### VS Code
- הרחבות: **GitHub Copilot** + **GitHub Copilot Chat** (ל-agent mode), או **Claude Code** (extension), או Cursor כ-IDE נפרד.
- ל-C# צריך גם את הרחבת **C# Dev Kit** (או C#) כדי שיהיו IntelliSense, build ו-tests.

### Claude Code (CLI)
Claude Code הוא כלי שורת פקודה שרץ בתיקיית הפרויקט, קורא את הקוד, מציע עריכות ומריץ פקודות (בהסכמתכם). התקנה ושימוש לפי התיעוד הרשמי: https://docs.claude.com/en/docs/claude-code/overview. תהליך עבודה טיפוסי:

```bash
cd MyWpfSolution
claude                     # פותח שיחה בתיקיית הפרויקט
# > "Add an ICustomerRepository interface and a JSON implementation. Run dotnet build when done."
```

הכלי מציג את ה-diff לפני החלה ומבקש אישור להרצת פקודות — זה ה-"human in the loop" בפועל.

### Cursor
IDE מבוסס VS Code. יש בו Chat/Agent ואפשר להגדיר כללי פרויקט (קובץ `.cursorrules` או תיקיית `.cursor/rules`) שמלווים כל בקשה. נרחיב על קובצי הוראות במודול 2.

## העיקרון: Human in the Loop

בכל תהליך עבודה עם AI יש ארבע נקודות שבהן אדם חייב להיות:

1. **ניסוח המשימה** — מה בדיוק רוצים, מה האילוצים (מודול 2).
2. **קריאת ה-diff** — כל שינוי שהכלי מציע נקרא לפני שהוא נכנס ל-git.
3. **אימות** — build, בדיקות, הרצה, analyzers (מודול 4).
4. **Code Review** — קוד שנוצר ב-AI עובר PR בדיוק כמו קוד אנושי (מודול 6).

הכלל של היום: **"AI כותב — אתם חותמים"**. אם אתם לא מוכנים לחתום על שורה, היא לא נכנסת.

## במה AI טוב ובמה פחות

**טוב מאוד ב-:**
- Boilerplate: DTOs, מיפויים, `INotifyPropertyChanged`, CRUD, קובצי הגדרה.
- הסברים על קוד קיים, תרגום בין ספריות (למשל Newtonsoft → System.Text.Json).
- כתיבת בדיקות ראשוניות, דוגמאות שימוש ל-API, regex.
- הצעת שמות, פירוק מתודה ארוכה, XML comments.
- "שותף לחשיבה" — לבקש חלופות עם יתרונות וחסרונות.

**חלש/מסוכן ב-:**
- **APIs מומצאים ("הזיות")**: מתודה שנשמעת הגיונית אך לא קיימת, פרמטר שלא קיים, package שלא קיים ב-NuGet.
- **ידע לא עדכני**: הצעת גרסאות ישנות, תבניות שהוחלפו (`WebClient`, `BinaryFormatter`), חבילות שהוצאו משימוש.
- **אבטחה**: שרשור מחרוזות ל-SQL, סודות בקוד, path traversal, אי-אימות קלט.
- **הקשר גדול**: ככל שהפרויקט גדול, כך הכלי "רואה" פחות וממציא יותר. חובה לתת הקשר (מודול 2).
- **דרישות עסקיות**: הכלי לא יודע מה החוק בחברה שלכם, מהי "הנחה מותרת", או איך נראה תהליך אישור. אם לא כתבתם — הוא ניחש.

## קריאה ביקורתית של פלט AI

כשאתם מקבלים קוד, שאלו לפי הסדר:

1. **האם זה מתקמפל?** — `dotnet build`. אם יש שגיאה, לעיתים קרובות זה API מומצא.
2. **האם זה עושה מה שביקשתי?** — לא "האם זה נראה טוב", אלא האם הוא עונה על הדרישה, כולל מקרי קצה.
3. **מה קורה כשהקלט שגוי?** — `null`, מחרוזת ריקה, מספר שלילי, רשת נופלת.
4. **מה הוא הוסיף שלא ביקשתי?** — חבילת NuGet חדשה? `try/catch` שבולע חריגות? `async void`?
5. **האם זה תואם את הארכיטקטורה שלנו?** — או שהוא פתח `HttpClient` בתוך ViewModel?

דוגמה קטנה: ביקשנו "parse a date from the user":

```csharp
// פלט טיפוסי מ-AI
var date = DateTime.Parse(input);
```

מתקמפל, "עובד" על המחשב של המפתח, ונופל בפרודקשן עם תרבות (culture) אחרת. הגרסה שנחתום עליה:

```csharp
if (!DateTime.TryParseExact(input, "yyyy-MM-dd", CultureInfo.InvariantCulture,
        DateTimeStyles.None, out var date))
{
    throw new FormatException($"Expected yyyy-MM-dd, got '{input}'.");
}
```

## תהליך עבודה מומלץ (Loop)

```text
1. הגדרה קצרה של המשימה ושל "מה זה הצלחה" (בדיקות/דוגמאות)
2. Prompt עם הקשר → פלט
3. build + tests + analyzers
4. קריאה של ה-diff, תיקון או prompt מתקן
5. commit קטן עם הודעה ברורה
6. PR + review (אנושי, ואפשר גם AI כקורא נוסף)
```

## טעויות נפוצות

- **קבלת הצעה ב-Tab בלי לקרוא** — במיוחד ב-Autocomplete, השלמות "נראות נכון".
- **בקשה ענקית אחת** ("תבנה לי את המערכת") במקום צעדים קטנים עם אימות בין לבין.
- **להאמין ל-package שלא קיים** — תמיד `dotnet add package` ובדיקה ב-nuget.org.
- **העתקת סודות/נתוני לקוחות ל-prompt** — ראו מודול 6.
- **לוותר על הבנה** — אם לא הבנתם למה הקוד עובד, בקשו הסבר לפני שאתם ממשיכים.

## לסיכום

- AI מאיץ את הלולאה: השלמה → צ'אט → סוכן. ככל שהאוטונומיה עולה, כך עולה הצורך באימות.
- ההכרעה נשארת של המפתח: ניסוח, קריאת diff, אימות, review.
- AI מצוין ל-boilerplate, הסברים ובדיקות; מסוכן ב-APIs מומצאים, ידע ישן ואבטחה.
- קראו פלט AI כמו קוד של מפתח חדש בצוות: מוכשר, מהיר, ולא מכיר את המערכת.

## קריאה נוספת

- GitHub Copilot docs: https://docs.github.com/copilot
- Claude Code overview: https://docs.claude.com/en/docs/claude-code/overview
- Visual Studio + Copilot: https://learn.microsoft.com/visualstudio/ide/visual-studio-github-copilot-extension
- Responsible AI (Microsoft): https://learn.microsoft.com/azure/ai-services/responsible-use-of-ai-overview
