using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Media.Imaging;
using Kexi.Interfaces;
using Kexi.ViewModel.Item;

namespace Kexi.ViewModel.TreeView
{
    public class TreeViewItem : ViewModelBase
    {

        private ObservableCollection<TreeViewItem> _children;
        private bool _isExpanded;
        private bool _isPopulated;
        private bool _isSelected;
        private ItemType? _itemType;
        private string _path;
        private string _displayName;
        private BitmapSource _thumbNail;

        public TreeViewItem(string path, ItemType itemType, string name)
        {
            Children = new ObservableCollection<TreeViewItem>();
            DisplayName = name;
            Path = path;
            if (string.IsNullOrEmpty(path))
                return;
        }

        public BitmapSource Thumbnail
        {
            get => _thumbNail;
            set
            {
                _thumbNail = value;
                OnPropertyChanged();
            }
        }

        public string DisplayName
        {
            get => _displayName;
            set
            {
                _displayName = value;
                OnPropertyChanged();
            }
        }

        public string Path
        {
            get => _path;
            set
            {
                _path = value;
                OnPropertyChanged();
            }
        }

        public ItemType? ItemType
        {
            get => _itemType;
            set
            {
                _itemType = value;
                OnPropertyChanged();
            }
        }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected == value)
                    return;
                _isSelected = value;
                OnPropertyChanged();
            }
        }

        public bool IsPopulated
        {
            get => _isPopulated;
            set
            {
                if (value.Equals(_isPopulated)) return;
                _isPopulated = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<TreeViewItem> Children
        {
            get => _children;
            set
            {
                _children = value;
                OnPropertyChanged();
            }
        }

        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                if (value == _isExpanded)
                    return;

                _isExpanded = value;
                OnPropertyChanged();
            }
        }

    }
}