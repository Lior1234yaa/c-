<div dir="rtl">

# Lab 1 — Unit Converter: האפליקציה הראשונה ב-WPF

**משך:** 45 דקות | **רמה:** ★ | **מודולים:** 01–03 (XAML, פריסה, אירועים)

## המטרה

לבנות ממיר יחידות אורך: המשתמש מקליד ערך, בוחר יחידת מקור ויחידת יעד, ורואה את התוצאה —
גם בלחיצה על Convert וגם "חי" בזמן ההקלדה. קלט לא תקין מוצג כהודעת שגיאה אדומה במקום קריסה.

<div dir="ltr">

```text
+----------------------------------------------+
| Value:  [ 10            ]  [⇄]               |
| From:   [ Meter (m)          v ]             |
| To:     [ Foot (ft)          v ]             |
|         (הודעת שגיאה אם יש)                  |
|                                              |
|            10 m = 32.8084 ft                 |
|                                              |
|                       [ Convert ] [ Clear ]  |
+----------------------------------------------+
```

</div>

## דרישות מוקדמות

- Windows + Visual Studio 2022 עם workload ‏"‎.NET desktop development" (ראו `../../../00-Setup/INSTALL.md`).
- פותחים את `Starter/Day3.Lab1.Starter.csproj` (או `dotnet run` מתוך התיקייה).

## שלבים

### שלב 1 — הלוגיקה (`UnitConverter.cs`)

1. **TODO 1:** ממשו `Convert` — כל היחידות מוגדרות ביחס למטר (`MetersPerUnit`), לכן:
   `value * from.MetersPerUnit / to.MetersPerUnit`.
2. **TODO 2:** ממשו `TryParseInput` — מחזיר `null` אם הקלט תקין, אחרת מחרוזת שגיאה בעברית.
   השתמשו ב-`double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out value)`.
   קלט ריק, לא-מספר ומספר שלילי — כולם שגיאות.

### שלב 2 — הפריסה (`MainWindow.xaml`)

3. **TODO 3:** החליפו את ה-Grid הזמני בפריסה מלאה: 3 עמודות (`Auto`, `*`, `Auto`) ו-6 שורות
   (חמש `Auto` ואחת `*` לתוצאה). הקפידו על `x:Name` זהים לאלה שבקוד: `ValueBox`, `FromUnit`, `ToUnit`,
   `ErrorText`, `ResultText`, `ConvertButton`.
   שימו לב ש-`Grid.ColumnSpan` מאפשר ל-ComboBox להתפרש על שתי עמודות.

### שלב 3 — חיבור הקוד (`MainWindow.xaml.cs`)

4. **TODO 4:** בבנאי, אחרי `InitializeComponent()`, מלאו את שני ה-ComboBox-ים:
   `FromUnit.ItemsSource = LengthUnit.All;` ובחרו ברירת מחדל עם `SelectedIndex`.
5. **TODO 5:** הוסיפו ב-XAML את `TextChanged="ValueBox_TextChanged"` ו-`SelectionChanged="Unit_SelectionChanged"`,
   וכתבו את שני ה-handlers כך שיקראו ל-`Convert()`. כך ההמרה מתעדכנת בזמן אמת.
6. **TODO 6:** ממשו את `Convert()`: ולידציה → אם יש שגיאה `ShowError(error)` → אחרת חישוב והצגה
   בפורמט `10 m = 32.8084 ft`. שדה ריק לא צריך להציג שגיאה (רק לנקות את התוצאה).

### שלב 4 — מקלדת ובונוס

7. **TODO 7:** ‏Enter כבר מפעיל את Convert כי הכפתור מסומן `IsDefault="True"`. הוסיפו `KeyDown` על
   ה-TextBox כך ש-Escape ינקה. בונוס: כפתור `⇄` שמחליף בין יחידת המקור ליעד (רמז: tuple swap).

## קריטריוני קבלה

- [ ] הפרויקט נבנה ללא אזהרות ורץ.
- [ ] הקלדת `10` עם Meter→Foot מציגה `10 m = 32.8084 ft` בלי ללחוץ על כפתור.
- [ ] הקלדת `abc` מציגה הודעת שגיאה אדומה, לא קורסת, וכפתור Convert מושבת.
- [ ] שינוי יחידה ב-ComboBox מעדכן את התוצאה מיד.
- [ ] Enter מפעיל המרה, Escape מנקה.
- [ ] החלון נראה תקין גם כשמרחיבים אותו (התוצאה ממורכזת, ה-TextBox נמתח).

## בונוס

- הוסיפו קטגוריה שנייה (משקל: g, kg, lb, oz) עם ComboBox לבחירת קטגוריה.
- הציגו את התוצאה ב-`Clipboard` בלחיצה כפולה (`Clipboard.SetText`).
- כתבו פרויקט xUnit שבודק את `UnitConverter.Convert` ו-`TryParseInput` — שימו לב שזה אפשרי רק כי
  הלוגיקה לא נוגעת ב-UI.

## רמזים

- `IsLoaded` על החלון עוזר להימנע מ-`SelectionChanged` שנורה לפני שהפקדים מוכנים.
- `Visibility.Collapsed` (לא `Hidden`) כדי שהשורה לא תתפוס מקום.
- `ResultText` עם `TextWrapping="Wrap"` כדי שמספרים ארוכים לא ייחתכו.

</div>
