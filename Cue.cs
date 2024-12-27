using NAudio.Wave;
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

        public static bool IsInitializing { get; set; }
        public string PlaylistFolderPath { get; set; }

        private static readonly object _lock = new object();

        public float CueNumber
        {
            get => _cueNumber;
            set
            {
                float oldCueNumber = value;
                lock (_lock)
                {
                    if (_cueNumber != value)
                    {
                        Log.Info($"PropertyChange.CueNumber - Attempting to set CueNumber to {value}");
                        bool proceed = !IsInitializing && !_renaming && CheckForDuplicateCueFile(value);

                        if (proceed)
                        {
                            RenameCueFile(oldCueNumber, value);
                            _cueNumber = value;
                            OnPropertyChanged(nameof(CueNumber));
                            Log.Info($"CueNumber set to {value}");

                        }
                        else
                        {
                            _cueNumber = oldCueNumber;
                            Log.Info("User chose not to replace the existing cue file.");
                        }
                    }
                    else
                    {
                        OnPropertyChanged(nameof(CueNumber));
                        Log.Info($"CueNumber set to {value}");
                    }
                }
            }
        }

        private TimeSpan _totalDuration;
        public event PropertyChangedEventHandler PropertyChanged;

        public Cue(string playlistFolderPath)
        {
            PlaylistFolderPath = playlistFolderPath;
        }

        public Cue() : this(string.Empty) { }


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
        private string _displayedDuration;
        private bool _isEditingDuration = false;

        public string Duration
        {
            get => _displayedDuration ?? $"{_totalDuration.Minutes:D2}:{_totalDuration.Seconds:D2}:{_totalDuration.Milliseconds:D3}";
            set
            {
                if (_isEditingDuration)
                {
                    _displayedDuration = value; // Temporarily store user input
                    OnPropertyChanged(nameof(Duration));
                    return;
                }

                if (TimeSpan.TryParseExact(value, @"mm\:ss\.ff", null, out TimeSpan parsedDuration))
                {
                    _totalDuration = parsedDuration;
                    _displayedDuration = value;
                    Log.Info($"Duration successfully updated to: {value}");
                }
                else
                {
                    Log.Warning($"Invalid duration format entered: {value}. Retaining previous value.");
                }

                OnPropertyChanged(nameof(Duration));
            }
        }
        public void Duration_GotFocus()
        {
            _isEditingDuration = true; // Mark as editing
            Log.Info("User started editing the Duration field.");
            OnPropertyChanged(nameof(Duration)); // Update the binding
        }

        public void Duration_LostFocus()
        {
            _isEditingDuration = false; // Mark as no longer editing
            Log.Info("User finished editing the Duration field.");
            OnPropertyChanged(nameof(Duration)); // Update the binding
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
                    UpdateDuration();
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


        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            Log.Info($"Save.Called - Property changed: {propertyName}");
            if (!IsInitializing)
            {
                Save();
            }

        }
        private void UpdateDuration()
        {
            if (_isEditingDuration)
            {
                Log.Info("Skipping duration update because the user is editing the Duration field.");
                return;
            }

            if (string.IsNullOrEmpty(_targetFile) || !File.Exists(_targetFile))
            {
                Duration = "00:00:00:00";
                Log.Warning("TargetFile is invalid or does not exist. Duration set to 00:00:00:00");
                return;
            }

            try
            {
                using (var audioFileReader = new AudioFileReader(_targetFile))
                {
                    var totalDuration = audioFileReader.TotalTime;
                    Duration = $"{(int)totalDuration.TotalHours:D2}:{totalDuration.Minutes:D2}:{totalDuration.Seconds:D2}:{totalDuration.Milliseconds / 10:D2}";
                    Log.Info($"Duration updated for TargetFile {_targetFile}: {Duration}");
                }
            }
            catch (Exception ex)
            {
                Duration = "Error";
                Log.Error($"Error calculating duration for file '{_targetFile}': {ex.Message}");
            }
        }

        public void Save()
        {
            if (_renaming || string.IsNullOrEmpty(PlaylistFolderPath)) 
            { 
                Log.Warning("Not Saved - PlaylistFolderPath is empty or null."); 
                return; 
            }
            CueManager.SaveCueToFile(this, PlaylistFolderPath);
            Log.Info("Saved successfully.");
        }
        private bool CheckForDuplicateCueFile(float newCueNumber)
        {
            if(Cue.IsInitializing == true) { return false; }
            string newFilePath = Path.Combine(PlaylistFolderPath, $"cue_{newCueNumber}.json");

            if (!Directory.Exists(PlaylistFolderPath))
            {
                Log.Warning($"Playlist folder does not exist: {PlaylistFolderPath}");
                return false;
            }

            if (File.Exists(newFilePath))
            {
                var result = MessageBox.Show(
                    $"Cue {newCueNumber} already exists. Replace it?",
                    "Duplicate File",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning
                );

                if (result == MessageBoxResult.No)
                {
                    newFilePath = null;
                    Log.Info("User chose not to replace the file.");
                    return false;
                }
                else
                {
                    try
                    {
                        File.Delete(newFilePath);
                        Log.Info($"Existing file deleted: {newFilePath}");
                        newFilePath = null;
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"Error deleting file: {ex.Message}");
                        return false;
                    }
                }
            }

            return true;
        }

        private void RenameCueFile(float oldCueNumber, float newCueNumber)
        {
            string oldFilePath = Path.Combine(PlaylistFolderPath, $"cue_{oldCueNumber}.json");
            string newFilePath = Path.Combine(PlaylistFolderPath, $"cue_{newCueNumber}.json");

            try
            {

                if (File.Exists(oldFilePath))
                {
                    File.Move(oldFilePath, newFilePath);
                    _cueFilePath = newFilePath;
                    Log.Info($"File renamed: {oldFilePath} -> {newFilePath}");
                }
                else
                {
                    Log.Warning($"Old cue file not found: {oldFilePath}");
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }

        }


    }
}
