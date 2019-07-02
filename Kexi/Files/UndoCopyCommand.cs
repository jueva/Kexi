using System;
using System.Collections.Specialized;
using System.IO;

namespace Kexi.Files
{
    public class UndoCopyCommand
    {
        private readonly StringCollection _items;
        private readonly string _path;

        public UndoCopyCommand(string path, StringCollection items)
        {
            _path = path;
            _items = items;
        }

        public void DoUndo()
        {
            foreach (var source in _items)
            {
                var item = GetCopiedItem(source, _path);
                if (File.Exists(item)) File.Delete(item);
                else if (Directory.Exists(item)) Directory.Delete(item);
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

    }
}