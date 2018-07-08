using System;
using System.ComponentModel.Composition;
using Kexi.Interfaces;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class RepeatLastCommandCommand : IKexiCommand
    {
        private readonly Workspace _workspace;

        [ImportingConstructor]
        public RepeatLastCommandCommand(Workspace workspace)
        {
            _workspace = workspace;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            //TODO: Implement
        }

        public event EventHandler CanExecuteChanged;
    }
}