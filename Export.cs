using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Win_Labs;

namespace Win_Labs
{
    internal class Export
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

        public string playlistFolderPath {  get; set; }
        public void createZIP(string playlistExportFolderPath)
        {
            //var zipFile =
                            if (string.IsNullOrEmpty(playlistFolderPath))
            {
                Log("Playlist folder path is not set.");
                return;
            }

            if (!Directory.Exists(playlistFolderPath))
            {
                Directory.CreateDirectory(playlistFolderPath);
            }
            System.IO.Compression.ZipFile.CreateFromDirectory(playlistExportFolderPath, zipFile);


        }
    }
}
