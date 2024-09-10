﻿using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;

namespace Win_Labs
{

    public static class CueManager
    {
        public static bool startUpFinished;
        public static bool validJsonFileInPlaylist;
        public static string GetCurrentTime()
        {
            return DateTime.Now.ToString("yy-MM-dd HH:mm:ss");
        }

        public static void Log(string message)
        {
            string timestamp = GetCurrentTime();
            Console.WriteLine($"[{timestamp}] Message: {message}");
        }

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
                    Log("Cue is null. Cannot save.");
                    return;
                }

                if (string.IsNullOrEmpty(playlistFolderPath))
                {
                    Log("Playlist folder path is not set.");
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

                Log($"Cue {cue.CueNumber} saved successfully to {MaskFilePath(filePath)}");
            }
            catch (Exception ex)
            {
                Log($"Error saving cue {cue.CueNumber}: {ex.Message}");
            }
        }

        // Method to save all cues in a collection
        public static void SaveAllCues(ObservableCollection<Cue> cues, string playlistFolderPath)
        {
            if (cues == null || string.IsNullOrEmpty(playlistFolderPath))
            {
                Log("Cues collection or playlist folder path is not set.");
                return;
            }

            foreach (var cue in cues)
            {
                SaveCueToFile(cue, playlistFolderPath);
            }
            Log("All cues saved.");
        }

        public static bool IsValidJsonFileInPlaylist(string playlistFolderPath)
        {
            // Check if the playlist folder path is valid
            if (string.IsNullOrEmpty(playlistFolderPath) || !Directory.Exists(playlistFolderPath))
            {
                Log("Invalid playlist folder path.");
                return false;
            }

            // Get all JSON files in the playlist folder
            string[] jsonFiles = Directory.GetFiles(playlistFolderPath, "*.json");
            Log($"Found {jsonFiles.Length} JSON files.");

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
                        Log($"Valid JSON file found: {file}");
                        validJsonFileInPlaylist = true;
                        return true; // A valid JSON file was found
                    }
                    else
                    {
                        Log($"Deserialization returned null for file: {file}");
                    }
                }
                catch (JsonException jsonEx)
                {
                    Log($"Error deserializing JSON from file {file}: {jsonEx.Message}");
                    // Optionally, continue checking other files
                }
                catch (IOException ioEx)
                {
                    Log($"I/O error while reading file {file}: {ioEx.Message}");
                    // Optionally, continue checking other files
                }
                catch (Exception ex)
                {
                    Log($"Unexpected error while processing file {file}: {ex.Message}");
                    // Optionally, continue checking other files
                }
            }

            // No valid JSON file found
            Log("No valid JSON file found in the playlist.");
            validJsonFileInPlaylist = false;
            return false;
        }


        // Method to load cues from a folder
        public static ObservableCollection<Cue> LoadCues(string playlistFolderPath)
        {

            var observableCollectionCue = new ObservableCollection<Cue>();

            // Log the start of the loading process
            Log($"Starting to load cues from playlist folder: {playlistFolderPath}");

            if (string.IsNullOrEmpty(playlistFolderPath))
            {
                Log("Playlist folder path is not set.");
                return observableCollectionCue;
            }

            // Load all cue files from the playlist folder
            string[] cueFilesArray;
            try
            {
                cueFilesArray = Directory.GetFiles(playlistFolderPath, "*.json");
                Log($"Found {cueFilesArray.Length} cue files.");
            }
            catch (Exception ex)
            {
                Log($"Error accessing playlist folder: {ex.Message}");
                MessageBox.Show($"Error accessing playlist folder: {ex.Message}", "Folder Access Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return observableCollectionCue;
            }

            foreach (var file in cueFilesArray)
            {
                Log($"Processing file: {file}");
                try
                {
                    string jsonContent = File.ReadAllText(file);
                    Log($"File content read successfully. Size: {jsonContent.Length} characters.");

                    Cue deserializedJson = JsonConvert.DeserializeObject<Cue>(jsonContent);

                    if (deserializedJson != null)
                    {
                        observableCollectionCue.Add(deserializedJson);
                        Log($"Cue Loaded");
                    }
                    else
                    {
                        Log($"Deserialization returned null for file: {file}");
                    }
                }
                catch (JsonException jsonEx)
                {
                    Log($"Error deserializing JSON from file {file}: {jsonEx.Message}");
                    MessageBox.Show($"Error deserializing JSON from file {file}: {jsonEx.Message}", "Deserialization Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (IOException ioEx)
                {
                    Log($"I/O error while reading file {file}: {ioEx.Message}");
                    MessageBox.Show($"I/O error while reading file {file}: {ioEx.Message}", "File Read Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    Log($"Unexpected error while processing file {file}: {ex.Message}");
                    MessageBox.Show($"Unexpected error while processing file {file}: {ex.Message}", "Unexpected Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            Log("Finished loading cues.");
            return observableCollectionCue;
        }

    }
}