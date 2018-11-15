using System;
using System.ComponentModel.Composition;
using Kexi.Interfaces;
using Kexi.ViewModel.Item;
using Kexi.ViewModel.Lister;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class ShowPropertyListerCommand : IKexiCommand
    {
        [ImportingConstructor]
        public ShowPropertyListerCommand(Workspace workspace)
        {
            _workspace = workspace;
        }

        public bool CanExecute(object parameter)
        {
            return _workspace.CurrentItem is FileItem;
        }

        public async void Execute(object parameter)
        {
            if (_workspace.CurrentItem is FileItem fileItem)
            {
                var propLister = KexContainer.Resolve<PropertyLister>();
                _workspace.Open(propLister);
                propLister.FileItem = fileItem;
                await propLister.Refresh().ConfigureAwait(false);
            }
        }

        public event EventHandler  CanExecuteChanged;
        private readonly Workspace _workspace;
    }
}