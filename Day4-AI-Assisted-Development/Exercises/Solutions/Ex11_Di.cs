using Microsoft.Extensions.DependencyInjection;

namespace Day4.Exercises.Solutions;

/// <summary>תרגיל 11: מ-new בתוך המתודה → ממשקים + constructor injection + רישום ב-ServiceCollection + fake בבדיקה.</summary>
public static class Ex11_Di
{
    public interface IClock { DateTimeOffset Now { get; } }
    public interface INotifier { void Send(string user, string message); }

    public sealed class SystemClock : IClock { public DateTimeOffset Now => DateTimeOffset.Now; }

    public sealed class ConsoleNotifier : INotifier
    {
        public void Send(string user, string message) => Console.WriteLine($"[email→{user}] {message}");
    }

    public sealed class ReminderService(IClock clock, INotifier notifier)
    {
        public string Remind(string user)
        {
            notifier.Send(user, $"Reminder at {clock.Now:HH:mm}");
            return "sent";
        }
    }

    // "בדיקה" עם fakes — בפרויקט אמיתי זה [Fact] ב-xUnit
    private sealed class FixedClock(DateTimeOffset now) : IClock { public DateTimeOffset Now => now; }
    private sealed class RecordingNotifier : INotifier
    {
        public List<string> Sent { get; } = [];
        public void Send(string user, string message) => Sent.Add($"{user}:{message}");
    }

    public static void Run()
    {
        var services = new ServiceCollection()
            .AddSingleton<IClock, SystemClock>()
            .AddSingleton<INotifier, ConsoleNotifier>()
            .AddTransient<ReminderService>()
            .BuildServiceProvider();

        services.GetRequiredService<ReminderService>().Remind("dana");

        var fakeNotifier = new RecordingNotifier();
        var svc = new ReminderService(new FixedClock(new DateTimeOffset(2025, 1, 1, 9, 30, 0, TimeSpan.Zero)), fakeNotifier);
        svc.Remind("test");
        Console.WriteLine(fakeNotifier.Sent.Single() == "test:Reminder at 09:30" ? "fake test PASSED" : "fake test FAILED");
    }
}
