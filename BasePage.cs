using System.Windows.Controls;

namespace Win_Labs
{
    public class BasePage : Page
    {
        protected AppSettings settings;

        public BasePage()
        {
            // Load settings
            settings = AppSettingsManager.Settings;
            Console.WriteLine($"Theme: {settings.Theme}, Language: {settings.Language}");

            // Apply theme
            ThemeManager.ApplyTheme(settings.Theme);
        }
    }
}
