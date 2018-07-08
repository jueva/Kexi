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
        private readonly Workspace _workspace;

        [ImportingConstructor]
        public ShowReferenceListerCommand(Workspace workspace)
        {
            _workspace = workspace;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public  void Execute(object parameter)
        {
            if (_workspace.CurrentItem is FileItem fileItem)
            {
                var referenceLister = KexContainer.Resolve<ReferenceLister>();
                referenceLister.Path = fileItem.GetPathResolved();
                _workspace.Open(referenceLister);
                referenceLister.Refresh();
            }
        }

        public event EventHandler CanExecuteChanged;
    }
}