namespace Day1.Lab1;

/// <summary>ספר בספרייה.</summary>
public class Book
{
    public string Isbn { get; }
    public string Title { get; }
    public string Author { get; }
    public int Year { get; }
    public bool IsBorrowed { get; private set; }
    public int Age => DateTime.Now.Year - Year;   // computed property

    public Book(string isbn, string title, string author, int year)
    {
        // guard clauses — נכשלים מוקדם עם הודעה ברורה
        ArgumentException.ThrowIfNullOrWhiteSpace(isbn);
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        ArgumentException.ThrowIfNullOrWhiteSpace(author);
        if (year < 1450 || year > DateTime.Now.Year)
            throw new ArgumentOutOfRangeException(nameof(year), year, "Year must be between 1450 and the current year");

        Isbn = isbn;
        Title = title;
        Author = author;
        Year = year;
    }

    public void Borrow()
    {
        if (IsBorrowed) throw new InvalidOperationException($"'{Title}' is already borrowed");
        IsBorrowed = true;
    }

    public void Return()
    {
        if (!IsBorrowed) throw new InvalidOperationException($"'{Title}' is not borrowed");
        IsBorrowed = false;
    }

    public override string ToString() =>
        $"[{Isbn}] {Title} / {Author} ({Year}) — {(IsBorrowed ? "BORROWED" : "available")}";
}
