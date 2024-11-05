using Microsoft.Win32;
using NAudio.Wave;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Win_Labs;

namespace Win_Labs
{
    public partial class MainWindow : Window
    {
        private string _cueFilePath; // Declare the _cueFilePath variable
        private string _playlistFolderPath;
        private float CueNumber;
        private WaveOutEvent? waveOut; // Nullable
        private AudioFileReader? audioFileReader; // Nullable
        public static List<Cue> _cueList = [];
        private string DefaultCueFilePath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"resources\defaultCue.json");
        private Cue _currentCue = new Cue();
        private bool ShowMode;
        private float CueNumberSelected = 0;
        private ObservableCollection<Cue> _cues = new ObservableCollection<Cue>();

        public MainWindow(string playlistFolderPath)
        {
            InitializeComponent();
            Log.log("MainWindow.Initialized");
            _playlistFolderPath = playlistFolderPath;
            Log.log($"Playlist Folder Path: {_playlistFolderPath}");
            CueListView.ItemsSource = _cues;
            LoadCues();
            InitializeCueData();
            Log.log("CueData.Initialized");
            DataContext = _currentCue;
            Log.log("UI.Binded_To.Cue");
            _currentCue.PropertyChanged += CurrentCue_PropertyChanged;
        }


        private void CurrentCue_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is Cue cue)
            {
                if (e.PropertyName == nameof(cue.CueNumber))
                {
                    if (cue.CueNumber > 0)
                    {
                        string filePath = Path.Combine(_playlistFolderPath, $"cue_{cue.CueNumber}.json");
                        cue.SetFilePath(filePath);
                        CueManager.SaveCueToFile(cue, _playlistFolderPath);
                    }
                }
            }
        }


        private void InitializeCueData()
        {
            try
            {
                // Set the file path for the current selected cue or default cue
                string cueFilePath = Path.Combine(_playlistFolderPath, $"cue_{CueNumberSelected}.json");

                if (CueManager.validJsonFileInPlaylist == false)
                {
                    if (!File.Exists(cueFilePath))
                    {
                        // Log and copy default cue file if not exists
                        Log.log($"Cue file not found at {cueFilePath}. Copying default cue file.", Log.LogLevel.Warning);
                        CopyDefaultCueFile(cueFilePath);
                        LoadCues();
                    }
                }
                else
                {
                    Log.log("A valid file was found in the playlist default cue not coppied", Log.LogLevel.Warning);
                }

                // Set _cueFilePath for the current cue
                _cueFilePath = cueFilePath;

                if (CueManager.validJsonFileInPlaylist == true)
                {
                    // Load the cue data
                    LoadCueData();
                } 
            }
            catch (Exception ex)
            {
                Log.log($"Error initializing cue data: {ex.Message}");
                MessageBox.Show($"Error initializing cue data: {ex.Message}", "Initialization Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void CopyDefaultCueFile(string destinationPath)
        {
            
            try
            {
                Log.log("DefaultCueFilePath: " + DefaultCueFilePath);
                Log.log("DestinationPath: " + destinationPath);

                if (!Directory.Exists(_playlistFolderPath))
                {
                    Directory.CreateDirectory(_playlistFolderPath);
                }

                if (File.Exists(DefaultCueFilePath))
                {
                    File.Copy(DefaultCueFilePath, destinationPath, true);
                    Log.log("Default cue file copied to " + destinationPath);
                }
                else
                {
                    MessageBox.Show("Default cue file not found.", "File Not Found", MessageBoxButton.OK, MessageBoxImage.Error);
                    Log.log("Default cue file not found at " + DefaultCueFilePath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error copying default cue file: {ex.Message}", "File Copy Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Log.log("Error copying default cue file: " + ex.Message);
            }
        }

        private void GoButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Add functionality");
            //Log.log("NAudio.Play");
            //var openFileDialog = new OpenFileDialog
            //{
            //    Filter = "Audio files (*.mp3;*.wav)|*.mp3;*.wav"
            //};
            ////wakatime test

            //Log.log("Dialog.Attempted");

            //if (openFileDialog.ShowDialog() == true)
            //{
            //    Log.log("Succeeded");
            //    try
            //    {
            //        CleanupAudio();

            //        audioFileReader = new AudioFileReader(openFileDialog.FileName);
            //        waveOut = new WaveOutEvent();

            //        if (waveOut != null && audioFileReader != null)
            //        {
            //            waveOut.Init(audioFileReader);
            //            waveOut.Play();
            //            CurrentTrack.Text = $"Playing: {Path.GetFileName(openFileDialog.FileName)}";
            //            Log.log("Success");
            //        }
            //        else
            //        {
            //            MessageBox.Show("Error initializing audio playback.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            //            Log.log("Error.Playback");
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        MessageBox.Show($"Error playing the track: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            //        Log.log("Error.Playing");
            //    }
            //}
        }

        private void CleanupAudio()
        {
            waveOut?.Dispose();
            audioFileReader?.Dispose();
            waveOut = null;
            audioFileReader = null;
            CurrentTrack.Text = "No Track Selected";
            Log.log("NAudio.Dispose_Complete");
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            waveOut?.Pause();
            Log.log("NAudio.Paused");
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            waveOut?.Stop();
            CleanupAudio();
            Log.log("NAudio.Stopped");
        }

        private void EditModeToggle_Click(object sender, RoutedEventArgs e)
        {
            Log.log("EditModeToggle.Toggled");
            if (EditModeToggle.IsChecked == true)
            {
                EditModeToggle.Content = "Show Mode";
                ShowMode = true;
                Log.log("Show Mode");
            }
            else
            {
                EditModeToggle.Content = "Edit Mode";
                ShowMode = false;
                Log.log("Edit Mode");
            }
        }


        protected override void OnClosing(CancelEventArgs e)
        {
            Log.log("Close.Detected");
            Log.log("Propting for confirmation");

            var result = MessageBox.Show(
                "Are you sure you want to close the program",
                "Have you saved tho??",
                MessageBoxButton.YesNo, 
                MessageBoxImage.Warning);
            if (result == MessageBoxResult.No) { Log.log("UserInput.No"); e.Cancel = true; ; } 
            else { 
                Log.log("UserInput.yes"); 
                Log.log("Closing");
                Log.log("Attemping to clean up audio player");
                CleanupAudio();
                Log.log("Trying to save all cue data");
                SaveAllCues();
                Log.log("Closed");
            }

        }

        public void SaveAllCues()
        {
            foreach (var cue in _cues)
            {
                SaveCueToFile(cue);
            }
            Log.log("All cues saved.");
        }

        private void SaveCueToFile(Cue cue)
        {
            _cueFilePath = Path.Combine(_playlistFolderPath, $"cue_{cue.CueNumber}.json");

            try
            {
                if (!Directory.Exists(_playlistFolderPath))
                {
                    Directory.CreateDirectory(_playlistFolderPath);
                }

                cue.PlaylistFolderPath = _playlistFolderPath; // Set PlaylistFolderPath

                string fileName = $"cue_{cue.CueNumber}.json";
                string filePath = Path.Combine(_playlistFolderPath, fileName);

                string json = JsonConvert.SerializeObject(cue, Formatting.Indented);
                File.WriteAllText(filePath, json);

                Log.log($"Cue {cue.CueNumber} saved successfully to {filePath}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving cue {cue.CueNumber}: {ex.Message}", "Save Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Log.log($"Error saving cue {cue.CueNumber}: {ex.Message}");
            } 
        }


        private void SaveCueData(float cueNumber)
        {
            _cueFilePath = Path.Combine(_playlistFolderPath, $"cue_{cueNumber}.json");

            var cue = _cues.FirstOrDefault(c => c.CueNumber == cueNumber);
            if (cue != null)
            {
                Log.log($"Saving cue data for cue number {cueNumber} to {_cueFilePath}");
                CueManager.SaveCueToFile(cue, _playlistFolderPath);
            }
            else
            {
                Log.log($"No cue found with number {cueNumber}");
            }
        }


        private void LoadCueData()
        {
            _cueFilePath = Path.Combine(_playlistFolderPath, $"cue_{CueNumberSelected}.json");

            Log.log("Attempting to load data");
            try
            {
                if (File.Exists(_cueFilePath))
                {
                    Log.log($"File exists at {_cueFilePath}");
                    string rawJSON = File.ReadAllText(_cueFilePath);
                    Cue loadedCue = JsonConvert.DeserializeObject<Cue>(rawJSON);

                    _currentCue = loadedCue ?? new Cue();
                    Log.log("Data loaded successfully");
                }
                else
                {
                    _currentCue = new Cue();
                    Log.log("File does not exist, new Cue created");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Load Error", MessageBoxButton.OK, MessageBoxImage.Error);
                _currentCue = new Cue();
                Log.log($"Error loading data: {ex.Message}");
            }
        }


        public void LoadCues()
        {
            _cues = CueManager.LoadCues(_playlistFolderPath);
            CueListView.ItemsSource = _cues;
        }

        //private void UnloadCues()
        //{
        //    _cues = CueManager.UnloadCues(_playlistFolderPath);
        //    CueListView.ItemsSource = _cues;
        //}

        private void CueListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CueListView.SelectedItem is Cue selectedCue)
            {
                // Update the DataContext with the selected cue
                DataContext = selectedCue;

                // Initialize _cueFilePath when a cue is selected
                _cueFilePath = Path.Combine(_playlistFolderPath, $"cue_{selectedCue.CueNumber}.json");
                Log.log($"Cue selected: {selectedCue.CueNumber}, file path: {_cueFilePath}");
            }
        }

        private void DeleteCue_Click(object sender, RoutedEventArgs e) 
        {
            Log.log("DeleteCue.ToolbarButton");
            DeleteCue();
        }
        private void CreateNewCue() 
        {
            // Pass the _playlistFolderPath when creating a Cue instance
            Cue newCue = new Cue(_playlistFolderPath)
            {
                PlaylistFolderPath = _playlistFolderPath,
                CueNumber = _cues.Count, // This will be the new cue number
                CueName = "New Cue",
                Duration = "00:00",
                PreWait = "00:00",
                PostWait = "00:00",
                AutoFollow = false,
                FileName = "",
                TargetFile = "",
                Notes = ""
            };

            // Add the new cue to the collection
            _cues.Add(newCue);
            CueListView.SelectedItem = newCue;
            DataContext = newCue;

            // Initialize _cueFilePath using the new cue's CueNumber
            _cueFilePath = Path.Combine(_playlistFolderPath, $"cue_{newCue.CueNumber}.json");
            //// Rename the file
            //File.Move(newCue, $"cue_{newCue.CueNumber}.json", true);
            //Log.log("New file made");
            //// Delete old file
            //File.Delete(oldFilePath);
            //Log.log("Old file delted");
            //// Update _cueFilePath to the new file path
            //_cueFilePath = newFilePath;

            // Save the new cue
            CueManager.SaveCueToFile(newCue, _playlistFolderPath);
            CreatingNewCue = false;
        }
        public static bool CreatingNewCue;
        private void CreateNewCue_Click(object sender, RoutedEventArgs e)
        {
            Log.log("Creating new cue");
            CreatingNewCue = true;
            CreateNewCue();
        }

        private void DeleteSelectedCue_Click(object sender, RoutedEventArgs e)
        {
            Log.log("DeleteCue.ContextMenu");
            DeleteCue();
        }
        private void DeleteCue()
        {
            if (CueListView.SelectedItem is Cue selectedCue)
            {
                Log.log("DeleteSelectedCue_Click.Attempted");

                string cueFilePath = Path.Combine(_playlistFolderPath, $"cue_{selectedCue.CueNumber}.json");

                DeleteFileIfExists(cueFilePath);

                _cues.Remove(selectedCue);

                DataContext = _cues.FirstOrDefault() ?? new Cue();
            }
            else
            {
                MessageBox.Show("No cue selected to delete.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteFileIfExists(string filePath)
        {
            if (File.Exists(filePath))
            {
                try
                {
                    File.Delete(filePath);
                    Log.log($"File deleted: {filePath}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to delete file {filePath}: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Log.log("File.Fail_Delete");
                }
            }
            else
            {
                Log.log($"File does not exist: {filePath}");
            }
        }

        private void SaveMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Log.log("Save.Clicked");
            CueManager.SaveAllCues(_cues, _playlistFolderPath);
        }

        private void ExportMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Log.log("Export.Clicked");
            var folderDialog = new System.Windows.Forms.FolderBrowserDialog();
            if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Log.log("Dialog.Opened");
                string playlistExportFolderPath = folderDialog.SelectedPath;
                Log.log("Selected Export Path: " + playlistExportFolderPath);
                export.createZIP(playlistExportFolderPath);
            }
        }

        private void ImportMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Log.log("Import.Clicked");
            // Implement import logic here.
        }

        private void CloseMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Log.log("Close.Clicked");
            Log.log("Starting Close Protocol");
            SaveAllCues();
            Log.log("Closing.Window");
            this.Close();
        }

        private void SelectTargetFile_Click(object sender, RoutedEventArgs e)
        {
            Log.log("SelectTargetFile.Clicked");
            SelectTargetFile();
        }
        public void SelectTargetFile()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Audio Files|*.mp3;*.wav;*.flac|All Files|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                if (CueListView.SelectedItem is Cue selectedCue)
                {
                    string fileNameWoutextention = Path.GetFileNameWithoutExtension(openFileDialog.FileName);
                    selectedCue.TargetFile = openFileDialog.FileName;
                    selectedCue.FileName = fileNameWoutextention;
                    DataContext = selectedCue;
                    CueManager.SaveCueToFile(selectedCue, _playlistFolderPath);
                    Log.log($"'{openFileDialog.FileName}' added to {CueListView.SelectedItem}");
                }
                else
                {
                    MessageBox.Show("No cue selected. Please select a cue before choosing a target file.", "No Cue Selected", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {

            Log.log("Saving");
            SaveAllCues();
            MessageBox.Show("Refresh feature in future update.");
            //Log.log("Refreshing");
            
            //Log.log("Cues.Refreshed");
        }



    }
}
