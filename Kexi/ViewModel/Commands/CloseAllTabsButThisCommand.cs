using System;
using System.ComponentModel.Composition;
using System.Linq;
using Kexi.Common;
using Kexi.Interfaces;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class CloseAllTabsButThisCommand : IKexiCommand
    {
        [ImportingConstructor]
        public CloseAllTabsButThisCommand(Workspace workspace)
        {
            _workspace = workspace;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var otherListers = _workspace.Docking.Files.Where(f => f.Content != _workspace.ActiveLister).ToArray();
            foreach (var lister in otherListers)
                _workspace.Docking.Files.Remove(lister);
        }

        public event EventHandler  CanExecuteChanged;
        private readonly Workspace _workspace;
    }
}
