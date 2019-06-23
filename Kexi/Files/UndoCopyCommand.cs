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
                if (Directory.Exists(item)) Directory.Delete(item);
            }
        }

        private string GetCopiedItem(string source, string path)
        {
            var info = new FileInfo(source);
            var name = info.Name;
            var targetPath = path + "\\" + name;
            return targetPath;
        }
    }
}