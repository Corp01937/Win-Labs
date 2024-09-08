using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NAudio.Wave;
using System.ComponentModel;
using System.IO;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using Microsoft.Win32;


namespace Win_Labs
{
    public partial class MainWindow : Window, IDisposable
    {
        private WaveOutEvent? waveOut; // Nullable
        private AudioFileReader? audioFileReader; // Nullable
        public static List<Cue> _cueList = new List<Cue>();
        private string DefaultCueFilePath => System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"resources\defaultCue.json");
        private string _playlistFolderPath;
        
        private Cue _currentCue = new Cue();
        private bool ShowMode;
        private float CueNumberSelected = 0; // Initialize with default value
        private ObservableCollection<Cue> _cues = new ObservableCollection<Cue>();

        public MainWindow(string playlistFolderPath)
        {
            InitializeComponent();
            _playlistFolderPath = playlistFolderPath;
            CueListView.ItemsSource = _cueList;
            CueListView.ItemsSource = _cues; // Initialize ListView with the cue
            LoadCues(); // Load cues from the playlist folder
            Console.WriteLine("MainWindow.Initialized");
            InitializeCueData();
            DataContext = _currentCue; // Bind the UI to the Cue object
            Console.WriteLine("UI.Binded_To.Cue");
        }


        private void InitializeCueData()
        {
            string cueFilePath = System.IO.Path.Combine(_playlistFolderPath, $"cue_{CueNumberSelected}.json");

            // Check if the cue file exists; if not, copy the default cue file
            if (!File.Exists(cueFilePath))
            {
                CopyDefaultCueFile(cueFilePath);
            }

            // Load the cue data
            LoadCueData();
        }



        private void CopyDefaultCueFile(string destinationPath)
        {
            try
            {
                // Log the DefaultCueFilePath
                Console.WriteLine("DefaultCueFilePath: " + DefaultCueFilePath);
                Console.WriteLine("DestinationPath: " + destinationPath);

                // Ensure the directory exists
                if (!Directory.Exists(_playlistFolderPath))
                {
                    Directory.CreateDirectory(_playlistFolderPath);
                }

                // Log whether the default cue file exists
                if (File.Exists(DefaultCueFilePath))
                {
                    File.Copy(DefaultCueFilePath, destinationPath, true); // Overwrite if it exists
                    Console.WriteLine("Default cue file copied to " + destinationPath);
                }
                else
                {
                    MessageBox.Show("Default cue file not found.", "File Not Found", MessageBoxButton.OK, MessageBoxImage.Error);
                    Console.WriteLine("Default cue file not found at " + DefaultCueFilePath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error copying default cue file: {ex.Message}", "File Copy Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Console.WriteLine("Error copying default cue file: " + ex.Message);
            }
        }



        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("NAudio.Play");
            var openFileDialog = new Microsoft.Win32.OpenFileDialog();
            Console.WriteLine("Dialog.Attempted");
            openFileDialog.Filter = "Audio files (*.mp3;*.wav)|*.mp3;*.wav";
            Console.WriteLine("Dialog.Opened");

            if (openFileDialog.ShowDialog() == true)
            {
                Console.WriteLine("Succeeded");
                try
                {
                    if (waveOut == null || waveOut.PlaybackState != PlaybackState.Playing)
                    {
                        // Dispose of previous resources if any
                        waveOut?.Dispose();
                        audioFileReader?.Dispose();

                        // Re-initialize
                        audioFileReader = new AudioFileReader(openFileDialog.FileName);
                        waveOut = new WaveOutEvent();

                        // Null-check before initialization
                        if (waveOut != null && audioFileReader != null)
                        {
                            waveOut.Init(audioFileReader);
                            waveOut.Play();
                            CurrentTrack.Text = $"Playing: {System.IO.Path.GetFileName(openFileDialog.FileName)}";
                            Console.WriteLine("Success");
                        }
                        else
                        {
                            MessageBox.Show("Error initializing audio playback.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            Console.WriteLine("Error.Playback");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error playing the track: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Console.WriteLine("Error.Playing");
                }
            }
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            waveOut?.Pause();
            Console.WriteLine("NAudio.Paused");
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            waveOut?.Stop();
            waveOut?.Dispose();
            audioFileReader?.Dispose();
            waveOut = null;
            audioFileReader = null;
            CurrentTrack.Text = "No Track Selected";
            Console.WriteLine("NAudio.Stopped");
        }

        private void EditModeToggle_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("EditModeToggle.Toggled");
            if (EditModeToggle.IsChecked == true)
            {
                EditModeToggle.Content = "Show Mode";
                ShowMode = true;
                Console.WriteLine("Show Mode");
            }
            else
            {
                EditModeToggle.Content = "Edit Mode";
                ShowMode = false;
                Console.WriteLine("Edit Mode");
            }
        }

        public void Dispose()
        {
            waveOut?.Dispose();
            audioFileReader?.Dispose();
            Console.WriteLine("NAudio.Dispose");
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Dispose();
            Console.WriteLine("Window.Dispose");
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            Console.WriteLine("Close.Detected");
            base.OnClosing(e);
            Console.WriteLine("Trying to save all cue data");
            SaveAllCues(); // Save all cues
        }



        // Method to save the Cue data to a JSON file with the selected cue number
        private void SaveCueData(float cueNumber)
        {
            try
            {
                // Ensure the directory exists
                if (!Directory.Exists(_playlistFolderPath))
                {
                    Directory.CreateDirectory(_playlistFolderPath);
                }

                // Generate file name based on the provided cue number
                string fileName = $"cue_{cueNumber}.json";
                string filePath = System.IO.Path.Combine(_playlistFolderPath, fileName);

                // Serialize the _currentCue object to JSON
                string json = JsonConvert.SerializeObject(_currentCue, Formatting.Indented);

                // Write the JSON string to the specified file
                File.WriteAllText(filePath, json);
                Console.WriteLine($"Data saved successfully to {filePath}");
            }
            catch (Exception ex)
            {
                // Handle any errors that occur during saving
                MessageBox.Show($"Error saving data: {ex.Message}", "Save Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Console.WriteLine($"Error saving data: {ex.Message}");
            }
        }

        private void LoadCueData()
        {
            Console.WriteLine("Attempting to load data");
            try
            {
                // Generate file name based on CueNumberSelected
                string fileName = $"cue_{CueNumberSelected}.json";
                string filePath = System.IO.Path.Combine(_playlistFolderPath, fileName);

                if (File.Exists(filePath))
                {
                    Console.WriteLine($"File exists at {filePath}");
                    string json = File.ReadAllText(filePath);
                    Cue loadedCue = JsonConvert.DeserializeObject<Cue>(json);

                    if (loadedCue != null)
                    {
                        _currentCue = loadedCue;
                        Console.WriteLine("Data loaded successfully");
                    }
                    else
                    {
                        _currentCue = new Cue();
                        Console.WriteLine("New Cue created as file is empty or invalid");
                    }
                }
                else
                {
                    _currentCue = new Cue();
                    Console.WriteLine("File does not exist, new Cue created");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Load Error", MessageBoxButton.OK, MessageBoxImage.Error);
                _currentCue = new Cue();
                Console.WriteLine($"Error loading data: {ex.Message}");
            }
        }



        private void LoadCues()
        {
            // Load all cue files from the playlist folder
            var cueFiles = Directory.GetFiles(_playlistFolderPath, "*.json");

            foreach (var file in cueFiles)
            {
                try
                {
                    string json = File.ReadAllText(file);
                    Cue cue = JsonConvert.DeserializeObject<Cue>(json);

                    if (cue != null)
                    {
                        _cues.Add(cue);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading cue data from file {file}: {ex.Message}", "Load Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void CueListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CueListView.SelectedItem is Cue selectedCue)
            {
                _currentCue = selectedCue;
                DataContext = _currentCue; // Update DataContext to reflect the selected cue
                Console.WriteLine($"Selected Cue: {selectedCue.CueNumber}");
                SaveAllCues();
            }
        }

        public void SaveAllCues()
        {
            foreach (var cue in _cues)
            {
                if (float.TryParse(cue.CueNumber.ToString(), out float cueNumber))
                {
                    SaveCueData(cueNumber);
                    Console.WriteLine("Cue: " + cueNumber + " saved.");
                }
                else
                {
                    Console.WriteLine($"Invalid CueNumber format: {cue.CueNumber}");
                }
            }
        }


        private void CreateNewCue_Click(object sender, RoutedEventArgs e)
        {
            // Create a new Cue object
            Cue newCue = new Cue
            {
                CueNumber = _cues.Count + 1,
                CueName = "New Cue",
                Duration = "00:00",
                PreWait = "00:00",
                PostWait = "00:00",
                AutoFollow = false,
                FileName = "",
                TargetFile = "",
                Notes = ""
            };

            // Add the new cue to the ObservableCollection
            _cues.Add(newCue);

            //select the newly created cue
            CueListView.SelectedItem = newCue;
            DataContext = newCue;
        }

        private void DeleteSelectedCue_Click(object sender, RoutedEventArgs e)
        {
            // Get the currently selected cue
            if (CueListView.SelectedItem is Cue selectedCue)
            {
                // Delete associated files if they exist
                DeleteFileIfExists(selectedCue.FileName);
                DeleteFileIfExists(selectedCue.TargetFile);

                // Remove the selected cue from the ObservableCollection
                _cues.Remove(selectedCue);

                // clear the DataContext if no cue is selected
                if (_cues.Count > 0)
                {
                    DataContext = _cues[0]; // Set DataContext to the first cue if available
                }
                else
                {
                    DataContext = new Cue(); // Or handle the case when no cues are left
                }
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
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to delete file {filePath}: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // Save Menu Item Click Event
        private void SaveMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Save.Clicked");
            SaveAllCues();
        }

        // Export Menu Item Click Event
        private void ExportMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Export.Clicked");
            // Implement export logic here.
        }

        // Import Menu Item Click Event
        private void ImportMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Import.Clicked");
            // Implement import logic here.
        }
        private void SelectTargetFile_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Audio Files|*.mp3;*.wav;*.flac|All Files|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                // Set the TargetFile to the selected file's path
                _currentCue.TargetFile = openFileDialog.FileName;
            }
        }
    }
}
