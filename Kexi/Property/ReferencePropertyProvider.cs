using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Kexi.Common;
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

        protected override Task<ObservableCollection<PropertyItem>> GetTopItems()
        {
            return Task.Run(() => new ObservableCollection<PropertyItem>(new[]
            {
                new PropertyItem("Name", Item?.DisplayName)
            }));
        }

        protected override Task<ObservableCollection<PropertyItem>> GetBottomItems()
        {
            if (Item == null)
                return Task.FromResult(new ObservableCollection<PropertyItem>());

            return  Task.Run(() =>
                new ObservableCollection<PropertyItem>(new[]
                {
                    new PropertyItem("Assembly", Item.Assembly),
                    new PropertyItem("Version", Item.Version),
                    new PropertyItem("Culture", Item.Culture),
                    new PropertyItem("PublicKeyToken", Item.PublicKeyToken),
                    new PropertyItem("Attributes", Item.Attributes.ToString())
                }));
        }

        protected override Task<BitmapSource> GetThumbnail()
        {
            return ThumbnailProvider.GetLargeThumbnailAsync(Item?.AssemblyPath);
        }
    }
}