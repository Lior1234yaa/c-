<div dir="rtl">

# Lab 2 — Contacts Manager: ‏MVVM-lite, ולידציה ושמירה ל-JSON

**משך:** 75 דקות | **רמה:** ★★ | **מודולים:** 04–06 (Data binding, חיבור ללוגיקה, ולידציה)

## המטרה

אפליקציית ניהול אנשי קשר במבנה MVVM-lite: רשימה (DataGrid) משמאל, טופס פרטים מימין,
הוספה/עריכה/מחיקה, חיפוש, ולידציה עם `INotifyDataErrorInfo`, ושמירה/טעינה מקובץ JSON ב-`%AppData%`.

<div dir="ltr">

```text
+---------------------------------------------------------------+
| [+ Add] [Delete] | [Save] [Reload] | Search: [________]       |
+-----------------------------------+---------------------------+
| ★ | Name        | Phone   | Email |  Dana Levi                |
|   | Dana Levi   | 050-... | d@..  |  First name * [Dana    ]  |
| ★ | Yossi Cohen | 052-... | y@..  |  Last name    [Levi    ]  |
|   | ...         |         |       |  Phone *      [050-... ]  |
|                                   |  Email        [d@x.com ]  |
|                                   |  Notes        [        ]  |
|                                   |  [x] Favorite             |
|                                   |         [Apply] [Cancel]  |
+-----------------------------------+---------------------------+
| Status: נשמר (3) ב-14:02:11                                   |
+---------------------------------------------------------------+
```

</div>

## מבנה הפרויקט (Starter)

<div dir="ltr">

```text
Models/Contact.cs              ← המודל (TODO 1, 4)
Mvvm/ObservableObject.cs       ← INotifyPropertyChanged מוכן
Mvvm/ValidatableObject.cs      ← INotifyDataErrorInfo (TODO 3)
Mvvm/RelayCommand.cs           ← מוכן
Services/IContactsStore.cs     ← ממשק
Services/JsonContactsStore.cs  ← שמירה/טעינה (TODO 6)
ViewModels/ContactsViewModel.cs← הפקודות והמצב (TODO 2, 5)
MainWindow.xaml                ← ה-View (TODO 7, 8)
```

</div>

## שלבים

### חלק א' — המודל מודיע על שינויים (15 דק')

1. **TODO 1 (`Contact.cs`):** החליפו את ה-setters הפשוטים ב-`SetProperty(ref _field, value)`.
   ב-`FirstName` ו-`LastName` הוסיפו גם `OnPropertyChanged(nameof(FullName))` — כי `FullName` מחושב.
   הריצו: שינוי שם בטופס אמור להתעדכן בכותרת הטופס (עדיין לא ברשימה — זה יגיע ב-Apply).

### חלק ב' — פקודות (15 דק')

2. **TODO 2 (`ContactsViewModel.cs`):** הוסיפו `CanExecute` לפקודות:
   `Delete`/`Cancel` רק כשיש בחירה, `Apply` רק כש-`Draft is { HasErrors: false }`, `Save` רק כש-`IsDirty`.
   ממשו `Delete` (הסרה + בחירת השכן) ו-`Apply` (‏`Selected.CopyFrom(Draft)`, ‏`MarkDirty()`, ‏`ContactsView.Refresh()`).
   שימו לב: הכפתורים בסרגל הכלים "מתים" ו"קמים" לבד — זו העבודה של `CanExecute`.

### חלק ג' — ולידציה (20 דק')

3. **TODO 3 (`ValidatableObject.cs`):** ממשו `GetErrors` ו-`SetErrors` (מילון `property → רשימת שגיאות`).
   `SetErrors` צריך להסיר את המפתח כשהרשימה ריקה, להפעיל `ErrorsChanged`, ולהודיע על `HasErrors`.
4. **TODO 4 (`Contact.cs`):** קראו לפונקציות הולידציה מה-setters וממשו אותן:
   שם פרטי חובה; טלפון חובה, 9–15 ספרות; אימייל אופציונלי אך תקין (`MailAddress.TryCreate`).
   הריצו: מחיקת השם הפרטי → מסגרת אדומה ו-ToolTip עם ההודעה (ה-Style ב-`App.xaml` כבר מוכן), וכפתור Apply מושבת.

### חלק ד' — חיפוש ושמירה (15 דק')

5. **TODO 5:** הגדירו `ContactsView.Filter` לפי `Search` (שם/טלפון/אימייל, לא תלוי רישיות) ו-`SortDescription` לפי שם.
6. **TODO 6 (`JsonContactsStore.cs`):** ממשו `LoadAsync`/`SaveAsync` עם `System.Text.Json` (זוכרים מ-Day 2?).
   אל תשכחו `Directory.CreateDirectory` ו-`ValidateAll()` אחרי טעינה.

### חלק ה' — ה-View (10 דק')

7. **TODO 7:** עמודות מפורשות ב-DataGrid (`DataGridCheckBoxColumn` ל-★, `DataGridTextColumn` לשאר) והשלמת
   טופס הפרטים עם כל השדות + Cancel.
8. **TODO 8 (בונוס):** `InputBindings` — ‏Ctrl+N, ‏Ctrl+S, ‏Delete.

## קריטריוני קבלה

- [ ] Add יוצר איש קשר חדש שנבחר אוטומטית, והטופס מתמלא.
- [ ] עריכה בטופס לא משנה את הרשימה עד Apply; ‏Cancel מחזיר את הערכים המקוריים.
- [ ] שם ריק / טלפון קצר / אימייל שגוי → מסגרת אדומה, ToolTip, ו-Apply מושבת.
- [ ] Delete מוסר ומעביר בחירה לשכן. הכותרת מציגה `*` כשיש שינויים.
- [ ] Save כותב JSON ל-`%AppData%\Day3.ContactsManager\contacts.json`; סגירה ופתיחה מחדש טוענת אותו.
- [ ] Search מסנן בזמן ההקלדה.
- [ ] סגירת חלון עם שינויים שואלת אם לשמור.

## בונוס

- ייצוא ל-CSV דרך `SaveFileDialog`.
- מיון בלחיצה על כותרת עמודה (DataGrid עושה זאת לבד — בדקו שזה עובד עם `ICollectionView`).
- הצגת מספר התוצאות המסוננות בשורת המצב.

## רמזים

- `CollectionViewSource.GetDefaultView(Contacts)` נותן "תצוגה" עם סינון ומיון בלי לשנות את האוסף.
- בטופס, `DataContext="{Binding Draft}"` — לכן כדי להגיע ל-`ApplyCommand` (שנמצא ב-VM של החלון) משתמשים
  ב-`RelativeSource AncestorType=Window` ו-`DataContext.ApplyCommand`.
- `Closing` הוא סינכרוני; כדי לשמור async לפני סגירה — מבטלים את הסגירה, שומרים, ואז קוראים `Close()` שוב (ראו Solution).

</div>
