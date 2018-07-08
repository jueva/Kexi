using System;
using System.ComponentModel.Composition;
using System.Linq;
using Kexi.Interfaces;
using Kexi.ViewModel.Popup;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class ShowSortPopupCommand : IKexiCommand
    {
        private readonly Workspace _workspace;
        private readonly SortPopupViewModel _sortPopup;

        [ImportingConstructor]
        public ShowSortPopupCommand(Workspace workspace, SortPopupViewModel sortPopup)
        {
            _workspace = workspace;
            _sortPopup = sortPopup;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _workspace.PopupViewModel  = _sortPopup;
            _sortPopup.Open();
            if (_workspace.ActiveLister.ItemsView.SortDescriptions.Any())
            {
                var sortColumn =
                    _workspace.ActiveLister.Columns.First(
                        c =>
                            c.BindingExpression ==
                            _workspace.ActiveLister.ItemsView.SortDescriptions.First().PropertyName);
                _sortPopup.Text = sortColumn.Header;

                _sortPopup.SelectAll();
            }
        }

        public event EventHandler CanExecuteChanged;
    }
}