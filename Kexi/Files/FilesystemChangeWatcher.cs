using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using Kexi.Interfaces;
using Kexi.ViewModel.Item;
using Kexi.ViewModel.Lister;

namespace Kexi.Files
{
    public class FilesystemChangeWatcher
    {
        public FilesystemChangeWatcher(FileLister fileLister)
        {
            _fileLister = fileLister;
        }

        private          ObservableCollection<FileItem> Items => _fileLister.Items;
        private readonly FileLister                     _fileLister;

        private FileSystemWatcher _watcher;

        public void Register()
        {
            _watcher = new FileSystemWatcher
            {
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName,
                Filter       = "*.*"
            };
            _watcher.Changed += FileWatcherChanged;
            _watcher.Created += FileWatcherChanged;
            _watcher.Deleted += FileWatcherChanged;
            _watcher.Renamed += FileWatcherRenamed;
        }

        public void ObservePath(string path)
        {
            if (path != null && Directory.Exists(path))
            {
                try
                {
                    _watcher.Path                = path;
                    _watcher.EnableRaisingEvents = true;
                }
                catch (Exception)
                {
                    //TODO: Invalid Path when navigating f.e. Zip Files, find if instead of catch
                } 
            }
        }

        private void FileWatcherRenamed(object sender, RenamedEventArgs e)
        {
            var item = Items.FirstOrDefault(f => f.Path == e.OldFullPath);
            if (item != null)
            {
                item.Path        = e.FullPath;
                item.DisplayName = item.Name;
            }
        }

        private void FileWatcherChanged(object sender, FileSystemEventArgs e)
        {
            var dispatcher = Application.Current.Dispatcher;
            switch (e.ChangeType)
            {
                case WatcherChangeTypes.Changed:
                    dispatcher.Invoke(() =>
                    {
                        var item = Items?.FirstOrDefault(f => f.Path == e.FullPath);
                        item?.Refresh();
                    });
                    break;

                case WatcherChangeTypes.Created:
                    var type = Directory.Exists(e.FullPath) ? ItemType.Container : ItemType.Item;
                    dispatcher.Invoke(() =>
                    {
                        if (type == ItemType.Item && !File.Exists(e.FullPath)
                            || type == ItemType.Container && !Directory.Exists(e.FullPath))
                        {
                            return;
                        }

                        var itemc = new FileItem(e.FullPath, type);
                        Items.Add(itemc);

                        var dummy = Items.FirstOrDefault(i => i.DisplayName == "..");
                        if (dummy != null)
                        {
                            Items.Remove(dummy);
                        }
                    }, DispatcherPriority.SystemIdle);
                    break;
                case WatcherChangeTypes.Deleted:
                    dispatcher.Invoke(() =>
                    {
                        var itemToDelete = Items.FirstOrDefault(f => f.Path == e.FullPath);
                        if (itemToDelete != null)
                        {
                            Items.Remove(itemToDelete);
                        }

                        if (!Items.Any())
                        {
                            var item = new FileItem(Directory.GetParent(_fileLister.Path).FullName, ItemType.Container, "..");
                            Items.Add(item);
                            _fileLister.Workspace.FocusCurrentOrFirst();
                        }
                    });
                    break;
            }
        }
    }
}