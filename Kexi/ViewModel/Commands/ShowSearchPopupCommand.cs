using System;
using System.ComponentModel.Composition;
using Kexi.Interfaces;
using Kexi.ViewModel.Popup;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class ShowSearchPopupCommand : IKexiCommand
    {
        [ImportingConstructor]
        public ShowSearchPopupCommand(Workspace workspace, SearchPopupViewModel searchPopup)
        {
            _workspace   = workspace;
            _searchPopup = searchPopup;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _workspace.PopupViewModel = _searchPopup;
            _searchPopup.Open();
        }

        public event EventHandler             CanExecuteChanged;
        private readonly SearchPopupViewModel _searchPopup;
        private readonly Workspace            _workspace;
    }
}