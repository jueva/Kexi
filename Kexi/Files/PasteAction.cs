using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Kexi.Shell;

namespace Kexi.Files
{
    public class PasteAction
    {

        public static string Paste(string target, StringCollection items, FileAction action, short flags = ShellNative.FOF_ALLOWUNDO)
        {
            if (!Clipboard.ContainsFileDropList())
                return null;

            if (action != FileAction.Copy && action != FileAction.Move && action != FileAction.Create)
                throw new ArgumentOutOfRangeException(nameof(action), action, "Action should be Copy or Move");

            var   it                    = items.Cast<string>().ToList();
            var   from                  = string.Join("\0", it) + "\0";
            var   to                    = target + "\0";

            if (items.Count == 1)
            {
                var info = new FileInfo(items[0]);
                if (info.Directory.FullName.ToLower() == target.ToLower())
                {
                    flags = (short) (flags | ShellNative.FOF_RENAMEONCOLLISION);
                }
            }

            var fileop = new ShellNative.SHFILEOPSTRUCT
            {
                wFunc  = action == FileAction.Copy ? ShellNative.FO_COPY : ShellNative.FO_MOVE,
                pFrom  = from,
                pTo    = to,
                fFlags = flags
            };
            ShellNative.SHFileOperation(ref fileop);

            return $@"{target}\{it.LastOrDefault()}";
        }
    }
}
