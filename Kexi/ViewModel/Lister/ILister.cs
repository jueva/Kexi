using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Kexi.Common;
using Kexi.Common.MultiSelection;
using Kexi.Interfaces;
using Kexi.ViewModel.Item;

namespace Kexi.ViewModel.Lister
{
    public interface ILister : INotifyPropertyChanged, IDisposable
    {
        string              Path         { get; set; }
        string              Title        { get; set; }
        string              Filter       { get; set; }
        string              PathName     { get; set; }
        NotificationItem    Notification { get; set; }
        IListerView          View         { get; set; }
        IEnumerable<Column> Columns      { get; }
        string              StatusString { get; set; }
        BitmapSource        Thumbnail    { get; set; }

        IEnumerable<CommandBoundItem> ContextMenuItems  { get; }
        ViewType                      CurrentViewMode   { get; set; }
        string                        ProtocolPrefix    { get; }
        LoadingStatus                 LoadingStatus     { get; set; }
        bool                          ShowInMenu        { get; }
        bool                          Loaded            { get; set; }
        bool                          SupportsMultiview { get; }
        IPropertyProvider             PropertyProvider  { get; }

        Options Options         { get; }

        string GroupBy { get; set; }

        Workspace Workspace { get; }

        IEnumerable<IItem> Items     { get; }
        ICollectionView    ItemsView { get; }
        IItem  CurrentItem     { get; }
        IEnumerable<IItem> SelectedItems { get; }
        string HighlightString { get; set; }
        Task Refresh(bool clearFilterAndGroup = true);
        string GetParentContainer();
        void DoAction(IItem item);
        event Action<string> PathChanged;
        void ShowContextMenu(IEnumerable<IItem> selectedItems);
        event Action<ILister> GotView;
        string GetStatusString();

        event EventHandler GotItems;

        void SetSelection(IItem item, bool selected);
        void ClearSelection();
    }

    public interface ILister<T> : ILister
        where T : class, IItem
    {
        new ObservableCollection<T>      Items     { get; }
        new MultiSelectCollectionView<T> ItemsView { get; }
        new IEnumerable<T> SelectedItems { get; }
        void DoAction(T item);
    }
}