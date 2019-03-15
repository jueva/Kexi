using Kexi.Interfaces;
using System;
using System.ComponentModel.Composition;
using Kexi.ViewModel.Item;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class AddCurrentFolderToTemporaryBookmarksCommand : IKexiCommand
    {
        [ImportingConstructor]
        public AddCurrentFolderToTemporaryBookmarksCommand(Workspace workspace)
        {
            _workspace = workspace;
        }

        public bool CanExecute(object parameter)
        {
            return _workspace.CurrentItem != null;
        }

        public void Execute(object parameter)
        {
            var item = new BaseItem(_workspace.ActiveLister.Path);
            _workspace.TemporaryFavorites.Add(item);
            _workspace.NotificationHost.AddInfo(item.Path + " added to Temporary Favorites");
        }

        public event EventHandler  CanExecuteChanged;
        private readonly Workspace _workspace;
    }
}