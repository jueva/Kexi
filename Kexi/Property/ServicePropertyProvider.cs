using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Kexi.Composition;
using Kexi.ViewModel;
using Kexi.ViewModel.Item;

namespace Kexi.Property
{
    [ExportPropertyProvider(typeof(ServiceLister), "Service Lister")]
    public class ServicePropertyProvider : BasePropertyProvider<ServiceItem>
    {
        [ImportingConstructor]
        public ServicePropertyProvider(Workspace workspace) : base(workspace)
        {
        }

        protected override  Task<ObservableCollection<PropertyItem>> GetTopItems()
        {
            return Task.Run(() => new ObservableCollection<PropertyItem>(new[]
            {
                new PropertyItem("Name", Item.DisplayName)
            }));
        }

        protected override Task<ObservableCollection<PropertyItem>> GetBottomItems()
        {
            return Task.Run(() =>
                new ObservableCollection<PropertyItem>(new[]
                {
                    new PropertyItem("Type", Item.ServiceType),
                    new PropertyItem("Dependent", Item.Dependent)
                }));
        }

        protected override Task<BitmapSource> GetThumbnail()
        {
            return Task.FromResult(default(BitmapSource));
        }
    }
}