using System;
using System.ComponentModel.Composition;
using Kexi.Interfaces;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class MarkSelectedCommand : IKexiCommand
    {
        [ImportingConstructor]
        public MarkSelectedCommand(Workspace workspace)
        {
            _workspace = workspace;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var current = _workspace.ActiveLister.CurrentItem;
            if (current != null)
                _workspace.ActiveLister.SetSelection(current, true);
        }

        public event EventHandler  CanExecuteChanged;
        private readonly Workspace _workspace;
    }
}