using System;
using System.Windows;

namespace Win_Labs
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        bool Launched = false;
        public string GetCurrentTime()
        {
            return DateTime.Now.ToString("yy-MM-dd HH:mm:ss");
        }

        public void Log(string message)
        {
            string timestamp = GetCurrentTime();
            Console.WriteLine($"[{timestamp}] Message: {message}");
        }
        private void LaunchDebug()
        {
            Launched = true;

        }

        private void Routing()
        {
            Log("Routing.Initialised");
        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            Log("Exception.Caught");
            MessageBox.Show("An unhandled exception just occurred: " + e.Exception.Message, "Exception Sample", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Log("Program.Start");
            LaunchDebug();
            Log("Creating.StartupWindow");

            // Create the startup window
            StartupWindow startupWindow = new StartupWindow();

            // Initialize Routing
            Routing();

            // Show the startup window
            Log("Showing.StartupWindow");
            startupWindow.Show();
        }
    }
}
