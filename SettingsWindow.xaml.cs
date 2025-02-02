using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Win_Labs
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : BaseWindow
    {
        public SettingsWindow()
        {
            InitializeComponent();

            // Load current settings
            var settings = AppSettingsManager.Settings;

            // Set Theme ComboBox
            ThemeComboBox.SelectedItem = ThemeComboBox.Items.OfType<ComboBoxItem>()
                .FirstOrDefault(item => item.Content.ToString() == settings.Theme);

            // Set Language ComboBox
            LanguageComboBox.SelectedItem = LanguageComboBox.Items.OfType<ComboBoxItem>()
                .FirstOrDefault(item => item.Content.ToString() == settings.Language);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Update settings
            var settings = AppSettingsManager.Settings;
            settings.Theme = ((ComboBoxItem)ThemeComboBox.SelectedItem).Content.ToString();
            settings.Language = ((ComboBoxItem)LanguageComboBox.SelectedItem).Content.ToString();

            // Save settings to file
            AppSettingsManager.SaveSettings();

            // Apply the selected theme
            ThemeManager.ApplyTheme(settings.Theme);

            // Close the settings window
            this.Close();
        }
    }
}
