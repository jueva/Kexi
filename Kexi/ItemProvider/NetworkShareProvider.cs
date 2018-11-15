using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Kexi.Common;
using Kexi.Interfaces;
using Kexi.ViewModel.Item;
using Microsoft.WindowsAPICodePack.Shell;

namespace Kexi.ItemProvider
{
    public static class NetworkShareProvider
    {
        public static  Task<IEnumerable<FileItem>> GetItems(string path)
        {
            var shareThumb = Utils.GetImageFromRessource("share.png");
            shareThumb.Freeze();

            return Task.Run(() =>
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

        //https://stackoverflow.com/questions/19123389/check-if-server-path-is-available-as-file-share-in-c-sharp
        public static string GetNetworkPathFromServerName(string serverName)
        {
            // Assume we can't connect to the server to start with.
            var networkPath = string.Empty;

            // If this is a rooted path, just make sure it is available.
            if (Path.IsPathRooted(serverName))
            {
                // If the path exists, use it.
                if (Directory.Exists(serverName))
                    networkPath = serverName;
            }
            // Else this is a network path.
            else
            {
                // If the server name has a backslash in it, remove the backslash and everything after it.
                serverName = serverName.Trim(@"\".ToCharArray());
                if (serverName.Contains(@"\"))
                    serverName = serverName.Remove(serverName.IndexOf(@"\", StringComparison.Ordinal));

                try
                {
                    // If the server is available, format the network path properly to use it.
                    if (Dns.GetHostEntry(serverName) != null)
                    {
                        // Root the path as a network path (i.e. add \\ to the front of it).
                        networkPath = $@"\\{serverName}";
                    }
                }
                // Eat any Host Not Found exceptions for if we can't connect to the server.
                catch (System.Net.Sockets.SocketException)
                { }
            }

            return networkPath;
        }
    }
}