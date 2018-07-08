using System;
using System.ComponentModel.Composition;
using Kexi.Interfaces;
using Kexi.ViewModel.Item;
using Kexi.ViewModel.Lister;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class ShowSecurityListerCommand : IKexiCommand
    {
        private readonly Workspace _workspace;

        [ImportingConstructor]
        public ShowSecurityListerCommand(Workspace workspace)
        {
            _workspace = workspace;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if (_workspace.CurrentItem is FileItem fileItem)
            {
                var securityLister = KexContainer.Resolve<SecurityLister>();
                securityLister.Path = fileItem.Path;
                _workspace.Open(securityLister);
                _workspace.ActiveLister.Refresh();
            }
        }

        public event EventHandler CanExecuteChanged;
    }
}