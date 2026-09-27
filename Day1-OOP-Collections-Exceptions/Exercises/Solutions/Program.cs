// Day1.Exercises.Solutions — פתרונות לתרגילי יום 1
// הרצה:  dotnet run -- <מספר תרגיל>     או  dotnet run  לתפריט
using Day1.Exercises;

var exercises = new (string Title, Action Run)[]
{
    ("ממיר טמפרטורות", Ex01.Run),
    ("FizzBuzz עם switch expression", Ex02.Run),
    ("מחלקת Rectangle", Ex03.Run),
    ("record מול class + static", Ex04.Run),
    ("כלי רכב — ירושה וממשקים", Ex05.Run),
    ("pattern matching", Ex06.Run),
    ("ספירת מילים (Dictionary)", Ex07.Run),
    ("MyStack<T> גנרי", Ex08.Run),
    ("האוסף הנכון", Ex09.Run),
    ("LINQ על מספרים", Ex10.Run),
    ("delegates ואירוע", Ex11.Run),
    ("קלט בטוח + ValidationException", Ex12.Run),
    ("סדר ריצה: using/try/finally", Ex13.Run),
    ("refactoring", Ex14.Run),
};

if (args.Length > 0 && int.TryParse(args[0], out int n) && n >= 1 && n <= exercises.Length)
{
    RunOne(n);
    return;
}

while (true)
{
    Console.WriteLine("\n=== Day 1 exercises ===");
    for (int i = 0; i < exercises.Length; i++) Console.WriteLine($"{i + 1,2}) {exercises[i].Title}");
    Console.Write("exercise number (0 = exit): ");
    var input = Console.ReadLine();
    if (input is null || input == "0") break;
    if (int.TryParse(input, out int choice) && choice >= 1 && choice <= exercises.Length) RunOne(choice);
    else Console.WriteLine("invalid choice");
}

void RunOne(int number)
{
    var (title, run) = exercises[number - 1];
    Console.WriteLine($"\n--- Exercise {number}: {title} ---");
    run();
}
