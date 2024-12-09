using System;
using System.ComponentModel;
using System.IO;
using System.Windows;

namespace Win_Labs
{
    public class Cue : INotifyPropertyChanged
    {
        private float _cueNumber;
        private string _cueFilePath;
        private string _duration;
        private string _preWait;
        private string _postWait;
        private bool _autoFollow;
        private string _fileName;
        private string _targetFile;
        private string _notes;
        private string _cueName;
        private bool _renaming;

        // Instance property
        public string PlaylistFolderPath { get; set; }

        public Cue(string playlistFolderPath)
        {
            PlaylistFolderPath = playlistFolderPath;
        }

        public Cue() : this(string.Empty) { }

        public float CueNumber
        {
            get => _cueNumber;
            set
            {
                if (_cueNumber != value)
                {
                    
                    Log.Info($"PropertyChange.CueNumber - Attempting to set CueNumber to {value}");
                    bool proceed = CheckForDuplicateCueFile(value);

                    if (proceed)
                    {
                        RenameCueFile(_cueNumber, value);
                        _cueNumber = value;
                        OnPropertyChanged(nameof(CueNumber));
                        Log.Info($"CueNumber set to {value}");
                    }
                    else
                    {
                        Log.Info("User chose not to replace the existing cue file.");
                    }
                }
            }
        }

        public string CueName
        {
            get => _cueName;
            set
            {
                if (_cueName != value)
                {
                    Log.Info("PropertyChange.CueName");
                    _cueName = value;
                    OnPropertyChanged(nameof(CueName));
                }
            }
        }

        public string Duration
        {
            get => _duration;
            set
            {
                if (_duration != value)
                {
                    Log.Info("PropertyChange.Duration");
                    _duration = value;
                    OnPropertyChanged(nameof(Duration));
                }
            }
        }

        public string PreWait
        {
            get => _preWait;
            set
            {
                if (_preWait != value)
                {
                    Log.Info("PropertyChange.PreWait");
                    _preWait = value;
                    OnPropertyChanged(nameof(PreWait));
                }
            }
        }

        public string PostWait
        {
            get => _postWait;
            set
            {
                if (_postWait != value)
                {
                    Log.Info("PropertyChange.PostWait");
                    _postWait = value;
                    OnPropertyChanged(nameof(PostWait));
                }
            }
        }

        public bool AutoFollow
        {
            get => _autoFollow;
            set
            {
                if (_autoFollow != value)
                {
                    Log.Info("PropertyChange.AutoFollow");
                    _autoFollow = value;
                    OnPropertyChanged(nameof(AutoFollow));
                }
            }
        }

        public string FileName
        {
            get => _fileName;
            set
            {
                if (_fileName != value)
                {
                    Log.Info("PropertyChange.FileName");
                    _fileName = value;
                    OnPropertyChanged(nameof(FileName));
                }
            }
        }

        public string TargetFile
        {
            get => _targetFile;
            set
            {
                if (_targetFile != value)
                {
                    Log.Info("PropertyChange.TargetFile");
                    _targetFile = value;
                    OnPropertyChanged(nameof(TargetFile));
                }
            }
        }

        public string Notes
        {
            get => _notes;
            set
            {
                if (_notes != value)
                {
                    Log.Info("PropertyChange.Notes");
                    _notes = value;
                    OnPropertyChanged(nameof(Notes));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            Log.Info($"Save.Called - Property changed: {propertyName}");
            Save();
        }

        public void Save()
        {
            if (_renaming) return;

            if (string.IsNullOrEmpty(PlaylistFolderPath))
            {
                Log.Warning("Not Saved - PlaylistFolderPath is empty or null.");
                return;
            }

            CueManager.SaveCueToFile(this, PlaylistFolderPath);
            Log.Info("Saved successfully.");
        }

        private bool CheckForDuplicateCueFile(float newCueNumber)
        {
            if (!CueManager.StartupFinished)
            {
                Log.Info("In startup mode - Duplicate check skipped.");
                return true;
            }

            string newFilePath = Path.Combine(PlaylistFolderPath, $"cue_{newCueNumber}.json");

            if (!Directory.Exists(PlaylistFolderPath))
            {
                Log.Warning($"Playlist folder does not exist: {PlaylistFolderPath}");
                MessageBox.Show(
                    $"The playlist folder '{PlaylistFolderPath}' is missing. Please verify the location.",
                    "Folder Not Found",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                return false;
            }
            if (File.Exists(newFilePath))
            {
                Log.Warning($"Duplicate file found: {newFilePath}");

                // Prompt user for action
                var result = MessageBox.Show(
                    $"A cue with the number {newCueNumber} already exists. Do you want to replace it or skip this operation?",
                    "File Conflict",
                    MessageBoxButton.YesNoCancel,
                    MessageBoxImage.Warning
                );

                switch (result)
                {
                    case MessageBoxResult.Cancel:
                        Log.Info("User canceled the operation.");
                        return false;

                    case MessageBoxResult.No:
                        Log.Info("User chose to skip the replacement.");
                        return false;

                    case MessageBoxResult.Yes:
                        try
                        {
                            File.Delete(newFilePath);
                            Log.Info($"Existing file deleted: {newFilePath}");
                        }
                        catch (Exception ex)
                        {
                            Log.Error($"Error deleting existing file '{newFilePath}': {ex.Message}");
                            MessageBox.Show(
                                $"Could not delete the existing file '{newFilePath}'. Check file permissions or try again.",
                                "Delete Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error
                            );
                            return false;
                        }
                        break;
                }
            }

            Log.Info("No duplicate file found. Proceeding.");
            return true;
        }


        private void RenameCueFile(float oldCueNumber, float newCueNumber)
        {
            if (!CueManager.StartupFinished)
            {
                Log.Info("In startup mode - Rename skipped.");
                return;
            }

            _renaming = true;

            string oldFilePath = Path.Combine(PlaylistFolderPath, $"cue_{oldCueNumber}.json");
            string newFilePath = Path.Combine(PlaylistFolderPath, $"cue_{newCueNumber}.json");

            try
            {
                if (File.Exists(newFilePath))
                {
                    Log.Warning($"File already exists at {newFilePath}. Deleting the existing file.");
                    File.Delete(newFilePath);
                }

                if (File.Exists(oldFilePath))
                {
                    File.Move(oldFilePath, newFilePath);
                    _cueFilePath = newFilePath;
                    Log.Info($"Cue file renamed successfully from {oldFilePath} to {newFilePath}");
                }
                else
                {
                    Log.Warning($"Old cue file not found: {oldFilePath}");
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Error renaming file: {ex.Message}");
            }
            finally
            {
                _renaming = false;
            }
        }
    }
}
