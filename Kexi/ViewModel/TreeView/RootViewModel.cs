using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using Kexi.Common;
using Kexi.Files;
using Kexi.Interfaces;
using Kexi.ViewModel.Item;
using Shell32;

namespace Kexi.ViewModel.TreeView
{
    public class RootViewModel
    {
        private readonly TreeViewItem _favorites;
        private readonly object _locker = new object();

        public RootViewModel(Workspace workspace)
        {
            Workspace = workspace;

            Root = new ObservableCollection<TreeViewItem>();
            var computerItem = GetMyComputerItem().Invoke();
            computerItem.IsExpanded = false;
            Root.Add(computerItem);

            var favLocation = Environment.GetFolderPath(Environment.SpecialFolder.Favorites);
            _favorites = new TreeViewItem(favLocation, ItemType.Container, "Favorites");
            BindingOperations.CollectionRegistering += BindingOperations_CollectionRegistering;
            ExpandItem(_favorites);
            Root.Add(_favorites);
        }

        public Workspace Workspace { get; }
        public Options Options => Workspace.Options;

        public ObservableCollection<TreeViewItem> Root { get; set; }

        public ICommand SelectedPathFromTreeCommand
        {
            get
            {
                return new RelayCommand(param =>
                {
                    if (param is TreeViewItem item)
                    {
                        ExpandItem(item, true);
                        var resolver = new FileItemTargetResolver(item.Path);
                        resolver.Parse();
                        Workspace.ActiveLister.Path = resolver.TargetPath;
                        Workspace.ActiveLister.Refresh();
                    }
                });
            }
        }

        private static Func<TreeViewItem> GetMyComputerItem()
        {
            return () =>
            {
                var myComputer = new TreeViewItem(@"", ItemType.Container, "My Computer") {IsExpanded = true};
                var compi = new Shell32.Shell().NameSpace(ShellSpecialFolderConstants.ssfDRIVES);
                var rootItems = compi.Items().Cast<FolderItem2>()
                    .Where(f => Directory.Exists(f.Path))
                    .Select(i => new TreeViewItem(i.Path, i.IsFolder ? ItemType.Container : ItemType.Item, i.Name));
                foreach (var item in rootItems)
                    myComputer.Children.Add(item);
                return myComputer;
            };
        }

        private void BindingOperations_CollectionRegistering(object sender, CollectionRegisteringEventArgs e)
        {
            if (e.Collection is ObservableCollection<TreeViewItem>)
                BindingOperations.EnableCollectionSynchronization(_favorites.Children, _locker);
        }

        private static void ExpandItem(TreeViewItem item, bool onlyContainers = false)
        {
            if (item.ItemType == ItemType.Item || item.IsPopulated)
                return;

            item.IsExpanded = true;
            item.IsPopulated = true;

            foreach (var fsi in Directory.EnumerateFileSystemEntries(item.Path))
            {
                var f = new FileItem(fsi, Directory.Exists(fsi) ? ItemType.Container : ItemType.Item);
                if (onlyContainers && f.ItemType != ItemType.Container)
                    continue;

                try
                {
                    var ti = new TreeViewItem(f);
                    item.Children.Add(ti);
                }
                catch (Exception)
                {
                }
            }
        }
    }
}