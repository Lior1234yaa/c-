using System.Globalization;

// TODO (Lab 2): refactor בעזרת AI לשירותים + DI + בדיקות. הפלט חייב להישאר זהה ל-expected-output.txt.
public class SubscriptionBiller
{
    public double grand = 0;
    public double totalTax = 0;
    public double totalDisc = 0;
    public int n = 0;
    public Dictionary<string, double> byPlan = new Dictionary<string, double>();

    public void Run()
    {
        // id,customer,plan,addons,daysActive,months,coupon
        // plan: BASIC/PRO/TEAM ; addons: מספר תוספים ; daysActive: ימים פעילים מתוך 30 ; months: ותק בחודשים
        string data = @"S-01,Dana Levi,PRO,2,30,14,NONE
S-02,Yossi Cohen,BASIC,0,30,2,NONE
S-03,ACME Ltd,TEAM,5,30,26,LOYAL
S-04,Noa Bar,PRO,1,15,1,WELCOME
S-05,Lior Katz,BASIC,3,30,7,NONE
S-06,Beta Inc,TEAM,0,10,3,NONE
S-07,Dana Levi,BASIC,1,30,14,LOYAL
S-08,Gal Peretz,PRO,0,30,0,WELCOME";

        Console.WriteLine("=== MONTHLY BILLING ===");
        Console.WriteLine("");

        var lines = data.Split('\n');
        for (int i = 0; i < lines.Length; i++)
        {
            var l = lines[i].Trim();
            if (l == "") continue;
            var p = l.Split(',');
            string id = p[0];
            string cust = p[1];
            string plan = p[2];
            int addons = int.Parse(p[3]);
            int days = int.Parse(p[4]);
            int months = int.Parse(p[5]);
            string coupon = p[6];

            // מחיר בסיס — מספרי קסם
            double basePrice = 0;
            if (plan == "BASIC")
            {
                basePrice = 29.9;
            }
            else if (plan == "PRO")
            {
                basePrice = 79.0;
            }
            else if (plan == "TEAM")
            {
                basePrice = 199.0;
            }
            else
            {
                basePrice = 0;
            }

            // תוספים — מחיר שונה לכל תוכנית, שוב מספרי קסם
            double addonPrice = 0;
            if (plan == "BASIC")
            {
                addonPrice = addons * 9.9;
            }
            else if (plan == "PRO")
            {
                addonPrice = addons * 14.9;
            }
            else if (plan == "TEAM")
            {
                if (addons > 3)
                {
                    addonPrice = 3 * 19.9 + (addons - 3) * 14.9;
                }
                else
                {
                    addonPrice = addons * 19.9;
                }
            }

            double sub = basePrice + addonPrice;

            // חלק יחסי לפי ימים פעילים
            double prorated = sub;
            if (days < 30)
            {
                prorated = sub * days / 30.0;
            }

            // הנחות — ותק וקופונים, לוגיקה כפולה
            double disc = 0;
            if (months >= 24)
            {
                disc = prorated * 0.15;
            }
            else if (months >= 12)
            {
                disc = prorated * 0.10;
            }
            else
            {
                disc = 0;
            }
            if (coupon == "WELCOME")
            {
                if (months < 1)
                {
                    disc = disc + prorated * 0.5;
                }
                else
                {
                    disc = disc + prorated * 0.2;
                }
            }
            else if (coupon == "LOYAL")
            {
                if (months >= 24)
                {
                    disc = disc + 10;
                }
                else if (months >= 12)
                {
                    disc = disc + 5;
                }
            }
            if (disc > prorated) disc = prorated;

            double afterDisc = prorated - disc;
            double tax = afterDisc * 0.18;
            double total = afterDisc + tax;

            n = n + 1;
            grand = grand + total;
            totalTax = totalTax + tax;
            totalDisc = totalDisc + disc;
            if (byPlan.ContainsKey(plan)) byPlan[plan] = byPlan[plan] + total; else byPlan[plan] = total;

            // עיצוב — משוכפל
            string s = "";
            s = s + id + " | " + cust.PadRight(12) + " | " + plan.PadRight(5) + " | ";
            s = s + "addons " + addons.ToString().PadLeft(2) + " | ";
            s = s + "days " + days.ToString().PadLeft(2) + " | ";
            s = s + "sub " + sub.ToString("0.00", CultureInfo.InvariantCulture).PadLeft(7) + " | ";
            s = s + "pror " + prorated.ToString("0.00", CultureInfo.InvariantCulture).PadLeft(7) + " | ";
            s = s + "disc " + disc.ToString("0.00", CultureInfo.InvariantCulture).PadLeft(6) + " | ";
            s = s + "tax " + tax.ToString("0.00", CultureInfo.InvariantCulture).PadLeft(6) + " | ";
            s = s + "TOTAL " + total.ToString("0.00", CultureInfo.InvariantCulture).PadLeft(8);
            if (coupon != "NONE")
            {
                s = s + " [" + coupon + "]";
            }
            if (days < 30)
            {
                s = s + " (partial)";
            }
            Console.WriteLine(s);
        }

        Console.WriteLine("");
        Console.WriteLine("--- Totals ---");
        string t = "";
        t = t + "Subscriptions : " + n.ToString().PadLeft(4) + "\n";
        t = t + "Discounts     : " + totalDisc.ToString("0.00", CultureInfo.InvariantCulture).PadLeft(9) + "\n";
        t = t + "Tax           : " + totalTax.ToString("0.00", CultureInfo.InvariantCulture).PadLeft(9) + "\n";
        t = t + "Grand total   : " + grand.ToString("0.00", CultureInfo.InvariantCulture).PadLeft(9) + "\n";
        Console.Write(t);

        Console.WriteLine("");
        Console.WriteLine("--- By plan ---");
        var keys = new List<string>(byPlan.Keys);
        keys.Sort(StringComparer.Ordinal);
        for (int i = 0; i < keys.Count; i++)
        {
            string s2 = "";
            s2 = s2 + keys[i].PadRight(6) + " : " + byPlan[keys[i]].ToString("0.00", CultureInfo.InvariantCulture).PadLeft(9);
            double pct = byPlan[keys[i]] / grand * 100;
            s2 = s2 + "  (" + pct.ToString("0.0", CultureInfo.InvariantCulture) + "%)";
            Console.WriteLine(s2);
        }
    }
}
