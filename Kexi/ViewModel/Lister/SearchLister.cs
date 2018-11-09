using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;
using Kexi.Common;
using Kexi.Composition;
using Kexi.Files;
using Kexi.Interfaces;
using Kexi.ItemProvider;
using Kexi.ViewModel.Item;

namespace Kexi.ViewModel.Lister
{
    [Export(typeof(ILister))]
    [Export(typeof(SearchLister))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class SearchLister : FileLister
    {
        [ImportingConstructor]
        public SearchLister(Workspace workspace, INotificationHost notificationHost, Options options, CommandRepository commandRepository
        ) : base(workspace, notificationHost, options, commandRepository)
        {
            Title                        =  null;
            SearchItemProvider           =  new SearchItemProvider();
            SearchItemProvider.ItemAdded += ItemProvider_ItemAdded;
            SearchItemProvider.SearchFinished += () =>
            {
                PathName += " - Finished";
                SearchFinished?.Invoke();
            };
            Items     = new ObservableCollection<FileItem>();
            Thumbnail = Utils.GetImageFromRessource("search.png");
        }

        public string SearchPattern { get; set; }

        public override IEnumerable<Column> Columns { get; } = new ObservableCollection<Column>
        {
            new Column("", "Thumbnail", ColumnType.Image),
            new Column("Name", "DisplayName", ColumnType.Highlightable) {Width = 200},
            new Column("LastModified", "Details.LastModified") {Width          = 150},
            new Column("Type", "Details.Type", ColumnType.Text, ColumnSize.Auto) {Width        = 180},
            new Column("ContainingFolder", "Directory") {Width                 = 500}
        };

        public int MaxItems
        {
            get => _maxItems;
            set
            {
                if (value == _maxItems)
                    return;

                _maxItems = value;
                OnNotifyPropertyChanged();
            }
        }

        public override string ProtocolPrefix => "search";

        public SearchItemProvider SearchItemProvider { get; }

        private int _maxItems;

        private IItem _lastFirst;

        protected override void GotTheView(ILister obj)
        {
            View.ListView.ItemContainerGenerator.StatusChanged += ItemContainerGenerator_StatusChanged;
            base.GotTheView(obj);
            ContextMenuItems = ContextMenuItems.Concat(
                ContextMenuItems = KexContainer.Container.InnerCompositionContainer.GetExports<ICommand, IExportCommandMetadata>()
                    .Where(e => e.Metadata.TargetListerType == typeof(FileLister))
                    .Select(e => new CommandBoundItem(e.Metadata.Name, e.Value)));
        }

        private void ItemContainerGenerator_StatusChanged(object sender, EventArgs e)
        {
            if (View.ListView.SelectedIndex != -1)
                return;

            var gen = sender as ItemContainerGenerator;
            if (gen?.Status == GeneratorStatus.ContainersGenerated)
            {
                ItemsView.MoveCurrentToFirst();
                Keyboard.Focus(gen.ContainerFromItem(ItemsView.CurrentItem) as ListViewItem);
            }
        }

        public event Action SearchFinished;

        private void ItemProvider_ItemAdded(FileItem item)
        {
            PathName = $"Search {SearchPattern} ({Items.Count})";
        }

        public override async Task Refresh()
        {
            PathName        = "Search " + SearchPattern;
            HighlightString = SearchPattern;
            var pattern = SearchPattern;
            SearchItemProvider.ItemAdded += SearchItemProvider_ItemAdded;
            await SearchItemProvider.GetItems(Path, pattern, Items);
        }

        protected override Task<IEnumerable<FileItem>> GetItems() => null;

        private void SearchItemProvider_ItemAdded(FileItem obj)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var it = ItemsView.Cast<IItem>().FirstOrDefault();
                if (!Equals(it, _lastFirst))
                {
                    Workspace.FocusItem(it);
                    _lastFirst = it;
                }
            }, DispatcherPriority.Background);
        }

        public override async void DoAction(FileItem searchItem)
        {
            var fItem  = new FileItem(searchItem.Path, searchItem.ItemType);
            var result = new FileListerAction(Workspace, fItem).DoAction();
            if (result != null)
            {
                var fileLister = KexContainer.Resolve<FileLister>();
                fileLister.Path = fItem.Path;
                Workspace.Open(fileLister);
                await fileLister.Refresh();
            }
        }

        public override string GetParentContainer()
        {
            return null;
        }

        [ExportContextMenuCommand(typeof(SearchLister), "Cancel Search")]
        public ICommand CancelSearchCommand
        {
            get
            {
                return new RelayCommand(c =>
                {
                    if (Workspace.ActiveLister is SearchLister searchLister)
                    {
                        searchLister.SearchItemProvider.CancellationTokenSource.Cancel();
                    }
                });
            }
        }
    }
}