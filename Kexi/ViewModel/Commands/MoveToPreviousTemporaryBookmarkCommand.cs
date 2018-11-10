using System;
using System.ComponentModel.Composition;
using Kexi.Interfaces;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class MoveToPreviousTemporaryBookmarkCommand : IKexiCommand
    {
        [ImportingConstructor]
        public MoveToPreviousTemporaryBookmarkCommand(Workspace workspace)
        {
            _workspace = workspace;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var previous = _workspace.TemporaryFavorites.Next();
            if (previous != null)
            {
                _workspace.ActiveLister.Path = previous.Path;
                _workspace.ActiveLister.Refresh();
            }
        }

        public event EventHandler  CanExecuteChanged;
        private readonly Workspace _workspace;
    }
}