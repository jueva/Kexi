using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Kexi.Common;
using Kexi.Interfaces;
using Kexi.UI.Base;
using Kexi.View;
using Kexi.ViewModel;
using Kexi.ViewModel.Item;
using Kexi.ViewModel.Lister;

namespace Kexi.UI.View
{
    public partial class ListerView : IListerView
    {
        public static readonly DependencyProperty MouseMovingProperty = DependencyProperty.Register(
            "MouseMoving",
            typeof(bool),
            typeof(ListerView),
            new FrameworkPropertyMetadata(false));

        public bool MouseMoving
        {
            get => (bool) GetValue(MouseMovingProperty);
            set => SetValue(MouseMovingProperty, value);
        }

        public ListerView()
        {
            InitializeComponent();
            _detailTimer = new DispatcherTimer(DispatcherPriority.Normal)
            {
                Interval = TimeSpan.FromMilliseconds(200)
            };
            _detailTimer.Tick += DetailTimer_Tick;
        }

        public Workspace Workspace => ViewModel?.Workspace;

        public IItem CurrentItem => ListView.SelectedItem as IItem ?? ViewModel?.Items?.FirstOrDefault();

        public ILister ViewModel => DataContext as ILister;

        public GridViewColumnHeader CurrentSortColumn { get; set; }

        public SortAdorner CurrentSortAdorner { get; set; }

        public async Task ShowDetail()
        {
            var lister = ViewModel;
            if (CurrentItem == null || lister == null)
                return;

            lister.StatusString = lister.GetStatusString(CurrentItem);

            if (Workspace.Docking.DetailViewModel.IsVisible)
            {
                var provider = lister.PropertyProvider;
                if (provider != null && !Equals(provider.Item, CurrentItem))
                    try
                    {
                        if (provider.Item != null)
                            _cancellationTokenSource?.Cancel(false);

                        _cancellationTokenSource         = new CancellationTokenSource();
                        provider.CancellationTokenSource = _cancellationTokenSource;
                        await provider.SetItem(CurrentItem);
                    }
                    catch (TaskCanceledException)
                    {
                    }
            }

            if (Workspace.Docking.PreviewViewModel.IsVisible)
            {
                if (CurrentItem is FileItem fileItem)
                    if (Workspace.Docking.PreviewViewModel.Content is PreviewContentView content)
                        await content.SetItem(fileItem);
            }
        }

        public void FocusItem(IItem iitem)
        {
            if (iitem == null || ViewModel.View == null)
                return;

            var currentView = ViewModel.View.ListView;
            if (!(currentView.ItemContainerGenerator.ContainerFromItem(iitem) is ListViewItem listViewItem))
            {
                currentView.ScrollIntoView(iitem);
                listViewItem = currentView.ItemContainerGenerator.ContainerFromItem(iitem) as ListViewItem;
            }

            if (listViewItem != null)
            {
                currentView.ScrollIntoView(iitem);
                if (!(Workspace.PopupViewModel?.IsOpen ?? false))
                {
                    FocusManager.SetFocusedElement(currentView, listViewItem);
                    Keyboard.Focus(listViewItem);
                }
                ViewModel.SetSelection(iitem, true);
            }
        }

        public KexiListView ListView
        {
            get => _ListView;
            set => _ListView = value;
        }

        public void FocusCurrentOrFirst()
        {
            var item = CurrentItem;
            FocusItem(item);
        }

        private  DispatcherTimer _detailTimer;

        private CancellationTokenSource _cancellationTokenSource;


        private IItem _lastItem;

        private DependencyObject _lastObject;

        private Point _lastPosition;

        private MouseHandler _mouseHandler;

        private async void DetailTimer_Tick(object sender, EventArgs e)
        {
            _detailTimer.Stop();
            await ShowDetail();
        }

        private void ListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
                _lastItem = e.AddedItems[e.AddedItems.Count - 1] as IItem;
            else if (e.RemovedItems.Count > 0)
                _lastItem = e.RemovedItems[e.RemovedItems.Count - 1] as IItem;
            else 
                _lastItem = ListView.SelectedItem as IItem;

            if (IsLoaded)
            {
                _detailTimer?.Stop();
                _detailTimer?.Start();
            }
        }

        private async void ListerView_Loaded(object sender, RoutedEventArgs e)
        {
            if (ViewModel.Loaded)
            {
                Keyboard.Focus(ListView.ItemContainerGenerator.ContainerFromItem(_lastItem) as ListViewItem);
            }
            else
            {
                ViewModel.View   = this;
                ViewModel.Loaded = true;
                _mouseHandler    = new MouseHandler(ViewModel.Workspace);
                _mouseHandler.RegisterTo(ListView);
                FocusCurrentOrFirst();
            }

            await ShowDetail();
        }

        private void ListerView_MouseMove(object sender, MouseEventArgs e)
        {
            var currentPosition = e.GetPosition(this);
            var hit             = VisualTreeHelper.HitTest(this, currentPosition)?.VisualHit;
            if (currentPosition != _lastPosition && !Equals(hit, _lastObject))
            {
                MouseMoving   = true;
                _lastPosition = currentPosition;
                _lastObject   = hit;
            }
        }

        private void ListerView_OnKeyDown(object sender, KeyEventArgs e)
        {
            Workspace.KeyHandler.Execute(e, ViewModel);
            if (!KeyIsModifier(e.Key))
                MouseMoving = false;
        }

        private static bool KeyIsModifier(Key k)
        {
            return k == Key.LeftCtrl || k == Key.RightCtrl || k == Key.LeftAlt || k == Key.RightAlt
                || k == Key.LeftShift || k == Key.RightShift;
        }

        public void Dispose()
        {
            _mouseHandler?.Dispose();
            ListBoxSelector.SetEnabled(ListView, false);
            _cancellationTokenSource?.Dispose();
            _detailTimer.Stop();
            _detailTimer = null;
            ListView = null;
        }
    }
}