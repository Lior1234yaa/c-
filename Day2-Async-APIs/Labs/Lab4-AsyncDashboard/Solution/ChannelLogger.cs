using System.Threading.Channels;

namespace Day2.Lab4;

/// <summary>לוגר שלא חוסם: Log כותב ל-Channel (TryWrite), תהליכון רקע יחיד מדפיס ל-stderr בסדר.</summary>
public sealed class ChannelLogger : IAsyncDisposable
{
    private readonly Channel<string> _channel = Channel.CreateUnbounded<string>(new UnboundedChannelOptions { SingleReader = true });
    private readonly Task _pump;
    private readonly bool _enabled;

    public ChannelLogger(bool enabled = true)
    {
        _enabled = enabled;
        _pump = Task.Run(PumpAsync);
    }

    public void Log(string message)
    {
        if (!_enabled) return;
        // TryWrite על unbounded channel תמיד מצליח ולא חוסם — הקורא ממשיך מיד
        _channel.Writer.TryWrite($"{DateTime.Now:HH:mm:ss.fff} [T{Environment.CurrentManagedThreadId,2}] {message}");
    }

    private async Task PumpAsync()
    {
        await foreach (var line in _channel.Reader.ReadAllAsync())
            Console.Error.WriteLine(line);            // stderr — לא מתערבב עם הלוח ב-stdout
    }

    public async ValueTask DisposeAsync()
    {
        _channel.Writer.Complete();                   // אין עוד הודעות
        await _pump;                                  // מחכים שהכל יודפס
    }
}
