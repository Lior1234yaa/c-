namespace Day2.Lab4.Sources;

public interface ISource<T>
{
    string Name { get; }
    Task<T> FetchAsync(CancellationToken ct);
}
