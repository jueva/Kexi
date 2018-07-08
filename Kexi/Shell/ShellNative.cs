using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace Kexi.Shell
{
    /// <summary>
    /// Summary description for ShellIcon.  Get a small or large Icon with an easy C# function call
    /// that returns a 32x32 or 16x16 System.Drawing.Icon depending on which function you call
    /// either GetSmallIcon(string fileName) or GetLargeIcon(string fileName)
    /// </summary>
    public class ShellNative
    {
        private readonly string _path;

        public ShellNative(string path)
        {
            _path = path;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct SHFILEINFO
        {
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        }

        public static class FILE_ATTRIBUTE
        {
            public const uint FILE_ATTRIBUTE_NORMAL = 0x80;
        }

        public static class SHGFI
        {
            public const uint SHGFI_TYPENAME = 0x000000400;
            public const uint SHGFI_USEFILEATTRIBUTES = 0x000000010;
            public const uint SHGFI_DISPLAYNAME = 0x000000200;     
        }

        public static class Win32
        {
            public const uint SHGFI_ICON = 0x100;
            public const uint SHGFI_LARGEICON = 0x0; // 'Large icon
            public const uint SHGFI_SMALLICON = 0x1; // 'Small icon

            [DllImport("shell32.dll", CharSet = CharSet.Ansi)]
            public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);

            [DllImport("User32.dll")]
            public static extern int DestroyIcon(IntPtr hIcon);

        }

        public static Tuple<BitmapSource, string, string> GetFullInfo(string fileName)
        {
            var info = new SHFILEINFO(); 
            uint uFlags = (uint)(Win32.SHGFI_ICON | Win32.SHGFI_LARGEICON | SHGFI.SHGFI_TYPENAME | SHGFI.SHGFI_TYPENAME | SHGFI.SHGFI_DISPLAYNAME);

            Win32.SHGetFileInfo(fileName, 0, ref info, (uint)Marshal.SizeOf(info), uFlags);

            var icon = (Icon)Icon.FromHandle(info.hIcon).Clone();
            Win32.DestroyIcon(info.hIcon);
            return new Tuple<BitmapSource, string, string>(GetBitMapSource(icon), info.szTypeName, info.szDisplayName);
        }


        private static Icon GetIcon(string fileName, uint flags)
        {
            try
            {
                SHFILEINFO shinfo = new SHFILEINFO();
                IntPtr hImgSmall = Win32.SHGetFileInfo(fileName, 0, ref shinfo, (uint)Marshal.SizeOf(shinfo),
                    Win32.SHGFI_ICON | flags);

                Icon icon = (Icon)System.Drawing.Icon.FromHandle(shinfo.hIcon).Clone();
                Win32.DestroyIcon(shinfo.hIcon);
                return icon;
            }
            catch
            {
                return null;
            }
        }

        public static Icon GetSmallIcon(string fileName)
        {
            return GetIcon(fileName, Win32.SHGFI_SMALLICON);
        }

        public static BitmapSource GetLargeBitmapSource(string fileName)
        {
            return GetBitMapSource(GetLargeIcon(fileName));
        }

        public static async Task<BitmapSource> GetBitmapSourceAsync(string fileName)
        {
            return await Task.Run(() => GetSmallBitmapSource(fileName));
        }

        public static BitmapSource GetSmallBitmapSource(string fileName)
        {
            try
            {
                return GetBitMapSource(GetSmallIcon(fileName));
            }
            catch
            {
                return null;
            }
            
        }

        public static BitmapSource GetBitMapSource(Icon icon)
        {
            if (icon == null)
                return null;

            Bitmap bitmap = icon.ToBitmap();
            IntPtr hBitmap = bitmap.GetHbitmap();

            BitmapSource wpfBitmap =
                Imaging.CreateBitmapSourceFromHBitmap(
                    hBitmap, IntPtr.Zero, Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
            wpfBitmap.Freeze();
            return wpfBitmap;
        }

        public static Icon GetLargeIcon(string fileName)
        {
            return GetIcon(fileName, Win32.SHGFI_LARGEICON);
        }


        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        static extern bool ShellExecuteEx(ref SHELLEXECUTEINFO lpExecInfo);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct SHELLEXECUTEINFO
        {
            public int    cbSize;
            public uint   fMask;
            public IntPtr hwnd;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpVerb;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpFile;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpParameters;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpDirectory;
            public int    nShow;
            public IntPtr hInstApp;
            public IntPtr lpIDList;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpClass;
            public IntPtr hkeyClass;
            public uint   dwHotKey;
            public IntPtr hIcon;
            public IntPtr hProcess;
        }

        private const int  SW_SHOW               = 5;
        private const uint SEE_MASK_INVOKEIDLIST = 12;
        public static bool ShowFileProperties(string Filename)
        {
            SHELLEXECUTEINFO info = new SHELLEXECUTEINFO();
            info.cbSize = System.Runtime.InteropServices.Marshal.SizeOf(info);
            info.lpVerb = "properties";
            info.lpFile = Filename;
            info.nShow  = SW_SHOW;
            info.fMask  = SEE_MASK_INVOKEIDLIST;
            return ShellExecuteEx(ref info);        
        }

        public const int FO_COPY                   = 0x0002;
        public const int FO_DELETE                 = 0x0003;
        public const int FO_MOVE                   = 0x0001;
        public const int FO_RENAME                 = 0x0004;
        public const int FOF_ALLOWUNDO             = 0x0040;
        public const int FOF_FILESONLY             = 0x0080;
        public const int FOF_MULTIDESTFILES        = 0x0001;
        public const int FOF_NOCONFIRMATION        = 0x0010;
        public const int FOF_NOCONFIRMMKDIR        = 0x0200;
        public const int FOF_NO_CONNECTED_ELEMENTS = 0x1000;
        public const int FOF_NOCOPYSECURITYATTRIBS = 0x0800;
        public const int FOF_NOERRORUI             = 0x0400;
        public const int FOF_RENAMEONCOLLISION     = 0x0008;
        public const int FOF_SILENT                = 0x0004;
        public const int FOF_SIMPLEPROGRESS        = 0x0100;
        public const int FOF_WANTMAPPINGHANDLE     = 0x0020;
        public const int FOF_WANTNUKEWARNING       = 0x2000;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct SHFILEOPSTRUCT
        {
            public IntPtr hwnd;
            [MarshalAs(UnmanagedType.U4)]
            public int wFunc;
            public string pFrom;
            public string pTo;
            public short  fFlags;
            [MarshalAs(UnmanagedType.Bool)]
            public bool fAnyOperationsAborted;
            public IntPtr hNameMappings;
            public string lpszProgressTitle;
        }
 
        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        public static extern int SHFileOperation(ref SHFILEOPSTRUCT FileOp);


        public static bool IsLink(string shortcutFilename)
        {
            string pathOnly     = System.IO.Path.GetDirectoryName(shortcutFilename);
            string filenameOnly = System.IO.Path.GetFileName(shortcutFilename);

            Shell32.Shell      shell      = new Shell32.Shell();
            Shell32.Folder     folder     = shell.NameSpace(pathOnly);
            Shell32.FolderItem folderItem = folder.ParseName(filenameOnly);
            if (folderItem != null)
            {
                return folderItem.IsLink;
            }
            return false; // not found
        }

        public static string GetShortcutTarget(string shortcutFilename)
        {
            string pathOnly     = System.IO.Path.GetDirectoryName(shortcutFilename);
            string filenameOnly = System.IO.Path.GetFileName(shortcutFilename);

            Shell32.Shell      shell      = new Shell32.Shell();
            Shell32.Folder     folder     = shell.NameSpace(pathOnly);
            Shell32.FolderItem folderItem = folder.ParseName(filenameOnly);
            if (folderItem != null)
            {
                if (folderItem.IsLink)
                {
                    Shell32.ShellLinkObject link = (Shell32.ShellLinkObject)folderItem.GetLink;
                    return link.Path;
                }
                return shortcutFilename;
            }
            return ""; // not found
        }

    }
}
