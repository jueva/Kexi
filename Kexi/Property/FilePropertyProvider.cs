using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Kexi.Common;
using Kexi.Composition;
using Kexi.Files;
using Kexi.ViewModel;
using Kexi.ViewModel.Item;
using Kexi.ViewModel.Lister;
using Microsoft.WindowsAPICodePack.Shell;

namespace Kexi.Property
{
    [ExportPropertyProvider(typeof(FileLister), "File Properties")]
    public class FilePropertyProvider : BasePropertyProvider<FileItem>
    {
        [ImportingConstructor]
        public FilePropertyProvider(Workspace workspace) : base(workspace)
        {
            Width = 100;
        }

        private bool IsMusic    => "music".Equals(GetKind());
        private bool IsPicture  => "picture".Equals(GetKind());
        private bool IsExeOrDll => Item?.Extension == ".exe" || Item?.Extension == ".dll";

        private ShellObject _shellObject;

        public override void Dispose()
        {
            _shellObject?.Dispose();
            _shellObject = null;
            base.Dispose();
        }

        private string GetKind()
        {
            if (Item == null || _shellObject == null)
                return null;

            try
            {
                return !(_shellObject?.Properties?.System?.Kind?.ValueAsObject is string[] kind) || kind.Length == 0
                    ? null
                    : kind[0];
            }
            catch
            {
                return null;
            }
        }

        public override Task SetItem(FileItem item)
        {
            _shellObject = ShellObject.FromParsingName(item.Path);
            return base.SetItem(item);
        }

        protected override async Task<ObservableCollection<PropertyItem>> GetTopItems()
        {
            if (Item.IsFileShare)
            {
                var nameOnly = new ObservableCollection<PropertyItem> {new PropertyItem("Name", Item.DisplayName)};
                return await Task.FromResult(nameOnly);
            }

            if (Item.IsNetwork())
                return await GetNetworkTopItems();

            var tempProp = new ObservableCollection<PropertyItem>();
            if ((CancellationTokenSource?.IsCancellationRequested ?? true) || Item == null)
                return tempProp;

            return await Task.Factory.StartNew(() =>
            {
                tempProp.Clear();
                tempProp.Add(new PropertyItem("Name", Item.DisplayName));
                tempProp.Add(new PropertyItem("Type", Item.Details.Type));
                return tempProp;
            }, CancellationTokenSource.Token);
        }

        protected override async Task<BitmapSource> GetThumbnail()
        {
            ThumbMaxHeight = 80;
            if (Item.IsFileShare)
            {
                return Utils.GetImageFromRessource("share.png");
            }

            if (Item.IsNetwork())
            {
                return Item.Details.Thumbnail;
            }

            if (IsPicture)
                ThumbMaxHeight = 120;
            Item.Details.LargeThumbnail = await Item.Details.GetLargeThumbAsync();
            return Item.Details.LargeThumbnail;
        }

        protected override async Task<ObservableCollection<PropertyItem>> GetBottomItems()
        {
            if (CancellationTokenSource?.IsCancellationRequested ?? Item == null)
                return new ObservableCollection<PropertyItem>();

            if (Item.IsFileShare)
                return new ObservableCollection<PropertyItem>();

            if (Item.IsNetwork())
                return await GetNetworkBottomItems();

            return await Task.Run(() =>
            {
                var tempProp = new ObservableCollection<PropertyItem>
                {
                    new PropertyItem("Attributes", Item.AttributeString),
                    new PropertyItem("Created", Item.Details.Created),
                    new PropertyItem("Last Modified", Item.Details.LastModified)
                };

                var props = new Files.FilePropertyProvider(_shellObject);

                if (Item.IsLink())
                {
                    var target = new FileItemTargetResolver(Item);
                    tempProp.Add(new PropertyItem("Target:", target.TargetPath));
                }

                if (IsMusic)
                {
                    tempProp.Add(new PropertyItem("Title", _shellObject.Properties.System.Title.ValueAsObject));
                    if (_shellObject.Properties?.System?.Author?.ValueAsObject is string[] authors)
                        tempProp.Add(new PropertyItem("Author(s)", string.Join(",", authors)));
                    tempProp.Add(new PropertyItem("Rating", _shellObject.Properties?.System?.Rating?.ValueAsObject));
                }

                if (IsPicture)
                {
                    tempProp.Add(new PropertyItem("Dimensions", props.GetValue("System.Image.Dimensions")));
                    foreach (var key in props.Items.Keys.Where(k => k.StartsWith("System.Photo")).OrderBy(k => k))
                        tempProp.Add(new PropertyItem(key.Substring(13), props.GetValue(key)));
                }

                return tempProp;
            });
        }

        private async Task<ObservableCollection<PropertyItem>> GetNetworkTopItems()
        {
            var tempProp = new ObservableCollection<PropertyItem>();
            if (CancellationTokenSource.IsCancellationRequested)
                return tempProp;

            return await Task.Run(() =>
            {
                tempProp.Clear();
                tempProp.Add(new PropertyItem("Name", Item.Name));
                tempProp.Add(new PropertyItem("Type", Item.Details.Type));
                return tempProp;
            }, CancellationTokenSource.Token);
        }

        private async Task<ObservableCollection<PropertyItem>> GetNetworkBottomItems()
        {
            if (CancellationTokenSource.IsCancellationRequested || Item == null)
                return new ObservableCollection<PropertyItem>();

            return await Task.Run(() =>
            {
                var tempProp = new ObservableCollection<PropertyItem>
                {
                    new PropertyItem("Location", "Network"),
                    new PropertyItem("Attributes", Item.AttributeString),
                    new PropertyItem("Created", Item.Details.Created),
                    new PropertyItem("Last Modified", Item.Details.LastModified)
                };

                return tempProp;
            }, CancellationTokenSource.Token);
        }
    }
}