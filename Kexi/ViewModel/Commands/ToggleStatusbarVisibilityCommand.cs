using System;
using System.ComponentModel.Composition;
using Kexi.Interfaces;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class ToggleStatusbarVisibilityCommand : IKexiCommand
    {
        [ImportingConstructor]
        public ToggleStatusbarVisibilityCommand(Workspace workspace)
        {
            _workspace = workspace;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _workspace.Options.StatusbarVisible = !_workspace.Options.StatusbarVisible;
        }

        public event EventHandler  CanExecuteChanged;
        private readonly Workspace _workspace;
    }
}