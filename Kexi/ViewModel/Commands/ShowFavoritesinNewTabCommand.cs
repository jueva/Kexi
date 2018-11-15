using System;
using System.ComponentModel.Composition;
using Kexi.Interfaces;
using Kexi.ViewModel.Lister;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class ShowFavoritesinNewTabCommand : IKexiCommand
    {
        [ImportingConstructor]
        public ShowFavoritesinNewTabCommand(Workspace workspace)
        {
            _workspace = workspace;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public async void Execute(object parameter)
        {
            var favoriteLocation = Environment.GetFolderPath(Environment.SpecialFolder.Favorites);
            var fileLister       = KexContainer.Resolve<FileLister>();
            fileLister.Path = favoriteLocation;
            await fileLister.Refresh().ConfigureAwait(false);
            _workspace.Open(fileLister);
        }

        public event EventHandler  CanExecuteChanged;
        private readonly Workspace _workspace;
    }
}