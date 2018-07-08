using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;
using Kexi.Annotations;
using Kexi.Interfaces;

namespace Kexi.ViewModel.Item
{
    public class BaseItem : IItem
    {
        public BaseItem()
        {
            Enabled          = true;
            ThumbnailOpacity = 1;
            TargetType       = () => ItemType.Item;
        }

        public BaseItem(string name) : this()
        {
            DisplayName  = name;
            FilterString = name;
            Path         = name;
        }

        public virtual BitmapSource Thumbnail
        {
            get => _thumbnail;
            set
            {
                if (Equals(value, _thumbnail)) return;
                _thumbnail = value;
                OnPropertyChanged();
            }
        }

        public double ThumbnailOpacity
        {
            get => _thumbnailOpacity;
            set
            {
                if (Math.Abs(_thumbnailOpacity - value) < 0.1)
                    return;
                _thumbnailOpacity = value;
                OnPropertyChanged();
            }
        }

        public bool HasError
        {
            get => _hasError;

            set
            {
                if (value == _hasError)
                    return;
                _hasError = value;
                OnPropertyChanged();
            }
        }


        public bool IsContainer => ItemType == ItemType.Container;

        public string DisplayName
        {
            get => _displayName;
            set
            {
                if (Equals(value, _displayName)) return;
                _displayName = value;
                OnPropertyChanged();
            }
        }

        public string FilterString
        {
            get => _filterString;
            set
            {
                if (Equals(value, _filterString)) return;
                _filterString = value;
                OnPropertyChanged();
            }
        }

        public string Path
        {
            get => _path;
            set
            {
                if (Equals(value, _path)) return;
                _path = value;
                OnPropertyChanged();
            }
        }

        public ItemType ItemType
        {
            get => _itemType;
            set
            {
                if (Equals(value, _itemType)) return;
                _itemType = value;
                OnPropertyChanged();
            }
        }

        public Func<ItemType> TargetType { get; protected set; }

        public bool Enabled
        {
            get => _enabled;
            set
            {
                if (value == _enabled)
                    return;
                _enabled = value;
                OnPropertyChanged();
            }
        }

        public bool Highlighted
        {
            get => _highlighted;
            set
            {
                if (value == _highlighted)
                    return;
                _highlighted = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private string                           _displayName;
        private bool                             _enabled;
        private string                           _filterString;
        private bool                             _hasError;
        private bool                             _highlighted;
        private ItemType                         _itemType;
        private string                           _path;
        private BitmapSource                     _thumbnail;
        private double                           _thumbnailOpacity;

        [NotifyPropertyChangedInvocator]
        internal virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}