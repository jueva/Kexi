using System;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.Linq;
using Kexi.Files;
using Kexi.Interfaces;
using Kexi.ViewModel.Popup;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class DeleteItemsCommand : IKexiCommand, IUndoable
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
        private Tuple<string, StringCollection, FileAction> _undoParameter;

        private void DeleteSelectedItems()
        {
            if (_workspace.ActiveLister is ICanDelete deleteable)
            {
                var items = new StringCollection();
                items.AddRange(_workspace.ActiveLister.SelectedItems.Select(i => i.Path).ToArray());
                _undoParameter = new Tuple<string, StringCollection, FileAction>(_workspace.ActiveLister.Path, items, FileAction.Delete);
                deleteable.Delete();
            }
        }

        public void Undo()
        {
            _workspace.NotificationHost.AddInfo($"{_undoParameter.Item1} - {_undoParameter.Item2.Count} - {_undoParameter.Item3}");
        }
    }
}