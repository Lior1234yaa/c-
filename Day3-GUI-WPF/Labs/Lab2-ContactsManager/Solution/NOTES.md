<div dir="rtl">

# Lab 2 — הערות על הפתרון

## החלטות מרכזיות

- **מודל "חכם" (Contact יורש מ-ValidatableObject):** בפרויקטים גדולים נהוג להפריד Model נקי מ-ViewModel
  לעריכה. כאן, כדי לצמצם קוד, המודל עצמו מודיע ומאמת. זו פשרה מודעת ל-MVVM-lite.
- **Draft (עותק לעריכה):** הטופס קשור ל-`Draft = Selected.Clone()`. כך Cancel פשוט זורק את העותק,
  ו-Apply מעתיק חזרה. בלי זה כל הקשה הייתה משנה את הרשימה מיד ואי אפשר היה לבטל.
- **ICollectionView לסינון/מיון:** במקום להחזיק שתי רשימות (מלאה ומסוננת), משתמשים ב-view שיושב מעל
  ה-`ObservableCollection`. ‏`Refresh()` אחרי Apply נחוץ כי המיון לא מאזין לשינויי property של פריטים.
- **`[JsonIgnore]` על `HasErrors`:** אחרת System.Text.Json היה מסרלז גם אותו (ולא היה יכול לדה-סרלז).
  `FullName` מסומן גם הוא כי הוא מחושב.
- **`ValidateAll()` אחרי טעינה:** דה-סריאליזציה עוקפת את ה-setters? לא בדיוק — היא קוראת להם, אבל
  `SetProperty` לא יפעיל ולידציה על ערך זהה לברירת המחדל (למשל טלפון ריק). `ValidateAll()` סוגר את הפינה.
- **Closing + async:** `Closing` הוא סינכרוני. ‏`SaveAsync().GetAwaiter().GetResult()` על ה-UI thread
  יגרום ל-deadlock (ה-continuation מחכה ל-Dispatcher שחסום). התבנית: `e.Cancel = true; await Save; Close()`.
- **DI-lite:** `MainWindow` בונה `JsonContactsStore` ומזריק ל-VM. בבדיקות אפשר להזריק `InMemoryStore`.

## נקודות למרצה

- להראות את ה-Output window ב-VS כשיש שגיאת binding (`System.Windows.Data Error`) — זו הדרך לדבג bindings.
- `RelativeSource AncestorType=Window` מבלבל; לצייר על הלוח את עץ ה-DataContext.

</div>
