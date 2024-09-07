using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NAudio.Wave;

namespace Win_Labs
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IDisposable
    {
        private WaveOutEvent? waveOut; //Nullable
        private AudioFileReader? audioFileReader; //Nullabe

        public MainWindow()
        {
            InitializeComponent();
            Console.WriteLine("MainWindow.Initialized");
            //IconLoader();
            //Console.WriteLine("IconLoader.Called");
        }

        //private void IconLoader()
        //{
        //    Icon = new BitmapImage(new Uri("C:\\Users\\xander\\OneDrive - Furze Platt Senior School\\Computing\\Course WOrk\\Win-Labs_Code\\Win-Labs\\resources\\Icons\\Win_Labs_logo.ico", UriKind.Relative));
        //    Console.WriteLine("Icon.Set");
        //}

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "Audio files (*.mp3;*.wav)|*.mp3;*.wav";

            if (openFileDialog.ShowDialog() == true)
            {
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
                        }
                        else
                        {
                            MessageBox.Show("Error initializing audio playback.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error playing the track: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            waveOut?.Pause();
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            waveOut?.Stop();
            waveOut?.Dispose();
            audioFileReader?.Dispose();
            waveOut = null;
            audioFileReader = null;
            CurrentTrack.Text = "No Track Selected";
        }

        private void EditModeToggle_Click(object sender, RoutedEventArgs e)
        {
            if (EditModeToggle.IsChecked == true)
            {
                EditModeToggle.Content = "Show Mode";
            }
            else
            {
                EditModeToggle.Content = "Edit Mode";
            }
        }

        // Event handler for Minimize button
        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        // Event handler for Maximize button
        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = (this.WindowState == WindowState.Maximized)
                ? WindowState.Normal
                : WindowState.Maximized;
        }

        // Event handler for Close button
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public void Dispose()
        {
            waveOut?.Dispose();
            audioFileReader?.Dispose();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Dispose();
        }
    }
}

