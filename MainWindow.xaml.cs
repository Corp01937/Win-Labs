using Microsoft.Win32;
using NAudio.Wave;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Win_Labs;

namespace Win_Labs
{
    public partial class MainWindow : Window, IDisposable
    {
        private string _cueFilePath; // Declare the _cueFilePath variable
        private string _playlistFolderPath;
        private float CueNumber;
        public string GetCurrentTime()
        {
            return DateTime.Now.ToString("yy-MM-dd HH:mm:ss");
        }

        public void Log(string message)
        {
            string timestamp = GetCurrentTime();
            Console.WriteLine($"[{timestamp}] Message: {message}");
        }

        private WaveOutEvent? waveOut; // Nullable
        private AudioFileReader? audioFileReader; // Nullable
        public static List<Cue> _cueList = new List<Cue>();
        private string DefaultCueFilePath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"resources\defaultCue.json");
        private Cue _currentCue = new Cue();
        private bool ShowMode;
        private float CueNumberSelected = 0;
        private ObservableCollection<Cue> _cues = new ObservableCollection<Cue>();

        public MainWindow(string playlistFolderPath)
        {
            InitializeComponent();
            Log("MainWindow.Initialized");
            _playlistFolderPath = playlistFolderPath;
            Log($"Playlist Folder Path: {_playlistFolderPath}");
            CueListView.ItemsSource = _cues;
            LoadCues();
            Log("Cues.Loaded");
            InitializeCueData();
            Log("CueData.Initialized");
            DataContext = _currentCue;
            Log("UI.Binded_To.Cue");
            _currentCue.PropertyChanged += CurrentCue_PropertyChanged;
            RefreshCues();
        }

        private void RefreshCues()
        {
            string cueFilePath = Path.Combine(_playlistFolderPath, $"cue_{CueNumberSelected}.json");
            Log("RefreshCues.Called");
            try
            {
                if (CueListView.SelectedItem is Cue selectedCue)//Save selected cue
                {
                    CueManager.SaveCueToFile(selectedCue, _playlistFolderPath);
                }

                //Load New cues
                var newCues = CueManager.LoadCues(_playlistFolderPath);
                _cues.Clear();// Clear existing cues
                foreach (var cue in newCues)
                {
                    cue.PlaylistFolderPath = _playlistFolderPath; // Set PlaylistFolderPath
                    Log($"Loaded Cue: {cue.CueNumber}");
                    _cues.Add(cue);
                }

                // Update the ListView
                CueListView.ItemsSource = null; // Reset the ItemsSource to force UI update
                CueListView.ItemsSource = _cues;

                Log("Cues refreshed and UI updated.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while refreshing the cues: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Log($"Error refreshing cues: {ex.Message}");
            }
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

                if (CueManager.validJsonFileInPlaylist == true)
                {
                    if (!File.Exists(cueFilePath))
                    {
                        // Log and copy default cue file if not exists
                        Log($"Cue file not found at {cueFilePath}. Copying default cue file.");
                        CopyDefaultCueFile(cueFilePath);
                    }
                }
                else
                {
                    Log("A valid file was found in the playlist default cue not coppied");
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
                Log($"Error initializing cue data: {ex.Message}");
                MessageBox.Show($"Error initializing cue data: {ex.Message}", "Initialization Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }




        private void CopyDefaultCueFile(string destinationPath)
        {
            
            try
            {
                Log("DefaultCueFilePath: " + DefaultCueFilePath);
                Log("DestinationPath: " + destinationPath);

                if (!Directory.Exists(_playlistFolderPath))
                {
                    Directory.CreateDirectory(_playlistFolderPath);
                }

                if (File.Exists(DefaultCueFilePath))
                {
                    File.Copy(DefaultCueFilePath, destinationPath, true);
                    Log("Default cue file copied to " + destinationPath);
                }
                else
                {
                    MessageBox.Show("Default cue file not found.", "File Not Found", MessageBoxButton.OK, MessageBoxImage.Error);
                    Log("Default cue file not found at " + DefaultCueFilePath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error copying default cue file: {ex.Message}", "File Copy Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Log("Error copying default cue file: " + ex.Message);
            }
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            Log("NAudio.Play");
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Audio files (*.mp3;*.wav)|*.mp3;*.wav"
            };

            Log("Dialog.Attempted");

            if (openFileDialog.ShowDialog() == true)
            {
                Log("Succeeded");
                try
                {
                    CleanupAudio();

                    audioFileReader = new AudioFileReader(openFileDialog.FileName);
                    waveOut = new WaveOutEvent();

                    if (waveOut != null && audioFileReader != null)
                    {
                        waveOut.Init(audioFileReader);
                        waveOut.Play();
                        CurrentTrack.Text = $"Playing: {Path.GetFileName(openFileDialog.FileName)}";
                        Log("Success");
                    }
                    else
                    {
                        MessageBox.Show("Error initializing audio playback.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        Log("Error.Playback");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error playing the track: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Log("Error.Playing");
                }
            }
        }

        private void CleanupAudio()
        {
            waveOut?.Dispose();
            audioFileReader?.Dispose();
            waveOut = null;
            audioFileReader = null;
            CurrentTrack.Text = "No Track Selected";
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            waveOut?.Pause();
            Log("NAudio.Paused");
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            waveOut?.Stop();
            CleanupAudio();
            Log("NAudio.Stopped");
        }

        private void EditModeToggle_Click(object sender, RoutedEventArgs e)
        {
            Log("EditModeToggle.Toggled");
            if (EditModeToggle.IsChecked == true)
            {
                EditModeToggle.Content = "Show Mode";
                ShowMode = true;
                Log("Show Mode");
            }
            else
            {
                EditModeToggle.Content = "Edit Mode";
                ShowMode = false;
                Log("Edit Mode");
            }
        }

        public void Dispose()
        {
            waveOut?.Dispose();
            audioFileReader?.Dispose();
            Log("NAudio.Dispose");
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Dispose();
            Log("Window.Dispose");
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            Log("Close.Detected");
            base.OnClosing(e);
            Log("Trying to save all cue data");
            SaveAllCues();
        }

        public void SaveAllCues()
        {
            foreach (var cue in _cues)
            {
                SaveCueToFile(cue);
            }
            Log("All cues saved.");
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

                Log($"Cue {cue.CueNumber} saved successfully to {filePath}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving cue {cue.CueNumber}: {ex.Message}", "Save Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Log($"Error saving cue {cue.CueNumber}: {ex.Message}");
            } 
        }


        private void SaveCueData(float cueNumber)
        {
            _cueFilePath = Path.Combine(_playlistFolderPath, $"cue_{cueNumber}.json");

            var cue = _cues.FirstOrDefault(c => c.CueNumber == cueNumber);
            if (cue != null)
            {
                Log($"Saving cue data for cue number {cueNumber} to {_cueFilePath}");
                CueManager.SaveCueToFile(cue, _playlistFolderPath);
            }
            else
            {
                Log($"No cue found with number {cueNumber}");
            }
        }


        private void LoadCueData()
        {
            _cueFilePath = Path.Combine(_playlistFolderPath, $"cue_{CueNumberSelected}.json");

            Log("Attempting to load data");
            try
            {
                if (File.Exists(_cueFilePath))
                {
                    Log($"File exists at {_cueFilePath}");
                    string json = File.ReadAllText(_cueFilePath);
                    Cue loadedCue = JsonConvert.DeserializeObject<Cue>(json);

                    _currentCue = loadedCue ?? new Cue();
                    Log("Data loaded successfully");
                }
                else
                {
                    _currentCue = new Cue();
                    Log("File does not exist, new Cue created");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Load Error", MessageBoxButton.OK, MessageBoxImage.Error);
                _currentCue = new Cue();
                Log($"Error loading data: {ex.Message}");
            }
        }


        private void LoadCues()
        {
            _cues = CueManager.LoadCues(_playlistFolderPath);
            CueListView.ItemsSource = _cues;
        }

        private void CueListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CueListView.SelectedItem is Cue selectedCue)
            {
                // Update the DataContext with the selected cue
                DataContext = selectedCue;

                // Initialize _cueFilePath when a cue is selected
                _cueFilePath = Path.Combine(_playlistFolderPath, $"cue_{selectedCue.CueNumber}.json");
                Log($"Cue selected: {selectedCue.CueNumber}, file path: {_cueFilePath}");
            }
        }



        private void CreateNewCue_Click(object sender, RoutedEventArgs e)
        {
            // Pass the _playlistFolderPath when creating a Cue instance
            Cue newCue = new Cue(_playlistFolderPath)
            {
                CueNumber = _cues.Count + 1, // This will be the new cue number
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

            // Save the new cue
            CueManager.SaveCueToFile(newCue, _playlistFolderPath);
        }



        private void DeleteSelectedCue_Click(object sender, RoutedEventArgs e)
        {
            if (CueListView.SelectedItem is Cue selectedCue)
            {
                Log("DeleteSelectedCue_Click.Attempted");

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
                    Log($"File deleted: {filePath}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to delete file {filePath}: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Log("File.Fail_Delete");
                }
            }
            else
            {
                Log($"File does not exist: {filePath}");
            }
        }

        private void SaveMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Log("Save.Clicked");
            CueManager.SaveAllCues(_cues, _playlistFolderPath);
        }

        private void ExportMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Log("Export.Clicked");
            // Implement export logic here.
        }

        private void ImportMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Log("Import.Clicked");
            // Implement import logic here.
        }

        private void CloseMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Log("Close.Clicked");
            Log("Starting Close Protocol");
            SaveAllCues();
            Log("Closing.Window");
            this.Close();
        }

        private void SelectTargetFile_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Audio Files|*.mp3;*.wav;*.flac|All Files|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                if (CueListView.SelectedItem is Cue selectedCue)
                {
                    selectedCue.TargetFile = openFileDialog.FileName;
                    DataContext = selectedCue;
                    CueManager.SaveCueToFile(selectedCue, _playlistFolderPath);
                    Log($"'{openFileDialog.FileName}' added to {CueListView.SelectedItem}");
                }
                else
                {
                    MessageBox.Show("No cue selected. Please select a cue before choosing a target file.", "No Cue Selected", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {

            Log("Saving");
            SaveAllCues();
            Log("Refreshing");
            RefreshCues();
            Log("Cues.Refreshed");
        }

        private void NewCueButton_Click(object sender, RoutedEventArgs e)
        {
            // Pass the _playlistFolderPath when creating a Cue instance
            Cue newCue = new Cue(_playlistFolderPath)
            {
                CueNumber = _cues.Count + 1, // This will be the new cue number
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

            // Save the new cue
            CueManager.SaveCueToFile(newCue, _playlistFolderPath);
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
