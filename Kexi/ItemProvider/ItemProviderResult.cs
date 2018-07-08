using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace Kexi.ItemProvider
{
    public class ItemProviderResult<T>
        where T : class
    {
        public string PathName { get; set; }
        public IEnumerable<T> Items { get; set; }
        public BitmapSource Thumbnail { get; set; }
    }
}