using System;
using System.ComponentModel.Composition;
using Kexi.Interfaces;
using Kexi.ViewModel.Lister;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class BreakConsoleCommand : IKexiCommand
    {
        [ImportingConstructor]
        public BreakConsoleCommand(Workspace workspace)
        {
            _workspace = workspace;
        }

        public bool CanExecute(object parameter)
        {
            return _workspace.ActiveLister is ConsoleLister;
        }

        public void Execute(object parameter)
        {
            if (_workspace.ActiveLister is ConsoleLister console)
                if (console.BreakConsole.CanExecute(parameter))
                    console.BreakConsole.Execute(parameter);
        }

        public event EventHandler  CanExecuteChanged;
        private readonly Workspace _workspace;
    }
}