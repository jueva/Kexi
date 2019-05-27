using System;
using System.ComponentModel.Composition;
using Kexi.Interfaces;
using Kexi.ViewModel.Lister;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class ShowFavoritesCommand : IKexiCommand
    {
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
            if (!(_workspace.ActiveLister is FileLister))
            {
                var l = KexContainer.Resolve<FileLister>();
                _workspace.ReplaceCurrentLister(l);
            }
            var favoriteLocation = Environment.GetFolderPath(Environment.SpecialFolder.Favorites);
            _workspace.ActiveLister.Path = favoriteLocation;
            _workspace.ActiveLister.Refresh();
        }

        public event EventHandler  CanExecuteChanged;
        private readonly Workspace _workspace;
    }
}