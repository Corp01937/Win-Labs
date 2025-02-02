using Newtonsoft.Json;
using System.IO;
using System;

namespace Win_Labs
{
    public static class AppSettingsManager
    {
        private static AppSettings _settings;
        private static readonly string SettingsFilePath = "settings.json";

        public static AppSettings Settings
        {
            get
            {
                if (_settings == null)
                {
                    LoadSettings();
                }
                return _settings;
            }
        }

        private static void LoadSettings()
        {
            if (!File.Exists(SettingsFilePath))
            {
                // Create default settings if the file doesn't exist
                _settings = new AppSettings {};
                SaveSettings();
                Log.Info("Default settings created and saved.");
            }
            else
            {
                var json = File.ReadAllText(SettingsFilePath);
                _settings = JsonConvert.DeserializeObject<AppSettings>(json);
                Log.Info("Settings loaded from file: " + SettingsFilePath);
            }
        }

        public static void SaveSettings()
        {
            var json = JsonConvert.SerializeObject(_settings, Formatting.Indented);
            File.WriteAllText(SettingsFilePath, json);
            Log.Info("Settings saved to file.");
        }
    }
}
