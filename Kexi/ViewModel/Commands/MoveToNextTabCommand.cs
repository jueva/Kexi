using System;
using System.ComponentModel.Composition;
using Kexi.Interfaces;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class MoveToNextTabCommand : IKexiCommand
    {
        [ImportingConstructor]
        public MoveToNextTabCommand(Workspace workspace)
        {
            _workspace = workspace;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _workspace.MoveToNextTab();
        }

        public event EventHandler  CanExecuteChanged;
        private readonly Workspace _workspace;
    }
}