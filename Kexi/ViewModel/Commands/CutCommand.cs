using System;
using System.ComponentModel.Composition;
using Kexi.Interfaces;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class CutCommand : IKexiCommand
    {
        [ImportingConstructor]
        public CutCommand(Workspace workspace)
        {
            _workspace = workspace;
        }

        public bool CanExecute(object parameter)
        {
            return _workspace.ActiveLister is ICanCopyPaste;
        }

        public void Execute(object parameter)
        {
            if (!CanExecute(parameter))
                return;

            if (_workspace.ActiveLister is ICanCopyPaste handler)
                handler.Cut();
        }

        public event EventHandler  CanExecuteChanged;
        private readonly Workspace _workspace;
    }
}