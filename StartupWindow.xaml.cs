using System.ComponentModel;
using System.IO;
using System.Reflection.Metadata;
using System.Windows;
using System.Xml.Serialization;


namespace Win_Labs
{
    public partial class StartupWindow : Window
    {
        private PlaylistManager playlistManager;
        public string playlistFolderPath { get; private set; } = string.Empty;
        public string playlistImportFilePath { get; private set; } = string.Empty;
        public bool StartupWindowClosing { get; set; }
        public StartupWindow()
        {
            InitializeComponent();
            StartupWindowClosing = false;
            playlistManager = new PlaylistManager(this);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (!StartupWindowClosing)
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
                StartupWindowClosing = true; // Mark that the window is closing
            }

            base.OnClosing(e);
        }
        private void CreateNewPlaylist_Click(object sender, RoutedEventArgs e)
        {
            Log.Info("Create New Playlist clicked.");
            playlistManager.CreateNewPlaylist();
        }

        private void OpenExistingPlaylist_Click(object sender, RoutedEventArgs e)
        {
            Log.Info("Open Playlist clicked.");
            playlistManager.OpenExistingPlaylist();
        }
        private void ImportPlaylist_Click(object sender, RoutedEventArgs e)
        {
            Log.Info("Import Playlist clicked.");
            playlistManager.ImportPlaylist();
        }

        private void CloseMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Log.Info("Close menu item clicked.");
            Close();
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
