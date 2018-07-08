using System;
using System.ComponentModel.Composition;
using System.Linq;
using Kexi.Common;
using Kexi.Interfaces;
using Kexi.ViewModel.Lister;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class HistoryForwardCommand : IKexiCommand
    {
        private readonly Workspace _workspace;

        [ImportingConstructor]
        public HistoryForwardCommand(Workspace workspace)
        {
            _workspace = workspace;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var history     = _workspace.ActiveLister as IHistorisationProvider;
            var historyForward = history?.History.Next;
            if (historyForward != null) MoveToHistoryItem(historyForward);
        }

        public void MoveToHistoryItem(HistoryItem item)
        {
            _currentHistoryItem              =  item;
            _workspace.ActiveLister.GotItems += HistoryFocus;
            _workspace.ActiveLister.Path     =  item.FullPath;
            _workspace.ActiveLister.Refresh();
        }

        private void HistoryFocus(object sender, EventArgs ea)
        {
            _workspace.ActiveLister.GotItems -= HistoryFocus;
            _workspace.ActiveLister.Filter   =  _currentHistoryItem.Filter;
            _workspace.ActiveLister.GroupBy  =  _currentHistoryItem.GroupBy;

            var selected = _workspace.ActiveLister.ItemsView.SourceCollection.Cast<IItem>().FirstOrDefault(f => f.Path == _currentHistoryItem.SelectedPath);
            _workspace.ActiveLister.View.FocusItem(selected);
        }


        private      HistoryItem  _currentHistoryItem;
        public event EventHandler CanExecuteChanged;
    }
}