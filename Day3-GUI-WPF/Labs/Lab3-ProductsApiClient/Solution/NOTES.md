# Lab 3 — הערות על הפתרון

## החלטות מרכזיות

- **ממשק + שני מימושים:** `IProductService` עם `FakeProductService` ו-`HttpProductService`. ה-ViewModel
  לא יודע (ולא אכפת לו) מאיפה הנתונים מגיעים. זה מאפשר לפתח ולבדוק UI בלי שרת, וזה גם הבסיס ל-DI אמיתי.
- **Composition root ב-`MainWindow`:** המקום היחיד שבו נבחרים מימושים קונקרטיים. `HttpClient` אחד סטטי
  לכל חיי האפליקציה (כמו שלמדנו ביום 2).
- **DTO סלחני:** `ProductDto` מקבל `name` או `title`, ו-`stock` או `quantity`, ומטפל גם במערך וגם ב-`{ products: [] }`.
  כך אותו קוד עובד מול `Day2.LocalApi` ומול API ציבורי (dummyjson.com). בפרויקט אמיתי מקובל DTO לכל API.
- **`AsyncRelayCommand`:** `async void Execute` הוא הכרח ב-ICommand, אבל עוטפים אותו ב-try/finally ומונעים
  הפעלה כפולה. ה-`LoadAsync` עצמו תופס את כל החריגות ומתרגם אותן ל-`Error`/`Status`.
- **שגיאה inline במקום MessageBox:** MessageBox חוסם, לא ניתן לניסיון חוזר, ומאבד הקשר. באנר עם Retry
  משאיר את המשתמש בשליטה. MessageBox שמור לאישורים ולשגיאות קטלניות.
- **Cancel כפול:** `CancellationTokenSource(TimeSpan.FromSeconds(30))` — גם המשתמש וגם timeout מבטלים
  דרך אותו token. `Task.Delay(…, ct)` ב-Fake ו-`GetAsync(…, ct)` ב-HTTP מכבדים אותו.
- **פריסה אדפטיבית ב-code-behind:** `SizeChanged` מחליף בין DataGrid ל-ItemsControl. זו החלטת View
  טהורה, לכן היא בקוד של החלון ולא ב-ViewModel.
- **`ICollectionView` לסינון:** אותה טכניקה כמו ב-Lab 2. `VisibleCount` מחושב מה-view ומתעדכן ב-`CollectionChanged`.

## נקודות למרצה

- להריץ עם ה-Fake ה"נכשל" כמה פעמים — כשל אקראי מדגים היטב את Retry.
- לפתוח את `Day2.LocalApi` בטרמינל אחד ואת הלקוח בשני; לכבות את השרת באמצע ולראות את הבאנר.
- `dummyjson.com` הוא API ציבורי לדוגמה; אם אין אינטרנט בכיתה — פשוט מדלגים.
