using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kexi.Interfaces;
using Kexi.ViewModel.Item;

namespace Kexi.Common
{
    public static class FileHelper
    {
        public static void Rename(FileItem fileItem, string newName)
        {
            if (fileItem != null)
            {
                if (fileItem.ItemType == ItemType.Container)
                {
                    var di     = new DirectoryInfo(fileItem.Path);
                    var parent = di.Parent ?? di.Root;
                    var dest   = Path.Combine(parent.FullName, newName);
                    if (fileItem.Path.Equals(dest, StringComparison.OrdinalIgnoreCase))
                        return;
                    di.MoveTo(dest);
                    fileItem.Path        = dest;
                    fileItem.DisplayName = newName;
                }
                else
                {
                    var di     = new FileInfo(fileItem.Path);
                    var parent = di.Directory;
                    if (parent != null)
                    {
                        var dest = Path.Combine(parent.FullName, $"{newName}");
                        if (fileItem.Path.Equals(dest, StringComparison.OrdinalIgnoreCase))
                            return;
                        di.MoveTo(dest);
                        fileItem.Path        = dest;
                        fileItem.DisplayName = newName;
                    }
                }
            }
        }

        public static (int, int) GetRenameSelectionBorder(FileItem targetItem)
        {
            var isDirectory                     = false;
            if (targetItem != null) isDirectory = targetItem.ItemType == ItemType.Container;

            var endIndex = targetItem.Name.LastIndexOf('.');
            if (endIndex == -1 || isDirectory)
                endIndex = targetItem.Name.Length;

            return (0, endIndex);
        }
    }
}
