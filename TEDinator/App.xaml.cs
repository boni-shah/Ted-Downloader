using System.Diagnostics;
using System.Windows;
using TEDinator.TEDClasses;

namespace TEDinator
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            // Get Reference to the current Process
            Process thisProc = Process.GetCurrentProcess();
            // Check how many total processes have the same name as the current one
            if (Process.GetProcessesByName(thisProc.ProcessName).Length > 1)
            {
                // If ther is more than one, than it is already running.
                MessageBox.Show(Constants.OneInstanceAllowedmsg, Constants.UserErrormsg);
                Application.Current.Shutdown();
                return;
            }
            base.OnStartup(e);
        }
    }
}
