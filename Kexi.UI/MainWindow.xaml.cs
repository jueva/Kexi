using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using Kexi.Common;
using Kexi.Shell;
using Kexi.ViewModel;
using Kexi.ViewModel.Lister;

namespace Kexi.UI
{
    internal enum AccentState
    {
        ACCENT_DISABLED = 1,
        ACCENT_ENABLE_GRADIENT = 0,
        ACCENT_ENABLE_TRANSPARENTGRADIENT = 2,
        ACCENT_ENABLE_BLURBEHIND = 3,
        ACCENT_INVALID_STATE = 4
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct AccentPolicy
    {
        public AccentState AccentState;
        public int AccentFlags;
        public int GradientColor;
        public int AnimationId;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct WindowCompositionAttributeData
    {
        public WindowCompositionAttribute Attribute;
        public IntPtr Data;
        public int SizeOfData;
    }

    internal enum WindowCompositionAttribute
    {
        WCA_ACCENT_POLICY = 19
    }

    public partial class MainWindow
    {
        [DllImport("user32.dll")]
        private static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);
        
        private const string LayoutConfig = @".\lastLayout.config";

        public MainWindow(Workspace workspace)
        {
            InitializeComponent();
            AllowsTransparency = true;
            Workspace   = workspace;
            DataContext = workspace;
            Workspace.ThemeHandler.ChangeTheme(Workspace.Options.Theme);
            mainWindow.GotFocus += RegisterHotKey;
            Workspace.Manager   =  mainWindow.DockManager.DockingManager;
            Loaded += MainWindow_Loaded1;
        }

        private void MainWindow_Loaded1(object sender, RoutedEventArgs e)
        {
            EnableBlur();
        }

        private static Workspace Workspace { get; set; }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var options = Workspace.Options;
            if (options.LoadLastLayout && File.Exists(LayoutConfig) && new FileInfo(LayoutConfig).Length > 0)
            {
                Workspace.DockingMananger.DeserializeLayout(LayoutConfig);
            }
            else
                OpenFavorites();
        }

        private async void OpenFavorites()
        {
            var fileLister = KexContainer.Resolve<FileLister>();
            fileLister.Path = Environment.GetFolderPath(Environment.SpecialFolder.Favorites);
            Workspace.Open(fileLister);
            await fileLister.Refresh().ConfigureAwait(false);
        }

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
                Workspace.KeyDispatcher.Execute(e, Workspace.ActiveLister);
        }

        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            if (Workspace.Options.LoadLastLayout)
                Workspace.DockingMananger.SerializeLayout(LayoutConfig);
        }

        #region Acrylic


        private void EnableBlur()
        {
            var windowHelper = new WindowInteropHelper(this);

            var accent = new AccentPolicy {AccentState = AccentState.ACCENT_ENABLE_BLURBEHIND};

            var accentStructSize = Marshal.SizeOf(accent);

            var accentPtr = Marshal.AllocHGlobal(accentStructSize);
            Marshal.StructureToPtr(accent, accentPtr, false);

            var data = new WindowCompositionAttributeData
            {
                Attribute = WindowCompositionAttribute.WCA_ACCENT_POLICY,
                SizeOfData = accentStructSize,
                Data = accentPtr
            };

            SetWindowCompositionAttribute(windowHelper.Handle, ref data);

            Marshal.FreeHGlobal(accentPtr);
        }



        #endregion
    }
}