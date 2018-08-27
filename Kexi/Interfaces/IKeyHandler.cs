using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Kexi.ViewModel.Lister;

namespace Kexi.Interfaces
{
    public interface IKeyHandler
    {
        bool Execute(KeyEventArgs args, ILister lister, string group = null);
        string SearchString { get; set; }
    }
}
