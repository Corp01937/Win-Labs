using Microsoft.VisualBasic.Logging;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;

namespace Win_Labs
{
    public class PlaylistManager
    {
        public static string playlistFolderPath;
        private string playlistImportFilePath;
        private Window _startupWindow;

        public PlaylistManager(Window startupWindow)
        {
            _startupWindow = startupWindow;
            Log.Info($"PlaylistManager initialized with window: {_startupWindow.GetType().Name}");
        }
        private void OpenMainWindow(string playlistFolderPath)
        {
            var mainWindow = new MainWindow(playlistFolderPath);
            Log.Info("MainWindow created and initialized.");
            CueManager.MarkStartupAsFinished();
            Log.Info("Startup process completed.");
            mainWindow.Show();
            if (_startupWindow is StartupWindow startupWindow)
            {
                startupWindow.StartupWindowClosing = true;
            }
            _startupWindow.Close();
        }

        public void CreateNewPlaylist()
        {
            string selectedPath = OpenFolderDialog("Select a folder to create a playlist in.");
            if (!string.IsNullOrEmpty(selectedPath))
            {
                try
                {
                    Log.Info("Show loading window");
                    var loadingWindow = new LoadingWindow();
                    loadingWindow.Show();

                    playlistFolderPath = selectedPath;
                    Log.Info($"New playlist folder selected: {playlistFolderPath}");
                    OpenMainWindow(playlistFolderPath);

                    Log.Info("Close loading window");
                    loadingWindow.Close();
                }
                catch (Exception ex)
                {
                    Log.Error($"Failed to create new playlist: {ex.Message}");
                    System.Windows.MessageBox.Show($"Error creating new playlist: {ex.Message}", "Create Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public void OpenExistingPlaylist()
        {
            var folderDialog = new FolderBrowserDialog
            {
                Description = "Select a playlist folder."
            };

            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                string folderPath = folderDialog.SelectedPath;
                Log.Info($"Opening existing playlist folder: {folderPath}");
                if (!Directory.EnumerateFiles(folderPath, "cue_*.json").Any())
                {
                    Log.Error("No cue files found in the selected folder: " + folderPath);
                    System.Windows.MessageBox.Show("The selected folder does not contain any cue files.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                try
                {
                    Log.Info("Show loading window");
                    var loadingWindow = new LoadingWindow();
                    loadingWindow.Show();

                    Log.Info("Open main window with the existing playlist");
                    OpenMainWindow(folderPath);

                    Log.Info("Close loading window");
                    loadingWindow.Close();
                }
                catch (Exception ex)
                {
                    Log.Error($"Failed to open existing playlist: {ex.Message}");
                    System.Windows.MessageBox.Show($"Error opening existing playlist: {ex.Message}", "Open Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public void ImportPlaylist()
        {
            var openFileDialog = new System.Windows.Forms.OpenFileDialog
            {
                Filter = "Zip files (*.zip)|*.zip",
                Title = "Select the playlist you want to import."
            };

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                playlistImportFilePath = openFileDialog.FileName;
                Log.Info($"Selected import file: {playlistImportFilePath}");

                string exportPath = OpenFolderDialog("Select the directory you want to import to.");
                if (!string.IsNullOrEmpty(exportPath))
                {
                    playlistFolderPath = exportPath;
                    Log.Info($"Importing playlist to: {playlistFolderPath}");
                    TryImportPlaylist();
                }
            }
        }

        private void TryImportPlaylist()
        {
            try
            {
                Log.Info("Show loading window");
                var loadingWindow = new LoadingWindow();
                loadingWindow.Show();

                Log.Info("Import the playlist");
                import.openZIP(playlistImportFilePath, playlistFolderPath);
                Log.Info($"Playlist imported successfully from {playlistImportFilePath} to {import.importFolderPath}.");

                Log.Info("Close Startup window");
                if (_startupWindow is StartupWindow startupWindow)
                {
                    startupWindow.StartupWindowClosing = true;
                    _startupWindow.Close();
                }

                Log.Info("Open new main window with the new playlist");
                var newMainWindow = new MainWindow(import.importFolderPath);
                newMainWindow.Show();

                Log.Info("Close loading window");
                loadingWindow.Close();
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to import playlist: {ex.Message}");
                Log.Exception(ex);
                System.Windows.MessageBox.Show($"Error importing playlist: {ex.Message}", "Import Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private string OpenFolderDialog(string description)
        {
            var folderDialog = new FolderBrowserDialog
            {
                Description = description
            };

            return folderDialog.ShowDialog() == DialogResult.OK ? folderDialog.SelectedPath : string.Empty;
        }

        public void ExportPlaylist(string _playlistFolderPath)
        {
            Log.Info("Export menu item clicked.");
            var folderDialog = new System.Windows.Forms.FolderBrowserDialog
            {
                Description = "Select a destination folder for the exported playlist."
            };

            if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string destinationPath = folderDialog.SelectedPath;
                try
                {
                    Log.Info("Show loading window");
                    var loadingWindow = new LoadingWindow();
                    loadingWindow.Show();

                    Log.Info("Exporting playlist");
                    export.createZIP(_playlistFolderPath, destinationPath);
                    Log.Info($"Playlist exported successfully to {destinationPath}.");

                    Log.Info("Close loading window");
                    loadingWindow.Close();
                }
                catch (Exception ex)
                {
                    Log.Error($"Error exporting playlist: {ex.Message}");
                    System.Windows.MessageBox.Show($"Failed to export playlist: {ex.Message}", "Export Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            var mainWindow = System.Windows.Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            if (mainWindow != null)
            {
                mainWindow.RefreshCueList();
            }
            else
            {
                Log.Warning("!!! No MainWindow instance found. !!! \n \n Please create a bug report and upload you log file to github. \n \n !!! No MainWindow instance found. !!! ");
            }
        }
    }
}
