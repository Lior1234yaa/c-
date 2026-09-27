// Day1.Lab4.Starter — בנק "עמיד": חריגות, ולידציה, דיבוג
// התוכנית מתקמפלת ורצה — אבל מכילה לפחות 8 באגים. מצאו ותקנו אותם (ראו README).
//
// פקודות (שורה אחת לכל פקודה, או קובץ דרך stdin):
//   open <id> <owner>      deposit <id> <amount>     withdraw <id> <amount>
//   transfer <from> <to> <amount>     show <id>     list     quit
using Day1.Lab4;

var log = new TransactionLog("transactions.log");   // BUG? מתי הקובץ נסגר ומתרוקן לדיסק?
var bank = new Bank(log);
int processed = 0, failed = 0;

Console.WriteLine("Bank ready. Type 'help' for commands.");
while (true)
{
    Console.Write("bank> ");
    var line = Console.ReadLine();
    if (line is null || line.Trim() == "quit") break;
    if (string.IsNullOrWhiteSpace(line)) continue;

    var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
    var cmd = parts[0].ToLowerInvariant();

    try
    {
        switch (cmd)
        {
            case "help":
                Console.WriteLine("open <id> <owner> | deposit <id> <amount> | withdraw <id> <amount> | transfer <from> <to> <amount> | show <id> | list | quit");
                break;
            case "open":
                var acc = bank.Open(parts[1], parts[2]);
                Console.WriteLine($"opened {acc.Id} for {acc.Owner}");
                break;
            case "deposit":
                // TODO B: החליפו את decimal.Parse ב-TryParse והדפיסו הודעה ידידותית לקלט לא מספרי
                bank.Deposit(parts[1], decimal.Parse(parts[2]));
                Console.WriteLine($"ok, balance = {bank.Get(parts[1]).Balance:N2}");
                break;
            case "withdraw":
                bank.Withdraw(parts[1], decimal.Parse(parts[2]));
                Console.WriteLine($"ok, balance = {bank.Get(parts[1]).Balance:N2}");
                break;
            case "transfer":
                bank.Transfer(parts[1], parts[2], decimal.Parse(parts[3]));
                Console.WriteLine("ok");
                break;
            case "show":
                var a = bank.Get(parts[1]);
                Console.WriteLine($"{a.Id} ({a.Owner}): {a.Balance:N2}");
                foreach (var h in a.RecentHistory(3)) Console.WriteLine("   " + h);
                break;
            case "list":
                foreach (var x in bank.Accounts) Console.WriteLine($"{x.Id} ({x.Owner}): {x.Balance:N2}");
                break;
            default:
                Console.WriteLine($"unknown command '{cmd}'");
                break;
        }
        processed++;
    }
    catch (BankException ex)
    {
        Console.WriteLine($"error: {ex.Message}");
        failed++;
    }
    // TODO C: מה קורה עם IndexOutOfRangeException כשחסר פרמטר? ועם FormatException? הוסיפו טיפול.
}

Console.WriteLine($"processed={processed}, failed={failed}, log lines={log.Lines}");
