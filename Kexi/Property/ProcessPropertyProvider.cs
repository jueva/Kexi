using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Kexi.Common;
using Kexi.Composition;
using Kexi.ViewModel;
using Kexi.ViewModel.Item;
using Kexi.ViewModel.Lister;

namespace Kexi.Property
{
    [ExportPropertyProvider(typeof(ProcessLister), "Process Lister")]
    public class ProcessPropertyProvider : BasePropertyProvider<ProcessItem>
    {
        [ImportingConstructor]
        public ProcessPropertyProvider(Workspace workspace) : base(workspace)
        {
        }

        protected override Task<ObservableCollection<PropertyItem>> GetTopItems()
        {
            var props = new ObservableCollection<PropertyItem>(new[]
            {
                new PropertyItem("Name", Item?.DisplayName)
            });
            return Task.FromResult(props);
        }

        protected override Task<ObservableCollection<PropertyItem>> GetBottomItems()
        {
            if (Item.Details == null)
                return Task.FromResult(new ObservableCollection<PropertyItem>()); 

            var props = new ObservableCollection<PropertyItem>(new[]
            {
                new PropertyItem("Memory", Item.Details.Memory),
                new PropertyItem("Cpu", Item.Details.Cpu),
                new PropertyItem("Pid", Item.Details.Pid),
                new PropertyItem("Description", Item.Details.Description)
            });
            return Task.FromResult(props);
        }

        protected override  Task<BitmapSource> GetThumbnail()
        {
            return ThumbnailProvider.GetLargeThumbnailAsync(Item.FileName);
        }
    }
}