namespace Day1.Lab1;

/// <summary>אוסף הספרים של הספרייה ופעולות עליו.</summary>
public class Library
{
    private readonly List<Book> _books = [];

    // List<Book> מממש IReadOnlyList<Book> — לכן אפשר להחזיר אותו ישירות,
    // והקורא לא יכול לקרוא Add/Remove דרך הממשק.
    public IReadOnlyList<Book> Books => _books;

    public int Count => _books.Count;

    public void AddBook(Book book)
    {
        ArgumentNullException.ThrowIfNull(book);
        if (FindByIsbn(book.Isbn) is not null)
            throw new InvalidOperationException($"A book with ISBN {book.Isbn} already exists");
        _books.Add(book);
    }

    public List<Book> SearchByTitle(string text) =>
        _books.Where(b => b.Title.Contains(text, StringComparison.OrdinalIgnoreCase)).ToList();

    public List<Book> SearchByAuthor(string author) =>
        _books.Where(b => string.Equals(b.Author, author, StringComparison.OrdinalIgnoreCase)).ToList();

    public Book? FindByIsbn(string isbn) => _books.FirstOrDefault(b => b.Isbn == isbn);

    public bool Borrow(string isbn)
    {
        var book = FindByIsbn(isbn);
        if (book is null) return false;
        book.Borrow();     // עשוי לזרוק InvalidOperationException — זו אחריות הקורא לטפל
        return true;
    }

    public bool Return(string isbn)
    {
        var book = FindByIsbn(isbn);
        if (book is null) return false;
        book.Return();
        return true;
    }

    public List<Book> GetAvailable() =>
        _books.Where(b => !b.IsBorrowed).OrderBy(b => b.Title).ToList();

    // בונוס
    public int BorrowedCount => _books.Count(b => b.IsBorrowed);
    public Book? Oldest => _books.MinBy(b => b.Year);
}
