using System;
using System.ComponentModel.Composition;
using Kexi.Interfaces;
using Kexi.ViewModel.Lister;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class ViewFileCommand : IKexiCommand
    {
        private readonly Workspace _workspace;

        [ImportingConstructor]
        public ViewFileCommand(Workspace workspace)
        {
            _workspace = workspace;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var viewer = KexContainer.Resolve<ViewFileLister>();
            viewer.Path = _workspace.CurrentItem.Path;
            _workspace.Open(viewer);
            _workspace.ActiveLister.Refresh();

        }

        public event EventHandler CanExecuteChanged;
    }
}