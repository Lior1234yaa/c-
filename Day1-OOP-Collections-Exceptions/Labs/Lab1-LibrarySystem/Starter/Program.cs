// Day1.Lab1.Starter — מערכת ספרייה
// השלימו את ה-TODO בקבצים Book.cs ו-Library.cs. התפריט כאן כבר מוכן.
using Day1.Lab1;

var library = new Library();
Seed(library);

while (true)
{
    Console.WriteLine();
    Console.WriteLine("=== Library ===");
    Console.WriteLine("1) List all   2) Search title   3) Search author   4) Borrow   5) Return   6) Available   0) Exit");
    Console.Write("> ");
    var choice = Console.ReadLine();
    if (choice is null || choice == "0") break;   // EOF או יציאה

    try
    {
        switch (choice)
        {
            case "1":
                foreach (var b in library.Books) Console.WriteLine(b);
                break;
            case "2":
                Console.Write("title contains: ");
                foreach (var b in library.SearchByTitle(Console.ReadLine() ?? "")) Console.WriteLine(b);
                break;
            case "3":
                Console.Write("author: ");
                foreach (var b in library.SearchByAuthor(Console.ReadLine() ?? "")) Console.WriteLine(b);
                break;
            case "4":
                Console.Write("isbn: ");
                Console.WriteLine(library.Borrow(Console.ReadLine() ?? "") ? "borrowed" : "not found");
                break;
            case "5":
                Console.Write("isbn: ");
                Console.WriteLine(library.Return(Console.ReadLine() ?? "") ? "returned" : "not found");
                break;
            case "6":
                foreach (var b in library.GetAvailable()) Console.WriteLine(b);
                break;
            default:
                Console.WriteLine("unknown option");
                break;
        }
    }
    catch (Exception ex) when (ex is InvalidOperationException or ArgumentException)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}
Console.WriteLine("Bye!");

static void Seed(Library library)
{
    // TODO 12: לאחר שסיימתם את Book/Library, הסירו את ההערה מהשורות הבאות:
    // library.AddBook(new Book("978-0132350884", "Clean Code", "Robert C. Martin", 2008));
    // library.AddBook(new Book("978-0201633610", "Design Patterns", "Erich Gamma", 1994));
    // library.AddBook(new Book("978-0135957059", "The Pragmatic Programmer", "David Thomas", 2019));
    // library.AddBook(new Book("978-0134685991", "Effective Java", "Joshua Bloch", 2018));
    // library.AddBook(new Book("978-1617294532", "C# in Depth", "Jon Skeet", 2019));
}
