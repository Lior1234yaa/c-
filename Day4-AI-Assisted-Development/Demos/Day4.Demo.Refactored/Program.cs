// Day4.Demo.Refactored — הגרסה הנקייה של Day4.Demo.LegacyMess.
// אותו פלט בדיוק (golden master), אבל: שכבות, ממשקים, DI, decimal לכסף, ובדיקות.
using Day4.Demo.Refactored.Infrastructure;
using Day4.Demo.Refactored.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton<IOrderSource, EmbeddedCsvOrderSource>();
builder.Services.AddSingleton<IDiscountPolicy, DiscountPolicy>();
builder.Services.AddSingleton<IShippingCalculator, ShippingCalculator>();
builder.Services.AddSingleton<IVatCalculator, IsraelVatCalculator>();
builder.Services.AddSingleton<OrderPricer>();
builder.Services.AddSingleton<ReportFormatter>();
builder.Services.AddSingleton<ReportRunner>();

// הדמו הזה לא צריך host שרץ ברקע — רק את ה-container.
using var host = builder.Build();

var runner = host.Services.GetRequiredService<ReportRunner>();
runner.Run(Console.Out);
