using System;
using System.IO;
using System.Windows;
using Win_Labs;

namespace Win_Labs
{
    public partial class StartupWindow : Window
    {
        public string GetCurrentTime()
        {
            return DateTime.Now.ToString("yy-MM-dd HH:mm:ss");
        }

        public void Log(string message)
        {
            string timestamp = GetCurrentTime();
            Console.WriteLine($"[{timestamp}] Message: {message}");
        }
        public StartupWindow()
        {
            InitializeComponent();
        }

        private void CreateNewPlaylist_Click(object sender, RoutedEventArgs e)
        {
            var folderDialog = new System.Windows.Forms.FolderBrowserDialog();
            if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string playlistFolderPath = folderDialog.SelectedPath;
                Log("Selected Path: "+playlistFolderPath);
                // Open the MainWindow with the selected playlist folder
                var mainWindow = new MainWindow(playlistFolderPath);
                mainWindow.Show();
                this.Close();
            }
        }

        private void OpenExistingPlaylist_Click(object sender, RoutedEventArgs e)
        {
            var folderDialog = new System.Windows.Forms.FolderBrowserDialog();
            if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string playlistFolderPath = folderDialog.SelectedPath;
                Log("Selected Path: " + playlistFolderPath);
                // Open the MainWindow with the selected playlist folder
                var mainWindow = new MainWindow(playlistFolderPath);
                Log("MainWindow.Created");
                CueManager.startUpFinished = true;
                Log("StartUp Finished");
                mainWindow.Show();
                this.Close();
            }
        }
    }
}
