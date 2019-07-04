﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Kexi.Common;
using Kexi.Files;
using Kexi.Interfaces;
using Kexi.ItemProvider;
using Kexi.Shell;
using Kexi.ViewModel.Commands;
using Kexi.ViewModel.Item;
using LengthConverter = Kexi.Converter.LengthConverter;

namespace Kexi.ViewModel.Lister
{
    [Export(typeof(FileLister))]
    [Export(typeof(ILister))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class FileLister : BaseLister<FileItem>, IHistorisationProvider, IBreadCrumbProvider, ICanDelete
    {
        private readonly FileItemProvider _itemProvider;

        private Tuple<string, StringCollection, FileAction> _undoParameter;
        private FilesystemChangeWatcher _watcher;

        [ImportingConstructor]
        public FileLister(Workspace workspace, Options options,
            CommandRepository commandRepository)
            : base(workspace, options, commandRepository)
        {
            Title = "Files";
            History = new BrowsingHistory();
            _watcher = new FilesystemChangeWatcher(this);
            _watcher.Register();
            _itemProvider = new FileItemProvider(workspace);
            PathChanged += FileLister_PathChanged;
        }

        public override ObservableCollection<Column> Columns { get; } = new ObservableCollection<Column>
        {
            new Column("", "Thumbnail", ColumnType.Image) {OneTimeBinding = true},
            new Column("Name", "DisplayName", ColumnType.Highlightable) {Width = 300},
            new Column("LastModified", "Details.LastModified", ColumnType.RightAligned) {Width = 180},
            new Column("Type", "Details.Type", ColumnType.Text, ColumnSize.Auto) {Width = 180},
            new Column("Size", "Details.Length", ColumnType.Number)
            {
                Width = 100,
                Converter = new LengthConverter()
            }
        };

        public override string ProtocolPrefix => "File";

        public override bool SupportsMultiview => true; 


        private void FetchDetails(FileItem item, bool thumbs, CancellationToken cancellationToken)
        {
            item.Details = item.GetDetail(thumbs, cancellationToken);
            item.Thumbnail = item.Details.Thumbnail;
        }


        public void LoadBackgroundData(IEnumerable items, CancellationToken cancellationToken)
        {
            LoadBackgroundData(items.Cast<FileItem>(), cancellationToken);
        }

        public async Task<bool> DoBreadcrumbAction(string breadPath)
        {
            var uri = new Uri(breadPath);
            if (uri.IsUnc)
            {
                try
                {
                    LoadingStatus = LoadingStatus.Loading;
                    var valid = await IsValidNetworkLocation(uri).ConfigureAwait(false);
                    if (valid) Path = breadPath;
                }
                finally
                {
                    LoadingStatus = LoadingStatus.Loaded;
                }
            }
            else if (File.Exists(breadPath) || breadPath.EndsWith("\\") && Directory.Exists(breadPath))
            {
                var path = System.IO.Path.GetDirectoryName(breadPath);
                Path = path;
            }
            else if (Directory.Exists(breadPath + "\\"))
            {
                var path = System.IO.Path.GetDirectoryName(breadPath + "\\");
                Path = path;
            }
            else
            {
                NotificationHost.AddError(breadPath + " could not be found.");
                return false;
            }

            await Refresh().ConfigureAwait(false);
            return true;
        }

        public void Delete()
        {
            var result = new FilesystemAction(Workspace.NotificationHost).Delete(ItemsView.SelectedItems);
            if (result != null) Workspace.NotificationHost.AddError(result);
        }

        public BrowsingHistory History { get; }

        private async Task<bool> IsValidNetworkLocation(Uri uri)
        {
            try
            {
                if (uri.Segments.Length == 1)
                {
                    await Dns.GetHostEntryAsync(uri.Host).ConfigureAwait(false);
                    return true;
                }

                return Directory.Exists(uri.OriginalString);
            }
            catch (Exception)
            {
                return false;
            }
        }

        private async void FileLister_PathChanged(string value)
        {
            if (value != null)
                try
                {
                    Thumbnail = await Task.Factory.StartNew(() =>
                        ThumbnailProvider.GetThumbnailSource(value, 32, 32, ThumbnailOptions.IconOnly));
                    _watcher?.ObservePath(value);
                }
                catch (Exception)
                {
                    //ignore
                }

            if (string.IsNullOrEmpty(value))
            {
                PathName = Constants.RootName;
            }
            else
            {
                var uri = new Uri(value);
                if (uri.IsUnc && uri.Segments.Length == 1)
                    PathName = value;
                else if (!uri.IsUnc && new DirectoryInfo(value).Parent == null)
                    PathName = $"Drive {value}";
                else
                    PathName = System.IO.Path.GetFileName(value);
            }

            Title = value;
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
            var action = FileAction.Copy;
            if (System.Windows.Clipboard.GetData("Preferred DropEffect") is Stream act)
            {
                var effectData = new byte[4];
                act.Read(effectData, 0, effectData.Length);
                action = effectData[0] == 2 ? FileAction.Move : FileAction.Copy;
            }

            if (!System.Windows.Clipboard.ContainsFileDropList())
                return;

            Items.CollectionChanged += FocusFirstPastedItem;
            var items = Clipboard.GetFileDropList();
            var copyTask = new TaskItem("Copying");
            await Workspace.TaskManager.RunAsync(copyTask,
                () => { new FilesystemAction(NotificationHost).Paste(Path, items, action); });
            _undoParameter = new Tuple<string, StringCollection, FileAction>(Path, items, action);
            if (action == FileAction.Move)
            {
                Clipboard.Clear();
                SelectedItems.Foreach(i => i.IsMarkedForMove = false);
            }
        }

        public override void Undo()
        {
            new FilesystemAction(NotificationHost).Undo(_undoParameter.Item1, _undoParameter.Item2,
                _undoParameter.Item3);
        }

        public override async void DoAction(FileItem selection)
        {
            if (selection == null)
                return;

            var result = new FileListerAction(Workspace, selection).DoAction();
            try
            {
                if (result != null)
                {
                    Path = result;
                    await Refresh().ConfigureAwait(false);
                }
            }
            catch (UnauthorizedAccessException)
            {
                await Application.Current.Dispatcher.Invoke(async () =>
                    {
                        Workspace.CommandRepository.GetCommandByName(nameof(GainAccessToDirectoryCommand))
                            .Execute(result);
                    }
                );
            }
            catch (Exception ex)
            {
                var message = selection.ItemType == ItemType.Container
                    ? "Could not navigate to "
                    : "Could not start ";
                NotificationHost.AddError(message + selection.Name, ex);
            }
        }


        public override string GetParentPath()
        {
            return FileItemProvider.GetParentContainer(Path);
        }

        public override void ShowContextMenu(IEnumerable<IItem> selectedItems)
        {
            var scm = new ShellContextMenu();
            if (selectedItems == null && Path != null)
            {
                var dirInfo = new[] {new DirectoryInfo(Path)};
                scm.ShowContextMenu(dirInfo, System.Windows.Forms.Cursor.Position);
            }
            else if (selectedItems != null)
            {
                var fileInfos = selectedItems.Select(getFileInfo).Where(i => i != null).ToArray();
                scm.ShowContextMenu(fileInfos, System.Windows.Forms.Cursor.Position);
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

        protected override Task<IEnumerable<FileItem>> GetItems()
        {
            _itemProvider.CancelCurrentTasks();
            if (string.IsNullOrEmpty(Path))
                PathName = Constants.RootName;
            else if (new Uri(Path).IsUnc && !Directory.Exists(Path)) return NetworkShareProvider.GetItems(Path);

            return _itemProvider.GetItems(Path);
        }

        public override string GetStatusString()
        {
            if (View == null || Items == null || ItemsView == null)
                return null;

            var selected = View.ListView.SelectedItems.Cast<FileItem>().ToList();
            var count = selected.Count();
            var size = "";
            if (count < 100)
                size = $"({new LengthConverter().Convert(selected.Sum(i => i.Details?.Length), null, null, null)})";
            return $"{Items.Count} Items, {count} selected {size}";
        }

        private void FocusFirstPastedItem(object sender, NotifyCollectionChangedEventArgs e)
        {
            Items.CollectionChanged -= FocusFirstPastedItem;
            if (e?.NewItems?.Count > 0 && e.NewItems?[0] is FileItem newItem)
            {
                ClearSelection();
                if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems.Count > 0)
                    View.FocusItem(newItem);
            }
        }

        protected override void Dispose(bool disposing)
        {
            _watcher = null;
            PathChanged -= FileLister_PathChanged;
            _itemProvider.Dispose();
            base.Dispose(disposing);
        }
    }
}