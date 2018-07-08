using System;
using System.ComponentModel.Composition;
using Kexi.Interfaces;
using Kexi.ViewModel.Popup;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class ShowCreateElementPopupCommand : IKexiCommand
    {
        [ImportingConstructor]
        public ShowCreateElementPopupCommand(Workspace workspace, CreateElementPopupViewModel createElementPopup)
        {
            _workspace          = workspace;
            _createElementPopup = createElementPopup;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _workspace.PopupViewModel = _createElementPopup;
            _createElementPopup.Open();
        }

        public event EventHandler                    CanExecuteChanged;
        private readonly CreateElementPopupViewModel _createElementPopup;
        private readonly Workspace                   _workspace;
    }
}