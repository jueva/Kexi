using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Kexi.Composition;
using Kexi.Interfaces;
using Kexi.ViewModel.Item;
using VersOne.Epub;

namespace Kexi.Extensions
{
    [Export(typeof(IExtendedPropertyProvider))]
    [ExportPropertyProvider(typeof(FileItem), "Epub Extensions")]
    public class EpubExtensions : IExtendedPropertyProvider
    {
        public string Description => "Additional informations for epub files";

        public  Task<IEnumerable<PropertyItem>> GetItems(IItem item)
        {
            return Task.Run(async () =>
            {
                var book = await EpubReader.OpenBookAsync(item.Path).ConfigureAwait(false);
                var cover = await book.ReadCoverAsync().ConfigureAwait(false);

                var properties = new List<PropertyItem>()
                {
                    new PropertyItem("Title", book.Title),
                    new PropertyItem("Authors", string.Join(Environment.NewLine, book.AuthorList)),
                };
                if (cover != null)
                    properties.Add(new PropertyItem("Thumbnail", ImageFromBuffer(cover)));
                
                return properties.AsEnumerable();
            });
        }

        public bool IsMatch(IItem item)
        {
            return item is FileItem fileItem && (fileItem.Extension == ".epub");
        }


        public BitmapImage ImageFromBuffer(byte[] bytes)
        {
            if (bytes == null)
                return null;
            var stream = new MemoryStream(bytes);
            var  image  = new BitmapImage();
            image.BeginInit();
            image.StreamSource = stream;
            image.EndInit();
            image.Freeze();
            return image;
        }
    }
}