using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Kexi.Interfaces;
using Kexi.ViewModel;
using Kexi.ViewModel.Item;

namespace Kexi.Property
{
    public class DefaultPropertyProvider : BasePropertyProvider<IItem>
    {
        public DefaultPropertyProvider(Workspace workspace) : base(workspace)
        {
        }

        protected override Task<ObservableCollection<PropertyItem>> GetTopItems()
        {
            var props = new ObservableCollection<PropertyItem>
            {
                new PropertyItem("Name", Item.DisplayName)
            };
            return Task.FromResult(props);
        }

        protected override Task<ObservableCollection<PropertyItem>> GetBottomItems()
        {
            return Task.FromResult(new ObservableCollection<PropertyItem>());
        }

        protected override Task<BitmapSource> GetThumbnail()
        {
            return Task.FromResult(default(BitmapSource));
        }
    }
}