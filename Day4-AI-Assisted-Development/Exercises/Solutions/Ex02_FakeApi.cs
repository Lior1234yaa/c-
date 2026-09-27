namespace Day4.Exercises.Solutions;

/// <summary>
/// תרגיל 2: שלושה APIs מומצאים — RemoveWhere, ContainsIgnoreCase, JoinWith.
/// כולם נשמעים הגיוניים; אף אחד לא קיים. הקומפיילר הוא קו ההגנה הראשון.
/// </summary>
public static class Ex02_FakeApi
{
    public static void Run()
    {
        var names = new List<string> { "Dana", "yossi", "Noa", "Li" };

        names.RemoveAll(n => n.Length < 3);                                   // במקום RemoveWhere
        if (names.Contains("YOSSI", StringComparer.OrdinalIgnoreCase))         // במקום ContainsIgnoreCase
            Console.WriteLine("found");
        var joined = string.Join(", ", names);                                 // במקום JoinWith

        Console.WriteLine(joined);
    }
}
