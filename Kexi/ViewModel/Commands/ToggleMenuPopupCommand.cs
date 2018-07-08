using System;
using System.ComponentModel.Composition;
using Kexi.Interfaces;
using Kexi.ViewModel.Popup;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class ToggleMenuPopupCommand : IKexiCommand
    {
        private readonly Workspace _workspace;
        private readonly MenuPopupViewModel _menuPopup;

        [ImportingConstructor]
        public ToggleMenuPopupCommand(Workspace workspace, MenuPopupViewModel menuPopup)
        {
            _workspace = workspace;
            _menuPopup = menuPopup;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if (_workspace.PopupViewModel is MenuPopupViewModel && _workspace.PopupViewModel.IsOpen)
            {
                _workspace.PopupViewModel.IsOpen = false;
            }
            else
            {
                _workspace.PopupViewModel      = _menuPopup;
                _menuPopup.Open();
            }
        }

        public event EventHandler CanExecuteChanged;
    }
}