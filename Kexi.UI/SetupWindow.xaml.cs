using System;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using Kexi.ViewModel;

namespace Kexi.UI
{

    /// <summary>
    /// Interaction logic for SetupWindow.xaml
    /// </summary>
    public partial class SetupWindow
    {

        public SetupWindow(Workspace workspace)
        {
            InitializeComponent();
            AllowsTransparency = true;
            Loaded += SetupWindow_Loaded;
            DataContext = new SetupViewModel(workspace);
        }

        private void SetupWindow_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            DarkThemeButton.Focus();
            EnableBlur();
        }

        #region Acrylic

        [DllImport("user32.dll")]
        private static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);

        private void EnableBlur()
        {
            var windowHelper = new WindowInteropHelper(this);

            var accent = new AccentPolicy {AccentState = AccentState.ACCENT_ENABLE_BLURBEHIND};

            var accentStructSize = Marshal.SizeOf(accent);

            var accentPtr = Marshal.AllocHGlobal(accentStructSize);
            Marshal.StructureToPtr(accent, accentPtr, false);

            var data = new WindowCompositionAttributeData
            {
                Attribute  = WindowCompositionAttribute.WCA_ACCENT_POLICY,
                SizeOfData = accentStructSize,
                Data       = accentPtr
            };

            SetWindowCompositionAttribute(windowHelper.Handle, ref data);

            Marshal.FreeHGlobal(accentPtr);
        }



#endregion
    }
}