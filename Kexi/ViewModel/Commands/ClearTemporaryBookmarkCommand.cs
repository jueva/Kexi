using System;
using System.ComponentModel.Composition;
using Kexi.Interfaces;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class ClearTemporaryBookmarkCommand : IKexiCommand
    {
        private readonly Workspace _workspace;

        [ImportingConstructor]
        public ClearTemporaryBookmarkCommand(Workspace workspace)
        {
            _workspace = workspace;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _workspace.TemporaryFavorites.Favorites.Clear();
            _workspace.NotificationHost.AddInfo("Temporary Favorites cleared");

        }

        public event EventHandler CanExecuteChanged;
    }
}