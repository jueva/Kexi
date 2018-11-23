using Kexi.Files;
using Kexi.Interfaces;
using Kexi.ViewModel;
using Kexi.ViewModel.Commands;
using Kexi.ViewModel.Item;
using Kexi.ViewModel.Lister;
using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Kexi.Common
{
    [Export]
    public class MouseHandler : IDisposable
    {
        [ImportingConstructor]
        public MouseHandler(Workspace workspace)
        {
            _workspace         = workspace;
            _options           = workspace.Options;
            _sortHandler       = new SortHandler(workspace.ActiveLister);
            _commandRepository = workspace.CommandRepository;
            _notificationHost  = workspace.NotificationHost;
        }

        public bool Dragged { get; set; }

        public void Dispose()
        {
            _listView.Drop                       -= DropList_Drop;
            _listView.DragEnter                  -= DropList_DragEnter;
            _listView.QueryContinueDrag          -= _listView_QueryContinueDrag;
            _listView.PreviewMouseLeftButtonDown -= List_PreviewMouseLeftButtonDown;
            _listView.PreviewMouseMove           -= List_MouseMove;
            _listView.MouseUp                    -= _listView_MouseUp;
            _listView                            =  null;
            _clickTimer?.Stop();
            _clickTimer = null;
        }

        private readonly CommandRepository _commandRepository;
        private readonly string            _dragFormat = DataFormats.FileDrop;
        private readonly INotificationHost _notificationHost;
        private readonly Options           _options;
        private readonly SortHandler       _sortHandler;
        private readonly Workspace         _workspace;
        private          DispatcherTimer   _clickTimer;

        private ListView _listView;
        private bool     _dragCanceled;
        private Point?   _dragStart;
        private bool     _mouseupSinceRenameInit;
        private bool     _multipleItemsClicked;

        private ModifierKeys _multipleItemsClickedModifier;
        private bool         _showRenameOnMouseup;

        public void RegisterTo(ListView listerView)
        {
            if (Equals(listerView, _listView))
                return;
            _listView                            =  listerView;
            _listView.Drop                       += DropList_Drop;
            _listView.DragEnter                  += DropList_DragEnter;
            _listView.QueryContinueDrag          += _listView_QueryContinueDrag;
            _listView.PreviewMouseLeftButtonDown += List_PreviewMouseLeftButtonDown;
            _listView.PreviewMouseLeftButtonUp   += _listView_PreviewMouseLeftButtonUp;
            _listView.PreviewMouseMove           += List_MouseMove;
            _listView.MouseUp                    += _listView_MouseUp;

            _clickTimer = new DispatcherTimer(
                TimeSpan.FromMilliseconds(_options.DoubleClickTime),
                DispatcherPriority.Background,
                ClickTimer_Tick,
                Dispatcher.CurrentDispatcher
            );
            _clickTimer.Stop();
        }

        private void _listView_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is Visual visual)
            {
                var header = visual as GridViewColumnHeader ?? Utils.FindParent<GridViewColumnHeader>(visual);
                if (header != null)
                {
                    var clickPos = e.GetPosition(header).X;
                    var offset   = header.ActualWidth - clickPos;
                    if (offset > 6 && clickPos > 6)
                    {
                        _sortHandler.HandleSorting(header);
                        _workspace.FocusListView();
                    }
                }
            }
        }

        private void _listView_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var item = Utils.GetDataContextFromOriginalSource(e.OriginalSource) as IItem;
            if (item == null && e.ChangedButton == MouseButton.Left)
                return;

            _dragStart = null;
            switch (e.ChangedButton)
            {
                case MouseButton.Left:
                    HandleLeftButtonUp(e, item);
                    break;
                case MouseButton.Middle:
                    _workspace.ActiveLister.Path = _workspace.ActiveLister.GetParentContainer();
                    _workspace.ActiveLister.Refresh();
                    e.Handled = true;
                    break;
                case MouseButton.Right:
                    _workspace.ActiveLister.ShowContextMenu();
                    e.Handled = true;
                    break;
            }
        }

        private void HandleLeftButtonUp(MouseButtonEventArgs e, IItem item)
        {
            if (!_listView.IsKeyboardFocusWithin) //Breadcrumbpopup hatte Focus, Click außerhalb der Gridzeilen
            {
                _workspace.FocusListView();
                e.Handled = true;
            }

            //Multiple Items 
            //Action is not performed in previewhandler because click could be strg/shift + drag operation
            if (_multipleItemsClicked)
            {
                switch (_multipleItemsClickedModifier)
                {
                    case ModifierKeys.Control:
                        _workspace.ActiveLister.SetSelection(item, false);
                        //Keyboard.Focus(_listView.ItemContainerGenerator.ContainerFromItem(item) as IInputElement);
                        break;
                    case ModifierKeys.Shift:
                        //var found = false;
                        //foreach (var i in _workspace.ActiveLister.SelectedItems)
                        //{
                        //    if (found)
                        //        _workspace.ActiveLister.SetSelection(item, false);
                        //    else if (Equals(i, item))
                        //        found = true;
                        //}
                        //Keyboard.Focus(_listView.ItemContainerGenerator.ContainerFromItem(item) as IInputElement);
                        break;
                    default:
                        _workspace.ActiveLister.ClearSelection();
                        _workspace.ActiveLister.SetSelection(item, true);
                        Keyboard.Focus(_listView.ItemContainerGenerator.ContainerFromItem(item) as IInputElement);
                        break;
                }

                _multipleItemsClicked         = false;
                _multipleItemsClickedModifier = ModifierKeys.None;
            }

            if (_clickTimer.Tag != null)
            {
                _mouseupSinceRenameInit = true;
            }

            if (_showRenameOnMouseup)
            {
                _showRenameOnMouseup = false;
                _commandRepository.GetCommandByName(nameof(RenameFileItemCommand)).Execute(_clickTimer.Tag);
                _clickTimer.Tag = null;
            }
        }

        private void List_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is Visual visual)
            {
                var header = Utils.FindParent<GridViewColumnHeader>(visual);
                if (header != null)
                {
                    e.Handled = false;
                    return;
                }
            }

            var dataItem = Utils.GetDataContextFromOriginalSource(e.OriginalSource);
            if (!(dataItem is IItem item))
            {
                if (e.ClickCount == 2) //Click on empty area -> go back
                {
                    _workspace.CommandRepository.GetCommandByName(nameof(HistoryBackCommand)).Execute();
                    e.Handled = true;
                }
                else
                {
                    _dragStart    = null;
                    _dragCanceled = true;
                }

                return;
            }

            if (e.ClickCount < 2)
            {
                _clickTimer.Stop();
                _dragStart    = e.GetPosition(_listView);
                _dragCanceled = false;
                Dragged      = false;
                //TODO: Dont check for Fileitem introduce IRenameable
                if (_workspace.ActiveLister.SelectedItems.Contains(item) && item is IRenameable)
                {
                    var selected = _workspace.ActiveLister.SelectedItems.Count();
                    if (selected > 1)
                    {
                        _multipleItemsClicked         = true;
                        _multipleItemsClickedModifier = Keyboard.Modifiers;
                        e.Handled                    = true;
                    }
                    else if (selected == 1)
                    {
                        _mouseupSinceRenameInit = false;
                        _clickTimer.Tag        = item;
                        _clickTimer.Start();
                    }
                }
            }
            else if (e.ClickCount == 2)
            {
                _clickTimer.Stop();
                if (e.ChangedButton == MouseButton.Left)
                {
                    if (dataItem is ILister lister)
                    {
                        item = lister.CurrentItem;
                    }

                    _workspace.ActiveLister.DoAction(item);
                    e.Handled = true;
                }
            }
        }

        private void _listView_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            if (e.EscapePressed)
            {
                e.Action     = DragAction.Cancel;
                _dragCanceled = true;
            }
        }

        private void DropList_DragEnter(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(_dragFormat))
                e.Effects = DragDropEffects.None;

            e.Handled = true;
        }

        private void DropList_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(_dragFormat) && !_dragCanceled)
            {
                var files = (string[]) e.Data.GetData(DataFormats.FileDrop);
                if (files == null || !files.Any())
                    return;

                string destination  = null;
                var    listViewItem = FindAnchestor<ListViewItem>(e.OriginalSource);

                if (listViewItem?.Content is FileItem item)
                {
                    destination = item.ItemType == ItemType.Container
                        ? item.Path
                        : item.Directory;
                }
                else
                {
                    var listView = FindAnchestor<ListView>(e.OriginalSource);
                    if (listView.DataContext is ILister lister)
                        destination = lister.Path;
                }

                if (destination != null)
                {
                    e.Effects = (e.KeyStates & DragDropKeyStates.ControlKey) != 0
                        ? DragDropEffects.Copy
                        : DragDropEffects.Move;
                    var fileAction = e.Effects == DragDropEffects.Copy ? FileAction.Copy : FileAction.Move;
                    if (_listView.DataContext is FileLister)
                        FilesystemAction.SetClipboard<FileItem>(fileAction, files);
                    else
                        FilesystemAction.SetClipboard<IItem>(fileAction, files);

                    var action = new FilesystemAction(_notificationHost);
                    var items  = Clipboard.GetFileDropList();
                    Task.Factory.StartNew(() => { action.Paste(destination, items, fileAction); });
                    if (fileAction == FileAction.Move)
                        Clipboard.Clear();
                    e.Handled = true;
                }
            }
        }

        private void ClickTimer_Tick(object sender, EventArgs ea)
        {
            if (_mouseupSinceRenameInit)
            {
                _commandRepository.GetCommandByName(nameof(RenameFileItemCommand)).Execute(_clickTimer.Tag);
                _clickTimer.Tag = null;
            }
            else
                _showRenameOnMouseup = true;

            _clickTimer.Stop();
        }

        private void List_MouseMove(object sender, MouseEventArgs e)
        {
            if (_dragStart == null)
                return;

            var mousePos = e.GetPosition(_listView);
            var diff     = _dragStart.Value - mousePos;

            if (e.LeftButton == MouseButtonState.Pressed &&
                (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                _clickTimer.Stop();
                _clickTimer.Tag     = null;
                _showRenameOnMouseup = false;
                Dragged             = true;
                var listViewItem = FindAnchestor<ListViewItem>(e.OriginalSource);

                if (listViewItem != null)
                {
                    var view     = _listView;
                    var dragData = new DataObject(_dragFormat, view.SelectedItems.OfType<FileItem>().Select(i => i.Path).ToArray());
                    DragDrop.DoDragDrop(listViewItem, dragData, DragDropEffects.Move | DragDropEffects.Copy | DragDropEffects.Link);
                }
            }
        }

        private static T FindAnchestor<T>(object obj) where T : DependencyObject
        {
            if (!(obj is DependencyObject current))
                return null;

            do
            {
                if (current is T variable) 
                    return variable;
                current = VisualTreeHelper.GetParent(current);
            } while (current != null);

            return null;
        }
    }
}