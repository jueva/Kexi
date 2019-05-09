using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Kexi.ViewModel.Item;

namespace Kexi.Interfaces
{
    public interface IExtendedPropertyProvider
    {
        string Description { get; }
        Task<IEnumerable<PropertyItem>> GetItems(IItem item, Detaillevel Details = Detaillevel.Normal);
        bool IsMatch(IItem item);

    }

    public interface IThumbnailProvider
    {
        Task<BitmapSource> GetThumbnail();
    }
}