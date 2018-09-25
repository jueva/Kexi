using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Kexi.Common;
using Kexi.Interfaces;
using Kexi.ViewModel.Item;
using Microsoft.WindowsAPICodePack.Shell;

namespace Kexi.ItemProvider
{
    public class NetworkShareProvider
    {

        public async Task<IEnumerable<FileItem>> GetItems(string path)
        {
            var                      shareThumb = Utils.GetImageFromRessource("share.png");
            shareThumb.Freeze();
            return await Task.Run(() =>
            {
                var folder = ShellObject.FromParsingName(path) as ShellFolder;
                if (folder == null)
                    return Enumerable.Empty<FileItem>();

                ImmutableArray<FileItem> items;
                try
                {
                    items = folder.Select(i => new FileItem(i.Properties.System.ParsingPath.Value, ItemType.Container)
                    {
                        Thumbnail   = shareThumb,
                        IsFileShare = true,
                    }).ToImmutableArray();
                }
                catch (UnauthorizedAccessException)
                {
                    PinvokeWindowsNetworking.ConnectToRemote(path, "", "", true);
                    items = folder.Select(i => new FileItem(i.Properties.System.ParsingPath.Value, ItemType.Container)
                    {
                        IsFileShare = true,
                        Thumbnail   = shareThumb,
                    }).ToImmutableArray();
                }

                foreach (var fi in items)
                {
                    fi.Details = new FileDetailItem(fi, CancellationToken.None)
                    {
                        Type = "Share",
                    };
                }

                return items;
            });
        }
    }
}