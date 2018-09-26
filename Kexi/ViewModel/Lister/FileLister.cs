using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Kexi.Common;
using Kexi.Files;
using Kexi.Interfaces;
using Kexi.ItemProvider;
using Kexi.Shell;
using Kexi.ViewModel.Item;
using Microsoft.WindowsAPICodePack.Shell;
using Clipboard = System.Windows.Clipboard;
using LengthConverter = Kexi.Converter.LengthConverter;

namespace Kexi.ViewModel.Lister
{
    [Export(typeof(FileLister))]
    [Export(typeof(ILister))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class FileLister : BaseLister<FileItem>, IHistorisationProvider, IBreadCrumbProvider
    {
        [ImportingConstructor]
        public FileLister(Workspace workspace, INotificationHost notificationHost, Options options,
            CommandRepository commandRepository)
            : base(workspace, notificationHost, options, commandRepository)
        {
            Title    = "Files";
            History  = new BrowsingHistory();
            _watcher = new FilesystemChangeWatcher(this);
            _watcher.Register();
            _itemProvider =  new FileItemProvider(workspace);
            PathChanged   += FileLister_PathChanging;
        }

        public override IEnumerable<Column> Columns { get; } = new ObservableCollection<Column>
        {
            new Column("", "Thumbnail", ColumnType.Image) {OneTimeBinding                      = true},
            new Column("Name", "DisplayName", ColumnType.Highlightable) {Width                 = 300},
            new Column("LastModified", "Details.LastModified", ColumnType.RightAligned) {Width = 180},
            new Column("Type", "Details.Type", ColumnType.Text, ColumnSize.Auto) {Width        = 180},
            new Column("Size", "Details.Length", ColumnType.Number)
            {
                Width     = 100,
                Converter = new LengthConverter()
            }
        };

        public override string ProtocolPrefix => "File";

        public override bool SupportsMultiview => true;

        public async Task<bool> DoBreadcrumbAction(string breadPath)
        {
            Path = breadPath;
            await Refresh();
            //TODO: Move to Lister.Path, Refactor that shit
            //if (File.Exists(breadPath) || breadPath.EndsWith("\\") && Directory.Exists(breadPath))
            //{
            //    var path = System.IO.Path.GetDirectoryName(breadPath);
            //    Path = path;
            //    await Refresh();
            //}
            //else if (Directory.Exists(breadPath + "\\"))
            //{
            //    var path = System.IO.Path.GetDirectoryName(breadPath + "\\");
            //    Path = path;
            //    await Refresh();
            //}
            //else if (breadPath.StartsWith(@"\\") && ShellObject.FromParsingName(breadPath) != null)
            //{
            //    Path = breadPath;
            //    await Refresh();
            //}
            //else
            //{
            //    NotificationHost.AddError(breadPath + " could not be found.");
            //    return false;
            //}

            return true;
        }

        public           BrowsingHistory         History { get; }
        private readonly FileItemProvider        _itemProvider;
        private          FilesystemChangeWatcher _watcher;

        private void FileLister_PathChanging(string value)
        {
            Thumbnail = ShellNative.GetSmallBitmapSource(Path);
            _watcher?.ObservePath(value);
            PathName = string.IsNullOrEmpty(_path) ? "My Computer" : System.IO.Path.GetFileName(_path);
        }

        public override void Copy()
        {
            SetSelection(FileAction.Copy);
        }

        public override void Cut()
        {
            Items.Where(i => i.IsMarkedForMove)
                .Foreach(i => i.IsMarkedForMove = false);
            SelectedItems
                .Foreach(i => i.IsMarkedForMove = true);

            SetSelection(FileAction.Move);
        }

        private void SetSelection(FileAction fa)
        {
            var selection = ItemsView.SelectedItems.Select(s => s.Path);
            FilesystemAction.SetClipboard<FileItem>(fa, selection);
        }

        public override async void Paste()
        {
            Items.CollectionChanged += FocusFirstPastedItem;

            var action = FileAction.Copy;
            if (Clipboard.GetData("Preferred DropEffect") is Stream act)
            {
                var effectData = new byte[4];
                act.Read(effectData, 0, effectData.Length);
                action = effectData[0] == 2 ? FileAction.Move : FileAction.Copy;
            }

            if (!Clipboard.ContainsFileDropList())
                return;

            var items = Clipboard.GetFileDropList();
            var copyTask = new TaskItem("Copying");
            await Workspace.TaskManager.RunAsync(copyTask, () => { new FilesystemAction(NotificationHost).Paste(Path, items, action); });
            if (action == FileAction.Move)
            {
                Clipboard.Clear();
                SelectedItems.Foreach(i => i.IsMarkedForMove = false);
            }
        }

        public override async void DoAction(FileItem selection)
        {
            if (selection == null)
                return;

            try
            {
                var result = new FileListerAction(Workspace, selection).DoAction();
                if (result != null)
                {
                    Path = result;
                    await Refresh();
                }
            }
            catch (Exception ex)
            {
                var message = selection.ItemType == ItemType.Container
                    ? "Could not navigate to "
                    : "Could not start ";
                NotificationHost.AddError(message + selection.Name, ex);
            }
        }

        public override string GetParentContainer()
        {
            return FileItemProvider.GetParentContainer(Path);
        }

        public override void ShowContextMenu(bool emptyPath)
        {
            var scm = new ShellContextMenu();
            if (emptyPath && Path != null)
            {
                var dirInfo = new[] {new DirectoryInfo(Path)};
                scm.ShowContextMenu(dirInfo, Cursor.Position);
            }
            else
            {
                var fileInfos = SelectedItems.Select(getFileInfo).Where(i => i != null).ToArray();
                scm.ShowContextMenu(fileInfos, Cursor.Position);
            }
        }

        private FileInfo getFileInfo(IItem item)
        {
            try
            {
                return new FileInfo(item.Path);
            }
            catch (ArgumentException)
            {
                return null;
            }
        }

        protected override async Task<IEnumerable<FileItem>> GetItems()
        {
            _itemProvider.CancelCurrentTasks();
            if (new Uri(Path).IsUnc && !Directory.Exists(Path))
            {
                return await NetworkShareProvider.GetItems(Path);
            }
            return await _itemProvider.GetItems(Path);
        }

        public override string GetStatusString()
        {
            if (View == null || Items == null || ItemsView == null)
                return null;

            var selected = View.ListView.SelectedItems.Cast<FileItem>().ToList();
            var count    = selected.Count();
            var size     = "";
            if (count < 100)
                size = $"({new LengthConverter().Convert(selected.Sum(i => i.Details?.Length), null, null, null)})";
            return $"{Items.Count} Items, {count} selected {size}";
        }

        private void FocusFirstPastedItem(object sender, NotifyCollectionChangedEventArgs e)
        {
            var newItem    = e.NewItems[0] as FileItem;
            var firstPaste = System.IO.Path.GetFileName(Clipboard.GetFileDropList().Cast<string>().FirstOrDefault());
            if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems.Count > 0 && newItem?.Name == firstPaste)
                View.FocusItem(newItem);
            Items.CollectionChanged -= FocusFirstPastedItem;
        }

        protected override void Dispose(bool disposing)
        {
            _watcher    =  null;
            PathChanged -= FileLister_PathChanging;
            _itemProvider.Dispose();
            base.Dispose(disposing);
        }
    }
}