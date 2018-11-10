using System;
using System.ComponentModel.Composition;
using System.IO;
using Kexi.Interfaces;
using Kexi.ViewModel.Item;
using Kexi.ViewModel.Lister;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class OpenDirectoryCommand : IKexiCommand
    {
        [ImportingConstructor]
        public OpenDirectoryCommand(Workspace workspace)
        {
            _workspace = workspace;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public async void Execute(object parameter)
        {
            if (!(_workspace.CurrentItem is FileItem currentFileItem))
                return;

            var fileLister = KexContainer.Resolve<FileLister>();
            if (currentFileItem.TargetType() == ItemType.Item)
            {
                var path = currentFileItem.GetPathResolved();
                fileLister.Path = Path.GetDirectoryName(path) ?? path;
            }
            else
            {
                var path = currentFileItem.GetPathResolved();
                fileLister.Path = path;
            }

            _workspace.Open(fileLister);
            await fileLister.Refresh();
        }

        public event EventHandler  CanExecuteChanged;
        private readonly Workspace _workspace;
    }
}