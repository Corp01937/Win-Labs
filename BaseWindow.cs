using System.Windows;

namespace Win_Labs
{
    public class BaseWindow : Window
    {
        protected AppSettings settings;

        public BaseWindow()
        {
            // Load settings
            settings = AppSettingsManager.Settings;
            Console.WriteLine($"Theme: {settings.Theme}, Language: {settings.Language}");

            // Apply theme
            ThemeManager.ApplyTheme(settings.Theme);
        }
    }
}
