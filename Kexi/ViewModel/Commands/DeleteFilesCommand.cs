using System;
using System.ComponentModel.Composition;
using System.Linq;
using Kexi.Common;
using Kexi.Files;
using Kexi.Interfaces;
using Kexi.ViewModel.Item;
using Kexi.ViewModel.Popup;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class DeleteFilesCommand : IKexiCommand
    {
        private readonly Workspace _workspace;
        private readonly DialogPopupViewModel _dialogPopup;

        [ImportingConstructor]
        public DeleteFilesCommand(Workspace workspace, DialogPopupViewModel dialogPopup)
        {
            _workspace = workspace;
            _dialogPopup = dialogPopup;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            void Delete(string a)
            {
                if (a == "Yes") DeleteSelectedFiles();
            }

            _workspace.PopupViewModel = _dialogPopup;
            _dialogPopup.Open("Delete Files", Delete, "Yes", "No");
        }

        private void DeleteSelectedFiles()
        {
            var selectedItems = _workspace.ActiveLister.SelectedItems.OfType<FileItem>();
            var result        = new FilesystemAction(_workspace.NotificationHost).Delete(selectedItems);
            if (result != null) _workspace.NotificationHost.AddError(result);
        }

        public event EventHandler CanExecuteChanged;
    }
}