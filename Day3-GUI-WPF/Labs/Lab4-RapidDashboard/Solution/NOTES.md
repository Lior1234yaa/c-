<div dir="rtl">

# Lab 4 — הערות על הפתרון

## החלטות מרכזיות

- **שלושה מילונים: Light / Dark / Styles.** צבעים ב-theme, סגנונות במילון נפרד שמפנה לצבעים ב-`DynamicResource`.
  החלפת theme = החלפת `MergedDictionaries[0]`. זו הדרך הסטנדרטית (כך עובדות גם ספריות כמו MaterialDesignInXAML).
- **`StatCard` עם DependencyProperty:** בלי DP אי אפשר לכתוב `Value="{Binding ...}"` על ה-UserControl.
  ה-Binding-ים בתוך הפקד משתמשים ב-`ElementName=Root` ולא ב-`DataContext` — כך הפקד לא "גונב" את ה-DataContext
  של ההורה ונשאר עצמאי.
- **ה-Timer בחלון, לא ב-ViewModel:** `DispatcherTimer` הוא מושג UI. ה-VM חושף `Tick()` ולא יודע מי קורא לו —
  בבדיקה אפשר לקרוא ידנית. `ThemeChanged` הוא אירוע מאותה סיבה: ה-VM לא מכיר את `App`.
- **גרף "ידני":** `ItemsControl` + `Rectangle` עם גובה מחושב ב-VM (`BarPoint.Height`). לפרוטוטייפ זה מספיק;
  לגרפים אמיתיים יש ספריות (LiveCharts2, ScottPlot, OxyPlot).
- **`UniformGrid` לכרטיסים:** במקום Grid עם 4 עמודות `*`. פחות XAML, אותה תוצאה.
- **`Snapshot` הוא record immutable:** בכל Tick נוצר אובייקט חדש ו-`OnPropertyChanged(nameof(Snapshot))` מעדכן
  את כל ה-Binding-ים `Snapshot.X` בבת אחת — פשוט יותר מארבע properties נפרדות.

## נקודות למרצה

- זה הזמן להראות XAML Hot Reload: לשנות צבע ב-Light.xaml או Margin בכרטיס בזמן ריצה.
- לרמוז ליום 4: את כל ה-boilerplate של DP ו-Styles כלי AI מייצרים היטב — אבל צריך לדעת לקרוא אותו.
- שאלה טובה לדיון: מה היה קורה אם `Tick()` היה נקרא מ-`System.Timers.Timer`? (cross-thread exception.)

</div>
