using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Kexi.Common;
using Kexi.Interfaces;
using Kexi.ViewModel;
using Kexi.ViewModel.Item;
using Microsoft.WindowsAPICodePack.Shell;
using Shell32;

namespace Kexi.ItemProvider
{
    [Export(typeof(FileItemProvider))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class FileItemProvider : IDisposable
    {
        [ImportingConstructor]
        public FileItemProvider(Workspace workspace)
        {
            CancellationTokenSource = new CancellationTokenSource();
            Workspace = workspace;
        }

        public Workspace Workspace { get; }

        public CancellationTokenSource CancellationTokenSource { get; private set; }
        public CancellationTokenSource OldCancellationTokenSource { get; private set; }

        public void CancelCurrentTasks()
        {
            OldCancellationTokenSource = CancellationTokenSource;
            CancellationTokenSource.Cancel();
            CancellationTokenSource = new CancellationTokenSource();
        }

        public async Task<IEnumerable<FileItem>> GetItems(string path)
        {
            return await Task.Run(() =>
            {
                if (string.IsNullOrEmpty(path))
                    return GetRootItems().Items;

                IEnumerable<FileItem> items;
                try
                {
                    items = GetItemsInternal(path);
                }
                catch (UnauthorizedAccessException)
                {
                    if (path.StartsWith(@"\\"))
                    {
                        PinvokeWindowsNetworking.ConnectToRemote(path, "", "", true);
                        items = GetItemsInternal(path);
                    }
                    else
                    {
                        throw;
                    }
                }


                if (items.Any())
                    return Workspace.Options.ShowHiddenItems ? items : items.Where(i => !i.IsSystemOrHidden);

                var parent = Directory.GetParent(path);
                var item = new FileItem(parent?.FullName, ItemType.Container, "..");
                items = new[] {item};
                return items;
            });
        }

        private IEnumerable<FileItem> GetItemsInternal(string path, bool showHidden = true)
        {
            if (path.StartsWith(@"\\") && !Regex.IsMatch(path, @"^\\\\.+\\.+$"))
            {
                var folder = ShellObject.FromParsingName(path) as ShellFolder;
                return folder?.Select(i => new FileItem(i.Properties.System.ParsingPath.Value, ItemType.Container, itemProvider: this)).ToList();
            }

            return Directory.EnumerateDirectories(path).Select(p => new FileItem(p, ItemType.Container, itemProvider: this))
                .Concat(
                    Directory.EnumerateFiles(path).Select(p => new FileItem(p, ItemType.Item, itemProvider: this))
                );
        }

        public static string GetParentContainer(string path)
        {
            if (path == null)
                return null;
            if (path.StartsWith("::{"))
            {
                var index = path.LastIndexOf(@"\", StringComparison.Ordinal);
                return index < 0 ? null : path.Substring(0, index);
            }
            var parent = Directory.GetParent(path);
            return parent?.FullName;
        }

        private static ItemProviderResult<FileItem> GetRootItems()
        {
            IEnumerable<FileItem> rootItems = null;
            string path = null;
            Application.Current.Dispatcher.Invoke(() =>
            {
                var compi = new Shell32.Shell().NameSpace(ShellSpecialFolderConstants.ssfDRIVES);
                rootItems = compi.Items().Cast<FolderItem2>()
                    .Where(f => Directory.Exists(f.Path))
                    .Select(i => new FileItem(i.Path, i.IsFolder ? ItemType.Container : ItemType.Item, i.Name));
                path = compi.Title;

                return new ItemProviderResult<FileItem>
                {
                    Items = rootItems,
                    PathName = path
                };
            });

            return new ItemProviderResult<FileItem>
            {
                Items = rootItems,
                PathName = path
            };
        }


        public static async void Rename(FileItem fileItem, string newName)
        {
            if (fileItem != null)
                if (fileItem.ItemType == ItemType.Container)
                {
                    var di     = new DirectoryInfo(fileItem.Path);
                    var parent = di.Parent ?? di.Root;
                    var dest   = Path.Combine(parent.FullName, newName);
                    if (fileItem.Path.Equals(dest, StringComparison.OrdinalIgnoreCase))
                        return;
                    di.MoveTo(dest);
                    fileItem.Path = dest;
                    fileItem.DisplayName = newName;
                }
                else
                {
                    var di     = new FileInfo(fileItem.Path);
                    var parent = di.Directory;
                    if (parent != null)
                    {
                        var dest                               = Path.Combine(parent.FullName, $"{newName}");
                        if (fileItem.Path.Equals(dest, StringComparison.OrdinalIgnoreCase))
                            return;
                        di.MoveTo(dest);
                        fileItem.Path = dest;
                        fileItem.DisplayName = newName;
                    }
                }
        }

        public static (int, int) GetRenameSelectionBorder(FileItem _targetItem)
        {
            var isDirectory                      = false;
            if (_targetItem != null) isDirectory = _targetItem.ItemType == ItemType.Container;

            var endIndex = _targetItem.Name.LastIndexOf('.');
            if (endIndex == -1 || isDirectory)
                endIndex = _targetItem.Name.Length;

            return (0, endIndex);
        }

        public void Dispose()
        {
            CancellationTokenSource?.Dispose();
            OldCancellationTokenSource?.Dispose();
        }
    }
}