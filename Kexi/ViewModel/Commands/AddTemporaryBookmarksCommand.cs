using Kexi.Interfaces;
using System;
using System.ComponentModel.Composition;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class AddTemporaryBookmarksCommand : IKexiCommand
    {
        [ImportingConstructor]
        public AddTemporaryBookmarksCommand(Workspace workspace)
        {
            _workspace = workspace;
        }

        public bool CanExecute(object parameter)
        {
            return _workspace.CurrentItem != null;
        }

        public void Execute(object parameter)
        {
            var item = _workspace.CurrentItem;
            _workspace.TemporaryFavorites.Add(item);
            _workspace.NotificationHost.AddInfo(item.Path + " added to Temporary Favorites");
        }

        public event EventHandler  CanExecuteChanged;
        private readonly Workspace _workspace;
    }
}