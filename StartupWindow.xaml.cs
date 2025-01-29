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
                    "Are you sure you want to close?",
                    "",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.No)
                {
                    Log.Info("User canceled the closing action.");
                    e.Cancel = true; // Prevent the window from closing
                    return;
                }

                if (result == MessageBoxResult.Yes)
                {
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
                Description = "Select a playlist folder."
            };

            if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string folderPath = folderDialog.SelectedPath;
                Log.Info($"Opening existing playlist folder: {folderPath}");
                if (!Directory.EnumerateFiles(folderPath, "cue_*.json").Any())
                {
                    Log.Error("No cue files found in the selected folder: " + folderPath);
                    MessageBox.Show("The selected folder does not contain any cue files.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                OpenMainWindow(folderPath);
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

        private void CloseButton_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Log.Info("Close Button clicked.");
            Close();
        }

        private void TitleBarIcon_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            MessageBox.Show("Hi there.", "!!EasterEgg!!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }

    }
}
