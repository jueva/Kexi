using System;
using System.ComponentModel.Composition;
using Kexi.Interfaces;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class MoveToNextTemporaryBookmarkCommand : IKexiCommand
    {
        private readonly Workspace _workspace;

        [ImportingConstructor]
        public MoveToNextTemporaryBookmarkCommand(Workspace workspace)
        {
            _workspace = workspace;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var next = _workspace.TemporaryFavorites.Next();
            if (next != null)
                _workspace.ActiveLister.Path = next.Path;
        }

        public event EventHandler CanExecuteChanged;
    }
}