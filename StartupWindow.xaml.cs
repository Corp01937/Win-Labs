using System;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using Win_Labs;

namespace Win_Labs
{
    public partial class StartupWindow : Window
    {
        public static string playlistFolderPath;
        public StartupWindow()
        {
            InitializeComponent();
        }

        private void CreateNewPlaylist_Click(object sender, RoutedEventArgs e)
        {
            var folderDialog = new System.Windows.Forms.FolderBrowserDialog();
            if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                playlistFolderPath = folderDialog.SelectedPath;
                Log.log("Selected Path: "+playlistFolderPath, Log.LogLevel.Info);
                // Open the MainWindow with the selected playlist folder
                var mainWindow = new MainWindow(playlistFolderPath);
                Log.log("MainWindow.Created", Log.LogLevel.Info);
                CueManager.startUpFinished = true;
                Log.log("StartUp Finished");
                mainWindow.Show();
                Log.log("Showing.MainWindow", Log.LogLevel.Info);
                this.Close();
            }
        }

        private void OpenExistingPlaylist_Click(object sender, RoutedEventArgs e)
        {
            var folderDialog = new System.Windows.Forms.FolderBrowserDialog();
            if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                playlistFolderPath = folderDialog.SelectedPath;
                Log.log("Selected Path: " + playlistFolderPath, Log.LogLevel.Info);
                // Open the MainWindow with the selected playlist folder
                var mainWindow = new MainWindow(playlistFolderPath);
                Log.log("MainWindow.Created", Log.LogLevel.Info);
                CueManager.startUpFinished = true;
                Log.log("StartUp Finished", Log.LogLevel.Info);
                mainWindow.Show();
                Log.log("Showing.MainWindow", Log.LogLevel.Info);
                this.Close();
            }
        }
    }
}
