using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32.SafeHandles;

namespace Kexi.Shell
{
    ///from: http://dotnet-snippets.de/snippet/symbolischen-link-erstellen-loeschen-und-auslesen/3791

    /// <summary>
    /// Stellt Funktionen zum erstellen, löschen und auslesen von symbolischen Links bereit.
    /// </summary>
    public static class SymbolicLink
    {
        #region WinAPI

        [DllImport("kernel32.dll", EntryPoint = "CreateSymbolicLinkW", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern bool CreateSymbolicLink([In] string lpSymlinkFileName, [In] string lpTargetFileName, [In] int dwFlags);

        [DllImport("kernel32.dll", EntryPoint = "GetFinalPathNameByHandleW", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern int GetFinalPathNameByHandle([In] IntPtr hFile, [Out] StringBuilder lpszFilePath, [In] int cchFilePath, [In] int dwFlags);

        [DllImport("kernel32.dll", EntryPoint = "CreateFileW", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern SafeFileHandle CreateFile(string lpFileName, int dwDesiredAccess, int dwShareMode, IntPtr SecurityAttributes, int dwCreationDisposition, int dwFlagsAndAttributes, IntPtr hTemplateFile);

        private const int CREATION_DISPOSITION_OPEN_EXISTING = 3;
        private const int FILE_FLAG_BACKUP_SEMANTICS = 0x02000000;
        private const int SYMBOLIC_LINK_FLAG_DIRECTORY = 0x1;

        #endregion

        /// <summary>
        /// Erstellt einen symbolischen Link.
        /// </summary>
        /// <param name="target">Das Ziel auf das der Link verweißen soll.</param>
        /// <param name="path">Der Pfad des symbolischen Links.</param>
        /// <param name="replaceExisting"><c>True</c>, wenn eine bereits existierende Datei bzw. ein bereits existierender symbolischer Link überschrieben werden soll.</param>
        public static void CreateSymbolicLink(string target, string path, bool replaceExisting)
        {
            if (replaceExisting) DeleteSymbolicLink(path);
            bool result;
            if (Directory.Exists(target))
            {
                result = CreateSymbolicLink(path, target, SYMBOLIC_LINK_FLAG_DIRECTORY);
                if (!result) throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            else if (File.Exists(target))
            {
                result = CreateSymbolicLink(path, target, 0);
                if (!result) throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            else
                throw new IOException("Path not found");
        }

        /// <summary>
        /// Löscht einen symbolischen Link.
        /// </summary>
        /// <param name="path">Der Pfad zum symbolischen Link.</param>
        public static void DeleteSymbolicLink(string path)
        {
            if (Directory.Exists(path))
                Directory.Delete(path);
            if (File.Exists(path))
                File.Delete(path);
        }

        /// <summary>
        /// Gibt den Pfad eines Ordners oder einer Datei zurück wobei symbolische Links und bereit gestellte NTFS Ordner aufgelöst wurden.
        /// </summary>
        /// <param name="path">Der Pfad zu einem Ordner oder einer Datei.</param>
        /// <returns>Der wahre Pfad von <paramref name="path"/>.</returns>
        /// <remarks>Sollte kein Laufwerkpfad für den NTFS-Ordner verfügbar sein, so wird der Bereitstellungspfad zurück gegeben.</remarks>
        public static string GetRealPath(string path)
        {
            if (!Directory.Exists(path) && !File.Exists(path))
                throw new IOException("Path not found");

            DirectoryInfo symlink = new DirectoryInfo(path);//Es ist egel ob es eine Datei oder ein Ordner ist
            SafeFileHandle directoryHandle = CreateFile(symlink.FullName, 0, 2, System.IntPtr.Zero, CREATION_DISPOSITION_OPEN_EXISTING, FILE_FLAG_BACKUP_SEMANTICS, System.IntPtr.Zero);//Handle zur Datei/Ordner
            if (directoryHandle.IsInvalid)
                throw new Win32Exception(Marshal.GetLastWin32Error());

            StringBuilder result = new StringBuilder(512);
            int mResult = GetFinalPathNameByHandle(directoryHandle.DangerousGetHandle(), result, result.Capacity, 0);
            if (mResult < 0)
                throw new Win32Exception(Marshal.GetLastWin32Error());
            if (result.Length >= 4 && result[0] == '\\' && result[1] == '\\' && result[2] == '?' && result[3] == '\\')
                return result.ToString().Substring(4);// "\\?\" entfernen
            else
                return result.ToString();
        }
    }
}
