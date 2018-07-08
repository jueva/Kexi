using System;
using System.ComponentModel.Composition;
using Kexi.Interfaces;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class GotoParentContainerCommand : IKexiCommand
    {
        private readonly Workspace _workspace;

        [ImportingConstructor]
        public GotoParentContainerCommand(Workspace workspace)
        {
            _workspace = workspace;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var parent = _workspace.ActiveLister.GetParentContainer();
            if (parent == null)
                return;
            _workspace.ActiveLister.Path = parent;
            _workspace.ActiveLister.Refresh();
        }

        public event EventHandler CanExecuteChanged;
    }
}