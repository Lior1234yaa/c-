<div dir="rtl">

# תרגילים — יום 3: פיתוח GUI מהיר עם WPF

12 תרגילים קצרים (5–15 דקות כל אחד), מקובצים לפי מודול. הפתרונות נמצאים בפרויקט
`Solutions/Day3.Exercises.Solutions` — חלון תפריט ראשי שממנו פותחים כל תרגיל.
לכל תרגיל צרו `Window` חדש בפרויקט WPF משלכם (`dotnet new wpf -n Day3.Exercises`), או הוסיפו חלון לפרויקט קיים.

רמת קושי: ★ קל | ★★ בינוני | ★★★ מאתגר

הרצת הפתרונות (Windows בלבד):

<div dir="ltr">

```bash
cd Exercises/Solutions
dotnet run
```

</div>

## מודול 01–02: XAML, פריסה ופקדים

### תרגיל 1 — Hello XAML ★
חלון עם `TextBox`, כפתור `Greet` ו-`TextBlock`. בלחיצה (או Enter — ‏`IsDefault`) מוצג `Hello, <שם>!`
עם מונה לחיצות. השתמשו ב-`x:Name` כדי לגשת לפקדים מה-code-behind.
**בדקו:** שדה ריק מציג `Hello, World!`.

### תרגיל 2 — Grid keypad ★
בנו לוח מקשים 4×4 עם `Grid` ו-star sizing: תצוגה בשורה העליונה (`ColumnSpan="4"`), כפתור `0` על שתי
עמודות, כפתור `OK` על שתי שורות (`RowSpan`). כל הכפתורים מטופלים ב-handler **אחד** — רמז:
`Button.Click="Key_Click"` על ה-Grid (bubbling) ו-`e.OriginalSource`.
**בדקו:** הגדלת החלון מגדילה את הכפתורים באופן אחיד.

### תרגיל 3 — Login form ★
טופס התחברות עם `DockPanel` (כותרת למעלה, כפתורים למטה) ו-`StackPanel` לשדות. השתמשו ב-`Label`
עם `Target` כדי ש-Alt+U יקפיץ לשדה המשתמש, `PasswordBox` לסיסמה, `IsDefault`/`IsCancel` לכפתורים,
ו-`TabIndex` לסדר מעבר הגיוני.

## מודול 03: אירועים ופקודות

### תרגיל 4 — Counter events ★★
מונה עם כפתורי `+`/`−`. הוסיפו: `KeyDown` על החלון (חיצים למעלה/למטה), `Loaded` שמציג רמז,
ו-`Closing` שמבקש אישור אם המונה שונה מאפס (`e.Cancel = true` מבטל את הסגירה).

### תרגיל 5 — RelayCommand + InputBindings ★★
אותו מונה, הפעם עם `ICommand`: כתבו `RelayCommand` (ראו Notes/03), ‏`CounterViewModel` עם
`IncrementCommand`/`DecrementCommand`/`ResetCommand`, ו-`CanExecute` שמגביל ל-0–10.
הוסיפו `Window.InputBindings` ל-Ctrl+↑ / Ctrl+↓ / Ctrl+R.
**בדקו:** הכפתורים מושבתים אוטומטית בגבולות — בלי שום `IsEnabled` בקוד.

### תרגיל 6 — Stopwatch ★★
שעון עצר עם `Start/Stop`, `Lap`, `Reset`. השתמשו ב-`System.Diagnostics.Stopwatch` למדידה מדויקת
וב-`DispatcherTimer` (100ms) רק לרענון התצוגה. פורמט: `mm:ss.f`.
**שאלה:** למה לא למדוד זמן לפי מספר ה-Tick-ים?

## מודול 04: Data binding

### תרגיל 7 — ElementName binding ★
אפס קוד C#: `Slider` שמשנה את גודל `Rectangle` (‏`Width`/`Height` קשורים ל-`Value`), ו-`TextBox`
שה-`TextBlock` מתחתיו מציג את תוכנו ואת מספר התווים (`Path=Text.Length`). השתמשו ב-`StringFormat`.

### תרגיל 8 — Shopping list ★★
`ObservableCollection<ShoppingItem>` עם `DataTemplate` (CheckBox + שם + מחיר), הוספה (שם + מחיר) ומחיקה
של הפריט הנבחר, ו-`Total` שמתעדכן אוטומטית (רמז: `CollectionChanged` → `OnPropertyChanged(nameof(Total))`).
`AddCommand` פעיל רק כשהשם לא ריק והמחיר מספר.

### תרגיל 9 — IValueConverter ★★
`Slider` של טמפרטורה (−10..45). כתבו `TempToBrushConverter` (כחול/ירוק/כתום/אדום לפי טווח) ו-
`ThresholdToVisibilityConverter` שמקבל את הסף ב-`ConverterParameter` ומציג אזהרה מעל 35°.

## מודול 05: חיבור ל-backend

### תרגיל 10 — Async load ★★
כפתור `Load` שמפעיל `FakeLoadAsync` (10 פריטים, `Task.Delay(300)` ביניהם) עם `IProgress<int>`
ל-`ProgressBar`, כפתור `Cancel` עם `CancellationTokenSource`, ומצב כפתורים נכון (`IsBusy`).
**בדקו:** אפשר להזיז את החלון בזמן הטעינה; Cancel מציג "Cancelled" ולא קורס.

## מודול 06: ולידציה

### תרגיל 11 — ValidationRule + ErrorTemplate ★★★
`TextBox` לאימייל עם `EmailRule : ValidationRule` (חובה + `MailAddress.TryCreate`), ‏`Validation.ErrorTemplate`
שמציג מסגרת אדומה **והודעה מתחת לשדה**, וכפתור `Subscribe` שמושבת כל עוד `Validation.HasError`
(רמז: `DataTrigger` עם `ElementName` ו-`Path=(Validation.HasError)`).

## מודול 07: פרוטוטייפינג מהיר

### תרגיל 12 — Styles & theme swap ★★
הגדירו ב-`Window.Resources` שלושה `SolidColorBrush` (‏`Bg`, `Fg`, `Accent`) ו-`Style` בשם `Pill` לכפתור
מעוגל (`ControlTemplate` עם `Border CornerRadius="14"`) שמשתמש ב-`DynamicResource Accent`.
כפתור `Toggle theme` מחליף את שלושת ה-brushes ב-`Resources[...]` — וכל החלון מתעדכן.
**שאלה:** מה היה קורה עם `StaticResource`?

</div>
