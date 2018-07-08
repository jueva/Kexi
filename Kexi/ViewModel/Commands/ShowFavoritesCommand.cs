using System;
using System.ComponentModel.Composition;
using Kexi.Interfaces;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class ShowFavoritesCommand : IKexiCommand
    {
        private readonly Workspace _workspace;

        [ImportingConstructor]
        public ShowFavoritesCommand(Workspace workspace)
        {
            _workspace = workspace;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var favoriteLocation = Environment.GetFolderPath(Environment.SpecialFolder.Favorites);
            _workspace.ActiveLister.Path = favoriteLocation;
            _workspace.ActiveLister.Refresh();
        }

        public event EventHandler CanExecuteChanged;
    }
}