using System;
using System.ComponentModel.Composition;
using Kexi.Interfaces;
using Kexi.ViewModel.Popup;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class ShowConsolePopupCommand : IKexiCommand
    {
        [ImportingConstructor]
        public ShowConsolePopupCommand(Workspace workspace, ConsoleCommandPopupViewModel consoleCommandPopup)
        {
            _workspace           = workspace;
            _consoleCommandPopup = consoleCommandPopup;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var command = parameter as string;
            _workspace.PopupViewModel = _consoleCommandPopup;
            _consoleCommandPopup.Open();
            if (!string.IsNullOrEmpty(command))
            {
                _consoleCommandPopup.Text = command;
                _consoleCommandPopup.SelectAll();
            }

            _consoleCommandPopup.UnSelectListView();
        }

        public event EventHandler                     CanExecuteChanged;
        private readonly ConsoleCommandPopupViewModel _consoleCommandPopup;
        private readonly Workspace                    _workspace;
    }
}