using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kexi.ViewModel.Item
{
    public class ConsoleCommandItem : BaseItem
    {
        private readonly string _command;
        private readonly string _workingDirectory;

        public ConsoleCommandItem(string commandString, string workingDirectory)
            : base(commandString)
        {
            DisplayName = FilterString = Path = commandString;
            _command = commandString;
            _workingDirectory = workingDirectory;
        }

        public string Command { get { return _command; } }
        public string WorkingDirectroy { get { return _workingDirectory; } }
    }
}
