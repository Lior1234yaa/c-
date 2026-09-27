<div dir="rtl">

# תבניות Prompt למפתחי C# / .NET (עברית)

החליפו את `{...}`. תמיד הדביקו את הקוד/הממשקים הרלוונטיים אחרי הבקשה. אפשר לכתוב את הבקשה בעברית — אבל שמות טיפוסים, מתודות וטכנולוגיות תמיד באנגלית, וה**קוד** שמתקבל יהיה באנגלית.

## 1. Records/DTOs מ-JSON
<div dir="ltr">

```text
צור C# records עבור ה-JSON שלמטה. יעד: .NET 10, System.Text.Json, nullable enabled.
שמות PascalCase עם [JsonPropertyName] רק כשהשם שונה. decimal לכסף, DateTimeOffset לזמנים,
enum לערכי מחרוזת קבועים (עם JsonStringEnumConverter). פלט: קוד בלבד.
{הדביקו JSON}
```

</div>

## 2. LINQ ממשפט
<div dir="ltr">

```text
כתוב שאילתת LINQ (method syntax) על {תיאור האוסף} שמחזירה {התוצאה}.
הסבר סיבוכיות בשורה אחת וציין הנחות (כפילויות, null).
```

</div>

## 3. ViewModel ל-MVVM
<div dir="ltr">

```text
המר את המחלקה ל-ViewModel של WPF: ירושה מ-ObservableObject (INotifyPropertyChanged + SetProperty),
פקודות כ-ICommand דרך RelayCommand. בלי ספריות MVVM חיצוניות. בלי לוגיקה ב-code-behind.
{הדביקו את המחלקה}
```

</div>

## 4. בדיקות יחידה (xUnit)
<div dir="ltr">

```text
כתוב בדיקות xUnit למחלקה שלמטה: מסלול תקין, ערכי גבול, קלט null/ריק, מקרה חריגה אחד.
השתמש ב-[Theory]/[InlineData] כשמתאים. שמות: Method_Scenario_Expected. אל תשנה את המחלקה.
{הדביקו את המחלקה}
```

</div>

## 5. בדיקות קודם (TDD)
<div dir="ltr">

```text
אל תממש עדיין. כתוב בדיקות xUnit עבור {ממשק/מתודה} לפי המפרט:
{נקודות המפרט}
המתן לאישור שלי לפני מימוש.
```

</div>

## 6. Refactoring עם שמירת התנהגות
<div dir="ltr">

```text
בצע refactoring למתודה: חלץ מתודות פרטיות עם שמות ברורים, החלף מספרי קסם בקבועים עם שם,
הסר כפילויות. ההתנהגות והחתימה הציבורית חייבות להישאר זהות. הצג את המחלקה המלאה ואז סיכום ב-3 שורות.
{הדביקו קוד}
```

</div>

## 7. הסבר קוד
<div dir="ltr">

```text
הסבר את הקוד למפתח מתחיל: מה הוא עושה, שורה-שורה במקומות לא ברורים.
אחר כך רשום את ההנחות שהוא מניח על הקלט ומה עלול להיכשל בפרודקשן.
{הדביקו קוד}
```

</div>

## 8. מצא את הבאג
<div dir="ltr">

```text
בקוד יש באג שקשור ל-{async | culture | disposal | thread-safety | null | off-by-one}.
התסמין: {תיאור}. מצא את הסיבה הסבירה ביותר, הסבר, והצע תיקון מינימלי. אל תכתוב הכול מחדש.
{הדביקו קוד + בדיקה נכשלת/שגיאה}
```

</div>

## 9. Regex
<div dir="ltr">

```text
כתוב regex ל-.NET עבור {תיאור}. עגן אותו (^...$). תן 5 דוגמאות תואמות ו-5 לא תואמות,
ובדיקת xUnit [Theory] שמאמתת אותן. השתמש ב-[GeneratedRegex] אם מתאים.
```

</div>

## 10. מיגרציה
<div dir="ltr">

```text
העבר את הקוד מ-{Newtonsoft.Json} ל-{System.Text.Json}. ה-JSON שנוצר חייב להישאר זהה.
רשום כל הבדל התנהגותי שידוע לך (אותיות, null, converters, dictionaries) ואיך טיפלת בו.
{הדביקו קוד}
```

</div>

## 11. תיעוד XML
<div dir="ltr">

```text
הוסף XML documentation לחברים ציבוריים: משפט ברור אחד ל-summary, <param>/<returns> כשמועיל,
<exception> לחריגות. אל תתעד getters מובנים מאליהם. אל תשנה קוד.
{הדביקו קוד}
```

</div>

## 12. הודעת commit / תיאור PR
<div dir="ltr">

```text
כתוב הודעת commit בפורמט conventional commits עבור ה-diff. נושא עד 72 תווים, לשון ציווי (באנגלית).
גוף: מה ולמה (לא איך), בדיקות שנוספו, breaking changes אם יש.
{הדביקו git diff}
```

</div>

## 13. סקירת קוד קפדנית (סיבוב שני)
<div dir="ltr">

```text
סקור את קוד ה-C# כ-senior reviewer קפדן. בדוק: טיפול ב-null, נכונות async (async void, .Result),
parsing תלוי-culture, שימוש ב-IDisposable/HttpClient, thread safety, בליעת חריגות, injection/path traversal,
תלויות מיותרות. לכל ממצא: חומרה, מיקום, למה, תיקון מינימלי. אל תכתוב את הקובץ מחדש.
{הדביקו קוד}
```

</div>

## 14. חלון WPF (XAML)
<div dir="ltr">

```text
צור {WindowName}.xaml בלבד, בלי לוגיקה ב-code-behind. .NET 10 WPF, MVVM.
DataContext: {שם ViewModel} עם החברים: {רשימת properties/commands עם טיפוסים}.
פריסה: {שורות/עמודות, אזורים}. FlowDirection="RightToLeft", תוויות בעברית.
צבעים/סטיילים דרך StaticResource שמוגדרים ב-Window.Resources. רק controls סטנדרטיים של WPF.
כלול מצבי loading (IsBusy), empty ו-error. הוסף AutomationProperties.Name לשדות ולכפתורים.
```

</div>

</div>
