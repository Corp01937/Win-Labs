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
            Console.WriteLine("Launching 'App.xaml.cs'");
        }

        private void Routing()
        {
            Console.WriteLine("Routing.Initialised");
        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            Console.WriteLine("Exception.Caught");
            MessageBox.Show("An unhandled exception just occurred: " + e.Exception.Message, "Exception Sample", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            LaunchDebug();
            Console.WriteLine("Creating.StartupWindow");

            // Create the startup window
            StartupWindow startupWindow = new StartupWindow();

            // Initialize Routing
            Routing();

            // Show the startup window
            Console.WriteLine("Showing.StartupWindow");
            startupWindow.Show();
        }
    }
}
