using Microsoft.WindowsAPICodePack.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Kexi.ViewModel.Item
{
    public class FileDetail : INotifyPropertyChanged
    {
        private readonly ShellObject _shellObject;
        private readonly FileItem _fileItem;

        public event PropertyChangedEventHandler PropertyChanged;

        public FileDetail(FileItem fileItem)
        {
            _shellObject = _fileItem.ShellObject;
            _fileItem = fileItem;
            doit();            
        }

        private async void doit()
        {
            Task.Run(() => init());
        }

        public FileAttributes Attributes { get; private set; }
        public DateTime? Created { get; private set; }
        public string FileVersion { get; private set; }
        public bool IsSystemOrHidden { get; private set; }
        public string ItemType { get; private set; }
        public DateTime? LastAccessTime { get; private set; }
        public DateTime? LastModified { get; private set; }
        public long Length { get; private set; }

        private BitmapSource _thumbnail;
        public BitmapSource Thumbnail
        {
            get
            {
                return _thumbnail;
            }
            set
            {
                _thumbnail = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Thumbnail"));
            }
        }

        private void init()
        {
            if (_shellObject == null)
                return;

            Length = (long)(_shellObject.Properties.System.Size.Value ?? 0);
            LastModified = _shellObject.Properties.System.DateModified.Value;
            LastAccessTime = _shellObject.Properties.System.DateAccessed.Value;
            Created = _shellObject.Properties.System.DateCreated.Value;
            ItemType = _shellObject.Properties.System.ItemTypeText.Value as string;
            FileVersion = _shellObject.Properties.System.FileVersion.Value;

            _shellObject.Thumbnail.FormatOption = ShellThumbnailFormatOption.IconOnly;
            _shellObject.Thumbnail.RetrievalOption = ShellThumbnailRetrievalOption.Default;
            Thumbnail = _shellObject.Thumbnail.SmallBitmapSource;
            Thumbnail.Freeze();

            var attV = _shellObject.Properties.System.FileAttributes.Value;
            int att = (int)(attV.HasValue ? attV.Value : 0);
            Attributes = (FileAttributes)att;
            IsSystemOrHidden = ((att & (int)FileAttributes.Hidden) == (int)FileAttributes.Hidden)
                               || ((att & (int)FileAttributes.System) == (int)FileAttributes.System);
            _fileItem.OnPropertyChanged("Details");
        }
    }
}
