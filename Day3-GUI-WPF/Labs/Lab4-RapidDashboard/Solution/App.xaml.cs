using System.Windows;

namespace Day3.Lab4.Solution;

public partial class App : Application
{
    /// <summary>מחליף את מילון ה-theme (index 0). כל DynamicResource בחלונות מתעדכן מיד.</summary>
    public static void ApplyTheme(string name)
    {
        var dict = new ResourceDictionary { Source = new Uri($"Themes/{name}.xaml", UriKind.Relative) };
        Current.Resources.MergedDictionaries[0] = dict;
    }
}
