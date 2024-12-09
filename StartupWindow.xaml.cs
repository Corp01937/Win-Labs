using System.ComponentModel;
using System.IO;
using System.Windows;

namespace Win_Labs
{
    public partial class StartupWindow : Window
    {
        public string playlistFolderPath { get; private set; } = string.Empty;
        public string playlistImportFilePath { get; private set; } = string.Empty;
        private bool _startupWindowClosing;


        public StartupWindow()
        {
            InitializeComponent();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (!_startupWindowClosing)
            {
                Log.Info("StartupWindow.Close.Detected");

                var result = MessageBox.Show(
                    "Do you want to save changes before closing?",
                    "Save Changes",
                    MessageBoxButton.YesNoCancel,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Cancel)
                {
                    Log.Info("User canceled the closing action.");
                    e.Cancel = true; // Prevent the window from closing
                    return;
                }

                if (result == MessageBoxResult.Yes)
                {
                    Log.Info("User chose to save changes.");
                    try
                    {
                        SaveChanges(); // Call the save logic
                        Log.Info("Changes saved successfully.");
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"Error saving changes: {ex.Message}");
                        MessageBox.Show($"Failed to save changes: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        e.Cancel = true; // Prevent closing if save fails
                        return;
                    }
                }

                Log.Info("Proceeding with closing the window.");
                _startupWindowClosing = true; // Mark that the window is closing
            }

            base.OnClosing(e);
        }

        private void SaveChanges()
        {
            // Example save logic: log a message for now
            Log.Info("SaveChanges called.");
            // TODO: Implement actual save functionality, e.g., saving settings or data.
        }



        private void CreateNewPlaylist_Click(object sender, RoutedEventArgs e)
        {
            string selectedPath = OpenFolderDialog("Select a folder to create a playlist in.");
            if (!string.IsNullOrEmpty(selectedPath))
            {
                playlistFolderPath = selectedPath;
                Log.Info($"New playlist folder selected: {playlistFolderPath}");
                OpenMainWindow(playlistFolderPath);
            }
        }

        private void OpenExistingPlaylist_Click(object sender, RoutedEventArgs e)
        {
            var folderDialog = new System.Windows.Forms.FolderBrowserDialog
            {
                Description = "Select a playlist folder to load."
            };

            if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                playlistFolderPath = folderDialog.SelectedPath;
                Log.Info($"Opening existing playlist folder: {playlistFolderPath}");

                // Verify that the folder contains cue files or is a valid playlist
                if (!Directory.EnumerateFiles(playlistFolderPath, "*.json").Any())
                {
                    MessageBox.Show("The selected folder does not contain any valid playlist files.", "Invalid Playlist", MessageBoxButton.OK, MessageBoxImage.Warning);
                    Log.Warning($"Selected folder '{playlistFolderPath}' does not contain any cue files.");
                    return;
                }
                OpenMainWindow(playlistFolderPath);
            }
        }


        private void ImportPlaylist_Click(object sender, RoutedEventArgs e)
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

        private void CloseMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Log.Info("Close menu item clicked.");
            Close();
        }

        private void OpenMainWindow(string playlistFolderPath)
        {
            var mainWindow = new MainWindow(playlistFolderPath);
            Log.Info("MainWindow created and initialized.");
            CueManager.MarkStartupAsFinished();
            Log.Info("Startup process completed.");
            mainWindow.Show();
            _startupWindowClosing = true;
            Close();
        }

        private string OpenFolderDialog(string description)
        {
            var folderDialog = new System.Windows.Forms.FolderBrowserDialog
            {
                Description = description
            };

            if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Log.Info($"Folder selected: {folderDialog.SelectedPath}");
                return folderDialog.SelectedPath;
            }

            Log.Warning("No folder selected.");
            return string.Empty;
        }

        private void TryImportPlaylist()
        {
            try
            {
                import.openZIP(playlistImportFilePath, playlistFolderPath);
                Log.Info("Playlist imported successfully.");
                OpenMainWindow(playlistFolderPath);
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to import playlist: {ex.Message}");
                MessageBox.Show($"Error importing playlist: {ex.Message}", "Import Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
