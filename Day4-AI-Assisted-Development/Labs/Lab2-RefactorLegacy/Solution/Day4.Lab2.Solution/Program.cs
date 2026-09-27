// Day4.Lab2.Solution — composition root בלבד. הפלט זהה ל-expected-output.txt של ה-Starter.
using Day4.Lab2.Infrastructure;
using Day4.Lab2.Services;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection()
    .AddSingleton<ISubscriptionSource, EmbeddedCsvSubscriptionSource>()
    .AddSingleton<IPricingService, PricingService>()
    .AddSingleton<ReportFormatter>()
    .AddSingleton<BillingReport>()
    .BuildServiceProvider();

services.GetRequiredService<BillingReport>().Write(Console.Out);
