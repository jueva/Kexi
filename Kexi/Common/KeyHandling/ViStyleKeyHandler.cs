using System.Collections.Generic;
using System.Windows.Input;
using Kexi.Interfaces;
using Kexi.ViewModel;
using Kexi.ViewModel.Lister;

namespace Kexi.Common.KeyHandling
{
    public class ViStyleKeyHandler : IKeyHandler
    {
        public ViStyleKeyHandler(Workspace workspace, List<KexBinding> bindings)
        {
            Bindings        = bindings;
            _bindingHandler = new BindingHandler(workspace, bindings);
        }

        public List<KexBinding> Bindings { get; }

        public void Execute(KeyEventArgs args, ILister lister, string group = null)
        {
            _bindingHandler.Handle(args, lister);
        }

        public           string         SearchString { get; set; }
        private readonly BindingHandler _bindingHandler;
    }
}