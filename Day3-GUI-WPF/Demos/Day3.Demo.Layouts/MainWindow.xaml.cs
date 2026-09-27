using System.Windows;
using System.Windows.Controls;

namespace Day3.Demo.Layouts;

public partial class MainWindow : Window
{
    private const double NarrowThreshold = 600;
    private bool _narrow;

    public MainWindow()
    {
        InitializeComponent();
        PeopleGrid.ItemsSource = SampleData.People();
    }

    private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        SizeText.Text = $"Size: {e.NewSize.Width:F0} x {e.NewSize.Height:F0}";

        var narrow = e.NewSize.Width < NarrowThreshold;
        if (narrow == _narrow) return;   // שינוי פריסה רק כשחוצים את הסף
        _narrow = narrow;
        ModeText.Text = narrow ? "Mode: narrow" : "Mode: wide";
        ApplyLayout(narrow);
    }

    /// <summary>פריסה אדפטיבית: בחלון צר הפאנל הצדדי עובר לשורה מעל התוכן.</summary>
    private void ApplyLayout(bool narrow)
    {
        if (narrow)
        {
            SideCol.Width = new GridLength(0);
            Grid.SetColumn(SidePanel, 1); Grid.SetRow(SidePanel, 0); Grid.SetRowSpan(SidePanel, 1);
            Grid.SetRow(MainPanel, 1); Grid.SetRowSpan(MainPanel, 1);
        }
        else
        {
            SideCol.Width = new GridLength(220);
            Grid.SetColumn(SidePanel, 0); Grid.SetRow(SidePanel, 0); Grid.SetRowSpan(SidePanel, 2);
            Grid.SetRow(MainPanel, 0); Grid.SetRowSpan(MainPanel, 2);
        }
    }

    private void Exit_Click(object sender, RoutedEventArgs e) => Close();

    private void About_Click(object sender, RoutedEventArgs e) =>
        MessageBox.Show("Day 3 — Layouts demo", "About", MessageBoxButton.OK, MessageBoxImage.Information);
}
