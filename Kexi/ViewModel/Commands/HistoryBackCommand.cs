using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using Kexi.Common;
using Kexi.Interfaces;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class HistoryBackCommand : IKexiCommand
    {
        [ImportingConstructor]
        public HistoryBackCommand(Workspace workspace)
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
            var historyBack = history?.History.Previous;
            if (historyBack != null)
                MoveToHistoryItem(historyBack);
        }

        public event EventHandler  CanExecuteChanged;
        private readonly Workspace _workspace;


        private HistoryItem _currentHistoryItem;

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
            var item     = _workspace.ActiveLister.View.ListView.ItemContainerGenerator.ContainerFromItem(selected) as ListViewItem;
            Keyboard.Focus(item);
        }
    }
}