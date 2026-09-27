using System.Windows;
using Day4.Demo.DiHostWpf.Services;
using Day4.Demo.DiHostWpf.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Day4.Demo.DiHostWpf;

/// <summary>
/// נקודת הכניסה: מקימה Generic Host (DI + Configuration + Logging) ופותחת את החלון הראשי מתוך ה-container.
/// </summary>
public partial class App : Application
{
    private readonly IHost _host;

    public App()
    {
        _host = Host.CreateDefaultBuilder()             // קורא appsettings.json, env vars, ומגדיר logging
            .ConfigureServices((context, services) =>
            {
                services.Configure<AppOptions>(context.Configuration.GetSection("App"));

                services.AddSingleton<IClock, SystemClock>();
                services.AddSingleton<IOrderRepository, InMemoryOrderRepository>();
                services.AddTransient<OrderService>();

                services.AddTransient<MainViewModel>();
                services.AddTransient<MainWindow>();
            })
            .Build();
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        await _host.StartAsync();

        var logger = _host.Services.GetRequiredService<ILogger<App>>();
        logger.LogInformation("Application starting at {Time}", DateTimeOffset.Now);

        var window = _host.Services.GetRequiredService<MainWindow>();
        window.Show();
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        await _host.StopAsync(TimeSpan.FromSeconds(3));
        _host.Dispose();
        base.OnExit(e);
    }
}
