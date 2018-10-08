using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using Kexi.ViewModel;

namespace Kexi.UI
{
    public partial class App : Application
    {
        private Workspace _workspace;

        private void App_Startup(object sender, StartupEventArgs e)
        {
            Assembly.Load("Kexi.Extensions");
            _workspace = KexContainer.Resolve<Workspace>();

            var mainWindow = new MainWindow(_workspace);
            _workspace.DockingMananger   =  new AvalonDockingManager(_workspace, mainWindow.DockManager.DockingManager);
            DispatcherUnhandledException += App_DispatcherUnhandledException;
            mainWindow.Show();
            if (!_workspace.Options.IsInitialized)
            {
                var setup = new SetupWindow(_workspace);
                setup.ShowDialog();
                _workspace.FocusListView();
            }
        }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            _workspace.NotificationHost.AddError("Unhandled: " + e.Exception.Message, e.Exception);
        }
    }
}