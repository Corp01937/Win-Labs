using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;
using Win_Labs;

namespace Win_Labs
{
    internal class export
    {
        public static string playlistFolderPath = StartupWindow.playlistFolderPath;
        public static void createZIP(string playlistExportFolderPath)
        {
            string playlistName = playlistFolderPath.Split(@"\")[^1] + ".zip";
            Log.log("File name set to: " + playlistName);
            var zipFile = playlistExportFolderPath + "\\" + playlistName;
            Log.log($"Export File Path: {zipFile}");
            if (string.IsNullOrEmpty(playlistFolderPath))
            {
                Log.log("Playlist folder path is not set.");
                return;
            }
            if (!Directory.Exists(playlistExportFolderPath))
            {
                Directory.CreateDirectory(playlistExportFolderPath);
                Log.log("Created Export folder as it did not exist.");
            }
            try
            {
                System.IO.Compression.ZipFile.CreateFromDirectory(playlistFolderPath, zipFile);
                Log.log($"{zipFile} Created.");
            }
            catch (Exception ex) 
            {
                MessageBox.Show($"Could not create file {zipFile}. Please check location and or if there is already a file with the same name as your playlist.","File Creation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }


        }
    }
}
