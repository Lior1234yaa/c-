# Lab 1 — מערכת ספרייה (45 דקות)

## מטרה

לבנות מודל אובייקטים קטן ומסודר: מחלקת `Book` עם properties ובנאי שמוודא קלט, ומחלקת `Library` שמנהלת `List<Book>` — הוספה, חיפוש, השאלה והחזרה. בסוף הלאב תדעו להסביר למה המצב של אובייקט צריך להשתנות רק דרך מתודות שלו.

## מה צריך לדעת לפני

- מודול 01 (חימום C#) ומודול 02 (מחלקות, אובייקטים, properties, בנאים).
- מודול 04 עוזר (List<T>), אבל אפשר להסתדר עם `foreach` בלבד.

## התחלה

```bash
cd Labs/Lab1-LibrarySystem/Starter
dotnet run
```

הפרויקט מתקמפל כבר עכשיו, אבל כל מתודה זורקת `NotImplementedException`. עברו על ה-`// TODO` לפי הסדר.

## שלבים

### שלב 1 — `Book` (15 דק')

1. **TODO 1** — הוסיפו את ה-properties: `Isbn` (קריאה בלבד), `Title`, `Author`, `Year`, `IsBorrowed` (`private set`), ו-`Age` מחושב.
2. **TODO 2** — כתבו את הבנאי. ודאו שהמחרוזות אינן ריקות (`ArgumentException.ThrowIfNullOrWhiteSpace`) ושהשנה הגיונית (`ArgumentOutOfRangeException`).
3. **TODO 3** — `Borrow()` ו-`Return()`. חשבו: מה צריך לקרות כשמנסים להשאיל ספר שכבר מושאל? (רמז: זו הפרת כלל — זרקו `InvalidOperationException`.)
4. **TODO 4** — `ToString()` בפורמט `[isbn] Title / Author (Year) — available|BORROWED`.

### שלב 2 — `Library` (20 דק')

5. **TODO 5** — חשפו את הספרים כ-`IReadOnlyList<Book>`. למה לא `List<Book>`?
6. **TODO 6** — `AddBook` שמונע ISBN כפול.
7. **TODO 7–9** — חיפושים: לפי חלק מהכותרת, לפי מחבר, לפי ISBN (מחזיר `null` אם אין).
8. **TODO 10** — `Borrow(isbn)` / `Return(isbn)` — מחזירים `false` אם הספר לא קיים, אחרת מפעילים את המתודה של הספר.
9. **TODO 11** — `GetAvailable()` ממוין לפי כותרת.

### שלב 3 — חיבור (10 דק')

10. **TODO 12** — הסירו את ההערות ב-`Seed` והריצו. נסו את כל אפשרויות התפריט, כולל השאלה כפולה.

## קריטריוני קבלה

- [ ] `dotnet build` עובר ללא שגיאות ואזהרות.
- [ ] יצירת `new Book("", "x", "y", 2000)` זורקת `ArgumentException`.
- [ ] יצירת ספר עם שנה 3000 זורקת `ArgumentOutOfRangeException`.
- [ ] השאלה של ספר פעמיים מדפיסה `Error: ... already borrowed` ולא מפילה את התוכנית.
- [ ] `SearchByTitle("CLEAN")` מוצא את "Clean Code" (לא תלוי רישיות).
- [ ] `GetAvailable()` לא כולל ספרים מושאלים, וממוין לפי כותרת.
- [ ] `Books` מוגדר כ-`IReadOnlyList<Book>` ואי אפשר לקרוא `library.Books.Add(...)`.

## בונוס

- `BorrowedCount` — כמה ספרים מושאלים כרגע.
- `Oldest` — הספר הישן ביותר (רמז: `MinBy`).
- הוסיפו `DueDate` (תאריך החזרה) שנקבע ב-`Borrow` ל-14 יום קדימה, ו-`IsOverdue`.

## רמזים

- `string.Contains(text, StringComparison.OrdinalIgnoreCase)` — חיפוש לא תלוי רישיות.
- `_books.FirstOrDefault(b => b.Isbn == isbn)` מחזיר `null` כשאין התאמה.
- `List<T>` מממש `IReadOnlyList<T>`, אז `public IReadOnlyList<Book> Books => _books;` עובד.
- `DateTime.Now.Year` — השנה הנוכחית.
