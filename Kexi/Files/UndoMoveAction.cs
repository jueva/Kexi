using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kexi.Shell;

namespace Kexi.Files
{
    public class UndoMoveAction
    {
        private readonly StringCollection _items;
        private readonly string _path;

        public UndoMoveAction(string path, StringCollection items)
        {
            _path = path;
            _items = items;
        }

        public void DoUndo()
        {
            foreach (var source in _items)
            {
                var item = GetCopiedItem(source, _path);
                Move(item, source);
            }
        }

        private string GetCopiedItem(string source, string path)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            var targetPath = Path.Combine(path, Path.GetFileName(source));
            return targetPath;
        }


        private void Move(string from, string to)
        {
            var fileop = new ShellNative.SHFILEOPSTRUCT
            {
                wFunc = ShellNative.FO_MOVE,
                pFrom = from,
                pTo = to,
                fFlags = ShellNative.FOF_RENAMEONCOLLISION
            };
            ShellNative.SHFileOperation(ref fileop);
        }
    }
}