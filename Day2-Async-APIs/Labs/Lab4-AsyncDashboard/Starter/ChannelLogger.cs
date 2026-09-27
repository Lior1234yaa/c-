using System.Threading.Channels;

namespace Day2.Lab4;

/// <summary>לוגר שלא חוסם: כותבים ל-Channel, תהליכון רקע אחד מדפיס ל-stderr.</summary>
public sealed class ChannelLogger : IAsyncDisposable
{
    // TODO (שלב 6): Channel<string> + Task רקע שקורא ReadAllAsync וכותב ל-Console.Error

    public void Log(string message)
    {
        // TODO (שלב 6): TryWrite עם חותמת זמן ו-thread id. בינתיים — הדפסה ישירה:
        Console.Error.WriteLine($"{DateTime.Now:HH:mm:ss.fff} [T{Environment.CurrentManagedThreadId,2}] {message}");
    }

    public ValueTask DisposeAsync()
    {
        // TODO (שלב 6): Complete() ואז await על ה-Task
        return ValueTask.CompletedTask;
    }
}
