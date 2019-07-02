using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Kexi.Common;
using Kexi.Common.MultiSelection;
using Kexi.Composition;
using Kexi.Interfaces;
using Kexi.Property;
using Kexi.ViewModel.Commands;
using Kexi.ViewModel.Item;

namespace Kexi.ViewModel.Lister
{
    public abstract class BaseLister<T> : ILister<T>, ICanCopyPaste
        where T : class, IItem
    {
        [ImportingConstructor]
        protected BaseLister(Workspace workspace, Options options, CommandRepository commandRepository)
        {
            Workspace = workspace;
            SortHandler = new SortHandler(this);
            NotificationHost = workspace.NotificationHost;
            GotView += GotTheView;
            Options = options;
            Options.PropertyChanged += Options_PropertyChanged;
            CommandRepository = commandRepository;

            PropertyProvider = GetPropertyProvider();

            _loadingSpinnerTimer = new DispatcherTimer {Interval = TimeSpan.FromSeconds(1)};
            _loadingSpinnerTimer.Tick += LoadingSpinnerTimer_Tick;
        }

        private void Options_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Options.Highlights))
            {
                HighlightString = Options.Highlights ? Filter : "";
            }
        }

        public Visibility LoadingSpinnerVisibility
        {
            get => _loadingSpinnerVisibility;
            set
            {
                if (_loadingSpinnerVisibility == value)
                    return;
                _loadingSpinnerVisibility = value;
                OnNotifyPropertyChanged();
            }
        }

        public string ContentId => "Lister" + GetHashCode();

        public CommandRepository CommandRepository { get; }

        public virtual Style CurrentContainerStyle
        {
            get => _currentContainerStyle;
            set
            {
                _currentContainerStyle = value;
                OnNotifyPropertyChanged();
            }
        }

        public SortHandler SortHandler { get; }

        public virtual void Copy()
        {
            var selection = ItemsView.SelectedItems.Select(s => s.DisplayName);
            var text = string.Join(Environment.NewLine, selection);
            Clipboard.SetText(text);
        }

        public virtual void Cut()
        {
            Copy();
            NotificationHost.AddInfo("Copied. Cut Action not supported.");
        }

        public virtual void Paste()
        {
            NotificationHost.AddInfo("Paste Action not supported");
        }

        public virtual void Undo()
        {
            NotificationHost.AddInfo("Undo Action not supported");
        }

        public Workspace Workspace { get; }

        public string HighlightString
        {
            get => _highlightString;
            set
            {
                _highlightString = value;
                OnNotifyPropertyChanged();
            }
        }

        public IItem CurrentItem => ItemsView?.CurrentItem as IItem;

        public Options Options { get; }

        public string Path
        {
            get => _path;
            set
            {
                if (_path == value)
                    return;

                OnPathChanging(value);
                _previousPath = _path;
                _path = value;
                OnPathChanged(value);

                OnNotifyPropertyChanged();
            }
        }

        public event EventHandler GotItems;

        public string Title
        {
            get => _title;
            set
            {
                if (_title == value)
                    return;

                _title = value;
                OnNotifyPropertyChanged();
            }
        }

        public string Filter
        {
            get => _filter;
            set
            {
                if (_filter == value)
                    return;

                if (Options.Highlights) HighlightString = value;
                _filter = value;
                OnNotifyPropertyChanged();
            }
        }

        public NotificationItem Notification
        {
            get => _notification;
            set
            {
                if (_notification == value)
                    return;

                _notification = value;
                OnNotifyPropertyChanged();
            }
        }

        public IListerView View
        {
            get => _view;
            set
            {
                if (_view == value)
                    return;

                _view = value;
                OnNotifyPropertyChanged();
                OnGotView(this);
            }
        }

        public string GroupBy
        {
            get => _groupBy;
            set
            {
                if (_groupBy == value)
                    return;
                _groupBy = value;
                ItemsView?.GroupDescriptions.Clear();
                if (value != null)
                    ItemsView?.GroupDescriptions.Add(new PropertyGroupDescription(value));
                OnNotifyPropertyChanged();
            }
        }

        public virtual void ShowContextMenu(IEnumerable<IItem> selectedItems)
        {
            CommandRepository.GetCommandByName(nameof(ShowListerContextMenuPopupCommand)).Execute(true);
        }

        public event Action<ILister> GotView;
        public event Action<string> PathChanged;

        public virtual string ProtocolPrefix => "";

        public BitmapSource Thumbnail
        {
            get => _thumbnail;
            set
            {
                if (Equals(value, _thumbnail)) return;
                _thumbnail = value;
                OnNotifyPropertyChanged();
            }
        }

        public string PathName
        {
            get => _pathName;
            set
            {
                if (value == _pathName)
                    return;

                _pathName = value;
                OnNotifyPropertyChanged();
            }
        }

        public abstract ObservableCollection<Column> Columns { get; }

        public ObservableCollection<Column> VisibleColumns { get; private set; }

        public virtual async Task Refresh(bool clearFilterAndGroup = true)
        {
            try
            {
                LoadingStatus = LoadingStatus.Loading;
                var items = await GetItems();
                Items = new ObservableCollection<T>(items);

                if (this is IBackgroundLoader preloadDetails)
                {
                    _cancellation?.Cancel();
                    var newCancellation = new CancellationTokenSource();
                    new Task(
                        () => preloadDetails.LoadBackgroundData(Items, newCancellation.Token),
                        newCancellation.Token).Start();
                    _cancellation = newCancellation;
                }

                if (this is IHistorisationProvider history)
                {
                    _oldFilter = Filter;
                    _oldSortExpression = SortHandler.CurrentSortDescription;
                    _oldGroupBy = GroupBy;
                    history.History.Push(Path, _oldFilter, _oldGroupBy, _oldSortExpression);
                }

                if (clearFilterAndGroup)
                {
                    Filter = null;
                    GroupBy = null;
                }

                SortHandler.ClearSort();
            }
            catch (Exception ex)
            {
                NotificationHost.AddError(ex.Message, ex.ToString());
                if (this is IHistorisationProvider history && history.History.Current != null)
                {
                    history.History.Current.SelectedPath = _previousPath;
                    Path = _previousPath;
                    CommandRepository.GetCommandByName(nameof(MoveToHistoryItemCommand))
                        .Execute(history.History.Current);
                    if (Workspace.PopupViewModel.IsOpen)
                        Workspace.PopupViewModel.IsOpen = false;
                }
            }
            finally
            {
                LoadingStatus = LoadingStatus.Loaded;
            }
        }

        public void DoAction(IItem item)
        {
            DoAction(item as T);
        }

        public virtual void DoAction(T item)
        {
        }

        public virtual string GetParentContainer()
        {
            return null;
        }

        public virtual string StatusString
        {
            get => _statusString;
            set
            {
                _statusString = value;
                OnNotifyPropertyChanged();
            }
        }

        IEnumerable<IItem> ILister.SelectedItems => SelectedItems;
        public IEnumerable<T> SelectedItems => ItemsView.SelectedItems;

        IEnumerable<IItem> ILister.Items => Items;

        public ObservableCollection<T> Items
        {
            get => _items;
            protected set
            {
                _items = value;
                ItemsView = value == null
                    ? null
                    : new MultiSelectCollectionView<T>(_items);
                OnNotifyPropertyChanged();
            }
        }

        ICollectionView ILister.ItemsView => ItemsView;

        public MultiSelectCollectionView<T> ItemsView
        {
            get => _itemsView;
            set
            {
                if (View?.ListView != null)
                    View.ListView.ItemContainerGenerator.StatusChanged += ItemContainerGenerator_StatusChanged;
                else
                    GotView += BaseLister_GotView;

                _itemsView = value;
                _itemsView?.MoveCurrentToFirst();
                OnNotifyPropertyChanged();
            }
        }

        public void ClearSelection()
        {
            SelectedItems.ToArray().Foreach(i => Workspace.ActiveLister.SetSelection(i, false));
        }

        public void SetSelection(IItem item, bool selected)
        {
            if (item == null || View == null)
                return;

            var currentView = View.ListView;
            if (!(currentView.ItemContainerGenerator.ContainerFromItem(item) is ListViewItem listViewItem))
            {
                currentView.ScrollIntoView(item);
                listViewItem = currentView.ItemContainerGenerator.ContainerFromItem(item) as ListViewItem;
            }

            if (listViewItem != null)
            {
                currentView.ScrollIntoView(item);
                listViewItem.IsSelected = selected;
            }
        }

        public virtual ViewType CurrentViewMode
        {
            get => _currentViewMode;
            set
            {
                _currentViewMode = value;

                OnNotifyPropertyChanged();
            }
        }

        public virtual IEnumerable<CommandBoundItem> ContextMenuItems { get; protected set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public LoadingStatus LoadingStatus
        {
            get => _loadingStatus;
            set
            {
                if (_loadingStatus == value)
                    return;

                if (value == LoadingStatus.Loading)
                {
                    _loadingSpinnerTimer?.Start(); //Got probs here. Disposed? => Check null
                }
                else
                {
                    _loadingSpinnerTimer?.Stop();
                    LoadingSpinnerVisibility = Visibility.Collapsed;
                }

                _loadingStatus = value;
                OnNotifyPropertyChanged();
            }
        }

        public virtual bool ShowInMenu => true;

        public bool Loaded { get; set; }

        public virtual bool SupportsMultiview => false;

        public IPropertyProvider PropertyProvider
        {
            get => _propertyProvider;
            private set
            {
                if (_propertyProvider == value)
                    return;
                _propertyProvider = value;
                OnNotifyPropertyChanged();
            }
        }

        public virtual string GetStatusString()
        {
            if (View == null || Items == null || ItemsView == null)
                return null;

            var selected = View.ListView.SelectedItems;
            var count = selected.Count;
            return $"{Items.Count} Items, {count} selected";
        }

        public void Dispose()
        {
            Options.PropertyChanged -= Options_PropertyChanged;
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected readonly INotificationHost NotificationHost;
        private Style _currentContainerStyle;
        private ViewType _currentViewMode;
        private string _filter;
        private string _groupBy;
        private string _highlightString;
        private ObservableCollection<T> _items;
        private MultiSelectCollectionView<T> _itemsView;
        private Visibility _loadingSpinnerVisibility = Visibility.Collapsed;
        private LoadingStatus _loadingStatus;
        private NotificationItem _notification;
        private string _oldFilter;
        private string _oldGroupBy;
        private SortDescription _oldSortExpression;
        private string _path;
        private string _pathName;
        private IPropertyProvider _propertyProvider;
        private string _statusString;
        private BitmapSource _thumbnail;
        private string _title;
        private IListerView _view;
        private CancellationTokenSource _cancellation;
        private DispatcherTimer _loadingSpinnerTimer;
        private string _previousPath;

        private IPropertyProvider GetPropertyProvider()
        {
            var allProviders = KexContainer.Container.InnerCompositionContainer
                .GetExports<IPropertyProvider, IExportPropertyProviderMetadata>();
            var propProvider = allProviders.SingleOrDefault(e => e.Metadata.TargetListerType == GetType());
            return propProvider?.Value ?? new DefaultPropertyProvider(Workspace);
        }

        protected abstract Task<IEnumerable<T>> GetItems();

        private void LoadingSpinnerTimer_Tick(object sender, EventArgs e)
        {
            LoadingSpinnerVisibility = Visibility.Visible;
            _loadingSpinnerTimer.Stop();
        }

        public event Action<string> PathChanging;

        private void BaseLister_GotView(ILister obj)
        {
            GotView -= BaseLister_GotView;
            View.ListView.ItemContainerGenerator.StatusChanged += ItemContainerGenerator_StatusChanged;
        }

        protected virtual void GotTheView(ILister obj)
        {
            ContextMenuItems = KexContainer.Container.InnerCompositionContainer
                .GetExports<ICommand, IExportCommandMetadata>()
                .Where(e => e.Metadata.TargetListerType == GetType())
                .Select(e => new CommandBoundItem(e.Metadata.Name, e.Value))
                .Concat(new[] {new CommandBoundItem("Copy", new CopyCommand(Workspace))})
                .Concat(new[] {new CommandBoundItem("Copy Path", new CopyPathCommand(Workspace))});

            VisibleColumns = Columns;
            CurrentViewMode = Options.DefaultViewMode;
        }

        protected void OnGotView(ILister obj)
        {
            GotView?.Invoke(obj);
        }

        protected void OnPathChanged(string value)
        {
            PathChanged?.Invoke(value);
        }

        protected void OnPathChanging(string value)
        {
            PathChanging?.Invoke(value);
        }

        protected void OnGotItems(object sender, EventArgs args)
        {
            GotItems?.Invoke(sender, args);
        }

        private void ItemContainerGenerator_StatusChanged(object sender, EventArgs e)
        {
            var gen = sender as ItemContainerGenerator;
            if (gen?.Status == GeneratorStatus.ContainersGenerated)
            {
                Workspace.ActiveLister?.View?.FocusCurrentOrFirst();
                View.ListView.ItemContainerGenerator.StatusChanged -= ItemContainerGenerator_StatusChanged;
                OnGotItems(this, EventArgs.Empty);
            }
        }

        protected void OnNotifyPropertyChanged([CallerMemberName] string property = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                GotView -= GotTheView;
                View?.Dispose();
                View = null;
                if (_loadingSpinnerTimer != null)
                {
                    _loadingSpinnerTimer.Tick -= LoadingSpinnerTimer_Tick;
                    _loadingSpinnerTimer.Stop();
                    _loadingSpinnerTimer = null;
                }

                if (PropertyProvider is IDisposable disposable)
                    disposable.Dispose();
                Items = null;
            }
        }

        ~BaseLister()
        {
            Dispose(false);
        }
    }
}