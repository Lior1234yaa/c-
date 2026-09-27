using System.Globalization;
using System.Text;

namespace Day4.Demo.LegacyMess;

// מחלקת "אלוהים": טוענת, מחשבת, מעצבת ומדפיסה — הכול במקום אחד.
public class OrderProcessor
{
    public List<string[]> data = new List<string[]>();
    public string log = "";
    public int cnt = 0;
    public double total = 0;
    public double totalDisc = 0;
    public double totalShip = 0;
    public double totalVat = 0;
    public Dictionary<string, double> byCust = new Dictionary<string, double>();
    public Dictionary<string, int> byStatus = new Dictionary<string, int>();

    public void Run()
    {
        // "טעינת" נתונים — CSV מוטבע. id,customer,type,qty,price,country,status,date
        string csv = @"1001,Dana Levi,VIP,3,120.5,IL,PAID,2025-03-01
1002,Yossi Cohen,REG,12,15,IL,PAID,2025-03-01
1003,ACME Ltd,BIZ,40,9.99,US,PENDING,2025-03-02
1004,Dana Levi,VIP,1,999,IL,PAID,2025-03-02
1005,Noa Bar,REG,2,49.9,DE,CANCELLED,2025-03-03
1006,Yossi Cohen,REG,1,15,IL,PENDING,2025-03-03
1007,ACME Ltd,BIZ,100,4.5,US,PAID,2025-03-04
1008,Lior Katz,NEW,5,20,IL,PAID,2025-03-04
1009,Noa Bar,REG,7,33.3,DE,PAID,2025-03-05
1010,Lior Katz,NEW,1,250,FR,PAID,2025-03-05";

        foreach (var line in csv.Split('\n'))
        {
            var l = line.Trim();
            if (l == "") continue;
            var parts = l.Split(',');
            data.Add(parts);
        }

        Console.WriteLine("=== ORDER REPORT ===");
        Console.WriteLine("");

        for (int i = 0; i < data.Count; i++)
        {
            var r = data[i];
            string id = r[0];
            string cust = r[1];
            string type = r[2];
            int qty = int.Parse(r[3]);
            double price = double.Parse(r[4], CultureInfo.InvariantCulture);
            string country = r[5];
            string status = r[6];
            string date = r[7];

            if (status == "CANCELLED")
            {
                if (byStatus.ContainsKey(status)) byStatus[status] = byStatus[status] + 1; else byStatus[status] = 1;
                log = log + "skip " + id + "\n";
                Console.WriteLine("Order " + id + " (" + cust + ") - CANCELLED, skipped");
                continue;
            }

            double sub = qty * price;
            double disc = 0;

            // הנחות: מספרי קסם, ענפים כפולים
            if (type == "VIP")
            {
                if (sub > 1000)
                {
                    disc = sub * 0.15;
                }
                else if (sub > 250)
                {
                    disc = sub * 0.12;
                }
                else
                {
                    disc = sub * 0.10;
                }
            }
            else if (type == "BIZ")
            {
                if (qty >= 50)
                {
                    disc = sub * 0.20;
                }
                else if (qty >= 10)
                {
                    disc = sub * 0.10;
                }
                else
                {
                    disc = 0;
                }
            }
            else if (type == "REG")
            {
                if (sub > 250)
                {
                    disc = sub * 0.05;
                }
                else if (qty >= 10)
                {
                    disc = sub * 0.05;
                }
                else
                {
                    disc = 0;
                }
            }
            else if (type == "NEW")
            {
                disc = sub * 0.07;
                if (disc > 30) disc = 30;
            }
            else
            {
                disc = 0;
            }

            double afterDisc = sub - disc;

            // משלוח: עוד מספרי קסם וכפילות
            double ship = 0;
            if (country == "IL")
            {
                if (afterDisc >= 200)
                {
                    ship = 0;
                }
                else
                {
                    ship = 25;
                }
            }
            else if (country == "US")
            {
                if (afterDisc >= 500)
                {
                    ship = 0;
                }
                else
                {
                    ship = 60;
                }
            }
            else if (country == "DE" || country == "FR")
            {
                if (afterDisc >= 400)
                {
                    ship = 0;
                }
                else
                {
                    ship = 45;
                }
            }
            else
            {
                ship = 80;
            }

            // מע"מ: רק בישראל
            double vat = 0;
            if (country == "IL")
            {
                vat = (afterDisc + ship) * 0.18;
            }

            double grand = afterDisc + ship + vat;

            // הצטברות
            cnt = cnt + 1;
            total = total + grand;
            totalDisc = totalDisc + disc;
            totalShip = totalShip + ship;
            totalVat = totalVat + vat;
            if (byCust.ContainsKey(cust)) byCust[cust] = byCust[cust] + grand; else byCust[cust] = grand;
            if (byStatus.ContainsKey(status)) byStatus[status] = byStatus[status] + 1; else byStatus[status] = 1;

            // עיצוב — מועתק שלוש פעמים בהמשך
            string s = "";
            s = s + "Order " + id + " | " + cust.PadRight(12) + " | " + type.PadRight(3) + " | ";
            s = s + qty.ToString().PadLeft(3) + " x " + price.ToString("0.00", CultureInfo.InvariantCulture).PadLeft(7) + " | ";
            s = s + "sub " + sub.ToString("0.00", CultureInfo.InvariantCulture).PadLeft(8) + " | ";
            s = s + "disc " + disc.ToString("0.00", CultureInfo.InvariantCulture).PadLeft(7) + " | ";
            s = s + "ship " + ship.ToString("0.00", CultureInfo.InvariantCulture).PadLeft(6) + " | ";
            s = s + "vat " + vat.ToString("0.00", CultureInfo.InvariantCulture).PadLeft(7) + " | ";
            s = s + "TOTAL " + grand.ToString("0.00", CultureInfo.InvariantCulture).PadLeft(9);
            if (status == "PENDING")
            {
                s = s + " (pending)";
            }
            Console.WriteLine(s);
            log = log + "ok " + id + "\n";

            // "בדיקת" תקינות אחרי החישוב, בלי טיפול אמיתי
            if (grand < 0)
            {
                Console.WriteLine("NEGATIVE?!");
            }
            if (date.Length != 10)
            {
                Console.WriteLine("BAD DATE " + date);
            }
        }

        Console.WriteLine("");
        Console.WriteLine("--- Summary ---");
        string s2 = "";
        s2 = s2 + "Orders processed: " + cnt.ToString().PadLeft(4) + "\n";
        s2 = s2 + "Total discounts : " + totalDisc.ToString("0.00", CultureInfo.InvariantCulture).PadLeft(10) + "\n";
        s2 = s2 + "Total shipping  : " + totalShip.ToString("0.00", CultureInfo.InvariantCulture).PadLeft(10) + "\n";
        s2 = s2 + "Total VAT       : " + totalVat.ToString("0.00", CultureInfo.InvariantCulture).PadLeft(10) + "\n";
        s2 = s2 + "Grand total     : " + total.ToString("0.00", CultureInfo.InvariantCulture).PadLeft(10) + "\n";
        Console.Write(s2);

        Console.WriteLine("");
        Console.WriteLine("--- By customer ---");
        var keys = new List<string>(byCust.Keys);
        keys.Sort(StringComparer.Ordinal);
        for (int i = 0; i < keys.Count; i++)
        {
            string s3 = "";
            s3 = s3 + keys[i].PadRight(12) + " : " + byCust[keys[i]].ToString("0.00", CultureInfo.InvariantCulture).PadLeft(10);
            if (byCust[keys[i]] > 1000)
            {
                s3 = s3 + "  *TOP*";
            }
            Console.WriteLine(s3);
        }

        Console.WriteLine("");
        Console.WriteLine("--- By status ---");
        var keys2 = new List<string>(byStatus.Keys);
        keys2.Sort(StringComparer.Ordinal);
        for (int i = 0; i < keys2.Count; i++)
        {
            string s4 = "";
            s4 = s4 + keys2[i].PadRight(12) + " : " + byStatus[keys2[i]].ToString().PadLeft(4);
            Console.WriteLine(s4);
        }

        // "לוג" שלא נשמר לשום מקום — רק נספר
        var sb = new StringBuilder();
        sb.Append(log);
        Console.WriteLine("");
        Console.WriteLine("log lines: " + sb.ToString().Split('\n').Length.ToString());
    }
}
