namespace Day3.Demo.WinForms;

/// <summary>
/// טופס WinForms שנבנה כולו בקוד (בלי קובץ Designer) כדי להראות את המודל:
/// פקדים = אובייקטים, פריסה = מאפיינים (Dock/Anchor/TableLayoutPanel), אירועים = delegates.
/// ב-Visual Studio, המעצב הגרפי מייצר בדיוק קוד כזה לתוך MainForm.Designer.cs.
/// </summary>
public class MainForm : Form
{
    private readonly TextBox _nameBox = new() { PlaceholderText = "Task name", Dock = DockStyle.Fill };
    private readonly Button _addButton = new() { Text = "Add", Dock = DockStyle.Fill };
    private readonly Button _removeButton = new() { Text = "Remove", Dock = DockStyle.Fill, Enabled = false };
    private readonly ListBox _list = new() { Dock = DockStyle.Fill };
    private readonly StatusStrip _status = new();
    private readonly ToolStripStatusLabel _statusLabel = new("0 tasks");

    public MainForm()
    {
        Text = "Day 3 — WinForms demo";
        Width = 480; Height = 360; MinimumSize = new Size(320, 240);
        StartPosition = FormStartPosition.CenterScreen;

        // TableLayoutPanel = ה-Grid של WinForms
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 3, RowCount = 2, Padding = new Padding(10) };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        layout.Controls.Add(_nameBox, 0, 0);
        layout.Controls.Add(_addButton, 1, 0);
        layout.Controls.Add(_removeButton, 2, 0);
        layout.Controls.Add(_list, 0, 1);
        layout.SetColumnSpan(_list, 3);

        _status.Items.Add(_statusLabel);
        Controls.Add(layout);
        Controls.Add(_status);
        AcceptButton = _addButton;   // Enter מפעיל Add

        // אירועים: אותו רעיון כמו ב-WPF, בלי routed events
        _addButton.Click += (_, _) => AddTask();
        _removeButton.Click += (_, _) => RemoveSelected();
        _list.SelectedIndexChanged += (_, _) => _removeButton.Enabled = _list.SelectedIndex >= 0;
        _nameBox.TextChanged += (_, _) => _addButton.Enabled = _nameBox.Text.Trim().Length > 0;
        _addButton.Enabled = false;
    }

    private void AddTask()
    {
        var name = _nameBox.Text.Trim();
        if (name.Length == 0) return;
        _list.Items.Add(name);
        _nameBox.Clear();
        _nameBox.Focus();
        UpdateStatus();
    }

    private void RemoveSelected()
    {
        if (_list.SelectedIndex < 0) return;
        var confirm = MessageBox.Show(this, $"Remove \"{_list.SelectedItem}\"?", "Confirm",
                                      MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (confirm != DialogResult.Yes) return;
        _list.Items.RemoveAt(_list.SelectedIndex);
        UpdateStatus();
    }

    private void UpdateStatus() => _statusLabel.Text = $"{_list.Items.Count} tasks";
}
