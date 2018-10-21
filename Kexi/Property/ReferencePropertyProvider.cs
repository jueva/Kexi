using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Kexi.Composition;
using Kexi.ViewModel;
using Kexi.ViewModel.Item;
using Kexi.ViewModel.Lister;

namespace Kexi.Property
{
    [ExportPropertyProvider(typeof(ReferenceLister), "Reference Lister")]
    public class ReferencePropertyProvider : BasePropertyProvider<ReferenceItem>
    {
        [ImportingConstructor]
        public ReferencePropertyProvider(Workspace workspace) : base(workspace)
        {
        }

        protected override async Task<ObservableCollection<PropertyItem>> GetTopItems()
        {
            return await Task.Run(() => new ObservableCollection<PropertyItem>(new[]
            {
                new PropertyItem("Name", Item?.DisplayName)
            }));
        }

        protected override async Task<ObservableCollection<PropertyItem>> GetBottomItems()
        {
            if (Item == null)
                return new ObservableCollection<PropertyItem>();

            return await Task.Run(() =>
                new ObservableCollection<PropertyItem>(new[]
                {
                    new PropertyItem("Assembly", Item.Assembly),
                    new PropertyItem("Version", Item.Version),
                    new PropertyItem("Culture", Item.Culture),
                    new PropertyItem("PublicKeyToken", Item.PublicKeyToken),
                    new PropertyItem("Attributes", Item.Attributes.ToString())
                }));
        }

        protected override async Task<BitmapSource> GetThumbnail()
        {
            var fi = new FileItem(Item?.AssemblyPath);
            fi.Details.LargeThumbnail = await fi.Details.GetLargeThumbAsync();
            return fi.Details.LargeThumbnail;
        }
    }
}