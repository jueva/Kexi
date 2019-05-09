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
        public ICommand ZipSelection => new RelayCommand(c => ZipAsync());

        private readonly Workspace _workspace;

        private async void ZipAsync()
        {
            var files = _workspace.GetSelection<FileItem>().ToArray();

            var zipTask = new TaskItem("Zipping");
            await _workspace.TaskManager.RunAsync(zipTask, () => ZipInternal(files, zipTask));
        }

        private void ZipInternal(FileItem[] files, TaskItem zipTask)
        {
            //TODO: set progress on zipTask ?!
            if (files.Length == 0)
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
                            CompressFolder(fi.Path, archive, rootPath.Length + 1);
                        else
                            archive.CreateEntryFromFile(fi.Path, fi.FileInfo.Name);
                }
            }
        }

        private static void CompressFolder(string path, ZipArchive archive, int rootPathLength)
        {
            foreach (var file in Directory.GetFiles(path))
            {
                var folderAndFilename = file.Substring(rootPathLength);
                archive.CreateEntryFromFile(file, folderAndFilename);
            }

            foreach (var directory in Directory.GetDirectories(path))
                CompressFolder(directory, archive, rootPathLength);
        }
    }
}