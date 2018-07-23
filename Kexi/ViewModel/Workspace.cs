using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Kexi.Common;
using Kexi.Common.KeyHandling;
using Kexi.Interfaces;
using Kexi.ViewModel.Dock;
using Kexi.ViewModel.Lister;
using Kexi.ViewModel.Popup;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout;

namespace Kexi.ViewModel
{
    [Export]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class Workspace : ViewModelBase
    {
        public Workspace()
        {
            Container             = KexContainer.Container;
            _renamePopupViewModel = new RenamePopupViewModel(this);
            Docking               = new DockViewModel(this);
            KeyHandler            = new KeyHandler(this);
            CommanderbarViewModel = new CommanderbarViewModel(this);
            PopupViewModel = new FilterPopupViewModel(this, new Options(), null);
        }

        public RenamePopupViewModel RenamePopupViewModel
        {
            get => _renamePopupViewModel;
            set
            {
                _renamePopupViewModel = value;
                OnPropertyChanged();
            }
        }

        public object ActiveLayoutContent
        {
            get => Docking.ActiveLayoutContent;
            set => Docking.ActiveLayoutContent = value;
        }

        public KeyHandler KeyHandler { get; }

        public CommanderbarViewModel CommanderbarViewModel { get; }

        [Import]
        public TemporaryFavorites<IItem> TemporaryFavorites { get; private set; }

        [Import]
        public NotificationHost NotificationHost { get; private set; }

        [Import]
        public ThemeHandler ThemeHandler { get; private set; }

        [Import]
        public CommandRepository CommandRepository { get; private set; }

        [Import]
        public Options Options { get; private set; }

        [Import]
        public AdressbarViewModel AdressbarViewModel { get; private set; }

        [Import]
        public BreadcrumbViewModel BreadcrumbViewModel { get; private set; }

        [Import]
        public FontHelper FontHelper { get; private set; }

        [Import]
        public RibbonViewModel RibbonViewModel { get; private set; }

        public double TabHeigth
        {
            get => _tabHeight;
            set
            {
                if (Math.Abs(_tabHeight - value) < 0.1)
                    return;
                _tabHeight = value;
                OnPropertyChanged();
            }
        }

        public bool HasMultipleTabs
        {
            get => _hasMultipleTabs;
            set
            {
                if (_hasMultipleTabs == value)
                    return;

                _hasMultipleTabs = value;
                OnPropertyChanged();
            }
        }

        public DockingManager       Manager   { get; set; }
        public LayoutAnchorablePane LeftPane  { get; set; }
        public LayoutAnchorablePane RightPane { get; set; }

        public DockViewModel Docking { get; }

        public DocumentViewModel ActiveDocumentView => Docking.ActiveLayoutContent as DocumentViewModel;

        public LayoutContent ActiveLayoutDocument => Docking.ActiveLayoutContent as LayoutContent;

        public ILister ActiveLister
        {
            get => _activeLister;
            set
            {
                if (value == _activeLister)
                    return;
                _activeLister = value;
                OnPropertyChanged();
            }
        }

        public IItem CurrentItem => ActiveLister?.ItemsView?.CurrentItem as IItem;

        public IEnumerable<IItem> CurrentItems => ActiveLister.Items;

        public PopupViewModel PopupViewModel
        {
            get => _popupViewModel;
            set
            {
                if (Equals(value, _popupViewModel)) return;
                _popupViewModel = value;
                OnPropertyChanged();
            }
        }

        public KexContainer Container { get; set; }

        protected LayoutDocumentPaneGroup CurrentDocumentPaneGroup => ActiveLayoutDocument == null
            ? Manager.Layout.Descendents().OfType<LayoutDocumentPaneGroup>().FirstOrDefault()
            : GetParent<LayoutDocumentPaneGroup>(ActiveLayoutDocument);

        protected LayoutDocumentPane CurrentDocumentPane
        {
            get
            {
                
                var layoutElement = Manager.Layout.LastFocusedDocument;

                var documentPane = layoutElement?.FindParent<LayoutDocumentPane>();
                return documentPane;
            }
        }

        public bool CommanderMode
        {
            get => _commanderMode;
            set
            {
                if (value.Equals(_commanderMode))
                    return;

                if (value && CurrentDocumentPaneGroup?.Children != null && CurrentDocumentPaneGroup.Children.Count == 1)
                {
                    var fileLister = KexContainer.Resolve<FileLister>();
                    fileLister.Path = ActiveLister.Path;
                    fileLister.Refresh();
                    Open(fileLister);
                    SplitVertical();
                }

                _commanderMode = value;
                OnPropertyChanged();
            }
        }

        public DocumentViewModel CommanderTargetLayoutDocument
        {
            get
            {
                if (CurrentDocumentPaneGroup.Children.Count <= 1)
                    return ActiveDocumentView;

                var pindex  = CurrentDocumentPaneGroup.Children.IndexOf(CurrentDocumentPane);
                var index   = pindex == 0 ? 1 : 0;
                var docPane = CurrentDocumentPaneGroup.Children.ElementAt(index);
                var doc     = ((LayoutDocumentPane) docPane).SelectedContent;
                var view = doc?.Content as DocumentViewModel;
                return view;
            }
        }

        public Func<ICommand> FocusSearchBoxCommand { get; set; }

        public RecentLocationPopupViewModel AdressbarHistoryDatasource => _adressbarHistoryDatasource ?? (
            _adressbarHistoryDatasource = new RecentLocationPopupViewModel(this)
            {
                HorizontalOffset = -200,
                VerticalOffset   = 5,
                Width            = 250
            });

        public RelayCommand ShowAdressbarHistoryPopupCommand
        {
            get
            {
                return _showAdressbarHistoryPopupCommand ??
                    (_showAdressbarHistoryPopupCommand = new RelayCommand(c =>
                    {
                        AdressbarHistoryDatasource.PopupTarget  = c as Button;
                        AdressbarHistoryDatasource.PopupVisible = true;
                    }));
            }
        }

        public  TextBox                      TitleTextBox    { get; set; }
        public  Border                       TitleTextBorder { get; set; }
        public  IDockingManager              DockingMananger { get; set; }

        private object                       _activeLayoutContent;
        private ILister                      _activeLister;
        private RecentLocationPopupViewModel _adressbarHistoryDatasource;
        private bool                         _commanderMode;
        private ICommand                     _editConfigurationCommand;
        private bool                         _hasMultipleTabs;
        private PopupViewModel _popupViewModel;
        private RenamePopupViewModel         _renamePopupViewModel;
        private RelayCommand                 _showAdressbarHistoryPopupCommand;
        private double                       _tabHeight;
        private ToolViewModel[]              _tools;

        public void EnsureUniquePathName()
        {
            var allLister = Docking.Files.Select(i => i.Content).OfType<FileLister>()
                .Select(f => new Tuple<string, FileLister>(f.PathName, f)).ToList();

            foreach (var l in allLister)
            foreach (var s in allLister.Where(a => Path.GetFileName(a.Item2.Path) == Path.GetFileName(l.Item2.Path) && a.Item2 != l.Item2))
            {
                var up = Directory.GetParent(s.Item2.Path);
                if (up != null)
                {
                    var parent  = Path.GetFileName(up.FullName);
                    var current = Path.GetFileName(s.Item2.Path);
                    s.Item2.PathName = $"{parent}/{current}";
                }
            }
        }

        public IEnumerable<T> CurrentItemsOfType<T>()
        {
            return ActiveLister.Items.OfType<T>();
        }

        public IEnumerable<T> GetSelection<T>() where T : IItem
        {
            return ActiveLister.SelectedItems.Cast<T>();
        }

        public void Open(ILister lister)
        {
            lister.GotItems += Lister_GotItems;
            var ld = new DocumentViewModel
            {
                CanClose   = true,
                ContentId  = Guid.NewGuid().ToString(),
                IsActive   = true,
                IsSelected = true,
                Content    = lister
            };
            ld.PropertyChanged += Ld_PropertyChanged;
            Docking.Files.Add(ld);
            ActiveLayoutContent = ld;
            HasMultipleTabs     = Manager.Layout.Descendents().OfType<LayoutDocumentPane>().Count() > 1;
            SetTabsVisibility();
        }

        private void Ld_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var ld = sender as DocumentViewModel;
            if (e.PropertyName == nameof(DocumentViewModel.IsClosed))
                if (ld.IsClosed)
                    Docking.Files.Remove(ld);
        }

        private void Lister_GotItems(object sender, EventArgs e)
        {
            if (sender is ILister lister)
            {
                lister.GotItems -= Lister_GotItems;
                lister.View?.ListView?.Dispatcher.BeginInvoke(DispatcherPriority.Background, (Action) (() => lister.View.FocusCurrentOrFirst()));
            }
        }

        public ILister CreateListerByProtocol(string protocol)
        {
            return KexContainer.ResolveMany<ILister>().FirstOrDefault(l => l.ProtocolPrefix.ToLower() == protocol);
        }

        public void ReplaceCurrentLister(ILister lister)
        {
            CloseCurrentLister();
            Open(lister);
            lister.Refresh();
        }

        public void CycleTab(int direction)
        {
            var docs = Manager.Layout.Descendents().OfType<LayoutDocument>().Select(c => c.Content).ToArray();
            for (var i = 0; i < docs.Length; i++)
                if (docs[i] == ActiveLayoutContent)
                {
                    var targetIndex = i + direction;
                    if (targetIndex < 0)
                        targetIndex = docs.Length - 1;
                    else if (targetIndex > docs.Length - 1)
                        targetIndex = 0;
                    ActiveLayoutContent = docs[targetIndex];
                    break;
                }
        }

        public void SplitHorizontal()
        {
            var content = Manager.Layout.ActiveContent;
            ExecuteNewHorizontalTabGroupCommand(content, null);
        }

        public void SplitVertical()
        {
            var content = Manager.Layout.ActiveContent;
            ExecuteNewVerticalTabGroupCommand(content, null);
        }

        public void MoveToNextTab()
        {
            if (!CanExecuteMoveToNextTabGroupCommand(null))
                return;

            var layoutElement = Manager.Layout.LastFocusedDocument;
            var paneGroup     = layoutElement.FindParent<LayoutDocumentPaneGroup>();
            var pane = paneGroup.Children.FirstOrDefault(
                c => c.Children.Cast<LayoutDocument>().Any(f => Equals(f.Content, ActiveLayoutContent)));
            var root = pane?.Root;
            var num  = paneGroup.IndexOfChild(pane);
            (paneGroup.Children[num + 1] as LayoutDocumentPane)?.InsertChildAt(0, layoutElement);
            layoutElement.Root.CollectGarbage();
            if (pane?.ChildrenCount == 0)
                root?.CollectGarbage();

            ActiveLayoutContent    = layoutElement.Content as LayoutDocument;
            layoutElement.IsActive = true;
            FocusCurrentOrFirst();
        }

        private bool CanExecuteMoveToNextTabGroupCommand(object parameter)
        {
            var layoutElement = Manager.Layout.LastFocusedDocument;
            var paneGroup     = layoutElement.FindParent<LayoutDocumentPaneGroup>();
            var pane = paneGroup.Children.FirstOrDefault(
                c => c.Children.Cast<LayoutDocument>().Any(f => Equals(f.Content, ActiveLayoutContent)));

            var paneIndex = paneGroup.IndexOfChild(pane);
            if (pane != null && paneGroup.ChildrenCount > 1 && paneIndex < paneGroup.ChildrenCount)
                return paneGroup.Children[paneIndex + 1] is LayoutDocumentPane;
            return false;
        }

        public void MoveToPreviousTab()
        {
            if (!CanExecuteMoveToPreviousTabGroupCommand(null))
                return;

            var layoutElement = Manager.Layout.LastFocusedDocument;
            var paneGroup     = layoutElement.FindParent<LayoutDocumentPaneGroup>();
            var pane = paneGroup.Children.FirstOrDefault(
                c => c.Children.Cast<LayoutDocument>().Any(f => Equals(f.Content, ActiveLayoutContent)));
            var root = pane?.Root;
            var num  = paneGroup.IndexOfChild(pane);
            (paneGroup.Children[num - 1] as LayoutDocumentPane)?.InsertChildAt(0, layoutElement);
            layoutElement.Root.CollectGarbage();
            if (pane?.ChildrenCount == 0)
                root?.CollectGarbage();

            ActiveLayoutContent    = layoutElement.Content as LayoutDocument;
            layoutElement.IsActive = true;
            FocusCurrentOrFirst();
        }

        private bool CanExecuteMoveToPreviousTabGroupCommand(object parameter)
        {
            var layoutElement = Manager.Layout.LastFocusedDocument;
            var paneGroup     = layoutElement.FindParent<LayoutDocumentPaneGroup>();
            var pane = paneGroup.Children.FirstOrDefault(
                c => c.Children.Cast<LayoutDocument>().Any(f => Equals(f.Content, ActiveLayoutContent)));

            if (pane != null && paneGroup.ChildrenCount > 1 && paneGroup.IndexOfChild(pane) > 0)
                return paneGroup.Children[paneGroup.IndexOfChild(pane) - 1] is LayoutDocumentPane;
            return false;
        }

        private static void ExecuteNewHorizontalTabGroupCommand(LayoutContent lElement, object parameter)
        {
            var layoutElement      = lElement;
            var documentPaneGroup1 = layoutElement.FindParent<LayoutDocumentPaneGroup>();
            if (documentPaneGroup1.ChildrenCount >= 2)
                return;
            var layoutDocumentPane1 = layoutElement.Parent as LayoutDocumentPane;
            documentPaneGroup1.Orientation = Orientation.Vertical;
            var num = documentPaneGroup1.IndexOfChild(layoutDocumentPane1);
            documentPaneGroup1.InsertChildAt(num + 1, new LayoutDocumentPane(layoutElement));
            layoutElement.IsActive = true;
            layoutElement.Root.CollectGarbage();
        }

        private void ExecuteNewVerticalTabGroupCommand(LayoutContent lElement, object parameter)
        {
            var layoutElement       = lElement;
            var documentPaneGroup1  = layoutElement.FindParent<LayoutDocumentPaneGroup>();
            var layoutDocumentPane1 = layoutElement.Parent as LayoutDocumentPane;
            documentPaneGroup1.Orientation = Orientation.Horizontal;
            var num     = documentPaneGroup1.IndexOfChild(layoutDocumentPane1);
            var newPane = new LayoutDocumentPane(layoutElement);
            documentPaneGroup1.InsertChildAt(num + 1, newPane);
        }

        public void FocusItem(IItem iitem) //TODO: check if this has really to be called from workspace
        {
            ActiveLister.View.FocusItem(iitem);
        }

        public void FocusCurrentOrFirst() //TODO: check if this has really to be called from workspace
        {
            ActiveLister?.View?.FocusCurrentOrFirst();
        }

        public void FocusListView(bool clearSelection = true)
        {
            var listView = ActiveLister?.View?.ListView;
            if (listView == null)
                return;

            listView.ScrollIntoView(listView.SelectedItem);
            var temp = listView.ItemContainerGenerator.ContainerFromItem(listView.SelectedItem ?? CurrentItems.FirstOrDefault());
            var container = temp as UIElement;
            if (clearSelection)
                container?.Focus();
            else
                Keyboard.Focus(container);
        }

        public void CloseCurrentLister()
        {
            ActiveLister.Filter  = null;
            ActiveLister.GroupBy = null;
            CloseLister(ActiveLister);
        }

        public void CloseLister(ILister lister)
        {
            if (lister == null)
                throw new ArgumentNullException(nameof(lister));

            var documents = Manager.Layout.Descendents().OfType<LayoutDocument>().OrderByDescending(d => d.LastActivationTimeStamp).ToArray();
            if (documents.Length <= 1)
                return;

            var current = documents.SingleOrDefault(d => d.Content == ActiveLayoutContent);
            if (current == null)
                return;
            current.Close();
            lister.Dispose();
            SetTabsVisibility();
        }

        internal void SetTabsVisibility()
        {
            var active =
                Manager.Layout.Descendents()
                    .OfType<LayoutDocument>()
                    .Count();
            TabHeigth = active > 1 ? double.NaN : 0;
        }

        private static T GetParent<T>(ILayoutElement content) where T : class, ILayoutContainer
        {
            var parent = content.Parent;
            while (parent != null)
            {
                if (parent is T casted)
                    return casted;
                parent = parent.Parent;
            }

            return null;
        }
    }
}