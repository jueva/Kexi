using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using Kexi.Common;
using Kexi.Interfaces;

namespace Kexi.ViewModel.Commands
{
    [Export]
    [Export(typeof(IKexiCommand))]
    public class HistoryBackKeepFilterCommand : IKexiCommand
    {
        [ImportingConstructor]
        public HistoryBackKeepFilterCommand(Workspace workspace)
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
        private bool        _focusHandled;

        public async void MoveToHistoryItem(HistoryItem item)
        {
            _currentHistoryItem              =  item;
            _workspace.ActiveLister.GotItems += HistoryFocus;
            _focusHandled                    =  false;
            _workspace.ActiveLister.Path     =  item.FullPath;
            await _workspace.ActiveLister.Refresh(false);
            HistoryFocus(null, EventArgs.Empty);
        }

        private async void HistoryFocus(object sender, EventArgs ea)
        {
            _workspace.ActiveLister.GotItems -= HistoryFocus;
            if (_focusHandled)
                return;
            _focusHandled = true;

            _workspace.ActiveLister.GroupBy = _currentHistoryItem.GroupBy;
            var filter = _currentHistoryItem.Filter;
            _workspace.ActiveLister.Filter = filter;
            var f = await Task.Run(() => GetFilterPredicate(filter));
            _workspace.ActiveLister.ItemsView.Filter                            = f;
            ((ListCollectionView) _workspace.ActiveLister.ItemsView).CustomSort = new FilterSorting(filter);
            var selected = _workspace.ActiveLister.ItemsView.SourceCollection.Cast<IItem>().FirstOrDefault(fi => fi.Path == _currentHistoryItem.SelectedPath);
            _workspace.FocusItem(selected);
        }

        private static Predicate<object> GetFilterPredicate(string filter)
        {
            return item => new ItemFilter<IItem>(item as IItem, filter).Any();
        }
    }
}