using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Kexi.ViewModel.Item;

namespace Kexi.Interfaces
{
    public interface IPropertyProvider<T> : IPropertyProvider
        where T : IItem
    {
        new T Item { get; }
        Task SetItem(T item);
    }

    public interface IPropertyProvider : IPropertyItemContainer
    {
        BitmapSource                       Thumbnail               { get; }
        double                             ThumbMaxHeight          { get; }
        int                                RotateThumb             { get; }
        CancellationTokenSource            CancellationTokenSource { get; set; }
        Task SetItem(IItem item);
    }

    public interface IPropertyItemContainer
    {
        ObservableCollection<PropertyItem> PropertiesTop    { get; }
        ObservableCollection<PropertyItem> PropertiesBottom { get; }
        IItem                              Item             { get; }
    }
}