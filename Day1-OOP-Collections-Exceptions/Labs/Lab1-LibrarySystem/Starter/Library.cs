namespace Day1.Lab1;

/// <summary>אוסף הספרים של הספרייה ופעולות עליו.</summary>
public class Library
{
    private readonly List<Book> _books = [];

    // TODO 5: חשפו את הספרים החוצה כ-IReadOnlyList<Book> (בלי לאפשר שינוי מבחוץ)
    public IReadOnlyList<Book> Books => throw new NotImplementedException();

    public int Count => _books.Count;

    // TODO 6: AddBook — אם כבר קיים ספר עם אותו ISBN, זרקו InvalidOperationException
    public void AddBook(Book book) => throw new NotImplementedException();

    // TODO 7: חיפוש לפי חלק מהכותרת (לא תלוי רישיות). החזירו את כל ההתאמות.
    public List<Book> SearchByTitle(string text) => throw new NotImplementedException();

    // TODO 8: חיפוש לפי מחבר (התאמה מלאה, לא תלוי רישיות)
    public List<Book> SearchByAuthor(string author) => throw new NotImplementedException();

    // TODO 9: FindByIsbn — מחזיר את הספר או null אם לא נמצא
    public Book? FindByIsbn(string isbn) => throw new NotImplementedException();

    // TODO 10: Borrow / Return לפי ISBN. אם הספר לא קיים — החזירו false.
    //          אם קיים — קראו ל-book.Borrow()/Return() והחזירו true.
    public bool Borrow(string isbn) => throw new NotImplementedException();
    public bool Return(string isbn) => throw new NotImplementedException();

    // TODO 11: רשימת כל הספרים הזמינים (לא מושאלים), ממוינת לפי כותרת
    public List<Book> GetAvailable() => throw new NotImplementedException();

    // בונוס: כמה ספרים מושאלים כרגע? מהו הספר הישן ביותר?
}
