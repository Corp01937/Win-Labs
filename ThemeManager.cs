using System;
using System.Windows;

namespace Win_Labs
{
    public static class ThemeManager
    {
        public static void ApplyTheme(string themeName)
        {
            // Clear existing merged dictionaries
            Application.Current.Resources.MergedDictionaries.Clear();

            // Load the selected theme
            var themeDict = new ResourceDictionary
            {
                Source = new Uri($"/Themes/{themeName}Theme.xaml", UriKind.Relative)
            };
            Application.Current.Resources.MergedDictionaries.Add(themeDict);
        }
    }
}
