using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kexi.ViewModel.Item
{
    public class ConsoleItem: BaseItem
    {
        public ConsoleItem(string displayName, string command = null) : base(displayName)
        {
            Command = command ?? displayName;
        }

        public string Command { get; set; }

    }
}
