using System;
using System.ComponentModel.Composition;
using Kexi.Interfaces;
using Kexi.ViewModel.Popup;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class DeleteItemsCommand : IKexiCommand
    {
        [ImportingConstructor]
        public DeleteItemsCommand(Workspace workspace, DialogPopupViewModel dialogPopup)
        {
            _workspace   = workspace;
            _dialogPopup = dialogPopup;
        }

        public bool CanExecute(object parameter)
        {
            return _workspace.ActiveLister is ICanDelete;
        }

        public void Execute(object parameter)
        {
            void Delete(string a)
            {
                if (a == optionYes) DeleteSelectedItems();
            }

            _workspace.PopupViewModel = _dialogPopup;
            _dialogPopup.Open("Delete Files", Delete, optionYes, optionNo);
        }

        public event EventHandler             CanExecuteChanged;
        private readonly DialogPopupViewModel _dialogPopup;
        private readonly Workspace            _workspace;
        private const string optionYes = "Yes";
        private const string optionNo = "No";

        private void DeleteSelectedItems()
        {
            if (_workspace.ActiveLister is ICanDelete deleteable)
            {
                deleteable.Delete();
            }
        }
    }
}