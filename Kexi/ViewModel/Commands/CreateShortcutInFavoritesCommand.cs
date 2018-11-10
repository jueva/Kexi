using System;
using System.ComponentModel.Composition;
using System.IO;
using Kexi.Interfaces;
using Kexi.Shell;
using Kexi.ViewModel.Item;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class CreateShortcutInFavoritesCommand : IKexiCommand
    {
        [ImportingConstructor]
        public CreateShortcutInFavoritesCommand(Workspace workspace)
        {
            _workspace = workspace;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if (!(_workspace.CurrentItem is FileItem fileItem)) return;
            var favorites = Environment.GetFolderPath(Environment.SpecialFolder.Favorites);
            var fileName  = Path.Combine(favorites, fileItem.Name + ".lnk");
            Shortcut.Create(fileName, fileItem.Path, null, null, "Shortcut to " + fileItem.Name, null);
        }

        public event EventHandler  CanExecuteChanged;
        private readonly Workspace _workspace;
    }
}