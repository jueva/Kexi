using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Kexi.ViewModel.Item
{
   
    public class CommandBoundItem : BaseItem
    {
        private readonly ICommand _command;

        public CommandBoundItem(string name, ICommand command) : base(name)
        {
            _command = command;
        }

        public ICommand Command
        {
            get { return _command; }
        }

    }
}
