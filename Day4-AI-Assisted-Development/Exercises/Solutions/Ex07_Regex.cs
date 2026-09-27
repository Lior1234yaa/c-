using System.Text.RegularExpressions;

namespace Day4.Exercises.Solutions;

/// <summary>תרגיל 7: regex בלי עוגנים תופס תת-מחרוזת; עם עוגנים וקידומת 0 — נכון.</summary>
public static partial class Ex07_Regex
{
    [GeneratedRegex(@"\d{3}-\d{7}")]
    private static partial Regex AiVersion();

    [GeneratedRegex(@"^0\d{2}-\d{7}$")]
    private static partial Regex Fixed();

    public static void Run()
    {
        string[] inputs = ["052-1234567", "x052-1234567y", "952-1234567", "052-12345678", "0521234567"];
        Console.WriteLine($"{"input",-16} {"AI (no anchors)",-16} {"fixed",-6}");
        foreach (var s in inputs)
            Console.WriteLine($"{s,-16} {AiVersion().IsMatch(s),-16} {Fixed().IsMatch(s),-6}");
    }
}
