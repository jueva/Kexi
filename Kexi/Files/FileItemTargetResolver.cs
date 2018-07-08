using System.IO;
using Kexi.Interfaces;
using Kexi.Shell;
using Kexi.ViewModel.Item;
using Shell32;

namespace Kexi.Files
{
    public class FileItemTargetResolver
    {
        public FileItemTargetResolver(FileItem fileItem, bool doParse = true)
        {
            _fileItem = fileItem;
            if (doParse)
                Parse();
        }

        public FileItemTargetResolver(string path, bool doParse = true) : this(new FileItem(path, Directory.Exists(path) ? ItemType.Container : ItemType.Item), doParse)
        {
        }

        public           string   TargetPath       { get; private set; }
        public           ItemType TargetType       { get; private set; }
        public           string   WorkingDirectory { get; set; }
        private readonly FileItem _fileItem;

        public void Parse()
        {
            WorkingDirectory = _fileItem.Directory;
            if ((_fileItem.Attributes & FileAttributes.ReparsePoint) == FileAttributes.ReparsePoint)
            {
                TargetPath = SymbolicLink.GetRealPath(_fileItem.Path);
                TargetType = Directory.Exists(TargetPath) ? ItemType.Container : ItemType.Item;
            }
            else if (_fileItem.IsLink())
            {
                var link = GetLink(_fileItem);
                if (link != null)
                {
                    TargetPath       = link.Path;
                    WorkingDirectory = link.WorkingDirectory;
                }

                TargetType = Directory.Exists(TargetPath) ? ItemType.Container : ItemType.Item;
            }
            else
            {
                TargetPath = _fileItem.Path;
                TargetType = _fileItem.ItemType;
            }
        }

        private static ShellLinkObject GetLink(FileItem fi)
        {
            var pathOnly     = fi.Directory;
            var filenameOnly = fi.Name;

            var wrapper    = new ShellWrapper();
            var folder     = wrapper.GetFolder(pathOnly);
            var folderItem = folder.ParseName(filenameOnly);
            if (folderItem != null)
                try
                {
                    return (ShellLinkObject) folderItem.GetLink;
                }
                catch
                {
                    return null;
                }

            return null;
        }
    }
}