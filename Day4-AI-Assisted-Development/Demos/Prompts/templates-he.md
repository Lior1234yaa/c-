# תבניות Prompt למפתחי C# / .NET (עברית)

החליפו את `{...}`. תמיד הדביקו את הקוד/הממשקים הרלוונטיים אחרי הבקשה. אפשר לכתוב את הבקשה בעברית — אבל שמות טיפוסים, מתודות וטכנולוגיות תמיד באנגלית, וה**קוד** שמתקבל יהיה באנגלית.

## 1. Records/DTOs מ-JSON
```text
צור C# records עבור ה-JSON שלמטה. יעד: .NET 10, System.Text.Json, nullable enabled.
שמות PascalCase עם [JsonPropertyName] רק כשהשם שונה. decimal לכסף, DateTimeOffset לזמנים,
enum לערכי מחרוזת קבועים (עם JsonStringEnumConverter). פלט: קוד בלבד.
{הדביקו JSON}
```

## 2. LINQ ממשפט
```text
כתוב שאילתת LINQ (method syntax) על {תיאור האוסף} שמחזירה {התוצאה}.
הסבר סיבוכיות בשורה אחת וציין הנחות (כפילויות, null).
```

## 3. ViewModel ל-MVVM
```text
המר את המחלקה ל-ViewModel של WPF: ירושה מ-ObservableObject (INotifyPropertyChanged + SetProperty),
פקודות כ-ICommand דרך RelayCommand. בלי ספריות MVVM חיצוניות. בלי לוגיקה ב-code-behind.
{הדביקו את המחלקה}
```

## 4. בדיקות יחידה (xUnit)
```text
כתוב בדיקות xUnit למחלקה שלמטה: מסלול תקין, ערכי גבול, קלט null/ריק, מקרה חריגה אחד.
השתמש ב-[Theory]/[InlineData] כשמתאים. שמות: Method_Scenario_Expected. אל תשנה את המחלקה.
{הדביקו את המחלקה}
```

## 5. בדיקות קודם (TDD)
```text
אל תממש עדיין. כתוב בדיקות xUnit עבור {ממשק/מתודה} לפי המפרט:
{נקודות המפרט}
המתן לאישור שלי לפני מימוש.
```

## 6. Refactoring עם שמירת התנהגות
```text
בצע refactoring למתודה: חלץ מתודות פרטיות עם שמות ברורים, החלף מספרי קסם בקבועים עם שם,
הסר כפילויות. ההתנהגות והחתימה הציבורית חייבות להישאר זהות. הצג את המחלקה המלאה ואז סיכום ב-3 שורות.
{הדביקו קוד}
```

## 7. הסבר קוד
```text
הסבר את הקוד למפתח מתחיל: מה הוא עושה, שורה-שורה במקומות לא ברורים.
אחר כך רשום את ההנחות שהוא מניח על הקלט ומה עלול להיכשל בפרודקשן.
{הדביקו קוד}
```

## 8. מצא את הבאג
```text
בקוד יש באג שקשור ל-{async | culture | disposal | thread-safety | null | off-by-one}.
התסמין: {תיאור}. מצא את הסיבה הסבירה ביותר, הסבר, והצע תיקון מינימלי. אל תכתוב הכול מחדש.
{הדביקו קוד + בדיקה נכשלת/שגיאה}
```

## 9. Regex
```text
כתוב regex ל-.NET עבור {תיאור}. עגן אותו (^...$). תן 5 דוגמאות תואמות ו-5 לא תואמות,
ובדיקת xUnit [Theory] שמאמתת אותן. השתמש ב-[GeneratedRegex] אם מתאים.
```

## 10. מיגרציה
```text
העבר את הקוד מ-{Newtonsoft.Json} ל-{System.Text.Json}. ה-JSON שנוצר חייב להישאר זהה.
רשום כל הבדל התנהגותי שידוע לך (אותיות, null, converters, dictionaries) ואיך טיפלת בו.
{הדביקו קוד}
```

## 11. תיעוד XML
```text
הוסף XML documentation לחברים ציבוריים: משפט ברור אחד ל-summary, <param>/<returns> כשמועיל,
<exception> לחריגות. אל תתעד getters מובנים מאליהם. אל תשנה קוד.
{הדביקו קוד}
```

## 12. הודעת commit / תיאור PR
```text
כתוב הודעת commit בפורמט conventional commits עבור ה-diff. נושא עד 72 תווים, לשון ציווי (באנגלית).
גוף: מה ולמה (לא איך), בדיקות שנוספו, breaking changes אם יש.
{הדביקו git diff}
```

## 13. סקירת קוד קפדנית (סיבוב שני)
```text
סקור את קוד ה-C# כ-senior reviewer קפדן. בדוק: טיפול ב-null, נכונות async (async void, .Result),
parsing תלוי-culture, שימוש ב-IDisposable/HttpClient, thread safety, בליעת חריגות, injection/path traversal,
תלויות מיותרות. לכל ממצא: חומרה, מיקום, למה, תיקון מינימלי. אל תכתוב את הקובץ מחדש.
{הדביקו קוד}
```

## 14. חלון WPF (XAML)
```text
צור {WindowName}.xaml בלבד, בלי לוגיקה ב-code-behind. .NET 10 WPF, MVVM.
DataContext: {שם ViewModel} עם החברים: {רשימת properties/commands עם טיפוסים}.
פריסה: {שורות/עמודות, אזורים}. FlowDirection="RightToLeft", תוויות בעברית.
צבעים/סטיילים דרך StaticResource שמוגדרים ב-Window.Resources. רק controls סטנדרטיים של WPF.
כלול מצבי loading (IsBusy), empty ו-error. הוסף AutomationProperties.Name לשדות ולכפתורים.
```
