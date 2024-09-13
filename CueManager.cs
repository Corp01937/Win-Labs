using NAudio.Wave;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using Win_Labs;

namespace Win_Labs
{

    public static class CueManager
    {
        public static bool startUpFinished;
        public static bool validJsonFileInPlaylist;

        private static string MaskFilePath(string filePath)
        {
            // Define a maximum length for the visible part of the path
            const int maxVisibleLength = 20;

            // Check if the filePath is longer than the maximum visible length
            if (filePath.Length > maxVisibleLength)
            {
                // Extract the leading part, the directory part, and the ending part of the path
                string directoryPart = Path.GetDirectoryName(filePath);
                string fileName = Path.GetFileName(filePath);

                // Mask the directory part
                string maskedDirectoryPart = MaskPathPart(directoryPart, maxVisibleLength);

                // Combine the masked directory part and file name
                return $"{maskedDirectoryPart}\\{fileName}";
            }
            return filePath; // Return the path as is if it's within the visible length
        }

        private static string MaskPathPart(string pathPart, int maxVisibleLength)
        {
            // Check if the part is already short enough
            if (pathPart.Length <= maxVisibleLength)
            {
                return pathPart;
            }

            // Find the last directory separator in the path part
            int lastSeparatorIndex = pathPart.LastIndexOf(Path.DirectorySeparatorChar);

            // Handle case where there's no separator (whole path is the file name)
            if (lastSeparatorIndex == -1)
            {
                // Ensures no out-of-range exception
                int visibleLength = Math.Min(maxVisibleLength, pathPart.Length);
                return $"...{pathPart.Substring(Math.Max(0, pathPart.Length - visibleLength))}";
            }

            // Calculate the starting index and the length of the visible part
            int startIndex = Math.Max(0, lastSeparatorIndex + 1);
            int length = Math.Min(maxVisibleLength, pathPart.Length - startIndex);
            string visiblePart = pathPart.Substring(startIndex, length);

            return $"...\\{visiblePart}";
        }
        // Method to save a single cue to a file
        public static void SaveCueToFile(Cue cue, string playlistFolderPath)
        {
            try
            {
                if (cue == null)
                {
                    Log.log("Cue is null. Cannot save.", Log.LogLevel.Warning);
                    return;
                }

                if (string.IsNullOrEmpty(playlistFolderPath))
                {
                    Log.log("Playlist folder path is not set.", Log.LogLevel.Warning);
                    return;
                }

                if (!Directory.Exists(playlistFolderPath))
                {
                    Directory.CreateDirectory(playlistFolderPath);
                }

                string fileName = $"cue_{cue.CueNumber}.json";
                string filePath = Path.Combine(playlistFolderPath, fileName);

                string json = JsonConvert.SerializeObject(cue, Formatting.Indented);
                File.WriteAllText(filePath, json);

                Log.log($"Cue {cue.CueNumber} saved successfully to {MaskFilePath(filePath)}");
            }
            catch (Exception ex)
            {
                Log.log($"Error saving cue {cue.CueNumber}: {ex.Message}");
            }
        }

        // Method to save all cues in a collection
        public static void SaveAllCues(ObservableCollection<Cue> cues, string playlistFolderPath)
        {
            if (cues == null || string.IsNullOrEmpty(playlistFolderPath))
            {
                Log.log("Cues collection or playlist folder path is not set.", Log.LogLevel.Warning);
                return;
            }

            foreach (var cue in cues)
            {
                SaveCueToFile(cue, playlistFolderPath);
            }
            Log.log("All cues saved.");
        }

        public static bool IsValidJsonFileInPlaylist(string playlistFolderPath)
        {
            // Check if the playlist folder path is valid
            if (string.IsNullOrEmpty(playlistFolderPath) || !Directory.Exists(playlistFolderPath))
            {
                Log.log("Invalid playlist folder path.", Log.LogLevel.Warning);
                return false;
            }

            // Get all JSON files in the playlist folder
            string[] jsonFiles = Directory.GetFiles(playlistFolderPath, "*.json");
            Log.log($"Found {jsonFiles.Length} JSON files.");

            foreach (var file in jsonFiles)
            {
                try
                {
                    // Read the file content
                    string jsonContent = File.ReadAllText(file);

                    // Attempt to deserialize to a Cue object
                    Cue cue = JsonConvert.DeserializeObject<Cue>(jsonContent);

                    // Check if deserialization returned a non-null Cue object
                    if (cue != null)
                    {
                        Log.log($"Valid JSON file found: {file}");
                        validJsonFileInPlaylist = true;
                        return true; // A valid JSON file was found
                    }
                    else
                    {
                        Log.log($"Deserialization returned null for file: {file}", Log.LogLevel.Warning);
                    }
                }
                catch (JsonException jsonEx)
                {
                    Log.log($"Error deserializing JSON from file {file}: {jsonEx.Message}");
                    // Optionally, continue checking other files
                }
                catch (IOException ioEx)
                {
                    Log.log($"I/O error while reading file {file}: {ioEx.Message}");
                    // Optionally, continue checking other files
                }
                catch (Exception ex)
                {
                    Log.log($"Unexpected error while processing file {file}: {ex.Message}");
                    // Optionally, continue checking other files
                }
            }

            // No valid JSON file found
            Log.log("No valid JSON file found in the playlist.", Log.LogLevel.Warning);
            validJsonFileInPlaylist = false;
            return false;
        }


        // Method to load cues from a folder
        public static ObservableCollection<Cue> LoadCues(string playlistFolderPath)
        {
            var observableCollectionCue = new ObservableCollection<Cue>();

            // Log the start of the loading process
            Log.log($"Starting to load cues from playlist folder: {playlistFolderPath}");

            if (string.IsNullOrEmpty(playlistFolderPath))
            {
                Log.log("Playlist folder path is not set.", Log.LogLevel.Warning);
                return observableCollectionCue;
            }

            // Load all cue files from the playlist folder
            string[] cueFilesArray; //Initializeses empty array
            try // Sets cueFileArray
            {
                cueFilesArray = Directory.GetFiles(playlistFolderPath, "*.json");
                Log.log($"Found {cueFilesArray.Length} cue files.");
            }
            catch (Exception ex)
            {
                Log.log($"Error accessing playlist folder: {ex.Message}");
                MessageBox.Show($"Error accessing playlist folder: {ex.Message}", "Folder Access Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return observableCollectionCue;
            }

            foreach (var file in cueFilesArray) // For each file in array
            {
                Log.log($"Processing file: {file}");
                try
                {
                    string jsonContent = File.ReadAllText(file); // reading file content
                    Log.log($"File content read successfully. Size: {jsonContent.Length} characters.");

                    Cue deserializedJson = JsonConvert.DeserializeObject<Cue>(jsonContent); // uses cue constructor to create deserialized json

                    if (deserializedJson != null)
                    {
                        observableCollectionCue.Add(deserializedJson); // updates collection
                        Log.log($"Cue Loaded");
                        Log.log("Finished loading cues.");
                    }
                    else // for cases with an empty file
                    {
                        Log.log($"Deserialization returned null for file: {file}", Log.LogLevel.Warning);
                                    Log.log("Finished loading cues.");
                    }
                }
                catch (JsonException jsonEx)
                {
                    Log.log($"Error deserializing JSON from file {file}: {jsonEx.Message}");
                    MessageBox.Show($"Error deserializing JSON from file {file}: {jsonEx.Message}", "Deserialization Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (IOException ioEx)
                {
                    Log.log($"I/O error while reading file {file}: {ioEx.Message}");
                    MessageBox.Show($"I/O error while reading file {file}: {ioEx.Message}", "File Read Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    Log.log($"Unexpected error while processing file {file}: {ex.Message}");
                    MessageBox.Show($"Unexpected error while processing file {file}: {ex.Message}", "Unexpected Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            return observableCollectionCue;
        }
        public static void UnloadCues(ObservableCollection<Cue> cues)
        {
            Log.log("UnloadCues.Caled");
            string PlaylistFolderPath = StartupWindow.playlistFolderPath; // is called here again as this method is called before the setter.
            Log.log("Saving");
            SaveAllCues(cues, PlaylistFolderPath); // saves cues to avoid loss of data


            return;
        }
    }
}
