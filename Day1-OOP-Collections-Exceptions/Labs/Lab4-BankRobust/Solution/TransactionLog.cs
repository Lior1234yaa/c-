namespace Day1.Lab4;

/// <summary>יומן פעולות שנכתב לקובץ. משאב שצריך לשחרר (IDisposable).</summary>
public class TransactionLog : IDisposable
{
    private readonly StreamWriter _writer;
    public string Path { get; }
    public int Lines { get; private set; }

    public TransactionLog(string path)
    {
        Path = path;
        _writer = new StreamWriter(path, append: false);
    }

    public void Write(string line)
    {
        _writer.WriteLine($"{DateTime.Now:HH:mm:ss} {line}");
        Lines++;
    }

    public void Dispose()
    {
        _writer.Dispose();   // Flush + סגירת הקובץ
    }
}
