using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Media.Imaging;
using Kexi.Interfaces;
using Kexi.ViewModel.Item;

namespace Kexi.ViewModel.TreeView
{
    public class TreeViewItem : ViewModelBase
    {
        private readonly FileItem _fileItem;

        private ObservableCollection<TreeViewItem> _children;
        private bool _isExpanded;
        private bool _isPopulated;
        private bool _isSelected;

        public TreeViewItem(string path, ItemType itemType, string name = null)
        {
            Children = new ObservableCollection<TreeViewItem>();
            if (string.IsNullOrEmpty(path))
                return;
            _fileItem = new FileItem(path, itemType, name);
            SetFileDetails();
        }

        public TreeViewItem(FileItem fi)
        {
            Children = new ObservableCollection<TreeViewItem>();
            _fileItem = fi;
            _fileItem.PropertyChanged += _fileItem_PropertyChanged;
        }

        public BitmapSource Thumbnail => _fileItem?.Details.Thumbnail;

        public string DisplayName => _fileItem?.DisplayName ?? "Computer";

        public string Path => _fileItem?.Path;

        public ItemType? ItemType => _fileItem?.ItemType;

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

        private async void SetFileDetails()
        {
            _fileItem.PropertyChanged += _fileItem_PropertyChanged;
            await _fileItem.SetDetailsAsync();
        }


        private void _fileItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Thumbnail")
                OnPropertyChanged(nameof(Thumbnail));
            else if (e.PropertyName == "DisplayName")
                OnPropertyChanged(nameof(DisplayName));
            else if (e.PropertyName == "Path")
                OnPropertyChanged(nameof(Path));
            else if (e.PropertyName == "ItemType")
                OnPropertyChanged(nameof(ItemType));
        }
    }
}