using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using Kexi.Interfaces;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class ClearFilterCommand : IKexiCommand
    {
        private readonly Workspace _workspace;

        [ImportingConstructor]
        public ClearFilterCommand(Workspace workspace)
        {
            _workspace = workspace;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var currentSelection = _workspace.ActiveLister.SelectedItems.ToArray();
            _workspace.ActiveLister.Filter           = null;
            _workspace.ActiveLister.ItemsView.Filter = null;
            if (_workspace.ActiveLister.ItemsView is ListCollectionView col)
            {
                col.CustomSort = null;
            }
            foreach(var i in currentSelection)
                _workspace.ActiveLister.SetSelection(i, true);
            _workspace.FocusListView();
        }

        public event EventHandler CanExecuteChanged;
    }
}