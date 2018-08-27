using System.Collections.Generic;
using System.Windows.Input;
using Kexi.Interfaces;
using Kexi.ViewModel;
using Kexi.ViewModel.Lister;

namespace Kexi.Common.KeyHandling
{
    public class LiveFilterKeyHandler : IKeyHandler
    {
        private readonly Workspace _workspace;

        public LiveFilterKeyHandler(Workspace workspace)
        {
            this._workspace = workspace;
        }

        public List<KexBinding> Bindings { get; set; }

        public bool Execute(KeyEventArgs args, ILister lister, string group)
        {
            return false;
        }

        public string SearchString { get; set; }
    }
}