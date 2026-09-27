namespace Day4.Demo.DiHostWpf.Services;

/// <summary>הפשטה של הזמן — כדי שבדיקות יוכלו "להקפיא" את השעון.</summary>
public interface IClock
{
    DateTimeOffset UtcNow { get; }
}

public sealed class SystemClock : IClock
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
