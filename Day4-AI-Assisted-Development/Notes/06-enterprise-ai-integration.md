# מודול 6 — שילוב AI בסביבת פיתוח ארגונית

## המעבר מ"אני" ל"אנחנו"

עד עכשיו דיברנו על מפתח אחד וכלי אחד. בארגון יש שאלות אחרות: מה מותר לשלוח לכלי? מי אחראי על קוד שנוצר? איך מודדים אם זה עוזר? איך מונעים דליפת סודות? במודול הזה נבנה **מדיניות**, **תהליך** ו**תוכנית הטמעה** — בלי להמציא מספרים; מה שמשתנה בין ספקים, בדקו בתיעוד ובחוזה.

## 1. פרטיות נתונים וקניין רוחני

שאלת המפתח: **מה קורה לטקסט שאני שולח?** התשובה שונה בין תוכנית אישית (Free/Pro) לתוכנית ארגונית (Business/Enterprise) ובין ספקים. עקרונות שנכונים לכל הכלים:

- **קראו את תנאי הפרטיות של התוכנית שאתם בפועל משתמשים בה** — במיוחד: האם הנתונים משמשים לאימון? כמה זמן נשמרים? היכן (אזור גאוגרפי)?
- תוכניות ארגוניות של הספקים הגדולים מציעות בדרך כלל התחייבויות חזקות יותר (אי-שימוש לאימון, שמירת נתונים מוגבלת, ניהול מרכזי, SSO, audit). בדקו את המסמכים העדכניים:
  - GitHub Copilot trust center: https://copilot.github.trust.page
  - Anthropic privacy & trust: https://trust.anthropic.com
  - OpenAI enterprise privacy: https://openai.com/enterprise-privacy
- **אפשרויות on-prem / self-hosted / cloud פרטי קיימות** (למשל מודלים דרך Azure OpenAI / Amazon Bedrock / Google Vertex בתוך ה-tenant שלכם, או מודלים פתוחים שרצים מקומית) — לארגונים עם דרישות רגולציה. זה trade-off של איכות/עלות/תפעול.

מדיניות מינימלית לצוות (דוגמה):

| סוג מידע | כלי בתוכנית אישית | כלי בתוכנית ארגונית מאושרת |
|----------|--------------------|-----------------------------|
| קוד פתוח / קוד תרגול | מותר | מותר |
| קוד קנייני של החברה | **אסור** | מותר |
| נתוני לקוחות / PII | אסור | רק אם הוסכם חוזית ומאושר ע"י DPO |
| סודות (מפתחות, סיסמאות, connection strings) | **אסור לעולם** | **אסור לעולם** |

## 2. סודות לעולם לא ב-prompt

- לפני הדבקת קוד: חפשו `password`, `apikey`, `token`, `connectionstring`. השתמשו ב-placeholders: `"<REDACTED>"`.
- סודות בקוד הם בעיה גם בלי AI: `dotnet user-secrets` בפיתוח, משתני סביבה / Key Vault בפרודקשן.
- כלי scanning: GitHub secret scanning, gitleaks, וכלל ב-`copilot-instructions.md`: "Never hardcode secrets; read from configuration."
- סוכנים (agent mode) קוראים קבצים — ודאו ש-`.env`/`appsettings.Production.json` לא בריפו וש-`.gitignore` תקין.

## 3. רישוי וקניין רוחני של הפלט

- פלט AI עשוי להיות דומה לקוד קיים. Copilot מציע סינון "הצעות שתואמות קוד ציבורי" ו-Enterprise מציע גם הגנה משפטית (indemnification) — פרטים בתיעוד העדכני.
- מדיניות: קוד שנוצר עובר את אותם כלי בדיקת רישוי (SCA) כמו כל תלות.
- הבהירו בחוזה/מדיניות: מי הבעלים של הפלט (בדרך כלל הלקוח, לפי תנאי הספק — בדקו).

## 4. Compliance ו-Audit

- Enterprise plans מספקים לרוב: ניהול משתמשים מרכזי, SSO, לוגים של שימוש, ומדיניות ארגונית (אילו features מותרים).
- לצורכי רגולציה (ISO 27001, SOC 2, GDPR, תקנות מקומיות): תעדו **אילו כלים מאושרים, למי, לאילו נתונים**. שמרו את ההחלטה כ-ADR/מסמך מדיניות.
- MCP וסוכנים: כל שרת MCP הוא ערוץ נוסף. אשרו שרתים ברשימה לבנה.

## 5. בעלות על קוד ושערי סקירה (Review gates)

עיקרון: **אין הבדל בין קוד AI לקוד אנושי מבחינת אחריות**. מי שעשה commit — אחראי.

- **כל קוד עובר PR** — גם אם סוכן כתב אותו. סוכן לא עושה merge.
- **CI חובה**: build, tests, `dotnet format --verify-no-changes`, analyzers, secret scanning, SCA.
- **Reviewer אנושי אחד לפחות**; AI reviewer כתוספת (Copilot code review / Claude).
- שקיפות: מקובל לציין ב-PR ש-AI סייע ("Generated with Copilot; reviewed and tested by X").
- **Branch protection** ב-GitHub/Azure DevOps כדי שהכללים לא יהיו המלצה.

## 6. CI/CD עם בדיקות AI

דוגמת workflow (GitHub Actions) — השלד הרגיל, וה-AI מצטרף כשלב סקירה:

```yaml
name: ci
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
      - run: dotnet test --no-build
```

סקירת AI על PR מופעלת בדרך כלל דרך הגדרות הריפו (Copilot) או אפליקציית GitHub (Claude) — לא צריך להמציא step; בדקו את התיעוד של הכלי. חשוב: תוצאת סקירת AI היא **הערה**, לא **check חוסם**, אלא אם החלטתם אחרת במודע.

## 7. מדידת השפעה

אל תמדדו "כמה שורות AI כתב". מדדו את מה שהיה חשוב גם קודם (DORA-style):

| מדד | שאלה |
|-----|------|
| Lead time | כמה זמן מ-issue ל-merge? |
| Deployment frequency | כמה פעמים משחררים? |
| Change failure rate | כמה שינויים גרמו לתקלה? |
| Defects per release | האם איכות נפגעה? |
| Review time | האם PRs נתקעים כי הם גדולים מדי? |
| שביעות רצון מפתחים | סקר קצר רבעוני |

הגדירו baseline לפני ההטמעה, ובדקו אחרי 3 חודשים. היזהרו מ"יותר קוד = יותר ערך".

## 8. הכשרת הצוות

- **סדנה קצרה** (כמו היום): prompt, review, כללים.
- **קובצי הוראות משותפים** בכל ריפו (`copilot-instructions.md` / `CLAUDE.md`) — מתחזקים אותם כמו קוד.
- **ספריית prompts צוותית** (`docs/prompts/`) עם תבניות שעובדות.
- **"AI champion"** בכל צוות: אוסף טיפים, מעדכן מדיניות.
- **Pairing**: מפתח ותיק + חדש עוברים יחד על פלט AI.

## 9. Playbook להטמעה (תוכנית 90 יום — דוגמה)

| שלב | שבועות | מה עושים |
|-----|--------|----------|
| 0. מדיניות | 1–2 | בחירת כלי ותוכנית, מדיניות נתונים, אישור משפטי/אבטחה, הגדרת admin |
| 1. פיילוט | 3–6 | צוות אחד, ריפו אחד, קובצי הוראות, CI מחמיר, מדידת baseline |
| 2. הרחבה | 7–10 | עוד צוותים, ספריית prompts, סדנאות, AI review ב-PR |
| 3. ייצוב | 11–13 | סקירת מדדים, עדכון מדיניות, החלטה על המשך/הרחבה |

## 10. סיכונים ומיתונים

| סיכון | מיתון |
|-------|-------|
| דליפת סודות/נתונים | מדיניות + secret scanning + תוכנית ארגונית + הדרכה |
| קוד לא בטוח (injection, path traversal) | analyzers, SAST, צ'ק-ליסט סקירה, בדיקות |
| APIs/חבילות מומצאים | build ב-CI, `dotnet add package` ידני, בדיקה ב-nuget.org |
| הפרות רישוי | סינון קוד ציבורי, SCA, מדיניות |
| ירידה במיומנות ("skill atrophy") | דורשים הסבר ב-PR, pairing, ימי "ללא AI" למתחילים |
| PRs ענקיים שלא נסקרים | כלל גודל PR, פיצול משימות, סוכנים עם משימות קטנות |
| תלות בספק | הפשטה מינימלית (prompts בקבצים, לא בכלי), מעקב אחר חלופות |
| עלות | תקציב לפי מושבים, מעקב שימוש דרך admin console |

## טעויות נפוצות

- להתחיל מכלי ולא ממדיניות. - לאסור לחלוטין (המפתחים ישתמשו בחשבון אישי בסתר — גרוע יותר). - לוותר על CI "כי יש AI review". - למדוד שורות קוד. - לשכוח לעדכן את קובצי ההוראות כשהארכיטקטורה משתנה.

## לסיכום

- מדיניות נתונים לפי תוכנית וסוג מידע; סודות לעולם לא ב-prompt; on-prem קיים כאופציה.
- אחריות על קוד AI זהה לקוד אנושי: PR + CI + reviewer אנושי.
- מדדו lead time ו-defects, לא שורות; הכשירו את הצוות; הטמיעו בשלבים.
- טבלת סיכונים/מיתונים היא נקודת פתיחה למדיניות שלכם.

## קריאה נוספת

- GitHub Copilot for business/enterprise docs: https://docs.github.com/copilot
- Anthropic Trust Center: https://trust.anthropic.com
- Secret management in .NET: https://learn.microsoft.com/aspnet/core/security/app-secrets
- GitHub secret scanning: https://docs.github.com/code-security/secret-scanning/introduction/about-secret-scanning
- DORA metrics: https://dora.dev
- GitHub Actions for .NET: https://learn.microsoft.com/dotnet/devops/github-actions-overview
