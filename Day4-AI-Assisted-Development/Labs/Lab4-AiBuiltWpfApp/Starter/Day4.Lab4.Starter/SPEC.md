<div dir="rtl">

# Expense Tracker — מפרט (עמוד אחד)

## מטרה
אפליקציית WPF לניהול הוצאות אישיות. משתמש יחיד, מקומי, קובץ JSON.

## מודל
`Expense(Guid Id, DateOnly Date, string Category, decimal Amount, string? Note)`.
קטגוריות קבועות: מזון, תחבורה, דיור, בילויים, בריאות, אחר.

## מסך ראשי (חלון יחיד, MinWidth 900, MinHeight 600, RTL, עברית)
1. **טופס הוספה** (למעלה): תאריך (DatePicker, ברירת מחדל היום), קטגוריה (ComboBox), סכום (TextBox, LTR), הערה (TextBox), כפתור "הוסף". שגיאות validation מוצגות מתחת לשדה; "הוסף" מושבת כשיש שגיאות.
2. **סינון**: חודש (ComboBox של החודשים הקיימים + "הכול"), קטגוריה ("הכול" + הרשימה).
3. **טבלה**: תאריך, קטגוריה, סכום (מעוצב `N2` ₪, LTR), הערה. בחירת שורה + כפתור "מחק" (מושבת כשאין בחירה). מצב ריק: "אין הוצאות להצגה".
4. **סיכום** (פאנל צד): סה"כ מסונן, ממוצע ליום בחודש הנבחר, פירוט לפי קטגוריה (שם + סכום + אחוז).
5. **שורת סטטוס**: הודעה אחרונה, ProgressBar ל-IsBusy, כפתור "שמור". טעינה אוטומטית בהפעלה; שמירה אוטומטית אחרי הוספה/מחיקה (וגם ידנית).

## Validation (INotifyDataErrorInfo)
- סכום: מספר > 0, עד 2 ספרות אחרי הנקודה, parsing עם InvariantCulture.
- קטגוריה: חובה מהרשימה.
- תאריך: לא בעתיד (ביחס ל-`TimeProvider` מוזרק).
- הערה: עד 100 תווים.

## אחסון
`IExpenseRepository { Task<IReadOnlyList<Expense>> LoadAsync(ct); Task SaveAsync(IReadOnlyList<Expense>, ct); }`
מימוש JSON ב-`%LOCALAPPDATA%\Day4Lab4\expenses.json`: יוצר תיקייה, כתיבה אטומית (temp + move), קובץ חסר = רשימה ריקה, קובץ פגום = חריגה עם הודעה ברורה (ה-UI מציג ErrorMessage, לא קורס).

## ארכיטקטורה
MVVM ללא ספריות חיצוניות (Mvvm.cs מסופק). אין לוגיקה ב-code-behind. סטיילים מ-Themes/Colors.xaml. פורמט כסף/תאריך ב-UI לפי culture נוכחית; קובץ JSON תמיד ISO/Invariant.

## מחוץ לטווח
ריבוי משתמשים, סנכרון ענן, גרפים.

</div>
