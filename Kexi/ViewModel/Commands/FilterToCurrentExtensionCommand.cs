using System;
using System.ComponentModel.Composition;
using Kexi.Interfaces;
using Kexi.ViewModel.Item;
using Kexi.ViewModel.Popup;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class FilterToCurrentExtensionCommand : IKexiCommand
    {
        private readonly Workspace _workspace;

        [ImportingConstructor]
        public FilterToCurrentExtensionCommand(Workspace workspace)
        {
            _workspace = workspace;
        }

        public bool CanExecute(object parameter)
        {
            return _workspace.CurrentItem is FileItem;
        }

        public void Execute(object parameter)
        {
            if (!CanExecute(parameter))
                return;

            if (_workspace.CurrentItem is FileItem fileItem)
            {
                _workspace.ActiveLister.Filter = fileItem.Extension;
                _workspace.ActiveLister.ItemsView.Filter =
                    FilterPopupViewModel.GetFilterPredicate(fileItem.Extension);
                _workspace.FocusListView();
            }
            
        }

        public event EventHandler CanExecuteChanged;
    }
}