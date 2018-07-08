using System;
using System.ComponentModel.Composition;
using Kexi.Interfaces;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class WindowCloseCommand : IKexiCommand
    {
        private readonly Workspace _workspace;

        [ImportingConstructor]
        public WindowCloseCommand(Workspace workspace)
        {
            _workspace = workspace;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _workspace.CloseCurrentLister();
        }

        public event EventHandler CanExecuteChanged;
    }
}