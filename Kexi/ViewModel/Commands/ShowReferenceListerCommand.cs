using System;
using System.ComponentModel.Composition;
using Kexi.Interfaces;
using Kexi.ViewModel.Item;
using Kexi.ViewModel.Lister;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class ShowReferenceListerCommand : IKexiCommand
    {
        [ImportingConstructor]
        public ShowReferenceListerCommand(Workspace workspace)
        {
            _workspace = workspace;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public async void Execute(object parameter)
        {
            if (_workspace.CurrentItem is FileItem fileItem)
            {
                var referenceLister = KexContainer.Resolve<ReferenceLister>();
                referenceLister.Path = fileItem.GetPathResolved();
                _workspace.Open(referenceLister);
                await referenceLister.Refresh().ConfigureAwait(false);
            }
        }

        public event EventHandler  CanExecuteChanged;
        private readonly Workspace _workspace;
    }
}