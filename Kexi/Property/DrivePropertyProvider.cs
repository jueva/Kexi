using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Kexi.Common;
using Kexi.Composition;
using Kexi.ViewModel;
using Kexi.ViewModel.Item;
using Kexi.ViewModel.Lister;

namespace Kexi.Property
{
    [ExportPropertyProvider(typeof(DriveLister), "Drive Properties")]
    public class DrivePropertyProvider : BasePropertyProvider<DriveItem>
    {
        [ImportingConstructor]
        public DrivePropertyProvider(Workspace workspace) : base(workspace)
        {
        }

        protected override async Task<ObservableCollection<PropertyItem>> GetTopItems()
        {
            if (Item == null)
                return default;

            return await Task.Run(() => new ObservableCollection<PropertyItem>(new[]
            {
                new PropertyItem("Name", Item.DisplayName)
            }));
        }

        protected override async Task<ObservableCollection<PropertyItem>> GetBottomItems()
        {
            if (Item == null)
                return default;

            return await Task.Run(() =>
            {
                var info = new DriveInfo(Item.DriveLetter);

                const int mb = 1000000;
                var props = new ObservableCollection<PropertyItem>(new[]
                {
                    new PropertyItem("Format", info.DriveFormat),
                    new PropertyItem("Type", info.DriveType),
                    new PropertyItem("Total Size", $"{info.TotalSize / mb} MB"),
                    new PropertyItem("Available free space", $"{info.AvailableFreeSpace / mb} MB"),
                    new PropertyItem("Total free space", $"{info.TotalFreeSpace / mb} MB"),
                    new PropertyItem("Ready", Item.IsReady)
                });
                return props;
            });
        }
        protected override async Task<BitmapSource> GetThumbnail()
        {
            return  await Task.Run(() => ThumbnailProvider.GetThumbnailSource(Item.Path, 256, 256, ThumbnailOptions.None));
        }
    }
}