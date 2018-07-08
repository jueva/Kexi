using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using Kexi.ViewModel;
using Kexi.ViewModel.Lister;
using NHotkey.Wpf;

namespace Kexi.Common
{
    public class GlobalHotkeyHandler
    {
        public GlobalHotkeyHandler(Window mainWindow, Workspace workspace)
        {
            _mainWindow = mainWindow;
            _workspace  = workspace;
            _options    = workspace.Options;
        }

        private readonly Window    _mainWindow;
        private readonly Options   _options;
        private readonly Workspace _workspace;

        public void Register()
        {
            var kexFocus      = (Key) Enum.Parse(typeof(Key), _options.GlobalHotKey);
            var showClipboard = (Key) Enum.Parse(typeof(Key), _options.ViewClipboardHotKey);
            try
            {
                HotkeyManager.Current.AddOrReplace("FocusWindows", kexFocus, ModifierKeys.Windows, (n, h) =>
                {
                    _mainWindow.Activate();
                    _mainWindow.WindowState = WindowState.Normal;
                    _workspace.FocusListView();
                });
                HotkeyManager.Current.AddOrReplace("ViewClipboard", showClipboard, ModifierKeys.Windows, (n, h) =>
                {
                    var textLister = KexContainer.Resolve<TextLister>();
                    _workspace.Open(textLister);
                    textLister.Path = textLister.PathName = textLister.Title = "Clipboard";
                    textLister.Text = Clipboard.GetText();
                    textLister.Refresh();
                    _mainWindow.Activate();
                    _mainWindow.WindowState = WindowState.Normal;
                });
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }
        }
    }
}