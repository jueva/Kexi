using System;
using System.ComponentModel.Composition;
using Kexi.Interfaces;
using Kexi.ViewModel.Popup;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class ShowSynchronizeFilenamesPopupCommand : IKexiCommand
    {
        [ImportingConstructor]
        public ShowSynchronizeFilenamesPopupCommand(Workspace workspace, SynchronizeFilenamesPopupViewModel popupViewModel)
        {
            _workspace      = workspace;
            _popupViewModel = popupViewModel;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _workspace.PopupViewModel = _popupViewModel;
            _popupViewModel.Open();
        }

        public event EventHandler                           CanExecuteChanged;
        private readonly SynchronizeFilenamesPopupViewModel _popupViewModel;
        private readonly Workspace                          _workspace;
    }
}