using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kexi.Files
{
    public class UndoDeleteCommand
    {
        private readonly string _path;
        private readonly StringCollection _items;

        public UndoDeleteCommand(string path, StringCollection items)
        {
            _path = path;
            _items = items;
        }

        public void DoUndo()
        {
        }
    }
}
