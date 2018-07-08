using System;
using System.ComponentModel.Composition;
using Kexi.Interfaces;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class ToggleAdressbarVisibiblityCommand : IKexiCommand
    {
        private readonly Workspace _workspace;

        [ImportingConstructor]
        public ToggleAdressbarVisibiblityCommand(Workspace workspace)
        {
            _workspace = workspace;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _workspace.Options.GlobalAdressbarVisible = !_workspace.Options.GlobalAdressbarVisible; 
        }

        public event EventHandler CanExecuteChanged;
    }
}