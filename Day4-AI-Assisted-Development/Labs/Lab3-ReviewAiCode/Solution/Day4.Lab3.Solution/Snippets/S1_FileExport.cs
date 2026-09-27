namespace Day4.Lab3.Snippets;

/// <summary>Saves a report into the exports folder. Rejects any name that escapes the root (path traversal).</summary>
public sealed class FileExport
{
    private readonly string _root;

    public FileExport(string root) => _root = Path.GetFullPath(root);

    public string Save(string userFileName, string content)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userFileName);

        // רק שם קובץ — בלי תיקיות, בלי "..", בלי תווים אסורים
        var name = Path.GetFileName(userFileName);
        if (name != userFileName || name.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            throw new ArgumentException($"Invalid file name '{userFileName}'.", nameof(userFileName));

        var full = Path.GetFullPath(Path.Combine(_root, name));
        if (!full.StartsWith(_root + Path.DirectorySeparatorChar, StringComparison.Ordinal))
            throw new UnauthorizedAccessException("Path escapes the export root.");

        Directory.CreateDirectory(_root);
        File.WriteAllText(full, content);
        return full;
    }
}
