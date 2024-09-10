using System;
using System.Windows;
using System.Windows.Input;

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
            
            Log.log("Routing.Initialised");
        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            Log.log("Exception.Caught");
            MessageBox.Show("An unhandled exception just occurred: " + e.Exception.Message, "Exception Sample", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Log.log("Program.Start");
            LaunchDebug();
            Log.log("Creating.StartupWindow");

            // Create the startup window
            StartupWindow startupWindow = new StartupWindow();

            // Initialize Routing
            Routing();

            // Show the startup window
            Log.log("Showing.StartupWindow");
            startupWindow.Show();
        }
    }
}
