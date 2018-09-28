using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;
using Kexi.Common;

namespace Kexi.Shell
{
    public class NativeFileInfo
    {
        private readonly string _fileName;

        private ShellNative.SHFILEINFO? _fileInfo;

        public NativeFileInfo(string fileName)
        {
            _fileName = fileName;
        }

        private ShellNative.SHFILEINFO FileInfo
        {
            get
            {
                if (_fileInfo == null)
                {
                    var info = new ShellNative.SHFILEINFO();
                    var uFlags = ShellNative.Win32.SHGFI_ICON | ShellNative.SHGFI.SHGFI_TYPENAME | ShellNative.SHGFI.SHGFI_DISPLAYNAME;
                    ShellNative.Win32.SHGetFileInfo(_fileName, 0, ref info, (uint) Marshal.SizeOf(info), uFlags);
                    _fileInfo = info;
                }
                return _fileInfo.Value;
            }
        }

        public string DisplayName => FileInfo.szDisplayName;
        public string TypeName => FileInfo.szTypeName;
        public int Attributes => (int) FileInfo.dwAttributes;

        public BitmapSource Icon
        {
            get
            {
                var icon = (Icon) System.Drawing.Icon.FromHandle(FileInfo.hIcon).Clone();
                ShellNative.Win32.DestroyIcon(FileInfo.hIcon);

                return ShellNative.GetBitMapSource(icon);
            }
        }
    }
}