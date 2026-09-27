namespace Day4.Lab3.Snippets;

/// <summary>Saves a report into the exports folder using a user-provided file name.</summary>
public class FileExport
{
    private readonly string _root;

    public FileExport(string root) => _root = root;

    public string Save(string userFileName, string content)
    {
        Directory.CreateDirectory(_root);
        var path = Path.Combine(_root, userFileName);
        File.WriteAllText(path, content);
        return path;
    }
}
