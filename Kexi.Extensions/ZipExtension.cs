using System.ComponentModel.Composition;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Windows.Input;
using Kexi.Composition;
using Kexi.Interfaces;
using Kexi.ViewModel;
using Kexi.ViewModel.Item;
using Kexi.ViewModel.Lister;

namespace Kexi.Extensions
{
    public class ZipExtension
    {
        [ImportingConstructor]
        public ZipExtension(Workspace workspace)
        {
            _workspace = workspace;
        }

        [ExportContextMenuCommand(typeof(FileLister), "Zip Selection")]
        public ICommand ZipSelection => new RelayCommand(c =>
        {
            var files = _workspace.GetSelection<FileItem>().ToArray();
            if (!files.Any())
                return;

            var rootPath = _workspace.ActiveLister.Path;
            var zipName  = files.First().FileInfo.Name;
            var zipPath  = Path.Combine(rootPath, $"{zipName}.zip");
            using (var zip = new FileStream(zipPath, FileMode.CreateNew))
            {
                using (var archive = new ZipArchive(zip, ZipArchiveMode.Create))
                {
                    foreach (var fi in files)
                        if (fi.ItemType == ItemType.Container)
                            CompressFolder(fi.Path, archive, rootPath);
                        else
                            archive.CreateEntryFromFile(fi.Path, fi.FileInfo.Name);
                }
            }
        });

        private readonly Workspace _workspace;

        private static void CompressFolder(string path, ZipArchive archive, string rootPath)
        {
            foreach (var file in Directory.GetFiles(path))
            {
                var filename = file.Substring(rootPath.Length+1); //+1 = Separator (\)
                archive.CreateEntryFromFile(file, filename);
            }

            foreach (var directory in Directory.GetDirectories(path))
                CompressFolder(directory, archive, rootPath);
        }
    }
}