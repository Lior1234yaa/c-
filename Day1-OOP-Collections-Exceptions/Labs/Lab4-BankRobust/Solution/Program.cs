// Day1.Lab4.Solution — בנק "עמיד": חריגות, ולידציה, דיבוג (פתרון מלא)
// הרצה: dotnet run   ואז פקודות. או:
//   printf 'open acc1 Dana\ndeposit acc1 100\nwithdraw ACC1 30\nwithdraw acc1 500\ndeposit acc1 abc\ndeposit acc1\nopen acc2 Yossi\ntransfer acc1 acc2 50\nlist\nshow acc1\nquit\n' | dotnet run
//
// פקודות:
//   open <id> <owner>      deposit <id> <amount>     withdraw <id> <amount>
//   transfer <from> <to> <amount>     show <id>     list     quit
using Day1.Lab4;

int processed = 0, failed = 0;

// FIX 7: using — הקובץ נסגר ומתרוקן לדיסק גם אם התוכנית נגמרת בחריגה
using (var log = new TransactionLog("transactions.log"))
{
    var bank = new Bank(log);
    Console.WriteLine("Bank ready. Type 'help' for commands.");

    while (true)
    {
        Console.Write("bank> ");
        var line = Console.ReadLine();
        if (line is null || line.Trim() == "quit") break;   // EOF או quit
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
                    RequireArgs(parts, 2);
                    var acc = bank.Open(parts[1], parts[2]);
                    Console.WriteLine($"opened {acc.Id} for {acc.Owner}");
                    break;
                case "deposit":
                    RequireArgs(parts, 2);
                    bank.Deposit(parts[1], ParseAmount(parts[2]));   // FIX 8: TryParse בפנים
                    Console.WriteLine($"ok, balance = {bank.Get(parts[1]).Balance:N2}");
                    break;
                case "withdraw":
                    RequireArgs(parts, 2);
                    bank.Withdraw(parts[1], ParseAmount(parts[2]));
                    Console.WriteLine($"ok, balance = {bank.Get(parts[1]).Balance:N2}");
                    break;
                case "transfer":
                    RequireArgs(parts, 3);
                    bank.Transfer(parts[1], parts[2], ParseAmount(parts[3]));
                    Console.WriteLine("ok");
                    break;
                case "show":
                    RequireArgs(parts, 1);
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
        }
        catch (InsufficientFundsException ex)
        {
            // catch ספציפי לפני הכללי — יש לנו נתונים נוספים להציג
            Console.WriteLine($"error: {ex.Message} (short by {ex.Shortfall:N2})");
            failed++;
            continue;   // ה-finally עדיין ירוץ
        }
        catch (BankException ex)
        {
            Console.WriteLine($"error: {ex.Message}");
            failed++;
            continue;
        }
        catch (ArgumentException ex)   // FIX 9: חסר פרמטר / קלט לא מספרי — הודעה ידידותית במקום קריסה
        {
            Console.WriteLine($"usage error: {ex.Message}");
            failed++;
            continue;
        }
        finally
        {
            processed++;   // נספר תמיד — הצלחה או כישלון
        }
    }

    Console.WriteLine($"processed={processed}, failed={failed}, log lines={log.Lines}");
}   // Dispose() כאן

Console.WriteLine($"log written to {Path.GetFullPath("transactions.log")}");

// ---- עזרים ----
static void RequireArgs(string[] parts, int count)
{
    if (parts.Length - 1 < count)
        throw new ArgumentException($"'{parts[0]}' needs {count} argument(s)");
}

static decimal ParseAmount(string text)
{
    // TryParse pattern: קלט שגוי הוא מצב צפוי — לא חריגה של FormatException
    if (!decimal.TryParse(text, System.Globalization.NumberStyles.Number,
                          System.Globalization.CultureInfo.InvariantCulture, out var amount))
        throw new ArgumentException($"'{text}' is not a number");
    return amount;
}
