using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Kexi.Common;
using Kexi.Composition;
using Kexi.Files;
using Kexi.Interfaces;
using Kexi.ViewModel;
using Kexi.ViewModel.Item;
using Kexi.ViewModel.Lister;
using Microsoft.WindowsAPICodePack.Shell;
using Ude;

namespace Kexi.Property
{
    [ExportPropertyProvider(typeof(FileLister), "File Properties")]
    public class FilePropertyProvider : BasePropertyProvider<FileItem>
    {
        [ImportingConstructor]
        public FilePropertyProvider(Workspace workspace) : base(workspace)
        {
            Width = 100;
        }

        private bool IsMusic    => "music".Equals(GetKind());
        private bool IsPicture  => "picture".Equals(GetKind());

        private ShellObject _shellObject;

        public override void Dispose()
        {
            _shellObject?.Dispose();
            _shellObject = null;
            base.Dispose();
        }

        private string GetKind()
        {
            if (Item == null || _shellObject == null)
                return null;

            try
            {
                return !(_shellObject?.Properties?.System?.Kind?.ValueAsObject is string[] kind) || kind.Length == 0
                    ? null
                    : kind[0];
            }
            catch
            {
                return null;
            }
        }

        public override async Task SetItem(FileItem item, Detaillevel detail)
        {
            _shellObject = await Task.Factory.StartNew(() => ShellObject.FromParsingName(item.Path)).ConfigureAwait(false);
            await base.SetItem(item, detail).ConfigureAwait(false);
        }

        protected override Task<ObservableCollection<PropertyItem>> GetTopItems()
        {
            if (Item.IsFileShare)
            {
                return Task.FromResult(new ObservableCollection<PropertyItem> {new PropertyItem("Name", Item.Name)});
            }

            if (Item.IsNetwork())
                return  GetNetworkTopItems();

            var tempProp = new ObservableCollection<PropertyItem>();
            if ((CancellationTokenSource?.IsCancellationRequested ?? true) || Item == null)
                return Task.FromResult(tempProp);

            return Task.Factory.StartNew(() =>
            {
                var detail = Item.GetDetail(false, CancellationToken.None);
                tempProp.Clear();
                tempProp.Add(new PropertyItem("Name", Item.DisplayName));
                tempProp.Add(new PropertyItem("Type", detail?.Type));
                return tempProp;
            }, CancellationTokenSource.Token);
        }

        protected override async Task<BitmapSource> GetThumbnail()
        {
            ThumbMaxHeight = 80;
            if (Item.IsFileShare)
            {
                var thumb = Utils.GetImageFromRessource("share.png");
                thumb.Freeze();
                return thumb;
            }

            if (Item.IsNetwork())
            {
                return Item.Details.Thumbnail;
            }

            if (IsPicture)
                ThumbMaxHeight = 120;

            return await ThumbnailProvider.GetLargeThumbnailAsync(Item.Path).ConfigureAwait(false);
        }

        protected override Task<ObservableCollection<PropertyItem>> GetBottomItems()
        {
            if (Item.IsFileShare || (CancellationTokenSource?.IsCancellationRequested ?? Item == null))
                return Task.FromResult(new ObservableCollection<PropertyItem>());

            if (Item.IsNetwork())
                return  GetNetworkBottomItems();

            return Task.Run(async () =>
            {
                var detail = Item.GetDetail(false, CancellationToken.None);
                var tempProp = new ObservableCollection<PropertyItem>
                {
                    new PropertyItem("Attributes", Item.AttributeString),
                    new PropertyItem("Created", detail.Created),
                    new PropertyItem("Last Modified", detail.LastModified)
                };

                var props = new Files.FilePropertyProvider(_shellObject);

                if (Item.IsLink())
                {
                    var target = new FileItemTargetResolver(Item);
                    tempProp.Add(new PropertyItem("Target:", target.TargetPath));
                }

                if (!Item.IsContainer)
                {
                    var encoding = await GetEncoding(Item.Path);
                    if (encoding != null)
                        tempProp.Add(new PropertyItem("Encoding", encoding.EncodingName));
                }

                if (IsMusic)
                {
                    tempProp.Add(new PropertyItem("Title", _shellObject.Properties.System.Title.ValueAsObject));
                    if (_shellObject.Properties?.System?.Author?.ValueAsObject is string[] authors)
                        tempProp.Add(new PropertyItem("Author(s)", string.Join(",", authors)));
                    tempProp.Add(new PropertyItem("Rating", _shellObject.Properties?.System?.Rating?.ValueAsObject));
                }

                if (IsPicture)
                {
                    tempProp.Add(new PropertyItem("Dimensions", props.GetValue("System.Image.Dimensions")));
                    foreach (var key in props.Items.Keys.Where(k => k.StartsWith("System.Photo")).OrderBy(k => k))
                        tempProp.Add(new PropertyItem(key.Substring(13), props.GetValue(key)));
                }


                return tempProp;
            });
        }

        private static async Task<Encoding> GetEncoding(string path)
        {
            try
            {
                using (var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    var size =(int) Math.Min(1024, stream.Length);
                    var buffer = new byte[size];
                    await stream.ReadAsync(buffer, 0, size);

                    var detector = new CharsetDetector();
                    detector.Feed(buffer, 0, size);
                    detector.DataEnd();
                    return detector.Charset == null ? null : Encoding.GetEncoding(detector.Charset);
                }
            }
            catch (Exception ex)
            {
                //File locks, etc.
                return null;
            }
        }

        //private static Encoding GetEncoding(string filename)
        //{
        //    // This is a direct quote from MSDN:  
        //    // The CurrentEncoding value can be different after the first
        //    // call to any Read method of StreamReader, since encoding
        //    // autodetection is not done until the first call to a Read method.

        //    using (var reader = new StreamReader(filename, Encoding.Default, true))
        //    {
        //        if (reader.Peek() >= 0) // you need this!
        //            reader.Read();

        //        return reader.CurrentEncoding;
        //    }
        //}

        private Task<ObservableCollection<PropertyItem>> GetNetworkTopItems()
        {
            var tempProp = new ObservableCollection<PropertyItem>();
            if (CancellationTokenSource?.IsCancellationRequested ?? false)
                return Task.FromResult(tempProp);

            return  Task.Run(() =>
            {
                var details = Item.GetDetail(false, CancellationToken.None);
                tempProp.Clear();
                tempProp.Add(new PropertyItem("Name", Item.Name));
                tempProp.Add(new PropertyItem("Type", details.Type));
                return tempProp;
            }, cancellationToken: CancellationTokenSource?.Token ?? default(CancellationToken));
        }

        private Task<ObservableCollection<PropertyItem>> GetNetworkBottomItems()
        {
            if (CancellationTokenSource != null && CancellationTokenSource.IsCancellationRequested || Item == null)
                return Task.FromResult(new ObservableCollection<PropertyItem>());

            return Task.Run(() =>
            {
                var details = Item.GetDetail(false, CancellationToken.None);
                var tempProp = new ObservableCollection<PropertyItem>
                {
                    new PropertyItem("Location", "Network"),
                    new PropertyItem("Attributes", Item.AttributeString),
                    new PropertyItem("Created", details.Created),
                    new PropertyItem("Last Modified", details.LastModified)
                };

                return tempProp;
            }, CancellationTokenSource?.Token ?? default(CancellationToken));
        }
    }
}