using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using Kexi.Common;
using Kexi.Interfaces;
using Kexi.Shell;
using Kexi.ViewModel.Item;

namespace Kexi.ViewModel
{
    public class RecentLocationPopupViewModel : ViewModelBase
    {
        public RecentLocationPopupViewModel(Workspace workspace)
        {
            Workspace = workspace;
        }

        public bool PopupVisible
        {
            get => _popupVisible;
            set
            {
                if (value.Equals(_popupVisible)) return;
                _popupVisible = value;
                if (value)
                    Refresh();
                OnPropertyChanged();
            }
        }

        public Control PopupTarget
        {
            get { return _popupTarget; }
            set
            {
                if (Equals(value, _popupTarget)) return;
                _popupTarget = value;
                OnPropertyChanged();
            }
        }

        public IEnumerable<IItem> Items
        {
            get
            {
                return _items;
            }
            set
            {
                if (Equals(value, _items)) return;
                _items = value;
                ItemsView = CollectionViewSource.GetDefaultView(_items);
                ItemsView.MoveCurrentToFirst();
                OnPropertyChanged();
            }
        }


        public ICollectionView ItemsView
        {
            get {
                return _collectionView;
            }
            set
            {
                if (Equals(value, _collectionView))
                    return;
                _collectionView = value;
                OnPropertyChanged();
            }
        }

        public int Width
        {
            get { return _width; }
            set
            {
                if (value == _width) return;
                _width = value;
                OnPropertyChanged();
            }
        }

        public int HorizontalOffset
        {
            get { return _horizontalOffset; }
            set
            {
                if (value == _horizontalOffset) return;
                _horizontalOffset = value;
                OnPropertyChanged();
            }
        }

        public int VerticalOffset
        {
            get { return _verticalOffset; }
            set
            {
                if (value == _verticalOffset) return;
                _verticalOffset = value;
                OnPropertyChanged();
            }
        }

        private void Refresh()
        {
            if (Workspace.ActiveLister is IHistorisationProvider history)
            {
                Items =
                    new ObservableCollection<IItem>(
                        history.History.Locations.Values.Reverse().Select(
                            i => new BaseItem
                            {
                                Path = i.FullPath,
                                DisplayName = Path.GetFileName(i.FullPath),
                                Thumbnail = ShellNative.GetLargeBitmapSource(i.FullPath)
                            }
                            ).DistinctBy(x => x.Path));
            }
        }

        public Workspace Workspace { get; }
        private IEnumerable<IItem> _items;
        private Control _popupTarget;
        private bool _popupVisible;
        private int _verticalOffset;
        private int _horizontalOffset;
        private int _width;
        private ICollectionView _collectionView;

    }
}