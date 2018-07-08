using System;
using System.ComponentModel.Composition;
using Kexi.Interfaces;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class WindowSplitHorizontalCommand : IKexiCommand
    {
        private readonly Workspace _workspace;

        [ImportingConstructor]
        public WindowSplitHorizontalCommand(Workspace workspace)
        {
            _workspace = workspace;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _workspace.SplitHorizontal();
        }

        public event EventHandler CanExecuteChanged;
    }
}