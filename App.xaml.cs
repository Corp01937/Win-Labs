using System.Configuration;
using System.Data;
using System.Windows;

namespace Win_Labs
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    /// 


    public partial class App : Application
    {
        bool Lauched = false;
        private void LaunchDebug()
        {
            Lauched = true;
            Console.WriteLine("Launching.'App.xaml.cs'");
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
            Console.WriteLine("Creating.MainWindow");
            // Create the startup window
            MainWindow wnd = new MainWindow();
            // Do stuff here, e.g. to the window
            Console.WriteLine("Setting.Title");
            wnd.Title = "Win-Labs";
            //Initailse Routing
            Routing();
            // Show the window
            Console.WriteLine("Showing.Window");
            wnd.Show();

        }


    }
}
