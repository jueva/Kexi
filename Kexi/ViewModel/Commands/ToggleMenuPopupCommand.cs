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
            if (parameter is string p && p == "FromButton")
            {
                _menuPopup.MenuButtonPlacement = true;
            }
            else
            {
                _menuPopup.MenuButtonPlacement = false;
            }

            if (_workspace.PopupViewModel is MenuPopupViewModel && _workspace.PopupViewModel.IsOpen)
            {
                _workspace.PopupViewModel.IsOpen = false;
            }
            else
            {
                _workspace.PopupViewModel = _menuPopup;
                _menuPopup.Open();
            }
        }

        public event EventHandler           CanExecuteChanged;
        private readonly MenuPopupViewModel _menuPopup;
        private readonly Workspace          _workspace;
    }
}