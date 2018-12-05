using System.Windows.Input;
using Kexi.ViewModel.Lister;

namespace Kexi.Interfaces
{
    public interface IKeyHandler
    {
        string SearchString { get; set; }
        void Execute(KeyEventArgs args, ILister lister, string group = null);
    }
}