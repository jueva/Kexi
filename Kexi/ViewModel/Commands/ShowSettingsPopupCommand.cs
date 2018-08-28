using System;
using System.ComponentModel.Composition;
using Kexi.Interfaces;
using Kexi.ViewModel.Popup;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class ShowSettingsPopupCommand : IKexiCommand
    {
        [ImportingConstructor]
        public ShowSettingsPopupCommand(Workspace workspace, SettingsPopupViewModel settingsPopup)
        {
            _workspace     = workspace;
            _settingsPopup = settingsPopup;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _workspace.PopupViewModel = _settingsPopup;
            _settingsPopup.Open();
        }

        public event EventHandler               CanExecuteChanged;
        private readonly SettingsPopupViewModel _settingsPopup;
        private readonly Workspace              _workspace;
    }
}