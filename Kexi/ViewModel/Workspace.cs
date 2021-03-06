﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
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
        [ImportingConstructor]
        public Workspace([ImportMany] IEnumerable<Lazy<ILister>> listers, Options options, CommandRepository commandRepository)
        {
            _listers              = listers;
            Options               = options;
            CommandRepository     = commandRepository;
            RenamePopupViewModel  = new RenamePopupViewModel(this);
            Docking               = new DockViewModel(this);
            KeyDispatcher         = new KeyDispatcher(this);
            CommanderbarViewModel = new CommanderbarViewModel(this);
            TaskManager           = new TaskManager(this);
            NotificationHost      = new NotificationHost(this);
            ThemeHandler          = new ThemeHandler(this);
            AddressbarViewModel   = new AdressbarViewModel(this);
            RibbonViewModel       = new RibbonViewModel(this);
            TemporaryFavorites    = new TemporaryFavorites<IItem>();
            PopupViewModel = new FilterPopupViewModel(this, options, null);
        }

        public Options Options { get; set; }

        public TemporaryFavorites<IItem> TemporaryFavorites { get; }

        public NotificationHost NotificationHost { get; }

        public ThemeHandler ThemeHandler { get; }

        public CommandRepository CommandRepository { get; }

        public AdressbarViewModel AddressbarViewModel { get; }

        public RibbonViewModel RibbonViewModel { get; }

        public KeyDispatcher KeyDispatcher { get; }

        public CommanderbarViewModel CommanderbarViewModel { get; }

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

        public TaskManager    TaskManager { get; }
        public DockingManager Manager     { get; set; }

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

        protected LayoutDocumentPaneGroup CurrentDocumentPaneGroup => ActiveLayoutDocument == null
            ? Manager.Layout.Descendents().OfType<LayoutDocumentPaneGroup>().FirstOrDefault()
            : Utils.GetParentLayoutContainer<LayoutDocumentPaneGroup>(ActiveLayoutDocument);

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
                    RefreshAndSplit();

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
                var view    = doc?.Content as DocumentViewModel;
                return view;
            }
        }

        public RecentLocationPopupViewModel AdressbarHistoryDatasource => _adressbarHistoryDatasource ?? (
            _adressbarHistoryDatasource = new RecentLocationPopupViewModel(this)
            {
                HorizontalOffset = -200,
                VerticalOffset   = 5,
                Width            = 250
            });

        public TextBox         TitleTextBox    { get; set; }
        public Border          TitleTextBorder { get; set; }
        public IDockingManager DockingMananger { get; set; }

        private readonly IEnumerable<Lazy<ILister>> _listers;

        private ILister                      _activeLister;
        private RecentLocationPopupViewModel _adressbarHistoryDatasource;
        private bool                         _commanderMode;
        private PopupViewModel               _popupViewModel;
        private RenamePopupViewModel         _renamePopupViewModel;

        private async void RefreshAndSplit()
        {
            var fileLister = KexContainer.Resolve<FileLister>();
            fileLister.Path = ActiveLister.Path;
            await fileLister.Refresh();
            Open(fileLister);
            SplitVertical();
        }


        public IEnumerable<T> CurrentItemsOfType<T>()
        {
            return ActiveLister.Items.OfType<T>();
        }

        public IEnumerable<T> GetSelection<T>() where T : IItem
        {
            return ActiveLister.SelectedItems.Cast<T>();
        }

        public DocumentViewModel Open(ILister lister, bool isActive = true, bool selected = true)
        {
            if (isActive)
                lister.GotItems += Lister_GotItems;

            var ld = new DocumentViewModel
            {
                CanClose  = true,
                ContentId = Guid.NewGuid().ToString(),
                Content   = lister
            };
            ld.PropertyChanged += Ld_PropertyChanged;
            Docking.Files.Add(ld);
            ld.IsSelected = selected;
            if (isActive)
            {
                ld.IsActive         = true;
                ActiveLayoutContent = ld;
            }

            return ld;
        }

        private void Ld_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is DocumentViewModel ld && e.PropertyName == nameof(DocumentViewModel.IsClosed) && ld.IsClosed)
            {
                Docking.Files.Remove(ld);
            }
        }

        private void Lister_GotItems(object sender, EventArgs e)
        {
            if (sender is ILister lister)
            {
                lister.GotItems -= Lister_GotItems;
                lister.View?.ListView?.Dispatcher?.BeginInvoke(DispatcherPriority.Background, (Action) (() => lister.View?.FocusCurrentOrFirst()));
            }
        }

        public ILister CreateListerByProtocol(string protocol)
        {
            return _listers.FirstOrDefault(l => l.Value.ProtocolPrefix.ToLower() == protocol)?.Value;
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
            ExecuteNewHorizontalTabGroupCommand(content);
        }

        public void SplitVertical()
        {
            var content = Manager.Layout.ActiveContent;
            ExecuteNewVerticalTabGroupCommand(content);
        }

        public void MoveToNextTab()
        {
            if (!CanExecuteMoveToNextTabGroupCommand())
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

        private bool CanExecuteMoveToNextTabGroupCommand()
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
            if (!CanExecuteMoveToPreviousTabGroupCommand())
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

        private bool CanExecuteMoveToPreviousTabGroupCommand()
        {
            var layoutElement = Manager.Layout.LastFocusedDocument;
            var paneGroup     = layoutElement.FindParent<LayoutDocumentPaneGroup>();
            var pane = paneGroup.Children.FirstOrDefault(
                c => c.Children.Cast<LayoutDocument>().Any(f => Equals(f.Content, ActiveLayoutContent)));

            if (pane != null && paneGroup.ChildrenCount > 1 && paneGroup.IndexOfChild(pane) > 0)
                return paneGroup.Children[paneGroup.IndexOfChild(pane) - 1] is LayoutDocumentPane;
            return false;
        }

        private static void ExecuteNewHorizontalTabGroupCommand(LayoutContent lElement)
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

        private void ExecuteNewVerticalTabGroupCommand(LayoutContent lElement)
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
            var temp      = listView.ItemContainerGenerator.ContainerFromItem(listView.SelectedItem ?? CurrentItems.FirstOrDefault());
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

            if (Docking.Files.Count <= 1)
                return;

            var current = Docking.Files.FirstOrDefault(f => f.Content == lister);
            Docking.Files.Remove(current);
            lister.Dispose();
        }
    }
}