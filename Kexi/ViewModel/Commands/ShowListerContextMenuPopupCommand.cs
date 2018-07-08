using System;
using System.ComponentModel.Composition;
using Kexi.Interfaces;
using Kexi.ViewModel.Popup;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class ShowListerContextMenuPopupCommand : IKexiCommand
    {
        private readonly Workspace _workspace;
        private readonly CommandBoundPopupViewModel _commandBoundPopup;

        [ImportingConstructor]
        public ShowListerContextMenuPopupCommand(Workspace workspace, CommandBoundPopupViewModel commandBoundPopup)
        {
            _workspace = workspace;
            _commandBoundPopup = commandBoundPopup;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object c)
        {
            var showMenuPopupViewModel = _commandBoundPopup;
            var mousePlacement                    = c is bool b && b;
            showMenuPopupViewModel.MousePlacement = mousePlacement;
            _workspace.PopupViewModel              = showMenuPopupViewModel;
            showMenuPopupViewModel.Open();
        }

        public event EventHandler CanExecuteChanged;
    }
}