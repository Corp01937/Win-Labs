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

        private void LaunchDebug()
        {
            Launched = true;

        }

        private void Routing()
        {
            
            Log.log("Routing.Initialised", Log.LogLevel.Info);
        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            Log.log("Exception.Caught", Log.LogLevel.Warning);
            MessageBox.Show("An unhandled exception just occurred: " + e.Exception.Message, "Exception Sample", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Log.logFile();
            Log.log("Program.Start", Log.LogLevel.Info);
            LaunchDebug();
            Log.log("Creating.StartupWindow", Log.LogLevel.Info);

            // Create the startup window
            StartupWindow startupWindow = new StartupWindow();

            // Initialize Routing
            Routing();

            // Show the startup window
            Log.log("Showing.StartupWindow", Log.LogLevel.Info);
            startupWindow.Show();
        }
    }
}
