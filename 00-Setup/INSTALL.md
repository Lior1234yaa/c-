<div dir="rtl">

# מה צריך להתקין — מדריך התקנה לקורס C# / .NET

מסמך זה מלווה אתכם מאפס עד סביבת עבודה מוכנה לכל ארבעת ימי הקורס. המדריך כתוב **Windows-first** כי ביום 3 נבנה אפליקציות WPF (שרצות רק ב-Windows), אבל בימים 1, 2 ו-4 אפשר לעבוד גם על macOS או Linux — יש הערות מתאימות לאורך הדרך.

מומלץ לסיים את ההתקנה **לפני** היום הראשון ולהריץ את תוכנית האימות (סעיף 10). מי שמגיע עם מחשב ארגוני — כדאי לבדוק מראש הרשאות התקנה ו-proxy (ראו "פתרון בעיות").

## 1. סקירה מהירה — מה נדרש לכל יום

| יום | נושא | חובה | מומלץ |
|-----|------|------|-------|
| 1 | OOP, Collections, Exceptions | .NET 10 SDK, Visual Studio או VS Code, Git | Windows Terminal, LINQPad |
| 2 | Multithreading, Async, REST + JSON | כל הנ"ל | Postman / REST Client, `curl` |
| 3 | WPF (והשוואה ל-WinForms) | **Windows + Visual Studio** עם workload של .NET desktop | — |
| 4 | פיתוח בעזרת AI וקוד תחזוקתי | חשבונות לכלי AI (Copilot / Claude / ChatGPT), Node.js LTS | Cursor |

## 2. .NET 10 SDK (LTS)

ה-SDK הוא הבסיס לכל דבר: הקומפיילר, ה-CLI (`dotnet`) וה-Runtime.

1. היכנסו ל-https://dotnet.microsoft.com/download
2. בחרו **.NET 10** (גרסת LTS — תמיכה ארוכת טווח) והורידו את ה-**SDK** (לא רק Runtime!) למערכת ההפעלה שלכם.
   - Windows: x64 Installer (ברוב המחשבים). אם יש לכם מחשב עם מעבד ARM — בחרו Arm64.
   - macOS: בחרו Arm64 למחשבי Apple Silicon (M1 ואילך) או x64 למחשבי Intel.
   - Linux: עדיף להתקין דרך מנהל החבילות של ההפצה (ההוראות בעמוד ההורדה, לפי הפצה).
3. הריצו את ההתקנה, ואז **פתחו טרמינל חדש** (טרמינל שהיה פתוח לפני ההתקנה לא מכיר את ה-PATH המעודכן).

אימות:

<div dir="ltr">

```bash
dotnet --version
dotnet --info
```

</div>

הפלט של `dotnet --version` אמור להתחיל ב-`10.` (למשל `10.0.100`). `dotnet --info` מציג את כל ה-SDKs וה-Runtimes המותקנים ואת מערכת ההפעלה — שימושי מאוד לפתרון בעיות.

הערה: אם אתם מתקינים Visual Studio (סעיף 3) עם workload של .NET, ה-SDK מותקן יחד איתו. עדיין כדאי להריץ `dotnet --version` ולוודא שהגרסה היא 10.

## 3. Visual Studio Community (Windows)

Visual Studio הוא ה-IDE המלא של מיקרוסופט ל-Windows, והוא **חובה ליום 3** בגלל מעצב ה-WPF (XAML Designer). גרסת Community חינמית לשימוש אישי, לימודי ולעסקים קטנים.

1. הורידו את המתקין מ-https://visualstudio.microsoft.com/ — בחרו **Community** ואת הגרסה **העדכנית ביותר** הזמינה (2022 או חדשה יותר; המדריך לא מתחייב למספר גרסה מדויק).
2. הריצו את **Visual Studio Installer**. במסך "Workloads" סמנו:
   - **.NET desktop development** — חובה. כולל WPF, Windows Forms, קונסולה, ואת ה-.NET SDK.
   - **ASP.NET and web development** — אופציונלי. שימושי ליום 2 אם תרצו להרים Web API מקומי משלכם.
3. בלשונית **Individual components** ודאו שמסומן ".NET 10 SDK" (בדרך כלל מסומן אוטומטית עם ה-workload). אם ה-SDK של 10 לא מופיע במתקין — התקינו אותו בנפרד מסעיף 2; Visual Studio מזהה SDK שהותקן בנפרד.
4. לחצו Install. ההתקנה שוקלת כמה GB ולוקחת זמן — אל תשאירו אותה ליום הקורס.

איך לבחור workloads? ה-workload הוא "חבילת יכולות" — אוסף של רכיבים שמותקנים יחד. אפשר תמיד להוסיף או להסיר workloads אחר כך דרך Visual Studio Installer -> Modify. אין צורך להתקין הכול; שני ה-workloads למעלה מספיקים לקורס.

איפה נמצא מה שנצטרך ביום 3:
- **XAML Designer**: פתיחת קובץ `.xaml` בפרויקט WPF מציגה חלון מפוצל — תצוגה ויזואלית למעלה ו-XAML למטה (Split View). דרך View -> Toolbox גוררים פקדים.
- **XAML Hot Reload**: פועל אוטומטית כשמריצים עם Debug (F5) — שינוי ב-XAML מתעדכן בחלון הרץ בלי להפעיל מחדש. אם זה לא עובד, בדקו ב-Tools -> Options -> Debugging -> Hot Reload.
- **Live Visual Tree / Live Property Explorer**: Debug -> Windows — חקירת עץ הפקדים בזמן ריצה.

מי שעובד על macOS: Visual Studio for Mac הופסק, ואין תחליף ל-WPF Designer מחוץ ל-Windows. ליום 3 תצטרכו מכונת Windows (פיזית, VM כמו Parallels/VMware/VirtualBox, או Windows ב-Cloud). ימים 1, 2 ו-4 עובדים מצוין עם VS Code.

## 4. חלופה: Visual Studio Code

VS Code הוא עורך קל, חינמי וחוצה פלטפורמות (Windows / macOS / Linux). הוא **מספיק לימים 1, 2 ו-4** (פרויקטי קונסולה, בדיקות, HTTP, כלי AI). הוא **לא מספיק ליום 3** — אין בו מעצב WPF ויזואלי, ו-WPF עצמו רץ רק ב-Windows.

1. הורידו מ-https://code.visualstudio.com/
2. התקינו את התוספים (Extensions, קיצור `Ctrl+Shift+X`):
   - **C# Dev Kit** (של Microsoft) — חוויית פיתוח מלאה: Solution Explorer, הרצת בדיקות, תבניות פרויקט. מתקין אוטומטית גם את התוסף **C#** (שירות השפה, IntelliSense, debugging).
   - **REST Client** (של Huachao Mao) — ליום 2, שליחת בקשות HTTP מתוך קובץ `.http` (ראו סעיף 6).
3. פתחו תיקייה של פרויקט (File -> Open Folder), ולחצו F5 כדי להריץ עם debugger. בפעם הראשונה VS Code ישאל באיזה פרויקט להשתמש.

טיפ: גם מי שמתקין Visual Studio — כדאי להתקין VS Code בנוסף. ביום 4 נעבוד עם GitHub Copilot ו-Claude Code, ושניהם משתלבים היטב ב-VS Code.

## 5. Git ו-GitHub

1. **Git**: ב-Windows הורידו **Git for Windows** מ-https://git-scm.com/download/win והתקינו עם ברירות המחדל (כולל Git Bash). ב-macOS: `xcode-select --install` או דרך Homebrew (`brew install git`). ב-Linux: `sudo apt install git` (או המקבילה בהפצה שלכם).
2. **חשבון GitHub**: פתחו חשבון חינמי ב-https://github.com/ — נצטרך אותו גם ל-GitHub Copilot ביום 4.
3. הגדרה חד-פעמית של זהות:

<div dir="ltr">

```bash
git config --global user.name "השם שלכם"
git config --global user.email "you@example.com"
```

</div>

4. שכפול (clone) של מאגר הקורס — הקישור המדויק יינתן על ידי המרצה:

<div dir="ltr">

```bash
git clone <course-repo-url>
cd <course-repo-folder>
```

</div>

אימות: `git --version` מדפיס גרסה.

## 6. כלי HTTP ליום 2

ביום 2 נקרא ל-REST APIs ונעבוד עם JSON. צריך כלי לשליחת בקשות ובדיקת תשובות:

- **Postman** (https://www.postman.com/downloads/) — ממשק גרפי מלא. מספיק החשבון החינמי.
- **REST Client** (תוסף ל-VS Code) — כותבים בקשות בקובץ טקסט ולוחצים "Send Request". קל, מהיר, ונשמר ב-Git יחד עם הקוד. דוגמה לקובץ `requests.http`:

<div dir="ltr">

```text
GET https://api.github.com/repos/dotnet/runtime
Accept: application/json
```

</div>

- **curl** — כלי שורת פקודה. מגיע מובנה ב-Windows 10/11, macOS ו-Linux:

<div dir="ltr">

```bash
curl -s https://api.github.com/repos/dotnet/runtime
```

</div>

שימו לב: ב-PowerShell `curl` הוא לפעמים כינוי (alias) ל-`Invoke-WebRequest` עם תחביר שונה. הקלידו `curl.exe` כדי להריץ את ה-curl האמיתי.

- **Windows Terminal** (מומלץ) — טרמינל מודרני עם לשוניות, תמיכה טובה ב-UTF-8 ובעברית. מותקן כברירת מחדל ב-Windows 11; ב-Windows 10 מתקינים מ-Microsoft Store.

## 7. כלי AI ליום 4 — לפתוח חשבונות מראש

ביום 4 נעבוד "ידיים על המקלדת" עם עוזרי AI. פתיחת חשבונות ואימות אמצעי תשלום לוקחים זמן — עשו זאת **לפני** הקורס. חלק מהכלים בתשלום או עם תקופת ניסיון; **בדקו את התמחור העדכני באתר של כל כלי**, הוא משתנה.

| כלי | מה זה | איך מתקינים |
|-----|-------|-------------|
| **GitHub Copilot** | השלמת קוד וצ'אט בתוך ה-IDE | דורש חשבון GitHub עם מנוי Copilot (יש תוכנית חינמית מוגבלת ותוכניות בתשלום — בדקו ב-https://github.com/features/copilot). ב-Visual Studio: מובנה בגרסאות החדשות, מתחברים עם חשבון GitHub. ב-VS Code: תוסף "GitHub Copilot" + "GitHub Copilot Chat". |
| **Claude** | צ'אט עם המודל של Anthropic בדפדפן | חשבון ב-https://claude.ai |
| **Claude Code** | סוכן קוד בטרמינל שעובד על המאגר שלכם | דורש Node.js (סעיף 8). התקנה: `npm install -g @anthropic-ai/claude-code`, ואז `claude` בתיקיית הפרויקט. מידע עדכני: https://docs.anthropic.com/en/docs/claude-code |
| **ChatGPT** | צ'אט של OpenAI בדפדפן | חשבון ב-https://chatgpt.com |
| **Cursor** (אופציונלי) | עורך קוד מבוסס VS Code עם AI מובנה | https://cursor.com — יש תקופת ניסיון; לא חובה לקורס |

אימות: התחברו לכל אחד מהכלים פעם אחת ושלחו שאלה קצרה, כדי לוודא שהחשבון פעיל.

## 8. Node.js LTS

Node.js נדרש לשני דברים בקורס: התקנת **Claude Code** (מופץ דרך npm) ובניית **מצגות הקורס** (הן נבנות מקוד JavaScript, ראו `tools/slides/README.md`).

1. הורידו את גרסת ה-**LTS** מ-https://nodejs.org/ והתקינו עם ברירות המחדל.
2. פתחו טרמינל חדש ובדקו:

<div dir="ltr">

```bash
node --version
npm --version
```

</div>

3. התקינו את Claude Code:

<div dir="ltr">

```bash
npm install -g @anthropic-ai/claude-code
claude --version
```

</div>

## 9. אופציונלי אבל שימושי

### LINQPad
https://www.linqpad.net/ — "מחברת" ל-C#: כותבים ביטוי או קטע קוד ורואים תוצאה מיד, בלי ליצור פרויקט. נהדר לניסויים ב-LINQ (יום 1) וב-async (יום 2). גרסה חינמית זמינה (Windows בלבד).

### dotnet tools
כלים גלובליים שמותקנים דרך ה-CLI. שימושי ליום 2 (ניטור threads ו-GC):

<div dir="ltr">

```bash
dotnet tool install --global dotnet-counters
dotnet-counters --version
```

</div>

### .NET CLI — פקודות שכדאי להכיר

<div dir="ltr">

```bash
dotnet new list                       # רשימת התבניות הזמינות
dotnet new console -n MyApp           # פרויקט קונסולה חדש בתיקייה MyApp
dotnet new xunit -n MyApp.Tests       # פרויקט בדיקות
dotnet run                            # בנייה + הרצה של הפרויקט בתיקייה הנוכחית
dotnet run -- arg1 arg2               # העברת ארגומנטים לתוכנית
dotnet build                          # בנייה בלבד
dotnet test                           # הרצת בדיקות
dotnet add package Newtonsoft.Json    # הוספת חבילת NuGet
dotnet add reference ../Lib/Lib.csproj # הפניה לפרויקט אחר
dotnet format                         # עיצוב קוד לפי .editorconfig
dotnet --list-sdks                    # אילו SDKs מותקנים
dotnet --list-runtimes                # אילו Runtimes מותקנים
```

</div>

## 10. אימות ההתקנה

### תוכנית האימות
בתיקייה `00-Setup/VerifySetup` יש פרויקט קונסולה קטן שבודק את הסביבה. הריצו:

<div dir="ltr">

```bash
cd 00-Setup/VerifySetup
dotnet run
```

</div>

התוכנית מדפיסה את גרסת .NET, מערכת ההפעלה, האם יש SDK 10, האם יש גישה ל-NuGet, ומבצעת בדיקת JSON ו-async. בסוף מופיע סיכום עם ✅ / ⚠️ / ❌. סימן ⚠️ על Windows או על NuGet הוא אזהרה בלבד (למשל ב-macOS); ❌ פירושו שיש משהו לתקן.

### רשימת בדיקה

| כלי | פקודה לבדיקה | תוצאה צפויה |
|-----|--------------|-------------|
| .NET SDK | `dotnet --version` | `10.x.y` |
| SDKs מותקנים | `dotnet --list-sdks` | שורה שמתחילה ב-`10.` |
| יצירה והרצה | `dotnet new console -n Hello && cd Hello && dotnet run` | `Hello, World!` |
| Visual Studio | פתיחת VS -> Create a new project -> חיפוש "WPF Application" | התבנית מופיעה (Windows בלבד) |
| VS Code + C# Dev Kit | פתיחת תיקיית פרויקט, F5 | התוכנית רצה עם debugger |
| Git | `git --version` | `git version 2.x` |
| GitHub | התחברות ב-https://github.com | דף הבית של החשבון |
| curl | `curl.exe --version` (Windows) / `curl --version` | גרסה מודפסת |
| Postman / REST Client | שליחת `GET https://api.github.com` | תשובת JSON עם קוד 200 |
| Node.js | `node --version` | `v22.x` או LTS חדש יותר |
| npm | `npm --version` | מספר גרסה |
| Claude Code | `claude --version` | מספר גרסה |
| GitHub Copilot | פתיחת קובץ `.cs` ב-IDE | הצעות השלמה באפור |
| Claude / ChatGPT | התחברות בדפדפן | צ'אט פעיל |
| תוכנית האימות | `cd 00-Setup/VerifySetup && dotnet run` | "✅ הכול מוכן" (או ⚠️ בלבד) |

## 11. פתרון בעיות (Troubleshooting)

### `dotnet` לא מזוהה כפקודה ('dotnet' is not recognized)
- סגרו ופתחו מחדש את הטרמינל (ואת VS Code / Visual Studio) — משתני הסביבה נטענים רק בתהליכים חדשים.
- ב-Windows בדקו ש-`C:\Program Files\dotnet\` נמצא ב-PATH: Settings -> System -> About -> Advanced system settings -> Environment Variables.
- ב-macOS/Linux ודאו ש-`/usr/local/share/dotnet` (או `/usr/share/dotnet`, לפי ההתקנה) נמצא ב-PATH, למשל בקובץ `~/.zshrc` או `~/.bashrc`.

### יש כמה SDKs מותקנים והפרויקט משתמש בגרסה הלא נכונה
ה-CLI בוחר את ה-SDK **החדש ביותר** אלא אם יש קובץ `global.json` בתיקייה או באחת מתיקיות האב. בדקו עם `dotnet --version` בתוך תיקיית הפרויקט. כדי לנעול גרסה (למשל אם יש לכם גם 11 preview):

<div dir="ltr">

```json
{
  "sdk": {
    "version": "10.0.100",
    "rollForward": "latestFeature"
  }
}
```

</div>

יוצרים אוטומטית עם `dotnet new globaljson --sdk-version 10.0.100`. אם מופיעה שגיאה "The specified SDK version could not be found" — או שמוחקים את `global.json`, או שמתקינים את הגרסה שהוא דורש.

### NuGet restore נכשל מאחורי proxy ארגוני
תסמינים: `error NU1301: Unable to load the service index for source https://api.nuget.org/v3/index.json`.
- הגדירו את ה-proxy כמשתני סביבה: `HTTPS_PROXY=http://proxy.company.com:8080` (ואם צריך גם `HTTP_PROXY`). NuGet וה-CLI מכבדים אותם.
- לחלופין, בקובץ `NuGet.Config` (ב-Windows: `%AppData%\NuGet\NuGet.Config`) מוסיפים:

<div dir="ltr">

```xml
<configuration>
  <config>
    <add key="http_proxy" value="http://proxy.company.com:8080" />
  </config>
</configuration>
```

</div>

- ארגונים רבים מחליפים תעודות TLS (SSL inspection). אם מופיעה שגיאת "certificate" — בקשו מה-IT את תעודת ה-CA הארגונית והתקינו אותה במאגר התעודות של המערכת.
- אם יש לכם NuGet פנימי (Artifactory / Azure Artifacts) — בקשו מה-IT את הכתובת והוסיפו: `dotnet nuget add source <url> -n corp`.

### `EnableWindowsTargeting` — בניית פרויקט WPF שלא על Windows
פרויקט עם `<TargetFramework>net10.0-windows</TargetFramework>` נכשל ב-macOS/Linux עם NETSDK1100. הוספת `<EnableWindowsTargeting>true</EnableWindowsTargeting>` ל-`.csproj` מאפשרת **לבנות** (compile) אותו גם שם — אבל לא להריץ; WPF רץ רק ב-Windows. פרויקטי ה-WPF בקורס כבר כוללים את ההגדרה, כך שאפשר לפחות לקרוא ולקמפל אותם בכל מערכת.

### HTTPS dev certificate — אזהרות אבטחה כשמריצים Web API מקומי
אם ביום 2 תרימו Web API מקומי (`dotnet new webapi`) והדפדפן/הלקוח מתלונן על תעודה לא מהימנה:

<div dir="ltr">

```bash
dotnet dev-certs https --trust
```

</div>

ב-Windows ו-macOS תופיע בקשת אישור. ב-Linux הפקודה מייצרת את התעודה אבל לא תמיד מוסיפה אותה למאגר המהימן — ראו את ההוראות בפלט. לחלופין, עבדו מול הכתובת `http://` שמופיעה ב-`launchSettings.json`.

### PowerShell: "running scripts is disabled on this system"
קורה כשמריצים סקריפט `.ps1` (למשל סקריפט התקנה או `dotnet-install.ps1`). פתרון למשתמש הנוכחי בלבד (לא דורש מנהל):

<div dir="ltr">

```text
Set-ExecutionPolicy -Scope CurrentUser RemoteSigned
```

</div>

לחלופין הריצו את הסקריפט הבודד עם `powershell -ExecutionPolicy Bypass -File script.ps1`.

### עברית או אמוג'י מופיעים כסימני שאלה בקונסולה
השתמשו ב-Windows Terminal (ולא ב-cmd הישן), ובחרו גופן שתומך ב-Unicode (למשל Cascadia Code). בקוד: `Console.OutputEncoding = Encoding.UTF8;` (תוכנית האימות כבר עושה זאת).

### Visual Studio לא מציע את התבנית "WPF Application"
ה-workload ".NET desktop development" לא מותקן. פתחו Visual Studio Installer -> Modify -> סמנו את ה-workload -> Modify.

### עדיין תקועים?
- הריצו `dotnet --info` ושלחו את הפלט למרצה (בקבוצת הקורס או במייל) יחד עם הודעת השגיאה המלאה.
- הגיעו 20 דקות מוקדם ביום הראשון — נקדיש את הזמן לפתרון בעיות התקנה.

</div>
