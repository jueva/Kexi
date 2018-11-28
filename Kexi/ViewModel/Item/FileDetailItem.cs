using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Kexi.Annotations;
using Kexi.Common;
using Kexi.Interfaces;
using Kexi.Shell;

namespace Kexi.ViewModel.Item
{
    public class FileDetailItem : INotifyPropertyChanged
    {
        public FileDetailItem(FileItem fileItem, CancellationToken? cancellationToken)
        {
            _fileItem          = fileItem;
            _cancellationToken = cancellationToken;
        }

        public DateTime? Created
        {
            get => _created;
            set
            {
                if (value.Equals(_created)) return;
                _created = value;
                OnPropertyChanged();
            }
        }

        public DateTime? LastModified
        {
            get => _lastModified;
            set
            {
                if (value.Equals(_lastModified)) return;
                _lastModified = value;

                OnPropertyChanged();
            }
        }

        public DateTime? LastAccessTime
        {
            get => _lastAccessed;
            set
            {
                if (value.Equals(_lastAccessed)) return;
                _lastAccessed = value;
                OnPropertyChanged();
            }
        }

        public BitmapSource LargeThumbnail
        {
            get
            {
                if (_largeThumbnail == null)
                    SetLargeThumbAsync();

                return _largeThumbnail;
            }
            set
            {
                _largeThumbnail = value;
                OnPropertyChanged();
            }
        }

        public string Type
        {
            get => _type;
            set
            {
                if (value == _type) return;
                _type = value;
                OnPropertyChanged();
            }
        }

        public long Length
        {
            get => _length;
            set
            {
                if (value == _length) return;
                _length = value;
                OnPropertyChanged();
            }
        }

        public string FileVersion
        {
            get => _fileVersion;
            set
            {
                if (_fileVersion == value)
                    return;

                _fileVersion = value;
                OnPropertyChanged();
            }
        }

        public BitmapSource Thumbnail
        {
            get => _thumbnail;
            set
            {
                _thumbnail = value;
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

        public event PropertyChangedEventHandler PropertyChanged;
        private readonly CancellationToken?      _cancellationToken;
        private readonly FileItem                _fileItem;
        private          string                  _displayName;
        private          string                  _fileVersion;
        private          BitmapSource            _largeThumbnail;
        private          DateTime?               _lastAccessed;
        private          DateTime?               _lastModified;
        private          BitmapSource            _thumbnail;
        private          DateTime?               _created;
        private long   _length;
        private string _type;


        public void Init(CancellationToken token)
        {
            if (token.IsCancellationRequested)
                return;
            if (_fileItem.ItemType == ItemType.Item && _fileItem.Path != null && File.Exists(_fileItem.Path))
            {
                Length = _fileItem.FileInfo.Length;
            }

            if (_fileItem.FileInfo != null)
            {
                LastModified   = _fileItem.FileSystemInfo.LastWriteTime;
                LastAccessTime = _fileItem.FileSystemInfo.LastAccessTime;
                Created        = _fileItem.FileSystemInfo.CreationTime;
            }
        }

        public void SetThumbs(CancellationToken token, bool largeThumbs = false)
        {
            try
            {
                var fullInfo = new NativeFileInfo(_fileItem.Path);
                Type      = fullInfo.TypeName;
                DisplayName = fullInfo.DisplayName;
                Thumbnail = fullInfo.Icon;
                Thumbnail.Freeze();
            }
            catch
            {
                // ignored
            }
        }

        private async void SetLargeThumbAsync()
        {
            LargeThumbnail = await GetLargeThumbAsync();
        }

        public  Task<BitmapSource> GetLargeThumbAsync()
        {
            return Task.Factory.StartNew(() => ThumbnailProvider.GetThumbnailSource(_fileItem.Path, 256, 256, ThumbnailOptions.None, _cancellationToken));
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}