using System;
using System.ComponentModel.Composition;
using Kexi.Interfaces;
using Kexi.ViewModel.Popup;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class ShowSpecialFoldersPopupCommand : IKexiCommand
    {
        private readonly Workspace _workspace;
        private readonly SpecialFolderPopupViewModel _popupViewModel;

        [ImportingConstructor]
        public ShowSpecialFoldersPopupCommand(Workspace workspace, SpecialFolderPopupViewModel popupViewModel)
        {
            _workspace = workspace;
            _popupViewModel = popupViewModel;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _workspace.PopupViewModel      = _popupViewModel;
            _popupViewModel.Open();
        }

        public event EventHandler CanExecuteChanged;
    }
}