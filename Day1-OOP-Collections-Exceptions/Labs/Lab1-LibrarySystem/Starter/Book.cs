namespace Day1.Lab1;

/// <summary>ספר בספרייה.</summary>
public class Book
{
    // TODO 1: הוסיפו properties:
    //   - Isbn (string, קריאה בלבד — נקבע בבנאי)
    //   - Title (string), Author (string), Year (int)
    //   - IsBorrowed (bool) — קריאה ציבורית, כתיבה פרטית
    //   - Age (int, מחושב): כמה שנים עברו מאז שנת ההוצאה

    // TODO 2: בנאי שמקבל isbn, title, author, year ומוודא שאף אחד מהם לא ריק/לא הגיוני
    //         (שנה בין 1450 לשנה הנוכחית). זרקו ArgumentException במקרה של קלט לא תקין.
    public Book(string isbn, string title, string author, int year)
    {
        throw new NotImplementedException();
    }

    // TODO 3: מתודות Borrow() ו-Return():
    //   Borrow — אם הספר כבר מושאל, זרקו InvalidOperationException; אחרת סמנו כמושאל.
    //   Return — אם הספר לא מושאל, זרקו InvalidOperationException; אחרת סמנו כזמין.
    public void Borrow() => throw new NotImplementedException();
    public void Return() => throw new NotImplementedException();

    // TODO 4: override ל-ToString שמחזיר למשל:  "[978-1] Clean Code / Robert Martin (2008) — available"
    public override string ToString() => throw new NotImplementedException();
}
