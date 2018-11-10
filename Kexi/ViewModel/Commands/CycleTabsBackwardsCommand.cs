using System;
using System.ComponentModel.Composition;
using Kexi.Interfaces;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class CycleTabsBackwardsCommand : IKexiCommand
    {
        [ImportingConstructor]
        public CycleTabsBackwardsCommand(Workspace workspace)
        {
            _workspace = workspace;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _workspace.CycleTab(-1);
        }

        public event EventHandler  CanExecuteChanged;
        private readonly Workspace _workspace;
    }
}