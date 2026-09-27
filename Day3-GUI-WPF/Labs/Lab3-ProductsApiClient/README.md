<div dir="rtl">

# Lab 3 — Products API Client: לקוח WPF מעל REST API

**משך:** 60 דקות | **רמה:** ★★ | **מודולים:** 02, 05 (UI רספונסיבי, חיבור ל-backend אסינכרוני)

## המטרה

לבנות לקוח שולחני שמציג קטלוג מוצרים מ-API: טעינה אסינכרונית עם התקדמות וביטול,
טיפול בשגיאות ידידותי, חיפוש/סינון, ופריסה שמתאימה את עצמה לרוחב החלון.
המעבדה עובדת **גם בלי שרת**: `FakeProductService` מדמה רשת איטית. מי שהריץ את `Day2.LocalApi`
מיום 2 יכול לעבור ל-`HttpProductService` ולראות נתונים אמיתיים.

<div dir="ltr">

```text
+------------------------------------------------------------------+
| [Fake (in-memory) v] [Load] [Cancel]  Search:[____] [Cat v] [x]  |
+------------------------------------------------------------------+
| ! לא ניתן לטעון מוצרים מהשרת... [Retry]        (רק בשגיאה)      |
+------------------------------------------------------------------+
| #  | Name              | Category | Price   | Stock               |
| 1  | Phone Model 01    | Phones   | ₪1,299  | 12                  |
| ...                                                              |
+------------------------------------------------------------------+
| Loaded 60 products | Showing 60 | [=====     ] 50%               |
+------------------------------------------------------------------+
```

</div>

## מבנה

<div dir="ltr">

```text
Models/Product.cs                 ← record של ה-UI
Services/IProductService.cs       ← הממשק שה-UI תלוי בו
Services/FakeProductService.cs    ← מוכן: in-memory, איטי, יודע להיכשל
Services/HttpProductService.cs    ← TODO 4: HttpClient + System.Text.Json
ViewModels/ProductsViewModel.cs   ← TODO 1–3
MainWindow.xaml(.cs)              ← TODO 1b/1c, 2b, 5, 6, 7
```

</div>

## שלבים

### חלק א' — טעינה אסינכרונית עם התקדמות (15 דק')

1. **TODO 1 (`ProductsViewModel.LoadAsync`):** הפכו את הטעינה ל"אמיתית": `CancellationTokenSource`,
   `IsBusy`, `Progress<int>` שמעדכן את `Progress`, ו-`Status`.
   **1b:** הוסיפו `ProgressBar` בשורת המצב, מוצג רק כש-`IsBusy` (`BooleanToVisibilityConverter` כבר מוגדר ב-`App.xaml` בשם `BoolToVis`).
   **1c:** שכבת "טוען…" חצי-שקופה מעל הטבלה עם כפתור Cancel. ‏Cancel צריך באמת לעצור (בדקו עם ה-Fake האיטי).

### חלק ב' — שגיאות (10 דק')

2. **TODO 2:** תפסו `OperationCanceledException` (סטטוס "Cancelled") ו-`HttpRequestException`
   (הודעה ידידותית בעברית ב-`Error`). ב-`finally` — ‏`IsBusy = false`.
   **2b:** באנר שגיאה inline (לא MessageBox!) עם כפתור Retry שמפעיל `LoadCommand`.
   בחרו ב-ComboBox את "Fake" השני (`FailRandomly`) כדי לבדוק.

### חלק ג' — חיפוש וסינון (10 דק')

3. **TODO 3:** חברו `ProductsView.Filter` ל-`Matches` וקראו ל-`ProductsView.Refresh()` בכל שינוי של
   `Search` / `Category` / `OnlyInStock`. שימו לב ש-`VisibleCount` בשורת המצב מתעדכן לבד.

### חלק ד' — HTTP אמיתי (10 דק')

4. **TODO 4 (`HttpProductService`):** ממשו עם `HttpClient` (‏`GetAsync` + `EnsureSuccessStatusCode` +
   `ReadFromJsonAsync<List<ProductDto>>` או `JsonDocument`). דווחו התקדמות ב-10/50/100.
   הריצו את `Day2.LocalApi` ובחרו "HTTP" ב-ComboBox. אין שרת? הבאנר מחלק ב' אמור להופיע — זה הצלחה!

### חלק ה' — פריסה רספונסיבית (15 דק')

5. **TODO 5:** תצוגת כרטיסים חלופית (`ItemsControl` עם `WrapPanel` ו-`DataTemplate`, ה-Style `Card` כבר קיים).
   **5b:** ב-`SizeChanged` — מתחת ל-700px הציגו כרטיסים במקום טבלה.
6. **TODO 6:** החליפו את ה-`StackPanel` בסרגל העליון ב-`WrapPanel` כדי שלא ייחתך בחלון צר.
7. **TODO 7 (בונוס):** ‏F5 טוען, Escape מבטל (`InputBindings`).

## קריטריוני קבלה

- [ ] בזמן טעינה: הכפתור Load מושבת, Cancel פעיל, ProgressBar מתקדם, החלון לא קופא (אפשר להזיז אותו).
- [ ] Cancel עוצר את הטעינה ומציג "Cancelled".
- [ ] כשל רשת מציג באנר עם הודעה בעברית ו-Retry; לא MessageBox ולא קריסה.
- [ ] חיפוש + קטגוריה + "In stock only" עובדים יחד, והמונה בשורת המצב נכון.
- [ ] `HttpProductService` מביא נתונים מ-`Day2.LocalApi` (או מציג באנר אם השרת כבוי).
- [ ] הצרת החלון מתחת ל-700px מחליפה לכרטיסים; הסרגל העליון נשבר לשורות ולא נחתך.

## בונוס

- Debounce לחיפוש: להריץ `Refresh()` רק 300ms אחרי ההקשה האחרונה (`DispatcherTimer`).
- מיון בלחיצה על כותרת (DataGrid עושה זאת לבד) — ודאו שזה שורד `Refresh()`.
- הצגת פרטי מוצר בחלון נפרד בלחיצה כפולה (`MouseDoubleClick`).

## רמזים

- `AsyncRelayCommand` (ב-`Mvvm/RelayCommand.cs`) כבר מונע לחיצה כפולה בזמן ריצה.
- `Progress<T>` חייב להיווצר על ה-UI thread (בבנאי/ב-LoadAsync לפני ה-await הראשון) — כך ה-callback רץ עליו.
- `Visibility` הוא enum, לא bool — לכן צריך converter.

</div>
