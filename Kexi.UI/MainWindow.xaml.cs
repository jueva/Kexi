using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Kexi.Common;
using Kexi.Common.KeyHandling;
using Kexi.Shell;
using Kexi.ViewModel;
using Kexi.ViewModel.Commands;
using Kexi.ViewModel.Lister;
using MahApps.Metro.Controls;

namespace Kexi.UI
{
    public partial class MainWindow : MetroWindow
    {
        public MainWindow(Workspace workspace)
        {
            InitializeComponent();
            Workspace = workspace;
            DataContext = workspace;
            Workspace.ThemeHandler.ChangeTheme(Workspace.Options.Theme);
            mainWindow.GotFocus += RegisterHotKey;
            Workspace.Manager = mainWindow.DockManager.DockingManager;
            Workspace.LeftPane = mainWindow.DockManager.leftAnchorPane;
            Workspace.RightPane = mainWindow.DockManager.rightAnchorPane;
        }

        private const string LayoutConfig = @".\lastLayout.config";

        private void MainWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            var options = Workspace.Options;
            if (options.LoadLastLayout)
                Workspace.DockingMananger.SerializeLayout(LayoutConfig);
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var options = Workspace.Options;
            if (options.LoadLastLayout && File.Exists(LayoutConfig) && new FileInfo(LayoutConfig).Length > 0)
                Workspace.DockingMananger.DeserializeLayout(LayoutConfig);
            else
                OpenFavorites();
        }

        private void OpenFavorites()
        {
            var fileLister = KexContainer.Resolve<FileLister>();
            fileLister.Path = Environment.GetFolderPath(Environment.SpecialFolder.Favorites);
            Workspace.Open(fileLister);
            fileLister.Refresh();
        }

        private static Workspace Workspace { get; set; }

        private void RegisterHotKey(object sender, EventArgs ea)
        {
            new GlobalHotkeyHandler(mainWindow, Workspace).Register();
            mainWindow.GotFocus -= RegisterHotKey;
        }

        private void RegisterNotification(object sender, EventArgs ea)
        {
            new WinNotificationHandler(mainWindow, Workspace.NotificationHost).Register();
            mainWindow.SourceInitialized -= RegisterNotification;
        }

        private void MainWindow_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (!(e.OriginalSource is TextBox))
                Workspace.KeyHandler.Execute(e, Workspace.ActiveLister);
        }
    }
}