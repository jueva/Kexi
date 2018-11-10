using System;
using System.ComponentModel.Composition;
using Kexi.Interfaces;
using Kexi.ViewModel.Popup;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class ShowCommandsPopupCommand : IKexiCommand
    {
        [ImportingConstructor]
        public ShowCommandsPopupCommand(Workspace workspace, CommandsPopupViewModel commandsPopup)
        {
            _workspace     = workspace;
            _commandsPopup = commandsPopup;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _workspace.PopupViewModel = _commandsPopup;
            _commandsPopup.Open();
        }

        public event EventHandler               CanExecuteChanged;
        private readonly CommandsPopupViewModel _commandsPopup;
        private readonly Workspace              _workspace;
    }
}