using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Kexi.ViewModel.Item;
using Shell32;

namespace Kexi.Shell
{
    public class ShellWrapper
    {
        private static readonly Lazy<ShellWrapper> instance = new Lazy<ShellWrapper>(() => new ShellWrapper());

        public ShellWrapper()
        {
            shellAppType = Type.GetTypeFromProgID("Shell.Application");
            shell        = Activator.CreateInstance(shellAppType);
        }

        public static    ShellWrapper Instance => instance.Value;
        private readonly dynamic      shell;
        private readonly Type         shellAppType;

        public Folder GetFolder(string path)
        {
            return (Folder) shellAppType.InvokeMember("NameSpace", BindingFlags.InvokeMethod, null, shell, new object[] {path});
        }

        public FolderItem GetItem(string path)
        {
            var folder = (Folder) shellAppType.InvokeMember("NameSpace", BindingFlags.InvokeMethod, null, shell, new object[] {Path.GetDirectoryName(path)});
            return folder?.ParseName(Path.GetFileName(path));
        }

        public bool IsLink(string path)
        {
            return GetItem(path)?.IsLink ?? false;
        }

        public IEnumerable<PropertyItem> GetFileDetails(string path)
        {
            string pathOnly     = Path.GetDirectoryName(path);
            string filenameOnly = Path.GetFileName(path);

            Folder     folder     = shell.NameSpace(pathOnly);
            var folderItem = folder.ParseName(filenameOnly) as FolderItem2;
            var headers = new Dictionary<int, string>();
            for( int i = 0; i < 1000; i++ )
            {
                string header = folder.GetDetailsOf(folderItem, i);
                if (!string.IsNullOrEmpty(header)) {
                    headers.Add(i, header);
                }
            }
            foreach(var h in headers)
            {
                var value = folder.GetDetailsOf(folderItem, h.Key);
                if (!string.IsNullOrEmpty(value))
                    yield return new PropertyItem(h.Value, folder.GetDetailsOf(folderItem, h.Key));
            }
        }

        private string EnsurePath(string path)
        {
            path = path.Trim();
            var sep1 = Path.DirectorySeparatorChar.ToString();
            var sep2 = Path.AltDirectorySeparatorChar.ToString();


            if (path.EndsWith(sep1) || path.EndsWith(sep2))
                return path;

            var ret = path.Contains(sep1) ? path + sep1 : path + sep2;
            return ret;
        }
    }
}