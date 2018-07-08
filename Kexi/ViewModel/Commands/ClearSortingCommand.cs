using System;
using System.ComponentModel.Composition;
using Kexi.Common;
using Kexi.Interfaces;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class ClearSortingCommand : IKexiCommand
    {
        private readonly Workspace _workspace;

        [ImportingConstructor]
        public ClearSortingCommand(Workspace workspace)
        {
            _workspace = workspace;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _workspace.ActiveLister.ItemsView.SortDescriptions.Clear();
            new SortHandler(_workspace.ActiveLister).ClearSort();
            _workspace.FocusListView();
        }

        public event EventHandler CanExecuteChanged;
    }
}