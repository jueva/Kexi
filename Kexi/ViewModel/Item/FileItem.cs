using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Kexi.Common;
using Kexi.Files;
using Kexi.Interfaces;
using Kexi.ItemProvider;
using Kexi.Shell;
using Kexi.ViewModel.Lister;

namespace Kexi.ViewModel.Item
{
    public class FileItem : BaseItem, IRenameable, IHasBackgroundData<FileItem>
    {
        private readonly bool _fetchDisplayName;
        private readonly FileItemProvider _itemProvider;
        private FileAttributes? _attributes;
        private string _attributeString;
        private FileDetailItem _details;
        private string _directory;
        private string _extension;
        private FileInfo _fileInfo;
        private FileSystemInfo _fileSystemInfo;
        private bool _isMarkedForMove;
        private bool _isSystemOrHidden;
        private long _length;
        private string _name;

        public FileItem(string path, ItemType itemType, string name = null, FileItemProvider itemProvider = null)
        {
            _itemProvider = itemProvider;
            CancellationToken = _itemProvider?.CancellationTokenSource.Token;
            if (name != null)
                _fetchDisplayName = name != ".." && !name.EndsWith(".lnk");
            Path = path;
            DisplayName = name ?? System.IO.Path.GetFileName(path);
            FilterString = DisplayName;
            ItemType = itemType;
        }

        public FileItem(string path) : this(path, System.IO.Directory.Exists(path) ? ItemType.Container : ItemType.Item)
        {
        }

        public CancellationToken? CancellationToken { get; }

        public FileDetailItem Details
        {
            get
            {
                if (_details == null)
                {
                    if (_itemProvider?.Workspace.ActiveLister is FileLister lister)
                        lister.AddPriorityItem(this);
                }
                
                return _details;
            }
            internal set
            {
                _details = value;
                OnPropertyChanged();
            }
        }

        public FileInfo FileInfo
        {
            get
            {
                if (_fileInfo == null)
                    try
                    {
                        _fileInfo = _fileInfo = string.IsNullOrEmpty(Path) ? null : new FileInfo(Path);
                    }
                    catch
                    {
                        //handle
                    }

                return _fileInfo;
            }
        }

        public FileSystemInfo FileSystemInfo
            =>
                _fileSystemInfo ??
                (_fileSystemInfo = string.IsNullOrEmpty(Path)
                    ? null
                    : ItemType == ItemType.Item
                        ? (FileSystemInfo) new FileInfo(Path)
                        : new DirectoryInfo(Path));

        public FileAttributes Attributes
        {
            get
            {
                if (_attributes == null)
                    _attributes = FileInfo?.Attributes ?? FileAttributes.Normal;
                return _attributes.Value;
            }
            set
            {
                if (value == _attributes)
                    return;

                _attributes = value;
                OnPropertyChanged();
            }
        }

        public bool IsSystemOrHidden
        {
            get => _isSystemOrHidden;
            set
            {
                _isSystemOrHidden = value;
                ThumbnailOpacity = value ? 0.6 : 1;
                OnPropertyChanged();
            }
        }

        public bool IsMarkedForMove
        {
            get => _isMarkedForMove;
            set
            {
                _isMarkedForMove = value;
                ThumbnailOpacity = value ? 0.6 : 1;
                OnPropertyChanged();
            }
        }

        public bool IsFileShare { get; set; }
        public string AttributeString => _attributeString ?? (_attributeString = GetAttributeString());

        public string Name => _name ?? (_name = System.IO.Path.GetFileName(Path));

        public string Directory => _directory ?? (_directory = System.IO.Path.GetDirectoryName(Path) ?? Path);

        public string Extension => _extension ?? (_extension = System.IO.Path.GetExtension(Path));

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

        public void Rename(string newName)
        {
            FileHelper.Rename(this, newName);
        }

        public (int, int) GetRenameSelectonBorder()
        {
            return FileHelper.GetRenameSelectionBorder(this);
        }

        public async Task<FileDetailItem> SetDetailsAsync()
        {
            if (CancellationToken?.IsCancellationRequested ?? false)
                return null;

            var largeThumb = _itemProvider?.Workspace.ActiveLister.CurrentViewMode == ViewType.Thumbnail;
            var token = CancellationToken ?? System.Threading.CancellationToken.None;
            _details = await Task.Factory.StartNew(() => GetDetail(largeThumb, token), token);
            Details = _details;
            Thumbnail = _details.Thumbnail;

            if (_fetchDisplayName && !string.IsNullOrEmpty(_details.DisplayName))
                DisplayName = _details.DisplayName;
            return _details;
        }

        public FileDetailItem GetDetail(bool largeThumb, CancellationToken token)
        {
            var det = new FileDetailItem(this, CancellationToken);
            SetTargetType();
            det.Init(token);
            det.SetThumbs(token, largeThumb);
            IsSystemOrHidden = Attributes.HasFlag(FileAttributes.Hidden | FileAttributes.System);
            return det;
        }

        private void SetTargetType()
        {
            TargetType = () => IsLink() ? new FileItemTargetResolver(this).TargetType : ItemType;
        }

        public bool IsNetwork()
        {
            if (Path.StartsWith(@"\\"))
                return true;
            var dir = new DirectoryInfo(Path);
            var drive = new DriveInfo(dir.Root.ToString());
            return drive.DriveType == DriveType.Network;
        }

        public bool IsLink()
        {
            return ShellWrapper.Instance.IsLink(Path);
        }

        public async void Refresh()
        {
            Details = await SetDetailsAsync();
        }

        public string GetPathResolved()
        {
            return IsLink() ? new FileItemTargetResolver(this).TargetPath : Path;
        }

        private string GetAttributeString()
        {
            try
            {
                var attributes = File.GetAttributes(Path);
                return string.Join(", ",
                    Enum.GetValues(typeof(FileAttributes))
                        .Cast<FileAttributes>()
                        .Where(f => (attributes & f) == f));
            }
            catch
            {
                return "N/A";
            }
        }

        public void LoadBackgroundData(FileItem item)
        {
            var largeThumbs = (_itemProvider?.Workspace.ActiveLister.CurrentViewMode == ViewType.Thumbnail);
            Details = GetDetail(largeThumbs, _itemProvider?.CancellationTokenSource.Token ?? System.Threading.CancellationToken.None);
            Thumbnail = Details.Thumbnail;
            BackgroundDataLoaded = true;
        }

        public bool BackgroundDataLoaded { get; private set; }
    }
}