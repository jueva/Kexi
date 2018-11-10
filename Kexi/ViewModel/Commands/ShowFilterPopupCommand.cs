using System;
using System.ComponentModel.Composition;
using Kexi.Interfaces;
using Kexi.ViewModel.Lister;
using Kexi.ViewModel.Popup;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class ShowFilterPopupCommand : IKexiCommand
    {
        [ImportingConstructor]
        public ShowFilterPopupCommand(Workspace workspace, FileFilterPopupViewModel fileFilterPopup, FilterPopupViewModel filterPopup)
        {
            _workspace       = workspace;
            _fileFilterPopup = fileFilterPopup;
            _filterPopup     = filterPopup;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var filterPopupViewModel = _workspace.ActiveLister is FileLister
                ? (PopupViewModel) _fileFilterPopup
                : _filterPopup;

            _workspace.PopupViewModel = filterPopupViewModel;
            filterPopupViewModel.Open();
            filterPopupViewModel.SelectAll();
        }

        public event EventHandler                 CanExecuteChanged;
        private readonly FileFilterPopupViewModel _fileFilterPopup;
        private readonly FilterPopupViewModel     _filterPopup;
        private readonly Workspace                _workspace;
    }
}