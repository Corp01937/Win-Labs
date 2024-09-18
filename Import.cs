using System;
using System.Windows;

namespace Win_Labs
{
    internal class import
    {
        private static string playlistImportFilePath = StartupWindow.playlistImportFilePath;
        public static string playlistFolderPath = StartupWindow.playlistFolderPath;

        public static void openZIP()
        {
            Log.log("Opening file: " + playlistImportFilePath);
            var zipFile = playlistImportFilePath;
            Log.log($"Import File Path: {zipFile}");
            try
            {
                Log.log($"Creating playlist at {playlistFolderPath}");
                System.IO.Compression.ZipFile.ExtractToDirectory(zipFile, playlistFolderPath);
                Log.log($"{playlistFolderPath} Created.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not open file {zipFile}. Please check location."
                    , "File Opening Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

    }
}
