using System;
using System.ComponentModel.Composition;
using Kexi.Interfaces;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class CycleThemeCommand : IKexiCommand
    {
        private readonly Workspace _workspace;

        [ImportingConstructor]
        public CycleThemeCommand(Workspace workspace)
        {
            _workspace = workspace;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _workspace.ThemeHandler.MoveNext();
        }

        public event EventHandler CanExecuteChanged;
    }
}