using System;
using System.ComponentModel.Composition;
using Kexi.Interfaces;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class ToggleRibbonVisibilityCommand : IKexiCommand
    {
        private readonly Workspace _workspace;

        [ImportingConstructor]
        public ToggleRibbonVisibilityCommand(Workspace workspace)
        {
            _workspace = workspace;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _workspace.RibbonViewModel.RibbonVisible = !_workspace.RibbonViewModel.RibbonVisible;
        }

        public event EventHandler CanExecuteChanged;
    }
}