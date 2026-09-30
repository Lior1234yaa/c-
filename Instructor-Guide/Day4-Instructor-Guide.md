<div dir="rtl">

# מדריך למרצה — יום 4: פיתוח בעזרת AI וקוד ניתן לתחזוקה

**C# ב-.NET — קורס מעשי | יום 4 (יום הסיום) | 09:00–16:30**

## תקציר היום

היום מחבר את שלושת הימים הקודמים לעבודה מקצועית עם כלי AI (Copilot, Claude/Claude Code, Cursor, ChatGPT). הוא בנוי כ"לולאה": איך מנסחים בקשה (מודולים 1–2), איך מאיצים כתיבת קוד (3), איך סוקרים ומאמתים את הפלט (4), איזה שלד ארכיטקטוני מחזיק את הקוד (5), ואיך עושים את כל זה בארגון (6). המושב האחרון (מודול 7) הוא מושב הסיום של הקורס: כלי AI לפיתוח UI, עם דמו חי מסקיצה ל-XAML מחובר ל-ViewModel.
המסר שחוזר בכל מודול: **"AI כותב — אתם חותמים."** בכיתה מריצים שתי מעבדות (Lab 1, Lab 3). Lab 2 ו-Lab 4 הן שיעורי בית / תרגול עצמי.

## מטרות למידה (מתוך README של היום)

בסוף היום המשתתפים יוכלו:
- לבחור מצב עבודה (השלמה / צ'אט / סוכן) לכל משימה, ולהגדיר את הכלים ב-Visual Studio וב-VS Code.
- לנסח prompts בתבנית Role / Context / Task / Constraints / Examples / Output, ולתחזק קובצי הוראות (`CLAUDE.md`, `copilot-instructions.md`).
- להאיץ כתיבת DTOs, LINQ, בדיקות, refactoring ומיגרציות, ולדעת מה לבדוק אחר כך.
- לסקור קוד AI לפי צ'ק-ליסט ולזהות באגים עדינים (async, culture, disposal, thread-safety, אבטחה).
- לבנות אפליקציית .NET בשכבות עם DI, configuration ו-logging, כולל ב-WPF.
- להגדיר מדיניות ארגונית: פרטיות, סודות, רישוי, שערי סקירה, CI ומדידה.
- לבנות UI ל-WPF בעזרת AI, מסקיצה ועד XAML מחובר ל-ViewModel.

חומרי המקור: [README של היום](../Day4-AI-Assisted-Development/README.md) · [Notes](../Day4-AI-Assisted-Development/Notes/) · [מצגת](../Day4-AI-Assisted-Development/Slides/Day4.pptx) (מקור: [Day4.slides.js](../Day4-AI-Assisted-Development/Slides/Day4.slides.js)) · [תרגילים](../Day4-AI-Assisted-Development/Exercises/README.md) · [Demos](../Day4-AI-Assisted-Development/Demos/)

> **מספור שקפים**: המצגת כוללת 54 שקפים. המספרים במדריך נספרו לפי הסדר ב-`Day4.slides.js`: 1 כותרת · 2 לו"ז · 3–9 מודול 1 · 10–17 מודול 2 · 18–22 מודול 3 + Lab 1 · 23–29 מודול 4 + Lab 3 · 30–36 מודול 5 + Lab 2 · 37–42 מודול 6 · 43–52 מודול 7 + Lab 4 · 53 ציטוט · 54 סיום. בשקפי המעבדות (22, 29, 36, 52) יש הערות דובר עם רמזים למרצה.

---

## הכנה לפני היום

### חשבונות וכלי AI (הכי חשוב — בלי זה אין מעבדות)
- [ ] **שבוע לפני**: שלחו למשתתפים תזכורת לפתוח חשבון ולוודא שהכניסה עובדת בכלי אחד לפחות, ורצוי בשניים: GitHub Copilot (ב-VS/VS Code), Claude (אתר או Claude Code), ChatGPT, Cursor או JetBrains AI. כך נדרש ב-[README](../Day4-AI-Assisted-Development/README.md) וב-[COURSE-OVERVIEW](../00-Setup/COURSE-OVERVIEW.md).
- [ ] **המחשב שלכם**: ודאו ש-Copilot Chat נפתח ב-Visual Studio (**View → GitHub Copilot Chat**) וב-VS Code, ושהפקודה `claude` רצה בטרמינל (אם תדגימו Claude Code). כדאי להכין גם כלי גיבוי: חשבון שני או צ'אט בדפדפן.
- [ ] **בבוקר, בשקף 2**: בקשו מכל המשתתפים לפתוח את הכלי ולשלוח prompt קצר. מי שלא מצליח להתחבר מקבל עזרה בדמו של 09:25 (הערת הדובר בשקף 6), לפני המעבדות.
- [ ] **תוכנית גיבוי** למשתתף בלי גישה: הוא עובד בזוג עם שכן. בתרגילי ה-prompt הוא כותב את ה-prompt ומשווה ל-[PROMPTS-ANSWERS.md](../Day4-AI-Assisted-Development/Exercises/Solutions/PROMPTS-ANSWERS.md), בלי להריץ.

### מדיניות ארגונית על הדבקת קוד
- [ ] ברר מראש עם הארגון המארח אילו כלים ותוכניות מאושרים, ואם מותר להדביק בהם קוד. כל קוד הקורס הוא קוד תרגול, ולפי הטבלה במודול 6 מותר להדביק אותו גם לכלי בתוכנית אישית. **קוד קנייני של החברה אסור** בתוכנית אישית.
- [ ] אמרו את זה בפתיחה, כדי שאף אחד לא ידביק קוד מהעבודה.
- [ ] שימו לב: ב-[S7_SettingsService.cs](../Day4-AI-Assisted-Development/Labs/Lab3-ReviewAiCode/Starter/Day4.Lab3.Starter/Snippets/S7_SettingsService.cs) של Lab 3 יש מחרוזת שנראית כמו מפתח API אמיתי (`sk-live-...`). היא מזויפת, אבל זה רגע לימודי טוב: בקשו מהמשתתפים להחליף אותה ב-`<REDACTED>` **לפני** שהם מדביקים את הקובץ ל-AI בשלב 2 של המעבדה.

### סביבה ופרויקטים
- [ ] .NET 10 SDK מותקן ו-Windows זמין (WPF רץ רק ב-Windows). אצל המשתתפים: [00-Setup/VerifySetup](../00-Setup/VerifySetup) עבר.
- [ ] בנו מראש את כל הקוד: `.\tools\build-all.ps1` משורש הריפו. אפשר לבנות רק את הפרויקטים של היום (ראו בהמשך).
- [ ] הריצו מראש וודאו שהתוצאה כצפוי:

<div dir="ltr">

```powershell
# משורש הריפו C:\c-
cd Day4-AI-Assisted-Development
dotnet test Demos/Day4.Demo.Refactored.Tests                           # 26 בדיקות עוברות (כולל golden master)
dotnet run --project Exercises/Solutions -- 9                          # מדפיס 1990 תחת de-DE
dotnet test Labs/Lab1-FeatureWithAi/Starter/Day4.Lab1.Starter.Tests    # צפוי: 18 נכשלות (NotImplementedException)
dotnet test Labs/Lab1-FeatureWithAi/Solution/Day4.Lab1.Solution.Tests # עוברות
dotnet run --project Labs/Lab3-ReviewAiCode/Starter/Day4.Lab3.Starter  # צפוי: FAIL ברוב הבדיקות
dotnet run --project Labs/Lab3-ReviewAiCode/Solution/Day4.Lab3.Solution # ALL CHECKS PASSED
dotnet run --project Demos/Day4.Demo.DiHostWpf                         # חלון WPF
dotnet run --project Demos/Day4.Demo.AiGeneratedUi                     # חלון Dashboard
```

</div>

- [ ] **פתחו מראש** ב-IDE: `Demos/Day4.Demo.LegacyMess`, `Demos/Day4.Demo.Refactored`, `Demos/Day4.Demo.DiHostWpf`, `Demos/Day4.Demo.AiGeneratedUi`, את התיקייה `Demos/Prompts`, ופרויקט אחד מיום 3 (למשל `Day3-GUI-WPF/Demos/Day3.Demo.Binding`) לדמו של מודול 1.
- [ ] **הכינו עותק עבודה לדמו החי של מודול 7** (ההוראות בבלוק 15:43). עשו זאת **מחוץ לריפו**, כדי שהמקור לא ישתנה.
- [ ] **הכינו עותק של LegacyMess** מחוץ לריפו לדמו של מודול 3 (בלוק 11:03).

### git
- [ ] git מותקן אצלכם ואצל המשתתפים. בכל עבודה עם סוכן (agent), ובכל refactoring, עובדים על עותק שעבר `git init` ו-commit. כך אפשר לראות diff ולחזור אחורה.

<div dir="ltr">

```powershell
Copy-Item -Recurse Day4-AI-Assisted-Development\Labs\Lab1-FeatureWithAi\Starter my-work\Day4-Lab1
cd my-work\Day4-Lab1; git init; git add .; git commit -m "starter"
```

</div>

### מה לחלק למשתתפים
- את תיקיות `Starter/` של Lab 1 ו-Lab 3 בבוקר, ואת אלה של Lab 2 ו-Lab 4 בסוף היום, לבית.
- את [Exercises/README.md](../Day4-AI-Assisted-Development/Exercises/README.md), ואת `Demos/Prompts/` (תבניות 13 ו-14 משמשות במעבדות).
- **אל תחלקו** את `Solution/`, `Exercises/Solutions/` ו-`FINDINGS.md` לפני סוף כל תרגול או מעבדה.

---

## לו"ז יומי

| שעה | סוג | נושא | חומרים |
|-----|-----|------|--------|
| 09:00–09:10 | הרצאה | פתיחה: מטרות היום, לו"ז, מדיניות הדבקת קוד, בדיקת התחברות לכלי AI | שקפים 1–2 |
| 09:10–09:25 | הרצאה | מודול 1: שלושת הדורות, מפת כלים, Human in the Loop, חוזקות וחולשות, קריאה ביקורתית | [Notes/01](../Day4-AI-Assisted-Development/Notes/01-ai-dev-workflows.md), שקפים 3–5, 7–9 |
| 09:25–09:32 | דמו | הגדרה מהירה ושימוש חי בכלי על קוד מיום 3 | שקף 6 |
| 09:32–09:45 | תרגול | תרגילים 1 (★), 2 (★) | [Exercises](../Day4-AI-Assisted-Development/Exercises/README.md) |
| 09:45–10:08 | הרצאה | מודול 2: אנטומיה של prompt, הקשר ואיטרציה, בדיקות קודם, חלופות, MCP | [Notes/02](../Day4-AI-Assisted-Development/Notes/02-prompt-engineering-for-devs.md), שקפים 10–14, 17 |
| 10:08–10:13 | דמו | ספריית Prompts וקובצי הוראות (`CLAUDE.md` / `copilot-instructions.md` / `.cursorrules`) | [Demos/Prompts](../Day4-AI-Assisted-Development/Demos/Prompts/README.md), שקפים 15–16 |
| 10:13–10:30 | תרגול | תרגילים 3 (★), 4 (★★) | [Exercises](../Day4-AI-Assisted-Development/Exercises/README.md) |
| 10:30–10:45 | הפסקה | | |
| 10:45–11:03 | הרצאה | מודול 3: DTOs, LINQ, boilerplate, regex, בדיקות, מיגרציה, Generate → Verify → Own | [Notes/03](../Day4-AI-Assisted-Development/Notes/03-code-generation-acceleration.md), שקפים 18–21 |
| 11:03–11:15 | דמו | Refactoring עם AI: `LegacyMess` → `Refactored` + golden master | [LegacyMess](../Day4-AI-Assisted-Development/Demos/Day4.Demo.LegacyMess/), [Refactored](../Day4-AI-Assisted-Development/Demos/Day4.Demo.Refactored/) |
| 11:15–11:30 | תרגול | תרגילים 6 (★★), 7 (★) | [Exercises](../Day4-AI-Assisted-Development/Exercises/README.md) |
| 11:30–12:20 | מעבדה | **Lab 1**: מנוע הנחות ממפרט, בדיקות קודם | [Lab1](../Day4-AI-Assisted-Development/Labs/Lab1-FeatureWithAi/README.md), שקף 22 |
| 12:20–12:30 | סיכום | דיון מסכם ל-Lab 1: טעויות AI טיפוסיות | [Lab1 NOTES](../Day4-AI-Assisted-Development/Labs/Lab1-FeatureWithAi/Solution/NOTES.md) |
| 12:30–13:15 | הפסקה | ארוחת צהריים | |
| 13:15–13:35 | הרצאה | מודול 4: צ'ק-ליסט, analyzers, 8 באגים עדינים, AI כסוקר שני (כולל מיני-דמו) | [Notes/04](../Day4-AI-Assisted-Development/Notes/04-reviewing-ai-code.md), שקפים 23–28 |
| 13:35–13:50 | תרגול | תרגילים 8 (★★), 9 (★★) | [Exercises](../Day4-AI-Assisted-Development/Exercises/README.md) |
| 13:50–14:28 | מעבדה | **Lab 3**: סקירת 7 קטעי קוד "מ-AI" | [Lab3](../Day4-AI-Assisted-Development/Labs/Lab3-ReviewAiCode/README.md), שקף 29 |
| 14:28–14:35 | סיכום | דיון מסכם ל-Lab 3: הממצאים ו-false positives | [FINDINGS.md](../Day4-AI-Assisted-Development/Labs/Lab3-ReviewAiCode/Solution/FINDINGS.md) |
| 14:35–14:48 | הרצאה | מודול 5: שכבות, DI, Options, ILogger, SOLID, עקביות של קוד AI | [Notes/05](../Day4-AI-Assisted-Development/Notes/05-maintainable-architecture.md), שקפים 30–31, 33–35 |
| 14:48–14:55 | דמו | WPF עם Generic Host: DI + `appsettings.json` + `ILogger` | [DiHostWpf](../Day4-AI-Assisted-Development/Demos/Day4.Demo.DiHostWpf/README.md), שקף 32 |
| 14:55–15:05 | תרגול | תרגיל 11 (★★) | [Exercises](../Day4-AI-Assisted-Development/Exercises/README.md) |
| 15:05–15:15 | הפסקה | | |
| 15:15–15:35 | הרצאה | מודול 6: פרטיות, סודות, רישוי, שערי סקירה, CI, מדידה, הטמעה, סיכונים | [Notes/06](../Day4-AI-Assisted-Development/Notes/06-enterprise-ai-integration.md), שקפים 37–42 |
| 15:35–15:43 | הרצאה | מודול 7 (מושב הסיום): קטגוריות כלי UI, תהליך העבודה ל-desktop | [Notes/07](../Day4-AI-Assisted-Development/Notes/07-ai-tools-for-ui-development.md), שקפים 43–45 |
| 15:43–15:58 | דמו | **דמו חי**: prompt → XAML → חידוד → ViewModel → review | שקפים 46–50, [AiGeneratedUi](../Day4-AI-Assisted-Development/Demos/Day4.Demo.AiGeneratedUi/) |
| 15:58–16:05 | תרגול | תרגיל 12 (★) | [Exercises](../Day4-AI-Assisted-Development/Exercises/README.md) |
| 16:05–16:10 | הרצאה | מתי Blazor, "מה הלאה", הצגת Lab 4 | שקפים 51–52 |
| 16:10–16:20 | סיכום | סיכום היום: שאלות חזרה ושיעורי בית (Lab 2, Lab 4, תרגילים 5 ו-10) | שקפים 36, 52 |
| 16:20–16:30 | סיכום | סיכום הקורס, משוב ושאלות | שקפים 53–54 |

**תרגילים שלא שובצו בכיתה:** 5 (★★, קובץ הוראות) ו-10 (★★★, משאבים ו-thread-safety). הם מיועדים למי שמסיים מהר ולשיעורי הבית. תרגיל 10 הוא חימום טוב ל-Lab 3, ותרגיל 5 הוא חימום ל-Lab 4.

---

## פירוט לפי בלוק

### 09:00–09:10 · הרצאה · פתיחה (שקפים 1–2)
- ברכה: "שלושה ימים כתבנו ביד; היום נלמד איך AI מאיץ את העבודה בלי לוותר על איכות."
- **שאלה לכיתה (הצבעה)**: מי כבר משתמש ב-Copilot / Claude / ChatGPT בעבודה? לפי התשובה התאימו את הקצב של מודול 1.
- עברו על הלו"ז. הדגישו שהמושב האחרון עוסק ב-UI, ושלכל מעבדה צריך כלי מחובר.
- מדיניות: אנחנו מדביקים רק קוד תרגול, ואף פעם לא סודות או קוד מהעבודה.
- משימה מיידית: כל אחד פותח את הכלי שלו ושולח "Explain what `ObservableCollection<T>` is in one sentence". מי שנתקע מקבל עזרה ב-09:25.

### 09:10–09:25 · הרצאה · מודול 1: תהליכי פיתוח עם AI (שקפים 3–5, 7–9)
**נקודות להוראה, לפי הסדר:**
1. המסר המרכזי: AI לא מחליף את הידע מימים 1–3 אלא נשען עליו. **האחריות על הקוד נשארת של המפתח.**
2. שלושה דורות (שקף 4):
   - **השלמה**: boilerplate ולולאות.
   - **צ'אט**: הסבר, באג, בדיקה.
   - **סוכן**: משימה רב-קבצית, בדרך כלל עם build ו-tests.

   בהשלמה ובצ'אט אתם מחליטים על כל שינוי. בסוכן אתם בודקים **תוצאה**. ככל שהאוטונומיה גדלה, גדל גם הצורך ב-git, בבדיקות וב-review.
3. מפת כלים (שקף 5): Copilot, Claude Code, ChatGPT/Codex, Cursor, Windsurf, JetBrains AI. **אל תתעכבו על תכונות** — הן משתנות כל חודש. הפנו לקישורים ב-Notes.
4. Human in the Loop (שקף 7): ארבע נקודות שבהן חייב להיות אדם — ניסוח, קריאת ה-diff, אימות, Code Review. "AI כותב — אתם חותמים." אנלוגיה: מפתח חדש ומוכשר שהגיע אתמול.
5. במה AI טוב ובמה מסוכן (שקף 8):
   - **טוב**: boilerplate, הסברים, בדיקות ראשוניות, regex.
   - **מסוכן**: APIs מומצאים (דוגמה: `List.RemoveWhere`), ידע ישן (`WebClient`, `BinaryFormatter`), אבטחה, הקשר גדול, דרישות עסקיות.
6. קריאה ביקורתית (שקף 9): חמש שאלות בסדר קבוע — מתקמפל? עושה מה שביקשתי? מה עם קלט שגוי? מה הוסיף שלא ביקשתי? תואם לארכיטקטורה? הדוגמה: `DateTime.Parse` מול `TryParseExact` עם `InvariantCulture`.
7. הלולאה: משימה, prompt, build/tests, diff, commit קטן, PR.

**שאלות לכיתה:**
- "מי ניסה agent mode? מה קרה?" (מהערת הדובר בשקף 4)
- "קוד שמתקמפל ועובר על המחשב שלי — למה זה עדיין לא מספיק?" (תשובה: culture, קלט שגוי, דרישה שלא נבדקה)

**תפיסות שגויות נפוצות:**
- "אם זה מתקמפל, זה נכון." הקומפיילר תופס API מומצא, אבל לא לוגיקה שגויה.
- "סוכן חוסך review." ההפך: ככל שיש יותר אוטונומיה, צריך יותר review.
- "הכלי יודע מה החוקים העסקיים שלנו." הוא לא יודע. אם לא כתבתם, הוא ניחש.

### 09:25–09:32 · דמו · הגדרה ושימוש חי (שקף 6)
- פתחו פרויקט מיום 3, למשל `Day3-GUI-WPF/Demos/Day3.Demo.Binding`, ב-Visual Studio.
- **View → GitHub Copilot Chat** ← "Explain this file". הראו שהתשובה מתייחסת לקוד הפתוח.
- בקובץ C# כתבו הערה (`// return the number of items whose price is above the average`) והראו הצעת השלמה: `Tab` מקבל, `Esc` דוחה. **לפני שמקבלים, קוראים את ההצעה.**
- אם יש לכם Claude Code: `claude` בתיקיית הפרויקט, ואז "Explain the project structure". הראו שהכלי מבקש אישור לפני הרצת פקודות — זה human in the loop בפועל.
- נצלו את הדקות האלה כדי לפתור בעיות התחברות של משתתפים.

### 09:32–09:45 · תרגול · תרגילים 1–2
| # | כותרת | קושי | תשובה צפויה |
|---|-------|------|-------------|
| 1 | איזה מצב עבודה? | ★ | ToString: השלמה. DataGrid לא מתעדכן: צ'אט (List לעומת ObservableCollection). מיגרציה של 14 קבצים + בדיקות: סוכן, עם git נקי לפני ו-review של ה-diff אחרי. `for`: השלמה. SQLite לעומת JSON: צ'אט עם trade-offs, וההחלטה אנושית. |
| 2 | API מומצא | ★ | `RemoveWhere` → `RemoveAll`; `ContainsIgnoreCase` → `Contains("YOSSI", StringComparer.OrdinalIgnoreCase)`; `JoinWith` → `string.Join(", ", names)`. |

- תשובות לתרגיל 1: [PROMPTS-ANSWERS.md](../Day4-AI-Assisted-Development/Exercises/Solutions/PROMPTS-ANSWERS.md).
- הרצת הפתרון לתרגיל 2:

<div dir="ltr">

```powershell
cd Day4-AI-Assisted-Development\Exercises\Solutions
dotnet run -- 2        # מדפיס "found" ואחר כך "Dana, yossi, Noa"
```

</div>

- בדיון: "הקומפיילר הוא קו ההגנה הראשון". הריצו את הקוד המקורי כדי להראות את `CS1061`.

### 09:45–10:08 · הרצאה · מודול 2: Prompt Engineering (שקפים 10–14, 17)
**נקודות להוראה:**
1. איכות הפלט תלויה באיכות הבקשה. prompt טוב הוא **מפרט טכני קצר**.
2. שישה מרכיבים (שקף 11): Role, Context, Task, Constraints, Examples, Output. כשהפלט גרוע, כמעט תמיד חסר Context או Examples.
3. דוגמה מלאה (שקף 12): `ParseCsvLine`. קראו אותה שורה-שורה והראו **שהדוגמאות הן למעשה בדיקות**. אם יש זמן, הריצו אותה חי.
4. תנו הקשר (שקף 13):
   - הדביקו את הממשק, המודל והבדיקה שנכשלת.
   - השתמשו ב-`#file` (Copilot) או `@file` (Cursor).
   - הדביקו הודעת שגיאה מלאה.
   - בשיחה ארוכה פתחו שיחה חדשה עם סיכום.
5. איטרציה: בקשה, שגיאת קומפילציה, הדבקת השגיאה, "Fix without adding packages", חידוד, ניקוי.
6. בדיקות קודם וחלופות (שקף 14):
   - **בדיקות קודם**: "Write tests only… Do not implement yet", ורק אחרי שאישרתם אותן: "Now implement".
   - **חלופות**: "2–3 options with pros/cons, no code yet".
7. Prompts של אבחון: "Explain line by line + assumptions", "Find the most likely cause; do not rewrite", ברווז גומי ("ask me questions that reveal holes").
8. MCP (שקף 17), אזכור קצר: הקשר אמין יותר, אבל גם ערוץ נוסף שדרכו נתונים יוצאים. נחזור לזה במודול 6.

**שאלות לכיתה:**
- "ה-prompt 'תכתוב לי repository' — מה הכלי ינחש?" (מודל, גרסה, ספרייה, אחסון)
- "למה קל יותר לסקור בדיקות מאשר מימוש?"

**תפיסות שגויות:**
- "prompt ארוך = prompt טוב." מה שחשוב הוא הקשר ודוגמאות, לא אורך.
- "אם זה לא עבד, צריך לנסח מחדש מאפס." בדרך כלל עדיף להדביק את השגיאה ולבקש "minimal change".
- "אותה שיחה לנצח." שיחה ארוכה צוברת רעש.

### 10:08–10:13 · דמו · ספריית Prompts וקובצי הוראות (שקפים 15–16)
- פתחו את [Demos/Prompts](../Day4-AI-Assisted-Development/Demos/Prompts/README.md):
  - `templates-he.md` / `templates-en.md`: 14 תבניות. הראו את **תבנית 13** (סקירה קפדנית, סיבוב שני) ואת **תבנית 14** (חלון WPF). שתיהן ישמשו במעבדות.
  - `CLAUDE.md`: Stack, Structure, Conventions (decimal, DateTimeOffset, IClock, InvariantCulture, בלי לבלוע חריגות), WPF rules (RTL, StaticResource, בלי code-behind), Commands, Workflow.
  - `.github/copilot-instructions.md` ו-`.cursorrules`: אותן הוראות בפורמט של כל כלי.
- להדגיש: קובץ הוראות הוא ההשקעה עם התשואה הגבוהה ביותר. כותבים אותו פעם אחת, וכל בקשה מקבלת הקשר. הוא גם **תיעוד לבני אדם**. ל-Lab 4 מצורף קובץ כזה מוכן.

### 10:13–10:30 · תרגול · תרגילים 3–4
| # | כותרת | קושי | תשובה צפויה |
|---|-------|------|-------------|
| 3 | שכתוב prompt גרוע | ★ | prompt לפי Role/Context/Task/Constraints/Examples/Output. כולל את חתימות `IOrderRepository`, `Order` ו-`IClock`, הזרקה בבנאי, `ArgumentOutOfRangeException` ל-total שלילי, בלי `DateTime.Now` ובלי חבילות חדשות, ובדיקות עם fakes. |
| 4 | בדיקות קודם | ★★ | "Do not implement yet". שמות `Method_Scenario_Expected`, גבולות 7/8 ו-64/65, בלי ספרה או אות גדולה, רווח/טאב, כמה הפרות יחד ("abc" → 3 שגיאות), null → `ArgumentNullException`. |

- התשובות ב-[PROMPTS-ANSWERS.md](../Day4-AI-Assisted-Development/Exercises/Solutions/PROMPTS-ANSWERS.md). אין תרגיל קוד להריץ.
- בדיון: בקשו ממתנדב להקריא את ה-prompt שלו, והכיתה מסמנת איזה מרכיב חסר. בתרגיל 4 הדגישו מקרה שקל לפספס: **אותיות עבריות אינן אותיות גדולות**.
- מי שמסיים מהר עובר לתרגיל 5 (★★, `CLAUDE.md` לאפליקציית CSV → JSON).

### 10:30–10:45 · הפסקה

### 10:45–11:03 · הרצאה · מודול 3: האצת כתיבת קוד (שקפים 18–21)
לכל דוגמה עברו על אותו סדר: **בקשה → פלט טיפוסי → מה תיקנו**.
1. **DTOs מ-JSON** (שקף 19):
   - `unit_price` יצא `double`. לכסף משתמשים ב-`decimal`.
   - `status` עדיף כ-`enum` עם `JsonStringEnumConverter`.
   - בדקו ש-`[property: JsonPropertyName]` נמצא על פרמטר ה-record.
2. **LINQ** (שקף 20):
   - הקבצה לפי `CustomerName` היא באג עסקי (שני לקוחות עם אותו שם). נכון להקבץ לפי `CustomerId`.
   - `ToList()` כדי שהשאילתה לא תחושב שוב בכל מעבר.
3. **Boilerplate**: `ObservableObject` עם `SetProperty` ו-`[CallerMemberName]`. בדקו שהקוד עקבי עם הממשקים הקיימים, ושלא נוספה ספריית מיפוי.
4. **Regex**: חסרו עוגנים `^…$`. השתמשו ב-`[GeneratedRegex]`.
5. **xUnit** (שקף 20):
   - `InlineData` עם `decimal` לא שלם (19.9) נכשל בהמרה. הפתרון: `MemberData` או ערכים שלמים.
   - בדיקה שמעתיקה את הנוסחה לא בודקת כלום.
6. **XML docs**: תיעוד שמשקר גרוע מהיעדר תיעוד.
7. **Refactoring** (שקף 21): בקשות ממוקדות (Extract / Rename / Strategy), **ורק עם רשת ביטחון** — בדיקות או golden master.
8. **מיגרציה** Newtonsoft → System.Text.Json:
   - ברירת המחדל רגישה לאותיות. אם צריך: `PropertyNameCaseInsensitive`.
   - `JsonSerializerOptions` צריך להיות `static readonly`, לא אובייקט חדש בכל קריאה.
9. **Commit/PR**: ה-AI מתאר קבצים במקום כוונה. תמיד לקרוא ולערוך.
10. הכלל: **Generate → Verify → Own**.

**שאלות לכיתה:**
- "ה-LINQ עובר את כל הבדיקות. מה עדיין יכול להיות שגוי?" (ההקבצה לפי שם)
- "מה ההבדל בין refactoring לבין 'שכתוב'?" (התנהגות זהה, מוכחת בבדיקות)

**תפיסות שגויות:** "`double` מספיק לכסף"; "מיגרציה שעוברת בדיקות לא משנה כלום" (היא יכולה לשנות את פורמט ה-JSON בקבצים שכבר שמורים).

### 11:03–11:15 · דמו · LegacyMess → Refactored (הערת הדובר בשקף 21)
**לפני הדמו:** העתיקו את `Demos/Day4.Demo.LegacyMess` לתיקייה מחוץ לריפו (למשל `C:\temp\LegacyLive`) והריצו `git init` ו-commit.

1. הראו את [OrderProcessor.cs](../Day4-AI-Assisted-Development/Demos/Day4.Demo.LegacyMess/OrderProcessor.cs): מחלקת "אלוהים" של כ-250 שורות, `double` לכסף, מספרי קסם (0.15, 0.12, 25, 60, 0.18), עיצוב משוכפל, שדות public.
2. צרו golden master:

<div dir="ltr">

```powershell
cd C:\temp\LegacyLive
dotnet run > before.txt
```

</div>

3. prompt לכלי (תבנית 6 / Lab 2 שלב 1): "List the code smells ranked by risk. Propose a target structure. Do NOT write code yet." קראו יחד את הרשימה.
4. prompt שני: "Extract the shipping calculation into a private method `CalculateShipping(string country, double afterDiscount)` with named constants. Keep output identical." החילו את השינוי.
5. `dotnet run > after.txt` ואז `Compare-Object (Get-Content before.txt) (Get-Content after.txt)`. פלט ריק פירושו שההתנהגות זהה. אחר כך `git diff`.
6. **מצב היעד**: פתחו את [Day4.Demo.Refactored](../Day4-AI-Assisted-Development/Demos/Day4.Demo.Refactored/):
   - `Domain/Order.cs`, `Services/` (`DiscountPolicy`, `ShippingCalculator`, `OrderPricer`, `ReportFormatter`, `ReportRunner`), `Infrastructure/EmbeddedCsvOrderSource.cs`.
   - `Program.cs` כ-composition root בלבד.
   - `TreatWarningsAsErrors` ב-csproj.

<div dir="ltr">

```powershell
cd C:\c-\Day4-AI-Assisted-Development
dotnet run --project Demos/Day4.Demo.Refactored           # אותו פלט בדיוק
dotnet test Demos/Day4.Demo.Refactored.Tests              # 26 בדיקות, כולל Run_WithEmbeddedData_MatchesGoldenMaster
```

</div>

- **להדגיש:** הקובץ `golden-master.txt` הופק מהגרסה הישנה. הבדיקות בודקות **כללים** (`DiscountPolicyTests`, `ShippingAndVatTests`) ולא רק את הפלט הכולל. זה בדיוק התהליך של Lab 2.

### 11:15–11:30 · תרגול · תרגילים 6–7
| # | כותרת | קושי | תשובה צפויה |
|---|-------|------|-------------|
| 6 | Records מ-JSON | ★★ | records עם `[property: JsonPropertyName]`: `UnitPrice` מסוג `decimal`, `Email` מסוג `string?`, `CreatedAt` מסוג `DateTimeOffset`. הסכום יוצא 39.8 בדיוק. |
| 7 | Regex בלי עוגנים | ★ | `x052-1234567y` ו-`952-1234567` עוברים את `\d{3}-\d{7}`. התיקון: `^0\d{2}-\d{7}$` עם `[GeneratedRegex]`. |

<div dir="ltr">

```powershell
cd Day4-AI-Assisted-Development\Exercises\Solutions
dotnet run -- 6     # Order 42 ... Total: 39.8
dotnet run -- 7     # טבלה: AI (no anchors) מול fixed
```

</div>

- בדיון על תרגיל 7: הראו בטבלת הפלט אילו קלטים עוברים בגרסת ה-AI ונכשלים בגרסה המתוקנת.

### 11:30–12:20 · מעבדה · Lab 1 — פיצ'ר ממפרט עם AI, בדיקות קודם
[README](../Day4-AI-Assisted-Development/Labs/Lab1-FeatureWithAi/README.md) · [NOTES](../Day4-AI-Assisted-Development/Labs/Lab1-FeatureWithAi/Solution/NOTES.md) · שקף 22

**מטרה:** לממש `DiscountEngine : IDiscountEngine` בעזרת AI, כשהבדיקות כבר קיימות ונכשלות, ולתרגל prompt עם הקשר, עבודה מול בדיקות וסקירה ביקורתית.

**מה יש ב-Starter:**
- `Day4.Lab1.Starter/Models.cs`: `CustomerType`, `OrderLine`, `Order` עם `Subtotal` ו-`TotalQuantity`, ו-`DiscountResult`.
- `IDiscountEngine.cs`.
- `DiscountEngine.cs` עם `throw new NotImplementedException` ו-TODO.
- `Day4.Lab1.Starter.Tests/DiscountEngineTests.cs`: 18 מקרי בדיקה שנכשלים.

**המפרט בקצרה:**
- Volume: 10–49 פריטים → 5%, 50 ומעלה → 10%.
- Amount: מעל 1000 → 8%.
- VIP: 12%.
- **הגבוהה מנצחת** (לא מצטברים).
- קופון `WELCOME10` מוסיף 10 ₪ קבועים אם Subtotal ≥ 50, בלי תלות באותיות גדולות/קטנות.
- ההנחה לא עולה על ה-Subtotal. עיגול ל-2 ספרות פעם אחת בסוף.
- `AppliedRule` מקבל "None"/"Volume"/"Amount"/"Vip", ו-"+Coupon" אם הקופון הופעל.

**שלבים (לוח זמנים מכווץ ל-50 דקות):**

| זמן | שלב |
|-----|------|
| 11:30–11:35 | הסבר קצר: העתקת Starter + `git init` |
| 11:35–11:42 | שלב 1: קריאת הבדיקות, `dotnet test` נכשל |
| 11:42–11:55 | שלב 2: מילוי דף העבודה (טבלת 6 המרכיבים) ו-prompt ראשון |
| 11:55–12:03 | שלב 3: build/test, והדבקת כישלונות ל-prompt המשך |
| 12:03–12:13 | שלב 4: סקירה לפי הצ'ק-ליסט + תבנית 13 |
| 12:13–12:20 | שלב 5: בדיקה חדשה — 60 ₪, 10 פריטים, קופון → 13 ₪, `"Volume+Coupon"` |

<div dir="ltr">

```powershell
cd my-work\Day4-Lab1\Day4.Lab1.Starter.Tests
dotnet test
```

</div>

**קריטריוני קבלה:**
- `dotnet test` ירוק.
- המימוש עומד בצ'ק-ליסט: בלי `double`, בלי static mutable, בלי I/O ובלי `DateTime.Now`, קופון עם `OrdinalIgnoreCase` ובטוח ל-null, כללים מופרדים, בלי תוספות שלא התבקשו, קבועים עם שם.
- דף העבודה מלא.
- הבדיקה משלב 5 נוספה ועוברת.

**איפה נתקעים ואיזה רמז לתת:**

| סימפטום | רמז |
|---------|-----|
| `Calculate_RulesDoNotStack_HighestWins` נכשל (216 = 120 + 96 במקום 120) | "המפרט אומר 'הגבוהה מנצחת'. בקשו מקסימום, לא סכום." |
| `Calculate_Coupon_AddsFixed10…` מחזיר 8 | "10 ₪ בנוסף זה סכום קבוע, לא אחוז." |
| `welcome10` נכשל | `StringComparison.OrdinalIgnoreCase` |
| `Calculate_RoundsToTwoDecimals` יוצא 11.99 | "עיגול פעם אחת בסוף, לא בכל שלב." |
| `"Coupon"` במקום `"Vip+Coupon"` / `"None+Coupon"` | שרשור של שם הכלל עם "+Coupon" |
| ה-AI שינה את הבדיקות כדי שיעברו | "Do not modify the tests" נמצא ב-prompt לדוגמה. חזרו ל-commit. |
| הכלי לא "רואה" את הקבצים | להדביק את `IDiscountEngine.cs`, `Models.cs` ו-`DiscountEngineTests.cs` |

עברו בין המשתתפים ובדקו שהם **קוראים את הקוד** ולא רק מריצים בדיקות (הערת הדובר בשקף 22).

**בונוס למי שמסיים מהר:**
- כל כלל הופך למחלקה `IDiscountRule`, ומוסיפים כלל "Seasonal 3%" בלי לגעת במנוע.
- `[Theory]` עם 10 מקרים אקראיים מה-AI, ובדיקה ידנית של שלושה מהם.

### 12:20–12:30 · סיכום · דיון מסכם ל-Lab 1
- **טבלת "טעויות AI טיפוסיות" מ-NOTES.md**: שאלו מי נתקל בכל אחת.
  - צבירת הנחות.
  - קופון כאחוז.
  - `==` לקופון.
  - עיגול בכל שלב.
  - `double`.
  - "Coupon" לבד.
  - `ILogger` ו-`DateTime.Now` שלא התבקשו.
  - `?.` על `order.Lines`.
- **החלטות העיצוב בפתרון** ([DiscountEngine.cs](../Day4-AI-Assisted-Development/Labs/Lab1-FeatureWithAi/Solution/Day4.Lab1.Solution/DiscountEngine.cs)):
  - `IDiscountRule` עם `Name` ו-`Rate(order)`. המנוע בוחר את האחוז הגבוה ביותר.
  - הקופון הוא **שלב נפרד**, לא `IDiscountRule`, כי הוא סכום קבוע.
  - בנאי ללא פרמטרים (כדי שהבדיקות המקוריות יעבדו) ובנאי עם `IEnumerable<IDiscountRule>` ל-DI.
  - עיגול פעם אחת אחרי `Math.Min(amount, subtotal)`.
  - הבדיקות משתמשות ב-`double` ב-`InlineData` ומבצעות cast. זה בטוח רק עם ערכים "עגולים" (חיבור למודול 3).
- בפתרון יש שתי בדיקות נוספות: `Calculate_VolumePlusCoupon` (שלב 5) ו-`Calculate_CustomRule_CanBeAddedWithoutChangingEngine` (הבונוס).

<div dir="ltr">

```powershell
cd Day4-AI-Assisted-Development\Labs\Lab1-FeatureWithAi\Solution\Day4.Lab1.Solution.Tests
dotnet test
```

</div>

### 12:30–13:15 · הפסקת צהריים

### 13:15–13:35 · הרצאה · מודול 4: סקירה ואימות (שקפים 23–28)
**נקודות להוראה:**
1. **Trust but Verify**: קוד AI *נראה* נכון — מעוצב, עם שמות טובים. התייחסו אליו כמו ל-PR של מפתח שהגיע אתמול.
2. **הצ'ק-ליסט** (שקף 24), עשרה תחומים: נכונות, מקרי קצה, אבטחה, ביצועים, async, חריגות, משאבים, רישוי, תלויות, עקביות. בקשו מהמשתתפים לשמור אותו פתוח ב-Lab 3.
3. **כלים ולא רק עיניים** (שקף 25):
   - `Nullable`, `TreatWarningsAsErrors`, `AnalysisLevel latest-recommended`, `EnforceCodeStyleInBuild`.
   - `dotnet format --verify-no-changes`.
   - הרצה עם קלט אמיתי: עברית, תאריכים, מספרים.
   - בדיקה טובה מתארת התנהגות, ולא מעתיקה את הנוסחה.
   - לכל באג: בדיקה שנכשלת לפני התיקון ועוברת אחריו.
4. **שמונה באגים עדינים** (שקפים 26–27). כתבו אותם על הלוח; כולם מופיעים ב-Lab 3.
   - off-by-one
   - `async void`
   - parsing תלוי culture
   - `DateTime.Now` לעומת `UtcNow`/`TimeProvider`
   - `HttpClient` ב-`using` בכל קריאה
   - `Dictionary` ממספר threads
   - `catch {}`
   - path traversal
5. **AI סוקר AI** (שקף 28): שיחה חדשה או כלי אחר, תבנית 13 עם צ'ק-ליסט, "Do not rewrite the file". הערות של AI הן **קלט** לסוקר האנושי, לא החלטה. Copilot code review על PR ו-Claude Code על `git diff`.
6. תהליך של 10 דקות: קריאת הדרישה → build/tests/format → צ'ק-ליסט → סקירת AI שנייה → תיקון → commit.

**מיני-דמו (3 דק', בתוך הבלוק):** הערת הדובר בשקף 28 מציעה להדביק קטע מ-Lab 3. כדי **לא לחשוף את Lab 3 מראש**, מומלץ להדביק במקומו את הקוד של תרגיל 10 (`PriceCache`) בשיחה חדשה עם תבנית 13. אחר כך השוו את הממצאים לצ'ק-ליסט, וחפשו false positive.

**שאלות לכיתה:**
- "איזה מהבאגים האלה ה-analyzers היו תופסים לבד?" (מעט מאוד. לכן צריך סוקר.)
- "ה-AI כתב 'I tested this'. מה עושים?" (לא סומכים עד שרואים את הבדיקה רצה.)

**תפיסות שגויות:**
- "בדיקה ירוקה = קוד נכון." בדיקה שמאשרת התנהגות שגויה גרועה מכלום.
- "לתקן באג = עוד prompt." הכלי לפעמים "מתקן" על ידי הסתרה (`try/catch`).
- "analyzers זה רק לפרויקטים גדולים."

### 13:35–13:50 · תרגול · תרגילים 8–9
| # | כותרת | קושי | תשובה צפויה |
|---|-------|------|-------------|
| 8 | async | ★★ | (1) `async void` → `async Task<int> LoadAsync`, כי בגרסה המקורית חריגה מפילה את התהליך. (2) `.Result` → `await`, כי `.Result` חוסם ועלול לגרום deadlock ב-UI. בונוס: `HttpClient` משותף ו-`CancellationToken`. |
| 9 | תרבות וזמן | ★★ | תחת `de-DE`, `decimal.Parse("19.90")` = **1990**. תיקון: `TryParse(..., InvariantCulture)`. התוקף: `DateTime.Now` → `TimeProvider.GetUtcNow()`, והטיפוס המוחזר הוא `DateTimeOffset`. |

<div dir="ltr">

```powershell
cd Day4-AI-Assisted-Development\Exercises\Solutions
dotnet run -- 8     # length = 22, ואז רשימת התיקונים (עם HttpMessageHandler מזויף, בלי רשת)
dotnet run -- 9     # de-DE: 1990 <-- ואז הגרסה המתוקנת
```

</div>

- **רגע השיא של הבלוק**: הריצו את `-- 9` מול הכיתה. המספר 1990 במקום 19.90 משכנע יותר מכל הסבר (הערת הדובר בשקף 27).
- מי שמסיים מהר עובר לתרגיל 10 (★★★).

### 13:50–14:28 · מעבדה · Lab 3 — סקירת קוד AI
[README](../Day4-AI-Assisted-Development/Labs/Lab3-ReviewAiCode/README.md) · [NOTES](../Day4-AI-Assisted-Development/Labs/Lab3-ReviewAiCode/Solution/NOTES.md) · [FINDINGS](../Day4-AI-Assisted-Development/Labs/Lab3-ReviewAiCode/Solution/FINDINGS.md) · שקף 29

**מטרה:** לסקור 7 קבצים ש"נוצרו ב-AI", למצוא באגים עדינים, לתקן ולתעד בטבלת ממצאים. AI משמש **כסוקר שני**, לא ראשון.

**מה יש ב-Starter:**
- `Snippets/S1…S7`: כולם מתקמפלים.
- `Program.cs` עם 8 בדיקות עצמיות (S1, S3×2, S4, S5×2, S6, S7). ל-S2 אין בדיקת ריצה, והוא נבדק בקריאה בלבד.
- ב-csproj יש `NoWarn` ל-CS1998, CS8602 ו-CS8600. לכן חלק מהאזהרות מוסתרות, ונשארת אזהרת CS8603 מ-S7.

**שלבים (מכווץ ל-38 דקות):**

| זמן | שלב |
|-----|------|
| 13:50–13:52 | הסבר קצר: טבלת הממצאים והכלל "בלי AI בשלב 1" |
| 13:52–14:07 | שלב 1: סקירה ידנית. כל אחד עובר על כל הקבצים לפי הצ'ק-ליסט. |
| 14:07–14:15 | שלב 2: סקירה שנייה עם תבנית 13, קובץ אחד בכל פעם. **מחליפים את המפתח ב-S7 ב-`<REDACTED>` לפני שמדביקים.** |
| 14:15–14:28 | שלב 3: תיקון, `dotnet build` ו-`dotnet run` עד `ALL CHECKS PASSED` |

<div dir="ltr">

```powershell
cd my-work\Day4-Lab3\Day4.Lab3.Starter
dotnet run      # ב-Starter: רוב הבדיקות FAIL (S4 לא דטרמיניסטית — לפעמים "עוברת" במזל)
```

</div>

**קריטריוני קבלה:**
- לפחות 12 ממצאים, ולפחות אחד בכל קובץ.
- כל הקבצים מתוקנים, `dotnet build` בלי אזהרות, `ALL CHECKS PASSED`.
- לכל ממצא: משפט "למה זה משנה בפרודקשן".
- רשימת false positives של ה-AI.

**התשובות — הבאגים המוסתרים בכל קובץ (למרצה):**

| קובץ | באגים (חומרה) | תיקון מינימלי |
|------|----------------|---------------|
| **S1_FileExport** | (1) **path traversal**: `Path.Combine(_root, userFileName)` מקבל `..\` או נתיב מוחלט (High). (2) אין בדיקת null, ריק או תווים אסורים (Med). | `Path.GetFileName` + בדיקה ש-`GetFullPath` מתחיל ב-root; `ThrowIfNullOrWhiteSpace`, `GetInvalidFileNameChars` |
| **S2_ReportLoader** | (3) `async void Load` (High). (4) `.Result` בתוך async (High). (5) `new HttpClient()` ב-`using` בכל קריאה, גם בלולאה של `LoadManyAsync` (Med). (6) אין `CancellationToken` (Low). | `async Task<int> LoadAsync`, `await`, `HttpClient` מוזרק, פרמטר `ct` |
| **S3_PriceParser** | (7) `double.Parse` בלי culture: תחת de-DE "19.90" הופך ל-1990 (High). (8) `double` לכסף ואז cast ל-decimal (Med). (9) `catch {}` מדלג בשקט על שורות פגומות (High). (10) `DateTime.Parse` תלוי culture, ו-`DateTime.Now` ב-`IsValidNow` (Med). | `decimal.TryParse(..., InvariantCulture)`, זריקת `FormatException` עם מספר השורה, `TryParseExact("yyyy-MM-dd")`, `TimeProvider` |
| **S4_TokenCache** | (11) `Dictionary` נכתב ממספר threads (High). (12) `DateTime.Now` לתפוגה (Med). (13) לוודא שכשל ב-fetch לא נשאר במטמון (Low). | `ConcurrentDictionary` + `Lazy<Task<…>>` (fetch פעם אחת גם תחת עומס), `TimeProvider.GetUtcNow()`, הסרה מהמטמון ב-catch + rethrow |
| **S5_Paginator** | (14) off-by-one: `page * pageSize` לעמודים שמתחילים מ-1, ו-`i <= end` (High). (15) `PageCount = total / pageSize` מעגל כלפי מטה, והעמוד האחרון נעלם (Med). | `(page-1)*pageSize`, `i < end`, ceiling: `(total + size - 1) / size`, ולידציה של הארגומנטים |
| **S6_CustomerSearch** | (16) **SQL injection**: שרשור של קלט המשתמש (High). (17) `id` כמחרוזת (Low). | שאילתה עם פרמטרים (`@pattern`, `@id`), נטרול wildcards (`%`, `_`, `[`), `int id` |
| **S7_SettingsService** | (18) מפתח API כברירת מחדל בקוד (High). (19) `Console.WriteLine` של המפתח (High). (20) `Deserialize` יכול להחזיר null, אזהרה CS8603 (Med). (21) `new JsonSerializerOptions` בכל שמירה (Low). (22) כתיבה ליד ה-exe (`BaseDirectory`) (Low). | להסיר את הסוד ולקרוא מסביבה/user-secrets/Key Vault, לא לרשום ערכים, `?? throw InvalidDataException`, `static readonly` options, `LocalApplicationData` |

**איפה נתקעים ואיזה רמז לתת:**
- **"אין לי מה למצוא ב-S5"**: "הריצו בראש את `GetPage([1..10], 1, 3)`. מה חוזר?" (בגרסה המקורית: `[4,5,6,7]`)
- **S4 עובר לפעמים**: "גם באג לא דטרמיניסטי הוא באג. מה קורה עם 1,000 threads?" (רמז מה-README)
- **תיקון של S6 משנה חתימה** (למשל מחזיר record עם `Sql` ו-`Parameters`, כמו בפתרון), ואז `Program.cs` לא מתקמפל: מותר לעדכן את הבדיקה ב-`Program.cs` לחתימה החדשה, כמו שעשו בפתרון. בדיון: חתימות שהשתנו הן **breaking changes** שמתעדים ב-PR (מ-NOTES).
- **ה-AI "מתקן" path traversal עם `Replace("..", "")`**, או SQL עם `Replace("'", "''")`: זה לא תיקון (טבלת "טעויות AI בשלב התיקון" ב-NOTES).
- **ה-AI מציע `lock` סביב `await`**: זה לא מתקמפל. `SemaphoreSlim` גלובלי מסלסל את כל הבקשות. הכיוון הנכון: `ConcurrentDictionary` + `Lazy<Task>`.
- **`catch (Exception ex) { Console.WriteLine(ex); }`**: זו עדיין בליעה. צריך לזרוק עם הקשר.
- רמז כללי מה-README: "ומה עם `de-DE`? ומה עם 1,000 threads? ומה עם `..\`?" ו-"`catch {}` הוא כמעט תמיד ממצא".

**בונוס:**
- להפעיל `TreatWarningsAsErrors` + `AnalysisLevel latest-recommended` ולבדוק כמה באגים ה-analyzers תופסים. בפתרון הם תופסים לבד את CS8603.
- בדיקת xUnit לכל תיקון.

### 14:28–14:35 · סיכום · דיון מסכם ל-Lab 3
- ספרו כמה ממצאים מצאה הכיתה ידנית לעומת ה-AI. בפתרון יש 22.
- **False positives טיפוסיים** (מ-FINDINGS):
  - "S5: `List<T>` צריך להיות `IEnumerable<T>`" — זה סגנון, לא באג.
  - "S1: `Directory.CreateDirectory` לא בטוח ל-threads" — הוא כן בטוח.
  - "S3: צריך `try/catch` סביב כל שורה" — ההפך: הבליעה הייתה הבאג.
- ה-analyzers תפסו לבד רק את CS8603. **הכלים משלימים את הסוקר, לא מחליפים אותו.**

<div dir="ltr">

```powershell
dotnet run --project Day4-AI-Assisted-Development\Labs\Lab3-ReviewAiCode\Solution\Day4.Lab3.Solution   # ALL CHECKS PASSED
```

</div>

### 14:35–14:48 · הרצאה · מודול 5: ארכיטקטורה ניתנת לתחזוקה (שקפים 30–31, 33–35)
> דלגו על שקף 32 בינתיים (הוא משמש בדמו), ועל שקף 36 (Lab 2) שיוצג בסוף היום כשיעורי בית.

**נקודות להוראה:**
1. **למה דווקא היום**: AI כותב מהר, ולכן המבנה הוא מה שמונע ערימה. בלי כללים, AI ישים `HttpClient` בתוך ViewModel ו-`static` בכל מקום.
2. **שכבות** (שקף 31): UI → Application → Domain ← Infrastructure. התלויות מצביעות פנימה, והדומיין לא מכיר אף אחד. בפרויקט קטן מספיקות תיקיות — מה שחשוב הוא הכיוון, לא מספר ה-csproj.
3. **DI**: המחלקה מקבלת תלויות בבנאי דרך ממשק. primary constructor של C# 12+. Lifetimes:
   - **Singleton**: repository, clock, options.
   - **Transient**: ViewModels ו-services קלים.
   - **Scoped**: רלוונטי בעיקר ל-ASP.NET.
4. **Options + ILogger** (שקף 33):
   - `services.Configure<AppOptions>(GetSection("App"))`. בלי קבועי קסם; משתני סביבה דורסים את הקובץ.
   - structured logging עם placeholders ולא interpolation. לא רושמים נתונים אישיים או סודות.
5. **בדיקתיות**: כל מה שמדבר עם העולם (זמן, קבצים, רשת) נמצא מאחורי ממשק. `FakeClock` בבדיקות, או `TimeProvider` / `FakeTimeProvider`.
6. **SOLID כתיקונים קונקרטיים** (שקף 34):
   - S: `OrderProcessor` מתפצל לשלוש מחלקות.
   - O: `switch` מוחלף ב-`IDiscountRule` (כמו ב-Lab 1).
   - L: `IReadRepository`.
   - I: ממשקים קטנים.
   - D: `IEmailSender`.
7. **עקביות של קוד AI** (שקף 35): קובצי הוראות, תבנית או "קובץ דוגמה", analyzers + `.editorconfig`, בדיקות ארכיטקטורה, review ("האם זה בשכבה הנכונה?"). README + ADRs עוזרים גם ל-AI להבין "למה".

**שאלות לכיתה:**
- "האם כל מחלקה צריכה ממשק?" (לא. ממשק שווה משהו כשיש מימוש שני, אמיתי או fake.)
- "מה רע ב-`_host.Services.GetService<T>()` בתוך ViewModel?" (Service Locator: התלויות מוסתרות)

**תפיסות שגויות:**
- "DI = ספרייה." DI זה קודם כול העברת תלויות בבנאי, וה-container הוא רק נוחות.
- "Singleton לכל דבר." לא ל-state של משתמש או של מסך.
- `IOptions<T>` עמוק בתוך ה-Domain.

### 14:48–14:55 · דמו · DiHostWpf (שקף 32)
[README](../Day4-AI-Assisted-Development/Demos/Day4.Demo.DiHostWpf/README.md)

<div dir="ltr">

```powershell
dotnet run --project Day4-AI-Assisted-Development\Demos\Day4.Demo.DiHostWpf    # Windows בלבד
```

</div>

עברו על הקבצים לפי הסדר:
1. `App.xaml`: **אין `StartupUri`**. אם הוא קיים, ייפתחו שני חלונות.
2. `App.xaml.cs`:
   - `Host.CreateDefaultBuilder()` קורא את `appsettings.json` ואת משתני הסביבה, ומגדיר logging.
   - הרישומים: `Configure<AppOptions>`, `IClock` → `SystemClock`, `IOrderRepository` → `InMemoryOrderRepository` (Singleton), `OrderService`, `MainViewModel` ו-`MainWindow` (Transient).
   - `OnStartup`: `StartAsync`, ואז `GetRequiredService<MainWindow>().Show()`.
   - `OnExit`: `StopAsync` ו-`Dispose`.
3. `appsettings.json`: המקטע `"App"` (Title, Currency...). ב-csproj: `CopyToOutputDirectory`.
4. `Services/OrderService.cs`: מקבל `IOrderRepository`, `IClock`, `IOptions<AppOptions>` ו-`ILogger` בבנאי. `LogInformation("Created order {OrderId}…")` הוא structured logging.
5. `MainWindow.xaml.cs`: `MainWindow(MainViewModel viewModel)`, כלומר ה-ViewModel מוזרק ואין לוגיקה ב-code-behind.
6. **הדגמה חיה**: שנו את `"Title"` ב-`appsettings.json` שב-output (או במקור, ואז build) והריצו שוב. הכותרת משתנה בלי לגעת בקוד. אחר כך הוסיפו הזמנה בחלון והראו את שורת הלוג ב-Output/Debug.

**להדגיש:** constructor injection ולא service locator. את Options ואת ILogger מקבלים "בחינם" מה-host.

### 14:55–15:05 · תרגול · תרגיל 11
| # | כותרת | קושי | תשובה צפויה |
|---|-------|------|-------------|
| 11 | הזרקת תלויות | ★★ | ממשקים `IClock` (`Now`) ו-`INotifier` (`Send`). `ReminderService(IClock clock, INotifier notifier)`. רישום ב-`ServiceCollection` (Singleton לשעון ול-notifier, Transient ל-service). "בדיקה" עם `FixedClock` + `RecordingNotifier` שמצפה ל-`"test:Reminder at 09:30"`. |

<div dir="ltr">

```powershell
cd Day4-AI-Assisted-Development\Exercises\Solutions
dotnet run -- 11    # [email→dana] Reminder at HH:mm ואחר כך "fake test PASSED"
```

</div>

- הזמן קצר (10 דקות). אם הכיתה איטית, עשו את התרגיל "ביחד": המשתתפים מכתיבים, והמרצה כותב.

### 15:05–15:15 · הפסקה

### 15:15–15:35 · הרצאה · מודול 6: שילוב AI בארגון (שקפים 37–42)
**נקודות להוראה:**
1. מעבר מ"אני" ל"אנחנו": מה מותר לשלוח, מי אחראי, איך מודדים, איך מונעים דליפה. **לא ממציאים מספרים ולא מצטטים תנאים של ספקים.** מפנים ל-trust centers.
2. **פרטיות** (שקף 38):
   - תוכנית אישית שונה מתוכנית ארגונית (שימוש לאימון, זמן שמירה, אזור גאוגרפי).
   - אפשרויות on-prem / private cloud (Azure OpenAI / Bedrock / Vertex, מודלים מקומיים).
   - **טבלת המדיניות** לפי סוג מידע: קוד תרגול, קוד קנייני, PII, סודות. **סודות: אסור לעולם.**
3. **סודות**: חפשו `password/apikey/token/connectionstring` לפני שמדביקים. `dotnet user-secrets`, Key Vault, secret scanning. סוכנים קוראים קבצים, ולכן `.gitignore` צריך להיות תקין.
4. **רישוי, Compliance ושערי סקירה** (שקף 39):
   - סינון קוד ציבורי ו-SCA.
   - SSO ו-audit.
   - רשימה לבנה לשרתי MCP.
   - **מי שעשה commit אחראי.**
   - כל קוד עובר PR, סוכן לא עושה merge, יש לפחות reviewer אנושי אחד, branch protection, ושקיפות ב-PR ("Generated with…; reviewed by…").
5. **CI** (שקף 40): restore, build עם `-warnaserror`, `format --verify-no-changes`, test. סקירת AI מוגדרת בכלי עצמו, **לא ממציאים step**. היא הערה ולא check חוסם.
6. **מדידה** (שקף 41): DORA-style — lead time, deployment frequency, change failure rate, defects, review time, שביעות רצון. baseline לפני, ובדיקה אחרי 3 חודשים. **לא מודדים שורות קוד.**
7. **הכשרה והטמעה**: קובצי הוראות משותפים, `docs/prompts/`, AI champion, pairing. Playbook של 90 יום: מדיניות, פיילוט, הרחבה, ייצוב.
8. **סיכונים ומיתונים** (שקף 42): טבלה שאפשר להעתיק למסמך מדיניות. כוללת skill atrophy, PRs ענקיים ותלות בספק.

**שאלות לכיתה:**
- "איזה סיכון מהטבלה הכי רלוונטי אצלכם?" (הערת הדובר בשקף 42. בדרך כלל: דליפת נתונים ו-PRs גדולים.)
- "הארגון אוסר AI לגמרי. מה יקרה?" (המפתחים ישתמשו בחשבון אישי בסתר, וזה גרוע יותר.)

**תפיסות שגויות:** "יש AI review, אז אפשר לוותר על CI"; "תוכנית ארגונית = מותר להדביק הכול" (סודות אסורים תמיד); "יותר קוד = יותר ערך".

### 15:35–15:43 · הרצאה · מודול 7: כלי AI ל-UI — פתיחה (שקפים 43–45)
1. למה UI הוא המושב האחרון: שם AI חוסך הכי הרבה זמן, ושם הוא מייצר הכי הרבה "כמעט נכון". XAML מפורט ומלא ב-namespaces ובשמות שקל לטעות בהם.
2. **קטגוריות** (שקף 44):
   - עוזרים בתוך ה-IDE — הדרך הראשית ל-WPF.
   - Prompt-to-UI בדפדפן (v0, Lovable, Bolt), שמייצרים web.
   - כלי עיצוב (Figma AI, Uizard, Galileo).
   - מתמונה לקוד.
   - Design-to-code.

   **הערה כנה:** ל-WPF, הכלים המעשיים הם העוזרים ב-IDE ומודלי הצ'אט. אפשר לעצב ב-v0 או ב-Figma ולבקש "translate to WPF XAML".
3. **תהליך בשישה שלבים** (שקף 45): סקיצה → prompt → XAML (build מיד) → חידוד → binding ל-ViewModel → review. הסיבוב הראשון נותן מבנה, והשני נותן איכות.

### 15:43–15:58 · דמו · **דמו חי: prompt → XAML → ViewModel** (שקפים 46–50)
**הכנה (לפני היום, מחוץ לריפו):**

<div dir="ltr">

```powershell
Copy-Item -Recurse C:\c-\Day4-AI-Assisted-Development\Demos\Day4.Demo.AiGeneratedUi C:\temp\LiveUi
cd C:\temp\LiveUi
Remove-Item ViewModels\OrdersDashboardViewModel.cs, Themes\Dashboard.xaml
# ערכו ידנית:
#  App.xaml            — מחקו את השורה <ResourceDictionary Source="Themes/Dashboard.xaml" />
#  MainWindow.xaml     — השאירו רק <Window x:Class=... Title="לוח הזמנות"> עם <Grid /> ריק (בלי d:DataContext)
#  MainWindow.xaml.cs  — הפכו להערה את DataContext = new OrdersDashboardViewModel();
dotnet build        # חייב לעבור לפני השיעור
git init; git add .; git commit -m "empty shell"
```

</div>

נשארים עם `ViewModels/Mvvm.cs` (`ObservableObject` + `RelayCommand`) וחלון ריק. **אל תשנו את ה-namespace**, כדי שה-x:Class יישאר תקין.

**מהלך הדמו:**
1. **סקיצה (דקה)**: הציגו את ה-ASCII של Orders Dashboard מ-[Notes/07](../Day4-AI-Assisted-Development/Notes/07-ai-tools-for-ui-development.md): סרגל כלים, 4 כרטיסי KPI, DataGrid ושורת סטטוס.
2. **Prompt ראשון (3 דק', שקף 46)**: הדביקו לכלי את ה-prompt הראשון מ-Notes/07 **כמו שהוא** (הוא מתחיל ב-"You are a WPF/XAML expert. Generate MainWindow.xaml only…").
   - הצביעו על חתימות ה-ViewModel שמודבקות ב-prompt. בלעדיהן הכלי ממציא שמות.
   - הצביעו על ה-Constraints (RTL, StaticResource, BooleanToVisibilityConverter, standard controls only). הם מונעים את שלוש הטעויות הנפוצות.
3. **XAML → build (2 דק')**: הדביקו את הפלט ל-`MainWindow.xaml` והריצו `dotnet build`. אם יש שגיאה, הדביקו אותה ל-prompt המשך ("Fix without adding packages").
4. **Review של הסיבוב הראשון (2 דק', שקף 47 + טבלת המלכודות בשקף 50)**: חפשו בפלט:
   - `xmlns:` של ספרייה חיצונית.
   - `StringFormat=C` (תלוי ב-culture).
   - צבעים קשיחים.
   - Converter שלא הוגדר.
   - control או property שלא קיימים (`CornerRadius` על `Button`).
   - חסר empty state.

   לפחות אחד מאלה כמעט תמיד מופיע.
5. **Prompt שני — חידוד (3 דק', שקף 48)**: הדביקו את ה-prompt השני מ-Notes/07:
   - empty state עם `DataTrigger` על `Orders.Count`.
   - מעבר ל-`Themes/Dashboard.xaml` ומיזוג שלו.
   - סגנון `KpiValue`.
   - DataGrid עם שורות מתחלפות ובלי row headers.
   - `AutomationProperties.Name`.
   - "Show only the changed parts".

   צרו את `Themes/Dashboard.xaml` ומזגו אותו (ב-`Window.Resources` או ב-`App.xaml`).
6. **ViewModel (3 דק')**: prompt מוצע (תבנית 3 + החתימות מה-prompt הראשון):

<div dir="ltr">

```text
Implement OrdersDashboardViewModel : ObservableObject (Mvvm.cs attached) in namespace Day4.Demo.AiGeneratedUi.ViewModels
with exactly these members: string SearchText; ObservableCollection<string> Statuses; string? SelectedStatus;
ICommand RefreshCommand, ExportCommand, NewOrderCommand; int TodayOrders; decimal TodayRevenue; int PendingOrders;
int ActiveCustomers; ObservableCollection<OrderRow> Orders (record OrderRow(int Id, string Customer, DateTime Date,
decimal Total, string Status)); OrderRow? SelectedOrder; bool IsBusy; string StatusMessage.
Use 6 in-memory sample rows with Hebrew names. Changing SearchText or SelectedStatus re-filters (Clear/Add on the same
ObservableCollection). No DateTime.Now. No external libraries. Commands via RelayCommand with CanExecute.
```

</div>

   לאחר מכן הוסיפו `DataContext = new OrdersDashboardViewModel();` ב-`MainWindow.xaml.cs` והריצו `dotnet run`.
   - הקלידו "zzz" בחיפוש: ה-empty state מופיע. המשתתפים אוהבים את הרגע הזה (הערת הדובר בשקף 48).
   - בדקו ב-Output את ה-binding errors.
7. **RTL (שקף 49)**: הראו את `FlowDirection="RightToLeft"` על החלון, ושהעמודה "סכום" צריכה `FlowDirection=LeftToRight` בתא. שאלו מי כבר נתקל ב-`DockPanel` "הפוך".
8. **השוואה לגרסה הסופית (1 דק')**:

<div dir="ltr">

```powershell
dotnet run --project C:\c-\Day4-AI-Assisted-Development\Demos\Day4.Demo.AiGeneratedUi
```

</div>

   - פתחו את [MainWindow.xaml](../Day4-AI-Assisted-Development/Demos/Day4.Demo.AiGeneratedUi/MainWindow.xaml). ההערה בראש הקובץ מפרטת מה תוקן בסקירה: הוסר namespace חיצוני, הצבעים עברו ל-`Themes/Dashboard.xaml`, ונוספו empty state, ProgressBar ו-AutomationProperties.
   - הפורמט הוא `{}{0:N2} ₪` במקום `StringFormat=C`.
   - ב-Visual Studio Designer הראו את **design-time data**: `d:DataContext` → `DesignOrdersDashboardViewModel`.
   - ב-[OrdersDashboardViewModel.cs](../Day4-AI-Assisted-Development/Demos/Day4.Demo.AiGeneratedUi/ViewModels/OrdersDashboardViewModel.cs), "היום" מחושב כתאריך המקסימלי בנתונים, בלי `DateTime.Now`.

**גיבוי אם הכלי או הרשת נופלים:** עברו על אותם שלבים בעזרת הקוד המוכן. הראו את ה-prompt מהשקף, ואחר כך את ה-XAML הסופי ואת טבלת המלכודות. הדמו עדיין עובד כ"לפני/אחרי".

**מה להדגיש:**
- תמיד מתחילים מ-ViewModel מוגדר, או לפחות מחתימות. אחרת מבזבזים שעה על תיקון bindings.
- עקביות מגיעה מ-ResourceDictionary ומקובץ הוראות, לא מ"בקשה יפה".
- מצבי loading, empty ו-error צריך לבקש במפורש.

### 15:58–16:05 · תרגול · תרגיל 12
| # | כותרת | קושי | תשובה צפויה |
|---|-------|------|-------------|
| 12 | Prompt לחלון התחברות | ★ | prompt לפי תבנית 14 שכולל:<br>• ViewModel עם `INotifyDataErrorInfo`, `UserName`, `Password`, `LoginCommand`, `IsBusy`, `ErrorMessage`<br>• RTL ותוויות בעברית, `ValidatesOnNotifyDataErrors` + `UpdateSourceTrigger=PropertyChanged`<br>• `Validation.ErrorTemplate`, ProgressBar, כפתור מושבת בזמן busy<br>• `AutomationProperties`, standard controls<br><br>3 בדיקות על הפלט: (1) אין namespace חיצוני ואין `Button_Click`. (2) ה-bindings תואמים ל-VM ו-`ValidatesOnNotifyDataErrors` קיים. (3) RTL תקין וכל ה-StaticResource מוגדרים. |

- תשובה לדוגמה ב-[PROMPTS-ANSWERS.md](../Day4-AI-Assisted-Development/Exercises/Solutions/PROMPTS-ANSWERS.md).
- **שימו לב** (ראו "אי-התאמות" בסוף): התשובה לדוגמה מציעה "TextBox with PasswordChar". ל-`TextBox` של WPF אין `PasswordChar`; המאפיין קיים רק ב-`PasswordBox` (וב-WinForms). הפכו את זה לרגע לימודי: "גם תשובה לדוגמה צריכה review". ב-WPF משתמשים ב-`PasswordBox`, ומכיוון ש-`Password` שלו אינו DependencyProperty, מעבירים אותו כ-`CommandParameter` או דרך attached behavior.

### 16:05–16:10 · הרצאה · מתי Blazor, מה הלאה, הצגת Lab 4 (שקפים 51–52)
- **Blazor + AI** מהיר יותר כשצריך דפדפן או מובייל, או כשהצוות מעורב web + .NET. כלי Prompt-to-UI בשלים יותר ל-HTML/React, ואפשר להמיר ל-`.razor`. Blazor Hybrid מאפשר להריץ Razor בתוך desktop.
- **WPF** עדיין הבחירה הנכונה ל-desktop-only ולאינטגרציה עמוקה עם Windows. ההחלטה ארכיטקטונית; AI לא משנה אותה, רק מאיץ.
- **מה הלאה**: MAUI, Blazor, Avalonia, CommunityToolkit.Mvvm, ASP.NET Core, Microsoft Learn (קישורים ב-Notes/07).
- **שקף 52**: הציגו את Lab 4 כ-capstone לבית (פירוט בהמשך).

### 16:10–16:20 · סיכום · סיכום היום ושיעורי בית
- שאלות החזרה מופיעות בסעיף "סיכום היום והקורס" למטה. בחרו 3–4 מהן ושאלו בעל פה.
- שיעורי בית: Lab 2 (שקף 36), Lab 4 (שקף 52), ותרגילים 5 ו-10.

### 16:20–16:30 · סיכום · סיכום הקורס (שקפים 53–54)
- שקף 53 (ציטוט): "הכלים ישתנו כל כמה חודשים; מה שיישאר הוא היכולת לנסח, לקרוא, לבדוק ולהסביר קוד." עצרו ושאלו: **"מה הדבר האחד שתעשו אחרת ביום ראשון בבוקר?"** תנו לכמה משתתפים לענות.
- שקף 54: שורה אחת לכל יום (ראו נקודות הסיכום בסוף המדריך), ואחר כך משוב ושאלות.

---

## מעבדות לבית — מה למסור ומה לבדוק

### Lab 2 — Refactoring של legacy עם Golden Master (75 דק', עבודה עצמית)
[README](../Day4-AI-Assisted-Development/Labs/Lab2-RefactorLegacy/README.md) · [NOTES](../Day4-AI-Assisted-Development/Labs/Lab2-RefactorLegacy/Solution/NOTES.md) · שקף 36

**מטרה:** להפוך את `SubscriptionBiller` (חיוב מנויים BASIC/PRO/TEAM עם תוספים, חלק יחסי לפי ימים, הנחות ותק, קופונים WELCOME/LOYAL ומע"מ 18%) לשירותים עם ממשקים, DI, `decimal` ובדיקות. הפלט חייב להישאר **זהה בתו אחד** ל-`expected-output.txt`.

**מה יש ב-Starter:**
- `SubscriptionBiller.cs` עם TODO.
- `Program.cs` בשתי שורות.
- `expected-output.txt`.

**שלבים:**
0. רשת ביטחון: `dotnet run > actual.txt`, השוואה, commit, ובדיקת golden master.
1. ניתוח smells עם AI, בלי קוד.
2. חמישה צעדי refactoring, כל אחד ב-prompt נפרד, עם diff אחרי כל צעד.
3. סקירה לפי הצ'ק-ליסט.
4. סקירה שנייה עם תבנית 13.

**קריטריוני קבלה:**
- פלט זהה.
- לפחות 3 שירותים עם ממשקים ו-DI.
- לפחות 8 בדיקות, כולל golden master.
- דף עבודה.

**מה לבדוק כשהמשתתפים מגישים (מ-NOTES):**
- **הטעות הנפוצה**: `Math.Round` בכל שלב "כדי להיות מדויק" שובר את ה-golden master בסנט אחד. המקור לא עיגל בכלל, רק עיצב ב-`0.00`.
- צריך לשמור על סדר החישוב `amount * days / 30`.
- `ToString("N2")` מכניס פסיקי אלפים. צריך `"0.00"` עם InvariantCulture.
- מיון לפי `PlanCode` עם Ordinal.
- `catch` סביב ה-parse מעלים שורות בשקט. צריך `FormatException` עם מספר השורה.
- ממשקים רק למה שמוחלף בפועל (source, pricing), ו-`ReportFormatter` נשאר מחלקה רגילה.
- `TextWriter` במקום `Console`, כדי שאפשר יהיה לבדוק.
- מלכודת ב-PowerShell: `>` עלול לכתוב UTF-16. משווים תוכן, או משתמשים ב-`Out-File -Encoding utf8`.

**אימות הפתרון:**

<div dir="ltr">

```powershell
cd Day4-AI-Assisted-Development\Labs\Lab2-RefactorLegacy\Solution\Day4.Lab2.Solution
dotnet run > actual.txt
Compare-Object (Get-Content ..\..\Starter\Day4.Lab2.Starter\expected-output.txt) (Get-Content actual.txt)
cd ..\Day4.Lab2.Solution.Tests; dotnet test
```

</div>

**בונוס:** מקור CSV חיצוני בלי לשנות את `BillingReport`. `ILogger` שמזהיר על הנחה מעל 50%.

### Lab 4 — Capstone: Expense Tracker ב-WPF (75 דק', משימת סיום ביתית)
[README](../Day4-AI-Assisted-Development/Labs/Lab4-AiBuiltWpfApp/README.md) · [NOTES](../Day4-AI-Assisted-Development/Labs/Lab4-AiBuiltWpfApp/Solution/NOTES.md) · [SPEC.md](../Day4-AI-Assisted-Development/Labs/Lab4-AiBuiltWpfApp/Starter/Day4.Lab4.Starter/SPEC.md) · שקף 52

**מטרה:** לבנות אפליקציית WPF שלמה ממפרט של עמוד אחד, מקצה לקצה עם AI, ואז לסקור, לתקן ולשפר. המעבדה מסכמת את כל היום.

**מה יש ב-Starter:**
- csproj (`net10.0-windows`).
- `App.xaml` שממזג את `Themes/Colors.xaml` (מברשות, `Card`, כפתורים, `ErrorBelow` template).
- `ViewModels/Mvvm.cs` (`ObservableObject` + `RelayCommand`, ו-TODO ל-`AsyncRelayCommand`).
- `MainWindow.xaml` כ-placeholder עם TODO.
- `SPEC.md`, `CLAUDE.md` ו-`.github/copilot-instructions.md`.

**שלבים:**
1. תכנון בלי קוד.
2. Model, repository (כתיבה אטומית) ובדיקות.
3. ViewModels עם `INotifyDataErrorInfo` ו-`AsyncRelayCommand`.
4. XAML.
5. סקירה וליטוש.

**קריטריוני קבלה:**
- הנתונים שורדים הפעלה מחדש.
- validation מוצג ליד השדה, ו"הוסף" מושבת כשיש שגיאות.
- סינון וסיכומים נכונים.
- בלי code-behind ובלי `async void`.
- RTL ו-empty state.
- רשימה של לפחות 5 פריטים "מה ה-AI טעה".

**מה לחפש כשבודקים (מ-NOTES):**
- `async void` בפקודות + `catch` ריק → `AsyncRelayCommand` + `RunAsync` מרכזי שתופס רק `IOException`, `InvalidDataException` ו-`UnauthorizedAccessException`.
- `DateTime.Now` → `TimeProvider`.
- `double` → `decimal` + InvariantCulture.
- `File.WriteAllText` ישיר → temp + `File.Move(overwrite)` ו-`CreateDirectory`.
- binding ל-`Form.AddCommand` → `RelativeSource AncestorType=Window`.
- validation שקורה רק בלחיצה → `UpdateSourceTrigger=PropertyChanged`.
- החלפת מופע ה-`ObservableCollection` → Clear/Add על אותו מופע.
- xmlns חיצוני ל-DatePicker.
- חלוקה באפס באחוזים.

**החלטת עיצוב לדיון:** הפתרון מפוצל ל-`Day4.Lab4.Core` (net10.0: Models, Validator, Repository) ול-WPF. כך הבדיקות רצות גם ב-CI לינוקס.

**רמזים למשתתפים:**
- `DatePicker.SelectedDate` הוא `DateTime?`, ולכן ממירים ל-`DateOnly` ב-VM.
- את `UpdateSourceTrigger=PropertyChanged` צריך לבקש במפורש.
- DataGrid שלא מתעדכן: צריך `ObservableCollection`.

<div dir="ltr">

```powershell
cd Day4-AI-Assisted-Development\Labs\Lab4-AiBuiltWpfApp\Solution\Day4.Lab4.Solution.Tests; dotnet test
cd ..\Day4.Lab4.Solution; dotnet run        # Windows
```

</div>

**בונוס:** ייצוא CSV דרך `IExporter`, SQLite בלי לשנות את ה-VM, DI מלא עם Hosting כמו ב-DiHostWpf.

---

## אם מאחרים / אם מקדימים

### אם מאחרים (לפי סדר העדיפות לקיצוץ)
1. **תרגילים בכיתה → שיעורי בית.** אפשר לקצר קודם כל את 3–4 (להשאיר רק את 3), את 6 (להשאיר את 7), ואת 11 (לעשות אותו "ביחד" ב-5 דקות). **לא מוותרים** על תרגיל 9 (הרגע של 1990) ועל תרגיל 2.
2. **דמו LegacyMess (11:03)**: לוותר על ה-prompt החי ולהראות רק `Refactored` + `dotnet test` (4 דק'). התהליך המלא נלמד ממילא ב-Lab 2 בבית.
3. **מודול 6**: לקצר ל-12 דקות. טבלת המדיניות (שקף 38), שערי הסקירה (שקף 39) וטבלת הסיכונים (שקף 42). את המדידה ואת ה-playbook להשאיר לקריאה ב-Notes.
4. **Lab 1**: לדלג על שלב 5 (הוא קיים בפתרון כ-`Calculate_VolumePlusCoupon`) ולקצר את שלב 4 לשלושה סעיפים מהצ'ק-ליסט.
5. **Lab 3**: לחלק את הקבצים בין זוגות (למשל S1–S3, S4–S5, S6–S7), וכל זוג מציג בדיון. לוותר על שלב 3 (תיקון) לכל הקבצים חוץ מאחד.
6. **לא לקצר את מודול 7** — זה מושב הסיום שהמשתתפים ביקשו. אם אין ברירה, מדלגים על הדמו החי (שלבים 2–6) ומציגים את הגרסה המוכנה.

### אם מקדימים
- תרגיל 10 (★★★) בכיתה, ואחריו סקירה שנייה עם תבנית 13 והשוואה ל-`Ex10_Resources.cs`.
- תרגיל 5: כל משתתף כותב `CLAUDE.md` ומשווה ל-[Demos/Prompts/CLAUDE.md](../Day4-AI-Assisted-Development/Demos/Prompts/CLAUDE.md).
- הבונוסים של Lab 1 (Seasonal 3% כמחלקה) ושל Lab 3 (`TreatWarningsAsErrors` + `AnalysisLevel`, וספירה של מה ה-analyzers תפסו).
- **להתחיל את Lab 4 בכיתה**: שלבים 1–2 (תכנון, repository ובדיקות) אחרי מודול 7, והשאר בבית.
- דמו נוסף: Claude Code / Copilot agent mode על עותק של Lab 2 Starter עם המשימה "Extract pricing rules… keep output identical", והרצת diff בסוף. זה מראה סוכן בפעולה עם golden master.

---

## סיכום היום והקורס

### שאלות חזרה (עם תשובות קצרות)
1. **מתי תבחרו סוכן ולא צ'אט, ומה חייב להיות מוכן לפני כן?**
   משימה רב-קבצית ומכנית עם אימות אוטומטי (למשל מיגרציה של 14 קבצים + בדיקות). לפני כן: git נקי ובדיקות. אחרי: קריאת ה-diff ו-PR.
2. **מהם ששת המרכיבים של prompt טוב, ואיזה מהם חסר בדרך כלל?**
   Role, Context, Task, Constraints, Examples, Output. כשהפלט גרוע, חסרים בדרך כלל Context או Examples.
3. **למה refactoring רק עם golden master או בדיקות, ומה שבר את ה-golden master ב-Lab 2?**
   כי refactoring מוגדר כשינוי מבנה בלי שינוי התנהגות, ורק רשת ביטחון מוכיחה את זה. מה ששבר: `Math.Round` בכל שלב, שינוי סדר החישוב, ו-`N2` במקום `0.00`.
4. **תנו ארבעה באגים עדינים שחוזרים בקוד AI, עם תיקון לכל אחד.**
   - `async void` → `async Task`.
   - `decimal.Parse` בלי culture → `InvariantCulture`.
   - `DateTime.Now` → `TimeProvider` / `UtcNow`.
   - `HttpClient` חדש בכל קריאה → `IHttpClientFactory` או מופע מוזרק.
   - `Dictionary` ממספר threads → `ConcurrentDictionary`.
   - `catch {}` → לוג + זריקה.
   - `Path.Combine` עם קלט משתמש → `GetFullPath` + בדיקת prefix.
   - שרשור SQL → פרמטרים.
5. **מה שומר על קוד AI עקבי עם הארכיטקטורה?**
   קובצי הוראות (`CLAUDE.md` / `copilot-instructions.md`), קובץ דוגמה להעתקה, analyzers + `.editorconfig`, בדיקות (כולל בדיקות ארכיטקטורה), ו-review עם השאלה "האם זה בשכבה הנכונה?".
6. **מה מותר להדביק לכלי AI בתוכנית אישית?**
   קוד פתוח או קוד תרגול בלבד. קוד קנייני — רק בתוכנית ארגונית מאושרת. PII — רק באישור חוזי ובאישור DPO. סודות — אף פעם, בשום תוכנית.
7. **(UI) מה חייבים לכתוב במפורש ב-prompt ל-XAML?**
   חתימות ה-ViewModel, RTL, StaticResource / ResourceDictionary, מצבי loading/empty/error, `AutomationProperties`, ו-"standard controls only / no code-behind".

### שיעורי בית
- **Lab 2** (75 דק'): Refactoring עם golden master. להגיש את הפלט הזהה, את הבדיקות ואת דף העבודה.
- **Lab 4** (75 דק', משימת הסיום המומלצת): Expense Tracker. להגיש את האפליקציה, את בדיקות ה-repository ואת רשימת "מה ה-AI טעה ומה תיקנתי" (לפחות 5 פריטים).
- **תרגילים 5 ו-10**, וכל תרגיל ★/★★ שלא הושלם בכיתה.
- הצעה למסגרת ציון (מ-COURSE-OVERVIEW): 40% מעבדות, 30% תרגילים, 30% פרויקט אישי שמשלב את כל הימים.

### נקודות לסיכום הקורס (שקפים 53–54)
- **יום 1**: OOP, collections, LINQ וחריגות — הבסיס לחשיבה ב-C#.
- **יום 2**: async, HttpClient ו-JSON — לדבר עם העולם.
- **יום 3**: WPF, MVVM, validation ו-styles — לבנות ממשק.
- **יום 4**: AI ככלי האצה — prompt טוב, review קפדני, ארכיטקטורה שמחזיקה ומדיניות ארגונית.
- מה לוקחים הביתה:
  - `Demos/Prompts` (14 תבניות וקובצי הוראות).
  - צ'ק-ליסט הסקירה ממודול 4.
  - Lab 4.
  - כל ה-Notes, שהם "הספר" של הקורס.
- "מה הלאה": MAUI, Blazor, Avalonia, CommunityToolkit.Mvvm, ASP.NET Core, Microsoft Learn.
- **המסר האחרון**: "AI כותב — אתם חותמים." הכלים ישתנו; היכולת לנסח, לקרוא, לבדוק ולהסביר קוד נשארת. תרגלו אותה עם AI ובלעדיו.
- משוב על הקורס, תודה למשתתפים.

---

## נספח: אי-התאמות שנמצאו בחומר המקור

| מקום | הבעיה | מה לעשות בכיתה |
|------|-------|----------------|
| [PROMPTS-ANSWERS.md](../Day4-AI-Assisted-Development/Exercises/Solutions/PROMPTS-ANSWERS.md), תרגיל 12 | מציע "TextBox with PasswordChar", אבל ל-`TextBox` של WPF אין `PasswordChar` (המאפיין קיים רק ב-`PasswordBox` וב-WinForms) | להציג את זה כדוגמה ל-"API מומצא" ולהשתמש ב-`PasswordBox` |
| [Lab4 Starter CLAUDE.md](../Day4-AI-Assisted-Development/Labs/Lab4-AiBuiltWpfApp/Starter/Day4.Lab4.Starter/CLAUDE.md) | ב-"Workflow" יש הפניה ל-`Services/CustomerService.cs` ול-`tests/Orders.Tests`, שלא קיימים בפרויקט (שרידים מתבנית Orders). בנוסף, הקובץ דורש "inject `IClock`" ו-"Timestamps are DateTimeOffset", אבל ה-SPEC דורש `TimeProvider` ו-`DateOnly`. גם `copilot-instructions.md` מזכיר `IClock`. | להזהיר מראש. זו גם הזדמנות ללמד ש"קובץ הוראות לא מעודכן מטעה את ה-AI" (מודול 6, טעויות נפוצות) |
| [Lab4 README](../Day4-AI-Assisted-Development/Labs/Lab4-AiBuiltWpfApp/README.md), שלב 4 | ה-prompt מבקש `Views/MainWindow.xaml`, אבל ב-Starter (וב-CLAUDE.md שלו) `MainWindow.xaml` נמצא בשורש הפרויקט | להנחות לעדכן את הקובץ הקיים בשורש |
| [Lab2 README](../Day4-AI-Assisted-Development/Labs/Lab2-RefactorLegacy/README.md) | כתוב "כ-230 שורות", בפועל `SubscriptionBiller.cs` באורך 183 שורות | לא משמעותי |
| [Lab3 README](../Day4-AI-Assisted-Development/Labs/Lab3-ReviewAiCode/README.md), קריטריון "build ללא אזהרות" | ב-Starter csproj יש `NoWarn` ל-CS1998, CS8602 ו-CS8600, כך שחלק מהאזהרות מוסתרות. תיקון שמשנה חתימות (S6) מחייב לעדכן גם את `Program.cs` | לציין זאת בהסבר הפתיחה של המעבדה |
| הערת הדובר בשקף 28 | מציעה להדגים סקירה שנייה על קטע מ-Lab 3, וזה חושף את התשובות לפני המעבדה | להדגים על הקוד של תרגיל 10 |
| [COURSE-OVERVIEW.md](../00-Setup/COURSE-OVERVIEW.md), "מקצב יומי" | הלו"ז הגנרי (צהריים 12:15–13:00, הפסקה 14:30, "Lab 2" אחרי הצהריים) לא תואם את הלו"ז של יום 4 ב-README (צהריים 12:30–13:15, Lab 3 אחרי הצהריים, Lab 2 לבית) | המדריך הזה עוקב אחרי ה-README של היום |
| מצגת, שקף 36 (Lab 2) | נמצא בתוך מודול 5, אבל Lab 2 אינו מתקיים בכיתה | לדלג עליו במודול 5 ולהציג בסוף היום |

</div>
