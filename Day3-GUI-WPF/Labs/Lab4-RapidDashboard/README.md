<div dir="rtl">

# Lab 4 — Rapid Dashboard: מ-wireframe לאפליקציה ב-45 דקות

**משך:** 60 דקות (45 בנייה + 15 בונוס) | **רמה:** ★★ | **מודולים:** 07 (+ 02, 04)

## המטרה

לתרגל **פרוטוטייפינג מהיר**: מקבלים wireframe, ובונים ממנו dashboard "חי" תוך שימוש בכלים שמאיצים
עבודה — Styles ו-ResourceDictionary, UserControl לשימוש חוזר (`StatCard`), TabControl, ו-`DispatcherTimer`
שמזין נתונים מזויפים. בונוס: החלפת theme (בהיר/כהה) בזמן ריצה.

## ה-wireframe

<div dir="ltr">

```text
+------------------------------------------------------------------------+
| Ops Dashboard                        Updated 14:02:11  [x] Live  [🌙/☀] |
+------------------------------------------------------------------------+
| [Overview] [Orders] [Settings]                                         |
+------------------------------------------------------------------------+
| +---------------+ +---------------+ +---------------+ +---------------+ |
| | • Orders today| | • Revenue     | | • Active users| | • Errors      | |
| |   127         | |   ₪19,240     | |   41          | |   2           | |
| |   since 00:00 | |   ILS         | |   right now   | |   last hour   | |
| +---------------+ +---------------+ +---------------+ +---------------+ |
| +--------------------------------------------------------------------+ |
| | Active users — last 20 samples                                     | |
| |  ▂▃▅▆▅▄▃▅▇█▆▅▄▃▂▃▄▅▆▅                                              | |
| +--------------------------------------------------------------------+ |
+------------------------------------------------------------------------+
Orders tab:   DataGrid — # | Customer | Amount | Status | Time
Settings tab: [x] Dark theme     Refresh every [====o----] 3 s
```

</div>

## מבנה (Starter)

<div dir="ltr">

```text
Themes/Light.xaml          ← צבעים (מוכן)
Themes/Styles.xaml         ← סגנונות (TODO 1)
Controls/StatCard.xaml(.cs)← UserControl (TODO 2)
Services/FakeMetricsService.cs ← נתונים "חיים" (מוכן)
ViewModels/DashboardViewModel.cs ← Tick(), Snapshot, RecentOrders, UsersHistory (מוכן)
MainWindow.xaml(.cs)       ← TODO 3–7
App.xaml.cs                ← TODO 7 (בונוס)
```

</div>

טיפ: עבדו עם **XAML Hot Reload** — הריצו את האפליקציה פעם אחת ותערכו XAML בזמן שהיא רצה.

## שלבים (45 דק')

1. **TODO 1 — Styles (5 דק'):** ב-`Themes/Styles.xaml` הוסיפו `H1`, `Muted` ו-`AccentButton`
   (כפתור עם `ControlTemplate` קצר: `Border` מעוגל + `ContentPresenter` + Trigger ל-`IsMouseOver`).
   השתמשו ב-`{DynamicResource ...}` לצבעים.
2. **TODO 2 — StatCard (10 דק'):** הוסיפו `DependencyProperty` ל-`Value`, `Subtitle`, `Accent`
   (העתיקו את התבנית של `Title`). ב-XAML של ה-UserControl בנו את הכרטיס עם `{Binding X, ElementName=Root}`.
3. **TODO 3 — כותרת וטיימר (5 דק'):** שורת כותרת עם `LastUpdated`, ‏CheckBox ‏`Live` וכפתור theme.
   בקוד: `DispatcherTimer` שקורא ל-`_vm.Tick()` כל שנייה.
4. **TODO 4 — כרטיסים (10 דק'):** `UniformGrid Columns="4"` עם ארבעה `StatCard` קשורים ל-`Snapshot.*`.
   ל-Revenue השתמשו ב-`StringFormat={}{0:C0}`.
5. **TODO 5 — גרף (5 דק'):** `ItemsControl` על `UsersHistory` עם `StackPanel` אופקי ו-`Rectangle` שגובהו `{Binding Height}`.
6. **TODO 6 — Orders (5 דק'):** DataGrid על `RecentOrders` — שורות חדשות נכנסות למעלה בזמן אמת.
7. **TODO 7 — Settings + theme (בונוס, 15 דק'):** צרו `Themes/Dark.xaml` עם אותם מפתחות וצבעים כהים;
   ממשו `App.ApplyTheme` (החלפת `MergedDictionaries[0]`); חברו `_vm.ThemeChanged += App.ApplyTheme`.
   Slider ל-`RefreshSeconds` שמשנה את `_timer.Interval`.

## קריטריוני קבלה

- [ ] ארבעה כרטיסים ברוחב שווה שמתעדכנים כל שנייה; ביטול `Live` עוצר את העדכון.
- [ ] גרף העמודות זז (ערך חדש נכנס מימין, ישן יוצא משמאל).
- [ ] טאב Orders מציג הזמנות חדשות למעלה (עד 25).
- [ ] הקטנת החלון לא שוברת את הפריסה (הכרטיסים מתכווצים, יש ScrollViewer).
- [ ] בונוס: לחיצה על 🌙/☀ מחליפה theme בלי לפתוח חלון מחדש.

## רמזים

- `UniformGrid` נותן עמודות שוות בלי להגדיר `ColumnDefinition` — מצוין לפרוטוטייפ.
- `DynamicResource` (ולא `StaticResource`) הוא מה שמאפשר להחליף צבעים בזמן ריצה.
- ב-`StatCard`, `x:Name="Root"` על ה-UserControl + `ElementName=Root` — אחרת ה-Binding יחפש ב-DataContext של החלון.
- אם `Value` הוא מספר, WPF ימיר אותו ל-string לבד; ל-`StringFormat` צריך `{}` בהתחלה כדי לברוח מסוגריים.

</div>
