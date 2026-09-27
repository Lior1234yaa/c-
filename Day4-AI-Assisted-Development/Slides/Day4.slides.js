// יום 4 — פיתוח בעזרת AI וקוד ניתן לתחזוקה. בנייה:
// node tools/slides/build-slides.js Day4-AI-Assisted-Development/Slides/Day4.slides.js Day4-AI-Assisted-Development/Slides/Day4.pptx
module.exports = {
  day: 4,
  course: 'C# ב-.NET — קורס מעשי',
  title: 'יום 4 — פיתוח בעזרת AI וקוד ניתן לתחזוקה',
  slides: [
    {
      type: 'title',
      title: 'פיתוח בעזרת AI וקוד ניתן לתחזוקה',
      subtitle: 'Copilot, Claude, Cursor — מ-prompt ועד review, ארכיטקטורה ומדיניות ארגונית',
      meta: 'יום 4 מתוך 4 · 09:00–16:30 · מושב סיום: כלי AI לפיתוח UI',
      notes: 'ברוכים הבאים ליום האחרון. שלושה ימים כתבנו קוד ביד; היום נלמד איך AI מאיץ את העבודה בלי לוותר על איכות. המסר המרכזי: AI כותב, אתם חותמים. שאלו כמה מהמשתתפים כבר משתמשים ב-Copilot או Claude — נתאים את הקצב.',
    },
    {
      type: 'bullets', title: 'לוח הזמנים והמטרות', icon: 'clock',
      bullets: [
        { text: '09:00 מודול 1–2: תהליכי עבודה עם AI, Prompt Engineering', sub: ['השלמה → צ\'אט → סוכן; קובצי הוראות'] },
        { text: '10:45 מודול 3 + Lab 1: האצת קוד, פיצ\'ר עם בדיקות קודם' },
        { text: '13:15 מודול 4 + Lab 3: סקירת קוד AI — צ\'ק-ליסט ובאגים עדינים' },
        { text: '14:35 מודול 5–6: ארכיטקטורה, DI, מדיניות ארגונית' },
        { text: '15:35 מודול 7 — מושב הסיום: כלי AI לפיתוח UI', bold: true },
        { text: '16:10 סיכום הקורס ו"מה הלאה"' },
        { text: 'Lab 2 (refactoring) ו-Lab 4 (capstone WPF) — תרגול עצמי/בית' },
      ],
      notes: 'עברו על הלו"ז. הדגישו שהמושב האחרון עוסק ב-UI — זה מה שהמשתתפים ביקשו. ציינו שכל המעבדות משתמשות בכלי AI חי, ולכן צריך חשבון פעיל. בקשו מכולם לוודא עכשיו שהכלי שלהם מחובר.',
    },

    // ---------- מודול 1 ----------
    { type: 'section', number: '01', title: 'תהליכי פיתוח בעזרת AI', subtitle: 'השלמה, צ\'אט, סוכנים — ומי אחראי על מה', notes: 'מודול קצר ומסגרתי. המטרה: שפה משותפת — מה זה autocomplete, chat, agent — ולהבין את העיקרון human in the loop.' },
    {
      type: 'cards', title: 'שלושה דורות של עזרה מ-AI',
      cards: [
        { icon: 'code', heading: 'השלמה (Autocomplete)', text: 'הצעת השורה/הפונקציה הבאה תוך כדי הקלדה. Copilot inline. מתאים ל-boilerplate. אתם מאשרים כל שורה.' },
        { icon: 'chat', heading: 'צ\'אט (Chat)', text: 'שיחה על הקוד: הסבר, מצא באג, כתוב בדיקה, הצע חלופות. Copilot Chat, Claude, ChatGPT, JetBrains AI.' },
        { icon: 'robot', heading: 'סוכן (Agent)', text: 'מקבל משימה, קורא קבצים, עורך, מריץ build/tests. Claude Code, Copilot agent mode, Cursor, Codex. אתם בודקים תוצאה.' },
      ],
      notes: 'שלושת הדורות חיים זה לצד זה. ככל שהאוטונומיה גדלה, גדל הצורך ברשת ביטחון: git, בדיקות, review. שאלו: מי ניסה agent mode? מה קרה?',
    },
    {
      type: 'two-col', title: 'מפת הכלים (בדקו בתיעוד העדכני)',
      right: { heading: 'כלי / היכן רץ', bullets: ['GitHub Copilot — VS, VS Code, JetBrains, CLI', 'Claude / Claude Code — אתר, CLI, תוספי IDE', 'ChatGPT / Codex — אתר, CLI, תוסף', 'Cursor, Windsurf — IDE עצמאי (מבוסס VS Code)', 'JetBrains AI Assistant — Rider'] },
      left: { heading: 'מתאים במיוחד ל-', bullets: ['השלמות, צ\'אט על הקוד, agent mode, סקירת PR', 'משימות רב-קבציות, הסברים, refactoring', 'צ\'אט כללי, יצירת קוד, סוכן', 'עריכה מונחית-AI + כללי פרויקט', 'מפתחי Rider'] },
      notes: 'אל תתעכבו על תכונות ספציפיות — הן משתנות כל חודש. הפנו לקישורים הרשמיים במודול 1 בחומר. הנקודה: לכל קטגוריה יש כמה ספקים, והעקרונות זהים.',
    },
    {
      type: 'steps', title: 'הגדרה מהירה',
      steps: [
        { heading: 'Visual Studio 2022 + Copilot', text: 'רכיב GitHub Copilot ב-Installer → התחברות עם חשבון GitHub → View → GitHub Copilot Chat' },
        { heading: 'VS Code', text: 'הרחבות GitHub Copilot + Copilot Chat (או Claude Code) + C# Dev Kit' },
        { heading: 'Claude Code (CLI)', text: 'cd לתיקיית הפרויקט → claude → משימה בשפה חופשית; מציג diff ומבקש אישור להרצת פקודות' },
        { heading: 'Cursor', text: 'IDE נפרד; Chat/Agent; כללי פרויקט ב-.cursorrules' },
        { heading: 'ניסיון ראשון', text: 'פתחו קובץ C# מיום 3 ובקשו "Explain this file" — ואז "find potential bugs"' },
      ],
      notes: 'הדגימו חי אחד מהכלים על קוד מיום 3. אם יש בעיות התחברות אצל משתתפים — זה הזמן לפתור, לפני המעבדות.',
    },
    {
      type: 'bullets', title: 'Human in the Loop — "AI כותב, אתם חותמים"', icon: 'user',
      bullets: [
        { text: '1. ניסוח המשימה — מה בדיוק, אילו אילוצים, מה זה הצלחה' },
        { text: '2. קריאת ה-diff — כל שינוי נקרא לפני שהוא נכנס ל-git' },
        { text: '3. אימות — build, tests, analyzers, הרצה עם קלט אמיתי' },
        { text: '4. Code Review — קוד AI עובר PR בדיוק כמו קוד אנושי' },
        { text: 'אם אתם לא מוכנים לחתום על שורה — היא לא נכנסת', bold: true },
      ],
      notes: 'זה העיקרון של כל היום. חזרו עליו בכל מודול. אנלוגיה: מפתח חדש ומוכשר שהגיע אתמול — מהיר, לא מכיר את המערכת, צריך review.',
    },
    {
      type: 'two-col', title: 'במה AI טוב — ובמה מסוכן',
      right: { heading: 'טוב מאוד', bullets: ['Boilerplate: DTOs, מיפויים, INotifyPropertyChanged, CRUD', 'הסבר קוד קיים, תרגום בין ספריות', 'בדיקות ראשוניות, regex, דוגמאות שימוש', 'שמות, פירוק מתודות, XML docs', 'שותף לחשיבה: חלופות ו-trade-offs'] },
      left: { heading: 'חלש / מסוכן', bullets: ['APIs ו-packages מומצאים ("הזיות")', 'ידע לא עדכני: WebClient, BinaryFormatter', 'אבטחה: SQL בשרשור, סודות, path traversal', 'הקשר גדול — רואה פחות, ממציא יותר', 'דרישות עסקיות שלא כתבתם — הוא ניחש'] },
      notes: 'תנו דוגמה להזיה: מתודה כמו List.RemoveWhere שנשמעת נכונה ולא קיימת. הקומפיילר הוא קו ההגנה הראשון, אבל לא תופס לוגיקה שגויה.',
    },
    {
      type: 'code', title: 'קריאה ביקורתית של פלט AI',
      code: `// פלט טיפוסי: "parse a date from the user"
var date = DateTime.Parse(input);

// הגרסה שנחתום עליה
if (!DateTime.TryParseExact(input, "yyyy-MM-dd",
        CultureInfo.InvariantCulture,
        DateTimeStyles.None, out var date))
{
    throw new FormatException(
        $"Expected yyyy-MM-dd, got '{input}'.");
}`,
      bullets: ['מתקמפל? build', 'עושה מה שביקשתי? כולל מקרי קצה', 'מה קורה בקלט שגוי?', 'מה הוא הוסיף שלא ביקשתי?', 'תואם לארכיטקטורה שלנו?'],
      notes: 'הדוגמה "עובדת על המחשב שלי" ונופלת עם culture אחרת. חמש השאלות משמאל הן הסדר הקבוע לקריאת כל פלט. נחזור אליהן במודול 4 בהרחבה.',
    },

    // ---------- מודול 2 ----------
    { type: 'section', number: '02', title: 'Prompt Engineering למפתחים', subtitle: 'prompt = מפרט טכני קצר', notes: 'מודול מעשי. איכות הקוד תלויה באיכות הבקשה. נלמד מבנה, איטרציה, ותבניות מוכנות.' },
    {
      type: 'cards', title: 'האנטומיה של Prompt טוב',
      cards: [
        { icon: 'user', heading: 'Role', text: '"Senior .NET developer who values readable code"' },
        { icon: 'sitemap', heading: 'Context', text: '.NET 10, WPF, MVVM, System.Text.Json, הקוד הרלוונטי מודבק' },
        { icon: 'target', heading: 'Task', text: 'מה בדיוק: "Implement CalculateDiscount in the class below"' },
        { icon: 'ban', heading: 'Constraints', text: 'No new packages, thread-safe, keep public API' },
        { icon: 'list', heading: 'Examples', text: 'קלט → פלט: "3 × 100 → 285 (5% over 250)"' },
        { icon: 'file', heading: 'Output', text: '"Return only the C# file, then a 3-line summary"' },
      ],
      notes: 'ששת המרכיבים. לא כולם חובה בכל פעם, אבל כשפלט גרוע — כמעט תמיד חסר אחד מהם, בדרך כלל Context או Examples.',
    },
    {
      type: 'code', title: 'דוגמה: prompt מלא',
      code: `You are a senior C# developer.
Context: .NET 10 console app, nullable enabled,
  file-scoped namespaces.
Task: Write static ParseCsvLine(string line) -> string[]
  supporting quoted fields with commas and "" escapes.
Constraints: no Regex, no external packages, O(n).
Examples:
  a,b,c        -> ["a","b","c"]
  "x, y",z     -> ["x, y","z"]
  "say ""hi""" -> ["say \\"hi\\""]
Output: the method with XML docs + 5 xUnit test cases.`,
      bullets: ['הקשר טכני מדויק', 'משימה עם חתימה', 'אילוצים שמונעים "יצירתיות"', 'דוגמאות = קריטריון הצלחה', 'פורמט פלט חוסך ניקוי'],
      notes: 'קראו את ה-prompt שורה-שורה. שימו לב שהדוגמאות הן למעשה בדיקות. הריצו אותו חי אם יש זמן והשוו את הפלט לציפיות.',
    },
    {
      type: 'bullets', title: 'תנו הקשר, עבדו באיטרציות', icon: 'arrows',
      bullets: [
        { text: 'הדביקו את הקוד הרלוונטי — ממשק, מודל, בדיקה נכשלת — לא את כל הפרויקט' },
        { text: 'ב-IDE: בחירה + #file (Copilot) / @file (Cursor); סוכנים קוראים לבד — כוונו אותם' },
        { text: 'הודעות שגיאה מלאות: "This fails with CS0246… fix without adding packages"' },
        { text: 'לולאה: בקשה קצרה → build/test → הדבקת שגיאה → חידוד → ניקוי' },
        { text: 'שיחה ארוכה = רעש; פתחו חדשה עם סיכום ההחלטות' },
        { text: '"הסבר", "מצא את הבאג", ברווז גומי: "ask me questions that reveal holes"' },
      ],
      notes: 'הטעות הנפוצה: בקשה ענקית אחת. הדגימו איטרציה: בקשה → שגיאת קומפילציה → הדבקה → תיקון. הברווז-גומי מפתיע משתתפים — הכלי טוב בלשאול "מה קורה אם".',
    },
    {
      type: 'two-col', title: 'בדיקות קודם · חלופות לפני מימוש',
      right: { heading: 'Tests first', bullets: ['"Write xUnit tests for IDiscountRule from this spec. Do not implement yet."', 'קל יותר לסקור בדיקות מאשר מימוש', 'אחרי אישור: "Now implement so the tests pass. Show the diff only."', 'זה בדיוק Lab 1'] },
      left: { heading: 'Alternatives & trade-offs', bullets: ['"Give 2–3 ways to persist settings in WPF (JSON, registry, user settings)"', '"For each: pros/cons and when you would choose it. No code yet."', 'החלטת עיצוב נשארת שלכם', 'מונע מימוש שרירותי'] },
      notes: 'שתי תבניות שמשנות את איכות העבודה. בדיקות-קודם הופכת את ה-AI לשותף ב-TDD. חלופות-קודם מונעת את "הפתרון הראשון שעלה לו".',
    },
    {
      type: 'bullets', title: 'ספריית תבניות — Demos/Prompts', icon: 'book',
      bullets: [
        { text: 'Records מ-JSON · LINQ ממשפט · ViewModel ל-MVVM · בדיקות xUnit · Tests-first' },
        { text: 'Refactor עם שמירת התנהגות · הסבר קוד · מצא את הבאג · Regex עם עוגנים' },
        { text: 'מיגרציה (Newtonsoft → System.Text.Json) · XML docs · commit / PR' },
        { text: 'סקירה קפדנית (סיבוב שני) · חלון WPF (XAML) עם RTL ומצבים' },
        { text: '14 תבניות בעברית ובאנגלית — templates-he.md / templates-en.md' },
        { text: 'שמרו את הספרייה בריפו הצוותי: docs/prompts/' },
      ],
      notes: 'הראו את הקבצים. עודדו להעתיק ולהתאים. תבנית 13 (סקירה) ותבנית 14 (XAML) ישמשו במעבדות היום.',
    },
    {
      type: 'code', title: 'קובץ הוראות לפרויקט: CLAUDE.md', file: 'CLAUDE.md (קטע)',
      code: `# Orders Desktop — project instructions
## Stack
- .NET 10, C# 14, WPF, MVVM without 3rd-party frameworks
- System.Text.Json only; HttpClient via IHttpClientFactory
## Structure
- src/Orders.Domain -> entities, rules (no dependencies)
- src/Orders.Application -> services, interfaces, DTOs
- src/Orders.Infrastructure -> JSON repos, HTTP clients
- src/Orders.Wpf -> Views, ViewModels, App.xaml.cs (DI)
## Conventions
- Never DateTime.Now in business logic; inject IClock
- Never swallow exceptions; no logic in code-behind
- FlowDirection="RightToLeft"; styles from Themes/*.xaml
## Commands
- dotnet build · dotnet test · dotnet format --verify-no-changes`,
      bullets: ['נקרא אוטומטית בכל בקשה', 'Copilot: .github/copilot-instructions.md', 'Cursor: .cursorrules', 'מתעד מוסכמות גם לבני אדם', 'דוגמה מלאה ב-Demos/Prompts'],
      notes: 'קובץ ההוראות הוא ההשקעה עם התשואה הגבוהה ביותר: פעם אחת, וכל בקשה מקבלת הקשר. הדגישו שזה גם תיעוד לצוות. Lab 4 מגיע עם קובץ כזה מוכן.',
    },
    {
      type: 'bullets', title: 'MCP — הקשר ממקורות חיצוניים', icon: 'link',
      bullets: [
        { text: 'Model Context Protocol: פרוטוקול פתוח לחיבור כלי AI ל"שרתי הקשר"' },
        { text: 'DB, GitHub, טיקטים, תיעוד פנימי, כלי build — הכלי שואל במקום שתדביקו' },
        { text: 'נתמך ב-Claude Code, Copilot, Cursor ואחרים — בדקו בתיעוד' },
        { text: 'עוד ערוץ שדרכו נתונים יוצאים → אישור שרתים ברשימה לבנה (מודול 6)' },
        { text: 'modelcontextprotocol.io' },
      ],
      notes: 'אזכור קצר בלבד. הרעיון: הקשר אמין יותר, פחות הדבקות. אבל גם סיכון אבטחה — נחזור לזה במודול הארגוני.',
    },

    // ---------- מודול 3 ----------
    { type: 'section', number: '03', title: 'האצת כתיבת קוד', subtitle: 'הבקשה · תוצאה ריאליסטית · מה היה צריך לתקן', notes: 'מודול של דוגמאות. בכל אחת: prompt, פלט טיפוסי, ומה תיקנו. המסר: תמיד יש "מה לתקן".' },
    {
      type: 'code', title: 'Records מ-JSON',
      code: `// Prompt: records for this API response, System.Text.Json,
// decimal for money, DateTimeOffset for dates
public record OrderDto(
    [property: JsonPropertyName("order_id")] int OrderId,
    [property: JsonPropertyName("customer")] CustomerDto Customer,
    [property: JsonPropertyName("items")] List<OrderItemDto> Items,
    [property: JsonPropertyName("created_at")]
    DateTimeOffset CreatedAt);

public record CustomerDto(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("email")] string? Email);

public record OrderItemDto(
    [property: JsonPropertyName("sku")] string Sku,
    [property: JsonPropertyName("qty")] int Qty,
    [property: JsonPropertyName("unit_price")] decimal UnitPrice);`,
      bullets: ['תיקנו: unit_price היה double', 'status → enum + JsonStringEnumConverter', 'בדקו [property: …] על records', 'email יכול להיות null'],
      notes: 'משימה שחוסכת 10 דקות של הקלדה. מה שבודקים: טיפוסים (כסף, זמן), nullability, ושה-attribute מוצב נכון על פרמטר record.',
    },
    {
      type: 'code', title: 'LINQ משפה טבעית · בדיקות xUnit',
      code: `// "top 3 customers by revenue in 2025"
var top = orders
    .Where(o => o.Date.Year == 2025)
    .GroupBy(o => o.CustomerName)          // עדיף CustomerId!
    .Select(g => (Name: g.Key,
        Revenue: g.Sum(o => o.Items.Sum(i => i.Qty * i.UnitPrice))))
    .OrderByDescending(x => x.Revenue)
    .Take(3).ToList();

// "xUnit tests for DiscountCalculator.Calculate"
[Theory]
[InlineData(100, 1, 100)]
[InlineData(300, 1, 285)]     // 5%
[InlineData(100, 10, 90)]     // 10%
[InlineData(300, 10, 270)]    // larger wins
public void Calculate_ReturnsExpected(
        decimal sub, int qty, decimal exp)
    => Assert.Equal(exp, DiscountCalculator.Calculate(sub, qty));`,
      bullets: ['GroupBy לפי שם — שני לקוחות עם אותו שם?', 'ToList: לא לחשב שוב בכל מעבר', 'InlineData + decimal: 19.9 יגיע כ-double', 'בדיקות שמעתיקות את הנוסחה = חסרות ערך'],
      notes: 'ה-LINQ נכון "כמעט" — הקבצה לפי שם היא באג עסקי שה-AI לא יכול לדעת. בבדיקות: מלכודת InlineData עם decimal לא-שלם. הדגישו שבדיקה שמעתיקה את המימוש לא בודקת כלום.',
    },
    {
      type: 'bullets', title: 'Refactoring, מיגרציה, commit — עם רשת ביטחון', icon: 'recycle',
      bullets: [
        { text: 'Extract method: "Extract lines 12–30 into ValidateOrder(Order); keep behavior"' },
        { text: 'Apply pattern: "Replace the switch with Strategy: IPaymentHandler + DI registration"' },
        { text: 'Refactoring רק עם בדיקות או golden master (פלט "לפני" להשוואה) — Lab 2', bold: true },
        { text: 'מיגרציה Newtonsoft → System.Text.Json: "keep JSON identical; list behavioral differences"' },
        { text: 'תיקנו: JsonSerializerOptions חדש בכל קריאה → static readonly' },
        { text: 'Commit/PR: "conventional commit, subject ≤ 72, body = why" — וקראו לפני' },
        { text: 'הכלל: Generate → Verify → Own' },
      ],
      notes: 'הדגימו על Demos/Day4.Demo.LegacyMess: בקשו מהכלי לנתח code smells ואז לחלץ מתודה אחת. הריצו ו-diff מול הפלט המקורי. זה בדיוק התהליך של Lab 2.',
    },
    {
      type: 'lab', title: 'Lab 1 — פיצ\'ר ממפרט עם AI, בדיקות קודם', duration: '60 דקות',
      goal: 'לממש מנוע כללי הנחה (IDiscountEngine) כשהבדיקות כבר קיימות ונכשלות — עם prompt worksheet וסקירה',
      deliverable: 'dotnet test ירוק ב-Starter, דף עבודה מלא, בדיקה חדשה שנוספה ועוברת',
      tasks: ['קראו את הבדיקות ב-Starter והריצו — 18 נכשלות', 'מלאו דף עבודה: Role/Context/Task/Constraints/Examples/Output', 'שלחו prompt עם הממשק, המודל והבדיקות; build + test', 'תקנו בעזרת הודעות הכישלון המלאות', 'סקירה לפי הצ\'ק-ליסט + סקירת AI שנייה (תבנית 13)', 'הוסיפו בדיקה Volume+Coupon ותקנו אם צריך'],
      notes: 'רמזים למרצה: ה-AI נוטה לצבור הנחות ולהתייחס לקופון כאחוז. עברו בין המשתתפים ובדקו שהם קוראים את הקוד ולא רק מריצים בדיקות. NOTES.md בפתרון מפרט טעויות טיפוסיות.',
    },

    // ---------- מודול 4 ----------
    { type: 'section', number: '04', title: 'סקירה ואימות של קוד AI', subtitle: 'Trust but verify', notes: 'המודול החשוב ביותר מבחינת סיכון. קוד AI נראה בטוח בעצמו — לכן צריך צ\'ק-ליסט ולא אינטואיציה.' },
    {
      type: 'bullets', title: 'צ\'ק-ליסט הסקירה', icon: 'check',
      bullets: [
        { text: 'נכונות ומקרי קצה: null, ריק, אפס, שלילי, אוסף ריק, עברית/Unicode' },
        { text: 'אבטחה: injection, סודות בקוד, path traversal, deserialization, PII בלוגים' },
        { text: 'ביצועים: N+1, ToList מיותר, Regex בלי cache, Options חדש בכל קריאה' },
        { text: 'Async: async void, .Result/.Wait, CancellationToken שלא מועבר' },
        { text: 'חריגות ומשאבים: catch ריק, throw ex, HttpClient ב-using, קבצים פתוחים' },
        { text: 'רישוי/העתקה, תלויות NuGet חדשות, עקביות עם הארכיטקטורה' },
      ],
      notes: 'הצ\'ק-ליסט המלא בחומר. בקשו מהמשתתפים לשמור אותו פתוח בזמן Lab 3. הדגישו: "נראה טוב" הוא לא קריטריון.',
    },
    {
      type: 'code', title: 'אימות עם כלים — לא רק עיניים', file: 'Directory.Build.props',
      code: `<PropertyGroup>
  <Nullable>enable</Nullable>
  <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
  <AnalysisLevel>latest-recommended</AnalysisLevel>
  <EnforceCodeStyleInBuild>true</EnforceCodeStyleInBuild>
</PropertyGroup>

<!-- CI -->
dotnet build -warnaserror
dotnet format --verify-no-changes
dotnet test`,
      bullets: ['Roslyn analyzers (CAxxxx) מובנים ב-SDK', 'TreatWarningsAsErrors: אזהרת nullable = build נכשל', 'dotnet format לפי .editorconfig', 'SonarLint, Roslynator, StyleCop — נוספים', 'הרצה עם קלט אמיתי: עברית, תאריכים, de-DE'],
      notes: 'קוד AI נוטה לייצר אזהרות nullable — TreatWarningsAsErrors הופך אותן לחסימה. ב-Lab 3 ה-analyzers תופסים לבד את CS8603. הראו את הקבצים בפתרון.',
    },
    {
      type: 'code', title: 'באגים עדינים שחוזרים בפלט AI (1)',
      code: `// 1. async void — חריגה מפילה את התהליך
private async void LoadAsync() { await _api.GetAsync(); }
private async Task LoadAsync() { await _api.GetAsync(); }   // תיקון

// 2. parsing תלוי-תרבות: ב-de-DE "19.90" → 1990
decimal.Parse("19.90");
decimal.Parse("19.90", CultureInfo.InvariantCulture);        // תיקון

// 3. Now לעומת UtcNow (שעון קיץ, אזורי זמן)
var expires = DateTime.Now.AddHours(1);
var expires = DateTimeOffset.UtcNow.AddHours(1);              // תיקון
// ועדיף: TimeProvider מוזרק → ניתן לבדיקה

// 4. off-by-one: "the last n items"
items.Skip(items.Count - n - 1)                               // n+1
items.Skip(Math.Max(0, items.Count - n))                      // תיקון`,
      bullets: ['async void רק ל-event handlers', 'נתוני מכונה = Invariant', 'זמן = תלות שמזריקים', 'גבולות: בדיקה עם 0, 1, n, n+1'],
      notes: 'ארבעה באגים שמופיעים שוב ושוב. הריצו את תרגיל 9 בפתרונות (de-DE) — המספר 1990 במקום 19.90 משכנע יותר מכל הסבר.',
    },
    {
      type: 'code', title: 'באגים עדינים שחוזרים בפלט AI (2)',
      code: `// 5. HttpClient חדש בכל קריאה → מיצוי sockets
using var client = new HttpClient();
// תיקון: מופע מוזרק / IHttpClientFactory

// 6. Dictionary ממספר threads → השחתה
private readonly Dictionary<string, int> _cache = new();
private readonly ConcurrentDictionary<string, int> _cache = new();

// 7. בליעת חריגות: "לא נפל" ≠ "עבד"
try { Save(); } catch { }
try { Save(); }
catch (IOException ex) { _logger.LogError(ex, "Save failed"); throw; }

// 8. path traversal: "..\\..\\secrets.txt"
var path = Path.Combine(root, userFileName);
var full = Path.GetFullPath(Path.Combine(root, userFileName));
var safeRoot = Path.GetFullPath(root) + Path.DirectorySeparatorChar;
if (!full.StartsWith(safeRoot)) throw new UnauthorizedAccessException();`,
      bullets: ['משאבים: מי מחזיק, מי משחרר', 'thread-safety לא נראה בבדיקה בודדת', 'catch ריק = ממצא כמעט תמיד', 'קלט משתמש בנתיב = אבטחה'],
      notes: 'כל שמונת הבאגים מופיעים ב-Lab 3 בקטעים "שנוצרו ב-AI". שאלו: איזה מהם ה-analyzers היו תופסים? (מעט מאוד — לכן צריך סוקר.)',
    },
    {
      type: 'bullets', title: 'AI סוקר AI · סקירת PR', icon: 'eye',
      bullets: [
        { text: 'הכלי שכתב "מאמין" בקוד — בקשו סקירה בשיחה חדשה או בכלי אחר' },
        { text: 'תבנית 13: "strict senior reviewer; check null, async, culture, IDisposable, thread safety, injection… severity + minimal fix"' },
        { text: '"List the assumptions" · "5 inputs that would break this" · "what would a security auditor flag"' },
        { text: 'Copilot code review ב-PR מציע הערות בשורות; Claude סוקר diff/PR — בדקו בתיעוד' },
        { text: 'הערות AI = קלט לסוקר האנושי. merge מאשר אדם', bold: true },
        { text: 'תהליך 10 דקות: דרישה → build/test/format → צ\'ק-ליסט → AI שני → תיקון → commit' },
      ],
      notes: 'הדגימו: הדביקו קטע מ-Lab 3 בשיחה חדשה עם תבנית 13. השוו את הממצאים לצ\'ק-ליסט. שימו לב ל-false positives — גם הם חלק מהלמידה.',
    },
    {
      type: 'lab', title: 'Lab 3 — סקירת קוד שנוצר ב-AI', duration: '45 דקות',
      goal: '7 קטעי C# "מ-AI" שמתקמפלים ונראים סבירים; בכל אחד 1–3 באגים מוסתרים. סקירה ידנית, סקירת AI שנייה, תיקון',
      deliverable: 'טבלת ממצאים (≥ 12), קבצים מתוקנים, dotnet run מדפיס ALL CHECKS PASSED',
      tasks: ['סקירה ידנית לפי הצ\'ק-ליסט — בלי AI (20 דק\')', 'טבלת ממצאים: קובץ, שורה, קטגוריה, חומרה, תיקון', 'סקירת AI שנייה עם תבנית 13 — מה פספסתם? מה הוא פספס? false positives?', 'תיקון מינימלי לכל ממצא; build ללא אזהרות', 'הריצו את הבדיקות העצמיות ב-Program.cs', 'בונוס: TreatWarningsAsErrors + AnalysisLevel — מה נתפס אוטומטית?'],
      notes: 'השלב הראשון בלי AI הוא קריטי — לאמן את העין. הפתרון כולל FINDINGS.md עם 22 ממצאים ורשימת false positives טיפוסיים. שימו לב ל-S2 (ללא בדיקה בזמן ריצה — סקירה בקריאה בלבד).',
    },

    // ---------- מודול 5 ----------
    { type: 'section', number: '05', title: 'ארכיטקטורה ניתנת לתחזוקה', subtitle: 'השלד שקוד AI צריך להשתלב בו', notes: 'כשקוד נכתב מהר, המבנה הוא מה שמונע ערימה. נבנה שכבות, DI, options, logging — גם ב-WPF.' },
    {
      type: 'cards', title: 'ארכיטקטורת שכבות — התלויות מצביעות פנימה',
      cards: [
        { icon: 'window', heading: 'UI', text: 'Views, ViewModels — "איך זה נראה". מכיר Application. אפס לוגיקה עסקית.' },
        { icon: 'gear', heading: 'Application', text: 'Services, use-cases, ממשקים (IOrderRepository), DTOs — "מה המערכת עושה".' },
        { icon: 'cube', heading: 'Domain', text: 'Entities, value objects, rules — "מה נכון עסקית". ללא תלויות בכלל.' },
        { icon: 'db', heading: 'Infrastructure', text: 'JSON/DB repositories, HTTP, קבצים, שעון — מממש את ממשקי Application.' },
      ],
      notes: 'בפרויקט קטן זה יכול להיות תיקיות בפרויקט אחד — מה שחשוב הוא כיוון התלויות. AI יפר את זה בשמחה (HttpClient ב-ViewModel) אלא אם קובץ ההוראות אומר אחרת.',
    },
    {
      type: 'code', title: 'DI + Hosting — גם ב-WPF', file: 'App.xaml.cs (Demos/Day4.Demo.DiHostWpf)',
      code: `public partial class App : Application
{
    private readonly IHost _host = Host.CreateDefaultBuilder()
        .ConfigureServices((ctx, services) =>
        {
            services.Configure<AppOptions>(
                ctx.Configuration.GetSection("App"));
            services.AddSingleton<IClock, SystemClock>();
            services.AddSingleton<IOrderRepository,
                JsonOrderRepository>();
            services.AddTransient<OrderService>();
            services.AddTransient<MainViewModel>();
            services.AddTransient<MainWindow>();
        })
        .Build();

    protected override async void OnStartup(StartupEventArgs e)
    {
        await _host.StartAsync();
        _host.Services.GetRequiredService<MainWindow>().Show();
    }
}`,
      bullets: ['Microsoft.Extensions.Hosting', 'להסיר StartupUri מ-App.xaml', 'CreateDefaultBuilder קורא appsettings.json', 'Singleton / Transient — לפי משמעות', 'MainWindow(MainViewModel vm) בבנאי'],
      notes: 'הריצו את הדמו (ב-Windows) או הראו את הקוד. הדגישו: constructor injection, לא service locator. ה-Options וה-ILogger מגיעים "בחינם" מה-host.',
    },
    {
      type: 'code', title: 'Options pattern · ILogger · בדיקתיות',
      code: `// appsettings.json: { "App": { "DataFile": "...", "Currency": "ILS" } }
public sealed class AppOptions
{
    public string DataFile { get; set; } = "orders.json";
    public string Currency { get; set; } = "ILS";
}

public sealed class OrderService(
    IOrderRepository repo, IClock clock,
    IOptions<AppOptions> options, ILogger<OrderService> logger)
{
    public async Task<Order> CreateAsync(NewOrderRequest req,
                                         CancellationToken ct)
    {
        var order = Order.Create(req.CustomerId, req.Lines,
                                 clock.UtcNow);
        await repo.SaveAsync(order, ct);
        logger.LogInformation("Order {OrderId} created ({Currency})",
            order.Id, options.Value.Currency);
        return order;
    }
}
// בבדיקה: new OrderService(new InMemoryRepo(), new FakeClock(..))`,
      bullets: ['אין קבועים קסומים בקוד', 'Structured logging: placeholders, לא interpolation', 'ממשק לכל מה שמדבר עם העולם', 'TimeProvider מובנה ב-.NET', 'primary constructor (C# 12+)'],
      notes: 'שלושה מנגנונים ששווה להכיר גם למי שלא בונה WPF: Options, ILogger, והפשטת הזמן. הבדיקה בתחתית מראה למה זה משתלם.',
    },
    {
      type: 'two-col', title: 'SOLID בקצרה · מוסכמות',
      right: { heading: 'SOLID עם C#', bullets: ['S: OrderProcessor שמחשב+שומר+מדפיס → 3 מחלקות', 'O: switch על סוג הנחה → IDiscountRule + רשימה', 'L: ReadOnlyRepository שזורק ב-Save → ממשק נפרד', 'I: IRepository עם 15 מתודות → Read / Write', 'D: new SmtpClient() בתוך service → IEmailSender'] },
      left: { heading: 'מוסכמות שמחזיקות', bullets: ['file-scoped namespaces, nullable, מחלקה לקובץ', 'PascalCase / _camelCase / I-prefix / Async-suffix', 'מתודות עד ~30 שורות; קובץ 800 שורות = אזהרה (גם ל-AI)', 'README קצר ונכון; ADR לכל החלטה משמעותית', 'חוב טכני: 10–15% מהספרינט + "כלל הצופה"'] },
      notes: 'SOLID לא כתאוריה אלא כתיקונים קונקרטיים. ADRs עוזרים גם ל-AI להבין "למה". Refactoring קבוע — אחרת AI מצטבר לערימה.',
    },
    {
      type: 'bullets', title: 'לשמור קוד AI עקבי עם הארכיטקטורה', icon: 'shield',
      bullets: [
        { text: 'קובצי הוראות מתארים שכבות, כללים ואיסורים (CLAUDE.md / copilot-instructions.md)' },
        { text: 'תבנית אחת "מושלמת" להפניה: "Follow the pattern in Services/CustomerService.cs"' },
        { text: 'Analyzers + .editorconfig אוכפים סגנון אוטומטית' },
        { text: 'בדיקות ארכיטקטורה: Domain לא מפנה ל-Infrastructure' },
        { text: 'Review עם שאלה קבועה: "האם זה בשכבה הנכונה?"' },
        { text: 'טעויות: service locator, singleton עם state של מסך, ממשק לכל מחלקה "כי ככה עושים"' },
      ],
      notes: 'חיבור בין מודול 2 (הוראות) למודול 5 (ארכיטקטורה). הדגישו: ממשק שווה כשיש מימוש שני — אמיתי או fake.',
    },
    {
      type: 'lab', title: 'Lab 2 — Refactoring של legacy עם Golden Master', duration: '75 דקות',
      goal: 'SubscriptionBiller מבולגן (מתודה אחת, מספרי קסם, double לכסף) → services + ממשקים + DI + בדיקות, בלי לשנות תו אחד בפלט',
      deliverable: 'dotnet run זהה ל-expected-output.txt, ≥ 3 שירותים עם DI, ≥ 8 בדיקות כולל golden master',
      tasks: ['רשת ביטחון: diff מול expected-output.txt + commit + בדיקת golden master', 'ניתוח עם AI: code smells לפי סיכון + מבנה יעד — בלי קוד', 'צעדים קטנים: extract → record + parser → decimal → services + DI → tests', 'אחרי כל צעד: run + diff', 'סקירה: אין double, אין מספרי קסם, Program = composition root', 'סקירת AI שנייה ותיקונים'],
      notes: 'מעבדה לתרגול עצמי או ליום נוסף. הטעות הנפוצה: עיגול "כדי להיות מדויק" ששובר את ה-golden master. NOTES.md בפתרון מפרט. Demos/Day4.Demo.Refactored הוא דוגמה מקבילה.',
    },

    // ---------- מודול 6 ----------
    { type: 'section', number: '06', title: 'שילוב AI בסביבה ארגונית', subtitle: 'מדיניות, אבטחה, CI, מדידה', notes: 'מעבר מ"אני" ל"אנחנו". בלי מספרים מומצאים — עקרונות, ותמיד: בדקו את החוזה והתיעוד של הספק.' },
    {
      type: 'bullets', title: 'פרטיות נתונים וקניין רוחני', icon: 'lock',
      bullets: [
        { text: 'השאלה: מה קורה לטקסט שאני שולח? אימון? שמירה? היכן? — תלוי בתוכנית ובספק' },
        { text: 'תוכניות ארגוניות בד"כ: אי-שימוש לאימון, ניהול מרכזי, SSO, audit — קראו את המסמכים העדכניים' },
        { text: 'On-prem / cloud פרטי (Azure OpenAI, Bedrock, Vertex, מודלים פתוחים) — קיימים, עם trade-offs' },
        { text: 'קוד פתוח/תרגול: מותר · קוד קנייני: רק בתוכנית מאושרת · PII: רק באישור DPO' },
        { text: 'סודות (מפתחות, סיסמאות, connection strings): לעולם לא, בשום כלי', bold: true },
        { text: 'לפני הדבקה: חפשו password/apikey/token; placeholders <REDACTED>' },
      ],
      notes: 'אל תצטטו תנאים ספציפיים — הם משתנים. הפנו ל-trust centers של הספקים. הדגישו את הטבלה: איזה מידע, לאיזה כלי. סודות — אפס סובלנות.',
    },
    {
      type: 'cards', title: 'רישוי · Compliance · שערי סקירה',
      cards: [
        { icon: 'tag', heading: 'רישוי ו-IP', text: 'פלט עשוי לדמות קוד קיים; סינון קוד ציבורי ו-indemnification בתוכניות ארגוניות (בדקו). SCA על קוד AI כמו על כל תלות.' },
        { icon: 'file', heading: 'Compliance & Audit', text: 'תעדו: אילו כלים מאושרים, למי, לאילו נתונים. שרתי MCP ברשימה לבנה. ADR למדיניות.' },
        { icon: 'git', heading: 'בעלות ו-Review gates', text: 'אין הבדל באחריות בין קוד AI לאנושי. כל קוד עובר PR; סוכן לא עושה merge; reviewer אנושי + branch protection.' },
        { icon: 'terminal', heading: 'CI חובה', text: 'build -warnaserror, format --verify-no-changes, tests, secret scanning, SCA. סקירת AI = הערה, לא check חוסם.' },
      ],
      notes: 'ארבעה עמודים של מדיניות. הנקודה החשובה: מי שעשה commit — אחראי. שקיפות ב-PR ("Generated with…; reviewed by…") מקובלת.',
    },
    {
      type: 'code', title: 'CI — השלד הרגיל, AI מצטרף כסוקר', file: '.github/workflows/ci.yml',
      code: `name: ci
on: [pull_request]
jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-dotnet@v4
        with: { dotnet-version: '10.0.x' }
      - run: dotnet restore
      - run: dotnet build --no-restore -warnaserror
      - run: dotnet format --verify-no-changes
      - run: dotnet test --no-build`,
      bullets: ['אותו CI לקוד אנושי ו-AI', 'Copilot code review / Claude — דרך הגדרות הריפו', 'תוצאת AI review = הערה', 'PRs קטנים — אחרת לא נסקרים', 'secret scanning + SCA כ-steps נוספים'],
      notes: 'אין קסם: CI רגיל וטוב. אל תמציאו step ל-AI review — הוא מוגדר בכלי עצמו. הדגישו שסוכן שמייצר PR ענק הוא בעיה של תהליך, לא של כלי.',
    },
    {
      type: 'two-col', title: 'מדידת השפעה · הכשרה · הטמעה',
      right: { heading: 'מה מודדים (לא שורות קוד!)', bullets: ['Lead time: מ-issue ל-merge', 'Deployment frequency', 'Change failure rate, defects per release', 'זמן review — האם PRs נתקעים?', 'שביעות רצון מפתחים', 'baseline לפני, בדיקה אחרי 3 חודשים'] },
      left: { heading: 'צוות ותוכנית 90 יום', bullets: ['סדנה (כמו היום), קובצי הוראות בכל ריפו', 'ספריית prompts צוותית, AI champion, pairing', '0: מדיניות ואישורים (שבועות 1–2)', '1: פיילוט — צוות אחד, CI מחמיר, baseline', '2: הרחבה — עוד צוותים, AI review ב-PR', '3: ייצוב — מדדים, עדכון מדיניות'] },
      notes: 'DORA-style. "יותר קוד = יותר ערך" הוא הטעות הנפוצה. ה-playbook הוא דוגמה — התאימו לארגון. אל תאסרו לגמרי: המפתחים ישתמשו בחשבון אישי בסתר.',
    },
    {
      type: 'bullets', title: 'סיכונים ומיתונים', icon: 'warning',
      bullets: [
        { text: 'דליפת סודות/נתונים → מדיניות + secret scanning + תוכנית ארגונית + הדרכה' },
        { text: 'קוד לא בטוח → analyzers, SAST, צ\'ק-ליסט, בדיקות' },
        { text: 'APIs/חבילות מומצאים → build ב-CI, dotnet add package ידני' },
        { text: 'ירידה במיומנות → הסבר חובה ב-PR, pairing, ימי "ללא AI" למתחילים' },
        { text: 'PRs ענקיים → כלל גודל, משימות קטנות לסוכנים' },
        { text: 'תלות בספק → prompts בקבצים, לא בכלי; מעקב חלופות' },
        { text: 'עלות → מושבים לפי צורך, מעקב שימוש ב-admin console' },
      ],
      notes: 'טבלה שאפשר להעתיק למסמך המדיניות של הארגון. שאלו את המשתתפים איזה סיכון הכי רלוונטי אצלם — בדרך כלל דליפת נתונים ו-PRs גדולים.',
    },

    // ---------- מודול 7 — מושב הסיום ----------
    { type: 'section', number: '07', title: 'כלי AI לפיתוח ממשק משתמש', subtitle: 'מושב הסיום: מסקיצה ל-XAML מחובר ל-ViewModel', notes: 'המושב שהמשתתפים ביקשו. UI הוא המקום שבו AI חוסך הכי הרבה זמן — ומייצר הכי הרבה "כמעט נכון". נחבר את כל היום: prompt, review, ארכיטקטורה.' },
    {
      type: 'cards', title: 'קטגוריות של כלי AI ל-UI',
      cards: [
        { icon: 'laptop', heading: 'עוזרים בתוך ה-IDE', text: 'Copilot, Claude Code, Cursor, JetBrains AI — XAML / WinForms / Blazor ישירות בפרויקט, עם הקשר של ה-ViewModel. הדרך הראשית שלנו.' },
        { icon: 'wand', heading: 'Prompt-to-UI (web)', text: 'v0 by Vercel, Lovable, Bolt.new — אפליקציית React/Next מתיאור, עם preview. אב-טיפוס מהיר; פלט web.' },
        { icon: 'paint', heading: 'עיצוב עם AI', text: 'Figma AI / Figma Make, Uizard, Galileo AI — mockups ומסכים, לפעמים קוד ראשוני. תיאום עם מעצבים.' },
        { icon: 'eye', heading: 'תמונה/סקיצה → קוד', text: 'כלי screenshot-to-code; Claude/Copilot עם תמונה כקלט; תוספי Figma design-to-code. שחזור מסך קיים.' },
      ],
      notes: 'הערה כנה: רוב כלי ה-Prompt-to-UI מייצרים web. ל-WPF/WinForms — העוזרים ב-IDE ומודלי צ\'אט. אפשר לעצב ב-v0/Figma ולבקש "translate this layout to WPF XAML". בדקו בתיעוד העדכני של כל כלי.',
    },
    {
      type: 'steps', title: 'תהליך עבודה ל-C# desktop',
      steps: [
        { heading: 'סקיצה', text: 'נייר / Figma / ASCII: אילו אזורים, מה בכל אזור' },
        { heading: 'Prompt', text: 'תיאור + ViewModel + אילוצים: MVVM, RTL, resources, מידות, מצבים' },
        { heading: 'XAML', text: 'הכלי מייצר — build מיד; הסירו namespaces/controls מומצאים' },
        { heading: 'חידוד', text: 'prompt שני: ResourceDictionary, סטיילים, empty/loading/error, נגישות' },
        { heading: 'Binding', text: 'ViewModel קיים/חדש, design-time data, Output window ל-binding errors' },
        { heading: 'Review', text: 'צ\'ק-ליסט UI + הרצה + RTL + נגישות + עקביות' },
      ],
      notes: 'ששה שלבים, כל אחד קצר. הסיבוב הראשון נותן מבנה, השני איכות. הדגימו חי את השלבים 2–4 עם Demos/Day4.Demo.AiGeneratedUi כתוצאה הסופית.',
    },
    {
      type: 'code', title: 'הדוגמה: Orders Dashboard — ה-prompt הראשון',
      code: `You are a WPF/XAML expert. Generate MainWindow.xaml only (no code-behind logic).
Context: .NET 10 WPF, MVVM. DataContext is OrdersDashboardViewModel with:
  string SearchText; ObservableCollection<string> Statuses; string? SelectedStatus;
  ICommand RefreshCommand, ExportCommand, NewOrderCommand;
  int TodayOrders; decimal TodayRevenue; int PendingOrders; int ActiveCustomers;
  ObservableCollection<OrderRow> Orders (Id, Customer, Date, Total, Status);
  OrderRow? SelectedOrder; bool IsBusy; string StatusMessage.
Layout: Grid, 4 rows: toolbar (search TextBox, status ComboBox, Refresh) + title;
  4 KPI cards in a UniformGrid; DataGrid (read-only, currency format);
  status bar with StatusMessage + Export/New Order buttons.
Constraints: FlowDirection="RightToLeft", Hebrew labels, MinWidth 900,
  brushes via StaticResource keys defined in Window.Resources,
  ProgressBar IsIndeterminate bound to IsBusy (BooleanToVisibilityConverter),
  standard WPF controls only (no third-party).`,
      notes: 'שימו לב לחתימות ה-ViewModel המודבקות — בלעדיהן הכלי ממציא שמות. הסקיצה (ASCII) נמצאת בחומר. ה"Constraints" מונעים את שלוש הטעויות הנפוצות: ספרייה חיצונית, צבעים קשיחים, code-behind.',
    },
    {
      type: 'code', title: 'התוצאה (מקוצרת) — Demos/Day4.Demo.AiGeneratedUi',
      code: `<Window ... Title="לוח הזמנות" MinWidth="900" MinHeight="600"
        FlowDirection="RightToLeft" FontFamily="Segoe UI">
  <Window.Resources>
    <BooleanToVisibilityConverter x:Key="BoolToVis" />
    <SolidColorBrush x:Key="AccentBrush" Color="#512BD4" />
    <Style x:Key="Card" TargetType="Border"> ... </Style>
  </Window.Resources>
  <Grid Margin="16">
    <!-- Rows: Auto / Auto / * / Auto -->
    <DockPanel Grid.Row="0"> <!-- title, search, status, refresh --> </DockPanel>
    <UniformGrid Grid.Row="1" Columns="4">
      <Border Style="{StaticResource Card}"> ...TodayOrders... </Border>
    </UniformGrid>
    <DataGrid Grid.Row="2" ItemsSource="{Binding Orders}"
              SelectedItem="{Binding SelectedOrder}"
              AutoGenerateColumns="False" IsReadOnly="True">
      <DataGridTextColumn Header="סכום"
          Binding="{Binding Total, StringFormat=C}" />
    </DataGrid>
    <DockPanel Grid.Row="3"> <!-- buttons, ProgressBar, status --> </DockPanel>
  </Grid>
</Window>`,
      bullets: ['build עבר — אבל:', 'xmlns של ספרייה חיצונית שלא ביקשנו → הוסר', 'StringFormat=C תלוי culture → פורמט מפורש ₪', 'חסר empty state', 'לכן — סיבוב שני'],
      notes: 'הפלט הראשון תמיד "כמעט". שלוש הבעיות משמאל הן הנפוצות ביותר. פתחו את הפרויקט האמיתי והראו את הגרסה אחרי הסקירה — כולל design-time data במעצב.',
    },
    {
      type: 'code', title: 'ה-prompt השני — חידוד',
      code: `Refine the XAML you produced:
1. Add an empty-state TextBlock "אין הזמנות להצגה" centered
   over the DataGrid, visible when Orders.Count == 0
   (DataTrigger in a Style, no converter).
2. Move brushes and the Card style to Themes/Dashboard.xaml
   and merge it.
3. KPI value TextBlocks use a shared style "KpiValue"
   (28, Bold, AccentBrush).
4. DataGrid: alternating row background, no row headers,
   bold column headers.
5. Add AutomationProperties.Name to the search TextBox
   and all buttons.
Show only the changed parts.`,
      bullets: ['סיבוב 1: מבנה', 'סיבוב 2: איכות ועקביות', 'סיבוב 3: מצבי שגיאה, מסכים קטנים', '"Show only the changed parts" — diff קטן לסקירה'],
      notes: 'בקשות ממוספרות וקונקרטיות. "Show only changed parts" חוסך זמן סקירה. הריצו חי אם אפשר — המשתתפים אוהבים לראות את ה-empty state מופיע.',
    },
    {
      type: 'bullets', title: 'טיפים ל-prompts טובים ל-UI', icon: 'bulb',
      bullets: [
        { text: 'Layout קודם: Grid N×M, מה ב-Dock, מה נמתח (*) ומה קבוע (Auto); מידות Min/Max; scale 4/8/12/16' },
        { text: 'מצבים במפורש: loading, empty, error, disabled — ה-AI מדלג עליהם' },
        { text: 'Validation: Validation.ErrorTemplate + ValidatesOnNotifyDataErrors (יום 3)' },
        { text: 'נגישות: AutomationProperties.Name, ניגודיות, TabIndex, גופנים מ-resources' },
        { text: 'RTL ועברית! FlowDirection על החלון; מספרים/סכומים LTR בתא; DockPanel.Dock מתהפך; גופן שתומך בעברית', bold: true },
        { text: 'הדביקו את ה-ViewModel; "standard controls only", "no code-behind logic", "no new packages"' },
      ],
      notes: 'RTL הוא הפינה שהכי קל לשכוח ושהכי כואבת בסוף. הדגימו: FlowDirection על Window, ואז שדה סכום עם LeftToRight. שאלו מי כבר נתקל ב-DockPanel הפוך.',
    },
    {
      type: 'two-col', title: 'מלכודות ב-XAML מ-AI · עקביות עיצוב',
      right: { heading: 'מלכודות נפוצות', bullets: ['Namespace חסר/מומצא (xmlns:mah, xmlns:sys לא מוגדר)', 'Control שלא קיים: <Card>, <Icon>, <NumericUpDown>', 'Property לא קיים: CornerRadius על Button', 'Binding לשם שגוי — בדקו Output window', 'Converter ב-StaticResource שלא הוגדר', 'Button_Click עם לוגיקה ב-code-behind'] },
      left: { heading: 'לשמור עקביות', bullets: ['ResourceDictionary אחד: Themes/Colors.xaml + Controls.xaml, ממוזג ב-App.xaml', 'קובץ הוראות: "never hardcode colors; FlowDirection RTL; spacing 4/8/12/16"', 'חלון דוגמה "מושלם": "Match the look of Views/CustomersView.xaml"', 'Review ויזואלי: צילום מסך ב-PR', 'design-time data (d:DataContext) למעצב'] },
      notes: 'טבלת המלכודות המלאה בחומר, עם תיקון לכל שורה. העקביות מגיעה מ-ResourceDictionary + הוראות — לא מ"בקשה יפה". Demos/Day4.Demo.AiGeneratedUi מדגים את כולם.',
    },
    {
      type: 'bullets', title: 'מתי Blazor + AI מהיר יותר · מה הלאה', icon: 'rocket',
      bullets: [
        { text: 'Web/מובייל או צוות מעורב → Blazor; כלי Prompt-to-UI מייצרים HTML/React בשל → המרה ל-.razor עם העוזר' },
        { text: 'Blazor Hybrid (MAUI/WPF host) משלב Razor בתוך desktop; WPF נשאר נכון ל-desktop-only ולמערכות קיימות' },
        { text: '.NET MAUI — desktop + mobile · Avalonia — XAML חוצה-פלטפורמות · CommunityToolkit.Mvvm — source generators' },
        { text: 'ASP.NET Core לצד השרת · Microsoft Learn paths (חינם) · "What\'s new in C#"' },
        { text: 'תרגלו: לנסח, לקרוא, לבדוק ולהסביר קוד — עם AI ובלעדיו', bold: true },
      ],
      notes: 'סגירה של המודול: הבחירה בין WPF ל-Blazor היא ארכיטקטונית, ו-AI לא משנה אותה — רק מאיץ. רשימת "מה הלאה" עם קישורים בחומר.',
    },
    {
      type: 'lab', title: 'Lab 4 — Capstone: Expense Tracker ב-WPF עם AI', duration: '75 דקות',
      goal: 'אפליקציית WPF שלמה ממפרט של עמוד אחד: XAML + ViewModel + JSON + validation — מקצה לקצה עם AI, ואז סקירה וליטוש',
      deliverable: 'אפליקציה שנבנית ורצה, נתונים שורדים הפעלה מחדש, validation ליד השדה, רשימת "מה ה-AI טעה ומה תיקנתי"',
      tasks: ['תכנון עם AI: רשימת קבצים + חברי ה-ViewModel — בלי קוד', 'Models + IExpenseRepository + JSON (כתיבה אטומית) + בדיקות', 'ViewModels עם INotifyDataErrorInfo ו-AsyncRelayCommand (בלי async void)', 'XAML לפי SPEC.md: RTL, Validation.ErrorTemplate, empty state, ProgressBar', 'סקירה: צ\'ק-ליסט מודול 4 + צ\'ק-ליסט UI + סקירת AI שנייה', 'בונוס: CSV export, SQLite, DI עם Hosting'],
      notes: 'ה-capstone. Starter מגיע עם CLAUDE.md, copilot-instructions.md, Themes ו-Mvvm.cs. הפתרון מפוצל ל-Core (נבדק ב-CI) + WPF. הטעויות הטיפוסיות ב-NOTES.md: async void, DateTime.Now, catch ריק, binding ל-Form.AddCommand.',
    },
    {
      type: 'quote',
      text: 'הכלים ישתנו כל כמה חודשים. מה שיישאר הוא היכולת שלכם לנסח, לקרוא, לבדוק ולהסביר קוד.',
      author: 'המסר של יום 4',
      notes: 'רגע לעצור. שאלו: מה הדבר האחד שתעשו אחרת ביום ראשון בבוקר? תנו לכמה משתתפים לענות.',
    },
    {
      type: 'end', title: 'סיכום הקורס — ארבעה ימים',
      bullets: [
        { text: 'יום 1: OOP, collections, LINQ, חריגות — הבסיס לחשיבה ב-C#' },
        { text: 'יום 2: async, HttpClient, JSON — לדבר עם העולם' },
        { text: 'יום 3: WPF, MVVM, validation, styles — לבנות ממשק' },
        { text: 'יום 4: AI ככלי האצה — prompt טוב, review קפדני, ארכיטקטורה שמחזיקה, מדיניות' },
        { text: 'לקחת הביתה: Demos/Prompts, קובצי הוראות, צ\'ק-ליסט הסקירה, Lab 4' },
        { text: 'AI כותב — אתם חותמים. תודה!' },
      ],
      footer: 'חומרים, דמואים ופתרונות בריפו הקורס · שאלות? זה הזמן',
      notes: 'סיכום ארבעת הימים בשורה לכל יום. הזכירו את החומרים שנשארים אצלם. פתחו לשאלות ולמשוב על הקורס. תודה למשתתפים.',
    },
  ],
};
