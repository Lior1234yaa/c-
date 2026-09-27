namespace Day3.Demo.WinForms;

static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();   // DPI, visual styles, default font
        Application.Run(new MainForm());
    }
}
