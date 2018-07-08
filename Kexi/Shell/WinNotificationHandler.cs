using System;
using System.Windows;
using System.Windows.Interop;
using Kexi.Interfaces;

namespace Kexi.Shell
{
    public class WinNotificationHandler
    {
        private readonly Window _mainWindow;
        private readonly INotificationHost _notificationHost;

        public WinNotificationHandler(Window mainWindow, INotificationHost notificationHost)
        {
            _mainWindow = mainWindow;
            _notificationHost = notificationHost;
        }

        public void Register()
        {
            HwndSource source = HwndSource.FromHwnd(new WindowInteropHelper(_mainWindow).Handle);
            if (source != null)
            {
                var windowHandle = source.Handle;
                source.AddHook(HwndHandler);
                UsbNotification.RegisterUsbDeviceNotification(windowHandle);
            }
        }

        private IntPtr HwndHandler(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam, ref bool handled)
        {
            
            if (msg == UsbNotification.WmDevicechange)
            {
                switch ((int)wparam)
                {
                    case UsbNotification.DbtDeviceremovecomplete:
                        _notificationHost.AddInfo("Usb disconnected");
                        break;
                    case UsbNotification.DbtDevicearrival:
                        _notificationHost.AddInfo("Usb connected");
                        break;
                }
            }
            handled = false;
            return IntPtr.Zero;
        }
    }
}
