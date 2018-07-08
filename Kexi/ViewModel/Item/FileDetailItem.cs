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
            get => created;
            set
            {
                if (value.Equals(created)) return;
                created = value;
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
            get => type;
            set
            {
                if (value == type) return;
                type = value;
                OnPropertyChanged();
            }
        }

        public long Length
        {
            get => length;
            set
            {
                if (value == length) return;
                length = value;
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
        private          DateTime?               created;


        private long   length;
        private string type;

        public async Task SetLargeThumbAsync()
        {
            var large = await ThreadHelper.StartTaskWithSingleThreadAffinity(GetLargeThumb);
            LargeThumbnail = large;
        }



        public void Init(CancellationToken token)
        {
            if (token.IsCancellationRequested)
                return;

            if (_fileItem.ItemType == ItemType.Item && _fileItem.Path != null && File.Exists(_fileItem.Path)) Length = _fileItem.FileInfo.Length;

            if (_fileItem.FileSystemInfo != null)
            {
                LastModified   = _fileItem.FileSystemInfo.LastWriteTime;
                LastAccessTime = _fileItem.FileSystemInfo.LastAccessTime;
                Created        = _fileItem.FileSystemInfo.CreationTime;
            }
        }

        public void SetThumbs(CancellationToken token, bool largeThumbs = false)
        {
            if (token.IsCancellationRequested)
                return;

            try
            {
                var fullInfo = new NativeFileInfo(_fileItem.Path);
                Type      = fullInfo.TypeName;
                Thumbnail = fullInfo.Icon;
                Thumbnail.Freeze();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }


        public BitmapSource GetLargeThumb()
        {
            return ThumbnailProvider.GetThumbnailSource(_fileItem.Path, 256, 256, ThumbnailOptions.None, _cancellationToken);
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}