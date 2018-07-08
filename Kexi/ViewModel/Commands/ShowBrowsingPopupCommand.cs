using System;
using System.ComponentModel.Composition;
using Kexi.Interfaces;
using Kexi.ViewModel.Popup;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class ShowBrowsingPopupCommand : IKexiCommand
    {
        private readonly Workspace _workspace;
        private readonly BrowsingPopupViewModel _browsingPopup;

        [ImportingConstructor]
        public ShowBrowsingPopupCommand(Workspace workspace, BrowsingPopupViewModel browsingPopup)
        {
            _workspace = workspace;
            _browsingPopup = browsingPopup;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
           _workspace.PopupViewModel = _browsingPopup;
           _browsingPopup.Open();
        }

        public event EventHandler CanExecuteChanged;
    }
}