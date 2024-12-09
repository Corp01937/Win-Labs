using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.IO;

namespace Win_Labs
{
    public static class CueManager
    {
        public static bool StartupFinished { get; private set; }

        public static void MarkStartupAsFinished()
        {
            StartupFinished = true;
        }
        public static bool ValidJsonFileInPlaylist { get; private set; }

        public static ObservableCollection<Cue> LoadCues(string playlistFolderPath)
        {
            var cues = new ObservableCollection<Cue>();
            Log.Info($"Starting to load cues from {playlistFolderPath}");

            if (string.IsNullOrEmpty(playlistFolderPath) || !Directory.Exists(playlistFolderPath))
            {
                Log.Warning("Invalid playlist folder path.");
                return cues;
            }

            var cueFiles = Directory.GetFiles(playlistFolderPath, "*.json");
            Log.Info($"Found {cueFiles.Length} cue files.");

            foreach (var file in cueFiles)
            {
                try
                {
                    var cue = DeserializeCue(file);
                    if (cue != null)
                    {
                        cues.Add(cue);
                        Log.Info($"Loaded cue from {MaskFilePath(file)}");
                    }
                }
                catch (Exception ex)
                {
                    Log.Error($"Error loading cue from {file}: {ex.Message}");
                }
            }

            StartupFinished = true;
            return cues;
        }

        public static Cue CreateNewCue(int cueNumber, string playlistFolderPath)
        {
            if (string.IsNullOrEmpty(playlistFolderPath))
            {
                Log.Warning("Playlist folder path is not set.");
                throw new ArgumentException("Playlist folder path cannot be null or empty.");
            }

            // Create a new cue object with default values
            var newCue = new Cue
            {
                CueNumber = cueNumber,
                PlaylistFolderPath = playlistFolderPath,
                CueName = $"Cue {cueNumber}",
                Duration = "00:00",
                PreWait = "00:00",
                PostWait = "00:00",
                AutoFollow = false,
                FileName = "",
                TargetFile = "",
                Notes = "Default Cue"
            };

            // Save the new cue to a file
            SaveCueToFile(newCue, playlistFolderPath);

            Log.Info($"New cue created: {newCue.CueName} with number {newCue.CueNumber}");
            return newCue;
        }


        public static void SaveCueToFile(Cue cue, string playlistFolderPath)
        {
            string filePath = Path.Combine(playlistFolderPath, $"cue_{cue.CueNumber}.json");
            string json = JsonConvert.SerializeObject(cue, Formatting.Indented);

            try
            {
                File.WriteAllText(filePath, json);
                Log.Info($"Cue {cue.CueNumber} saved successfully to {filePath}");
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to save cue {cue.CueNumber}: {ex.Message}");
            }
        }


        public static void SaveAllCues(ObservableCollection<Cue> cues, string playlistFolderPath)
        {
            if (cues == null || string.IsNullOrEmpty(playlistFolderPath))
            {
                Log.Warning("Cues collection or playlist folder path is not set.");
                return;
            }

            foreach (var cue in cues)
            {
                SaveCueToFile(cue, playlistFolderPath);
            }
            Log.Info("All cues saved successfully.");
        }

        public static void DeleteCueFile(Cue cue, string playlistFolderPath)
        {
            var filePath = GetCueFilePath(cue, playlistFolderPath);

            if (File.Exists(filePath))
            {
                try
                {
                    File.Delete(filePath);
                    Log.Info($"Deleted cue file: {MaskFilePath(filePath)}");
                }
                catch (Exception ex)
                {
                    Log.Error($"Failed to delete cue file {MaskFilePath(filePath)}: {ex.Message}");
                }
            }
            else
            {
                Log.Warning("Cue file not found for deletion.");
            }
        }

        public static bool IsValidJsonFileInPlaylist(string playlistFolderPath)
        {
            if (string.IsNullOrEmpty(playlistFolderPath) || !Directory.Exists(playlistFolderPath))
            {
                Log.Warning("Invalid playlist folder path.");
                return false;
            }

            var jsonFiles = Directory.GetFiles(playlistFolderPath, "*.json");
            foreach (var file in jsonFiles)
            {
                if (DeserializeCue(file) != null)
                {
                    ValidJsonFileInPlaylist = true;
                    return true;
                }
            }

            ValidJsonFileInPlaylist = false;
            return false;
        }

        private static string GetCueFilePath(Cue cue, string folderPath) =>
            Path.Combine(folderPath, $"cue_{cue.CueNumber}.json");

        private static Cue DeserializeCue(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    Log.Warning($"Cue file not found: {MaskFilePath(filePath)}");
                    return new Cue(); // Return a default Cue if the file is missing
                }

                var json = File.ReadAllText(filePath);
                var cue = JsonConvert.DeserializeObject<Cue>(json);

                if (cue == null)
                {
                    Log.Warning($"Failed to deserialize cue: JSON resulted in a null object from {MaskFilePath(filePath)}");
                    return new Cue(); // Return a default Cue if deserialization failed
                }

                return cue;
            }
            catch (JsonException jsonEx)
            {
                Log.Error($"Invalid JSON format in file {MaskFilePath(filePath)}: {jsonEx.Message}");
                return new Cue(); // Return a default Cue on JSON error
            }
            catch (IOException ioEx)
            {
                Log.Error($"File access error for {MaskFilePath(filePath)}: {ioEx.Message}");
                return new Cue(); // Return a default Cue on file access error
            }
            catch (Exception ex)
            {
                Log.Error($"Unexpected error deserializing cue from {MaskFilePath(filePath)}: {ex.Message}");
                return new Cue(); // Return a default Cue for any other errors
            }
        }


        private static string MaskFilePath(string filePath)
        {
            const int maxLength = 30;
            return filePath.Length > maxLength
                ? $"...{filePath[^maxLength..]}"
                : filePath;
        }
    }
}
