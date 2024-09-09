using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows;

namespace Win_Labs
{
    public class Cue : INotifyPropertyChanged
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
        public string PlaylistFolderPath { get; set; }

    // Constructor with a parameter for playlistFolderPath
    public Cue(string playlistFolderPath)
    {
        PlaylistFolderPath = playlistFolderPath;
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
                    Log($"Attempting to set CueNumber to {value}");
                    bool shouldProceed = CheckForDuplicateCueFile(value);

                    if (shouldProceed ==true)
                    {
                        RenameCueFile(cueNumber, value);
                        cueNumber = value;
                        OnPropertyChanged(nameof(CueNumber));
                        Log($"CueNumber set to {value}");
                    }
                    else
                    {
                        Log($"User chose not to replace the existing cue file.");
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
                    Log("PropertyChange.CueName");
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
                    Log("PropertyChange.Duration");
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
                    Log("PropertyChange.PreWait");
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
                    Log("PropertyChange.PostWait");
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
                    Log("PropertyChange.AutoFollow");
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
                if (fileName != value)
                {
                    Log("PropertyChange.FileName");
                    fileName = value;
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
                    Log("PropertyChange.TargetFile");
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
                    Log("PropertyChange.Notes");
                    notes = value;
                    OnPropertyChanged(nameof(Notes));

                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;


        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            Log("Save.Called");
            Save();
            
        }

        public void Save()
        {
            if (string.IsNullOrEmpty(PlaylistFolderPath)==false)
            {
                CueManager.SaveCueToFile(this, PlaylistFolderPath);
                Log("Saved");
            }
            else
            {
                Log("Not Saved");
                Log("String.IsNullOrEmpty(PlaylistFolderPath)"+ " = True");
                Log("PlayslistFolderPath = "+ PlaylistFolderPath);
            }
        }

        private bool CheckForDuplicateCueFile(float newCueNumber)
        {
            if(CueManager.startUpFinished == false)
            {
                Log("In startup mode duplicate check skiped.");
                return true;
            }
            // Construct the new file path for the cue
            string fileName = $"cue_{newCueNumber}.json";
            string newFilePath = Path.Combine(PlaylistFolderPath, fileName);

            // Check if the new file already exists
            if (File.Exists(newFilePath)==true)
            {
                Log("Duplicate file found: "+newFilePath);
                var result = MessageBox.Show(
                    $"A cue with the number {newCueNumber} already exists. Do you want to replace it?",
                    "File Already Exists",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning
                );
                if (result == MessageBoxResult.Yes) { Log("UserInput.Yes"); } else { Log("UserInput.No"); }
                return result == MessageBoxResult.Yes; // If user confirms, proceed
            }
            Log("No duplicate found, proceed");
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
                Log("In startup mode Rename skiped.");
                return;
            }
            // Construct file paths
            string oldFilePath = Path.Combine(PlaylistFolderPath, $"cue_{oldCueNumber}.json");
            string newFilePath = Path.Combine(PlaylistFolderPath, $"cue_{newCueNumber}.json");


            // Verify the PlaylistFolderPath
            if (!Directory.Exists(PlaylistFolderPath))
            {
                Log($"Playlist folder does not exist: {PlaylistFolderPath}");
                return;
            }

            try
            {
                Log($"Attempting to rename file from {MaskFilePath(oldFilePath)} to {MaskFilePath(newFilePath)}");

                // Check if the old file exists
                if (File.Exists(oldFilePath))
                {
                    // Rename the file
                    File.Move(oldFilePath, newFilePath, true);
                    Log("New file made");
                    // Delete old file
                    File.Delete(oldFilePath);
                    Log("Old file delted");
                    // Update _cueFilePath to the new file path
                    _cueFilePath = newFilePath;

                    // Mask paths before logging
                    string maskedOldFilePath = MaskPathPart(oldFilePath, 40); // Adjust max visible length as needed
                    string maskedNewFilePath = MaskPathPart(newFilePath, 40); // Adjust max visible length as needed
                    // Log success
                    Log($"Cue file renamed successfully from {maskedOldFilePath} to {maskedNewFilePath}");
                    Log("Rename.Sucess");
                }
                else
                {
                    // Log if the old file is not found
                    Log($"Old cue file not found: {MaskPathPart(oldFilePath, 40)}");
                }
            }
            catch (Exception ex)
            {
                // Log any exceptions that occur
                Log($"Error renaming file from {oldFilePath} to {newFilePath}: {ex.Message}");
            }
        }




        public void SetFilePath(string filePath)
        {
            _cueFilePath = filePath;
        }
    }
}
