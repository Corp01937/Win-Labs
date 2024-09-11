using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.IO;
using System.Security.Permissions;
using System.Windows;
using Win_Labs;

namespace Win_Labs
{
    public class Cue : INotifyPropertyChanged
    {
        private float cueNumber;
        private string _cueFilePath;
        private string duration;
        private string preWait;
        private string postWait;
        private bool autoFollow;
        private string fileName;
        private string targetFile;
        private string notes;
        private string _CueName;
        private bool renaming;
        public string PlaylistFolderPath { // setting playlist folder path for use in Cue class
            get;
            set;

        }

        // Constructor with a parameter for playlistFolderPath
        public Cue(string playlistFolderPath)
        {
            PlaylistFolderPath = StartupWindow.playlistFolderPath;

        }

        // Default constructor
        public Cue()
        {
            PlaylistFolderPath = string.Empty;
        }

        public float CueNumber
        {
            get => cueNumber;
            set
            {
                if (cueNumber != value)
                {
                    Log.log("PropertyChange.CueNumber");
                    Log.log($"Attempting to set CueNumber to {value}");
                    bool shouldProceed = CheckForDuplicateCueFile(value);
                    
                    if (shouldProceed ==true)
                    {
                        RenameCueFile(cueNumber, value);
                        cueNumber = value;
                        OnPropertyChanged(nameof(CueNumber));
                        Log.log($"CueNumber set to {value}");
                    }
                    else
                    {
                        Log.log($"User chose not to replace the existing cue file.");
                    }
                }
            }
        }



        public string CueName
        {
            get => _CueName;
            set
            {
                if (_CueName != value)
                {
                    Log.log("PropertyChange.CueName");
                    _CueName = value;
                    OnPropertyChanged(nameof(CueName));

                }
            }
        }

        public string Duration
        {
            get => duration;
            set
            {
                if (duration != value)
                {
                    Log.log("PropertyChange.Duration");
                    duration = value;
                    OnPropertyChanged(nameof(Duration));

                }
            }
        }

        public string PreWait
        {
            get => preWait;
            set
            {
                if (preWait != value)
                {
                    Log.log("PropertyChange.PreWait");
                    preWait = value;
                    OnPropertyChanged(nameof(PreWait));

                }
            }
        }

        public string PostWait
        {
            get => postWait;
            set
            {
                if (postWait != value)
                {
                    Log.log("PropertyChange.PostWait");
                    postWait = value;
                    OnPropertyChanged(nameof(PostWait));

                }
            }
        }

        public bool AutoFollow
        {
            get => autoFollow;
            set
            {
                if (autoFollow != value)
                {
                    Log.log("PropertyChange.AutoFollow");
                    autoFollow = value;
                    OnPropertyChanged(nameof(AutoFollow));

                }
            }
        }

        public string FileName
        {
            get => fileName;
            set
            {
                if (fileName != targetFile)
                {
                    Log.log("PropertyChange.FileName");
                    fileName = targetFile;
                    OnPropertyChanged(nameof(FileName));

                }
            }
        }

        public string TargetFile
        {
            get => targetFile;
            set
            {
                if (targetFile != value)
                {
                    Log.log("PropertyChange.TargetFile");
                    targetFile = value;
                    OnPropertyChanged(nameof(TargetFile));

                }
            }
        }

        public string Notes
        {
            get => notes;
            set
            {
                if (notes != value)
                {
                    Log.log("PropertyChange.Notes");
                    notes = value;
                    OnPropertyChanged(nameof(Notes));

                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;


        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            Log.log("Save.Called");
            Save();
            
        }

        public void Save()
        {
            if(renaming== true) { return; }
            PlaylistFolderPath = StartupWindow.playlistFolderPath; // is called here again as this method is called before the setter.

            if (string.IsNullOrEmpty(PlaylistFolderPath)==false)
            {
                CueManager.SaveCueToFile(this, PlaylistFolderPath);
                Log.log("Saved");
            }
            else
            {
                Log.log("Not Saved");
                Log.log("String.IsNullOrEmpty(PlaylistFolderPath)"+ " = True");
                Log.log("PlayslistFolderPath = "+ PlaylistFolderPath);
            }
        }

        private bool CheckForDuplicateCueFile(float newCueNumber)
        {
            if(CueManager.startUpFinished == false)
            {
                Log.log("In startup mode duplicate check skiped.");
                return true;
            }
            // Construct the new file path for the cue
            string fileName = $"cue_{newCueNumber}.json";
            string newFilePath = Path.Combine(PlaylistFolderPath, fileName);

            // Check if the new file already exists
            if (File.Exists(newFilePath)==true)
            {
                Log.log("Duplicate file found: "+newFilePath);
                var result = MessageBox.Show(
                    $"A cue with the number {newCueNumber} already exists. Do you want to replace it?",
                    "File Already Exists",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning
                );
                if (result == MessageBoxResult.Yes) { Log.log("UserInput.Yes"); } else { Log.log("UserInput.No"); }
                return result == MessageBoxResult.Yes; // If user confirms, proceed
            }
            Log.log("No duplicate found, proceed");
            return true;
        }

        private string MaskFilePath(string filePath)
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

        private string MaskPathPart(string pathPart, int maxVisibleLength)
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


        private void RenameCueFile(float oldCueNumber, float newCueNumber)
        {
            if(CueManager.startUpFinished == false)
            {
                Log.log("In startup mode Rename skiped.");
                return;
            }
            renaming = true;
            // Construct file paths
            string oldFilePath = Path.Combine(PlaylistFolderPath, $"cue_{oldCueNumber}.json");
            string newFilePath = Path.Combine(PlaylistFolderPath, $"cue_{newCueNumber}.json");


            // Verify the PlaylistFolderPath
            if (!Directory.Exists(PlaylistFolderPath))
            {
                Log.log($"Playlist folder does not exist: {PlaylistFolderPath}");
                return;
            }

            try
            {
                Log.log($"Attempting to rename file from {MaskFilePath(oldFilePath)} to {MaskFilePath(newFilePath)}");

                // Check if the old file exists
                if (File.Exists(oldFilePath))
                {
                    // Rename the file
                    File.Move(oldFilePath, newFilePath, true);
                    Log.log("New file made");
                    // Delete old file
                    File.Delete(oldFilePath);
                    Log.log("Old file delted");
                    // Update _cueFilePath to the new file path
                    _cueFilePath = newFilePath;

                    // Mask paths before logging
                    string maskedOldFilePath = MaskPathPart(oldFilePath, 40); // Adjust max visible length as needed
                    string maskedNewFilePath = MaskPathPart(newFilePath, 40); // Adjust max visible length as needed
                    // Log success
                    Log.log($"Cue file renamed successfully from {maskedOldFilePath} to {maskedNewFilePath}");
                    Log.log("Rename.Sucess");
                }
                else
                {
                    // Log if the old file is not found
                    Log.log($"Old cue file not found: {MaskPathPart(oldFilePath, 40)}", Log.LogLevel.Error);
                }
            }
            catch (Exception ex)
            {
                // Log any exceptions that occur
                Log.log($"Error renaming file from {oldFilePath} to {newFilePath}: {ex.Message}");
            }
            renaming = false;
        }




        public void SetFilePath(string filePath)
        {
            _cueFilePath = filePath;
        }
    }
}
